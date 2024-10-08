#if ENABLE_SIMULATION_DEBUG_VISUALS
using System.Collections.Generic;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Provide visualizations for the voxels in a voxel grid discovered in simulation.
    /// </summary>
    class SimulationVoxelVisualizer
    {
        struct VoxelData
        {
            public Color color;
            public Vector3 center;
            public Vector3 yOffsetCenter;
        }

        static readonly Color k_DebugUnusedVoxelsColor = new Color(1f, 1f, 1f, 0.3f);

        const float k_DebugVoxelMargin = 0.01f;
        const float k_DebugYOffsetCubeThickness = 0.005f;

        readonly Vector3 m_DebugVoxelSize;
        readonly Vector3 m_DebugVoxelYOffsetSize;
        readonly Dictionary<Vector3Int, VoxelData> m_DebugUpdatedVoxels = new();

        readonly PlaneDiscoveryVoxelGrid m_VoxelGrid;
        readonly float m_VoxelSize;

        public SimulationVoxelVisualizer(PlaneDiscoveryVoxelGrid voxelGrid)
        {
            m_VoxelGrid = voxelGrid;
            m_VoxelSize = voxelGrid.voxelSize;

            m_DebugVoxelSize = Vector3.one * (m_VoxelSize - k_DebugVoxelMargin * 2f);
            m_DebugVoxelYOffsetSize = m_DebugVoxelSize;
            switch (voxelGrid.orientation)
            {
                case VoxelGridOrientation.Up:
                case VoxelGridOrientation.Down:
                    m_DebugVoxelYOffsetSize.y = k_DebugYOffsetCubeThickness;
                    break;
                case VoxelGridOrientation.Forward:
                case VoxelGridOrientation.Back:
                    m_DebugVoxelYOffsetSize.z = k_DebugYOffsetCubeThickness;
                    break;
                case VoxelGridOrientation.Right:
                case VoxelGridOrientation.Left:
                    m_DebugVoxelYOffsetSize.x = k_DebugYOffsetCubeThickness;
                    break;
            }

            m_VoxelGrid.prePlaneUpdating += PrePlaneUpdate;
            m_VoxelGrid.postPlaneUpdating += PostPlaneUpdate;
        }

        void PrePlaneUpdate()
        {
            m_DebugUpdatedVoxels.Clear();
            for (var i = 0; i < m_VoxelGrid.layers; i++)
            {
                var layerOrigin = m_VoxelGrid.GetLayerOriginPose(i);
                var updatedVoxels = m_VoxelGrid.GetUpdatedVoxelsPerLayer(i);
                foreach (var voxel in updatedVoxels)
                {
                    AddVoxelData(i, voxel, layerOrigin);
                }
            }
        }

        void PostPlaneUpdate()
        {
            // Debug colors for voxels must be assigned at the end since the planes voxels belong to can change during merges
            AssignColors();
        }

        public void DrawVoxels()
        {
            foreach (var kvp in m_DebugUpdatedVoxels)
            {
                var debugData = kvp.Value;
                Gizmos.color = debugData.color;
                Gizmos.DrawWireCube(debugData.center, m_DebugVoxelSize);
                Gizmos.DrawCube(debugData.yOffsetCenter, m_DebugVoxelYOffsetSize);
            }
        }

        void AssignColors()
        {
            for (var i = 0; i < m_VoxelGrid.layers; i++)
            {
                var layerPlanes = m_VoxelGrid.GetModifiedPlanesPerLayer(i);
                foreach (var layerPlane in layerPlanes)
                {
                    foreach (var voxel in layerPlane.voxels)
                    {
                        var voxelKey = new Vector3Int(i, voxel.x, voxel.y);
                        if (m_DebugUpdatedVoxels.TryGetValue(voxelKey, out var voxelData))
                        {
                            m_DebugUpdatedVoxels[voxelKey] = new VoxelData
                            {
                                center = voxelData.center,
                                color = layerPlane.color,
                                yOffsetCenter = voxelData.yOffsetCenter
                            };
                        }
                    }

                    if (!layerPlane.crossLayer)
                        continue;

                    var nextLayer = i + 1;
                    foreach (var voxel in layerPlane.voxels)
                    {
                        var voxelKey = new Vector3Int(nextLayer, voxel.x, voxel.y);
                        if (m_DebugUpdatedVoxels.TryGetValue(voxelKey, out var voxelData))
                        {
                            m_DebugUpdatedVoxels[voxelKey] = new VoxelData
                            {
                                center = voxelData.center,
                                color = layerPlane.color,
                                yOffsetCenter = voxelData.yOffsetCenter
                            };
                        }
                    }
                }
            }
        }

        void AddVoxelData(int layer, Vector2Int layerCoordinates, Pose layerOriginPose)
        {
            var column = layerCoordinates.x;
            var row = layerCoordinates.y;
            var voxel = m_VoxelGrid.GetVoxel(layer, column, row);
            var rotation = layerOriginPose.rotation;
            var center = layerOriginPose.position + rotation * new Vector3(column * m_VoxelSize, 0f, row * m_VoxelSize);
            var voxelData = new VoxelData
            {
                center = center,
                yOffsetCenter = center + rotation * (Vector3.up * voxel.pointYOffset),
                color = k_DebugUnusedVoxelsColor
            };
            m_DebugUpdatedVoxels.Add(new Vector3Int(layer, column, row), voxelData);
        }
    }
}
#endif
