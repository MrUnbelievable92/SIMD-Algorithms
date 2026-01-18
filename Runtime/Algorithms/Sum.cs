using System;
using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst;
using DevTools;
using MaxMath;

using static Unity.Burst.Intrinsics.X86;
using static MaxMath.maxmath;
using MaxMath.Intrinsics;

namespace SIMDAlgorithms
{
    unsafe public static partial class Algorithms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Sum(byte* ptr, long length, TypeCode range = TypeCode.Empty)
        {
            static bool ConstChecks256(void* startPtr, ulong length, out v256 result)
            {
                if (Avx2.IsAvx2Supported)
                {
                    if (constexpr.IS_TRUE(length == 128))
                    {
                        v256 sad0 = Avx2.mm256_sad_epu8(Avx.mm256_setzero_si256(), ((v256*)startPtr)[0]);
                        v256 sad1 = Avx2.mm256_sad_epu8(Avx.mm256_setzero_si256(), ((v256*)startPtr)[1]);
                        v256 sad2 = Avx2.mm256_sad_epu8(Avx.mm256_setzero_si256(), ((v256*)startPtr)[2]);
                        v256 sad3 = Avx2.mm256_sad_epu8(Avx.mm256_setzero_si256(), ((v256*)startPtr)[3]);

                        v256 sum0 = Avx2.mm256_add_epi64(sad0, sad1);
                        v256 sum1 = Avx2.mm256_add_epi64(sad2, sad3);
                        v256 sum2 = Avx2.mm256_add_epi64(sum1, sum0);

                        result = sum2;

                        return true;
                    }

                    if (constexpr.IS_TRUE(length == 96))
                    {
                        result = Avx2.mm256_sad_epu8(Avx.mm256_setzero_si256(), ((v256*)startPtr)[0]);
                        result = Avx2.mm256_add_epi64(result, Avx2.mm256_sad_epu8(Avx.mm256_setzero_si256(), ((v256*)startPtr)[1]));
                        result = Avx2.mm256_add_epi64(result, Avx2.mm256_sad_epu8(Avx.mm256_setzero_si256(), ((v256*)startPtr)[2]));

                        return true;
                    }

                    if (constexpr.IS_TRUE(length == 64))
                    {
                        result = Avx2.mm256_sad_epu8(Avx.mm256_setzero_si256(), ((v256*)startPtr)[0]);
                        result = Avx2.mm256_add_epi64(result, Avx2.mm256_sad_epu8(Avx.mm256_setzero_si256(), ((v256*)startPtr)[1]));

                        return true;
                    }

                    if (constexpr.IS_TRUE(length == 32))
                    {
                        result = Avx2.mm256_sad_epu8(Avx.mm256_setzero_si256(), ((v256*)startPtr)[0]);

                        return true;
                    }
                }


                result = default;
                return false;
            }
            static bool ConstChecks128(void* startPtr, ulong length, out v128 result)
            {
                if (BurstArchitecture.IsSIMDSupported)
                {
                    if (constexpr.IS_TRUE(length == 16))
                    {
                        result = Xse.sad_epu8(Xse.setzero_si128(), Xse.loadu_si128(startPtr));
                        result = Xse.add_epi64(result, Xse.shuffle_epi32(result, Sse.SHUFFLE(0, 0, 3, 2)));

                        return true;
                    }

                    if (constexpr.IS_TRUE(length == 8))
                    {
                        result = Xse.sad_epu8(Xse.setzero_si128(), Xse.cvtsi64x_si128(*(long*)startPtr));

                        return true;
                    }

                    if (constexpr.IS_TRUE(length == 4))
                    {
                        result = Xse.sad_epu8(Xse.setzero_si128(), Xse.cvtsi32_si128(*(int*)startPtr));

                        return true;
                    }
                }

                result = default;
                return false;
            }
            static bool ConstChecksScalar(void* startPtr, ulong length, out ulong scalarResult)
            {
                if (BurstArchitecture.IsSIMDSupported)
                {
                    if (constexpr.IS_TRUE(length == 3))
                    {
                        scalarResult = ((byte*)startPtr)[0];
                        scalarResult += ((byte*)startPtr)[1];
                        scalarResult += ((byte*)startPtr)[2];

                        return true;
                    }
                    if (constexpr.IS_TRUE(length == 2))
                    {
                        scalarResult = ((byte*)startPtr)[0];
                        scalarResult += ((byte*)startPtr)[1];

                        return true;
                    }
                    if (constexpr.IS_TRUE(length == 1))
                    {
                        scalarResult = ((byte*)startPtr)[0];

                        return true;
                    }
                    if (constexpr.IS_TRUE(length == 0))
                    {
                        scalarResult = 0;
                        return true;
                    }
                }

                scalarResult = 0;
                return false;
            }


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
                        if (ConstChecks256(   ptr, (ulong)length, out v256 result256)) return maxmath.csum((ulong4)result256);
                        if (ConstChecks128(   ptr, (ulong)length, out v128 result128)) return maxmath.csum((ulong2)result128);
                        if (ConstChecksScalar(ptr, (ulong)length, out ulong result8))  return result8;

                        v256 ZERO= Avx.mm256_setzero_si256();
                        v256* address = (v256*)ptr;
                        byte* endPtr = ptr + (length - 1);

                        v256 acc0= Avx.mm256_setzero_si256();
                        v256 acc1= Avx.mm256_setzero_si256();
                        v256 acc2= Avx.mm256_setzero_si256();
                        v256 acc3= Avx.mm256_setzero_si256();

                        while (Hint.Likely((ulong)endPtr - (ulong)address >= 128))
                        {
                            v256 sad0 = Avx2.mm256_sad_epu8(ZERO, Avx.mm256_loadu_si256(address++));
                            v256 sad1 = Avx2.mm256_sad_epu8(ZERO, Avx.mm256_loadu_si256(address++));
                            v256 sad2 = Avx2.mm256_sad_epu8(ZERO, Avx.mm256_loadu_si256(address++));
                            v256 sad3 = Avx2.mm256_sad_epu8(ZERO, Avx.mm256_loadu_si256(address++));

                            acc0 = Avx2.mm256_add_epi64(acc0, sad0);
                            acc1 = Avx2.mm256_add_epi64(acc1, sad1);
                            acc2 = Avx2.mm256_add_epi64(acc2, sad2);
                            acc3 = Avx2.mm256_add_epi64(acc3, sad3);
                        }

