using System;
using System.Runtime.InteropServices;
using System.Text;

namespace UnityEngine.XR.ARCore
{
    unsafe struct ArString : IDisposable
    {
        byte* m_Bytes;

        public override string ToString() => m_Bytes == null
            ? string.Empty
            : Marshal.PtrToStringAnsi(new IntPtr(m_Bytes));

        public string ToString(Encoding encoding) => m_Bytes == null
            ? null
            : encoding.GetString(m_Bytes, CalculateNullTerminatedByteCount());

        public void Dispose()
        {
            if (m_Bytes != null)
            {
                Release(m_Bytes);
            }

            m_Bytes = null;
        }

        public int CalculateNullTerminatedByteCount()
        {
            if (m_Bytes == null)
            {
                return 0;
            }

            var bytes = m_Bytes;
            var byteCount = 0;
            while (*bytes++ != 0)
            {
                byteCount++;
            }

            return byteCount;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        [DllImport("arcore_sdk_c", EntryPoint = "ArString_release")]
        static extern void Release(byte* str);
#else
        static void Release(byte* str) { }
#endif
    }
}
