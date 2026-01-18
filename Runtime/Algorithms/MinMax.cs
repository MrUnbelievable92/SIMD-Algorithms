using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst.Intrinsics;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using MaxMath.Intrinsics;
using DevTools;

using static Unity.Burst.Intrinsics.X86;

namespace SIMDAlgorithms
{
    unsafe public static partial class Algorithms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(byte* ptr, long length, out byte min, out byte max)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;

                v256 max0 = Avx.mm256_set1_epi8(byte.MinValue);

                v256 min0 = Avx.mm256_set1_epi8(byte.MaxValue);

                if (Hint.Likely(length >= 128))
                {
                    v256 max1 = Avx.mm256_set1_epi8(byte.MinValue);
                    v256 max2 = Avx.mm256_set1_epi8(byte.MinValue);
                    v256 max3 = Avx.mm256_set1_epi8(byte.MinValue);

                    v256 min1 = Avx.mm256_set1_epi8(byte.MaxValue);
                    v256 min2 = Avx.mm256_set1_epi8(byte.MaxValue);
                    v256 min3 = Avx.mm256_set1_epi8(byte.MaxValue);

                    do
                    {
                        v256 load0 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load1 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load2 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load3 = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Avx2.mm256_max_epu8(max0, load0);
                        max1 = Avx2.mm256_max_epu8(max1, load1);
                        max2 = Avx2.mm256_max_epu8(max2, load2);
                        max3 = Avx2.mm256_max_epu8(max3, load3);

                        min0 = Avx2.mm256_min_epu8(min0, load0);
                        min1 = Avx2.mm256_min_epu8(min1, load1);
                        min2 = Avx2.mm256_min_epu8(min2, load2);
                        min3 = Avx2.mm256_min_epu8(min3, load3);

                        length -= 128;
                    }
                    while (Hint.Likely(length >= 128));

                    max0 = Avx2.mm256_max_epu8(max0, max1);
                    max2 = Avx2.mm256_max_epu8(max2, max3);
                    max0 = Avx2.mm256_max_epu8(max0, max2);

                    min0 = Avx2.mm256_min_epu8(min0, min1);
                    min2 = Avx2.mm256_min_epu8(min2, min3);
                    min0 = Avx2.mm256_min_epu8(min0, min2);
                }

                if (Hint.Likely((int)length >= 32))
                {
                    v256 load = Avx.mm256_loadu_si256(ptr_v256++);

                    max0 = Avx2.mm256_max_epu8(max0, load);
                    min0 = Avx2.mm256_min_epu8(min0, load);

                    if (Hint.Likely((int)length >= 2 * 32))
                    {
                        load = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Avx2.mm256_max_epu8(max0, load);
                        min0 = Avx2.mm256_min_epu8(min0, load);

                        if (Hint.Likely((int)length >= 3 * 32))
                        {
                            load = Avx.mm256_loadu_si256(ptr_v256++);

                            max0 = Avx2.mm256_max_epu8(max0, load);
                            min0 = Avx2.mm256_min_epu8(min0, load);

                            length -= 3 * 32;
                        }
                        else
                        {
                            length -= 2 * 32;
                        }
                    }
                    else
                    {
                        length -= 32;
                    }
                }

