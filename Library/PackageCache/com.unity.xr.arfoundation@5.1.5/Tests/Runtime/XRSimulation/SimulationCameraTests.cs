using NUnit.Framework;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARFoundation.InternalUtils;

namespace UnityEngine.XR.Simulation.Tests
{
    [TestFixture]
    class SimulationCameraTestFixture : SimulationSessionTestSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SetupSession();
            SetupInput();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            TearDownInput();
            TearDownSession();
        }

        [Test]
        [Order(1)]
        public void PoseProviderAvailable()
        {
            var xrOrigin = FindObjectsUtility.FindAnyObjectByType<XROrigin>();
            Assert.IsNotNull(xrOrigin);

            var xrCamera = xrOrigin.Camera;
            Assert.IsNotNull(xrCamera);

            var poseProvider = FindObjectsUtility.FindAnyObjectByType<SimulationCamera>();
            Assert.IsNotNull(poseProvider, $"No active {nameof(SimulationCamera)} is available.");
        }
    }
}
