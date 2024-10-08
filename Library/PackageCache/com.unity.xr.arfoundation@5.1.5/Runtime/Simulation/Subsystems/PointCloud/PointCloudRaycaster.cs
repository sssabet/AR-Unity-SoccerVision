using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// <see cref="IRaycaster"/> implementation for raycasting against a point cloud in XR Simulation.
    /// </summary>
    class PointCloudRaycaster : IRaycaster
    {
        const float k_RaycastAngleInRadians = Mathf.Deg2Rad * 5f;
        static float s_CosineThreshold = Mathf.Cos(k_RaycastAngleInRadians * .5f);

        TrackableId m_PointCloudTrackableId;
        Func<NativeArray<Vector3>.ReadOnly> m_PointsGetter;

        /// <summary>
        /// Constructs a <see cref="PointCloudRaycaster"/> instance using a point cloud's <see cref="TrackableId"/> and
        /// a readonly getter for its point positions.
        /// </summary>
        /// <param name="pointCloudTrackableId">The <see cref="TrackableId"/> of the point cloud.</param>
        /// <param name="pointsGetter">Gets a <see cref="NativeArray{T}.ReadOnly"/> of points in the point cloud.</param>
        public PointCloudRaycaster(TrackableId pointCloudTrackableId, Func<NativeArray<Vector3>.ReadOnly> pointsGetter)
        {
            m_PointCloudTrackableId = pointCloudTrackableId;
            m_PointsGetter = pointsGetter;
            SimulationRaycasterRegistry.instance.RegisterRaycaster(this);
        }

        /// <summary>
        /// Unregister this raycaster from the <see cref="SimulationRaycasterRegistry"/> so that it is not
        /// included for future raycasts. For AR subsystem providers, this would typically be called as part of
        /// <see cref="ISubsystem.Stop"/>.
        /// </summary>
        public void Stop()
        {
            SimulationRaycasterRegistry.instance.UnregisterRaycaster(this);
        }

        /// <summary>
        /// Simulation point cloud implementation for the <see cref="IRaycaster"/> interface.
        /// </summary>
        /// <param name="ray">A <see cref="Ray"/>, in session space.</param>
        /// <param name="trackableTypeMask">The type(s) of trackables to raycast against.
        /// If the <c>TrackableType.FeaturePoint</c> flag is not set, this method returns <c>default</c>.</param>
        /// <param name="allocator">The allocator to use for the returned <see cref="NativeArray{T}"/>.</param>
        /// <returns>
        /// A new <see cref="NativeArray{T}"/>, allocated using <paramref name="allocator"/>, containing
        /// all <see cref="XRRaycastHit"/>s from the raycast.
        /// </returns>
        public NativeArray<XRRaycastHit> Raycast(
            Ray ray,
            TrackableType trackableTypeMask,
            Allocator allocator)
        {
            if (!trackableTypeMask.HasFlag(TrackableType.FeaturePoint))
                return default;

            var points = m_PointsGetter.Invoke();

            using(new ScopedProfiler("XRSimulationPointCloudRaycasting"))
            {
                // Perform the raycast against each point
                var infos = new NativeArray<PointCloudRaycastInfo>(points.Length, Allocator.TempJob);
                var raycastJob = new PointCloudRaycastJob
                {
                    points = points,
                    ray = ray,
                    infoOut = infos
                };
                var raycastHandle = raycastJob.Schedule(infos.Length, 1);
                raycastHandle.Complete();

                // Collect the hits
                var hitBuffer = new NativeArray<XRRaycastHit>(infos.Length, Allocator.TempJob);
                var numHits = 0;
                for (var i = 0; i < points.Length; ++i)
                {
                    if (infos[i].cosineAngleWithRay >= s_CosineThreshold)
                    {
                        hitBuffer[numHits++] = new XRRaycastHit(
                            m_PointCloudTrackableId,
                            new Pose(points[i], Quaternion.identity),
                            infos[i].distance,
                            TrackableType.FeaturePoint);
                    }
                }

                // Copy out the results
                var hitsArray = new NativeArray<XRRaycastHit>(numHits, allocator);
                NativeArray<XRRaycastHit>.Copy(hitBuffer, 0, hitsArray, 0, numHits);

                infos.Dispose();
                hitBuffer.Dispose();
                return hitsArray;
            }
        }

        struct PointCloudRaycastInfo
        {
            public float distance;
            public float cosineAngleWithRay;
        }

        struct PointCloudRaycastJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<Vector3>.ReadOnly points;

            [WriteOnly]
            public NativeArray<PointCloudRaycastInfo> infoOut;

            [ReadOnly]
            public Ray ray;

            public void Execute(int i)
            {
                var originToPoint = points[i] - ray.origin;
                var distance = originToPoint.magnitude;
                infoOut[i] = new PointCloudRaycastInfo
                {
                    distance = distance,
                    cosineAngleWithRay = Vector3.Dot(originToPoint, ray.direction) / distance
                };
            }
        }
    }
}
