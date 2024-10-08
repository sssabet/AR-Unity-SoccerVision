using System;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityEditor.XR.ARFoundation
{
    static class CreateUtils
    {
        /// <summary>
        /// Makes <paramref name="gameObject"/> a child of <paramref name="parent"/> with no offset position or rotation.
        /// If <paramref name="parent"/> is null, moves <paramref name="gameObject"/> to the scene pivot, or instead to
        /// the world origin if there is also no Scene view.
        /// </summary>
        /// <param name="gameObject">The GameObject to place.</param>
        /// <param name="parent">Optional GameObject to use as a parent of <paramref name="gameObject"/>.</param>
        public static void Place(GameObject gameObject, Transform parent = null)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException(nameof(gameObject));
            }

            var transform = gameObject.transform;

            if (parent != null)
            {
                ResetTransform(transform);
                Undo.SetTransformParent(transform, parent, "Reparenting");
                ResetTransform(transform);
                gameObject.layer = parent.gameObject.layer;
            }
            else
            {
                // Puts it at the scene pivot, and otherwise world origin if there is no Scene view.
                var view = SceneView.lastActiveSceneView;
                if (view != null)
                    view.MoveToView(transform);
                else
                    transform.position = Vector3.zero;

                StageUtility.PlaceGameObjectInCurrentStage(gameObject);
            }

            GameObjectUtility.EnsureUniqueNameForSibling(gameObject);
        }

        static void ResetTransform(Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            if (transform is RectTransform rectTransform)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = Vector2.zero;
            }
        }
    }
}
