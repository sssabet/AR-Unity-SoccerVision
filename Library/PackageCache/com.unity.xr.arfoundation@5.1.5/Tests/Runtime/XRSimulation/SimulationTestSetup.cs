using NUnit.Framework;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Management;
using UnityEngine.InputSystem;
using UnityEditor.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.TestTooling;

namespace UnityEngine.XR.Simulation.Tests
{
    abstract class SimulationTestSetup : LoaderTestSetup<SimulationLoader>
    {
        static bool noXrmOrActiveSimulationLoader => XRGeneralSettings.Instance == null || XRGeneralSettings.Instance.Manager == null ||
                                    XRGeneralSettings.Instance.Manager.activeLoader == null || XRGeneralSettings.Instance.Manager.activeLoader is not SimulationLoader;

        /// <summary>
        /// Set up the <c>SimulationLoader</c> for runtime test.
        /// </summary>
        protected void SetupLoader()
        {
            if (noXrmOrActiveSimulationLoader)
            {
                base.SetupTest();
                InitializeAndStart();
            }
            else
            {
                m_XrGeneralSettings = XRGeneralSettings.Instance;
                m_Loader = m_XrGeneralSettings.Manager.activeLoader as SimulationLoader;
            }
        }

        /// <summary>
        /// Tear down the <c>SimulationLoader</c> after the runtime tests are complete.
        /// </summary>
        protected void TearDownLoader()
        {
            if (noXrmOrActiveSimulationLoader)
            {
                StopAndShutdown();
                base.TearDownTest();
            }
        }

        protected void AddXROrigin()
        {
            XROriginCreateUtil.CreateXROriginWithParent(null);
            CreateInputDevices();
        }

        protected void RemoveXROrigin()
        {
            var xrOrigin = FindObjectsUtility.FindAnyObjectByType<XROrigin>();

            if (xrOrigin != null)
                Object.Destroy(xrOrigin.gameObject);
        }

        void CreateInputDevices()
        {
            var keyboard = InputSystem.InputSystem.AddDevice<Keyboard>();
            keyboard.MakeCurrent();

            var mouse = InputSystem.InputSystem.AddDevice<Mouse>();
            mouse.MakeCurrent();
        }

        protected TXRSubsystem GetSubsystem<TXRSubsystem>()
            where TXRSubsystem : class, ISubsystem, new()
        {
            // Simulation loader exist
            Assert.IsNotNull(m_Loader, $"No active {nameof(SimulationLoader)} is available.");

            return m_Loader.GetLoadedSubsystem<TXRSubsystem>();
        }

        protected TSubsystem GetSimulationSubsystem<TXRSubsystem, TSubsystem>()
            where TXRSubsystem : class, ISubsystem, new()
            where TSubsystem : class, ISubsystem, new()
        {
            var xrSubsystem = GetSubsystem<TXRSubsystem>();

            // Subsystem exist
            Assert.IsNotNull(xrSubsystem, $"No active {typeof(TXRSubsystem).FullName} is available.");
            Assert.IsInstanceOf<TSubsystem>(xrSubsystem, $"No active {typeof(TSubsystem).FullName} is available.");

            return xrSubsystem as TSubsystem;
        }
    }
}
