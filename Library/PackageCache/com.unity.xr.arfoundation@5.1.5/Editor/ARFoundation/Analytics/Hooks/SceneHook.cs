using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityEditor.XR.ARFoundation
{
    [InitializeOnLoad]
    class SceneHook
    {
        static SceneHook()
        {
            EditorSceneManager.sceneSaved += OnSceneSaved;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        static void OnSceneOpened(Scene scene, OpenSceneMode openSceneMode)
        {
            SendARUsageAnalyticsEvent(ARUsageAnalyticsArgs.EventName.SceneOpen, scene);
        }

        static void OnSceneSaved(Scene scene)
        {
            SendARUsageAnalyticsEvent(ARUsageAnalyticsArgs.EventName.SceneSave, scene);
        }

        static void SendARUsageAnalyticsEvent(ARUsageAnalyticsArgs.EventName eventName, Scene scene)
        {
            AREditorAnalytics.arUsageAnalyticEvent.Send(new ARUsageAnalyticsArgs(
                eventName: eventName,
                sceneGuid: AssetDatabase.GUIDFromAssetPath(scene.path),
                arManagersInfo: ARSceneAnalysis.GetARManagersInfo(scene)));
        }
    }
}
