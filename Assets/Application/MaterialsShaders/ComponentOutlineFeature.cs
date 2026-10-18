using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ComponentOutlineFeature : ScriptableRendererFeature
{
    private ComponentOutlinePass outlinePass;

    public override void Create()
    {
        outlinePass = new ComponentOutlinePass();
    }

    public override void AddRenderPasses(
        ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        renderer.EnqueuePass(outlinePass);
    }

    protected override void Dispose(bool disposing)
    {
        outlinePass?.Dispose();
    }
}