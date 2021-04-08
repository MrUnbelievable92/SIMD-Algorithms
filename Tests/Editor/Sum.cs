using System;
using NUnit.Framework;
using Unity.Collections;

namespace SIMDAlgorithms.Tests
{
    public static class Sum
    {
        [Test]
        public static void TypeByte_RangeULong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<byte> array = new NativeArray<byte>(rng.NextInt(0, 2000000000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);
                }

                ulong std = 0;

                for (int j = 0; j < array.Length; j++)
                {
                    std += array[j];
                }

                Assert.AreEqual(std, array.SIMD_Sum(TypeCode.UInt64));

                array.Dispose();
                array = new NativeArray<byte>(rng.NextInt(0, 2000000000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void TypeUShort_RangeULong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<ushort> array = new NativeArray<ushort>(rng.NextInt(0, 2000000000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);
                }

                ulong std = 0;

                for (int j = 0; j < array.Length; j++)
                {
                    std += array[j];
                }

                Assert.AreEqual(std, array.SIMD_Sum(TypeCode.UInt64));

                array.Dispose();
                array = new NativeArray<ushort>(rng.NextInt(0, 200000000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void TypeSByte_RangInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<sbyte> array = new NativeArray<sbyte>(rng.NextInt(0, 200000000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);
                }

                int std = 0;

                for (int j = 0; j < array.Length; j++)
                {
                    std += array[j];
                }

                Assert.AreEqual(std, array.SIMD_Sum(TypeCode.Int32));

                array.Dispose();
                array = new NativeArray<sbyte>(rng.NextInt(0, 200000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void TypeSByte_RangeLong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<sbyte> array = new NativeArray<sbyte>(rng.NextInt(0, 2000000000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);
                }

                long std = 0;

                for (int j = 0; j < array.Length; j++)
                {
                    std += array[j];
                }

                Assert.AreEqual(std, array.SIMD_Sum(TypeCode.Int64));

                array.Dispose();
                array = new NativeArray<sbyte>(rng.NextInt(0, 2000000000), Allocator.Persistent);
            }

            array.Dispose();
        }

        [Test]
        public static void TypeShort_RangeLong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(CONSTANTS.RNG_SEED);
            NativeArray<short> array = new NativeArray<short>(rng.NextInt(0, 200000000), Allocator.Persistent);

            for (int i = 0; i < CONSTANTS.NUM_TESTS; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);
                }

                long std = 0;

                for (int j = 0; j < array.Length; j++)
                {
                    std += array[j];
                }

                Assert.AreEqual(std, array.SIMD_Sum(TypeCode.Int64));

                array.Dispose();
                array = new NativeArray<short>(rng.NextInt(0, 2000000000), Allocator.Persistent);
            }

            array.Dispose();
        }
    }
}