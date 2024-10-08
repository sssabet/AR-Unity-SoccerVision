using System;
using UnityEngine;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [CustomPropertyDrawer(typeof(TrackedImageDiscoveryParams))]
    class TrackedImageDiscoveryParamsPropertyDrawer : PropertyDrawer
    {
        static readonly GUIContent k_MinTimeUntilUpdateLabel = new("Min Time Until Update");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            var trackingUpdateInterval = property.FindPropertyRelative("m_TrackingUpdateInterval");

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(trackingUpdateInterval, k_MinTimeUntilUpdateLabel);
                EditorGUI.indentLevel--;
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
