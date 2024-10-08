#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Listens to the <see cref="ARTrackedObjectManager.trackedObjectsChanged"/> event and forwards it to the visual scripting event bus.
    /// </summary>
    /// <seealso cref="TrackedObjectsChangedEventUnit"/>
    public sealed class ARTrackedObjectManagerListener : TrackableManagerListener<ARTrackedObjectManager>
    {
        void OnTrackablesChanged(ARTrackedObjectsChangedEventArgs args)
            => EventBus.Trigger(Constants.EventHookNames.trackedObjectsChanged, gameObject, args);

        /// <inheritdoc/>
        protected override void RegisterTrackablesChangedDelegate(ARTrackedObjectManager manager)
            => manager.trackedObjectsChanged += OnTrackablesChanged;

        /// <inheritdoc/>
        protected override void UnregisterTrackablesChangedDelegate(ARTrackedObjectManager manager)
            => manager.trackedObjectsChanged -= OnTrackablesChanged;
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
