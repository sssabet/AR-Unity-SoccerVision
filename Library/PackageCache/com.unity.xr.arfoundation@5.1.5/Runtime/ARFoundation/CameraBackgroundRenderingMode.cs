using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Describes when the AR Camera Background should render.
    /// </summary>
    public enum CameraBackgroundRenderingMode : byte
    {
        /// <summary>
        /// Allows the platform to select its preferred render mode.
        /// </summary>
        Any,

        /// <summary>
        /// Perform background rendering prior to rendering opaque geometry in the scene.
        /// </summary>
        BeforeOpaques,

        /// <summary>
        /// Perform background rendering after opaques have been rendered.
        /// </summary>
        AfterOpaques
    }

    /// <summary>
    /// Provides conversion utilities between <see cref="XRCameraBackgroundRenderingMode"/> and <see cref="CameraBackgroundRenderingMode"/>.
    /// </summary>
    public static class CameraBackgroundRenderingModeUtilities
    {
        /// <summary>
        /// Converts a <see cref="CameraBackgroundRenderingMode"/> to an <see cref="XRCameraBackgroundRenderingMode"/>.
        /// </summary>
        /// <param name="mode">The <see cref="CameraBackgroundRenderingMode"/> to convert.</param>
        /// <returns>
        /// The converted <see cref="XRCameraBackgroundRenderingMode"/>.
        /// </returns>
        public static XRSupportedCameraBackgroundRenderingMode ToXRSupportedCameraBackgroundRenderingMode(this CameraBackgroundRenderingMode mode)
        {
            return mode switch
            {
                CameraBackgroundRenderingMode.Any => XRSupportedCameraBackgroundRenderingMode.Any,
                CameraBackgroundRenderingMode.BeforeOpaques => XRSupportedCameraBackgroundRenderingMode.BeforeOpaques,
                CameraBackgroundRenderingMode.AfterOpaques => XRSupportedCameraBackgroundRenderingMode.AfterOpaques,
                _ => XRSupportedCameraBackgroundRenderingMode.None
            };
        }

        /// <summary>
        /// Converts an <see cref="XRCameraBackgroundRenderingMode"/> to a <see cref="CameraBackgroundRenderingMode"/>.
        /// </summary>
        /// <param name="mode">The <see cref="XRCameraBackgroundRenderingMode"/> to convert.
        /// </param>
        /// <returns>
        /// The converted <see cref="CameraBackgroundRenderingMode"/>.
        /// </returns>
        public static CameraBackgroundRenderingMode ToBackgroundRenderingMode(this XRSupportedCameraBackgroundRenderingMode mode)
        {
            switch (mode)
            {
                case XRSupportedCameraBackgroundRenderingMode.BeforeOpaques:
                    return CameraBackgroundRenderingMode.BeforeOpaques;

                case XRSupportedCameraBackgroundRenderingMode.AfterOpaques:
                    return CameraBackgroundRenderingMode.AfterOpaques;

                case XRSupportedCameraBackgroundRenderingMode.Any:
                case XRSupportedCameraBackgroundRenderingMode.None:
                default:
                    return CameraBackgroundRenderingMode.Any;
            }
        }
    }
}
