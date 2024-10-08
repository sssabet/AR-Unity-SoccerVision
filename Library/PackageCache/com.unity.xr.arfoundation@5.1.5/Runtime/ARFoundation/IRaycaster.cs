using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// An interface for performing raycasts against trackables. Generally implemented by 
    /// derived classes of <see cref="ARTrackableManager{TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable}"/>.
    /// </summary>
    internal interface IRaycaster
    {
        /// <summary>
        /// Performs a raycast.
        /// </summary>
        /// <param name="sessionSpaceRay">A ray, in session space.</param>
        /// <param name="trackableTypeMask">The types of raycast to perform.</param>
        /// <param name="allocator">The type of memory allocation to use for the array returned by this raycast.</param>
        /// <returns>An array of raycast results.</returns>
        NativeArray<XRRaycastHit> Raycast(
            Ray sessionSpaceRay,
            TrackableType trackableTypeMask,
            Allocator allocator);
    }
}
