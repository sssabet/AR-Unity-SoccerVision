using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Simulation backend Singleton which registers <see cref="IRaycaster"/> implementations and forwards
    /// <c>Raycast</c> calls to them, as a helper class for the <see cref="SimulationRaycastSubsystem"/>.
    /// </summary>
    class SimulationRaycasterRegistry
    {
        static SimulationRaycasterRegistry s_Instance;

        List<IRaycaster> m_Raycasters = new();

        /// <summary>
        /// The non-null Singleton instance of this class.
        /// </summary>
        public static SimulationRaycasterRegistry instance =>
            s_Instance ??= new SimulationRaycasterRegistry();

        public ReadOnlyCollection<IRaycaster> registeredRaycasters => m_Raycasters.AsReadOnly();

        /// <summary>
        /// Private constructor for Singleton type.
        /// </summary>
        SimulationRaycasterRegistry() { }

        /// <summary>
        /// Registers <c>raycaster</c> as an <see cref="IRaycaster"/> implementation for the XR Simulation.
        /// When subsequent <c>Raycast</c> calls occur, they will be forwarded to <c>raycaster</c>,
        /// and hits will be aggregated with results from all other registered raycasters.
        /// </summary>
        /// <param name="raycaster">The <see cref="IRaycaster"/> implementation to register.</param>
        public void RegisterRaycaster(IRaycaster raycaster)
        {
            if (m_Raycasters.Contains(raycaster))
                return;

            m_Raycasters.Add(raycaster);
        }

        /// <summary>
        /// Unregisters <c>raycaster</c> as an <see cref="IRaycaster"/> implementation.
        /// </summary>
        /// <param name="raycaster">The <see cref="IRaycaster"/>implementation to unregister.</param>
        public void UnregisterRaycaster(IRaycaster raycaster)
        {
            m_Raycasters.Remove(raycaster);
        }
    }
}
