using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.ARSubsystems;
using static UnityEngine.XR.Simulation.SimulationUtils;

namespace UnityEngine.XR.Simulation
{
    class SimulationEnvironmentProbeDiscoverer : IDisposable
    {
        const string k_EnvironmentProbeShaderPropertyName = "_SimulationEnvironmentProbe";
        const string k_ManualProbePrefix = "XRSimManualEnvironmentProbe";
        const string k_RenderCameraName = "XRSimulationEnvironmentProbeRenderCamera";

        const HideFlags k_ProbeCameraHideFlags = HideFlags.HideAndDontSave;
        const HideFlags k_GeneratedHideFlags = HideFlags.NotEditable | HideFlags.DontSave;

        const int k_InitialListCacheCapacity = 16;
        const int k_FrustumPlaneCount = 6;

        static readonly int k_EnvironmentProbeShaderPropertyId
            = Shader.PropertyToID(k_EnvironmentProbeShaderPropertyName);

        // cached and reused to prevent GC
        static readonly Plane[] s_CameraPlanes = new Plane[k_FrustumPlaneCount];
        static readonly List<SimulatedEnvironmentProbe> s_NeedsGeneratedCubemaps = new(k_InitialListCacheCapacity);
        static readonly List<SimulatedEnvironmentProbe> s_HasGeneratedCubemaps = new(k_InitialListCacheCapacity);
        static readonly List<SimulatedEnvironmentProbe> s_CollectedProbes = new(k_InitialListCacheCapacity);

        readonly List<XREnvironmentProbe> m_Added = new();
        readonly List<XREnvironmentProbe> m_Updated = new();
        readonly List<TrackableId> m_Removed = new();
        readonly List<ProbeData> m_AutomaticProbes = new();
        readonly List<ProbeData> m_ManualProbes = new();

        CameraData m_ProbeCameraData;
        CameraData m_OriginCameraData;

        EnvironmentProbeParams m_ProbeParams;

        int m_DiscoveredAutomaticCount;
        bool m_IsSceneInitialized;
        bool m_IsStarted;

        public IReadOnlyList<XREnvironmentProbe> added => m_Added;
        public IReadOnlyList<XREnvironmentProbe> updated => m_Updated;
        public IReadOnlyList<TrackableId> removed => m_Removed;

        public bool IsReady => m_IsSceneInitialized && m_IsStarted;

        public bool HasAnyChanges => m_Added.Count > 0 || m_Updated.Count > 0 || m_Removed.Count > 0;

        public void Dispose()
        {
            if (!m_IsStarted)
                return;

            Stop();
        }

        public void Start()
        {
            if (m_IsStarted)
                return;

            SetupRendering();

            m_ProbeParams = XRSimulationRuntimeSettings.Instance.environmentProbeDiscoveryParams;

            BaseSimulationSceneManager.environmentSetupFinished += OnEnvironmentSetupFinished;
            BaseSimulationSceneManager.environmentTeardownStarted += OnEnvironmentTeardownStarted;

            if (!string.IsNullOrEmpty(BaseSimulationSceneManager.activeSceneName) && !m_IsSceneInitialized)
                OnEnvironmentSetupFinished();

            var xrOrigin = FindObjectsUtility.FindAnyObjectByType<XROrigin>();
            m_OriginCameraData = new CameraData(xrOrigin.Camera);
            m_IsStarted = true;
        }

        public void Stop()
        {
            if (!m_IsStarted)
                return;

            BaseSimulationSceneManager.environmentSetupFinished -= OnEnvironmentSetupFinished;
            BaseSimulationSceneManager.environmentTeardownStarted -= OnEnvironmentTeardownStarted;

            TeardownRendering();

            m_IsSceneInitialized = false;

            RemoveProbeSet(m_AutomaticProbes);
            RemoveProbeSet(m_ManualProbes);

            ClearChangeBuffers();

            m_IsStarted = false;
        }

        public void Update()
        {
            using (new ScopedProfiler("XRSimulationEnvironmentProbeUpdate"))
            {
                CheckAndMarkProbeSet(m_ManualProbes);
                CheckAndMarkProbeSet(m_AutomaticProbes, m_DiscoveredAutomaticCount);
                ScanForUndiscoveredProbes();
            }
        }

        public void Reset()
        {
            if (!m_IsStarted)
                return;

            RemoveProbeSet(m_ManualProbes);

            OnEnvironmentSetupFinished();
            m_DiscoveredAutomaticCount = 0;
        }

