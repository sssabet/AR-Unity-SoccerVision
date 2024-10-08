#if VISUALSCRIPTING_1_8_OR_NEWER

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Outputs all human bodies managed by the input <see cref="ARHumanBodyManager"/>.
    /// </summary>
    [UnitTitle("Get Human Bodies")]
    [UnitCategory(Constants.k_ARFoundation_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.BodyTracking)]
    [TypeIcon(typeof(List<ARHumanBody>))]
    public sealed class GetHumanBodiesUnit : GetTrackablesUnit<
        ARHumanBodyManager,
        XRHumanBodySubsystem,
        XRHumanBodySubsystemDescriptor,
        XRHumanBodySubsystem.Provider,
        XRHumanBody,
        ARHumanBody>
    {
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
