using System;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Defines the rendering modes for the Camera Background.
    /// </summary>
    /// <remarks>
    /// The platform provider is the source of truth for when the background should be rendered and
    /// can elect to only support certain render passes and not others.
    /// </remarks>
    public enum XRCameraBackgroundRenderingMode : byte
    {
        /// <summary>
        /// Camera background rendering is disabled or unsupported.
        /// </summary>
        None = 0,

        /// <summary>
        /// Render camera background prior to rendering opaques.
        /// </summary>
        BeforeOpaques,

        /// <summary>
        /// Render camera background after rendering opaques.
        /// </summary>
        AfterOpaques,
    }

    /// <summary>
    /// Defines the supported rendering modes for the Camera Background.
    /// </summary>
    /// <remarks>
    /// The platform provider is the source of truth for when the background should be rendered and
    /// can elect to only support certain render passes and not others.
    /// </remarks>
    [Flags]
    public enum XRSupportedCameraBackgroundRenderingMode : byte
    {
        /// <summary>
        /// Represents when rendering the camera background is unsupported.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Request the camera background be rendered prior to rendering opaques.
        /// </summary>
        BeforeOpaques = 0x1,

        /// <summary>
        /// Request the camera background be rendered after rendering opaques.
        /// </summary>
        AfterOpaques = 0x1 << 1,

        /// <summary>
        /// Allow the platform provider to determine the rendering mode for the camera background.
        /// </summary>
        Any = BeforeOpaques | AfterOpaques,
    }
}
