using System;
using System.Runtime.CompilerServices;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.ARFoundation.InternalUtils
{
    static class SerializableGuidUtility
    {
        /// <summary>Given a <see cref="System.Guid"/>, resolves it into the equivalent SerializableGuid.</summary>
        /// <param name="guid">The guid to resolve.</param>
        /// <returns>A <see cref="UnityEngine.XR.ARSubsystems.SerializableGuid"/> containing the same data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ARSubsystems.SerializableGuid AsSerializedGuid(Guid guid)
        {
            guid.Decompose(out var low, out var high);
            return new ARSubsystems.SerializableGuid(low, high);
        }
    }
}
