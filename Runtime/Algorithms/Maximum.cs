using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst.Intrinsics;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using DevTools;

using static Unity.Burst.Intrinsics.X86;

namespace SIMDAlgorithms
{
    unsafe public static partial class Algorithms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Maximum(byte* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 max0 = Avx.mm256_set1_epi8(byte.MinValue);
                
                if (Hint.Likely(length >= 128))
                {
                    v256 max1 = Avx.mm256_set1_epi8(byte.MinValue);
                    v256 max2 = Avx.mm256_set1_epi8(byte.MinValue);
                    v256 max3 = Avx.mm256_set1_epi8(byte.MinValue);

                    do
                    {
                        max0 = Avx2.mm256_max_epu8(max0, Avx.mm256_loadu_si256(ptr_v256++));
                        max1 = Avx2.mm256_max_epu8(max1, Avx.mm256_loadu_si256(ptr_v256++));
                        max2 = Avx2.mm256_max_epu8(max2, Avx.mm256_loadu_si256(ptr_v256++));
                        max3 = Avx2.mm256_max_epu8(max3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 128;
                    } 
                    while (Hint.Likely(length >= 128));

                    max0 = Avx2.mm256_max_epu8(max0, max1);
                    max2 = Avx2.mm256_max_epu8(max2, max3);
                    max0 = Avx2.mm256_max_epu8(max0, max2);
                }

                if (Hint.Likely((int)length >= 32))
                {
                    max0 = Avx2.mm256_max_epu8(max0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 32))
                    {
                        max0 = Avx2.mm256_max_epu8(max0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 32))
                        {
                            max0 = Avx2.mm256_max_epu8(max0, Avx.mm256_loadu_si256(ptr_v256++));
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
                else { }

                v128 max128 = Sse2.max_epu8(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));

                if (Hint.Likely((int)length >= 16))
                {
                    max128 = Sse2.max_epu8(max128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 16;
                }

                v128 cmp = default(v128);
                max128 = Sse2.max_epu8(max128, Sse2.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    max128 = Sse2.max_epu8(max128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 8;
                }

                max128 = Sse2.max_epu8(max128, Sse2.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    max128 = Sse2.max_epu8(max128, Sse2.cvtsi32_si128(*(int*)ptr_v256));
                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 4;
                }

                max128 = Sse2.max_epu8(max128, Sse2.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    max128 = Sse2.max_epu8(max128, Sse2.insert_epi16(cmp, *(short*)ptr_v256, 0));
                    ptr_v256 = (v256*)((short*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Sse2.max_epu8(max128, Sse2.bsrli_si128(max128, 1 * sizeof(byte)));

                if (Hint.Likely(length != 0))
                {
                    return Sse2.max_epu8(max128, Sse4_1.insert_epi8(cmp, *(byte*)ptr_v256, 0)).Byte0;
                }
                else
                {
                    return max128.Byte0;
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 max0 = Sse2.set1_epi8(unchecked((sbyte)byte.MinValue));
                
                if (Hint.Likely(length >= 64))
                {
                    v128 max1 = Sse2.set1_epi8(unchecked((sbyte)byte.MinValue));
                    v128 max2 = Sse2.set1_epi8(unchecked((sbyte)byte.MinValue));
                    v128 max3 = Sse2.set1_epi8(unchecked((sbyte)byte.MinValue));

                    do
                    {
                        max0 = Sse2.max_epu8(max0, Sse2.loadu_si128(ptr_v128++));
                        max1 = Sse2.max_epu8(max1, Sse2.loadu_si128(ptr_v128++));
                        max2 = Sse2.max_epu8(max2, Sse2.loadu_si128(ptr_v128++));
                        max3 = Sse2.max_epu8(max3, Sse2.loadu_si128(ptr_v128++));

                        length -= 64;
                    } 
                    while (Hint.Likely(length >= 64));

                    max0 = Sse2.max_epu8(max0, max1);
                    max2 = Sse2.max_epu8(max2, max3);
                    max0 = Sse2.max_epu8(max0, max2);
                }

                if (Hint.Likely((int)length >= 16))
                {
                    max0 = Sse2.max_epu8(max0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        max0 = Sse2.max_epu8(max0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            max0 = Sse2.max_epu8(max0, Sse2.loadu_si128(ptr_v128++));
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
                else { }

                v128 cmp = default(v128);
                max0 = Sse2.max_epu8(max0, Sse2.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    max0 = Sse2.max_epu8(max0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));
                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 8;
                }

                max0 = Sse2.max_epu8(max0, Sse2.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    max0 = Sse2.max_epu8(max0, Sse2.cvtsi32_si128(*(int*)ptr_v128));
                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 4;
                }

                max0 = Sse2.max_epu8(max0, Sse2.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    max0 = Sse2.max_epu8(max0, Sse2.insert_epi16(cmp, *(short*)ptr_v128, 0));
                    ptr_v128 = (v128*)((short*)ptr_v128 + 1);
                    length -= 2;
                }

                max0 = Sse2.max_epu8(max0, Sse2.bsrli_si128(max0, 1 * sizeof(byte)));

                if (Hint.Likely(length != 0))
                {
                    if (Sse4_1.IsSse41Supported)
                    {
                        return Sse2.max_epu8(max0, Sse4_1.insert_epi8(cmp, *(byte*)ptr_v128, 0)).Byte0;
                    }
                    else
                    {
                        return Sse2.max_epu8(max0, Sse2.cvtsi32_si128(*(byte*)ptr_v128)).Byte0;
                    }
                }
                else
                {
                    return max0.Byte0;
                }
            }
            else
            {
                byte x = byte.MinValue;

                for (long i = 0; i < length; i++)
                {
                    x = (byte)math.max((uint)x, (uint)ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Maximum(this NativeArray<byte> array)
        {
            return SIMD_Maximum((byte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Maximum(this NativeArray<byte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Maximum(this NativeArray<byte> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Maximum(this NativeList<byte> array)
        {
            return SIMD_Maximum((byte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Maximum(this NativeList<byte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Maximum(this NativeList<byte> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Maximum(this NativeSlice<byte> array)
        {
            return SIMD_Maximum((byte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Maximum(this NativeSlice<byte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SIMD_Maximum(this NativeSlice<byte> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Maximum(ushort* ptr, long length)
        {
            static v128 Max(v128 a, v128 b)
            {
                if (Sse4_1.IsSse41Supported)
                {
                    return Sse4_1.max_epu16(b, a);
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 mask = Sse2.set1_epi16(unchecked((short)(1 << 15)));

                    return Sse2.xor_si128(mask,
                                          Sse2.max_epi16(Sse2.xor_si128(a, mask),
                                                         Sse2.xor_si128(b, mask)));
                }
                else
                {
                    return default(v128);
                }
            }

Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 max0 = Avx.mm256_set1_epi16(unchecked((short)ushort.MinValue));
                
                if (Hint.Likely(length >= 64))
                {
                    v256 max1 = Avx.mm256_set1_epi16(unchecked((short)ushort.MinValue));
                    v256 max2 = Avx.mm256_set1_epi16(unchecked((short)ushort.MinValue));
                    v256 max3 = Avx.mm256_set1_epi16(unchecked((short)ushort.MinValue));

                    do
                    {
                        max0 = Avx2.mm256_max_epu16(max0, Avx.mm256_loadu_si256(ptr_v256++));
                        max1 = Avx2.mm256_max_epu16(max1, Avx.mm256_loadu_si256(ptr_v256++));
                        max2 = Avx2.mm256_max_epu16(max2, Avx.mm256_loadu_si256(ptr_v256++));
                        max3 = Avx2.mm256_max_epu16(max3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 64;
                    } 
                    while (Hint.Likely(length >= 64));

                    max0 = Avx2.mm256_max_epu16(max0, max1);
                    max2 = Avx2.mm256_max_epu16(max2, max3);
                    max0 = Avx2.mm256_max_epu16(max0, max2);
                }

                if (Hint.Likely((int)length >= 16))
                {
                    max0 = Avx2.mm256_max_epu16(max0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        max0 = Avx2.mm256_max_epu16(max0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            max0 = Avx2.mm256_max_epu16(max0, Avx.mm256_loadu_si256(ptr_v256++));
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
                else { }

                v128 max128 = Sse4_1.max_epu16(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));

                if (Hint.Likely((int)length >= 8))
                {
                    max128 = Sse4_1.max_epu16(max128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 8;
                }

                v128 cmp = default(v128);
                max128 = Sse4_1.max_epu16(max128, Sse2.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    max128 = Sse4_1.max_epu16(max128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 4;
                }

                max128 = Sse4_1.max_epu16(max128, Sse2.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    max128 = Sse4_1.max_epu16(max128, Sse2.cvtsi32_si128(*(int*)ptr_v256));
                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Sse4_1.max_epu16(max128, Sse2.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Sse4_1.max_epu16(max128, Sse2.insert_epi16(cmp, *(ushort*)ptr_v256, 0)).UShort0;
                }
                else
                {
                    return max128.UShort0;
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 max0 = Sse2.set1_epi16(unchecked((short)ushort.MinValue));
                
                if (Hint.Likely(length >= 32))
                {
                    v128 max1 = Sse2.set1_epi16(unchecked((short)ushort.MinValue));
                    v128 max2 = Sse2.set1_epi16(unchecked((short)ushort.MinValue));
                    v128 max3 = Sse2.set1_epi16(unchecked((short)ushort.MinValue));

                    do
                    {
                        max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));
                        max1 = Max(max1, Sse2.loadu_si128(ptr_v128++));
                        max2 = Max(max2, Sse2.loadu_si128(ptr_v128++));
                        max3 = Max(max3, Sse2.loadu_si128(ptr_v128++));

                        length -= 32;
                    } 
                    while (Hint.Likely(length >= 32));

                    max0 = Max(max0, max1);
                    max2 = Max(max2, max3);
                    max0 = Max(max0, max2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));
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
                else { }

                v128 cmp = default(v128);
                max0 = Max(max0, Sse2.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    max0 = Max(max0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 4;
                }

                max0 = Max(max0, Sse2.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    max0 = Max(max0, Sse2.cvtsi32_si128(*(int*)ptr_v128));

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 2;
                }

                max0 = Max(max0, Sse2.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    if (Sse4_1.IsSse41Supported)
                    {
                        return Sse4_1.max_epu16(max0, Sse2.insert_epi16(cmp, *(ushort*)ptr_v128, 0)).UShort0;
                    }
                    else
                    {
                        return (ushort)math.max((uint)max0.UShort0, *(ushort*)ptr_v128);
                    }
                }
                else
                {
                    return max0.UShort0;
                }
            }
            else
            {
                ushort x = ushort.MinValue;

                for (long i = 0; i < length; i++)
                {
                    x = (ushort)math.max((uint)x, (uint)ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Maximum(this NativeArray<ushort> array)
        {
            return SIMD_Maximum((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Maximum(this NativeArray<ushort> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Maximum(this NativeArray<ushort> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Maximum(this NativeList<ushort> array)
        {
            return SIMD_Maximum((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Maximum(this NativeList<ushort> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Maximum(this NativeList<ushort> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Maximum(this NativeSlice<ushort> array)
        {
            return SIMD_Maximum((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Maximum(this NativeSlice<ushort> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Maximum(this NativeSlice<ushort> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Maximum(uint* ptr, long length)
        {
            static v128 Max(v128 a, v128 b)
            {
                if (Sse4_1.IsSse41Supported)
                {
                    return Sse4_1.max_epu32(b, a);
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 mask = Sse2.set1_epi32(unchecked((int)(1 << 31)));

                    v128 greaterMask = Sse2.cmpgt_epi32(Sse2.xor_si128(a, mask),
                                                        Sse2.xor_si128(b, mask));

                    return Sse2.or_si128(Sse2.and_si128(greaterMask, a),
                                         Sse2.andnot_si128(greaterMask, b));
                }
                else
                {
                    return default(v128);
                }
            }

Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 max0 = Avx.mm256_set1_epi32(unchecked((int)uint.MinValue));
                
                if (Hint.Likely(length >= 32))
                {
                    v256 max1 = Avx.mm256_set1_epi32(unchecked((int)uint.MinValue));
                    v256 max2 = Avx.mm256_set1_epi32(unchecked((int)uint.MinValue));
                    v256 max3 = Avx.mm256_set1_epi32(unchecked((int)uint.MinValue));

                    do
                    {
                        max0 = Avx2.mm256_max_epu32(max0, Avx.mm256_loadu_si256(ptr_v256++));
                        max1 = Avx2.mm256_max_epu32(max1, Avx.mm256_loadu_si256(ptr_v256++));
                        max2 = Avx2.mm256_max_epu32(max2, Avx.mm256_loadu_si256(ptr_v256++));
                        max3 = Avx2.mm256_max_epu32(max3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 32;
                    } 
                    while (Hint.Likely(length >= 32));

                    max0 = Avx2.mm256_max_epu32(max0, max1);
                    max2 = Avx2.mm256_max_epu32(max2, max3);
                    max0 = Avx2.mm256_max_epu32(max0, max2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    max0 = Avx2.mm256_max_epu32(max0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        max0 = Avx2.mm256_max_epu32(max0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            max0 = Avx2.mm256_max_epu32(max0, Avx.mm256_loadu_si256(ptr_v256++));
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
                else { }

                v128 max128 = Sse4_1.max_epu32(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));

                if (Hint.Likely((int)length >= 4))
                {
                    max128 = Sse4_1.max_epu32(max128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 4;
                }

                max128 = Sse4_1.max_epu32(max128, Sse2.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    max128 = Sse4_1.max_epu32(max128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Sse4_1.max_epu32(max128, Sse2.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Sse4_1.max_epu32(max128, Sse2.cvtsi32_si128(*(int*)ptr_v256)).UInt0;
                }
                else
                {
                    return max128.UInt0;
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 max0 = Sse2.set1_epi32(unchecked((int)uint.MinValue));
                
                if (Hint.Likely(length >= 16))
                {
                    v128 max1 = Sse2.set1_epi32(unchecked((int)uint.MinValue));
                    v128 max2 = Sse2.set1_epi32(unchecked((int)uint.MinValue));
                    v128 max3 = Sse2.set1_epi32(unchecked((int)uint.MinValue));

                    do
                    {
                        max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));
                        max1 = Max(max1, Sse2.loadu_si128(ptr_v128++));
                        max2 = Max(max2, Sse2.loadu_si128(ptr_v128++));
                        max3 = Max(max3, Sse2.loadu_si128(ptr_v128++));

                        length -= 16;
                    } 
                    while (Hint.Likely(length >= 16));

                    max0 = Max(max0, max1);
                    max2 = Max(max2, max3);
                    max0 = Max(max0, max2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));
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
                else { }


                max0 = Max(max0, Sse2.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    max0 = Max(max0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 2;
                }

                max0 = Max(max0, Sse2.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    if (Sse4_1.IsSse41Supported)
                    {
                        return Max(max0, Sse2.cvtsi32_si128(*(int*)ptr_v128)).UInt0;
                    }
                    else
                    {
                        return math.max(max0.UInt0, *(uint*)ptr_v128);
                    }
                }
                else
                {
                    return max0.UInt0;
                }
            }
            else
            {
                uint x = uint.MinValue;

                for (long i = 0; i < length; i++)
                {
                    x = math.max(x, ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Maximum(this NativeArray<uint> array)
        {
            return SIMD_Maximum((uint*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Maximum(this NativeArray<uint> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Maximum(this NativeArray<uint> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Maximum(this NativeList<uint> array)
        {
            return SIMD_Maximum((uint*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Maximum(this NativeList<uint> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Maximum(this NativeList<uint> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Maximum(this NativeSlice<uint> array)
        {
            return SIMD_Maximum((uint*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Maximum(this NativeSlice<uint> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Maximum(this NativeSlice<uint> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Maximum(ulong* ptr, long length)
        {
            static v256 Max256(v256 a, v256 b, v256 mask)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 greaterMask = Avx2.mm256_cmpgt_epi64(Avx2.mm256_xor_si256(a, mask),
                                                              Avx2.mm256_xor_si256(b, mask));

                    return Avx2.mm256_blendv_epi8(b, a, greaterMask);
                }
                else
                {
                    return default(v256);
                }
            }

            static v128 Max128(v128 a, v128 b, v128 mask)
            {
                if (Sse4_2.IsSse42Supported)
                {
                    v128 greaterMask = Sse4_2.cmpgt_epi64(Sse2.xor_si128(a, mask),
                                                          Sse2.xor_si128(b, mask));

                    return Sse4_1.blendv_epi8(b, a, greaterMask);
                }
                else
                {
                    return default(v128);
                }
            }

Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 max0 = Avx.mm256_set1_epi64x(unchecked((long)ulong.MinValue));

                v256 mask = Avx.mm256_set1_epi64x(1L << 63);
                
                if (Hint.Likely(length >= 16))
                {
                    v256 max1 = Avx.mm256_set1_epi64x(unchecked((long)ulong.MinValue));
                    v256 max2 = Avx.mm256_set1_epi64x(unchecked((long)ulong.MinValue));
                    v256 max3 = Avx.mm256_set1_epi64x(unchecked((long)ulong.MinValue));

                    do
                    {
                        max0 = Max256(max0, Avx.mm256_loadu_si256(ptr_v256++), mask);
                        max1 = Max256(max1, Avx.mm256_loadu_si256(ptr_v256++), mask);
                        max2 = Max256(max2, Avx.mm256_loadu_si256(ptr_v256++), mask);
                        max3 = Max256(max3, Avx.mm256_loadu_si256(ptr_v256++), mask);

                        length -= 16;
                    } 
                    while (Hint.Likely(length >= 16));

                    max0 = Max256(max0, max1, mask);
                    max2 = Max256(max2, max3, mask);
                    max0 = Max256(max0, max2, mask);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    max0 = Max256(max0, Avx.mm256_loadu_si256(ptr_v256++), mask);

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        max0 = Max256(max0, Avx.mm256_loadu_si256(ptr_v256++), mask);

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            max0 = Max256(max0, Avx.mm256_loadu_si256(ptr_v256++), mask);
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
                else { }

                v128 max128 = Max128(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1), Avx.mm256_castsi256_si128(mask));

                if (Hint.Likely((int)length >= 2))
                {
                    max128 = Max128(max128, Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(mask));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 2;
                }

                if (Hint.Likely(length != 0))
                {
                    return math.max(*(ulong*)ptr_v256, math.max(max128.ULong0, max128.ULong1));
                }
                else
                {
                    return math.max(max128.ULong0, max128.ULong1);
                }
            }
            else if (Sse4_2.IsSse42Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 max0 = Sse2.set1_epi64x(unchecked((long)ulong.MinValue));

                v128 mask = Sse2.set1_epi64x(1L << 63);
                
                if (Hint.Likely(length >= 8))
                {
                    v128 max1 = Sse2.set1_epi64x(unchecked((long)ulong.MinValue));
                    v128 max2 = Sse2.set1_epi64x(unchecked((long)ulong.MinValue));
                    v128 max3 = Sse2.set1_epi64x(unchecked((long)ulong.MinValue));

                    do
                    {
                        max0 = Max128(max0, Sse2.loadu_si128(ptr_v128++), mask);
                        max1 = Max128(max1, Sse2.loadu_si128(ptr_v128++), mask);
                        max2 = Max128(max2, Sse2.loadu_si128(ptr_v128++), mask);
                        max3 = Max128(max3, Sse2.loadu_si128(ptr_v128++), mask);

                        length -= 8;
                    } 
                    while (Hint.Likely(length >= 8));

                    max0 = Max128(max0, max1, mask);
                    max2 = Max128(max2, max3, mask);
                    max0 = Max128(max0, max2, mask);
                }

                if (Hint.Likely((int)length >= 2))
                {
                    max0 = Max128(max0, Sse2.loadu_si128(ptr_v128++), mask);

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        max0 = Max128(max0, Sse2.loadu_si128(ptr_v128++), mask);

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            max0 = Max128(max0, Sse2.loadu_si128(ptr_v128++), mask);
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
                else { }

                if (Hint.Likely(length != 0))
                {
                    return math.max(*(ulong*)ptr_v128, math.max(max0.ULong0, max0.ULong1));
                }
                else
                {
                    return math.max(max0.ULong0, max0.ULong1);
                }
            }
            else
            {
                ulong x = ulong.MinValue;

                for (long i = 0; i < length; i++)
                {
                    x = math.max(x, ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Maximum(this NativeArray<ulong> array)
        {
            return SIMD_Maximum((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Maximum(this NativeArray<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Maximum(this NativeArray<ulong> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Maximum(this NativeList<ulong> array)
        {
            return SIMD_Maximum((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Maximum(this NativeList<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Maximum(this NativeList<ulong> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Maximum(this NativeSlice<ulong> array)
        {
            return SIMD_Maximum((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Maximum(this NativeSlice<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Maximum(this NativeSlice<ulong> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Maximum(sbyte* ptr, long length)
        {
            static v128 Max(v128 a, v128 b)
            {
                if (Sse4_1.IsSse41Supported)
                {
                    return Sse4_1.max_epi8(b, a);
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 greaterMask = Sse2.cmpgt_epi8(b, a);

                    return Sse2.or_si128(Sse2.and_si128(greaterMask, b),
                                         Sse2.andnot_si128(greaterMask, a));
                }
                else
                {
                    return default(v128);
                }
            }

Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 max0 = Avx.mm256_set1_epi8(unchecked((byte)sbyte.MinValue));
                
                if (Hint.Likely(length >= 128))
                {
                    v256 max1 = Avx.mm256_set1_epi8(unchecked((byte)sbyte.MinValue));
                    v256 max2 = Avx.mm256_set1_epi8(unchecked((byte)sbyte.MinValue));
                    v256 max3 = Avx.mm256_set1_epi8(unchecked((byte)sbyte.MinValue));

                    do
                    {
                        max0 = Avx2.mm256_max_epi8(max0, Avx.mm256_loadu_si256(ptr_v256++));
                        max1 = Avx2.mm256_max_epi8(max1, Avx.mm256_loadu_si256(ptr_v256++));
                        max2 = Avx2.mm256_max_epi8(max2, Avx.mm256_loadu_si256(ptr_v256++));
                        max3 = Avx2.mm256_max_epi8(max3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 128;
                    } 
                    while (Hint.Likely(length >= 128));

                    max0 = Avx2.mm256_max_epi8(max0, max1);
                    max2 = Avx2.mm256_max_epi8(max2, max3);
                    max0 = Avx2.mm256_max_epi8(max0, max2);
                }

                if (Hint.Likely((int)length >= 32))
                {
                    max0 = Avx2.mm256_max_epi8(max0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 32))
                    {
                        max0 = Avx2.mm256_max_epi8(max0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 32))
                        {
                            max0 = Avx2.mm256_max_epi8(max0, Avx.mm256_loadu_si256(ptr_v256++));
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
                else { }

                v128 max128 = Sse4_1.max_epi8(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));

                if (Hint.Likely((int)length >= 16))
                {
                    max128 = Sse4_1.max_epi8(max128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 16;
                }

                v128 cmp = default(v128);
                max128 = Sse4_1.max_epi8(max128, Sse2.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    max128 = Sse4_1.max_epi8(max128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 8;
                }

                max128 = Sse4_1.max_epi8(max128, Sse2.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    max128 = Sse4_1.max_epi8(max128, Sse2.cvtsi32_si128(*(int*)ptr_v256));
                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 4;
                }

                max128 = Sse4_1.max_epi8(max128, Sse2.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    max128 = Sse4_1.max_epi8(max128, Sse2.insert_epi16(cmp, *(short*)ptr_v256, 0));
                    ptr_v256 = (v256*)((short*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Sse4_1.max_epi8(max128, Sse2.bsrli_si128(max128, 1 * sizeof(sbyte)));

                if (Hint.Likely(length != 0))
                {
                    return Sse4_1.max_epi8(max128, Sse4_1.insert_epi8(cmp, *(byte*)ptr_v256, 0)).SByte0;
                }
                else
                {
                    return max128.SByte0;
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 max0 = Sse2.set1_epi8(sbyte.MinValue);
                
                if (Hint.Likely(length >= 64))
                {
                    v128 max1 = Sse2.set1_epi8(sbyte.MinValue);
                    v128 max2 = Sse2.set1_epi8(sbyte.MinValue);
                    v128 max3 = Sse2.set1_epi8(sbyte.MinValue);

                    do
                    {
                        max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));
                        max1 = Max(max1, Sse2.loadu_si128(ptr_v128++));
                        max2 = Max(max2, Sse2.loadu_si128(ptr_v128++));
                        max3 = Max(max3, Sse2.loadu_si128(ptr_v128++));

                        length -= 64;
                    } 
                    while (Hint.Likely(length >= 64));

                    max0 = Max(max0, max1);
                    max2 = Max(max2, max3);
                    max0 = Max(max0, max2);
                }

                if (Hint.Likely((int)length >= 16))
                {
                    max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));
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
                else { }

                v128 cmp = default(v128);
                max0 = Max(max0, Sse2.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    max0 = Max(max0, new v128(*(long*)ptr_v128, 0));
                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 8;
                }

                max0 = Max(max0, Sse2.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    max0 = Max(max0, Sse2.cvtsi32_si128(*(int*)ptr_v128));
                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 4;
                }

                max0 = Max(max0, Sse2.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    max0 = Max(max0, Sse2.insert_epi16(cmp, *(short*)ptr_v128, 0));
                    ptr_v128 = (v128*)((short*)ptr_v128 + 1);
                    length -= 2;
                }

                max0 = Max(max0, Sse2.bsrli_si128(max0, 1 * sizeof(sbyte)));

                if (Hint.Likely(length != 0))
                {
                    if (Sse4_1.IsSse41Supported)
                    {
                        return Sse4_1.max_epi8(max0, Sse4_1.insert_epi8(cmp, *(byte*)ptr_v128, 0)).SByte0;
                    }
                    else
                    {
                        return (sbyte)math.max((int)max0.SByte0, (int)(*(sbyte*)ptr_v128));
                    }
                }
                else
                {
                    return max0.SByte0;
                }
            }
            else
            {
                sbyte x = sbyte.MinValue;

                for (long i = 0; i < length; i++)
                {
                    x = (sbyte)math.max((int)x, (int)ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Maximum(this NativeArray<sbyte> array)
        {
            return SIMD_Maximum((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Maximum(this NativeArray<sbyte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Maximum(this NativeArray<sbyte> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Maximum(this NativeList<sbyte> array)
        {
            return SIMD_Maximum((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Maximum(this NativeList<sbyte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Maximum(this NativeList<sbyte> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Maximum(this NativeSlice<sbyte> array)
        {
            return SIMD_Maximum((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Maximum(this NativeSlice<sbyte> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Maximum(this NativeSlice<sbyte> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Maximum(short* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 max0 = Avx.mm256_set1_epi16(short.MinValue);
                
                if (Hint.Likely(length >= 64))
                {
                    v256 max1 = Avx.mm256_set1_epi16(short.MinValue);
                    v256 max2 = Avx.mm256_set1_epi16(short.MinValue);
                    v256 max3 = Avx.mm256_set1_epi16(short.MinValue);

                    do
                    {
                        max0 = Avx2.mm256_max_epi16(max0, Avx.mm256_loadu_si256(ptr_v256++));
                        max1 = Avx2.mm256_max_epi16(max1, Avx.mm256_loadu_si256(ptr_v256++));
                        max2 = Avx2.mm256_max_epi16(max2, Avx.mm256_loadu_si256(ptr_v256++));
                        max3 = Avx2.mm256_max_epi16(max3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 64;
                    } 
                    while (Hint.Likely(length >= 64));

                    max0 = Avx2.mm256_max_epi16(max0, max1);
                    max2 = Avx2.mm256_max_epi16(max2, max3);
                    max0 = Avx2.mm256_max_epi16(max0, max2);
                }

                if (Hint.Likely((int)length >= 16))
                {
                    max0 = Avx2.mm256_max_epi16(max0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        max0 = Avx2.mm256_max_epi16(max0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            max0 = Avx2.mm256_max_epi16(max0, Avx.mm256_loadu_si256(ptr_v256++));
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
                else { }

                v128 max128 = Sse2.max_epi16(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));

                if (Hint.Likely((int)length >= 8))
                {
                    max128 = Sse2.max_epi16(max128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 8;
                }

                v128 cmp = default(v128);
                max128 = Sse2.max_epi16(max128, Sse2.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    max128 = Sse2.max_epi16(max128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 4;
                }

                max128 = Sse2.max_epi16(max128, Sse2.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    max128 = Sse2.max_epi16(max128, Sse2.cvtsi32_si128(*(int*)ptr_v256));
                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Sse2.max_epi16(max128, Sse2.shufflelo_epi16(max128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Sse2.max_epi16(max128, Sse2.insert_epi16(cmp, *(short*)ptr_v256, 0)).SShort0;
                }
                else
                {
                    return max128.SShort0;
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 max0 = Sse2.set1_epi16(short.MinValue);
                
                if (Hint.Likely(length >= 32))
                {
                    v128 max1 = Sse2.set1_epi16(short.MinValue);
                    v128 max2 = Sse2.set1_epi16(short.MinValue);
                    v128 max3 = Sse2.set1_epi16(short.MinValue);

                    do
                    {
                        max0 = Sse2.max_epi16(max0, Sse2.loadu_si128(ptr_v128++));
                        max1 = Sse2.max_epi16(max1, Sse2.loadu_si128(ptr_v128++));
                        max2 = Sse2.max_epi16(max2, Sse2.loadu_si128(ptr_v128++));
                        max3 = Sse2.max_epi16(max3, Sse2.loadu_si128(ptr_v128++));

                        length -= 32;
                    } 
                    while (Hint.Likely(length >= 32));

                    max0 = Sse2.max_epi16(max0, max1);
                    max2 = Sse2.max_epi16(max2, max3);
                    max0 = Sse2.max_epi16(max0, max2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    max0 = Sse2.max_epi16(max0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        max0 = Sse2.max_epi16(max0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            max0 = Sse2.max_epi16(max0, Sse2.loadu_si128(ptr_v128++));
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
                else { }

                v128 cmp = default(v128);
                max0 = Sse2.max_epi16(max0, Sse2.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    max0 = Sse2.max_epi16(max0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 4;
                }

                max0 = Sse2.max_epi16(max0, Sse2.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    max0 = Sse2.max_epi16(max0, Sse2.cvtsi32_si128(*(int*)ptr_v128));

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 2;
                }

                max0 = Sse2.max_epi16(max0, Sse2.shufflelo_epi16(max0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Sse2.max_epi16(max0, Sse2.insert_epi16(cmp, *(short*)ptr_v128, 0)).SShort0;
                }
                else
                {
                    return max0.SShort0;
                }
            }
            else
            {
                short x = short.MinValue;

                for (long i = 0; i < length; i++)
                {
                    x = (short)math.max((int)x, (int)ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Maximum(this NativeArray<short> array)
        {
            return SIMD_Maximum((short*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Maximum(this NativeArray<short> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Maximum(this NativeArray<short> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Maximum(this NativeList<short> array)
        {
            return SIMD_Maximum((short*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Maximum(this NativeList<short> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Maximum(this NativeList<short> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Maximum(this NativeSlice<short> array)
        {
            return SIMD_Maximum((short*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Maximum(this NativeSlice<short> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Maximum(this NativeSlice<short> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Maximum(int* ptr, long length)
        {
            static v128 Max(v128 a, v128 b)
            {
                if (Sse4_1.IsSse41Supported)
                {
                    return Sse4_1.max_epi32(b, a);
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 greaterMask = Sse2.cmpgt_epi32(b, a);

                    return Sse2.or_si128(Sse2.and_si128(greaterMask, b),
                                         Sse2.andnot_si128(greaterMask, a));
                }
                else
                {
                    return default(v128);
                }
            }

Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 max0 = Avx.mm256_set1_epi32(int.MinValue);
                
                if (Hint.Likely(length >= 32))
                {
                    v256 max1 = Avx.mm256_set1_epi32(int.MinValue);
                    v256 max2 = Avx.mm256_set1_epi32(int.MinValue);
                    v256 max3 = Avx.mm256_set1_epi32(int.MinValue);

                    do
                    {
                        max0 = Avx2.mm256_max_epi32(max0, Avx.mm256_loadu_si256(ptr_v256++));
                        max1 = Avx2.mm256_max_epi32(max1, Avx.mm256_loadu_si256(ptr_v256++));
                        max2 = Avx2.mm256_max_epi32(max2, Avx.mm256_loadu_si256(ptr_v256++));
                        max3 = Avx2.mm256_max_epi32(max3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 32;
                    } 
                    while (Hint.Likely(length >= 32));

                    max0 = Avx2.mm256_max_epi32(max0, max1);
                    max2 = Avx2.mm256_max_epi32(max2, max3);
                    max0 = Avx2.mm256_max_epi32(max0, max2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    max0 = Avx2.mm256_max_epi32(max0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        max0 = Avx2.mm256_max_epi32(max0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            max0 = Avx2.mm256_max_epi32(max0, Avx.mm256_loadu_si256(ptr_v256++));
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
                else { }

                v128 max128 = Sse4_1.max_epi32(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));

                if (Hint.Likely((int)length >= 4))
                {
                    max128 = Sse4_1.max_epi32(max128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 4;
                }

                max128 = Sse4_1.max_epi32(max128, Sse2.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    max128 = Sse4_1.max_epi32(max128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Sse4_1.max_epi32(max128, Sse2.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Sse4_1.max_epi32(max128, Sse2.cvtsi32_si128(*(int*)ptr_v256)).SInt0;
                }
                else
                {
                    return max128.SInt0;
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 max0 = Sse2.set1_epi32(int.MinValue);
                
                if (Hint.Likely(length >= 16))
                {
                    v128 max1 = Sse2.set1_epi32(int.MinValue);
                    v128 max2 = Sse2.set1_epi32(int.MinValue);
                    v128 max3 = Sse2.set1_epi32(int.MinValue);

                    do
                    {
                        max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));
                        max1 = Max(max1, Sse2.loadu_si128(ptr_v128++));
                        max2 = Max(max2, Sse2.loadu_si128(ptr_v128++));
                        max3 = Max(max3, Sse2.loadu_si128(ptr_v128++));

                        length -= 16;
                    } 
                    while (Hint.Likely(length >= 16));

                    max0 = Max(max0, max1);
                    max2 = Max(max2, max3);
                    max0 = Max(max0, max2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            max0 = Max(max0, Sse2.loadu_si128(ptr_v128++));
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
                else { }

                max0 = Max(max0, Sse2.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    max0 = Max(max0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 2;
                }

                max0 = Max(max0, Sse2.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    if (Sse4_1.IsSse41Supported)
                    {
                        return Max(max0, Sse2.cvtsi32_si128(*(int*)ptr_v128)).SInt0;
                    }
                    else
                    {
                        return math.max(max0.SInt0, *(int*)ptr_v128);
                    }
                }
                else
                {
                    return max0.SInt0;
                }
            }
            else
            {
                int x = int.MinValue;

                for (long i = 0; i < length; i++)
                {
                    x = math.max(x, ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Maximum(this NativeArray<int> array)
        {
            return SIMD_Maximum((int*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Maximum(this NativeArray<int> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Maximum(this NativeArray<int> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Maximum(this NativeList<int> array)
        {
            return SIMD_Maximum((int*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Maximum(this NativeList<int> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Maximum(this NativeList<int> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Maximum(this NativeSlice<int> array)
        {
            return SIMD_Maximum((int*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Maximum(this NativeSlice<int> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Maximum(this NativeSlice<int> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Maximum(long* ptr, long length)
        {
            static v256 Max256(v256 a, v256 b)
            {
                return Avx2.mm256_blendv_epi8(a, b, Avx2.mm256_cmpgt_epi64(b, a));
            }

            static v128 Max128(v128 a, v128 b)
            {
                return Sse4_1.blendv_epi8(a, b, Sse4_2.cmpgt_epi64(b, a));
            }

Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 max0 = Avx.mm256_set1_epi64x(long.MinValue);
                
                if (Hint.Likely(length >= 16))
                {
                    v256 max1 = Avx.mm256_set1_epi64x(long.MinValue);
                    v256 max2 = Avx.mm256_set1_epi64x(long.MinValue);
                    v256 max3 = Avx.mm256_set1_epi64x(long.MinValue);

                    do
                    {
                        max0 = Max256(max0, Avx.mm256_loadu_si256(ptr_v256++));
                        max1 = Max256(max1, Avx.mm256_loadu_si256(ptr_v256++));
                        max2 = Max256(max2, Avx.mm256_loadu_si256(ptr_v256++));
                        max3 = Max256(max3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 16;
                    } 
                    while (Hint.Likely(length >= 16));

                    max0 = Max256(max0, max1);
                    max2 = Max256(max2, max3);
                    max0 = Max256(max0, max2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    max0 = Max256(max0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        max0 = Max256(max0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            max0 = Max256(max0, Avx.mm256_loadu_si256(ptr_v256++));
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
                else { }

                v128 max128 = Max128(Avx.mm256_castsi256_si128(max0), Avx2.mm256_extracti128_si256(max0, 1));

                if (Hint.Likely((int)length >= 2))
                {
                    max128 = Max128(max128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 2;
                }

                max128 = Max128(max128, Sse2.shuffle_epi32(max128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    return math.max(max128.SLong0, *(long*)ptr_v256);
                }
                else
                {
                    return max128.SLong0;
                }
            }
            else if (Sse4_2.IsSse42Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 max0 = Sse2.set1_epi64x(long.MinValue);
                
                if (Hint.Likely(length >= 8))
                {
                    v128 max1 = Sse2.set1_epi64x(long.MinValue);
                    v128 max2 = Sse2.set1_epi64x(long.MinValue);
                    v128 max3 = Sse2.set1_epi64x(long.MinValue);

                    do
                    {
                        max0 = Max128(max0, Sse2.loadu_si128(ptr_v128++));
                        max1 = Max128(max1, Sse2.loadu_si128(ptr_v128++));
                        max2 = Max128(max2, Sse2.loadu_si128(ptr_v128++));
                        max3 = Max128(max3, Sse2.loadu_si128(ptr_v128++));

                        length -= 8;
                    } 
                    while (Hint.Likely(length >= 8));

                    max0 = Max128(max0, max1);
                    max2 = Max128(max2, max3);
                    max0 = Max128(max0, max2);
                }

                if (Hint.Likely((int)length >= 2))
                {
                    max0 = Max128(max0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        max0 = Max128(max0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            max0 = Max128(max0, Sse2.loadu_si128(ptr_v128++));
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
                else { }

                max0 = Max128(max0, Sse2.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    return math.max(max0.SLong0, *(long*)ptr_v128);
                }
                else
                {
                    return max0.SLong0;
                }
            }
            else
            {
                long x = long.MinValue;

                for (long i = 0; i < length; i++)
                {
                    x = math.max(x, ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Maximum(this NativeArray<long> array)
        {
            return SIMD_Maximum((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Maximum(this NativeArray<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Maximum(this NativeArray<long> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Maximum(this NativeList<long> array)
        {
            return SIMD_Maximum((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Maximum(this NativeList<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Maximum(this NativeList<long> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Maximum(this NativeSlice<long> array)
        {
            return SIMD_Maximum((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Maximum(this NativeSlice<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Maximum(this NativeSlice<long> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Maximum(float* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx.IsAvxSupported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 max0 = new v256(float.NegativeInfinity);
                
                if (Hint.Likely(length >= 32))
                {
                    v256 max1 = new v256(float.NegativeInfinity);
                    v256 max2 = new v256(float.NegativeInfinity);
                    v256 max3 = new v256(float.NegativeInfinity);

                    do
                    {
                        max0 = Avx.mm256_max_ps(max0, Avx.mm256_loadu_ps(ptr_v256++));
                        max1 = Avx.mm256_max_ps(max1, Avx.mm256_loadu_ps(ptr_v256++));
                        max2 = Avx.mm256_max_ps(max2, Avx.mm256_loadu_ps(ptr_v256++));
                        max3 = Avx.mm256_max_ps(max3, Avx.mm256_loadu_ps(ptr_v256++));

                        length -= 32;
                    } 
                    while (Hint.Likely(length >= 32));

                    max0 = Avx.mm256_max_ps(max0, max1);
                    max2 = Avx.mm256_max_ps(max2, max3);
                    max0 = Avx.mm256_max_ps(max0, max2);
                }

                if (Hint.Likely((int)length >= 8))
                {
                    max0 = Avx.mm256_max_ps(max0, Avx.mm256_loadu_ps(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        max0 = Avx.mm256_max_ps(max0, Avx.mm256_loadu_ps(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            max0 = Avx.mm256_max_ps(max0, Avx.mm256_loadu_ps(ptr_v256++));
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
                else { }

                v128 max128 = Sse.max_ps(Avx.mm256_castps256_ps128(max0), Avx.mm256_extractf128_ps(max0, 1));

                if (Hint.Likely((int)length >= 4))
                {
                    max128 = Sse.max_ps(max128, Sse.loadu_ps(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 4;
                }
                
                max128 = Sse.max_ps(max128, Avx.permute_ps(max128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    max128 = Sse.max_ps(max128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);

                    length -= 2;
                }

                max128 = Sse.max_ps(max128, Avx.permute_ps(max128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    max128 = Sse.max_ss(max128, Sse2.cvtsi32_si128(*(int*)ptr_v256));
                }

                return max128.Float0;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 max0 = new v128(float.NegativeInfinity);
                
                if (Hint.Likely(length >= 16))
                {
                    v128 max1 = new v128(float.NegativeInfinity);
                    v128 max2 = new v128(float.NegativeInfinity);
                    v128 max3 = new v128(float.NegativeInfinity);

                    do
                    {
                        max0 = Sse.max_ps(max0, Sse.loadu_ps(ptr_v128++));
                        max1 = Sse.max_ps(max1, Sse.loadu_ps(ptr_v128++));
                        max2 = Sse.max_ps(max2, Sse.loadu_ps(ptr_v128++));
                        max3 = Sse.max_ps(max3, Sse.loadu_ps(ptr_v128++));

                        length -= 16;
                    } 
                    while (Hint.Likely(length >= 16));

                    max0 = Sse.max_ps(max0, max1);
                    max2 = Sse.max_ps(max2, max3);
                    max0 = Sse.max_ps(max0, max2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    max0 = Sse.max_ps(max0, Sse.loadu_ps(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        max0 = Sse.max_ps(max0, Sse.loadu_ps(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            max0 = Sse.max_ps(max0, Sse.loadu_ps(ptr_v128++));
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
                else { }

                max0 = Sse.max_ps(max0, Sse2.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    max0 = Sse.max_ps(max0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));
                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);

                    length -= 2;
                }

                max0 = Sse.max_ps(max0, Sse2.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    max0 = Sse.max_ss(max0, Sse2.cvtsi32_si128(*(int*)ptr_v128));
                }

                return max0.Float0;
            }
            else
            {
                float x = float.NegativeInfinity;

                for (long i = 0; i < length; i++)
                {
                    x = math.max(x, ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Maximum(this NativeArray<float> array)
        {
            return SIMD_Maximum((float*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Maximum(this NativeArray<float> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Maximum(this NativeArray<float> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Maximum(this NativeList<float> array)
        {
            return SIMD_Maximum((float*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Maximum(this NativeList<float> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Maximum(this NativeList<float> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Maximum(this NativeSlice<float> array)
        {
            return SIMD_Maximum((float*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Maximum(this NativeSlice<float> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Maximum(this NativeSlice<float> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Maximum(double* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx.IsAvxSupported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 max0 = new v256(double.NegativeInfinity);
                
                if (Hint.Likely(length >= 16))
                {
                    v256 max1 = new v256(double.NegativeInfinity);
                    v256 max2 = new v256(double.NegativeInfinity);
                    v256 max3 = new v256(double.NegativeInfinity);

                    do
                    {
                        max0 = Avx.mm256_max_pd(max0, Avx.mm256_loadu_pd(ptr_v256++));
                        max1 = Avx.mm256_max_pd(max1, Avx.mm256_loadu_pd(ptr_v256++));
                        max2 = Avx.mm256_max_pd(max2, Avx.mm256_loadu_pd(ptr_v256++));
                        max3 = Avx.mm256_max_pd(max3, Avx.mm256_loadu_pd(ptr_v256++));

                        length -= 16;
                    } 
                    while (Hint.Likely(length >= 16));

                    max0 = Avx.mm256_max_pd(max0, max1);
                    max2 = Avx.mm256_max_pd(max2, max3);
                    max0 = Avx.mm256_max_pd(max0, max2);
                }

                if (Hint.Likely((int)length >= 4))
                {
                    max0 = Avx.mm256_max_pd(max0, Avx.mm256_loadu_pd(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        max0 = Avx.mm256_max_pd(max0, Avx.mm256_loadu_pd(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            max0 = Avx.mm256_max_pd(max0, Avx.mm256_loadu_pd(ptr_v256++));
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
                else { }

                v128 max128 = Sse2.max_pd(Avx.mm256_castpd256_pd128(max0), Avx.mm256_extractf128_pd(max0, 1));

                if (Hint.Likely((int)length >= 2))
                {
                    max128 = Sse2.max_pd(max128, Sse.loadu_ps(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 2;
                }

                max128 = Sse2.max_pd(max128, Avx.permute_pd(max128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    max128 = Sse2.max_sd(max128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                }

                return max128.Double0;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 max0 = new v128(double.NegativeInfinity);
                
                if (Hint.Likely(length >= 8))
                {
                    v128 max1 = new v128(double.NegativeInfinity);
                    v128 max2 = new v128(double.NegativeInfinity);
                    v128 max3 = new v128(double.NegativeInfinity);

                    do
                    {
                        max0 = Sse2.max_pd(max0, Sse.loadu_ps(ptr_v128++));
                        max1 = Sse2.max_pd(max1, Sse.loadu_ps(ptr_v128++));
                        max2 = Sse2.max_pd(max2, Sse.loadu_ps(ptr_v128++));
                        max3 = Sse2.max_pd(max3, Sse.loadu_ps(ptr_v128++));

                        length -= 8;
                    } 
                    while (Hint.Likely(length >= 8));

                    max0 = Sse2.max_pd(max0, max1);
                    max2 = Sse2.max_pd(max2, max3);
                    max0 = Sse2.max_pd(max0, max2);
                }

                if (Hint.Likely((int)length >= 2))
                {
                    max0 = Sse2.max_pd(max0, Sse.loadu_ps(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        max0 = Sse2.max_pd(max0, Sse.loadu_ps(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            max0 = Sse2.max_pd(max0, Sse.loadu_ps(ptr_v128++));
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
                else { }

                max0 = Sse2.max_pd(max0, Sse2.shuffle_epi32(max0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    max0 = Sse2.max_sd(max0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));
                }

                return max0.Double0;
            }
            else
            {
                double x = double.NegativeInfinity;

                for (long i = 0; i < length; i++)
                {
                    x = math.max(x, ptr[i]);
                }

                return x;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Maximum(this NativeArray<double> array)
        {
            return SIMD_Maximum((double*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Maximum(this NativeArray<double> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Maximum(this NativeArray<double> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Maximum(this NativeList<double> array)
        {
            return SIMD_Maximum((double*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Maximum(this NativeList<double> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Maximum(this NativeList<double> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Maximum(this NativeSlice<double> array)
        {
            return SIMD_Maximum((double*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Maximum(this NativeSlice<double> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Maximum((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Maximum(this NativeSlice<double> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Maximum((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }
    }
}