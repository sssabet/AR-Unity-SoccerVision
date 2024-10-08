namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARCameraManagerSamples
    {
        #region DescriptorChecks
        void CheckForOptionalFeatureSupport()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindObjectOfType<ARCameraManager>();

            // Use manager.descriptor to determine which optional features
            // are supported on the device. For example:

            if (manager.descriptor.supportsCameraImage)
            {
                // Camera image is supported.
            }
        }
        #endregion
    }
}
