using System.Collections.Generic;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Marks an object in a simulation environment as a source from which to provide a tracked bounded plane.
    /// This is used to provide a fully scanned plane and is not required to support plane finding in a simulation environment.
    /// </summary>
    [AddComponentMenu("")]
    class SimulatedBoundedPlane : MonoBehaviour
    {
        [SerializeField]
        Vector2 m_Center;

        [SerializeField]
        List<Vector2> m_Boundary = new();

        public Vector2 center => m_Center;

        public IReadOnlyList<Vector2> GetBoundary()
        {
            return m_Boundary;
        }
    }
}
