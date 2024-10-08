using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.Simulation
{
    [Serializable]
    abstract class BaseSimulationSceneManager
    {
        const string k_EnvironmentSceneName = "Simulated Environment Scene";

        [SerializeField]
        Scene m_EnvironmentScene;

        [SerializeField]
        GameObject m_EnvironmentRoot;

        [SerializeField]
        SimulationEnvironment m_SimulationEnvironment;

        static bool s_Initialized;
        static event Action s_EnvironmentSetupFinished;
        static string s_ActiveSceneName;

        internal static string activeSceneName => s_ActiveSceneName;

        public static event Action environmentSetupFinished
        {
            add
            {
                // check and invoke the callback immediately if the manager is
                // already initialized but still add it to the event in case the
                // environment is setup again after a teardown
                if (s_Initialized)
                {
                    value.Invoke();
                }

                s_EnvironmentSetupFinished += value;
            }

            remove => s_EnvironmentSetupFinished -= value;
        }

        public static event Action environmentTeardownStarted;

        public Scene environmentScene
        {
            get => m_EnvironmentScene;
            protected set => m_EnvironmentScene = value;
        }

        public SimulationEnvironment simulationEnvironment => m_SimulationEnvironment;

        // Local method use only -- created here to reduce garbage collection. Collections must be cleared before use
        // Reference type collections must also be cleared after use
        static readonly List<Light> k_SimulationLights = new();

        protected static string GenerateUniqueSceneName() => $"{k_EnvironmentSceneName} {Guid.NewGuid().ToString()}";

        /// <summary>
        /// Setup a simulation environment based on the current Simulation Settings.
        /// </summary>
        public void SetupEnvironment()
        {
            if (m_EnvironmentScene.IsValid() && m_EnvironmentScene != default)
                TearDownEnvironment();

            m_EnvironmentScene = CreateEnvironmentScene();

            var prefab = XRSimulationPreferences.Instance.activeEnvironmentPrefab;
            if (prefab == null)
                throw new InvalidOperationException("No environment prefab set. Pick an environment in a Scene View with Overlays -> XR Environment.");

            m_EnvironmentRoot = InstantiateEnvironment(prefab);
            m_SimulationEnvironment = m_EnvironmentRoot.GetComponentInChildren<SimulationEnvironment>();
            SceneManager.MoveGameObjectToScene(m_EnvironmentRoot, m_EnvironmentScene);

            EnsureLightMasking(m_EnvironmentRoot);

            m_EnvironmentRoot.SetLayerRecursively(XRSimulationRuntimeSettings.Instance.environmentLayer);
            m_EnvironmentRoot.SetHideFlagsRecursively(HideFlags.HideInHierarchy);

            s_Initialized = true;
            s_ActiveSceneName = m_EnvironmentScene.name;
            s_EnvironmentSetupFinished?.Invoke();
        }

        /// <summary>
        /// Destroy the current simulation environment.
        /// </summary>
        public void TearDownEnvironment()
        {
            environmentTeardownStarted?.Invoke();
            s_Initialized = false;

            if (m_EnvironmentRoot != null)
                UnityObjectUtils.Destroy(m_EnvironmentRoot);

            m_EnvironmentRoot = null;
            m_SimulationEnvironment = null;

            DestroyEnvironmentScene();

            m_EnvironmentScene = default;
            s_ActiveSceneName = null;
        }

        protected abstract Scene CreateEnvironmentScene();
        protected abstract void DestroyEnvironmentScene();
        protected abstract GameObject InstantiateEnvironment(GameObject environmentPrefab);

        static void EnsureLightMasking(GameObject root)
        {
            root.GetComponentsInChildren(k_SimulationLights);
            foreach (var simulationLight in k_SimulationLights)
            {
                simulationLight.cullingMask = 1 << XRSimulationRuntimeSettings.Instance.environmentLayer;
            }

            k_SimulationLights.Clear();
        }
    }
}
