using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using DevTools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using MaxMath.Intrinsics;

using static Unity.Burst.Intrinsics.X86;

namespace SIMDAlgorithms
{
    unsafe public static partial class Algorithms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(byte* ptr, long length)
        {
Assert.IsNonNegative(length);

            v128* vecPtr = (v128*)ptr;

            if (Avx2.IsAvx2Supported)
            {
                while (length >= 3 * BYTES_IN_V256)
                {
                    v256 inductionMask0 = Xse.mm256_cmpge_epu8(Avx.mm256_loadu_si256((byte*)vecPtr + (BYTES_IN_V256 - 1)),     Avx.mm256_loadu_si256(vecPtr));
                    v256 inductionMask1 = Xse.mm256_cmpge_epu8(Avx.mm256_loadu_si256((byte*)vecPtr + (2 * BYTES_IN_V256 - 1)), Avx.mm256_loadu_si256((byte*)vecPtr + BYTES_IN_V256));

                    vecPtr += 2 * 2;
                    length -= 2 * BYTES_IN_V256;

                    if (Avx.mm256_testc_si256(inductionMask0, inductionMask1) != 1)
                    {
                        return false;
                    }
                }

                if (length >= 3 * BYTES_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpge_epu8(Xse.loadu_si128((byte*)vecPtr + (BYTES_IN_V128 - 1)),     Xse.loadu_si128(vecPtr));
                    v128 inductionMask1 = Xse.cmpge_epu8(Xse.loadu_si128((byte*)vecPtr + (2 * BYTES_IN_V128 - 1)), Xse.loadu_si128((byte*)vecPtr + BYTES_IN_V128));

                    vecPtr += 2;
                    length -= 2 * BYTES_IN_V128;

                    if (Xse.testc_si128(inductionMask0, inductionMask1) != 1)
                    {
                        return false;
                    }
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                while (length >= 3 * BYTES_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpge_epu8(Xse.loadu_si128((byte*)vecPtr + (BYTES_IN_V128 - 1)),     Xse.loadu_si128(vecPtr));
                    v128 inductionMask1 = Xse.cmpge_epu8(Xse.loadu_si128((byte*)vecPtr + (2 * BYTES_IN_V128 - 1)), Xse.loadu_si128((byte*)vecPtr + BYTES_IN_V128));

                    vecPtr += 2;
                    length -= 2 * BYTES_IN_V128;

                    bool notSorted;
                    if (Sse4_1.IsSse41Supported)
                    {
                        notSorted = Xse.testc_si128(inductionMask0, inductionMask1) != 1;
                    }
                    else
                    {
                        notSorted = Xse.movemask_epi8(Xse.and_si128(inductionMask0, inductionMask1)) != ushort.MaxValue;
                    }

                    if (notSorted)
                    {
                        return false;
                    }
                }
            }


            ptr = (byte*)vecPtr;

            while (length-- > 1)
            {
                if (ptr[0] > ptr[1])
                {
                    return false;
                }

                ptr++;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<byte> array)
        {
            return SIMD_IsSorted((byte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<byte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<byte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<byte> array)
        {
            return SIMD_IsSorted((byte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<byte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<byte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<byte> array)
        {
            return SIMD_IsSorted((byte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<byte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<byte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(ushort* ptr, long length)
        {
Assert.IsNonNegative(length);

            v128* vecPtr = (v128*)ptr;

            if (Avx2.IsAvx2Supported)
            {
                while (length >= 3 * SHORTS_IN_V256)
                {
                    v256 inductionMask0 = Xse.mm256_cmpge_epu16(Avx.mm256_loadu_si256((ushort*)vecPtr + (SHORTS_IN_V256 - 1)),     Avx.mm256_loadu_si256(vecPtr));
                    v256 inductionMask1 = Xse.mm256_cmpge_epu16(Avx.mm256_loadu_si256((ushort*)vecPtr + (2 * SHORTS_IN_V256 - 1)), Avx.mm256_loadu_si256((ushort*)vecPtr + SHORTS_IN_V256));

                    vecPtr += 2 * 2;
                    length -= 2 * SHORTS_IN_V256;

                    if (Avx.mm256_testc_si256(inductionMask0, inductionMask1) != 1)
                    {
                        return false;
                    }
                }

                if (length >= 3 * SHORTS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpge_epu16(Xse.loadu_si128((ushort*)vecPtr + (SHORTS_IN_V128 - 1)),     Xse.loadu_si128(vecPtr));
                    v128 inductionMask1 = Xse.cmpge_epu16(Xse.loadu_si128((ushort*)vecPtr + (2 * SHORTS_IN_V128 - 1)), Xse.loadu_si128((ushort*)vecPtr + SHORTS_IN_V128));

                    vecPtr += 2;
                    length -= 2 * SHORTS_IN_V128;

                    if (Xse.testc_si128(inductionMask0, inductionMask1) != 1)
                    {
                        return false;
                    }
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                while (length >= 3 * SHORTS_IN_V128)
                {
                    bool notSorted;
                    if (Sse4_1.IsSse41Supported)
                    {
                        v128 inductionMask0 = Xse.cmpge_epu16(Xse.loadu_si128((ushort*)vecPtr + (SHORTS_IN_V128 - 1)),     Xse.loadu_si128(vecPtr));
                        v128 inductionMask1 = Xse.cmpge_epu16(Xse.loadu_si128((ushort*)vecPtr + (2 * SHORTS_IN_V128 - 1)), Xse.loadu_si128((ushort*)vecPtr + SHORTS_IN_V128));

                        notSorted = Xse.testc_si128(inductionMask0, inductionMask1) != 1;
                    }
                    else
                    {
                        v128 inductionMask0 = Xse.cmpgt_epu16(Xse.loadu_si128(vecPtr),                             Xse.loadu_si128((ushort*)vecPtr + (SHORTS_IN_V128 - 1)));
                        v128 inductionMask1 = Xse.cmpgt_epu16(Xse.loadu_si128((ushort*)vecPtr + SHORTS_IN_V128),   Xse.loadu_si128((ushort*)vecPtr + (2 * SHORTS_IN_V128 - 1)));

                        notSorted = Xse.movemask_epi8(Xse.or_si128(inductionMask0, inductionMask1)) != 0;
                    }

                    vecPtr += 2;
                    length -= 2 * SHORTS_IN_V128;

                    if (notSorted)
                    {
                        return false;
                    }
                }
            }


            ptr = (ushort*)vecPtr;

            while (length-- > 1)
            {
                if (ptr[0] > ptr[1])
                {
                    return false;
                }

                ptr++;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<ushort> array)
        {
            return SIMD_IsSorted((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<ushort> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<ushort> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<ushort> array)
        {
            return SIMD_IsSorted((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<ushort> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<ushort> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<ushort> array)
        {
            return SIMD_IsSorted((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<ushort> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<ushort> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(uint* ptr, long length)
        {
Assert.IsNonNegative(length);

            v128* vecPtr = (v128*)ptr;

            if (Avx2.IsAvx2Supported)
            {
                while (length >= 3 * INTS_IN_V256)
                {
                    v256 inductionMask0 = Xse.mm256_cmpge_epu32(Avx.mm256_loadu_si256((uint*)vecPtr + (INTS_IN_V256 - 1)),     Avx.mm256_loadu_si256(vecPtr));
                    v256 inductionMask1 = Xse.mm256_cmpge_epu32(Avx.mm256_loadu_si256((uint*)vecPtr + (2 * INTS_IN_V256 - 1)), Avx.mm256_loadu_si256((uint*)vecPtr + INTS_IN_V256));

                    vecPtr += 2 * 2;
                    length -= 2 * INTS_IN_V256;

                    if (Avx.mm256_testc_si256(inductionMask0, inductionMask1) != 1)
                    {
                        return false;
                    }
                }

                if (length >= 3 * INTS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpge_epu32(Xse.loadu_si128((uint*)vecPtr + (INTS_IN_V128 - 1)),     Xse.loadu_si128(vecPtr));
                    v128 inductionMask1 = Xse.cmpge_epu32(Xse.loadu_si128((uint*)vecPtr + (2 * INTS_IN_V128 - 1)), Xse.loadu_si128((uint*)vecPtr + INTS_IN_V128));

                    vecPtr += 2;
                    length -= 2 * INTS_IN_V128;

                    if (Xse.testc_si128(inductionMask0, inductionMask1) != 1)
                    {
                        return false;
                    }
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                while (length >= 3 * INTS_IN_V128)
                {
                    bool notSorted;
                    if (Sse4_1.IsSse41Supported)
                    {
                        v128 inductionMask0 = Xse.cmpge_epu32(Xse.loadu_si128((uint*)vecPtr + (INTS_IN_V128 - 1)),     Xse.loadu_si128(vecPtr));
                        v128 inductionMask1 = Xse.cmpge_epu32(Xse.loadu_si128((uint*)vecPtr + (2 * INTS_IN_V128 - 1)), Xse.loadu_si128((uint*)vecPtr + INTS_IN_V128));

                        notSorted = Xse.testc_si128(inductionMask0, inductionMask1) != 1;
                    }
                    else
                    {
                        v128 inductionMask0 = Xse.cmpgt_epu32(Xse.loadu_si128(vecPtr),                         Xse.loadu_si128((uint*)vecPtr + (INTS_IN_V128 - 1)));
                        v128 inductionMask1 = Xse.cmpgt_epu32(Xse.loadu_si128((uint*)vecPtr + INTS_IN_V128),   Xse.loadu_si128((uint*)vecPtr + (2 * INTS_IN_V128 - 1)));

                        notSorted = Xse.movemask_epi8(Xse.or_si128(inductionMask0, inductionMask1)) != 0;
                    }

                    vecPtr += 2;
                    length -= 2 * INTS_IN_V128;

                    if (notSorted)
                    {
                        return false;
                    }
                }
            }


            ptr = (uint*)vecPtr;

            while (length-- > 1)
            {
                if (ptr[0] > ptr[1])
                {
                    return false;
                }

                ptr++;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<uint> array)
        {
            return SIMD_IsSorted((uint*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<uint> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<uint> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<uint> array)
        {
            return SIMD_IsSorted((uint*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<uint> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<uint> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<uint> array)
        {
            return SIMD_IsSorted((uint*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<uint> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<uint> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(ulong* ptr, long length)
        {
Assert.IsNonNegative(length);

            v128* vecPtr = (v128*)ptr;

            if (Avx2.IsAvx2Supported)
            {
                while (length >= 3 * LONGS_IN_V256)
                {
                    v256 inductionMask0 = Xse.mm256_cmpgt_epu64(Avx.mm256_loadu_si256(vecPtr),                          Avx.mm256_loadu_si256((ulong*)vecPtr + (LONGS_IN_V256 - 1)));
                    v256 inductionMask1 = Xse.mm256_cmpgt_epu64(Avx.mm256_loadu_si256((ulong*)vecPtr + LONGS_IN_V256),  Avx.mm256_loadu_si256((ulong*)vecPtr + (2 * LONGS_IN_V256 - 1)));

                    vecPtr += 2 * 2;
                    length -= 2 * LONGS_IN_V256;

                    if (Avx.mm256_testz_si256(Avx2.mm256_or_si256(inductionMask0, inductionMask1), Xse.mm256_setall_si256()) != 1)
                    {
                        return false;
                    }
                }

                if (length >= 3 * LONGS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpgt_epu64(Xse.loadu_si128(vecPtr),                           Xse.loadu_si128((ulong*)vecPtr + (LONGS_IN_V128 - 1)));
                    v128 inductionMask1 = Xse.cmpgt_epu64(Xse.loadu_si128((ulong*)vecPtr + LONGS_IN_V128),   Xse.loadu_si128((ulong*)vecPtr + (2 * LONGS_IN_V128 - 1)));

                    vecPtr += 2;
                    length -= 2 * LONGS_IN_V128;

                    if (Xse.testz_si128(Xse.or_si128(inductionMask0, inductionMask1), Xse.setall_si128()) != 1)
                    {
                        return false;
                    }
                }
            }
            else if (BurstArchitecture.IsCMP64Supported)
            {
                while (length >= 3 * LONGS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpgt_epu64(Xse.loadu_si128(vecPtr),                           Xse.loadu_si128((ulong*)vecPtr + (LONGS_IN_V128 - 1)));
                    v128 inductionMask1 = Xse.cmpgt_epu64(Xse.loadu_si128((ulong*)vecPtr + LONGS_IN_V128),   Xse.loadu_si128((ulong*)vecPtr + (2 * LONGS_IN_V128 - 1)));

                    vecPtr += 2;
                    length -= 2 * LONGS_IN_V128;

                    if (Xse.testz_si128(Xse.or_si128(inductionMask0, inductionMask1), Xse.setall_si128()) != 1)
                    {
                        return false;
                    }
                }
            }


            ptr = (ulong*)vecPtr;

            while (length-- > 1)
            {
                if (ptr[0] > ptr[1])
                {
                    return false;
                }

                ptr++;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<ulong> array)
        {
            return SIMD_IsSorted((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<ulong> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<ulong> array)
        {
            return SIMD_IsSorted((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<ulong> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<ulong> array)
        {
            return SIMD_IsSorted((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<ulong> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(sbyte* ptr, long length)
        {
Assert.IsNonNegative(length);

            v128* vecPtr = (v128*)ptr;

            if (Avx2.IsAvx2Supported)
            {
                while (length >= 3 * BYTES_IN_V256)
                {
                    v256 inductionMask0 = Avx2.mm256_cmpgt_epi8(Avx.mm256_loadu_si256(vecPtr),                         Avx.mm256_loadu_si256((sbyte*)vecPtr + (BYTES_IN_V256 - 1)));
                    v256 inductionMask1 = Avx2.mm256_cmpgt_epi8(Avx.mm256_loadu_si256((sbyte*)vecPtr + BYTES_IN_V256), Avx.mm256_loadu_si256((sbyte*)vecPtr + (2 * BYTES_IN_V256 - 1)));

                    vecPtr += 2 * 2;
                    length -= 2 * BYTES_IN_V256;

                    if (Avx.mm256_testz_si256(Avx2.mm256_or_si256(inductionMask0, inductionMask1), Xse.mm256_setall_si256()) != 1)
                    {
                        return false;
                    }
                }

                if (length >= 3 * BYTES_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpgt_epi8(Xse.loadu_si128(vecPtr),                         Xse.loadu_si128((sbyte*)vecPtr + (BYTES_IN_V128 - 1)));
                    v128 inductionMask1 = Xse.cmpgt_epi8(Xse.loadu_si128((sbyte*)vecPtr + BYTES_IN_V128), Xse.loadu_si128((sbyte*)vecPtr + (2 * BYTES_IN_V128 - 1)));

                    vecPtr += 2;
                    length -= 2 * BYTES_IN_V128;

                    if (Xse.testz_si128(Xse.or_si128(inductionMask0, inductionMask1), Xse.setall_si128()) != 1)
                    {
                        return false;
                    }
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                while (length >= 3 * BYTES_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpgt_epi8(Xse.loadu_si128(vecPtr),                         Xse.loadu_si128((sbyte*)vecPtr + (BYTES_IN_V128 - 1)));
                    v128 inductionMask1 = Xse.cmpgt_epi8(Xse.loadu_si128((sbyte*)vecPtr + BYTES_IN_V128), Xse.loadu_si128((sbyte*)vecPtr + (2 * BYTES_IN_V128 - 1)));

                    vecPtr += 2;
                    length -= 2 * BYTES_IN_V128;

                    if (Sse4_1.IsSse41Supported)
                    {
                        if (Xse.testz_si128(Xse.or_si128(inductionMask0, inductionMask1), Xse.setall_si128()) != 1)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Xse.movemask_epi8(Xse.or_si128(inductionMask0, inductionMask1)) != 0)
                        {
                            return false;
                        }
                    }
                }
            }


            ptr = (sbyte*)vecPtr;

            while (length-- > 1)
            {
                if (ptr[0] > ptr[1])
                {
                    return false;
                }

                ptr++;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<sbyte> array)
        {
            return SIMD_IsSorted((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<sbyte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<sbyte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<sbyte> array)
        {
            return SIMD_IsSorted((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<sbyte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<sbyte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<sbyte> array)
        {
            return SIMD_IsSorted((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<sbyte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<sbyte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(short* ptr, long length)
        {
Assert.IsNonNegative(length);

            v128* vecPtr = (v128*)ptr;

            if (Avx2.IsAvx2Supported)
            {
                while (length >= 3 * SHORTS_IN_V256)
                {
                    v256 inductionMask0 = Avx2.mm256_cmpgt_epi16(Avx.mm256_loadu_si256(vecPtr),                          Avx.mm256_loadu_si256((short*)vecPtr + (SHORTS_IN_V256 - 1)));
                    v256 inductionMask1 = Avx2.mm256_cmpgt_epi16(Avx.mm256_loadu_si256((short*)vecPtr + SHORTS_IN_V256), Avx.mm256_loadu_si256((short*)vecPtr + (2 * SHORTS_IN_V256 - 1)));

                    vecPtr += 2 * 2;
                    length -= 2 * SHORTS_IN_V256;

                    if (Avx.mm256_testz_si256(Avx2.mm256_or_si256(inductionMask0, inductionMask1), Xse.mm256_setall_si256()) != 1)
                    {
                        return false;
                    }
                }

                if (length >= 3 * SHORTS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpgt_epi16(Xse.loadu_si128(vecPtr),                            Xse.loadu_si128((short*)vecPtr + (SHORTS_IN_V128 - 1)));
                    v128 inductionMask1 = Xse.cmpgt_epi16(Xse.loadu_si128((short*)vecPtr + SHORTS_IN_V128),   Xse.loadu_si128((short*)vecPtr + (2 * SHORTS_IN_V128 - 1)));

                    vecPtr += 2;
                    length -= 2 * SHORTS_IN_V128;

                    if (Xse.testz_si128(Xse.or_si128(inductionMask0, inductionMask1), Xse.setall_si128()) != 1)
                    {
                        return false;
                    }
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                while (length >= 3 * SHORTS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpgt_epi16(Xse.loadu_si128(vecPtr),                            Xse.loadu_si128((short*)vecPtr + (SHORTS_IN_V128 - 1)));
                    v128 inductionMask1 = Xse.cmpgt_epi16(Xse.loadu_si128((short*)vecPtr + SHORTS_IN_V128),   Xse.loadu_si128((short*)vecPtr + (2 * SHORTS_IN_V128 - 1)));

                    vecPtr += 2;
                    length -= 2 * SHORTS_IN_V128;

                    if (Sse4_1.IsSse41Supported)
                    {
                        if (Xse.testz_si128(Xse.or_si128(inductionMask0, inductionMask1), Xse.setall_si128()) != 1)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Xse.movemask_epi8(Xse.or_si128(inductionMask0, inductionMask1)) != 0)
                        {
                            return false;
                        }
                    }
                }
            }


            ptr = (short*)vecPtr;

            while (length-- > 1)
            {
                if (ptr[0] > ptr[1])
                {
                    return false;
                }

                ptr++;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<short> array)
        {
            return SIMD_IsSorted((short*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<short> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<short> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<short> array)
        {
            return SIMD_IsSorted((short*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<short> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<short> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<short> array)
        {
            return SIMD_IsSorted((short*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<short> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<short> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(int* ptr, long length)
        {
Assert.IsNonNegative(length);

            v128* vecPtr = (v128*)ptr;

            if (Avx2.IsAvx2Supported)
            {
                while (length >= 3 * INTS_IN_V256)
                {
                    v256 inductionMask0 = Avx2.mm256_cmpgt_epi32(Avx.mm256_loadu_si256(vecPtr),                      Avx.mm256_loadu_si256((int*)vecPtr + (INTS_IN_V256 - 1)));
                    v256 inductionMask1 = Avx2.mm256_cmpgt_epi32(Avx.mm256_loadu_si256((int*)vecPtr + INTS_IN_V256), Avx.mm256_loadu_si256((int*)vecPtr + (2 * INTS_IN_V256 - 1)));

                    vecPtr += 2 * 2;
                    length -= 2 * INTS_IN_V256;

                    if (Avx.mm256_testz_si256(Avx2.mm256_or_si256(inductionMask0, inductionMask1), Xse.mm256_setall_si256()) != 1)
                    {
                        return false;
                    }
                }

                if (length >= 3 * INTS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpgt_epi32(Xse.loadu_si128(vecPtr),                        Xse.loadu_si128((int*)vecPtr + (INTS_IN_V128 - 1)));
                    v128 inductionMask1 = Xse.cmpgt_epi32(Xse.loadu_si128((int*)vecPtr + INTS_IN_V128),   Xse.loadu_si128((int*)vecPtr + (2 * INTS_IN_V128 - 1)));

                    vecPtr += 2;
                    length -= 2 * INTS_IN_V128;

                    if (Xse.testz_si128(Xse.or_si128(inductionMask0, inductionMask1), Xse.setall_si128()) != 1)
                    {
                        return false;
                    }
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                while (length >= 3 * INTS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpgt_epi32(Xse.loadu_si128(vecPtr),                        Xse.loadu_si128((int*)vecPtr + (INTS_IN_V128 - 1)));
                    v128 inductionMask1 = Xse.cmpgt_epi32(Xse.loadu_si128((int*)vecPtr + INTS_IN_V128),   Xse.loadu_si128((int*)vecPtr + (2 * INTS_IN_V128 - 1)));

                    vecPtr += 2;
                    length -= 2 * INTS_IN_V128;

                    if (Sse4_1.IsSse41Supported)
                    {
                        if (Xse.testz_si128(Xse.or_si128(inductionMask0, inductionMask1), Xse.setall_si128()) != 1)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Xse.movemask_epi8(Xse.or_si128(inductionMask0, inductionMask1)) != 0)
                        {
                            return false;
                        }
                    }
                }
            }


            ptr = (int*)vecPtr;

            while (length-- > 1)
            {
                if (ptr[0] > ptr[1])
                {
                    return false;
                }

                ptr++;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<int> array)
        {
            return SIMD_IsSorted((int*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<int> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<int> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<int> array)
        {
            return SIMD_IsSorted((int*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<int> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<int> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<int> array)
        {
            return SIMD_IsSorted((int*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<int> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<int> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(long* ptr, long length)
        {
Assert.IsNonNegative(length);

            v128* vecPtr = (v128*)ptr;

            if (Avx2.IsAvx2Supported)
            {
                while (length >= 3 * LONGS_IN_V256)
                {
                    v256 inductionMask0 = Avx2.mm256_cmpgt_epi64(Avx.mm256_loadu_si256(vecPtr),                        Avx.mm256_loadu_si256((long*)vecPtr + (LONGS_IN_V256 - 1)));
                    v256 inductionMask1 = Avx2.mm256_cmpgt_epi64(Avx.mm256_loadu_si256((long*)vecPtr + LONGS_IN_V256), Avx.mm256_loadu_si256((long*)vecPtr + (2 * LONGS_IN_V256 - 1)));

                    vecPtr += 2 * 2;
                    length -= 2 * LONGS_IN_V256;

                    if (Avx.mm256_testz_si256(Avx2.mm256_or_si256(inductionMask0, inductionMask1), Xse.mm256_setall_si256()) != 1)
                    {
                        return false;
                    }
                }

                if (length >= 3 * LONGS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpgt_epi64(Xse.loadu_si128(vecPtr),                          Xse.loadu_si128((long*)vecPtr + (LONGS_IN_V128 - 1)));
                    v128 inductionMask1 = Xse.cmpgt_epi64(Xse.loadu_si128((long*)vecPtr + LONGS_IN_V128),   Xse.loadu_si128((long*)vecPtr + (2 * LONGS_IN_V128 - 1)));

                    vecPtr += 2;
                    length -= 2 * LONGS_IN_V128;

                    if (Xse.testz_si128(Xse.or_si128(inductionMask0, inductionMask1), Xse.setall_si128()) != 1)
                    {
                        return false;
                    }
                }
            }
            else if (BurstArchitecture.IsCMP64Supported)
            {
                while (length >= 3 * LONGS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpgt_epi64(Xse.loadu_si128(vecPtr),                          Xse.loadu_si128((long*)vecPtr + (LONGS_IN_V128 - 1)));
                    v128 inductionMask1 = Xse.cmpgt_epi64(Xse.loadu_si128((long*)vecPtr + LONGS_IN_V128),   Xse.loadu_si128((long*)vecPtr + (2 * LONGS_IN_V128 - 1)));

                    vecPtr += 2;
                    length -= 2 * LONGS_IN_V128;

                    if (Xse.testz_si128(Xse.or_si128(inductionMask0, inductionMask1), Xse.setall_si128()) != 1)
                    {
                        return false;
                    }
                }
            }


            ptr = (long*)vecPtr;

            while (length-- > 1)
            {
                if (ptr[0] > ptr[1])
                {
                    return false;
                }

                ptr++;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<long> array)
        {
            return SIMD_IsSorted((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<long> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<long> array)
        {
            return SIMD_IsSorted((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<long> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<long> array)
        {
            return SIMD_IsSorted((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<long> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(float* ptr, long length)
        {
Assert.IsNonNegative(length);

            v128* vecPtr = (v128*)ptr;

            if (Avx2.IsAvx2Supported)
            {
                while (length >= 3 * INTS_IN_V256)
                {
                    v256 inductionMask0 = Avx.mm256_cmp_ps(Avx.mm256_loadu_ps((float*)vecPtr + (INTS_IN_V256 - 1)),     Avx.mm256_loadu_ps(vecPtr),                         (int)Avx.CMP.GE_OQ);
                    v256 inductionMask1 = Avx.mm256_cmp_ps(Avx.mm256_loadu_ps((float*)vecPtr + (2 * INTS_IN_V256 - 1)), Avx.mm256_loadu_ps((float*)vecPtr + INTS_IN_V256),  (int)Avx.CMP.GE_OQ);

                    vecPtr += 2 * 2;
                    length -= 2 * INTS_IN_V256;

                    if (Avx.mm256_testc_si256(inductionMask0, inductionMask1) != 1)
                    {
                        return false;
                    }
                }

                if (length >= 3 * INTS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpge_ps(*(v128*)((float*)vecPtr + (INTS_IN_V128 - 1)),     *vecPtr);
                    v128 inductionMask1 = Xse.cmpge_ps(*(v128*)((float*)vecPtr + (2 * INTS_IN_V128 - 1)), *(v128*)((float*)vecPtr + INTS_IN_V128));

                    vecPtr += 2;
                    length -= 2 * INTS_IN_V128;

                    if (Xse.testc_si128(inductionMask0, inductionMask1) != 1)
                    {
                        return false;
                    }
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                while (length >= 3 * INTS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpge_ps(*(v128*)((float*)vecPtr + (INTS_IN_V128 - 1)),     *vecPtr);
                    v128 inductionMask1 = Xse.cmpge_ps(*(v128*)((float*)vecPtr + (2 * INTS_IN_V128 - 1)), *(v128*)((float*)vecPtr + INTS_IN_V128));

                    vecPtr += 2;
                    length -= 2 * INTS_IN_V128;

                    bool notSorted;
                    if (Sse4_1.IsSse41Supported)
                    {
                        notSorted = Xse.testc_si128(inductionMask0, inductionMask1) != 1;
                    }
                    else
                    {
                        notSorted = Xse.movemask_epi8(Xse.and_ps(inductionMask0, inductionMask1)) != ushort.MaxValue;
                    }

                    if (notSorted)
                    {
                        return false;
                    }
                }
            }


            ptr = (float*)vecPtr;

            while (length-- > 1)
            {
                if (ptr[0] > ptr[1])
                {
                    return false;
                }

                ptr++;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<float> array)
        {
            return SIMD_IsSorted((float*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<float> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<float> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<float> array)
        {
            return SIMD_IsSorted((float*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<float> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<float> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<float> array)
        {
            return SIMD_IsSorted((float*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<float> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<float> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(double* ptr, long length)
        {
Assert.IsNonNegative(length);

            v128* vecPtr = (v128*)ptr;

            if (Avx2.IsAvx2Supported)
            {
                while (length >= 3 * LONGS_IN_V256)
                {
                    v256 inductionMask0 = Avx.mm256_cmp_pd(Avx.mm256_loadu_pd((double*)vecPtr + (LONGS_IN_V256 - 1)),     Avx.mm256_loadu_pd(vecPtr),                         (int)Avx.CMP.GE_OQ);
                    v256 inductionMask1 = Avx.mm256_cmp_pd(Avx.mm256_loadu_pd((double*)vecPtr + (2 * LONGS_IN_V256 - 1)), Avx.mm256_loadu_pd((double*)vecPtr + LONGS_IN_V256),  (int)Avx.CMP.GE_OQ);

                    vecPtr += 2 * 2;
                    length -= 2 * LONGS_IN_V256;

                    if (Avx.mm256_testc_si256(inductionMask0, inductionMask1) != 1)
                    {
                        return false;
                    }
                }

                if (length >= 3 * LONGS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpge_pd(*(v128*)((double*)vecPtr + (LONGS_IN_V128 - 1)),     *vecPtr);
                    v128 inductionMask1 = Xse.cmpge_pd(*(v128*)((double*)vecPtr + (2 * LONGS_IN_V128 - 1)), *(v128*)((double*)vecPtr + LONGS_IN_V128));

                    vecPtr += 2;
                    length -= 2 * LONGS_IN_V128;

                    if (Xse.testc_si128(inductionMask0, inductionMask1) != 1)
                    {
                        return false;
                    }
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                while (length >= 3 * LONGS_IN_V128)
                {
                    v128 inductionMask0 = Xse.cmpge_pd(*(v128*)((double*)vecPtr + (LONGS_IN_V128 - 1)),     *vecPtr);
                    v128 inductionMask1 = Xse.cmpge_pd(*(v128*)((double*)vecPtr + (2 * LONGS_IN_V128 - 1)), *(v128*)((double*)vecPtr + LONGS_IN_V128));

                    vecPtr += 2;
                    length -= 2 * LONGS_IN_V128;

                    bool notSorted;
                    if (Sse4_1.IsSse41Supported)
                    {
                        notSorted = Xse.testc_si128(inductionMask0, inductionMask1) != 1;
                    }
                    else
                    {
                        notSorted = Xse.movemask_epi8(Xse.and_pd(inductionMask0, inductionMask1)) != ushort.MaxValue;
                    }

                    if (notSorted)
                    {
                        return false;
                    }
                }
            }


            ptr = (double*)vecPtr;

            while (length-- > 1)
            {
                if (ptr[0] > ptr[1])
                {
                    return false;
                }

                ptr++;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<double> array)
        {
            return SIMD_IsSorted((double*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<double> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeArray<double> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<double> array)
        {
            return SIMD_IsSorted((double*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<double> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeList<double> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<double> array)
        {
            return SIMD_IsSorted((double*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<double> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_IsSorted((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_IsSorted(this NativeSlice<double> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_IsSorted((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }
    }
}