using NUnit.Framework;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

namespace UnityEditor.XR.ARFoundation.Tests
{
    [TestFixture]
    class ARMeshManagerTests
    {
        [Test]
        public void CanBeUsedInPrefab()
        {
            var arSessionGameObject =
                ObjectFactory.CreateGameObject("XR Origin", typeof(XROrigin));
            var arMeshManagerGameObject = ObjectFactory.CreateGameObject("Meshing");
            arMeshManagerGameObject.transform.SetParent(arSessionGameObject.transform);
            arMeshManagerGameObject.AddComponent<ARMeshManager>();

            var prefab = PrefabUtility.SaveAsPrefabAsset(
                arSessionGameObject,
                "Assets/MeshingPrefab.prefab");

            Assert.IsTrue(prefab != null);
            var meshManager = prefab.GetComponentInChildren<ARMeshManager>(true);
            Assert.IsTrue(meshManager != null);
            Assert.IsTrue(meshManager.IsValid());

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prefab));
        }
    }
}
