using System;
using UnityEngine;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [CustomPropertyDrawer(typeof(EnvironmentProbeParams))]
    class EnvironmentProbeParamsPropertyDrawer : PropertyDrawer
    {
        static readonly GUIContent k_MinTimeUntilUpdateLabel = new("Min Time Until Update");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            var minUpdateTime = property.FindPropertyRelative("m_MinUpdateTime");
            var maxDiscoveryDistance = property.FindPropertyRelative("m_MaxDiscoveryDistance");
            var discoveryDelayTime = property.FindPropertyRelative("m_DiscoveryDelayTime");
            var cubemapFaceSize = property.FindPropertyRelative("m_CubemapFaceSize");

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(minUpdateTime, k_MinTimeUntilUpdateLabel);
                EditorGUILayout.PropertyField(maxDiscoveryDistance);
                EditorGUILayout.PropertyField(discoveryDelayTime);
                EditorGUILayout.PropertyField(cubemapFaceSize);
                EditorGUI.indentLevel--;
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
