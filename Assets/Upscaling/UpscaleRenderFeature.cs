using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UpscaleRenderFeature : ScriptableRendererFeature
{
    public Material material;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.renderPassEvent = RenderPassEvent.AfterRendering;
        renderer.EnqueuePass(pass);
    }

    UpscaleRenderPass pass;
    public override void Create() => pass = new UpscaleRenderPass(this);
}