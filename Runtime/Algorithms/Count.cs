using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst.Intrinsics;
using Unity.Burst.CompilerServices;
using DevTools;

using static Unity.Burst.Intrinsics.X86;

namespace SIMDAlgorithms
{
    unsafe public static partial class Algorithms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(bool* ptr, long length, bool value = true)
        {
Assert.IsNonNegative(length);
Assert.IsSafeBoolean(value);
#if DEBUG
byte* ptr_byte = (byte*)ptr;
for (int i = 0; i < length; i++)
{
    if (ptr_byte[i] > 1)
    {
        throw new ArgumentException($"The array contains a boolean value at index { i } with a numerical representation other than 0 or 1, resulting in undefined behavior.");
    }
}
#endif
            if (Avx2.IsAvx2Supported)
            {
                ulong originalLength = (ulong)length;
                v256* ptr_v256 = (v256*)ptr;
                v256 ZERO = default(v256);
                v256 count0;
                v256 count1;
                v256 count2;
                v256 count3;
                v256 longs = default(v256);

                while (Hint.Likely(length >= 255 * 32 * 4))
                {
                    count0 = default(v256);
                    count1 = default(v256);
                    count2 = default(v256);
                    count3 = default(v256);

                    for (int i = 0; i < 255; i++)
                    {
                        count0 = Avx2.mm256_add_epi8(count0, Avx.mm256_loadu_si256(ptr_v256++));
                        count1 = Avx2.mm256_add_epi8(count1, Avx.mm256_loadu_si256(ptr_v256++));
                        count2 = Avx2.mm256_add_epi8(count2, Avx.mm256_loadu_si256(ptr_v256++));
                        count3 = Avx2.mm256_add_epi8(count3, Avx.mm256_loadu_si256(ptr_v256++));
                    }

                    v256 sad0 = Avx2.mm256_sad_epu8(count0, ZERO);
                    v256 sad1 = Avx2.mm256_sad_epu8(count1, ZERO);
                    v256 sad2 = Avx2.mm256_sad_epu8(count2, ZERO);
                    v256 sad3 = Avx2.mm256_sad_epu8(count3, ZERO);

                    v256 csum0 = Avx2.mm256_add_epi64(sad0, sad1);
                    v256 csum1 = Avx2.mm256_add_epi64(sad2, sad3);
                    v256 csum2 = Avx2.mm256_add_epi64(csum0, csum1);
                    longs = Avx2.mm256_add_epi64(longs, csum2);


                    length -= 255 * 32 * 4;
                }


                {
                    count0 = default(v256);
                    count1 = default(v256);
                    count2 = default(v256);
                    count3 = default(v256);

                    while (Hint.Likely((int)length >= 32 * 4))
                    {
                        count0 = Avx2.mm256_add_epi8(count0, Avx.mm256_loadu_si256(ptr_v256++));
                        count1 = Avx2.mm256_add_epi8(count1, Avx.mm256_loadu_si256(ptr_v256++));
                        count2 = Avx2.mm256_add_epi8(count2, Avx.mm256_loadu_si256(ptr_v256++));
                        count3 = Avx2.mm256_add_epi8(count3, Avx.mm256_loadu_si256(ptr_v256++));

                        length -= 32 * 4;
                    }

                    v256 sad0 = Avx2.mm256_sad_epu8(count0, ZERO);
                    v256 sad1 = Avx2.mm256_sad_epu8(count1, ZERO);
                    v256 sad2 = Avx2.mm256_sad_epu8(count2, ZERO);
                    v256 sad3 = Avx2.mm256_sad_epu8(count3, ZERO);

                    v256 csum0 = Avx2.mm256_add_epi64(sad0, sad1);
                    v256 csum1 = Avx2.mm256_add_epi64(sad2, sad3);
                    v256 csum2 = Avx2.mm256_add_epi64(csum0, csum1);
                    longs = Avx2.mm256_add_epi64(longs, csum2);
                }


                if (Hint.Likely((int)length >= 32))
                {
                    count0 = Avx.mm256_loadu_si256(ptr_v256++);

                    if (Hint.Likely((int)length >= 2 * 32))
                    {
                        count0 = Avx2.mm256_add_epi8(count0, Avx.mm256_loadu_si256(ptr_v256++));

                        if (Hint.Likely((int)length >= 3 * 32))
                        {
                            count0 = Avx2.mm256_add_epi8(count0, Avx.mm256_loadu_si256(ptr_v256++));
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

                    longs = Avx2.mm256_add_epi64(longs, Avx2.mm256_sad_epu8(count0, ZERO));
                }
                else { }


                v128 bytes;
                v128 longs128 = Sse2.add_epi64(Avx.mm256_castsi256_si128(longs), Avx2.mm256_extracti128_si256(longs, 1));

                if (Hint.Likely((int)length >= 16))
                {
                    bytes = Sse2.loadu_si128(ptr_v256);
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                    length -= 16;
                }
                else
                {
                    bytes = default(v128);
                }


                if (Hint.Likely((int)length >= 8))
                {
                    bytes = Sse2.add_epi8(bytes, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);

                    length -= 8;
                }
                else { }


                if (Hint.Likely((int)length >= 4))
                {
                    bytes = Sse2.add_epi8(bytes, Sse2.cvtsi32_si128(*(int*)ptr_v256));
                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);

                    length -= 4;
                }
                else { }


                if (Hint.Likely((int)length >= 2))
                {
                    bytes = Sse2.add_epi8(bytes, Sse2.insert_epi16(default(v128), *(ushort*)ptr_v256, 0));
                    ptr_v256 = (v256*)((ushort*)ptr_v256 + 1);

                    length -= 2;
                }
                else { }


                longs128 = Sse2.add_epi64(longs128, Sse2.sad_epu8(bytes, Avx.mm256_castsi256_si128(ZERO)));
                longs128 = Sse2.add_epi64(longs128, Sse2.shuffle_epi32(longs128, Sse.SHUFFLE(0, 0, 3, 2)));

                ulong countTotal = longs128.ULong0;

                if (Hint.Likely(length != 0))
                {
                    countTotal += *(byte*)ptr_v256;
                }
                else { }


                if (value == true)
                {
                    return countTotal;
                }
                else
                {
                    return originalLength - countTotal;
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                ulong originalLength = (ulong)length;
                v128* ptr_v128 = (v128*)ptr;
                v128 ZERO = default(v128);
                v128 count0;
                v128 count1;
                v128 count2;
                v128 count3;
                v128 longs = default(v128);

                while (Hint.Likely(length >= 255 * 16 * 4))
                {
                    count0 = default(v128);
                    count1 = default(v128);
                    count2 = default(v128);
                    count3 = default(v128);

                    for (int i = 0; i < 255; i++)
                    {
                        count0 = Sse2.add_epi8(count0, Sse2.loadu_si128(ptr_v128++));
                        count1 = Sse2.add_epi8(count1, Sse2.loadu_si128(ptr_v128++));
                        count2 = Sse2.add_epi8(count2, Sse2.loadu_si128(ptr_v128++));
                        count3 = Sse2.add_epi8(count3, Sse2.loadu_si128(ptr_v128++));
                    }

                    v128 sad0 = Sse2.sad_epu8(count0, ZERO);
                    v128 sad1 = Sse2.sad_epu8(count1, ZERO);
                    v128 sad2 = Sse2.sad_epu8(count2, ZERO);
                    v128 sad3 = Sse2.sad_epu8(count3, ZERO);

                    v128 csum0 = Sse2.add_epi64(sad0, sad1);
                    v128 csum1 = Sse2.add_epi64(sad2, sad3);
                    v128 csum2 = Sse2.add_epi64(csum0, csum1);
                    longs = Sse2.add_epi64(longs, csum2);


                    length -= 255 * 16 * 4;
                }


                {
                    count0 = default(v128);
                    count1 = default(v128);
                    count2 = default(v128);
                    count3 = default(v128);
                
                    while (Hint.Likely((int)length >= 16 * 4))
                    {
                        count0 = Sse2.add_epi8(count0, Sse2.loadu_si128(ptr_v128++));
                        count1 = Sse2.add_epi8(count1, Sse2.loadu_si128(ptr_v128++));
                        count2 = Sse2.add_epi8(count2, Sse2.loadu_si128(ptr_v128++));
                        count3 = Sse2.add_epi8(count3, Sse2.loadu_si128(ptr_v128++));
                
                        length -= 16 * 4;
                    }

                    v128 sad0 = Sse2.sad_epu8(count0, ZERO);
                    v128 sad1 = Sse2.sad_epu8(count1, ZERO);
                    v128 sad2 = Sse2.sad_epu8(count2, ZERO);
                    v128 sad3 = Sse2.sad_epu8(count3, ZERO);
                
                    v128 csum0 = Sse2.add_epi64(sad0, sad1);
                    v128 csum1 = Sse2.add_epi64(sad2, sad3);
                    v128 csum2 = Sse2.add_epi64(csum0, csum1);
                    longs = Sse2.add_epi64(longs, csum2);
                }


                if (Hint.Likely((int)length >= 16))
                {
                    count0 = Sse2.loadu_si128(ptr_v128++);

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        count0 = Sse2.add_epi8(count0, Sse2.loadu_si128(ptr_v128++));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            count0 = Sse2.add_epi8(count0, Sse2.loadu_si128(ptr_v128++));
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

                    longs = Sse2.add_epi64(longs, Sse2.sad_epu8(count0, ZERO));
                }
                else { }


                v128 bytes;

                if (Hint.Likely((int)length >= 16))
                {
                    bytes = Sse2.loadu_si128(ptr_v128++);

                    length -= 16;
                }
                else
                {
                    bytes = default(v128);
                }


                if (Hint.Likely((int)length >= 8))
                {
                    bytes = Sse2.add_epi8(bytes, Sse2.cvtsi64x_si128(*(long*)ptr_v128));
                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);

                    length -= 8;
                }
                else { }


                if (Hint.Likely((int)length >= 4))
                {
                    bytes = Sse2.add_epi8(bytes, Sse2.cvtsi32_si128(*(int*)ptr_v128));
                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);

                    length -= 4;
                }
                else { }


                if (Hint.Likely((int)length >= 2))
                {
                    bytes = Sse2.add_epi8(bytes, Sse2.insert_epi16(default(v128), *(ushort*)ptr_v128, 0));
                    ptr_v128 = (v128*)((ushort*)ptr_v128 + 1);

                    length -= 2;
                }
                else { }


                longs = Sse2.add_epi64(longs, Sse2.sad_epu8(bytes, ZERO));
                longs = Sse2.add_epi64(longs, Sse2.shuffle_epi32(longs, Sse.SHUFFLE(0, 0, 3, 2)));

                ulong countTotal = longs.ULong0;

                if (Hint.Likely(length != 0))
                {
                    countTotal += *(byte*)ptr_v128;
                }
                else { }


                if (value == true)
                {
                    return countTotal;
                }
                else
                {
                    return originalLength - countTotal;
                }
            }
            else
            {
                ulong count = 0;

                for (long i = 0; i < length; i++)
                {
                    count += *(byte*)ptr;
                    ptr++;
                }

                if (value == true)
                {
                    return count;
                }
                else
                {
                    return (ulong)length - count;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<bool> array, bool value = true)
        {
            return SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<bool> array, int index, bool value = true)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<bool> array, int index, int numEntries, bool value = true)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<bool> array, bool value = true)
        {
            return SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<bool> array, int index, bool value = true)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<bool> array, int index, int numEntries, bool value = true)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<bool> array, bool value = true)
        {
            return SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<bool> array, int index, bool value = true)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<bool> array, int index, int numEntries, bool value = true)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(byte* ptr, long length, byte value)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256 broadcast = Avx.mm256_set1_epi8(value);
                v256* ptr_v256 = (v256*)ptr;
                ulong count0 = 0;
                ulong count1 = 0;
                ulong count2 = 0;
                ulong count3 = 0;
                ulong countTotal = 0;

                while (Hint.Likely(length >= 128))
                {
                    v256 load0 = Avx.mm256_loadu_si256(ptr_v256++);
                    v256 load1 = Avx.mm256_loadu_si256(ptr_v256++);
                    v256 load2 = Avx.mm256_loadu_si256(ptr_v256++);
                    v256 load3 = Avx.mm256_loadu_si256(ptr_v256++);

                    length -= 128;

                    v256 cmpeq0 = Avx2.mm256_cmpeq_epi8(broadcast, load0);
                    v256 cmpeq1 = Avx2.mm256_cmpeq_epi8(broadcast, load1);
                    v256 cmpeq2 = Avx2.mm256_cmpeq_epi8(broadcast, load2);
                    v256 cmpeq3 = Avx2.mm256_cmpeq_epi8(broadcast, load3);

                    int mask0 = Avx2.mm256_movemask_epi8(cmpeq0);
                    int mask1 = Avx2.mm256_movemask_epi8(cmpeq1);
                    int mask2 = Avx2.mm256_movemask_epi8(cmpeq2);
                    int mask3 = Avx2.mm256_movemask_epi8(cmpeq3);

                    int tempCount0 = math.countbits(mask0);
                    int tempCount1 = math.countbits(mask1);
                    int tempCount2 = math.countbits(mask2);
                    int tempCount3 = math.countbits(mask3);

                    count0 += (uint)tempCount0;
                    count1 += (uint)tempCount1;
                    count2 += (uint)tempCount2;
                    count3 += (uint)tempCount3;
                }

                countTotal = (count0 + count1) + (count2 + count3);


                if (Hint.Likely((int)length >= 32))
                {
                    countTotal += (uint)math.countbits(Avx2.mm256_movemask_epi8(Avx2.mm256_cmpeq_epi8(broadcast, Avx.mm256_loadu_si256(ptr_v256++))));

                    if (Hint.Likely((int)length >= 2 * 32))
                    {
                        countTotal += (uint)math.countbits(Avx2.mm256_movemask_epi8(Avx2.mm256_cmpeq_epi8(broadcast, Avx.mm256_loadu_si256(ptr_v256++))));

                        if (Hint.Likely((int)length >= 3 * 32))
                        {
                            countTotal += (uint)math.countbits(Avx2.mm256_movemask_epi8(Avx2.mm256_cmpeq_epi8(broadcast, Avx.mm256_loadu_si256(ptr_v256++))));
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


                if (Hint.Likely((int)length >= 16))
                {
                    countTotal += (uint)math.countbits(Sse2.movemask_epi8(Sse2.cmpeq_epi8(Avx.mm256_castsi256_si128(broadcast), Sse2.loadu_si128(ptr_v256))));

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 16;
                }
                else { }


                if (Hint.Likely((int)length >= 8))
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(Avx.mm256_castsi256_si128(broadcast), Sse2.cvtsi64x_si128(*(long*)ptr_v256)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b1111_1111;
                    }

                    countTotal += (uint)math.countbits(mask);

                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 8;
                }
                else { }


                if (Hint.Likely((int)length >= 4))
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(Avx.mm256_castsi256_si128(broadcast), Sse2.cvtsi32_si128(*(int*)ptr_v256)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b1111;
                    }

                    countTotal += (uint)math.countbits(mask);

                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 4;
                }
                else { }


                if (Hint.Likely((int)length >= 2))
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(Avx.mm256_castsi256_si128(broadcast), Sse2.insert_epi16(default(v128), *(ushort*)ptr_v256, 0)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b0011;
                    }

                    countTotal += (uint)math.countbits(mask);

                    ptr_v256 = (v256*)((ushort*)ptr_v256 + 1);
                    length -= 2;
                }
                else { }


                if (Hint.Likely(length != 0))
                {
                    bool areEqual = *(byte*)ptr_v256 == value;
                    countTotal += *(byte*)&areEqual;
                }
                else { }


                return countTotal;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128 broadcast = Sse2.set1_epi8((sbyte)value);
                v128* ptr_v128 = (v128*)ptr;
                ulong count0 = 0;
                ulong count1 = 0;
                ulong count2 = 0;
                ulong count3 = 0;
                ulong countTotal = 0;

                while (Hint.Likely(length >= 64))
                {
                    v128 load0 = Sse2.loadu_si128(ptr_v128++);
                    v128 load1 = Sse2.loadu_si128(ptr_v128++);
                    v128 load2 = Sse2.loadu_si128(ptr_v128++);
                    v128 load3 = Sse2.loadu_si128(ptr_v128++);

                    length -= 64;

                    v128 cmpeq0 = Sse2.cmpeq_epi8(broadcast, load0);
                    v128 cmpeq1 = Sse2.cmpeq_epi8(broadcast, load1);
                    v128 cmpeq2 = Sse2.cmpeq_epi8(broadcast, load2);
                    v128 cmpeq3 = Sse2.cmpeq_epi8(broadcast, load3);

                    int mask0 = Sse2.movemask_epi8(cmpeq0);
                    int mask1 = Sse2.movemask_epi8(cmpeq1);
                    int mask2 = Sse2.movemask_epi8(cmpeq2);
                    int mask3 = Sse2.movemask_epi8(cmpeq3);

                    int tempCount0 = math.countbits(mask0);
                    int tempCount1 = math.countbits(mask1);
                    int tempCount2 = math.countbits(mask2);
                    int tempCount3 = math.countbits(mask3);

                    count0 += (uint)tempCount0;
                    count1 += (uint)tempCount1;
                    count2 += (uint)tempCount2;
                    count3 += (uint)tempCount3;
                }

                countTotal = (count0 + count1) + (count2 + count3);


                if (Hint.Likely((int)length >= 16))
                {
                    countTotal += (uint)math.countbits(Sse2.movemask_epi8(Sse2.cmpeq_epi8(broadcast, Sse2.loadu_si128(ptr_v128++))));

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        countTotal += (uint)math.countbits(Sse2.movemask_epi8(Sse2.cmpeq_epi8(broadcast, Sse2.loadu_si128(ptr_v128++))));

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            countTotal += (uint)math.countbits(Sse2.movemask_epi8(Sse2.cmpeq_epi8(broadcast, Sse2.loadu_si128(ptr_v128++))));
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


                if (Hint.Likely((int)length >= 8))
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(broadcast, Sse2.cvtsi64x_si128(*(long*)ptr_v128)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b1111_1111;
                    }

                    countTotal += (uint)math.countbits(mask);

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 8;
                }
                else { }


                if (Hint.Likely((int)length >= 4))
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(broadcast, Sse2.cvtsi32_si128(*(int*)ptr_v128)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b1111;
                    }

                    countTotal += (uint)math.countbits(mask);

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 4;
                }
                else { }


                if (Hint.Likely((int)length >= 2))
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(broadcast, Sse2.insert_epi16(default(v128), *(ushort*)ptr_v128, 0)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b0011;
                    }

                    countTotal += (uint)math.countbits(mask);

                    ptr_v128 = (v128*)((ushort*)ptr_v128 + 1);
                    length -= 2;
                }
                else { }


                if (Hint.Likely(length != 0))
                {
                    bool areEqual = *(byte*)ptr_v128 == value;
                    countTotal += *(byte*)&areEqual;
                }
                else { }


                return countTotal;
            }
            else
            {
                ulong count = 0;

                for (long i = 0; i < length; i++)
                {
                    bool areEqual = ptr[i] == value;
                    count += *(byte*)&areEqual;
                }

                return count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<byte> array, byte value)
        {
            return SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<byte> array, int index, byte value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<byte> array, int index, int numEntries, byte value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<byte> array, byte value)
        {
            return SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<byte> array, int index, byte value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<byte> array, int index, int numEntries, byte value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<byte> array, byte value)
        {
            return SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<byte> array, int index, byte value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<byte> array, int index, int numEntries, byte value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(ushort* ptr, long length, ushort value)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256 broadcast = Avx.mm256_set1_epi16((short)value);
                v256* ptr_v256 = (v256*)ptr;
                ulong count0 = 0;
                ulong count1 = 0;
                ulong count2 = 0;
                ulong count3 = 0;
                ulong countTotal = 0;

                while (Hint.Likely(length >= 64))
                {
                    v256 load0 = Avx.mm256_loadu_si256(ptr_v256++);
                    v256 load1 = Avx.mm256_loadu_si256(ptr_v256++);
                    v256 load2 = Avx.mm256_loadu_si256(ptr_v256++);
                    v256 load3 = Avx.mm256_loadu_si256(ptr_v256++);

                    length -= 64;

                    v256 cmpeq0 = Avx2.mm256_cmpeq_epi16(broadcast, load0);
                    v256 cmpeq1 = Avx2.mm256_cmpeq_epi16(broadcast, load1);
                    v256 cmpeq2 = Avx2.mm256_cmpeq_epi16(broadcast, load2);
                    v256 cmpeq3 = Avx2.mm256_cmpeq_epi16(broadcast, load3);

                    int mask0 = Avx2.mm256_movemask_epi8(cmpeq0);
                    int mask1 = Avx2.mm256_movemask_epi8(cmpeq1);
                    int mask2 = Avx2.mm256_movemask_epi8(cmpeq2);
                    int mask3 = Avx2.mm256_movemask_epi8(cmpeq3);

                    int tempCount0 = math.countbits(mask0);
                    int tempCount1 = math.countbits(mask1);
                    int tempCount2 = math.countbits(mask2);
                    int tempCount3 = math.countbits(mask3);
                    tempCount0 >>= 1;
                    tempCount1 >>= 1;
                    tempCount2 >>= 1;
                    tempCount3 >>= 1;

                    count0 += (uint)tempCount0;
                    count1 += (uint)tempCount1;
                    count2 += (uint)tempCount2;
                    count3 += (uint)tempCount3;
                }

                countTotal = (count0 + count1) + (count2 + count3);


                if (Hint.Likely((int)length >= 16))
                {
                    countTotal += (uint)math.countbits(Avx2.mm256_movemask_epi8(Avx2.mm256_cmpeq_epi16(broadcast, Avx.mm256_loadu_si256(ptr_v256++)))) >> 1;

                    if (Hint.Likely((int)length >= 2 * 16))
                    {
                        countTotal += (uint)math.countbits(Avx2.mm256_movemask_epi8(Avx2.mm256_cmpeq_epi16(broadcast, Avx.mm256_loadu_si256(ptr_v256++)))) >> 1;

                        if (Hint.Likely((int)length >= 3 * 16))
                        {
                            countTotal += (uint)math.countbits(Avx2.mm256_movemask_epi8(Avx2.mm256_cmpeq_epi16(broadcast, Avx.mm256_loadu_si256(ptr_v256++)))) >> 1;
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


                if (Hint.Likely((int)length >= 8))
                {
                    countTotal += (uint)math.countbits(Sse2.movemask_epi8(Sse2.cmpeq_epi16(Avx.mm256_castsi256_si128(broadcast), Sse2.loadu_si128(ptr_v256)))) >> 1;

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 8;
                }
                else { }

                if (Hint.Likely((int)length >= 4))
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi16(Avx.mm256_castsi256_si128(broadcast), Sse2.cvtsi64x_si128(*(long*)ptr_v256)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b1111_1111;
                    }

                    countTotal += (uint)math.countbits(mask) >> 1;

                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 4;
                }
                else { }

                if (Hint.Likely((int)length >= 2))
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi16(Avx.mm256_castsi256_si128(broadcast), Sse2.cvtsi32_si128(*(int*)ptr_v256)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b1111;
                    }

                    countTotal += (uint)math.countbits(mask) >> 1;

                    ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    length -= 2;
                }
                else { }

                if (Hint.Likely(length != 0))
                {
                    bool areEqual = *(ushort*)ptr_v256 == value;
                    countTotal += *(byte*)&areEqual;
                }
                else { }


                return countTotal;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128 broadcast = Sse2.set1_epi16((short)value);
                v128* ptr_v128 = (v128*)ptr;
                ulong count0 = 0;
                ulong count1 = 0;
                ulong count2 = 0;
                ulong count3 = 0;
                ulong countTotal = 0;

                while (Hint.Likely(length >= 32))
                {
                    v128 load0 = Sse2.loadu_si128(ptr_v128++);
                    v128 load1 = Sse2.loadu_si128(ptr_v128++);
                    v128 load2 = Sse2.loadu_si128(ptr_v128++);
                    v128 load3 = Sse2.loadu_si128(ptr_v128++);

                    length -= 32;

                    v128 cmpeq0 = Sse2.cmpeq_epi16(broadcast, load0);
                    v128 cmpeq1 = Sse2.cmpeq_epi16(broadcast, load1);
                    v128 cmpeq2 = Sse2.cmpeq_epi16(broadcast, load2);
                    v128 cmpeq3 = Sse2.cmpeq_epi16(broadcast, load3);

                    int mask0 = Sse2.movemask_epi8(cmpeq0);
                    int mask1 = Sse2.movemask_epi8(cmpeq1);
                    int mask2 = Sse2.movemask_epi8(cmpeq2);
                    int mask3 = Sse2.movemask_epi8(cmpeq3);

                    int tempCount0 = math.countbits(mask0);
                    int tempCount1 = math.countbits(mask1);
                    int tempCount2 = math.countbits(mask2);
                    int tempCount3 = math.countbits(mask3);
                    tempCount0 >>= 1;
                    tempCount1 >>= 1;
                    tempCount2 >>= 1;
                    tempCount3 >>= 1;

                    count0 += (uint)tempCount0;
                    count1 += (uint)tempCount1;
                    count2 += (uint)tempCount2;
                    count3 += (uint)tempCount3;
                }

                countTotal = (count0 + count1) + (count2 + count3);


                if (Hint.Likely((int)length >= 8))
                {
                    countTotal += (uint)math.countbits(Sse2.movemask_epi8(Sse2.cmpeq_epi16(broadcast, Sse2.loadu_si128(ptr_v128++)))) >> 1;

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        countTotal += (uint)math.countbits(Sse2.movemask_epi8(Sse2.cmpeq_epi16(broadcast, Sse2.loadu_si128(ptr_v128++)))) >> 1;

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            countTotal += (uint)math.countbits(Sse2.movemask_epi8(Sse2.cmpeq_epi16(broadcast, Sse2.loadu_si128(ptr_v128++)))) >> 1;
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

                if (Hint.Likely((int)length >= 4))
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi16(broadcast, Sse2.cvtsi64x_si128(*(long*)ptr_v128)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b1111_1111;
                    }

                    countTotal += (uint)math.countbits(mask) >> 1;

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 4;
                }
                else { }

                if (Hint.Likely((int)length >= 2))
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi16(broadcast, Sse2.cvtsi32_si128(*(int*)ptr_v128)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b1111;
                    }

                    countTotal += (uint)math.countbits(mask) >> 1;

                    ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    length -= 2;
                }
                else { }

                if (Hint.Likely(length != 0))
                {
                    bool areEqual = *(ushort*)ptr_v128 == value;
                    countTotal += *(byte*)&areEqual;
                }
                else { }


                return countTotal;
            }
            else
            {
                ulong count = 0;

                for (long i = 0; i < length; i++)
                {
                    bool areEqual = ptr[i] == value;
                    count += *(byte*)&areEqual;
                }

                return count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<ushort> array, ushort value)
        {
            return SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<ushort> array, int index, ushort value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<ushort> array, int index, int numEntries, ushort value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<ushort> array, ushort value)
        {
            return SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<ushort> array, int index, ushort value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<ushort> array, int index, int numEntries, ushort value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<ushort> array, ushort value)
        {
            return SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<ushort> array, int index, ushort value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<ushort> array, int index, int numEntries, ushort value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(uint* ptr, long length, uint value)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256 broadcast = Avx.mm256_set1_epi32((int)value);
                v256* ptr_v256 = (v256*)ptr;
                ulong count0 = 0;
                ulong count1 = 0;
                ulong count2 = 0;
                ulong count3 = 0;
                ulong countTotal = 0;

                while (Hint.Likely(length >= 32))
                {
                    v256 load0 = Avx.mm256_loadu_si256(ptr_v256++);
                    v256 load1 = Avx.mm256_loadu_si256(ptr_v256++);
                    v256 load2 = Avx.mm256_loadu_si256(ptr_v256++);
                    v256 load3 = Avx.mm256_loadu_si256(ptr_v256++);

                    length -= 32;

                    v256 cmpeq0 = Avx2.mm256_cmpeq_epi32(broadcast, load0);
                    v256 cmpeq1 = Avx2.mm256_cmpeq_epi32(broadcast, load1);
                    v256 cmpeq2 = Avx2.mm256_cmpeq_epi32(broadcast, load2);
                    v256 cmpeq3 = Avx2.mm256_cmpeq_epi32(broadcast, load3);

                    int mask0 = Avx.mm256_movemask_ps(cmpeq0);
                    int mask1 = Avx.mm256_movemask_ps(cmpeq1);
                    int mask2 = Avx.mm256_movemask_ps(cmpeq2);
                    int mask3 = Avx.mm256_movemask_ps(cmpeq3);

                    int tempCount0 = math.countbits(mask0);
                    int tempCount1 = math.countbits(mask1);
                    int tempCount2 = math.countbits(mask2);
                    int tempCount3 = math.countbits(mask3);

                    count0 += (uint)tempCount0;
                    count1 += (uint)tempCount1;
                    count2 += (uint)tempCount2;
                    count3 += (uint)tempCount3;
                }

                countTotal = (count0 + count1) + (count2 + count3);


                if (Hint.Likely((int)length >= 8))
                {
                    countTotal += (uint)math.countbits(Avx.mm256_movemask_ps(Avx2.mm256_cmpeq_epi32(broadcast, Avx.mm256_loadu_si256(ptr_v256++))));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        countTotal += (uint)math.countbits(Avx.mm256_movemask_ps(Avx2.mm256_cmpeq_epi32(broadcast, Avx.mm256_loadu_si256(ptr_v256++))));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            countTotal += (uint)math.countbits(Avx.mm256_movemask_ps(Avx2.mm256_cmpeq_epi32(broadcast, Avx.mm256_loadu_si256(ptr_v256++))));
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

                if (Hint.Likely((int)length >= 4))
                {
                    countTotal += (uint)math.countbits(Sse.movemask_ps(Sse2.cmpeq_epi32(Avx.mm256_castsi256_si128(broadcast), Sse2.loadu_si128(ptr_v256))));

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 4;
                }
                else { }

                if (Hint.Likely((int)length >= 2))
                {
                    int mask = Sse.movemask_ps(Sse2.cmpeq_epi32(Avx.mm256_castsi256_si128(broadcast), Sse2.cvtsi64x_si128(*(long*)ptr_v256)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b0011;
                    }

                    countTotal += (uint)math.countbits(mask);

                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 2;
                }
                else { }

                if (Hint.Likely(length != 0))
                {
                    bool areEqual = *(uint*)ptr_v256 == value;
                    countTotal += *(byte*)&areEqual;
                }
                else { }


                return countTotal;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128 broadcast = Sse2.set1_epi32((int)value);
                v128* ptr_v128 = (v128*)ptr;
                ulong count0 = 0;
                ulong count1 = 0;
                ulong count2 = 0;
                ulong count3 = 0;
                ulong countTotal = 0;

                while (Hint.Likely(length >= 16))
                {
                    v128 load0 = Sse2.loadu_si128(ptr_v128++);
                    v128 load1 = Sse2.loadu_si128(ptr_v128++);
                    v128 load2 = Sse2.loadu_si128(ptr_v128++);
                    v128 load3 = Sse2.loadu_si128(ptr_v128++);

                    length -= 16;

                    v128 cmpeq0 = Sse2.cmpeq_epi32(broadcast, load0);
                    v128 cmpeq1 = Sse2.cmpeq_epi32(broadcast, load1);
                    v128 cmpeq2 = Sse2.cmpeq_epi32(broadcast, load2);
                    v128 cmpeq3 = Sse2.cmpeq_epi32(broadcast, load3);

                    int mask0 = Sse.movemask_ps(cmpeq0);
                    int mask1 = Sse.movemask_ps(cmpeq1);
                    int mask2 = Sse.movemask_ps(cmpeq2);
                    int mask3 = Sse.movemask_ps(cmpeq3);

                    int tempCount0 = math.countbits(mask0);
                    int tempCount1 = math.countbits(mask1);
                    int tempCount2 = math.countbits(mask2);
                    int tempCount3 = math.countbits(mask3);

                    count0 += (uint)tempCount0;
                    count1 += (uint)tempCount1;
                    count2 += (uint)tempCount2;
                    count3 += (uint)tempCount3;
                }

                countTotal = (count0 + count1) + (count2 + count3);


                if (Hint.Likely((int)length >= 4))
                {
                    countTotal += (uint)math.countbits(Sse.movemask_ps(Sse2.cmpeq_epi32(broadcast, Sse2.loadu_si128(ptr_v128++))));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        countTotal += (uint)math.countbits(Sse.movemask_ps(Sse2.cmpeq_epi32(broadcast, Sse2.loadu_si128(ptr_v128++))));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            countTotal += (uint)math.countbits(Sse.movemask_ps(Sse2.cmpeq_epi32(broadcast, Sse2.loadu_si128(ptr_v128++))));
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

                if (Hint.Likely((int)length >= 2))
                {
                    int mask = Sse.movemask_ps(Sse2.cmpeq_epi32(broadcast, Sse2.cvtsi64x_si128(*(long*)ptr_v128)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b0011;
                    }

                    countTotal += (uint)math.countbits(mask);

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 2;
                }
                else { }

                if (Hint.Likely(length != 0))
                {
                    bool areEqual = *(uint*)ptr_v128 == value;
                    countTotal += *(byte*)&areEqual;
                }
                else { }


                return countTotal;
            }
            else
            {
                ulong count = 0;

                for (long i = 0; i < length; i++)
                {
                    bool areEqual = ptr[i] == value;
                    count += *(byte*)&areEqual;
                }

                return count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<uint> array, uint value)
        {
            return SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<uint> array, int index, uint value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<uint> array, int index, int numEntries, uint value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<uint> array, uint value)
        {
            return SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<uint> array, int index, uint value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<uint> array, int index, int numEntries, uint value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<uint> array, uint value)
        {
            return SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<uint> array, int index, uint value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<uint> array, int index, int numEntries, uint value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(ulong* ptr, long length, ulong value)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256 broadcast = Avx.mm256_set1_epi64x((long)value);
                v256* ptr_v256 = (v256*)ptr;
                ulong count0 = 0;
                ulong count1 = 0;
                ulong count2 = 0;
                ulong count3 = 0;
                ulong countTotal = 0;

                while (Hint.Likely(length >= 16))
                {
                    v256 load0 = Avx.mm256_loadu_si256(ptr_v256++);
                    v256 load1 = Avx.mm256_loadu_si256(ptr_v256++);
                    v256 load2 = Avx.mm256_loadu_si256(ptr_v256++);
                    v256 load3 = Avx.mm256_loadu_si256(ptr_v256++);

                    length -= 16;

                    v256 cmpeq0 = Avx2.mm256_cmpeq_epi64(broadcast, load0);
                    v256 cmpeq1 = Avx2.mm256_cmpeq_epi64(broadcast, load1);
                    v256 cmpeq2 = Avx2.mm256_cmpeq_epi64(broadcast, load2);
                    v256 cmpeq3 = Avx2.mm256_cmpeq_epi64(broadcast, load3);

                    int mask0 = Avx.mm256_movemask_pd(cmpeq0);
                    int mask1 = Avx.mm256_movemask_pd(cmpeq1);
                    int mask2 = Avx.mm256_movemask_pd(cmpeq2);
                    int mask3 = Avx.mm256_movemask_pd(cmpeq3);

                    int tempCount0 = math.countbits(mask0);
                    int tempCount1 = math.countbits(mask1);
                    int tempCount2 = math.countbits(mask2);
                    int tempCount3 = math.countbits(mask3);

                    count0 += (uint)tempCount0;
                    count1 += (uint)tempCount1;
                    count2 += (uint)tempCount2;
                    count3 += (uint)tempCount3;
                }

                countTotal = (count0 + count1) + (count2 + count3);


                if (Hint.Likely((int)length >= 4))
                {
                    countTotal += (uint)math.countbits(Avx.mm256_movemask_pd(Avx2.mm256_cmpeq_epi64(broadcast, Avx.mm256_loadu_si256(ptr_v256++))));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        countTotal += (uint)math.countbits(Avx.mm256_movemask_pd(Avx2.mm256_cmpeq_epi64(broadcast, Avx.mm256_loadu_si256(ptr_v256++))));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            countTotal += (uint)math.countbits(Avx.mm256_movemask_pd(Avx2.mm256_cmpeq_epi64(broadcast, Avx.mm256_loadu_si256(ptr_v256++))));
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

                if (Hint.Likely((int)length >= 2))
                {
                    countTotal += (uint)math.countbits(Sse2.movemask_pd(Sse4_1.cmpeq_epi64(Avx.mm256_castsi256_si128(broadcast), Sse2.loadu_si128(ptr_v256))));

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 2;
                }
                else { }

                if (Hint.Likely(length != 0))
                {
                    bool areEqual = *(ulong*)ptr_v256 == value;
                    countTotal += *(byte*)&areEqual;
                }
                else { }


                return countTotal;
            }
            else if (Sse4_1.IsSse41Supported)
            {
                v128 broadcast = Sse2.set1_epi64x((long)value);
                v128* ptr_v128 = (v128*)ptr;
                ulong count0 = 0;
                ulong count1 = 0;
                ulong count2 = 0;
                ulong count3 = 0;
                ulong countTotal = 0;

                while (Hint.Likely(length >= 8))
                {
                    v128 load0 = Sse2.loadu_si128(ptr_v128++);
                    v128 load1 = Sse2.loadu_si128(ptr_v128++);
                    v128 load2 = Sse2.loadu_si128(ptr_v128++);
                    v128 load3 = Sse2.loadu_si128(ptr_v128++);

                    length -= 8;

                    v128 cmpeq0 = Sse4_1.cmpeq_epi64(broadcast, load0);
                    v128 cmpeq1 = Sse4_1.cmpeq_epi64(broadcast, load1);
                    v128 cmpeq2 = Sse4_1.cmpeq_epi64(broadcast, load2);
                    v128 cmpeq3 = Sse4_1.cmpeq_epi64(broadcast, load3);

                    int mask0 = Sse2.movemask_pd(cmpeq0);
                    int mask1 = Sse2.movemask_pd(cmpeq1);
                    int mask2 = Sse2.movemask_pd(cmpeq2);
                    int mask3 = Sse2.movemask_pd(cmpeq3);

                    int tempCount0 = math.countbits(mask0);
                    int tempCount1 = math.countbits(mask1);
                    int tempCount2 = math.countbits(mask2);
                    int tempCount3 = math.countbits(mask3);

                    count0 += (uint)tempCount0;
                    count1 += (uint)tempCount1;
                    count2 += (uint)tempCount2;
                    count3 += (uint)tempCount3;
                }

                countTotal = (count0 + count1) + (count2 + count3);


                if (Hint.Likely((int)length >= 2))
                {
                    countTotal += (uint)math.countbits(Sse2.movemask_pd(Sse4_1.cmpeq_epi64(broadcast, Sse2.loadu_si128(ptr_v128++))));

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        countTotal += (uint)math.countbits(Sse2.movemask_pd(Sse4_1.cmpeq_epi64(broadcast, Sse2.loadu_si128(ptr_v128++))));

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            countTotal += (uint)math.countbits(Sse2.movemask_pd(Sse4_1.cmpeq_epi64(broadcast, Sse2.loadu_si128(ptr_v128++))));
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
                    bool areEqual = *(ulong*)ptr_v128 == value;
                    countTotal += *(byte*)&areEqual;
                }
                else { }


                return countTotal;
            }
            else
            {
                ulong count = 0;

                for (long i = 0; i < length; i++)
                {
                    bool areEqual = ptr[i] == value;
                    count += *(byte*)&areEqual;
                }

                return count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<ulong> array, ulong value)
        {
            return SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<ulong> array, int index, ulong value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<ulong> array, int index, int numEntries, ulong value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<ulong> array, ulong value)
        {
            return SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<ulong> array, int index, ulong value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<ulong> array, int index, int numEntries, ulong value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<ulong> array, ulong value)
        {
            return SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<ulong> array, int index, ulong value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<ulong> array, int index, int numEntries, ulong value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(sbyte* ptr, long length, sbyte value)
        {
Assert.IsNonNegative(length);

            return SIMD_Count((byte*)ptr, length, (byte)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<sbyte> array, sbyte value)
        {
            return SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<sbyte> array, int index, sbyte value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<sbyte> array, int index, int numEntries, sbyte value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<sbyte> array, sbyte value)
        {
            return SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<sbyte> array, int index, sbyte value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<sbyte> array, int index, int numEntries, sbyte value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<sbyte> array, sbyte value)
        {
            return SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<sbyte> array, int index, sbyte value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<sbyte> array, int index, int numEntries, sbyte value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(short* ptr, long length, short value)
        {
Assert.IsNonNegative(length);

            return SIMD_Count((ushort*)ptr, length, (ushort)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<short> array, short value)
        {
            return SIMD_Count((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<short> array, int index, short value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<short> array, int index, int numEntries, short value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<short> array, short value)
        {
            return SIMD_Count((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<short> array, int index, short value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<short> array, int index, int numEntries, short value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<short> array, short value)
        {
            return SIMD_Count((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<short> array, int index, short value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<short> array, int index, int numEntries, short value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(int* ptr, long length, int value)
        {
Assert.IsNonNegative(length);

            return SIMD_Count((uint*)ptr, length, (uint)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<int> array, int value)
        {
            return SIMD_Count((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<int> array, int index, int value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<int> array, int index, int numEntries, int value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<int> array, int value)
        {
            return SIMD_Count((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<int> array, int index, int value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<int> array, int index, int numEntries, int value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<int> array, int value)
        {
            return SIMD_Count((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<int> array, int index, int value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<int> array, int index, int numEntries, int value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(long* ptr, long length, long value)
        {
Assert.IsNonNegative(length);

            return SIMD_Count((ulong*)ptr, length, (ulong)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<long> array, long value)
        {
            return SIMD_Count((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<long> array, int index, long value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<long> array, int index, int numEntries, long value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<long> array, long value)
        {
            return SIMD_Count((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<long> array, int index, long value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<long> array, int index, int numEntries, long value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<long> array, long value)
        {
            return SIMD_Count((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<long> array, int index, long value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<long> array, int index, int numEntries, long value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(float* ptr, long length, float value)
        {
Assert.IsNonNegative(length);
Assert.IsFalse(math.isnan(value));

            if (Avx.IsAvxSupported)
            {
                v256 broadcast = Avx.mm256_set1_ps(value);
                v256* ptr_v256 = (v256*)ptr;
                ulong count0 = 0;
                ulong count1 = 0;
                ulong count2 = 0;
                ulong count3 = 0;
                ulong countTotal = 0;

                while (Hint.Likely(length >= 32))
                {
                    v256 load0 = Avx.mm256_loadu_ps(ptr_v256++);
                    v256 load1 = Avx.mm256_loadu_ps(ptr_v256++);
                    v256 load2 = Avx.mm256_loadu_ps(ptr_v256++);
                    v256 load3 = Avx.mm256_loadu_ps(ptr_v256++);

                    length -= 32;

                    v256 cmpeq0 = Avx.mm256_cmp_ps(broadcast, load0, (int)Avx.CMP.EQ_OQ);
                    v256 cmpeq1 = Avx.mm256_cmp_ps(broadcast, load1, (int)Avx.CMP.EQ_OQ);
                    v256 cmpeq2 = Avx.mm256_cmp_ps(broadcast, load2, (int)Avx.CMP.EQ_OQ);
                    v256 cmpeq3 = Avx.mm256_cmp_ps(broadcast, load3, (int)Avx.CMP.EQ_OQ);

                    int mask0 = Avx.mm256_movemask_ps(cmpeq0);
                    int mask1 = Avx.mm256_movemask_ps(cmpeq1);
                    int mask2 = Avx.mm256_movemask_ps(cmpeq2);
                    int mask3 = Avx.mm256_movemask_ps(cmpeq3);

                    int tempCount0 = math.countbits(mask0);
                    int tempCount1 = math.countbits(mask1);
                    int tempCount2 = math.countbits(mask2);
                    int tempCount3 = math.countbits(mask3);

                    count0 += (uint)tempCount0;
                    count1 += (uint)tempCount1;
                    count2 += (uint)tempCount2;
                    count3 += (uint)tempCount3;
                }

                countTotal = (count0 + count1) + (count2 + count3);


                if (Hint.Likely((int)length >= 8))
                {
                    countTotal += (uint)math.countbits(Avx.mm256_movemask_ps(Avx.mm256_cmp_ps(broadcast, Avx.mm256_loadu_si256(ptr_v256++), (int)Avx.CMP.EQ_OQ)));

                    if (Hint.Likely((int)length >= 2 * 8))
                    {
                        countTotal += (uint)math.countbits(Avx.mm256_movemask_ps(Avx.mm256_cmp_ps(broadcast, Avx.mm256_loadu_si256(ptr_v256++), (int)Avx.CMP.EQ_OQ)));

                        if (Hint.Likely((int)length >= 3 * 8))
                        {
                            countTotal += (uint)math.countbits(Avx.mm256_movemask_ps(Avx.mm256_cmp_ps(broadcast, Avx.mm256_loadu_si256(ptr_v256++), (int)Avx.CMP.EQ_OQ)));
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


                if (Hint.Likely((int)length >= 4))
                {
                    countTotal += (uint)math.countbits(Sse.movemask_ps(Sse.cmpeq_ps(Avx.mm256_castsi256_si128(broadcast), Sse.loadu_ps(ptr_v256))));

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 4;
                }
                else { }

                if (Hint.Likely((int)length >= 2))
                {
                    int mask = Sse.movemask_ps(Sse.cmpeq_ps(Avx.mm256_castsi256_si128(broadcast), Sse2.cvtsi64x_si128(*(long*)ptr_v256)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b0011;
                    }

                    countTotal += (uint)math.countbits(mask);

                    ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    length -= 2;
                }
                else { }

                if (Hint.Likely(length != 0))
                {
                    bool areEqual = *(float*)ptr_v256 == value;
                    countTotal += *(byte*)&areEqual;
                }
                else { }


                return countTotal;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128 broadcast = Sse.set1_ps(value);
                v128* ptr_v128 = (v128*)ptr;
                ulong count0 = 0;
                ulong count1 = 0;
                ulong count2 = 0;
                ulong count3 = 0;
                ulong countTotal = 0;

                while (Hint.Likely(length >= 16))
                {
                    v128 load0 = Sse.loadu_ps(ptr_v128++);
                    v128 load1 = Sse.loadu_ps(ptr_v128++);
                    v128 load2 = Sse.loadu_ps(ptr_v128++);
                    v128 load3 = Sse.loadu_ps(ptr_v128++);

                    length -= 16;

                    v128 cmpeq0 = Sse.cmpeq_ps(broadcast, load0);
                    v128 cmpeq1 = Sse.cmpeq_ps(broadcast, load1);
                    v128 cmpeq2 = Sse.cmpeq_ps(broadcast, load2);
                    v128 cmpeq3 = Sse.cmpeq_ps(broadcast, load3);

                    int mask0 = Sse.movemask_ps(cmpeq0);
                    int mask1 = Sse.movemask_ps(cmpeq1);
                    int mask2 = Sse.movemask_ps(cmpeq2);
                    int mask3 = Sse.movemask_ps(cmpeq3);

                    int tempCount0 = math.countbits(mask0);
                    int tempCount1 = math.countbits(mask1);
                    int tempCount2 = math.countbits(mask2);
                    int tempCount3 = math.countbits(mask3);

                    count0 += (uint)tempCount0;
                    count1 += (uint)tempCount1;
                    count2 += (uint)tempCount2;
                    count3 += (uint)tempCount3;
                }

                countTotal = (count0 + count1) + (count2 + count3);


                if (Hint.Likely((int)length >= 4))
                {
                    countTotal += (uint)math.countbits(Sse.movemask_ps(Sse.cmpeq_ps(broadcast, Sse.loadu_ps(ptr_v128++))));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        countTotal += (uint)math.countbits(Sse.movemask_ps(Sse.cmpeq_ps(broadcast, Sse.loadu_ps(ptr_v128++))));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            countTotal += (uint)math.countbits(Sse.movemask_ps(Sse.cmpeq_ps(broadcast, Sse.loadu_ps(ptr_v128++))));
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

                if (Hint.Likely((int)length >= 2))
                {
                    int mask = Sse.movemask_ps(Sse.cmpeq_ps(broadcast, Sse2.cvtsi64x_si128(*(long*)ptr_v128)));

                    if (Constant.IsConstantExpression(value) && value != 0)
                    {
                        ;
                    }
                    else
                    {
                        mask &= 0b0011;
                    }

                    countTotal += (uint)math.countbits(mask);

                    ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    length -= 2;
                }
                else { }

                if (Hint.Likely(length != 0))
                {
                    bool areEqual = *(float*)ptr_v128 == value;
                    countTotal += *(byte*)&areEqual;
                }
                else { }


                return countTotal;
            }
            else
            {
                ulong count = 0;

                for (long i = 0; i < length; i++)
                {
                    bool areEqual = ptr[i] == value;
                    count += *(byte*)&areEqual;
                }

                return count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<float> array, float value)
        {
            return SIMD_Count((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<float> array, int index, float value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<float> array, int index, int numEntries, float value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<float> array, float value)
        {
            return SIMD_Count((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<float> array, int index, float value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<float> array, int index, int numEntries, float value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<float> array, float value)
        {
            return SIMD_Count((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<float> array, int index, float value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<float> array, int index, int numEntries, float value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(double* ptr, long length, double value)
        {
Assert.IsNonNegative(length);
Assert.IsFalse(math.isnan(value));

            if (Avx.IsAvxSupported)
            {
                v256 broadcast = Avx.mm256_set1_pd(value);
                v256* ptr_v256 = (v256*)ptr;
                ulong count0 = 0;
                ulong count1 = 0;
                ulong count2 = 0;
                ulong count3 = 0;
                ulong countTotal = 0;

                while (Hint.Likely(length >= 16))
                {
                    v256 load0 = Avx.mm256_loadu_pd(ptr_v256++);
                    v256 load1 = Avx.mm256_loadu_pd(ptr_v256++);
                    v256 load2 = Avx.mm256_loadu_pd(ptr_v256++);
                    v256 load3 = Avx.mm256_loadu_pd(ptr_v256++);

                    length -= 16;

                    v256 cmpeq0 = Avx.mm256_cmp_pd(broadcast, load0, (int)Avx.CMP.EQ_OQ);
                    v256 cmpeq1 = Avx.mm256_cmp_pd(broadcast, load1, (int)Avx.CMP.EQ_OQ);
                    v256 cmpeq2 = Avx.mm256_cmp_pd(broadcast, load2, (int)Avx.CMP.EQ_OQ);
                    v256 cmpeq3 = Avx.mm256_cmp_pd(broadcast, load3, (int)Avx.CMP.EQ_OQ);

                    int mask0 = Avx.mm256_movemask_pd(cmpeq0);
                    int mask1 = Avx.mm256_movemask_pd(cmpeq1);
                    int mask2 = Avx.mm256_movemask_pd(cmpeq2);
                    int mask3 = Avx.mm256_movemask_pd(cmpeq3);

                    int tempCount0 = math.countbits(mask0);
                    int tempCount1 = math.countbits(mask1);
                    int tempCount2 = math.countbits(mask2);
                    int tempCount3 = math.countbits(mask3);

                    count0 += (uint)tempCount0;
                    count1 += (uint)tempCount1;
                    count2 += (uint)tempCount2;
                    count3 += (uint)tempCount3;
                }

                countTotal = (count0 + count1) + (count2 + count3);


                if (Hint.Likely((int)length >= 4))
                {
                    countTotal += (uint)math.countbits(Avx.mm256_movemask_pd(Avx.mm256_cmp_pd(broadcast, Avx.mm256_loadu_pd(ptr_v256++), (int)Avx.CMP.EQ_OQ)));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        countTotal += (uint)math.countbits(Avx.mm256_movemask_pd(Avx.mm256_cmp_pd(broadcast, Avx.mm256_loadu_pd(ptr_v256++), (int)Avx.CMP.EQ_OQ)));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            countTotal += (uint)math.countbits(Avx.mm256_movemask_pd(Avx.mm256_cmp_pd(broadcast, Avx.mm256_loadu_pd(ptr_v256++), (int)Avx.CMP.EQ_OQ)));
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

                if (Hint.Likely((int)length >= 2))
                {
                    countTotal += (uint)math.countbits(Sse2.movemask_pd(Sse2.cmpeq_pd(Avx.mm256_castpd256_pd128(broadcast), Sse.loadu_ps(ptr_v256))));

                    length -= 2;
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                }
                else { }

                if (Hint.Likely(length != 0))
                {
                    bool areEqual = *(double*)ptr_v256 == value;
                    countTotal += *(byte*)&areEqual;
                }
                else { }


                return countTotal;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128 broadcast = Sse2.set1_pd(value);
                v128* ptr_v128 = (v128*)ptr;
                ulong count0 = 0;
                ulong count1 = 0;
                ulong count2 = 0;
                ulong count3 = 0;
                ulong countTotal = 0;

                while (Hint.Likely(length >= 8))
                {
                    v128 load0 = Sse.loadu_ps(ptr_v128++);
                    v128 load1 = Sse.loadu_ps(ptr_v128++);
                    v128 load2 = Sse.loadu_ps(ptr_v128++);
                    v128 load3 = Sse.loadu_ps(ptr_v128++);

                    length -= 8;

                    v128 cmpeq0 = Sse2.cmpeq_pd(broadcast, load0);
                    v128 cmpeq1 = Sse2.cmpeq_pd(broadcast, load1);
                    v128 cmpeq2 = Sse2.cmpeq_pd(broadcast, load2);
                    v128 cmpeq3 = Sse2.cmpeq_pd(broadcast, load3);

                    int mask0 = Sse2.movemask_pd(cmpeq0);
                    int mask1 = Sse2.movemask_pd(cmpeq1);
                    int mask2 = Sse2.movemask_pd(cmpeq2);
                    int mask3 = Sse2.movemask_pd(cmpeq3);

                    int tempCount0 = math.countbits(mask0);
                    int tempCount1 = math.countbits(mask1);
                    int tempCount2 = math.countbits(mask2);
                    int tempCount3 = math.countbits(mask3);

                    count0 += (uint)tempCount0;
                    count1 += (uint)tempCount1;
                    count2 += (uint)tempCount2;
                    count3 += (uint)tempCount3;
                }

                countTotal = (count0 + count1) + (count2 + count3);


                if (Hint.Likely((int)length >= 2))
                {
                    countTotal += (uint)math.countbits(Sse2.movemask_pd(Sse2.cmpeq_pd(broadcast, Sse.loadu_ps(ptr_v128++))));

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        countTotal += (uint)math.countbits(Sse2.movemask_pd(Sse2.cmpeq_pd(broadcast, Sse.loadu_ps(ptr_v128++))));

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            countTotal += (uint)math.countbits(Sse2.movemask_pd(Sse2.cmpeq_pd(broadcast, Sse.loadu_ps(ptr_v128++))));
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
                    bool areEqual = *(double*)ptr_v128 == value;
                    countTotal += *(byte*)&areEqual;
                }
                else { }


                return countTotal;
            }
            else
            {
                ulong count = 0;

                for (long i = 0; i < length; i++)
                {
                    bool areEqual = ptr[i] == value;
                    count += *(byte*)&areEqual;
                }

                return count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<double> array, double value)
        {
            return SIMD_Count((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<double> array, int index, double value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeArray<double> array, int index, int numEntries, double value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<double> array, double value)
        {
            return SIMD_Count((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<double> array, int index, double value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeList<double> array, int index, int numEntries, double value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<double> array, double value)
        {
            return SIMD_Count((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<double> array, int index, double value)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Count((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(this NativeSlice<double> array, int index, int numEntries, double value)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Count((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }
    }
}