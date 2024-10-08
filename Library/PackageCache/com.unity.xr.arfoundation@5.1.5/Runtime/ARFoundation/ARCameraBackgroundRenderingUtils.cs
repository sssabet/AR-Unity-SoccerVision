namespace UnityEngine.XR.ARFoundation
{
    internal static class ARCameraBackgroundRenderingUtils
    {
        static bool s_InitializedFarClipMesh;
        static Mesh s_FarClipMesh;

        internal static Mesh fullScreenFarClipMesh
        {
            get
            {
                if (!s_InitializedFarClipMesh)
                {
                    s_FarClipMesh = BuildFullscreenMesh(-1f);
                    s_InitializedFarClipMesh = s_FarClipMesh != null;
                }

                return s_FarClipMesh;
            }
        }

        static bool s_InitializedNearClipMesh;
        static Mesh s_NearClipMesh;

        /// <summary>
        /// A mesh that is placed near the near-clip plane
        /// </summary>
        internal static Mesh fullScreenNearClipMesh
        {
            get
            {
                if (!s_InitializedNearClipMesh)
                {
                    s_NearClipMesh = BuildFullscreenMesh(0.1f);
                    s_InitializedNearClipMesh = s_NearClipMesh != null;
                }

                return s_NearClipMesh;
            }
        }

        static Mesh BuildFullscreenMesh(float zVal)
        {
            const float bottomV = 0f;
            const float topV = 1f;
            var mesh = new Mesh
            {
                vertices = new Vector3[]
                {
                    new Vector3(0f, 0f, zVal),
                    new Vector3(0f, 1f, zVal),
                    new Vector3(1f, 1f, zVal),
                    new Vector3(1f, 0f, zVal),
                },
                uv = new Vector2[]
                {
                    new Vector2(0f, bottomV),
                    new Vector2(0f, topV),
                    new Vector2(1f, topV),
                    new Vector2(1f, bottomV),
                },
                triangles = new int[] { 0, 1, 2, 0, 2, 3 }
            };

            mesh.UploadMeshData(false);
            return mesh;
        }

        /// <summary>
        /// The orthogonal projection matrix for the before opaque background rendering. For use when drawing the
        /// <see cref="fullScreenNearClipMesh"/>.
        /// </summary>
        internal static Matrix4x4 beforeOpaquesOrthoProjection { get; } = Matrix4x4.Ortho(0f, 1f, 0f, 1f, -0.1f, 9.9f);

        /// <summary>
        /// The orthogonal projection matrix for the after opaque background rendering. For use when drawing the
        /// <see cref="fullScreenFarClipMesh"/>.
        /// </summary>
        internal static Matrix4x4 afterOpaquesOrthoProjection { get; } = Matrix4x4.Ortho(0, 1, 0, 1, 0, 1);
    }
}
