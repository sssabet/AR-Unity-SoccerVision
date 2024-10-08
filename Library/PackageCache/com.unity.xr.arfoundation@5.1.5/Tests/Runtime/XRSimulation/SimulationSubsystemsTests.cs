using NUnit.Framework;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.Simulation.Tests
{
    [TestFixture]
    class SimulationSubsystemsTestFixture : SimulationTestSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SetupLoader();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            TearDownLoader();
        }

        void CheckSubsystemAvailable<TXRSubsystem, TSubsystem>()
            where TXRSubsystem : class, ISubsystem, new()
            where TSubsystem : class, ISubsystem, new()
        {
            GetSimulationSubsystem<TXRSubsystem, TSubsystem>();
        }

        void CheckSubsystemFunctional<TXRSubsystem, TSubsystem>()
            where TXRSubsystem : class, ISubsystem, new()
            where TSubsystem : class, ISubsystem, new()
        {
            var xrSubsystem = GetSimulationSubsystem<TXRSubsystem, TSubsystem>();

            // Initial state is not running
            Assert.IsFalse(xrSubsystem.running);

            // After start subsystem is running
            xrSubsystem.Start();
            Assert.IsTrue(xrSubsystem.running);

            // After stop subsystem is not running
            xrSubsystem.Stop();
            Assert.IsFalse(xrSubsystem.running);
        }

        static bool CheckARAvailabilitySync<TSubsystem>(TSubsystem subsystem)
            where TSubsystem : XRSessionSubsystem
        {
            var availabilityPromise = subsystem.GetAvailabilityAsync();

            while (availabilityPromise.MoveNext())
            {
            }

            var availability = availabilityPromise.result;
            return availability.IsSupported() && availability.IsInstalled();
        }

        [Test]
        [Order(1)]
        public void LoaderAvailable()
        {
            // XRM is setup properly
            Assert.IsNotNull(m_XrGeneralSettings, $"No instance of {nameof(XRGeneralSettings)} is available.");
            Assert.IsNotNull(m_XrGeneralSettings.Manager, $"No instance of {nameof(XRManagerSettings)} is available.");

            // Simulation loader exist
            Assert.IsNotNull(m_Loader, $"No active {nameof(SimulationLoader)} is available.");
        }

        #region SimulationSubsystemsAvailable

        [Test]
        [Order(2)]
        [Category("SimulationSubsystemsAvailable")]
        public void SessionSubsystemAvailable()
        {
            CheckSubsystemAvailable<XRSessionSubsystem, SimulationSessionSubsystem>();
        }

        [Test]
        [Order(2)]
        [Category("SimulationSubsystemsAvailable")]
        public void CameraSubsystemAvailable()
        {
            CheckSubsystemAvailable<XRCameraSubsystem, SimulationCameraSubsystem>();
        }

        [Test]
        [Order(2)]
        [Category("SimulationSubsystemsAvailable")]
        public void InputSubsystemAvailable()
        {
            CheckSubsystemAvailable<XRInputSubsystem, XRInputSubsystem>();
        }

        [Test]
        [Order(2)]
        [Category("SimulationSubsystemsAvailable")]
        public void PointCloudSubsystemAvailable()
        {
            CheckSubsystemAvailable<XRPointCloudSubsystem, SimulationPointCloudSubsystem>();
        }

        [Test]
        [Order(2)]
        [Category("SimulationSubsystemsAvailable")]
        public void PlaneSubsystemAvailable()
        {
            CheckSubsystemAvailable<XRPlaneSubsystem, SimulationPlaneSubsystem>();
        }

        [Test]
        [Order(2)]
        [Category("SimulationSubsystemsAvailable")]
        public void ImageTrackingSubsystemAvailable()
        {
            CheckSubsystemAvailable<XRImageTrackingSubsystem, SimulationImageTrackingSubsystem>();
        }

        [Test]
        [Order(2)]
        [Category("SimulationSubsystemsAvailable")]
        public void MeshSubsystemAvailable()
        {
            CheckSubsystemAvailable<XRMeshSubsystem, XRMeshSubsystem>();
        }

        [Test]
        [Order(2)]
        [Category("SimulationSubsystemsAvailable")]
        public void RaycastSubsystemAvailable()
        {
            CheckSubsystemAvailable<XRRaycastSubsystem, SimulationRaycastSubsystem>();
        }

        [Test]
        [Order(2)]
        [Category("SimulationSubsystemsAvailable")]
        public void AnchorSubsystemAvailable()
        {
            CheckSubsystemAvailable<XRAnchorSubsystem, SimulationAnchorSubsystem>();
        }

        #endregion

        [Test]
        [Order(3)]
        public void ARSessionSupported()
        {
            // Simulation loader exist
            Assert.IsNotNull(m_Loader, $"No active {nameof(SimulationLoader)} is available.");

            var xrSubsystem = m_Loader.GetLoadedSubsystem<XRSessionSubsystem>();

            // Subsystem exist
            Assert.IsInstanceOf<SimulationSessionSubsystem>(xrSubsystem, $"No active {nameof(SimulationSessionSubsystem)} is available.");

            var supported = CheckARAvailabilitySync(xrSubsystem);
            Assert.IsTrue(supported, "AR is not supported in Simulation.");
        }

        #region SimulationSubsystemsFunctional

        [Test]
        [Order(4)]
        [Category("SimulationSubsystemsFunctional")]
        public void SessionSubsystemFunctional()
        {
            AddXROrigin();
            CheckSubsystemFunctional<XRSessionSubsystem, SimulationSessionSubsystem>();
            RemoveXROrigin();
        }

        [Test]
        [Order(4)]
        [Category("SimulationSubsystemsFunctional")]
        public void CameraSubsystemFunctional()
        {
            AddXROrigin();
            CheckSubsystemFunctional<XRCameraSubsystem, SimulationCameraSubsystem>();
            RemoveXROrigin();
        }

        [Test]
        [Order(4)]
        [Category("SimulationSubsystemsFunctional")]
        public void InputSubsystemFunctional()
        {
            CheckSubsystemFunctional<XRInputSubsystem, XRInputSubsystem>();
        }

        [Test]
        [Order(4)]
        [Category("SimulationSubsystemsFunctional")]
        public void PointCloudSubsystemFunctional()
        {
            CheckSubsystemFunctional<XRPointCloudSubsystem, SimulationPointCloudSubsystem>();
        }

        [Test]
        [Order(4)]
        [Category("SimulationSubsystemsFunctional")]
        public void PlaneSubsystemFunctional()
        {
            CheckSubsystemFunctional<XRPlaneSubsystem, SimulationPlaneSubsystem>();
        }

        [Test]
        [Order(4)]
        [Category("SimulationSubsystemsFunctional")]
        public void ImageTrackingSubsystemFunctionalWithLibrary()
        {
            // Image tracking subsystem needs a image library to run
            var subsystem = m_Loader.GetLoadedSubsystem<XRImageTrackingSubsystem>();
            var imageLibrary = ScriptableObject.CreateInstance<XRReferenceImageLibrary>();
            subsystem.imageLibrary = subsystem.CreateRuntimeLibrary(imageLibrary);

            CheckSubsystemFunctional<XRImageTrackingSubsystem, SimulationImageTrackingSubsystem>();
        }

        [Test]
        [Order(4)]
        [Category("SimulationSubsystemsFunctional")]
        public void MeshSubsystemFunctional()
        {
            CheckSubsystemFunctional<XRMeshSubsystem, XRMeshSubsystem>();
        }

        [Test]
        [Order(4)]
        [Category("SimulationSubsystemsFunctional")]
        public void RaycastSubsystemFunctional()
        {
            CheckSubsystemFunctional<XRRaycastSubsystem, SimulationRaycastSubsystem>();
        }

        [Test]
        [Order(4)]
        [Category("SimulationSubsystemsFunctional")]
        public void AnchorSubsystemFunctional()
        {
            AddXROrigin();
            CheckSubsystemFunctional<XRAnchorSubsystem, SimulationAnchorSubsystem>();
            RemoveXROrigin();
        }

        #endregion
    }
}
