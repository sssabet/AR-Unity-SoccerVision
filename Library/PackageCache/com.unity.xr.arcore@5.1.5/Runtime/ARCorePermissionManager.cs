using System;
using UnityEngine.Scripting;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// Manages Android permissions for the application.
    /// Allows you to determine whether a permission has been
    /// granted and request permission.
    /// </summary>
    public class ARCorePermissionManager : AndroidJavaProxy
    {
        const string k_IsPermissionGrantedString = "IsPermissionGranted";
        const string k_AndroidPermissionsClass = "com.unity3d.plugin.UnityAndroidPermissions$IPermissionRequestResult";
        const string k_AndroidPermissionService = "com.unity3d.plugin.UnityAndroidPermissions";

        static ARCorePermissionManager s_Instance;
        static AndroidJavaObject s_Activity;
        static AndroidJavaObject s_PermissionService;
        static Action<string, bool> s_CurrentCallback;

#if UNITY_2022_2_OR_NEWER
        static IntPtr s_IsPermissionGrantedMethodId;
#endif

        /// <summary>
        /// Checks if an Android permission is granted to the application.
        /// </summary>
        /// <param name="permissionName">The full name of the Android permission to check (e.g.
        /// android.permission.CAMERA).</param>
        /// <returns><see langword="true"/> if <paramref name="permissionName"/> is granted to the application.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool IsPermissionGranted(string permissionName)
        {
            if (Application.isEditor)
                return true;
            
#if UNITY_2022_2_OR_NEWER
            if (s_IsPermissionGrantedMethodId == IntPtr.Zero)
            {
                var androidPermissionClass = new AndroidJavaClass(k_AndroidPermissionService).GetRawClass();
                object[] args = { activity, permissionName };
                s_IsPermissionGrantedMethodId = AndroidJNIHelper.GetMethodID<bool>(androidPermissionClass, k_IsPermissionGrantedString, args, false);
            }

            return permissionsService.Call<bool>(s_IsPermissionGrantedMethodId, activity, permissionName);
#else
            return permissionsService.Call<bool>(k_IsPermissionGrantedString, activity, permissionName);
#endif
        }

        /// <summary>
        /// Requests an Android permission from the user.
        /// </summary>
        /// <param name="permissionName">The permission to request (e.g. android.permission.CAMERA).</param>
        /// <param name="callback">A delegate to invoke when the permission has been granted or denied. The
        /// parameters of the delegate are the <paramref name="permissionName"/> being requested and a <c>bool</c>
        /// indicating whether permission was granted.</param>
        public static void RequestPermission(string permissionName, Action<string, bool> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            if (IsPermissionGranted(permissionName))
            {
                callback(permissionName, true);
                return;
            }

            if (s_CurrentCallback != null)
                throw new InvalidOperationException("Cannot start a new permissions request before the current one finishes.");

            permissionsService.Call("RequestPermissionAsync", activity, new[] { permissionName }, instance);
            s_CurrentCallback = callback;
        }

        /// <summary>
        /// Cancels any pending Android permission requests.
        /// </summary>
        public static void CancelPermissionRequest()
        {
            s_CurrentCallback = null;
        }

        // UnityAndroidPermissions interface
        [Preserve]
        void OnPermissionGranted(string permissionName)
        {
            s_CurrentCallback(permissionName, true);
            s_CurrentCallback = null;
        }

        // UnityAndroidPermissions interface
        [Preserve]
        void OnPermissionDenied(string permissionName)
        {
            s_CurrentCallback(permissionName, false);
            s_CurrentCallback = null;
        }

        // UnityAndroidPermissions interface (unused)
        [Preserve]
        void OnActivityResult() { }

        ARCorePermissionManager() : base(k_AndroidPermissionsClass)
        { }

        static ARCorePermissionManager instance => s_Instance ??= new ARCorePermissionManager();

        static AndroidJavaObject activity
        {
            get
            {
                if (s_Activity == null)
                {
                    var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    s_Activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }

                return s_Activity;
            }
        }

        static AndroidJavaObject permissionsService =>
            s_PermissionService ??= new AndroidJavaObject(k_AndroidPermissionService);
    }
}
