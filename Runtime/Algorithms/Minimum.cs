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
        public static byte SIMD_Minimum(byte* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 min0 = Avx.mm256_set1_epi8(byte.MaxValue);

                if (Hint.Likely(length >= 128))
                {
                    v256 min1 = Avx.mm256_set1_epi8(byte.MaxValue);
                    v256 min2 = Avx.mm256_set1_epi8(byte.MaxValue);
                    v256 min3 = Avx.mm256_set1_epi8(byte.MaxValue);

                    do
                    {
                        min0 = Avx2.mm256_min_epu8(min0, Avx.mm256_loadu_si256(ptr_v256++));
                        min1 = Avx2.mm256_min_epu8(min1, Avx.mm256_loadu_si256(ptr_v256++));
                        min2 = Avx2.mm256_min_epu8(min2, Avx.mm256_loadu_si256(ptr_v256++));
                        min3 = Avx2.mm256_min_epu8(min3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 128;
                    }
                    while (Hint.Likely(length >= 128));

                    min0 = Avx2.mm256_min_epu8(min0, min1);
                    min2 = Avx2.mm256_min_epu8(min2, min3);
                    min0 = Avx2.mm256_min_epu8(min0, min2);
                }

                if (Hint.Likely((int)length >= 32))
                {
                    min0 = Avx2.mm256_min_epu8(min0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 32))
                    {
                        min0 = Avx2.mm256_min_epu8(min0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 32))
                        {
                            min0 = Avx2.mm256_min_epu8(min0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 min128 = Xse.min_epu8(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 16))
                {
                    min128 = Xse.min_epu8(min128, Xse.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 16;
                }

                v128 cmp = Xse.setzero_si128();
                min128 = Xse.min_epu8(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    min128 = Xse.min_epu8(min128, Xse.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 8;
                }

                min128 = Xse.min_epu8(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    min128 = Xse.min_epu8(min128, Xse.cvtsi32_si128(*(int*)ptr_v256));
                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 4;
                }

                min128 = Xse.min_epu8(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    min128 = Xse.min_epu8(min128, Xse.insert_epi16(cmp, *(ushort*)ptr_v256, 0));
                    ptr_v256 = (v256*)((short*)ptr_v256 + 1);
                    length -= 2;
                }

                min128 = Xse.min_epu8(min128, Xse.bsrli_si128(min128, 1 * sizeof(byte)));

                if (Hint.Likely(length != 0))
                {
                    return Xse.min_epu8(min128, Xse.insert_epi8(cmp, *(byte*)ptr_v256, 0)).Byte0;
                }
                else
                {
                    return min128.Byte0;
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 min0 = Xse.set1_epi8(unchecked((sbyte)byte.MaxValue));

                if (Hint.Likely(length >= 64))
                {
                    v128 min1 = Xse.set1_epi8(unchecked((sbyte)byte.MaxValue));
                    v128 min2 = Xse.set1_epi8(unchecked((sbyte)byte.MaxValue));
                    v128 min3 = Xse.set1_epi8(unchecked((sbyte)byte.MaxValue));

                    do
                    {
                        min0 = Xse.min_epu8(min0, Xse.loadu_si128(ptr_v128++));
                        min1 = Xse.min_epu8(min1, Xse.loadu_si128(ptr_v128++));
                        min2 = Xse.min_epu8(min2, Xse.loadu_si128(ptr_v128++));
                        min3 = Xse.min_epu8(min3, Xse.loadu_si128(ptr_v128++));

                        length -= 64;
                    }
                    while (Hint.Likely(length >= 64));

                    min0 = Xse.min_epu8(min0, min1);
                    min2 = Xse.min_epu8(min2, min3);
                    min0 = Xse.min_epu8(min0, min2);
                }

                if (Hint.Likely((int)length >= 16))
                {
                    min0 = Xse.min_epu8(min0, Xse.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        min0 = Xse.min_epu8(min0, Xse.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            min0 = Xse.min_epu8(min0, Xse.loadu_si128(ptr_v128++));
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
                min0 = Xse.min_epu8(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    min0 = Xse.min_epu8(min0, Xse.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 8;
                }

                min0 = Xse.min_epu8(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    min0 = Xse.min_epu8(min0, Xse.cvtsi32_si128(*(int*)ptr_v128));

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 4;
                }

                min0 = Xse.min_epu8(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    min0 = Xse.min_epu8(min0, Xse.insert_epi16(cmp, *(ushort*)ptr_v128, 0));
                    ptr_v128 = (v128*)((short*)ptr_v128 + 1);
                    length -= 2;
                }

                min0 = Xse.min_epu8(min0, Xse.bsrli_si128(min0, 1 * sizeof(byte)));

                if (Hint.Likely(length != 0))
                {
                    if (BurstArchitecture.IsMinMaxSupported)
                    {
                        return Xse.min_epu8(min0, Xse.insert_epi8(cmp, *(byte*)ptr_v128, 0)).Byte0;
                    }
                    else
                    {
                        return Xse.min_epu8(min0, Xse.cvtsi32_si128(*(byte*)ptr_v128)).Byte0;
                    }
                }
                else
                {
                    return min0.Byte0;
                }
            }
            else
            {
                byte x = byte.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    x = (byte)math.min((uint)x, (uint)ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Minimum(this NativeArray<byte> array)
        {
            return SIMD_Minimum((byte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Minimum(this NativeArray<byte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Minimum(this NativeArray<byte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Minimum(this NativeList<byte> array)
        {
            return SIMD_Minimum((byte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Minimum(this NativeList<byte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Minimum(this NativeList<byte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Minimum(this NativeSlice<byte> array)
        {
            return SIMD_Minimum((byte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Minimum(this NativeSlice<byte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Minimum(this NativeSlice<byte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Minimum(ushort* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 min0 = Avx.mm256_set1_epi16(unchecked((short)ushort.MaxValue));

                if (Hint.Likely(length >= 64))
                {
                    v256 min1 = Avx.mm256_set1_epi16(unchecked((short)ushort.MaxValue));
                    v256 min2 = Avx.mm256_set1_epi16(unchecked((short)ushort.MaxValue));
                    v256 min3 = Avx.mm256_set1_epi16(unchecked((short)ushort.MaxValue));

                    do
                    {
                        min0 = Avx2.mm256_min_epu16(min0, Avx.mm256_loadu_si256(ptr_v256++));
                        min1 = Avx2.mm256_min_epu16(min1, Avx.mm256_loadu_si256(ptr_v256++));
                        min2 = Avx2.mm256_min_epu16(min2, Avx.mm256_loadu_si256(ptr_v256++));
                        min3 = Avx2.mm256_min_epu16(min3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 64;
                    }
                    while (Hint.Likely(length >= 64));

                    min0 = Avx2.mm256_min_epu16(min0, min1);
                    min2 = Avx2.mm256_min_epu16(min2, min3);
                    min0 = Avx2.mm256_min_epu16(min0, min2);
                }

                if (Hint.Likely((int)length >= 16))
                {
                    min0 = Avx2.mm256_min_epu16(min0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        min0 = Avx2.mm256_min_epu16(min0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            min0 = Avx2.mm256_min_epu16(min0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 min128 = Xse.min_epu16(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 8))
                {
                    min128 = Xse.min_epu16(min128, Xse.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 8;
                }

                v128 cmp = Xse.setzero_si128();
                min128 = Xse.min_epu16(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    min128 = Xse.min_epu16(min128, Xse.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 4;
                }

                min128 = Xse.min_epu16(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    min128 = Xse.min_epu16(min128, Xse.cvtsi32_si128(*(int*)ptr_v256));
                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 2;
                }

                min128 = Xse.min_epu16(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Xse.min_epu16(min128, Xse.insert_epi16(cmp, *(ushort*)ptr_v256, 0)).UShort0;
                }
                else
                {
                    return min128.UShort0;
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 min0 = Xse.set1_epi16(unchecked((short)ushort.MaxValue));

                if (Hint.Likely(length >= 32))
                {
                    v128 min1 = Xse.set1_epi16(unchecked((short)ushort.MaxValue));
                    v128 min2 = Xse.set1_epi16(unchecked((short)ushort.MaxValue));
                    v128 min3 = Xse.set1_epi16(unchecked((short)ushort.MaxValue));

                    do
                    {
                        min0 = Xse.min_epu16(min0, Xse.loadu_si128(ptr_v128++));
                        min1 = Xse.min_epu16(min1, Xse.loadu_si128(ptr_v128++));
                        min2 = Xse.min_epu16(min2, Xse.loadu_si128(ptr_v128++));
                        min3 = Xse.min_epu16(min3, Xse.loadu_si128(ptr_v128++));

                        length -= 32;
                    }
                    while (Hint.Likely(length >= 32));

                    min0 = Xse.min_epu16(min0, min1);
                    min2 = Xse.min_epu16(min2, min3);
                    min0 = Xse.min_epu16(min0, min2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    min0 = Xse.min_epu16(min0, Xse.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        min0 = Xse.min_epu16(min0, Xse.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            min0 = Xse.min_epu16(min0, Xse.loadu_si128(ptr_v128++));
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
                min0 = Xse.min_epu16(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    min0 = Xse.min_epu16(min0, Xse.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 4;
                }

                min0 = Xse.min_epu16(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    min0 = Xse.min_epu16(min0, Xse.cvtsi32_si128(*(int*)ptr_v128));

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 2;
                }

                min0 = Xse.min_epu16(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    if (BurstArchitecture.IsMinMaxSupported)
                    {
                        return Xse.min_epu16(min0, Xse.insert_epi16(cmp, *(ushort*)ptr_v128, 0)).UShort0;
                    }
                    else
                    {
                        return (ushort)math.min((uint)min0.UShort0, *(ushort*)ptr_v128);
                    }
                }
                else
                {
                    return min0.UShort0;
                }
            }
            else
            {
                ushort x = ushort.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    x = (ushort)math.min((uint)x, (uint)ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Minimum(this NativeArray<ushort> array)
        {
            return SIMD_Minimum((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Minimum(this NativeArray<ushort> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Minimum(this NativeArray<ushort> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Minimum(this NativeList<ushort> array)
        {
            return SIMD_Minimum((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Minimum(this NativeList<ushort> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Minimum(this NativeList<ushort> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Minimum(this NativeSlice<ushort> array)
        {
            return SIMD_Minimum((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Minimum(this NativeSlice<ushort> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Minimum(this NativeSlice<ushort> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Minimum(uint* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 min0 = Avx.mm256_set1_epi32(unchecked((int)uint.MaxValue));

                if (Hint.Likely(length >= 32))
                {
                    v256 min1 = Avx.mm256_set1_epi32(unchecked((int)uint.MaxValue));
                    v256 min2 = Avx.mm256_set1_epi32(unchecked((int)uint.MaxValue));
                    v256 min3 = Avx.mm256_set1_epi32(unchecked((int)uint.MaxValue));

                    do
                    {
                        min0 = Avx2.mm256_min_epu32(min0, Avx.mm256_loadu_si256(ptr_v256++));
                        min1 = Avx2.mm256_min_epu32(min1, Avx.mm256_loadu_si256(ptr_v256++));
                        min2 = Avx2.mm256_min_epu32(min2, Avx.mm256_loadu_si256(ptr_v256++));
                        min3 = Avx2.mm256_min_epu32(min3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 32;
                    }
                    while (Hint.Likely(length >= 32));

                    min0 = Avx2.mm256_min_epu32(min0, min1);
                    min2 = Avx2.mm256_min_epu32(min2, min3);
                    min0 = Avx2.mm256_min_epu32(min0, min2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    min0 = Avx2.mm256_min_epu32(min0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        min0 = Avx2.mm256_min_epu32(min0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            min0 = Avx2.mm256_min_epu32(min0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 min128 = Xse.min_epu32(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 4))
                {
                    min128 = Xse.min_epu32(min128, Xse.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 4;
                }

                min128 = Xse.min_epu32(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    min128 = Xse.min_epu32(min128, Xse.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 2;
                }

                min128 = Xse.min_epu32(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Xse.min_epu32(min128, Xse.cvtsi32_si128(*(int*)ptr_v256)).UInt0;
                }
                else
                {
                    return min128.UInt0;
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 min0 = Xse.set1_epi32(unchecked((int)uint.MaxValue));

                if (Hint.Likely(length >= 16))
                {
                    v128 min1 = Xse.set1_epi32(unchecked((int)uint.MaxValue));
                    v128 min2 = Xse.set1_epi32(unchecked((int)uint.MaxValue));
                    v128 min3 = Xse.set1_epi32(unchecked((int)uint.MaxValue));

                    do
                    {
                        min0 = Xse.min_epu32(min0, Xse.loadu_si128(ptr_v128++));
                        min1 = Xse.min_epu32(min1, Xse.loadu_si128(ptr_v128++));
                        min2 = Xse.min_epu32(min2, Xse.loadu_si128(ptr_v128++));
                        min3 = Xse.min_epu32(min3, Xse.loadu_si128(ptr_v128++));

                        length -= 16;
                    }
                    while (Hint.Likely(length >= 16));

                    min0 = Xse.min_epu32(min0, min1);
                    min2 = Xse.min_epu32(min2, min3);
                    min0 = Xse.min_epu32(min0, min2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    min0 = Xse.min_epu32(min0, Xse.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        min0 = Xse.min_epu32(min0, Xse.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            min0 = Xse.min_epu32(min0, Xse.loadu_si128(ptr_v128++));
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

                min0 = Xse.min_epu32(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    min0 = Xse.min_epu32(min0, Xse.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 2;
                }

                min0 = Xse.min_epu32(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    if (BurstArchitecture.IsMinMaxSupported)
                    {
                        return Xse.min_epu32(min0, Xse.cvtsi32_si128(*(int*)ptr_v128)).UInt0;
                    }
                    else
                    {
                        return math.min(min0.UInt0, *(uint*)ptr_v128);
                    }
                }
                else
                {
                    return min0.UInt0;
                }
            }
            else
            {
                uint x = uint.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    x = math.min(x, ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Minimum(this NativeArray<uint> array)
        {
            return SIMD_Minimum((uint*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Minimum(this NativeArray<uint> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Minimum(this NativeArray<uint> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Minimum(this NativeList<uint> array)
        {
            return SIMD_Minimum((uint*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Minimum(this NativeList<uint> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Minimum(this NativeList<uint> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Minimum(this NativeSlice<uint> array)
        {
            return SIMD_Minimum((uint*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Minimum(this NativeSlice<uint> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Minimum(this NativeSlice<uint> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Minimum(ulong* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 min0 = Avx.mm256_set1_epi64x(unchecked((long)ulong.MaxValue));

                if (Hint.Likely(length >= 16))
                {
                    v256 min1 = Avx.mm256_set1_epi64x(unchecked((long)ulong.MaxValue));
                    v256 min2 = Avx.mm256_set1_epi64x(unchecked((long)ulong.MaxValue));
                    v256 min3 = Avx.mm256_set1_epi64x(unchecked((long)ulong.MaxValue));

                    do
                    {
                        min0 = Xse.mm256_min_epu64(min0, Avx.mm256_loadu_si256(ptr_v256++));
                        min1 = Xse.mm256_min_epu64(min1, Avx.mm256_loadu_si256(ptr_v256++));
                        min2 = Xse.mm256_min_epu64(min2, Avx.mm256_loadu_si256(ptr_v256++));
                        min3 = Xse.mm256_min_epu64(min3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 16;
                    }
                    while (Hint.Likely(length >= 16));

                    min0 = Xse.mm256_min_epu64(min0, min1);
                    min2 = Xse.mm256_min_epu64(min2, min3);
                    min0 = Xse.mm256_min_epu64(min0, min2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    min0 = Xse.mm256_min_epu64(min0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        min0 = Xse.mm256_min_epu64(min0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            min0 = Xse.mm256_min_epu64(min0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 min128 = Xse.min_epu64(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 2))
                {
                    min128 = Xse.min_epu64(min128, Xse.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 2;
                }

                if (Hint.Likely(length != 0))
                {
                    return math.min(*(ulong*)ptr_v256, math.min(min128.ULong0, min128.ULong1));
                }
                else
                {
                    return math.min(min128.ULong0, min128.ULong1);
                }
            }
            else if (BurstArchitecture.IsCMP64Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 min0 = Xse.set1_epi64x(unchecked((long)ulong.MaxValue));

                if (Hint.Likely(length >= 8))
                {
                    v128 min1 = Xse.set1_epi64x(unchecked((long)ulong.MaxValue));
                    v128 min2 = Xse.set1_epi64x(unchecked((long)ulong.MaxValue));
                    v128 min3 = Xse.set1_epi64x(unchecked((long)ulong.MaxValue));

                    do
                    {
                        min0 = Xse.min_epu64(min0, Xse.loadu_si128(ptr_v128++));
                        min1 = Xse.min_epu64(min1, Xse.loadu_si128(ptr_v128++));
                        min2 = Xse.min_epu64(min2, Xse.loadu_si128(ptr_v128++));
                        min3 = Xse.min_epu64(min3, Xse.loadu_si128(ptr_v128++));

                        length -= 8;
                    }
                    while (Hint.Likely(length >= 8));

                    min0 = Xse.min_epu64(min0, min1);
                    min2 = Xse.min_epu64(min2, min3);
                    min0 = Xse.min_epu64(min0, min2);
                }

                if (Hint.Likely((int)length >= 2))
                {
                    min0 = Xse.min_epu64(min0, Xse.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        min0 = Xse.min_epu64(min0, Xse.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            min0 = Xse.min_epu64(min0, Xse.loadu_si128(ptr_v128++));
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

                if (Hint.Likely(length != 0))
                {
                    return math.min(*(ulong*)ptr_v128, math.min(min0.ULong0, min0.ULong1));
                }
                else
                {
                    return math.min(min0.ULong0, min0.ULong1);
                }
            }
            else
            {
                ulong x = ulong.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    x = math.min(x, ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Minimum(this NativeArray<ulong> array)
        {
            return SIMD_Minimum((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Minimum(this NativeArray<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Minimum(this NativeArray<ulong> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Minimum(this NativeList<ulong> array)
        {
            return SIMD_Minimum((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Minimum(this NativeList<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Minimum(this NativeList<ulong> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Minimum(this NativeSlice<ulong> array)
        {
            return SIMD_Minimum((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Minimum(this NativeSlice<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Minimum(this NativeSlice<ulong> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Minimum(sbyte* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 min0 = Avx.mm256_set1_epi8((byte)sbyte.MaxValue);

                if (Hint.Likely(length >= 128))
                {
                    v256 min1 = Avx.mm256_set1_epi8((byte)sbyte.MaxValue);
                    v256 min2 = Avx.mm256_set1_epi8((byte)sbyte.MaxValue);
                    v256 min3 = Avx.mm256_set1_epi8((byte)sbyte.MaxValue);

                    do
                    {
                        min0 = Avx2.mm256_min_epi8(min0, Avx.mm256_loadu_si256(ptr_v256++));
                        min1 = Avx2.mm256_min_epi8(min1, Avx.mm256_loadu_si256(ptr_v256++));
                        min2 = Avx2.mm256_min_epi8(min2, Avx.mm256_loadu_si256(ptr_v256++));
                        min3 = Avx2.mm256_min_epi8(min3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 128;
                    }
                    while (Hint.Likely(length >= 128));

                    min0 = Avx2.mm256_min_epi8(min0, min1);
                    min2 = Avx2.mm256_min_epi8(min2, min3);
                    min0 = Avx2.mm256_min_epi8(min0, min2);
                }

                if (Hint.Likely((int)length >= 32))
                {
                    min0 = Avx2.mm256_min_epi8(min0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 32))
                    {
                        min0 = Avx2.mm256_min_epi8(min0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 32))
                        {
                            min0 = Avx2.mm256_min_epi8(min0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 min128 = Xse.min_epi8(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 16))
                {
                    min128 = Xse.min_epi8(min128, Xse.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 16;
                }

                v128 cmp = Xse.setzero_si128();
                min128 = Xse.min_epi8(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    min128 = Xse.min_epi8(min128, Xse.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 8;
                }

                min128 = Xse.min_epi8(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    min128 = Xse.min_epi8(min128, Xse.cvtsi32_si128(*(int*)ptr_v256));
                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 4;
                }

                min128 = Xse.min_epi8(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    min128 = Xse.min_epi8(min128, Xse.insert_epi16(cmp, *(ushort*)ptr_v256, 0));
                    ptr_v256 = (v256*)((short*)ptr_v256 + 1);
                    length -= 2;
                }

                min128 = Xse.min_epi8(min128, Xse.bsrli_si128(min128, 1 * sizeof(sbyte)));

                if (Hint.Likely(length != 0))
                {
                    return Xse.min_epi8(min128, Xse.insert_epi8(cmp, *(byte*)ptr_v256, 0)).SByte0;
                }
                else
                {
                    return min128.SByte0;
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 min0 = Xse.set1_epi8(sbyte.MaxValue);

                if (Hint.Likely(length >= 64))
                {
                    v128 min1 = Xse.set1_epi8(sbyte.MaxValue);
                    v128 min2 = Xse.set1_epi8(sbyte.MaxValue);
                    v128 min3 = Xse.set1_epi8(sbyte.MaxValue);

                    do
                    {
                        min0 = Xse.min_epi8(min0, Xse.loadu_si128(ptr_v128++));
                        min1 = Xse.min_epi8(min1, Xse.loadu_si128(ptr_v128++));
                        min2 = Xse.min_epi8(min2, Xse.loadu_si128(ptr_v128++));
                        min3 = Xse.min_epi8(min3, Xse.loadu_si128(ptr_v128++));

                        length -= 64;
                    }
                    while (Hint.Likely(length >= 64));

                    min0 = Xse.min_epi8(min0, min1);
                    min2 = Xse.min_epi8(min2, min3);
                    min0 = Xse.min_epi8(min0, min2);
                }

                if (Hint.Likely((int)length >= 16))
                {
                    min0 = Xse.min_epi8(min0, Xse.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        min0 = Xse.min_epi8(min0, Xse.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            min0 = Xse.min_epi8(min0, Xse.loadu_si128(ptr_v128++));
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
                min0 = Xse.min_epi8(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    min0 = Xse.min_epi8(min0, Xse.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 8;
                }

                min0 = Xse.min_epi8(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    min0 = Xse.min_epi8(min0, Xse.cvtsi32_si128(*(int*)ptr_v128));

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 4;
                }

                min0 = Xse.min_epi8(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    min0 = Xse.min_epi8(min0, Xse.insert_epi16(cmp, *(ushort*)ptr_v128, 0));
                    ptr_v128 = (v128*)((short*)ptr_v128 + 1);
                    length -= 2;
                }

                min0 = Xse.min_epi8(min0, Xse.bsrli_si128(min0, 1 * sizeof(sbyte)));

                if (Hint.Likely(length != 0))
                {
                    if (BurstArchitecture.IsMinMaxSupported)
                    {
                        return Xse.min_epi8(min0, Xse.insert_epi8(cmp, *(byte*)ptr_v128, 0)).SByte0;
                    }
                    else
                    {
                        return (sbyte)math.min((int)min0.SByte0, (int)(*(sbyte*)ptr_v128));
                    }
                }
                else
                {
                    return min0.SByte0;
                }
            }
            else
            {
                sbyte x = sbyte.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    x = (sbyte)math.min((int)x, (int)ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Minimum(this NativeArray<sbyte> array)
        {
            return SIMD_Minimum((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Minimum(this NativeArray<sbyte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Minimum(this NativeArray<sbyte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Minimum(this NativeList<sbyte> array)
        {
            return SIMD_Minimum((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Minimum(this NativeList<sbyte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Minimum(this NativeList<sbyte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Minimum(this NativeSlice<sbyte> array)
        {
            return SIMD_Minimum((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Minimum(this NativeSlice<sbyte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Minimum(this NativeSlice<sbyte> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Minimum(short* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 min0 = Avx.mm256_set1_epi16(short.MaxValue);

                if (Hint.Likely(length >= 64))
                {
                    v256 min1 = Avx.mm256_set1_epi16(short.MaxValue);
                    v256 min2 = Avx.mm256_set1_epi16(short.MaxValue);
                    v256 min3 = Avx.mm256_set1_epi16(short.MaxValue);

                    do
                    {
                        min0 = Avx2.mm256_min_epi16(min0, Avx.mm256_loadu_si256(ptr_v256++));
                        min1 = Avx2.mm256_min_epi16(min1, Avx.mm256_loadu_si256(ptr_v256++));
                        min2 = Avx2.mm256_min_epi16(min2, Avx.mm256_loadu_si256(ptr_v256++));
                        min3 = Avx2.mm256_min_epi16(min3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 64;
                    }
                    while (Hint.Likely(length >= 64));

                    min0 = Avx2.mm256_min_epi16(min0, min1);
                    min2 = Avx2.mm256_min_epi16(min2, min3);
                    min0 = Avx2.mm256_min_epi16(min0, min2);
                }

                if (Hint.Likely((int)length >= 16))
                {
                    min0 = Avx2.mm256_min_epi16(min0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        min0 = Avx2.mm256_min_epi16(min0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            min0 = Avx2.mm256_min_epi16(min0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 min128 = Xse.min_epi16(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 8))
                {
                    min128 = Xse.min_epi16(min128, Xse.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 8;
                }

                v128 cmp = Xse.setzero_si128();
                min128 = Xse.min_epi16(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    min128 = Xse.min_epi16(min128, Xse.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 4;
                }

                min128 = Xse.min_epi16(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    min128 = Xse.min_epi16(min128, Xse.cvtsi32_si128(*(int*)ptr_v256));
                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 2;
                }

                min128 = Xse.min_epi16(min128, Xse.shufflelo_epi16(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Xse.min_epi16(min128, Xse.insert_epi16(cmp, *(ushort*)ptr_v256, 0)).SShort0;
                }
                else
                {
                    return min128.SShort0;
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 min0 = Xse.set1_epi16(short.MaxValue);

                if (Hint.Likely(length >= 32))
                {
                    v128 min1 = Xse.set1_epi16(short.MaxValue);
                    v128 min2 = Xse.set1_epi16(short.MaxValue);
                    v128 min3 = Xse.set1_epi16(short.MaxValue);

                    do
                    {
                        min0 = Xse.min_epi16(min0, Xse.loadu_si128(ptr_v128++));
                        min1 = Xse.min_epi16(min1, Xse.loadu_si128(ptr_v128++));
                        min2 = Xse.min_epi16(min2, Xse.loadu_si128(ptr_v128++));
                        min3 = Xse.min_epi16(min3, Xse.loadu_si128(ptr_v128++));

                        length -= 32;
                    }
                    while (Hint.Likely(length >= 32));

                    min0 = Xse.min_epi16(min0, min1);
                    min2 = Xse.min_epi16(min2, min3);
                    min0 = Xse.min_epi16(min0, min2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    min0 = Xse.min_epi16(min0, Xse.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        min0 = Xse.min_epi16(min0, Xse.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            min0 = Xse.min_epi16(min0, Xse.loadu_si128(ptr_v128++));
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
                min0 = Xse.min_epi16(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    min0 = Xse.min_epi16(min0, Xse.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 4;
                }

                min0 = Xse.min_epi16(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    min0 = Xse.min_epi16(min0, Xse.cvtsi32_si128(*(int*)ptr_v128));

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 2;
                }

                min0 = Xse.min_epi16(min0, Xse.shufflelo_epi16(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Xse.min_epi16(min0, Xse.insert_epi16(cmp, *(ushort*)ptr_v128, 0)).SShort0;
                }
                else
                {
                    return min0.SShort0;
                }
            }
            else
            {
                short x = short.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    x = (short)math.min((int)x, (int)ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Minimum(this NativeArray<short> array)
        {
            return SIMD_Minimum((short*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Minimum(this NativeArray<short> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Minimum(this NativeArray<short> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Minimum(this NativeList<short> array)
        {
            return SIMD_Minimum((short*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Minimum(this NativeList<short> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Minimum(this NativeList<short> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Minimum(this NativeSlice<short> array)
        {
            return SIMD_Minimum((short*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Minimum(this NativeSlice<short> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Minimum(this NativeSlice<short> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Minimum(int* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 min0 = Avx.mm256_set1_epi32(int.MaxValue);

                if (Hint.Likely(length >= 32))
                {
                    v256 min1 = Avx.mm256_set1_epi32(int.MaxValue);
                    v256 min2 = Avx.mm256_set1_epi32(int.MaxValue);
                    v256 min3 = Avx.mm256_set1_epi32(int.MaxValue);

                    do
                    {
                        min0 = Avx2.mm256_min_epi32(min0, Avx.mm256_loadu_si256(ptr_v256++));
                        min1 = Avx2.mm256_min_epi32(min1, Avx.mm256_loadu_si256(ptr_v256++));
                        min2 = Avx2.mm256_min_epi32(min2, Avx.mm256_loadu_si256(ptr_v256++));
                        min3 = Avx2.mm256_min_epi32(min3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 32;
                    }
                    while (Hint.Likely(length >= 32));

                    min0 = Avx2.mm256_min_epi32(min0, min1);
                    min2 = Avx2.mm256_min_epi32(min2, min3);
                    min0 = Avx2.mm256_min_epi32(min0, min2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    min0 = Avx2.mm256_min_epi32(min0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        min0 = Avx2.mm256_min_epi32(min0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            min0 = Avx2.mm256_min_epi32(min0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 min128 = Xse.min_epi32(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 4))
                {
                    min128 = Xse.min_epi32(min128, Xse.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 4;
                }

                min128 = Xse.min_epi32(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    min128 = Xse.min_epi32(min128, Xse.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 2;
                }

                min128 = Xse.min_epi32(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Xse.min_epi32(min128, Xse.cvtsi32_si128(*(int*)ptr_v256)).SInt0;
                }
                else
                {
                    return min128.SInt0;
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 min0 = Xse.set1_epi32(int.MaxValue);

                if (Hint.Likely(length >= 16))
                {
                    v128 min1 = Xse.set1_epi32(int.MaxValue);
                    v128 min2 = Xse.set1_epi32(int.MaxValue);
                    v128 min3 = Xse.set1_epi32(int.MaxValue);

                    do
                    {
                        min0 = Xse.min_epi32(min0, Xse.loadu_si128(ptr_v128++));
                        min1 = Xse.min_epi32(min1, Xse.loadu_si128(ptr_v128++));
                        min2 = Xse.min_epi32(min2, Xse.loadu_si128(ptr_v128++));
                        min3 = Xse.min_epi32(min3, Xse.loadu_si128(ptr_v128++));

                        length -= 16;
                    }
                    while (Hint.Likely(length >= 16));

                    min0 = Xse.min_epi32(min0, min1);
                    min2 = Xse.min_epi32(min2, min3);
                    min0 = Xse.min_epi32(min0, min2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    min0 = Xse.min_epi32(min0, Xse.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        min0 = Xse.min_epi32(min0, Xse.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            min0 = Xse.min_epi32(min0, Xse.loadu_si128(ptr_v128++));
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

                min0 = Xse.min_epi32(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    min0 = Xse.min_epi32(min0, Xse.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 2;
                }

                min0 = Xse.min_epi32(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    if (BurstArchitecture.IsMinMaxSupported)
                    {
                        return Xse.min_epi32(min0, Xse.cvtsi32_si128(*(int*)ptr_v128)).SInt0;
                    }
                    else
                    {
                        return math.min(min0.SInt0, *(int*)ptr_v128);
                    }
                }
                else
                {
                    return min0.SInt0;
                }
            }
            else
            {
                int x = int.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    x = math.min(x, ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Minimum(this NativeArray<int> array)
        {
            return SIMD_Minimum((int*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Minimum(this NativeArray<int> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Minimum(this NativeArray<int> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Minimum(this NativeList<int> array)
        {
            return SIMD_Minimum((int*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Minimum(this NativeList<int> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Minimum(this NativeList<int> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Minimum(this NativeSlice<int> array)
        {
            return SIMD_Minimum((int*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Minimum(this NativeSlice<int> array, int index)
        {
            return SIMD_Minimum((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Minimum(this NativeSlice<int> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Minimum(long* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 min0 = Avx.mm256_set1_epi64x(long.MaxValue);

                if (Hint.Likely(length >= 16))
                {
                    v256 min1 = Avx.mm256_set1_epi64x(long.MaxValue);
                    v256 min2 = Avx.mm256_set1_epi64x(long.MaxValue);
                    v256 min3 = Avx.mm256_set1_epi64x(long.MaxValue);

                    do
                    {
                        min0 =  Xse.mm256_min_epi64(min0, Avx.mm256_loadu_si256(ptr_v256++));
                        min1 =  Xse.mm256_min_epi64(min1, Avx.mm256_loadu_si256(ptr_v256++));
                        min2 =  Xse.mm256_min_epi64(min2, Avx.mm256_loadu_si256(ptr_v256++));
                        min3 =  Xse.mm256_min_epi64(min3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 16;
                    }
                    while (Hint.Likely(length >= 16));

                    min0 =  Xse.mm256_min_epi64(min0, min1);
                    min2 =  Xse.mm256_min_epi64(min2, min3);
                    min0 =  Xse.mm256_min_epi64(min0, min2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    min0 =  Xse.mm256_min_epi64(min0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        min0 =  Xse.mm256_min_epi64(min0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            min0 =  Xse.mm256_min_epi64(min0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 min128 = Xse.min_epi64(Avx.mm256_castsi256_si128(min0), Avx2.mm256_extracti128_si256(min0, 1));

                if (Hint.Likely((int)length >= 2))
                {
                    min128 = Xse.min_epi64(min128, Xse.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 2;
                }

                min128 = Xse.min_epi64(min128, Xse.shuffle_epi32(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    return math.min(min128.SLong0, *(long*)ptr_v256);
                }
                else
                {
                    return min128.SLong0;
                }
            }
            else if (BurstArchitecture.IsCMP64Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 min0 = Xse.set1_epi64x(long.MaxValue);

                if (Hint.Likely(length >= 8))
                {
                    v128 min1 = Xse.set1_epi64x(long.MaxValue);
                    v128 min2 = Xse.set1_epi64x(long.MaxValue);
                    v128 min3 = Xse.set1_epi64x(long.MaxValue);

                    do
                    {
                        min0 = Xse.min_epi64(min0, Xse.loadu_si128(ptr_v128++));
                        min1 = Xse.min_epi64(min1, Xse.loadu_si128(ptr_v128++));
                        min2 = Xse.min_epi64(min2, Xse.loadu_si128(ptr_v128++));
                        min3 = Xse.min_epi64(min3, Xse.loadu_si128(ptr_v128++));

                        length -= 8;
                    }
                    while (Hint.Likely(length >= 8));

                    min0 = Xse.min_epi64(min0, min1);
                    min2 = Xse.min_epi64(min2, min3);
                    min0 = Xse.min_epi64(min0, min2);
                }

                if (Hint.Likely((int)length >= 2))
                {
                    min0 = Xse.min_epi64(min0, Xse.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        min0 = Xse.min_epi64(min0, Xse.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            min0 = Xse.min_epi64(min0, Xse.loadu_si128(ptr_v128++));
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

                min0 = Xse.min_epi64(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    return math.min(min0.SLong0, *(long*)ptr_v128);
                }
                else
                {
                    return min0.SLong0;
                }
            }
            else
            {
                long x = long.MaxValue;

                for (long i = 0; i < length; i++)
                {
                    x = math.min(x, ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Minimum(this NativeArray<long> array)
        {
            return SIMD_Minimum((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Minimum(this NativeArray<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Minimum(this NativeArray<long> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Minimum(this NativeList<long> array)
        {
            return SIMD_Minimum((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Minimum(this NativeList<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Minimum(this NativeList<long> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Minimum(this NativeSlice<long> array)
        {
            return SIMD_Minimum((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Minimum(this NativeSlice<long> array, int index)
        {
            return SIMD_Minimum((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Minimum(this NativeSlice<long> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Minimum(float* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx.IsAvxSupported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 min0 = new v256(float.PositiveInfinity);

                if (Hint.Likely(length >= 32))
                {
                    v256 min1 = new v256(float.PositiveInfinity);
                    v256 min2 = new v256(float.PositiveInfinity);
                    v256 min3 = new v256(float.PositiveInfinity);

                    do
                    {
                        min0 = Avx.mm256_min_ps(min0, Avx.mm256_loadu_ps(ptr_v256++));
                        min1 = Avx.mm256_min_ps(min1, Avx.mm256_loadu_ps(ptr_v256++));
                        min2 = Avx.mm256_min_ps(min2, Avx.mm256_loadu_ps(ptr_v256++));
                        min3 = Avx.mm256_min_ps(min3, Avx.mm256_loadu_ps(ptr_v256++));

                        length -= 32;
                    }
                    while (Hint.Likely(length >= 32));

                    min0 = Avx.mm256_min_ps(min0, min1);
                    min2 = Avx.mm256_min_ps(min2, min3);
                    min0 = Avx.mm256_min_ps(min0, min2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    min0 = Avx.mm256_min_ps(min0, Avx.mm256_loadu_ps(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        min0 = Avx.mm256_min_ps(min0, Avx.mm256_loadu_ps(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            min0 = Avx.mm256_min_ps(min0, Avx.mm256_loadu_ps(ptr_v256++));
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

                v128 min128 = Xse.min_ps(Avx.mm256_castps256_ps128(min0), Avx.mm256_extractf128_ps(min0, 1));

                if (Hint.Likely((int)length >= 4))
                {
                    min128 = Xse.min_ps(min128, *(v128*)ptr_v256);
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 4;
                }

                min128 = Xse.min_ps(min128, Avx.permute_ps(min128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    min128 = Xse.min_ps(min128, Xse.cvtsi64x_si128(*(long*)ptr_v256));

                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 2;
                }

                min128 = Xse.min_ps(min128, Avx.permute_ps(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    min128 = Xse.min_ss(min128, Xse.cvtsi32_si128(*(int*)ptr_v256));
                }

                return min128.Float0;
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 min0 = new v128(float.PositiveInfinity);

                if (Hint.Likely(length >= 16))
                {
                    v128 min1 = new v128(float.PositiveInfinity);
                    v128 min2 = new v128(float.PositiveInfinity);
                    v128 min3 = new v128(float.PositiveInfinity);

                    do
                    {
                        min0 = Xse.min_ps(min0, *ptr_v128++);
                        min1 = Xse.min_ps(min1, *ptr_v128++);
                        min2 = Xse.min_ps(min2, *ptr_v128++);
                        min3 = Xse.min_ps(min3, *ptr_v128++);

                        length -= 16;
                    }
                    while (Hint.Likely(length >= 16));

                    min0 = Xse.min_ps(min0, min1);
                    min2 = Xse.min_ps(min2, min3);
                    min0 = Xse.min_ps(min0, min2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    min0 = Xse.min_ps(min0, *ptr_v128++);

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        min0 = Xse.min_ps(min0, *ptr_v128++);

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            min0 = Xse.min_ps(min0, *ptr_v128++);
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

                min0 = Xse.min_ps(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    min0 = Xse.min_ps(min0, Xse.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 2;
                }

                min0 = Xse.min_ps(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    min0 = Xse.min_ss(min0, Xse.cvtsi32_si128(*(int*)ptr_v128));
                }

                return min0.Float0;
            }
            else
            {
                float x = float.PositiveInfinity;

                for (long i = 0; i < length; i++)
                {
                    x = math.min(x, ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Minimum(this NativeArray<float> array)
        {
            return SIMD_Minimum((float*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Minimum(this NativeArray<float> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Minimum(this NativeArray<float> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Minimum(this NativeList<float> array)
        {
            return SIMD_Minimum((float*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Minimum(this NativeList<float> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Minimum(this NativeList<float> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Minimum(this NativeSlice<float> array)
        {
            return SIMD_Minimum((float*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Minimum(this NativeSlice<float> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Minimum(this NativeSlice<float> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Minimum(double* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx.IsAvxSupported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 min0 = new v256(double.PositiveInfinity);

                if (Hint.Likely(length >= 16))
                {
                    v256 min1 = new v256(double.PositiveInfinity);
                    v256 min2 = new v256(double.PositiveInfinity);
                    v256 min3 = new v256(double.PositiveInfinity);

                    do
                    {
                        min0 = Avx.mm256_min_pd(min0, Avx.mm256_loadu_pd(ptr_v256++));
                        min1 = Avx.mm256_min_pd(min1, Avx.mm256_loadu_pd(ptr_v256++));
                        min2 = Avx.mm256_min_pd(min2, Avx.mm256_loadu_pd(ptr_v256++));
                        min3 = Avx.mm256_min_pd(min3, Avx.mm256_loadu_pd(ptr_v256++));

                        length -= 16;
                    }
                    while (Hint.Likely(length >= 16));

                    min0 = Avx.mm256_min_pd(min0, min1);
                    min2 = Avx.mm256_min_pd(min2, min3);
                    min0 = Avx.mm256_min_pd(min0, min2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    min0 = Avx.mm256_min_pd(min0, Avx.mm256_loadu_pd(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        min0 = Avx.mm256_min_pd(min0, Avx.mm256_loadu_pd(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            min0 = Avx.mm256_min_pd(min0, Avx.mm256_loadu_pd(ptr_v256++));
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

                v128 min128 = Xse.min_pd(Avx.mm256_castpd256_pd128(min0), Avx.mm256_extractf128_pd(min0, 1));

                if (Hint.Likely((int)length >= 2))
                {
                    min128 = Xse.min_pd(min128, *(v128*)ptr_v256);
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 2;
                }

                min128 = Xse.min_pd(min128, Avx.permute_pd(min128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    min128 = Xse.min_sd(min128, Xse.cvtsi64x_si128(*(long*)ptr_v256));
                }

                return min128.Double0;
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 min0 = new v128(double.PositiveInfinity);

                if (Hint.Likely(length >= 8))
                {
                    v128 min1 = new v128(double.PositiveInfinity);
                    v128 min2 = new v128(double.PositiveInfinity);
                    v128 min3 = new v128(double.PositiveInfinity);

                    do
                    {
                        min0 = Xse.min_pd(min0, *ptr_v128++);
                        min1 = Xse.min_pd(min1, *ptr_v128++);
                        min2 = Xse.min_pd(min2, *ptr_v128++);
                        min3 = Xse.min_pd(min3, *ptr_v128++);

                        length -= 8;
                    }
                    while (Hint.Likely(length >= 8));

                    min0 = Xse.min_pd(min0, min1);
                    min2 = Xse.min_pd(min2, min3);
                    min0 = Xse.min_pd(min0, min2);
                }

                if (Hint.Likely((int)length >= 2))
                {
                    min0 = Xse.min_pd(min0, *ptr_v128++);

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        min0 = Xse.min_pd(min0, *ptr_v128++);

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            min0 = Xse.min_pd(min0, *ptr_v128++);
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

                min0 = Xse.min_pd(min0, Xse.shuffle_epi32(min0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    min0 = Xse.min_sd(min0, Xse.cvtsi64x_si128(*(long*)ptr_v128));
                }

                return min0.Double0;
            }
            else
            {
                double x = double.PositiveInfinity;

                for (long i = 0; i < length; i++)
                {
                    x = math.min(x, ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Minimum(this NativeArray<double> array)
        {
            return SIMD_Minimum((double*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Minimum(this NativeArray<double> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Minimum(this NativeArray<double> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Minimum(this NativeList<double> array)
        {
            return SIMD_Minimum((double*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Minimum(this NativeList<double> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Minimum(this NativeList<double> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Minimum(this NativeSlice<double> array)
        {
            return SIMD_Minimum((double*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Minimum(this NativeSlice<double> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Minimum((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Minimum(this NativeSlice<double> array, int index, int numEntries)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Minimum((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }
    }
}