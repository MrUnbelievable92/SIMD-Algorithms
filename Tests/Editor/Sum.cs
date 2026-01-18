using System;
using NUnit.Framework;

namespace SIMDAlgorithms.Tests
{
    public static class Sum
    {
        [Test, Timeout(int.MaxValue)]
        public static void TypeByte_RangeULong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                ulong std = 0;

                for (int j = 0; j < array.Length; j++)
                {
                    std += array[j];
                }

                Assert.AreEqual(std, array.SIMD_Sum(TypeCode.UInt64));
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            2000000000,
            2);
        }

        [Test, Timeout(int.MaxValue)]
        public static void TypeUShort_RangeULong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                ulong std = 0;

                for (int j = 0; j < array.Length; j++)
                {
                    std += array[j];
                }

                Assert.AreEqual(std, array.SIMD_Sum(TypeCode.UInt64));
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            2000000000,
            2);
        }

        [Test, Timeout(int.MaxValue)]
        public static void TypeSByte_RangInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                int std = 0;

                for (int j = 0; j < array.Length; j++)
                {
                    std += array[j];
                }

                Assert.AreEqual(std, array.SIMD_Sum(TypeCode.Int32));
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1),
            2000000000,
            3);
        }

        [Test, Timeout(int.MaxValue)]
        public static void TypeSByte_RangeLong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                long std = 0;

                for (int j = 0; j < array.Length; j++)
                {
                    std += array[j];
                }

                Assert.AreEqual(std, array.SIMD_Sum(TypeCode.Int64));
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1),
            2000000000,
            2);
        }

        [Test, Timeout(int.MaxValue)]
        public static void TypeShort_RangeLong()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                long std = 0;

                for (int j = 0; j < array.Length; j++)
                {
                    std += array[j];
                }

                Assert.AreEqual(std, array.SIMD_Sum(TypeCode.Int64));
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1),
            2000000000,
            2);
        }
    }
}