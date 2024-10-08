using System;

namespace UnityEngine.XR.Simulation
{
    [Serializable]
    class AnchorDiscoveryParams
    {
        [SerializeField]
        [Tooltip("Minimum time in seconds between anchor discovery updates.")]
        [Range(SimulationConstants.sixtyFps, 2.0f)]
        float m_MinTimeUntilUpdate = 0.2f;

        /// <summary>
        /// Minimum time in seconds between anchor discovery updates.
        /// </summary>
        public float minTimeUntilUpdate => m_MinTimeUntilUpdate;
    }
}
