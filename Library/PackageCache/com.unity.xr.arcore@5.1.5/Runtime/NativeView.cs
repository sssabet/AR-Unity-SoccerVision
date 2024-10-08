using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// Similar to NativeSlice but blittable. Provides a "view"
    /// into a contiguous array of memory. Used to interop with C.
    /// </summary>
    unsafe struct NativeView
    {
        public void* ptr;
        public int count;
    }

    static class NativeViewExtensions
    {
        public static unsafe NativeView AsNativeView<T>(this NativeArray<T> array) where T : struct => new NativeView
        {
            ptr = array.GetUnsafePtr(),
            count = array.Length
        };

        public static unsafe NativeView AsNativeView<T>(this NativeSlice<T> slice) where T : struct => new NativeView
        {
            ptr = slice.GetUnsafePtr(),
            count = slice.Length
        };
    }
}
