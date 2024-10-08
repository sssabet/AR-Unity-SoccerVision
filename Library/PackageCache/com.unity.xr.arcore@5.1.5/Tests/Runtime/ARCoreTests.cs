using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.SubsystemsImplementation;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.XR.ARSubsystems;
#endif

namespace UnityEngine.XR.ARCore.Tests
{
    [TestFixture]
    public class ARCoreTestFixture
    {
        [Test]
        public void SessionSubsystemRegistered()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Assert.That(SubsystemDescriptorRegistered<XRSessionSubsystemDescriptor, XRSessionSubsystem, XRSessionSubsystem.Provider>(
                "ARCore-Session"));
#endif
        }

        [Test]
        public void CameraSubsystemRegistered()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Assert.That(SubsystemDescriptorRegistered<XRCameraSubsystemDescriptor, XRCameraSubsystem, XRCameraSubsystem.Provider>(
                "ARCore-Camera"));
#endif
        }

        [Test]
        public void PlaneSubsystemRegistered()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Assert.That(SubsystemDescriptorRegistered<XRPlaneSubsystemDescriptor, XRPlaneSubsystem, XRPlaneSubsystem.Provider>(
                "ARCore-Plane"));
#endif
        }

        [Test]
        public void PointCloudSubsystemRegistered()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Assert.That(SubsystemDescriptorRegistered<XRPointCloudSubsystemDescriptor, XRPointCloudSubsystem, XRPointCloudSubsystem.Provider>(
                "ARCore-PointCloud"));
#endif
        }

        [Test]
        public void AnchorSubsystemRegistered()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Assert.That(SubsystemDescriptorRegistered<XRAnchorSubsystemDescriptor, XRAnchorSubsystem, XRAnchorSubsystem.Provider>(
                "ARCore-Anchor"));
#endif
        }

        [Test]
        public void RaycastSubsystemRegistered()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Assert.That(SubsystemDescriptorRegistered<XRRaycastSubsystemDescriptor, XRRaycastSubsystem, XRRaycastSubsystem.Provider>(
                "ARCore-Raycast"));
#endif
        }

        static bool SubsystemDescriptorRegistered<TSubsystemDescriptor, TSubsystem, TProvider>(string id)
            where TSubsystemDescriptor : SubsystemDescriptorWithProvider<TSubsystem, TProvider>
            where TSubsystem : SubsystemWithProvider, new()
            where TProvider : SubsystemProvider<TSubsystem>
        {
            List<TSubsystemDescriptor> descriptors = new();
            SubsystemManager.GetSubsystemDescriptors(descriptors);

            foreach (TSubsystemDescriptor descriptor in descriptors)
            {
                if (descriptor.id == id)
                    return true;
            }

            return false;
        }
    }
}
