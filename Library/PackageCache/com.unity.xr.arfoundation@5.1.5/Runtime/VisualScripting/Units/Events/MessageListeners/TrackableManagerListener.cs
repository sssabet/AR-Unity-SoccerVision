#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using System.Text;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Listens to the trackables-changed event of <typeparamref name="TManager"/> and forwards it to the visual scripting event bus.
    /// </summary>
    /// <typeparam name="TManager">The event sender type. This should be an AR Foundation trackable manager.</typeparam>
    /// <remarks>
    /// <typeparamref name="TManager"/> is expected to be an AR Foundation trackable manager, but the current version
    /// of AR Foundation lacks a shared interface for trackables-changed events. For now the best we can do is bind
    /// to <c>MonoBehaviour</c> as a base class.
    /// </remarks>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public abstract class TrackableManagerListener<TManager> : MessageListener where TManager : MonoBehaviour
    {
        TManager m_Manager;
        StringBuilder m_LogBuilder = new();

        void OnEnable()
        {
            m_Manager = GetComponent<TManager>();
            if (m_Manager == null)
            {
#if UNITY_2023_1_OR_NEWER
                m_Manager = FindAnyObjectByType<TManager>();
#else
                m_Manager = FindObjectOfType<TManager>();
#endif
                if (m_Manager == null)
                    throw new InvalidOperationException(
                        $"{GetType().Name} requires an active and enabled {typeof(TManager).Name} in the scene, but none was found.");

                m_LogBuilder.Append($"{GetType().Name} requires an active and enabled {typeof(TManager).Name} in the scene,");
                m_LogBuilder.Append($" but but there was no such component on target GameObject {name}.");
                m_LogBuilder.Append($" A runtime search was able to locate the component on GameObject {m_Manager.gameObject.name}.");
                m_LogBuilder.Append($" For better performance, you should use {m_Manager.gameObject.name} instead of {name}");
                m_LogBuilder.Append(" as your Target input in visual scripting graphs.");
                Debug.LogWarning(m_LogBuilder.ToString());
                m_LogBuilder.Clear();
            }

            RegisterTrackablesChangedDelegate(m_Manager);
        }

        void OnDisable()
        {
            if (m_Manager != null)
                UnregisterTrackablesChangedDelegate(m_Manager);
        }

        /// <summary>
        /// Subscribes to the trackables-changed event of <paramref name="manager"/>.
        /// This is method is called in <c>OnEnable</c>.
        /// </summary>
        /// <param name="manager">The manager instance.</param>
        protected abstract void RegisterTrackablesChangedDelegate(TManager manager);

        /// <summary>
        /// Unsubscribes from the trackables-changed event of <paramref name="manager"/>.
        /// This is method is called in <c>OnDisable</c>.
        /// </summary>
        /// <param name="manager">The manager instance.</param>
        protected abstract void UnregisterTrackablesChangedDelegate(TManager manager);
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
