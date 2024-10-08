using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Input handler for movement in the device view and game view when using <c>CameraPoseProvider</c>.
    /// </summary>
    class CameraFPSModeHandler
    {
        const float k_MoveSpeed = 1f;
        const float k_PitchClamp = 85f;

        bool m_ShiftSpeed;
        Vector2 m_CurrentMousePosition;
        Vector2 m_LastMousePosition;
        Vector2 m_MouseDelta;
        Vector3 m_CurrentInputVector;
        bool m_Dragging;

        /// <summary>
        /// Is there active movement.
        /// </summary>
        public bool moveActive { get; private set; }

        /// <summary>
        /// Use <c>MovementBounds</c> bounds to restrict movement.
        /// </summary>
        public bool useMovementBounds { get; set; }

        /// <summary>
        /// The bounds of the movement area.
        /// </summary>
        public Bounds movementBounds { get; set; }

        /// <summary>
        /// Handle the FPS mode movement from a game view.
        /// </summary>
        public void HandleGameInput()
        {
            var keyboard = Keyboard.current;
            m_CurrentInputVector.z = keyboard.wKey.isPressed ? 1f : m_CurrentInputVector.z > 0f ? 0f : m_CurrentInputVector.z;
            m_CurrentInputVector.z = keyboard.sKey.isPressed ? -1f : m_CurrentInputVector.z < 0f ? 0f : m_CurrentInputVector.z;
            m_CurrentInputVector.x = keyboard.aKey.isPressed ? -1f : m_CurrentInputVector.x < 0f ? 0f : m_CurrentInputVector.x;
            m_CurrentInputVector.x = keyboard.dKey.isPressed ? 1f : m_CurrentInputVector.x > 0f ? 0f : m_CurrentInputVector.x;
            m_CurrentInputVector.y = keyboard.eKey.isPressed ? 1f : m_CurrentInputVector.y > 0f ? 0f : m_CurrentInputVector.y;
            m_CurrentInputVector.y = keyboard.qKey.isPressed ? -1f : m_CurrentInputVector.y < 0f ? 0f : m_CurrentInputVector.y;
            m_ShiftSpeed = keyboard.shiftKey.isPressed;

            var mouse = Mouse.current;
            m_CurrentMousePosition = mouse.position.ReadValue();
            m_Dragging = true;

            if (mouse.rightButton.wasPressedThisFrame)
            {
                m_LastMousePosition = m_CurrentMousePosition;
                moveActive = true;
            }
            else if (mouse.rightButton.wasReleasedThisFrame)
            {
                StopMoveInput(m_CurrentMousePosition);
            }
            else if (mouse.rightButton.isPressed)
            {
                m_MouseDelta = m_CurrentMousePosition - m_LastMousePosition;
                m_LastMousePosition = m_CurrentMousePosition;
            }
        }

        /// <summary>
        /// End all movement.
        /// </summary>
        /// <param name="mousePosition">The current mose position.</param>
        public void StopMoveInput(Vector2 mousePosition)
        {
            moveActive = false;
            m_CurrentMousePosition = mousePosition;
            m_MouseDelta = Vector2.zero;
            m_LastMousePosition = m_CurrentMousePosition;
            m_CurrentInputVector = Vector3.zero;
            m_ShiftSpeed = false;
        }

        Vector3 GetMovementDirection()
        {
            if (m_CurrentInputVector.sqrMagnitude < float.Epsilon)
                return Vector3.zero;

            var speedModifier = k_MoveSpeed;

            if (!Application.isPlaying && m_ShiftSpeed)
                speedModifier *= 5f;
            else if (Keyboard.current.shiftKey.isPressed)
                speedModifier *= 5f;

            return Time.deltaTime * speedModifier * m_CurrentInputVector.normalized;
        }

        /// <summary>
        /// Calculate the new pose for the camera based on the fps mode movement.
        /// </summary>
        /// <param name="pose">Current Pose of the transform we are going to move.</param>
        /// <param name="invertY">Invert the Y axis of the mouse position.</param>
        /// <returns>The new camera pose.</returns>
        public Pose CalculateMovement(Pose pose, bool invertY = false)
        {
            var rotation = CalculateMouseRotation(pose.rotation, invertY);
            var position = pose.position + rotation.ConstrainYawNormalized() * GetMovementDirection();
            if (useMovementBounds && movementBounds != default && !movementBounds.Contains(position))
                position = movementBounds.ClosestPoint(position);

            return new Pose(position, rotation);
        }

        Quaternion CalculateMouseRotation(Quaternion rotation, bool invertY = false)
        {
            if (!m_Dragging)
                return rotation;

            var eulerAngles = rotation.eulerAngles;
            var yaw = eulerAngles.y;
            var pitch = eulerAngles.x;
            if (pitch > 180)
                pitch = pitch - 360;

            const float pixelsToDegrees = 0.003f * Mathf.Rad2Deg;

            var deltaY = m_MouseDelta.y;
            if (invertY)
                deltaY *= -1;

            yaw += m_MouseDelta.x * pixelsToDegrees;
            pitch += deltaY * pixelsToDegrees;

            pitch = Mathf.Clamp(pitch, -k_PitchClamp, k_PitchClamp);

            return Quaternion.AngleAxis(yaw, Vector3.up) * Quaternion.AngleAxis(pitch, Vector3.right);
        }
    }
}
