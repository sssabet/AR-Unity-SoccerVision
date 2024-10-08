using System;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    /// <summary>
    /// This Editor renders to the XR Plug-in Management category of the Project Settings window.
    /// </summary>
    [CustomEditor(typeof(XRSimulationSettings))]
    class XRSimulationSettingsEditor : Editor
    {
        Editor m_Editor;

        void OnEnable()
        {
            CreateCachedEditor(XRSimulationRuntimeSettings.Instance, typeof(XRSimulationRuntimeSettingsEditor), ref m_Editor);
        }

        public override void OnInspectorGUI()
        {
            m_Editor.OnInspectorGUI();
        }
    }
}
