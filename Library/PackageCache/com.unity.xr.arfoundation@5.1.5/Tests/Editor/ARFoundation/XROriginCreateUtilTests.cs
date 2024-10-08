using NUnit.Framework;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation.InternalUtils;

namespace UnityEditor.XR.ARFoundation.Tests
{
    class XROriginCreateUtilTests
    {
        /// <summary>
        /// This is necessary for correctness because Edit Mode tests contain all GameObjects in the active scene.
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DestroyAllGameObjects();
        }

        [Test]
        public void CreateXROriginWithParent_CreatesSuccessfully()
        {
            var parent = new GameObject().transform;
            var origin = XROriginCreateUtil.CreateXROriginWithParent(parent);
            Assert.IsNotNull(origin);
            Assert.AreEqual(parent, origin.transform.parent);
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void CreateXROriginWithoutParent_CreatesSuccessfully()
        {
            var origin = XROriginCreateUtil.CreateXROriginWithParent(null);
            Assert.IsNotNull(origin);
            Assert.IsNull(origin.transform.parent);
            Object.DestroyImmediate(origin);
        }

        [Test]
        public void UndoRedo_WorksWithNoErrors()
        {
            Undo.IncrementCurrentGroup();
            var origin = XROriginCreateUtil.CreateXROriginWithParent(null);
            Assert.IsNotNull(origin);
            Undo.PerformUndo();
            origin = FindObjectsUtility.FindAnyObjectByType<XROrigin>();
            Assert.IsTrue(origin == null); // using Unity's overload for the == operator
            Undo.PerformRedo();
            origin = FindObjectsUtility.FindAnyObjectByType<XROrigin>();
            Assert.IsNotNull(origin);
            Object.DestroyImmediate(origin);
        }

        static void DestroyAllGameObjects()
        {
            foreach (var g in FindObjectsUtility.FindObjectsByType<GameObject>())
            {
                // Don't destroy GameObjects that are children within a Prefab instance
                if (g == null || (PrefabUtility.IsPartOfAnyPrefab(g) && !PrefabUtility.IsAnyPrefabInstanceRoot(g)))
                    continue;

                Object.DestroyImmediate(g);
            }
        }
    }
}
