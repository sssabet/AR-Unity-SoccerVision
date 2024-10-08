using System.Collections.Generic;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// A structure for camera texture related information pertaining to a particular frame.
    /// </summary>
    internal struct CameraTextureFrameEventArgs
    {
        /// <summary>
        /// The time, in nanoseconds, associated with this frame.
        /// Use <c>timestampNs.HasValue</c> to determine if this data is available.
        /// </summary>
        public long? timestampNs { get; set; }

        /// <summary>
        /// Gets or sets the projection matrix for the AR Camera. Use
        /// <c>projectionMatrix.HasValue</c> to determine if this data is available.
        /// </summary>
        public Matrix4x4? projectionMatrix { get; set; }
        
        /// <summary>
        /// The textures associated with this camera frame. These are generally
        /// external textures, which exist only on the GPU. To use them on the
        /// CPU, e.g., for computer vision processing, you will need to read
        /// them back from the GPU.
        /// </summary>
        public List<Texture2D> textures { get; set; }
    }
}
