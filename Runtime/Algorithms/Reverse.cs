using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Burst.Intrinsics;
using Unity.Burst.CompilerServices;
using MaxMath;
using DevTools;

using static Unity.Burst.Intrinsics.X86;
using static MaxMath.maxmath;
using static Unity.Mathematics.math;
using MaxMath.Intrinsics;

namespace SIMDAlgorithms
{
    unsafe public static partial class Algorithms
    {
        private static v128 REVERSE_MASK_EPI8 => new v128(15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0);
        private static v256 MM256_REVERSE_MASK_EPI8 => new v256(REVERSE_MASK_EPI8, REVERSE_MASK_EPI8);

        private static v128 REVERSE_MASK_EPI16 => new v128(14, 15, 12, 13, 10, 11, 8, 9, 6, 7, 4, 5, 2, 3, 0, 1);
        private static v256 MM256_REVERSE_MASK_EPI16 => new v256(REVERSE_MASK_EPI16, REVERSE_MASK_EPI16);

        private static v256 REVERSE_MASK_EPI32 => new v256(7, 6, 5, 4, 3, 2, 1, 0);

        private static v128 REVERSE_MASK_3BYTE_0 => new v128(9, 10, 11,   6, 7, 8,   3, 4, 5,   0, 1, 2,   -1, -1, -1, -1);
        private static v128 REVERSE_MASK_3BYTE_1 => new v128(-1, -1, -1, -1,     13, 14, 15,   10, 11, 12,   7, 8, 9,   4, 5, 6);
        private static v128 REVERSE_MASK_3BYTE_2 => new v128(9, 10, 11,   6, 7, 8,   3, 4,   7, 8,   3, 4, 5,   0, 1, 2);

