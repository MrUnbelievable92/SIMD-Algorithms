using NUnit.Framework;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Jobs;

namespace SIMDAlgorithms.Tests
{
    public static class Reverse
    {
        [Test, Timeout(int.MaxValue)]
        public static void Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 50; i++)
            {
                // test all remainders
                int length = rng.NextInt(1, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<byte> test = new NativeArray<byte>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                byte[] cpy = new byte[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (byte)rng.NextInt(0, 256);
                    cpy[j] = test[j];
                }

                System.Array.Reverse(cpy);
                test.SIMD_Reverse();

                for (int j = 0; j < length; j++)
                {
                    Assert.AreEqual(cpy[j], test[j]);
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 50; i++)
            {
                // test all remainders
                int length = rng.NextInt(1, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<ushort> test = new NativeArray<ushort>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                ushort[] cpy = new ushort[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (ushort)rng.NextInt(0, ushort.MaxValue + 1);
                    cpy[j] = test[j];
                }

                System.Array.Reverse(cpy);
                test.SIMD_Reverse();

                for (int j = 0; j < length; j++)
                {
                    Assert.AreEqual(cpy[j], test[j]);
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void Byte3()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 50; i++)
            {
                // test all remainders
                int length = rng.NextInt(1, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<Byte3> test = new NativeArray<Byte3>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                Byte3[] cpy = new Byte3[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = new Byte3{ a = (byte)rng.NextInt(0, byte.MaxValue + 1), b = (byte)rng.NextInt(0, byte.MaxValue + 1), c = (byte)rng.NextInt(0, byte.MaxValue + 1) };
                    cpy[j] = test[j];
                }

                System.Array.Reverse(cpy);
                test.SIMD_Reverse();

                for (int j = 0; j < length; j++)
                {
                    Assert.AreEqual(cpy[j].a, test[j].a);
                    Assert.AreEqual(cpy[j].b, test[j].b);
                    Assert.AreEqual(cpy[j].c, test[j].c);
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt()
        {

            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 50; i++)
            {
                // test all remainders
                int length = rng.NextInt(1, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<uint> test = new NativeArray<uint>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                uint[] cpy = new uint[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (uint)rng.NextInt();
                    cpy[j] = test[j];
                }

                System.Array.Reverse(cpy);
                test.SIMD_Reverse();

                for (int j = 0; j < length; j++)
                {
                    Assert.AreEqual(cpy[j], test[j]);
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void Byte5()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 50; i++)
            {
                // test all remainders
                int length = rng.NextInt(1, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<Byte5> test = new NativeArray<Byte5>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                Byte5[] cpy = new Byte5[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = new Byte5{ a = (byte)rng.NextInt(0, byte.MaxValue + 1),
                                         b = (byte)rng.NextInt(0, byte.MaxValue + 1),
                                         c = (byte)rng.NextInt(0, byte.MaxValue + 1),
                                         d = (byte)rng.NextInt(0, byte.MaxValue + 1),
                                         e = (byte)rng.NextInt(0, byte.MaxValue + 1) };
                    cpy[j] = test[j];
                }

                System.Array.Reverse(cpy);
                test.SIMD_Reverse();

                for (int j = 0; j < length; j++)
                {
                    Assert.AreEqual(cpy[j].a, test[j].a);
                    Assert.AreEqual(cpy[j].b, test[j].b);
                    Assert.AreEqual(cpy[j].c, test[j].c);
                    Assert.AreEqual(cpy[j].d, test[j].d);
                    Assert.AreEqual(cpy[j].e, test[j].e);
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void Byte6()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 50; i++)
            {
                // test all remainders
                int length = rng.NextInt(1, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<Byte6> test = new NativeArray<Byte6>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                Byte6[] cpy = new Byte6[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = new Byte6{ a = (byte)rng.NextInt(0, byte.MaxValue + 1),
                                         b = (byte)rng.NextInt(0, byte.MaxValue + 1),
                                         c = (byte)rng.NextInt(0, byte.MaxValue + 1),
                                         d = (byte)rng.NextInt(0, byte.MaxValue + 1),
                                         e = (byte)rng.NextInt(0, byte.MaxValue + 1),
                                         f = (byte)rng.NextInt(0, byte.MaxValue + 1) };
                    cpy[j] = test[j];
                }

                System.Array.Reverse(cpy);
                test.SIMD_Reverse();

                for (int j = 0; j < length; j++)
                {
                    Assert.AreEqual(cpy[j].a, test[j].a);
                    Assert.AreEqual(cpy[j].b, test[j].b);
                    Assert.AreEqual(cpy[j].c, test[j].c);
                    Assert.AreEqual(cpy[j].d, test[j].d);
                    Assert.AreEqual(cpy[j].e, test[j].e);
                    Assert.AreEqual(cpy[j].f, test[j].f);
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void ULong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 50; i++)
            {
                // test all remainders
                int length = rng.NextInt(10, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<ulong> test = new NativeArray<ulong>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                ulong[] cpy = new ulong[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = (ulong)rng.NextInt();
                    cpy[j] = test[j];
                }

                System.Array.Reverse(cpy);
                test.SIMD_Reverse();

                for (int j = 0; j < length; j++)
                {
                    Assert.AreEqual(cpy[j], test[j]);
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void V128()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 50; i++)
            {
                // test all remainders
                int length = rng.NextInt(10, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<v128> test = new NativeArray<v128>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                v128[] cpy = new v128[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = new v128(rng.NextInt(), rng.NextInt(), rng.NextInt(), rng.NextInt());
                    cpy[j] = test[j];
                }

                System.Array.Reverse(cpy);
                test.SIMD_Reverse();

                for (int j = 0; j < length; j++)
                {
                    v128 t = test[j];
                    v128 c = cpy[j];
                    Assert.AreEqual(t.SLong0, c.SLong0);
                    Assert.AreEqual(t.SLong1, c.SLong1);
                }

                test.Dispose(default(JobHandle));
            }
        }

        [Test, Timeout(int.MaxValue)]
        public static void V256()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            for (int i = 0; i < 50; i++)
            {
                // test all remainders
                int length = rng.NextInt(10, 100_000);
                length &= unchecked((int)0xFFFF_FFC0);
                length |= i;

                NativeArray<v256> test = new NativeArray<v256>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                v256[] cpy = new v256[length];

                for (int j = 0; j < length; j++)
                {
                    test[j] = new v256(rng.NextInt(), rng.NextInt(), rng.NextInt(), rng.NextInt(), rng.NextInt(), rng.NextInt(), rng.NextInt(), rng.NextInt());
                    cpy[j] = test[j];
                }

                System.Array.Reverse(cpy);
                test.SIMD_Reverse();

                for (int j = 0; j < length; j++)
                {
                    v256 t = test[j];
                    v256 c = cpy[j];
                    Assert.AreEqual(t.SLong0, c.SLong0);
                    Assert.AreEqual(t.SLong1, c.SLong1);
                    Assert.AreEqual(t.SLong2, c.SLong2);
                    Assert.AreEqual(t.SLong3, c.SLong3);
                }

                test.Dispose(default(JobHandle));
            }
        }
    }
}