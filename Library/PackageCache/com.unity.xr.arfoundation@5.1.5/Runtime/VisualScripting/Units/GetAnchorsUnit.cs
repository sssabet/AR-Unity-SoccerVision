#if VISUALSCRIPTING_1_8_OR_NEWER

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Outputs all anchors managed by the input <see cref="ARAnchorManager"/>.
    /// </summary>
    [UnitTitle("Get Anchors")]
    [UnitCategory(Constants.k_ARFoundation_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.Anchors)]
    [TypeIcon(typeof(List<ARAnchor>))]
    public sealed class GetAnchorsUnit : GetTrackablesUnit<
        ARAnchorManager,
        XRAnchorSubsystem,
        XRAnchorSubsystemDescriptor,
        XRAnchorSubsystem.Provider,
        XRAnchor,
        ARAnchor>
    {
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
