using System;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Runtime settings for XR Simulation.
    /// </summary>
    [Serializable]
    [ScriptableSettingsPath(SimulationConstants.runtimeSettingsPath)]
    class XRSimulationRuntimeSettings : ScriptableSettings<XRSimulationRuntimeSettings>
    {
        [SerializeField, Tooltip("Layer used for the simulation environment")]
        int m_EnvironmentLayer = 30;

        [SerializeField]
        EnvironmentScanParams m_EnvironmentScanParams;

        [SerializeField]
        PlaneFindingParams m_PlaneFindingParams = new();

        [SerializeField]
        TrackedImageDiscoveryParams m_TrackedImageDiscoveryParams = new();

        [SerializeField]
        EnvironmentProbeParams m_EnvironmentProbeDiscoveryParams = new();

        [SerializeField] 
        AnchorDiscoveryParams m_AnchorDiscoveryParams = new();

        [Header("X-Ray Options")]
        [SerializeField, Tooltip("Use x-ray visuals on the simulation environments.")]
        bool m_UseXRay = true;

        [SerializeField, Tooltip("Flip the Depth direction that the X-Ray clips the model in URP.")]
        bool m_FlipXRayDirection;

        /// <summary>
        /// Layer used for the simulation environment
        /// </summary>
        public int environmentLayer => m_EnvironmentLayer;

        public EnvironmentScanParams environmentScanParams => m_EnvironmentScanParams;

        public TrackedImageDiscoveryParams trackedImageDiscoveryParams => m_TrackedImageDiscoveryParams;

        public PlaneFindingParams planeFindingParams => m_PlaneFindingParams;

        public EnvironmentProbeParams environmentProbeDiscoveryParams => m_EnvironmentProbeDiscoveryParams;

        public AnchorDiscoveryParams anchorDiscoveryParams => m_AnchorDiscoveryParams;

        /// <summary>
        /// Use x-ray visuals on the simulation environments.
        /// </summary>
        public bool useXRay => m_UseXRay;

        /// <summary>
        /// Flip the Depth direction that the X-Ray clips the model in URP.
        /// </summary>
        public bool flipXRayDirection => m_FlipXRayDirection;
    }
}
