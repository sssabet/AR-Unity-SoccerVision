using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;

namespace UnityEngine.XR.ARSubsystems.Tests
{
    [TestFixture]
    class NativeCopyUtilityTestFixture
    {
        struct TwoFloats
        {
            public TwoFloats(float a, float b)
            {
                this.a = a;
                this.b = b;
            }

            public bool Equals(TwoFloats other)
            {
                return a.Equals(other.a) && b.Equals(other.b);
            }

            public float a;
            public float b;
        }

        // TrackableChangesTests.TestTrackableChangesCopiesPointers already covers
        // NativeCopyUtility.PtrToNativeArrayWithDefault usage fairly heavily.

        [TestCase(Allocator.Temp)]
        [TestCase(Allocator.TempJob)]
        [TestCase(Allocator.Persistent)]
        public void FillArrayWithValueTest(Allocator allocator)
        {
            const int testLength = 7;
            var dst = new NativeArray<TwoFloats>(testLength, allocator);

            try
            {
                var defaultValue = new TwoFloats(float.NegativeInfinity, float.PositiveInfinity);
                NativeCopyUtility.FillArrayWithValue(dst, defaultValue);

                for (var i = 0; i < testLength; i++)
                {
                    Assert.IsTrue(dst[i].Equals(defaultValue));
                }
            }
            finally
            {
                dst.Dispose();
            }
        }

        [TestCase(Allocator.Temp)]
        [TestCase(Allocator.TempJob)]
        [TestCase(Allocator.Persistent)]
        public void CreateArrayFilledWithValue(Allocator allocator)
        {
            NativeArray<TwoFloats> dst = default;

            try
            {
                const int testLength = 21;
                var defaultValue = new TwoFloats(1.0f, 137.0f);
                dst = NativeCopyUtility.CreateArrayFilledWithValue(defaultValue, testLength, allocator);

                for (var i = 0; i < testLength; i++)
                {
                    Assert.IsTrue(dst[i].Equals(defaultValue));
                }
            }
            finally
            {
                if(dst.IsCreated)
                    dst.Dispose();
            }
        }
        
        [TestCase(Allocator.Temp)]
        [TestCase(Allocator.TempJob)]
        [TestCase(Allocator.Persistent)]
        public void CopyFromReadOnlyCollection(Allocator allocator)
        {
            const int testLength = 42;
            var src = new List<TwoFloats>(42);
            for (var i = 0; i < testLength; i++)
            {
                src.Add(new TwoFloats(-i, i * 2));
            }

            NativeArray<TwoFloats> dst = default;

            try
            {
                dst = new NativeArray<TwoFloats>(src.Count, allocator, NativeArrayOptions.UninitializedMemory);
                NativeCopyUtility.CopyFromReadOnlyCollection(src, dst);

                for (var i = 0; i < testLength; i++)
                {
                    Assert.IsTrue(src[i].Equals(dst[i]));
                }
            }
            finally
            {
                if (dst.IsCreated)
                    dst.Dispose();
            }
        }
        
        [TestCase(Allocator.Temp)]
        [TestCase(Allocator.TempJob)]
        [TestCase(Allocator.Persistent)]
        public void CopyFromReadOnlyList(Allocator allocator)
        {
            const int testLength = 42;
            var src = new List<TwoFloats>(42);
            for (var i = 0; i < testLength; i++)
            {
                src.Add(new TwoFloats(-i, i * 2));
            }

            NativeArray<TwoFloats> dst = default;

            try
            {
                dst = new NativeArray<TwoFloats>(src.Count, allocator, NativeArrayOptions.UninitializedMemory);
                NativeCopyUtility.CopyFromReadOnlyList(src.AsReadOnly(), dst);

                for (var i = 0; i < testLength; i++)
                {
                    Assert.IsTrue(src[i].Equals(dst[i]));
                }
            }
            finally
            {
                if (dst.IsCreated)
                    dst.Dispose();
            }
        }
    }
}
