#if UNITY_EDITOR
using System.Collections.Generic;

namespace UnityEngine.XR.Simulation
{
    static class SimulationSubsystemAnalytics
    {
        static readonly HashSet<string> s_StartedSubsystems = new();

        public static IEnumerable<string> StartedSubsystems => s_StartedSubsystems;

        public static void SubsystemStarted(string id)
        {
            s_StartedSubsystems.Add(id);
        }

        public static void ClearStartedSubsystems()
        {
            s_StartedSubsystems.Clear();
        }
    }
}
#endif
