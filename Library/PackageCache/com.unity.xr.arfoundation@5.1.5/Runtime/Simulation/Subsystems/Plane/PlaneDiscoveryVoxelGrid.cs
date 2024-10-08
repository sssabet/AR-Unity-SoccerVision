using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;
using static UnityEngine.XR.Simulation.SimulationUtils;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// A grid of voxels that is used to find and grow planes of a specific orientation over time.
    /// Planes can be found in each individual layer of the grid based on the density of points in each voxel.
    /// </summary>
    class PlaneDiscoveryVoxelGrid : PlaneVoxelGrid
    {
#if ENABLE_SIMULATION_DEBUG_VISUALS
        public event Action prePlaneUpdating;
        public event Action postPlaneUpdating;
        public event Action<Color> planeColorAdded;
        public event Action<LayerPlaneData, LayerPlaneData> planeMerged;
#endif

        readonly PlaneAlignment m_PlaneAlignment;

        // In discovery, planes created from processing updated voxels should always be small enough that there's
        // no need to check how much empty area they take up. It is sufficient to only check empty area when merging planes.
        protected override bool CheckEmptyAreaForInitialPlanes => false;

        /// <inheritdoc/>
        public PlaneDiscoveryVoxelGrid(VoxelGridOrientation orientation, Bounds containedBounds, PlaneFindingParams planeFindingParams)
            : base(orientation, containedBounds, planeFindingParams)
        {
            m_PlaneAlignment = orientation == VoxelGridOrientation.Up ? PlaneAlignment.HorizontalUp : orientation == VoxelGridOrientation.Down ?
                PlaneAlignment.HorizontalDown : PlaneAlignment.Vertical;
        }

        public void FindPlanes(List<DiscoveredPlane> addedPlanes, List<DiscoveredPlane> updatedPlanes, List<DiscoveredPlane> removedPlanes)
        {
#if ENABLE_SIMULATION_DEBUG_VISUALS
            prePlaneUpdating?.Invoke();
#endif

            using (new ScopedProfiler("PlaneDiscoveryVoxelGrid.FindPlanes"))
            {
                FindPlanes();
            }

            for (var i = 0; i < m_Layers; i++)
            {
                var layerOrigin = GetLayerOriginPose(i);
                var modifiedLayerPlanes = m_ModifiedPlanesPerLayer[i];
                foreach (var layerPlane in modifiedLayerPlanes)
                {
                    var plane = layerPlane.plane;
                    if (plane.trackableId == TrackableId.invalidId)
                    {
                        // This layer plane doesn't have an Plane yet, so try to create one.
                        // This can fail if the plane is too small.
                        if (TryCreatePlane(layerPlane, layerOrigin))
                        {
                            addedPlanes.Add(layerPlane.plane);
#if ENABLE_SIMULATION_DEBUG_VISUALS
                            planeColorAdded?.Invoke(layerPlane.color);
#endif
                        }
                    }
                    else
                    {
                        UpdatePlane(layerPlane, layerOrigin);
                        updatedPlanes.Add(layerPlane.plane);
                    }
                }

                var removedLayerPlanes = m_RemovedPlanesPerLayer[i];
                foreach (var layerPlane in removedLayerPlanes)
                {
                    // Some layer planes get removed in merges before they have ever created a Plane
                    var plane = layerPlane.plane;
                    if (plane.trackableId != TrackableId.invalidId)
                        removedPlanes.Add(plane);
                }
            }

#if ENABLE_SIMULATION_DEBUG_VISUALS
            postPlaneUpdating?.Invoke();
#endif
        }

        protected override LayerPlaneData CreateLayerPlane(PlaneVoxel startingVoxel)
        {
            return LayerPlaneData.GetOrCreate(startingVoxel);
        }

        protected override bool TryMergePlanes(LayerPlaneData planeDataA, LayerPlaneData planeDataB, float planeAHeight, float planeBHeight)
        {
            if (!base.TryMergePlanes(planeDataA, planeDataB, planeAHeight, planeBHeight))
                return false;

            var planeA = planeDataA.plane;
            var planeB = planeDataB.plane;
            if (planeA.trackableId == TrackableId.invalidId && planeB.trackableId != TrackableId.invalidId)
            {
                // Swapping Planes helps us avoid an unnecessary plane loss event when one Plane is valid and the other is not.
                // Since it is always layer plane B that gets removed during a merge, we have to account for the case of
                // B having a valid Plane and A having an invalid one. We can avoid a loss of the valid Plane by swapping
                // the Planes. Layer plane A will update its Plane and layer plane B will be removed without a loss event for its Plane.
                planeDataB.SetPlane(ref planeA);
                planeDataA.SetPlane(ref planeB);
            }

            // Since plane B is getting subsumed by plane A and removed, the subsumed Id of plane B should be set to plane A's trackable Id
            SetSubsumedId(planeDataB, planeDataA.plane.trackableId);

#if ENABLE_SIMULATION_DEBUG_VISUALS
            planeMerged?.Invoke(planeDataA, planeDataB);
#endif

            return true;
        }

        void SetSubsumedId(LayerPlaneData subsumedPlaneData, TrackableId subsumedById)
        {
            // if the plane is subsumed by an invalid plane then there is no need to set the subsumedById
            if (subsumedById == TrackableId.invalidId)
                return;

            var plane = subsumedPlaneData.plane;
            var trackableId = plane.trackableId;

            // if the plane being subsumed is invalid then there is no need to set the subsumedById
            if (trackableId == TrackableId.invalidId)
                return;

            var vertices = plane.vertices;
#if UNITY_2022_1_OR_NEWER
            if (!vertices.IsCreated)
#else
            if (vertices.Length <= 0)
#endif
                return;

            var subsumedVertices = new NativeArray<Vector2>(vertices.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            try
            {
                NativeArray<Vector2>.Copy(vertices, subsumedVertices);

                var subsumedPlane = new DiscoveredPlane(
                    trackableId,
                    subsumedById,
                    plane.pose,
                    plane.center,
                    plane.size,
                    plane.alignment,
                    plane.trackingState,
                    plane.classification,
                    subsumedVertices);
                subsumedPlaneData.DisposeAndSetPlane(ref subsumedPlane);
            }
            finally
            {
                subsumedVertices.Dispose();
            }
        }

        bool TryCreatePlane(LayerPlaneData layerPlane, Pose layerOriginPose)
        {
            if (!TryGetPlaneData(layerPlane, layerOriginPose, out var vertices, out var pose, out var center, out var size))
                return false;

            try
            {
                var newPlane = new DiscoveredPlane(
                    GenerateTrackableId(),
                    TrackableId.invalidId,
                    pose,
                    new Vector2(center.x, center.z),
                    size,
                    m_PlaneAlignment,
                    TrackingState.Tracking,
                    PlaneClassification.None,
                    vertices);
                layerPlane.DisposeAndSetPlane(ref newPlane);
            }
            finally
            {
                vertices.Dispose();
            }

            return true;
        }

        void UpdatePlane(LayerPlaneData layerPlane, Pose layerOriginPose)
        {
            TryGetPlaneData(layerPlane, layerOriginPose, out var vertices, out var pose, out var center, out var size);

            try
            {
                var plane = layerPlane.plane;
                var newPlane = new DiscoveredPlane(
                    plane.trackableId,
                    plane.subsumedById,
                    pose,
                    new Vector2(center.x, center.z),
                    size,
                    plane.alignment,
                    plane.trackingState,
                    plane.classification,
                    vertices);
                layerPlane.DisposeAndSetPlane(ref newPlane);
            }
            finally
            {
                vertices.Dispose();
            }
        }
    }
}
