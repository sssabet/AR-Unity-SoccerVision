using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of <see cref="UnityEngine.XR.ARSubsystems.XRAnchorSubsystem"/>.
    /// Do not create this directly. Use the <see cref="UnityEngine.SubsystemManager"/> instead.
    /// </summary>
    public sealed class SimulationAnchorSubsystem : XRAnchorSubsystem
    {
        internal const string k_SubsystemId = "XRSimulation-Anchors";

        class SimulationProvider : Provider, ISimulationSessionResetHandler
        {
            SimulationAnchorImpl m_AnchorImpl;
            AnchorDiscoveryParams m_AnchorDiscoveryParams;
            float m_LastUpdateTime;

            protected override bool TryInitialize()
            {
                m_AnchorDiscoveryParams = XRSimulationRuntimeSettings.Instance.anchorDiscoveryParams;
                m_AnchorImpl = new SimulationAnchorImpl();

                return base.TryInitialize();
            }

            public override void Destroy()
            {
                m_AnchorImpl.Stop();
                m_AnchorImpl = null;
                m_AnchorDiscoveryParams = null;
            }

            public override void Start()
            {
#if UNITY_EDITOR
                SimulationSubsystemAnalytics.SubsystemStarted(k_SubsystemId);
#endif

                m_AnchorImpl.Start();
                SimulationSessionSubsystem.s_SimulationSessionReset += OnSimulationSessionReset;
            }

            public override void Stop()
            {
                SimulationSessionSubsystem.s_SimulationSessionReset -= OnSimulationSessionReset;
                m_AnchorImpl.Stop();
            }

            public void OnSimulationSessionReset()
            {
                m_LastUpdateTime = Time.timeSinceLevelLoad;
                m_AnchorImpl.Reset();
            }

            public override TrackableChanges<XRAnchor> GetChanges(XRAnchor defaultAnchor, Allocator allocator)
            {
                if (!m_AnchorImpl.isReady)
                    return default;

                if (Time.timeSinceLevelLoad - m_LastUpdateTime > m_AnchorDiscoveryParams.minTimeUntilUpdate)
                {
                    m_AnchorImpl.Update();
                    m_LastUpdateTime = Time.timeSinceLevelLoad;
                }

                if (!m_AnchorImpl.hasAnyChanges)
                    return default;

                var added = m_AnchorImpl.added;
                var updated = m_AnchorImpl.updated;
                var removed = m_AnchorImpl.removed;

                var addedCount = added.Count;
                var updateCount = updated.Count;
                var removedCount = removed.Count;

                var changes = new TrackableChanges<XRAnchor>(addedCount, updateCount, removedCount, allocator);

                NativeCopyUtility.CopyFromReadOnlyCollection(added, changes.added);
                NativeCopyUtility.CopyFromReadOnlyCollection(updated, changes.updated);
                NativeCopyUtility.CopyFromReadOnlyCollection(removed, changes.removed);

                m_AnchorImpl.ClearChangeBuffers();

                return changes;
            }

            public override bool TryAddAnchor(Pose pose, out XRAnchor anchor)
            {
                anchor = XRAnchor.defaultValue;

                var hasImpl = m_AnchorImpl != null;
                if(hasImpl)
                {
                    m_AnchorImpl.AddPoseAnchor(pose, out anchor);
                    return true;
                }

                return false;
            }

            public override bool TryAttachAnchor(TrackableId attachedToId, Pose pose, out XRAnchor anchor)
            {
                anchor = XRAnchor.defaultValue;
                return m_AnchorImpl != null &&
                       m_AnchorImpl.TryAttachAnchor(attachedToId, pose, out anchor);
            }

            public override bool TryRemoveAnchor(TrackableId anchorId) =>
                m_AnchorImpl != null && m_AnchorImpl.TryRemoveAnchor(anchorId);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            XRAnchorSubsystemDescriptor.Create(new XRAnchorSubsystemDescriptor.Cinfo
            {
                id = k_SubsystemId,
                providerType = typeof(SimulationProvider),
                subsystemTypeOverride = typeof(SimulationAnchorSubsystem),
                supportsTrackableAttachments = true,
            });
        }
    }
}
