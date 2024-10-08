using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Script that handles button highlighting in menus when button is selected.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class ToolButton : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The list of other highlight images for the buttons in the menu.")]
        List<Image> m_buttonHighlights;

        /// <summary>
        /// The list of other highlight images for the buttons in the menu.
        /// </summary>
        /// <value>
        /// A list of other highlight images for the buttons in the menu.
        /// </value>
        public List<Image> buttonHighlights
        {
            get => m_buttonHighlights;
            set => m_buttonHighlights = value;
        }
        
        void Start()
        {
            m_Image = GetComponent<Image>();
        }

        /// <summary>
        /// Method that shows or hides the image that highlights the button.
        /// </summary>
        public void HighlightButton()
        {
            if(m_Image.enabled)
            {
                m_Image.enabled = false;
            }
            else
            {
                foreach (var highlightImage in m_buttonHighlights)
                {
                    highlightImage.enabled = false;
                }
                m_Image.enabled = true;
            }
        }
        Image m_Image;
    }
}
