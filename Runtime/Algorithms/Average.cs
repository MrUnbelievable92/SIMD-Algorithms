using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using DevTools;
using Unity.Burst;

namespace SIMDAlgorithms
{
    unsafe public static partial class Algorithms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Average(byte* ptr, long length, TypeCode range = TypeCode.Empty)
        {
Assert.IsNonNegative(length);

            range = (range == TypeCode.Empty) ? TypeCode.UInt64 : range;

            return (byte)((SIMD_Sum(ptr, length, range) + 1) / (ulong)length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Average(this NativeArray<byte> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Average(this NativeArray<byte> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Average(this NativeArray<byte> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Average(this NativeList<byte> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Average(this NativeList<byte> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Average(this NativeList<byte> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Average(this NativeSlice<byte> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Average(this NativeSlice<byte> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Average(this NativeSlice<byte> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Average(ushort* ptr, long length, TypeCode range = TypeCode.Empty)
        {
Assert.IsNonNegative(length);

            range = (range == TypeCode.Empty) ? GetSafeRange.Summation(TypeCode.UInt16, length) : range;

            return (ushort)((SIMD_Sum(ptr, length, range) + 1) / (ulong)length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Average(this NativeArray<ushort> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Average(this NativeArray<ushort> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Average(this NativeArray<ushort> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Average(this NativeList<ushort> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Average(this NativeList<ushort> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Average(this NativeList<ushort> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Average(this NativeSlice<ushort> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Average(this NativeSlice<ushort> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Average(this NativeSlice<ushort> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Average(uint* ptr, long length, TypeCode range = TypeCode.Empty)
        {
Assert.IsNonNegative(length);

            range = (range == TypeCode.Empty) ? TypeCode.UInt64 : range;

            return (uint)((SIMD_Sum(ptr, length, range) + 1) / (ulong)length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Average(this NativeArray<uint> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Average(this NativeArray<uint> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Average(this NativeArray<uint> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Average(this NativeList<uint> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Average(this NativeList<uint> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Average(this NativeList<uint> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Average(this NativeSlice<uint> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Average(this NativeSlice<uint> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Average(this NativeSlice<uint> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Average(ulong* ptr, long length)
        {
Assert.IsNonNegative(length);

            return (SIMD_Sum(ptr, length) + 1) / (ulong)length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Average(this NativeArray<ulong> array)
        {
            return SIMD_Average((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Average(this NativeArray<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Average(this NativeArray<ulong> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Average(this NativeList<ulong> array)
        {
            return SIMD_Average((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Average(this NativeList<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Average(this NativeList<ulong> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Average(this NativeSlice<ulong> array)
        {
            return SIMD_Average((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Average(this NativeSlice<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Average(this NativeSlice<ulong> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Average(sbyte* ptr, long length, TypeCode range = TypeCode.Empty)
        {
Assert.IsNonNegative(length);

            range = (range == TypeCode.Empty) ? GetSafeRange.Summation(TypeCode.SByte, length) : range;

            long sum = SIMD_Sum(ptr, length, range);
            bool greater0 = sum > 0;

            return (sbyte)((sum + *(byte*)&greater0) / length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Average(this NativeArray<sbyte> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Average(this NativeArray<sbyte> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Average(this NativeArray<sbyte> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Average(this NativeList<sbyte> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Average(this NativeList<sbyte> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Average(this NativeList<sbyte> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Average(this NativeSlice<sbyte> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Average(this NativeSlice<sbyte> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Average(this NativeSlice<sbyte> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Average(short* ptr, long length, TypeCode range = TypeCode.Empty)
        {
Assert.IsNonNegative(length);

            range = (range == TypeCode.Empty) ? GetSafeRange.Summation(TypeCode.UInt16, length) : range;

            long sum = SIMD_Sum(ptr, length, range);
            bool greater0 = sum > 0;

            return (short)((sum + *(byte*)&greater0) / length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Average(this NativeArray<short> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((short*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Average(this NativeArray<short> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Average(this NativeArray<short> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Average(this NativeList<short> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((short*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Average(this NativeList<short> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Average(this NativeList<short> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Average(this NativeSlice<short> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((short*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Average(this NativeSlice<short> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Average(this NativeSlice<short> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Average(int* ptr, long length, TypeCode range = TypeCode.Empty)
        {
Assert.IsNonNegative(length);

            range = (range == TypeCode.Empty) ? TypeCode.Int64 : range;

            long sum = SIMD_Sum(ptr, length, range);
            bool greater0 = sum > 0;

            return (int)((sum + *(byte*)&greater0) / length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Average(this NativeArray<int> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((int*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Average(this NativeArray<int> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Average(this NativeArray<int> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Average(this NativeList<int> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((int*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Average(this NativeList<int> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Average(this NativeList<int> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Average(this NativeSlice<int> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Average((int*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Average(this NativeSlice<int> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Average(this NativeSlice<int> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Average(long* ptr, long length)
        {
Assert.IsNonNegative(length);

            long sum = SIMD_Sum(ptr, length);
            bool greater0 = sum > 0;

            return (sum + *(byte*)&greater0) / length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Average(this NativeArray<long> array)
        {
            return SIMD_Average((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Average(this NativeArray<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Average(this NativeArray<long> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Average(this NativeList<long> array)
        {
            return SIMD_Average((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Average(this NativeList<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Average(this NativeList<long> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Average(this NativeSlice<long> array)
        {
            return SIMD_Average((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Average(this NativeSlice<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Average(this NativeSlice<long> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Average(float* ptr, long length, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsNonNegative(length);

            float freeFullAccuracyRCP = 1f / (float)length;

            return SIMD_Sum(ptr, length, floatMode) * freeFullAccuracyRCP;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Average(this NativeArray<float> array, FloatMode floatMode = FloatMode.Strict)
        {
            return SIMD_Average((float*)array.GetUnsafeReadOnlyPtr(), array.Length, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Average(this NativeArray<float> array, int index, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Average(this NativeArray<float> array, int index, int numEntries, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Average(this NativeList<float> array, FloatMode floatMode = FloatMode.Strict)
        {
            return SIMD_Average((float*)array.GetUnsafeReadOnlyPtr(), array.Length, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Average(this NativeList<float> array, int index, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Average(this NativeList<float> array, int index, int numEntries, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Average(this NativeSlice<float> array, FloatMode floatMode = FloatMode.Strict)
        {
            return SIMD_Average((float*)array.GetUnsafeReadOnlyPtr(), array.Length, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Average(this NativeSlice<float> array, int index, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Average(this NativeSlice<float> array, int index, int numEntries, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, floatMode);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Average(double* ptr, long length, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsNonNegative(length);

            double freeFullAccuracyRCP = 1d / (double)length;

            return SIMD_Sum(ptr, length, floatMode) * freeFullAccuracyRCP;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Average(this NativeArray<double> array, FloatMode floatMode = FloatMode.Strict)
        {
            return SIMD_Average((double*)array.GetUnsafeReadOnlyPtr(), array.Length, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Average(this NativeArray<double> array, int index, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Average(this NativeArray<double> array, int index, int numEntries, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Average(this NativeList<double> array, FloatMode floatMode = FloatMode.Strict)
        {
            return SIMD_Average((double*)array.GetUnsafeReadOnlyPtr(), array.Length, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Average(this NativeList<double> array, int index, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Average(this NativeList<double> array, int index, int numEntries, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Average(this NativeSlice<double> array, FloatMode floatMode = FloatMode.Strict)
        {
            return SIMD_Average((double*)array.GetUnsafeReadOnlyPtr(), array.Length, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Average(this NativeSlice<double> array, int index, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Average((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Average(this NativeSlice<double> array, int index, int numEntries, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Average((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, floatMode);
        }
    }
}