        private static v128 REVERSE_MASK_6BYTE_0 => new v128(6, 7, 8, 9, 10, 11,     0, 1, 2, 3, 4, 5,     -1, -1, -1, -1);
        private static v128 REVERSE_MASK_6BYTE_1 => new v128(-1, -1, -1, -1,     10, 11, 12, 13, 14, 15,     4, 5, 6, 7, 8, 9);
        private static v128 REVERSE_MASK_6BYTE_2 => new v128(6, 7, 8, 9, 10, 11,   0, 1,   10, 11,    0, 1, 2, 3, 4, 5);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ExchangeLastPair<T>(T* startPtr, ulong numRemainders)
            where T : unmanaged
        {
            if (constexpr.IS_TRUE(numRemainders <= 1))
            {
                return;
            }
            else
            {
                 T* endPtr = (T*)max((ulong)(startPtr + (numRemainders - 1)), (ulong)startPtr);
                 T lo = *startPtr;
                 T hi = *endPtr;
                 *startPtr = hi;
                 *endPtr = lo;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void StdReverse<T>(T* startPtr, long length)
            where T : unmanaged
        {
            T* startPtrLow = startPtr;
            T* startPtrHigh = startPtrLow + 1;
            T* endPtrHigh = startPtr + (length - 1);
            T* endPtrLow = endPtrHigh - 1;

            while (startPtrHigh < endPtrLow)
            {
                T lolo = *startPtrLow;
                T lohi = *startPtrHigh;
                T hihi = *endPtrHigh;
                T hilo = *endPtrLow;
                *startPtrLow = hihi;
                *startPtrHigh = hilo;
                *endPtrHigh = lolo;
                *endPtrLow = lohi;
                startPtrLow += 2;
                startPtrHigh += 2;
                endPtrHigh -= 2;
                endPtrLow -= 2;
            }

            ulong numRemainders = (ulong)length % 4;

            T* startPtr256 = startPtrLow;
            if (constexpr.IS_TRUE(numRemainders == 2))
            {
                T* endPtr256 = startPtr256 + 1;

                T start = *startPtr256;
                T end = *endPtr256;
                *endPtr256 = start;
                *startPtr256 = end;

                return;
            }

            ExchangeLastPair(startPtr256, numRemainders);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Reverse<T>(T* array, long length)
            where T : unmanaged
        {
            switch (sizeof(T))
            {
                case 1:   Reverse1Byte((byte*)array, length);     return;
                case 2:   Reverse2Bytes((ushort*)array, length);  return;
                case 3:   Reverse3Bytes((Byte3*)array, length);   return;
                case 4:   Reverse4Bytes((uint*)array, length);    return;
                case 6:   Reverse6Bytes((Byte6*)array, length);   return;
                case 8:   Reverse8Bytes((ulong*)array, length);   return;
                case 16:  Reverse16Bytes((v128*)array, length);   return;

                default:  StdReverse(array, length);              return;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Reverse<T>(this NativeArray<T> array)
            where T : unmanaged
        {
            SIMD_Reverse((T*)array.GetUnsafePtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Reverse<T>(this NativeArray<T> array, int index)
            where T : unmanaged
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_Reverse((T*)array.GetUnsafePtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Reverse<T>(this NativeArray<T> array, int index, int numEntries)
            where T : unmanaged
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_Reverse((T*)array.GetUnsafePtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Reverse<T>(this NativeList<T> array)
            where T : unmanaged
        {
            SIMD_Reverse((T*)array.GetUnsafePtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Reverse<T>(this NativeList<T> array, int index)
            where T : unmanaged
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_Reverse((T*)array.GetUnsafePtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Reverse<T>(this NativeList<T> array, int index, int numEntries)
            where T : unmanaged
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_Reverse((T*)array.GetUnsafePtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Reverse<T>(this NativeSlice<T> array)
            where T : unmanaged
        {
            SIMD_Reverse((T*)array.GetUnsafePtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Reverse<T>(this NativeSlice<T> array, int index)
            where T : unmanaged
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_Reverse((T*)array.GetUnsafePtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_Reverse<T>(this NativeSlice<T> array, int index, int numEntries)
            where T : unmanaged
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_Reverse((T*)array.GetUnsafePtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Reverse1Byte(byte* ptr, long length)
        {
            static bool ConstChecks(void* startPtr, ulong numRemainders)
            {
                if (constexpr.IS_TRUE(numRemainders == 4 * BYTES_IN_V256))
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256* startPtrLow = (v256*)startPtr;
                        v256* startPtrHigh = startPtrLow + 1;
                        v256* endPtrHigh = (v256*)((byte*)startPtr + (3 * BYTES_IN_V256));
                        v256* endPtrLow = endPtrHigh - 1;

                        v256 lolo = Avx2.mm256_shuffle_epi8(*startPtrLow, MM256_REVERSE_MASK_EPI8);
                        v256 lohi = Avx2.mm256_shuffle_epi8(*startPtrHigh, MM256_REVERSE_MASK_EPI8);
                        v256 hihi = Avx2.mm256_shuffle_epi8(*endPtrHigh, MM256_REVERSE_MASK_EPI8);
                        v256 hilo = Avx2.mm256_shuffle_epi8(*endPtrLow, MM256_REVERSE_MASK_EPI8);
                        *endPtrHigh = Avx2.mm256_permute4x64_epi64(lolo, Sse.SHUFFLE(1, 0, 3, 2));
                        *endPtrLow = Avx2.mm256_permute4x64_epi64(lohi, Sse.SHUFFLE(1, 0, 3, 2));
                        *startPtrLow = Avx2.mm256_permute4x64_epi64(hihi, Sse.SHUFFLE(1, 0, 3, 2));
                        *startPtrHigh = Avx2.mm256_permute4x64_epi64(hilo, Sse.SHUFFLE(1, 0, 3, 2));

                        return true;
                    }
                }

                if (constexpr.IS_TRUE(numRemainders == 2 * BYTES_IN_V256))
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256* endPtr256 = (v256*)startPtr + 1;

                        v256 lo = Avx2.mm256_shuffle_epi8(*(v256*)startPtr, MM256_REVERSE_MASK_EPI8);
                        v256 hi = Avx2.mm256_shuffle_epi8(*endPtr256, MM256_REVERSE_MASK_EPI8);
                        *(v256*)startPtr = Avx2.mm256_permute4x64_epi64(hi, Sse.SHUFFLE(1, 0, 3, 2));
                        *endPtr256 = Avx2.mm256_permute4x64_epi64(lo, Sse.SHUFFLE(1, 0, 3, 2));

                        return true;
                    }
                    else if (BurstArchitecture.IsTableLookupSupported)
                    {
                        v128* startPtrLow = (v128*)startPtr;
                        v128* startPtrHigh = startPtrLow + 1;
                        v128* endPtrHigh = (v128*)((byte*)startPtr + (3 * BYTES_IN_V128));
                        v128* endPtrLow = endPtrHigh - 1;

                        v128 lolo = Xse.shuffle_epi8(*startPtrLow, REVERSE_MASK_EPI8);
                        v128 lohi = Xse.shuffle_epi8(*startPtrHigh, REVERSE_MASK_EPI8);
                        v128 hihi = Xse.shuffle_epi8(*endPtrHigh, REVERSE_MASK_EPI8);
                        v128 hilo = Xse.shuffle_epi8(*endPtrLow, REVERSE_MASK_EPI8);
                        *endPtrHigh = lolo;
                        *endPtrLow = lohi;
                        *startPtrLow = hihi;
                        *startPtrHigh = hilo;

                        return true;
                    }
                }
                if (constexpr.IS_TRUE(numRemainders == 2 * BYTES_IN_V128))
                {
                    if (BurstArchitecture.IsTableLookupSupported)
                    {
                        v128* endPtr128 = (v128*)startPtr + 1;

                        v128 lo = Xse.shuffle_epi8(*(v128*)startPtr, REVERSE_MASK_EPI8);
                        v128 hi = Xse.shuffle_epi8(*endPtr128, REVERSE_MASK_EPI8);
                        *(v128*)startPtr = hi;
                        *endPtr128 = lo;
                    }
                    else
                    {
                        ulong* startPtrLow = (ulong*)startPtr;
                        ulong* startPtrHigh = startPtrLow + 1;
                        ulong* endPtrHigh = (ulong*)((byte*)startPtr + (numRemainders - BYTES_IN_LONG));
                        ulong* endPtrLow = endPtrHigh - 1;

                        ulong lolo = reversebytes(*startPtrLow);
                        ulong lohi = reversebytes(*startPtrHigh);
                        ulong hihi = reversebytes(*endPtrHigh);
                        ulong hilo = reversebytes(*endPtrLow);
                        *endPtrHigh = lolo;
                        *endPtrLow = lohi;
                        *startPtrLow = hihi;
                        *startPtrHigh = hilo;

                        startPtrLow += 2;
                        startPtrHigh += 2;
                        endPtrHigh -= 2;
                        endPtrLow -= 2;
                    }

                    return true;
                }

                if (constexpr.IS_TRUE(numRemainders == BYTES_IN_V128))
                {
                    if (BurstArchitecture.IsTableLookupSupported)
                    {
                        *(v128*)startPtr = Xse.shuffle_epi8(*(v128*)startPtr, REVERSE_MASK_EPI8);
                    }
                    else
                    {
                        ulong* endPtr64 = (ulong*)((byte*)startPtr + BYTES_IN_LONG);

                        ulong lo = reversebytes(*(ulong*)startPtr);
                        ulong hi = reversebytes(*endPtr64);
                        *(ulong*)startPtr = hi;
                        *endPtr64 = lo;
                    }

                    return true;
                }

                if (constexpr.IS_TRUE(numRemainders == BYTES_IN_LONG))
                {
                    *(ulong*)startPtr = reversebytes(*(ulong*)startPtr);

                    return true;
                }

                if (constexpr.IS_TRUE(numRemainders == BYTES_IN_INT))
                {
                    *(uint*)startPtr = reversebytes(*(uint*)startPtr);

                    return true;
                }

                if (constexpr.IS_TRUE(numRemainders == BYTES_IN_SHORT))
                {
                    *(ushort*)startPtr = reversebytes(*(ushort*)startPtr);

                    return true;
                }

                return false;
            }
            static void ScalarChain128(v128* startPtr, ulong numRemainders)
            {
                if (ConstChecks(startPtr, numRemainders)) return;
                v128* startPtr128 = (v128*)startPtr;
                if (numRemainders >= 2 * BYTES_IN_V128)
                {
                    numRemainders -= BYTES_IN_V128;
                    v128* endPtr128 = (v128*)((byte*)startPtr128 + numRemainders);
                    numRemainders -= BYTES_IN_V128;

                    if (BurstArchitecture.IsTableLookupSupported)
                    {
                        v128 lo = Xse.shuffle_epi8(*startPtr128, REVERSE_MASK_EPI8);
                        v128 hi = Xse.shuffle_epi8(*endPtr128, REVERSE_MASK_EPI8);
                        *startPtr128 = hi;
                        *endPtr128 = lo;
                    }
                    else if (BurstArchitecture.IsSIMDSupported)
                    {
                        ulong* startPtrLow = (ulong*)startPtr;
                        ulong* startPtrHigh = startPtrLow + 1;
                        ulong* endPtrLow = (ulong*)endPtr128;
                        ulong* endPtrHigh = endPtrLow + 1;

                        ulong lolo = reversebytes(*startPtrLow);
                        ulong lohi = reversebytes(*startPtrHigh);
                        ulong hihi = reversebytes(*endPtrHigh);
                        ulong hilo = reversebytes(*endPtrLow);
                        *endPtrHigh = lolo;
                        *endPtrLow = lohi;
                        *startPtrLow = hihi;
                        *startPtrHigh = hilo;
                    }

                    startPtr128++;
                }

                ScalarChain((ulong*)startPtr128, numRemainders);
            }
            static void ScalarChain(ulong* startPtr128, ulong numRemainders)
            {
                if (ConstChecks(startPtr128, numRemainders)) return;
                ulong* startPtr64 = (ulong*)startPtr128;
                if (numRemainders >= 2 * BYTES_IN_LONG)
                {
                    numRemainders -= BYTES_IN_LONG;
                    ulong* endPtr64 = (ulong*)((byte*)startPtr64 + numRemainders);
                    numRemainders -= BYTES_IN_LONG;

                    ulong lo = reversebytes(*startPtr64);
                    ulong hi = reversebytes(*endPtr64);
                    *startPtr64++ = hi;
                    *endPtr64 = lo;
                }

                if (ConstChecks(startPtr64, numRemainders)) return;
                uint* startPtr32 = (uint*)startPtr64;
                if (numRemainders >= 2 * BYTES_IN_INT)
                {
                    numRemainders -= BYTES_IN_INT;
                    uint* endPtr32 = (uint*)((byte*)startPtr32 + numRemainders);
                    numRemainders -= BYTES_IN_INT;

                    uint lo = reversebytes(*startPtr32);
                    uint hi = reversebytes(*endPtr32);
                    *startPtr32++ = hi;
                    *endPtr32 = lo;
                }

                if (ConstChecks(startPtr32, numRemainders)) return;
                ushort* startPtr16 = (ushort*)startPtr32;
                if (numRemainders >= 2 * BYTES_IN_SHORT)
                {
                    numRemainders -= BYTES_IN_SHORT;
                    ushort* endPtr16 = (ushort*)((byte*)startPtr16 + numRemainders);
                    numRemainders -= BYTES_IN_SHORT;

                    ushort lo = reversebytes(*startPtr16);
                    ushort hi = reversebytes(*endPtr16);
                    *startPtr16++ = hi;
                    *endPtr16 = lo;
                }

                if (ConstChecks(startPtr16, numRemainders)) return;
                ExchangeLastPair((byte*)startPtr16, numRemainders);
            }


Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                if (ConstChecks(ptr, (ulong)length)) return;

                v256* startPtrLow = (v256*)ptr;
                v256* startPtrHigh = startPtrLow + 1;
                v256* endPtrHigh = (v256*)(ptr + (length - BYTES_IN_V256));
                v256* endPtrLow = endPtrHigh - 1;

                while (startPtrHigh + 1 <= endPtrLow)
                {
                    v256 lolo = Avx2.mm256_shuffle_epi8(*startPtrLow, MM256_REVERSE_MASK_EPI8);
                    v256 lohi = Avx2.mm256_shuffle_epi8(*startPtrHigh, MM256_REVERSE_MASK_EPI8);
                    v256 hihi = Avx2.mm256_shuffle_epi8(*endPtrHigh, MM256_REVERSE_MASK_EPI8);
                    v256 hilo = Avx2.mm256_shuffle_epi8(*endPtrLow, MM256_REVERSE_MASK_EPI8);
                    *endPtrHigh = Avx2.mm256_permute4x64_epi64(lolo, Sse.SHUFFLE(1, 0, 3, 2));
                    *endPtrLow = Avx2.mm256_permute4x64_epi64(lohi, Sse.SHUFFLE(1, 0, 3, 2));
                    *startPtrLow = Avx2.mm256_permute4x64_epi64(hihi, Sse.SHUFFLE(1, 0, 3, 2));
                    *startPtrHigh = Avx2.mm256_permute4x64_epi64(hilo, Sse.SHUFFLE(1, 0, 3, 2));

                    startPtrLow += 2;
                    startPtrHigh += 2;
                    endPtrHigh -= 2;
                    endPtrLow -= 2;
                }

                ulong numRemainders = (ulong)length % (4 * BYTES_IN_V256);

                v256* startPtr256 = (v256*)startPtrLow;
                if (ConstChecks(startPtr256, numRemainders)) return;
                if (numRemainders >= 2 * BYTES_IN_V256)
                {
                    numRemainders -= BYTES_IN_V256;
                    v256* endPtr256 = (v256*)((byte*)startPtr256 + numRemainders);
                    numRemainders -= BYTES_IN_V256;

                    v256 lo = Avx2.mm256_shuffle_epi8(*startPtr256, MM256_REVERSE_MASK_EPI8);
                    v256 hi = Avx2.mm256_shuffle_epi8(*endPtr256, MM256_REVERSE_MASK_EPI8);
                    *startPtr256++ = Avx2.mm256_permute4x64_epi64(hi, Sse.SHUFFLE(1, 0, 3, 2));
                    *endPtr256 = Avx2.mm256_permute4x64_epi64(lo, Sse.SHUFFLE(1, 0, 3, 2));
                }

                ScalarChain128((v128*)startPtr256, numRemainders);
            }
            else if (BurstArchitecture.IsTableLookupSupported)
            {
                if (ConstChecks(ptr, (ulong)length)) return;

                v128* startPtrLow = (v128*)ptr;
                v128* startPtrHigh = startPtrLow + 1;
                v128* endPtrHigh = (v128*)(ptr + (length - BYTES_IN_V128));
                v128* endPtrLow = endPtrHigh - 1;

                while (startPtrHigh + 1 <= endPtrLow)
                {
                    v128 lolo = Xse.shuffle_epi8(*startPtrLow, REVERSE_MASK_EPI8);
                    v128 lohi = Xse.shuffle_epi8(*startPtrHigh, REVERSE_MASK_EPI8);
                    v128 hihi = Xse.shuffle_epi8(*endPtrHigh, REVERSE_MASK_EPI8);
                    v128 hilo = Xse.shuffle_epi8(*endPtrLow, REVERSE_MASK_EPI8);
                    *endPtrHigh = lolo;
                    *endPtrLow = lohi;
                    *startPtrLow = hihi;
                    *startPtrHigh = hilo;

                    startPtrLow += 2;
                    startPtrHigh += 2;
                    endPtrHigh -= 2;
                    endPtrLow -= 2;
                }

                ScalarChain128(startPtrLow, numRemainders : (ulong)length % (4 * BYTES_IN_V128));
            }
            else
            {
                if (ConstChecks(ptr, (ulong)length)) return;

                ulong* startPtrLow = (ulong*)ptr;
                ulong* startPtrHigh = startPtrLow + 1;
                ulong* endPtrHigh = (ulong*)(ptr + (length - BYTES_IN_LONG));
                ulong* endPtrLow = endPtrHigh - 1;

                while (startPtrHigh + 1 <= endPtrLow)
                {
                    ulong lolo = reversebytes(*startPtrLow);
                    ulong lohi = reversebytes(*startPtrHigh);
                    ulong hihi = reversebytes(*endPtrHigh);
                    ulong hilo = reversebytes(*endPtrLow);
                    *endPtrHigh = lolo;
                    *endPtrLow = lohi;
                    *startPtrLow = hihi;
                    *startPtrHigh = hilo;

                    startPtrLow += 2;
                    startPtrHigh += 2;
                    endPtrHigh -= 2;
                    endPtrLow -= 2;
                }

                ScalarChain(startPtrLow, numRemainders: (ulong)length % (4 * BYTES_IN_LONG));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Reverse2Bytes(ushort* ptr, long length)
        {
            static bool ConstChecks(void* startPtr, ulong numRemainders)
            {
                if (BurstArchitecture.IsSIMDSupported)
                {
                    if (constexpr.IS_TRUE(numRemainders == 4 * SHORTS_IN_V256))
                    {
                        if (Avx2.IsAvx2Supported)
                        {
                            v256* startPtrLow = (v256*)startPtr;
                            v256* startPtrHigh = startPtrLow + 1;
                            v256* endPtrHigh = (v256*)((ushort*)startPtr + (3 * SHORTS_IN_V256));
                            v256* endPtrLow = endPtrHigh - 1;

                            v256 lolo = Avx2.mm256_shuffle_epi8(*startPtrLow, MM256_REVERSE_MASK_EPI16);
                            v256 lohi = Avx2.mm256_shuffle_epi8(*startPtrHigh, MM256_REVERSE_MASK_EPI16);
                            v256 hihi = Avx2.mm256_shuffle_epi8(*endPtrHigh, MM256_REVERSE_MASK_EPI16);
                            v256 hilo = Avx2.mm256_shuffle_epi8(*endPtrLow, MM256_REVERSE_MASK_EPI16);
                            *endPtrHigh = Avx2.mm256_permute4x64_epi64(lolo, Sse.SHUFFLE(1, 0, 3, 2));
                            *endPtrLow = Avx2.mm256_permute4x64_epi64(lohi, Sse.SHUFFLE(1, 0, 3, 2));
                            *startPtrLow = Avx2.mm256_permute4x64_epi64(hihi, Sse.SHUFFLE(1, 0, 3, 2));
                            *startPtrHigh = Avx2.mm256_permute4x64_epi64(hilo, Sse.SHUFFLE(1, 0, 3, 2));

                            return true;
                        }
                    }
                    if (constexpr.IS_TRUE(numRemainders == 2 * SHORTS_IN_V256))
                    {
                        if (Avx2.IsAvx2Supported)
                        {
                            v256* endPtr256 = (v256*)startPtr + 1;

                            v256 lo = Avx2.mm256_shuffle_epi8(*(v256*)startPtr, MM256_REVERSE_MASK_EPI16);
                            v256 hi = Avx2.mm256_shuffle_epi8(*endPtr256, MM256_REVERSE_MASK_EPI16);
                            *(v256*)startPtr = Avx2.mm256_permute4x64_epi64(hi, Sse.SHUFFLE(1, 0, 3, 2));
                            *endPtr256 = Avx2.mm256_permute4x64_epi64(lo, Sse.SHUFFLE(1, 0, 3, 2));
                        }
                        else
                        {
                            v128* startPtrLow = (v128*)startPtr;
                            v128* startPtrHigh = startPtrLow + 1;
                            v128* endPtrHigh = (v128*)((ushort*)startPtr + (3 * SHORTS_IN_V128));
                            v128* endPtrLow = endPtrHigh - 1;

                            v128 lolo = reverse(*(ushort8*)startPtrLow);
                            v128 lohi = reverse(*(ushort8*)startPtrHigh);
                            v128 hihi = reverse(*(ushort8*)endPtrHigh);
                            v128 hilo = reverse(*(ushort8*)endPtrLow);
                            *endPtrHigh = lolo;
                            *endPtrLow = lohi;
                            *startPtrLow = hihi;
                            *startPtrHigh = hilo;
                        }

                        return true;
                    }
                    if (constexpr.IS_TRUE(numRemainders == 2 * SHORTS_IN_V128))
                    {
                        v128* endPtr128 = (v128*)startPtr + 1;

                        v128 lo = reverse(*(ushort8*)startPtr);
                        v128 hi = reverse(*(ushort8*)endPtr128);
                        *(v128*)startPtr = hi;
                        *endPtr128 = lo;

                        return true;
                    }

                    if (constexpr.IS_TRUE(numRemainders == SHORTS_IN_V128))
                    {
                        *(v128*)startPtr = reverse(*(ushort8*)startPtr);

                        return true;
                    }

                    if (constexpr.IS_TRUE(numRemainders == SHORTS_IN_LONG))
                    {
                        *(long*)startPtr = Xse.cvtsi128_si64x(Xse.shufflelo_epi16(Xse.cvtsi64x_si128(*(long*)startPtr), Sse.SHUFFLE(0, 1, 2, 3)));

                        return true;
                    }
                }

                if (constexpr.IS_TRUE(numRemainders == SHORTS_IN_INT))
                {
                    *(int*)startPtr = rol(*(int*)startPtr, 8 * sizeof(ushort));

                    return true;
                }

                return false;
            }
            static void ScalarChain(v128* startPtr, ulong numRemainders)
            {
                if (BurstArchitecture.IsSIMDSupported)
                {
                    if (ConstChecks(startPtr, numRemainders)) return;
                    v128* startPtr128 = (v128*)startPtr;
                    if (numRemainders >= 2 * SHORTS_IN_V128)
                    {
                        numRemainders -= SHORTS_IN_V128;
                        v128* endPtr128 = (v128*)((ushort*)startPtr128 + numRemainders);
                        numRemainders -= SHORTS_IN_V128;

                        v128 lo = reverse(*(ushort8*)startPtr128);
                        v128 hi = reverse(*(ushort8*)endPtr128);
                        *startPtr128++ = hi;
                        *endPtr128 = lo;
                    }

                    if (ConstChecks(startPtr128, numRemainders)) return;
                    long* startPtr64 = (long*)startPtr128;
                    if (numRemainders >= 2 * SHORTS_IN_LONG)
                    {
                        numRemainders -= SHORTS_IN_LONG;
                        long* endPtr64 = (long*)((ushort*)startPtr64 + numRemainders);
                        numRemainders -= SHORTS_IN_LONG;

                        v128 reverseLo = Xse.shufflelo_epi16(Xse.cvtsi64x_si128(*startPtr64), Sse.SHUFFLE(0, 1, 2, 3));
                        v128 reverseHi = Xse.shufflelo_epi16(Xse.cvtsi64x_si128(*endPtr64), Sse.SHUFFLE(0, 1, 2, 3));
                        *startPtr64++ = Xse.cvtsi128_si64x(reverseHi);
                        *endPtr64 = Xse.cvtsi128_si64x(reverseLo);
                    }

                    if (ConstChecks(startPtr64, numRemainders)) return;
                    int* startPtr32 = (int*)startPtr64;
                    if (numRemainders >= 2 * SHORTS_IN_INT)
                    {
                        numRemainders -= SHORTS_IN_INT;
                        int* endPtr32 = (int*)((ushort*)startPtr32 + numRemainders);
                        numRemainders -= SHORTS_IN_INT;

                        v128 reverseLo = Xse.shufflelo_epi16(Xse.cvtsi32_si128(*startPtr32), Sse.SHUFFLE(0, 0, 0, 1));
                        v128 reverseHi = Xse.shufflelo_epi16(Xse.cvtsi32_si128(*endPtr32), Sse.SHUFFLE(0, 0, 0, 1));
                        *startPtr32++ = Xse.cvtsi128_si32(reverseHi);
                        *endPtr32 = Xse.cvtsi128_si32(reverseLo);
                    }

                    if (ConstChecks(startPtr32, numRemainders)) return;
                    ExchangeLastPair((ushort*)startPtr32, numRemainders);
                }
            }


Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                if (ConstChecks(ptr, (ulong)length)) return;

                v256* startPtrLow = (v256*)ptr;
                v256* startPtrHigh = startPtrLow + 1;
                v256* endPtrHigh = (v256*)(ptr + (length - SHORTS_IN_V256));
                v256* endPtrLow = endPtrHigh - 1;

                while (startPtrHigh + 1 <= endPtrLow)
                {
                    v256 lolo = Avx2.mm256_shuffle_epi8(*startPtrLow, MM256_REVERSE_MASK_EPI16);
                    v256 lohi = Avx2.mm256_shuffle_epi8(*startPtrHigh, MM256_REVERSE_MASK_EPI16);
                    v256 hihi = Avx2.mm256_shuffle_epi8(*endPtrHigh, MM256_REVERSE_MASK_EPI16);
                    v256 hilo = Avx2.mm256_shuffle_epi8(*endPtrLow, MM256_REVERSE_MASK_EPI16);
                    *endPtrHigh = Avx2.mm256_permute4x64_epi64(lolo, Sse.SHUFFLE(1, 0, 3, 2));
                    *endPtrLow = Avx2.mm256_permute4x64_epi64(lohi, Sse.SHUFFLE(1, 0, 3, 2));
                    *startPtrLow = Avx2.mm256_permute4x64_epi64(hihi, Sse.SHUFFLE(1, 0, 3, 2));
                    *startPtrHigh = Avx2.mm256_permute4x64_epi64(hilo, Sse.SHUFFLE(1, 0, 3, 2));

                    startPtrLow += 2;
                    startPtrHigh += 2;
                    endPtrHigh -= 2;
                    endPtrLow -= 2;
                }

                ulong numRemainders = (ulong)length % (4 * SHORTS_IN_V256);

                v256* startPtr256 = (v256*)startPtrLow;
                if (ConstChecks(startPtr256, numRemainders)) return;
                if (numRemainders >= 2 * SHORTS_IN_V256)
                {
                    numRemainders -= SHORTS_IN_V256;
                    v256* endPtr256 = (v256*)((ushort*)startPtr256 + numRemainders);
                    numRemainders -= SHORTS_IN_V256;

                    v256 lo = Avx2.mm256_shuffle_epi8(*startPtr256, MM256_REVERSE_MASK_EPI16);
                    v256 hi = Avx2.mm256_shuffle_epi8(*endPtr256, MM256_REVERSE_MASK_EPI16);
                    *startPtr256++ = Avx2.mm256_permute4x64_epi64(hi, Sse.SHUFFLE(1, 0, 3, 2));
                    *endPtr256 = Avx2.mm256_permute4x64_epi64(lo, Sse.SHUFFLE(1, 0, 3, 2));
                }

                ScalarChain((v128*)startPtr256, numRemainders);
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                if (ConstChecks(ptr, (ulong)length)) return;

                v128* startPtrLow = (v128*)ptr;
                v128* startPtrHigh = startPtrLow + 1;
                v128* endPtrHigh = (v128*)(ptr + (length - SHORTS_IN_V128));
                v128* endPtrLow = endPtrHigh - 1;

                while (startPtrHigh + 1 <= endPtrLow)
                {
                    v128 lolo = reverse(*(ushort8*)startPtrLow);
                    v128 lohi = reverse(*(ushort8*)startPtrHigh);
                    v128 hihi = reverse(*(ushort8*)endPtrHigh);
                    v128 hilo = reverse(*(ushort8*)endPtrLow);
                    *endPtrHigh = lolo;
                    *endPtrLow = lohi;
                    *startPtrLow = hihi;
                    *startPtrHigh = hilo;

                    startPtrLow += 2;
                    startPtrHigh += 2;
                    endPtrHigh -= 2;
                    endPtrLow -= 2;
                }

                ulong numRemainders = (ulong)length % (4 * SHORTS_IN_V128);

                ScalarChain(startPtrLow, numRemainders);
            }
            else
            {
                StdReverse(ptr, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Reverse3Bytes(Byte3* ptr, long length)
        {
            static bool ConstChecks(void* startPtr, ulong numRemainders)
            {
                if (constexpr.IS_TRUE(numRemainders == 32))
                {
                    if (BurstArchitecture.IsTableLookupSupported)
                    {
                        v128* startPtr0 = (v128*)startPtr;
                        v128* startPtr1 = (v128*)((byte*)startPtr0 + 12);
                        v128* startPtr2 = (v128*)((byte*)startPtr0 + 24);
                        v128* startPtr3 = (v128*)((byte*)startPtr0 + 32);
                        v128* endPtr0 = (v128*)((Byte3*)startPtr + (numRemainders - (uint)sizeof(v128)));
                        v128* endPtr1 = (v128*)((byte*)endPtr0 + 12);
                        v128* endPtr2 = (v128*)((byte*)endPtr0 + 24);
                        v128* endPtr3 = (v128*)((byte*)endPtr0 + 32);

                        v128 low0 = Xse.shuffle_epi8(*startPtr0, REVERSE_MASK_3BYTE_0);
                        v128 low1 = Xse.shuffle_epi8(*startPtr1, REVERSE_MASK_3BYTE_2);
                        v128 low2 = Xse.shuffle_epi8(*startPtr2, REVERSE_MASK_3BYTE_2);
                        v128 low3 = Xse.shuffle_epi8(*startPtr3, REVERSE_MASK_3BYTE_1);
                        v128 high0 = Xse.shuffle_epi8(*endPtr0, REVERSE_MASK_3BYTE_0);
                        v128 high1 = Xse.shuffle_epi8(*endPtr1, REVERSE_MASK_3BYTE_2);
                        v128 high2 = Xse.shuffle_epi8(*endPtr2, REVERSE_MASK_3BYTE_2);
                        v128 high3 = Xse.shuffle_epi8(*endPtr3, REVERSE_MASK_3BYTE_1);

                        endPtr0[0] = Xse.alignr_epi8(low3, low2, 4);
                        endPtr0[1] = Xse.alignr_epi8(low2, low1, 8);
                        endPtr0[2] = Xse.alignr_epi8(low1, low0, 12);
                        startPtr0[0] = Xse.alignr_epi8(high3, high2, 4);
                        startPtr0[1] = Xse.alignr_epi8(high2, high1, 8);
                        startPtr0[2] = Xse.alignr_epi8(high1, high0, 12);

                        return true;
                    }
                }

                return false;
            }


Assert.IsNonNegative(length);

            if (BurstArchitecture.IsTableLookupSupported)
            {
                if (ConstChecks(ptr, (ulong)length)) return;

                v128* startPtr0 = (v128*)ptr;
                v128* startPtr1 = (v128*)((byte*)startPtr0 + 12);
                v128* startPtr2 = (v128*)((byte*)startPtr0 + 24);
                v128* startPtr3 = (v128*)((byte*)startPtr0 + 32);
                v128* endPtr0 = (v128*)(ptr + (length - 16));
                v128* endPtr1 = (v128*)((byte*)endPtr0 + 12);
                v128* endPtr2 = (v128*)((byte*)endPtr0 + 24);
                v128* endPtr3 = (v128*)((byte*)endPtr0 + 32);

                while (startPtr3 + 1 <= endPtr0)
                {
                    v128 low0 = Xse.shuffle_epi8(*startPtr0, REVERSE_MASK_3BYTE_0);
                    v128 low1 = Xse.shuffle_epi8(*startPtr1, REVERSE_MASK_3BYTE_2);
                    v128 low2 = Xse.shuffle_epi8(*startPtr2, REVERSE_MASK_3BYTE_2);
                    v128 low3 = Xse.shuffle_epi8(*startPtr3, REVERSE_MASK_3BYTE_1);
                    v128 high0 = Xse.shuffle_epi8(*endPtr0, REVERSE_MASK_3BYTE_0);
                    v128 high1 = Xse.shuffle_epi8(*endPtr1, REVERSE_MASK_3BYTE_2);
                    v128 high2 = Xse.shuffle_epi8(*endPtr2, REVERSE_MASK_3BYTE_2);
                    v128 high3 = Xse.shuffle_epi8(*endPtr3, REVERSE_MASK_3BYTE_1);

                    endPtr0[0] = Xse.alignr_epi8(low3, low2, 4);
                    endPtr0[1] = Xse.alignr_epi8(low2, low1, 8);
                    endPtr0[2] = Xse.alignr_epi8(low1, low0, 12);
                    startPtr0[0] = Xse.alignr_epi8(high3, high2, 4);
                    startPtr0[1] = Xse.alignr_epi8(high2, high1, 8);
                    startPtr0[2] = Xse.alignr_epi8(high1, high0, 12);

                    startPtr0 += 3;
                    startPtr1 += 3;
                    startPtr2 += 3;
                    startPtr3 += 3;
                    endPtr0 -= 3;
                    endPtr1 -= 3;
                    endPtr2 -= 3;
                    endPtr3 -= 3;
                }

                ulong numRemainders = (ulong)length % 32u;
                // TODO ConstChecks

                StdReverse((Byte3*)startPtr0, (long)numRemainders);
            }
            else
            {
                StdReverse((Byte3*)ptr, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Reverse4Bytes(uint* ptr, long length)
        {
            static bool ConstChecks(void* startPtr, ulong numRemainders)
            {
                if (BurstArchitecture.IsSIMDSupported)
                {
                    if (constexpr.IS_TRUE(numRemainders == 4 * INTS_IN_V256))
                    {
                        if (Avx2.IsAvx2Supported)
                        {
                            v256* startPtrLow = (v256*)startPtr;
                            v256* startPtrHigh = startPtrLow + 1;
                            v256* endPtrHigh = (v256*)((uint*)startPtr + (3 * INTS_IN_V256));
                            v256* endPtrLow = endPtrHigh - 1;

                            v256 lolo = Avx2.mm256_permutevar8x32_ps(*startPtrLow, REVERSE_MASK_EPI32);
                            v256 lohi = Avx2.mm256_permutevar8x32_ps(*startPtrHigh, REVERSE_MASK_EPI32);
                            v256 hihi = Avx2.mm256_permutevar8x32_ps(*endPtrHigh, REVERSE_MASK_EPI32);
                            v256 hilo = Avx2.mm256_permutevar8x32_ps(*endPtrLow, REVERSE_MASK_EPI32);
                            *endPtrHigh = lolo;
                            *endPtrLow = lohi;
                            *startPtrLow = hihi;
                            *startPtrHigh = hilo;

                            return true;
                        }
                    }
                    if (constexpr.IS_TRUE(numRemainders == 2 * INTS_IN_V256))
                    {
                        if (Avx2.IsAvx2Supported)
                        {
                            v256* endPtr256 = (v256*)startPtr + 1;

                            v256 lo = Avx2.mm256_permutevar8x32_ps(*(v256*)startPtr, REVERSE_MASK_EPI32);
                            v256 hi = Avx2.mm256_permutevar8x32_ps(*endPtr256, REVERSE_MASK_EPI32);
                            *(v256*)startPtr = hi;
                            *endPtr256 = lo;
                        }
                        else
                        {
                            v128* startPtrLow = (v128*)startPtr;
                            v128* startPtrHigh = startPtrLow + 1;
                            v128* endPtrHigh = (v128*)((uint*)startPtr + (3 * INTS_IN_V128));
                            v128* endPtrLow = endPtrHigh - 1;

                            v128 lolo = Xse.shuffle_epi32(*startPtrLow, Sse.SHUFFLE(0, 1, 2, 3));
                            v128 lohi = Xse.shuffle_epi32(*startPtrHigh, Sse.SHUFFLE(0, 1, 2, 3));
                            v128 hihi = Xse.shuffle_epi32(*endPtrHigh, Sse.SHUFFLE(0, 1, 2, 3));
                            v128 hilo = Xse.shuffle_epi32(*endPtrLow, Sse.SHUFFLE(0, 1, 2, 3));
                            *endPtrHigh = lolo;
                            *endPtrLow = lohi;
                            *startPtrLow = hihi;
                            *startPtrHigh = hilo;
                        }

                        return true;
                    }

                    if (constexpr.IS_TRUE(numRemainders == 2 * INTS_IN_V128))
                    {
                        v128* endPtr128 = (v128*)startPtr + 1;

                        v128 lo = Xse.shuffle_epi32(*(v128*)startPtr, Sse.SHUFFLE(0, 1, 2, 3));
                        v128 hi = Xse.shuffle_epi32(*endPtr128, Sse.SHUFFLE(0, 1, 2, 3));
                        *(v128*)startPtr = hi;
                        *endPtr128 = lo;

                        return true;
                    }

                    if (constexpr.IS_TRUE(numRemainders == INTS_IN_V128))
                    {
                        *(v128*)startPtr = Xse.shuffle_epi32(*(v128*)startPtr, Sse.SHUFFLE(0, 1, 2, 3));

                        return true;
                    }
                }

                if (constexpr.IS_TRUE(numRemainders == INTS_IN_LONG))
                {
                    *(ulong*)startPtr = rol(*(ulong*)startPtr, 8 * sizeof(uint));

                    return true;
                }

                return false;
            }
            static void ScalarChain128(v128* startPtr128, ulong numRemainders)
            {
                if (ConstChecks(startPtr128, numRemainders)) return;
                if (BurstArchitecture.IsSIMDSupported)
                {
                    if (numRemainders >= 2 * INTS_IN_V128)
                    {
                        numRemainders -= INTS_IN_V128;
                        v128* endPtr128 = (v128*)((uint*)startPtr128 + numRemainders);
                        numRemainders -= INTS_IN_V128;

                        v128 lo = Xse.shuffle_epi32(*startPtr128, Sse.SHUFFLE(0, 1, 2, 3));
                        v128 hi = Xse.shuffle_epi32(*endPtr128, Sse.SHUFFLE(0, 1, 2, 3));
                        *startPtr128++ = hi;
                        *endPtr128 = lo;
                    }
                }

                if (ConstChecks(startPtr128, numRemainders)) return;
                ulong* startPtr64 = (ulong*)startPtr128;
                if (numRemainders >= 2 * INTS_IN_LONG)
                {
                    numRemainders -= INTS_IN_LONG;
                    ulong* endPtr64 = (ulong*)((uint*)startPtr64 + numRemainders);
                    numRemainders -= INTS_IN_LONG;

                    ulong lo = rol( *startPtr64, 8 * sizeof(uint));
                    ulong hi = ror(*endPtr64, 8 * sizeof(uint));
                    *startPtr64++ = hi;
                    *endPtr64 = lo;
                }

                if (ConstChecks(startPtr64, numRemainders)) return;
                ExchangeLastPair((uint*)startPtr64, numRemainders);
            }


Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                if (ConstChecks(ptr, (ulong)length)) return;

                v256* startPtrLow = (v256*)ptr;
                v256* startPtrHigh = startPtrLow + 1;
                v256* endPtrHigh = (v256*)(ptr + (length - INTS_IN_V256));
                v256* endPtrLow = endPtrHigh - 1;

                while (startPtrHigh + 1 <= endPtrLow)
                {
                    v256 lolo = Avx2.mm256_permutevar8x32_ps(*startPtrLow, REVERSE_MASK_EPI32);
                    v256 lohi = Avx2.mm256_permutevar8x32_ps(*startPtrHigh, REVERSE_MASK_EPI32);
                    v256 hihi = Avx2.mm256_permutevar8x32_ps(*endPtrHigh, REVERSE_MASK_EPI32);
                    v256 hilo = Avx2.mm256_permutevar8x32_ps(*endPtrLow, REVERSE_MASK_EPI32);
                    *endPtrHigh = lolo;
                    *endPtrLow = lohi;
                    *startPtrLow = hihi;
                    *startPtrHigh = hilo;

                    startPtrLow += 2;
                    startPtrHigh += 2;
                    endPtrHigh -= 2;
                    endPtrLow -= 2;
                }

                ulong numRemainders = (ulong)length % (4 * INTS_IN_V256);

                v256* startPtr256 = (v256*)startPtrLow;
                if (ConstChecks(startPtr256, numRemainders)) return;
                if (numRemainders >= 2 * INTS_IN_V256)
                {
                    numRemainders -= INTS_IN_V256;
                    v256* endPtr256 = (v256*)((uint*)startPtr256 + numRemainders);
                    numRemainders -= INTS_IN_V256;

                    v256 lo = Avx2.mm256_permutevar8x32_ps(*startPtr256, REVERSE_MASK_EPI32);
                    v256 hi = Avx2.mm256_permutevar8x32_ps(*endPtr256, REVERSE_MASK_EPI32);
                    *startPtr256++ = hi;
                    *endPtr256 = lo;
                }

                ScalarChain128((v128*)startPtr256, numRemainders);
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                if (ConstChecks(ptr, (ulong)length)) return;

                v128* startPtrLow = (v128*)ptr;
                v128* startPtrHigh = startPtrLow + 1;
                v128* endPtrHigh = (v128*)(ptr + (length - INTS_IN_V128));
                v128* endPtrLow = endPtrHigh - 1;

                while (startPtrHigh + 1 <= endPtrLow)
                {
                    v128 lolo = Xse.shuffle_epi32(*startPtrLow, Sse.SHUFFLE(0, 1, 2, 3));
                    v128 lohi = Xse.shuffle_epi32(*startPtrHigh, Sse.SHUFFLE(0, 1, 2, 3));
                    v128 hihi = Xse.shuffle_epi32(*endPtrHigh, Sse.SHUFFLE(0, 1, 2, 3));
                    v128 hilo = Xse.shuffle_epi32(*endPtrLow, Sse.SHUFFLE(0, 1, 2, 3));
                    *endPtrHigh = lolo;
                    *endPtrLow = lohi;
                    *startPtrLow = hihi;
                    *startPtrHigh = hilo;

                    startPtrLow += 2;
                    startPtrHigh += 2;
                    endPtrHigh -= 2;
                    endPtrLow -= 2;
                }

                ulong numRemainders = (ulong)length % (4 * INTS_IN_V128);

                ScalarChain128((v128*)startPtrLow, numRemainders);
            }
            else
            {
                StdReverse(ptr, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Reverse5Bytes(Byte5* ptr, long length)
        {
            static bool ConstChecks(void* startPtr, ulong numRemainders)
            {
                if (constexpr.IS_TRUE(numRemainders == 30u))
                {
                    if (BurstArchitecture.IsTableLookupSupported)
                    {


                        return true;
                    }
                }

                return false;
            }


Assert.IsNonNegative(length);

            if (BurstArchitecture.IsTableLookupSupported)
            {
                if (ConstChecks(ptr, (ulong)length)) return;

                v128* startPtr0 = (v128*)ptr;
                v128* startPtr1 = (v128*)((byte*)startPtr0 + 15);
                v128* startPtr2 = (v128*)((byte*)startPtr0 + 30);
                v128* startPtr3 = (v128*)((byte*)startPtr0 + 45);
                v128* startPtr4 = (v128*)((byte*)startPtr0 + 60);
                v128* startPtr5 = (v128*)((byte*)startPtr0 + 75);
                v128* endPtr0 = (v128*)(ptr + (length - 15));
                v128* endPtr1 = (v128*)((byte*)endPtr0 + 15);
                v128* endPtr2 = (v128*)((byte*)endPtr0 + 30);
                v128* endPtr3 = (v128*)((byte*)endPtr0 + 45);
                v128* endPtr4 = (v128*)((byte*)endPtr0 + 60);
                v128* endPtr5 = (v128*)((byte*)endPtr0 + 75);

                v128 REVERSE_MASK_5BYTE_0 = new v128(-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,    0, 1, 2, 3, 4);
                v128 REVERSE_MASK_5BYTE_1 = new v128(10, 11, 12, 13, 14,    5, 6, 7, 8, 9,    0, 1, 2, 3, 4,    -1);

                while (startPtr5 + 1 <= endPtr0)
                {
                    v128 low0 = *startPtr0;
                    v128 low1 = *startPtr1;
                    v128 low2 = *startPtr2;
                    v128 low3 = *startPtr3;
                    v128 low4 = *startPtr4;
                    v128 low5 = *startPtr5;
                    v128 high0 = *endPtr0;
                    v128 high1 = *endPtr1;
                    v128 high2 = *endPtr2;
                    v128 high3 = *endPtr3;
                    v128 high4 = *endPtr4;
                    v128 high5 = *endPtr5;

                    endPtr0[0]   = Xse.alignr_epi8(Xse.shuffle_epi8(low5,  REVERSE_MASK_5BYTE_0), Xse.shuffle_epi8(low4,  REVERSE_MASK_5BYTE_1), 11);
                    endPtr0[1]   = Xse.alignr_epi8(Xse.shuffle_epi8(low4,  REVERSE_MASK_5BYTE_0), Xse.shuffle_epi8(low3,  REVERSE_MASK_5BYTE_1), 12);
                    endPtr0[2]   = Xse.alignr_epi8(Xse.shuffle_epi8(low3,  REVERSE_MASK_5BYTE_0), Xse.shuffle_epi8(low2,  REVERSE_MASK_5BYTE_1), 13);
                    endPtr0[3]   = Xse.alignr_epi8(Xse.shuffle_epi8(low2,  REVERSE_MASK_5BYTE_0), Xse.shuffle_epi8(low1,  REVERSE_MASK_5BYTE_1), 14);
                    endPtr0[4]   = Xse.alignr_epi8(Xse.shuffle_epi8(low1,  REVERSE_MASK_5BYTE_0), Xse.shuffle_epi8(low0,  REVERSE_MASK_5BYTE_1), 15);

                    startPtr0[0] = Xse.alignr_epi8(Xse.shuffle_epi8(high5, REVERSE_MASK_5BYTE_0), Xse.shuffle_epi8(high4, REVERSE_MASK_5BYTE_1), 11);
                    startPtr0[1] = Xse.alignr_epi8(Xse.shuffle_epi8(high4, REVERSE_MASK_5BYTE_0), Xse.shuffle_epi8(high3, REVERSE_MASK_5BYTE_1), 12);
                    startPtr0[2] = Xse.alignr_epi8(Xse.shuffle_epi8(high3, REVERSE_MASK_5BYTE_0), Xse.shuffle_epi8(high2, REVERSE_MASK_5BYTE_1), 13);
                    startPtr0[3] = Xse.alignr_epi8(Xse.shuffle_epi8(high2, REVERSE_MASK_5BYTE_0), Xse.shuffle_epi8(high1, REVERSE_MASK_5BYTE_1), 14);
                    startPtr0[4] = Xse.alignr_epi8(Xse.shuffle_epi8(high1, REVERSE_MASK_5BYTE_0), Xse.shuffle_epi8(high0, REVERSE_MASK_5BYTE_1), 15);

                    startPtr0 += 5;
                    startPtr1 += 5;
                    startPtr2 += 5;
                    startPtr3 += 5;
                    startPtr4 += 5;
                    startPtr5 += 5;
                    endPtr0 -= 5;
                    endPtr1 -= 5;
                    endPtr2 -= 5;
                    endPtr3 -= 5;
                    endPtr4 -= 5;
                    endPtr5 -= 5;
                }

                ulong numRemainders = (ulong)length % 30u;
                // TODO ConstChecks

                StdReverse((Byte5*)startPtr0, (long)numRemainders);
            }
            else
            {
                StdReverse((Byte5*)ptr, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Reverse6Bytes(Byte6* ptr, long length)
        {
            static bool ConstChecks(void* startPtr, ulong numRemainders)
            {
                if (constexpr.IS_TRUE(numRemainders == 16))
                {
                    v128* startPtr0 = (v128*)startPtr;
                    v128* startPtr1 = (v128*)((byte*)startPtr0 + 12);
                    v128* startPtr2 = (v128*)((byte*)startPtr0 + 24);
                    v128* startPtr3 = (v128*)((byte*)startPtr0 + 32);
                    v128* endPtr0 = (v128*)((Byte6*)startPtr + (numRemainders - 8));
                    v128* endPtr1 = (v128*)((byte*)endPtr0 + 12);
                    v128* endPtr2 = (v128*)((byte*)endPtr0 + 24);
                    v128* endPtr3 = (v128*)((byte*)endPtr0 + 32);

                    v128 low0 = Xse.shuffle_epi8(*startPtr0, REVERSE_MASK_6BYTE_0);
                    v128 low1 = Xse.shuffle_epi8(*startPtr1, REVERSE_MASK_6BYTE_2);
                    v128 low2 = Xse.shuffle_epi8(*startPtr2, REVERSE_MASK_6BYTE_2);
                    v128 low3 = Xse.shuffle_epi8(*startPtr3, REVERSE_MASK_6BYTE_1);
                    v128 high0 = Xse.shuffle_epi8(*endPtr0, REVERSE_MASK_6BYTE_0);
                    v128 high1 = Xse.shuffle_epi8(*endPtr1, REVERSE_MASK_6BYTE_2);
                    v128 high2 = Xse.shuffle_epi8(*endPtr2, REVERSE_MASK_6BYTE_2);
                    v128 high3 = Xse.shuffle_epi8(*endPtr3, REVERSE_MASK_6BYTE_1);

                    endPtr0[0] = Xse.alignr_epi8(low3, low2, 4);
                    endPtr0[1] = Xse.alignr_epi8(low2, low1, 8);
                    endPtr0[2] = Xse.alignr_epi8(low1, low0, 12);
                    startPtr0[0] = Xse.alignr_epi8(high3, high2, 4);
                    startPtr0[1] = Xse.alignr_epi8(high2, high1, 8);
                    startPtr0[2] = Xse.alignr_epi8(high1, high0, 12);

                    return true;
                }

                return false;
            }


Assert.IsNonNegative(length);

            if (BurstArchitecture.IsTableLookupSupported)
            {
                if (ConstChecks(ptr, (ulong)length)) return;

                v128* startPtr0 = (v128*)ptr;
                v128* startPtr1 = (v128*)((byte*)startPtr0 + 12);
                v128* startPtr2 = (v128*)((byte*)startPtr0 + 24);
                v128* startPtr3 = (v128*)((byte*)startPtr0 + 32);
                v128* endPtr0 = (v128*)(ptr + (length - 8));
                v128* endPtr1 = (v128*)((byte*)endPtr0 + 12);
                v128* endPtr2 = (v128*)((byte*)endPtr0 + 24);
                v128* endPtr3 = (v128*)((byte*)endPtr0 + 32);

                while (startPtr3 + 1 <= endPtr0)
                {
                    v128 low0 = Xse.shuffle_epi8(*startPtr0, REVERSE_MASK_6BYTE_0);
                    v128 low1 = Xse.shuffle_epi8(*startPtr1, REVERSE_MASK_6BYTE_2);
                    v128 low2 = Xse.shuffle_epi8(*startPtr2, REVERSE_MASK_6BYTE_2);
                    v128 low3 = Xse.shuffle_epi8(*startPtr3, REVERSE_MASK_6BYTE_1);
                    v128 high0 = Xse.shuffle_epi8(*endPtr0, REVERSE_MASK_6BYTE_0);
                    v128 high1 = Xse.shuffle_epi8(*endPtr1, REVERSE_MASK_6BYTE_2);
                    v128 high2 = Xse.shuffle_epi8(*endPtr2, REVERSE_MASK_6BYTE_2);
                    v128 high3 = Xse.shuffle_epi8(*endPtr3, REVERSE_MASK_6BYTE_1);

                    endPtr0[0] = Xse.alignr_epi8(low3, low2, 4);
                    endPtr0[1] = Xse.alignr_epi8(low2, low1, 8);
                    endPtr0[2] = Xse.alignr_epi8(low1, low0, 12);
                    startPtr0[0] = Xse.alignr_epi8(high3, high2, 4);
                    startPtr0[1] = Xse.alignr_epi8(high2, high1, 8);
                    startPtr0[2] = Xse.alignr_epi8(high1, high0, 12);

                    startPtr0 += 3;
                    startPtr1 += 3;
                    startPtr2 += 3;
                    startPtr3 += 3;
                    endPtr0 -= 3;
                    endPtr1 -= 3;
                    endPtr2 -= 3;
                    endPtr3 -= 3;
                }

                ulong numRemainders = (ulong)length % 16u;
                // TODO ConstChecks

                StdReverse((Byte6*)startPtr0, (long)numRemainders);
            }
            else
            {
                StdReverse((Byte6*)ptr, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Reverse8Bytes(ulong* ptr, long length)
        {
            static bool ConstChecks(void* startPtr, ulong numRemainders)
            {
                if (BurstArchitecture.IsSIMDSupported)
                {
                    if (constexpr.IS_TRUE(numRemainders == 4 * LONGS_IN_V256))
                    {
                        if (Avx2.IsAvx2Supported)
                        {
                            v256* startPtrLow = (v256*)startPtr;
                            v256* startPtrHigh = startPtrLow + 1;
                            v256* endPtrHigh = (v256*)((ulong*)startPtr + (3 * LONGS_IN_V256));
                            v256* endPtrLow = endPtrHigh - 1;

                            v256 lolo = Avx2.mm256_permute4x64_pd(*startPtrLow, Sse.SHUFFLE(0, 1, 2, 3));
                            v256 lohi = Avx2.mm256_permute4x64_pd(*startPtrHigh, Sse.SHUFFLE(0, 1, 2, 3));
                            v256 hihi = Avx2.mm256_permute4x64_pd(*endPtrHigh, Sse.SHUFFLE(0, 1, 2, 3));
                            v256 hilo = Avx2.mm256_permute4x64_pd(*endPtrLow, Sse.SHUFFLE(0, 1, 2, 3));
                            *endPtrHigh = lolo;
                            *endPtrLow = lohi;
                            *startPtrLow = hihi;
                            *startPtrHigh = hilo;

                            return true;
                        }
                    }

                    if (constexpr.IS_TRUE(numRemainders == 2 * LONGS_IN_V256))
                    {
                        if (Avx2.IsAvx2Supported)
                        {
                            v256* endPtr256 = (v256*)startPtr + 1;

                            v256 lo = Avx2.mm256_permute4x64_pd(*(v256*)startPtr, Sse.SHUFFLE(0, 1, 2, 3));
                            v256 hi = Avx2.mm256_permute4x64_pd(*endPtr256, Sse.SHUFFLE(0, 1, 2, 3));
                            *(v256*)startPtr = hi;
                            *endPtr256 = lo;
                        }
                        else
                        {
                            v128* startPtrLow = (v128*)startPtr;
                            v128* startPtrHigh = startPtrLow + 1;
                            v128* endPtrHigh = (v128*)((ulong*)startPtr + (3 * LONGS_IN_V128));
                            v128* endPtrLow = endPtrHigh - 1;

                            v128 lolo = Xse.shuffle_epi32(*startPtrLow, Sse.SHUFFLE(1, 0, 3, 2));
                            v128 lohi = Xse.shuffle_epi32(*startPtrHigh, Sse.SHUFFLE(1, 0, 3, 2));
                            v128 hihi = Xse.shuffle_epi32(*endPtrHigh, Sse.SHUFFLE(1, 0, 3, 2));
                            v128 hilo = Xse.shuffle_epi32(*endPtrLow, Sse.SHUFFLE(1, 0, 3, 2));
                            *endPtrHigh = lolo;
                            *endPtrLow = lohi;
                            *startPtrLow = hihi;
                            *startPtrHigh = hilo;
                        }

                        return true;
                    }

                    if (constexpr.IS_TRUE(numRemainders == 2 * LONGS_IN_V128))
                    {
                        v128* endPtr128 = (v128*)startPtr + 1;

                        v128 lo = Xse.shuffle_epi32(*(v128*)startPtr, Sse.SHUFFLE(1, 0, 3, 2));
                        v128 hi = Xse.shuffle_epi32(*endPtr128, Sse.SHUFFLE(1, 0, 3, 2));
                        *(v128*)startPtr = hi;
                        *endPtr128 = lo;

                        return true;
                    }

                    if (constexpr.IS_TRUE(numRemainders == LONGS_IN_V128))
                    {
                        *(v128*)startPtr = Xse.shuffle_epi32(*(v128*)startPtr, Sse.SHUFFLE(1, 0, 3, 2));

                        return true;
                    }
                }

                return false;
            }
            static void ScalarChain256(v128* startPtr128, ulong numRemainders)
            {
                if (ConstChecks(startPtr128, numRemainders)) return;

                if (numRemainders >= 2 * LONGS_IN_V128)
                {
                    if (BurstArchitecture.IsSIMDSupported)
                    {
                        numRemainders -= LONGS_IN_V128;
                        v128* endPtr128 = (v128*)((ulong*)startPtr128 + numRemainders);
                        numRemainders -= LONGS_IN_V128;

                        v128 lo = Xse.shuffle_epi32(*startPtr128, Sse.SHUFFLE(1, 0, 3, 2));
                        v128 hi = Xse.shuffle_epi32(*endPtr128, Sse.SHUFFLE(1, 0, 3, 2));
                        *startPtr128++ = hi;
                        *endPtr128 = lo;
                    }
                }

                if (ConstChecks(startPtr128, numRemainders)) return;

                ExchangeLastPair((ulong*)startPtr128, numRemainders);
            }


Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                if (ConstChecks(ptr, (ulong)length)) return;

                v256* startPtrLow = (v256*)ptr;
                v256* startPtrHigh = startPtrLow + 1;
                v256* endPtrHigh = (v256*)(ptr + (length - LONGS_IN_V256));
                v256* endPtrLow = endPtrHigh - 1;

                while (startPtrHigh + 1 <= endPtrLow)
                {
                    v256 lolo = Avx2.mm256_permute4x64_pd(*startPtrLow, Sse.SHUFFLE(0, 1, 2, 3));
                    v256 lohi = Avx2.mm256_permute4x64_pd(*startPtrHigh, Sse.SHUFFLE(0, 1, 2, 3));
                    v256 hihi = Avx2.mm256_permute4x64_pd(*endPtrHigh, Sse.SHUFFLE(0, 1, 2, 3));
                    v256 hilo = Avx2.mm256_permute4x64_pd(*endPtrLow, Sse.SHUFFLE(0, 1, 2, 3));
                    *endPtrHigh = lolo;
                    *endPtrLow = lohi;
                    *startPtrLow = hihi;
                    *startPtrHigh = hilo;

                    startPtrLow += 2;
                    startPtrHigh += 2;
                    endPtrHigh -= 2;
                    endPtrLow -= 2;
                }

                ulong numRemainders = (ulong)length % (4 * LONGS_IN_V256);

                v256* startPtr256 = (v256*)startPtrLow;
                if (ConstChecks(startPtr256, numRemainders)) return;
                if (numRemainders >= 2 * LONGS_IN_V256)
                {
                    numRemainders -= LONGS_IN_V256;
                    v256* endPtr256 = (v256*)((ulong*)startPtr256 + numRemainders);
                    numRemainders -= LONGS_IN_V256;

                    v256 lo = Avx2.mm256_permute4x64_pd(*startPtr256, Sse.SHUFFLE(0, 1, 2, 3));
                    v256 hi = Avx2.mm256_permute4x64_pd(*endPtr256, Sse.SHUFFLE(0, 1, 2, 3));
                    *startPtr256++ = hi;
                    *endPtr256 = lo;
                }

                ScalarChain256((v128*)startPtr256, numRemainders);
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                if (ConstChecks(ptr, (ulong)length)) return;

                v128* startPtrLow = (v128*)ptr;
                v128* startPtrHigh = startPtrLow + 1;
                v128* endPtrHigh = (v128*)(ptr + (length - LONGS_IN_V128));
                v128* endPtrLow = endPtrHigh - 1;

                while (startPtrHigh + 1 <= endPtrLow)
                {
                    v128 lolo = Xse.shuffle_epi32(*startPtrLow, Sse.SHUFFLE(1, 0, 3, 2));
                    v128 lohi = Xse.shuffle_epi32(*startPtrHigh, Sse.SHUFFLE(1, 0, 3, 2));
                    v128 hihi = Xse.shuffle_epi32(*endPtrHigh, Sse.SHUFFLE(1, 0, 3, 2));
                    v128 hilo = Xse.shuffle_epi32(*endPtrLow, Sse.SHUFFLE(1, 0, 3, 2));
                    *endPtrHigh = lolo;
                    *endPtrLow = lohi;
                    *startPtrLow = hihi;
                    *startPtrHigh = hilo;

                    startPtrLow += 2;
                    startPtrHigh += 2;
                    endPtrHigh -= 2;
                    endPtrLow -= 2;
                }

                ulong numRemainders = (ulong)length % (4 * LONGS_IN_V128);

                ScalarChain256(startPtrLow, numRemainders);
            }
            else
            {
                StdReverse(ptr, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Reverse16Bytes(v128* ptr, long length)
        {
            static bool ConstChecks(void* startPtr, ulong numRemainders)
            {
                if (Avx2.IsAvx2Supported)
                {
                    if (constexpr.IS_TRUE(numRemainders == 4 * V128_IN_V256))
                    {
                        v256* startPtrLow = (v256*)startPtr;
                        v256* startPtrHigh = startPtrLow + 1;
                        v256* endPtrHigh = (v256*)((v128*)startPtr + (3 * V128_IN_V256));
                        v256* endPtrLow = endPtrHigh - 1;

                        v256 lolo = Avx2.mm256_permute4x64_pd(*startPtrLow, Sse.SHUFFLE(1, 0, 3, 2));
                        v256 lohi = Avx2.mm256_permute4x64_pd(*startPtrHigh, Sse.SHUFFLE(1, 0, 3, 2));
                        v256 hihi = Avx2.mm256_permute4x64_pd(*endPtrHigh, Sse.SHUFFLE(1, 0, 3, 2));
                        v256 hilo = Avx2.mm256_permute4x64_pd(*endPtrLow, Sse.SHUFFLE(1, 0, 3, 2));
                        *endPtrHigh = lolo;
                        *endPtrLow = lohi;
                        *startPtrLow = hihi;
                        *startPtrHigh = hilo;

                        return true;
                    }

                    if (constexpr.IS_TRUE(numRemainders == 2 * V128_IN_V256))
                    {
                        v256* endPtr256 = (v256*)startPtr + 1;

                        v256 lo = Avx2.mm256_permute4x64_pd(*(v256*)startPtr, Sse.SHUFFLE(1, 0, 3, 2));
                        v256 hi = Avx2.mm256_permute4x64_pd(*endPtr256, Sse.SHUFFLE(1, 0, 3, 2));
                        *(v256*)startPtr = hi;
                        *endPtr256 = lo;

                        return true;
                    }

                    if (constexpr.IS_TRUE(numRemainders == V128_IN_V256))
                    {
                        v128* endPtr128 = (v128*)startPtr + 1;

                        v128 lo = *(v128*)startPtr;
                        v128 hi = *endPtr128;
                        *(v128*)startPtr = hi;
                        *endPtr128 = lo;

                        return true;
                    }
                }


                return false;
            }


Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                if (ConstChecks(ptr, (ulong)length)) return;

                v256* startPtrLow = (v256*)ptr;
                v256* startPtrHigh = startPtrLow + 1;
                v256* endPtrHigh = (v256*)(ptr + (length - V128_IN_V256));
                v256* endPtrLow = endPtrHigh - 1;

                while (startPtrHigh + 1 <= endPtrLow)
                {
                    v256 lolo = Avx2.mm256_permute4x64_pd(*startPtrLow, Sse.SHUFFLE(1, 0, 3, 2));
                    v256 lohi = Avx2.mm256_permute4x64_pd(*startPtrHigh, Sse.SHUFFLE(1, 0, 3, 2));
                    v256 hihi = Avx2.mm256_permute4x64_pd(*endPtrHigh, Sse.SHUFFLE(1, 0, 3, 2));
                    v256 hilo = Avx2.mm256_permute4x64_pd(*endPtrLow, Sse.SHUFFLE(1, 0, 3, 2));
                    *endPtrHigh = lolo;
                    *endPtrLow = lohi;
                    *startPtrLow = hihi;
                    *startPtrHigh = hilo;

                    startPtrLow += 2;
                    startPtrHigh += 2;
                    endPtrHigh -= 2;
                    endPtrLow -= 2;
                }

                ulong numRemainders = (ulong)length % (4 * V128_IN_V256);

                v256* startPtr256 = startPtrLow;
                if (ConstChecks(startPtr256, numRemainders)) return;
                if (numRemainders >= 2 * V128_IN_V256)
                {
                    numRemainders -= V128_IN_V256;
                    v256* endPtr256 = (v256*)((v128*)startPtr256 + numRemainders);
                    numRemainders -= V128_IN_V256;

                    v256 lo = Avx2.mm256_permute4x64_pd(*startPtr256, Sse.SHUFFLE(1, 0, 3, 2));
                    v256 hi = Avx2.mm256_permute4x64_pd(*endPtr256, Sse.SHUFFLE(1, 0, 3, 2));
                    *startPtr256++ = hi;
                    *endPtr256 = lo;
                }

                v128* startPtr128 = (v128*)startPtr256;
                if (ConstChecks(startPtr128, numRemainders)) return;
                ExchangeLastPair(startPtr128, numRemainders);
            }
            else
            {
                StdReverse(ptr, length);
            }
        }
    }
}