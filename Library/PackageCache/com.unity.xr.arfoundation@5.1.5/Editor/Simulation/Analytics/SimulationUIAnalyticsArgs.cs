using System;
using UnityEngine;

namespace UnityEditor.XR.Simulation
{
    [Serializable]
    struct SimulationUIAnalyticsArgs
    {
        [SerializeField]
        private string eventName;

        [SerializeField]
        private WindowUsed windowUsed;

        [SerializeField]
        private string environmentGuid;

        public SimulationUIAnalyticsArgs(EventName eventName, WindowUsed windowUsed = null, GUID? environmentGuid = null)
        {
            this.eventName = eventName.ToString();
            this.windowUsed = windowUsed;
            this.environmentGuid = environmentGuid?.ToString();
        }

        public enum EventName
        {
            WindowUsed,
            EnvironmentCycle
        }

        [Serializable]
        public class WindowUsed
        {
            public string name;
            public bool isActive;
        }
    }
}
