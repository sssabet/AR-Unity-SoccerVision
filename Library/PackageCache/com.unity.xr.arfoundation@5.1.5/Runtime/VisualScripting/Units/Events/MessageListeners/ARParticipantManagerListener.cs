#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Listens to the <see cref="ARParticipantManager.participantsChanged"/> event and forwards it to the visual scripting event bus.
    /// </summary>
    /// <seealso cref="ParticipantsChangedEventUnit"/>
    public sealed class ARParticipantManagerListener : TrackableManagerListener<ARParticipantManager>
    {
        void OnTrackablesChanged(ARParticipantsChangedEventArgs args)
            => EventBus.Trigger(Constants.EventHookNames.participantsChanged, gameObject, args);

        /// <inheritdoc/>
        protected override void RegisterTrackablesChangedDelegate(ARParticipantManager manager)
            => manager.participantsChanged += OnTrackablesChanged;

        /// <inheritdoc/>
        protected override void UnregisterTrackablesChangedDelegate(ARParticipantManager manager)
            => manager.participantsChanged -= OnTrackablesChanged;
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
