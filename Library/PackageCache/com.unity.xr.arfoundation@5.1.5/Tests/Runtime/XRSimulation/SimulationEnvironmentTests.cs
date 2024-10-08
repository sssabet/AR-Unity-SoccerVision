using NUnit.Framework;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.Simulation.Tests
{
    [TestFixture]
    class SimulationEnvironmentTestFixture : SimulationSessionTestSetup
    {
        [OneTimeSetUp]
        public void Setup() => SetupSession();

        [OneTimeTearDown]
        public void TearDown() => TearDownSession();

        static Scene FindSimulationScene(string sceneName)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName)
                    return scene;
            }

            return default;
        }

        static (string sceneName, Scene environmentScene) FindSimulationSceneAndAssertLoaded()
        {
            var activeSceneName = SimulationSceneManager.activeSceneName;
            var environmentScene = FindSimulationScene(activeSceneName);

            // Check simulation scene is initialized
            Assert.AreEqual(environmentScene.name, activeSceneName);
            Assert.AreEqual(1, environmentScene.rootCount, $"{activeSceneName} should only have one root GameObject.");

            return (activeSceneName, environmentScene);
        }

        [Test]
        [Order(1)]
        public void EnvironmentLoaded()
        {
            (var sceneName, var environmentScene) = FindSimulationSceneAndAssertLoaded();

            // Check the environment root is valid
            var rootGO = environmentScene.GetRootGameObjects()[0];
            var simulationEnvironment = rootGO.GetComponent<SimulationEnvironment>();
            Assert.IsNotNull(simulationEnvironment, $"{sceneName} doesn't have a valid environment root GameObject.");
        }

        [Test]
        [Order(2)]
        public void CorrectEnvironmentPrefab()
        {
            (var _, var environmentScene) = FindSimulationSceneAndAssertLoaded();

            // Check if the environment was created from the right prefab
            // When instantiating a prefab the resulting GameObject will
            // have a name "<Prefab.name>(Clone)
            var environmentPrefab = XRSimulationPreferences.Instance.activeEnvironmentPrefab;
            var rootGO = environmentScene.GetRootGameObjects()[0];
            Assert.AreEqual($"{environmentPrefab.name}(Clone)", rootGO.name, $"\"{rootGO.name}\" root game object is not created from \"{environmentPrefab.name}\" environment prefab.");
        }
    }
}
