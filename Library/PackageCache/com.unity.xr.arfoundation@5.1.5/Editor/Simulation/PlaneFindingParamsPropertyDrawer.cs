using System;
using UnityEngine;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [CustomPropertyDrawer(typeof(PlaneFindingParams))]
    class PlaneFindingParamsPropertyDrawer : PropertyDrawer
    {
        static readonly GUIContent k_PlaneDiscoveryParamsLabel = new("Plane Discovery Params");
        static readonly GUIContent k_MinTimeUntilUpdateLabel = new("Min Time Until Update");
        static readonly GUIContent k_MinPlaneSideLengthLabel = new("Min Plane Side Length");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            var minimumPlaneUpdateTime = property.FindPropertyRelative("m_MinimumPlaneUpdateTime");
            var minPointsPerSqMeter = property.FindPropertyRelative("m_MinPointsPerSqMeter");
            var minSideLength = property.FindPropertyRelative("m_MinSideLength");
            var inLayerMergeDistance = property.FindPropertyRelative("m_InLayerMergeDistance");
            var crossLayerMergeDistance = property.FindPropertyRelative("m_CrossLayerMergeDistance");
            var checkEmptyArea = property.FindPropertyRelative("m_CheckEmptyArea");
            var allowedEmptyAreaCurve = property.FindPropertyRelative("m_AllowedEmptyAreaCurve");
            var pointUpdateDropoutRate = property.FindPropertyRelative("m_PointUpdateDropoutRate");
            var normalToleranceAngle = property.FindPropertyRelative("m_NormalToleranceAngle");
            var voxelSize = property.FindPropertyRelative("m_VoxelSize");

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, k_PlaneDiscoveryParamsLabel);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(minimumPlaneUpdateTime, k_MinTimeUntilUpdateLabel);
                EditorGUILayout.PropertyField(minPointsPerSqMeter);
                EditorGUILayout.PropertyField(minSideLength, k_MinPlaneSideLengthLabel);
                EditorGUILayout.PropertyField(inLayerMergeDistance);
                EditorGUILayout.PropertyField(crossLayerMergeDistance);
                EditorGUILayout.PropertyField(checkEmptyArea);
                EditorGUILayout.PropertyField(allowedEmptyAreaCurve);
                EditorGUILayout.PropertyField(pointUpdateDropoutRate);
                EditorGUILayout.PropertyField(normalToleranceAngle);
                EditorGUILayout.PropertyField(voxelSize);
                EditorGUI.indentLevel--;
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
