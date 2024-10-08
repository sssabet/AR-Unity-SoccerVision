#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Listens to the <see cref="ARAnchorManager.anchorsChanged"/> event and forwards it to the visual scripting event bus.
    /// </summary>
    /// <seealso cref="AnchorsChangedEventUnit"/>
    public sealed class ARAnchorManagerListener : TrackableManagerListener<ARAnchorManager>
    {
        void OnTrackablesChanged(ARAnchorsChangedEventArgs args)
            => EventBus.Trigger(Constants.EventHookNames.anchorsChanged, gameObject, args);

        /// <inheritdoc/>
        protected override void RegisterTrackablesChangedDelegate(ARAnchorManager manager)
            => manager.anchorsChanged += OnTrackablesChanged;

        /// <inheritdoc/>
        protected override void UnregisterTrackablesChangedDelegate(ARAnchorManager manager)
            => manager.anchorsChanged -= OnTrackablesChanged;
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
