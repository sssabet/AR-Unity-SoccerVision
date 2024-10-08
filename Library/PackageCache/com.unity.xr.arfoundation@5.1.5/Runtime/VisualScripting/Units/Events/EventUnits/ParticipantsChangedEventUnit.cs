#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Uses an <see cref="ARParticipantManagerListener"/> to listen for a participants-changed event on the visual scripting
    /// event bus and assign the <see cref="ARParticipantsChangedEventArgs"/> to a newly triggered Flow.
    /// </summary>
    [UnitTitle("On Participants Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.Participants)]
    [TypeIcon(typeof(ARParticipantManager))]
    public sealed class ParticipantsChangedEventUnit : TrackablesChangedEventUnit<
        ARParticipantManager,
        XRParticipantSubsystem,
        XRParticipantSubsystemDescriptor,
        XRParticipantSubsystem.Provider,
        XRParticipant,
        ARParticipant,
        ARParticipantsChangedEventArgs,
        ARParticipantManagerListener>
    {
        /// <summary>
        /// The hook name.
        /// </summary>
        protected override string hookName => Constants.EventHookNames.participantsChanged;

        /// <summary>
        /// Assigns <paramref name="args"/> to <paramref name="flow"/>.
        /// </summary>
        /// <param name="flow">The flow.</param>
        /// <param name="args">The arguments.</param>
        protected override void AssignArguments(Flow flow, ARParticipantsChangedEventArgs args)
        {
            flow.SetValue(added, args.added);
            flow.SetValue(updated, args.updated);
            flow.SetValue(removed, args.removed);
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
