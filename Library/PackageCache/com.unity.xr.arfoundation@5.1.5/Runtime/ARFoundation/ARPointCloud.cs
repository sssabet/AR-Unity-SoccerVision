using System;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Represents a detected point cloud. These are also known as feature points.
    /// </summary>
    [DefaultExecutionOrder(ARUpdateOrder.k_PointCloud)]
    [DisallowMultipleComponent]
    [HelpURL(typeof(ARPointCloud))]
    public class ARPointCloud : ARTrackable<XRPointCloud, ARPointCloud>
    {
        XRPointCloudData m_Data;
        bool m_PointsUpdated;

        /// <summary>
        /// Invoked whenever the point cloud is updated.
        /// </summary>
        public event Action<ARPointCloudUpdatedEventArgs> updated;

        /// <summary>
        /// An array of positions for each point in the point cloud.
        /// This array is parallel to <see cref="identifiers"/> and
        /// <see cref="confidenceValues"/>. Positions are provided in
        /// point cloud space, that is, relative to this <see cref="ARPointCloud"/>'s
        /// local position and rotation.
        /// </summary>
        public NativeSlice<Vector3>? positions
        {
            get
            {
                if (m_Data.positions.IsCreated)
                {
                    return m_Data.positions;
                }

                return null;
            }
        }

        /// <summary>
        /// An array of identifiers for each point in the point cloud.
        /// This array is parallel to <see cref="positions"/> and
        /// <see cref="confidenceValues"/>.
        /// </summary>
        public NativeSlice<ulong>? identifiers
        {
            get
            {
                if (m_Data.identifiers.IsCreated)
                {
                    return m_Data.identifiers;
                }

                return null;
            }
        }

        /// <summary>
        /// An array of confidence values for each point in the point cloud
        /// ranging from 0..1.
        /// This array is parallel to <see cref="positions"/> and
        /// <see cref="identifiers"/>. Check for existence with
        /// <c>confidenceValues.IsCreated</c>.
        /// </summary>
        public NativeArray<float>? confidenceValues
        {
            get
            {
                if (m_Data.confidenceValues.IsCreated)
                {
                    return m_Data.confidenceValues;
                }

                return null;
            }
        }

        void Update()
        {
            if (m_PointsUpdated && updated != null)
            {
                m_PointsUpdated = false;
                updated(new ARPointCloudUpdatedEventArgs());
            }
        }

#if UNITY_EDITOR
        void Awake()
        {
            AssemblyReloadEvents.beforeAssemblyReload += DisposeNativeContainers;
        }
#endif

        void OnDestroy()
        {
            DisposeNativeContainers();
#if UNITY_EDITOR
            AssemblyReloadEvents.beforeAssemblyReload -= DisposeNativeContainers;
#endif
        }

        void DisposeNativeContainers()
        {
            m_Data.Dispose();
        }

        internal void UpdateData(XRPointCloudSubsystem subsystem)
        {
            m_Data.Dispose();
            m_Data = subsystem.GetPointCloudData(trackableId, Allocator.Persistent);
            m_PointsUpdated = m_Data.positions.IsCreated;
        }
    }
}