                v128 max128 = Xse.max_epu8(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));
                v128 min128 = Xse.min_epu8(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 16))
                {
                    v128 load = Xse.loadu_si128(ptr_v256);

                    max128 = Xse.max_epu8(max128, load);
                    min128 = Xse.min_epu8(min128, load);

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 16;
                }

                v128 cmp = Xse.setzero_si128();
                max128 = Xse.max_epu8(max128, Xse.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));
                min128 = Xse.min_epu8(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v256);

                    max128 = Xse.max_epu8(max128, load);
                    min128 = Xse.min_epu8(min128, load);

                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 8;
                }

                max128 = Xse.max_epu8(max128, Xse.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 3, 2)));
                min128 = Xse.min_epu8(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v256);

                    max128 = Xse.max_epu8(max128, load);
                    min128 = Xse.min_epu8(min128, load);

                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 4;
                }

                max128 = Xse.max_epu8(max128, Xse.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 0, 1)));
                min128 = Xse.min_epu8(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.insert_epi16(cmp, *(ushort*)ptr_v256, 0);

                    max128 = Xse.max_epu8(max128, load);
                    min128 = Xse.min_epu8(min128, load);

                    ptr_v256 = (v256*)((short*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Xse.max_epu8(max128, Xse.bsrli_si128(max128, 1 * sizeof(byte)));
                min128 = Xse.min_epu8(min128, Xse.bsrli_si128(min128, 1 * sizeof(byte)));

                if (Hint.Likely(length != 0))
                {
                    v128 load = Xse.insert_epi8(cmp, *(byte*)ptr_v256, 0);

                    max = Xse.max_epu8(max128, load).Byte0;
                    min = Xse.min_epu8(min128, load).Byte0;
                }
                else
                {
                    max = max128.Byte0;
                    min = min128.Byte0;
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;

                v128 max0 = Xse.set1_epi8(unchecked((sbyte)byte.MinValue));

                v128 min0 = Xse.set1_epi8(unchecked((sbyte)byte.MaxValue));

                if (Hint.Likely(length >= 64))
                {
                    v128 max1 = Xse.set1_epi8(unchecked((sbyte)byte.MinValue));
                    v128 max2 = Xse.set1_epi8(unchecked((sbyte)byte.MinValue));
                    v128 max3 = Xse.set1_epi8(unchecked((sbyte)byte.MinValue));

                    v128 min1 = Xse.set1_epi8(unchecked((sbyte)byte.MaxValue));
                    v128 min2 = Xse.set1_epi8(unchecked((sbyte)byte.MaxValue));
                    v128 min3 = Xse.set1_epi8(unchecked((sbyte)byte.MaxValue));

                    do
                    {
                        v128 load0 = Xse.loadu_si128(ptr_v128++);
                        v128 load1 = Xse.loadu_si128(ptr_v128++);
                        v128 load2 = Xse.loadu_si128(ptr_v128++);
                        v128 load3 = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epu8(max0, load0);
                        max1 = Xse.max_epu8(max1, load1);
                        max2 = Xse.max_epu8(max2, load2);
                        max3 = Xse.max_epu8(max3, load3);

                        min0 = Xse.min_epu8(min0, load0);
                        min1 = Xse.min_epu8(min1, load1);
                        min2 = Xse.min_epu8(min2, load2);
                        min3 = Xse.min_epu8(min3, load3);

                        length -= 64;
                    }
                    while (Hint.Likely(length >= 64));

                    max0 = Xse.max_epu8(max0, max1);
                    max2 = Xse.max_epu8(max2, max3);
                    max0 = Xse.max_epu8(max0, max2);

                    min0 = Xse.min_epu8(min0, min1);
                    min2 = Xse.min_epu8(min2, min3);
                    min0 = Xse.min_epu8(min0, min2);
                }

                if (Hint.Likely((int)length >= 16))
                {
                    v128 load = Xse.loadu_si128(ptr_v128++);

                    max0 = Xse.max_epu8(max0, load);
                    min0 = Xse.min_epu8(min0, load);

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        load = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epu8(max0, load);
                        min0 = Xse.min_epu8(min0, load);

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            load = Xse.loadu_si128(ptr_v128++);

                            max0 = Xse.max_epu8(max0, load);
                            min0 = Xse.min_epu8(min0, load);
                            length -= 3 * 16;

                        }
                        else
                        {
                            length -= 2 * 16;
                        }
                    }
                    else
                    {
                        length -= 16;
                    }
                }

                v128 cmp = Xse.setzero_si128();
                max0 = Xse.max_epu8(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_epu8(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v128);

                    max0 = Xse.max_epu8(max0, load);
                    min0 = Xse.min_epu8(min0, load);

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 8;
                }

                max0 = Xse.max_epu8(max0, Xse.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_epu8(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v128);

                    max0 = Xse.max_epu8(max0, load);
                    min0 = Xse.min_epu8(min0, load);

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 4;
                }

                max0 = Xse.max_epu8(max0, Xse.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 0, 1)));
                min0 = Xse.min_epu8(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.insert_epi16(cmp, *(ushort*)ptr_v128, 0);

                    max0 = Xse.max_epu8(max0, load);
                    min0 = Xse.min_epu8(min0, load);

                    ptr_v128 = (v128*)((short*)ptr_v128 + 1);
                    length -= 2;
                }

                max0 = Xse.max_epu8(max0, Xse.bsrli_si128(max0, 1 * sizeof(byte)));
                min0 = Xse.min_epu8(min0, Xse.bsrli_si128(min0, 1 * sizeof(byte)));

                if (Hint.Likely(length != 0))
                {
                    if (Sse4_1.IsSse41Supported)
                    {
                        max = Xse.max_epu8(max0, Xse.insert_epi8(cmp, *(byte*)ptr_v128, 0)).Byte0;
                        min = Xse.min_epu8(min0, Xse.insert_epi8(cmp, *(byte*)ptr_v128, 0)).Byte0;
                    }
                    else
                    {
                        max = Xse.max_epu8(max0,Xse.cvtsi32_si128(*(byte*)ptr_v128)).Byte0;
                        min = Xse.min_epu8(min0,Xse.cvtsi32_si128(*(byte*)ptr_v128)).Byte0;
                    }
                }
                else
                {
                    max = max0.Byte0;
                    min = min0.Byte0;
                }
            }
            else
            {
                max = byte.MinValue;
                min = byte.MinValue;

                for (long i = 0; i < length; i++)
                {
                    uint cmp = (uint)ptr[i];

                    max = (byte)math.max((uint)max, cmp);
                    min = (byte)math.min((uint)min, cmp);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<byte> array, out byte min, out byte max)
        {
            SIMD_MinMax((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<byte> array, int index, out byte min, out byte max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<byte> array, int index, int numEntries, out byte min, out byte max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<byte> array, out byte min, out byte max)
        {
            SIMD_MinMax((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<byte> array, int index, out byte min, out byte max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<byte> array, int index, int numEntries, out byte min, out byte max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<byte> array, out byte min, out byte max)
        {
            SIMD_MinMax((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<byte> array, int index, out byte min, out byte max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<byte> array, int index, int numEntries, out byte min, out byte max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(ushort* ptr, long length, out ushort min, out ushort max)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;

                v256 max0 = Avx.mm256_set1_epi16(unchecked((short)ushort.MinValue));

                v256 min0 = Avx.mm256_set1_epi16(unchecked((short)ushort.MaxValue));

                if (Hint.Likely(length >= 64))
                {
                    v256 max1 = Avx.mm256_set1_epi16(unchecked((short)ushort.MinValue));
                    v256 max2 = Avx.mm256_set1_epi16(unchecked((short)ushort.MinValue));
                    v256 max3 = Avx.mm256_set1_epi16(unchecked((short)ushort.MinValue));

                    v256 min1 = Avx.mm256_set1_epi16(unchecked((short)ushort.MaxValue));
                    v256 min2 = Avx.mm256_set1_epi16(unchecked((short)ushort.MaxValue));
                    v256 min3 = Avx.mm256_set1_epi16(unchecked((short)ushort.MaxValue));

                    do
                    {
                        v256 load0 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load1 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load2 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load3 = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Avx2.mm256_max_epu16(max0, load0);
                        max1 = Avx2.mm256_max_epu16(max1, load1);
                        max2 = Avx2.mm256_max_epu16(max2, load2);
                        max3 = Avx2.mm256_max_epu16(max3, load3);

                        min0 = Avx2.mm256_min_epu16(min0, load0);
                        min1 = Avx2.mm256_min_epu16(min1, load1);
                        min2 = Avx2.mm256_min_epu16(min2, load2);
                        min3 = Avx2.mm256_min_epu16(min3, load3);

                        length -= 64;
                    }
                    while (Hint.Likely(length >= 64));

                    max0 = Avx2.mm256_max_epu16(max0, max1);
                    max2 = Avx2.mm256_max_epu16(max2, max3);
                    max0 = Avx2.mm256_max_epu16(max0, max2);

                    min0 = Avx2.mm256_min_epu16(min0, min1);
                    min2 = Avx2.mm256_min_epu16(min2, min3);
                    min0 = Avx2.mm256_min_epu16(min0, min2);
                }

                if (Hint.Likely((int)length >= 16))
                {
                    v256 load = Avx.mm256_loadu_si256(ptr_v256++);

                    max0 = Avx2.mm256_max_epu16(max0, load);
                    min0 = Avx2.mm256_min_epu16(min0, load);

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        load = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Avx2.mm256_max_epu16(max0, load);
                        min0 = Avx2.mm256_min_epu16(min0, load);

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            load = Avx.mm256_loadu_si256(ptr_v256++);

                            max0 = Avx2.mm256_max_epu16(max0, load);
                            min0 = Avx2.mm256_min_epu16(min0, load);

                            length -= 3 * 16;
                        }
                        else
                        {
                            length -= 2 * 16;
                        }
                    }
                    else
                    {
                        length -= 16;
                    }
                }

                v128 max128 = Xse.max_epu16(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));
                v128 min128 = Xse.min_epu16(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 8))
                {
                    v128 load = Xse.loadu_si128(ptr_v256);

                    max128 = Xse.max_epu16(max128, load);
                    min128 = Xse.min_epu16(min128, load);

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 8;
                }

                v128 cmp = Xse.setzero_si128();
                max128 = Xse.max_epu16(max128, Xse.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));
                min128 = Xse.min_epu16(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v256);

                    max128 = Xse.max_epu16(max128, load);
                    min128 = Xse.min_epu16(min128, load);

                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 4;
                }

                max128 = Xse.max_epu16(max128, Xse.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 3, 2)));
                min128 = Xse.min_epu16(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v256);

                    max128 = Xse.max_epu16(max128, load);
                    min128 = Xse.min_epu16(min128, load);

                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Xse.max_epu16(max128, Xse.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 0, 1)));
                min128 = Xse.min_epu16(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    v128 load = Xse.insert_epi16(cmp, *(ushort*)ptr_v256, 0);

                    max = Xse.max_epu16(max128, load).UShort0;
                    min = Xse.min_epu16(min128, load).UShort0;
                }
                else
                {
                    max = max128.UShort0;
                    min = min128.UShort0;
                }
            }
            else if (Sse4_1.IsSse41Supported)
            {
                v128* ptr_v128 = (v128*)ptr;

                v128 max0 = Xse.set1_epi16(unchecked((short)ushort.MinValue));

                v128 min0 = Xse.set1_epi16(unchecked((short)ushort.MaxValue));

                if (Hint.Likely(length >= 32))
                {
                    v128 max1 = Xse.set1_epi16(unchecked((short)ushort.MinValue));
                    v128 max2 = Xse.set1_epi16(unchecked((short)ushort.MinValue));
                    v128 max3 = Xse.set1_epi16(unchecked((short)ushort.MinValue));

                    v128 min1 = Xse.set1_epi16(unchecked((short)ushort.MaxValue));
                    v128 min2 = Xse.set1_epi16(unchecked((short)ushort.MaxValue));
                    v128 min3 = Xse.set1_epi16(unchecked((short)ushort.MaxValue));

                    do
                    {
                        v128 load0 = Xse.loadu_si128(ptr_v128++);
                        v128 load1 = Xse.loadu_si128(ptr_v128++);
                        v128 load2 = Xse.loadu_si128(ptr_v128++);
                        v128 load3 = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epu16(max0, load0);
                        max1 = Xse.max_epu16(max1, load1);
                        max2 = Xse.max_epu16(max2, load2);
                        max3 = Xse.max_epu16(max3, load3);

                        min0 = Xse.min_epu16(min0, load0);
                        min1 = Xse.min_epu16(min1, load1);
                        min2 = Xse.min_epu16(min2, load2);
                        min3 = Xse.min_epu16(min3, load3);

                        length -= 32;
                    }
                    while (Hint.Likely(length >= 32));

                    max0 = Xse.max_epu16(max0, max1);
                    max2 = Xse.max_epu16(max2, max3);
                    max0 = Xse.max_epu16(max0, max2);

                    min0 = Xse.min_epu16(min0, min1);
                    min2 = Xse.min_epu16(min2, min3);
                    min0 = Xse.min_epu16(min0, min2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    v128 load = Xse.loadu_si128(ptr_v128++);

                    max0 = Xse.max_epu16(max0, load);
                    min0 = Xse.min_epu16(min0, load);

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        load = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epu16(max0, load);
                        min0 = Xse.min_epu16(min0, load);

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            load = Xse.loadu_si128(ptr_v128++);

                            max0 = Xse.max_epu16(max0, load);
                            min0 = Xse.min_epu16(min0, load);

                            length -= 3 * 8;
                        }
                        else
                        {
                            length -= 2 * 8;
                        }
                    }
                    else
                    {
                        length -= 8;
                    }
                }

                v128 cmp = Xse.setzero_si128();
                max0 = Xse.max_epu16(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_epu16(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v128);

                    max0 = Xse.max_epu16(max0, load);
                    min0 = Xse.min_epu16(min0, load);

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 4;
                }

                max0 = Xse.max_epu16(max0, Xse.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_epu16(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v128);

                    max0 = Xse.max_epu16(max0, load);
                    min0 = Xse.min_epu16(min0, load);

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 2;
                }

                max0 = Xse.max_epu16(max0, Xse.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 0, 1)));
                min0 = Xse.min_epu16(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    v128 load = Xse.insert_epi16(cmp, *(ushort*)ptr_v128, 0);

                    max = Xse.max_epu16(max0, load).UShort0;
                    min = Xse.min_epu16(min0, load).UShort0;
                }
                else
                {
                    max = max0.UShort0;
                    min = min0.UShort0;
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;

                v128 XOR_MASK = Xse.set1_epi16(unchecked((short)(1 << 15)));

                v128 max0 = Xse.set1_epi16(unchecked((short)ushort.MinValue));

                v128 min0 = Xse.set1_epi16(unchecked((short)ushort.MaxValue));

                if (Hint.Likely(length >= 32))
                {
                    v128 max1 = Xse.set1_epi16(unchecked((short)ushort.MinValue));
                    v128 max2 = Xse.set1_epi16(unchecked((short)ushort.MinValue));
                    v128 max3 = Xse.set1_epi16(unchecked((short)ushort.MinValue));

                    v128 min1 = Xse.set1_epi16(unchecked((short)ushort.MaxValue));
                    v128 min2 = Xse.set1_epi16(unchecked((short)ushort.MaxValue));
                    v128 min3 = Xse.set1_epi16(unchecked((short)ushort.MaxValue));

                    do
                    {
                        v128 load0 = Xse.loadu_si128(ptr_v128++);
                        v128 load1 = Xse.loadu_si128(ptr_v128++);
                        v128 load2 = Xse.loadu_si128(ptr_v128++);
                        v128 load3 = Xse.loadu_si128(ptr_v128++);
                        load0 = Xse.xor_si128(XOR_MASK, load0);
                        load1 = Xse.xor_si128(XOR_MASK, load1);
                        load2 = Xse.xor_si128(XOR_MASK, load2);
                        load3 = Xse.xor_si128(XOR_MASK, load3);

                        max0 = Xse.xor_si128(XOR_MASK, max0);
                        max1 = Xse.xor_si128(XOR_MASK, max1);
                        max2 = Xse.xor_si128(XOR_MASK, max2);
                        max3 = Xse.xor_si128(XOR_MASK, max3);
                        max0 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(load0, max0));
                        max1 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(load1, max1));
                        max2 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(load2, max2));
                        max3 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(load3, max3));

                        min0 = Xse.xor_si128(XOR_MASK, min0);
                        min1 = Xse.xor_si128(XOR_MASK, min1);
                        min2 = Xse.xor_si128(XOR_MASK, min2);
                        min3 = Xse.xor_si128(XOR_MASK, min3);
                        min0 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(load0, min0));
                        min1 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(load1, min1));
                        min2 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(load2, min2));
                        min3 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(load3, min3));

                        length -= 32;
                    }
                    while (Hint.Likely(length >= 32));

                    max0 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(Xse.xor_si128(XOR_MASK, max0), Xse.xor_si128(XOR_MASK, max1)));
                    max2 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(Xse.xor_si128(XOR_MASK, max2), Xse.xor_si128(XOR_MASK, max3)));
                    max0 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(Xse.xor_si128(XOR_MASK, max0), Xse.xor_si128(XOR_MASK, max2)));

                    min0 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(Xse.xor_si128(XOR_MASK, min0), Xse.xor_si128(XOR_MASK, min1)));
                    min2 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(Xse.xor_si128(XOR_MASK, min2), Xse.xor_si128(XOR_MASK, min3)));
                    min0 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(Xse.xor_si128(XOR_MASK, min0), Xse.xor_si128(XOR_MASK, min2)));
                }

                if (Hint.Likely((int)length >= 8))
                {
                    v128 load = Xse.loadu_si128(ptr_v128++);
                    load = Xse.xor_si128(XOR_MASK, load);

                    max0 = Xse.xor_si128(XOR_MASK, max0);
                    min0 = Xse.xor_si128(XOR_MASK, min0);
                    max0 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(max0, load));
                    min0 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(min0, load));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        load = Xse.loadu_si128(ptr_v128++);
                        load = Xse.xor_si128(XOR_MASK, load);

                        max0 = Xse.xor_si128(XOR_MASK, max0);
                        min0 = Xse.xor_si128(XOR_MASK, min0);
                        max0 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(max0, load));
                        min0 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(min0, load));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            load = Xse.loadu_si128(ptr_v128++);
                            load = Xse.xor_si128(XOR_MASK, load);

                            max0 = Xse.xor_si128(XOR_MASK, max0);
                            min0 = Xse.xor_si128(XOR_MASK, min0);
                            max0 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(max0, load));
                            min0 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(min0, load));

                            length -= 3 * 8;
                        }
                        else
                        {
                            length -= 2 * 8;
                        }
                    }
                    else
                    {
                        length -= 8;
                    }
                }

                max0 = Xse.xor_si128(XOR_MASK, max0);
                min0 = Xse.xor_si128(XOR_MASK, min0);
                max0 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2))));
                min0 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2))));

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v128);
                    load = Xse.xor_si128(XOR_MASK, load);

                    max0 = Xse.xor_si128(XOR_MASK, max0);
                    min0 = Xse.xor_si128(XOR_MASK, min0);
                    max0 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(max0, load));
                    min0 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(min0, load));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 4;
                }

                max0 = Xse.xor_si128(XOR_MASK, max0);
                min0 = Xse.xor_si128(XOR_MASK, min0);
                max0 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(max0, Xse.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 3, 2))));
                min0 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 3, 2))));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v128);
                    load = Xse.xor_si128(XOR_MASK, load);

                    max0 = Xse.xor_si128(XOR_MASK, max0);
                    min0 = Xse.xor_si128(XOR_MASK, min0);
                    max0 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(max0, load));
                    min0 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(min0, load));

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 2;
                }

                max0 = Xse.xor_si128(XOR_MASK, max0);
                min0 = Xse.xor_si128(XOR_MASK, min0);
                max0 = Xse.xor_si128(XOR_MASK, Xse.max_epi16(max0, Xse.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 0, 1))));
                min0 = Xse.xor_si128(XOR_MASK, Xse.min_epi16(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 0, 1))));

                if (Hint.Likely(length != 0))
                {
                    uint load = *(ushort*)ptr_v128;

                    max = (ushort)math.max(max0.UShort0, load);
                    min = (ushort)math.min(min0.UShort0, load);
                }
                else
                {
                    max = max0.UShort0;
                    min = min0.UShort0;
                }
            }
            else
            {
                max = ushort.MinValue;
                min = ushort.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    uint deref = (uint)ptr[i];

                    max = (ushort)math.max((uint)max, deref);
                    min = (ushort)math.min((uint)min, deref);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<ushort> array, out ushort min, out ushort max)
        {
            SIMD_MinMax((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<ushort> array, int index, out ushort min, out ushort max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<ushort> array, int index, int numEntries, out ushort min, out ushort max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<ushort> array, out ushort min, out ushort max)
        {
            SIMD_MinMax((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<ushort> array, int index, out ushort min, out ushort max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<ushort> array, int index, int numEntries, out ushort min, out ushort max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<ushort> array, out ushort min, out ushort max)
        {
            SIMD_MinMax((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<ushort> array, int index, out ushort min, out ushort max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<ushort> array, int index, int numEntries, out ushort min, out ushort max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(uint* ptr, long length, out uint min, out uint max)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;

                v256 max0 = Avx.mm256_set1_epi32(unchecked((int)uint.MinValue));

                v256 min0 = Avx.mm256_set1_epi32(unchecked((int)uint.MaxValue));

                if (Hint.Likely(length >= 32))
                {
                    v256 max1 = Avx.mm256_set1_epi32(unchecked((int)uint.MinValue));
                    v256 max2 = Avx.mm256_set1_epi32(unchecked((int)uint.MinValue));
                    v256 max3 = Avx.mm256_set1_epi32(unchecked((int)uint.MinValue));

                    v256 min1 = Avx.mm256_set1_epi32(unchecked((int)uint.MaxValue));
                    v256 min2 = Avx.mm256_set1_epi32(unchecked((int)uint.MaxValue));
                    v256 min3 = Avx.mm256_set1_epi32(unchecked((int)uint.MaxValue));

                    do
                    {
                        v256 load0 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load1 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load2 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load3 = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Avx2.mm256_max_epu32(max0, load0);
                        max1 = Avx2.mm256_max_epu32(max1, load1);
                        max2 = Avx2.mm256_max_epu32(max2, load2);
                        max3 = Avx2.mm256_max_epu32(max3, load3);

                        min0 = Avx2.mm256_min_epu32(min0, load0);
                        min1 = Avx2.mm256_min_epu32(min1, load1);
                        min2 = Avx2.mm256_min_epu32(min2, load2);
                        min3 = Avx2.mm256_min_epu32(min3, load3);

                        length -= 32;
                    }
                    while (Hint.Likely(length >= 32));

                    max0 = Avx2.mm256_max_epu32(max0, max1);
                    max2 = Avx2.mm256_max_epu32(max2, max3);
                    max0 = Avx2.mm256_max_epu32(max0, max2);

                    min0 = Avx2.mm256_min_epu32(min0, min1);
                    min2 = Avx2.mm256_min_epu32(min2, min3);
                    min0 = Avx2.mm256_min_epu32(min0, min2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    v256 load = Avx.mm256_loadu_si256(ptr_v256++);

                    max0 = Avx2.mm256_max_epu32(max0, load);
                    min0 = Avx2.mm256_min_epu32(min0, load);

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        load = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Avx2.mm256_max_epu32(max0, load);
                        min0 = Avx2.mm256_min_epu32(min0, load);

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            load = Avx.mm256_loadu_si256(ptr_v256++);

                            max0 = Avx2.mm256_max_epu32(max0, load);
                            min0 = Avx2.mm256_min_epu32(min0, load);

                            length -= 3 * 8;
                        }
                        else
                        {
                            length -= 2 * 8;
                        }
                    }
                    else
                    {
                        length -= 8;
                    }
                }

                v128 max128 = Xse.max_epu32(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));
                v128 min128 = Xse.min_epu32(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.loadu_si128(ptr_v256);

                    max128 = Xse.max_epu32(max128, load);
                    min128 = Xse.min_epu32(min128, load);

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 4;
                }

                max128 = Xse.max_epu32(max128, Xse.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));
                min128 = Xse.min_epu32(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v256);

                    max128 = Xse.max_epu32(max128, load);
                    min128 = Xse.min_epu32(min128, load);

                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Xse.max_epu32(max128, Xse.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 0, 1)));
                min128 = Xse.min_epu32(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v256);

                    max = Xse.max_epu32(max128, load).UInt0;
                    min = Xse.min_epu32(min128, load).UInt0;
                }
                else
                {
                    max = max128.UInt0;
                    min = min128.UInt0;
                }
            }
            else if (Sse4_1.IsSse41Supported)
            {
                v128* ptr_v128 = (v128*)ptr;

                v128 max0 = Xse.set1_epi32(unchecked((int)uint.MinValue));

                v128 min0 = Xse.set1_epi32(unchecked((int)uint.MaxValue));

                if (Hint.Likely(length >= 16))
                {
                    v128 max1 = Xse.set1_epi32(unchecked((int)uint.MinValue));
                    v128 max2 = Xse.set1_epi32(unchecked((int)uint.MinValue));
                    v128 max3 = Xse.set1_epi32(unchecked((int)uint.MinValue));

                    v128 min1 = Xse.set1_epi32(unchecked((int)uint.MaxValue));
                    v128 min2 = Xse.set1_epi32(unchecked((int)uint.MaxValue));
                    v128 min3 = Xse.set1_epi32(unchecked((int)uint.MaxValue));

                    do
                    {
                        v128 load0 = Xse.loadu_si128(ptr_v128++);
                        v128 load1 = Xse.loadu_si128(ptr_v128++);
                        v128 load2 = Xse.loadu_si128(ptr_v128++);
                        v128 load3 = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epu32(max0, load0);
                        max1 = Xse.max_epu32(max1, load1);
                        max2 = Xse.max_epu32(max2, load2);
                        max3 = Xse.max_epu32(max3, load3);

                        min0 = Xse.min_epu32(min0, load0);
                        min1 = Xse.min_epu32(min1, load1);
                        min2 = Xse.min_epu32(min2, load2);
                        min3 = Xse.min_epu32(min3, load3);

                        length -= 16;
                    }
                    while (Hint.Likely(length >= 16));

                    max0 = Xse.max_epu32(max0, max1);
                    max2 = Xse.max_epu32(max2, max3);
                    max0 = Xse.max_epu32(max0, max2);

                    min0 = Xse.min_epu32(min0, min1);
                    min2 = Xse.min_epu32(min2, min3);
                    min0 = Xse.min_epu32(min0, min2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.loadu_si128(ptr_v128++);

                    max0 = Xse.max_epu32(max0, load);
                    min0 = Xse.min_epu32(min0, load);

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        load = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epu32(max0, load);
                        min0 = Xse.min_epu32(min0, load);

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            load = Xse.loadu_si128(ptr_v128++);

                            max0 = Xse.max_epu32(max0, load);
                            min0 = Xse.min_epu32(min0, load);

                            length -= 3 * 4;
                        }
                        else
                        {
                            length -= 2 * 4;
                        }
                    }
                    else
                    {
                        length -= 4;
                    }
                }

                max0 = Xse.max_epu32(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_epu32(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v128);

                    max0 = Xse.max_epu32(max0, load);
                    min0 = Xse.min_epu32(min0, load);

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 2;
                }

                max0 = Xse.max_epu32(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 0, 1)));
                min0 = Xse.min_epu32(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v128);

                    max = Xse.max_epu32(max0, load).UInt0;
                    min = Xse.min_epu32(min0, load).UInt0;
                }
                else
                {
                    max = max0.UInt0;
                    min = min0.UInt0;
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;

                v128 XOR_MASK = Xse.set1_epi32(unchecked((int)(1 << 31)));

                v128 max0 = Xse.set1_epi32(unchecked((int)uint.MinValue));

                v128 min0 = Xse.set1_epi32(unchecked((int)uint.MaxValue));

                if (Hint.Likely(length >= 16))
                {
                    v128 max1 = Xse.set1_epi32(unchecked((int)uint.MinValue));
                    v128 max2 = Xse.set1_epi32(unchecked((int)uint.MinValue));
                    v128 max3 = Xse.set1_epi32(unchecked((int)uint.MinValue));

                    v128 min1 = Xse.set1_epi32(unchecked((int)uint.MaxValue));
                    v128 min2 = Xse.set1_epi32(unchecked((int)uint.MaxValue));
                    v128 min3 = Xse.set1_epi32(unchecked((int)uint.MaxValue));
                    do
                    {
                        v128 load0 = Xse.loadu_si128(ptr_v128++);
                        v128 load1 = Xse.loadu_si128(ptr_v128++);
                        v128 load2 = Xse.loadu_si128(ptr_v128++);
                        v128 load3 = Xse.loadu_si128(ptr_v128++);
                        v128 xor_load0 = Xse.xor_si128(XOR_MASK, load0);
                        v128 xor_load1 = Xse.xor_si128(XOR_MASK, load1);
                        v128 xor_load2 = Xse.xor_si128(XOR_MASK, load2);
                        v128 xor_load3 = Xse.xor_si128(XOR_MASK, load3);

                        max0 = Xse.blendv_si128(max0, load0, Xse.cmpgt_epi32(xor_load0, Xse.xor_si128(XOR_MASK, max0)));
                        max1 = Xse.blendv_si128(max1, load1, Xse.cmpgt_epi32(xor_load1, Xse.xor_si128(XOR_MASK, max1)));
                        max2 = Xse.blendv_si128(max2, load2, Xse.cmpgt_epi32(xor_load2, Xse.xor_si128(XOR_MASK, max2)));
                        max3 = Xse.blendv_si128(max3, load3, Xse.cmpgt_epi32(xor_load3, Xse.xor_si128(XOR_MASK, max3)));

                        min0 = Xse.blendv_si128(load0, min0, Xse.cmpgt_epi32(xor_load0, Xse.xor_si128(XOR_MASK, min0)));
                        min1 = Xse.blendv_si128(load1, min1, Xse.cmpgt_epi32(xor_load1, Xse.xor_si128(XOR_MASK, min1)));
                        min2 = Xse.blendv_si128(load2, min2, Xse.cmpgt_epi32(xor_load2, Xse.xor_si128(XOR_MASK, min2)));
                        min3 = Xse.blendv_si128(load3, min3, Xse.cmpgt_epi32(xor_load3, Xse.xor_si128(XOR_MASK, min3)));

                        length -= 16;
                    }
                    while (Hint.Likely(length >= 16));

                    max0 = Xse.blendv_si128(max0, max1, Xse.cmpgt_epi32(Xse.xor_si128(XOR_MASK, max1), Xse.xor_si128(XOR_MASK, max0)));
                    max2 = Xse.blendv_si128(max2, max3, Xse.cmpgt_epi32(Xse.xor_si128(XOR_MASK, max3), Xse.xor_si128(XOR_MASK, max2)));
                    max0 = Xse.blendv_si128(max0, max2, Xse.cmpgt_epi32(Xse.xor_si128(XOR_MASK, max2), Xse.xor_si128(XOR_MASK, max0)));

                    min0 = Xse.blendv_si128(min1, min0, Xse.cmpgt_epi32(Xse.xor_si128(XOR_MASK, min1), Xse.xor_si128(XOR_MASK, min0)));
                    min2 = Xse.blendv_si128(min3, min2, Xse.cmpgt_epi32(Xse.xor_si128(XOR_MASK, min3), Xse.xor_si128(XOR_MASK, min2)));
                    min0 = Xse.blendv_si128(min2, min0, Xse.cmpgt_epi32(Xse.xor_si128(XOR_MASK, min2), Xse.xor_si128(XOR_MASK, min0)));
                }

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.loadu_si128(ptr_v128++);
                    v128 xor_load = Xse.xor_si128(XOR_MASK, load);

                    max0 = Xse.blendv_si128(max0, load, Xse.cmpgt_epi32(xor_load, Xse.xor_si128(XOR_MASK, max0)));
                    min0 = Xse.blendv_si128(load, min0, Xse.cmpgt_epi32(xor_load, Xse.xor_si128(XOR_MASK, min0)));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        load = Xse.loadu_si128(ptr_v128++);
                        xor_load = Xse.xor_si128(XOR_MASK, load);

                        max0 = Xse.blendv_si128(max0, load, Xse.cmpgt_epi32(xor_load, Xse.xor_si128(XOR_MASK, max0)));
                        min0 = Xse.blendv_si128(load, min0, Xse.cmpgt_epi32(xor_load, Xse.xor_si128(XOR_MASK, min0)));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            load = Xse.loadu_si128(ptr_v128++);
                            xor_load = Xse.xor_si128(XOR_MASK, load);

                            max0 = Xse.blendv_si128(max0, load, Xse.cmpgt_epi32(xor_load, Xse.xor_si128(XOR_MASK, max0)));
                            min0 = Xse.blendv_si128(load, min0, Xse.cmpgt_epi32(xor_load, Xse.xor_si128(XOR_MASK, min0)));

                            length -= 3 * 4;
                        }
                        else
                        {
                            length -= 2 * 4;
                        }
                    }
                    else
                    {
                        length -= 4;
                    }
                }

                v128 maxShuf = Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2));
                max0 = Xse.blendv_si128(max0, maxShuf, Xse.cmpgt_epi32(Xse.xor_si128(XOR_MASK, maxShuf), Xse.xor_si128(XOR_MASK, max0)));
                v128 minShuf = Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2));
                min0 = Xse.blendv_si128(minShuf, min0, Xse.cmpgt_epi32(Xse.xor_si128(XOR_MASK, minShuf), Xse.xor_si128(XOR_MASK, min0)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v128);
                    v128 xor_load = Xse.xor_si128(XOR_MASK, load);

                     max0 = Xse.blendv_si128(max0, load, Xse.cmpgt_epi32(xor_load, Xse.xor_si128(XOR_MASK, max0)));
                     min0 = Xse.blendv_si128(load, min0, Xse.cmpgt_epi32(xor_load, Xse.xor_si128(XOR_MASK, min0)));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 2;
                }

                maxShuf = Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 0, 1));
                max0 = Xse.blendv_si128(max0, maxShuf, Xse.cmpgt_epi32(Xse.xor_si128(XOR_MASK, maxShuf), Xse.xor_si128(XOR_MASK, max0)));
                minShuf = Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 0, 1));
                min0 = Xse.blendv_si128(minShuf, min0, Xse.cmpgt_epi32(Xse.xor_si128(XOR_MASK, minShuf), Xse.xor_si128(XOR_MASK, min0)));

                if (Hint.Likely(length != 0))
                {
                    uint load = *(uint*)ptr_v128;

                    max = math.max(max0.UInt0, load);
                    min = math.min(min0.UInt0, load);
                }
                else
                {
                    max = max0.UInt0;
                    min = min0.UInt0;
                }
            }
            else
            {
                max = uint.MinValue;
                min = uint.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    uint deref = ptr[i];

                    max = math.max(max, deref);
                    min = math.min(min, deref);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<uint> array, out uint min, out uint max)
        {
            SIMD_MinMax((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<uint> array, int index, out uint min, out uint max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<uint> array, int index, int numEntries, out uint min, out uint max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<uint> array, out uint min, out uint max)
        {
            SIMD_MinMax((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<uint> array, int index, out uint min, out uint max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<uint> array, int index, int numEntries, out uint min, out uint max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<uint> array, out uint min, out uint max)
        {
            SIMD_MinMax((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<uint> array, int index, out uint min, out uint max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<uint> array, int index, int numEntries, out uint min, out uint max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(ulong* ptr, long length, out ulong min, out ulong max)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;

                v256 max0 = Xse.mm256_set1_epi64x(ulong.MinValue);

                v256 min0 = Xse.mm256_set1_epi64x(ulong.MaxValue);

                if (Hint.Likely(length >= 16))
                {
                    v256 max1 = Xse.mm256_set1_epi64x(ulong.MinValue);
                    v256 max2 = Xse.mm256_set1_epi64x(ulong.MinValue);
                    v256 max3 = Xse.mm256_set1_epi64x(ulong.MinValue);

                    v256 min1 = Xse.mm256_set1_epi64x(ulong.MaxValue);
                    v256 min2 = Xse.mm256_set1_epi64x(ulong.MaxValue);
                    v256 min3 = Xse.mm256_set1_epi64x(ulong.MaxValue);

                    do
                    {
                        v256 load0 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load1 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load2 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load3 = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Xse.mm256_max_epu64(max0, load0);
                        max1 = Xse.mm256_max_epu64(max1, load1);
                        max2 = Xse.mm256_max_epu64(max2, load2);
                        max3 = Xse.mm256_max_epu64(max3, load3);

                        min0 = Xse.mm256_min_epu64(min0, load0);
                        min1 = Xse.mm256_min_epu64(min1, load1);
                        min2 = Xse.mm256_min_epu64(min2, load2);
                        min3 = Xse.mm256_min_epu64(min3, load3);

                        length -= 16;
                    }
                    while (Hint.Likely(length >= 16));

                    max0 = Xse.mm256_max_epu64(max0, max1);
                    max2 = Xse.mm256_max_epu64(max2, max3);
                    max0 = Xse.mm256_max_epu64(max0, max2);

                    min0 = Xse.mm256_min_epu64(min0, min1);
                    min2 = Xse.mm256_min_epu64(min2, min3);
                    min0 = Xse.mm256_min_epu64(min0, min2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    v256 load = Avx.mm256_loadu_si256(ptr_v256++);

                    max0 = Xse.mm256_max_epu64(max0, load);
                    min0 = Xse.mm256_min_epu64(min0, load);

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        load = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Xse.mm256_max_epu64(max0, load);
                        min0 = Xse.mm256_min_epu64(min0, load);

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            load = Avx.mm256_loadu_si256(ptr_v256++);

                            max0 = Xse.mm256_max_epu64(max0, load);
                            min0 = Xse.mm256_min_epu64(min0, load);

                            length -= 3 * 4;
                        }
                        else
                        {
                            length -= 2 * 4;
                        }
                    }
                    else
                    {
                        length -= 4;
                    }
                }

                v128 max128 = Xse.max_epu64(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));
                v128 min128 = Xse.min_epu64(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.loadu_si128(ptr_v256);

                    max128 = Xse.max_epu64(max128, load);
                    min128 = Xse.min_epu64(min128, load);

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Xse.max_epu64(max128, Xse.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));
                min128 = Xse.min_epu64(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    ulong load = *(ulong*)ptr_v256;

                    max = math.max(max128.ULong0, load);
                    min = math.min(min128.ULong0, load);
                }
                else
                {
                    max = max128.ULong0;
                    min = min128.ULong0;
                }
            }
            else if (BurstArchitecture.IsCMP64Supported)
            {
                v128* ptr_v128 = (v128*)ptr;

                v128 max0 = Xse.set1_epi64x(ulong.MinValue);

                v128 min0 = Xse.set1_epi64x(ulong.MaxValue);

                if (Hint.Likely(length >= 8))
                {
                    v128 max1 = Xse.set1_epi64x(ulong.MinValue);
                    v128 max2 = Xse.set1_epi64x(ulong.MinValue);
                    v128 max3 = Xse.set1_epi64x(ulong.MinValue);

                    v128 min1 = Xse.set1_epi64x(ulong.MaxValue);
                    v128 min2 = Xse.set1_epi64x(ulong.MaxValue);
                    v128 min3 = Xse.set1_epi64x(ulong.MaxValue);

                    do
                    {
                        v128 load0 = Xse.loadu_si128(ptr_v128++);
                        v128 load1 = Xse.loadu_si128(ptr_v128++);
                        v128 load2 = Xse.loadu_si128(ptr_v128++);
                        v128 load3 = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epu64(max0, load0);
                        max1 = Xse.max_epu64(max1, load1);
                        max2 = Xse.max_epu64(max2, load2);
                        max3 = Xse.max_epu64(max3, load3);

                        min0 = Xse.min_epu64(min0, load0);
                        min1 = Xse.min_epu64(min1, load1);
                        min2 = Xse.min_epu64(min2, load2);
                        min3 = Xse.min_epu64(min3, load3);

                        length -= 8;
                    }
                    while (Hint.Likely(length >= 8));

                    max0 = Xse.max_epu64(max0, max1);
                    max2 = Xse.max_epu64(max2, max3);
                    max0 = Xse.max_epu64(max0, max2);

                    min0 = Xse.min_epu64(min0, min1);
                    min2 = Xse.min_epu64(min2, min3);
                    min0 = Xse.min_epu64(min0, min2);
                }

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.loadu_si128(ptr_v128++);

                    max0 = Xse.max_epu64(max0, load);
                    min0 = Xse.min_epu64(min0, load);

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        load = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epu64(max0, load);
                        min0 = Xse.min_epu64(min0, load);

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            load = Xse.loadu_si128(ptr_v128++);

                            max0 = Xse.max_epu64(max0, load);
                            min0 = Xse.min_epu64(min0, load);

                            length -= 3 * 2;
                        }
                        else
                        {
                            length -= 2 * 2;
                        }
                    }
                    else
                    {
                        length -= 2;
                    }
                }

                max0 = Xse.max_epu64(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_epu64(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    ulong load = *(ulong*)ptr_v128;

                    max = math.max(max0.ULong0, load);
                    min = math.min(min0.ULong0, load);
                }
                else
                {
                    max = max0.ULong0;
                    min = min0.ULong0;
                }
            }
            else
            {
                max = ulong.MinValue;
                min = ulong.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    ulong deref = ptr[i];

                    max = math.max(max, deref);
                    min = math.min(min, deref);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<ulong> array, out ulong min, out ulong max)
        {
            SIMD_MinMax((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<ulong> array, int index, out ulong min, out ulong max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<ulong> array, int index, int numEntries, out ulong min, out ulong max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<ulong> array, out ulong min, out ulong max)
        {
            SIMD_MinMax((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<ulong> array, int index, out ulong min, out ulong max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<ulong> array, int index, int numEntries, out ulong min, out ulong max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<ulong> array, out ulong min, out ulong max)
        {
            SIMD_MinMax((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<ulong> array, int index, out ulong min, out ulong max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<ulong> array, int index, int numEntries, out ulong min, out ulong max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(sbyte* ptr, long length, out sbyte min, out sbyte max)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;

                v256 max0 = Avx.mm256_set1_epi8(unchecked((byte)sbyte.MinValue));

                v256 min0 = Avx.mm256_set1_epi8(unchecked((byte)sbyte.MaxValue));

                if (Hint.Likely(length >= 128))
                {
                    v256 max1 = Avx.mm256_set1_epi8(unchecked((byte)sbyte.MinValue));
                    v256 max2 = Avx.mm256_set1_epi8(unchecked((byte)sbyte.MinValue));
                    v256 max3 = Avx.mm256_set1_epi8(unchecked((byte)sbyte.MinValue));

                    v256 min1 = Avx.mm256_set1_epi8(unchecked((byte)sbyte.MaxValue));
                    v256 min2 = Avx.mm256_set1_epi8(unchecked((byte)sbyte.MaxValue));
                    v256 min3 = Avx.mm256_set1_epi8(unchecked((byte)sbyte.MaxValue));

                    do
                    {
                        v256 load0 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load1 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load2 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load3 = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Avx2.mm256_max_epi8(max0, load0);
                        max1 = Avx2.mm256_max_epi8(max1, load1);
                        max2 = Avx2.mm256_max_epi8(max2, load2);
                        max3 = Avx2.mm256_max_epi8(max3, load3);

                        min0 = Avx2.mm256_min_epi8(min0, load0);
                        min1 = Avx2.mm256_min_epi8(min1, load1);
                        min2 = Avx2.mm256_min_epi8(min2, load2);
                        min3 = Avx2.mm256_min_epi8(min3, load3);

                        length -= 128;
                    }
                    while (Hint.Likely(length >= 128));

                    max0 = Avx2.mm256_max_epi8(max0, max1);
                    max2 = Avx2.mm256_max_epi8(max2, max3);
                    max0 = Avx2.mm256_max_epi8(max0, max2);

                    min0 = Avx2.mm256_min_epi8(min0, min1);
                    min2 = Avx2.mm256_min_epi8(min2, min3);
                    min0 = Avx2.mm256_min_epi8(min0, min2);
                }

                if (Hint.Likely((int)length >= 32))
                {
                    v256 load = Avx.mm256_loadu_si256(ptr_v256++);

                    max0 = Avx2.mm256_max_epi8(max0, load);
                    min0 = Avx2.mm256_min_epi8(min0, load);

                    if (Hint.Likely((int)length >= 2 * 32))
                    {
                        load = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Avx2.mm256_max_epi8(max0, load);
                        min0 = Avx2.mm256_min_epi8(min0, load);

                        if (Hint.Likely((int)length >= 3 * 32))
                        {
                            load = Avx.mm256_loadu_si256(ptr_v256++);

                            max0 = Avx2.mm256_max_epi8(max0, load);
                            min0 = Avx2.mm256_min_epi8(min0, load);

                            length -= 3 * 32;
                        }
                        else
                        {
                            length -= 2 * 32;
                        }
                    }
                    else
                    {
                        length -= 32;
                    }
                }

                v128 max128 = Xse.max_epi8(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));
                v128 min128 = Xse.min_epi8(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 16))
                {
                    v128 load = Xse.loadu_si128(ptr_v256);

                    max128 = Xse.max_epi8(max128, load);
                    min128 = Xse.min_epi8(min128, load);

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 16;
                }

                v128 cmp = Xse.setzero_si128();
                max128 = Xse.max_epi8(max128, Xse.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));
                min128 = Xse.min_epi8(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v256);

                    max128 = Xse.max_epi8(max128, load);
                    min128 = Xse.min_epi8(min128, load);

                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 8;
                }

                max128 = Xse.max_epi8(max128, Xse.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 3, 2)));
                min128 = Xse.min_epi8(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v256);

                    max128 = Xse.max_epi8(max128, load);
                    min128 = Xse.min_epi8(min128, load);

                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 4;
                }

                max128 = Xse.max_epi8(max128, Xse.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 0, 1)));
                min128 = Xse.min_epi8(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.insert_epi16(cmp, *(ushort*)ptr_v256, 0);

                    max128 = Xse.max_epi8(max128, load);
                    min128 = Xse.min_epi8(min128, load);

                    ptr_v256 = (v256*)((short*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Xse.max_epi8(max128, Xse.bsrli_si128(max128, 1 * sizeof(sbyte)));
                min128 = Xse.min_epi8(min128, Xse.bsrli_si128(min128, 1 * sizeof(sbyte)));

                if (Hint.Likely(length != 0))
                {
                    max = Xse.max_epi8(max128, Xse.insert_epi8(cmp, *(byte*)ptr_v256, 0)).SByte0;
                    min = Xse.min_epi8(min128, Xse.insert_epi8(cmp, *(byte*)ptr_v256, 0)).SByte0;
                }
                else
                {
                    max = max128.SByte0;
                    min = min128.SByte0;
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;

                v128 max0 = Xse.set1_epi8(sbyte.MinValue);

                v128 min0 = Xse.set1_epi8(sbyte.MaxValue);

                if (Hint.Likely(length >= 64))
                {
                    v128 max1 = Xse.set1_epi8(sbyte.MinValue);
                    v128 max2 = Xse.set1_epi8(sbyte.MinValue);
                    v128 max3 = Xse.set1_epi8(sbyte.MinValue);

                    v128 min1 = Xse.set1_epi8(sbyte.MaxValue);
                    v128 min2 = Xse.set1_epi8(sbyte.MaxValue);
                    v128 min3 = Xse.set1_epi8(sbyte.MaxValue);

                    do
                    {
                        v128 load0 = Xse.loadu_si128(ptr_v128++);
                        v128 load1 = Xse.loadu_si128(ptr_v128++);
                        v128 load2 = Xse.loadu_si128(ptr_v128++);
                        v128 load3 = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epi8(max0, load0);
                        max1 = Xse.max_epi8(max1, load1);
                        max2 = Xse.max_epi8(max2, load2);
                        max3 = Xse.max_epi8(max3, load3);

                        min0 = Xse.min_epi8(min0, load0);
                        min1 = Xse.min_epi8(min1, load1);
                        min2 = Xse.min_epi8(min2, load2);
                        min3 = Xse.min_epi8(min3, load3);

                        length -= 64;
                    }
                    while (Hint.Likely(length >= 64));

                    max0 = Xse.max_epi8(max0, max1);
                    max2 = Xse.max_epi8(max2, max3);
                    max0 = Xse.max_epi8(max0, max2);

                    min0 = Xse.min_epi8(min0, min1);
                    min2 = Xse.min_epi8(min2, min3);
                    min0 = Xse.min_epi8(min0, min2);
                }

                if (Hint.Likely((int)length >= 16))
                {
                    v128 load = Xse.loadu_si128(ptr_v128++);

                    max0 = Xse.max_epi8(max0, load);
                    min0 = Xse.min_epi8(min0, load);

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        load = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epi8(max0, load);
                        min0 = Xse.min_epi8(min0, load);

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            load = Xse.loadu_si128(ptr_v128++);

                            max0 = Xse.max_epi8(max0, load);
                            min0 = Xse.min_epi8(min0, load);

                            length -= 3 * 16;
                        }
                        else
                        {
                            length -= 2 * 16;
                        }
                    }
                    else
                    {
                        length -= 16;
                    }
                }

                v128 cmp = Xse.setzero_si128();
                max0 = Xse.max_epi8(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_epi8(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v128);

                    max0 = Xse.max_epi8(max0, load);
                    min0 = Xse.min_epi8(min0, load);

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 8;
                }

                max0 = Xse.max_epi8(max0, Xse.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_epi8(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v128);

                    max0 = Xse.max_epi8(max0, load);
                    min0 = Xse.min_epi8(min0, load);

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 4;
                }

                max0 = Xse.max_epi8(max0, Xse.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 0, 1)));
                min0 = Xse.min_epi8(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.insert_epi16(cmp, *(ushort*)ptr_v128, 0);

                    max0 = Xse.max_epi8(max0, load);
                    min0 = Xse.min_epi8(min0, load);

                    ptr_v128 = (v128*)((short*)ptr_v128 + 1);
                    length -= 2;
                }

                max0 = Xse.max_epi8(max0, Xse.bsrli_si128(max0, 1 * sizeof(sbyte)));
                min0 = Xse.min_epi8(min0, Xse.bsrli_si128(min0, 1 * sizeof(sbyte)));

                if (Hint.Likely(length != 0))
                {
                    if (Sse4_1.IsSse41Supported)
                    {
                        v128 load = Xse.insert_epi8(cmp, *(byte*)ptr_v128, 0);

                        max = Xse.max_epi8(max0, load).SByte0;
                        min = Xse.min_epi8(min0, load).SByte0;
                    }
                    else
                    {
                        max = (sbyte)math.max((int)max0.SByte0, (int)(*(sbyte*)ptr_v128));
                        min = (sbyte)math.min((int)min0.SByte0, (int)(*(sbyte*)ptr_v128));
                    }
                }
                else
                {
                    max = max0.SByte0;
                    min = min0.SByte0;
                }
            }
            else
            {
                max = sbyte.MinValue;
                min = sbyte.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    sbyte deref = ptr[i];

                    max = (sbyte)math.max(max, deref);
                    min = (sbyte)math.min(min, deref);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<sbyte> array, out sbyte min, out sbyte max)
        {
            SIMD_MinMax((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<sbyte> array, int index, out sbyte min, out sbyte max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<sbyte> array, int index, int numEntries, out sbyte min, out sbyte max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<sbyte> array, out sbyte min, out sbyte max)
        {
            SIMD_MinMax((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<sbyte> array, int index, out sbyte min, out sbyte max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<sbyte> array, int index, int numEntries, out sbyte min, out sbyte max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<sbyte> array, out sbyte min, out sbyte max)
        {
            SIMD_MinMax((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<sbyte> array, int index, out sbyte min, out sbyte max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<sbyte> array, int index, int numEntries, out sbyte min, out sbyte max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(short* ptr, long length, out short min, out short max)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;

                v256 max0 = Avx.mm256_set1_epi16(short.MinValue);

                v256 min0 = Avx.mm256_set1_epi16(short.MaxValue);

                if (Hint.Likely(length >= 64))
                {
                    v256 max1 = Avx.mm256_set1_epi16(short.MinValue);
                    v256 max2 = Avx.mm256_set1_epi16(short.MinValue);
                    v256 max3 = Avx.mm256_set1_epi16(short.MinValue);

                    v256 min1 = Avx.mm256_set1_epi16(short.MaxValue);
                    v256 min2 = Avx.mm256_set1_epi16(short.MaxValue);
                    v256 min3 = Avx.mm256_set1_epi16(short.MaxValue);

                    do
                    {
                        v256 load0 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load1 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load2 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load3 = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Avx2.mm256_max_epi16(max0, load0);
                        max1 = Avx2.mm256_max_epi16(max1, load1);
                        max2 = Avx2.mm256_max_epi16(max2, load2);
                        max3 = Avx2.mm256_max_epi16(max3, load3);

                        min0 = Avx2.mm256_min_epi16(min0, load0);
                        min1 = Avx2.mm256_min_epi16(min1, load1);
                        min2 = Avx2.mm256_min_epi16(min2, load2);
                        min3 = Avx2.mm256_min_epi16(min3, load3);

                        length -= 64;
                    }
                    while (Hint.Likely(length >= 64));

                    max0 = Avx2.mm256_max_epi16(max0, max1);
                    max2 = Avx2.mm256_max_epi16(max2, max3);
                    max0 = Avx2.mm256_max_epi16(max0, max2);

                    min0 = Avx2.mm256_min_epi16(min0, min1);
                    min2 = Avx2.mm256_min_epi16(min2, min3);
                    min0 = Avx2.mm256_min_epi16(min0, min2);
                }

                if (Hint.Likely((int)length >= 16))
                {
                    v256 load = Avx.mm256_loadu_si256(ptr_v256++);

                    max0 = Avx2.mm256_max_epi16(max0, load);
                    min0 = Avx2.mm256_min_epi16(min0, load);

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        load = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Avx2.mm256_max_epi16(max0, load);
                        min0 = Avx2.mm256_min_epi16(min0, load);

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            load = Avx.mm256_loadu_si256(ptr_v256++);

                            max0 = Avx2.mm256_max_epi16(max0, load);
                            min0 = Avx2.mm256_min_epi16(min0, load);

                            length -= 3 * 16;
                        }
                        else
                        {
                            length -= 2 * 16;
                        }
                    }
                    else
                    {
                        length -= 16;
                    }
                }

                v128 max128 = Xse.max_epi16(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));
                v128 min128 = Xse.min_epi16(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 8))
                {
                    v128 load = Xse.loadu_si128(ptr_v256);

                    max128 = Xse.max_epi16(max128, load);
                    min128 = Xse.min_epi16(min128, load);

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 8;
                }

                v128 cmp = Xse.setzero_si128();
                max128 = Xse.max_epi16(max128, Xse.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));
                min128 = Xse.min_epi16(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v256);

                    max128 = Xse.max_epi16(max128, load);
                    min128 = Xse.min_epi16(min128, load);

                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 4;
                }

                max128 = Xse.max_epi16(max128, Xse.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 3, 2)));
                min128 = Xse.min_epi16(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v256);

                    max128 = Xse.max_epi16(max128, load);
                    min128 = Xse.min_epi16(min128, load);

                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Xse.max_epi16(max128, Xse.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 0, 1)));
                min128 = Xse.min_epi16(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    v128 load = Xse.insert_epi16(cmp, *(ushort*)ptr_v256, 0);

                    max = Xse.max_epi16(max128, load).SShort0;
                    min = Xse.min_epi16(min128, load).SShort0;
                }
                else
                {
                    max = max128.SShort0;
                    min = min128.SShort0;
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;

                v128 max0 = Xse.set1_epi16(short.MinValue);

                v128 min0 = Xse.set1_epi16(short.MaxValue);

                if (Hint.Likely(length >= 32))
                {
                    v128 max1 = Xse.set1_epi16(short.MinValue);
                    v128 max2 = Xse.set1_epi16(short.MinValue);
                    v128 max3 = Xse.set1_epi16(short.MinValue);

                    v128 min1 = Xse.set1_epi16(short.MaxValue);
                    v128 min2 = Xse.set1_epi16(short.MaxValue);
                    v128 min3 = Xse.set1_epi16(short.MaxValue);

                    do
                    {
                        v128 load0 = Xse.loadu_si128(ptr_v128++);
                        v128 load1 = Xse.loadu_si128(ptr_v128++);
                        v128 load2 = Xse.loadu_si128(ptr_v128++);
                        v128 load3 = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epi16(max0, load0);
                        max1 = Xse.max_epi16(max1, load1);
                        max2 = Xse.max_epi16(max2, load2);
                        max3 = Xse.max_epi16(max3, load3);

                        min0 = Xse.min_epi16(min0, load0);
                        min1 = Xse.min_epi16(min1, load1);
                        min2 = Xse.min_epi16(min2, load2);
                        min3 = Xse.min_epi16(min3, load3);

                        length -= 32;
                    }
                    while (Hint.Likely(length >= 32));

                    max0 = Xse.max_epi16(max0, max1);
                    max2 = Xse.max_epi16(max2, max3);
                    max0 = Xse.max_epi16(max0, max2);

                    min0 = Xse.min_epi16(min0, min1);
                    min2 = Xse.min_epi16(min2, min3);
                    min0 = Xse.min_epi16(min0, min2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    v128 load = Xse.loadu_si128(ptr_v128++);

                    max0 = Xse.max_epi16(max0, load);
                    min0 = Xse.min_epi16(min0, load);

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        load = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epi16(max0, load);
                        min0 = Xse.min_epi16(min0, load);

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            load = Xse.loadu_si128(ptr_v128++);

                            max0 = Xse.max_epi16(max0, load);
                            min0 = Xse.min_epi16(min0, load);

                            length -= 3 * 8;
                        }
                        else
                        {
                            length -= 2 * 8;
                        }
                    }
                    else
                    {
                        length -= 8;
                    }
                }

                v128 cmp = Xse.setzero_si128();
                max0 = Xse.max_epi16(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_epi16(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v128);

                    max0 = Xse.max_epi16(max0, load);
                    min0 = Xse.min_epi16(min0, load);

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 4;
                }

                max0 = Xse.max_epi16(max0, Xse.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_epi16(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v128);

                    max0 = Xse.max_epi16(max0, load);
                    min0 = Xse.min_epi16(min0, load);

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 2;
                }

                max0 = Xse.max_epi16(max0, Xse.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 0, 1)));
                min0 = Xse.min_epi16(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    v128 load = Xse.insert_epi16(cmp, *(ushort*)ptr_v128, 0);

                    max = Xse.max_epi16(max0, load).SShort0;
                    min = Xse.min_epi16(min0, load).SShort0;
                }
                else
                {
                    max = max0.SShort0;
                    min = min0.SShort0;
                }
            }
            else
            {
                max = short.MinValue;
                min = short.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    short deref = ptr[i];

                    max = (short)math.max(max, deref);
                    min = (short)math.min(min, deref);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<short> array, out short min, out short max)
        {
            SIMD_MinMax((short*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<short> array, int index, out short min, out short max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<short> array, int index, int numEntries, out short min, out short max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<short> array, out short min, out short max)
        {
            SIMD_MinMax((short*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<short> array, int index, out short min, out short max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<short> array, int index, int numEntries, out short min, out short max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<short> array, out short min, out short max)
        {
            SIMD_MinMax((short*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<short> array, int index, out short min, out short max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<short> array, int index, int numEntries, out short min, out short max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(int* ptr, long length, out int min, out int max)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;

                v256 max0 = Avx.mm256_set1_epi32(int.MinValue);

                v256 min0 = Avx.mm256_set1_epi32(int.MaxValue);

                if (Hint.Likely(length >= 32))
                {
                    v256 max1 = Avx.mm256_set1_epi32(int.MinValue);
                    v256 max2 = Avx.mm256_set1_epi32(int.MinValue);
                    v256 max3 = Avx.mm256_set1_epi32(int.MinValue);

                    v256 min1 = Avx.mm256_set1_epi32(int.MaxValue);
                    v256 min2 = Avx.mm256_set1_epi32(int.MaxValue);
                    v256 min3 = Avx.mm256_set1_epi32(int.MaxValue);

                    do
                    {
                        v256 load0 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load1 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load2 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load3 = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Avx2.mm256_max_epi32(max0, load0);
                        max1 = Avx2.mm256_max_epi32(max1, load1);
                        max2 = Avx2.mm256_max_epi32(max2, load2);
                        max3 = Avx2.mm256_max_epi32(max3, load3);

                        min0 = Avx2.mm256_min_epi32(min0, load0);
                        min1 = Avx2.mm256_min_epi32(min1, load1);
                        min2 = Avx2.mm256_min_epi32(min2, load2);
                        min3 = Avx2.mm256_min_epi32(min3, load3);

                        length -= 32;
                    }
                    while (Hint.Likely(length >= 32));

                    max0 = Avx2.mm256_max_epi32(max0, max1);
                    max2 = Avx2.mm256_max_epi32(max2, max3);
                    max0 = Avx2.mm256_max_epi32(max0, max2);

                    min0 = Avx2.mm256_min_epi32(min0, min1);
                    min2 = Avx2.mm256_min_epi32(min2, min3);
                    min0 = Avx2.mm256_min_epi32(min0, min2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    v256 load = Avx.mm256_loadu_si256(ptr_v256++);

                    max0 = Avx2.mm256_max_epi32(max0, load);
                    min0 = Avx2.mm256_min_epi32(min0, load);

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        load = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Avx2.mm256_max_epi32(max0, load);
                        min0 = Avx2.mm256_min_epi32(min0, load);

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            load = Avx.mm256_loadu_si256(ptr_v256++);

                            max0 = Avx2.mm256_max_epi32(max0, load);
                            min0 = Avx2.mm256_min_epi32(min0, load);

                            length -= 3 * 8;
                        }
                        else
                        {
                            length -= 2 * 8;
                        }
                    }
                    else
                    {
                        length -= 8;
                    }
                }

                v128 max128 = Xse.max_epi32(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));
                v128 min128 = Xse.min_epi32(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.loadu_si128(ptr_v256);

                    max128 = Xse.max_epi32(max128, load);
                    min128 = Xse.min_epi32(min128, load);

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 4;
                }

                max128 = Xse.max_epi32(max128, Xse.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));
                min128 = Xse.min_epi32(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v256);

                    max128 = Xse.max_epi32(max128, load);
                    min128 = Xse.min_epi32(min128, load);

                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Xse.max_epi32(max128, Xse.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 0, 1)));
                min128 = Xse.min_epi32(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v256);

                    max = Xse.max_epi32(max128, load).SInt0;
                    min = Xse.min_epi32(min128, load).SInt0;
                }
                else
                {
                    max = max128.SInt0;
                    min = min128.SInt0;
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;

                v128 max0 = Xse.set1_epi32(int.MinValue);

                v128 min0 = Xse.set1_epi32(int.MaxValue);

                if (Hint.Likely(length >= 16))
                {
                    v128 max1 = Xse.set1_epi32(int.MinValue);
                    v128 max2 = Xse.set1_epi32(int.MinValue);
                    v128 max3 = Xse.set1_epi32(int.MinValue);

                    v128 min1 = Xse.set1_epi32(int.MaxValue);
                    v128 min2 = Xse.set1_epi32(int.MaxValue);
                    v128 min3 = Xse.set1_epi32(int.MaxValue);

                    do
                    {
                        v128 load0 = Xse.loadu_si128(ptr_v128++);
                        v128 load1 = Xse.loadu_si128(ptr_v128++);
                        v128 load2 = Xse.loadu_si128(ptr_v128++);
                        v128 load3 = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epi32(max0, load0);
                        max1 = Xse.max_epi32(max1, load1);
                        max2 = Xse.max_epi32(max2, load2);
                        max3 = Xse.max_epi32(max3, load3);

                        min0 = Xse.min_epi32(min0, load0);
                        min1 = Xse.min_epi32(min1, load1);
                        min2 = Xse.min_epi32(min2, load2);
                        min3 = Xse.min_epi32(min3, load3);

                        length -= 16;
                    }
                    while (Hint.Likely(length >= 16));

                    max0 = Xse.max_epi32(max0, max1);
                    max2 = Xse.max_epi32(max2, max3);
                    max0 = Xse.max_epi32(max0, max2);

                    min0 = Xse.min_epi32(min0, min1);
                    min2 = Xse.min_epi32(min2, min3);
                    min0 = Xse.min_epi32(min0, min2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = Xse.loadu_si128(ptr_v128++);

                    max0 = Xse.max_epi32(max0, load);
                    min0 = Xse.min_epi32(min0, load);

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        load = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epi32(max0, load);
                        min0 = Xse.min_epi32(min0, load);

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            load = Xse.loadu_si128(ptr_v128++);

                            max0 = Xse.max_epi32(max0, load);
                            min0 = Xse.min_epi32(min0, load);

                            length -= 3 * 4;
                        }
                        else
                        {
                            length -= 2 * 4;
                        }
                    }
                    else
                    {
                        length -= 4;
                    }
                }

                max0 = Xse.max_epi32(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_epi32(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v128);

                    max0 = Xse.max_epi32(max0, load);
                    min0 = Xse.min_epi32(min0, load);

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 2;
                }

                max0 = Xse.max_epi32(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 0, 1)));
                min0 = Xse.min_epi32(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    if (Sse4_1.IsSse41Supported)
                    {
                        v128 load = Xse.cvtsi32_si128(*(int*)ptr_v128);

                        max = Xse.max_epi32(max0, load).SInt0;
                        min = Xse.min_epi32(min0, load).SInt0;
                    }
                    else
                    {
                        int load = *(int*)ptr_v128;

                        max = math.max(max0.SInt0, load);
                        min = math.min(min0.SInt0, load);
                    }
                }
                else
                {
                    max = max0.SInt0;
                    min = min0.SInt0;
                }
            }
            else
            {
                max = int.MinValue;
                min = int.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    int deref = ptr[i];

                    max = math.max(max, deref);
                    min = math.min(min, deref);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<int> array, out int min, out int max)
        {
            SIMD_MinMax((int*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<int> array, int index, out int min, out int max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<int> array, int index, int numEntries, out int min, out int max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<int> array, out int min, out int max)
        {
            SIMD_MinMax((int*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<int> array, int index, out int min, out int max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<int> array, int index, int numEntries, out int min, out int max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<int> array, out int min, out int max)
        {
            SIMD_MinMax((int*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<int> array, int index, out int min, out int max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<int> array, int index, int numEntries, out int min, out int max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(long* ptr, long length, out long min, out long max)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;

                v256 max0 = Avx.mm256_set1_epi64x(long.MinValue);

                v256 min0 = Avx.mm256_set1_epi64x(long.MaxValue);

                if (Hint.Likely(length >= 16))
                {
                    v256 max1 = Avx.mm256_set1_epi64x(long.MinValue);
                    v256 max2 = Avx.mm256_set1_epi64x(long.MinValue);
                    v256 max3 = Avx.mm256_set1_epi64x(long.MinValue);

                    v256 min1 = Avx.mm256_set1_epi64x(long.MaxValue);
                    v256 min2 = Avx.mm256_set1_epi64x(long.MaxValue);
                    v256 min3 = Avx.mm256_set1_epi64x(long.MaxValue);

                    do
                    {
                        v256 load0 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load1 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load2 = Avx.mm256_loadu_si256(ptr_v256++);
                        v256 load3 = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Xse.mm256_max_epi64(max0, load0);
                        max1 = Xse.mm256_max_epi64(max1, load1);
                        max2 = Xse.mm256_max_epi64(max2, load2);
                        max3 = Xse.mm256_max_epi64(max3, load3);

                        min0 = Xse.mm256_min_epi64(min0, load0);
                        min1 = Xse.mm256_min_epi64(min1, load1);
                        min2 = Xse.mm256_min_epi64(min2, load2);
                        min3 = Xse.mm256_min_epi64(min3, load3);

                        length -= 16;
                    }
                    while (Hint.Likely(length >= 16));

                    max0 = Xse.mm256_max_epi64(max0, max1);
                    max2 = Xse.mm256_max_epi64(max2, max3);
                    max0 = Xse.mm256_max_epi64(max0, max2);

                    min0 = Xse.mm256_min_epi64(min0, min1);
                    min2 = Xse.mm256_min_epi64(min2, min3);
                    min0 = Xse.mm256_min_epi64(min0, min2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    v256 load = Avx.mm256_loadu_si256(ptr_v256++);

                    max0 = Xse.mm256_max_epi64(max0, load);
                    min0 = Xse.mm256_min_epi64(min0, load);

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        load = Avx.mm256_loadu_si256(ptr_v256++);

                        max0 = Xse.mm256_max_epi64(max0, load);
                        min0 = Xse.mm256_min_epi64(min0, load);

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            load = Avx.mm256_loadu_si256(ptr_v256++);

                            max0 = Xse.mm256_max_epi64(max0, load);
                            min0 = Xse.mm256_min_epi64(min0, load);

                            length -= 3 * 4;
                        }
                        else
                        {
                            length -= 2 * 4;
                        }
                    }
                    else
                    {
                        length -= 4;
                    }
                }

                v128 max128 = Xse.max_epi64(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));
                v128 min128 = Xse.min_epi64(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.loadu_si128(ptr_v256);

                    max128 = Xse.max_epi64(max128, load);
                    min128 = Xse.min_epi64(min128, load);

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Xse.max_epi64(max128, Xse.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));
                min128 = Xse.min_epi64(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    long load = *(long*)ptr_v256;

                    max = math.max(max128.SLong0, load);
                    min = math.min(min128.SLong0, load);
                }
                else
                {
                    max = max128.SLong0;
                    min = min128.SLong0;
                }
            }
            else if (BurstArchitecture.IsCMP64Supported)
            {
                v128* ptr_v128 = (v128*)ptr;

                v128 max0 = Xse.set1_epi64x(long.MinValue);

                v128 min0 = Xse.set1_epi64x(long.MaxValue);

                if (Hint.Likely(length >= 8))
                {
                    v128 max1 = Xse.set1_epi64x(long.MinValue);
                    v128 max2 = Xse.set1_epi64x(long.MinValue);
                    v128 max3 = Xse.set1_epi64x(long.MinValue);

                    v128 min1 = Xse.set1_epi64x(long.MaxValue);
                    v128 min2 = Xse.set1_epi64x(long.MaxValue);
                    v128 min3 = Xse.set1_epi64x(long.MaxValue);

                    do
                    {
                        v128 load0 = Xse.loadu_si128(ptr_v128++);
                        v128 load1 = Xse.loadu_si128(ptr_v128++);
                        v128 load2 = Xse.loadu_si128(ptr_v128++);
                        v128 load3 = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epi64(max0, load0);
                        max1 = Xse.max_epi64(max1, load1);
                        max2 = Xse.max_epi64(max2, load2);
                        max3 = Xse.max_epi64(max3, load3);

                        min0 = Xse.min_epi64(min0, load0);
                        min1 = Xse.min_epi64(min1, load1);
                        min2 = Xse.min_epi64(min2, load2);
                        min3 = Xse.min_epi64(min3, load3);

                        length -= 8;
                    }
                    while (Hint.Likely(length >= 8));

                    max0 = Xse.max_epi64(max0, max1);
                    max2 = Xse.max_epi64(max2, max3);
                    max0 = Xse.max_epi64(max0, max2);

                    min0 = Xse.min_epi64(min0, min1);
                    min2 = Xse.min_epi64(min2, min3);
                    min0 = Xse.min_epi64(min0, min2);
                }

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.loadu_si128(ptr_v128++);

                    max0 = Xse.max_epi64(max0, load);
                    min0 = Xse.min_epi64(min0, load);

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        load = Xse.loadu_si128(ptr_v128++);

                        max0 = Xse.max_epi64(max0, load);
                        min0 = Xse.min_epi64(min0, load);

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            load = Xse.loadu_si128(ptr_v128++);

                            max0 = Xse.max_epi64(max0, load);
                            min0 = Xse.min_epi64(min0, load);

                            length -= 3 * 2;
                        }
                        else
                        {
                            length -= 2 * 2;
                        }
                    }
                    else
                    {
                        length -= 2;
                    }
                }

                max0 = Xse.max_epi64(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_epi64(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    long load = *(long*)ptr_v128;

                    max = math.max(max0.SLong0, load);
                    min = math.min(min0.SLong0, load);
                }
                else
                {
                    max = max0.SLong0;
                    min = min0.SLong0;
                }
            }
            else
            {
                max = long.MinValue;
                min = long.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    long deref = ptr[i];

                    max = math.max(max, deref);
                    min = math.min(min, deref);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<long> array, out long min, out long max)
        {
            SIMD_MinMax((long*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<long> array, int index, out long min, out long max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<long> array, int index, int numEntries, out long min, out long max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<long> array, out long min, out long max)
        {
            SIMD_MinMax((long*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<long> array, int index, out long min, out long max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<long> array, int index, int numEntries, out long min, out long max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<long> array, out long min, out long max)
        {
            SIMD_MinMax((long*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<long> array, int index, out long min, out long max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<long> array, int index, int numEntries, out long min, out long max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(float* ptr, long length, out float min, out float max)
        {
Assert.IsNonNegative(length);

            if (Avx.IsAvxSupported)
            {
                v256* ptr_v256 = (v256*)ptr;

                v256 max0 = new v256(float.NegativeInfinity);

                v256 min0 = new v256(float.PositiveInfinity);

                if (Hint.Likely(length >= 32))
                {
                    v256 max1 = new v256(float.NegativeInfinity);
                    v256 max2 = new v256(float.NegativeInfinity);
                    v256 max3 = new v256(float.NegativeInfinity);

                    v256 min1 = new v256(float.PositiveInfinity);
                    v256 min2 = new v256(float.PositiveInfinity);
                    v256 min3 = new v256(float.PositiveInfinity);

                    do
                    {
                        v256 load0 = Avx.mm256_loadu_ps(ptr_v256++);
                        v256 load1 = Avx.mm256_loadu_ps(ptr_v256++);
                        v256 load2 = Avx.mm256_loadu_ps(ptr_v256++);
                        v256 load3 = Avx.mm256_loadu_ps(ptr_v256++);

                        max0 = Avx.mm256_max_ps(max0, load0);
                        max1 = Avx.mm256_max_ps(max1, load1);
                        max2 = Avx.mm256_max_ps(max2, load2);
                        max3 = Avx.mm256_max_ps(max3, load3);

                        min0 = Avx.mm256_min_ps(min0, load0);
                        min1 = Avx.mm256_min_ps(min1, load1);
                        min2 = Avx.mm256_min_ps(min2, load2);
                        min3 = Avx.mm256_min_ps(min3, load3);

                        length -= 32;
                    }
                    while (Hint.Likely(length >= 32));

                    max0 = Avx.mm256_max_ps(max0, max1);
                    max2 = Avx.mm256_max_ps(max2, max3);
                    max0 = Avx.mm256_max_ps(max0, max2);

                    min0 = Avx.mm256_min_ps(min0, min1);
                    min2 = Avx.mm256_min_ps(min2, min3);
                    min0 = Avx.mm256_min_ps(min0, min2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    v256 load = Avx.mm256_loadu_ps(ptr_v256++);

                    max0 = Avx.mm256_max_ps(max0, load);
                    min0 = Avx.mm256_min_ps(min0, load);

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        load = Avx.mm256_loadu_ps(ptr_v256++);

                        max0 = Avx.mm256_max_ps(max0, load);
                        min0 = Avx.mm256_min_ps(min0, load);

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            load = Avx.mm256_loadu_ps(ptr_v256++);

                            max0 = Avx.mm256_max_ps(max0, load);
                            min0 = Avx.mm256_min_ps(min0, load);

                            length -= 3 * 8;
                        }
                        else
                        {
                            length -= 2 * 8;
                        }
                    }
                    else
                    {
                        length -= 8;
                    }
                }

                v128 max128 = Xse.max_ps(Avx.mm256_castps256_ps128(max0), Avx.mm256_extractf128_ps(max0, 1));
                v128 min128 = Xse.min_ps(Avx.mm256_castps256_ps128(min0), Avx.mm256_extractf128_ps(min0, 1));

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = *(v128*)ptr_v256;

                    max128 = Xse.max_ps(max128, load);
                    min128 = Xse.min_ps(min128, load);

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 4;
                }

                max128 = Xse.max_ps(max128, Avx.permute_ps(max128, Sse.SHUFFLE(0, 0, 3, 2)));
                min128 = Xse.min_ps(min128, Avx.permute_ps(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v256);

                    max128 = Xse.max_ps(max128, load);
                    min128 = Xse.min_ps(min128, load);

                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);

                    length -= 2;
                }

                max128 = Xse.max_ps(max128, Avx.permute_ps(max128, Sse.SHUFFLE(0, 0, 0, 1)));
                min128 = Xse.min_ps(min128, Avx.permute_ps(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v256);

                    max128 = Xse.max_ss(max128, load);
                    min128 = Xse.min_ss(min128, load);
                }

                max = max128.Float0;
                min = min128.Float0;
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;

                v128 max0 = new v128(float.NegativeInfinity);

                v128 min0 = new v128(float.PositiveInfinity);

                if (Hint.Likely(length >= 16))
                {
                    v128 max1 = new v128(float.NegativeInfinity);
                    v128 max2 = new v128(float.NegativeInfinity);
                    v128 max3 = new v128(float.NegativeInfinity);

                    v128 min1 = new v128(float.PositiveInfinity);
                    v128 min2 = new v128(float.PositiveInfinity);
                    v128 min3 = new v128(float.PositiveInfinity);

                    do
                    {
                        v128 load0 = *ptr_v128++;
                        v128 load1 = *ptr_v128++;
                        v128 load2 = *ptr_v128++;
                        v128 load3 = *ptr_v128++;

                        max0 = Xse.max_ps(max0, load0);
                        max1 = Xse.max_ps(max1, load1);
                        max2 = Xse.max_ps(max2, load2);
                        max3 = Xse.max_ps(max3, load3);

                        min0 = Xse.min_ps(min0, load0);
                        min1 = Xse.min_ps(min1, load1);
                        min2 = Xse.min_ps(min2, load2);
                        min3 = Xse.min_ps(min3, load3);

                        length -= 16;
                    }
                    while (Hint.Likely(length >= 16));

                    max0 = Xse.max_ps(max0, max1);
                    max2 = Xse.max_ps(max2, max3);
                    max0 = Xse.max_ps(max0, max2);

                    min0 = Xse.min_ps(min0, min1);
                    min2 = Xse.min_ps(min2, min3);
                    min0 = Xse.min_ps(min0, min2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    v128 load = *ptr_v128++;

                    max0 = Xse.max_ps(max0, load);
                    min0 = Xse.min_ps(min0, load);

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        load = *ptr_v128++;

                        max0 = Xse.max_ps(max0, load);
                        min0 = Xse.min_ps(min0, load);

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            load = *ptr_v128++;

                            max0 = Xse.max_ps(max0, load);
                            min0 = Xse.min_ps(min0, load);

                            length -= 3 * 4;
                        }
                        else
                        {
                            length -= 2 * 4;
                        }
                    }
                    else
                    {
                        length -= 4;
                    }
                }

                max0 = Xse.max_ps(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_ps(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v128);

                    max0 = Xse.max_ps(max0, load);
                    min0 = Xse.min_ps(min0, load);

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);

                    length -= 2;
                }

                max0 = Xse.max_ps(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 0, 1)));
                min0 = Xse.min_ps(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    v128 load = Xse.cvtsi32_si128(*(int*)ptr_v128);

                    max0 = Xse.max_ss(max0, load);
                    min0 = Xse.min_ss(min0, load);
                }

                max = max0.Float0;
                min = min0.Float0;
            }
            else
            {
                max = float.NegativeInfinity;
                min = float.PositiveInfinity;

                for (long i = 0; i < length; i++)
                {
                    float deref = ptr[i];

                    max = math.max(max, deref);
                    min = math.min(min, deref);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<float> array, out float min, out float max)
        {
            SIMD_MinMax((float*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<float> array, int index, out float min, out float max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<float> array, int index, int numEntries, out float min, out float max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<float> array, out float min, out float max)
        {
            SIMD_MinMax((float*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<float> array, int index, out float min, out float max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<float> array, int index, int numEntries, out float min, out float max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<float> array, out float min, out float max)
        {
            SIMD_MinMax((float*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<float> array, int index, out float min, out float max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<float> array, int index, int numEntries, out float min, out float max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(double* ptr, long length, out double min, out double max)
        {
Assert.IsNonNegative(length);

            if (Avx.IsAvxSupported)
            {
                v256* ptr_v256 = (v256*)ptr;

                v256 max0 = new v256(double.NegativeInfinity);

                v256 min0 = new v256(double.PositiveInfinity);

                if (Hint.Likely(length >= 16))
                {
                    v256 max1 = new v256(double.NegativeInfinity);
                    v256 max2 = new v256(double.NegativeInfinity);
                    v256 max3 = new v256(double.NegativeInfinity);

                    v256 min1 = new v256(double.PositiveInfinity);
                    v256 min2 = new v256(double.PositiveInfinity);
                    v256 min3 = new v256(double.PositiveInfinity);

                    do
                    {
                        v256 load0 = Avx.mm256_loadu_pd(ptr_v256++);
                        v256 load1 = Avx.mm256_loadu_pd(ptr_v256++);
                        v256 load2 = Avx.mm256_loadu_pd(ptr_v256++);
                        v256 load3 = Avx.mm256_loadu_pd(ptr_v256++);

                        max0 = Avx.mm256_max_pd(max0, load0);
                        max1 = Avx.mm256_max_pd(max1, load1);
                        max2 = Avx.mm256_max_pd(max2, load2);
                        max3 = Avx.mm256_max_pd(max3, load3);

                        min0 = Avx.mm256_min_pd(min0, load0);
                        min1 = Avx.mm256_min_pd(min1, load1);
                        min2 = Avx.mm256_min_pd(min2, load2);
                        min3 = Avx.mm256_min_pd(min3, load3);

                        length -= 16;
                    }
                    while (Hint.Likely(length >= 16));

                    max0 = Avx.mm256_max_pd(max0, max1);
                    max2 = Avx.mm256_max_pd(max2, max3);
                    max0 = Avx.mm256_max_pd(max0, max2);

                    min0 = Avx.mm256_min_pd(min0, min1);
                    min2 = Avx.mm256_min_pd(min2, min3);
                    min0 = Avx.mm256_min_pd(min0, min2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    v256 load = Avx.mm256_loadu_pd(ptr_v256++);

                    max0 = Avx.mm256_max_pd(max0, load);
                    min0 = Avx.mm256_min_pd(min0, load);

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        load = Avx.mm256_loadu_pd(ptr_v256++);

                        max0 = Avx.mm256_max_pd(max0, load);
                        min0 = Avx.mm256_min_pd(min0, load);

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            load = Avx.mm256_loadu_pd(ptr_v256++);

                            max0 = Avx.mm256_max_pd(max0, load);
                            min0 = Avx.mm256_min_pd(min0, load);

                            length -= 3 * 4;
                        }
                        else
                        {
                            length -= 2 * 4;
                        }
                    }
                    else
                    {
                        length -= 4;
                    }
                }

                v128 max128 = Xse.max_pd(Avx.mm256_castpd256_pd128(max0), Avx.mm256_extractf128_pd(max0, 1));
                v128 min128 = Xse.min_pd(Avx.mm256_castpd256_pd128(min0), Avx.mm256_extractf128_pd(min0, 1));

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = *(v128*)ptr_v256;

                    max128 = Xse.max_pd(max128, load);
                    min128 = Xse.min_pd(min128, load);

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 2;
                }

                max128 = Xse.max_pd(max128, Avx.permute_pd(max128, Sse.SHUFFLE(0, 0, 0, 1)));
                min128 = Xse.min_pd(min128, Avx.permute_pd(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v256);

                    max128 = Xse.max_sd(max128, load);
                    min128 = Xse.min_sd(min128, load);
                }

                max = max128.Double0;
                min = min128.Double0;
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;

                v128 max0 = new v128(double.NegativeInfinity);

                v128 min0 = new v128(double.PositiveInfinity);

                if (Hint.Likely(length >= 8))
                {
                    v128 max1 = new v128(double.NegativeInfinity);
                    v128 max2 = new v128(double.NegativeInfinity);
                    v128 max3 = new v128(double.NegativeInfinity);

                    v128 min1 = new v128(double.PositiveInfinity);
                    v128 min2 = new v128(double.PositiveInfinity);
                    v128 min3 = new v128(double.PositiveInfinity);

                    do
                    {
                        v128 load0 = *ptr_v128++;
                        v128 load1 = *ptr_v128++;
                        v128 load2 = *ptr_v128++;
                        v128 load3 = *ptr_v128++;

                        max0 = Xse.max_pd(max0, load0);
                        max1 = Xse.max_pd(max1, load1);
                        max2 = Xse.max_pd(max2, load2);
                        max3 = Xse.max_pd(max3, load3);

                        min0 = Xse.min_pd(min0, load0);
                        min1 = Xse.min_pd(min1, load1);
                        min2 = Xse.min_pd(min2, load2);
                        min3 = Xse.min_pd(min3, load3);

                        length -= 8;
                    }
                    while (Hint.Likely(length >= 8));

                    max0 = Xse.max_pd(max0, max1);
                    max2 = Xse.max_pd(max2, max3);
                    max0 = Xse.max_pd(max0, max2);

                    min0 = Xse.min_pd(min0, min1);
                    min2 = Xse.min_pd(min2, min3);
                    min0 = Xse.min_pd(min0, min2);
                }

                if (Hint.Likely((int)length >= 2))
                {
                    v128 load = *ptr_v128++;

                    max0 = Xse.max_pd(max0, load);
                    min0 = Xse.min_pd(min0, load);

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        load = *ptr_v128++;

                        max0 = Xse.max_pd(max0, load);
                        min0 = Xse.min_pd(min0, load);

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            load = *ptr_v128++;

                            max0 = Xse.max_pd(max0, load);
                            min0 = Xse.min_pd(min0, load);

                            length -= 3 * 2;
                        }
                        else
                        {
                            length -= 2 * 2;
                        }
                    }
                    else
                    {
                        length -= 2;
                    }
                }

                max0 = Xse.max_pd(max0, Xse.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));
                min0 = Xse.min_pd(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    v128 load = Xse.cvtsi64x_si128(*(long*)ptr_v128);

                    max0 = Xse.max_sd(max0, load);
                    min0 = Xse.min_sd(min0, load);
                }

                max = max0.Double0;
                min = min0.Double0;
            }
            else
            {
                max = double.NegativeInfinity;
                min = double.PositiveInfinity;

                for (long i = 0; i < length; i++)
                {
                    double deref = ptr[i];

                    max = math.max(max, deref);
                    min = math.min(min, deref);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<double> array, out double min, out double max)
        {
            SIMD_MinMax((double*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<double> array, int index, out double min, out double max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeArray<double> array, int index, int numEntries, out double min, out double max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<double> array, out double min, out double max)
        {
            SIMD_MinMax((double*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<double> array, int index, out double min, out double max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeList<double> array, int index, int numEntries, out double min, out double max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<double> array, out double min, out double max)
        {
            SIMD_MinMax((double*)array.GetUnsafeReadOnlyPtr(), array.Length, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<double> array, int index, out double min, out double max)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            SIMD_MinMax((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SIMD_MinMax(this NativeSlice<double> array, int index, int numEntries, out double min, out double max)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            SIMD_MinMax((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, out min, out max);
        }
    }
}