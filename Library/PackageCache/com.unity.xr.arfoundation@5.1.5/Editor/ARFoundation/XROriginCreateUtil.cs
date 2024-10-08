using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.ARFoundation;
#if !XRI_PRESENT
using UnityEditor.SceneManagement;
#endif

namespace UnityEditor.XR.ARFoundation
{
    static class XROriginCreateUtil
    {
#if !XRI_PRESENT
        [MenuItem("GameObject/XR/XR Origin (Mobile AR)", false, 10)]
        static void CreateXROrigin(MenuCommand menuCommand)
        {
            var context = (menuCommand.context as GameObject);
            var parent = context != null ? context.transform : null;
            var xrOrigin = CreateXROriginWithParent(parent);
            Selection.activeGameObject = xrOrigin.gameObject;
        }
#endif

        public static XROrigin CreateXROriginWithParent(Transform parent)
        {
            var originGo = ObjectFactory.CreateGameObject("XR Origin", typeof(XROrigin));
            CreateUtils.Place(originGo, parent);

            var offsetGo = ObjectFactory.CreateGameObject("Camera Offset");
            CreateUtils.Place(offsetGo, originGo.transform);
            
            var arCamera = CreateARMainCamera();
            CreateUtils.Place(arCamera.gameObject, offsetGo.transform);

            var origin = originGo.GetComponent<XROrigin>();
            origin.CameraFloorOffsetObject = offsetGo;
            origin.Camera = arCamera;
            
            Undo.RegisterCreatedObjectUndo(originGo, "Create XR Origin");
            return origin;
        }

        static Camera CreateARMainCamera()
        {
            var mainCam = Camera.main;
            if (mainCam != null)
            {
                Debug.LogWarningFormat(
                    mainCam.gameObject,
                    "XR Origin Main Camera requires the \"MainCamera\" Tag, but the current scene contains another enabled Camera tagged \"MainCamera\". For AR to function properly, remove the \"MainCamera\" Tag from \'{0}\' or disable it.",
                    mainCam.name);
            }

            var cameraGo = ObjectFactory.CreateGameObject(
                "Main Camera",
                typeof(Camera),
                typeof(AudioListener),
                typeof(ARCameraManager),
                typeof(ARCameraBackground),
                typeof(TrackedPoseDriver));

            var camera = cameraGo.GetComponent<Camera>();
            Undo.RecordObject(camera, "Configure Camera");
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.black;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 20f;

            var trackedPoseDriver = cameraGo.GetComponent<TrackedPoseDriver>();

            Undo.RecordObject(trackedPoseDriver, "Configure Tracked Pose Driver");
            var positionAction = new InputAction("Position", binding: "<XRHMD>/centerEyePosition", expectedControlType: "Vector3");
            positionAction.AddBinding("<HandheldARInputDevice>/devicePosition");
            var rotationAction = new InputAction("Rotation", binding: "<XRHMD>/centerEyeRotation", expectedControlType: "Quaternion");
            rotationAction.AddBinding("<HandheldARInputDevice>/deviceRotation");
            trackedPoseDriver.positionInput = new InputActionProperty(positionAction);
            trackedPoseDriver.rotationInput = new InputActionProperty(rotationAction);
            return camera;
        }
    }
}
