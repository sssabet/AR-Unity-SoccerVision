#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Listens to the <see cref="ARPlaneManager.planesChanged"/> event and forwards it to the visual scripting event bus.
    /// </summary>
    /// <seealso cref="PlanesChangedEventUnit"/>
    public sealed class ARPlaneManagerListener : TrackableManagerListener<ARPlaneManager>
    {
        void OnTrackablesChanged(ARPlanesChangedEventArgs args)
            => EventBus.Trigger(Constants.EventHookNames.planesChanged, gameObject, args);

        /// <inheritdoc/>
        protected override void RegisterTrackablesChangedDelegate(ARPlaneManager manager)
            => manager.planesChanged += OnTrackablesChanged;

        /// <inheritdoc/>
        protected override void UnregisterTrackablesChangedDelegate(ARPlaneManager manager)
            => manager.planesChanged -= OnTrackablesChanged;
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
