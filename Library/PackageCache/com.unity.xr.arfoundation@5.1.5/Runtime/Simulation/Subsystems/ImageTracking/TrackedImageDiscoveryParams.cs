using System;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// User-accessible settings for the <see cref="SimulationImageTrackingSubsystem"/>.
    /// </summary>
    [Serializable]
    class TrackedImageDiscoveryParams
    {
        [SerializeField]
        [Range(SimulationConstants.oneHundredTwentyFps, 1)]
        [Tooltip("Minimum time in seconds that must elapse between image tracking updates.")]
        float m_TrackingUpdateInterval = 0.09f;

        public float trackingUpdateInterval => m_TrackingUpdateInterval;
    }
}
