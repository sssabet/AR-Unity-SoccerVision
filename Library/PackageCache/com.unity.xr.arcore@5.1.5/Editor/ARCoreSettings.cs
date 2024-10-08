using UnityEngine;
using UnityEngine.XR.Management;
using System.IO;

namespace UnityEditor.XR.ARCore
{
    /// <summary>
    /// Holds settings that are used to configure the Google ARCore XR Plug-in.
    /// </summary>
    [System.Serializable]
    [XRConfigurationData("Google ARCore", "UnityEditor.XR.ARCore.ARCoreSettings")]
    public class ARCoreSettings : ScriptableObject
    {
        /// <summary>
        /// Enum used to specify whether a feature is required or optional.
        /// </summary>
        public enum Requirement
        {
            /// <summary>
            /// The feature is required, which means the app cannot be installed on devices that do not support the feature.
            /// </summary>
            Required,

            /// <summary>
            /// The feature is optional, which means the app can be installed on devices that do not support the feature.
            /// </summary>
            Optional
        }

        [SerializeField, Tooltip("Toggles whether ARCore is required for this app. Will make app only downloadable by devices with ARCore support if set to 'Required'.")]
        Requirement m_Requirement;

        /// <summary>
        /// Specifies whether ARCore is required or optional for this app.
        /// </summary>
        /// <remarks>
        /// Set to <see cref="Requirement.Required"/> if the app should only be downloadable by devices with ARCore support.
        /// </remarks>
        public Requirement requirement
        {
            get => m_Requirement;
            set
            {
                if (value != m_Requirement)
                {
                    m_Requirement = value;
                    EditorUtility.SetDirty(this);
                }
            }
        }

        [SerializeField, Tooltip("Toggles whether depth is required for this app. Will make app only downloadable by devices with depth support if set to 'Required'.")]
        Requirement m_Depth;

        /// <summary>
        /// Specifies whether depth is required or optional for this app.
        /// </summary>
        /// <remarks>
        /// Set to <see cref="Requirement.Required"/> if the app should only be downloadable by devices with depth support.
        /// </remarks>
        public Requirement depth
        {
            get => m_Depth;
            set
            {
                if (value != m_Depth)
                {
                    m_Depth = value;
                    EditorUtility.SetDirty(this);
                }
            }
        }

        [SerializeField, Tooltip("Toggles whether the Gradle version is validated during Player build.")]
        bool m_IgnoreGradleVersion;

        /// <summary>
        /// Whether to validate the Gradle version during a Player build.
        /// </summary>
        /// <remarks>
        /// When building an Android Player with ARCore enabled, this ARCore package checks the Gradle version and warns if it determines
        /// the Gradle version to be too low. You can suppress this check (and resulting warning notification) by setting
        /// `ignoreGradleVersion` to `true`.
        /// </remarks>
        public bool ignoreGradleVersion
        {
            get => m_IgnoreGradleVersion;
            set
            {
                if (value != m_IgnoreGradleVersion)
                {
                    m_IgnoreGradleVersion = value;
                    EditorUtility.SetDirty(this);
                }
            }
        }

        /// <summary>
        /// Gets the currently selected settings, or creates a default one if no <see cref="ARCoreSettings"/> has been set in Project Settings.
        /// </summary>
        /// <returns>The ARCore settings to use for the current Player build.</returns>
        /// <remarks>
        /// The current settings object, if one exists, is saved in the Project's XR Settings folder. You can also view
        /// and modify these settings in the **XR Plug-in Management** section of your **Project Settings** window.
        /// </remarks>
        public static ARCoreSettings GetOrCreateSettings()
        {
            var settings = currentSettings;
            if (settings != null)
                return settings;

            return CreateInstance<ARCoreSettings>();
        }

        /// <summary>
        /// The <see cref="ARCoreSettings"/> for the Project.
        /// </summary>
        /// <remarks>
        /// The current settings object, if one exists, is saved in the Project's XR Settings folder. You can also view
        /// and modify these settings in the **XR Plug-in Management** section of your **Project Settings** window.
        /// </remarks>
        public static ARCoreSettings currentSettings
        {
            get => EditorBuildSettings.TryGetConfigObject(k_SettingsKey, out ARCoreSettings settings) ? settings : null;

            set
            {
                if (value == null)
                {
                    EditorBuildSettings.RemoveConfigObject(k_SettingsKey);
                }
                else
                {
                    EditorBuildSettings.AddConfigObject(k_SettingsKey, value, true);
                }
            }
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        internal static bool TrySelect()
        {
            var settings = currentSettings;
            if (settings == null)
                return false;

            Selection.activeObject = settings;
            return true;
        }

        void Awake()
        {
            if (EditorBuildSettings.TryGetConfigObject(k_OldConfigObjectName, out ARCoreSettings result))
            {
                EditorBuildSettings.RemoveConfigObject(k_OldConfigObjectName);
            }
        }

        const string k_SettingsKey = "UnityEditor.XR.ARCore.ARCoreSettings";
        const string k_OldConfigObjectName = "com.unity.xr.arcore.PlayerSettings";
    }
}
