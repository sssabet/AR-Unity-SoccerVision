using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [CustomEditor(typeof(XRSimulationRuntimeSettings))]
    class XRSimulationRuntimeSettingsEditor : Editor
    {
        const string k_UnnamedName = "(Unnamed)";
        const string k_ReservedName = "{0} (Reserved)";
        const string k_NamedLayerName = "{0} - {1}";
        const float k_MinLabelWidth = 220f;

        SerializedObject m_SerializedObject;
        GUIContent[] m_LayersContent;
        SerializedProperty m_EnvLayerProperty;
        SerializedProperty m_EnvScanParamsProperty;
        SerializedProperty m_ImageDiscoveryPramsProperty;
        SerializedProperty m_PlaneFindingParamsProperty;
        SerializedProperty m_ProbesParamsProperty;
        SerializedProperty m_AnchorParamsProperty;

        void OnEnable()
        {
            m_SerializedObject = new SerializedObject(XRSimulationRuntimeSettings.Instance);
            m_EnvLayerProperty = m_SerializedObject.FindProperty("m_EnvironmentLayer");
            m_EnvScanParamsProperty = m_SerializedObject.FindProperty("m_EnvironmentScanParams");
            m_ImageDiscoveryPramsProperty = m_SerializedObject.FindProperty("m_TrackedImageDiscoveryParams");
            m_PlaneFindingParamsProperty = m_SerializedObject.FindProperty("m_PlaneFindingParams");
            m_ProbesParamsProperty = m_SerializedObject.FindProperty("m_EnvironmentProbeDiscoveryParams");
            m_AnchorParamsProperty = m_SerializedObject.FindProperty("m_AnchorDiscoveryParams");
        }

        public override void OnInspectorGUI()
        {
            LayerContentsUpdate();
            m_SerializedObject.Update();

            using (var change = new EditorGUI.ChangeCheckScope())
            {
                var layerIndex = m_EnvLayerProperty.intValue;
                var activeIndexOffset = layerIndex - SimulationConstants.firstValidLayer;
                var selectedIndex = activeIndexOffset;
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel(new GUIContent(m_EnvLayerProperty.displayName, m_EnvLayerProperty.tooltip));

                    selectedIndex = EditorGUILayout.Popup(selectedIndex, m_LayersContent);
                }

                // Prevent fields with long names from getting cut off
                var previousLabelWidth = EditorGUIUtility.labelWidth;
                if (previousLabelWidth < k_MinLabelWidth)
                    EditorGUIUtility.labelWidth = k_MinLabelWidth;

                EditorGUILayout.PropertyField(m_EnvScanParamsProperty);
                EditorGUILayout.PropertyField(m_PlaneFindingParamsProperty);
                EditorGUILayout.PropertyField(m_ImageDiscoveryPramsProperty);
                EditorGUILayout.PropertyField(m_ProbesParamsProperty);
                EditorGUILayout.PropertyField(m_AnchorParamsProperty);

                EditorGUIUtility.labelWidth = previousLabelWidth;

                if (change.changed)
                {
                    if (selectedIndex != activeIndexOffset)
                    {
                        layerIndex = selectedIndex + SimulationConstants.firstValidLayer;
                        m_EnvLayerProperty.intValue = layerIndex;
                    }

                    m_SerializedObject.ApplyModifiedProperties();
                }
            }
        }

        void LayerContentsUpdate()
        {
            m_LayersContent = new GUIContent[SimulationConstants.validLayerCount];
            for (var i = 0; i < SimulationConstants.validLayerCount; i++)
            {
                m_LayersContent[i] = GetLayerIndexGUIContent(i + SimulationConstants.firstValidLayer);
            }
        }

        static GUIContent GetLayerIndexGUIContent(int index)
        {
            var layerName = LayerMask.LayerToName(index);
            if (string.IsNullOrEmpty(layerName))
                layerName = k_UnnamedName;

            if (SimulationConstants.reservedLayers.Contains(index))
                layerName = string.Format(k_ReservedName, layerName);

            var name = string.Format(k_NamedLayerName, index.ToString(), layerName);
            return new GUIContent(name);
        }
    }
}
