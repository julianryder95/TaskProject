using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ComponentOutlinePass : ScriptableRenderPass
{
    public ComponentOutlinePass()
    {
        renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public override void Execute(
        ScriptableRenderContext context,
        ref RenderingData renderingData)
    {
        // Outline rendering will be implemented next.
    }

    public void Dispose()
    {
    }
}