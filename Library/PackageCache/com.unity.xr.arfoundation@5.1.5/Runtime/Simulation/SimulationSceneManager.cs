using System;
using Unity.XR.CoreUtils;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Manages the runtime simulation scene and environment instance.
    /// </summary>
    class SimulationSceneManager : BaseSimulationSceneManager
    {
        static readonly CreateSceneParameters k_EnvironmentSceneParameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);

        protected override Scene CreateEnvironmentScene()
        {
            var scene = SceneManager.CreateScene(GenerateUniqueSceneName(), k_EnvironmentSceneParameters);
            if (!scene.IsValid())
                throw new InvalidOperationException("Environment scene could not be created.");

            return scene;
        }

        protected override void DestroyEnvironmentScene()
        {
            if (environmentScene.IsValid() && environmentScene != default)
                SceneManager.UnloadSceneAsync(environmentScene);
        }

        protected override GameObject InstantiateEnvironment(GameObject environmentPrefab)
        {
            return GameObjectUtils.Instantiate(environmentPrefab);
        }
    }
}
