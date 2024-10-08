using NUnit.Framework;
using Unity.Collections;
using System.Collections.Generic;

namespace UnityEngine.XR.ARSubsystems.Tests
{
    internal class XRPointCloudSubsystemImpl : UnityEngine.XR.ARSubsystems.XRPointCloudSubsystem
    {
        public class TestProvider : Provider
        {
            public TestProvider() { }
            public override TrackableChanges<XRPointCloud> GetChanges(XRPointCloud defaultPointCloud, Allocator allocator) => default;
            public override XRPointCloudData GetPointCloudData(TrackableId trackableId, Allocator allocator) => default;
        }
    }

    [TestFixture]
    public class XRPointCloudSubsystemTestFixture
    {
        const string k_TestSubsystemId = "Test-PointCloud";
        
        [OneTimeSetUp]
        public void RegisterTestDescriptor()
        {
            UnityEngine.XR.ARSubsystems.XRPointCloudSubsystemDescriptor.RegisterDescriptor(new UnityEngine.XR.ARSubsystems.XRPointCloudSubsystemDescriptor.Cinfo
            {
                id = k_TestSubsystemId,
                providerType = typeof(XRPointCloudSubsystemImpl.TestProvider),
                subsystemTypeOverride = typeof(XRPointCloudSubsystemImpl)
            });
        }

        static List<UnityEngine.XR.ARSubsystems.XRPointCloudSubsystemDescriptor> s_Descs = new List<UnityEngine.XR.ARSubsystems.XRPointCloudSubsystemDescriptor>();
        static UnityEngine.XR.ARSubsystems.XRPointCloudSubsystem CreateTestPointCloudSubsystem()
        {
            SubsystemManager.GetSubsystemDescriptors(s_Descs);
            foreach (var desc in s_Descs)
            {
                if (desc.id == k_TestSubsystemId)
                    return desc.Create();
            }

            return s_Descs[0].Create();
        }

        [Test]
        public void RunningStateTests()
        {
            UnityEngine.XR.ARSubsystems.XRPointCloudSubsystem subsystem = CreateTestPointCloudSubsystem();

            // Initial state is not running
            Assert.That(subsystem.running == false);

            // After start subsystem is running
            subsystem.Start();
            Assert.That(subsystem.running == true);

            // After start subsystem is running
            subsystem.Stop();
            Assert.That(subsystem.running == false);
        }
    }
}
