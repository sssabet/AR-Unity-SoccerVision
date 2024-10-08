using System;
using UnityEngine;

namespace UnityEditor.XR.Simulation
{
    [Serializable]
    struct SimulationSessionAnalyticsArgs
    {
        [SerializeField]
        private string eventName;

        [SerializeField]
        private string environmentGuid;

        [SerializeField]
        private string[] arSubsystemsInfo;

        [SerializeField]
        private double duration;

        public SimulationSessionAnalyticsArgs(EventName eventName, GUID environmentGuid, string[] arSubsystemsInfo, double duration)
        {
            this.eventName = eventName.ToString();
            this.environmentGuid = environmentGuid.ToString();
            this.arSubsystemsInfo = arSubsystemsInfo;
            this.duration = duration;
        }

        public enum EventName
        {
            SimulationEnded
        }
    }
}
