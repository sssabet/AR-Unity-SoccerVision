#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Listens to the <see cref="ARFace.updated"/> event and forwards it to the visual scripting event bus.
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public sealed class ARFaceListener : MessageListener
    {
        ARFace m_Face;

        void OnFaceUpdated(ARFaceUpdatedEventArgs args)
            => EventBus.Trigger(Constants.EventHookNames.faceUpdated, gameObject, args);

        void OnEnable()
        {
            m_Face = GetComponent<ARFace>();
            if (m_Face == null)
                throw new InvalidOperationException(
                    $"{nameof(ARFaceListener)} requires an active and enabled {nameof(ARFace)} component on"
                    + $"{name} GameObject, but none was found.");

            m_Face.updated += OnFaceUpdated;
        }

        void OnDisable()
            => m_Face.updated -= OnFaceUpdated;
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
