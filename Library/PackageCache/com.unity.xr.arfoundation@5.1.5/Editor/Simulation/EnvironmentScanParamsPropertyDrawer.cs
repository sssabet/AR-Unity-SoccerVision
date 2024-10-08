using System;
using UnityEngine;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [CustomPropertyDrawer(typeof(EnvironmentScanParams))]
    class EnvironmentScanParamsPropertyDrawer : PropertyDrawer
    {
        static readonly GUIContent k_MinTimeUntilRescanLabel = new("Min Time Until Rescan");
        static readonly GUIContent k_MinCameraDistanceUntilRescanLabel = new("Min Camera Distance Until Rescan");
        static readonly GUIContent k_MinCameraRotationUntilRescanLabel = new("Min Camera Rotation Until Rescan");
        static readonly GUIContent k_RaycastsPerScanLabel = new("Raycasts Per Scan");
        static readonly GUIContent k_MaxRaycastHitDistanceLabel = new("Max Raycast Hit Distance");
        static readonly GUIContent k_MinRaycastHitDistanceLabel = new("Min Raycast Hit Distance");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            var minimumRescanTime = property.FindPropertyRelative("m_MinimumRescanTime");
            var deltaCameraDistanceToRescan = property.FindPropertyRelative("m_DeltaCameraDistanceToRescan");
            var deltaCameraAngleToRescan = property.FindPropertyRelative("m_DeltaCameraAngleToRescan");
            var raysPerCast = property.FindPropertyRelative("m_RaysPerCast");
            var maximumHitDistance = property.FindPropertyRelative("m_MaximumHitDistance");
            var minimumHitDistance = property.FindPropertyRelative("m_MinimumHitDistance");

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(minimumRescanTime, k_MinTimeUntilRescanLabel);
                EditorGUILayout.PropertyField(deltaCameraDistanceToRescan, k_MinCameraDistanceUntilRescanLabel);
                EditorGUILayout.PropertyField(deltaCameraAngleToRescan, k_MinCameraRotationUntilRescanLabel);
                EditorGUILayout.PropertyField(raysPerCast, k_RaycastsPerScanLabel);
                EditorGUILayout.PropertyField(maximumHitDistance, k_MaxRaycastHitDistanceLabel);
                EditorGUILayout.PropertyField(minimumHitDistance, k_MinRaycastHitDistanceLabel);
                EditorGUI.indentLevel--;
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
