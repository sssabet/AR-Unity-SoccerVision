using System;
using System.ComponentModel;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents the light estimation mode.
    /// </summary>
    [Obsolete("This enum is no longer used. Its functionality was replaced by the Feature flags enum. (2023-02-02)")]
    public enum LightEstimationMode
    {
        /// <summary>
        /// Light estimation is disabled.
        /// </summary>
        [Description("Disabled")]
        Disabled = 0,

        /// <summary>
        /// Ambient lighting will be estimated as a single-value intensity.
        /// </summary>
        [Description("AmbientIntensity")]
        AmbientIntensity = 1,

        /// <summary>
        /// Scene lighting will be estimated using Environmental HDR.
        /// </summary>
        [Description("EnvironmentalHDR")]
        EnvironmentalHDR = 2,
    }
}
