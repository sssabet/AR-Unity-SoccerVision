using UnityEditor.XR.ARAnalytics;

namespace UnityEditor.XR.Simulation
{
    static class AREditorAnalytics
    {
        const string k_UITableName = "xrsimulation_ui";
        const int k_UIVersion = 1;

        const string k_SessionTableName = "xrsimulation_session";
        const int k_SessionVersion = 2;

        public static readonly AREditorAnalyticsEvent<SimulationUIAnalyticsArgs> simulationUIAnalyticsEvent = new(k_UITableName, k_UIVersion);
        public static readonly AREditorAnalyticsEvent<SimulationSessionAnalyticsArgs> simulationSessionAnalyticsEvent = new(k_SessionTableName, k_SessionVersion);

        [InitializeOnLoadMethod]
        static void SetupAndRegister()
        {
            // The check for whether analytics is enabled or not is already done
            // by the Editor Analytics API.
            simulationUIAnalyticsEvent.Register();
            simulationSessionAnalyticsEvent.Register();
        }
    }
}
