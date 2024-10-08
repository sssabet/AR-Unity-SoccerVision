using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

namespace UnityEditor.XR.ARFoundation
{
    class BuildHook : IProcessSceneWithReport
    {
        int IOrderedCallback.callbackOrder => 1;

        void IProcessSceneWithReport.OnProcessScene(Scene scene, BuildReport report)
        {
            if (report == null)
                return;

            AREditorAnalytics.arUsageAnalyticEvent.Send(new ARUsageAnalyticsArgs(
                eventName: ARUsageAnalyticsArgs.EventName.BuildPlayer,
                buildGuid: report.summary.guid,
                targetPlatform: report.summary.platform,
                sceneGuid: AssetDatabase.GUIDFromAssetPath(scene.path),
                arManagersInfo: ARSceneAnalysis.GetARManagersInfo(scene)));
        }
    }
}
