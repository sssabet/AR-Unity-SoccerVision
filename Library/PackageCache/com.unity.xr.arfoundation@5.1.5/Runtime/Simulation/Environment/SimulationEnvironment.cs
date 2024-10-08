using System;
using Unity.XR.CoreUtils;
#if INCLUDE_POST_PROCESSING
using UnityEngine.Rendering.PostProcessing;
#else
using PostProcessProfile = UnityEngine.ScriptableObject;
#endif

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Contains metadata for the simulation environment.
    /// </summary>
    [DisallowMultipleComponent]
    class SimulationEnvironment : MonoBehaviour
    {
        // By default start 1.5 meters off the ground, 1 meter back, facing forward
        static readonly Pose k_DefaultPose = new(new Vector3(0f, 1.5f, -1f), Quaternion.identity);

        static readonly Vector3[] k_CameraPoints = new Vector3[]
        {
            // camera vertices
            //     c1               c6 ________ c10        +y
            //      /|\              /|       /|           |  +x
            //     / |  \           / |      / |           | /
            // c0 /__|____\ c4  c5 /__|_____/c9|     +z____|/
            //    |c2| _- /        |c7|_____|__| c11
            //    |  /  /          |  /     |  /
            //    | / /            | /      | /
            // c3 |/            c8 |/_______|/ c12

            // pyramid verts
            new(-1.0f, 1.0f, 2.0f),
            new(1.0f, 1.0f, 2.0f),
            new(1.0f, -1.0f, 2.0f),
            new(-1.0f, -1.0f, 2.0f),
            new(0.0f, 0.0f, 1.0f),
            // box verts
            new(-1.0f, 1.0f, 1.0f),
            new(1.0f, 1.0f, 1.0f),
            new(1.0f, -1.0f, 1.0f),
            new(-1.0f, -1.0f, 1.0f),
            new(-1.0f, 1.0f, -1.0f),
            new(1.0f, 1.0f, -1.0f),
            new(1.0f, -1.0f, -1.0f),
            new(-1.0f, -1.0f, -1.0f)
        };

        [SerializeField]
        [Tooltip("Initial camera pose that the simulation will provide.")]
        Pose m_CameraStartingPose = k_DefaultPose;

        [SerializeField]
        [Tooltip("Bounds within which to restrict simulation camera movement.")]
        Bounds m_CameraMovementBounds;

        [SerializeField]
        Pose m_DefaultViewPose = k_DefaultPose;

        [SerializeField]
        [Tooltip("Default world pivot of the scene camera for this environment scene")]
        Vector3 m_DefaultViewPivot;

        [SerializeField]
        [Tooltip("Default orbit radius of the camera when viewing this environment scene")]
        float m_DefaultViewSize;

        [SerializeField]
        SimulationRenderSettings m_RenderSettings;

        [SerializeField]
        [HideInInspector]
        PostProcessProfile m_PostProcessProfile;

        [SerializeField]
        [Tooltip("When enabled, this environment prefab will be excluded from the selection dropdown in the XR Environment toolbar. " +
            "You can use this to exclude a base prefab that is only used as a template.")]
        bool m_ExcludeFromSelectionUI;

        /// <summary>
        /// Initial camera pose that the simulation will provide.
        /// </summary>
        public Pose cameraStartingPose => m_CameraStartingPose;

        /// <summary>
        /// Bounds within which to restrict simulation camera movement.
        /// </summary>
        public Bounds cameraMovementBounds => m_CameraMovementBounds;

        /// <summary>
        /// When enabled, this environment prefab will be excluded from the selection dropdown in the XR Environment toolbar.
        /// </summary>
        internal bool excludeFromSelectionUI
        {
            get => m_ExcludeFromSelectionUI;
            set => m_ExcludeFromSelectionUI = value;
        }

        internal Pose defaultViewPose => m_DefaultViewPose;

        /// <summary>
        /// Default world pivot of the scene camera for this environment scene.
        /// </summary>
        internal Vector3 defaultViewPivot => m_DefaultViewPivot;

        /// <summary>
        /// Default orbit radius of the camera when viewing this environment scene.
        /// </summary>
        internal float defaultViewSize => m_DefaultViewSize;

        internal SimulationRenderSettings renderSettings => m_RenderSettings;

        internal PostProcessProfile postProcessProfile => m_PostProcessProfile;

        void OnValidate()
        {
            // Make sure the camera movement bounds has volume
            var boundsSize = m_CameraMovementBounds.size;
            var sizeX = boundsSize.x > 0f;
            var sizeY = boundsSize.y > 0f;
            var sizeZ = boundsSize.z > 0f;
            if (!(sizeX && sizeY && sizeZ))
            {
                Debug.LogWarningFormat("Camera movement bounds for environment '{0}' has no volume. Expanding the size " +
                    "of sides with 0 size to 1.", gameObject.name);
                m_CameraMovementBounds = new Bounds(m_CameraMovementBounds.center, new Vector3(
                    sizeX ? m_CameraMovementBounds.size.x : 1f,
                    sizeY ? m_CameraMovementBounds.size.y : 1f,
                    sizeZ ? m_CameraMovementBounds.size.z : 1f
                ));
            }

            // Make sure camera starting pose is inside movement bounds
            if (!m_CameraMovementBounds.Contains(m_CameraStartingPose.position))
            {
                // See if we can grow the camera movement bounds to include the starting pose based on the gameObject's bounds
                var goBounds = BoundsUtils.GetBounds(gameObject.transform);
                if (goBounds.Contains(m_CameraStartingPose.position))
                {
                    Debug.LogWarningFormat("Camera starting pose for environment '{0}' is outside the camera movement bounds " +
                        "but inside the bounds of {0}'s GameObject. Growing the camera movement bounds to encapsulate " +
                        "the GameObject's bounds.", gameObject.name);
                    m_CameraMovementBounds.Encapsulate(goBounds);
                }
                else
                {
                    Debug.LogWarningFormat("Camera starting pose for environment '{0}' is outside the camera movement bounds. " +
                        "Moving the starting pose to the closest point on the bounds.", gameObject.name);
                    m_CameraStartingPose.position = m_CameraMovementBounds.ClosestPoint(m_CameraStartingPose.position);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(m_CameraMovementBounds.center, m_CameraMovementBounds.size);

            Gizmos.color = Color.cyan;
            DrawWireCamera(Gizmos.DrawLine, cameraStartingPose, 0.2f);
        }

        internal static void DrawWireCamera(Action<Vector3, Vector3> drawLineAction, Pose pose, float scale)
        {
            // camera vertices
            //     c1               c6 ________ c10        +y
            //      /|\              /|       /|           |  +x
            //     / |  \           / |      / |           | /
            // c0 /__|____\ c4  c5 /__|_____/c9|     +z____|/
            //    |c2| _- /        |c7|_____|__| c11
            //    |  /  /          |  /     |  /
            //    | / /            | /      | /
            // c3 |/            c8 |/_______|/ c12

            var cameraVerts = new Vector3[13];

            var trsMatrix = Matrix4x4.TRS(pose.position , pose.rotation, Vector3.one * scale);

            for (var i = 0; i < k_CameraPoints.Length; i++)
            {
                cameraVerts[i] = trsMatrix.MultiplyPoint(k_CameraPoints[i]);
            }

            // pyramid lines
            drawLineAction(cameraVerts[0], cameraVerts[1]);
            drawLineAction(cameraVerts[1], cameraVerts[2]);
            drawLineAction(cameraVerts[2], cameraVerts[3]);
            drawLineAction(cameraVerts[3], cameraVerts[0]);

            drawLineAction(cameraVerts[0], cameraVerts[4]);
            drawLineAction(cameraVerts[1], cameraVerts[4]);
            drawLineAction(cameraVerts[2], cameraVerts[4]);
            drawLineAction(cameraVerts[3], cameraVerts[4]);

            // box lines
            drawLineAction(cameraVerts[5], cameraVerts[6]);
            drawLineAction(cameraVerts[6], cameraVerts[7]);
            drawLineAction(cameraVerts[7], cameraVerts[8]);
            drawLineAction(cameraVerts[8], cameraVerts[5]);

            drawLineAction(cameraVerts[5], cameraVerts[9]);
            drawLineAction(cameraVerts[6], cameraVerts[10]);
            drawLineAction(cameraVerts[7], cameraVerts[11]);
            drawLineAction(cameraVerts[8], cameraVerts[12]);

            drawLineAction(cameraVerts[9], cameraVerts[10]);
            drawLineAction(cameraVerts[10], cameraVerts[11]);
            drawLineAction(cameraVerts[11], cameraVerts[12]);
            drawLineAction(cameraVerts[12], cameraVerts[9]);
        }
    }
}
