using System;
using UnityEngine;
using UnityEngine.XR.Management;

namespace UnityEditor.XR.Simulation
{
    /// <summary>
    /// Build settings for XR Simulation.
    /// </summary>
    [Serializable]
    [XRConfigurationData("XR Simulation", k_SettingsKey)]
    public class XRSimulationSettings : ScriptableObject
    {
        /// <summary>
        /// Configuration key for the settings.
        /// </summary>
        public const string k_SettingsKey = "com.unity.xr.arfoundation.simulation_settings";

        /// <summary>
        /// Get the instance of the <see cref="XRSimulationSettings"/>.
        /// </summary>
        public static XRSimulationSettings currentSettings => EditorBuildSettings.TryGetConfigObject(k_SettingsKey, out XRSimulationSettings settings) ? settings : null;

        void Awake()
        {
            EditorApplication.delayCall += XREnvironmentViewUtilities.ToggleXREnvironmentOverlays;
        }
    }
}
