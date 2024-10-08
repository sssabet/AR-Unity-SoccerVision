using System;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Helper for compile-time constant strings for the [HelpURL](xref:UnityEngine.HelpURLAttribute) attribute.
    /// </summary>
    class HelpURLAttribute : UnityEngine.HelpURLAttribute
    {
        const string k_Version = "5.1";

        public HelpURLAttribute(Type type)
            : base($"https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@{k_Version}/api/{type.FullName}.html")
        { }
    }
}
