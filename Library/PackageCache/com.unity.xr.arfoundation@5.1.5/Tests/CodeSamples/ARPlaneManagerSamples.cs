namespace UnityEngine.XR.ARFoundation.Tests
{
    class ARPlaneManagerSamples
    {
        #region DescriptorChecks
        void CheckForOptionalFeatureSupport()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindObjectOfType<ARPlaneManager>();

            // Use manager.descriptor to determine which optional features
            // are supported on the device. For example:

            if (manager.descriptor.supportsClassification)
            {
                // Classification is supported.
            }
        }
        #endregion

        #region PlanesChanged
        public void OnPlanesChanged(ARPlanesChangedEventArgs changes)
        {
            foreach (var plane in changes.added)
            {
                // handle added planes
            }

            foreach (var plane in changes.updated)
            {
                // handle updated planes
            }

            foreach (var plane in changes.removed)
            {
                // handle removed planes
            }
        }
        #endregion

        #region PlanesSubscribe
        void SubscribeToPlanesChanged()
        {
            // This is inefficient. You should re-use a saved reference instead.
            var manager = Object.FindObjectOfType<ARPlaneManager>();

            manager.planesChanged += OnPlanesChanged;
        }
        #endregion
    }
}
