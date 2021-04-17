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
        public static byte SIMD_Minimum(byte* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 acc0 = Avx.mm256_set1_epi8(byte.MaxValue);
                v256 acc1 = Avx.mm256_set1_epi8(byte.MaxValue);
                v256 acc2 = Avx.mm256_set1_epi8(byte.MaxValue);
                v256 acc3 = Avx.mm256_set1_epi8(byte.MaxValue);

                while (Hint.Likely(length >= 128))
                {
                    acc0 = Avx2.mm256_min_epu8(acc0, Avx.mm256_loadu_si256(ptr_v256++));
                    acc1 = Avx2.mm256_min_epu8(acc1, Avx.mm256_loadu_si256(ptr_v256++));
                    acc2 = Avx2.mm256_min_epu8(acc2, Avx.mm256_loadu_si256(ptr_v256++));
                    acc3 = Avx2.mm256_min_epu8(acc3, Avx.mm256_loadu_si256(ptr_v256++));

                    length -= 128;
                }

                acc0 = Avx2.mm256_min_epu8(acc0, acc1);
                acc2 = Avx2.mm256_min_epu8(acc2, acc3);
                acc0 = Avx2.mm256_min_epu8(acc0, acc2);

                if (Hint.Likely((int)length >= 32))
                {
                    acc0 = Avx2.mm256_min_epu8(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 32))
                    {
                        acc0 = Avx2.mm256_min_epu8(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 32))
                        {
                            acc0 = Avx2.mm256_min_epu8(acc0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 acc128 = Sse2.min_epu8(Avx.mm256_castsi256_si128(acc0), Avx2.mm256_extracti128_si256(acc0, 1));

                if (Hint.Likely((int)length >= 16))
                {
                    acc128 = Sse2.min_epu8(acc128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 16;
                }

                v128 cmp = default(v128);
                acc128 = Sse2.min_epu8(acc128, Sse2.shuffle_epi32(acc128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    acc128 = Sse2.min_epu8(acc128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 8;
                }

                acc128 = Sse2.min_epu8(acc128, Sse2.shufflelo_epi16(acc128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    acc128 = Sse2.min_epu8(acc128, Sse2.cvtsi32_si128(*(int*)ptr_v256));
                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 4;
                }

                acc128 = Sse2.min_epu8(acc128, Sse2.shufflelo_epi16(acc128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc128 = Sse2.min_epu8(acc128, Sse2.insert_epi16(cmp, *(short*)ptr_v256, 0));
                    ptr_v256 = (v256*)((short*)ptr_v256 + 1);
                    length -= 2;
                }

                acc128 = Sse2.min_epu8(acc128, Sse2.bsrli_si128(acc128, 1 * sizeof(byte)));

                if (Hint.Likely(length != 0))
                {
                    return Sse2.min_epu8(acc128, Sse4_1.insert_epi8(cmp, *(byte*)ptr_v256, 0)).Byte0;
                }
                else
                {
                    return acc128.Byte0;
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 acc0 = Sse2.set1_epi8(unchecked((sbyte)byte.MaxValue));
                v128 acc1 = Sse2.set1_epi8(unchecked((sbyte)byte.MaxValue));
                v128 acc2 = Sse2.set1_epi8(unchecked((sbyte)byte.MaxValue));
                v128 acc3 = Sse2.set1_epi8(unchecked((sbyte)byte.MaxValue));

                while (Hint.Likely(length >= 64))
                {
                    acc0 = Sse2.min_epu8(acc0, Sse2.loadu_si128(ptr_v128++));
                    acc1 = Sse2.min_epu8(acc1, Sse2.loadu_si128(ptr_v128++));
                    acc2 = Sse2.min_epu8(acc2, Sse2.loadu_si128(ptr_v128++));
                    acc3 = Sse2.min_epu8(acc3, Sse2.loadu_si128(ptr_v128++));

                    length -= 64;
                }

                acc0 = Sse2.min_epu8(acc0, acc1);
                acc2 = Sse2.min_epu8(acc2, acc3);
                acc0 = Sse2.min_epu8(acc0, acc2);

                if (Hint.Likely((int)length >= 16))
                {
                    acc0 = Sse2.min_epu8(acc0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        acc0 = Sse2.min_epu8(acc0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            acc0 = Sse2.min_epu8(acc0, Sse2.loadu_si128(ptr_v128++));
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
                acc0 = Sse2.min_epu8(acc0, Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    acc0 = Sse2.min_epu8(acc0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 8;
                }

                acc0 = Sse2.min_epu8(acc0, Sse2.shufflelo_epi16(acc0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    acc0 = Sse2.min_epu8(acc0, Sse2.cvtsi32_si128(*(int*)ptr_v128));

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 4;
                }

                acc0 = Sse2.min_epu8(acc0, Sse2.shufflelo_epi16(acc0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc0 = Sse2.min_epu8(acc0, Sse2.insert_epi16(cmp, *(short*)ptr_v128, 0));
                    ptr_v128 = (v128*)((short*)ptr_v128 + 1);
                    length -= 2;
                }

                acc0 = Sse2.min_epu8(acc0, Sse2.bsrli_si128(acc0, 1 * sizeof(byte)));

                if (Hint.Likely(length != 0))
                {
                    if (Sse4_1.IsSse41Supported)
                    {
                        return Sse2.min_epu8(acc0, Sse4_1.insert_epi8(cmp, *(byte*)ptr_v128, 0)).Byte0;
                    }
                    else
                    {
                        return (byte)math.min((uint)acc0.Byte0, (uint)(*(byte*)ptr_v128));
                    }
                }
                else
                {
                    return acc0.Byte0;
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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Minimum((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SIMD_Minimum(ushort* ptr, long length)
        {
            static v128 Min(v128 a, v128 b)
            {
                if (Sse4_1.IsSse41Supported)
                {
                    return Sse4_1.min_epu16(a, b);
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 mask = Sse2.set1_epi16(unchecked((short)(1 << 15)));

                    return Sse2.xor_si128(mask,
                                          Sse2.min_epi16(Sse2.xor_si128(a, mask),
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
                v256 acc0 = Avx.mm256_set1_epi16(unchecked((short)ushort.MaxValue));
                v256 acc1 = Avx.mm256_set1_epi16(unchecked((short)ushort.MaxValue));
                v256 acc2 = Avx.mm256_set1_epi16(unchecked((short)ushort.MaxValue));
                v256 acc3 = Avx.mm256_set1_epi16(unchecked((short)ushort.MaxValue));

                while (Hint.Likely(length >= 64))
                {
                    acc0 = Avx2.mm256_min_epu16(acc0, Avx.mm256_loadu_si256(ptr_v256++));
                    acc1 = Avx2.mm256_min_epu16(acc1, Avx.mm256_loadu_si256(ptr_v256++));
                    acc2 = Avx2.mm256_min_epu16(acc2, Avx.mm256_loadu_si256(ptr_v256++));
                    acc3 = Avx2.mm256_min_epu16(acc3, Avx.mm256_loadu_si256(ptr_v256++));

                    length -= 64;
                }

                acc0 = Avx2.mm256_min_epu16(acc0, acc1);
                acc2 = Avx2.mm256_min_epu16(acc2, acc3);
                acc0 = Avx2.mm256_min_epu16(acc0, acc2);

                if (Hint.Likely((int)length >= 16))
                {
                    acc0 = Avx2.mm256_min_epu16(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        acc0 = Avx2.mm256_min_epu16(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            acc0 = Avx2.mm256_min_epu16(acc0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 acc128 = Sse4_1.min_epu16(Avx.mm256_castsi256_si128(acc0), Avx2.mm256_extracti128_si256(acc0, 1));

                if (Hint.Likely((int)length >= 8))
                {
                    acc128 = Sse4_1.min_epu16(acc128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 8;
                }

                v128 cmp = default(v128);
                acc128 = Sse4_1.min_epu16(acc128, Sse2.shuffle_epi32(acc128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    acc128 = Sse4_1.min_epu16(acc128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 4;
                }

                acc128 = Sse4_1.min_epu16(acc128, Sse2.shufflelo_epi16(acc128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc128 = Sse4_1.min_epu16(acc128, Sse2.cvtsi32_si128(*(int*)ptr_v256));
                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 2;
                }

                acc128 = Sse4_1.min_epu16(acc128, Sse2.shufflelo_epi16(acc128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Sse4_1.min_epu16(acc128, Sse2.insert_epi16(cmp, *(ushort*)ptr_v256, 0)).UShort0;
                }
                else
                {
                    return acc128.UShort0;
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 acc0 = Sse2.set1_epi16(unchecked((short)ushort.MaxValue));
                v128 acc1 = Sse2.set1_epi16(unchecked((short)ushort.MaxValue));
                v128 acc2 = Sse2.set1_epi16(unchecked((short)ushort.MaxValue));
                v128 acc3 = Sse2.set1_epi16(unchecked((short)ushort.MaxValue));

                while (Hint.Likely(length >= 32))
                {
                    acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));
                    acc1 = Min(acc1, Sse2.loadu_si128(ptr_v128++));
                    acc2 = Min(acc2, Sse2.loadu_si128(ptr_v128++));
                    acc3 = Min(acc3, Sse2.loadu_si128(ptr_v128++));

                    length -= 32;
                }

                acc0 = Min(acc0, acc1);
                acc2 = Min(acc2, acc3);
                acc0 = Min(acc0, acc2);

                if (Hint.Likely((int)length >= 8))
                {
                    acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));
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
                acc0 = Min(acc0, Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    acc0 = Min(acc0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 4;
                }

                acc0 = Min(acc0, Sse2.shufflelo_epi16(acc0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc0 = Min(acc0, Sse2.cvtsi32_si128(*(int*)ptr_v128));

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 2;
                }

                acc0 = Min(acc0, Sse2.shufflelo_epi16(acc0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    if (Sse4_1.IsSse41Supported)
                    {
                        return Sse4_1.min_epu16(acc0, Sse2.insert_epi16(cmp, *(ushort*)ptr_v128, 0)).UShort0;
                    }
                    else
                    {
                        return (ushort)math.min((uint)acc0.UShort0, *(ushort*)ptr_v128);
                    }
                }
                else
                {
                    return acc0.UShort0;
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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Minimum((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SIMD_Minimum(uint* ptr, long length)
        {
            static v128 Min(v128 a, v128 b)
            {
                if (Sse4_1.IsSse41Supported)
                {
                    return Sse4_1.min_epu32(a, b);
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 mask = Sse2.set1_epi32(unchecked((int)(1 << 31)));

                    v128 greaterMask = Sse2.cmpgt_epi32(Sse2.xor_si128(a, mask),
                                                        Sse2.xor_si128(b, mask));

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
                v256 acc0 = Avx.mm256_set1_epi32(unchecked((int)uint.MaxValue));
                v256 acc1 = Avx.mm256_set1_epi32(unchecked((int)uint.MaxValue));
                v256 acc2 = Avx.mm256_set1_epi32(unchecked((int)uint.MaxValue));
                v256 acc3 = Avx.mm256_set1_epi32(unchecked((int)uint.MaxValue));

                while (Hint.Likely(length >= 32))
                {
                    acc0 = Avx2.mm256_min_epu32(acc0, Avx.mm256_loadu_si256(ptr_v256++));
                    acc1 = Avx2.mm256_min_epu32(acc1, Avx.mm256_loadu_si256(ptr_v256++));
                    acc2 = Avx2.mm256_min_epu32(acc2, Avx.mm256_loadu_si256(ptr_v256++));
                    acc3 = Avx2.mm256_min_epu32(acc3, Avx.mm256_loadu_si256(ptr_v256++));

                    length -= 32;
                }

                acc0 = Avx2.mm256_min_epu32(acc0, acc1);
                acc2 = Avx2.mm256_min_epu32(acc2, acc3);
                acc0 = Avx2.mm256_min_epu32(acc0, acc2);

                if (Hint.Likely((int)length >= 8))
                {
                    acc0 = Avx2.mm256_min_epu32(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        acc0 = Avx2.mm256_min_epu32(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            acc0 = Avx2.mm256_min_epu32(acc0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 acc128 = Sse4_1.min_epu32(Avx.mm256_castsi256_si128(acc0), Avx2.mm256_extracti128_si256(acc0, 1));

                if (Hint.Likely((int)length >= 4))
                {
                    acc128 = Sse4_1.min_epu32(acc128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 4;
                }

                acc128 = Sse4_1.min_epu32(acc128, Sse2.shuffle_epi32(acc128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc128 = Sse4_1.min_epu32(acc128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 2;
                }

                acc128 = Sse4_1.min_epu32(acc128, Sse2.shuffle_epi32(acc128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Sse4_1.min_epu32(acc128, Sse2.cvtsi32_si128(*(int*)ptr_v256)).UInt0;
                }
                else
                {
                    return acc128.UInt0;
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 acc0 = Sse2.set1_epi32(unchecked((int)uint.MaxValue));
                v128 acc1 = Sse2.set1_epi32(unchecked((int)uint.MaxValue));
                v128 acc2 = Sse2.set1_epi32(unchecked((int)uint.MaxValue));
                v128 acc3 = Sse2.set1_epi32(unchecked((int)uint.MaxValue));

                while (Hint.Likely(length >= 16))
                {
                    acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));
                    acc1 = Min(acc1, Sse2.loadu_si128(ptr_v128++));
                    acc2 = Min(acc2, Sse2.loadu_si128(ptr_v128++));
                    acc3 = Min(acc3, Sse2.loadu_si128(ptr_v128++));

                    length -= 16;
                }

                acc0 = Min(acc0, acc1);
                acc2 = Min(acc2, acc3);
                acc0 = Min(acc0, acc2);

                if (Hint.Likely((int)length >= 4))
                {
                    acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));
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


                acc0 = Min(acc0, Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc0 = Min(acc0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 2;
                }

                acc0 = Min(acc0, Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Min(acc0, Sse2.cvtsi32_si128(*(int*)ptr_v128)).UInt0;
                }
                else
                {
                    return acc0.UInt0;
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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Minimum((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Minimum(ulong* ptr, long length)
        {
            static v256 Min256(v256 a, v256 b, v256 mask)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 greaterMask = Avx2.mm256_cmpgt_epi64(Avx2.mm256_xor_si256(a, mask),
                                                              Avx2.mm256_xor_si256(b, mask));

                    return Avx2.mm256_blendv_epi8(a, b, greaterMask);
                }
                else
                {
                    return default(v256);
                }
            }

            static v128 Min128(v128 a, v128 b, v128 mask)
            {
                if (Sse4_2.IsSse42Supported)
                {
                    v128 greaterMask = Sse4_2.cmpgt_epi64(Sse2.xor_si128(a, mask),
                                                          Sse2.xor_si128(b, mask));

                    return Sse4_1.blendv_epi8(a, b, greaterMask);
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
                v256 acc0 = Avx.mm256_set1_epi64x(unchecked((long)ulong.MaxValue));
                v256 acc1 = Avx.mm256_set1_epi64x(unchecked((long)ulong.MaxValue));
                v256 acc2 = Avx.mm256_set1_epi64x(unchecked((long)ulong.MaxValue));
                v256 acc3 = Avx.mm256_set1_epi64x(unchecked((long)ulong.MaxValue));

                v256 mask = Avx.mm256_set1_epi64x(1L << 63);

                while (Hint.Likely(length >= 16))
                {
                    acc0 = Min256(acc0, Avx.mm256_loadu_si256(ptr_v256++), mask);
                    acc1 = Min256(acc1, Avx.mm256_loadu_si256(ptr_v256++), mask);
                    acc2 = Min256(acc2, Avx.mm256_loadu_si256(ptr_v256++), mask);
                    acc3 = Min256(acc3, Avx.mm256_loadu_si256(ptr_v256++), mask);

                    length -= 16;
                }

                acc0 = Min256(acc0, acc1, mask);
                acc2 = Min256(acc2, acc3, mask);
                acc0 = Min256(acc0, acc2, mask);

                if (Hint.Likely((int)length >= 4))
                {
                    acc0 = Min256(acc0, Avx.mm256_loadu_si256(ptr_v256++), mask);

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        acc0 = Min256(acc0, Avx.mm256_loadu_si256(ptr_v256++), mask);

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            acc0 = Min256(acc0, Avx.mm256_loadu_si256(ptr_v256++), mask);
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

                v128 acc128 = Min128(Avx.mm256_castsi256_si128(acc0), Avx2.mm256_extracti128_si256(acc0, 1), Avx.mm256_castsi256_si128(mask));

                if (Hint.Likely((int)length >= 2))
                {
                    acc128 = Min128(acc128, Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(mask));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 2;
                }

                if (Hint.Likely(length != 0))
                {
                    return math.min(*(ulong*)ptr_v256, math.min(acc128.ULong0, acc128.ULong1));
                }
                else
                {
                    return math.min(acc128.ULong0, acc128.ULong1);
                }
            }
            else if (Sse4_2.IsSse42Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 acc0 = Sse2.set1_epi64x(unchecked((long)ulong.MaxValue));
                v128 acc1 = Sse2.set1_epi64x(unchecked((long)ulong.MaxValue));
                v128 acc2 = Sse2.set1_epi64x(unchecked((long)ulong.MaxValue));
                v128 acc3 = Sse2.set1_epi64x(unchecked((long)ulong.MaxValue));

                v128 mask = Sse2.set1_epi64x(1L << 63);

                while (Hint.Likely(length >= 8))
                {
                    acc0 = Min128(acc0, Sse2.loadu_si128(ptr_v128++), mask);
                    acc1 = Min128(acc1, Sse2.loadu_si128(ptr_v128++), mask);
                    acc2 = Min128(acc2, Sse2.loadu_si128(ptr_v128++), mask);
                    acc3 = Min128(acc3, Sse2.loadu_si128(ptr_v128++), mask);

                    length -= 8;
                }

                acc0 = Min128(acc0, acc1, mask);
                acc2 = Min128(acc2, acc3, mask);
                acc0 = Min128(acc0, acc2, mask);

                if (Hint.Likely((int)length >= 2))
                {
                    acc0 = Min128(acc0, Sse2.loadu_si128(ptr_v128++), mask);

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        acc0 = Min128(acc0, Sse2.loadu_si128(ptr_v128++), mask);

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            acc0 = Min128(acc0, Sse2.loadu_si128(ptr_v128++), mask);
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
                    return math.min(*(ulong*)ptr_v128, math.min(acc0.ULong0, acc0.ULong1));
                }
                else
                {
                    return math.min(acc0.ULong0, acc0.ULong1);
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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Minimum((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte SIMD_Minimum(sbyte* ptr, long length)
        {
            static v128 Min(v128 a, v128 b)
            {
                if (Sse4_1.IsSse41Supported)
                {
                    return Sse4_1.min_epi8(a, b);
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 greaterMask = Sse2.cmpgt_epi8(a, b);

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
                v256 acc0 = Avx.mm256_set1_epi8((byte)sbyte.MaxValue);
                v256 acc1 = Avx.mm256_set1_epi8((byte)sbyte.MaxValue);
                v256 acc2 = Avx.mm256_set1_epi8((byte)sbyte.MaxValue);
                v256 acc3 = Avx.mm256_set1_epi8((byte)sbyte.MaxValue);

                while (Hint.Likely(length >= 128))
                {
                    acc0 = Avx2.mm256_min_epi8(acc0, Avx.mm256_loadu_si256(ptr_v256++));
                    acc1 = Avx2.mm256_min_epi8(acc1, Avx.mm256_loadu_si256(ptr_v256++));
                    acc2 = Avx2.mm256_min_epi8(acc2, Avx.mm256_loadu_si256(ptr_v256++));
                    acc3 = Avx2.mm256_min_epi8(acc3, Avx.mm256_loadu_si256(ptr_v256++));

                    length -= 128;
                }

                acc0 = Avx2.mm256_min_epi8(acc0, acc1);
                acc2 = Avx2.mm256_min_epi8(acc2, acc3);
                acc0 = Avx2.mm256_min_epi8(acc0, acc2);

                if (Hint.Likely((int)length >= 32))
                {
                    acc0 = Avx2.mm256_min_epi8(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 32))
                    {
                        acc0 = Avx2.mm256_min_epi8(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 32))
                        {
                            acc0 = Avx2.mm256_min_epi8(acc0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 acc128 = Sse4_1.min_epi8(Avx.mm256_castsi256_si128(acc0), Avx2.mm256_extracti128_si256(acc0, 1));

                if (Hint.Likely((int)length >= 16))
                {
                    acc128 = Sse4_1.min_epi8(acc128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 16;
                }

                v128 cmp = default(v128);
                acc128 = Sse4_1.min_epi8(acc128, Sse2.shuffle_epi32(acc128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    acc128 = Sse4_1.min_epi8(acc128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 8;
                }

                acc128 = Sse4_1.min_epi8(acc128, Sse2.shufflelo_epi16(acc128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    acc128 = Sse4_1.min_epi8(acc128, Sse2.cvtsi32_si128(*(int*)ptr_v256));
                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 4;
                }

                acc128 = Sse4_1.min_epi8(acc128, Sse2.shufflelo_epi16(acc128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc128 = Sse4_1.min_epi8(acc128, Sse2.insert_epi16(cmp, *(short*)ptr_v256, 0));
                    ptr_v256 = (v256*)((short*)ptr_v256 + 1);
                    length -= 2;
                }

                acc128 = Sse4_1.min_epi8(acc128, Sse2.bsrli_si128(acc128, 1 * sizeof(sbyte)));

                if (Hint.Likely(length != 0))
                {
                    return (sbyte)math.min((int)acc128.SByte0, (int)(*(sbyte*)ptr_v256));
                }
                else
                {
                    return acc128.SByte0;
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 acc0 = Sse2.set1_epi8(sbyte.MaxValue);
                v128 acc1 = Sse2.set1_epi8(sbyte.MaxValue);
                v128 acc2 = Sse2.set1_epi8(sbyte.MaxValue);
                v128 acc3 = Sse2.set1_epi8(sbyte.MaxValue);

                while (Hint.Likely(length >= 64))
                {
                    acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));
                    acc1 = Min(acc1, Sse2.loadu_si128(ptr_v128++));
                    acc2 = Min(acc2, Sse2.loadu_si128(ptr_v128++));
                    acc3 = Min(acc3, Sse2.loadu_si128(ptr_v128++));

                    length -= 64;
                }

                acc0 = Min(acc0, acc1);
                acc2 = Min(acc2, acc3);
                acc0 = Min(acc0, acc2);

                if (Hint.Likely((int)length >= 16))
                {
                    acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));
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
                acc0 = Min(acc0, Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 8))
                {
                    acc0 = Min(acc0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 8;
                }

                acc0 = Sse2.min_epu8(acc0, Sse2.shufflelo_epi16(acc0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    acc0 = Min(acc0, Sse2.cvtsi32_si128(*(int*)ptr_v128));

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 4;
                }

                acc0 = Min(acc0, Sse2.shufflelo_epi16(acc0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc0 = Min(acc0, Sse2.insert_epi16(cmp, *(short*)ptr_v128, 0));
                    ptr_v128 = (v128*)((short*)ptr_v128 + 1);
                    length -= 2;
                }

                acc0 = Min(acc0, Sse2.bsrli_si128(acc0, 1 * sizeof(sbyte)));

                if (Hint.Likely(length != 0))
                {
                    if (Sse4_1.IsSse41Supported)
                    {
                        return Sse4_1.min_epi8(acc0, Sse4_1.insert_epi8(cmp, *(byte*)ptr_v128, 0)).SByte0;
                    }
                    else
                    {
                        return (sbyte)math.min((int)acc0.SByte0, (int)(*(sbyte*)ptr_v128));
                    }
                }
                else
                {
                    return acc0.SByte0;
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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Minimum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short SIMD_Minimum(short* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 acc0 = Avx.mm256_set1_epi16(short.MaxValue);
                v256 acc1 = Avx.mm256_set1_epi16(short.MaxValue);
                v256 acc2 = Avx.mm256_set1_epi16(short.MaxValue);
                v256 acc3 = Avx.mm256_set1_epi16(short.MaxValue);

                while (Hint.Likely(length >= 64))
                {
                    acc0 = Avx2.mm256_min_epi16(acc0, Avx.mm256_loadu_si256(ptr_v256++));
                    acc1 = Avx2.mm256_min_epi16(acc1, Avx.mm256_loadu_si256(ptr_v256++));
                    acc2 = Avx2.mm256_min_epi16(acc2, Avx.mm256_loadu_si256(ptr_v256++));
                    acc3 = Avx2.mm256_min_epi16(acc3, Avx.mm256_loadu_si256(ptr_v256++));

                    length -= 64;
                }

                acc0 = Avx2.mm256_min_epi16(acc0, acc1);
                acc2 = Avx2.mm256_min_epi16(acc2, acc3);
                acc0 = Avx2.mm256_min_epi16(acc0, acc2);

                if (Hint.Likely((int)length >= 16))
                {
                    acc0 = Avx2.mm256_min_epi16(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        acc0 = Avx2.mm256_min_epi16(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            acc0 = Avx2.mm256_min_epi16(acc0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 acc128 = Sse2.min_epi16(Avx.mm256_castsi256_si128(acc0), Avx2.mm256_extracti128_si256(acc0, 1));

                if (Hint.Likely((int)length >= 8))
                {
                    acc128 = Sse2.min_epi16(acc128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 8;
                }

                v128 cmp = default(v128);
                acc128 = Sse2.min_epi16(acc128, Sse2.shuffle_epi32(acc128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    acc128 = Sse2.min_epi16(acc128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 4;
                }

                acc128 = Sse2.min_epi16(acc128, Sse2.shufflelo_epi16(acc128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc128 = Sse2.min_epi16(acc128, Sse2.cvtsi32_si128(*(int*)ptr_v256));
                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 2;
                }

                acc128 = Sse2.min_epi16(acc128, Sse2.shufflelo_epi16(acc128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Sse2.min_epi16(acc128, Sse2.insert_epi16(cmp, *(short*)ptr_v256, 0)).SShort0;
                }
                else
                {
                    return acc128.SShort0;
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 acc0 = Sse2.set1_epi16(short.MaxValue);
                v128 acc1 = Sse2.set1_epi16(short.MaxValue);
                v128 acc2 = Sse2.set1_epi16(short.MaxValue);
                v128 acc3 = Sse2.set1_epi16(short.MaxValue);

                while (Hint.Likely(length >= 32))
                {
                    acc0 = Sse2.min_epi16(acc0, Sse2.loadu_si128(ptr_v128++));
                    acc1 = Sse2.min_epi16(acc1, Sse2.loadu_si128(ptr_v128++));
                    acc2 = Sse2.min_epi16(acc2, Sse2.loadu_si128(ptr_v128++));
                    acc3 = Sse2.min_epi16(acc3, Sse2.loadu_si128(ptr_v128++));

                    length -= 32;
                }

                acc0 = Sse2.min_epi16(acc0, acc1);
                acc2 = Sse2.min_epi16(acc2, acc3);
                acc0 = Sse2.min_epi16(acc0, acc2);

                if (Hint.Likely((int)length >= 8))
                {
                    acc0 = Sse2.min_epi16(acc0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        acc0 = Sse2.min_epi16(acc0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            acc0 = Sse2.min_epi16(acc0, Sse2.loadu_si128(ptr_v128++));
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
                acc0 = Sse2.min_epi16(acc0, Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 4))
                {
                    acc0 = Sse2.min_epi16(acc0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 4;
                }

                acc0 = Sse2.min_epi16(acc0, Sse2.shufflelo_epi16(acc0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc0 = Sse2.min_epi16(acc0, Sse2.cvtsi32_si128(*(int*)ptr_v128));

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 2;
                }

                acc0 = Sse2.min_epi16(acc0, Sse2.shufflelo_epi16(acc0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Sse2.min_epi16(acc0, Sse2.insert_epi16(cmp, *(short*)ptr_v128, 0)).SShort0;
                }
                else
                {
                    return acc0.SShort0;
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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Minimum((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Minimum(int* ptr, long length)
        {
            static v128 Min(v128 a, v128 b)
            {
                if (Sse4_1.IsSse41Supported)
                {
                    return Sse4_1.min_epi32(a, b);
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 greaterMask = Sse2.cmpgt_epi32(a, b);

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
                v256 acc0 = Avx.mm256_set1_epi32(int.MaxValue);
                v256 acc1 = Avx.mm256_set1_epi32(int.MaxValue);
                v256 acc2 = Avx.mm256_set1_epi32(int.MaxValue);
                v256 acc3 = Avx.mm256_set1_epi32(int.MaxValue);

                while (Hint.Likely(length >= 32))
                {
                    acc0 = Avx2.mm256_min_epi32(acc0, Avx.mm256_loadu_si256(ptr_v256++));
                    acc1 = Avx2.mm256_min_epi32(acc1, Avx.mm256_loadu_si256(ptr_v256++));
                    acc2 = Avx2.mm256_min_epi32(acc2, Avx.mm256_loadu_si256(ptr_v256++));
                    acc3 = Avx2.mm256_min_epi32(acc3, Avx.mm256_loadu_si256(ptr_v256++));

                    length -= 32;
                }

                acc0 = Avx2.mm256_min_epi32(acc0, acc1);
                acc2 = Avx2.mm256_min_epi32(acc2, acc3);
                acc0 = Avx2.mm256_min_epi32(acc0, acc2);

                if (Hint.Likely((int)length >= 8))
                {
                    acc0 = Avx2.mm256_min_epi32(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        acc0 = Avx2.mm256_min_epi32(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            acc0 = Avx2.mm256_min_epi32(acc0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 acc128 = Sse4_1.min_epi32(Avx.mm256_castsi256_si128(acc0), Avx2.mm256_extracti128_si256(acc0, 1));

                if (Hint.Likely((int)length >= 4))
                {
                    acc128 = Sse4_1.min_epi32(acc128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 4;
                }

                acc128 = Sse4_1.min_epi32(acc128, Sse2.shuffle_epi32(acc128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc128 = Sse4_1.min_epi32(acc128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 2;
                }

                acc128 = Sse4_1.min_epi32(acc128, Sse2.shuffle_epi32(acc128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Sse4_1.min_epi32(acc128, Sse2.cvtsi32_si128(*(int*)ptr_v256)).SInt0;
                }
                else
                {
                    return acc128.SInt0;
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 acc0 = Sse2.set1_epi32(int.MaxValue);
                v128 acc1 = Sse2.set1_epi32(int.MaxValue);
                v128 acc2 = Sse2.set1_epi32(int.MaxValue);
                v128 acc3 = Sse2.set1_epi32(int.MaxValue);

                while (Hint.Likely(length >= 16))
                {
                    acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));
                    acc1 = Min(acc1, Sse2.loadu_si128(ptr_v128++));
                    acc2 = Min(acc2, Sse2.loadu_si128(ptr_v128++));
                    acc3 = Min(acc3, Sse2.loadu_si128(ptr_v128++));

                    length -= 16;
                }

                acc0 = Min(acc0, acc1);
                acc2 = Min(acc2, acc3);
                acc0 = Min(acc0, acc2);

                if (Hint.Likely((int)length >= 4))
                {
                    acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            acc0 = Min(acc0, Sse2.loadu_si128(ptr_v128++));
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

                acc0 = Min(acc0, Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc0 = Min(acc0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 2;
                }

                acc0 = Min(acc0, Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    return Min(acc0, Sse2.cvtsi32_si128(*(int*)ptr_v128)).SInt0;
                }
                else
                {
                    return acc0.SInt0;
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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Minimum((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Minimum(long* ptr, long length)
        {
            static v256 Min256(v256 a, v256 b)
            {
                return Avx2.mm256_blendv_epi8(a, b, Avx2.mm256_cmpgt_epi64(a, b));
            }

            static v128 Min128(v128 a, v128 b)
            {
                return Sse4_1.blendv_epi8(a, b, Sse4_2.cmpgt_epi64(a, b));
            }

Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 acc0 = Avx.mm256_set1_epi64x(long.MaxValue);
                v256 acc1 = Avx.mm256_set1_epi64x(long.MaxValue);
                v256 acc2 = Avx.mm256_set1_epi64x(long.MaxValue);
                v256 acc3 = Avx.mm256_set1_epi64x(long.MaxValue);

                while (Hint.Likely(length >= 16))
                {
                    acc0 = Min256(acc0, Avx.mm256_loadu_si256(ptr_v256++));
                    acc1 = Min256(acc1, Avx.mm256_loadu_si256(ptr_v256++));
                    acc2 = Min256(acc2, Avx.mm256_loadu_si256(ptr_v256++));
                    acc3 = Min256(acc3, Avx.mm256_loadu_si256(ptr_v256++));

                    length -= 16;
                }

                acc0 = Min256(acc0, acc1);
                acc2 = Min256(acc2, acc3);
                acc0 = Min256(acc0, acc2);

                if (Hint.Likely((int)length >= 4))
                {
                    acc0 = Min256(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        acc0 = Min256(acc0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            acc0 = Min256(acc0, Avx.mm256_loadu_si256(ptr_v256++));
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

                v128 acc128 = Min128(Avx.mm256_castsi256_si128(acc0), Avx2.mm256_extracti128_si256(acc0, 1));

                if (Hint.Likely((int)length >= 2))
                {
                    acc128 = Min128(acc128, Sse2.loadu_si128(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 2;
                }

                acc128 = Min128(acc128, Sse2.shuffle_epi32(acc128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    return math.min(acc128.SLong0, *(long*)ptr_v256);
                }
                else
                {
                    return acc128.SLong0;
                }
            }
            else if (Sse4_2.IsSse42Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 acc0 = Sse2.set1_epi64x(long.MaxValue);
                v128 acc1 = Sse2.set1_epi64x(long.MaxValue);
                v128 acc2 = Sse2.set1_epi64x(long.MaxValue);
                v128 acc3 = Sse2.set1_epi64x(long.MaxValue);

                while (Hint.Likely(length >= 8))
                {
                    acc0 = Min128(acc0, Sse2.loadu_si128(ptr_v128++));
                    acc1 = Min128(acc1, Sse2.loadu_si128(ptr_v128++));
                    acc2 = Min128(acc2, Sse2.loadu_si128(ptr_v128++));
                    acc3 = Min128(acc3, Sse2.loadu_si128(ptr_v128++));

                    length -= 8;
                }

                acc0 = Min128(acc0, acc1);
                acc2 = Min128(acc2, acc3);
                acc0 = Min128(acc0, acc2);

                if (Hint.Likely((int)length >= 2))
                {
                    acc0 = Min128(acc0, Sse2.loadu_si128(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        acc0 = Min128(acc0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            acc0 = Min128(acc0, Sse2.loadu_si128(ptr_v128++));
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

                acc0 = Min128(acc0, Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    return math.min(acc0.SLong0, *(long*)ptr_v128);
                }
                else
                {
                    return acc0.SLong0;
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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Minimum((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Minimum(float* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx.IsAvxSupported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 acc0 = new v256(float.PositiveInfinity);
                v256 acc1 = new v256(float.PositiveInfinity);
                v256 acc2 = new v256(float.PositiveInfinity);
                v256 acc3 = new v256(float.PositiveInfinity);

                while (Hint.Likely(length >= 32))
                {
                    acc0 = Avx.mm256_min_ps(acc0, Avx.mm256_loadu_ps(ptr_v256++));
                    acc1 = Avx.mm256_min_ps(acc1, Avx.mm256_loadu_ps(ptr_v256++));
                    acc2 = Avx.mm256_min_ps(acc2, Avx.mm256_loadu_ps(ptr_v256++));
                    acc3 = Avx.mm256_min_ps(acc3, Avx.mm256_loadu_ps(ptr_v256++));

                    length -= 32;
                }

                acc0 = Avx.mm256_min_ps(acc0, acc1);
                acc2 = Avx.mm256_min_ps(acc2, acc3);
                acc0 = Avx.mm256_min_ps(acc0, acc2);

                if (Hint.Likely((int)length >= 8))
                {
                    acc0 = Avx.mm256_min_ps(acc0, Avx.mm256_loadu_ps(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        acc0 = Avx.mm256_min_ps(acc0, Avx.mm256_loadu_ps(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            acc0 = Avx.mm256_min_ps(acc0, Avx.mm256_loadu_ps(ptr_v256++));
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

                v128 acc128 = Sse.min_ps(Avx.mm256_castps256_ps128(acc0), Avx.mm256_extractf128_ps(acc0, 1));

                if (Hint.Likely((int)length >= 4))
                {
                    acc128 = Sse.min_ps(acc128, Sse.loadu_ps(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 4;
                }

                acc128 = Sse.min_ps(acc128, Avx.permute_ps(acc128, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc128 = Sse.min_ps(acc128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));

                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 2;
                }

                acc128 = Sse.min_ps(acc128, Avx.permute_ps(acc128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    acc128 = Sse.min_ss(acc128, Sse2.cvtsi32_si128(*(int*)ptr_v256));
                }

                return acc128.Float0;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 acc0 = new v128(float.PositiveInfinity);
                v128 acc1 = new v128(float.PositiveInfinity);
                v128 acc2 = new v128(float.PositiveInfinity);
                v128 acc3 = new v128(float.PositiveInfinity);

                while (Hint.Likely(length >= 16))
                {
                    acc0 = Sse.min_ps(acc0, Sse.loadu_ps(ptr_v128++));
                    acc1 = Sse.min_ps(acc1, Sse.loadu_ps(ptr_v128++));
                    acc2 = Sse.min_ps(acc2, Sse.loadu_ps(ptr_v128++));
                    acc3 = Sse.min_ps(acc3, Sse.loadu_ps(ptr_v128++));

                    length -= 16;
                }

                acc0 = Sse.min_ps(acc0, acc1);
                acc2 = Sse.min_ps(acc2, acc3);
                acc0 = Sse.min_ps(acc0, acc2);

                if (Hint.Likely((int)length >= 4))
                {
                    acc0 = Sse.min_ps(acc0, Sse.loadu_ps(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        acc0 = Sse.min_ps(acc0, Sse.loadu_ps(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            acc0 = Sse.min_ps(acc0, Sse.loadu_ps(ptr_v128++));
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

                acc0 = Sse.min_ps(acc0, Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely((int)length >= 2))
                {
                    acc0 = Sse.min_ps(acc0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 2;
                }

                acc0 = Sse.min_ps(acc0, Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    acc0 = Sse.min_ss(acc0, Sse2.cvtsi32_si128(*(int*)ptr_v128));
                }

                return acc0.Float0;
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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Minimum((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Minimum(double* ptr, long length)
        {
Assert.IsNonNegative(length);

            if (Avx.IsAvxSupported)
            {
                v256* ptr_v256 = (v256*)ptr;
                v256 acc0 = new v256(double.PositiveInfinity);
                v256 acc1 = new v256(double.PositiveInfinity);
                v256 acc2 = new v256(double.PositiveInfinity);
                v256 acc3 = new v256(double.PositiveInfinity);

                while (Hint.Likely(length >= 16))
                {
                    acc0 = Avx.mm256_min_pd(acc0, Avx.mm256_loadu_pd(ptr_v256++));
                    acc1 = Avx.mm256_min_pd(acc1, Avx.mm256_loadu_pd(ptr_v256++));
                    acc2 = Avx.mm256_min_pd(acc2, Avx.mm256_loadu_pd(ptr_v256++));
                    acc3 = Avx.mm256_min_pd(acc3, Avx.mm256_loadu_pd(ptr_v256++));

                    length -= 16;
                }

                acc0 = Avx.mm256_min_pd(acc0, acc1);
                acc2 = Avx.mm256_min_pd(acc2, acc3);
                acc0 = Avx.mm256_min_pd(acc0, acc2);

                if (Hint.Likely((int)length >= 4))
                {
                    acc0 = Avx.mm256_min_pd(acc0, Avx.mm256_loadu_pd(ptr_v256++));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        acc0 = Avx.mm256_min_pd(acc0, Avx.mm256_loadu_pd(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            acc0 = Avx.mm256_min_pd(acc0, Avx.mm256_loadu_pd(ptr_v256++));
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

                v128 acc128 = Sse2.min_pd(Avx.mm256_castpd256_pd128(acc0), Avx.mm256_extractf128_pd(acc0, 1));

                if (Hint.Likely((int)length >= 2))
                {
                    acc128 = Sse2.min_pd(acc128, Sse.loadu_ps(ptr_v256));
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 2;
                }

                acc128 = Sse2.min_pd(acc128, Avx.permute_pd(acc128, Sse.SHUFFLE(0, 0, 0, 1)));

                if (Hint.Likely(length != 0))
                {
                    acc128 = Sse2.min_sd(acc128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                }

                return acc128.Double0;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128* ptr_v128 = (v128*)ptr;
                v128 acc0 = new v128(double.PositiveInfinity);
                v128 acc1 = new v128(double.PositiveInfinity);
                v128 acc2 = new v128(double.PositiveInfinity);
                v128 acc3 = new v128(double.PositiveInfinity);

                while (Hint.Likely(length >= 8))
                {
                    acc0 = Sse2.min_pd(acc0, Sse.loadu_ps(ptr_v128++));
                    acc1 = Sse2.min_pd(acc1, Sse.loadu_ps(ptr_v128++));
                    acc2 = Sse2.min_pd(acc2, Sse.loadu_ps(ptr_v128++));
                    acc3 = Sse2.min_pd(acc3, Sse.loadu_ps(ptr_v128++));

                    length -= 8;
                }

                acc0 = Sse2.min_pd(acc0, acc1);
                acc2 = Sse2.min_pd(acc2, acc3);
                acc0 = Sse2.min_pd(acc0, acc2);

                if (Hint.Likely((int)length >= 2))
                {
                    acc0 = Sse2.min_pd(acc0, Sse.loadu_ps(ptr_v128++));

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        acc0 = Sse2.min_pd(acc0, Sse.loadu_ps(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            acc0 = Sse2.min_pd(acc0, Sse.loadu_ps(ptr_v128++));
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

                acc0 = Sse2.min_pd(acc0, Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 3, 2)));

                if (Hint.Likely(length != 0))
                {
                    acc0 = Sse2.min_sd(acc0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));
                }

                return acc0.Double0;
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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

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
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Minimum((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }
    }
}
