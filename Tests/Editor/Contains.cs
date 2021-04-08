using NUnit.Framework;
using Unity.Collections;

namespace SIMDAlgorithms.Tests
{
    public static class Contains
    {
        [Test]
        public static void Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<byte> array = new NativeArray<byte>(rng.NextInt(0, 20000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);
                }
                ;
                for (int j = 0; j < 10; j++)
                {
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }

                array.Dispose();
                array = new NativeArray<byte>(rng.NextInt(0, 20000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void UShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<ushort> array = new NativeArray<ushort>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);
                }

                for (int j = 0; j < 10; j++)
                {
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }

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

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = rng.NextUInt();
                }

                for (int j = 0; j < 50; j++)
                {
                    uint test = rng.NextUInt();

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }

                array.Dispose();
                array = new NativeArray<uint>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void ULong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<ulong> array = new NativeArray<ulong>(rng.NextInt(0, 2000000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);
                }

                for (int j = 0; j < 100; j++)
                {
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }

                array.Dispose();
                array = new NativeArray<ulong>(rng.NextInt(0, 2000000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void Float()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<float> array = new NativeArray<float>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = rng.NextFloat();
                }

                for (int j = 0; j < 50; j++)
                {
                    float test = rng.NextFloat();

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }

                array.Dispose();
                array = new NativeArray<float>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void Double()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<double> array = new NativeArray<double>(rng.NextInt(0, 2000000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = rng.NextDouble();
                }

                for (int j = 0; j < 100; j++)
                {
                    double test = rng.NextDouble();

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }

                array.Dispose();
                array = new NativeArray<double>(rng.NextInt(0, 2000000), Allocator.Persistent);
            }

            array.Dispose();
        }
    }
}