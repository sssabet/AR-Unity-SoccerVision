using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [InitializeOnLoad]
    static class SimulationEnvironmentLayerNameCheck
    {
        const float k_CoroutineFrequencySeconds = 1f;
        const string k_DesiredLayerName = "XR Simulation";
        const string k_TagManagerAssetPath = "ProjectSettings/TagManager.asset";
        const string k_SerializedPropertyName = "layers";

        static bool s_HasWarningBeenShownThisSession;
        static bool s_HasInitialized;
        static int s_EnvironmentLayerNum;

        static SerializedObject s_TagManager;
        static SerializedProperty s_LayersProperty;

        static SimulationEnvironmentLayerNameCheck()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(SetSimulationEnvironmentLayerNameCoroutine());
        }

        static IEnumerator SetSimulationEnvironmentLayerNameCoroutine()
        {
            var waitObj = new EditorWaitForSeconds(k_CoroutineFrequencySeconds);

            // It is unsafe to access XRSimulationRuntimeSettings.Instances within InitializeOnLoad
            EditorApplication.delayCall += EditorDelayCallback;

            yield return new WaitUntil(() => s_HasInitialized);

            s_TagManager = new SerializedObject(AssetDatabase.LoadAssetAtPath<Object>(k_TagManagerAssetPath));
            s_LayersProperty = s_TagManager.FindProperty(k_SerializedPropertyName);

            while (true)
            {
                if (XRManagerUtility.IsLoaderActive<SimulationLoader>())
                    SetSimulationEnvironmentLayerName();

                yield return waitObj;
            }

            // ReSharper disable once IteratorNeverReturns -- coroutine runs forever
        }

        static void EditorDelayCallback() => s_HasInitialized = true;

        static void SetSimulationEnvironmentLayerName()
        {
            var settingsInstance = XRSimulationRuntimeSettings.Instance;
            if (settingsInstance == null)
                return;

            s_TagManager.Update();

            var environmentLayer = settingsInstance.environmentLayer;
            if (environmentLayer != s_EnvironmentLayerNum)
            {
                var previousLayerName = LayerMask.LayerToName(s_EnvironmentLayerNum);
                if (previousLayerName == k_DesiredLayerName)
                {
                    s_LayersProperty.GetArrayElementAtIndex(s_EnvironmentLayerNum).stringValue = string.Empty;
                    s_HasWarningBeenShownThisSession = false;
                }
            }

            var environmentLayerName = LayerMask.LayerToName(environmentLayer);
            if (environmentLayerName != k_DesiredLayerName)
            {
                if (string.IsNullOrEmpty(environmentLayerName))
                {
                    s_LayersProperty.GetArrayElementAtIndex(environmentLayer).stringValue = k_DesiredLayerName;
                }
                else if (!s_HasWarningBeenShownThisSession)
                {
                    Debug.LogWarning($"Layer {environmentLayer} is currently named \"{environmentLayerName}\", " +
                        "and conflicts with XR Simulation's environment layer. Consider moving " +
                        "XR Simulation to a different layer by going to " +
                        "<b>Project Settings</b> > <b>XR Plug-in Management</b> > <b>XR Simulation</b>.");

                    s_HasWarningBeenShownThisSession = true;
                }
            }

            s_TagManager.ApplyModifiedProperties();
            s_EnvironmentLayerNum = environmentLayer;
        }
    }
}
