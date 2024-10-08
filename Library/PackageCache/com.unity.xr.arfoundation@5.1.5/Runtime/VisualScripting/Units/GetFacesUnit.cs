#if VISUALSCRIPTING_1_8_OR_NEWER

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Outputs all faces managed by the input <see cref="ARFaceManager"/>.
    /// </summary>
    [UnitTitle("Get Faces")]
    [UnitCategory(Constants.k_ARFoundation_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.FaceTracking)]
    [TypeIcon(typeof(List<ARFace>))]
    public sealed class GetFacesUnit : GetTrackablesUnit<
        ARFaceManager,
        XRFaceSubsystem,
        XRFaceSubsystemDescriptor,
        XRFaceSubsystem.Provider,
        XRFace,
        ARFace>
    {
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
