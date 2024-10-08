using System;

namespace UnityEngine.XR.Simulation
{
    [Serializable]
    class PlaneFindingParams
    {
        [SerializeField]
        [Range(SimulationConstants.oneHundredTwentyFps, 1)]
        [Tooltip("Minimum time in seconds that must elapse between plane discovery updates.")]
        float m_MinimumPlaneUpdateTime = 0.13f;

        [SerializeField]
        [Range(1, 9999)]
        [Tooltip("Voxel point density, independent of voxel size.")]
        int m_MinPointsPerSqMeter = 30;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("A plane with x or y size less than this value in meters will be ignored.")]
        float m_MinSideLength = 0.11f;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Planes within the same layer that are at most this distance in meters from each other will be merged.")]
        float m_InLayerMergeDistance = 0.2f;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Planes in adjacent layers with an elevation difference of at most this distance in meters will be merged.")]
        float m_CrossLayerMergeDistance = 0.05f;

        [SerializeField]
        [Tooltip("When enabled, planes will only be created if they do not contain too much empty area.")]
        bool m_CheckEmptyArea;

        [SerializeField]
        [Tooltip("Curve that maps the area of a plane to the ratio of area that is allowed to be empty.")]
        AnimationCurve m_AllowedEmptyAreaCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Probability of dropping per-plane updates. If a random number between 0 and 1 is below this number, the update is dropped.")]
        float m_PointUpdateDropoutRate = 0.4f;

        [SerializeField]
        [Range(0, 90)]
        [Tooltip("If the angle between a point's normal and a voxel grid direction is within this range, the point is added to the grid.")]
        float m_NormalToleranceAngle = 15f;

        [SerializeField]
        [Range(.001f, 1)]
        [Tooltip("Side length of each voxel in the plane voxel grid.")]
        float m_VoxelSize = 0.1f;

        public float minimumPlaneUpdateTime => m_MinimumPlaneUpdateTime;

        public int minPointsPerSqMeter => m_MinPointsPerSqMeter;

        public float minSideLength => m_MinSideLength;

        public float inLayerMergeDistance => m_InLayerMergeDistance;

        public float crossLayerMergeDistance => m_CrossLayerMergeDistance;

        public bool checkEmptyArea => m_CheckEmptyArea;

        public AnimationCurve allowedEmptyAreaCurve => m_AllowedEmptyAreaCurve;

        public float pointUpdateDropoutRate => m_PointUpdateDropoutRate;

        public float normalToleranceAngle => m_NormalToleranceAngle;

        public float voxelSize => m_VoxelSize;
    }
}
