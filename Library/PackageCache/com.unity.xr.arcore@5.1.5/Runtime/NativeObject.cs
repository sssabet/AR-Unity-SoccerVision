using System;

namespace UnityEngine.XR.ARCore
{
    static class NativeObject
    {
        public static bool ArePointersEqual(IntPtr? lhs, IntPtr? rhs)
        {
            // Both non null; compare pointers
            if (lhs.HasValue && rhs.HasValue)
                return lhs.Value == rhs.Value;

            // rhs is null
            if (lhs.HasValue)
                return lhs.Value == IntPtr.Zero;

            // lhs is null
            if (rhs.HasValue)
                return rhs.Value == IntPtr.Zero;

            // both null
            return true;
        }
    }
}
