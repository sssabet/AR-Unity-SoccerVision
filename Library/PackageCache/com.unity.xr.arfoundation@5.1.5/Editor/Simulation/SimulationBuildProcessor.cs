using UnityEditor.XR.Management;

namespace UnityEditor.XR.Simulation
{
    /// <summary>
    /// Build processor for XR Simulation.
    /// </summary>
    public class SimulationBuildProcessor : XRBuildHelper<XRSimulationSettings>
    {
        /// <summary>
        /// Settings key for <see cref="XRSimulationSettings"/>.
        /// </summary>
        /// <returns>A string specifying the key to be used to set/get settings in EditorBuildSettings.</returns>
        public override string BuildSettingsKey => XRSimulationSettings.k_SettingsKey;
    }
}
