using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of
    /// [XREnvironmentProbeSubsystem](xref:UnityEngine.XR.ARSubsystems.XREnvironmentProbeSubsystem).
    /// Do not create this directly. Use the <c>SubsystemManager</c> instead.
    /// </summary>
    public class SimulationEnvironmentProbeSubsystem : XREnvironmentProbeSubsystem
    {
        internal const string k_SubsystemId = "XRSimulation-EnvironmentProbe";
        
        class SimulationProvider : Provider, ISimulationSessionResetHandler
        {
            SimulationEnvironmentProbeDiscoverer m_ProbeDiscoverer;
            EnvironmentProbeParams m_EnvironmentProbeParams;
            float m_LastUpdateTime;

            public override bool automaticPlacementRequested { get; set; }

            public override bool automaticPlacementEnabled => true;

            public override bool environmentTextureHDREnabled => true;

            public override bool environmentTextureHDRRequested { get; set; }

            protected override bool TryInitialize()
            {
                m_ProbeDiscoverer = new SimulationEnvironmentProbeDiscoverer();
                m_EnvironmentProbeParams = XRSimulationRuntimeSettings.Instance.environmentProbeDiscoveryParams;
                return base.TryInitialize();
            }

            public override void Destroy()
            {
                m_ProbeDiscoverer.Dispose();
                m_ProbeDiscoverer = null;
                m_EnvironmentProbeParams = null;
            }

            public override void Start()
            {
                m_ProbeDiscoverer.Start();
                SimulationSessionSubsystem.s_SimulationSessionReset += OnSimulationSessionReset;
            }

            public override void Stop()
            {
                SimulationSessionSubsystem.s_SimulationSessionReset -= OnSimulationSessionReset;
                m_ProbeDiscoverer.Stop();
            }

            public void OnSimulationSessionReset()
            {
                m_LastUpdateTime = Time.timeSinceLevelLoad;
                m_ProbeDiscoverer.Reset();
            }

            public override TrackableChanges<XREnvironmentProbe> GetChanges(XREnvironmentProbe defaultEnvironmentProbe, Allocator allocator)
            {
                if (!m_ProbeDiscoverer.IsReady)
                    return new TrackableChanges<XREnvironmentProbe>();

                using (new ScopedProfiler("SimulationProbeSubsystem.GetChanges"))
                {
                    if (Time.timeSinceLevelLoad - m_LastUpdateTime > m_EnvironmentProbeParams.minUpdateTime)
                    {
                        m_ProbeDiscoverer.Update();
                        m_LastUpdateTime = Time.timeSinceLevelLoad;
                    }

                    var added = m_ProbeDiscoverer.added;
                    var updated = m_ProbeDiscoverer.updated;
                    var removed = m_ProbeDiscoverer.removed;

                    var addedCount = added.Count;
                    var updatedCount = updated.Count;
                    var removedCount = removed.Count;

                    if (!m_ProbeDiscoverer.HasAnyChanges)
                        return new TrackableChanges<XREnvironmentProbe>();

                    var changes = new TrackableChanges<XREnvironmentProbe>(addedCount, updatedCount, removedCount, allocator);

                    NativeCopyUtility.CopyFromReadOnlyList(added, changes.added);
                    NativeCopyUtility.CopyFromReadOnlyList(updated, changes.updated);
                    NativeCopyUtility.CopyFromReadOnlyList(removed, changes.removed);

                    m_ProbeDiscoverer.ClearChangeBuffers();

                    return changes;
                }
            }

            public override bool TryAddEnvironmentProbe(
                Pose pose,
                Vector3 scale,
                Vector3 size,
                out XREnvironmentProbe environmentProbe) =>
                m_ProbeDiscoverer.TryAddManualProbe(
                    pose,
                    scale,
                    size,
                    out environmentProbe);

            public override bool RemoveEnvironmentProbe(TrackableId trackableId) =>
                m_ProbeDiscoverer.TryRemoveManualProbe(trackableId);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            Register(new XREnvironmentProbeSubsystemCinfo
            {
                id = k_SubsystemId,
                providerType = typeof(SimulationProvider),
                subsystemTypeOverride = typeof(SimulationEnvironmentProbeSubsystem),
                supportsManualPlacement = true,
                supportsRemovalOfManual = true,
                supportsAutomaticPlacement = true,
                supportsRemovalOfAutomatic = true,
                supportsEnvironmentTexture = true,
                supportsEnvironmentTextureHDR = false,
            });
        }
    }
}
