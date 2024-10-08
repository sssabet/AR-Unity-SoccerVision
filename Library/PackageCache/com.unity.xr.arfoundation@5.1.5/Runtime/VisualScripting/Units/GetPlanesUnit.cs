#if VISUALSCRIPTING_1_8_OR_NEWER

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Outputs all planes managed by the input <see cref="ARPlaneManager"/>.
    /// </summary>
    [UnitTitle("Get Planes")]
    [UnitCategory(Constants.k_ARFoundation_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.PlaneDetection)]
    [TypeIcon(typeof(List<ARPlane>))]
    public sealed class GetPlanesUnit : GetTrackablesUnit<
        ARPlaneManager,
        XRPlaneSubsystem,
        XRPlaneSubsystemDescriptor,
        XRPlaneSubsystem.Provider,
        BoundedPlane,
        ARPlane>
    {
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
