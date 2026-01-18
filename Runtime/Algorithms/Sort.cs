using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using DevTools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Burst.CompilerServices;
using MaxMath;
using MaxMath.Intrinsics;

using static Unity.Burst.Intrinsics.X86;

namespace SIMDAlgorithms
{
    unsafe public static partial class Algorithms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong SplatByte(byte value)
        {
            if (BurstArchitecture.IsTableLookupSupported)
            {
                return Xse.shuffle_epi8(Xse.cvtsi32_si128(value), Xse.setzero_si128()).ULong0;
            }
            else
            {
                return maxmath.bitfield((byte)value, (byte)value, (byte)value, (byte)value, (byte)value, (byte)value, (byte)value, (byte)value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InsertionSortMemMove<T>(T* ptr, int i, int idx, T current)
            where T : unmanaged
        {
            if (BurstArchitecture.IsSIMDSupported)
            {
                int idx2 = idx + 1;

                if (Avx2.IsAvx2Supported)
                {
                    while (Hint.Likely(idx2 + BYTES_IN_V256 < i))
                    {
                        Avx.mm256_storeu_si256(ptr + (idx2 + 1), Avx.mm256_loadu_si256(ptr + idx2));
                        idx2 += BYTES_IN_V256;
                    }

                    if (Hint.Likely(idx2 + BYTES_IN_V128 < i))
                    {
                        *(v128*)(ptr + (idx2 + 1)) = Xse.loadu_si128(ptr + idx2);
                        idx2 += BYTES_IN_V128;
                    }
                }
                else
                {
                    while (idx2 + BYTES_IN_V128 < i)
                    {
                        *(v128*)(ptr + (idx2 + 1)) = Xse.loadu_si128(ptr + idx2);
                        idx2 += BYTES_IN_V128;
                    }
                }

                if (Hint.Likely(idx2 + BYTES_IN_LONG < i))
                {
                    *(ulong*)(ptr + (idx2 + 1)) = *(ulong*)(ptr + idx2);
                    idx2 += BYTES_IN_LONG;
                }

                if (Hint.Likely(idx2 + BYTES_IN_INT < i))
                {
                    *(uint*)(ptr + (idx2 + 1)) = *(uint*)(ptr + idx2);
                    idx2 += BYTES_IN_INT;
                }

                if (Hint.Likely(idx2 + BYTES_IN_SHORT < i))
                {
                    *(ushort*)(ptr + (idx2 + 1)) = *(ushort*)(ptr + idx2);
                    idx2 += BYTES_IN_SHORT;
                }

                if (Hint.Likely(idx2 < i))
                {
                    ptr[idx2 + 1] = ptr[idx2];

                    if (Hint.Likely(++idx2 < i))
                    {
                        ptr[idx2 + 1] = ptr[idx2];
                    }
                }

                ptr[idx] = current;
            }
            else throw new IllegalInstructionException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InsertionSort(byte* ptr, int length)
        {
            if (BurstArchitecture.IsSIMDSupported)
            {
                for (int i = 1; i < length; i++)
                {
                    byte current = ptr[i];

                    int idx = (int)SIMD_IndexOf(ptr, i, current, Comparison.GreaterThan, TraversalOrder.Ascending);

                    if (idx != -1)
                    {
                        InsertionSortMemMove(ptr, i, idx, current);
                    }
                }
            }
            else
            {
                for (int i = 1; i < length; i++)
                {
                    byte current = ptr[i];
                    int j = i;

                    while (--j >= 0 && ptr[j] > current)
                    {
                        ptr[j + 1] = ptr[j];
                    }

                    ptr[j + 1] = current;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InsertionSort(sbyte* ptr, int length)
        {
            if (BurstArchitecture.IsSIMDSupported)
            {
                for (int i = 1; i < length; i++)
                {
                    sbyte current = ptr[i];

                    int idx = (int)SIMD_IndexOf(ptr, i, current, Comparison.GreaterThan, TraversalOrder.Ascending);

                    if (idx != -1)
                    {
                        InsertionSortMemMove(ptr, i, idx, current);
                    }
                }
            }
            else
            {
                for (int i = 1; i < length; i++)
                {
                    sbyte current = ptr[i];
                    int j = i;

                    while (--j >= 0 && ptr[j] > current)
                    {
                        ptr[j + 1] = ptr[j];
                    }

                    ptr[j + 1] = current;
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CountingSortMemSetIteration<T>([NoAlias] ref void* ptr, [NoAlias] ref ulong initial, [NoAlias] T* counters, int i)
            where T : unmanaged
        {
            long counter = (sizeof(T) == 4) ? ((int*)counters)[i] : ((long*)counters)[i];

            while (Hint.Likely(counter >= BYTES_IN_LONG))
            {
                *(ulong*)ptr = initial;
                ptr = (byte*)ptr + BYTES_IN_LONG;
                counter -= BYTES_IN_LONG;
            }

            if (Hint.Likely(counter >= BYTES_IN_INT))
            {
                *(uint*)ptr = (uint)initial;
                ptr = (byte*)ptr + BYTES_IN_INT;
                counter -= BYTES_IN_INT;
            }

            if (Hint.Likely(counter >= BYTES_IN_SHORT))
            {
                *(ushort*)ptr = (ushort)initial;
                ptr = (byte*)ptr + BYTES_IN_SHORT;
                counter -= BYTES_IN_SHORT;
            }

            if (Hint.Likely(counter != 0))
            {
                *(byte*)ptr = (byte)initial;
                ptr = (byte*)ptr + 1;
            }

            initial += SplatByte(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CountingSortMemSet<T>([NoAlias] void* ptr, [NoAlias] T* counters, int min, int max, bool signed)
            where T : unmanaged
        {
            if (Avx2.IsAvx2Supported)
            {
                v256 initial = Avx.mm256_set1_epi8((byte)min);

                for (int i = min; i <= max; i++)
                {
                    CountingSortMemSetIteration256(ref ptr, ref initial, counters, i - (signed ? sbyte.MinValue : 0));
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128 initial = Xse.set1_epi8((byte)min);

                for (int i = min; i <= max; i++)
                {
                    CountingSortMemSetIteration128(ref ptr, ref initial, counters, i - (signed ? sbyte.MinValue : 0));
                }
            }
            else
            {
                int i = min;
                ulong initial = SplatByte((byte)i);

                if (signed)
                {
                    for (; i < 0; i++)
                    {
                        CountingSortMemSetIteration(ref ptr, ref initial, counters, i - sbyte.MinValue);
                    }

                    initial = 0ul;
                }

                for (; i <= max; i++)
                {
                    CountingSortMemSetIteration(ref ptr, ref initial, counters, i - (signed ? sbyte.MinValue : 0));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CountingSortMemSetIteration128<T>([NoAlias] ref void* ptr, [NoAlias] ref v128 initial, [NoAlias] T* counters, int i)
            where T : unmanaged
        {
            if (BurstArchitecture.IsSIMDSupported)
            {
                long counter = (sizeof(T) == 4) ? ((int*)counters)[i] : ((long*)counters)[i];

                while (Hint.Likely(counter >= BYTES_IN_V128))
                {
                    *(v128*)ptr = initial;
                    ptr = (byte*)ptr + BYTES_IN_V128;
                    counter -= BYTES_IN_V128;
                }

                if (Hint.Likely(counter >= BYTES_IN_LONG))
                {
                    *(long*)ptr = Xse.cvtsi128_si64x(initial);
                    ptr = (byte*)ptr + BYTES_IN_LONG;
                    counter -= BYTES_IN_LONG;
                }

                if (Hint.Likely(counter >= BYTES_IN_INT))
                {
                    *(int*)ptr = Xse.cvtsi128_si32(initial);
                    ptr = (byte*)ptr + BYTES_IN_INT;
                    counter -= BYTES_IN_INT;
                }

                if (Hint.Likely(counter >= BYTES_IN_SHORT))
                {
                    *(ushort*)ptr = Xse.extract_epi16(initial, 0);
                    ptr = (byte*)ptr + BYTES_IN_SHORT;
                    counter -= BYTES_IN_SHORT;
                }

                if (Hint.Likely(counter != 0))
                {
                    if (BurstArchitecture.IsInsertExtractSupported)
                    {
                        *(byte*)ptr = Xse.extract_epi8(initial, 0);
                    }
                    else
                    {
                        *(byte*)ptr = (byte)Xse.cvtsi128_si32(initial);
                    }

                    ptr = (byte*)ptr + 1;
                }

                initial = Xse.inc_epi8(initial);
            }
            else throw new IllegalInstructionException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CountingSortMemSetIteration256<T>([NoAlias] ref void* ptr, [NoAlias] ref v256 initial, [NoAlias] T* counters, int i)
            where T : unmanaged
        {
            if (Avx2.IsAvx2Supported)
            {
                long counter = (sizeof(T) == 4) ? ((int*)counters)[i] : ((long*)counters)[i];

                while (Hint.Likely(counter >= BYTES_IN_V256))
                {
                    *(v256*)ptr = initial;
                    ptr = (byte*)ptr + BYTES_IN_V256;
                    counter -= BYTES_IN_V256;
                }

                if (Hint.Likely(counter >= BYTES_IN_V128))
                {
                    *(v128*)ptr = Avx.mm256_castsi256_si128(initial);
                    ptr = (byte*)ptr + BYTES_IN_V128;
                    counter -= BYTES_IN_V128;
                }

                if (Hint.Likely(counter >= BYTES_IN_LONG))
                {
                    *(long*)ptr = Xse.cvtsi128_si64x(Avx.mm256_castsi256_si128(initial));
                    ptr = (byte*)ptr + BYTES_IN_LONG;
                    counter -= BYTES_IN_LONG;
                }

                if (Hint.Likely(counter >= BYTES_IN_INT))
                {
                    *(int*)ptr = Xse.cvtsi128_si32(Avx.mm256_castsi256_si128(initial));
                    ptr = (byte*)ptr + BYTES_IN_INT;
                    counter -= BYTES_IN_INT;
                }

                if (Hint.Likely(counter >= BYTES_IN_SHORT))
                {
                    *(ushort*)ptr = Xse.extract_epi16(Avx.mm256_castsi256_si128(initial), 0);
                    ptr = (byte*)ptr + BYTES_IN_SHORT;
                    counter -= BYTES_IN_SHORT;
                }

                if (Hint.Likely(counter != 0))
                {
                    *(byte*)ptr = Xse.extract_epi8(Avx.mm256_castsi256_si128(initial), 0);
                    ptr = (byte*)ptr + 1;
                }

                initial = Xse.mm256_inc_epi8(initial);
            }
            else throw new IllegalInstructionException();
        }

        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CountingSort<T>(void* ptr, T length, bool signed)
            where T : unmanaged
        {
            byte* originalPtr = (byte*)ptr;
            byte* pastEndPtr = (byte*)ptr + (sizeof(T) == sizeof(int) ? *(uint*)&length : *(ulong*)&length);

            T* counters = stackalloc T[byte.MaxValue + 1];
            for (int i = 0; i < byte.MaxValue + 1; i++)
            {
                counters[i] = default(T);
            }

            int globalMin = signed ? sbyte.MaxValue : byte.MaxValue;
            int globalMax = signed ? sbyte.MinValue : byte.MinValue;

            while (Hint.Likely(ptr < pastEndPtr))
            {
                if (sizeof(T) == sizeof(int))
                {
                    int value = (signed ? *(sbyte*)ptr : *(byte*)ptr);
                    ((int*)counters)[(byte)(value - (signed ? sbyte.MinValue : 0))]++;
                    globalMin = math.min(globalMin, value);
                    globalMax = math.max(globalMax, value);
                    ptr = (byte*)ptr + 1;
                }
                else
                {
                    int value = (signed ? *(sbyte*)ptr : *(byte*)ptr);
                    ((long*)counters)[(byte)(value - (signed ? sbyte.MinValue : 0))]++;
                    globalMin = math.min(globalMin, value);
                    globalMax = math.max(globalMax, value);
                    ptr = (byte*)ptr + 1;
                }
            }

            CountingSortMemSet(originalPtr, counters, globalMin, globalMax, signed);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(byte* ptr, long length)
        {
            if (constexpr.IS_TRUE(length <= int.MaxValue))
            {
                SIMD_Sort(ptr, (int)length);
            }
            else
            {
                if (Hint.Unlikely(length < 500))
                {
                    InsertionSort(ptr, (int)length);
                }
                else
                {
                    CountingSort(ptr, length, false);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(byte* ptr, int length)
        {
            if (Hint.Unlikely(length < 500))
            {
                InsertionSort(ptr, length);
            }
            else
            {
                CountingSort(ptr, length, false);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeArray<byte> array)
        {
            SIMD_Sort((byte*)array.GetUnsafePtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeArray<byte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_Sort((byte*)array.GetUnsafePtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeArray<byte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_Sort((byte*)array.GetUnsafePtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeList<byte> array)
        {
            SIMD_Sort((byte*)array.GetUnsafePtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeList<byte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_Sort((byte*)array.GetUnsafePtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeList<byte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_Sort((byte*)array.GetUnsafePtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeSlice<byte> array)
        {
            SIMD_Sort((byte*)array.GetUnsafePtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeSlice<byte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_Sort((byte*)array.GetUnsafePtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeSlice<byte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_Sort((byte*)array.GetUnsafePtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(sbyte* ptr, long length)
        {
            if (constexpr.IS_TRUE(length <= int.MaxValue))
            {
                SIMD_Sort(ptr, (int)length);
            }
            else
            {
                if (Hint.Unlikely(length < 500))
                {
                    InsertionSort(ptr, (int)length);
                }
                else
                {
                    CountingSort(ptr, length, true);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(sbyte* ptr, int length)
        {
            if (Hint.Unlikely(length < 500))
            {
                InsertionSort(ptr, length);
            }
            else
            {
                CountingSort(ptr, length, true);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeArray<sbyte> array)
        {
            SIMD_Sort((sbyte*)array.GetUnsafePtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeArray<sbyte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_Sort((sbyte*)array.GetUnsafePtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeArray<sbyte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_Sort((sbyte*)array.GetUnsafePtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeList<sbyte> array)
        {
            SIMD_Sort((sbyte*)array.GetUnsafePtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeList<sbyte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_Sort((sbyte*)array.GetUnsafePtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeList<sbyte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_Sort((sbyte*)array.GetUnsafePtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeSlice<sbyte> array)
        {
            SIMD_Sort((sbyte*)array.GetUnsafePtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeSlice<sbyte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_Sort((sbyte*)array.GetUnsafePtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Sort(this NativeSlice<sbyte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_Sort((sbyte*)array.GetUnsafePtr() + index, numEntries);
        }
    }
}