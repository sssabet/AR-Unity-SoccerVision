using UnityEngine;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [CustomEditor(typeof(SimulationEnvironment))]
    class SimulationEnvironmentEditor : Editor
    {
        SerializedProperty m_CameraStartingPoseProp;
        SerializedProperty m_CameraStartingPositionProp;
        SerializedProperty m_CameraStartingRotationProp;
        SerializedProperty m_CameraMovementBoundsProp;
        SerializedProperty m_CameraMovementCenterProp;
        SerializedProperty m_CameraMovementExtentsProp;
        SerializedProperty m_DefaultViewPoseProp;
        SerializedProperty m_DefaultViewPositionProp;
        SerializedProperty m_DefaultViewRotationProp;
        SerializedProperty m_DefaultViewPivotProp;
        SerializedProperty m_DefaultViewSizeProp;
        SerializedProperty m_RenderSettingsProp;
        SerializedProperty m_ExcludeFromSelectionUIProp;

        bool m_ShowCameraStartingPose;
        bool m_ShowMovementBounds;
        bool m_ShowDefaultViewPose;

        void OnEnable()
        {
            m_CameraStartingPoseProp = serializedObject.FindProperty("m_CameraStartingPose");
            m_CameraStartingPositionProp = m_CameraStartingPoseProp.FindPropertyRelative("position");
            m_CameraStartingRotationProp = m_CameraStartingPoseProp.FindPropertyRelative("rotation");
            m_CameraMovementBoundsProp = serializedObject.FindProperty("m_CameraMovementBounds");
            m_CameraMovementCenterProp = m_CameraMovementBoundsProp.FindPropertyRelative("m_Center");
            m_CameraMovementExtentsProp = m_CameraMovementBoundsProp.FindPropertyRelative("m_Extent");
            m_DefaultViewPoseProp = serializedObject.FindProperty("m_DefaultViewPose");
            m_DefaultViewPositionProp = m_DefaultViewPoseProp.FindPropertyRelative("position");
            m_DefaultViewRotationProp = m_DefaultViewPoseProp.FindPropertyRelative("rotation");
            m_DefaultViewPivotProp = serializedObject.FindProperty("m_DefaultViewPivot");
            m_DefaultViewSizeProp = serializedObject.FindProperty("m_DefaultViewSize");
            m_RenderSettingsProp = serializedObject.FindProperty("m_RenderSettings");
            m_ExcludeFromSelectionUIProp = serializedObject.FindProperty("m_ExcludeFromSelectionUI");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            m_ShowCameraStartingPose = EditorGUILayout.Foldout(m_ShowCameraStartingPose, m_CameraStartingPoseProp.displayName);
            if (m_ShowCameraStartingPose)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_CameraStartingPositionProp);
                EditorGUILayout.PropertyField(m_CameraStartingRotationProp);
                EditorGUI.indentLevel--;
            }

            m_ShowMovementBounds = EditorGUILayout.Foldout(m_ShowMovementBounds, m_CameraMovementBoundsProp.displayName);
            if (m_ShowMovementBounds)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_CameraMovementCenterProp);
                m_CameraMovementExtentsProp.vector3Value = EditorGUILayout.Vector3Field(
                    "Extents", m_CameraMovementExtentsProp.vector3Value);
                EditorGUI.indentLevel--;
            }

            m_ShowDefaultViewPose = EditorGUILayout.Foldout(m_ShowDefaultViewPose, m_DefaultViewPoseProp.displayName);
            if (m_ShowDefaultViewPose)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_DefaultViewPositionProp);
                EditorGUILayout.PropertyField(m_DefaultViewRotationProp);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(m_DefaultViewPivotProp);
            EditorGUILayout.PropertyField(m_DefaultViewSizeProp);
            EditorGUILayout.PropertyField(m_RenderSettingsProp);
            EditorGUILayout.PropertyField(m_ExcludeFromSelectionUIProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
