using System;
using UnityEngine;

namespace UnityEditor.XR.ARFoundation
{
    [Serializable]
    struct ARUsageAnalyticsArgs
    {
        /// <summary>
        /// The actual event name which define the context of the event under a common table.
        /// </summary>
        [SerializeField]
        private string eventName;

        /// <summary>
        /// The <see cref="GUID"/> of the build with the scene specified by <see cref="sceneGuid"/>.
        /// </summary>
        [SerializeField]
        private string buildGuid;

        /// <summary>
        /// The <see cref="GUID"/> of the scene with the AR Managers listed under <see cref="arManagersInfo"/>.
        /// </summary>
        [SerializeField]
        private string sceneGuid;

        /// <summary>
        /// The target platform.
        /// </summary>
        [SerializeField]
        private string targetPlatform;

        /// <summary>
        /// List of AR Managers in the scene specified by <see cref="sceneGuid"/>.
        /// </summary>
        [SerializeField]
        private ARManagerInfo[] arManagersInfo;

        public ARUsageAnalyticsArgs(
            EventName eventName,
            GUID sceneGuid,
            ARManagerInfo[] arManagersInfo,
            GUID? buildGuid = null,
            BuildTarget? targetPlatform = null)
        {
            this.eventName = eventName.ToString();
            this.buildGuid = buildGuid.ToString();
            this.sceneGuid = sceneGuid.ToString();
            this.targetPlatform = targetPlatform?.ToString();
            this.arManagersInfo = arManagersInfo;
        }

        public enum EventName
        {
            SceneSave,
            SceneOpen,
            BuildPlayer
        }

        [Serializable]
        public struct ARManagerInfo
        {
            /// <summary>
            /// Name of the AR Manager.
            /// </summary>
            public string name;

            /// <summary>
            /// <c>True</c> if the AR Manager is active in the scene, otherwise <c>False</c>.
            /// </summary>
            public bool active;
        }
    }
}
