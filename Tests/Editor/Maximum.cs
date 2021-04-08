using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;

namespace SIMDAlgorithms.Tests
{
    public static class Maximum
    {
        [Test]
        public static void Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<byte> array = new NativeArray<byte>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < 10 * CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = (byte)rng.NextUInt(byte.MinValue, byte.MaxValue + 1);
                }

                byte x = byte.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = (byte)math.max((uint)x, (uint)array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());

                array.Dispose();
                array = new NativeArray<byte>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void UShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<ushort> array = new NativeArray<ushort>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < 10 * CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = (ushort)rng.NextUInt(ushort.MinValue, ushort.MaxValue + 1);
                }

                ushort x = ushort.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = (ushort)math.max((uint)x, (uint)array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());

                array.Dispose();
                array = new NativeArray<ushort>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void UInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<uint> array = new NativeArray<uint>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < 10 * CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = rng.NextUInt();
                }

                uint x = uint.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.max(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());

                array.Dispose();
                array = new NativeArray<uint>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void ULong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<ulong> array = new NativeArray<ulong>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < 10 * CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);
                }

                ulong x = ulong.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.max(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());

                array.Dispose();
                array = new NativeArray<ulong>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void SByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<sbyte> array = new NativeArray<sbyte>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < 10 * CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);
                }

                sbyte x = sbyte.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = (sbyte)math.max((int)x, (int)array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());

                array.Dispose();
                array = new NativeArray<sbyte>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void Short()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<short> array = new NativeArray<short>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < 10 * CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);
                }

                short x = short.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = (short)math.max((int)x, (int)array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());

                array.Dispose();
                array = new NativeArray<short>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void Int()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<int> array = new NativeArray<int>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < 10 * CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = rng.NextInt();
                }

                int x = int.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.max(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());

                array.Dispose();
                array = new NativeArray<int>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void Long()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<long> array = new NativeArray<long>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < 10 * CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = rng.NextInt() | ((long)rng.NextInt() << 32);
                }

                long x = long.MinValue;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.max(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());

                array.Dispose();
                array = new NativeArray<long>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void Float()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<float> array = new NativeArray<float>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < 10 * CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = rng.NextFloat();
                }

                float x = float.NegativeInfinity;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.max(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());

                array.Dispose();
                array = new NativeArray<float>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void Double()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<double> array = new NativeArray<double>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < 10 * CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = rng.NextDouble();
                }

                double x = double.NegativeInfinity;

                for (int j = 0; j < array.Length; j++)
                {
                    x = math.max(x, array[j]);
                }

                Assert.AreEqual(x, array.SIMD_Maximum());

                array.Dispose();
                array = new NativeArray<double>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }
    }
}