                        length = (long)((ulong)length % 128);
                        v256 add0 = Avx2.mm256_add_epi64(acc0, acc1);
                        v256 add1 = Avx2.mm256_add_epi64(acc2, acc3);
                        v256 add2 = Avx2.mm256_add_epi64(add0, add1);

                        //if (ConstChecks256(ptr, (ulong)length, out result256))
                        //{
                        //    result256 = Avx2.mm256_add_epi64(add2, result256);
                        //    return maxmath.csum((ulong4)result256);
                        //}
                        //if (ConstChecks128(ptr, (ulong)length, out result128))
                        //{
                        //    v128 csum128 = Xse.add_epi64(Avx.mm256_castsi256_si128(add2), Avx2.mm256_extracti128_si256(add2, 1));
                        //    result128 = Xse.add_epi64(result128, csum128);
                        //    return maxmath.csum((ulong2)result128);
                        //}
                        //if (ConstChecksScalar(ptr, (ulong)length, out result8))
                        //{
                        //    v128 csum128 = Xse.add_epi64(Avx.mm256_castsi256_si128(add2), Avx2.mm256_extracti128_si256(add2, 1));
                        //    result128 = Xse.add_epi64(result128, csum128);
                        //    return result8 + maxmath.csum((ulong2)result128);
                        //}

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

                        v128 csum0 = Xse.add_epi64(Avx.mm256_castsi256_si128(add2), Avx2.mm256_extracti128_si256(add2, 1));


                        //if (ConstChecks128(ptr, (ulong)length, out result128))
                        //{
                        //    result128 = Xse.add_epi64(result128, csum0);
                        //    return maxmath.csum((ulong2)result128);
                        //}
                        //if (ConstChecksScalar(ptr, (ulong)length, out result8))
                        //{
                        //    result128 = Xse.add_epi64(result128, csum0);
                        //    return result8 + maxmath.csum((ulong2)result128);
                        //}

                        if (Hint.Likely((int)length >= 16))
                        {
                            csum0 = Xse.add_epi64(csum0, Xse.sad_epu8(Avx.mm256_castsi256_si128(ZERO), Xse.loadu_si128(address)));

                            address = (v256*)((v128*)address + 1);
                            length -= 16;
                        }

                        //if (ConstChecks128(ptr, (ulong)length, out result128))
                        //{
                        //    result128 = Xse.add_epi64(result128, csum0);
                        //    return maxmath.csum((ulong2)result128);
                        //}
                        //if (ConstChecksScalar(ptr, (ulong)length, out result8))
                        //{
                        //    result128 = Xse.add_epi64(result128, csum0);
                        //    return result8 + maxmath.csum((ulong2)result128);
                        //}
                        if (Hint.Likely((int)length >= 8))
                        {
                            csum0 = Xse.add_epi64(csum0, Xse.sad_epu8(Avx.mm256_castsi256_si128(ZERO), Xse.cvtsi64x_si128(*(long*)address)));
                            length -= 8;
                            address = (v256*)((long*)address + 1);
                        }

                        //if (ConstChecks128(ptr, (ulong)length, out result128))
                        //{
                        //    result128 = Xse.add_epi64(result128, csum0);
                        //    return maxmath.csum((ulong2)result128);
                        //}
                        //if (ConstChecksScalar(ptr, (ulong)length, out result8))
                        //{
                        //    result128 = Xse.add_epi64(result128, csum0);
                        //    return result8 + maxmath.csum((ulong2)result128);
                        //}
                        if (Hint.Likely((int)length >= 4))
                        {
                            csum0 = Xse.add_epi64(csum0, Xse.sad_epu8(Avx.mm256_castsi256_si128(ZERO), Xse.cvtsi32_si128(*(int*)address)));
                            length -= 4;
                            address = (v256*)((int*)address + 1);
                        }

                        v128 csum = Xse.add_epi64(csum0, Xse.shuffle_epi32(csum0, Sse.SHUFFLE(0, 0, 3, 2)));
                        ulong sum = csum.ULong0;