        static int FindProbeIndexById(List<XREnvironmentProbe> probes, TrackableId id)
        {
            for (var i = 0; i < probes.Count; i++)
            {
                if (id == probes[i].trackableId)
                    return i;
            }

            return -1;
        }

        static bool CheckSceneInitialized(Scene scene) => scene.IsValid() && scene.isLoaded;

        void SetupRendering()
        {
            var probeCamSource = new GameObject(k_RenderCameraName, typeof(Camera))
            {
                hideFlags = k_ProbeCameraHideFlags
            };

            m_ProbeCameraData = new CameraData(probeCamSource.GetComponent<Camera>())
            {
                camera =
                {
                    depth = -100,
                    cullingMask = 1 << XRSimulationRuntimeSettings.Instance.environmentLayer,
                    clearFlags = CameraClearFlags.Color,
                    backgroundColor = Color.clear,
                    enabled = false
                }
            };
        }

        void TeardownRendering()
        {
            if (m_ProbeCameraData.camera != null)
                Object.DestroyImmediate(m_ProbeCameraData.camera.gameObject);

            m_ProbeCameraData = default;
        }

        TrackableId RenderCubemapAndMarkNewProbe(SimulatedEnvironmentProbe probe, bool isManualProbe)
        {
            var probeTransform = probe.transform;
            m_ProbeCameraData.transform.SetPositionAndRotation(probeTransform.position, probeTransform.rotation);
            
#if UNITY_2022_1_OR_NEWER
            var cubemap = new Cubemap(
                m_ProbeParams.cubemapFaceSize,
                TextureFormat.RGBAHalf,
                0,
                true)
#else
            var cubemap = new Cubemap(
                m_ProbeParams.cubemapFaceSize,
                TextureFormat.RGBAHalf,
                false)
#endif
            {
                name = $"{probe.name}-Generated-Cubemap",
                hideFlags = k_GeneratedHideFlags
            };

            if (m_ProbeCameraData.camera.RenderToCubemap(cubemap, 63))
            {
                probe.cubemap = cubemap;
                return MarkNewProbe(new ProbeData(probe, isManualProbe));
            }

            Object.DestroyImmediate(cubemap);
            return TrackableId.invalidId;
        }

        TrackableId MarkNewProbe(in ProbeData probeData)
        {
            // ensure the newly created probes don't also immediately get marked as Updated
            probeData.ResetChangeFlags();

            if (probeData.isManual)
            {
                m_ManualProbes.Add(probeData);
                m_Added.Add(probeData.xrProbe);
            }
            else
            {
                m_AutomaticProbes.Add(probeData);
            }

            return probeData.id;
        }

        void MarkRemovedProbe(List<ProbeData> probes, int index)
        {
            var probeData = probes[index];
            RemoveFromChangeBuffers(probeData.xrProbe);

            probeData.Cleanup();

            probes.RemoveAt(index);
        }

        void RemoveFromChangeBuffers(in XREnvironmentProbe probe)
        {
            var probeId = probe.trackableId;

            var removedIndex = m_Removed.IndexOf(probeId);
            if (removedIndex == -1)
                m_Removed.Add(probeId);

            var addedIndex = FindProbeIndexById(m_Added, probeId);
            if (addedIndex != -1)
                m_Added.RemoveAt(addedIndex);

            var updatedIndex = FindProbeIndexById(m_Updated, probeId);
            if (updatedIndex != -1)
                m_Updated.RemoveAt(updatedIndex);
        }

        public void ClearChangeBuffers()
        {
            m_Added.Clear();
            m_Updated.Clear();
            m_Removed.Clear();
        }

        void CheckAndMarkProbeSet(List<ProbeData> probes, int maxProcessCount = -1)
        {
            if (maxProcessCount == -1)
                maxProcessCount = probes.Count;

            for (var i = 0; i < maxProcessCount; i++)
            {
                var probeData = probes[i];
                if (!probeData.hasChanges)
                    continue;

                probeData.ResetChangeFlags();

                var probeIndex = FindProbeIndexById(m_Updated, probeData.id);
                if (probeIndex == -1)
                {
                    m_Updated.Add(probeData.xrProbe);
                }
                else
                {
                    m_Updated[probeIndex] = probeData.xrProbe;
                }
            }
        }

