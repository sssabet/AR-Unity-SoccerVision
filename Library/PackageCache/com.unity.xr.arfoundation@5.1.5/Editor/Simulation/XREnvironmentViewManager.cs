using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Simulation;
using UnityEngine.XR.ARFoundation.InternalUtils;

namespace UnityEditor.XR.Simulation
{
    class XREnvironmentViewManager : ScriptableSingleton<XREnvironmentViewManager>
    {
        const string k_ViewName = "XR Environment";
        static GUIContent s_SimulationSubsystemNotLoadedContent = new($"{k_ViewName} is not Available.\nEnable XR Simulation in Project Settings > XR Plug-in Management.");
        static GUIContent s_BaseSceneViewContent = new($"{k_ViewName} is not Available.\nUse a Scene View window.");
        static GUIContent s_XREnvironmentViewTitleContent;

        public static event Action<SceneView> environmentViewEnabled;
        public static event Action<SceneView> environmentViewDisabled;

        [SerializeField]
        GUIContent m_SceneViewTitleContent;

        [SerializeField]
        EditorSimulationSceneManager m_EditorSimulationSceneManager;

        [SerializeField]
        XREnvironmentViewCamera m_EnvironmentViewCamera;

        [SerializeField]
        SceneView[] m_ActiveEnvironmentViewsAtReload;

        bool m_Initialized;
        ulong m_CurrentSceneMask;

        SimulationXRayManager m_XRayManager;

        HashSet<SceneView> m_EnvironmentViews = new();
        HashSet<Camera> m_EnvironmentCameras = new();

        public bool IsEnvironmentViewEnabled(EditorWindow window)
        {
            if (window is SceneView sceneView)
            {
                return m_EnvironmentViews.Contains(sceneView);
            }

            return false;
        }

        static GUIContent xrEnvironmentViewTitleContent
        {
            get
            {
                if (s_XREnvironmentViewTitleContent == null)
                {
                    s_XREnvironmentViewTitleContent = EditorGUIUtility.TrIconContent(XREnvironmentToolbarOverlay.xrEnvironmentIconPath);
                    s_XREnvironmentViewTitleContent.text = k_ViewName;
                }

                return s_XREnvironmentViewTitleContent;
            }
        }

        static bool useEditorSceneManager => SimulationEditorUtilities.simulationSubsystemEnabled
            && !EditorApplication.isPlayingOrWillChangePlaymode;

        internal BaseSimulationSceneManager activeSceneManager => !EditorApplication.isPlayingOrWillChangePlaymode
            ? m_EditorSimulationSceneManager
            : SimulationSessionSubsystem.simulationSceneManager;

        internal HashSet<Camera> environmentCameras => m_EnvironmentCameras;

        [MenuItem("Window/XR/AR Foundation/" + k_ViewName)]
        static void GetXREnvironmentView()
        {
            var environmentViewManager = instance;
            SceneView sceneView = null;
            if (environmentViewManager.m_EnvironmentViews.Count > 0)
            {
                var lastActiveView = SceneView.lastActiveSceneView;
                sceneView = environmentViewManager.m_EnvironmentViews.Contains(lastActiveView) ? lastActiveView : instance.m_EnvironmentViews.First();
            }

            if (sceneView == null)
            {
                sceneView = EditorWindow.CreateWindow<SceneView>();
            }
            else
            {
                sceneView.Show();
                sceneView.Focus();
                return;
            }

            environmentViewManager.CacheTitleContent(sceneView);

            var environmentOverlay = ShowEnvironmentOverlay(sceneView);
            if (environmentOverlay != null)
            {
                // Place toolbar with enough space for toolbars docked on top and side with another toolbar in top left corner
                environmentOverlay.Undock();
                environmentOverlay.floatingPosition = new Vector2(90f, 26f);
            }

            environmentViewManager.EnableEnvironmentView(sceneView);

            var assetGuid = SimulationEnvironmentAssetsManager.GetActiveEnvironmentAssetGuid();
            AREditorAnalytics.simulationUIAnalyticsEvent.Send(
                new SimulationUIAnalyticsArgs(
                    eventName: SimulationUIAnalyticsArgs.EventName.WindowUsed,
                    environmentGuid: assetGuid,
                    windowUsed: new SimulationUIAnalyticsArgs.WindowUsed { name = XREnvironmentToolbarOverlay.toolbarDisplayName, isActive = true }));

            sceneView.Show();
            sceneView.Focus();
        }

