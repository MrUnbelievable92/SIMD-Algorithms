using System;
using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst;
using DevTools;

using static Unity.Burst.Intrinsics.X86;

namespace SIMDAlgorithms
{
    unsafe public static partial class Algorithms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(byte* ptr, long length, TypeCode range = TypeCode.Empty)
        {
Assert.IsNonNegative(length);

            switch (range)
            {
                case TypeCode.Byte:
                {
                    byte sum = 0;

                    for (long i = 0; Hint.Likely(i < length); i++)
                    {
                        sum += ptr[i];
                    }

                    return sum;
                }
                case TypeCode.Empty:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 ZERO = default(v256); 
                        v256* address = (v256*)ptr;

                        v256 acc0 = default(v256);
                        v256 acc1 = default(v256);
                        v256 acc2 = default(v256);
                        v256 acc3 = default(v256);

                        while (Hint.Likely(length >= 128))
                        {
                            v256 sad0 = Avx2.mm256_sad_epu8(ZERO, Avx.mm256_loadu_si256(address++));
                            v256 sad1 = Avx2.mm256_sad_epu8(ZERO, Avx.mm256_loadu_si256(address++));
                            v256 sad2 = Avx2.mm256_sad_epu8(ZERO, Avx.mm256_loadu_si256(address++));
                            v256 sad3 = Avx2.mm256_sad_epu8(ZERO, Avx.mm256_loadu_si256(address++));

                            acc0 = Avx2.mm256_add_epi64(acc0, sad0);
                            acc1 = Avx2.mm256_add_epi64(acc1, sad1);
                            acc2 = Avx2.mm256_add_epi64(acc2, sad2);
                            acc3 = Avx2.mm256_add_epi64(acc3, sad3);

                            length -= 128;
                        }

                        v256 add0 = Avx2.mm256_add_epi64(acc0, acc1);
                        v256 add1 = Avx2.mm256_add_epi64(acc2, acc3);
                        v256 add2 = Avx2.mm256_add_epi64(add0, add1);


                        if (Hint.Likely((int)length >= 32))
                        {
                            add2 = Avx2.mm256_add_epi64(add2, Avx2.mm256_sad_epu8(ZERO, Avx.mm256_loadu_si256(address++)));

                            if (Hint.Likely((int)length >= 2 * 32))
                            {
                                add2 = Avx2.mm256_add_epi64(add2, Avx2.mm256_sad_epu8(ZERO, Avx.mm256_loadu_si256(address++)));

                                if (Hint.Likely((int)length >= 3 * 32))
                                {
                                    add2 = Avx2.mm256_add_epi64(add2, Avx2.mm256_sad_epu8(ZERO, Avx.mm256_loadu_si256(address++)));
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

                        v128 csum0 = Sse2.add_epi64(Avx.mm256_castsi256_si128(add2), Avx2.mm256_extracti128_si256(add2, 1));


                        if (Hint.Likely((int)length >= 16))
                        {
                            csum0 = Sse2.add_epi64(csum0, Sse2.sad_epu8(Avx.mm256_castsi256_si128(ZERO), Sse2.loadu_si128(address)));

                            address = (v256*)((v128*)address + 1);
                            length -= 16;
                        }
                        else { }

                        if (Hint.Likely((int)length >= 8))
                        {
                            csum0 = Sse2.add_epi64(csum0, Sse2.sad_epu8(Avx.mm256_castsi256_si128(ZERO), Sse2.cvtsi64x_si128(*(long*)address)));
                            length -= 8;
                            address = (v256*)((long*)address + 1);
                        }
                        else { }

                        if (Hint.Likely((int)length >= 4))
                        {
                            csum0 = Sse2.add_epi64(csum0, Sse2.sad_epu8(Avx.mm256_castsi256_si128(ZERO), Sse2.cvtsi32_si128(*(int*)address)));
                            length -= 4;
                            address = (v256*)((int*)address + 1);
                        }
                        else { }

                        v128 csum = Sse2.add_epi64(csum0, Sse2.shuffle_epi32(csum0, Sse.SHUFFLE(0, 0, 3, 2)));
                        ulong sum = csum.ULong0;


                        if (Hint.Likely(length != 0))
                        {
                            sum += *(byte*)address;
                            length--;

                            if (Hint.Likely(length != 0))
                            {
                                address = (v256*)((byte*)address + 1);
                                sum += *(byte*)address;
                                length--;

                                if (Hint.Likely(length != 0))
                                {
                                    address = (v256*)((byte*)address + 1);
                                    sum += *(byte*)address;
                                }
                                else { }
                            }
                            else { }
                        }
                        else { }


                        return sum;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 ZERO = default(v128);
                        v128* address = (v128*)ptr;

                        v128 acc0 = default(v128);
                        v128 acc1 = default(v128);
                        v128 acc2 = default(v128);
                        v128 acc3 = default(v128);

                        while (Hint.Likely(length >= 64))
                        {
                            v128 sad0 = Sse2.sad_epu8(ZERO, Sse2.loadu_si128(address++));
                            v128 sad1 = Sse2.sad_epu8(ZERO, Sse2.loadu_si128(address++));
                            v128 sad2 = Sse2.sad_epu8(ZERO, Sse2.loadu_si128(address++));
                            v128 sad3 = Sse2.sad_epu8(ZERO, Sse2.loadu_si128(address++));

                            acc0 = Sse2.add_epi64(acc0, sad0);
                            acc1 = Sse2.add_epi64(acc1, sad1);
                            acc2 = Sse2.add_epi64(acc2, sad2);
                            acc3 = Sse2.add_epi64(acc3, sad3);

                            length -= 64;
                        }

                        acc0 = Sse2.add_epi64(Sse2.add_epi64(acc0, acc1), Sse2.add_epi64(acc2, acc3));

                        if (Hint.Likely((int)length >= 16))
                        {
                            acc0 = Sse2.add_epi64(acc0, Sse2.sad_epu8(ZERO, Sse2.loadu_si128(address++)));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                acc0 = Sse2.add_epi64(acc0, Sse2.sad_epu8(ZERO, Sse2.loadu_si128(address++)));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    acc0 = Sse2.add_epi64(acc0, Sse2.sad_epu8(ZERO, Sse2.loadu_si128(address++)));
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
                            acc0 = Sse2.add_epi64(acc0, Sse2.sad_epu8(ZERO, Sse2.cvtsi64x_si128(*(long*)address)));
                            length -= 8;
                            address = (v128*)((long*)address + 1);
                        }
                        else { }

                        if (Hint.Likely((int)length >= 4))
                        {
                            acc0 = Sse2.add_epi64(acc0, Sse2.sad_epu8(ZERO, Sse2.cvtsi32_si128(*(int*)address)));
                            length -= 4;
                            address = (v128*)((int*)address + 1);
                        }
                        else { }

                        v128 csum = Sse2.add_epi64(acc0, Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 3, 2)));
                        ulong sum = csum.ULong0;


                        if (Hint.Likely(length != 0))
                        {
                            sum += *(byte*)address;
                            length--;

                            if (Hint.Likely(length != 0))
                            {
                                address = (v128*)((byte*)address + 1);
                                sum += *(byte*)address;
                                length--;

                                if (Hint.Likely(length != 0))
                                {
                                    address = (v128*)((byte*)address + 1);
                                    sum += *(byte*)address;
                                }
                                else { }
                            }
                            else { }
                        }
                        else { }


                        return sum;
                    }
                    else
                    {
                        ulong sum = 0;

                        for (long i = 0; i < length; i++)
                        {
                            sum += ptr[i];
                        }

                        return sum;
                    }
                }
                default:
                {
#if DEBUG
                    throw new ArgumentException($"Invalid TypeCode argument '{ range }'. The supported type codes are: Byte, UInt16, UInt32 and UInt64");
#else
                    return 0;
#endif
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeArray<byte> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeArray<byte> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeArray<byte> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeList<byte> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeList<byte> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeList<byte> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeSlice<byte> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeSlice<byte> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeSlice<byte> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(ushort* ptr, long length, TypeCode range = TypeCode.Empty)
        {
Assert.IsNonNegative(length);

            range = (range == TypeCode.Empty) ? GetSafeRange.Summation(TypeCode.UInt16, length) : range;

            switch (range)
            {
                case TypeCode.UInt16:
                {
                    ushort sum = 0;

                    for (long i = 0; Hint.Likely(i < length); i++)
                    {
                        sum += ptr[i];
                    }

                    return sum;
                }
                case TypeCode.UInt32:
                {
                    uint sum = 0;

                    for (long i = 0; Hint.Likely(i < length); i++)
                    {
                        sum += ptr[i];
                    }

                    return sum;
                }
                case TypeCode.UInt64:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v128* address = (v128*)ptr;
                        v256 longs = default(v256);
                        
                        
                        while (Hint.Likely(length >= 32))
                        {
                            int iterations = 0;
                            v256 acc0 = default(v256);
                            v256 acc1 = default(v256);
                            v256 acc2 = default(v256);
                            v256 acc3 = default(v256);
                        
                            while (Hint.Likely(iterations < uint.MaxValue / ushort.MaxValue))
                            {
                                acc0 = Avx2.mm256_add_epi32(acc0, Avx2.mm256_cvtepu16_epi32(Sse2.loadu_si128(address++)));
                                acc1 = Avx2.mm256_add_epi32(acc1, Avx2.mm256_cvtepu16_epi32(Sse2.loadu_si128(address++)));
                                acc2 = Avx2.mm256_add_epi32(acc2, Avx2.mm256_cvtepu16_epi32(Sse2.loadu_si128(address++)));
                                acc3 = Avx2.mm256_add_epi32(acc3, Avx2.mm256_cvtepu16_epi32(Sse2.loadu_si128(address++)));
                        
                                iterations++;
                        
                                if (Hint.Unlikely((length = length - 32) < 32))
                                {
                                    break;
                                }
                                else continue;
                            }
                        
                            v256 cast0 = Avx2.mm256_add_epi64(Avx2.mm256_cvtepu32_epi64(Avx.mm256_castsi256_si128(acc0)),
                                                              Avx2.mm256_cvtepu32_epi64(Avx2.mm256_extracti128_si256(acc0, 1)));
                            v256 cast1 = Avx2.mm256_add_epi64(Avx2.mm256_cvtepu32_epi64(Avx.mm256_castsi256_si128(acc1)),
                                                              Avx2.mm256_cvtepu32_epi64(Avx2.mm256_extracti128_si256(acc1, 1)));
                            v256 cast2 = Avx2.mm256_add_epi64(Avx2.mm256_cvtepu32_epi64(Avx.mm256_castsi256_si128(acc2)),
                                                              Avx2.mm256_cvtepu32_epi64(Avx2.mm256_extracti128_si256(acc2, 1)));
                            v256 cast3 = Avx2.mm256_add_epi64(Avx2.mm256_cvtepu32_epi64(Avx.mm256_castsi256_si128(acc3)),
                                                              Avx2.mm256_cvtepu32_epi64(Avx2.mm256_extracti128_si256(acc3, 1)));

                            longs = Avx2.mm256_add_epi64(longs, Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(cast0, cast1),
                                                                                     Avx2.mm256_add_epi64(cast2, cast3)));
                        }
                        
                            
                        if (Hint.Likely((int)length >= 8))
                        {
                            v256 ints = Avx2.mm256_cvtepu16_epi32(Sse2.loadu_si128(address++));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                ints = Avx2.mm256_add_epi32(ints, Avx2.mm256_cvtepu16_epi32(Sse2.loadu_si128(address++)));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    ints = Avx2.mm256_add_epi32(ints, Avx2.mm256_cvtepu16_epi32(Sse2.loadu_si128(address++)));
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

                            ints = Avx2.mm256_cvtepu32_epi64(Sse2.add_epi32(Avx.mm256_castsi256_si128(ints), Avx2.mm256_extracti128_si256(ints, 1)));
                            longs = Avx2.mm256_add_epi64(longs, ints);
                        }
                        else { }

                        if (Hint.Likely((int)length >= 4))
                        {
                            longs = Avx2.mm256_add_epi64(longs, Avx2.mm256_cvtepu16_epi64(Sse2.cvtsi64x_si128(*(long*)address)));
                            length -= 4;
                            address = (v128*)((long*)address + 1);
                        }
                        else { }

                        v128 csum_0 = Sse2.add_epi64(Avx.mm256_castsi256_si128(longs), Avx2.mm256_extracti128_si256(longs, 1));

                        if (Hint.Likely((int)length >= 2))
                        {
                            csum_0 = Sse2.add_epi64(csum_0, Sse4_1.cvtepu16_epi64(Sse2.cvtsi32_si128(*(int*)address)));
                            length -= 2;
                            address = (v128*)((int*)address + 1);
                        }

                        v128 csum_1 = Sse2.add_epi64(csum_0, Sse2.shuffle_epi32(csum_0, Sse.SHUFFLE(0, 0, 3, 2)));
                        ulong sum = csum_1.ULong0;

                        if (Hint.Likely(length != 0))
                        {
                            sum += *(ushort*)address;
                        }


                        return sum;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128* address = (v128*)ptr;
                        v128 longs = default(v128);
                        v128 ZERO = default(v128);
                        
                        
                        while (Hint.Likely(length >= 16))
                        {
                            int iterations = 0;
                            v128 acc0 = default(v128);
                            v128 acc1 = default(v128);
                            v128 acc2 = default(v128);
                            v128 acc3 = default(v128);
                        
                            while (Hint.Likely(iterations < uint.MaxValue / ushort.MaxValue))
                            {
                                v128 _0_7 = Sse2.loadu_si128(address++);
                                v128 _8_15 = Sse2.loadu_si128(address++);

                                if (Sse4_1.IsSse41Supported)
                                {
                                    acc0 = Sse2.add_epi32(acc0, Sse4_1.cvtepu16_epi32(_0_7));
                                    acc1 = Sse2.add_epi32(acc1, Sse2.unpackhi_epi16(_0_7, ZERO));
                                    acc2 = Sse2.add_epi32(acc2, Sse4_1.cvtepu16_epi32(_8_15));
                                    acc3 = Sse2.add_epi32(acc3, Sse2.unpackhi_epi16(_8_15, ZERO));
                                }
                                else
                                {
                                    acc0 = Sse2.add_epi32(acc0, Sse2.unpacklo_epi16(_0_7, ZERO));
                                    acc1 = Sse2.add_epi32(acc1, Sse2.unpackhi_epi16(_0_7, ZERO));
                                    acc2 = Sse2.add_epi32(acc2, Sse2.unpacklo_epi16(_8_15, ZERO));
                                    acc3 = Sse2.add_epi32(acc3, Sse2.unpackhi_epi16(_8_15, ZERO));
                                }
                        
                                iterations++;
                        
                                if (Hint.Unlikely((length = length - 16) < 16))
                                {
                                    break;
                                }
                                else continue;
                            }


                            if (Sse4_1.IsSse41Supported)
                            {
                                v128 cast0 = Sse2.add_epi64(Sse4_1.cvtepu32_epi64(acc0),
                                                            Sse2.unpackhi_epi32(acc0, ZERO));
                                v128 cast1 = Sse2.add_epi64(Sse4_1.cvtepu32_epi64(acc1),
                                                            Sse2.unpackhi_epi32(acc1, ZERO));
                                v128 cast2 = Sse2.add_epi64(Sse4_1.cvtepu32_epi64(acc2),
                                                            Sse2.unpackhi_epi32(acc2, ZERO));
                                v128 cast3 = Sse2.add_epi64(Sse4_1.cvtepu32_epi64(acc3),
                                                            Sse2.unpackhi_epi32(acc3, ZERO));

                                longs = Sse2.add_epi64(longs, Sse2.add_epi64(Sse2.add_epi64(cast0, cast1),
                                                                             Sse2.add_epi64(cast2, cast3)));
                            }
                            else
                            {
                                v128 cast0 = Sse2.add_epi64(Sse2.unpacklo_epi32(acc0, ZERO),
                                                            Sse2.unpackhi_epi32(acc0, ZERO));
                                v128 cast1 = Sse2.add_epi64(Sse2.unpacklo_epi32(acc1, ZERO),
                                                            Sse2.unpackhi_epi32(acc1, ZERO));
                                v128 cast2 = Sse2.add_epi64(Sse2.unpacklo_epi32(acc2, ZERO),
                                                            Sse2.unpackhi_epi32(acc2, ZERO));
                                v128 cast3 = Sse2.add_epi64(Sse2.unpacklo_epi32(acc3, ZERO),
                                                            Sse2.unpackhi_epi32(acc3, ZERO));

                                longs = Sse2.add_epi64(longs, Sse2.add_epi64(Sse2.add_epi64(cast0, cast1),
                                                                             Sse2.add_epi64(cast2, cast3)));
                            }
                        }


                        v128 ints;

                        if (Hint.Likely((int)length >= 8))
                        {
                            v128 load = Sse2.loadu_si128(address++);

                            if (Sse4_1.IsSse41Supported)
                            {
                                ints = Sse2.add_epi32(Sse4_1.cvtepu16_epi32(load),
                                                      Sse2.unpackhi_epi16(load, ZERO));
                            }
                            else
                            {
                                ints = Sse2.add_epi32(Sse2.unpacklo_epi16(load, ZERO),
                                                      Sse2.unpackhi_epi16(load, ZERO));
                            }

                            length -= 8;
                        }
                        else 
                        {
                            ints = default(v128);
                        }

                        if (Hint.Likely((int)length >= 4))
                        {
                            if (Sse4_1.IsSse41Supported)
                            {
                                ints = Sse2.add_epi32(ints, Sse4_1.cvtepu16_epi32(Sse2.cvtsi64x_si128(*(long*)address)));
                            }
                            else
                            {
                                ints = Sse2.add_epi32(ints, Sse2.unpacklo_epi16(Sse2.cvtsi64x_si128(*(long*)address), ZERO));
                            }

                            length -= 4;
                            address = (v128*)((long*)address + 1);
                        }
                        else { }

                        if (Hint.Likely((int)length >= 2))
                        {
                            if (Sse4_1.IsSse41Supported)
                            {
                                ints = Sse2.add_epi32(ints, Sse4_1.cvtepu16_epi32(Sse2.cvtsi32_si128(*(int*)address)));
                            }
                            else
                            {
                                ints = Sse2.add_epi32(ints, Sse2.unpacklo_epi16(Sse2.cvtsi32_si128(*(int*)address), ZERO));
                            }

                            length -= 2;
                            address = (v128*)((int*)address + 1);
                        }

                        if (Sse4_1.IsSse41Supported)
                        {
                            longs = Sse2.add_epi64(longs, Sse4_1.cvtepu32_epi64(Sse2.add_epi32(ints, Sse2.shuffle_epi32(ints, Sse.SHUFFLE(0, 0, 3, 2)))));
                        }
                        else
                        {
                            longs = Sse2.add_epi64(longs, Sse2.unpacklo_epi32(Sse2.add_epi32(ints, Sse2.shuffle_epi32(ints, Sse.SHUFFLE(0, 0, 3, 2))), ZERO));
                        }


                        v128 csum = Sse2.add_epi64(longs, Sse2.shuffle_epi32(longs, Sse.SHUFFLE(0, 0, 3, 2)));
                        ulong sum = csum.ULong0;

                        if (Hint.Likely(length != 0))
                        {
                            sum += *(ushort*)address;
                        }


                        return sum;
                    }
                    else
                    {
                        ulong sum = 0;

                        for (long i = 0; Hint.Likely(i < length); i++)
                        {
                            sum += ptr[i];
                        }

                        return sum;
                    }
                }
                default:
                {
#if DEBUG
                    throw new ArgumentException($"Invalid TypeCode argument '{ range }'. The supported type codes are: UInt16, UInt32 and UInt64.");
#else
                    return 0;
#endif
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeArray<ushort> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeArray<ushort> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeArray<ushort> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeList<ushort> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeList<ushort> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeList<ushort> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeSlice<ushort> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeSlice<ushort> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeSlice<ushort> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(uint* ptr, long length, TypeCode range = TypeCode.Empty)
        {
Assert.IsNonNegative(length);

            switch (range)
            {
                case TypeCode.UInt32:
                {
                    uint sum = 0;

                    for (long i = 0; Hint.Likely(i < length); i++)
                    {
                        sum += ptr[i];
                    }

                    return sum;
                }
                case TypeCode.Empty:
                case TypeCode.UInt64:
                {
                    ulong sum = 0;

                    for (long i = 0; Hint.Likely(i < length); i++)
                    {
                        sum += ptr[i];
                    }

                    return sum;
                }
                default:
                {
#if DEBUG
                    throw new ArgumentException($"Invalid TypeCode argument '{ range }'. The supported type codes are: UInt32 and UInt64.");
#else
                    return 0;
#endif
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeArray<uint> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeArray<uint> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeArray<uint> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeList<uint> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeList<uint> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeList<uint> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeSlice<uint> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeSlice<uint> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeSlice<uint> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(ulong* ptr, long length)
        {
Assert.IsNonNegative(length);

            ulong sum = 0;

            for (long i = 0; Hint.Likely(i < length); i++)
            {
                sum += ptr[i];
            }

            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeArray<ulong> array)
        {
            return SIMD_Sum((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeArray<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeArray<ulong> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeList<ulong> array)
        {
            return SIMD_Sum((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeList<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeList<ulong> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeSlice<ulong> array)
        {
            return SIMD_Sum((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeSlice<ulong> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(this NativeSlice<ulong> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(sbyte* ptr, long length, TypeCode range = TypeCode.Empty)
        {
Assert.IsNonNegative(length);

            range = (range == TypeCode.Empty) ? GetSafeRange.Summation(TypeCode.SByte, length) : range;

            switch (range)
            {
                case TypeCode.SByte:
                {
                    sbyte sum = 0;

                    for (long i = 0; Hint.Likely(i < length); i++)
                    {
                        sum += ptr[i];
                    }

                    return sum;
                }
                case TypeCode.Int16:
                {
                    short sum = 0;

                    for (long i = 0; Hint.Likely(i < length); i++)
                    {
                        sum += ptr[i];
                    }

                    return sum;
                }
                case TypeCode.Int32:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v128* address = (v128*)ptr;
                        v256 ints = default(v256);
                
                
                        while (Hint.Likely(length >= 64))
                        {
                            int iterations = 0;
                            v256 acc0 = default(v256);
                            v256 acc1 = default(v256);
                            v256 acc2 = default(v256);
                            v256 acc3 = default(v256);
                
                            while (Hint.Likely(iterations < short.MinValue / sbyte.MinValue))
                            {
                                acc0 = Avx2.mm256_add_epi16(acc0, Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++)));
                                acc1 = Avx2.mm256_add_epi16(acc1, Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++)));
                                acc2 = Avx2.mm256_add_epi16(acc2, Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++)));
                                acc3 = Avx2.mm256_add_epi16(acc3, Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++)));
                
                                iterations++;
                
                                if (Hint.Unlikely((length = length - 64) < 64))
                                {
                                    break;
                                }
                                else continue;
                            }
                
                            v256 cast0 = Avx2.mm256_add_epi32(Avx2.mm256_cvtepi16_epi32(Avx.mm256_castsi256_si128(acc0)),
                                                              Avx2.mm256_cvtepi16_epi32(Avx2.mm256_extracti128_si256(acc0, 1)));
                            v256 cast1 = Avx2.mm256_add_epi32(Avx2.mm256_cvtepi16_epi32(Avx.mm256_castsi256_si128(acc1)),
                                                              Avx2.mm256_cvtepi16_epi32(Avx2.mm256_extracti128_si256(acc1, 1)));
                            v256 cast2 = Avx2.mm256_add_epi32(Avx2.mm256_cvtepi16_epi32(Avx.mm256_castsi256_si128(acc2)),
                                                              Avx2.mm256_cvtepi16_epi32(Avx2.mm256_extracti128_si256(acc2, 1)));
                            v256 cast3 = Avx2.mm256_add_epi32(Avx2.mm256_cvtepi16_epi32(Avx.mm256_castsi256_si128(acc3)),
                                                              Avx2.mm256_cvtepi16_epi32(Avx2.mm256_extracti128_si256(acc3, 1)));
                
                            ints = Avx2.mm256_add_epi32(ints, Avx2.mm256_add_epi32(Avx2.mm256_add_epi32(cast0, cast1),
                                                                                   Avx2.mm256_add_epi32(cast2, cast3)));
                        }
                        

                        if (Hint.Likely((int)length >= 16))
                        {
                            v256 shorts = Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                shorts = Avx2.mm256_add_epi16(shorts, Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++)));
                            
                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    shorts = Avx2.mm256_add_epi16(shorts, Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++)));
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
                            
                            ints = Avx2.mm256_add_epi32(ints, Avx2.mm256_cvtepi16_epi32(Sse2.add_epi16(Avx.mm256_castsi256_si128(shorts), Avx2.mm256_extracti128_si256(shorts, 1))));
                        }
                        else { }

                        if (Hint.Likely((int)length >= 8))
                        {
                            ints = Avx2.mm256_add_epi32(ints, Avx2.mm256_cvtepi8_epi32(Sse2.cvtsi64x_si128(*(long*)address)));

                            length -= 8;
                            address = (v128*)((long*)address + 1);
                        }
                        else { }

                        v128 csum_0 = Sse2.add_epi32(Avx.mm256_castsi256_si128(ints), Avx2.mm256_extracti128_si256(ints, 1));

                        if (Hint.Likely((int)length >= 4))
                        {
                            csum_0 = Sse2.add_epi32(csum_0, Sse4_1.cvtepi8_epi32(Sse2.cvtsi32_si128(*(int*)address)));

                            length -= 4;
                            address = (v128*)((int*)address + 1);
                        }
                        else { }

                        v128 csum_1 = Sse2.add_epi32(csum_0, Sse2.shuffle_epi32(csum_0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            csum_1 = Sse2.add_epi32(csum_1, Sse4_1.cvtepi8_epi32(Sse2.cvtsi32_si128(*(ushort*)address)));

                            length -= 2;
                            address = (v128*)((ushort*)address + 1);
                        }
                        else { }

                        v128 csum_2 = Sse2.add_epi32(csum_1, Sse2.shuffle_epi32(csum_1, Sse.SHUFFLE(0, 0, 0, 1)));
                        int sum = csum_2.SInt0;

                        if (Hint.Likely(length != 0))
                        {
                            sum += *(sbyte*)address;
                        }
                        else { }


                        return sum;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128* address = (v128*)ptr;
                        v128 ints = default(v128);
                        v128 ZERO = default(v128);
                
                
                        while (Hint.Likely(length >= 32))
                        {
                            int iterations = 0;
                            v128 acc0 = default(v128);
                            v128 acc1 = default(v128);
                            v128 acc2 = default(v128);
                            v128 acc3 = default(v128);
                
                            while (Hint.Likely(iterations < short.MinValue / sbyte.MinValue))
                            {
                                v128 _0_15 = Sse2.loadu_si128(address++);
                                v128 _16_31 = Sse2.loadu_si128(address++);

                                if (Sse4_1.IsSse41Supported)
                                {
                                    acc0 = Sse2.add_epi16(acc0, Sse4_1.cvtepi8_epi16(_0_15));
                                    acc1 = Sse2.add_epi16(acc1, Sse4_1.cvtepi8_epi16(Sse2.shuffle_epi32(_0_15, Sse.SHUFFLE(0, 0, 3, 2))));
                                    acc2 = Sse2.add_epi16(acc2, Sse4_1.cvtepi8_epi16(_16_31));
                                    acc3 = Sse2.add_epi16(acc3, Sse4_1.cvtepi8_epi16(Sse2.shuffle_epi32(_16_31, Sse.SHUFFLE(0, 0, 3, 2))));
                                }
                                else
                                {
                                    v128 sign_lo = Sse2.cmpgt_epi8(ZERO, _0_15);
                                    v128 sign_hi = Sse2.cmpgt_epi8(ZERO, _16_31);

                                    acc0 = Sse2.add_epi16(acc0, Sse2.unpacklo_epi8(_0_15, sign_lo));
                                    acc1 = Sse2.add_epi16(acc1, Sse2.unpackhi_epi8(_0_15, sign_lo));
                                    acc2 = Sse2.add_epi16(acc2, Sse2.unpacklo_epi8(_16_31, sign_hi));
                                    acc3 = Sse2.add_epi16(acc3, Sse2.unpackhi_epi8(_16_31, sign_hi));
                                }

                
                                iterations++;
                
                                if (Hint.Unlikely((length = length - 32) < 32))
                                {
                                    break;
                                }
                                else continue;
                            }

                            if (Sse4_1.IsSse41Supported)
                            {
                                v128 cast0 = Sse2.add_epi32(Sse4_1.cvtepi16_epi32(acc0),
                                                            Sse4_1.cvtepi16_epi32(Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 3, 2))));
                                v128 cast1 = Sse2.add_epi32(Sse4_1.cvtepi16_epi32(acc1),
                                                            Sse4_1.cvtepi16_epi32(Sse2.shuffle_epi32(acc1, Sse.SHUFFLE(0, 0, 3, 2))));
                                v128 cast2 = Sse2.add_epi32(Sse4_1.cvtepi16_epi32(acc2),
                                                            Sse4_1.cvtepi16_epi32(Sse2.shuffle_epi32(acc2, Sse.SHUFFLE(0, 0, 3, 2))));
                                v128 cast3 = Sse2.add_epi32(Sse4_1.cvtepi16_epi32(acc3),
                                                            Sse4_1.cvtepi16_epi32(Sse2.shuffle_epi32(acc3, Sse.SHUFFLE(0, 0, 3, 2))));

                                ints = Sse2.add_epi32(ints, Sse2.add_epi32(Sse2.add_epi32(cast0, cast1),
                                                                           Sse2.add_epi32(cast2, cast3)));
                            }
                            else
                            {
                                v128 sign0 = Sse2.cmpgt_epi16(ZERO, acc0);
                                v128 sign1 = Sse2.cmpgt_epi16(ZERO, acc1);
                                v128 sign2 = Sse2.cmpgt_epi16(ZERO, acc2);
                                v128 sign3 = Sse2.cmpgt_epi16(ZERO, acc3);

                                v128 cast0 = Sse2.add_epi32(Sse2.unpacklo_epi16(acc0, sign0),
                                                            Sse2.unpackhi_epi16(acc0, sign0));
                                v128 cast1 = Sse2.add_epi32(Sse2.unpacklo_epi16(acc1, sign1),
                                                            Sse2.unpackhi_epi16(acc1, sign1));
                                v128 cast2 = Sse2.add_epi32(Sse2.unpacklo_epi16(acc2, sign2),
                                                            Sse2.unpackhi_epi16(acc2, sign2));
                                v128 cast3 = Sse2.add_epi32(Sse2.unpacklo_epi16(acc3, sign3),
                                                            Sse2.unpackhi_epi16(acc3, sign3));

                                ints = Sse2.add_epi32(ints, Sse2.add_epi32(Sse2.add_epi32(cast0, cast1),
                                                                           Sse2.add_epi32(cast2, cast3)));
                            }
                        }
                        

                        v128 shorts;

                        if (Hint.Likely((int)length >= 16))
                        {
                            v128 load = Sse2.loadu_si128(address++);

                            if (Sse4_1.IsSse41Supported)
                            {
                                shorts = Sse2.add_epi16(Sse4_1.cvtepi8_epi16(load),
                                                        Sse4_1.cvtepi8_epi16(Sse2.shuffle_epi32(load, Sse.SHUFFLE(0, 0, 3, 2))));
                            }
                            else
                            {
                                v128 sign8 = Sse2.cmpgt_epi8(ZERO, load);

                                shorts = Sse2.add_epi16(Sse2.unpacklo_epi8(load, sign8),
                                                        Sse2.unpackhi_epi8(load, sign8));
                            }

                            length -= 16;
                        }
                        else 
                        {
                            shorts = default(v128);
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            v128 load = Sse2.cvtsi64x_si128(*(long*)address);

                            if (Sse4_1.IsSse41Supported)
                            {
                                shorts = Sse2.add_epi16(shorts, Sse4_1.cvtepi8_epi16(load));
                            }
                            else
                            {
                                shorts = Sse2.add_epi16(shorts, Sse2.unpacklo_epi8(load, Sse2.cmpgt_epi8(ZERO, load)));
                            }

                            address = (v128*)((long*)address + 1);
                            length -= 8;
                        }
                        else { }

                        if (Hint.Likely((int)length >= 4))
                        {
                            v128 load = Sse2.cvtsi32_si128(*(int*)address);

                            if (Sse4_1.IsSse41Supported)
                            {
                                shorts = Sse2.add_epi16(shorts, Sse4_1.cvtepi8_epi16(load));
                            }
                            else
                            {
                                shorts = Sse2.add_epi16(shorts, Sse2.unpacklo_epi8(load, Sse2.cmpgt_epi8(ZERO, load)));
                            }

                            address = (v128*)((int*)address + 1);
                            length -= 4;
                        }
                        else { }

                        if (Hint.Likely((int)length >= 2))
                        {
                            v128 load = Sse2.cvtsi32_si128(*(ushort*)address);

                            if (Sse4_1.IsSse41Supported)
                            {
                                shorts = Sse2.add_epi16(shorts, Sse4_1.cvtepi8_epi16(load));
                            }
                            else
                            {
                                shorts = Sse2.add_epi16(shorts, Sse2.unpacklo_epi8(load, Sse2.cmpgt_epi8(ZERO, load)));
                            }

                            address = (v128*)((ushort*)address + 1);
                            length -= 2;
                        }
                        else { }

                        if (Sse4_1.IsSse41Supported)
                        {
                            ints = Sse2.add_epi32(ints, Sse4_1.cvtepi16_epi32(Sse2.add_epi16(shorts,
                                                                                             Sse2.shuffle_epi32(shorts, Sse.SHUFFLE(0, 0, 3, 2)))));
                        }
                        else
                        {
                            shorts = Sse2.add_epi16(shorts,
                                                    Sse2.shuffle_epi32(shorts, Sse.SHUFFLE(0, 0, 3, 2)));

                            ints = Sse2.add_epi32(ints, Sse2.unpacklo_epi16(shorts, Sse2.cmpgt_epi16(ZERO, shorts)));
                        }

                        v128 csum_0 = Sse2.add_epi32(ints, Sse2.shuffle_epi32(ints, Sse.SHUFFLE(0, 0, 3, 2)));
                        v128 csum_1 = Sse2.add_epi32(csum_0, Sse2.shuffle_epi32(csum_0, Sse.SHUFFLE(0, 0, 0, 1)));
                        int sum = csum_1.SInt0;

                        if (Hint.Likely(length != 0))
                        {
                            sum += *(sbyte*)address;
                        }
                        else { }
                
                
                        return sum;
                    }
                    else
                    {
                        int sum = 0;
                
                        for (long i = 0; i < length; i++)
                        {
                            sum += ptr[i];
                        }
                
                        return sum;
                    }
                }
                case TypeCode.Int64:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v128* address = (v128*)ptr;
                        v256 longs = default(v256);

