using System;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    [Serializable]
    class EditorSimulationSceneManager : BaseSimulationSceneManager
    {
        protected override Scene CreateEnvironmentScene()
        {
            var scene = EditorSceneManager.NewPreviewScene();
            scene.name = $"Preview {GenerateUniqueSceneName()}";
            return scene;
        }

        protected override void DestroyEnvironmentScene()
        {
            if (environmentScene != default)
                EditorSceneManager.ClosePreviewScene(environmentScene);
        }

        protected override GameObject InstantiateEnvironment(GameObject environmentPrefab)
        {
            return (GameObject)PrefabUtility.InstantiatePrefab(environmentPrefab);
        }
    }
}
