using NUnit.Framework;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation.Tests
{
    abstract class SimulationSessionTestSetup : SimulationTestSetup
    {
        protected void SetupSession()
        {
            SetupLoader();
            AddXROrigin();
            StartSubsystem<XRSessionSubsystem, SimulationSessionSubsystem>();
        }

        protected void TearDownSession()
        {
            StopSubsystem<XRSessionSubsystem, SimulationSessionSubsystem>();
            RemoveXROrigin();
            TearDownLoader();
        }

        protected void SetupInput()
        {
            StartSubsystem<XRInputSubsystem, XRInputSubsystem>();
        }

        protected void TearDownInput()
        {
            StopSubsystem<XRInputSubsystem, XRInputSubsystem>();
        }

        void StartSubsystem<TXRSubsystem, TSubsystem>()
            where TXRSubsystem : class, ISubsystem, new()
            where TSubsystem : class, ISubsystem, new()
        {
            var xrSubsystem = GetSimulationSubsystem<TXRSubsystem, TSubsystem>();

            // Initial state is not running
            Assert.IsFalse(xrSubsystem.running);

            // After start subsystem is running
            xrSubsystem.Start();
            Assert.IsTrue(xrSubsystem.running);
        }

        void StopSubsystem<TXRSubsystem, TSubsystem>()
            where TXRSubsystem : class, ISubsystem, new()
            where TSubsystem : class, ISubsystem, new()
        {
            var xrSubsystem = GetSimulationSubsystem<TXRSubsystem, TSubsystem>();

            // Initial state is running
            Assert.IsTrue(xrSubsystem.running);

            // After stop subsystem is not running
            xrSubsystem.Stop();
            Assert.IsFalse(xrSubsystem.running);
        }
    }
}