                        while (Hint.Likely(length >= 64))
                        {
                            int iterations_int = 0;
                            v256 ints0 = default(v256);
                            v256 ints1 = default(v256);
                            v256 ints2 = default(v256);
                            v256 ints3 = default(v256);

                            while (Hint.Likely(iterations_int < int.MinValue / short.MinValue))
                            {
                                int iterations_short = 0;
                                v256 shorts0 = default(v256);
                                v256 shorts1 = default(v256);
                                v256 shorts2 = default(v256);
                                v256 shorts3 = default(v256);

                                while (Hint.Likely(iterations_short < short.MinValue / sbyte.MinValue))
                                {
                                    shorts0 = Avx2.mm256_add_epi16(shorts0, Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++)));
                                    shorts1 = Avx2.mm256_add_epi16(shorts1, Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++)));
                                    shorts2 = Avx2.mm256_add_epi16(shorts2, Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++)));
                                    shorts3 = Avx2.mm256_add_epi16(shorts3, Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++)));

                                    iterations_short++;

                                    if (Hint.Unlikely((length = length - 64) < 64))
                                    {
                                        break;
                                    }
                                    else continue;
                                }

                                ints0 = Avx2.mm256_add_epi32(ints0, Avx2.mm256_add_epi32(Avx2.mm256_cvtepi16_epi32(Avx.mm256_castsi256_si128(shorts0)),
                                                                                         Avx2.mm256_cvtepi16_epi32(Avx2.mm256_extracti128_si256(shorts0, 1))));
                                ints1 = Avx2.mm256_add_epi32(ints1, Avx2.mm256_add_epi32(Avx2.mm256_cvtepi16_epi32(Avx.mm256_castsi256_si128(shorts1)),
                                                                                         Avx2.mm256_cvtepi16_epi32(Avx2.mm256_extracti128_si256(shorts1, 1))));
                                ints2 = Avx2.mm256_add_epi32(ints2, Avx2.mm256_add_epi32(Avx2.mm256_cvtepi16_epi32(Avx.mm256_castsi256_si128(shorts2)),
                                                                                         Avx2.mm256_cvtepi16_epi32(Avx2.mm256_extracti128_si256(shorts2, 1))));
                                ints3 = Avx2.mm256_add_epi32(ints3, Avx2.mm256_add_epi32(Avx2.mm256_cvtepi16_epi32(Avx.mm256_castsi256_si128(shorts3)),
                                                                                         Avx2.mm256_cvtepi16_epi32(Avx2.mm256_extracti128_si256(shorts3, 1))));

                                iterations_int++;

                                if (Hint.Unlikely(length < 64))
                                {
                                    break;
                                }
                                else continue;
                            }

                            v256 add0 = Avx2.mm256_add_epi64(Avx2.mm256_cvtepi32_epi64(Avx.mm256_castsi256_si128(ints0)),
                                                             Avx2.mm256_cvtepi32_epi64(Avx2.mm256_extracti128_si256(ints0, 1)));
                            v256 add1 = Avx2.mm256_add_epi64(Avx2.mm256_cvtepi32_epi64(Avx.mm256_castsi256_si128(ints1)),
                                                             Avx2.mm256_cvtepi32_epi64(Avx2.mm256_extracti128_si256(ints1, 1)));
                            v256 add2 = Avx2.mm256_add_epi64(Avx2.mm256_cvtepi32_epi64(Avx.mm256_castsi256_si128(ints2)),
                                                             Avx2.mm256_cvtepi32_epi64(Avx2.mm256_extracti128_si256(ints2, 1)));
                            v256 add3 = Avx2.mm256_add_epi64(Avx2.mm256_cvtepi32_epi64(Avx.mm256_castsi256_si128(ints3)),
                                                             Avx2.mm256_cvtepi32_epi64(Avx2.mm256_extracti128_si256(ints3, 1)));

                            longs = Avx2.mm256_add_epi64(longs, Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(add0, add1),
                                                                                     Avx2.mm256_add_epi64(add2, add3)));
                        }


                        v128 shortSum;

                        if (Hint.Likely((int)length >= 16))
                        {
                            v256 shorts = Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                shorts = Avx2.mm256_add_epi16(shorts, Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++)));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    shorts = Avx2.mm256_add_epi16(shorts, Avx2.mm256_cvtepi8_epi16(Sse2.loadu_si128(address++)));
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

                            shortSum = Sse2.add_epi16(Avx.mm256_castsi256_si128(shorts), Avx2.mm256_extracti128_si256(shorts, 1));
                        }
                        else 
                        {
                            shortSum = default(v128);
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            shortSum = Sse2.add_epi16(shortSum, Sse4_1.cvtepi8_epi16(Sse2.cvtsi64x_si128(*(long*)address)));

                            address = (v128*)((long*)address + 1);
                            length -= 8;
                        }
                        else { }

                        if (Hint.Likely((int)length >= 4))
                        {
                            shortSum = Sse2.add_epi16(shortSum, Sse4_1.cvtepi8_epi16(Sse2.cvtsi32_si128(*(int*)address)));

                            address = (v128*)((int*)address + 1);
                            length -= 4;
                        }
                        else { }

                        if (Hint.Likely((int)length >= 2))
                        {
                            shortSum = Sse2.add_epi16(shortSum, Sse4_1.cvtepi8_epi16(Sse2.cvtsi32_si128(*(ushort*)address)));

                            address = (v128*)((ushort*)address + 1);
                            length -= 2;
                        }
                        else { }

                        shortSum = Sse2.add_epi16(shortSum, Sse2.shuffle_epi32(shortSum, Sse.SHUFFLE(0, 0, 3, 2)));
                        longs = Avx2.mm256_add_epi64(longs, Avx2.mm256_cvtepi16_epi64(shortSum));

                        v128 csum_0 = Sse2.add_epi64(Avx.mm256_castsi256_si128(longs), Avx2.mm256_extracti128_si256(longs, 1));
                        v128 csum_1 = Sse2.add_epi64(csum_0, Sse2.shuffle_epi32(csum_0, Sse.SHUFFLE(0, 0, 3, 2)));
                        long sum = csum_1.SLong0;


                        if (Hint.Likely(length != 0))
                        {
                            sum += *(sbyte*)address;
                        }
                        else { }
                        
                        
                        return sum;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128* address = (v128*)ptr;
                        v128 longs = default(v128);
                        v128 ZERO = default(v128);

                        while (Hint.Likely(length >= 32))
                        {
                            int iterations_int = 0;
                            v128 ints0 = default(v128);
                            v128 ints1 = default(v128);
                            v128 ints2 = default(v128);
                            v128 ints3 = default(v128);

                            while (Hint.Likely(iterations_int < int.MinValue / short.MinValue))
                            {
                                int iterations_short = 0;
                                v128 shorts0 = default(v128);
                                v128 shorts1 = default(v128);
                                v128 shorts2 = default(v128);
                                v128 shorts3 = default(v128);

                                while (Hint.Likely(iterations_short < short.MinValue / sbyte.MinValue))
                                {
                                    v128 _0_15 = Sse2.loadu_si128(address++);
                                    v128 _16_31 = Sse2.loadu_si128(address++);

                                    if (Sse4_1.IsSse41Supported)
                                    {
                                        shorts0 = Sse2.add_epi16(shorts0, Sse4_1.cvtepi8_epi16(_0_15));
                                        shorts1 = Sse2.add_epi16(shorts1, Sse4_1.cvtepi8_epi16(Sse2.shuffle_epi32(_0_15, Sse.SHUFFLE(0, 0, 3, 2))));
                                        shorts2 = Sse2.add_epi16(shorts2, Sse4_1.cvtepi8_epi16(_16_31));
                                        shorts3 = Sse2.add_epi16(shorts3, Sse4_1.cvtepi8_epi16(Sse2.shuffle_epi32(_16_31, Sse.SHUFFLE(0, 0, 3, 2))));
                                    }
                                    else
                                    {
                                        v128 sign_lo = Sse2.cmpgt_epi8(ZERO, _0_15);
                                        v128 sign_hi = Sse2.cmpgt_epi8(ZERO, _16_31);

                                        shorts0 = Sse2.add_epi16(shorts0, Sse2.unpacklo_epi8(_0_15, sign_lo));
                                        shorts1 = Sse2.add_epi16(shorts1, Sse2.unpackhi_epi8(_0_15, sign_lo));
                                        shorts2 = Sse2.add_epi16(shorts2, Sse2.unpacklo_epi8(_16_31, sign_hi));
                                        shorts3 = Sse2.add_epi16(shorts3, Sse2.unpackhi_epi8(_16_31, sign_hi));
                                    }

                                    iterations_short++;

                                    if (Hint.Unlikely((length = length - 32) < 32))
                                    {
                                        break;
                                    }
                                    else continue;
                                }

                                if (Sse4_1.IsSse41Supported)
                                {
                                    ints0 = Sse2.add_epi32(ints0, Sse2.add_epi32(Sse4_1.cvtepi16_epi32(shorts0),
                                                                                 Sse4_1.cvtepi16_epi32(Sse2.shuffle_epi32(shorts0, Sse.SHUFFLE(0, 0, 3, 2)))));
                                    ints1 = Sse2.add_epi32(ints1, Sse2.add_epi32(Sse4_1.cvtepi16_epi32(shorts1),
                                                                                 Sse4_1.cvtepi16_epi32(Sse2.shuffle_epi32(shorts1, Sse.SHUFFLE(0, 0, 3, 2)))));
                                    ints2 = Sse2.add_epi32(ints2, Sse2.add_epi32(Sse4_1.cvtepi16_epi32(shorts2),
                                                                                 Sse4_1.cvtepi16_epi32(Sse2.shuffle_epi32(shorts2, Sse.SHUFFLE(0, 0, 3, 2)))));
                                    ints3 = Sse2.add_epi32(ints3, Sse2.add_epi32(Sse4_1.cvtepi16_epi32(shorts3),
                                                                                 Sse4_1.cvtepi16_epi32(Sse2.shuffle_epi32(shorts3, Sse.SHUFFLE(0, 0, 3, 2)))));
                                }
                                else
                                {
                                    v128 sign0 = Sse2.cmpgt_epi16(ZERO, shorts0);
                                    v128 sign1 = Sse2.cmpgt_epi16(ZERO, shorts1);
                                    v128 sign2 = Sse2.cmpgt_epi16(ZERO, shorts2);
                                    v128 sign3 = Sse2.cmpgt_epi16(ZERO, shorts3);

                                    ints0 = Sse2.add_epi32(ints0, Sse2.add_epi32(Sse2.unpacklo_epi16(shorts0, sign0),
                                                                                 Sse2.unpackhi_epi16(shorts0, sign0)));
                                    ints1 = Sse2.add_epi32(ints1, Sse2.add_epi32(Sse2.unpacklo_epi16(shorts1, sign1),
                                                                                 Sse2.unpackhi_epi16(shorts1, sign1)));
                                    ints2 = Sse2.add_epi32(ints2, Sse2.add_epi32(Sse2.unpacklo_epi16(shorts2, sign2),
                                                                                 Sse2.unpackhi_epi16(shorts2, sign2)));
                                    ints3 = Sse2.add_epi32(ints3, Sse2.add_epi32(Sse2.unpacklo_epi16(shorts3, sign3),
                                                                                 Sse2.unpackhi_epi16(shorts3, sign3)));
                                }

                                iterations_int++;

                                if (Hint.Unlikely(length < 32))
                                {
                                    break;
                                }
                                else continue;
                            }

                            if (Sse4_1.IsSse41Supported)
                            {
                                v128 add0 = Sse2.add_epi64(Sse4_1.cvtepi32_epi64(ints0),
                                                           Sse4_1.cvtepi32_epi64(Sse2.shuffle_epi32(ints0, Sse.SHUFFLE(0, 0, 3, 2))));
                                v128 add1 = Sse2.add_epi64(Sse4_1.cvtepi32_epi64(ints1),
                                                           Sse4_1.cvtepi32_epi64(Sse2.shuffle_epi32(ints1, Sse.SHUFFLE(0, 0, 3, 2))));
                                v128 add2 = Sse2.add_epi64(Sse4_1.cvtepi32_epi64(ints2),
                                                           Sse4_1.cvtepi32_epi64(Sse2.shuffle_epi32(ints2, Sse.SHUFFLE(0, 0, 3, 2))));
                                v128 add3 = Sse2.add_epi64(Sse4_1.cvtepi32_epi64(ints3),
                                                           Sse4_1.cvtepi32_epi64(Sse2.shuffle_epi32(ints3, Sse.SHUFFLE(0, 0, 3, 2))));

                                longs = Sse2.add_epi64(longs, Sse2.add_epi64(Sse2.add_epi64(add0, add1),
                                                                             Sse2.add_epi64(add2, add3)));
                            }
                            else
                            {
                                v128 sign0 = Sse2.cmpgt_epi32(ZERO, ints0);
                                v128 sign1 = Sse2.cmpgt_epi32(ZERO, ints1);
                                v128 sign2 = Sse2.cmpgt_epi32(ZERO, ints2);
                                v128 sign3 = Sse2.cmpgt_epi32(ZERO, ints3);

                                v128 add0 = Sse2.add_epi64(Sse2.unpacklo_epi32(ints0, sign0),
                                                           Sse2.unpackhi_epi32(ints0, sign0));
                                v128 add1 = Sse2.add_epi64(Sse2.unpacklo_epi32(ints1, sign1),
                                                           Sse2.unpackhi_epi32(ints1, sign1));
                                v128 add2 = Sse2.add_epi64(Sse2.unpacklo_epi32(ints2, sign2),
                                                           Sse2.unpackhi_epi32(ints2, sign2));
                                v128 add3 = Sse2.add_epi64(Sse2.unpacklo_epi32(ints3, sign3),
                                                           Sse2.unpackhi_epi32(ints3, sign3));

                                longs = Sse2.add_epi64(longs, Sse2.add_epi64(Sse2.add_epi64(add0, add1),
                                                                             Sse2.add_epi64(add2, add3)));
                            }
                        }


                        v128 shorts;

                        if (Hint.Likely((int)length >= 16))
                        {
                            v128 load = Sse2.loadu_si128(address++);

                            if (Sse4_1.IsSse41Supported)
                            {
                                shorts = Sse2.add_epi16(Sse4_1.cvtepi8_epi16(load),
                                                        Sse4_1.cvtepi8_epi16(Sse2.shuffle_epi32(load, Sse.SHUFFLE(0, 0, 3, 2))));
                            }
                            else
                            {
                                v128 sign8 = Sse2.cmpgt_epi8(ZERO, load);
                                shorts = Sse2.add_epi16(Sse2.unpacklo_epi8(load, sign8),
                                                        Sse2.unpackhi_epi8(load, sign8));
                            }

                            length -= 16;
                        }
                        else 
                        {
                            shorts = default(v128);
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            v128 load = Sse2.cvtsi64x_si128(*(long*)address);
                        
                            if (Sse4_1.IsSse41Supported)
                            {
                                shorts = Sse2.add_epi16(shorts, Sse4_1.cvtepi8_epi16(load));
                            }
                            else
                            {
                                shorts = Sse2.add_epi16(shorts, Sse2.unpacklo_epi8(load, Sse2.cmpgt_epi8(ZERO, load)));
                            }
                        
                            length -= 8;
                            address = (v128*)((long*)address + 1);
                        }
                        else { }
                        
                        if (Hint.Likely((int)length >= 4))
                        {
                            v128 load = Sse2.cvtsi32_si128(*(int*)address);
                        
                            if (Sse4_1.IsSse41Supported)
                            {
                                shorts = Sse2.add_epi16(shorts, Sse4_1.cvtepi8_epi16(load));
                            }
                            else
                            {
                                shorts = Sse2.add_epi16(shorts, Sse2.unpacklo_epi8(load, Sse2.cmpgt_epi8(ZERO, load)));
                            }
                        
                            length -= 4;
                            address = (v128*)((int*)address + 1);
                        }
                        else { }
                        
                        if (Hint.Likely((int)length >= 2))
                        {
                            v128 load = Sse2.cvtsi32_si128(*(ushort*)address);
                        
                            if (Sse4_1.IsSse41Supported)
                            {
                                shorts = Sse2.add_epi16(shorts, Sse4_1.cvtepi8_epi16(load));
                            }
                            else
                            {
                                shorts = Sse2.add_epi16(shorts, Sse2.unpacklo_epi8(load, Sse2.cmpgt_epi8(ZERO, load)));
                            }
                        
                            length -= 2;
                            address = (v128*)((ushort*)address + 1);
                        }
                        else { }
                        
                        if (Sse4_1.IsSse41Supported)
                        {
                            v128 shortSum = Sse2.add_epi16(shorts, Sse2.shuffle_epi32(shorts, Sse.SHUFFLE(0, 0, 3, 2)));
                            shortSum = Sse2.add_epi16(shortSum, Sse2.shufflelo_epi16(shortSum, Sse.SHUFFLE(0, 0, 3, 2)));
                            longs = Sse2.add_epi64(longs, Sse4_1.cvtepi16_epi64(shortSum));
                        }
                        else
                        {
                            v128 shortSum = Sse2.add_epi16(shorts, Sse2.shuffle_epi32(shorts, Sse.SHUFFLE(0, 0, 3, 2)));
                            shortSum = Sse2.add_epi16(shortSum, Sse2.shufflelo_epi16(shortSum, Sse.SHUFFLE(0, 0, 3, 2)));
                        
                            v128 sign16 = Sse2.cmpgt_epi16(ZERO, shortSum);
                            v128 ints = Sse2.unpacklo_epi16(shortSum, sign16);
                        
                            sign16 = Sse2.unpacklo_epi16(sign16, sign16);
                            longs = Sse2.add_epi64(longs, Sse2.unpacklo_epi32(ints, sign16));
                        }

                        v128 csum_0 = Sse2.add_epi64(longs, Sse2.shuffle_epi32(longs, Sse.SHUFFLE(0, 0, 3, 2)));
                        long sum = csum_0.SLong0;

                        if (Hint.Likely(length != 0))
                        {
                            sum += *(sbyte*)address;
                        }


                        return sum;
                    }
                    else
                    {
                        long sum = 0;

                        for (long i = 0; i < length; i++)
                        {
                            sum += ptr[i];
                        }

                        return sum;
                    }
                }
                default:
                {
#if DEBUG
                    throw new ArgumentException($"Invalid TypeCode argument '{ range }'. The supported type codes are: SByte, Int16, Int32 and Int64");
#else
                    return 0;
#endif
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeArray<sbyte> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeArray<sbyte> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeArray<sbyte> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeList<sbyte> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeList<sbyte> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeList<sbyte> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeSlice<sbyte> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeSlice<sbyte> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeSlice<sbyte> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(short* ptr, long length, TypeCode range = TypeCode.Empty)
        {
Assert.IsNonNegative(length);

            range = (range == TypeCode.Empty) ? GetSafeRange.Summation(TypeCode.Int16, length) : range;

            switch (range)
            {
                case TypeCode.Int16:
                {
                    short sum = 0;

                    for (long i = 0; Hint.Likely(i < length); i++)
                    {
                        sum += ptr[i];
                    }

                    return sum;
                }
                case TypeCode.Int32:
                {
                    int sum = 0;

                    for (long i = 0; Hint.Likely(i < length); i++)
                    {
                        sum += ptr[i];
                    }

                    return sum;
                }
                case TypeCode.Int64:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v128* address = (v128*)ptr;
                        v256 longs = default(v256);
                        
                        
                        while (Hint.Likely(length >= 32))
                        {
                            int iterations = 0;
                            v256 acc0 = default(v256);
                            v256 acc1 = default(v256);
                            v256 acc2 = default(v256);
                            v256 acc3 = default(v256);
                        
                            while (Hint.Likely(iterations < int.MinValue / short.MinValue))
                            {
                                acc0 = Avx2.mm256_add_epi32(acc0, Avx2.mm256_cvtepi16_epi32(Sse2.loadu_si128(address++)));
                                acc1 = Avx2.mm256_add_epi32(acc1, Avx2.mm256_cvtepi16_epi32(Sse2.loadu_si128(address++)));
                                acc2 = Avx2.mm256_add_epi32(acc2, Avx2.mm256_cvtepi16_epi32(Sse2.loadu_si128(address++)));
                                acc3 = Avx2.mm256_add_epi32(acc3, Avx2.mm256_cvtepi16_epi32(Sse2.loadu_si128(address++)));
                        
                                iterations++;
                        
                                if (Hint.Unlikely((length = length - 32) < 32))
                                {
                                    break;
                                }
                                else continue;
                            }
                        
                            v256 cast0 = Avx2.mm256_add_epi64(Avx2.mm256_cvtepi32_epi64(Avx.mm256_castsi256_si128(acc0)),
                                                              Avx2.mm256_cvtepi32_epi64(Avx2.mm256_extracti128_si256(acc0, 1)));
                            v256 cast1 = Avx2.mm256_add_epi64(Avx2.mm256_cvtepi32_epi64(Avx.mm256_castsi256_si128(acc1)),
                                                              Avx2.mm256_cvtepi32_epi64(Avx2.mm256_extracti128_si256(acc1, 1)));
                            v256 cast2 = Avx2.mm256_add_epi64(Avx2.mm256_cvtepi32_epi64(Avx.mm256_castsi256_si128(acc2)),
                                                              Avx2.mm256_cvtepi32_epi64(Avx2.mm256_extracti128_si256(acc2, 1)));
                            v256 cast3 = Avx2.mm256_add_epi64(Avx2.mm256_cvtepi32_epi64(Avx.mm256_castsi256_si128(acc3)),
                                                              Avx2.mm256_cvtepi32_epi64(Avx2.mm256_extracti128_si256(acc3, 1)));
                        
                            longs = Avx2.mm256_add_epi64(longs, Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(cast0, cast1),
                                                                                     Avx2.mm256_add_epi64(cast2, cast3)));
                        }


                        if (Hint.Likely((int)length >= 8))
                        {
                            v256 ints = Avx2.mm256_cvtepi16_epi32(Sse2.loadu_si128(address++));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                ints = Avx2.mm256_add_epi32(ints, Avx2.mm256_cvtepi16_epi32(Sse2.loadu_si128(address++)));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    ints = Avx2.mm256_add_epi32(ints, Avx2.mm256_cvtepi16_epi32(Sse2.loadu_si128(address++)));
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

                            v128 intSum = Sse2.add_epi32(Avx.mm256_castsi256_si128(ints), Avx2.mm256_extracti128_si256(ints, 1));
                            longs = Avx2.mm256_add_epi64(longs, Avx2.mm256_cvtepi32_epi64(intSum));
                        }
                        else { }

                        if (Hint.Likely((int)length >= 4))
                        {
                            longs = Avx2.mm256_add_epi64(longs, Avx2.mm256_cvtepi16_epi64(Sse2.cvtsi64x_si128(*(long*)address)));

                            length -= 4;
                            address = (v128*)((long*)address + 1);
                        }
                        else { }

                        v128 csum_0 = Sse2.add_epi64(Avx.mm256_castsi256_si128(longs), Avx2.mm256_extracti128_si256(longs, 1));

                        if (Hint.Likely((int)length >= 2))
                        {
                            csum_0 = Sse2.add_epi64(csum_0, Sse4_1.cvtepi16_epi64(Sse2.cvtsi32_si128(*(int*)address)));

                            length -= 2;
                            address = (v128*)((int*)address + 1);
                        }
                        else { }

                        v128 csum_1 = Sse2.add_epi64(csum_0, Sse2.shuffle_epi32(csum_0, Sse.SHUFFLE(0, 0, 3, 2)));
                        long sum = csum_1.SLong0;


                        if (Hint.Likely(length != 0))
                        {
                            sum += *(short*)address;
                        }
                        else { }
                        
                        
                        return sum;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128* address = (v128*)ptr;
                        v128 longs = default(v128);
                        v128 ZERO = default(v128);
                        
                        
                        while (Hint.Likely(length >= 16))
                        {
                            int iterations = 0;
                            v128 acc0 = default(v128);
                            v128 acc1 = default(v128);
                            v128 acc2 = default(v128);
                            v128 acc3 = default(v128);
                        
                            while (Hint.Likely(iterations < int.MinValue / short.MinValue))
                            {
                                v128 _0_7 = Sse2.loadu_si128(address++);
                                v128 _8_15 = Sse2.loadu_si128(address++);

                                if (Sse4_1.IsSse41Supported)
                                {
                                    acc0 = Sse2.add_epi32(acc0, Sse4_1.cvtepi16_epi32(_0_7));
                                    acc1 = Sse2.add_epi32(acc1, Sse4_1.cvtepi16_epi32(Sse2.shuffle_epi32(_0_7, Sse.SHUFFLE(0, 0, 3, 2))));
                                    acc2 = Sse2.add_epi32(acc2, Sse4_1.cvtepi16_epi32(_8_15));
                                    acc3 = Sse2.add_epi32(acc3, Sse4_1.cvtepi16_epi32(Sse2.shuffle_epi32(_8_15, Sse.SHUFFLE(0, 0, 3, 2))));
                                }
                                else
                                {
                                    v128 sign_lo = Sse2.cmpgt_epi16(ZERO, _0_7);
                                    v128 sign_hi = Sse2.cmpgt_epi16(ZERO, _8_15);

                                    acc0 = Sse2.add_epi32(acc0, Sse2.unpacklo_epi16(_0_7, sign_lo));
                                    acc1 = Sse2.add_epi32(acc1, Sse2.unpackhi_epi16(_0_7, sign_lo));
                                    acc2 = Sse2.add_epi32(acc2, Sse2.unpacklo_epi16(_8_15, sign_hi));
                                    acc3 = Sse2.add_epi32(acc3, Sse2.unpackhi_epi16(_8_15, sign_hi));
                                }
                        
                                iterations++;
                        
                                if (Hint.Unlikely((length = length - 16) < 16))
                                {
                                    break;
                                }
                                else continue;
                            }

                            if (Sse4_1.IsSse41Supported)
                            {
                                v128 cast0 = Sse2.add_epi64(Sse4_1.cvtepi32_epi64(acc0),
                                                            Sse4_1.cvtepi32_epi64(Sse2.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 3, 2))));
                                v128 cast1 = Sse2.add_epi64(Sse4_1.cvtepi32_epi64(acc1),
                                                            Sse4_1.cvtepi32_epi64(Sse2.shuffle_epi32(acc1, Sse.SHUFFLE(0, 0, 3, 2))));
                                v128 cast2 = Sse2.add_epi64(Sse4_1.cvtepi32_epi64(acc2),
                                                            Sse4_1.cvtepi32_epi64(Sse2.shuffle_epi32(acc2, Sse.SHUFFLE(0, 0, 3, 2))));
                                v128 cast3 = Sse2.add_epi64(Sse4_1.cvtepi32_epi64(acc3),
                                                            Sse4_1.cvtepi32_epi64(Sse2.shuffle_epi32(acc3, Sse.SHUFFLE(0, 0, 3, 2))));

                                longs = Sse2.add_epi64(longs, Sse2.add_epi64(Sse2.add_epi64(cast0, cast1),
                                                                             Sse2.add_epi64(cast2, cast3)));
                            }
                            else
                            {
                                v128 sign0 = Sse2.cmpgt_epi32(ZERO, acc0);
                                v128 sign1 = Sse2.cmpgt_epi32(ZERO, acc1);
                                v128 sign2 = Sse2.cmpgt_epi32(ZERO, acc2);
                                v128 sign3 = Sse2.cmpgt_epi32(ZERO, acc3);

                                v128 cast0 = Sse2.add_epi64(Sse2.unpacklo_epi32(acc0, sign0),
                                                            Sse2.unpackhi_epi32(acc0, sign0));
                                v128 cast1 = Sse2.add_epi64(Sse2.unpackhi_epi32(acc1, sign1),
                                                            Sse2.unpacklo_epi32(acc1, sign1));
                                v128 cast2 = Sse2.add_epi64(Sse2.unpackhi_epi32(acc2, sign2),
                                                            Sse2.unpacklo_epi32(acc2, sign2));
                                v128 cast3 = Sse2.add_epi64(Sse2.unpackhi_epi32(acc3, sign3),
                                                            Sse2.unpacklo_epi32(acc3, sign3));

                                longs = Sse2.add_epi64(longs, Sse2.add_epi64(Sse2.add_epi64(cast0, cast1),
                                                                             Sse2.add_epi64(cast2, cast3)));
                            }
                        }


                        v128 ints;

                        if (Hint.Likely((int)length >= 8))
                        {
                            v128 load = Sse2.loadu_si128(address++);

                            if (Sse4_1.IsSse41Supported)
                            {
                                ints = Sse2.add_epi32(Sse4_1.cvtepi16_epi32(load),
                                                      Sse4_1.cvtepi16_epi32(Sse2.shuffle_epi32(load, Sse.SHUFFLE(0, 0, 3, 2))));
                            }
                            else
                            {
                                v128 signShort = Sse2.cmpgt_epi16(ZERO, load);

                                ints = Sse2.add_epi32(Sse2.unpacklo_epi16(load, signShort),
                                                      Sse2.unpackhi_epi16(load, signShort));
                            }

                            length -= 8;
                        }
                        else
                        {
                            ints = default(v128);
                        }

                        if (Hint.Likely((int)length >= 4))
                        {
                            v128 load = Sse2.cvtsi64x_si128(*(long*)address);

                            if (Sse4_1.IsSse41Supported)
                            {
                                ints = Sse2.add_epi32(ints, Sse4_1.cvtepi16_epi32(load));
                            }
                            else
                            {
                                ints = Sse2.add_epi32(ints, Sse2.unpacklo_epi16(load, Sse2.cmpgt_epi16(ZERO, load)));
                            }

                            length -= 4;
                            address = (v128*)((long*)address + 1);
                        }
                        else { }

                        if (Hint.Likely((int)length >= 2))
                        {
                            v128 load = Sse2.cvtsi32_si128(*(int*)address);

                            if (Sse4_1.IsSse41Supported)
                            {
                                ints = Sse2.add_epi32(ints, Sse4_1.cvtepi16_epi32(load));
                            }
                            else
                            {
                                ints = Sse2.add_epi32(ints, Sse2.unpacklo_epi16(load, Sse2.cmpgt_epi16(ZERO, load)));
                            }

                            length -= 2;
                            address = (v128*)((int*)address + 1);
                        }
                        else { }

                        ints = Sse2.add_epi32(ints, Sse2.shuffle_epi32(ints, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Sse4_1.IsSse41Supported)
                        {
                            longs = Sse2.add_epi64(longs, Sse4_1.cvtepi32_epi64(ints));
                        }
                        else
                        {
                            v128 signInt = Sse2.cmpgt_epi32(ZERO, ints);

                            longs = Sse2.add_epi64(longs, Sse2.unpacklo_epi32(ints, signInt));
                        }

                        v128 csum_0 = Sse2.add_epi64(longs, Sse2.shuffle_epi32(longs, Sse.SHUFFLE(0, 0, 3, 2)));
                        long sum = csum_0.SLong0;


                        if (Hint.Likely(length != 0))
                        {
                            sum += *(short*)address;
                        }
                        else { }
                        
                        
                        return sum;
                    }
                    else
                    {
                        long sum = 0;

                        for (long i = 0; Hint.Likely(i < length); i++)
                        {
                            sum += ptr[i];
                        }

                        return sum;
                    }
                }
                default:
                {
#if DEBUG
                    throw new ArgumentException($"Invalid TypeCode argument '{ range }'. The supported type codes are: Int16, Int32 and Int64.");
#else
                    return 0;
#endif
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeArray<short> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((short*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeArray<short> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeArray<short> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeList<short> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((short*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeList<short> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeList<short> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeSlice<short> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((short*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeSlice<short> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeSlice<short> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(int* ptr, long length, TypeCode range = TypeCode.Empty)
        {
Assert.IsNonNegative(length);

            switch (range)
            {
                case TypeCode.Int32:
                {
                    int sum = 0;

                    for (long i = 0; Hint.Likely(i < length); i++)
                    {
                        sum += ptr[i];
                    }

                    return sum;
                }
                case TypeCode.Empty:
                case TypeCode.Int64:
                {
                    long sum = 0;

                    for (long i = 0; Hint.Likely(i < length); i++)
                    {
                        sum += ptr[i];
                    }

                    return sum;
                }
                default:
                {
#if DEBUG
                    throw new ArgumentException($"Invalid TypeCode argument '{ range }'. The supported type codes are: Int32 and Int64.");
#else
                    return 0;
#endif
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeArray<int> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((int*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeArray<int> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeArray<int> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeList<int> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((int*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeList<int> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeList<int> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeSlice<int> array, TypeCode range = TypeCode.Empty)
        {
            return SIMD_Sum((int*)array.GetUnsafeReadOnlyPtr(), array.Length, range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeSlice<int> array, int index, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeSlice<int> array, int index, int numEntries, TypeCode range = TypeCode.Empty)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, range);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(long* ptr, long length)
        {
Assert.IsNonNegative(length);

            long sum = 0;

            for (long i = 0; Hint.Likely(i < length); i++)
            {
                sum += ptr[i];
            }

            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeArray<long> array)
        {
            return SIMD_Sum((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeArray<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeArray<long> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeList<long> array)
        {
            return SIMD_Sum((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeList<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeList<long> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeSlice<long> array)
        {
            return SIMD_Sum((long*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeSlice<long> array, int index)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_Sum(this NativeSlice<long> array, int index, int numEntries)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Sum(float* ptr, long length, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsNonNegative(length);

            switch (floatMode)
            {
                case FloatMode.Fast:
                {
                    if (Avx.IsAvxSupported)
                    {
                        v256 acc0 = default(v256);
                        v256 acc1 = default(v256);
                        v256 acc2 = default(v256);
                        v256 acc3 = default(v256);
                        v256* ptr_v256 = (v256*)ptr;

                        while (Hint.Likely(length >= 32))
                        {
                            acc0 = Avx.mm256_add_ps(acc0, Avx.mm256_loadu_ps(ptr_v256++));
                            acc1 = Avx.mm256_add_ps(acc1, Avx.mm256_loadu_ps(ptr_v256++));
                            acc2 = Avx.mm256_add_ps(acc2, Avx.mm256_loadu_ps(ptr_v256++));
                            acc3 = Avx.mm256_add_ps(acc3, Avx.mm256_loadu_ps(ptr_v256++));

                            length -= 32;
                        }

                        v256 csum0 = Avx.mm256_add_ps(Avx.mm256_add_ps(acc0, acc1), Avx.mm256_add_ps(acc2, acc3));

                        if (Hint.Likely((int)length >= 8))
                        {
                            csum0 = Avx.mm256_add_ps(csum0, Avx.mm256_loadu_ps(ptr_v256++));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                csum0 = Avx.mm256_add_ps(csum0, Avx.mm256_loadu_ps(ptr_v256++));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    csum0 = Avx.mm256_add_ps(csum0, Avx.mm256_loadu_ps(ptr_v256++));
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

                        v128 csum1 = Sse.add_ps(Avx.mm256_castps256_ps128(csum0), Avx.mm256_extractf128_ps(csum0, 1));

                        if (Hint.Likely(length >= 4))
                        {
                            csum1 = Sse.add_ps(csum1, Sse.loadu_ps(ptr_v256));
                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }

                        v128 csum2 = Sse.add_ps(csum1, Sse.shuffle_ps(csum1, csum1, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely(length >= 2))
                        {
                            csum2 = Sse.add_ps(csum2, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        v128 csum3 = Sse.add_ss(csum2, Sse.shuffle_ps(csum2, csum2, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely(length != 0))
                        {
                            csum3 = Sse.add_ss(csum3, Sse2.cvtsi32_si128(*(int*)ptr_v256));
                        }
                        else { }


                        return csum3.Float0;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 acc0 = default(v128);
                        v128 acc1 = default(v128);
                        v128 acc2 = default(v128);
                        v128 acc3 = default(v128);
                        v128* ptr_v128 = (v128*)ptr;

                        while (Hint.Likely(length >= 16))
                        {
                            acc0 = Sse.add_ps(acc0, Sse.loadu_ps(ptr_v128++));
                            acc1 = Sse.add_ps(acc1, Sse.loadu_ps(ptr_v128++));
                            acc2 = Sse.add_ps(acc2, Sse.loadu_ps(ptr_v128++));
                            acc3 = Sse.add_ps(acc3, Sse.loadu_ps(ptr_v128++));

                            length -= 16;
                        }

                        v128 csum0 = Sse.add_ps(Sse.add_ps(acc0, acc1), Sse.add_ps(acc2, acc3));

                        if (Hint.Likely((int)length >= 4))
                        {
                            csum0 = Sse.add_ps(csum0, Sse.loadu_ps(ptr_v128++));

                            if (Hint.Likely((int)length >= 2 * 4))
                            {
                                csum0 = Sse.add_ps(csum0, Sse.loadu_ps(ptr_v128++));

                                if (Hint.Likely((int)length >= 3 * 4))
                                {
                                    csum0 = Sse.add_ps(csum0, Sse.loadu_ps(ptr_v128++));
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

                        v128 csum1 = Sse.add_ps(csum0, Sse.shuffle_ps(csum0, csum0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely(length >= 2))
                        {
                            csum1 = Sse.add_ps(csum1, Sse2.cvtsi64x_si128(*(long*)ptr_v128));
                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }

                        v128 csum2 = Sse.add_ss(csum1, Sse.shuffle_ps(csum1, csum1, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely(length != 0))
                        {
                            csum2 = Sse.add_ss(csum2, Sse2.cvtsi32_si128(*(int*)ptr_v128));
                        }
                        else { }


                        return csum2.Float0;
                    }
                    else
                    {
                        goto default;
                    }
                }
                default:
                {
                    float sum = 0;

                    for (long i = 0; Hint.Likely(i < length); i++)
                    {
                        sum += ptr[i];
                    }

                    return sum;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Sum(this NativeArray<float> array, FloatMode floatMode = FloatMode.Strict)
        {
            return SIMD_Sum((float*)array.GetUnsafeReadOnlyPtr(), array.Length, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Sum(this NativeArray<float> array, int index, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Sum(this NativeArray<float> array, int index, int numEntries, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Sum(this NativeList<float> array, FloatMode floatMode = FloatMode.Strict)
        {
            return SIMD_Sum((float*)array.GetUnsafeReadOnlyPtr(), array.Length, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Sum(this NativeList<float> array, int index, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Sum(this NativeList<float> array, int index, int numEntries, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Sum(this NativeSlice<float> array, FloatMode floatMode = FloatMode.Strict)
        {
            return SIMD_Sum((float*)array.GetUnsafeReadOnlyPtr(), array.Length, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Sum(this NativeSlice<float> array, int index, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SIMD_Sum(this NativeSlice<float> array, int index, int numEntries, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, floatMode);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Sum(double* ptr, long length, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsNonNegative(length);

            switch (floatMode)
            {
                case FloatMode.Fast:
                {
                    if (Avx.IsAvxSupported)
                    {
                        v256 acc0 = default(v256);
                        v256 acc1 = default(v256);
                        v256 acc2 = default(v256);
                        v256 acc3 = default(v256);
                        v256* ptr_v256 = (v256*)ptr;

                        while (Hint.Likely(length >= 16))
                        {
                            acc0 = Avx.mm256_add_pd(acc0, Avx.mm256_loadu_pd(ptr_v256++));
                            acc1 = Avx.mm256_add_pd(acc1, Avx.mm256_loadu_pd(ptr_v256++));
                            acc2 = Avx.mm256_add_pd(acc2, Avx.mm256_loadu_pd(ptr_v256++));
                            acc3 = Avx.mm256_add_pd(acc3, Avx.mm256_loadu_pd(ptr_v256++));

                            length -= 16;
                        }

                        v256 csum0 = Avx.mm256_add_pd(Avx.mm256_add_pd(acc0, acc1), Avx.mm256_add_pd(acc2, acc3));

                        if (Hint.Likely((int)length >= 4))
                        {
                            csum0 = Avx.mm256_add_pd(csum0, Avx.mm256_loadu_pd(ptr_v256++));

                            if (Hint.Likely((int)length >= 2 * 4))
                            {
                                csum0 = Avx.mm256_add_pd(csum0, Avx.mm256_loadu_pd(ptr_v256++));

                                if (Hint.Likely((int)length >= 3 * 4))
                                {
                                    csum0 = Avx.mm256_add_pd(csum0, Avx.mm256_loadu_pd(ptr_v256++));
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

                        v128 csum1 = Sse2.add_pd(Avx.mm256_castpd256_pd128(csum0), Avx.mm256_extractf128_pd(csum0, 1));

                        if (Hint.Likely(length >= 2))
                        {
                            csum1 = Sse2.add_pd(csum1, Sse.loadu_ps(ptr_v256));
                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        v128 csum2 = Sse2.add_sd(csum1, Sse2.shuffle_pd(csum1, csum1, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely(length != 0))
                        {
                            csum2 = Sse2.add_sd(csum2, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                        }
                        else { }


                        return csum2.Float0;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 acc0 = default(v128);
                        v128 acc1 = default(v128);
                        v128 acc2 = default(v128);
                        v128 acc3 = default(v128);
                        v128* ptr_v128 = (v128*)ptr;

                        while (Hint.Likely(length >= 8))
                        {
                            acc0 = Sse2.add_pd(acc0, Sse.loadu_ps(ptr_v128++));
                            acc1 = Sse2.add_pd(acc1, Sse.loadu_ps(ptr_v128++));
                            acc2 = Sse2.add_pd(acc2, Sse.loadu_ps(ptr_v128++));
                            acc3 = Sse2.add_pd(acc3, Sse.loadu_ps(ptr_v128++));

                            length -= 8;
                        }

                        v128 csum0 = Sse2.add_pd(Sse2.add_pd(acc0, acc1), Sse2.add_pd(acc2, acc3));

                        if (Hint.Likely((int)length >= 2))
                        {
                            csum0 = Sse2.add_pd(csum0, Sse.loadu_ps(ptr_v128++));

                            if (Hint.Likely((int)length >= 2 * 2))
                            {
                                csum0 = Sse2.add_pd(csum0, Sse.loadu_ps(ptr_v128++));

                                if (Hint.Likely((int)length >= 3 * 2))
                                {
                                    csum0 = Sse2.add_pd(csum0, Sse.loadu_ps(ptr_v128++));
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

                        v128 csum1 = Sse2.add_sd(csum0, Sse2.shuffle_pd(csum0, csum0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely(length != 0))
                        {
                            csum1 = Sse2.add_sd(csum1, Sse2.cvtsi64x_si128(*(long*)ptr_v128));
                        }
                        else { }


                        return csum1.Float0;
                    }
                    else
                    {
                        goto default;
                    }
                }
                default:
                {
                    double sum = 0;

                    for (long i = 0; Hint.Likely(i < length); i++)
                    {
                        sum += ptr[i];
                    }

                    return sum;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Sum(this NativeArray<double> array, FloatMode floatMode = FloatMode.Strict)
        {
            return SIMD_Sum((double*)array.GetUnsafeReadOnlyPtr(), array.Length, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Sum(this NativeArray<double> array, int index, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Sum(this NativeArray<double> array, int index, int numEntries, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Sum(this NativeList<double> array, FloatMode floatMode = FloatMode.Strict)
        {
            return SIMD_Sum((double*)array.GetUnsafeReadOnlyPtr(), array.Length, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Sum(this NativeList<double> array, int index, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Sum(this NativeList<double> array, int index, int numEntries, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Sum(this NativeSlice<double> array, FloatMode floatMode = FloatMode.Strict)
        {
            return SIMD_Sum((double*)array.GetUnsafeReadOnlyPtr(), array.Length, floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Sum(this NativeSlice<double> array, int index, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Sum((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), floatMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SIMD_Sum(this NativeSlice<double> array, int index, int numEntries, FloatMode floatMode = FloatMode.Strict)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return SIMD_Sum((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, floatMode);
        }
    }
}