        internal void OnEnable()
        {
            if (m_Initialized)
            {
                RestoreEnvironmentViewsAfterReload();
                return;
            }

            SceneView.beforeSceneGui += BeforeSceneGui;
            SceneView.duringSceneGui += DuringSceneGui;
            SimulationEditorUtilities.simulationSubsystemLoaderAddedOrRemoved += SetUpOrChangeEnvironment;
            SimulationEditorUtilities.simulationSubsystemLoaderAddedOrRemoved += SimulationSubsystemDisabledMessage;

            if (useEditorSceneManager)
                m_EditorSimulationSceneManager = new EditorSimulationSceneManager();

            m_XRayManager = new SimulationXRayManager();

            var environmentAssetsManager = SimulationEnvironmentAssetsManager.Instance;
            environmentAssetsManager.activeEnvironmentChanged += SetUpOrChangeEnvironment;

            RestoreEnvironmentViewsAfterReload();
            m_Initialized = true;
        }

        internal void OnDisable()
        {
            if (!m_Initialized)
                return;

            m_Initialized = false;

            CacheEnvironmentViewsBeforeReload();
            SceneView.beforeSceneGui -= BeforeSceneGui;
            SceneView.duringSceneGui -= DuringSceneGui;
            SimulationEditorUtilities.simulationSubsystemLoaderAddedOrRemoved -= SetUpOrChangeEnvironment;
            SimulationEditorUtilities.simulationSubsystemLoaderAddedOrRemoved -= SimulationSubsystemDisabledMessage;

            var environmentAssetsManager = SimulationEnvironmentAssetsManager.Instance;
            environmentAssetsManager.activeEnvironmentChanged -= SetUpOrChangeEnvironment;

            CleanUpEnvironmentViews();
            CleanUpEnvironment();

            m_EnvironmentViews.Clear();
            m_EnvironmentCameras.Clear();
            m_EditorSimulationSceneManager = null;
            m_XRayManager = null;
        }

        internal void EnableEnvironmentView(SceneView sceneView)
        {
            if (!XREnvironmentViewUtilities.IsBaseSceneView(sceneView))
            {
                BaseSceneViewMessage(sceneView);
                DisableEnvironmentView(sceneView);
                return;
            }

            CacheTitleContent(sceneView);

            m_EnvironmentViews.Add(sceneView);
            m_EnvironmentCameras.Add(sceneView.camera);
            environmentViewEnabled?.Invoke(sceneView);

            EditorApplication.delayCall += () => sceneView.titleContent = xrEnvironmentViewTitleContent;

            if (ShowEnvironmentOverlay(sceneView) == null)
                EditorApplication.delayCall += () => ShowEnvironmentOverlay(sceneView);

            if (CheckRemoveNotifications(sceneView))
                sceneView.RemoveNotification();
            
            if (!SimulationEditorUtilities.simulationSubsystemEnabled)
            {
                sceneView.ShowNotification(s_SimulationSubsystemNotLoadedContent);
                return;
            }

            if (!EnvironmentSceneLoaded())
                SetUpOrChangeEnvironment();

            SetViewToEnvironment(sceneView);
        }

        internal void DisableEnvironmentView(SceneView sceneView)
        {
            if (!XREnvironmentViewUtilities.IsBaseSceneView(sceneView) || sceneView == null)
            {
                m_EnvironmentViews.Remove(sceneView);
                return;
            }

            m_EnvironmentCameras.Remove(sceneView.camera);
            m_EnvironmentViews.Remove(sceneView);

            environmentViewDisabled?.Invoke(sceneView);
            if (sceneView == null || sceneView.camera == null)
                return;

            sceneView.titleContent = m_SceneViewTitleContent;

            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                return;

            sceneView.camera.overrideSceneCullingMask = EditorSceneManager.DefaultSceneCullingMask;

            if (useEditorSceneManager && EnabledEnvironmentViewsCount() == 0)
                CleanUpEnvironment();
        }

        void BeforeSceneGui(SceneView sceneView)
        {
            if (!m_Initialized)
                return;

            // Cache the render settings in case they are being modified by the user.
            if (XREnvironmentViewUtilities.renderingOverrideEnabled && !XREnvironmentViewUtilities.lightingOverrideActive)
                XREnvironmentViewUtilities.simulationRenderSettings.UseSceneRenderSettings();
        }

