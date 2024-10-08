namespace UnityEditor.XR.Simulation
{
    static class EnvironmentChangeHook
    {
        [InitializeOnLoadMethod]
        static void RegisterForEnvironmentChange()
        {
            // Do not access EditorScriptableSettings.Instance during InitializeOnLoad because it can fail to find the
            // existing settings asset on first import, which causes lots of issues down the line
            EditorApplication.delayCall += () =>
            {
                var environmentAssetsManager = SimulationEnvironmentAssetsManager.Instance;
                environmentAssetsManager.activeEnvironmentChanged += OnEnvironmentChange;
            };
        }

        static void OnEnvironmentChange()
        {
            AREditorAnalytics.simulationUIAnalyticsEvent.Send(
                new SimulationUIAnalyticsArgs(
                    eventName: SimulationUIAnalyticsArgs.EventName.EnvironmentCycle,
                    environmentGuid: SimulationEnvironmentAssetsManager.GetActiveEnvironmentAssetGuid()));
        }
    }
}
