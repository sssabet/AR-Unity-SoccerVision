using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Provide visualizations for the planes discovered in simulation.
    /// </summary>
    [AddComponentMenu("")]
    class SimulationPlaneVisualizer: MonoBehaviour
    {
#if ENABLE_SIMULATION_DEBUG_VISUALS
        struct VisualsData
        {
            public readonly Color color;
            public readonly Vector3[] worldBoundingBox;
            public readonly List<Vector3> worldVertices;

            public VisualsData(Color color)
            {
                this.color = color;
                worldBoundingBox = new Vector3[4];
                worldVertices = new List<Vector3>();
            }
        }

        static readonly Color k_BoundingBoxColor = Color.black;
        static readonly Color k_CenterColor = Color.white;
        static readonly List<Color> k_AddedPlaneColors = new();

        readonly Dictionary<TrackableId, DiscoveredPlane> m_ProvidedPlanesById = new();
        readonly Dictionary<TrackableId, VisualsData> m_DataById = new();

        bool m_Initialized;
        CameraOffset m_CameraOffset;

        SimulationPlaneSubsystem m_Subsystem;
        ReadOnlyCollection<PlaneDiscoveryVoxelGrid> m_VoxelGrids;
        SimulationVoxelVisualizer[] m_VoxelVisualizers;

        void OnEnable()
        {
            m_Subsystem = GetActivePlaneSubsystemInstance() as SimulationPlaneSubsystem;
            if (m_Subsystem == null)
            {
                Disable($"No active {typeof(SimulationPlaneSubsystem).FullName} is available.");
                return;
            }

            var origin = FindObjectsUtility.FindObjectOfType<XROrigin>();
            if (origin == null)
            {
                Disable("Cannot find XROrigin in the scene.");
                return;
            }

            m_CameraOffset = new CameraOffset(origin.Camera);

            m_Subsystem.subsystemInitialized += SetupVoxelGrids;
        }

        void Disable(string message)
        {
            Debug.LogWarning($"{message} The debug visualization for simulated planes will be disabled.", this);
            enabled = false;
        }

        XRPlaneSubsystem GetActivePlaneSubsystemInstance()
        {
            // Query the currently active loader for the created subsystem, if one exists.
            if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
            {
                XRLoader loader = XRGeneralSettings.Instance.Manager.activeLoader;
                if (loader != null)
                    return loader.GetLoadedSubsystem<XRPlaneSubsystem>();
            }

            return null;
        }

        void SetupVoxelGrids(ReadOnlyCollection<PlaneDiscoveryVoxelGrid> voxelGrids)
        {
            if (voxelGrids == null)
            {
                Disable("No voxel grids available to visualize.");
                return;
            }

            m_VoxelGrids = voxelGrids;

            var numVoxelGrids = m_VoxelGrids.Count;
            m_VoxelVisualizers = new SimulationVoxelVisualizer[numVoxelGrids];

            for (var i = 0; i < numVoxelGrids; i++)
            {
                var voxelGrid = m_VoxelGrids[i];

                voxelGrid.planeColorAdded += AddPlaneColor;
                voxelGrid.planeMerged += SwapPlanColor;

                m_VoxelVisualizers[i] = new SimulationVoxelVisualizer(voxelGrid);
            }

            m_Subsystem.prePlaneUpdating += PrePlanesUpdate;
            m_Subsystem.postPlaneUpdating += PostPlanesUpdate;

            m_Initialized = true;
        }

        void PrePlanesUpdate()
        {
            if (m_CameraOffset == null || !m_Initialized)
                return;

            // Make sure we have debug data if debugging state changes
            foreach (var kvp in m_ProvidedPlanesById)
            {
                UpdateBounds(m_DataById[kvp.Key], kvp.Value);
            }

            foreach (var kvp in m_ProvidedPlanesById)
            {
                UpdateVertices(m_DataById[kvp.Key], kvp.Value);
            }

            k_AddedPlaneColors.Clear();
        }

        void PostPlanesUpdate(IReadOnlyList<DiscoveredPlane> addedPlanes, IReadOnlyList<DiscoveredPlane> updatedPlanes, IReadOnlyList<DiscoveredPlane> removedPlanes)
        {
            if (m_CameraOffset == null || !m_Initialized)
                return;

            for (var i = 0; i < addedPlanes.Count; i++)
            {
                var plane = addedPlanes[i];
                m_DataById.Add(plane.trackableId, new VisualsData(k_AddedPlaneColors[i]));
                m_ProvidedPlanesById.Add(plane.trackableId, plane);
            }

            foreach (var plane in updatedPlanes)
            {
                m_ProvidedPlanesById[plane.trackableId] = plane;
            }

            foreach (var plane in removedPlanes)
            {
                m_ProvidedPlanesById.Remove(plane.trackableId);
            }

            // extentsDebug
            foreach (var plane in addedPlanes)
            {
                UpdateBounds(m_DataById[plane.trackableId], plane);
            }

            foreach (var plane in updatedPlanes)
            {
                UpdateBounds(m_DataById[plane.trackableId], plane);
            }

            // verticesDebug
            foreach (var plane in addedPlanes)
            {
                UpdateVertices(m_DataById[plane.trackableId], plane);
            }

            foreach (var plane in updatedPlanes)
            {
                UpdateVertices(m_DataById[plane.trackableId], plane);
            }

            foreach (var plane in removedPlanes)
            {
                var trackableId = plane.trackableId;
                if (m_DataById.ContainsKey(trackableId))
                    m_DataById.Remove(trackableId);
            }
        }

        void AddPlaneColor(Color color)
        {
            if (!m_Initialized)
                return;

            k_AddedPlaneColors.Add(color);
        }

        void SwapPlanColor(LayerPlaneData planeDataA, LayerPlaneData planeDataB)
        {
            if (!m_Initialized)
                return;

            // We need to preserve the debug color of the valid plane
            planeDataA.color = planeDataB.color;
        }

        void UpdateBounds(VisualsData visualData, DiscoveredPlane plane)
        {
            var planePose = plane.pose;
            var center = plane.center;
            var extents = plane.extents;
            var boundsVertices = visualData.worldBoundingBox;

            boundsVertices[0] = m_CameraOffset.ApplyToPosition(planePose.ApplyOffsetTo(new Vector3(center.x, 0f, center.y) + new Vector3(-extents.x, 0f, extents.y)));
            boundsVertices[1] = m_CameraOffset.ApplyToPosition(planePose.ApplyOffsetTo(new Vector3(center.x, 0f, center.y) + new Vector3(-extents.x, 0f, -extents.y)));
            boundsVertices[2] = m_CameraOffset.ApplyToPosition(planePose.ApplyOffsetTo(new Vector3(center.x, 0f, center.y) + new Vector3(extents.x, 0f, -extents.y)));
            boundsVertices[3] = m_CameraOffset.ApplyToPosition(planePose.ApplyOffsetTo(new Vector3(center.x, 0f, center.y) + new Vector3(extents.x, 0f, extents.y)));
        }

        void UpdateVertices(VisualsData visualData, DiscoveredPlane plane)
        {
            var planePose = plane.pose;
            var worldVertices = visualData.worldVertices;
            worldVertices.Clear();

            var nativeVertices = plane.vertices;
#if UNITY_2022_1_OR_NEWER
            if (!nativeVertices.IsCreated)
#else
            if (nativeVertices.Length <= 0)
#endif
                return;

            var count = nativeVertices.Length;
            for (var i = 0; i < count; i++)
            {
                var nativeVertex = nativeVertices[i];
                var vertex = new Vector3(nativeVertex.x, 0.0f, nativeVertex.y);
                worldVertices.Add(m_CameraOffset.ApplyToPosition(planePose.ApplyOffsetTo(vertex)));
            }
        }

        void DrawPlaneVertices()
        {
            // debugPlaneVertices
            foreach (var kvp in m_DataById)
            {
                var debugData = kvp.Value;
                var vertices = debugData.worldVertices;
                var lastVertex = vertices.Count - 1;
                if (lastVertex < 0)
                    continue;

                Gizmos.color = debugData.color;
                for (var i = 0; i < lastVertex; ++i)
                {
                    Gizmos.DrawLine(vertices[i], vertices[i + 1]);
                }

                Gizmos.DrawLine(vertices[lastVertex], vertices[0]);
            }
        }

        void DrawPlaneBoundingBox()
        {
            // debugPlaneExtents
            Gizmos.color = k_BoundingBoxColor;
            foreach (var kvp in m_DataById)
            {
                var boundsVertices = kvp.Value.worldBoundingBox;
                Gizmos.DrawLine(boundsVertices[0], boundsVertices[1]);
                Gizmos.DrawLine(boundsVertices[1], boundsVertices[2]);
                Gizmos.DrawLine(boundsVertices[2], boundsVertices[3]);
                Gizmos.DrawLine(boundsVertices[3], boundsVertices[0]);
            }
        }

        void DrawPlanePolygons()
        {
            DrawPlaneBoundingBox();
            DrawPlaneVertices();
        }

        void DrawPlaneVoxels()
        {
            // SimDiscoveryVoxelsDebug
            // Set matrix since Gizmos cubes are axis-aligned
            var gizmosMatrix = Gizmos.matrix;
            // Gizmos.matrix = this.GetCameraOffsetMatrix();
            foreach (var voxelVisualizer in m_VoxelVisualizers)
            {
                voxelVisualizer.DrawVoxels();
            }

            Gizmos.matrix = gizmosMatrix;
        }

        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || !m_Initialized || m_CameraOffset == null)
                return;

            var gizmosColor = Gizmos.color;

            DrawPlaneVoxels();

            // SimDiscoveryModulePlaneCenterDebug
            Gizmos.color = k_CenterColor;
            foreach (var kvp in m_ProvidedPlanesById)
            {
                var plane = kvp.Value;
                var center = new Vector3(plane.center.x, 0, plane.center.y);
                var cameraCenter = m_CameraOffset.ApplyToPosition(plane.pose.ApplyOffsetTo(center));
                Gizmos.DrawWireSphere(cameraCenter, 0.075f);
            }

            DrawPlanePolygons();

            Gizmos.color = gizmosColor;
        }
#endif
    }
}