        void DuringSceneGui(SceneView sceneView)
        {
            if (!m_Initialized)
                return;

            if (!m_EnvironmentViews.Contains(sceneView) || activeSceneManager == null 
                || PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                DoSceneViewXRay();
                if (sceneView.titleContent == xrEnvironmentViewTitleContent)
                    sceneView.titleContent = m_SceneViewTitleContent;

                return;
            }

            if (sceneView.titleContent != xrEnvironmentViewTitleContent)
                sceneView.titleContent = xrEnvironmentViewTitleContent;

            sceneView.camera.overrideSceneCullingMask = m_CurrentSceneMask;

            CheckARCamera();

            var scene = activeSceneManager.environmentScene;
            var hasXRayRegion = XRayRuntimeUtils.xRayRegions.TryGetValue(scene, out var xRayRegion);
            m_XRayManager?.UpdateXRayShader(hasXRayRegion, xRayRegion);

            var simEnv = activeSceneManager.simulationEnvironment;
            if (simEnv != null)
            {
                Handles.color = Color.cyan;
                SimulationEnvironment.DrawWireCamera(Handles.DrawLine, simEnv.cameraStartingPose, sceneView.size * 0.06f);
            }

            XREnvironmentViewUtilities.RestoreBaseLighting();
        }

        void SetUpOrChangeEnvironment()
        {
            if (Application.isPlaying)
                return;

            CleanUpEnvironment();

            if (useEditorSceneManager && m_EnvironmentViews.Count > 0 && m_EditorSimulationSceneManager != null)
            {
                m_EditorSimulationSceneManager.SetupEnvironment();
                m_CurrentSceneMask = EditorSceneManager.GetSceneCullingMask(m_EditorSimulationSceneManager.environmentScene);
                
                if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                    return;

                foreach (var sceneView in m_EnvironmentViews)
                {
                   SetViewToEnvironment(sceneView);
                }
            }
        }

        void SetViewToEnvironment(SceneView sceneView)
        {
            // When called from OnEnable the scene view may not be fully initialized
            // Do not change the view if in prefab stage
            if (sceneView == null || sceneView.camera == null 
                || PrefabStageUtility.GetCurrentPrefabStage() != null
                || activeSceneManager == null)
                return;

            sceneView.camera.overrideSceneCullingMask = m_CurrentSceneMask;
            var simulationEnvironment = activeSceneManager.simulationEnvironment;
            if (simulationEnvironment != null)
            {
                var pivot = simulationEnvironment.defaultViewPivot;
                var rotation = simulationEnvironment.defaultViewPose.rotation;
                var size = simulationEnvironment.defaultViewSize;

                sceneView.LookAt(pivot, rotation, size, sceneView.orthographic, true);
            }
        }

        void CleanUpEnvironment()
        {
            if (Application.isPlaying)
                return;

            m_CurrentSceneMask = EditorSceneManager.DefaultSceneCullingMask;
            XREnvironmentViewUtilities.RestoreBaseLighting();
            XREnvironmentViewUtilities.renderingOverrideEnabled = false;
            m_EditorSimulationSceneManager?.TearDownEnvironment();
        }

        void CleanUpEnvironmentViews()
        {
            m_CurrentSceneMask = EditorSceneManager.DefaultSceneCullingMask;
            OverrideCameraSceneMask(EditorSceneManager.DefaultSceneCullingMask);
        }

        /// <summary>
        /// Checks the the 'main' camera has the <see cref="XREnvironmentViewCamera"/> component added to it.
        /// This component is used to set render overrides when rendering the scene view camera. It is done this way
        /// since rendering components are copied over from the main camera to the scene view camera before every render.
        /// Directly trying to add the <see cref="XREnvironmentViewCamera"/> to the scene view camera will not work
        /// since it will be removed before rendering.
        /// </summary>
        void CheckARCamera()
        {
            if (m_EnvironmentViewCamera != null && m_EnvironmentViewCamera.camera == Camera.main)
                return;

            var camera = Camera.main;
            if (camera == null)
            {
                var xrOrigin = FindObjectsUtility.FindAnyObjectByType<XROrigin>();
                if (xrOrigin != null)
                    camera = xrOrigin.Camera;
            }

            if (camera == null)
                return;

            if (m_EnvironmentViewCamera != null && m_EnvironmentViewCamera.camera != camera)
            {
                UnityObjectUtils.Destroy(m_EnvironmentViewCamera);
            }
            else
            {
                var environmentViewCamera = camera.GetComponent<XREnvironmentViewCamera>();
                m_EnvironmentViewCamera = environmentViewCamera != null ? environmentViewCamera
                    : camera.gameObject.AddComponent<XREnvironmentViewCamera>();

                m_EnvironmentViewCamera.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
            }
        }

