#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Listens to the <see cref="ARFaceManager.facesChanged"/> event and forwards it to the visual scripting event bus.
    /// </summary>
    /// <seealso cref="FacesChangedEventUnit"/>
    public sealed class ARFaceManagerListener : TrackableManagerListener<ARFaceManager>
    {
        void OnTrackablesChanged(ARFacesChangedEventArgs args)
            => EventBus.Trigger(Constants.EventHookNames.facesChanged, gameObject, args);

        /// <inheritdoc/>
        protected override void RegisterTrackablesChangedDelegate(ARFaceManager manager)
            => manager.facesChanged += OnTrackablesChanged;

        /// <inheritdoc/>
        protected override void UnregisterTrackablesChangedDelegate(ARFaceManager manager)
            => manager.facesChanged -= OnTrackablesChanged;
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
