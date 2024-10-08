using System;

namespace UnityEngine.XR.Simulation
{
    [Serializable]
    class EnvironmentScanParams
    {
        [SerializeField]
        [Range(SimulationConstants.oneHundredTwentyFps, 1)]
        [Tooltip("Minimum time in seconds that must elapse between environment scans.")]
        float m_MinimumRescanTime = 0.10f;

        [SerializeField]
        [Range(0.001f, 1)]
        [Tooltip("Minimum distance in meters the camera must move before the next environment scan. "
            + "\n\nThe next environment scan will trigger on the first Update after Min Time Until Rescan has elapsed "
            + "where the camera has either moved at least Min Camera Distance or rotated at least Min Camera Rotation.")]
        float m_DeltaCameraDistanceToRescan = 0.025f;

        [SerializeField]
        [Range(0.001f, 90)]
        [Tooltip("Minimum angle change in degrees the camera must rotate before the next environment scan. "
            + "\n\nThe next environment scan will trigger on the first Update after Min Time Until Rescan has elapsed "
            + "where the camera has either moved at least Min Camera Distance or rotated at least Min Camera Rotation.")]
        float m_DeltaCameraAngleToRescan = 4f;

        [SerializeField]
        [Range(1, 9999)]
        [Tooltip("Total number of rays to cast in each environment scan. Higher values may impact system performance.")]
        int m_RaysPerCast = 10;

        [SerializeField]
        [Range(0, 9999)]
        [Tooltip("Distance in meters from the camera beyond which feature points will not be detected.")]
        float m_MaximumHitDistance = 12f;

        [SerializeField]
        [Range(0, 9999)]
        [Tooltip("Distance in meters from the camera within which feature points will not be detected.")]
        float m_MinimumHitDistance = 0.05f;

        public float minimumRescanTime
        {
            get => m_MinimumRescanTime;
            set => m_MinimumRescanTime = value;
        }

        public float deltaCameraDistanceToRescan
        {
            get => m_DeltaCameraDistanceToRescan;
            set => m_DeltaCameraDistanceToRescan = value;
        }

        public float deltaCameraAngleToRescan
        {
            get => m_DeltaCameraAngleToRescan;
            set => m_DeltaCameraAngleToRescan = value;
        }

        public int raysPerCast
        {
            get => m_RaysPerCast;
            set => m_RaysPerCast = value;
        }

        public float maximumHitDistance
        {
            get => m_MaximumHitDistance;
            set => m_MaximumHitDistance = value;
        }

        public float minimumHitDistance
        {
            get => m_MinimumHitDistance;
            set => m_MinimumHitDistance = value;
        }
    }
}
