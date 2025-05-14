using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class UpscaleRenderPass : ScriptableRenderPass
{
    public static RenderTexture TargetTexture;
    private readonly UpscaleRenderFeature rendererAsset;
    public UpscaleRenderPass(UpscaleRenderFeature rendererAsset)
    {
        this.rendererAsset = rendererAsset;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (!TargetTexture)
            return;

        var resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer)
            return;
        var srcCamColor = resourceData.activeColorTexture;

        var pass = renderGraph.AddRasterRenderPass<object>("Upscale", out var passData);
        pass.SetRenderAttachment(srcCamColor, 0);
        pass.SetRenderFunc<object>((data, renderGraphContext) =>
        {
            var to = new Vector2(Screen.width, Screen.height);
            var from = new Vector2(TargetTexture.width, TargetTexture.height);
            var texelSize = new Vector4(1f / from.x, 1f / from.y, from.x, from.y);
            rendererAsset.material.SetTexture("_EnvironmentColor", TargetTexture);
            rendererAsset.material.SetVector("_EnvironmentColor_TexelSize", texelSize);
            rendererAsset.material.SetVector("_pxPerTex", to / from);
            renderGraphContext.cmd.DrawProcedural(Matrix4x4.identity, rendererAsset.material, 0, MeshTopology.Triangles, 3);
        });
        pass.Dispose();
    }
}

