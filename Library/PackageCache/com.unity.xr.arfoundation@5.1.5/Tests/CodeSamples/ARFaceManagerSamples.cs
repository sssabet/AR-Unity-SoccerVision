namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARFaceManagerSamples
    {
        #region DescriptorChecks
        void CheckForOptionalFeatureSupport()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindObjectOfType<ARFaceManager>();

            // Use manager.descriptor to determine which optional features
            // are supported on the device. For example:

            if (manager.descriptor.supportsFacePose)
            {
                // Classification is supported.
            }
        }
        #endregion

        #region FacesChanged
        public void OnFacesChanged(ARFacesChangedEventArgs changes)
        {
            foreach (var face in changes.added)
            {
                // handle added faces
            }

            foreach (var face in changes.updated)
            {
                // handle updated faces
            }

            foreach (var face in changes.removed)
            {
                // handle removed faces
            }
        }
        #endregion

        #region FacesSubscribe
        void SubscribeToFacesChanged()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindObjectOfType<ARFaceManager>();

            manager.facesChanged += OnFacesChanged;
        }
        #endregion
    }
}
