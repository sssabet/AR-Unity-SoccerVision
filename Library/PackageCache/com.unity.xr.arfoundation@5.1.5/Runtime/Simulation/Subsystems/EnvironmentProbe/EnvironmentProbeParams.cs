using System;

namespace UnityEngine.XR.Simulation
{
    [Serializable]
    class EnvironmentProbeParams
    {
        [SerializeField]
        [Tooltip("Minimum time in seconds between environment probe discovery updates.")]
        [Range(SimulationConstants.sixtyFps, 2.0f)]
        float m_MinUpdateTime = 0.2f;

        [SerializeField]
        [Tooltip("Maximum distance in meters from the camera at which automatically placed environment probes can be discovered.")]
        [Range(1.0f, 20.0f)]
        float m_MaxDiscoveryDistance = 3.0f;
        
        [SerializeField]
        [Tooltip("Time in seconds after an environment probe is discovered before it is added as a trackable.")]
        [Range(0.0f, 5.0f)]
        float m_DiscoveryDelayTime = 1.0f;

        [SerializeField]
        [Tooltip("Width and height in pixels of each face of each environment probe's Cubemap.")]
        [Range(8, 256)]
        int m_CubemapFaceSize = 16;

        /// <summary>
        /// Minimum time in seconds between environment probe discovery updates.
        /// </summary>
        public float minUpdateTime => m_MinUpdateTime;

        /// <summary>
        /// Maximum in distance in meters from the camera at which automatically placed environment probes can be discovered.
        /// </summary>
        public float maxDiscoveryDistance => m_MaxDiscoveryDistance;

        /// <summary>
        /// Time in seconds after an environment probe is discovered before it is added as a trackable.
        /// </summary>
        public float discoveryDelayTime => m_DiscoveryDelayTime;

        /// <summary>
        /// Width and height in pixels of each face of each environment probe's Cubemap.
        /// </summary>
        public int cubemapFaceSize => m_CubemapFaceSize;
    }
}
