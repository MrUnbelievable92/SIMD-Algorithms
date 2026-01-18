using NUnit.Framework;

namespace SIMDAlgorithms.Tests
{
    public static class MinMax
    {
        [Test, Timeout(int.MaxValue)]
        public static void Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                array.SIMD_MinMax(out byte min, out byte max);

                Assert.AreEqual(min, array.SIMD_Minimum());
                Assert.AreEqual(max, array.SIMD_Maximum());
            },
            () => (byte)rng.NextUInt(byte.MinValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                array.SIMD_MinMax(out ushort min, out ushort max);

                Assert.AreEqual(min, array.SIMD_Minimum());
                Assert.AreEqual(max, array.SIMD_Maximum());
            },
            () => (ushort)rng.NextUInt(ushort.MinValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                array.SIMD_MinMax(out uint min, out uint max);

                Assert.AreEqual(min, array.SIMD_Minimum());
                Assert.AreEqual(max, array.SIMD_Maximum());
            },
            rng.NextUInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void ULong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                array.SIMD_MinMax(out ulong min, out ulong max);

                Assert.AreEqual(min, array.SIMD_Minimum());
                Assert.AreEqual(max, array.SIMD_Maximum());
            },
            () => ((ulong)rng.NextUInt() << 32) | rng.NextUInt());
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                array.SIMD_MinMax(out sbyte min, out sbyte max);

                Assert.AreEqual(min, array.SIMD_Minimum());
                Assert.AreEqual(max, array.SIMD_Maximum());
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                array.SIMD_MinMax(out short min, out short max);

                Assert.AreEqual(min, array.SIMD_Minimum());
                Assert.AreEqual(max, array.SIMD_Maximum());
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                array.SIMD_MinMax(out int min, out int max);

                Assert.AreEqual(min, array.SIMD_Minimum());
                Assert.AreEqual(max, array.SIMD_Maximum());
            },
            rng.NextInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Long()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                array.SIMD_MinMax(out long min, out long max);

                Assert.AreEqual(min, array.SIMD_Minimum());
                Assert.AreEqual(max, array.SIMD_Maximum());
            },
            () => (long)(((ulong)rng.NextUInt() << 32) | rng.NextUInt()));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Float()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                array.SIMD_MinMax(out float min, out float max);

                Assert.AreEqual(min, array.SIMD_Minimum());
                Assert.AreEqual(max, array.SIMD_Maximum());
            },
            () => rng.NextFloat(float.MinValue, float.MaxValue));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                array.SIMD_MinMax(out double min, out double max);

                Assert.AreEqual(min, array.SIMD_Minimum());
                Assert.AreEqual(max, array.SIMD_Maximum());
            },
            () => rng.NextDouble(double.MinValue / 2d, double.MaxValue / 2d));
        }
    }
}