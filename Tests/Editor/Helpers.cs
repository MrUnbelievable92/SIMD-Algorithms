using System;
using Unity.Collections;
using Unity.Jobs;

namespace SIMDAlgorithms.Tests
{
    public static class Helpers
    {
        public const int NUM_TESTS = 4;


        public static uint GetRngSeed { get { long t = Environment.TickCount; return (uint)t != 0 ? (uint)t : 1; } }


        public static void Test<T>(Action<NativeArray<T>> test, Func<T> generateElement, int maxLength = 5000000, int numTests = NUM_TESTS)
            where T : unmanaged
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(GetRngSeed);
            NativeArray<T> array = new NativeArray<T>(rng.NextInt(0, maxLength), Allocator.Persistent);

            for (int i = 0; i < numTests; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = generateElement();
                }

                test(array);

                array.Dispose(default(JobHandle));
                array = new NativeArray<T>(rng.NextInt(0, maxLength), Allocator.Persistent);
            }

            array.Dispose(default(JobHandle));
        }
    }
}