namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Marks an object in a simulation environment with a classification type to apply to the tracked mesh
    /// generated from this object and its children.
    /// </summary>
    class SimulatedMeshClassification : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The classification type for the tracked mesh generated from this object and its children.")]
        string m_ClassificationType;

        /// <summary>
        /// The classification type for the tracked mesh generated from this object and its children.
        /// </summary>
        public string classificationType
        {
            get => m_ClassificationType;
            set => m_ClassificationType = value;
        }
    }
}
