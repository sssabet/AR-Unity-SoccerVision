using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

namespace UnityEditor.XR.ARFoundation
{
    static class SceneUtils
    {
        const string k_BasePath = "Packages/com.unity.xr.arfoundation/";
        const string k_DebugFaceMaterial = k_BasePath + "Assets/Materials/DebugFace.mat";
        const string k_DebugPlaneMaterial = k_BasePath + "Assets/Materials/DebugPlane.mat";
        const string k_DebugMenuPrefab = k_BasePath + "Assets/Prefabs/DebugMenu.prefab";

        const string k_ParticleMaterial = "Default-Particle.mat";
        const string k_LineMaterial = "Default-Line.mat";

        const float k_ParticleSize = 0.02f;
        static readonly Color k_ParticleColor = new(253f / 255f, 184f / 255f, 19f / 255f);

        static void SpawnObjectWithContext(Func<Transform, GameObject> spawnObjectWithParent, MenuCommand command)
        {
            var context = (command.context as GameObject);
            var parent = context != null ? context.transform : null;
            var spawnedObject = spawnObjectWithParent(parent);
            Selection.activeGameObject = spawnedObject;
        }

        [MenuItem("GameObject/XR/AR Session", false, 10)]
        static void CreateARSession(MenuCommand menuCommand)
        {
            SpawnObjectWithContext(CreateARSessionWithParent, menuCommand);
        }

        public static GameObject CreateARSessionWithParent(Transform parent)
        {
            var arSession = ObjectFactory.CreateGameObject(
                "AR Session", typeof(ARSession), typeof(ARInputManager));
            
            CreateUtils.Place(arSession, parent);
            Undo.RegisterCreatedObjectUndo(arSession, "Create AR Session");
            return arSession;
        }

        [MenuItem("GameObject/XR/AR Default Point Cloud", false, 10)]
        static void CreateARPointCloudVisualizer(MenuCommand menuCommand)
        {
            SpawnObjectWithContext(CreateARPointCloudVisualizerWithParent, menuCommand);
        }

        public static GameObject CreateARPointCloudVisualizerWithParent(Transform parent)
        {
            var go = ObjectFactory.CreateGameObject("AR Default Point Cloud",
                typeof(ARPointCloudParticleVisualizer));
            var particleSystem = go.GetComponent<ParticleSystem>();
            UnityEditorInternal.ComponentUtility.MoveComponentDown(particleSystem);
            UnityEditorInternal.ComponentUtility.MoveComponentDown(particleSystem);
            var main = particleSystem.main;
            main.loop = false;
            main.startSize = k_ParticleSize;
            main.startColor = k_ParticleColor;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            main.playOnAwake = false;

            var emission = particleSystem.emission;
            emission.enabled = false;

            var shape = particleSystem.shape;
            shape.enabled = false;

            var renderer = particleSystem.GetComponent<Renderer>();
            renderer.material = AssetDatabase.GetBuiltinExtraResource<Material>(k_ParticleMaterial);

            CreateUtils.Place(go, parent);
            Undo.RegisterCreatedObjectUndo(go, "Create AR Default Point Cloud");
            return go;
        }

        [MenuItem("GameObject/XR/AR Default Plane", false, 10)]
        static void CreateARPlaneVisualizer(MenuCommand menuCommand)
        {
            SpawnObjectWithContext(CreateARPlaneVisualizerWithParent, menuCommand);
        }

        public static GameObject CreateARPlaneVisualizerWithParent(Transform parent)
        {
            var go = ObjectFactory.CreateGameObject("AR Default Plane",
                typeof(ARPlaneMeshVisualizer), typeof(MeshCollider), typeof(MeshFilter),
                typeof(MeshRenderer), typeof(LineRenderer));
            SetupMeshRenderer(go.GetComponent<MeshRenderer>(), k_DebugPlaneMaterial);
            SetupLineRenderer(go.GetComponent<LineRenderer>());

            CreateUtils.Place(go, parent);
            Undo.RegisterCreatedObjectUndo(go, "Create AR Default Plane");
            return go;
        }

        [MenuItem("GameObject/XR/AR Default Face", false, 10)]
        static void CreateARFaceVisualizer(MenuCommand menuCommand)
        {
            SpawnObjectWithContext(CreateARFaceVisualizerWithParent, menuCommand);
        }

        public static GameObject CreateARFaceVisualizerWithParent(Transform parent)
        {
            var go = ObjectFactory.CreateGameObject("AR Default Face",
                typeof(ARFaceMeshVisualizer), typeof(MeshCollider), typeof(MeshFilter),
                typeof(MeshRenderer));
            var meshRenderer = go.GetComponent<MeshRenderer>();
            SetupMeshRenderer(meshRenderer, k_DebugFaceMaterial);
            //self shadowing doesn't look good on the default face
            meshRenderer.receiveShadows = false;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            
            CreateUtils.Place(go, parent);
            Undo.RegisterCreatedObjectUndo(go, "Create AR Default Face");
            return go;
        }

        [MenuItem("GameObject/XR/AR Debug Menu", false, 10)]
        static void CreateARDebugMenu(MenuCommand menuCommand)
        {
            SpawnObjectWithContext(CreateARDebugMenuWithParent, menuCommand);
        }

        public static GameObject CreateARDebugMenuWithParent(Transform parent)
        {
            var debugMenu = (GameObject)PrefabUtility.InstantiatePrefab(
                AssetDatabase.LoadAssetAtPath<GameObject>(k_DebugMenuPrefab));

            CreateUtils.Place(debugMenu, parent);
            Undo.RegisterCreatedObjectUndo(debugMenu, "Create AR Debug Menu");
            return debugMenu;
        }

        static void SetupLineRenderer(LineRenderer lineRenderer)
        {
            var materials = new Material[1];
            materials[0] = AssetDatabase.GetBuiltinExtraResource<Material>(k_LineMaterial);
            lineRenderer.materials = materials;
            lineRenderer.loop = true;
            var curve = new AnimationCurve();
            curve.AddKey(0f, 0.005f);
            lineRenderer.widthCurve = curve;
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.black;
            lineRenderer.numCornerVertices = 4;
            lineRenderer.numCapVertices = 4;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.useWorldSpace = false;
        }

        static void SetupMeshRenderer(MeshRenderer meshRenderer, string materialName)
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialName);
            meshRenderer.sharedMaterials = new[] { material };
        }
    }
}
