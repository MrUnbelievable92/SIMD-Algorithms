using NUnit.Framework;

namespace SIMDAlgorithms.Tests
{
    public static class Sort
    {
        [Test, Timeout(int.MaxValue)]
        public static void Byte()
        {
            uint seed = Helpers.GetRngSeed;
            seed = seed == 0 ? 1 : seed;

            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(seed);

            for (int i = 0; i < 16; i++)
            {
                Helpers.Test<byte>(
                (array) =>
                {
                    if (!array.SIMD_IsSorted())
                    {
                        array.SIMD_Sort();
                        Assert.IsTrue(array.SIMD_IsSorted());
                    }
                },
                () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
                maxLength: 20000);

                Helpers.Test<byte>(
                (array) =>
                {
                    if (!array.SIMD_IsSorted())
                    {
                        array.SIMD_Sort();
                        Assert.IsTrue(array.SIMD_IsSorted());
                    }
                },
                () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
                maxLength: 499);
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte()
        {
            uint seed = Helpers.GetRngSeed;
            seed = seed == 0 ? 1 : seed;

            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(seed);

            for (int i = 0; i < 16; i++)
            {
                Helpers.Test<sbyte>(
                (array) =>
                {
                    if (!array.SIMD_IsSorted())
                    {
                        array.SIMD_Sort();
                        Assert.IsTrue(array.SIMD_IsSorted());
                    }
                },
                () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1),
                maxLength: 20000);

                Helpers.Test<sbyte>(
                (array) =>
                {
                    if (!array.SIMD_IsSorted())
                    {
                        array.SIMD_Sort();
                        Assert.IsTrue(array.SIMD_IsSorted());
                    }
                },
                () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1),
                maxLength: 499);
            }
        }
    }
}
