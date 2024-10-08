using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    class AROcclusionSamples
    {
        #region DescriptorChecks
        void CheckForOptionalFeatureSupport()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindObjectOfType<AROcclusionManager>();

            // Use manager.descriptor to determine which optional features
            // are supported on the device. For example:

            if (manager.descriptor.environmentDepthImageSupported == Supported.Supported)
            {
                // Environment depth image is supported.
            }
        }
        #endregion
    }
}