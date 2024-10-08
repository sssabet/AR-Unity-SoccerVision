using System.Collections.Generic;

namespace UnityEngine.XR.Simulation
{
    class LayerPlaneData
    {
        static readonly Stack<LayerPlaneData> k_Pool = new();

        // Voxels already contains one element--increase capacity to 32
        const int k_VoxelPreWarmCount = 31;
        const int k_InitialPoolSize = 32;
        const int k_InitialVertexCapacity = 16;

        readonly List<Vector3> m_Vertices;
        readonly HashSet<Vector2Int> m_Voxels;

        float m_YOffsetFromLayer;
        bool m_CrossLayer;

        DiscoveredPlane m_Plane;

        /// <summary>
        /// XZ vertices of the plane, relative to the layer origin.
        /// </summary>
        public List<Vector3> vertices => m_Vertices;

        /// <summary>
        /// Coordinates of voxels that contribute to this plane.
        /// </summary>
        public HashSet<Vector2Int> voxels => m_Voxels;

        /// <summary>
        /// Y offset of the plane, relative to the layer origin.
        /// </summary>
        public float yOffsetFromLayer
        {
            get => m_YOffsetFromLayer;
            set => m_YOffsetFromLayer = value;
        }

        /// <summary>
        /// Is this plane included in the layer above it due to a cross-layer merge?
        /// </summary>
        public bool crossLayer
        {
            get => m_CrossLayer;
            set => m_CrossLayer = value;
        }

        /// <summary>
        /// The plane associated with this plane data.
        /// </summary>
        public DiscoveredPlane plane => m_Plane;

#if ENABLE_SIMULATION_DEBUG_VISUALS
        Color m_Color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);

        public Color color
        {
            get => m_Color;
            set => m_Color = value;
        }
#endif

        static LayerPlaneData()
        {
            var allocated = new List<LayerPlaneData>();
            for (var i = 0; i < k_InitialPoolSize; i++)
            {
                var data = GetOrCreate(default);
                allocated.Add(data);
                for (var j = 0; j < k_VoxelPreWarmCount; j++)
                {
                    data.voxels.Add(new Vector2Int(i,j));
                }
            }
            foreach (var item in allocated)
            {
                item.Recycle();
            }
        }

        protected LayerPlaneData(PlaneVoxel startingVoxel)
        {
            m_Voxels = new HashSet<Vector2Int>
            {
                startingVoxel.layerCoordinates
            };
            m_YOffsetFromLayer = startingVoxel.pointYOffset;
            m_Vertices = new List<Vector3>(k_InitialVertexCapacity);
            m_Plane = DiscoveredPlane.defaultValue;
        }

        internal virtual void Reset(PlaneVoxel startingVoxel)
        {
            voxels.Clear();
            voxels.Add(startingVoxel.layerCoordinates);
            m_YOffsetFromLayer = startingVoxel.pointYOffset;
            vertices.Clear();
            m_CrossLayer = false;
        }

        public static LayerPlaneData GetOrCreate(PlaneVoxel startingVoxel)
        {
            if (k_Pool.Count > 0)
            {
                var data = k_Pool.Pop();
                data.Reset(startingVoxel);
                return data;
            }

            return new LayerPlaneData(startingVoxel);
        }

        public void AddVoxel(PlaneVoxel voxel)
        {
            var numVoxelsInPlane = voxels.Count;
            voxels.Add(voxel.layerCoordinates);
            m_YOffsetFromLayer = (yOffsetFromLayer * numVoxelsInPlane + voxel.pointYOffset) / voxels.Count;
        }

        internal virtual void Recycle()
        {
            if (m_Plane != DiscoveredPlane.defaultValue)
            {
                m_Plane.Dispose();
                m_Plane = DiscoveredPlane.defaultValue;
            }

            k_Pool.Push(this);
        }

        /// <summary>
        /// Dispose the current plane and set the new plane.
        /// </summary>
        /// <param name="newPlane">The plane to set to this layer</param>
        public void DisposeAndSetPlane(ref DiscoveredPlane newPlane)
        {
            DisposePlane();
            SetPlane(ref newPlane);
        }

        /// <summary>
        /// Set the new plane without disposing the current plane. This should be
        /// used with caution because it will not dispose the previous plane which could lead
        /// to memory leaks. When possible, use <see cref="DisposeAndSetPlane"/> instead.
        /// </summary>
        /// <param name="newPlane">The plane to set to this layer</param>
        public void SetPlane(ref DiscoveredPlane newPlane)
        {
            m_Plane = newPlane;
        }

        /// <summary>
        /// Dispose the current plane if it is not the default value.
        /// </summary>
        public void DisposePlane()
        {
            if (m_Plane == DiscoveredPlane.defaultValue)
                return;

            m_Plane.Dispose();
        }
    }
}
