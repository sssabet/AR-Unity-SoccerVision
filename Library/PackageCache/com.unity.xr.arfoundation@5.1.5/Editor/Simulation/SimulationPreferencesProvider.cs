using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    class SimulationPreferencesProvider : SettingsProvider
    {
        const string k_SettingsRootTitle = "Preferences/XR Simulation";

        SerializedObject m_SerializedObject;
        Editor m_Editor;

        SimulationPreferencesProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords) {}

        [SettingsProvider]
        static SettingsProvider Create()
        {
            var provider = new SimulationPreferencesProvider(k_SettingsRootTitle, SettingsScope.User,
                new HashSet<string>(new[] {"ARF", "AR", "Simulation", "Sim", "Foundation", "XR",
                "Augmented", "Reality", "XRF"}));

            return provider;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_SerializedObject = new SerializedObject(XRSimulationPreferences.Instance);

            var scrollElement = new ScrollView(ScrollViewMode.Vertical);
            rootElement.Add(scrollElement);

            var headerElement = new Label("XR Simulation Preferences");
            headerElement.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
            headerElement.style.fontSize = new StyleLength(19f);
            headerElement.style.marginLeft = new StyleLength(8f);
            scrollElement.Add(headerElement);

            Editor.CreateCachedEditor(m_SerializedObject.targetObject, typeof(XRSimulationPreferencesEditor), ref m_Editor);
            if (m_Editor)
            {
                var editorElement = new InspectorElement(m_Editor);
                scrollElement.Add(editorElement);
            }

            rootElement.Bind(m_SerializedObject);
        }
    }
}
