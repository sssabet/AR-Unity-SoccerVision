using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Menu that is added to a scene to surface tracking data and visualize trackables in order to aid in debugging.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class ARDebugMenu : MonoBehaviour
    {

        [SerializeField]
        [Tooltip("A debug prefab that visualizes the position of the XROrigin.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        GameObject m_OriginAxisPrefab;

        /// <summary>
        /// Specifies a debug prefab that will be attached to the <see cref="XROrigin"/>.
        /// </summary>
        /// <value>
        /// A debug prefab that will be attached to the XR origin.
        /// </value>
        public GameObject originAxisPrefab
        {
            get => m_OriginAxisPrefab;
            set => m_OriginAxisPrefab = value;
        }

        [SerializeField]
        [Tooltip("A particle system to represent ARPointClouds.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        ParticleSystem m_PointCloudParticleSystem;

        /// <summary>
        /// Specifies a particle system to visualize an <see cref="ARPointCloud"/>.
        /// </summary>
        /// <value>
        /// A particle system that will visualize point clouds.
        /// </value>
        public ParticleSystem pointCloudParticleSystem
        {
            get => m_PointCloudParticleSystem;
            set => m_PointCloudParticleSystem = value;
        }

        [SerializeField]
        [Tooltip("A line renderer used to outline the ARPlanes in a scene.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        LineRenderer m_LineRendererPrefab;

        /// <summary>
        /// Specifies the line renderer that will be used to outline planes in the scene.
        /// </summary>
        /// <value>
        /// A line renderer used to outline planes in the scene.
        /// </value>
        public LineRenderer lineRendererPrefab
        {
            get => m_LineRendererPrefab;
            set => m_LineRendererPrefab = value;
        }

        [SerializeField]
        [Tooltip("A debug prefab that visualizes the position of ARAnchors in a scene.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        GameObject m_AnchorPrefab;

        /// <summary>
        /// Specifies a debug prefab that will be attached to an <see cref="ARAnchor"/>.
        /// </summary>
        /// <value>
        /// A debug prefab that will be attached to the AR anchors in a scene.
        /// </value>
        public GameObject anchorPrefab
        {
            get => m_AnchorPrefab;
            set => m_AnchorPrefab = value;
        }

        [SerializeField]
        [Tooltip("The button that displays the AR Debug Menu's info sub-menu.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        Button m_DisplayInfoMenuButton;

        /// <summary>
        /// The button that displays the AR Debug Menu's info sub-menu.
        /// </summary>
        /// <value>
        /// A button that will be used to display the AR Debug Menu's info sub-menu.
        /// </value>
        public Button displayInfoMenuButton
        {
            get => m_DisplayInfoMenuButton;
            set => m_DisplayInfoMenuButton = value;
        }

        [SerializeField]
        [Tooltip("The button that displays the AR Debug Menu's configuration sub-menu.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        Button m_DisplayConfigurationsMenuButton;

        /// <summary>
        /// The button that displays the AR Debug Menu's session configuration sub-menu.
        /// </summary>
        /// <value>
        /// A button that will be used to display the AR Debug Menu's session configuration sub-menu.
        /// </value>
        public Button displayConfigurationsMenuButton
        {
            get => m_DisplayConfigurationsMenuButton;
            set => m_DisplayConfigurationsMenuButton = value;
        }

        [SerializeField]
        [Tooltip("The button that displays the AR Debug Menu's camera configuration sub-menu.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        Button m_DisplayCameraConfigurationsMenuButton;

        /// <summary>
        /// The button that displays the AR Debug Menu's camera configuration sub-menu.
        /// </summary>
        /// <value>
        /// A button that will be used to display the AR Debug Menu's camera configuration sub-menu.
        /// </value>
        public Button displayCameraConfigurationsMenuButton
        {
            get => m_DisplayCameraConfigurationsMenuButton;
            set => m_DisplayCameraConfigurationsMenuButton = value;
        }

        [SerializeField]
        [Tooltip("The button that displays the AR Debug Menu's debug options sub-menu.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        Button m_DisplayDebugOptionsMenuButton;

        /// <summary>
        /// The button that displays the AR Debug Menu's debug options sub-menu.
        /// </summary>
        /// <value>
        /// A button that will be used to display the AR Debug Menu's debug options sub-menu.
        /// </value>
        public Button displayDebugOptionsMenuButton
        {
            get => m_DisplayDebugOptionsMenuButton;
            set => m_DisplayDebugOptionsMenuButton = value;
        }

        [SerializeField]
        [Tooltip("The menu that contains debug info such as current FPS and tracking state.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        GameObject m_InfoMenu;

        /// <summary>
        /// The menu that contains debug info such as current FPS and tracking state.
        /// </summary>
        /// <value>
        /// A menu that will be used to display debug info such as current FPS and tracking state.
        /// </value>
        public GameObject infoMenu
        {
            get => m_InfoMenu;
            set => m_InfoMenu = value;
        }

        [SerializeField]
        [Tooltip("The menu that displays available camera configurations for the current session configuration.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        GameObject m_CameraConfigurationMenu;

        /// <summary>
        /// The menu that displays available <see cref="XRCameraConfiguration"/>s for the current session configuration.
        /// </summary>
        /// <value>
        /// A menu that will be used to display available camera configurations for the current session configuration.
        /// </value>
        public GameObject cameraConfigurationMenu
        {
            get => m_CameraConfigurationMenu;
            set => m_CameraConfigurationMenu = value;
        }

        [SerializeField]
        [Tooltip("The menu that displays available session configurations for the current platform.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        GameObject m_ConfigurationMenu;

        /// <summary>
        /// The menu that displays available <see cref="Configuration"/>s for the current platform.
        /// </summary>
        /// <value>
        /// A menu that will be used to display available session configurations for the current platform.
        /// </value>
        public GameObject configurationMenu
        {
            get => m_ConfigurationMenu;
            set => m_ConfigurationMenu = value;
        }

        [SerializeField]
        [Tooltip("The root of the menu where the available configurations for the current platform will be displayed and anchored.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        GameObject m_ConfigurationMenuRoot;

        /// <summary>
        /// The root of the menu that displays available <see cref="Configuration"/>s for the current platform. This helps center the data in the generated menu.
        /// </summary>
        /// <value>
        /// The root of the menu where the available configurations for the current platform will be displayed and anchored.
        /// </value>
        public GameObject configurationMenuRoot
        {
            get => m_ConfigurationMenuRoot;
            set => m_ConfigurationMenuRoot = value;
        }

        [SerializeField]
        [Tooltip("The menu that provides buttons for visualizing different trackables for debugging purposes.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        GameObject m_DebugOptionsMenu;

        /// <summary>
        /// The menu that provides buttons for visualizing different trackables for debugging purposes.
        /// </summary>
        /// <value>
        /// A menu that will be used to display different trackables for debugging purposes.
        /// </value>
        public GameObject debugOptionsMenu
        {
            get => m_DebugOptionsMenu;
            set => m_DebugOptionsMenu = value;
        }

        [SerializeField]
        [Tooltip("A menu that displays an informative warning messages.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        GameObject m_DebugOptionsToastMenu;

        /// <summary>
        /// The menu that displays informative warning messages.
        /// </summary>
        /// <value>
        /// A menu that displays an informative warning messages.
        /// </value>
        public GameObject debugOptionsToastMenu
        {
            get => m_DebugOptionsToastMenu;
            set => m_DebugOptionsToastMenu = value;
        }

        [SerializeField, FormerlySerializedAs("m_ShowSessionOriginButton")]
        [Tooltip("The button that displays the XR origin prefab.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        DebugSlider m_ShowOriginButton;

        /// <summary>
        /// The button that displays the XR origin prefab.
        /// </summary>
        /// <value>
        /// A button that will be used to display the XR origin prefab.
        /// </value>
        public DebugSlider showOriginButton
        {
            get => m_ShowOriginButton;
            set => m_ShowOriginButton = value;
        }

        [SerializeField]
        [Tooltip("The button that displays detected AR planes in the scene.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        DebugSlider m_ShowPlanesButton;

        /// <summary>
        /// The button that displays detected AR planes in the scene.
        /// </summary>
        /// <value>
        /// A button that will be used to display detected AR planes in the scene.
        /// </value>
        public DebugSlider showPlanesButton
        {
            get => m_ShowPlanesButton;
            set => m_ShowPlanesButton = value;
        }

        [SerializeField]
        [Tooltip("The button that displays anchors in the scene.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        DebugSlider m_ShowAnchorsButton;

        /// <summary>
        /// The button that displays anchors in the scene.
        /// </summary>
        /// <value>
        /// A button that will be used to display anchors in the scene.
        /// </value>
        public DebugSlider showAnchorsButton
        {
            get => m_ShowAnchorsButton;
            set => m_ShowAnchorsButton = value;
        }

        [SerializeField]
        [Tooltip("The button that displays detected point clouds in the scene.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        DebugSlider m_ShowPointCloudsButton;

        /// <summary>
        /// The button that displays detected point clouds in the scene.
        /// </summary>
        /// <value>
        /// A button that will be used to display detected point clouds in the scene.
        /// </value>
        public DebugSlider showPointCloudsButton
        {
            get => m_ShowPointCloudsButton;
            set => m_ShowPointCloudsButton = value;
        }

        [SerializeField]
        [Tooltip("The text object that will display current FPS.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        Text m_FpsLabel;

        /// <summary>
        /// The text object that will display current FPS.
        /// </summary>
        /// <value>
        /// A text object that will display current FPS.
        /// </value>
        public Text fpsLabel
        {
            get => m_FpsLabel;
            set => m_FpsLabel = value;
        }

        [SerializeField]
        [Tooltip("The text object that will display current tracking mode.\n For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.")]
        Text m_TrackingModeLabel;

        /// <summary>
        /// The text object that will display current tracking mode.
        /// </summary>
        /// <value>
        /// A text object that will display current tracking mode.
        /// </value>
        public Text trackingModeLabel
        {
            get => m_TrackingModeLabel;
            set => m_TrackingModeLabel = value;
        }

        [SerializeField]
        [Tooltip("The checkmark texture that will be used in the configuration submenu to display available configurations.")]
        Texture2D m_CheckMarkTexture;

        /// <summary>
        /// The checkmark texture that will be used in the <see cref="Configuration"/> submenu to display available configurations.
        /// </summary>
        /// <value>
        /// A checkmark texture that will be used in the configuration submenu to display available configurations.
        /// </value>
        public Texture2D checkMarkTexture
        {
            get => m_CheckMarkTexture;
            set => m_CheckMarkTexture = value;
        }

        [SerializeField]
        [Tooltip("The side bar that contains the buttons to the status info, configurations, and debug options menus.")]
        GameObject m_Toolbar;
        /// <summary>
        /// The side bar that contains the buttons to the status info, configurations, and debug options menus.
        /// </summary>
        /// <value>
        /// A side bar that contains the buttons to the status info, configurations, and debug options menus.
        /// </value>
        public GameObject toolbar
        {
            get => m_Toolbar;
            set => m_Toolbar = value;
        }

        [SerializeField]
        [Tooltip("The font that will be used for the generated parts of the menu.")]
        Font m_MenuFont;

        /// <summary>
        /// The font that will be used for the generated parts of the menu.
        /// </summary>
        /// <value>
        /// A font that will be used for the generated parts of the menu.
        /// </value>
        public Font menuFont
        {
            get => m_MenuFont;
            set => m_MenuFont = value;
        }

        [SerializeField]
        [Tooltip("The label that will display the resolution of the current camera configuration.")]
        Text m_CameraResolutionLabel;

        /// <summary>
        /// The label that will display the resolution of the current camera configuration.
        /// </summary>
        /// <value>
        /// A label that will display the resolution of the current camera configuration.
        /// </value>
        public Text cameraResolutionLabel
        {
            get => m_CameraResolutionLabel;
            set => m_CameraResolutionLabel = value;
        }

        [SerializeField]
        [Tooltip("The label that will display the frame rate of the current camera configuration.")]
        Text m_CameraFrameRateLabel;

        /// <summary>
        /// The label that will display the frame rate of the current camera configuration.
        /// </summary>
        /// <value>
        /// A label that will display the frame rate of the current camera configuration.
        /// </value>
        public Text cameraFrameRateLabel
        {
            get => m_CameraFrameRateLabel;
            set => m_CameraFrameRateLabel = value;
        }

        [SerializeField]
        [Tooltip("The label that will display whether depth sensor is supported in the current camera configuration.")]
        Text m_CameraDepthSensorLabel;

        /// <summary>
        /// The label that will display whether depth sensor is supported in the current camera configuration.
        /// </summary>
        /// <value>
        /// A label that will display whether depth sensor is supported in the current camera configuration.
        /// </value>
        public Text cameraDepthSensorLabel
        {
            get => m_CameraDepthSensorLabel;
            set => m_CameraDepthSensorLabel = value;
        }

        [SerializeField]
        [Tooltip("The dropdown that will display the list of currently available camera configurations.")]
        Dropdown m_CameraConfigurationDropdown;

        /// <summary>
        /// The dropdown that will display the list of currently available camera configurations.
        /// </summary>
        /// <value>
        /// A dropdown that will display the list of currently available camera configurations.
        /// </value>
        public Dropdown cameraConfigurationDropdown
        {
            get => m_CameraConfigurationDropdown;
            set => m_CameraConfigurationDropdown = value;
        }

        //Managers
        XROrigin m_Origin;

        ARSession m_Session;

        ARCameraManager m_CameraManager;

        //Visuals
        GameObject m_OriginAxis;

        GameObject m_PlaneVisualizers;

        ParticleSystem m_PointCloudVisualizer;

        GameObject m_AnchorVisualizers;

        //Labels
        int m_PreviousFps;

        int m_PreviousTrackingMode = -1;

        //Dictionaries
        Dictionary<ARPlane, LineRenderer> m_PlaneLineRenderers = new();

        Dictionary<ARAnchor, GameObject> m_AnchorPrefabs = new();

        Dictionary<ulong, Vector3> m_Points = new();

        ParticleSystem.Particle[] m_Particles;

        //Misc
        Camera m_CameraAR;

        bool m_CameraFollow;

        int m_NumParticles;

        //Configuration
        XRSessionSubsystem m_SessionSubsystem;

        bool m_ConfigMenuSetup;

        bool m_CameraMenuSetup;

        Configuration m_CurrentConfiguration;

        XRCameraConfiguration m_CurrentCameraConfiguration;

        int m_PreviousConfigCol = -1;

        List<List<Image>> m_ConfigurationUI = new();

        List<Text> m_ColumnLabels = new();

        void Start()
        {
            if(!CheckMenuConfigured())
            {
                enabled = false;
                Debug.LogError($"The menu has not been configured correctly and will currently be disabled. For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu.");
            }
            else
            {
                ConfigureMenuPosition();
            }
        }

        bool CheckMenuConfigured()
        {
            if(m_DisplayInfoMenuButton == null && m_DisplayConfigurationsMenuButton == null && m_DisplayDebugOptionsMenuButton == null && m_DisplayCameraConfigurationsMenuButton == null && m_Toolbar == null)
            {
                return false;
            }
            else if(m_DisplayInfoMenuButton == null || m_DisplayConfigurationsMenuButton == null || m_DisplayDebugOptionsMenuButton == null || m_DisplayCameraConfigurationsMenuButton == null || m_ShowOriginButton == null ||
                m_ShowPlanesButton == null || m_ShowAnchorsButton == null || m_ShowPointCloudsButton == null || m_FpsLabel == null || m_ConfigurationMenu == null ||
                m_TrackingModeLabel == null || m_OriginAxisPrefab == null || m_AnchorPrefab == null || m_LineRendererPrefab == null || m_InfoMenu == null || m_DebugOptionsMenu == null || m_ConfigurationMenuRoot == null ||
                m_MenuFont == null || m_Toolbar == null || m_DebugOptionsToastMenu == null || m_PointCloudParticleSystem == null || m_CameraConfigurationMenu == null || m_CameraResolutionLabel == null || m_CameraFrameRateLabel == null ||
                m_CameraDepthSensorLabel == null || m_CameraConfigurationDropdown == null)
            {
                Debug.LogWarning("The menu has not been fully configured so some functionality will be disabled. For an already configured menu, right-click on the Scene Inspector > XR > ARDebugMenu");
            }

            return true;
        }

        void OnEnable()
        {
            InitMenu();
            ConfigureButtons();
        }

        void OnDisable()
        {
            if(m_Origin != null)
            {
                var planeManager = m_Origin.GetComponent<ARPlaneManager>();
                if(planeManager != null)
                {
                    planeManager.planesChanged -= OnPlaneChanged;
                }

                var anchorManager = m_Origin.GetComponent<ARAnchorManager>();
                if(anchorManager != null)
                {
                    anchorManager.anchorsChanged -= OnAnchorChanged;
                }

                var pointCloudManager = m_Origin.GetComponent<ARPointCloudManager>();
                if(pointCloudManager != null)
                {
                    pointCloudManager.pointCloudsChanged -= OnPointCloudChanged;
                }
            }

            DeregisterUIListeners();
        }

        void Update()
        {
            int fps = (int)(1.0f / Time.unscaledDeltaTime);
            if(fps != m_PreviousFps)
            {
                m_FpsLabel.text = fps.ToString();
                m_PreviousFps = fps;
            }

            var state = (int)m_Session.currentTrackingMode;
            if(state != m_PreviousTrackingMode)
            {
                m_TrackingModeLabel.text = m_Session.currentTrackingMode.ToString();
                m_PreviousTrackingMode = state;
            }

            if(m_CameraFollow == true)
            {
                FollowCamera();
            }
        }

        void LateUpdate()
        {
            if(!m_ConfigMenuSetup)
            {
                SetupConfigurationMenu();
                m_ConfigMenuSetup = true;
            }

            if(!m_CameraMenuSetup)
            {
                PopulateCameraDropdown();
            }

            if(m_SessionSubsystem != null)
            {
                if(m_SessionSubsystem.currentConfiguration.HasValue && m_SessionSubsystem.currentConfiguration.Value != m_CurrentConfiguration)
                {
                    m_CurrentConfiguration = (Configuration)m_SessionSubsystem.currentConfiguration;
                    HighlightCurrentConfiguration(m_CurrentConfiguration.descriptor);
                }
            }

            if(m_CameraManager != null)
            {
                var cameraConfig = m_CameraManager.currentConfiguration;
                if(cameraConfig.HasValue && cameraConfig != m_CurrentCameraConfiguration)
                {
                    m_CameraResolutionLabel.text = cameraConfig.Value.resolution != null? cameraConfig.Value.resolution.ToString() : "Not Available";
                    m_CameraFrameRateLabel.text = cameraConfig.Value.framerate != null? cameraConfig.Value.framerate.ToString() : "Not Available";
                    m_CameraDepthSensorLabel.text = cameraConfig.Value.depthSensorSupported.ToString();
                    m_CurrentCameraConfiguration = (XRCameraConfiguration) cameraConfig;
                }
            }
        }

        void InitMenu()
        {
            var eventSystem = FindObjectsUtility.FindAnyObjectByType<EventSystem>();
            if(eventSystem == null)
            {
                Debug.LogError($"Failed to find EventSystem in current scene. As a result, this component will be disabled.");
                enabled = false;
                return;
            }

            var session = FindObjectsUtility.FindAnyObjectByType<ARSession>();
            if(session == null)
            {
                Debug.LogError($"Failed to find ARSession in current scene. As a result, this component will be disabled.");
                enabled = false;
                return;
            }
            m_Session = session;

            var origin = FindObjectsUtility.FindAnyObjectByType<XROrigin>();
            if(origin == null)
            {
                Debug.LogError($"Failed to find XROrigin in current scene. As a result, this component will be disabled.");
                enabled = false;
                return;
            }
            m_Origin = origin;

            var cameraManager = FindObjectsUtility.FindAnyObjectByType<ARCameraManager>();
            if(cameraManager == null)
            {
                Debug.LogWarning($"Failed to find an ARCameraManager in current scene. As a result, the camera configuration menu will be disabled.");
                DisableToolbarButton(m_DisplayCameraConfigurationsMenuButton);
            }
            else
            {
                m_CameraManager = cameraManager;
            }

            m_CameraAR = m_Origin.Camera;
#if !UNITY_IOS && !UNITY_ANDROID
            if(m_CameraAR == null)
            {
                Debug.LogError($"Failed to find camera attached to XROrigin. As a result, this component will be disabled.");
                enabled = false;
                return;
            }
#endif
            m_DisplayInfoMenuButton.onClick.AddListener(delegate { ShowMenu(m_InfoMenu); });
            m_DisplayConfigurationsMenuButton.onClick.AddListener(delegate { ShowMenu(m_ConfigurationMenu); });
            m_DisplayDebugOptionsMenuButton.onClick.AddListener(delegate { ShowMenu(m_DebugOptionsMenu); });
            m_DisplayCameraConfigurationsMenuButton.onClick.AddListener(delegate { ShowMenu(m_CameraConfigurationMenu); });
            m_CameraConfigurationDropdown.onValueChanged.AddListener(delegate { OnCameraDropdownValueChanged(m_CameraConfigurationDropdown); });

            Canvas menu = GetComponent<Canvas>();
#if UNITY_IOS || UNITY_ANDROID
            menu.renderMode = RenderMode.ScreenSpaceOverlay;
#else
            var rectTransform = GetComponent<RectTransform>();
            menu.renderMode = RenderMode.WorldSpace;
            menu.worldCamera  = m_CameraAR;
            m_CameraFollow = true;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 575);
#endif
        }

        void ConfigureMenuPosition()
        {
            float screenWidthInInches = Screen.width / Screen.dpi;

            if(screenWidthInInches < 5)
            {
                var rect = m_Toolbar.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0);
                rect.anchorMax = new Vector2(0.5f, 0);
                rect.eulerAngles = new Vector3(rect.eulerAngles.x, rect.eulerAngles.y, 90);
                rect.anchoredPosition = new Vector2(0, 20);
                var infoMenuButtonRect = m_DisplayInfoMenuButton.GetComponent<RectTransform>();
                var configurationsMenuButtonRect = m_DisplayConfigurationsMenuButton.GetComponent<RectTransform>();
                var cameraConfigurationsMenuButtonRect = m_DisplayCameraConfigurationsMenuButton.GetComponent<RectTransform>();
                var debugOptionsMenuButtonRect = m_DisplayDebugOptionsMenuButton.GetComponent<RectTransform>();
                infoMenuButtonRect.localEulerAngles =  new Vector3(infoMenuButtonRect.localEulerAngles.x, infoMenuButtonRect.localEulerAngles.y, -90);
                configurationsMenuButtonRect.localEulerAngles = new Vector3(configurationsMenuButtonRect.localEulerAngles.x, configurationsMenuButtonRect.localEulerAngles.y, -90);
                cameraConfigurationsMenuButtonRect.localEulerAngles = new Vector3(cameraConfigurationsMenuButtonRect.localEulerAngles.x, cameraConfigurationsMenuButtonRect.localEulerAngles.y, -90);
                debugOptionsMenuButtonRect.localEulerAngles = new Vector3(debugOptionsMenuButtonRect.localEulerAngles.x, debugOptionsMenuButtonRect.localEulerAngles.y, -90);

               var infoMenuRect = m_InfoMenu.GetComponent<RectTransform>();
               infoMenuRect.anchorMin = new Vector2(0.5f, 0);
               infoMenuRect.anchorMax = new Vector2(0.5f, 0);
               infoMenuRect.pivot = new Vector2(0.5f, 0);
               infoMenuRect.anchoredPosition = new Vector2(0, 150);

               var configurationsMenuRect = m_ConfigurationMenu.GetComponent<RectTransform>();
               configurationsMenuRect.anchorMin = new Vector2(0.5f, 0);
               configurationsMenuRect.anchorMax = new Vector2(0.5f, 0);
               configurationsMenuRect.pivot = new Vector2(0.5f, 0);
               configurationsMenuRect.anchoredPosition = new Vector2(0, 150);

               var cameraConfigurationsMenuRect = m_CameraConfigurationMenu.GetComponent<RectTransform>();
               cameraConfigurationsMenuRect.anchorMin = new Vector2(0.5f, 0);
               cameraConfigurationsMenuRect.anchorMax = new Vector2(0.5f, 0);
               cameraConfigurationsMenuRect.pivot = new Vector2(0.5f, 0);
               cameraConfigurationsMenuRect.anchoredPosition = new Vector2(0, 150);

               var debugOptionsMenuRect = m_DebugOptionsMenu.GetComponent<RectTransform>();
               debugOptionsMenuRect.anchorMin = new Vector2(0.5f, 0);
               debugOptionsMenuRect.anchorMax = new Vector2(0.5f, 0);
               debugOptionsMenuRect.pivot = new Vector2(0.5f, 0);
               debugOptionsMenuRect.anchoredPosition = new Vector2(0, 150);
            }
        }

        void ConfigureButtons()
        {
            if(m_ShowOriginButton && m_OriginAxisPrefab)
            {
                m_ShowOriginButton.interactable = true;
                m_ShowOriginButton.onValueChanged.AddListener(delegate {ToggleOriginVisibility();});
            }

            var planeManager = m_Origin.GetComponent<ARPlaneManager>();
            if(m_ShowPlanesButton && m_LineRendererPrefab && planeManager)
            {
                if (m_PlaneVisualizers == null)
                {
                    m_PlaneVisualizers = new GameObject("PlaneVisualizers");
                }

                m_PlaneVisualizers.SetActive(false);
                m_ShowPlanesButton.interactable = true;
                m_ShowPlanesButton.onValueChanged.AddListener(delegate {TogglePlanesVisibility();});
                planeManager.planesChanged += OnPlaneChanged;
            }

            var anchorManager = m_Origin.GetComponent<ARAnchorManager>();
            if(m_ShowAnchorsButton && m_AnchorPrefab && anchorManager)
            {
                if (m_AnchorVisualizers == null)
                {
                    m_AnchorVisualizers = new GameObject("AnchorVisualizers");
                }

                m_AnchorVisualizers.SetActive(false);
                m_ShowAnchorsButton.interactable = true;
                m_ShowAnchorsButton.onValueChanged.AddListener(delegate {ToggleAnchorsVisibility();});
                anchorManager.anchorsChanged += OnAnchorChanged;
            }

            var pointCloudManager = m_Origin.GetComponent<ARPointCloudManager>();
            if(m_ShowPointCloudsButton && m_PointCloudParticleSystem && pointCloudManager)
            {
                m_PointCloudVisualizer = Instantiate(m_PointCloudParticleSystem, m_Origin.TrackablesParent);
                var renderer = m_PointCloudVisualizer.GetComponent<Renderer>();
                renderer.enabled = false;
                pointCloudManager.pointCloudsChanged += OnPointCloudChanged;
                m_ShowPointCloudsButton.interactable = true;
                m_ShowPointCloudsButton.onValueChanged.AddListener(delegate {TogglePointCloudVisibility(renderer);});
            }
        }

        void DeregisterUIListeners()
        {
            if (m_DisplayInfoMenuButton)
                m_DisplayInfoMenuButton.onClick.RemoveAllListeners();

            if (m_DisplayConfigurationsMenuButton)
                m_DisplayConfigurationsMenuButton.onClick.RemoveAllListeners();

            if (m_DisplayDebugOptionsMenuButton)
                m_DisplayDebugOptionsMenuButton.onClick.RemoveAllListeners();

            if (m_DisplayCameraConfigurationsMenuButton)
                m_DisplayCameraConfigurationsMenuButton.onClick.RemoveAllListeners();

            if (m_CameraConfigurationDropdown)
                m_CameraConfigurationDropdown.onValueChanged.RemoveAllListeners();

            if (m_ShowOriginButton)
                m_ShowOriginButton.onValueChanged.RemoveAllListeners();

            if (m_ShowPlanesButton)
                m_ShowPlanesButton.onValueChanged.RemoveAllListeners();

            if (m_ShowAnchorsButton)
                m_ShowAnchorsButton.onValueChanged.RemoveAllListeners();

            if (m_ShowPointCloudsButton)
                m_ShowPointCloudsButton.onValueChanged.RemoveAllListeners();
        }

        void ShowMenu(GameObject menu)
        {
            if(menu.activeSelf)
            {
                menu.SetActive(false);
            }
            else
            {
                //Clear any currently open menus.
                m_InfoMenu.SetActive(false);
                m_ConfigurationMenu.SetActive(false);
                m_CameraConfigurationMenu.SetActive(false);
                m_DebugOptionsMenu.SetActive(false);

                menu.SetActive(true);
            }
        }

        void ToggleOriginVisibility()
        {
            if(m_OriginAxis == null)
            {
                m_OriginAxis = Instantiate(m_OriginAxisPrefab, m_Origin.transform);
            }
            else if(m_OriginAxis.activeSelf)
            {
               m_OriginAxis.SetActive(false);
            }
            else
            {
               m_OriginAxis.SetActive(true);
            }
        }

        void TogglePlanesVisibility()
        {
            if(m_PlaneVisualizers.activeSelf)
            {
               m_PlaneVisualizers.SetActive(false);
            }
            else
            {
               m_PlaneVisualizers.SetActive(true);
            }
        }

        void TogglePointCloudVisibility(Renderer renderer)
        {
            if(m_ShowPointCloudsButton.value == 0)
            {
                renderer.enabled = false;
            }
            else if(m_ShowPointCloudsButton.value == 1)
            {
                renderer.enabled = true;
            }
        }

        void ToggleAnchorsVisibility()
        {
            if(m_AnchorVisualizers.activeSelf)
            {
               m_AnchorVisualizers.SetActive(false);
            }
            else
            {
               m_AnchorVisualizers.SetActive(true);
               if(m_AnchorVisualizers.transform.childCount == 0 && m_DebugOptionsToastMenu)
               {
                   StartCoroutine(FadeToastDialog());
               }
            }
        }

        IEnumerator FadeToastDialog()
        {
            int seconds = 2;
            m_DebugOptionsToastMenu.SetActive(true);
            yield return new WaitForSeconds(seconds);
            m_DebugOptionsToastMenu.SetActive(false);
        }

        void SetupConfigurationMenu()
        {
            m_SessionSubsystem = GetSessionSubsystem();
            if(m_SessionSubsystem != null)
            {
                Dictionary<string, int[]> configurationGraph = new Dictionary<string, int[]>();
                var descriptors = m_SessionSubsystem.GetConfigurationDescriptors(Allocator.Temp);

                for (int i = 0; i < descriptors.Length; i++)
                {
                    string capabilities = descriptors[i].capabilities.ToStringList();
                    string[] features = capabilities.Split(", ");

                    foreach (var feature in features)
                    {
                        if(!configurationGraph.ContainsKey(feature))
                        {
                            configurationGraph.TryAdd(feature, new int[descriptors.Length]);
                        }
                        configurationGraph[feature][i] = 1;
                    }
                }
                CreateConfigurationGraph(configurationGraph, descriptors.Length);
            }
            else
            {
                Debug.LogWarning($"As there is no active XRSessionSubsystem available, the {typeof(ARDebugMenu).FullName}'s configuration sub-menu will not be enabled.");
                DisableToolbarButton(m_DisplayConfigurationsMenuButton);
            }
        }

        void DisableToolbarButton(Button button)
        {
            button.interactable = false;
            var icons = button.GetComponentsInChildren<Image>();
            if(icons.Length > 1)
            {
                var buttonImage = button.GetComponentsInChildren<Image>()[1];
                if(buttonImage != null)
                {
                    var newAlpha =  buttonImage.color;
                    newAlpha.a = 0.4f;
                    buttonImage.color = newAlpha;
                }
            }
        }

        void CreateConfigurationGraph(Dictionary<string, int[]> graph, int configurationsLength)
        {
            //Generate the initial size of the menu.
            var configurationMenu = m_ConfigurationMenu.GetComponent<RectTransform>();
            var xOffset = 295;
            var yOffset = 100;
            var colSize = 30;
            var rowSize = 25;
            configurationMenu.sizeDelta = new Vector2(xOffset + (configurationsLength * rowSize), yOffset + (graph.Count * colSize));

            int rowOffset = 10;
            int colOffset = 240;
            int fontSize = 20;
            for (int i = 0; i < configurationsLength; i++)
            {
                if(i%2 == 0)
                {
                    CreateBackgroundColumn(colOffset-12, 60 + (graph.Count * 45));
                }

                GameObject columnNumberLabel = new GameObject("ConfigurationLabel");
                columnNumberLabel.transform.SetParent(m_ConfigurationMenuRoot.transform);

                RectTransform rect = columnNumberLabel.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2 (50, 40);
                rect.anchoredPosition = new Vector2(colOffset, 40);
                colOffset += 30;

                Text text = columnNumberLabel.AddComponent<Text>();
                text.text = (i+1).ToString();
                text.font = m_MenuFont;
                text.fontSize = fontSize;
                var newAlpha =  text.color;
                newAlpha.a = 0.4f;
                text.color =  newAlpha;

                m_ConfigurationUI.Add(new List<Image>());
                m_ColumnLabels.Add(text);
            }

            int subRowOffset = 17;
            List<string> features = new List<string>(graph.Keys);
            features.Sort();
            foreach (var feature in features)
            {
                GameObject featureLabel = new GameObject(feature + "Label");
                featureLabel.transform.SetParent(m_ConfigurationMenuRoot.transform);

                RectTransform rect = featureLabel.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2 (350, 40);
                rect.anchoredPosition = new Vector2(80, rowOffset);
                rowOffset -= 30;

                Text text = featureLabel.AddComponent<Text>();
                text.text = feature;
                text.font = m_MenuFont;
                text.fontSize = fontSize;

                int subColOffset = 228;
                for (int a = 0; a < graph[feature].Length; a++)
                {
                    GameObject valueLabel = new GameObject("ValueLabel");
                    valueLabel.transform.SetParent(m_ConfigurationMenuRoot.transform);

                    RectTransform subRect = valueLabel.AddComponent<RectTransform>();
                    subRect.sizeDelta = new Vector2 (25, 25);
                    subRect.anchoredPosition = new Vector2(subColOffset, subRowOffset);
                    subColOffset += 30;

                    if(graph[feature][a] == 1)
                    {
                        Image image = valueLabel.AddComponent<Image>();
                        image.sprite = Sprite.Create(m_CheckMarkTexture, new Rect(0, 0, m_CheckMarkTexture.width, m_CheckMarkTexture.height), new Vector2(0.5f, 0.5f));
                        var newAlpha =  image.color;
                        newAlpha.a = 0.4f;
                        image.color =  newAlpha;
                        m_ConfigurationUI[a].Add(image);
                    }
                }

                subRowOffset -= 30;
            }
        }

        XRSessionSubsystem GetSessionSubsystem()
        {
            if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
            {
                var loader = XRGeneralSettings.Instance.Manager.activeLoader;
                if (loader != null)
                {
                   return loader.GetLoadedSubsystem<XRSessionSubsystem>();
                }
            }

            return null;
        }

        void CreateBackgroundColumn(int offset, int length)
        {
            GameObject columnBackground = new GameObject("ConfigurationColBackground");
            columnBackground.transform.SetParent(m_ConfigurationMenuRoot.transform);

            RectTransform rect = columnBackground.AddComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2 (40, length);
            rect.anchoredPosition = new Vector2(offset, 60);

            Image image = columnBackground.AddComponent<Image>();
            var newAlpha =  image.color;
            newAlpha.a = 0.05f;
            image.color =  newAlpha;
        }

        void HighlightCurrentConfiguration(ConfigurationDescriptor currentConfiguration)
        {
            var descriptors = m_SessionSubsystem.GetConfigurationDescriptors(Allocator.Temp);
            int configColumn = -1;
            for (int i = 0; i < descriptors.Length; i++)
            {
                if(descriptors[i] == currentConfiguration)
                {
                    configColumn = i;
                }
            }

            if(m_PreviousConfigCol != -1)
            {
                var prevColAlpha =  m_ColumnLabels[m_PreviousConfigCol].color;
                prevColAlpha.a = 0.4f;
                m_ColumnLabels[m_PreviousConfigCol].color = prevColAlpha;

                for (int k = 0; k < m_ConfigurationUI[m_PreviousConfigCol].Count; k++)
                {
                    var newAlpha =  m_ConfigurationUI[m_PreviousConfigCol][k].color;
                    newAlpha.a = 0.4f;
                    m_ConfigurationUI[m_PreviousConfigCol][k].color =  newAlpha;
                }
            }

            if(configColumn != -1)
            {
                var newColAlpha =  m_ColumnLabels[configColumn].color;
                newColAlpha.a = 1f;
                m_ColumnLabels[configColumn].color = newColAlpha;

                for (int j = 0; j < m_ConfigurationUI[configColumn].Count; j++)
                {
                    var newAlpha =  m_ConfigurationUI[configColumn][j].color;
                    newAlpha.a = 1f;
                    m_ConfigurationUI[configColumn][j].color =  newAlpha;
                }
            }

            m_PreviousConfigCol = configColumn;
        }

        void FollowCamera()
        {
            const float distance = 0.3f;
            const float smoothFactor = 0.1f;

            Vector3 targetPosition = m_CameraAR.transform.position + m_CameraAR.transform.forward * distance;
            Vector3 currentPosition = transform.position;

            if(Application.platform == RuntimePlatform.WSAPlayerX86)
            {
                transform.position = Vector3.Lerp(currentPosition, new Vector3(0, 0, targetPosition.z), smoothFactor);
                transform.rotation = Quaternion.LookRotation(currentPosition - m_CameraAR.transform.position);
            }
            else
            {
                transform.position = Vector3.Lerp(currentPosition, targetPosition, smoothFactor);
                transform.rotation = m_CameraAR.transform.rotation;
            }

            float height = 0;
            if (m_CameraAR.orthographic)
                height = m_CameraAR.orthographicSize * 2;
            else
                height = distance * Mathf.Tan(Mathf.Deg2Rad * (m_CameraAR.fieldOfView * 0.5f));

            float heightScale = height / m_CameraAR.scaledPixelHeight;
            transform.localScale = new Vector3(heightScale, heightScale, 1);
        }

        void OnPlaneChanged(ARPlanesChangedEventArgs eventArgs)
        {
            foreach(var plane in eventArgs.added)
            {
                var lineRenderer = GetOrCreateLineRenderer(plane);
                UpdateLine(plane, lineRenderer);
            }

            foreach(var plane in eventArgs.updated)
            {
                var lineRenderer = GetOrCreateLineRenderer(plane);
                UpdateLine(plane, lineRenderer);
            }

            foreach(var plane in eventArgs.removed)
            {
                if(m_PlaneLineRenderers.TryGetValue(plane, out var lineRenderer))
                {
                    m_PlaneLineRenderers.Remove(plane);
                    if(lineRenderer)
                    {
                        Destroy(lineRenderer.gameObject);
                    }
                }
            }
        }

        void OnAnchorChanged(ARAnchorsChangedEventArgs eventArgs)
        {
            foreach(var anchor in eventArgs.added)
            {
                var anchorPrefab = GetOrCreateAnchorPrefab(anchor);
                anchorPrefab.transform.SetPositionAndRotation(anchor.transform.position, anchor.transform.rotation);
            }

            foreach(var anchor in eventArgs.updated)
            {
                var anchorPrefab = GetOrCreateAnchorPrefab(anchor);
                anchorPrefab.transform.SetPositionAndRotation(anchor.transform.position, anchor.transform.rotation);
            }

            foreach(var anchor in eventArgs.removed)
            {
                if(m_AnchorPrefabs.TryGetValue(anchor, out var anchorPrefab))
                {
                    m_AnchorPrefabs.Remove(anchor);
                    if(anchorPrefab)
                    {
                        Destroy(anchorPrefab);
                    }
                }
            }
        }

        void OnPointCloudChanged(ARPointCloudChangedEventArgs eventArgs)
        {
            foreach(var pointCloud in eventArgs.added)
            {
                CreateOrUpdatePoints(pointCloud);
            }

            foreach(var pointCloud in eventArgs.updated)
            {
                CreateOrUpdatePoints(pointCloud);
            }

            foreach(var pointCloud in eventArgs.removed)
            {
                RemovePoints(pointCloud);
            }

            RenderPoints();
        }

        LineRenderer GetOrCreateLineRenderer(ARPlane plane)
        {
            if(m_PlaneLineRenderers.TryGetValue(plane, out var foundLineRenderer) && foundLineRenderer)
            {
                return foundLineRenderer;
            }

            var go = Instantiate(m_LineRendererPrefab, m_PlaneVisualizers.transform);
            var lineRenderer = go.GetComponent<LineRenderer>();
            m_PlaneLineRenderers[plane] = lineRenderer;

            return lineRenderer;
        }

        GameObject GetOrCreateAnchorPrefab(ARAnchor anchor)
        {
            if(m_AnchorPrefabs.TryGetValue(anchor, out var foundAnchorPrefab) && foundAnchorPrefab)
            {
                return foundAnchorPrefab;
            }

            var go = Instantiate(m_AnchorPrefab, m_AnchorVisualizers.transform);
            m_AnchorPrefabs[anchor] = go;

            return go;
        }

        void CreateOrUpdatePoints(ARPointCloud pointCloud)
        {
            if (!pointCloud.positions.HasValue)
                return;

            var positions = pointCloud.positions.Value;

            // Store all the positions over time associated with their unique identifiers
            if (pointCloud.identifiers.HasValue)
            {
                var identifiers = pointCloud.identifiers.Value;
                for (int i = 0; i < positions.Length; ++i)
                {
                    m_Points[identifiers[i]] = positions[i];
                }
            }
        }

        void RemovePoints(ARPointCloud pointCloud)
        {
            if (!pointCloud.positions.HasValue)
                return;

            var positions = pointCloud.positions.Value;

            if (pointCloud.identifiers.HasValue)
            {
                var identifiers = pointCloud.identifiers.Value;
                for (int i = 0; i < positions.Length; ++i)
                {
                    m_Points.Remove(identifiers[i]);
                }
            }
        }

        void RenderPoints()
        {
            if (m_Particles == null || m_Particles.Length < m_Points.Count)
            {
                m_Particles = new ParticleSystem.Particle[m_Points.Count];
            }

            int particleIndex = 0;
            foreach (var kvp in m_Points)
            {
                SetParticlePosition(particleIndex++, kvp.Value);
            }

            for (int i = m_Points.Count; i < m_NumParticles; ++i)
            {
                m_Particles[i].remainingLifetime = -1f;
            }

            m_PointCloudVisualizer.SetParticles(m_Particles, Math.Max(m_Points.Count, m_NumParticles));
            m_NumParticles = m_Points.Count;
        }

        void SetParticlePosition(int index, Vector3 position)
        {
            m_Particles[index].startColor = m_PointCloudVisualizer.main.startColor.color;
            m_Particles[index].startSize = m_PointCloudVisualizer.main.startSize.constant;
            m_Particles[index].position = position;
            m_Particles[index].remainingLifetime = 1f;
        }

        void UpdateLine(ARPlane plane, LineRenderer lineRenderer)
        {
            if(!lineRenderer)
            {
                return;
            }

            Transform planeTransform = plane.transform;
            bool useWorldSpace = lineRenderer.useWorldSpace;
            if(!useWorldSpace)
            {
                lineRenderer.transform.SetPositionAndRotation(planeTransform.position, planeTransform.rotation);
            }

            var boundary = plane.boundary;
            lineRenderer.positionCount = boundary.Length;
            for (int i = 0; i < boundary.Length; ++i)
            {
                var point2 = boundary[i];
                var localPoint = new Vector3(point2.x, 0, point2.y);
                if(useWorldSpace)
                {
                    lineRenderer.SetPosition(i, planeTransform.position + (planeTransform.rotation * localPoint));
                }
                else
                {
                    lineRenderer.SetPosition(i, new Vector3(point2.x, 0, point2.y));
                }
            }
        }

        void OnCameraDropdownValueChanged(Dropdown dropdown)
        {
            if ((m_CameraManager == null) || (m_CameraManager.subsystem == null) || !m_CameraManager.subsystem.running)
            {
                return;
            }

            var configurationIndex = dropdown.value;

            using (var configurations = m_CameraManager.GetConfigurations(Allocator.Temp))
            {
                if (configurationIndex >= configurations.Length)
                {
                    return;
                }

                var configuration = configurations[configurationIndex];
                m_CameraManager.currentConfiguration = configuration;
            }
        }

        void PopulateCameraDropdown()
        {
            if ((m_CameraManager == null) || (m_CameraManager.subsystem == null) || !m_CameraManager.subsystem.running)
                return;

            using (var configurations = m_CameraManager.GetConfigurations(Allocator.Temp))
            {
                if (!configurations.IsCreated || (configurations.Length <= 0))
                {
                    return;
                }

                List<string> configurationNames = new List<string>();
                foreach (var config in configurations)
                {
                    configurationNames.Add($"{config.width}x{config.height}{(config.framerate.HasValue ? $" at {config.framerate.Value} Hz" : "")}{(config.depthSensorSupported == Supported.Supported ? " depth sensor" : "")}");
                }
                m_CameraConfigurationDropdown.AddOptions(configurationNames);

                var currentConfig = m_CameraManager.currentConfiguration;
                for (int i = 0; i < configurations.Length; ++i)
                {
                    if (currentConfig == configurations[i])
                    {
                        m_CameraConfigurationDropdown.value = i;
                    }
                }
            }
            m_CameraMenuSetup = true;
        }
    }
}
