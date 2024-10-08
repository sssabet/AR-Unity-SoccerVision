using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Cinfo = UnityEngine.XR.Simulation.SimulationImageInfo.Cinfo;
using ImageType = UnityEngine.XR.Simulation.SimulationXRCpuImageApi.ImageType;

namespace UnityEngine.XR.Simulation
{
    
    [StructLayout(LayoutKind.Sequential)]
    struct SimulationPlatformInterface
    {
        internal struct Delegates
        {
            public delegate bool TryGetImageDataDelegate(IntPtr nativeHandle, out Cinfo imageData);
            public delegate IntPtr AcquireImageDelegate(ImageType imageType, out double timestamp);
            public delegate void ReleaseImageDelegate(IntPtr imageHandle);
            public delegate void LogDelegate(IntPtr messagePtr);
            public delegate int GetAcquiredImageCountDelegate();
            

            public TryGetImageDataDelegate TryGetImageData;
            public AcquireImageDelegate AcquireImage;
            public ReleaseImageDelegate ReleaseImage;
            public LogDelegate Log;
            public GetAcquiredImageCountDelegate GetAcquiredImageCount;
        }

        internal SimulationPlatformInterface(in Delegates delegates)
        {
            m_TryGetImageDataFuncPtr = GetFunctionPointerForDelegate(delegates.TryGetImageData);
            m_AcquireImageFuncPtr = GetFunctionPointerForDelegate(delegates.AcquireImage);
            m_ReleaseImageFuncPtr = GetFunctionPointerForDelegate(delegates.ReleaseImage);
            m_LogFuncPtr = GetFunctionPointerForDelegate(delegates.Log);
            m_GetAcquireImageCountFuncPtr = GetFunctionPointerForDelegate(delegates.GetAcquiredImageCount);
        }

        readonly IntPtr m_TryGetImageDataFuncPtr;
        readonly IntPtr m_AcquireImageFuncPtr;
        readonly IntPtr m_ReleaseImageFuncPtr;
        readonly IntPtr m_LogFuncPtr;
        readonly IntPtr m_GetAcquireImageCountFuncPtr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IntPtr GetFunctionPointerForDelegate(Delegate d)
        {
            return d != null ? Marshal.GetFunctionPointerForDelegate(d) : IntPtr.Zero;
        }
    }
}
