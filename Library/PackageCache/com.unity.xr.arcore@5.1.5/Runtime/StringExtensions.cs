using System;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.ARCore
{
    static class StringExtensions
    {
        public static unsafe NativeArray<byte> ToBytes(this string @string, Encoding encoding, Allocator allocator)
        {
            if (@string == null)
                throw new ArgumentNullException(nameof(@string));

            var byteCount = encoding.GetByteCount(@string);
            var bytes = new NativeArray<byte>(byteCount + 1, allocator);
            fixed (char* chars = @string)
            {
                encoding.GetBytes(chars, @string.Length, (byte*)bytes.GetUnsafePtr(), byteCount);
            }

            return bytes;
        }
    }
}
