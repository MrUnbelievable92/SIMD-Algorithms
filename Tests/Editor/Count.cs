using NUnit.Framework;
using Unity.Collections;

namespace SIMDAlgorithms.Tests
{
    public static class Count
    {
        [Test]
        public static void Bool()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<bool> array = new NativeArray<bool>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = rng.NextBool();
                }

                long std_true = 0;
                long std_false = 0;

                for (int j = 0; j < array.Length; j++)
                {
                    if (array[j] == true)
                    {
                        std_true++;
                    }
                    else
                    {
                        std_false++;
                    }
                }

                Assert.AreEqual(std_true, array.SIMD_Count());
                Assert.AreEqual(std_false, array.SIMD_Count(false));

                array.Dispose();
                array = new NativeArray<bool>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<byte> array = new NativeArray<byte>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);
                }

                for (int j = 0; j < 100; j++)
                {
                    long std = 0;
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
                }

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

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);
                }

                for (int j = 0; j < 100; j++)
                {
                    long std = 0;
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
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

                for (int j = 0; j < 100; j++)
                {
                    long std = 0;
                    uint test = rng.NextUInt();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    if (std == 0)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] == test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
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
            NativeArray<ulong> array = new NativeArray<ulong>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);
                }

                for (int j = 0; j < 100; j++)
                {
                    long std = 0;
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    if (std == 0)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] == test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
                }

                array.Dispose();
                array = new NativeArray<ulong>(rng.NextInt(0, 200000), Allocator.Persistent);
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

                for (int j = 0; j < 100; j++)
                {
                    long std = 0;
                    float test = rng.NextFloat();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    if (std == 0)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] == test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
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
            NativeArray<double> array = new NativeArray<double>(rng.NextInt(0, 200000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = rng.NextDouble();
                }

                for (int j = 0; j < 100; j++)
                {
                    long std = 0;
                    double test = rng.NextDouble();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    if (std == 0)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] == test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
                }

                array.Dispose();
                array = new NativeArray<double>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }
    }
}