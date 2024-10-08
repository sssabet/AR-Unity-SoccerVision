#if VISUALSCRIPTING_1_8_OR_NEWER

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Outputs all raycasts managed by the input <see cref="ARRaycastManager"/>.
    /// </summary>
    [UnitTitle("Get Raycasts")]
    [UnitCategory(Constants.k_ARFoundation_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.Raycasts)]
    [TypeIcon(typeof(List<ARRaycast>))]
    public sealed class GetRaycastsUnit : GetTrackablesUnit<
        ARRaycastManager,
        XRRaycastSubsystem,
        XRRaycastSubsystemDescriptor,
        XRRaycastSubsystem.Provider,
        XRRaycast,
        ARRaycast>
    {
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
