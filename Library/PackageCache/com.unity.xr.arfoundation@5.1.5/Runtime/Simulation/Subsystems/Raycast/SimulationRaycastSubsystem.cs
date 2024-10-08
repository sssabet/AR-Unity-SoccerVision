using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of the <see cref="UnityEngine.XR.ARSubsystems.XRRaycastSubsystem"/>.
    /// Do not create this directly. Use the <c>SubsystemManager</c> instead.
    /// </summary>
    public sealed class SimulationRaycastSubsystem : XRRaycastSubsystem
    {
        internal const string k_SubsystemId = "XRSimulation-Raycast";

        /// <summary>
        /// Provider for the <see cref="SimulationRaycastSubsystem"/>.
        /// </summary>
        class SimulationProvider : Provider
        {
            readonly List<NativeArray<XRRaycastHit>> m_RaycasterHitResults = new();

#if UNITY_EDITOR
            public override void Start()
            {
                SimulationSubsystemAnalytics.SubsystemStarted(k_SubsystemId);
            }
#endif

            /// <inheritdoc/>
            public override NativeArray<XRRaycastHit> Raycast(XRRaycastHit defaultRaycastHit, Ray ray, TrackableType trackableTypeMask, Allocator allocator)
            {
                var count = 0;

                foreach (var raycaster in SimulationRaycasterRegistry.instance.registeredRaycasters)
                {
                    var hits = raycaster.Raycast(ray, trackableTypeMask, Allocator.Temp);
                    if (hits.IsCreated)
                    {
                        if (hits.Length != 0)
                        {
                            m_RaycasterHitResults.Add(hits);
                            count += hits.Length;
                        }
                        else if (hits.Length == 0)
                        {
                            hits.Dispose();
                        }
                    }
                }

                var allHits = new NativeArray<XRRaycastHit>(count, allocator);
                var dstIndex = 0;
                foreach (var hitArray in m_RaycasterHitResults)
                {
                    NativeArray<XRRaycastHit>.Copy(hitArray, 0, allHits, dstIndex, hitArray.Length);
                    dstIndex += hitArray.Length;
                    hitArray.Dispose();
                }

                m_RaycasterHitResults.Clear();
                return allHits;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            XRRaycastSubsystemDescriptor.RegisterDescriptor(new XRRaycastSubsystemDescriptor.Cinfo
            {
                id = k_SubsystemId,
                providerType = typeof(SimulationProvider),
                subsystemTypeOverride = typeof(SimulationRaycastSubsystem),
                supportsViewportBasedRaycast = false,
                supportsWorldBasedRaycast = true,
                supportedTrackableTypes = TrackableType.FeaturePoint,
                supportsTrackedRaycasts = false
            });
        }
    }
}
