#if MODULE_URP_ENABLED
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Simulation;

class SimulationCameraTextureReadbackPass : ScriptableRenderPass
{
    CameraTextureProvider m_Provider;

    public SimulationCameraTextureReadbackPass(CameraTextureProvider cameraTextureProvider)
    {
        m_Provider = cameraTextureProvider;
        renderPassEvent = RenderPassEvent.AfterRendering;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        using var commandBuffer = CommandBufferPool.Get("SimulationCameraTextureReadbackPass");
        if (m_Provider.TryConfigureReadbackCommandBuffer(commandBuffer))
        {
            context.ExecuteCommandBuffer(commandBuffer);
        }
    }
}
#endif // end MODULE_URP_ENABLED