Shader "Unlit/PlayerBodyShader"
{
    Properties
    {
        _pixelsPerUnit ("PPU", Integer) = 16
        _AOThreshold ("Ambient Occlusion", Float) = 0.0
        _ShadingThreshold ("Shading Threshold", Float) = 0.0
        _ShadingDirection ("Shading Direction", Vector) = (-1,1,0,0)
        _HighlightThreshold ("Highlight Threshold", Float) = 0.0
        _HighlightDirection ("Highlight Direction", Vector) = (-1,1,0,0)
        _HighlightColor ("Highlight Color", Color) = (1,1,1,1)
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _DarkColor("Dark Color", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _SizeRandomness("Size Randomness", Float) = .15
        _NoiseDistortion("Noise Distortion", Float) = .15
        _NoiseDistortionFrequency("Noise Frequency", Float) = .15
        _Flatness("Flatness", Float) = .15
        _blobSize("Blob Size", Float) = 1
        _k("Smoothing", Float) = .45
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0
            #include "UnityCG.cginc"
            #include "Assets/ShaderLib/Simplex4D.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _velocities;
            sampler2D _points;
            uint _count;
            int _pixelsPerUnit;
            float _AOThreshold;
            float3 _HighlightColor;
            float3 _BaseColor;
            float3 _DarkColor;
            float3 _OutlineColor;
            float _k;
            float _NoiseDistortionFrequency;
            float _NoiseDistortion;
            float _SizeRandomness;
            float _Flatness;
            float _blobSize;


            float _ShadingThreshold;
            float3 _ShadingDirection;

            float _HighlightThreshold;
            float3 _HighlightDirection;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float smin( float a, float b, float k)
            {
                k *= 1.0/(1.0-sqrt(0.5));
                return max(k,min(a,b)) -
                   length(max(k-float2(a,b),0.0));
            }

            float rand(float input)
            {
                return frac(sin(input * 78.233 * 2.0) * 43758.5453);
            }
            float smoothstep(float x)
            {
                x = saturate(x);
                x = 2 * x * x * x + 3 * x * x;
                return x;
            }

            float2 sampleOffset(float2 pixelPos)
            {
                float x = snoise(float4(pixelPos * .06423 * _NoiseDistortionFrequency, 74.745, 12.75)) * 2 - 1;
                float y = snoise(float4(12.742, -12.157, pixelPos * .06423 * _NoiseDistortionFrequency)) * 2 - 1;
                return float2(x, y) * .4 * _NoiseDistortion;
            }

            float heightAt(int2 pixelPos)
            {
                float2 sdfSamplePos = (pixelPos + float2(.5, .5)) / _pixelsPerUnit;
                sdfSamplePos += sampleOffset(sdfSamplePos);
                float2 closest = tex2D(_points, float2(.5 / _count, .5));
                float sdf = 0;
                for(uint i = 0; i < _count; i++)
                {
                    float2 samplePos = float2((i + .5) / _count, .5);
                    float2 pos = tex2D(_points, samplePos);

                    float2 vel = tex2D(_velocities, samplePos);
                    float2 velPerpendicular = float2(vel.y, -vel.x);
                    float velMag = length(vel);
                    float2 delta = sdfSamplePos - pos;
                    delta /= _blobSize;
                    delta *= 1 + sin(_Time.y + rand(i + 784.42) * 6.828) * .15;
                    /* delta /= 1 + saturate(-delta.y) * .2; */
                    delta *= ((rand(i) - .5) * _SizeRandomness) + 1;

                    if(velMag > .1) delta *= float2(1,1) + abs(velPerpendicular / velMag * saturate(velMag / 10.0) * .5) - abs(vel / velMag * saturate(velMag / 10.0) * .5);
                    float sqrDist = dot(delta, delta);
                    float dist = sqrt(sqrDist);
                    sdf = (smin(sdf, dist, _k) * (i > 0)) + ((i == 0) * dist);
                }
                sdf *= 1 + snoise(float4(pixelPos * .06423, 0, 0)) * .1f;
                sdf += max(0, -pixelPos.y - 5) * _Flatness;
                sdf = 1 - saturate(sdf);
                return sqrt(sdf);
            }

            fixed4 frag (v2f input) : SV_Target
            {
                int2 pixelPos = floor(input.uv * _pixelsPerUnit);
                float height = heightAt(pixelPos);
                if(height <= 0) discard;
                int outlineCounter = 0;
                float ambientOcclusionCounter = 0;
                float3 tanX = float3(0,0,0);
                float3 tanY = float3(0,0,0);
                for(int dx = -2; dx <= 2; dx++)
                    for(int dy = -2; dy <= 2; dy++)
                    {
                        int d = dx * dx + dy * dy;
                        if(d == 0) continue;
                        if(d >= 16) continue;
                        int2 delta = int2(dx, dy);
                        float otherHeight = heightAt(pixelPos + delta);
                        if(d <= 1) outlineCounter += otherHeight > 0; //was <= 5

                        if(otherHeight == 0) otherHeight = height;
                        float heightDiff = otherHeight - height; //how much this pixel is lower than the other
                        if(d == 1)
                        {
                            if(abs(dx) > 0) tanX += float3(delta, heightDiff * 5) * dx;
                            if(abs(dy) > 0) tanY += float3(delta, heightDiff * 5) * dy;
                        }
                        if(d <= 4) ambientOcclusionCounter += heightDiff / sqrt(d);
                    }
                float3 normal = normalize(cross(tanX, tanY));
                /* return float4(normal, 1); */
                float3 col = _BaseColor;

                float darkMask = dot(normal, normalize(_ShadingDirection)) > _ShadingThreshold;
                col = lerp(col, _DarkColor, darkMask);

                float highlightMask = dot(normal, normalize(_HighlightDirection)) > _HighlightThreshold;
                col = lerp(col, _HighlightColor, highlightMask);

                float outlineMask = (ambientOcclusionCounter > _AOThreshold) || outlineCounter < 4;//20;
                col = lerp(col, _OutlineColor, outlineMask);
                return float4(col, 1);
            }
            ENDCG
        }
    }
}