        void ScanForUndiscoveredProbes()
        {
            var automaticProbesCount = m_AutomaticProbes.Count;
            if (m_DiscoveredAutomaticCount == automaticProbesCount)
                return;

            GeometryUtility.CalculateFrustumPlanes(m_OriginCameraData.camera, s_CameraPlanes);
            var cameraPos = m_OriginCameraData.transform.position;

            var maxDiscoveryDistance = m_ProbeParams.maxDiscoveryDistance;
            var sqrMaxDiscoveryDistance = maxDiscoveryDistance * maxDiscoveryDistance;

            for (var i = m_DiscoveredAutomaticCount; i < automaticProbesCount; i++)
            {
                var probeData = m_AutomaticProbes[i];

                if (probeData.hasChanges)
                    probeData.ResetChangeFlags();

                var transform = probeData.transform;
                var probePose = new Pose(transform.position, transform.rotation);
                var xrProbe = probeData.xrProbe;

                var probeBounds = new Bounds(probePose.position, Vector3.Scale(xrProbe.scale, xrProbe.size));
                if (probeBounds.SqrDistance(cameraPos) > sqrMaxDiscoveryDistance ||
                   !GeometryUtility.TestPlanesAABB(s_CameraPlanes, probeBounds))
                {
                    probeData.discoveryTimer = m_ProbeParams.discoveryDelayTime;
                    m_AutomaticProbes[i] = probeData;
                    continue;
                }

                var discoveryTime = probeData.discoveryTimer;
                discoveryTime -= m_ProbeParams.minUpdateTime;
                probeData.discoveryTimer = discoveryTime;
                if (discoveryTime > 0.0f)
                {
                    m_AutomaticProbes[i] = probeData;
                    continue;
                }

                probeData.discoveryTimer = 0.0f;

                probeData.ResetChangeFlags();
                m_AutomaticProbes[i] = probeData;

                // swap at partition and increment discovered count
                m_AutomaticProbes.SwapAtIndices(i, m_DiscoveredAutomaticCount);
                m_DiscoveredAutomaticCount++;

                m_Added.Add(probeData.xrProbe);
            }
        }

        void OnEnvironmentSetupFinished()
        {
            if (m_IsSceneInitialized)
            {
                OnEnvironmentTeardownStarted();
            }

            if (string.IsNullOrEmpty(BaseSimulationSceneManager.activeSceneName))
            {
                m_IsSceneInitialized = false;
                return;
            }

            var sceneManager = SimulationSessionSubsystem.simulationSceneManager;
            m_IsSceneInitialized = CheckSceneInitialized(sceneManager.environmentScene);
            if (!m_IsSceneInitialized)
            {
                return;
            }

            s_NeedsGeneratedCubemaps.Clear();
            s_HasGeneratedCubemaps.Clear();
            s_CollectedProbes.Clear();

            var environmentRoot = sceneManager.simulationEnvironment.gameObject;
            environmentRoot.GetComponentsInChildren(s_CollectedProbes);

            foreach (var environmentProbe in s_CollectedProbes)
            {
                if (environmentProbe.cubemap == null)
                    s_NeedsGeneratedCubemaps.Add(environmentProbe);
                else
                    s_HasGeneratedCubemaps.Add(environmentProbe);
            }
            s_CollectedProbes.Clear();

            foreach (var probe in s_HasGeneratedCubemaps)
            {
                MarkNewProbe(new ProbeData(probe));
            }
            s_HasGeneratedCubemaps.Clear();

            if (s_NeedsGeneratedCubemaps.Count <= 0)
                return;

            try
            {
                foreach (var probe in s_NeedsGeneratedCubemaps)
                {
                    RenderCubemapAndMarkNewProbe(probe, false);
                }
            }
            finally
            {
                s_NeedsGeneratedCubemaps.Clear();
            }
        }

        void OnEnvironmentTeardownStarted()
        {
            if (!m_IsSceneInitialized)
                return;

            m_IsSceneInitialized = false;
            RemoveProbeSet(m_AutomaticProbes);
        }

        void RemoveProbeSet(List<ProbeData> probeSet)
        {
            var probeSetCount = probeSet.Count;
            if (probeSetCount == 0)
                return;

            for (var i = probeSetCount - 1; i >= 0; i--)
            {
                MarkRemovedProbe(probeSet, i);
            }

            probeSet.Clear();
        }

