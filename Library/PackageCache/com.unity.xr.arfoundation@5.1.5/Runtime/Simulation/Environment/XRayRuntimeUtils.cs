using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace UnityEngine.XR.Simulation
{
    static class XRayRuntimeUtils
    {
        static readonly Dictionary<Scene, XRayRegion> m_XRayRegions = new();

        /// <summary>
        /// Dictionary of scenes that contain an XRay Region.
        /// </summary>
        public static IReadOnlyDictionary<Scene, XRayRegion> xRayRegions => m_XRayRegions;

        /// <summary>
        /// Assign an XRay Region to be the active region for the region's scene.
        /// </summary>
        /// <param name="xRayRegion">XRay Region to assign to be used the the XRayModule.</param>
        public static void AssignXRayRegion(XRayRegion xRayRegion)
        {
            var scene = xRayRegion.gameObject.scene;
            if (!scene.IsValid())
                return;

            if (!m_XRayRegions.TryGetValue(scene, out var cachedRegion))
            {
                m_XRayRegions.Add(scene, xRayRegion);
            }
            else if (cachedRegion != null)
            {
                Debug.LogWarning(
                    $"{scene.name} already has a XRay Region on {cachedRegion.gameObject.name}. Replacing with new region on {xRayRegion.gameObject.name}");

                m_XRayRegions[scene] = xRayRegion;
            }
            else
            {
                m_XRayRegions[scene] = xRayRegion;
            }
        }

        /// <summary>
        /// Remove a XRay Region from use.
        /// </summary>
        /// <param name="xRayRegion">XRay Region to remove from use.</param>
        public static void RemoveXRayRegion(XRayRegion xRayRegion)
        {
            var scene = xRayRegion.gameObject.scene;
            if (!scene.IsValid())
                return;

            if (m_XRayRegions.TryGetValue(scene, out var cachedRegion))
            {
                if (cachedRegion == xRayRegion)
                    m_XRayRegions.Remove(scene);
            }
        }
    }
}
