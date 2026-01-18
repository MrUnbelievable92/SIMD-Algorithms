using NUnit.Framework;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using System.Linq;
using Unity.Jobs;

namespace SIMDAlgorithms.Tests
{
    public static class IsSorted
    {
        [Test, Timeout(int.MaxValue)]
        public static void Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 200; i++)
            {
                // test all remainders
                int length = rng.NextInt(20, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<byte> test = new NativeArray<byte>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                byte[] cpy = new byte[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (byte)rng.NextInt(0, byte.MaxValue + 1);
                }

                Assert.IsFalse(test.SIMD_IsSorted());
                test.Sort();
                Assert.IsTrue(test.SIMD_IsSorted());

                byte previous = test[0];
                test[0] = byte.MaxValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[0] = previous;
                previous = test[length - 1];
                test[length - 1] = byte.MinValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[length - 1] = previous;



                int randomInt = rng.NextInt(5, 20);
                bool sameVal = true;
                byte val = test[test.Length - randomInt];
                for (int j = test.Length - randomInt + 1; j < test.Length; j++)
                {
                    sameVal &= (val == test[j]);
                }
                if (!sameVal)
                {
                    test.SIMD_Reverse(test.Length - randomInt);
                    Assert.IsFalse(test.SIMD_IsSorted());
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 200; i++)
            {
                // test all remainders
                int length = rng.NextInt(20, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<ushort> test = new NativeArray<ushort>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                ushort[] cpy = new ushort[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (ushort)rng.NextInt(0, ushort.MaxValue + 1);
                }

                for (int j = 0; j < length - 1; j++)
                {
                    if (test[j] > test[j + 1])
                    {
                        ushort t = test[j];
                        test[j] = test[j + 1];
                        test[j + 1] = t;

                        break;
                    }
                }

                Assert.IsFalse(test.SIMD_IsSorted());
                test.Sort();
                Assert.IsTrue(test.SIMD_IsSorted());

                ushort previous = test[0];
                test[0] = ushort.MaxValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[0] = previous;
                previous = test[length - 1];
                test[length - 1] = ushort.MinValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[length - 1] = previous;

                int randomInt = rng.NextInt(5, 20);
                bool sameVal = true;
                ushort val = test[test.Length - randomInt];
                for (int j = test.Length - randomInt + 1; j < test.Length; j++)
                {
                    sameVal &= (val == test[j]);
                }
                if (!sameVal)
                {
                    test.SIMD_Reverse(test.Length - randomInt);
                    Assert.IsFalse(test.SIMD_IsSorted());
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 200; i++)
            {
                // test all remainders
                int length = rng.NextInt(20, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<uint> test = new NativeArray<uint>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                uint[] cpy = new uint[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (uint)rng.NextInt();
                }

                for (int j = 0; j < length - 1; j++)
                {
                    if (test[j] > test[j + 1])
                    {
                        uint t = test[j];
                        test[j] = test[j + 1];
                        test[j + 1] = t;

                        break;
                    }
                }

                Assert.IsFalse(test.SIMD_IsSorted());
                test.Sort();
                Assert.IsTrue(test.SIMD_IsSorted());

                uint previous = test[0];
                test[0] = uint.MaxValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[0] = previous;
                previous = test[length - 1];
                test[length - 1] = uint.MinValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[length - 1] = previous;

                int randomInt = rng.NextInt(5, 20);
                bool sameVal = true;
                uint val = test[test.Length - randomInt];
                for (int j = test.Length - randomInt + 1; j < test.Length; j++)
                {
                    sameVal &= (val == test[j]);
                }
                if (!sameVal)
                {
                    test.SIMD_Reverse(test.Length - randomInt);
                    Assert.IsFalse(test.SIMD_IsSorted());
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void ULong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 200; i++)
            {
                // test all remainders
                int length = rng.NextInt(20, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<ulong> test = new NativeArray<ulong>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                ulong[] cpy = new ulong[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (ulong)rng.NextInt();
                }

                for (int j = 0; j < length - 1; j++)
                {
                    if (test[j] > test[j + 1])
                    {
                        ulong t = test[j];
                        test[j] = test[j + 1];
                        test[j + 1] = t;

                        break;
                    }
                }

                Assert.IsFalse(test.SIMD_IsSorted());
                test.Sort();
                Assert.IsTrue(test.SIMD_IsSorted());

                ulong previous = test[0];
                test[0] = ulong.MaxValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[0] = previous;
                previous = test[length - 1];
                test[length - 1] = ulong.MinValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[length - 1] = previous;

                int randomInt = rng.NextInt(5, 20);
                bool sameVal = true;
                ulong val = test[test.Length - randomInt];
                for (int j = test.Length - randomInt + 1; j < test.Length; j++)
                {
                    sameVal &= (val == test[j]);
                }
                if (!sameVal)
                {
                    test.SIMD_Reverse(test.Length - randomInt);
                    Assert.IsFalse(test.SIMD_IsSorted());
                }

                test.Dispose(default(JobHandle));
            }
        }


        [Test, Timeout(int.MaxValue)]
        public static void SByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 200; i++)
            {
                // test all remainders
                int length = rng.NextInt(20, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<sbyte> test = new NativeArray<sbyte>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                sbyte[] cpy = new sbyte[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);
                }

                Assert.IsFalse(test.SIMD_IsSorted());
                test.Sort();
                Assert.IsTrue(test.SIMD_IsSorted());

                sbyte previous = test[0];
                test[0] = sbyte.MaxValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[0] = previous;
                previous = test[length - 1];
                test[length - 1] = sbyte.MinValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[length - 1] = previous;



                int randomInt = rng.NextInt(5, 20);
                bool sameVal = true;
                sbyte val = test[test.Length - randomInt];
                for (int j = test.Length - randomInt + 1; j < test.Length; j++)
                {
                    sameVal &= (val == test[j]);
                }
                if (!sameVal)
                {
                    test.SIMD_Reverse(test.Length - randomInt);
                    Assert.IsFalse(test.SIMD_IsSorted());
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 200; i++)
            {
                // test all remainders
                int length = rng.NextInt(20, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<short> test = new NativeArray<short>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                short[] cpy = new short[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);
                }

                for (int j = 0; j < length - 1; j++)
                {
                    if (test[j] > test[j + 1])
                    {
                        short t = test[j];
                        test[j] = test[j + 1];
                        test[j + 1] = t;

                        break;
                    }
                }

                Assert.IsFalse(test.SIMD_IsSorted());
                test.Sort();
                Assert.IsTrue(test.SIMD_IsSorted());

                short previous = test[0];
                test[0] = short.MaxValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[0] = previous;
                previous = test[length - 1];
                test[length - 1] = short.MinValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[length - 1] = previous;

                int randomInt = rng.NextInt(5, 20);
                bool sameVal = true;
                short val = test[test.Length - randomInt];
                for (int j = test.Length - randomInt + 1; j < test.Length; j++)
                {
                    sameVal &= (val == test[j]);
                }
                if (!sameVal)
                {
                    test.SIMD_Reverse(test.Length - randomInt);
                    Assert.IsFalse(test.SIMD_IsSorted());
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 200; i++)
            {
                // test all remainders
                int length = rng.NextInt(20, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<int> test = new NativeArray<int>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                int[] cpy = new int[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (int)rng.NextInt();
                }

                for (int j = 0; j < length - 1; j++)
                {
                    if (test[j] > test[j + 1])
                    {
                        int t = test[j];
                        test[j] = test[j + 1];
                        test[j + 1] = t;

                        break;
                    }
                }

                Assert.IsFalse(test.SIMD_IsSorted());
                test.Sort();
                Assert.IsTrue(test.SIMD_IsSorted());

                int previous = test[0];
                test[0] = int.MaxValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[0] = previous;
                previous = test[length - 1];
                test[length - 1] = int.MinValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[length - 1] = previous;

                int randomInt = rng.NextInt(5, 20);
                bool sameVal = true;
                int val = test[test.Length - randomInt];
                for (int j = test.Length - randomInt + 1; j < test.Length; j++)
                {
                    sameVal &= (val == test[j]);
                }
                if (!sameVal)
                {
                    test.SIMD_Reverse(test.Length - randomInt);
                    Assert.IsFalse(test.SIMD_IsSorted());
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void Long()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 200; i++)
            {
                // test all remainders
                int length = rng.NextInt(20, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<long> test = new NativeArray<long>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                long[] cpy = new long[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (long)rng.NextInt();
                }

                for (int j = 0; j < length - 1; j++)
                {
                    if (test[j] > test[j + 1])
                    {
                        long t = test[j];
                        test[j] = test[j + 1];
                        test[j + 1] = t;

                        break;
                    }
                }

                Assert.IsFalse(test.SIMD_IsSorted());
                test.Sort();
                Assert.IsTrue(test.SIMD_IsSorted());

                long previous = test[0];
                test[0] = long.MaxValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[0] = previous;
                previous = test[length - 1];
                test[length - 1] = long.MinValue;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[length - 1] = previous;

                int randomInt = rng.NextInt(5, 20);
                bool sameVal = true;
                long val = test[test.Length - randomInt];
                for (int j = test.Length - randomInt + 1; j < test.Length; j++)
                {
                    sameVal &= (val == test[j]);
                }
                if (!sameVal)
                {
                    test.SIMD_Reverse(test.Length - randomInt);
                    Assert.IsFalse(test.SIMD_IsSorted());
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void Float()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 200; i++)
            {
                // test all remainders
                int length = rng.NextInt(20, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<float> test = new NativeArray<float>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                float[] cpy = new float[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (float)rng.NextFloat(0, float.MaxValue / 2);
                }

                for (int j = 0; j < length - 1; j++)
                {
                    if (test[j] > test[j + 1])
                    {
                        float t = test[j];
                        test[j] = test[j + 1];
                        test[j + 1] = t;

                        break;
                    }
                }

                Assert.IsFalse(test.SIMD_IsSorted());
                test.Sort();
                Assert.IsTrue(test.SIMD_IsSorted());

                float previous = test[0];
                test[0] = float.PositiveInfinity;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[0] = previous;
                previous = test[length - 1];
                test[length - 1] = float.NegativeInfinity;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[length - 1] = previous;

                int randomInt = rng.NextInt(5, 20);
                bool sameVal = true;
                float val = test[test.Length - randomInt];
                for (int j = test.Length - randomInt + 1; j < test.Length; j++)
                {
                    sameVal &= (val == test[j]);
                }
                if (!sameVal)
                {
                    test.SIMD_Reverse(test.Length - randomInt);
                    Assert.IsFalse(test.SIMD_IsSorted());
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 200; i++)
            {
                // test all remainders
                int length = rng.NextInt(20, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<double> test = new NativeArray<double>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                double[] cpy = new double[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (double)rng.NextDouble(0, double.MaxValue / 2);
                }

                for (int j = 0; j < length - 1; j++)
                {
                    if (test[j] > test[j + 1])
                    {
                        double t = test[j];
                        test[j] = test[j + 1];
                        test[j + 1] = t;

                        break;
                    }
                }

                Assert.IsFalse(test.SIMD_IsSorted());
                test.Sort();
                Assert.IsTrue(test.SIMD_IsSorted());

                double previous = test[0];
                test[0] = double.PositiveInfinity;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[0] = previous;
                previous = test[length - 1];
                test[length - 1] = double.NegativeInfinity;
                Assert.IsFalse(test.SIMD_IsSorted());
                test[length - 1] = previous;

                int randomInt = rng.NextInt(5, 20);
                bool sameVal = true;
                double val = test[test.Length - randomInt];
                for (int j = test.Length - randomInt + 1; j < test.Length; j++)
                {
                    sameVal &= (val == test[j]);
                }
                if (!sameVal)
                {
                    test.SIMD_Reverse(test.Length - randomInt);
                    Assert.IsFalse(test.SIMD_IsSorted());
                }

                test.Dispose(default(JobHandle));
            }
        }
    }
}