using System;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.SubsystemsImplementation.Extensions;

namespace UnityEngine.XR.ARSubsystems.Tests
{
    public class XRTestSubsystemDescriptor :
        SubsystemDescriptorWithProvider<XRTestSubsystem, XRTestSubsystem.Provider>
    {
        public struct Cinfo : IEquatable<Cinfo>
        {
            public string id { get; set; }

            public Type providerType { get; set; }

            public Type subsystemTypeOverride { get; set; }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (id != null ? id.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (providerType != null ? providerType.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (subsystemTypeOverride != null ? subsystemTypeOverride.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public bool Equals(Cinfo other)
            {
                return
                    string.Equals(id, other.id) &&
                    ReferenceEquals(providerType, other.providerType) &&
                    ReferenceEquals(subsystemTypeOverride, other.subsystemTypeOverride);
            }

            public override bool Equals(object obj) => obj is Cinfo && Equals((Cinfo)obj);

            public static bool operator ==(Cinfo lhs, Cinfo rhs) => lhs.Equals(rhs);

            public static bool operator !=(Cinfo lhs, Cinfo rhs) => !lhs.Equals(rhs);
        }

        public static void RegisterDescriptor(Cinfo cinfo)
        {
            SubsystemDescriptorStore.RegisterDescriptor(new XRTestSubsystemDescriptor(cinfo));
        }

        XRTestSubsystemDescriptor(Cinfo cinfo)
        {
            id = cinfo.id;
            providerType = cinfo.providerType;
            subsystemTypeOverride = cinfo.subsystemTypeOverride;
        }
    }

    public class XRTestSubsystem
        : SubsystemWithProvider<XRTestSubsystem, XRTestSubsystemDescriptor, XRTestSubsystem.Provider>
    {
        public class Provider : SubsystemProvider<XRTestSubsystem>
        {
            public int startCount { get; private set; }
            public int stopCount { get; private set; }
            public int destroyCount { get; private set; }

            public override void Start() => ++startCount;

            public override void Stop() => ++stopCount;

            public override void Destroy() => ++destroyCount;
        }
    }

    public class XRTestSubsystemImpl : XRTestSubsystem
    {
        public class ProviderImpl : Provider
        {
            public ProviderImpl() { }
        }
    }

    [TestFixture]
    public class XRSubsystemTestFixture
    {
        [OneTimeSetUp]
        public void RegisterTestDescriptor()
        {
            XRTestSubsystemDescriptor.RegisterDescriptor(new XRTestSubsystemDescriptor.Cinfo
            {
                id = "Test-Subsystem",
                providerType = typeof(XRTestSubsystemImpl.ProviderImpl),
                subsystemTypeOverride = typeof(XRTestSubsystemImpl)
            });
        }

        static List<XRTestSubsystemDescriptor> s_Descs = new List<XRTestSubsystemDescriptor>();
        static XRTestSubsystem CreateTestSubsystem()
        {
            SubsystemManager.GetSubsystemDescriptors(s_Descs);
            foreach (var desc in s_Descs)
            {
                if (desc.id == "Test-Subsystem")
                    return desc.Create();
            }

            return s_Descs[0].Create();
        }

        [Test]
        public void IsRunningAfterStartCalled()
        {
            var subsystem = CreateTestSubsystem();
            subsystem.Start();
            Assert.IsTrue(subsystem.running);
            subsystem.Destroy();
        }

        [Test]
        public void IsNotRunningAfterStopCalled()
        {
            var subsystem = CreateTestSubsystem();
            subsystem.Start();
            subsystem.Stop();
            Assert.IsFalse(subsystem.running);
            subsystem.Destroy();
        }

        [Test]
        public void DestroyCallsStopWhenRunning()
        {
            var subsystem = CreateTestSubsystem();
            subsystem.Start();
            Assert.AreEqual(0, subsystem.GetProvider().stopCount);
            subsystem.Destroy();
            Assert.IsFalse(subsystem.running);
            Assert.AreEqual(1, subsystem.GetProvider().stopCount);
        }

        [Test]
        public void DestroyDoesNotCallStopWhenNotRunning()
        {
            var subsystem = CreateTestSubsystem();
            subsystem.Start();
            subsystem.Stop();
            Assert.IsFalse(subsystem.running);
            Assert.AreEqual(1, subsystem.GetProvider().stopCount);
            subsystem.Destroy();
            Assert.AreEqual(1, subsystem.GetProvider().stopCount);
        }

        [Test]
        public void DestroyOnlyCalledOnce()
        {
            var subsystem = CreateTestSubsystem();
            Assert.AreEqual(0, subsystem.GetProvider().destroyCount);

            subsystem.Destroy();
            Assert.AreEqual(1, subsystem.GetProvider().destroyCount);
            subsystem.Destroy();
            subsystem.Destroy();
            subsystem.Destroy();
            subsystem.Destroy();
            Assert.AreEqual(1, subsystem.GetProvider().destroyCount);
        }

        [Test]
        public void StartOnlyCalledWhenNotRunning()
        {
            var subsystem = CreateTestSubsystem();
            Assert.AreEqual(0, subsystem.GetProvider().startCount);
            subsystem.Start();
            Assert.IsTrue(subsystem.running);
            Assert.AreEqual(1, subsystem.GetProvider().startCount);
            subsystem.Start();
            Assert.IsTrue(subsystem.running);
            Assert.AreEqual(1, subsystem.GetProvider().startCount);
            subsystem.Stop();
            Assert.IsFalse(subsystem.running);
            subsystem.Start();
            Assert.IsTrue(subsystem.running);
            Assert.AreEqual(2, subsystem.GetProvider().startCount);
            subsystem.Destroy();
        }

        [Test]
        public void StopOnlyCalledWhenRunning()
        {
            var subsystem = CreateTestSubsystem();

            subsystem.Stop();
            Assert.AreEqual(0, subsystem.GetProvider().stopCount);

            subsystem.Start();
            Assert.IsTrue(subsystem.running);
            Assert.AreEqual(0, subsystem.GetProvider().stopCount);

            subsystem.Stop();
            Assert.IsFalse(subsystem.running);
            Assert.AreEqual(1, subsystem.GetProvider().stopCount);

            subsystem.Stop();
            Assert.AreEqual(1, subsystem.GetProvider().startCount);

            subsystem.Start();
            Assert.IsTrue(subsystem.running);
            subsystem.Stop();
            Assert.IsFalse(subsystem.running);
            Assert.AreEqual(2, subsystem.GetProvider().stopCount);
            subsystem.Destroy();
        }
    }
}