        public bool TryAddManualProbe(Pose pose, Vector3 scale, Vector3 size, out XREnvironmentProbe xrProbe)
        {
            var probeGo = new GameObject
            {
                hideFlags = k_GeneratedHideFlags
            };

            var probeTransform = probeGo.transform;
            probeTransform.SetPositionAndRotation(pose.position, pose.rotation);
            probeTransform.localScale = scale;

            var probe = probeGo.AddComponent<SimulatedEnvironmentProbe>();
            probe.size = size;

            var id = RenderCubemapAndMarkNewProbe(probe, true);

            if (id == TrackableId.invalidId)
            {
                xrProbe = XREnvironmentProbe.defaultValue;
                return false;
            }

            probeGo.name = $"{k_ManualProbePrefix}-{id.ToString()}";
            xrProbe = m_ManualProbes[^1].xrProbe;

            return true;
        }

        public bool TryRemoveManualProbe(TrackableId trackableId)
        {
            for (var i = 0; i < m_ManualProbes.Count; i++)
            {
                var probeData = m_ManualProbes[i];
                if (probeData.id != trackableId)
                    continue;

                MarkRemovedProbe(m_ManualProbes, i);

                return true;
            }

            return false;
        }

        readonly struct CameraData
        {
            public readonly Camera camera;
            public readonly Transform transform;

            public CameraData(Camera camera)
            {
                if (camera == null)
                    throw new NullReferenceException($"No {nameof(camera)} for {nameof(CameraData)} constructor!");

                this.camera = camera;
                transform = this.camera.transform;
            }
        }

        struct ProbeData
        {
            readonly SimulatedEnvironmentProbe m_SimProbe;
            readonly bool m_HasGenerated;

            public readonly XREnvironmentProbe xrProbe;
            public readonly Transform transform;
            public float discoveryTimer;
            public readonly bool isManual;

            public TrackableId id => xrProbe.trackableId;

            public bool hasChanges => m_SimProbe.hasChanged || transform.hasChanged;

            static XREnvironmentProbe CreateProbe(
                SimulatedEnvironmentProbe simProbe,
                Transform transform)
            {
                var descriptor = new XRTextureDescriptor(
                    nativeTexture: simProbe.cubemap.GetNativeTexturePtr(),
                    width: simProbe.cubemap.width,
                    height: simProbe.cubemap.height,
                    mipmapCount: simProbe.cubemap.mipmapCount,
                    format: simProbe.cubemap.format,
                    propertyNameId: k_EnvironmentProbeShaderPropertyId,
                    depth: 1,
                    dimension: simProbe.cubemap.dimension);

                return new XREnvironmentProbe(
                    trackableId: GenerateTrackableId(),
                    scale: transform.localScale,
                    pose: new Pose(transform.position, transform.rotation),
                    size: simProbe.size,
                    descriptor: descriptor,
                    trackingState: TrackingState.Tracking,
                    nativePtr: descriptor.nativeTexture);
            }

            ProbeData(SimulatedEnvironmentProbe probe, bool isManual, bool hasGenerated)
            {
                if (probe == null || probe.cubemap == null)
                {
                    throw new NullReferenceException(
                        $"Null reference with {(probe != null && probe.cubemap == null ? "cubemap" : "probe")}");
                }

                m_SimProbe = probe;
                transform = m_SimProbe.transform;
                xrProbe = CreateProbe(m_SimProbe, transform);
                discoveryTimer = 0.0f;
                this.isManual = isManual;
                m_HasGenerated = hasGenerated;
            }

            /// <summary>
            /// <see cref="ProbeData"/> constructor for probes with generated cubemapds (scene w/o preset cubemap or manual)
            /// </summary>
            /// <param name="probe">The <see cref="SimulatedEnvironmentProbe"/> to be tracked with this data.</param>
            /// <param name="isManual">
            /// If <see langword="true"/>, this is a user-added manual probe.
            /// If <see langword="false"/>, this is a probe defined in a scene.
            /// </param>
            public ProbeData(SimulatedEnvironmentProbe probe, bool isManual) : this(probe, isManual, true)
            {
            }

            /// <summary><see cref="ProbeData"/> constructor for scene-defined probes that already have preset cubemaps.</summary>
            /// <param name="probe">The <see cref="SimulatedEnvironmentProbe"/> to be tracked with this data.</param>
            public ProbeData(SimulatedEnvironmentProbe probe) : this(probe, false, false)
            {
            }

            public readonly void ResetChangeFlags()
            {
                m_SimProbe.hasChanged = false;
                transform.hasChanged = false;
            }

            public void Cleanup()
            {
                if (!m_HasGenerated)
                    return;

                Object.Destroy(m_SimProbe.cubemap);

                if (isManual)
                {
                    Object.Destroy(m_SimProbe.gameObject);
                }
            }
        }
    }
}