                        //if (ConstChecksScalar(ptr, (ulong)length, out result8))
                        //{
                        //    return sum + result8;
                        //}

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
                            }
                        }

                        return sum;
                    }
                    else if (BurstArchitecture.IsSIMDSupported)
                    {
                        v128 ZERO = Xse.setzero_si128();
                        v128* address = (v128*)ptr;

                        v128 acc0 = Xse.setzero_si128();
                        v128 acc1 = Xse.setzero_si128();
                        v128 acc2 = Xse.setzero_si128();
                        v128 acc3 = Xse.setzero_si128();

                        while (Hint.Likely(length >= 64))
                        {
                            v128 sad0 = Xse.sad_epu8(ZERO, Xse.loadu_si128(address++));
                            v128 sad1 = Xse.sad_epu8(ZERO, Xse.loadu_si128(address++));
                            v128 sad2 = Xse.sad_epu8(ZERO, Xse.loadu_si128(address++));
                            v128 sad3 = Xse.sad_epu8(ZERO, Xse.loadu_si128(address++));

                            acc0 = Xse.add_epi64(acc0, sad0);
                            acc1 = Xse.add_epi64(acc1, sad1);
                            acc2 = Xse.add_epi64(acc2, sad2);
                            acc3 = Xse.add_epi64(acc3, sad3);

                            length -= 64;
                        }

                        acc0 = Xse.add_epi64(Xse.add_epi64(acc0, acc1), Xse.add_epi64(acc2, acc3));

                        if (Hint.Likely((int)length >= 16))
                        {
                            acc0 = Xse.add_epi64(acc0, Xse.sad_epu8(ZERO, Xse.loadu_si128(address++)));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                acc0 = Xse.add_epi64(acc0, Xse.sad_epu8(ZERO, Xse.loadu_si128(address++)));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    acc0 = Xse.add_epi64(acc0, Xse.sad_epu8(ZERO, Xse.loadu_si128(address++)));
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

                        if (Hint.Likely((int)length >= 8))
                        {
                            acc0 = Xse.add_epi64(acc0, Xse.sad_epu8(ZERO, Xse.cvtsi64x_si128(*(long*)address)));
                            length -= 8;
                            address = (v128*)((long*)address + 1);
                        }

                        if (Hint.Likely((int)length >= 4))
                        {
                            acc0 = Xse.add_epi64(acc0, Xse.sad_epu8(ZERO, Xse.cvtsi32_si128(*(int*)address)));
                            length -= 4;
                            address = (v128*)((int*)address + 1);
                        }

                        v128 csum = Xse.add_epi64(acc0, Xse.shuffle_epi32(acc0, Sse.SHUFFLE(0, 0, 3, 2)));
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
                            }
                        }

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
                        v256 longs = Avx.mm256_setzero_si256();

                        while (Hint.Likely(length >= 32))
                        {
                            int iterations = 0;
                            v256 acc0 = Avx.mm256_setzero_si256();
                            v256 acc1 = Avx.mm256_setzero_si256();
                            v256 acc2 = Avx.mm256_setzero_si256();
                            v256 acc3 = Avx.mm256_setzero_si256();

                            while (Hint.Likely(iterations < uint.MaxValue / ushort.MaxValue
                                & length >= 32))
                            {
                                v256 load0 = Xse.mm256_cvt2x2epu16_epi32(Avx.mm256_loadu_si256(address), out v256 load1);
                                address += 2;
                                v256 load2 = Xse.mm256_cvt2x2epu16_epi32(Avx.mm256_loadu_si256(address), out v256 load3);
                                address += 2;

                                acc0 = Avx2.mm256_add_epi32(acc0, load0);
                                acc1 = Avx2.mm256_add_epi32(acc1, load1);
                                acc2 = Avx2.mm256_add_epi32(acc2, load2);
                                acc3 = Avx2.mm256_add_epi32(acc3, load3);

                                length -= 32;
                                iterations++;
                            }

                            v256 cast = Xse.mm256_cvt2x2epu32_epi64(acc0, out acc0);
                            acc0 = Avx2.mm256_add_epi64(cast, acc0);
                            cast = Xse.mm256_cvt2x2epu32_epi64(acc1, out acc1);
                            acc1 = Avx2.mm256_add_epi64(cast, acc1);
                            cast = Xse.mm256_cvt2x2epu32_epi64(acc2, out acc2);
                            acc2 = Avx2.mm256_add_epi64(cast, acc2);
                            cast = Xse.mm256_cvt2x2epu32_epi64(acc3, out acc3);
                            acc3 = Avx2.mm256_add_epi64(cast, acc3);

                            longs = Avx2.mm256_add_epi64(longs, Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(acc0, acc1), Avx2.mm256_add_epi64(acc2, acc3)));
                        }

                        v128 csum_0 = Xse.add_epi64(Avx.mm256_castsi256_si128(longs), Avx2.mm256_extracti128_si256(longs, 1));
                        v128 csum_1 = Xse.vsum_epi64(csum_0);
                        ulong sum = csum_1.ULong0;

                        v128 intSum;

                        if (Hint.Likely((int)length >= 8))
                        {
                            v128 load = Xse.loadu_si128(address++);
                            v128 lo = Xse.cvt2x2epu16_epi32(load, out v128 hi);
                            intSum = Xse.add_epi32(lo, hi);

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                load = Xse.loadu_si128(address++);
                                lo = Xse.cvt2x2epu16_epi32(load, out hi);
                                intSum = Xse.add_epi32(intSum, Xse.add_epi32(lo, hi));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    load = Xse.loadu_si128(address++);
                                    lo = Xse.cvt2x2epu16_epi32(load, out hi);
                                    intSum = Xse.add_epi32(intSum, Xse.add_epi32(lo, hi));

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
                        else
                        {
                            intSum = Xse.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 4))
                        {
                            intSum = Xse.add_epi32(intSum, Xse.cvtepu16_epi32(Xse.cvtsi64x_si128(*(long*)address)));
                            length -= 4;
                            address = (v128*)((long*)address + 1);
                        }

                        intSum = Xse.add_epi32(intSum, Xse.shuffle_epi32(intSum, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            intSum = Xse.add_epi32(intSum, Xse.cvtepu16_epi32(Xse.cvtsi32_si128(*(int*)address)));
                            length -= 2;
                            address = (v128*)((int*)address + 1);
                        }

                        intSum = Xse.add_epi32(intSum, Xse.shuffle_epi32(intSum, Sse.SHUFFLE(0, 0, 0, 1)));
                        sum += intSum.UInt0;

                        if (Hint.Likely(length != 0))
                        {
                            sum += *(ushort*)address;
                        }

                        return sum;
                    }
                    else if (BurstArchitecture.IsSIMDSupported)
                    {
                        v128* address = (v128*)ptr;
                        v128 longs = Xse.setzero_si128();
                        v128 ZERO = Xse.setzero_si128();

                        while (Hint.Likely(length >= 16))
                        {
                            int iterations = 0;
                            v128 acc0 = Xse.setzero_si128();
                            v128 acc1 = Xse.setzero_si128();
                            v128 acc2 = Xse.setzero_si128();
                            v128 acc3 = Xse.setzero_si128();

                            while (Hint.Likely(iterations < uint.MaxValue / ushort.MaxValue
                                 & length >= 16))
                            {
                                v128 load0 = Xse.cvt2x2epu16_epi32(Xse.loadu_si128(address++), out v128 load1);
                                v128 load2 = Xse.cvt2x2epu16_epi32(Xse.loadu_si128(address++), out v128 load3);

                                acc0 = Xse.add_epi32(acc0, load0);
                                acc1 = Xse.add_epi32(acc1, load1);
                                acc2 = Xse.add_epi32(acc2, load2);
                                acc3 = Xse.add_epi32(acc3, load3);

                                length -= 16;
                                iterations++;
                            }

                            v128 cast = Xse.cvt2x2epu32_epi64(acc0, out acc0);
                            acc0 = Xse.add_epi64(cast, acc0);
                            cast = Xse.cvt2x2epu32_epi64(acc1, out acc1);
                            acc1 = Xse.add_epi64(cast, acc1);
                            cast = Xse.cvt2x2epu32_epi64(acc2, out acc2);
                            acc2 = Xse.add_epi64(cast, acc2);
                            cast = Xse.cvt2x2epu32_epi64(acc3, out acc3);
                            acc3 = Xse.add_epi64(cast, acc3);

                            longs = Xse.add_epi64(longs, Xse.add_epi64(Xse.add_epi64(acc0, acc1), Xse.add_epi64(acc2, acc3)));
                        }

                        v128 csum = Xse.vsum_epi64(longs);
                        ulong sum = csum.ULong0;

                        v128 intSum;

                        if (Hint.Likely((int)length >= 8))
                        {
                            v128 load = Xse.loadu_si128(address++);
                            v128 lo = Xse.cvt2x2epu16_epi32(load, out v128 hi);
                            intSum = Xse.add_epi32(lo, hi);

                            length -= 8;
                        }
                        else
                        {
                            intSum = Xse.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 4))
                        {
                            intSum = Xse.add_epi32(intSum, Xse.cvtepu16_epi32(Xse.cvtsi64x_si128(*(long*)address)));

                            length -= 4;
                            address = (v128*)((long*)address + 1);
                        }

                        intSum = Xse.add_epi32(intSum, Xse.shuffle_epi32(intSum, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            intSum = Xse.add_epi32(intSum, Xse.cvtepu16_epi32(Xse.cvtsi32_si128(*(int*)address)));

                            length -= 2;
                            address = (v128*)((int*)address + 1);
                        }

                        intSum = Xse.add_epi32(intSum, Xse.shuffle_epi32(intSum, Sse.SHUFFLE(0, 0, 0, 1)));
                        sum  += intSum.UInt0;

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
                        v256 ints= Avx.mm256_setzero_si256();

                        while (Hint.Likely(length >= 64))
                        {
                            int iterations = 0;
                            v256 acc0= Avx.mm256_setzero_si256();
                            v256 acc1= Avx.mm256_setzero_si256();
                            v256 acc2= Avx.mm256_setzero_si256();
                            v256 acc3= Avx.mm256_setzero_si256();

                            while (Hint.Likely(iterations < short.MinValue / sbyte.MinValue
                                & length >= 64))
                            {
                                v256 load0 = Xse.mm256_cvt2x2epi8_epi16(Avx.mm256_loadu_si256(address), out v256 load1);
                                address += 2;
                                v256 load2 = Xse.mm256_cvt2x2epi8_epi16(Avx.mm256_loadu_si256(address), out v256 load3);
                                address += 2;

                                acc0 = Avx2.mm256_add_epi16(acc0, load0);
                                acc1 = Avx2.mm256_add_epi16(acc1, load1);
                                acc2 = Avx2.mm256_add_epi16(acc2, load2);
                                acc3 = Avx2.mm256_add_epi16(acc3, load3);

                                length -= 64;
                                iterations++;
                            }

                            v256 ints0 = Xse.mm256_cvt2x2epi16_epi32(acc0, out v256 ints1);
                            v256 ints2 = Xse.mm256_cvt2x2epi16_epi32(acc1, out v256 ints3);
                            v256 ints4 = Xse.mm256_cvt2x2epi16_epi32(acc2, out v256 ints5);
                            v256 ints6 = Xse.mm256_cvt2x2epi16_epi32(acc3, out v256 ints7);
                            ints0 = Avx2.mm256_add_epi32(ints0, ints1);
                            ints2 = Avx2.mm256_add_epi32(ints2, ints3);
                            ints4 = Avx2.mm256_add_epi32(ints4, ints5);
                            ints6 = Avx2.mm256_add_epi32(ints6, ints7);
                            ints = Avx2.mm256_add_epi32(ints, Avx2.mm256_add_epi32(Avx2.mm256_add_epi32(ints0, ints2), Avx2.mm256_add_epi32(ints4, ints6)));
                        }

                        v128 csum_0 = Xse.add_epi32(Avx.mm256_castsi256_si128(ints), Avx2.mm256_extracti128_si256(ints, 1));
                        v128 csum_1 = Xse.vsum_epi32(csum_0);
                        int sum = csum_1.SInt0;

                        v128 shortSum;

                        if (Hint.Likely((int)length >= 16))
                        {
                            v128 load = Xse.loadu_si128(address++);
                            v128 lo = Xse.cvt2x2epi8_epi16(load, out v128 hi);
                            shortSum = Xse.add_epi16(lo, hi);

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                load = Xse.loadu_si128(address++);
                                lo = Xse.cvt2x2epi8_epi16(load, out hi);
                                shortSum = Xse.add_epi16(shortSum, Xse.add_epi16(lo, hi));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    load = Xse.loadu_si128(address++);
                                    lo = Xse.cvt2x2epi8_epi16(load, out hi);
                                    shortSum = Xse.add_epi16(shortSum, Xse.add_epi16(lo, hi));

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
                        else
                        {
                            shortSum = Xse.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            shortSum = Xse.add_epi16(shortSum, Xse.cvtepi8_epi16(Xse.cvtsi64x_si128(*(long*)address)));

                            address = (v128*)((long*)address + 1);
                            length -= 8;
                        }

                        shortSum = Xse.add_epi16(shortSum, Xse.shuffle_epi32(shortSum, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            shortSum = Xse.add_epi16(shortSum, Xse.cvtepi8_epi16(Xse.cvtsi32_si128(*(int*)address)));

                            address = (v128*)((int*)address + 1);
                            length -= 4;
                        }

                        shortSum = Xse.add_epi16(shortSum, Xse.shufflelo_epi16(shortSum, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            shortSum = Xse.add_epi16(shortSum, Xse.cvtepi8_epi16(Xse.cvtsi32_si128(*(ushort*)address)));

                            address = (v128*)((ushort*)address + 1);
                            length -= 2;
                        }

                        shortSum = Xse.add_epi16(shortSum, Xse.shufflelo_epi16(shortSum, Sse.SHUFFLE(0, 0, 0, 1)));
                        sum += shortSum.SShort0;

                        if (Hint.Likely(length != 0))
                        {
                            sum += *(sbyte*)address;
                        }

                        return sum;
                    }
                    else if (BurstArchitecture.IsSIMDSupported)
                    {
                        v128* address = (v128*)ptr;
                        v128 ints = Xse.setzero_si128();
                        v128 ZERO = Xse.setzero_si128();


                        while (Hint.Likely(length >= 32))
                        {
                            int iterations_short = 0;
                            v128 shorts0 = Xse.setzero_si128();
                            v128 shorts1 = Xse.setzero_si128();
                            v128 shorts2 = Xse.setzero_si128();
                            v128 shorts3 = Xse.setzero_si128();

                            while (Hint.Likely(iterations_short < short.MinValue / sbyte.MinValue
                                 & length >= 32))
                            {
                                v128 load0 = Xse.cvt2x2epi8_epi16(Xse.loadu_si128(address++), out v128 load1);
                                v128 load2 = Xse.cvt2x2epi8_epi16(Xse.loadu_si128(address++), out v128 load3);

                                shorts0 = Xse.add_epi16(shorts0, load0);
                                shorts1 = Xse.add_epi16(shorts1, load1);
                                shorts2 = Xse.add_epi16(shorts2, load2);
                                shorts3 = Xse.add_epi16(shorts3, load3);

                                iterations_short++;
                                length -= 32;
                            }

                            v128 ints0 = Xse.cvt2x2epi16_epi32(shorts0, out v128 ints1);
                            v128 ints2 = Xse.cvt2x2epi16_epi32(shorts1, out v128 ints3);
                            v128 ints4 = Xse.cvt2x2epi16_epi32(shorts2, out v128 ints5);
                            v128 ints6 = Xse.cvt2x2epi16_epi32(shorts3, out v128 ints7);
                            ints0 = Xse.add_epi32(ints0, ints1);
                            ints2 = Xse.add_epi32(ints2, ints3);
                            ints4 = Xse.add_epi32(ints4, ints5);
                            ints6 = Xse.add_epi32(ints6, ints7);
                            ints = Xse.add_epi32(ints, Xse.add_epi32(Xse.add_epi32(ints0, ints2), Xse.add_epi32(ints4, ints6)));
                        }

                        v128 csum = Xse.vsum_epi32(ints);
                        int sum = csum.SInt0;
                        v128 shortSum;

                        if (Hint.Likely((int)length >= 16))
                        {
                            v128 load = Xse.loadu_si128(address++);
                            v128 lo = Xse.cvt2x2epi8_epi16(load, out v128 hi);
                            shortSum = Xse.add_epi16(lo, hi);

                            length -= 16;
                        }
                        else
                        {
                            shortSum = Xse.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            v128 load = Xse.cvtsi64x_si128(*(long*)address);
                            shortSum = Xse.add_epi16(shortSum, Xse.cvtepi8_epi16(load));

                            address = (v128*)((long*)address + 1);
                            length -= 8;
                        }

                        shortSum = Xse.add_epi16(shortSum, Xse.shuffle_epi32(shortSum, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            v128 load = Xse.cvtsi32_si128(*(int*)address);
                            shortSum = Xse.add_epi16(shortSum, Xse.cvtepi8_epi16(load));

                            address = (v128*)((int*)address + 1);
                            length -= 4;
                        }

                        shortSum = Xse.add_epi16(shortSum, Xse.shufflelo_epi16(shortSum, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            v128 load = Xse.cvtsi32_si128(*(ushort*)address);
                            shortSum = Xse.add_epi16(shortSum, Xse.cvtepi8_epi16(load));

                            address = (v128*)((ushort*)address + 1);
                            length -= 2;
                        }

                        shortSum = Xse.add_epi16(shortSum, Xse.shufflelo_epi16(shortSum, Sse.SHUFFLE(0, 0, 0, 1)));
                        sum += shortSum.SShort0;

                        if (Hint.Likely(length != 0))
                        {
                            sum += *(sbyte*)address;
                        }

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
                        v256 longs = Avx.mm256_setzero_si256();

                        while (Hint.Likely(length >= 64))
                        {
                            int iterations_short = 0;
                            v256 shorts0 = Avx.mm256_setzero_si256();
                            v256 shorts1 = Avx.mm256_setzero_si256();
                            v256 shorts2 = Avx.mm256_setzero_si256();
                            v256 shorts3 = Avx.mm256_setzero_si256();

                            while (Hint.Likely(iterations_short < short.MinValue / sbyte.MinValue
                                 & length >= 64))
                            {
                                v256 load0 = Xse.mm256_cvt2x2epi8_epi16(Avx.mm256_loadu_si256(address), out v256 load1);
                                address += 2;
                                v256 load2 = Xse.mm256_cvt2x2epi8_epi16(Avx.mm256_loadu_si256(address), out v256 load3);
                                address += 2;

                                shorts0 = Avx2.mm256_add_epi16(shorts0, load0);
                                shorts1 = Avx2.mm256_add_epi16(shorts1, load1);
                                shorts2 = Avx2.mm256_add_epi16(shorts2, load2);
                                shorts3 = Avx2.mm256_add_epi16(shorts3, load3);

                                iterations_short++;
                                length -= 64;
                            }

                            Xse.mm256_cvt4x4epi16_epi64(shorts0, out v256 longs0,  out v256 longs1,  out v256 longs2,  out v256 longs3);
                            Xse.mm256_cvt4x4epi16_epi64(shorts1, out v256 longs4,  out v256 longs5,  out v256 longs6,  out v256 longs7);
                            Xse.mm256_cvt4x4epi16_epi64(shorts2, out v256 longs8,  out v256 longs9,  out v256 longs10, out v256 longs11);
                            Xse.mm256_cvt4x4epi16_epi64(shorts3, out v256 longs12, out v256 longs13, out v256 longs14, out v256 longs15);
                            longs0  = Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(longs0,  longs1),  Avx2.mm256_add_epi64(longs2,  longs3));
                            longs4  = Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(longs4,  longs5),  Avx2.mm256_add_epi64(longs6,  longs7));
                            longs8  = Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(longs8,  longs9),  Avx2.mm256_add_epi64(longs10, longs11));
                            longs12 = Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(longs12, longs13), Avx2.mm256_add_epi64(longs14, longs15));
                            longs = Avx2.mm256_add_epi64(longs, Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(longs0, longs4), Avx2.mm256_add_epi64(longs8, longs12)));
                        }

                        v128 csum_0 = Xse.add_epi64(Avx.mm256_castsi256_si128(longs), Avx2.mm256_extracti128_si256(longs, 1));
                        v128 csum_1 = Xse.vsum_epi64(csum_0);
                        long sum = csum_1.SLong0;

                        v128 shortSum;

                        if (Hint.Likely((int)length >= 16))
                        {
                            v128 load = Xse.loadu_si128(address++);
                            v128 lo = Xse.cvt2x2epi8_epi16(load, out v128 hi);
                            shortSum = Xse.add_epi16(lo, hi);

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                load = Xse.loadu_si128(address++);
                                lo = Xse.cvt2x2epi8_epi16(load, out hi);
                                shortSum = Xse.add_epi16(shortSum, Xse.add_epi16(lo, hi));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    load = Xse.loadu_si128(address++);
                                    lo = Xse.cvt2x2epi8_epi16(load, out hi);
                                    shortSum = Xse.add_epi16(shortSum, Xse.add_epi16(lo, hi));

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
                        else
                        {
                            shortSum = Xse.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            shortSum = Xse.add_epi16(shortSum, Xse.cvtepi8_epi16(Xse.cvtsi64x_si128(*(long*)address)));

                            address = (v128*)((long*)address + 1);
                            length -= 8;
                        }

                        shortSum = Xse.add_epi16(shortSum, Xse.shuffle_epi32(shortSum, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            shortSum = Xse.add_epi16(shortSum, Xse.cvtepi8_epi16(Xse.cvtsi32_si128(*(int*)address)));

                            address = (v128*)((int*)address + 1);
                            length -= 4;
                        }

                        shortSum = Xse.add_epi16(shortSum, Xse.shufflelo_epi16(shortSum, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            shortSum = Xse.add_epi16(shortSum, Xse.cvtepi8_epi16(Xse.cvtsi32_si128(*(ushort*)address)));

                            address = (v128*)((ushort*)address + 1);
                            length -= 2;
                        }

                        shortSum = Xse.add_epi16(shortSum, Xse.shufflelo_epi16(shortSum, Sse.SHUFFLE(0, 0, 0, 1)));
                        sum += shortSum.SShort0;


                        if (Hint.Likely(length != 0))
                        {
                            sum += *(sbyte*)address;
                        }

                        return sum;
                    }
                    else if (BurstArchitecture.IsSIMDSupported)
                    {
                        v128* address = (v128*)ptr;
                        v128 longs = Xse.setzero_si128();

                        while (Hint.Likely(length >= 32))
                        {
                            int iterations_short = 0;
                            v128 shorts0 = Xse.setzero_si128();
                            v128 shorts1 = Xse.setzero_si128();
                            v128 shorts2 = Xse.setzero_si128();
                            v128 shorts3 = Xse.setzero_si128();

                            while (Hint.Likely(iterations_short < short.MinValue / sbyte.MinValue
                                 & length >= 32))
                            {
                                v128 load0 = Xse.cvt2x2epi8_epi16(Xse.loadu_si128(address++), out v128 load1);
                                v128 load2 = Xse.cvt2x2epi8_epi16(Xse.loadu_si128(address++), out v128 load3);

                                shorts0 = Xse.add_epi16(shorts0, load0);
                                shorts1 = Xse.add_epi16(shorts1, load1);
                                shorts2 = Xse.add_epi16(shorts2, load2);
                                shorts3 = Xse.add_epi16(shorts3, load3);

                                iterations_short++;
                                length -= 32;
                            }

                            Xse.cvt4x4epi16_epi64(shorts0, out v128 longs0,  out v128 longs1,  out v128 longs2,  out v128 longs3);
                            Xse.cvt4x4epi16_epi64(shorts1, out v128 longs4,  out v128 longs5,  out v128 longs6,  out v128 longs7);
                            Xse.cvt4x4epi16_epi64(shorts2, out v128 longs8,  out v128 longs9,  out v128 longs10, out v128 longs11);
                            Xse.cvt4x4epi16_epi64(shorts3, out v128 longs12, out v128 longs13, out v128 longs14, out v128 longs15);
                            longs0  = Xse.add_epi64(Xse.add_epi64(longs0,  longs1),  Xse.add_epi64(longs2,  longs3));
                            longs4  = Xse.add_epi64(Xse.add_epi64(longs4,  longs5),  Xse.add_epi64(longs6,  longs7));
                            longs8  = Xse.add_epi64(Xse.add_epi64(longs8,  longs9),  Xse.add_epi64(longs10, longs11));
                            longs12 = Xse.add_epi64(Xse.add_epi64(longs12, longs13), Xse.add_epi64(longs14, longs15));
                            longs = Xse.add_epi64(longs, Xse.add_epi64(Xse.add_epi64(longs0, longs4), Xse.add_epi64(longs8, longs12)));
                        }

                        v128 csum_0 = Xse.vsum_epi64(longs);
                        long sum = csum_0.SLong0;
                        v128 shortSum;

                        if (Hint.Likely((int)length >= 16))
                        {
                            v128 load = Xse.loadu_si128(address++);

                            v128 lo = Xse.cvt2x2epi8_epi16(load, out v128 hi);
                            shortSum = Xse.add_epi16(lo, hi);

                            length -= 16;
                        }
                        else
                        {
                            shortSum = Xse.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            v128 load = Xse.cvtsi64x_si128(*(long*)address);
                            shortSum = Xse.add_epi16(shortSum, Xse.cvtepi8_epi16(load));

                            length -= 8;
                            address = (v128*)((long*)address + 1);
                        }

                        shortSum = Xse.add_epi16(shortSum, Xse.shuffle_epi32(shortSum, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            v128 load = Xse.cvtsi32_si128(*(int*)address);
                            shortSum = Xse.add_epi16(shortSum, Xse.cvtepi8_epi16(load));

                            length -= 4;
                            address = (v128*)((int*)address + 1);
                        }

                        shortSum = Xse.add_epi16(shortSum, Xse.shufflelo_epi16(shortSum, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            v128 load = Xse.cvtsi32_si128(*(ushort*)address);
                            shortSum = Xse.add_epi16(shortSum, Xse.cvtepi8_epi16(load));

                            length -= 2;
                            address = (v128*)((ushort*)address + 1);
                        }

                        shortSum = Xse.add_epi16(shortSum, Xse.shufflelo_epi16(shortSum, Sse.SHUFFLE(0, 0, 0, 1)));
                        sum += shortSum.SShort0;

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
                        v256 longs = Avx.mm256_setzero_si256();

                        while (Hint.Likely(length >= 32))
                        {
                            int iterations = 0;
                            v256 acc0 = Avx.mm256_setzero_si256();
                            v256 acc1 = Avx.mm256_setzero_si256();
                            v256 acc2 = Avx.mm256_setzero_si256();
                            v256 acc3 = Avx.mm256_setzero_si256();

                            while (Hint.Likely(iterations < uint.MaxValue / ushort.MaxValue
                                & length >= 32))
                            {
                                v256 load0 = Xse.mm256_cvt2x2epi16_epi32(Avx.mm256_loadu_si256(address), out v256 load1);
                                address += 2;
                                v256 load2 = Xse.mm256_cvt2x2epi16_epi32(Avx.mm256_loadu_si256(address), out v256 load3);
                                address += 2;

                                acc0 = Avx2.mm256_add_epi32(acc0, load0);
                                acc1 = Avx2.mm256_add_epi32(acc1, load1);
                                acc2 = Avx2.mm256_add_epi32(acc2, load2);
                                acc3 = Avx2.mm256_add_epi32(acc3, load3);

                                length -= 32;
                                iterations++;
                            }

                            v256 cast = Xse.mm256_cvt2x2epi32_epi64(acc0, out acc0);
                            acc0 = Avx2.mm256_add_epi64(cast, acc0);
                            cast = Xse.mm256_cvt2x2epi32_epi64(acc1, out acc1);
                            acc1 = Avx2.mm256_add_epi64(cast, acc1);
                            cast = Xse.mm256_cvt2x2epi32_epi64(acc2, out acc2);
                            acc2 = Avx2.mm256_add_epi64(cast, acc2);
                            cast = Xse.mm256_cvt2x2epi32_epi64(acc3, out acc3);
                            acc3 = Avx2.mm256_add_epi64(cast, acc3);

                            longs = Avx2.mm256_add_epi64(longs, Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(acc0, acc1), Avx2.mm256_add_epi64(acc2, acc3)));
                        }

                        v128 csum_0 = Xse.add_epi64(Avx.mm256_castsi256_si128(longs), Avx2.mm256_extracti128_si256(longs, 1));
                        v128 csum_1 = Xse.vsum_epi64(csum_0);
                        long sum = csum_1.SLong0;

                        v128 intSum;

                        if (Hint.Likely((int)length >= 8))
                        {
                            v128 load = Xse.loadu_si128(address++);
                            v128 lo = Xse.cvt2x2epi16_epi32(load, out v128 hi);
                            intSum = Xse.add_epi32(lo, hi);

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                load = Xse.loadu_si128(address++);
                                lo = Xse.cvt2x2epi16_epi32(load, out hi);
                                intSum = Xse.add_epi32(intSum, Xse.add_epi32(lo, hi));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    load = Xse.loadu_si128(address++);
                                    lo = Xse.cvt2x2epi16_epi32(load, out hi);
                                    intSum = Xse.add_epi32(intSum, Xse.add_epi32(lo, hi));

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
                        else
                        {
                            intSum = Xse.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 4))
                        {
                            intSum = Xse.add_epi32(intSum, Xse.cvtepi16_epi32(Xse.cvtsi64x_si128(*(long*)address)));
                            length -= 4;
                            address = (v128*)((long*)address + 1);
                        }

                        intSum = Xse.add_epi32(intSum, Xse.shuffle_epi32(intSum, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            intSum = Xse.add_epi32(intSum, Xse.cvtepi16_epi32(Xse.cvtsi32_si128(*(int*)address)));
                            length -= 2;
                            address = (v128*)((int*)address + 1);
                        }

                        intSum = Xse.add_epi32(intSum, Xse.shuffle_epi32(intSum, Sse.SHUFFLE(0, 0, 0, 1)));
                        sum += intSum.SInt0;

                        if (Hint.Likely(length != 0))
                        {
                            sum += *(short*)address;
                        }

                        return sum;
                    }
                    else if (BurstArchitecture.IsSIMDSupported)
                    {
                        v128* address = (v128*)ptr;
                        v128 longs = Xse.setzero_si128();
                        v128 ZERO = Xse.setzero_si128();

                        while (Hint.Likely(length >= 16))
                        {
                            int iterations = 0;
                            v128 acc0 = Xse.setzero_si128();
                            v128 acc1 = Xse.setzero_si128();
                            v128 acc2 = Xse.setzero_si128();
                            v128 acc3 = Xse.setzero_si128();

                            while (Hint.Likely(iterations < uint.MaxValue / ushort.MaxValue
                                 & length >= 16))
                            {
                                v128 load0 = Xse.cvt2x2epi16_epi32(Xse.loadu_si128(address++), out v128 load1);
                                v128 load2 = Xse.cvt2x2epi16_epi32(Xse.loadu_si128(address++), out v128 load3);

                                acc0 = Xse.add_epi32(acc0, load0);
                                acc1 = Xse.add_epi32(acc1, load1);
                                acc2 = Xse.add_epi32(acc2, load2);
                                acc3 = Xse.add_epi32(acc3, load3);

                                length -= 16;
                                iterations++;
                            }

                            v128 cast = Xse.cvt2x2epi32_epi64(acc0, out acc0);
                            acc0 = Xse.add_epi64(cast, acc0);
                            cast = Xse.cvt2x2epi32_epi64(acc1, out acc1);
                            acc1 = Xse.add_epi64(cast, acc1);
                            cast = Xse.cvt2x2epi32_epi64(acc2, out acc2);
                            acc2 = Xse.add_epi64(cast, acc2);
                            cast = Xse.cvt2x2epi32_epi64(acc3, out acc3);
                            acc3 = Xse.add_epi64(cast, acc3);

                            longs = Xse.add_epi64(longs, Xse.add_epi64(Xse.add_epi64(acc0, acc1), Xse.add_epi64(acc2, acc3)));
                        }

                        v128 csum = Xse.vsum_epi64(longs);
                        long sum = csum.SLong0;

                        v128 intSum;

                        if (Hint.Likely((int)length >= 8))
                        {
                            v128 load = Xse.loadu_si128(address++);
                            v128 lo = Xse.cvt2x2epi16_epi32(load, out v128 hi);
                            intSum = Xse.add_epi32(lo, hi);

                            length -= 8;
                        }
                        else
                        {
                            intSum = Xse.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 4))
                        {
                            intSum = Xse.add_epi32(intSum, Xse.cvtepi16_epi32(Xse.cvtsi64x_si128(*(long*)address)));

                            length -= 4;
                            address = (v128*)((long*)address + 1);
                        }

                        intSum = Xse.add_epi32(intSum, Xse.shuffle_epi32(intSum, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            intSum = Xse.add_epi32(intSum, Xse.cvtepi16_epi32(Xse.cvtsi32_si128(*(int*)address)));

                            length -= 2;
                            address = (v128*)((int*)address + 1);
                        }

                        intSum = Xse.add_epi32(intSum, Xse.shuffle_epi32(intSum, Sse.SHUFFLE(0, 0, 0, 1)));
                        sum += intSum.SInt0;

                        if (Hint.Likely(length != 0))
                        {
                            sum += *(short*)address;
                        }

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
                        v256 acc0= Avx.mm256_setzero_ps();
                        v256 acc1= Avx.mm256_setzero_ps();
                        v256 acc2= Avx.mm256_setzero_ps();
                        v256 acc3= Avx.mm256_setzero_ps();
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

                        v128 csum1 = Xse.add_ps(Avx.mm256_castps256_ps128(csum0), Avx.mm256_extractf128_ps(csum0, 1));

                        if (Hint.Likely(length >= 4))
                        {
                            csum1 = Xse.add_ps(csum1, *(v128*)ptr_v256);
                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 4;
                        }

                        v128 csum2 = Xse.add_ps(csum1, Xse.shuffle_ps(csum1, csum1, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely(length >= 2))
                        {
                            csum2 = Xse.add_ps(csum2, Xse.cvtsi64x_si128(*(long*)ptr_v256));
                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 2;
                        }

                        v128 csum3 = Xse.add_ss(csum2, Xse.shuffle_ps(csum2, csum2, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely(length != 0))
                        {
                            csum3 = Xse.add_ss(csum3, Xse.cvtsi32_si128(*(int*)ptr_v256));
                        }

                        return csum3.Float0;
                    }
                    else if (BurstArchitecture.IsSIMDSupported)
                    {
                        v128 acc0 = Xse.setzero_ps();
                        v128 acc1 = Xse.setzero_ps();
                        v128 acc2 = Xse.setzero_ps();
                        v128 acc3 = Xse.setzero_ps();
                        v128* ptr_v128 = (v128*)ptr;

                        while (Hint.Likely(length >= 16))
                        {
                            acc0 = Xse.add_ps(acc0, *ptr_v128++);
                            acc1 = Xse.add_ps(acc1, *ptr_v128++);
                            acc2 = Xse.add_ps(acc2, *ptr_v128++);
                            acc3 = Xse.add_ps(acc3, *ptr_v128++);

                            length -= 16;
                        }

                        v128 csum0 = Xse.add_ps(Xse.add_ps(acc0, acc1), Xse.add_ps(acc2, acc3));

                        if (Hint.Likely((int)length >= 4))
                        {
                            csum0 = Xse.add_ps(csum0, *ptr_v128++);

                            if (Hint.Likely((int)length >= 2 * 4))
                            {
                                csum0 = Xse.add_ps(csum0, *ptr_v128++);

                                if (Hint.Likely((int)length >= 3 * 4))
                                {
                                    csum0 = Xse.add_ps(csum0, *ptr_v128++);
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

                        v128 csum1 = Xse.add_ps(csum0, Xse.shuffle_ps(csum0, csum0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely(length >= 2))
                        {
                            csum1 = Xse.add_ps(csum1, Xse.cvtsi64x_si128(*(long*)ptr_v128));
                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 2;
                        }

                        v128 csum2 = Xse.add_ss(csum1, Xse.shuffle_ps(csum1, csum1, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely(length != 0))
                        {
                            csum2 = Xse.add_ss(csum2, Xse.cvtsi32_si128(*(int*)ptr_v128));
                        }

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
                        v256 acc0= Avx.mm256_setzero_pd();
                        v256 acc1= Avx.mm256_setzero_pd();
                        v256 acc2= Avx.mm256_setzero_pd();
                        v256 acc3= Avx.mm256_setzero_pd();
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

                        v128 csum1 = Xse.add_pd(Avx.mm256_castpd256_pd128(csum0), Avx.mm256_extractf128_pd(csum0, 1));

                        if (Hint.Likely(length >= 2))
                        {
                            csum1 = Xse.add_pd(csum1, *(v128*)ptr_v256);
                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 2;
                        }

                        v128 csum2 = Xse.add_sd(csum1, Xse.shuffle_pd(csum1, csum1, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely(length != 0))
                        {
                            csum2 = Xse.add_sd(csum2, Xse.cvtsi64x_si128(*(long*)ptr_v256));
                        }

                        return csum2.Float0;
                    }
                    else if (BurstArchitecture.IsSIMDSupported)
                    {
                        v128 acc0 = Xse.setzero_ps();
                        v128 acc1 = Xse.setzero_ps();
                        v128 acc2 = Xse.setzero_ps();
                        v128 acc3 = Xse.setzero_ps();
                        v128* ptr_v128 = (v128*)ptr;

                        while (Hint.Likely(length >= 8))
                        {
                            acc0 = Xse.add_pd(acc0, *ptr_v128++);
                            acc1 = Xse.add_pd(acc1, *ptr_v128++);
                            acc2 = Xse.add_pd(acc2, *ptr_v128++);
                            acc3 = Xse.add_pd(acc3, *ptr_v128++);

                            length -= 8;
                        }

                        v128 csum0 = Xse.add_pd(Xse.add_pd(acc0, acc1), Xse.add_pd(acc2, acc3));

                        if (Hint.Likely((int)length >= 2))
                        {
                            csum0 = Xse.add_pd(csum0, *ptr_v128++);

                            if (Hint.Likely((int)length >= 2 * 2))
                            {
                                csum0 = Xse.add_pd(csum0, *ptr_v128++);

                                if (Hint.Likely((int)length >= 3 * 2))
                                {
                                    csum0 = Xse.add_pd(csum0, *ptr_v128++);
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

                        v128 csum1 = Xse.add_sd(csum0, Xse.shuffle_pd(csum0, csum0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely(length != 0))
                        {
                            csum1 = Xse.add_sd(csum1, Xse.cvtsi64x_si128(*(long*)ptr_v128));
                        }

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Sum((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, floatMode);
        }
    }
}