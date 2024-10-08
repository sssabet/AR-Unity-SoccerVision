
namespace UnityEngine.XR.Simulation
{
    struct PlaneVoxel
    {
        /// <summary>
        /// The column and row in the layer where this voxel exists.
        /// </summary>
        public Vector2Int layerCoordinates;

        /// <summary>
        /// Average local y offset of all points within this voxel.
        /// If a voxel is part of a plane, this value affects the y offset of the plane relative to the layer origin.
        /// </summary>
        public float pointYOffset;

        /// <summary>
        /// Number of points within this voxel.
        /// A voxel only contributes to a plane if its point density exceeds a minimum threshold.
        /// </summary>
        public int pointDensity;
    }
}
