using NUnit.Framework;
using Unity.Collections;
using UnityEngine.AI;

namespace SIMDAlgorithms.Tests
{
    public static class BitsEqual
    {
        [Test, Timeout(int.MaxValue)]
        public static void AllTests()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 10; i++)
            {
                int length = rng.NextInt(0, 500);

                NativeArray<byte> lhs = new NativeArray<byte>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                NativeArray<byte> rhs = new NativeArray<byte>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

                for (int j = 0; j < length; j++)
                {
                    lhs[j] = rhs[j] = (byte)rng.NextInt(0, 256);
                }

                int rngIndex = rng.NextInt(0, length);
                int rngCount = rng.NextInt(0, length - rngIndex);

                Assert.IsTrue(lhs.SIMD_BitsEqual(rhs, traversalOrder: TraversalOrder.Ascending));
                Assert.IsTrue(lhs.SIMD_BitsEqual(rhs, rngIndex, rngIndex, TraversalOrder.Ascending));
                Assert.IsTrue(lhs.SIMD_BitsEqual(rhs, rngCount, rngIndex, rngIndex, TraversalOrder.Ascending));

                Assert.IsTrue(lhs.SIMD_BitsEqual(rhs, traversalOrder: TraversalOrder.Descending));
                Assert.IsTrue(lhs.SIMD_BitsEqual(rhs, rngIndex, rngIndex, TraversalOrder.Descending));
                Assert.IsTrue(lhs.SIMD_BitsEqual(rhs, rngCount, rngIndex, rngIndex, TraversalOrder.Descending));

                int changedIndex = rng.NextInt(rngIndex, rngIndex + rngCount);

                if (changedIndex < length)
                {
                    lhs[changedIndex] += 1;

                    Assert.IsFalse(lhs.SIMD_BitsEqual(rhs, traversalOrder: TraversalOrder.Ascending));
                    //Assert.IsFalse(lhs.SIMD_BitsEqual(rhs, rngIndex, rngIndex, TraversalOrder.Ascending));
                    //Assert.IsFalse(lhs.SIMD_BitsEqual(rhs, rngCount, rngIndex, rngIndex, TraversalOrder.Ascending));

                    Assert.IsFalse(lhs.SIMD_BitsEqual(rhs, traversalOrder: TraversalOrder.Descending));
                    //Assert.IsFalse(lhs.SIMD_BitsEqual(rhs, rngIndex, rngIndex, TraversalOrder.Descending));
                    //Assert.IsFalse(lhs.SIMD_BitsEqual(rhs, rngCount, rngIndex, rngIndex, TraversalOrder.Descending));
                }

                lhs.Dispose(default);
                rhs.Dispose(default);
            }
        }

    }
}