        void DoSceneViewXRay()
        {
            if (m_XRayManager == null)
                return;

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                m_XRayManager.UpdateXRayShader(false, null);
                return;
            }

            var useXRay = XRayRuntimeUtils.xRayRegions.TryGetValue(prefabStage.scene, out var xRayRegion);
            m_XRayManager.UpdateXRayShader(useXRay, xRayRegion);
        }

        void OverrideCameraSceneMask(ulong sceneMask)
        {
            m_CurrentSceneMask = sceneMask;

            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                return;

            foreach (var sceneView in m_EnvironmentViews)
            {
                if (sceneView.camera != null)
                    sceneView.camera.overrideSceneCullingMask = sceneMask;
            }
        }

        void SimulationSubsystemDisabledMessage()
        {
            foreach (var sceneView in m_EnvironmentViews)
            {
                if (!SimulationEditorUtilities.simulationSubsystemEnabled)
                    sceneView.ShowNotification(s_SimulationSubsystemNotLoadedContent);
                else if (CheckRemoveNotifications(sceneView))
                    sceneView.RemoveNotification();
            }
        }

        static void BaseSceneViewMessage(SceneView sceneView)
        {
            if (!XREnvironmentViewUtilities.IsBaseSceneView(sceneView))
                sceneView.ShowNotification(s_BaseSceneViewContent);
            else if (CheckRemoveNotifications(sceneView))
                sceneView.RemoveNotification();
        }

        static bool CheckRemoveNotifications(SceneView sceneView)
        {
            return SimulationEditorUtilities.simulationSubsystemEnabled
                && XREnvironmentViewUtilities.IsBaseSceneView(sceneView);
        }

        void CacheEnvironmentViewsBeforeReload()
        {
            var enabledViews = new List<SceneView>();
            foreach (var viewCandidate in m_EnvironmentViews)
            {
                if (viewCandidate != null)
                    enabledViews.Add(viewCandidate);
            }

            if (m_EnvironmentViews != null && enabledViews.Count > 0)
                m_ActiveEnvironmentViewsAtReload = enabledViews.ToArray();
            else
                m_ActiveEnvironmentViewsAtReload = Array.Empty<SceneView>();
        }

        void RestoreEnvironmentViewsAfterReload()
        {
            if (m_ActiveEnvironmentViewsAtReload == null || m_ActiveEnvironmentViewsAtReload.Length < 1)
                return;

            m_EnvironmentViews.Clear();
            foreach (var environmentView in m_ActiveEnvironmentViewsAtReload)
            {
                EnableEnvironmentView(environmentView);
            }
        }

        void CacheTitleContent(SceneView sceneView)
        {
            if (m_SceneViewTitleContent != null && m_SceneViewTitleContent.text == k_ViewName)
                m_SceneViewTitleContent = null;

            // Only cache the title contents if we know the view is not a environment view.
            if (m_SceneViewTitleContent == null || m_SceneViewTitleContent.image == null
                && XREnvironmentViewUtilities.IsBaseSceneView(sceneView)
                && (s_XREnvironmentViewTitleContent == null || sceneView.titleContent != s_XREnvironmentViewTitleContent))
            {
                m_SceneViewTitleContent = sceneView.titleContent;
            }
        }

        int EnabledEnvironmentViewsCount()
        {
            var environmentViewsCount = 0;
            foreach (var viewCandidate in m_EnvironmentViews)
            {
                if (viewCandidate != null)
                    environmentViewsCount++;
            }

            return environmentViewsCount;
        }

        bool EnvironmentSceneLoaded()
        {
            return activeSceneManager != null 
                && activeSceneManager.environmentScene != default 
                && activeSceneManager.environmentScene.isLoaded;
        }

        static Overlay ShowEnvironmentOverlay(SceneView sceneView)
        {
            if (sceneView == null)
                return null;
            
            if (sceneView.TryGetOverlay(XREnvironmentToolbarOverlay.overlayId, out var environmentOverlay))
            {
                if (!environmentOverlay.displayed)
                    environmentOverlay.displayed = true;
            }

            return environmentOverlay;
        }
    }
}

