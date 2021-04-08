using NUnit.Framework;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace SIMDAlgorithms.Tests
{
    unsafe public static class CountBits
    {
        private static ulong Scalar(void* ptr, long bytes)
        {
            ulong bits = 0;
            long longs = bytes / 8;
            long residuals = bytes % 8;

            for (long j = 0; j < longs; j++)
            {
                bits += (ulong)math.countbits((*(ulong*)ptr));
                ptr = (ulong*)ptr + 1;
            }

            for (long j = 0; j < residuals; j++)
            {
                bits += (ulong)math.countbits((uint)((byte*)ptr)[j]);
            }

            return bits;
        }

        [Test]
        public static void Generic()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                NativeArray<int3> test = new NativeArray<int3>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = rng.NextInt3();
                }

                Assert.AreEqual(Scalar(test.GetUnsafePtr(), sizeof(int3) * test.Length), test.SIMD_CountBits());

                test.Dispose();
            }
        }

        [Test]
        public static void Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                NativeArray<byte> test = new NativeArray<byte>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (byte)rng.NextInt();
                }

                Assert.AreEqual(Scalar(test.GetUnsafePtr(), sizeof(byte) * test.Length), test.SIMD_CountBits());

                test.Dispose();
            }
        }

        [Test]
        public static void UShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                NativeArray<ushort> test = new NativeArray<ushort>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (ushort)rng.NextInt();
                }

                Assert.AreEqual(Scalar(test.GetUnsafePtr(), sizeof(ushort) * test.Length), test.SIMD_CountBits());

                test.Dispose();
            }
        }

        [Test]
        public static void UInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                NativeArray<uint> test = new NativeArray<uint>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (uint)rng.NextInt();
                }

                Assert.AreEqual(Scalar(test.GetUnsafePtr(), sizeof(uint) * test.Length), test.SIMD_CountBits());

                test.Dispose();
            }
        }

        [Test]
        public static void ULong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                NativeArray<ulong> test = new NativeArray<ulong>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = rng.NextUInt() | (ulong)rng.NextUInt() << 32;
                }

                Assert.AreEqual(Scalar(test.GetUnsafePtr(), sizeof(ulong) * test.Length), test.SIMD_CountBits());

                test.Dispose();
            }
        }

        [Test]
        public static void NOT_1Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                byte operand = (byte)rng.NextInt(byte.MinValue + 1, byte.MaxValue + 1);

                NativeArray<byte> test = new NativeArray<byte>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (byte)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(0x0000_00FF & ~((uint)test[j]));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.NOT, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void AND_1Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                byte operand = (byte)rng.NextInt(byte.MinValue + 1, byte.MaxValue + 1);

                NativeArray<byte> test = new NativeArray<byte>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (byte)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((uint)(test[j] & operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.AND, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void OR_1Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                byte operand = (byte)rng.NextInt(byte.MinValue + 1, byte.MaxValue + 1);

                NativeArray<byte> test = new NativeArray<byte>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (byte)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((uint)(test[j] | operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.OR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void XOR_1Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                byte operand = (byte)rng.NextInt(byte.MinValue + 1, byte.MaxValue + 1);

                NativeArray<byte> test = new NativeArray<byte>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (byte)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((uint)(test[j] ^ operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.XOR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void NAND_1Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                byte operand = (byte)rng.NextInt(byte.MinValue + 1, byte.MaxValue + 1);

                NativeArray<byte> test = new NativeArray<byte>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (byte)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(0x0000_00FF & ~((uint)(test[j] & operand)));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.NAND, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void NOR_1Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                byte operand = (byte)rng.NextInt(byte.MinValue + 1, byte.MaxValue + 1);

                NativeArray<byte> test = new NativeArray<byte>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (byte)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(0x0000_00FF & ~((uint)(test[j] | operand)));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.NOR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void XNOR_1Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                byte operand = (byte)rng.NextInt(byte.MinValue + 1, byte.MaxValue + 1);

                NativeArray<byte> test = new NativeArray<byte>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (byte)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(0x0000_00FF & ~((uint)(test[j] ^ operand)));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.XNOR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void ANDNOT_1Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                byte operand = (byte)rng.NextInt(byte.MinValue + 1, byte.MaxValue + 1);

                NativeArray<byte> test = new NativeArray<byte>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (byte)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(0x0000_00FF & (~(uint)test[j] & operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.ANDNOT, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void ORNOT_1Byte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                byte operand = (byte)rng.NextInt(byte.MinValue + 1, byte.MaxValue + 1);

                NativeArray<byte> test = new NativeArray<byte>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (byte)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(0x0000_00FF & (~(uint)test[j] | operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.ORNOT, operand));

                test.Dispose();
            }
        }


        [Test]
        public static void NOT_2Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ushort operand = (ushort)rng.NextInt(ushort.MinValue + 1, ushort.MaxValue + 1);

                NativeArray<ushort> test = new NativeArray<ushort>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (ushort)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(0x0000_FFFF & ~((uint)test[j]));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.NOT, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void AND_2Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ushort operand = (ushort)rng.NextInt(ushort.MinValue + 1, ushort.MaxValue + 1);

                NativeArray<ushort> test = new NativeArray<ushort>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (ushort)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((uint)(test[j] & operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.AND, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void OR_2Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ushort operand = (ushort)rng.NextInt(ushort.MinValue + 1, ushort.MaxValue + 1);

                NativeArray<ushort> test = new NativeArray<ushort>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (ushort)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((uint)(test[j] | operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.OR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void XOR_2Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ushort operand = (ushort)rng.NextInt(ushort.MinValue + 1, ushort.MaxValue + 1);

                NativeArray<ushort> test = new NativeArray<ushort>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (ushort)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((uint)(test[j] ^ operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.XOR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void NAND_2Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ushort operand = (ushort)rng.NextInt(ushort.MinValue + 1, ushort.MaxValue + 1);

                NativeArray<ushort> test = new NativeArray<ushort>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (ushort)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(0x0000_FFFF & ~((uint)(test[j] & operand)));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.NAND, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void NOR_2Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ushort operand = (ushort)rng.NextInt(ushort.MinValue + 1, ushort.MaxValue + 1);

                NativeArray<ushort> test = new NativeArray<ushort>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (ushort)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(0x0000_FFFF & ~((uint)(test[j] | operand)));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.NOR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void XNOR_2Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ushort operand = (ushort)rng.NextInt(ushort.MinValue + 1, ushort.MaxValue + 1);

                NativeArray<ushort> test = new NativeArray<ushort>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (ushort)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(0x0000_FFFF & ~((uint)(test[j] ^ operand)));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.XNOR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void ANDNOT_2Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ushort operand = (ushort)rng.NextInt(ushort.MinValue + 1, ushort.MaxValue + 1);

                NativeArray<ushort> test = new NativeArray<ushort>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (ushort)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(0x0000_FFFF & (~(uint)test[j] & operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.ANDNOT, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void ORNOT_2Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ushort operand = (ushort)rng.NextInt(ushort.MinValue + 1, ushort.MaxValue + 1);

                NativeArray<ushort> test = new NativeArray<ushort>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (ushort)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(0x0000_FFFF & (~(uint)test[j] | operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.ORNOT, operand));

                test.Dispose();
            }
        }


        [Test]
        public static void NOT_4Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                uint operand = (uint)rng.NextInt();

                NativeArray<uint> test = new NativeArray<uint>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (uint)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(~((uint)test[j]));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.NOT, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void AND_4Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                uint operand = (uint)rng.NextInt();

                NativeArray<uint> test = new NativeArray<uint>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (uint)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((uint)(test[j] & operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.AND, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void OR_4Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                uint operand = (uint)rng.NextInt();

                NativeArray<uint> test = new NativeArray<uint>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (uint)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((uint)(test[j] | operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.OR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void XOR_4Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                uint operand = (uint)rng.NextInt();

                NativeArray<uint> test = new NativeArray<uint>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (uint)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((uint)(test[j] ^ operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.XOR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void NAND_4Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                uint operand = (uint)rng.NextInt();

                NativeArray<uint> test = new NativeArray<uint>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (uint)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(~((uint)(test[j] & operand)));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.NAND, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void NOR_4Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                uint operand = (uint)rng.NextInt();

                NativeArray<uint> test = new NativeArray<uint>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (uint)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(~((uint)(test[j] | operand)));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.NOR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void XNOR_4Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                uint operand = (uint)rng.NextInt();

                NativeArray<uint> test = new NativeArray<uint>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (uint)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(~((uint)(test[j] ^ operand)));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.XNOR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void ANDNOT_4Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                uint operand = (uint)rng.NextInt();

                NativeArray<uint> test = new NativeArray<uint>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (uint)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((~(uint)test[j] & operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.ANDNOT, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void ORNOT_4Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                uint operand = (uint)rng.NextInt();

                NativeArray<uint> test = new NativeArray<uint>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = (uint)rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((~(uint)test[j] | operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.ORNOT, operand));

                test.Dispose();
            }
        }


        [Test]
        public static void NOT_8Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ulong operand = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();

                NativeArray<ulong> test = new NativeArray<ulong>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(~((ulong)test[j]));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.NOT, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void AND_8Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ulong operand = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();

                NativeArray<ulong> test = new NativeArray<ulong>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((ulong)(test[j] & operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.AND, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void OR_8Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ulong operand = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();

                NativeArray<ulong> test = new NativeArray<ulong>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((ulong)(test[j] | operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.OR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void XOR_8Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ulong operand = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();

                NativeArray<ulong> test = new NativeArray<ulong>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((ulong)(test[j] ^ operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.XOR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void NAND_8Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ulong operand = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();

                NativeArray<ulong> test = new NativeArray<ulong>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(~((ulong)(test[j] & operand)));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.NAND, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void NOR_8Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ulong operand = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();

                NativeArray<ulong> test = new NativeArray<ulong>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(~((ulong)(test[j] | operand)));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.NOR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void XNOR_8Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ulong operand = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();

                NativeArray<ulong> test = new NativeArray<ulong>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits(~((ulong)(test[j] ^ operand)));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.XNOR, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void ANDNOT_8Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ulong operand = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();

                NativeArray<ulong> test = new NativeArray<ulong>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((~(ulong)test[j] & operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.ANDNOT, operand));

                test.Dispose();
            }
        }

        [Test]
        public static void ORNOT_8Bytes()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);

            for (int i = 0; i < 3; i++)
            {
                ulong operand = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();

                NativeArray<ulong> test = new NativeArray<ulong>(rng.NextInt(1, 1_000_000), Allocator.Persistent);

                for (int j = 0; j < test.Length; j++)
                {
                    test[j] = ((ulong)rng.NextUInt() << 32) | rng.NextUInt();
                }

                ulong scalar = 0;

                for (int j = 0; j < test.Length; j++)
                {
                    scalar += (ulong)math.countbits((~(ulong)test[j] | operand));
                }

                Assert.AreEqual(scalar, test.SIMD_CountBits(BitwiseOperation.ORNOT, operand));

                test.Dispose();
            }
        }
    }
}
