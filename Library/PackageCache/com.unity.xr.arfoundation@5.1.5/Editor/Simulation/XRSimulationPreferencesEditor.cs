using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [CustomEditor(typeof(XRSimulationPreferences))]
    class XRSimulationPreferencesEditor : Editor
    {
        SerializedObject m_SerializedObject;

        public override VisualElement CreateInspectorGUI()
        {
            var rootElement = new VisualElement();

            m_SerializedObject = new SerializedObject(XRSimulationPreferences.Instance);

            var envPrefabProperty = m_SerializedObject.FindProperty("m_EnvironmentPrefab");
            var envPrefabElement = new PropertyField(envPrefabProperty);
            envPrefabElement.RegisterValueChangeCallback(PropertyChanged);
            rootElement.Add(envPrefabElement);

            var enableNavProperty = m_SerializedObject.FindProperty("m_EnableNavigation");
            var enableNavElement = new PropertyField(enableNavProperty);
            enableNavElement.RegisterValueChangeCallback(PropertyChanged);
            rootElement.Add(enableNavElement);

            rootElement.Bind(m_SerializedObject);
            return rootElement;
        }

        void PropertyChanged(SerializedPropertyChangeEvent evt)
        {
            m_SerializedObject.ApplyModifiedProperties();
        }
    }
}
