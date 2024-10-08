using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Slider used for debug options in <see cref="ARDebugMenu"/>.
    /// </summary>
    public class DebugSlider: Slider
    {
        /// <summary>
        /// Use this callback to detect pointer down events.
        /// </summary>
        /// <param name="eventData">The pointer down event data.</param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if(interactable)
            {
                value = (value == 1) ? 0 : 1;
            }
        }
    }
}
