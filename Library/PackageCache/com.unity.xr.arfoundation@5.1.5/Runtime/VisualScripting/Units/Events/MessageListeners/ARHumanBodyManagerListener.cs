#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Listens to the <see cref="ARHumanBodyManager.humanBodiesChanged"/> event and forwards it to the visual scripting event bus.
    /// </summary>
    /// <seealso cref="HumanBodiesChangedEventUnit"/>
    public sealed class ARHumanBodyManagerListener : TrackableManagerListener<ARHumanBodyManager>
    {
        void OnTrackablesChanged(ARHumanBodiesChangedEventArgs args)
            => EventBus.Trigger(Constants.EventHookNames.humanBodiesChanged, gameObject, args);

        /// <inheritdoc/>
        protected override void RegisterTrackablesChangedDelegate(ARHumanBodyManager manager)
            => manager.humanBodiesChanged += OnTrackablesChanged;

        /// <inheritdoc/>
        protected override void UnregisterTrackablesChangedDelegate(ARHumanBodyManager manager)
            => manager.humanBodiesChanged -= OnTrackablesChanged;
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
