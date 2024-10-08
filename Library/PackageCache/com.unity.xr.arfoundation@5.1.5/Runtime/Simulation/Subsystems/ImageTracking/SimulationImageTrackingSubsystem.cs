using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of <see cref="XRImageTrackingSubsystem"/>.
    /// Do not create this directly. Use the <c>SubsystemManager</c> instead.
    /// </summary>
    public sealed class SimulationImageTrackingSubsystem : XRImageTrackingSubsystem
    {
        internal const string k_SubsystemId = "XRSimulation-ImageTracking";

        class SimulationProvider : Provider, ISimulationSessionResetHandler
        {
            readonly SimulationTrackedImageDiscoverer m_Discoverer = new();

            XRTrackedImage[] m_Added;
            int m_NumAdded;
            Dictionary<TrackableId, XRTrackedImage> m_Updated;
            XRTrackedImage[] m_UpdatedCopyBuffer;
            TrackableId[] m_Removed;
            int m_NumRemoved;

            /// <inheritdoc/>
            /// <remarks>
            /// Setting the <c>RuntimeReferenceImageLibrary</c> is optional. For ease of scene setup, all
            /// <see cref="SimulatedTrackedImage"/>s in a simulated environment will track regardless of whether
            /// there are matching reference images in the <c>RuntimeReferenceImageLibrary</c>. If the library
            /// property is set, the subsystem will use the source image GUIDs contained therein.
            /// </remarks>
            public override RuntimeReferenceImageLibrary imageLibrary
            {
                set
                {
                    switch (value)
                    {
                        case null:
                            m_Discoverer.simulationRuntimeImageLibrary = null;
                            return;
                        case SimulationRuntimeImageLibrary runtimeLibrary:
                            m_Discoverer.simulationRuntimeImageLibrary = runtimeLibrary;
                            return;
                        default:
                            throw new ArgumentException($"{nameof(XRImageTrackingSubsystem)} {nameof(imageLibrary)} setter was given an invalid {nameof(RuntimeReferenceImageLibrary)}. Use {nameof(CreateRuntimeLibrary)} to generate a valid runtime library.");
                    }
                }
            }

            /// <inheritdoc/>
            /// <remarks>
            /// Currently unused. All images in a simulated environment can move regardless of a requested maximum.
            /// </remarks>
            public override int requestedMaxNumberOfMovingImages { get; set; }

            public override int currentMaxNumberOfMovingImages => requestedMaxNumberOfMovingImages;

            bool isInitialized => m_Added != null && m_Updated != null && m_Removed != null;

            public SimulationProvider()
            {
                m_Discoverer.imagesInitialized += OnImagesInitialized;
                m_Discoverer.imageAdded += OnImageAdded;
                m_Discoverer.imageUpdated += OnImageUpdated;
                m_Discoverer.imageRemoved += OnImageRemoved;
            }

            /// <inheritdoc/>
            public override RuntimeReferenceImageLibrary CreateRuntimeLibrary(XRReferenceImageLibrary serializedLibrary)
            {
                return new SimulationRuntimeImageLibrary(serializedLibrary);
            }

            /// <inheritdoc/>
            public override void Start()
            {
#if UNITY_EDITOR
                SimulationSubsystemAnalytics.SubsystemStarted(k_SubsystemId);
#endif

                SimulationEnvironmentScanner.GetOrCreate().EnsureMeshColliders();
                m_Discoverer.Start();

                SimulationSessionSubsystem.s_SimulationSessionReset += OnSimulationSessionReset;
            }

            /// <inheritdoc/>
            public override void Stop()
            {
                SimulationSessionSubsystem.s_SimulationSessionReset -= OnSimulationSessionReset;
                m_Discoverer.Stop();
                m_NumAdded = 0;
                m_NumRemoved = 0;
            }

            public override void Destroy() { }

            public void OnSimulationSessionReset()
            {
                if (!isInitialized)
                    return;

                ValidateChanges();

                m_NumAdded = 0;
                Array.Clear(m_Added, 0, m_Added.Length);
                m_Updated.Clear();
                
                m_Discoverer.Restart();
            }

            /// <inheritdoc/>
            public override TrackableChanges<XRTrackedImage> GetChanges(XRTrackedImage defaultTrackedImage, Allocator allocator)
            {
                if (!isInitialized)
                    return new TrackableChanges<XRTrackedImage>(0, 0, 0, allocator);

                ValidateChanges();

                var numUpdated = m_Updated.Count;

                var changes = new TrackableChanges<XRTrackedImage>(
                    m_NumAdded,
                    numUpdated,
                    m_NumRemoved,
                    allocator,
                    defaultTrackedImage);

                if (m_NumAdded > 0)
                {
                    NativeArray<XRTrackedImage>.Copy(m_Added, 0, changes.added, 0, m_NumAdded);
                    m_NumAdded = 0;
                }

                if (numUpdated > 0)
                {
                    m_Updated.Values.CopyTo(m_UpdatedCopyBuffer, 0);
                    NativeArray<XRTrackedImage>.Copy(m_UpdatedCopyBuffer, 0, changes.updated, 0, m_Updated.Count);
                    m_Updated.Clear();
                }

                if (m_NumRemoved > 0)
                {
                    NativeArray<TrackableId>.Copy(m_Removed, 0, changes.removed, 0, m_NumRemoved);
                    m_NumRemoved = 0;
                }

                return changes;
            }

            /// <summary>
            /// If the same <c>TrackableId</c> is present in more than one list per frame, the
            /// <see cref="ValidationUtility{T}"/> will throw an exception in Editor and development builds.
            /// Because subsystem tracking doesn't update every frame, it's possible that the same trackable has
            /// appeared in multiple lists. Use the most recent update as an add if this happens.
            /// </summary>
            void ValidateChanges()
            {
                for (var i = 0; i < m_NumAdded; i++)
                {
                    var trackableId = m_Added[i].trackableId;
                    if (!m_Updated.TryGetValue(trackableId, out var latestUpdatedImage))
                        continue;

                    m_Added[i] = latestUpdatedImage;
                    m_Updated.Remove(trackableId);
                }
            }

            void OnImagesInitialized(int numImages)
            {
                m_Added = new XRTrackedImage[numImages];
                m_NumAdded = 0;
                m_Updated = new Dictionary<TrackableId, XRTrackedImage>(numImages);
                m_UpdatedCopyBuffer = new XRTrackedImage[numImages];
                m_Removed = new TrackableId[numImages];
                m_NumRemoved = 0;
            }

            void OnImageAdded(XRTrackedImage image)
            {
                m_Added[m_NumAdded++] = image;
            }

            void OnImageUpdated(XRTrackedImage image)
            {
                m_Updated[image.trackableId] = image;
            }

            void OnImageRemoved(TrackableId image)
            {
                m_Removed[m_NumRemoved++] = image;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            XRImageTrackingSubsystemDescriptor.Create(new XRImageTrackingSubsystemDescriptor.Cinfo
            {
                id = k_SubsystemId,
                providerType = typeof(SimulationProvider),
                subsystemTypeOverride = typeof(SimulationImageTrackingSubsystem),
                requiresPhysicalImageDimensions = false,
                supportsMovingImages = true,
                supportsMutableLibrary = true,
                supportsImageValidation = true
            });
        }
    }
}
