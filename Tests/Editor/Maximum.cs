using NUnit.Framework;
using Unity.Mathematics;

namespace SIMDAlgorithms.Tests
{
    public static class Maximum
    {
        [Test, Timeout(int.MaxValue)]
        public static void Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                byte x = byte.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = (byte)math.max((uint)x, (uint)array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());
            },
            () => (byte)rng.NextUInt(byte.MaxValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                ushort x = ushort.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = (ushort)math.max((uint)x, (uint)array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());
            },
            () => (ushort)rng.NextUInt(ushort.MaxValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                uint x = uint.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.max(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());
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
                ulong x = ulong.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.max(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());
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
                sbyte x = sbyte.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = (sbyte)math.max((int)x, (int)array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());
            },
            () => (sbyte)rng.NextInt(sbyte.MaxValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                short x = short.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = (short)math.max((int)x, (int)array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());
            },
            () => (short)rng.NextInt(short.MaxValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                int x = int.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.max(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());
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
                long x = long.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.max(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());
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
                float x = float.NegativeInfinity;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.max(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());
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
                double x = double.NegativeInfinity;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.max(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());
            },
            rng.NextDouble);
        }
    }
}