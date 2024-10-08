using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// An interface for performing raycasts against trackables.
    /// </summary>
    /// <remarks>
    /// A duplicate definition of AR Foundation's internal <c>IRaycaster</c>, but made distinct so that
    /// the AR Foundation and Simulation layers can work independently.
    /// </remarks>
    interface IRaycaster
    {
        /// <summary>
        /// Performs a raycast in session space.
        /// </summary>
        /// <param name="sessionSpaceRay">A ray, in session space.</param>
        /// <param name="trackableTypeMask">The types of raycast to perform.</param>
        /// <param name="allocator">The <see cref="Allocator"/> strategy to use for the returned array.</param>
        /// <returns>An array of raycast results in session space.</returns>
        NativeArray<XRRaycastHit> Raycast(
            Ray sessionSpaceRay,
            TrackableType trackableTypeMask,
            Allocator allocator);
    }
}
