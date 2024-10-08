#if VISUALSCRIPTING_1_8_OR_NEWER

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Outputs all tracked objects managed by the input <see cref="ARTrackedObjectManager"/>.
    /// </summary>
    [UnitTitle("Get Tracked Objects")]
    [UnitCategory(Constants.k_ARFoundation_Unit_Category)]
    [UnitOrder((int) Constants.ARFoundationFeatureOrder.ObjectTracking)]
    [TypeIcon(typeof(List<ARTrackedObject>))]
    public sealed class GetTrackedObjectsUnit : GetTrackablesUnit<
        ARTrackedObjectManager,
        XRObjectTrackingSubsystem,
        XRObjectTrackingSubsystemDescriptor,
        XRObjectTrackingSubsystem.Provider,
        XRTrackedObject,
        ARTrackedObject>
    {
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
