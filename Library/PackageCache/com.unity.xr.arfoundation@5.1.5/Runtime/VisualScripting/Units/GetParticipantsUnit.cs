#if VISUALSCRIPTING_1_8_OR_NEWER

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Outputs all participants managed by the input <see cref="ARParticipantManager"/>.
    /// </summary>
    [UnitTitle("Get Participants")]
    [UnitCategory(Constants.k_ARFoundation_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.Participants)]
    [TypeIcon(typeof(List<ARParticipant>))]
    public sealed class GetParticipantsUnit : GetTrackablesUnit<
        ARParticipantManager,
        XRParticipantSubsystem,
        XRParticipantSubsystemDescriptor,
        XRParticipantSubsystem.Provider,
        XRParticipant,
        ARParticipant>
    {
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
