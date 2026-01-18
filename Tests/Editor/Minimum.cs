using NUnit.Framework;
using Unity.Mathematics;

namespace SIMDAlgorithms.Tests
{
    public static class Minimum
    {
        [Test, Timeout(int.MaxValue)]
        public static void Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                byte x = byte.MaxValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = (byte)math.min((uint)x, (uint)array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Minimum());
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
                ushort x = ushort.MaxValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = (ushort)math.min((uint)x, (uint)array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Minimum());
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
                uint x = uint.MaxValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.min(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Minimum());
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
                ulong x = ulong.MaxValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.min(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Minimum());
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                sbyte x = sbyte.MaxValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = (sbyte)math.min((int)x, (int)array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Minimum());
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
                short x = short.MaxValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = (short)math.min((int)x, (int)array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Minimum());
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
                int x = int.MaxValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.min(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Minimum());
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
                long x = long.MaxValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.min(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Minimum());
            },
            () => (long)rng.NextInt() | ((long)rng.NextInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Float()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                float x = float.PositiveInfinity;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.min(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Minimum());
            },
            rng.NextFloat);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                double x = double.PositiveInfinity;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.min(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Minimum());
            },
            rng.NextDouble);
        }
    }
}