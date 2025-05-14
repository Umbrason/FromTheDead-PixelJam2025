float rand(int index)
{
    return frac(sin(index * 62.4196345) * 7325.5754235);
}

float Perlin1D(float position)
{
    position += .62346246;
    float t = frac(position);
    float a = rand(floor(position));
    float b = rand(floor(position) + 1);
    return smoothstep(a, b, t);
}

float rand2D(float2 pos)
{
    return frac(sin(dot(pos.xy, float2(12.9898, 78.233))) * 43758.5453123);
}

float Perlin2D(float2 position)
{
    float2 index = floor(position);
    float2 t = frac(position);

    float a = rand2D(index + float2(0, 0));
    float b = rand2D(index + float2(1, 0));
    float c = rand2D(index + float2(0, 1));
    float d = rand2D(index + float2(1, 1));
    
    float2 u = t * t * (3.0 - 2.0 * t);
    
    return lerp(a, b, u.x) +
    (c - a)* u.y * (1.0 - u.x) +
    (d - b) * u.x * u.y;    
}



