using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;

namespace SIMDAlgorithms.Tests
{
    public static class ShouldMask
    {
        [Test, Timeout(int.MaxValue)]
        public static void Unsigned()
        {
            NativeArray<byte> array = new NativeArray<byte>(8, Allocator.Persistent);

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 100;
            }

            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }

            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            array.Dispose(default(JobHandle));

            NativeArray<ushort> array2 = new NativeArray<ushort>(4, Allocator.Persistent);

            for (int i = 0; i < array2.Length; i++)
            {
                array2[i] = 100;
            }

            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            for (int i = 0; i < array2.Length; i++)
            {
                array2[i] = 0;
            }

            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            array2.Dispose(default(JobHandle));

            NativeArray<uint> array3 = new NativeArray<uint>(2, Allocator.Persistent);

            for (int i = 0; i < array3.Length; i++)
            {
                array3[i] = 100;
            }

            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            for (int i = 0; i < array3.Length; i++)
            {
                array3[i] = 0;
            }

            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            array3.Dispose(default(JobHandle));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Signed()
        {
            NativeArray<sbyte> array = new NativeArray<sbyte>(8, Allocator.Persistent);

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 100;
            }

            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array.SIMD_Contains(-100, Comparison.EqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(-100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(-100, Comparison.LessThan));
            Assert.AreEqual(true,  array.SIMD_Contains(-100, Comparison.GreaterThan));
            Assert.AreEqual(false, array.SIMD_Contains(-100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(-100, Comparison.GreaterThanOrEqualTo));

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }

            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array.SIMD_Contains(-100, Comparison.EqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(-100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(-100, Comparison.LessThan));
            Assert.AreEqual(true,  array.SIMD_Contains(-100, Comparison.GreaterThan));
            Assert.AreEqual(false, array.SIMD_Contains(-100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(-100, Comparison.GreaterThanOrEqualTo));

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = -100;
            }

            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(true,  array.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(true,  array.SIMD_Contains(-100, Comparison.EqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(-100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array.SIMD_Contains(-100, Comparison.LessThan));
            Assert.AreEqual(false, array.SIMD_Contains(-100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array.SIMD_Contains(-100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array.SIMD_Contains(-100, Comparison.GreaterThanOrEqualTo));

            array.Dispose(default(JobHandle));

            NativeArray<short> array2 = new NativeArray<short>(4, Allocator.Persistent);

            for (int i = 0; i < array2.Length; i++)
            {
                array2[i] = 100;
            }

            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array2.SIMD_Contains(-100, Comparison.EqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(-100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(-100, Comparison.LessThan));
            Assert.AreEqual(true,  array2.SIMD_Contains(-100, Comparison.GreaterThan));
            Assert.AreEqual(false, array2.SIMD_Contains(-100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(-100, Comparison.GreaterThanOrEqualTo));

            for (int i = 0; i < array2.Length; i++)
            {
                array2[i] = 0;
            }

            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array2.SIMD_Contains(-100, Comparison.EqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(-100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(-100, Comparison.LessThan));
            Assert.AreEqual(true,  array2.SIMD_Contains(-100, Comparison.GreaterThan));
            Assert.AreEqual(false, array2.SIMD_Contains(-100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(-100, Comparison.GreaterThanOrEqualTo));

            for (int i = 0; i < array2.Length; i++)
            {
                array2[i] = -100;
            }

            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(true,  array2.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array2.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(true,  array2.SIMD_Contains(-100, Comparison.EqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(-100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array2.SIMD_Contains(-100, Comparison.LessThan));
            Assert.AreEqual(false, array2.SIMD_Contains(-100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array2.SIMD_Contains(-100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array2.SIMD_Contains(-100, Comparison.GreaterThanOrEqualTo));

            array2.Dispose(default(JobHandle));

            NativeArray<int> array3 = new NativeArray<int>(2, Allocator.Persistent);

            for (int i = 0; i < array3.Length; i++)
            {
                array3[i] = 100;
            }

            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array3.SIMD_Contains(-100, Comparison.EqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(-100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(-100, Comparison.LessThan));
            Assert.AreEqual(true,  array3.SIMD_Contains(-100, Comparison.GreaterThan));
            Assert.AreEqual(false, array3.SIMD_Contains(-100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(-100, Comparison.GreaterThanOrEqualTo));

            for (int i = 0; i < array3.Length; i++)
            {
                array3[i] = 0;
            }

            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array3.SIMD_Contains(-100, Comparison.EqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(-100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(-100, Comparison.LessThan));
            Assert.AreEqual(true,  array3.SIMD_Contains(-100, Comparison.GreaterThan));
            Assert.AreEqual(false, array3.SIMD_Contains(-100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(-100, Comparison.GreaterThanOrEqualTo));

            for (int i = 0; i < array3.Length; i++)
            {
                array3[i] = -100;
            }

            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(true,  array3.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array3.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(true,  array3.SIMD_Contains(-100, Comparison.EqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(-100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array3.SIMD_Contains(-100, Comparison.LessThan));
            Assert.AreEqual(false, array3.SIMD_Contains(-100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array3.SIMD_Contains(-100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array3.SIMD_Contains(-100, Comparison.GreaterThanOrEqualTo));

            array3.Dispose(default(JobHandle));

            NativeArray<float> array4 = new NativeArray<float>(4, Allocator.Persistent);

            for (int i = 0; i < array4.Length; i++)
            {
                array4[i] = 100;
            }

            Assert.AreEqual(false, array4.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array4.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(true,  array4.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(false, array4.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(true,  array4.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(false, array4.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array4.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array4.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array4.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array4.SIMD_Contains(-100, Comparison.EqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(-100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array4.SIMD_Contains(-100, Comparison.LessThan));
            Assert.AreEqual(true,  array4.SIMD_Contains(-100, Comparison.GreaterThan));
            Assert.AreEqual(false, array4.SIMD_Contains(-100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(-100, Comparison.GreaterThanOrEqualTo));

            for (int i = 0; i < array4.Length; i++)
            {
                array4[i] = 0;
            }

            Assert.AreEqual(true,  array4.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(false, array4.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(false, array4.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(false, array4.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(true,  array4.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array4.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array4.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array4.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array4.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array4.SIMD_Contains(-100, Comparison.EqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(-100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array4.SIMD_Contains(-100, Comparison.LessThan));
            Assert.AreEqual(true,  array4.SIMD_Contains(-100, Comparison.GreaterThan));
            Assert.AreEqual(false, array4.SIMD_Contains(-100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(-100, Comparison.GreaterThanOrEqualTo));

            for (int i = 0; i < array4.Length; i++)
            {
                array4[i] = -100;
            }

            Assert.AreEqual(false, array4.SIMD_Contains(0, Comparison.EqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(0, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(0, Comparison.LessThan));
            Assert.AreEqual(false, array4.SIMD_Contains(0, Comparison.GreaterThan));
            Assert.AreEqual(true,  array4.SIMD_Contains(0, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array4.SIMD_Contains(0, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(false, array4.SIMD_Contains(100, Comparison.EqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(100, Comparison.NotEqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(100, Comparison.LessThan));
            Assert.AreEqual(false, array4.SIMD_Contains(100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array4.SIMD_Contains(100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(false, array4.SIMD_Contains(100, Comparison.GreaterThanOrEqualTo));

            Assert.AreEqual(true,  array4.SIMD_Contains(-100, Comparison.EqualTo));
            Assert.AreEqual(false, array4.SIMD_Contains(-100, Comparison.NotEqualTo));
            Assert.AreEqual(false, array4.SIMD_Contains(-100, Comparison.LessThan));
            Assert.AreEqual(false, array4.SIMD_Contains(-100, Comparison.GreaterThan));
            Assert.AreEqual(true,  array4.SIMD_Contains(-100, Comparison.LessThanOrEqualTo));
            Assert.AreEqual(true,  array4.SIMD_Contains(-100, Comparison.GreaterThanOrEqualTo));

            array4.Dispose(default(JobHandle));
        }
    }
}