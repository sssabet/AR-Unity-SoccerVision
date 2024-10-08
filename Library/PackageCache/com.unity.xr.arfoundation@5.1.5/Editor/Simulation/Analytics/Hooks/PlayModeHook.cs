using System;
using System.Linq;
using UnityEngine.XR.Management;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    static class PlayModeHook
    {
        static DateTime s_StartTime;

        [InitializeOnEnterPlayMode]
        static void OnPlayModeBegan()
        {
            s_StartTime = DateTime.UtcNow;
            SimulationSubsystemAnalytics.ClearStartedSubsystems();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (stateChange != PlayModeStateChange.ExitingPlayMode)
                return;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

            if (!Api.loaderPresent)
                return;

            var environmentGuid = SimulationEnvironmentAssetsManager.GetActiveEnvironmentAssetGuid().ToString();
            if (string.IsNullOrEmpty(environmentGuid))
                return;

            if (SimulationSubsystemAnalytics.StartedSubsystems.Count() == 0)
                return;

            var subsystemIds = SimulationSubsystemAnalytics.StartedSubsystems.ToArray();

            var durationSpan = DateTime.UtcNow - s_StartTime;

            AREditorAnalytics.simulationSessionAnalyticsEvent.Send(
                new SimulationSessionAnalyticsArgs(
                    eventName: SimulationSessionAnalyticsArgs.EventName.SimulationEnded,
                    environmentGuid: new GUID(environmentGuid),
                    arSubsystemsInfo: subsystemIds,
                    duration: durationSpan.TotalSeconds));
        }
    }
}
