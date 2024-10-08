using System;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Simulation
{
    [ScriptableSettingsPath(SimulationConstants.userSettingsPath)]
    class XRSimulationPreferences : ScriptableSettings<XRSimulationPreferences>
    {
        [SerializeField, Tooltip("Simulation environment prefab")]
        GameObject m_EnvironmentPrefab;

        [SerializeField, Tooltip("Fallback simulation environment prefab")]
        [HideInInspector]
        GameObject m_FallbackEnvironmentPrefab;

        [SerializeField, Tooltip("Navigate in Play Mode with WASD keys")]
        bool m_EnableNavigation = true;

        /// <summary>
        /// The prefab for the simulation environment.
        /// </summary>
        public GameObject environmentPrefab
        {
            get => m_EnvironmentPrefab;
            set => m_EnvironmentPrefab = value;
        }

        internal GameObject fallbackEnvironmentPrefab => m_FallbackEnvironmentPrefab;

        /// <summary>
        /// The current simulation environment prefab, or a fallback environment prefab if no environment is set.
        /// </summary>
        public GameObject activeEnvironmentPrefab => m_EnvironmentPrefab != null ? m_EnvironmentPrefab : m_FallbackEnvironmentPrefab;

        public bool enableNavigation => m_EnableNavigation;
    }
}
