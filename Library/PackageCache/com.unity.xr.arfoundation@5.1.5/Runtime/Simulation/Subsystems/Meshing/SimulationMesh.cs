using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    struct SimulationMesh
    {
        TrackableId m_TrackableId;
        string m_MeshType;
        Pose m_Pose;
        Mesh m_Mesh;

        /// <summary>
        /// The id of this mesh as determined by the provider.
        /// </summary>
        public TrackableId trackableId => m_TrackableId;

        /// <summary>
        /// The classification type of this mesh.
        /// </summary>
        public string meshType => m_MeshType;

        /// <summary>
        /// The pose of this mesh.
        /// </summary>
        public Pose pose => m_Pose;

        /// <summary>
        /// Mesh object with the vertex and triangle data for the mesh.
        /// </summary>
        public Mesh mesh => m_Mesh;

        /// <summary>
        /// Constructs a new <see cref="SimulationMesh"/>. This is just a data container
        /// for a mesh's simulation data.
        /// </summary>
        /// <param name="trackableId">The <see cref="TrackableId"/> associated with the mesh.</param>
        /// <param name="meshType">The classification type of this mesh.</param>
        /// <param name="pose">The <c>Pose</c> associated with the mesh.</param>
        /// <param name="mesh">Mesh object with the vertex and triangle data for the mesh.</param>
        public SimulationMesh(
            TrackableId trackableId,
            string meshType,
            Pose pose,
            Mesh mesh
        )
        {
            m_TrackableId = trackableId;
            m_MeshType = meshType;
            m_Pose = pose;
            m_Mesh = mesh;
        }
    }
}
