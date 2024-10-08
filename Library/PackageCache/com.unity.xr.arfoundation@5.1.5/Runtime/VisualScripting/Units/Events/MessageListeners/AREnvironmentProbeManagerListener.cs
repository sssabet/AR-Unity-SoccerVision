#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Listens to the <see cref="AREnvironmentProbeManager.environmentProbesChanged"/> event and forwards it to the
    /// visual scripting event bus.
    /// </summary>
    /// <seealso cref="EnvironmentProbesChangedEventUnit"/>
    public sealed class AREnvironmentProbeManagerListener : TrackableManagerListener<AREnvironmentProbeManager>
    {
        void OnTrackablesChanged(AREnvironmentProbesChangedEvent args)
            => EventBus.Trigger(Constants.EventHookNames.environmentProbesChanged, gameObject, args);

        protected override void RegisterTrackablesChangedDelegate(AREnvironmentProbeManager manager)
            => manager.environmentProbesChanged += OnTrackablesChanged;

        protected override void UnregisterTrackablesChangedDelegate(AREnvironmentProbeManager manager)
            => manager.environmentProbesChanged -= OnTrackablesChanged;
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
