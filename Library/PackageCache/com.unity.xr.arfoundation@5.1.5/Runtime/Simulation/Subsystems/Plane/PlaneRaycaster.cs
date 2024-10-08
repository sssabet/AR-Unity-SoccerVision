using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// <see cref="IRaycaster"/> implementation for raycasting against planes in XR Simulation.
    /// </summary>
    class PlaneRaycaster : IRaycaster
    {
        readonly Func<IReadOnlyDictionary<TrackableId, DiscoveredPlane>> m_PlanesGetter;

        /// <summary>
        /// Constructs a <see cref="PlaneRaycaster"/> instance.
        /// </summary>
        /// <param name="planesGetter">
        /// Gets a <see cref="NativeArray{T}"/> of <see cref="DiscoveredPlane"/>s in session space.
        /// The getter is called once per call to <see cref="PlaneRaycaster.Raycast"/>.
        /// </param>
        public PlaneRaycaster(Func<IReadOnlyDictionary<TrackableId, DiscoveredPlane>> planesGetter)
        {
            m_PlanesGetter = planesGetter;
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
            if ((trackableTypeMask & TrackableType.Planes) == TrackableType.None)
                return default;

            var planes = m_PlanesGetter.Invoke();

            var hitBuffer = new NativeArray<XRRaycastHit>(planes.Count, Allocator.Temp);
            try
            {
                var hitCount = 0;
                foreach (var (_, plane) in planes)
                {
                    var planePosition = plane.pose.position;
                    var planeRotation = plane.pose.rotation;

                    // Perform the raycast
                    var infinitePlane = new Plane(plane.pose.up, planePosition);
                    if (!infinitePlane.Raycast(ray, out var hitDistance))
                        continue;

                    // Construct the correct output TrackableTypes, using more math when needed
                    var trackableTypes = TrackableType.None;

                    if ((trackableTypeMask & TrackableType.PlaneWithinInfinity) != 0)
                        trackableTypes |= TrackableType.PlaneWithinInfinity;

                    var hitPose = new Pose(ray.origin + ray.direction * hitDistance, planeRotation);
                    var hitPositionPlaneSpace3d = Quaternion.Inverse(planeRotation) * (hitPose.position - planePosition);
                    var hitPositionPlaneSpace = new Vector2(hitPositionPlaneSpace3d.x, hitPositionPlaneSpace3d.z);

                    if ((trackableTypeMask & TrackableType.PlaneWithinPolygon) != 0)
                    {
                        if (WindingNumber(hitPositionPlaneSpace, plane.vertices) != 0)
                            trackableTypes |= TrackableType.PlaneWithinPolygon;
                    }

                    const TrackableType estimatedOrWithinBounds = TrackableType.PlaneWithinBounds | TrackableType.PlaneEstimated;
                    if ((trackableTypeMask & estimatedOrWithinBounds) != TrackableType.None)
                    {
                        var differenceFromCenter = hitPositionPlaneSpace - plane.center;
                        if ((Mathf.Abs(differenceFromCenter.x) <= plane.extents.x) &&
                            (Mathf.Abs(differenceFromCenter.y) <= plane.extents.y))
                        {
                            trackableTypes |= (estimatedOrWithinBounds & trackableTypeMask);
                        }
                    }

                    if (trackableTypes != TrackableType.None)
                    {
                        hitBuffer[hitCount++] = new XRRaycastHit(
                            plane.trackableId,
                            hitPose,
                            hitDistance,
                            trackableTypes);
                    }
                }

                if (hitCount == 0)
                    return default;

                var hitResults = new NativeArray<XRRaycastHit>(hitCount, allocator);
                NativeArray<XRRaycastHit>.Copy(hitBuffer, hitResults, hitCount);
                return hitResults;
            }
            finally
            {
                hitBuffer.Dispose();
            }
        }

        static int WindingNumber(Vector2 positionInPlaneSpace, NativeArray<Vector2>.ReadOnly boundaryInPlaneSpace)
        {
            var windingNumber = 0;
            for (var i = 0; i < boundaryInPlaneSpace.Length; ++i)
            {
                var j = (i + 1) % boundaryInPlaneSpace.Length;
                var vi = boundaryInPlaneSpace[i];
                var vj = boundaryInPlaneSpace[j];

                if (vi.y <= positionInPlaneSpace.y)
                {
                    if (vj.y > positionInPlaneSpace.y)                                     // an upward crossing
                    {
                        if (GetCrossDirection(vj - vi, positionInPlaneSpace - vi) < 0f)    // P left of edge
                            ++windingNumber;
                    }
                    // have  a valid up intersect
                }
                else
                {                                                                     // y > P.y (no test needed)
                    if (vj.y <= positionInPlaneSpace.y)                                    // a downward crossing
                    {
                        if (GetCrossDirection(vj - vi, positionInPlaneSpace - vi) > 0f)    // P right of edge
                            --windingNumber;
                    }
                    // have  a valid down intersect
                }
            }

            return windingNumber;
        }

        static float GetCrossDirection(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
    }
}
