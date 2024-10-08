namespace UnityEngine.XR.Simulation
{
    class SimulationXRayManager
    {
        // Default values for the XRay shader if no region is set
        const float k_DefaultFloorHeight = 0.0f;
        const float k_DefaultCeilingHeight = 2.5f;
        const float k_DefaultThickness = 0.5f;
        const float k_DefaultRoomWidth = 3.0f;
        const float k_DefaultScale = 1.0f;
        const string k_XRayEnabledKeyword = "SIMULATION_XRAY_ENABLED";
        const string k_FlipXRayDirectionKeyword = "SIMULATION_XRAY_FLIP_DEPTH";

        // Shader IDs for the same properties
        static readonly int k_RoomCenterShaderID = Shader.PropertyToID("_SimulationRoomCenter");
        static readonly int k_FloorHeightShaderID = Shader.PropertyToID("_SimulationFloorHeight");
        static readonly int k_CeilingHeightShaderID = Shader.PropertyToID("_SimulationCeilingHeight");
        static readonly int k_ClipOffsetShaderID = Shader.PropertyToID("_SimulationRoomClipOffset");
        static readonly int k_XRayScaleID = Shader.PropertyToID("_SimulationXRayScale");

        XRayRegion m_LastXRay;

        Vector3 m_XRayCenter = Vector3.zero;
        float m_XRayFloorHeight = k_DefaultFloorHeight;
        float m_XRayCeilingHeight = k_DefaultCeilingHeight;
        float m_XRayThickness = k_DefaultThickness;
        float m_XRayScale = k_DefaultScale;

        internal void UpdateXRayShader(bool useXRay, XRayRegion activeXRay)
        {
            if (!useXRay || !XRSimulationRuntimeSettings.Instance.useXRay)
            {
                Shader.DisableKeyword(k_XRayEnabledKeyword);
                return;
            }

            // If the selected XRay data has changed, update the active X-Ray values for physics and shader
            if (m_LastXRay != activeXRay)
            {
                m_LastXRay = activeXRay;

                if (activeXRay != null)
                {
                    var transform = activeXRay.transform;
                    // Scale can vary between views and prefabs, so we grab 'ground truth' from the active XRay Region
                    m_XRayScale = transform.lossyScale.x;

                    m_XRayCenter = transform.position;
                    m_XRayFloorHeight = activeXRay.floorHeight;
                    m_XRayCeilingHeight = activeXRay.ceilingHeight;
                    m_XRayThickness = activeXRay.clipOffset;
                }
                else
                {
                    m_XRayScale = k_DefaultScale;
                    m_XRayCenter = Vector3.zero;
                    m_XRayFloorHeight = k_DefaultFloorHeight;
                    m_XRayCeilingHeight = k_DefaultCeilingHeight;
                    m_XRayThickness = k_DefaultThickness;
                }
            }

            Shader.SetGlobalVector(k_RoomCenterShaderID, m_XRayCenter);
            Shader.SetGlobalFloat(k_FloorHeightShaderID, m_XRayFloorHeight);
            Shader.SetGlobalFloat(k_CeilingHeightShaderID, m_XRayCeilingHeight);
            Shader.SetGlobalFloat(k_ClipOffsetShaderID, m_XRayThickness);
            Shader.SetGlobalFloat(k_XRayScaleID, m_XRayScale);
            Shader.EnableKeyword(k_XRayEnabledKeyword);
            SetXRayDirectionKeyword();
        }

        static void SetXRayDirectionKeyword()
        {
            if (XRSimulationRuntimeSettings.Instance.flipXRayDirection)
                Shader.EnableKeyword(k_FlipXRayDirectionKeyword);
            else
                Shader.DisableKeyword(k_FlipXRayDirectionKeyword);
        }
    }
}
