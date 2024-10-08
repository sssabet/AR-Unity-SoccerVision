using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Used to configure the types of planes to detect.
    /// </summary>
    [Flags]
    public enum PlaneDetectionMode
    {
        /// <summary>
        /// No planes can be detected.
        /// </summary>
        None = 0,

        /// <summary>
        /// Horizontally aligned planes can be detected.
        /// </summary>
        Horizontal = 1 << 0,

        /// <summary>
        /// Vertically aligned planes can be detected.
        /// </summary>
        Vertical = 1 << 1
    }
}
