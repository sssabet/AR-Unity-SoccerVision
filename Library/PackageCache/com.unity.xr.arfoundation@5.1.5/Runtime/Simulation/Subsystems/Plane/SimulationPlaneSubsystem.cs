using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation implementation of
    /// [XRPlaneSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystem).
    /// Do not create this directly. Use the <c>SubsystemManager</c> instead.
    /// </summary>
    public sealed class SimulationPlaneSubsystem : XRPlaneSubsystem
    {
        internal const string k_SubsystemId = "XRSimulation-Plane";

#if ENABLE_SIMULATION_DEBUG_VISUALS
        internal event Action<ReadOnlyCollection<PlaneDiscoveryVoxelGrid>> subsystemInitialized
        {
            add => ((SimulationProvider)provider).subsystemInitialized += value;
            remove => ((SimulationProvider)provider).subsystemInitialized -= value;
        }

        /// <summary>
        /// Invoked just before the planes discovery is triggered.
        /// </summary>
        /// <value>An <see cref="Action"/> delegate that you can use to perform pre-processing
        /// steps before planes are discovered.</value>
        internal event Action prePlaneUpdating
        {
            add => ((SimulationProvider)provider).prePlaneUpdating += value;
            remove => ((SimulationProvider)provider).prePlaneUpdating -= value;
        }

        /// <summary>
        /// Invoked right after the planes discovery is finished.
        /// </summary>
        /// <value>An Action delegate that you can use to process <see cref="DiscoveredPlane"/>s.</value>
        internal event Action<IReadOnlyList<DiscoveredPlane>, IReadOnlyList<DiscoveredPlane>, IReadOnlyList<DiscoveredPlane>> postPlaneUpdating
        {
            add => ((SimulationProvider)provider).postPlaneUpdating += value;
            remove => ((SimulationProvider)provider).postPlaneUpdating -= value;
        }
#endif

        /// <summary>
        /// Invoked just after the subsystem provider starts.
        /// </summary>
        protected override void OnStart()
        {
            SimulationEnvironmentScanner.GetOrCreate().RegisterSubsystem(this);
            base.OnStart();
        }

        /// <summary>
        /// Invoked just after the subsystem provider stops.
        /// </summary>
        protected override void OnStop()
        {
            SimulationEnvironmentScanner.GetOrCreate().UnregisterSubsystem(this);
            base.OnStop();
        }

        class SimulationProvider : Provider, ISimulationSessionResetHandler
        {
            const int k_InitialPlanesCapacity = 4;

            readonly List<DiscoveredPlane> m_AddedPlanes = new(k_InitialPlanesCapacity);
            readonly List<DiscoveredPlane> m_UpdatedPlanes = new(k_InitialPlanesCapacity);
            readonly List<DiscoveredPlane> m_RemovedPlanes = new(k_InitialPlanesCapacity);
            readonly Dictionary<TrackableId, DiscoveredPlane> m_AllPlanes = new();

            PlaneDiscoveryVoxelGrid[] m_VoxelGrids;

            PlaneFindingParams m_PlaneFindingParams;
            SimulationEnvironmentScanner m_SimulationEnvironmentScanner;
            float m_LastPlaneUpdateTime;
            bool m_Initialized;

            PlaneRaycaster m_PlaneRaycaster;

#if ENABLE_SIMULATION_DEBUG_VISUALS
            event Action<ReadOnlyCollection<PlaneDiscoveryVoxelGrid>> m_SubsystemInitialized;
            public event Action<ReadOnlyCollection<PlaneDiscoveryVoxelGrid>> subsystemInitialized
            {
                add
                {
                    if (m_Initialized)
                        value.Invoke(Array.AsReadOnly(m_VoxelGrids));

                    m_SubsystemInitialized += value;
                }
                remove => m_SubsystemInitialized -= value;
            }
            public event Action prePlaneUpdating;
            public event Action<IReadOnlyList<DiscoveredPlane>, IReadOnlyList<DiscoveredPlane>, IReadOnlyList<DiscoveredPlane>> postPlaneUpdating;
#endif

            protected override bool TryInitialize()
            {
                m_SimulationEnvironmentScanner = SimulationEnvironmentScanner.GetOrCreate();
                m_PlaneFindingParams = XRSimulationRuntimeSettings.Instance.planeFindingParams;

                m_LastPlaneUpdateTime = 0f;
                m_PlaneRaycaster = new PlaneRaycaster(GetPlanesReadOnly);

                BaseSimulationSceneManager.environmentSetupFinished += CreateVoxelGrids;
                SimulationSessionSubsystem.s_SimulationSessionReset += OnSimulationSessionReset;

                return true;
            }

            public override void Start()
            {
#if UNITY_EDITOR
                SimulationSubsystemAnalytics.SubsystemStarted(k_SubsystemId);
#endif
            }

            // Unused but required by parent class
            public override void Stop() { }

            public override void Destroy()
            {
                SimulationSessionSubsystem.s_SimulationSessionReset -= OnSimulationSessionReset;
                BaseSimulationSceneManager.environmentSetupFinished -= CreateVoxelGrids;

                m_PlaneRaycaster.Stop();
                m_PlaneRaycaster = null;
                m_AllPlanes.Clear();
                DestroyVoxelGrids();
            }

            void DestroyVoxelGrids()
            {
                if (m_VoxelGrids == null)
                    return;

                foreach (var voxelGrid in m_VoxelGrids)
                {
                    voxelGrid.Dispose();
                }

                m_VoxelGrids = null;
                m_Initialized = false;
            }

            public void OnSimulationSessionReset()
            {
                var removedPlanesCount = m_RemovedPlanes.Count;
                for (var i = 0; i < removedPlanesCount; i++)
                {
                    var removed = m_RemovedPlanes[i];
                    m_AllPlanes.Remove(removed.trackableId);
                }

                m_RemovedPlanes.AddRange(m_AllPlanes.Values);
                m_AddedPlanes.Clear();
                m_UpdatedPlanes.Clear();
                m_AllPlanes.Clear();

                DestroyVoxelGrids();
                CreateVoxelGrids();
                m_LastPlaneUpdateTime = 0f;
            }

            /// <inheritdoc/>
            public override PlaneDetectionMode requestedPlaneDetectionMode { get; set; }

            /// <inheritdoc/>
            public override PlaneDetectionMode currentPlaneDetectionMode => requestedPlaneDetectionMode;

            public override TrackableChanges<BoundedPlane> GetChanges(BoundedPlane defaultPlane, Allocator allocator)
            {
                if (!m_Initialized)
                    return new TrackableChanges<BoundedPlane>();

                using (new ScopedProfiler("SimulationPlaneSubsystem.GetChanges"))
                {
                    if (Time.timeSinceLevelLoad - m_LastPlaneUpdateTime > m_PlaneFindingParams.minimumPlaneUpdateTime)
                    {
                        UpdatePointCloudData();
                        FindPlanes();
                    }

                    var numAddedPlanes = m_AddedPlanes.Count;
                    var numUpdatedPlanes = m_UpdatedPlanes.Count;
                    var numRemovedPlanes = m_RemovedPlanes.Count;

                    var changes = new TrackableChanges<BoundedPlane>(numAddedPlanes, numUpdatedPlanes, numRemovedPlanes, allocator);

                    if (numAddedPlanes > 0)
                    {
                        var added = changes.added;
                        for (var i = 0; i < numAddedPlanes; i++)
                        {
                            added[i] = m_AddedPlanes[i].boundedPlane;
                        }

                        m_AddedPlanes.Clear();
                    }

                    if (numUpdatedPlanes > 0)
                    {
                        var updated = changes.updated;
                        for (var i = 0; i < numUpdatedPlanes; i++)
                        {
                            updated[i] = m_UpdatedPlanes[i].boundedPlane;
                        }

                        m_UpdatedPlanes.Clear();
                    }

                    if (numRemovedPlanes > 0)
                    {
                        var removed = changes.removed;
                        for (var i = 0; i < numRemovedPlanes; i++)
                        {
                            removed[i] = m_RemovedPlanes[i].trackableId;
                        }

                        m_RemovedPlanes.Clear();
                    }

                    return changes;
                }
            }

            public override void GetBoundary(TrackableId trackableId, Allocator allocator, ref NativeArray<Vector2> boundary)
            {
                if (!m_Initialized || !TryFindPlane(trackableId, out var plane))
                {
                    if (boundary.IsCreated)
                        boundary.Dispose();

                    return;
                }

                var vertices = plane.vertices;
#if UNITY_2022_1_OR_NEWER
                if (vertices.IsCreated)
#else
                if (vertices.Length > 0)
#endif
                {
                    CreateOrResizeNativeArrayIfNecessary(vertices.Length, allocator, ref boundary);
                    NativeArray<Vector2>.Copy(vertices, boundary);
                }
                else if (boundary.IsCreated)
                    boundary.Dispose();
            }

            bool TryFindPlane(TrackableId trackableId, out DiscoveredPlane discoveredPlane)
            {
                if (trackableId == TrackableId.invalidId || !m_AllPlanes.TryGetValue(trackableId, out discoveredPlane))
                {
                    discoveredPlane = DiscoveredPlane.defaultValue;
                    return false;
                }

                return true;
            }

            void FindPlanes()
            {
#if ENABLE_SIMULATION_DEBUG_VISUALS
                prePlaneUpdating?.Invoke();
#endif
                foreach (var voxelGrid in m_VoxelGrids)
                {
                    voxelGrid.FindPlanes(m_AddedPlanes, m_UpdatedPlanes, m_RemovedPlanes);
                }

                // Remove the plane before adding new ones to avoid a potential resize
                foreach (var plane in m_RemovedPlanes)
                {
                    m_AllPlanes.Remove(plane.trackableId);
                }

                foreach (var plane in m_AddedPlanes)
                {
                    m_AllPlanes.Add(plane.trackableId, plane);
                }

                foreach (var plane in m_UpdatedPlanes)
                {
                    m_AllPlanes[plane.trackableId] = plane;
                }

                m_LastPlaneUpdateTime = Time.timeSinceLevelLoad;
#if ENABLE_SIMULATION_DEBUG_VISUALS
                postPlaneUpdating?.Invoke(m_AddedPlanes.AsReadOnly(), m_UpdatedPlanes.AsReadOnly(), m_RemovedPlanes.AsReadOnly());
#endif
            }

            void UpdatePointCloudData()
            {
                var points = m_SimulationEnvironmentScanner.GetPoints(Allocator.Temp);
                var normals = m_SimulationEnvironmentScanner.GetNormals(Allocator.Temp);

                var upGridPoints = new NativeArray<Vector3>(points.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                var downGridPoints = new NativeArray<Vector3>(points.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                var forwardGridPoints = new NativeArray<Vector3>(points.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                var backGridPoints = new NativeArray<Vector3>(points.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                var rightGridPoints = new NativeArray<Vector3>(points.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                var leftGridPoints = new NativeArray<Vector3>(points.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

                int upPointCount = 0;
                int downPointCount = 0;
                int forwardPointCount = 0;
                int backPointCount = 0;
                int rightPointCount = 0;
                int leftPointCount = 0;

                try
                {
                    for (var i = 0; i < points.Length; i++)
                    {
                        // don't update a portion of the surfaces we get new points for each update,
                        // the idea being that for a surface to update you need shots from a few perspectives.
                        // this is one of our factors for pretending that acquiring planes is more like on device
                        if (Random.Range(0f, 1f) < m_PlaneFindingParams.pointUpdateDropoutRate)
                            continue;

                        var normal = normals[i];
                        var position = points[i];
                        bool allowHorizontal = (currentPlaneDetectionMode & PlaneDetectionMode.Horizontal) > 0;
                        bool allowVertical = (currentPlaneDetectionMode & PlaneDetectionMode.Vertical) > 0;

                        if (allowHorizontal)
                        {
                            if (Vector3.Angle(normal, Vector3.up) <= m_PlaneFindingParams.normalToleranceAngle)
                                upGridPoints[upPointCount++] = position;

                            if (Vector3.Angle(normal, Vector3.down) <= m_PlaneFindingParams.normalToleranceAngle)
                                downGridPoints[downPointCount++] = position;
                        }

                        if (allowVertical)
                        {
                            if (Vector3.Angle(normal, Vector3.forward) <= m_PlaneFindingParams.normalToleranceAngle)
                                forwardGridPoints[forwardPointCount++] = position;

                            if (Vector3.Angle(normal, Vector3.back) <= m_PlaneFindingParams.normalToleranceAngle)
                                backGridPoints[backPointCount++] = position;

                            if (Vector3.Angle(normal, Vector3.right) <= m_PlaneFindingParams.normalToleranceAngle)
                                rightGridPoints[rightPointCount++] = position;

                            if (Vector3.Angle(normal, Vector3.left) <= m_PlaneFindingParams.normalToleranceAngle)
                                leftGridPoints[leftPointCount++] = position;
                        }
                    }

                    m_VoxelGrids[0].AddPoints(upGridPoints.Slice(0, upPointCount));
                    m_VoxelGrids[1].AddPoints(downGridPoints.Slice(0, downPointCount));
                    m_VoxelGrids[2].AddPoints(forwardGridPoints.Slice(0, forwardPointCount));
                    m_VoxelGrids[3].AddPoints(backGridPoints.Slice(0, backPointCount));
                    m_VoxelGrids[4].AddPoints(rightGridPoints.Slice(0, rightPointCount));
                    m_VoxelGrids[5].AddPoints(leftGridPoints.Slice(0, leftPointCount));
                }
                finally
                {
                    upGridPoints.Dispose();
                    downGridPoints.Dispose();
                    forwardGridPoints.Dispose();
                    backGridPoints.Dispose();
                    rightGridPoints.Dispose();
                    leftGridPoints.Dispose();

                    points.Dispose();
                    normals.Dispose();
                }
            }

            /// <remarks>
            /// This method assumes the simulation environment setup is completed.
            /// </remarks>
            void CreateVoxelGrids()
            {
                if (SimulationSessionSubsystem.simulationSceneManager == null || m_Initialized)
                    return;

                var cameraMovementBounds = SimulationSessionSubsystem.simulationSceneManager.simulationEnvironment.cameraMovementBounds;
                var maximumHitDistance = XRSimulationRuntimeSettings.Instance.environmentScanParams.maximumHitDistance;
                var extents = cameraMovementBounds.extents;
                extents = new Vector3(extents.x + maximumHitDistance, extents.y + maximumHitDistance, extents.z + maximumHitDistance);
                var environmentBounds = new Bounds(cameraMovementBounds.center, extents * 2f);
                m_VoxelGrids = new[]
                {
                    new PlaneDiscoveryVoxelGrid(VoxelGridOrientation.Up, environmentBounds, m_PlaneFindingParams),
                    new PlaneDiscoveryVoxelGrid(VoxelGridOrientation.Down, environmentBounds, m_PlaneFindingParams),
                    new PlaneDiscoveryVoxelGrid(VoxelGridOrientation.Forward, environmentBounds, m_PlaneFindingParams),
                    new PlaneDiscoveryVoxelGrid(VoxelGridOrientation.Back, environmentBounds, m_PlaneFindingParams),
                    new PlaneDiscoveryVoxelGrid(VoxelGridOrientation.Right, environmentBounds, m_PlaneFindingParams),
                    new PlaneDiscoveryVoxelGrid(VoxelGridOrientation.Left, environmentBounds, m_PlaneFindingParams)
                };

                m_Initialized = true;
#if ENABLE_SIMULATION_DEBUG_VISUALS
                m_SubsystemInitialized?.Invoke(Array.AsReadOnly(m_VoxelGrids));
#endif
            }

            internal IReadOnlyDictionary<TrackableId, DiscoveredPlane> GetPlanesReadOnly()
            {
                return m_AllPlanes;
            }
        }

        internal IReadOnlyDictionary<TrackableId, DiscoveredPlane> GetPlanesReadOnly() =>
            ((SimulationProvider)provider).GetPlanesReadOnly();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            var cinfo = new XRPlaneSubsystemDescriptor.Cinfo
            {
                id = k_SubsystemId,
                providerType = typeof(SimulationPlaneSubsystem.SimulationProvider),
                subsystemTypeOverride = typeof(SimulationPlaneSubsystem),
                supportsHorizontalPlaneDetection = true,
                supportsVerticalPlaneDetection = true,
                supportsArbitraryPlaneDetection = false,
                supportsBoundaryVertices = true
            };

            XRPlaneSubsystemDescriptor.Create(cinfo);
        }
    }
}
