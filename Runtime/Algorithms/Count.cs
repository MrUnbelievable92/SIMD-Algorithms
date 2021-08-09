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
        public static ulong SIMD_Count(bool* ptr, long length, bool value, TypeCode returnType)
        {
Assert.IsBetween((int)returnType, (int)TypeCode.SByte, (int)TypeCode.UInt64);
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

            switch (returnType)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        ulong originalLength = (ulong)length;
                        v256* ptr_v256 = (v256*)ptr;
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);

                        while (Hint.Likely((int)length >= 32 * 4))
                        {
                            count0 = Avx2.mm256_add_epi8(count0, Avx.mm256_loadu_si256(ptr_v256++));
                            count1 = Avx2.mm256_add_epi8(count1, Avx.mm256_loadu_si256(ptr_v256++));
                            count2 = Avx2.mm256_add_epi8(count2, Avx.mm256_loadu_si256(ptr_v256++));
                            count3 = Avx2.mm256_add_epi8(count3, Avx.mm256_loadu_si256(ptr_v256++));

                            length -= 32 * 4;
                        }

                        count0 = Avx2.mm256_add_epi8(Avx2.mm256_add_epi8(count0, count1), Avx2.mm256_add_epi8(count2, count3));

                        if (Hint.Likely((int)length >= 32))
                        {
                            count0 = Avx2.mm256_add_epi8(count0, Avx.mm256_loadu_si256(ptr_v256++));

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
                        }
                        else { }

                        v128 count128 = Sse2.add_epi8(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1)); 

                        if (Hint.Likely((int)length >= 16))
                        {
                            count128 = Sse2.add_epi8(count128, Sse2.loadu_si128(ptr_v256));
                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);

                            length -= 16;
                        }
                        else { }

                        count128 = Sse2.add_epi8(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count128 = Sse2.add_epi8(count128, Sse2.cvtsi64x_si128(*(long*)ptr_v256));
                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);

                            length -= 8;
                        }
                        else { }

                        count128 = Sse2.add_epi8(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.add_epi8(count128, Sse2.cvtsi32_si128(*(int*)ptr_v256));
                            ptr_v256 = (v256*)((int*)ptr_v256 + 1);

                            length -= 4;
                        }
                        else { }

                        count128 = Sse2.add_epi8(count128, Sse2.shufflelo_epi16(count128, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.add_epi8(count128, Sse2.insert_epi16(default(v128), *(ushort*)ptr_v256, 0));
                            ptr_v256 = (v256*)((ushort*)ptr_v256 + 1);

                            length -= 2;
                        }
                        else { }

                        count128 = Sse2.add_epi8(count128, Sse2.bsrli_si128(count128, 1 * sizeof(byte)));

                        byte countTotal = count128.Byte0;

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
                            return (byte)(originalLength - countTotal);
                        }
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        ulong originalLength = (ulong)length;
                        v128* ptr_v128 = (v128*)ptr;
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);

                        while (Hint.Likely((int)length >= 16 * 4))
                        {
                            count0 = Sse2.add_epi8(count0, Sse2.loadu_si128(ptr_v128++));
                            count1 = Sse2.add_epi8(count1, Sse2.loadu_si128(ptr_v128++));
                            count2 = Sse2.add_epi8(count2, Sse2.loadu_si128(ptr_v128++));
                            count3 = Sse2.add_epi8(count3, Sse2.loadu_si128(ptr_v128++));
                        
                            length -= 16 * 4;
                        }

                        count0 = Sse2.add_epi8(Sse2.add_epi8(count0, count1), Sse2.add_epi8(count2, count3)); 

                        if (Hint.Likely((int)length >= 16))
                        {
                            count0 = Sse2.add_epi8(count0, Sse2.loadu_si128(ptr_v128++));

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
                        }
                        else { }

                        count0 = Sse2.add_epi8(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Sse2.add_epi8(count0, Sse2.cvtsi64x_si128(*(long*)ptr_v128));
                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);

                            length -= 8;
                        }
                        else { }

                        count0 = Sse2.add_epi8(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.add_epi8(count0, Sse2.cvtsi32_si128(*(int*)ptr_v128));
                            ptr_v128 = (v128*)((int*)ptr_v128 + 1);

                            length -= 4;
                        }
                        else { }

                        count0 = Sse2.add_epi8(count0, Sse2.shufflelo_epi16(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.add_epi8(count0, Sse2.insert_epi16(default(v128), *(ushort*)ptr_v128, 0));
                            ptr_v128 = (v128*)((ushort*)ptr_v128 + 1);

                            length -= 2;
                        }
                        else { }

                        count0 = Sse2.add_epi8(count0, Sse2.bsrli_si128(count0, 1 * sizeof(byte)));

                        byte countTotal = count0.Byte0;

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
                            return (byte)(originalLength - countTotal);
                        }
                    }
                    else
                    {
                        byte count = 0;

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
                            return (byte)(length - count);
                        }
                    }
                }
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        ulong originalLength = (ulong)length;
                        v256* ptr_v256 = (v256*)ptr;
                        v256 ZERO = default(v256);
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);
                        v256 longs = default(v256);

                        while (Hint.Likely(length >= 255 * 32 * 4))
                        {
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
                            
                            count0 = Avx.mm256_setzero_si256();
                            count1 = Avx.mm256_setzero_si256();
                            count2 = Avx.mm256_setzero_si256();
                            count3 = Avx.mm256_setzero_si256();

                            length -= 255 * 32 * 4;
                        }


                        while (Hint.Likely((int)length >= 32 * 4))
                        {
                            count0 = Avx2.mm256_add_epi8(count0, Avx.mm256_loadu_si256(ptr_v256++));
                            count1 = Avx2.mm256_add_epi8(count1, Avx.mm256_loadu_si256(ptr_v256++));
                            count2 = Avx2.mm256_add_epi8(count2, Avx.mm256_loadu_si256(ptr_v256++));
                            count3 = Avx2.mm256_add_epi8(count3, Avx.mm256_loadu_si256(ptr_v256++));
                        
                            length -= 32 * 4;
                        }
                        
                        v256 sad0_2 = Avx2.mm256_sad_epu8(count0, ZERO);
                        v256 sad1_2 = Avx2.mm256_sad_epu8(count1, ZERO);
                        v256 sad2_2 = Avx2.mm256_sad_epu8(count2, ZERO);
                        v256 sad3_2 = Avx2.mm256_sad_epu8(count3, ZERO);
                        
                        v256 csum0_2 = Avx2.mm256_add_epi64(sad0_2, sad1_2);
                        v256 csum1_2 = Avx2.mm256_add_epi64(sad2_2, sad3_2);
                        v256 csum2_2 = Avx2.mm256_add_epi64(csum0_2, csum1_2);
                        longs = Avx2.mm256_add_epi64(longs, csum2_2);


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
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);
                        v128 longs = default(v128);

                        while (Hint.Likely(length >= 255 * 16 * 4))
                        {
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

                            count0 = Sse2.setzero_si128();
                            count1 = Sse2.setzero_si128();
                            count2 = Sse2.setzero_si128();
                            count3 = Sse2.setzero_si128();

                            length -= 255 * 16 * 4;
                        }


                        while (Hint.Likely((int)length >= 16 * 4))
                        {
                            count0 = Sse2.add_epi8(count0, Sse2.loadu_si128(ptr_v128++));
                            count1 = Sse2.add_epi8(count1, Sse2.loadu_si128(ptr_v128++));
                            count2 = Sse2.add_epi8(count2, Sse2.loadu_si128(ptr_v128++));
                            count3 = Sse2.add_epi8(count3, Sse2.loadu_si128(ptr_v128++));
                        
                            length -= 16 * 4;
                        }
                        
                        v128 sad0_2 = Sse2.sad_epu8(count0, ZERO);
                        v128 sad1_2 = Sse2.sad_epu8(count1, ZERO);
                        v128 sad2_2 = Sse2.sad_epu8(count2, ZERO);
                        v128 sad3_2 = Sse2.sad_epu8(count3, ZERO);
                        
                        v128 csum0_2 = Sse2.add_epi64(sad0_2, sad1_2);
                        v128 csum1_2 = Sse2.add_epi64(sad2_2, sad3_2);
                        v128 csum2_2 = Sse2.add_epi64(csum0_2, csum1_2);
                        longs = Sse2.add_epi64(longs, csum2_2);


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

                        if (Hint.Likely((int)length >= 8))
                        {
                            bytes = Sse2.cvtsi64x_si128(*(long*)ptr_v128);
                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);

                            length -= 8;
                        }
                        else 
                        { 
                            bytes = default(v128);
                        }


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

                default: return 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<bool> array, bool value = true, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<bool> array, int index, bool value= true, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<bool> array, int index, int numEntries, bool value= true, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<bool> array, bool value= true, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<bool> array, int index, bool value= true, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<bool> array, int index, int numEntries, bool value= true, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<bool> array, bool value= true, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<bool> array, int index, bool value= true, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<bool> array, int index, int numEntries, bool value= true, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, returnType);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(byte* ptr, long length, byte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.UInt64)
        {
Assert.IsBetween((int)returnType, (int)TypeCode.SByte, (int)TypeCode.UInt64);
Assert.IsNonNegative(length);
            
            long originalLength = length;

            switch (returnType)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 ZERO = Avx.mm256_setzero_si256();

                        v256 broadcast = Avx.mm256_set1_epi8(value);
                        v256* ptr_v256 = (v256*)ptr;

                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);
                        
                        while (Hint.Likely(length >= 4 * 32))
                        {
                            count0 = Avx2.mm256_sub_epi8(count0, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count1 = Avx2.mm256_sub_epi8(count1, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count2 = Avx2.mm256_sub_epi8(count2, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count3 = Avx2.mm256_sub_epi8(count3, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            length -= 4 * 32;
                        }

                        count0 = Avx2.mm256_add_epi8(Avx2.mm256_add_epi8(count0, count1), Avx2.mm256_add_epi8(count2, count3));
                        
                        if (Hint.Likely((int)length >= 32))
                        {
                            count0 = Avx2.mm256_sub_epi8(count0, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 32))
                            {
                                count0 = Avx2.mm256_sub_epi8(count0, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 32))
                                {
                                    count0 = Avx2.mm256_sub_epi8(count0, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi8(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                        if (Hint.Likely((int)length >= 16))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.Bytes128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 16;
                        }
                        else { }

                        count128 = Sse2.add_epi8(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.Bytes128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 8;
                        }
                        else { }

                        count128 = Sse2.add_epi8(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.Bytes128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }
                        
                        count128 = Sse2.add_epi8(count128, Sse2.shufflelo_epi16(count128, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.Bytes128(Sse2.insert_epi16(default(v128), *(ushort*)ptr_v256, 0), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((ushort*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }
                        
                        count128 = Sse2.add_epi8(count128, Sse2.bsrli_si128(count128, 1 * sizeof(byte)));
                        byte countTotal = count128.Byte0;

                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (byte)((ulong)originalLength - 1 - countTotal);
                            }

                            bool comparison = Compare.Bytes(*(byte*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (byte)((ulong)originalLength - countTotal);
                            }
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 ZERO = Sse2.setzero_si128();
                        
                        v128* ptr_v128 = (v128*)ptr;

                        v128 broadcast = Sse2.set1_epi8((sbyte)value);
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);

                        while (Hint.Likely(length >= 4 * 16))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count1 = Sse2.sub_epi8(count1, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count2 = Sse2.sub_epi8(count2, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count3 = Sse2.sub_epi8(count3, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            length -= 4 * 16;
                        }

                        count0 = Sse2.add_epi8(Sse2.add_epi8(count0, count1), Sse2.add_epi8(count2, count3));

                        if (Hint.Likely((int)length >= 16))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi8(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 8;
                        }
                        else { }

                        count0 = Sse2.add_epi8(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                            length -= 4;
                        }
                        else { }
                        
                        count0 = Sse2.add_epi8(count0, Sse2.shufflelo_epi16(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.insert_epi16(ZERO, *(ushort*)ptr_v128, 0), broadcast, where));

                            ptr_v128 = (v128*)((ushort*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }
                        
                        count0 = Sse2.add_epi8(count0, Sse2.bsrli_si128(count0, 1 * sizeof(byte)));
                        byte countTotal = count0.Byte0;

                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (byte)((ulong)originalLength - 1 - countTotal);
                            }

                            bool comparison = Compare.Bytes(*(byte*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (byte)((ulong)originalLength - countTotal);
                            }
                        }

                        return countTotal;
                    }
                    else
                    {
                        byte count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.Bytes(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 ZERO = Avx.mm256_setzero_si256();

                        v256 broadcast = Avx.mm256_set1_epi8(value);
                        v256* ptr_v256 = (v256*)ptr;

                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);
                        v256 longCount = default(v256);
                        
                        while (Hint.Likely(length >= 4 * 32 * byte.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Avx2.mm256_sub_epi8(count0, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi8(count1, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi8(count2, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi8(count3, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < byte.MaxValue));

                            length -= 4 * 32 * byte.MaxValue;
                            
                            v256 sum0 = Avx2.mm256_sad_epu8(ZERO, count0);
                            v256 sum1 = Avx2.mm256_sad_epu8(ZERO, count1);
                            v256 sum2 = Avx2.mm256_sad_epu8(ZERO, count2);
                            v256 sum3 = Avx2.mm256_sad_epu8(ZERO, count3);

                            sum0 = Avx2.mm256_add_epi64(sum0, sum1);
                            sum1 = Avx2.mm256_add_epi64(sum2, sum3);

                            longCount = Avx2.mm256_add_epi64(longCount, Avx2.mm256_add_epi64(sum0, sum1));

                            count0 = Avx.mm256_setzero_si256();
                            count1 = Avx.mm256_setzero_si256();
                            count2 = Avx.mm256_setzero_si256();
                            count3 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely(length >= 4 * 32))
                        {
                            do
                            {
                                count0 = Avx2.mm256_sub_epi8(count0, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi8(count1, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi8(count2, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi8(count3, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                length -= 4 * 32;
                            }
                            while (Hint.Likely(length >= 4 * 32));

                            v256 sum0 = Avx2.mm256_sad_epu8(ZERO, count0);
                            v256 sum1 = Avx2.mm256_sad_epu8(ZERO, count1);
                            v256 sum2 = Avx2.mm256_sad_epu8(ZERO, count2);
                            v256 sum3 = Avx2.mm256_sad_epu8(ZERO, count3);

                            sum0 = Avx2.mm256_add_epi64(sum0, sum1);
                            sum1 = Avx2.mm256_add_epi64(sum2, sum3);

                            longCount = Avx2.mm256_add_epi64(longCount, Avx2.mm256_add_epi64(sum0, sum1));

                            count0 = Avx.mm256_setzero_si256();
                        }
                        
                        if (Hint.Likely((int)length >= 32))
                        {
                            count0 = Avx2.mm256_sub_epi8(count0, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 32))
                            {
                                count0 = Avx2.mm256_sub_epi8(count0, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 32))
                                {
                                    count0 = Avx2.mm256_sub_epi8(count0, Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi8(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                        if (Hint.Likely((int)length >= 16))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.Bytes128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 16;
                        }
                        else { }

                        count128 = Sse2.add_epi8(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.Bytes128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 8;
                        }
                        else { }

                        v128 longCount128 = Sse2.add_epi64(Avx.mm256_castsi256_si128(longCount), Avx2.mm256_extracti128_si256(longCount, 1));
                        count128 = Sse2.add_epi8(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.Bytes128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }
                        
                        count128 = Sse2.add_epi8(count128, Sse2.shufflelo_epi16(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        longCount128 = Sse2.add_epi64(longCount128, Sse2.bsrli_si128(longCount128, 1 * sizeof(ulong)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.Bytes128(Sse2.insert_epi16(default(v128), *(ushort*)ptr_v256, 0), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((ushort*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }
                        
                        count128 = Sse2.add_epi8(count128, Sse2.bsrli_si128(count128, 1 * sizeof(byte)));
                        ulong countTotal = Sse4_1.extract_epi8(count128, 0);
                        countTotal += (ulong)Sse2.cvtsi128_si64x(longCount128);

                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (ulong)originalLength - 1 - countTotal;
                            }

                            bool comparison = Compare.Bytes(*(byte*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (ulong)originalLength - countTotal;
                            }
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 ZERO = Sse2.setzero_si128();
                        
                        v128* ptr_v128 = (v128*)ptr;

                        v128 broadcast = Sse2.set1_epi8((sbyte)value);
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);
                        v128 longCount = default(v128);

                        while (Hint.Likely(length >= 4 * 16 * byte.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi8(count1, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi8(count2, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi8(count3, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < byte.MaxValue));

                            length -= 4 * 16 * byte.MaxValue;
                            
                            v128 sum0 = Sse2.sad_epu8(ZERO, count0);
                            v128 sum1 = Sse2.sad_epu8(ZERO, count1);
                            v128 sum2 = Sse2.sad_epu8(ZERO, count2);
                            v128 sum3 = Sse2.sad_epu8(ZERO, count3);

                            sum0 = Sse2.add_epi64(sum0, sum1);
                            sum1 = Sse2.add_epi64(sum2, sum3);

                            longCount = Sse2.add_epi64(longCount, Sse2.add_epi64(sum0, sum1));

                            count0 = Sse2.setzero_si128();
                            count1 = Sse2.setzero_si128();
                            count2 = Sse2.setzero_si128();
                            count3 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely(length >= 4 * 16))
                        {
                            do
                            {
                                count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi8(count1, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi8(count2, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi8(count3, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                length -= 4 * 16;
                            }
                            while (Hint.Likely(length >= 4 * 16));

                            v128 sum0 = Sse2.sad_epu8(ZERO, count0);
                            v128 sum1 = Sse2.sad_epu8(ZERO, count1);
                            v128 sum2 = Sse2.sad_epu8(ZERO, count2);
                            v128 sum3 = Sse2.sad_epu8(ZERO, count3);

                            sum0 = Sse2.add_epi64(sum0, sum1);
                            sum1 = Sse2.add_epi64(sum2, sum3);

                            longCount = Sse2.add_epi64(longCount, Sse2.add_epi64(sum0, sum1));

                            count0 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 16))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi8(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 8;
                        }
                        else { }

                        count0 = Sse2.add_epi8(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                            length -= 4;
                        }
                        else { }
                        
                        count0 = Sse2.add_epi8(count0, Sse2.shufflelo_epi16(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        longCount = Sse2.add_epi64(longCount, Sse2.bsrli_si128(longCount, 1 * sizeof(ulong)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.Bytes128(Sse2.insert_epi16(ZERO, *(ushort*)ptr_v128, 0), broadcast, where));

                            ptr_v128 = (v128*)((ushort*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }
                        
                        count0 = Sse2.add_epi8(count0, Sse2.bsrli_si128(count0, 1 * sizeof(byte)));
                        ulong countTotal;

                        if (Sse4_1.IsSse41Supported)
                        {
                            countTotal = Sse4_1.extract_epi8(count0, 0);
                            countTotal += (ulong)Sse2.cvtsi128_si64x(longCount);
                        }
                        else
                        {
                            count0 = Sse2.and_si128(count0, Sse2.cvtsi32_si128(byte.MaxValue));
                            longCount = Sse2.add_epi64(longCount, count0);
                            countTotal = (ulong)Sse2.cvtsi128_si64x(longCount);
                        }

                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (ulong)originalLength - 1 - countTotal;
                            }

                            bool comparison = Compare.Bytes(*(byte*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (ulong)originalLength - countTotal;
                            }
                        }

                        return countTotal;
                    }
                    else
                    {
                        ulong count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.Bytes(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }

                default: return 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<byte> array, byte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<byte> array, int index, byte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<byte> array, int index, int numEntries, byte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<byte> array, byte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<byte> array, int index, byte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<byte> array, int index, int numEntries, byte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<byte> array, byte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<byte> array, int index, byte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<byte> array, int index, int numEntries, byte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, returnType);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(ushort* ptr, long length, ushort value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.UInt64)
        {
Assert.IsBetween((int)returnType, (int)TypeCode.SByte, (int)TypeCode.UInt64);
Assert.IsNonNegative(length);

            long originalLength = length;

            switch (returnType)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 broadcast = Avx.mm256_set1_epi16((short)value);
                        v256* ptr_v256 = (v256*)ptr;
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);

                        while (Hint.Likely(length >= 4 * 16))
                        {
                            count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count1 = Avx2.mm256_sub_epi16(count1, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count2 = Avx2.mm256_sub_epi16(count2, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count3 = Avx2.mm256_sub_epi16(count3, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            length -= 4 * 16;
                        }

                        count0 = Avx2.mm256_add_epi16(count0, count1);
                        count2 = Avx2.mm256_add_epi16(count2, count3);
                        count0 = Avx2.mm256_add_epi16(count0, count2);

                        if (Hint.Likely((int)length >= 16))
                        {
                            count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi16(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.UShorts128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 8;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.UShorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.UShorts128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shufflelo_epi16(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        ushort countTotal = count128.UShort0;

                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (ushort)((ulong)originalLength - 1 - countTotal);
                            }

                            bool comparison = Compare.UShorts(*(ushort*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (ushort)((ulong)originalLength - countTotal);
                            }
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 broadcast = Sse2.set1_epi16((short)value);
                        v128* ptr_v128 = (v128*)ptr;
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);

                        while (Hint.Likely(length >= 4 * 8))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count1 = Sse2.sub_epi16(count1, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count2 = Sse2.sub_epi16(count2, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count3 = Sse2.sub_epi16(count3, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            length -= 4 * 8;
                        }

                        count0 = Sse2.add_epi16(count0, count1);
                        count2 = Sse2.add_epi16(count2, count3);
                        count0 = Sse2.add_epi16(count0, count2);

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi16(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 4;
                        }
                        else { }

                        count0 = Sse2.add_epi16(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }

                        count0 = Sse2.add_epi16(count0, Sse2.shufflelo_epi16(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        ushort countTotal = count0.UShort0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (Sse4_1.IsSse41Supported)
                            {
                                if (where == Comparison.NotEqualTo)
                                {
                                    countTotal = (ushort)((ulong)originalLength - 1 - countTotal);
                                }
                            }
                            else
                            {  
                                if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                                {
                                    countTotal = (ushort)((ulong)originalLength - 1 - countTotal);
                                }
                            }

                            bool comparison = Compare.UShorts(*(ushort*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (Sse4_1.IsSse41Supported)
                            {
                                if (where == Comparison.NotEqualTo)
                                {
                                    countTotal = (ushort)((ulong)originalLength - countTotal);
                                }
                            }
                            else
                            {  
                                if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                                {
                                    countTotal = (ushort)((ulong)originalLength - countTotal);
                                }
                            }
                        }

                        return countTotal;
                    }
                    else
                    {
                        ushort count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.UShorts(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }
                case TypeCode.Int32:
                case TypeCode.UInt32:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 ZERO = Avx.mm256_setzero_si256();
                        
                        v256 broadcast = Avx.mm256_set1_epi16((short)value);
                        v256* ptr_v256 = (v256*)ptr;
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);
                        v256 intCount = default(v256);

                        while (Hint.Likely(length >= 4 * 16 * ushort.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi16(count1, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi16(count2, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi16(count3, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < ushort.MaxValue));

                            length -= 4 * 16 * ushort.MaxValue;

                            v256 intSum0 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count0, ZERO), Avx2.mm256_unpacklo_epi16(count1, ZERO));
                            v256 intSum1 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count0, ZERO), Avx2.mm256_unpackhi_epi16(count1, ZERO));
                            v256 intSum2 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count2, ZERO), Avx2.mm256_unpacklo_epi16(count3, ZERO));
                            v256 intSum3 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count2, ZERO), Avx2.mm256_unpackhi_epi16(count3, ZERO));
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum1);
                            intSum2 = Avx2.mm256_add_epi32(intSum2, intSum3);
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum2);

                            intCount = Avx2.mm256_add_epi32(intCount, intSum0);

                            count0 = Avx.mm256_setzero_si256();
                            count1 = Avx.mm256_setzero_si256();
                            count2 = Avx.mm256_setzero_si256();
                            count3 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely(length >= 4 * 16))
                        {
                            do
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi16(count1, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi16(count2, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi16(count3, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                length -= 4 * 16;
                            } 
                            while (Hint.Likely(length >= 4 * 16));

                            v256 intSum0 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count0, ZERO), Avx2.mm256_unpacklo_epi16(count1, ZERO));
                            v256 intSum1 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count0, ZERO), Avx2.mm256_unpackhi_epi16(count1, ZERO));
                            v256 intSum2 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count2, ZERO), Avx2.mm256_unpacklo_epi16(count3, ZERO));
                            v256 intSum3 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count2, ZERO), Avx2.mm256_unpackhi_epi16(count3, ZERO));
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum1);
                            intSum2 = Avx2.mm256_add_epi32(intSum2, intSum3);
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum2);

                            intCount = Avx2.mm256_add_epi32(intCount, intSum0);

                            count0 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely((int)length >= 16))
                        {
                            count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi16(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.UShorts128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 8;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));
                        v128 intCount128 = Sse2.add_epi32(Avx.mm256_castsi256_si128(intCount), Avx2.mm256_extracti128_si256(intCount, 1));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.UShorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }
                        
                        count128 = Sse2.add_epi16(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        intCount128 = Sse2.add_epi32(intCount128, Sse2.bsrli_si128(intCount128, 2 * sizeof(uint)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.UShorts128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shufflelo_epi16(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        count128 = Sse4_1.cvtepu16_epi32(count128);
                        intCount128 = Sse2.add_epi32(intCount128, Sse2.bsrli_si128(intCount128, 1 * sizeof(uint)));
                        uint countTotal = Sse2.add_epi32(intCount128, count128).UInt0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (uint)((ulong)originalLength - 1 - countTotal);
                            }

                            bool comparison = Compare.UShorts(*(ushort*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (uint)((ulong)originalLength - countTotal);
                            }
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 ZERO = Sse2.setzero_si128();

                        v128 broadcast = Sse2.set1_epi16((short)value);
                        v128* ptr_v128 = (v128*)ptr;
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);
                        v128 intCount = default(v128);

                        while (Hint.Likely(length >= 4 * 8 * ushort.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi16(count1, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi16(count2, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi16(count3, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < ushort.MaxValue));

                            length -= 4 * 8 * ushort.MaxValue;

                            v128 intSum0 = Sse2.add_epi32(Sse2.unpacklo_epi16(count0, ZERO), Sse2.unpacklo_epi16(count1, ZERO));
                            v128 intSum1 = Sse2.add_epi32(Sse2.unpackhi_epi16(count0, ZERO), Sse2.unpackhi_epi16(count1, ZERO));
                            v128 intSum2 = Sse2.add_epi32(Sse2.unpacklo_epi16(count2, ZERO), Sse2.unpacklo_epi16(count3, ZERO));
                            v128 intSum3 = Sse2.add_epi32(Sse2.unpackhi_epi16(count2, ZERO), Sse2.unpackhi_epi16(count3, ZERO));

                            intSum0 = Sse2.add_epi32(intSum0, intSum1);
                            intSum2 = Sse2.add_epi32(intSum2, intSum3);
                            intSum0 = Sse2.add_epi32(intSum0, intSum2);

                            intCount = Sse2.add_epi32(intCount, intSum0);

                            count0 = Sse2.setzero_si128();
                            count1 = Sse2.setzero_si128();
                            count2 = Sse2.setzero_si128();
                            count3 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely(length >= 4 * 8))
                        {
                            do
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi16(count1, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi16(count2, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi16(count3, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                length -= 4 * 8;
                            } 
                            while (Hint.Likely(length >= 4 * 8));

                            v128 intSum0 = Sse2.add_epi32(Sse2.unpacklo_epi16(count0, ZERO), Sse2.unpacklo_epi16(count1, ZERO));
                            v128 intSum1 = Sse2.add_epi32(Sse2.unpackhi_epi16(count0, ZERO), Sse2.unpackhi_epi16(count1, ZERO));
                            v128 intSum2 = Sse2.add_epi32(Sse2.unpacklo_epi16(count2, ZERO), Sse2.unpacklo_epi16(count3, ZERO));
                            v128 intSum3 = Sse2.add_epi32(Sse2.unpackhi_epi16(count2, ZERO), Sse2.unpackhi_epi16(count3, ZERO));

                            intSum0 = Sse2.add_epi32(intSum0, intSum1);
                            intSum2 = Sse2.add_epi32(intSum2, intSum3);
                            intSum0 = Sse2.add_epi32(intSum0, intSum2);

                            intCount = Sse2.add_epi32(intCount, intSum0);

                            count0 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi16(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 4;
                        }
                        else { }

                        count0 = Sse2.add_epi16(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        intCount = Sse2.add_epi32(intCount, Sse2.bsrli_si128(intCount, 2 * sizeof(uint)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }

                        count0 = Sse2.add_epi16(count0, Sse2.shufflelo_epi16(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Sse4_1.IsSse41Supported)
                        {
                            count0 = Sse4_1.cvtepu16_epi32(count0);
                        }
                        else
                        {
                            count0 = Sse2.and_si128(count0, Sse2.cvtsi32_si128(ushort.MaxValue));
                        }

                        intCount = Sse2.add_epi32(intCount, Sse2.bsrli_si128(intCount, 1 * sizeof(uint)));
                        uint countTotal = Sse2.add_epi32(intCount, count0).UInt0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (Sse4_1.IsSse41Supported)
                            {
                                if (where == Comparison.NotEqualTo)
                                {
                                    countTotal = (uint)((ulong)originalLength - 1 - countTotal);
                                }
                            }
                            else
                            {  
                                if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                                {
                                    countTotal = (uint)((ulong)originalLength - 1 - countTotal);
                                }
                            }

                            bool comparison = Compare.UShorts(*(ushort*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (Sse4_1.IsSse41Supported)
                            {
                                if (where == Comparison.NotEqualTo)
                                {
                                    countTotal = (uint)((ulong)originalLength - countTotal);
                                }
                            }
                            else
                            {  
                                if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                                {
                                    countTotal = (uint)((ulong)originalLength - countTotal);
                                }
                            }
                        }

                        return countTotal;
                    }
                    else
                    {
                        uint count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.UShorts(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }
                case TypeCode.Int64:
                case TypeCode.UInt64:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 ZERO = Avx.mm256_setzero_si256();
                        
                        v256 broadcast = Avx.mm256_set1_epi16((short)value);
                        v256* ptr_v256 = (v256*)ptr;
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);
                        v256 longCount = default(v256);

                        while (Hint.Likely(length >= 4 * 16 * ushort.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi16(count1, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi16(count2, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi16(count3, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < ushort.MaxValue));

                            length -= 4 * 16 * ushort.MaxValue;

                            v256 intSum0 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count0, ZERO), Avx2.mm256_unpacklo_epi16(count1, ZERO));
                            v256 intSum1 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count0, ZERO), Avx2.mm256_unpackhi_epi16(count1, ZERO));
                            v256 intSum2 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count2, ZERO), Avx2.mm256_unpacklo_epi16(count3, ZERO));
                            v256 intSum3 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count2, ZERO), Avx2.mm256_unpackhi_epi16(count3, ZERO));
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum1);
                            intSum2 = Avx2.mm256_add_epi32(intSum2, intSum3);
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum2);

                            v256 longSum = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(intSum0, ZERO), Avx2.mm256_unpackhi_epi32(intSum0, ZERO));
                            longCount = Avx2.mm256_add_epi64(longCount, longSum);

                            count0 = Avx.mm256_setzero_si256();
                            count1 = Avx.mm256_setzero_si256();
                            count2 = Avx.mm256_setzero_si256();
                            count3 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely(length >= 4 * 16))
                        {
                            do
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi16(count1, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi16(count2, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi16(count3, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                length -= 4 * 16;
                            } 
                            while (Hint.Likely(length >= 4 * 16));

                            v256 intSum0 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count0, ZERO), Avx2.mm256_unpacklo_epi16(count1, ZERO));
                            v256 intSum1 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count0, ZERO), Avx2.mm256_unpackhi_epi16(count1, ZERO));
                            v256 intSum2 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count2, ZERO), Avx2.mm256_unpacklo_epi16(count3, ZERO));
                            v256 intSum3 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count2, ZERO), Avx2.mm256_unpackhi_epi16(count3, ZERO));
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum1);
                            intSum2 = Avx2.mm256_add_epi32(intSum2, intSum3);
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum2);

                            v256 longSum = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(intSum0, ZERO), Avx2.mm256_unpackhi_epi32(intSum0, ZERO));
                            longCount = Avx2.mm256_add_epi64(longCount, longSum);

                            count0 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely((int)length >= 16))
                        {
                            count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    count0 = Avx2.mm256_sub_epi16(count0, Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi16(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.UShorts128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 8;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.UShorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }
                        
                        v128 longCount128 = Sse2.add_epi64(Avx.mm256_castsi256_si128(longCount), Avx2.mm256_extracti128_si256(longCount, 1));
                        count128 = Sse2.add_epi16(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.UShorts128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shufflelo_epi16(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        count128 = Sse4_1.cvtepu16_epi64(count128);
                        longCount128 = Sse2.add_epi64(longCount128, Sse2.bsrli_si128(longCount128, 1 * sizeof(ulong)));
                        ulong countTotal = Sse2.add_epi64(longCount128, count128).ULong0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (ulong)originalLength - 1 - countTotal;
                            }

                            bool comparison = Compare.UShorts(*(ushort*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (ulong)originalLength - countTotal;
                            }
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 ZERO = Sse2.setzero_si128();

                        v128 broadcast = Sse2.set1_epi16((short)value);
                        v128* ptr_v128 = (v128*)ptr;
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);
                        v128 longCount = default(v128);

                        while (Hint.Likely(length >= 4 * 8 * ushort.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi16(count1, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi16(count2, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi16(count3, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < ushort.MaxValue));

                            length -= 4 * 8 * ushort.MaxValue;

                            v128 intSum0 = Sse2.add_epi32(Sse2.unpacklo_epi16(count0, ZERO), Sse2.unpacklo_epi16(count1, ZERO));
                            v128 intSum1 = Sse2.add_epi32(Sse2.unpackhi_epi16(count0, ZERO), Sse2.unpackhi_epi16(count1, ZERO));
                            v128 intSum2 = Sse2.add_epi32(Sse2.unpacklo_epi16(count2, ZERO), Sse2.unpacklo_epi16(count3, ZERO));
                            v128 intSum3 = Sse2.add_epi32(Sse2.unpackhi_epi16(count2, ZERO), Sse2.unpackhi_epi16(count3, ZERO));

                            intSum0 = Sse2.add_epi32(intSum0, intSum1);
                            intSum2 = Sse2.add_epi32(intSum2, intSum3);
                            intSum0 = Sse2.add_epi32(intSum0, intSum2);

                            v128 longSum = Sse2.add_epi64(Sse2.unpacklo_epi32(intSum0, ZERO), Sse2.unpackhi_epi32(intSum0, ZERO));
                            longCount = Sse2.add_epi64(longCount, longSum);

                            count0 = Sse2.setzero_si128();
                            count1 = Sse2.setzero_si128();
                            count2 = Sse2.setzero_si128();
                            count3 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely(length >= 4 * 8))
                        {
                            do
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi16(count1, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi16(count2, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi16(count3, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                length -= 4 * 8;
                            } 
                            while (Hint.Likely(length >= 4 * 8));

                            v128 intSum0 = Sse2.add_epi32(Sse2.unpacklo_epi16(count0, ZERO), Sse2.unpacklo_epi16(count1, ZERO));
                            v128 intSum1 = Sse2.add_epi32(Sse2.unpackhi_epi16(count0, ZERO), Sse2.unpackhi_epi16(count1, ZERO));
                            v128 intSum2 = Sse2.add_epi32(Sse2.unpacklo_epi16(count2, ZERO), Sse2.unpacklo_epi16(count3, ZERO));
                            v128 intSum3 = Sse2.add_epi32(Sse2.unpackhi_epi16(count2, ZERO), Sse2.unpackhi_epi16(count3, ZERO));

                            intSum0 = Sse2.add_epi32(intSum0, intSum1);
                            intSum2 = Sse2.add_epi32(intSum2, intSum3);
                            intSum0 = Sse2.add_epi32(intSum0, intSum2);

                            v128 longSum = Sse2.add_epi64(Sse2.unpacklo_epi32(intSum0, ZERO), Sse2.unpackhi_epi32(intSum0, ZERO));
                            longCount = Sse2.add_epi64(longCount, longSum);

                            count0 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi16(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 4;
                        }
                        else { }

                        count0 = Sse2.add_epi16(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.UShorts128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }

                        count0 = Sse2.add_epi16(count0, Sse2.shufflelo_epi16(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        if (Sse4_1.IsSse41Supported)
                        {
                            count0 = Sse4_1.cvtepu16_epi64(count0);
                        }
                        else
                        {
                            count0 = Sse2.and_si128(count0, Sse2.cvtsi32_si128(ushort.MaxValue));
                        }

                        longCount = Sse2.add_epi64(longCount, Sse2.bsrli_si128(longCount, 1 * sizeof(ulong)));
                        ulong countTotal = Sse2.add_epi64(longCount, count0).ULong0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (Sse4_1.IsSse41Supported)
                            {
                                if (where == Comparison.NotEqualTo)
                                {
                                    countTotal = (ulong)originalLength - 1 - countTotal;
                                }
                            }
                            else
                            {  
                                if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                                {
                                    countTotal = (ulong)originalLength - 1 - countTotal;
                                }
                            }

                            bool comparison = Compare.UShorts(*(ushort*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (Sse4_1.IsSse41Supported)
                            {
                                if (where == Comparison.NotEqualTo)
                                {
                                    countTotal = (ulong)originalLength - countTotal;
                                }
                            }
                            else
                            {  
                                if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                                {
                                    countTotal = (ulong)originalLength - countTotal;
                                }
                            }
                        }

                        return countTotal;
                    }
                    else
                    {
                        ulong count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.UShorts(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }

                default: return 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<ushort> array, ushort value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<ushort> array, int index, ushort value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<ushort> array, int index, int numEntries, ushort value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<ushort> array, ushort value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<ushort> array, int index, ushort value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<ushort> array, int index, int numEntries, ushort value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<ushort> array, ushort value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<ushort> array, int index, ushort value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<ushort> array, int index, int numEntries, ushort value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, returnType);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(uint* ptr, long length, uint value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.UInt64)
        {
Assert.IsBetween((int)returnType, (int)TypeCode.SByte, (int)TypeCode.UInt64);
Assert.IsNonNegative(length);

            long originalLength = length;
            
            switch (returnType)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 broadcast = Avx.mm256_set1_epi32((int)value);
                        v256* ptr_v256 = (v256*)ptr;
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);

                        while (Hint.Likely(length >= 32))
                        {
                            count0 = Avx2.mm256_sub_epi32(count0, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count1 = Avx2.mm256_sub_epi32(count1, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count2 = Avx2.mm256_sub_epi32(count2, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count3 = Avx2.mm256_sub_epi32(count3, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            
                            length -= 32;
                        }

                        count0 = Avx2.mm256_add_epi32(Avx2.mm256_add_epi32(count0, count1), Avx2.mm256_add_epi32(count2, count3));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Avx2.mm256_sub_epi32(count0, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                count0 = Avx2.mm256_sub_epi32(count0, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    count0 = Avx2.mm256_sub_epi32(count0, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi32(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi32(count128, Compare.UInts128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }

                        count128 = Sse2.add_epi32(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi32(count128, Compare.UInts128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        count128 = Sse2.add_epi32(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        uint countTotal = count128.UInt0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (uint)((ulong)originalLength - 1 - countTotal);
                            }

                            bool comparison = Compare.UInts(*(uint*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (uint)((ulong)originalLength - countTotal);
                            }
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 broadcast = Sse2.set1_epi32((int)value);
                        v128* ptr_v128 = (v128*)ptr;
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);

                        while (Hint.Likely(length >= 16))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count1 = Sse2.sub_epi32(count1, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count2 = Sse2.sub_epi32(count2, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count3 = Sse2.sub_epi32(count3, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            length -= 16;
                        }

                        count0 = Sse2.add_epi32(Sse2.add_epi32(count0, count1), Sse2.add_epi32(count2, count3));


                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 4))
                            {
                                count0 = Sse2.sub_epi32(count0, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 4))
                                {
                                    count0 = Sse2.sub_epi32(count0, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi32(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.UInts128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }

                        count0 = Sse2.add_epi32(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        uint countTotal = count0.UInt0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (Sse4_1.IsSse41Supported)
                            {
                                if (where == Comparison.NotEqualTo)
                                {
                                    countTotal = (uint)((ulong)originalLength - 1 - countTotal);
                                }
                            }
                            else
                            {  
                                if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                                {
                                    countTotal = (uint)((ulong)originalLength - 1 - countTotal);
                                }
                            }

                            bool comparison = Compare.UInts(*(uint*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (Sse4_1.IsSse41Supported)
                            {
                                if (where == Comparison.NotEqualTo)
                                {
                                    countTotal = (uint)((ulong)originalLength - countTotal);
                                }
                            }
                            else
                            {  
                                if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                                {
                                    countTotal = (uint)((ulong)originalLength - countTotal);
                                }
                            }
                        }

                        return countTotal;
                    }
                    else
                    {
                        uint count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.UInts(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }
                case TypeCode.Int64:
                case TypeCode.UInt64:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 ZERO = default(v256);

                        v256 broadcast = Avx.mm256_set1_epi32((int)value);
                        v256* ptr_v256 = (v256*)ptr;
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);
                        v256 longCount = default(v256);

                        while (Hint.Likely(length >= 4L * 8 * uint.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Avx2.mm256_sub_epi32(count0, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi32(count1, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi32(count2, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi32(count3, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < uint.MaxValue));

                            v256 longSum0 = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(count0, ZERO), Avx2.mm256_unpacklo_epi32(count1, ZERO));
                            v256 longSum1 = Avx2.mm256_add_epi64(Avx2.mm256_unpackhi_epi32(count0, ZERO), Avx2.mm256_unpackhi_epi32(count1, ZERO));
                            v256 longSum2 = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(count2, ZERO), Avx2.mm256_unpacklo_epi32(count3, ZERO));
                            v256 longSum3 = Avx2.mm256_add_epi64(Avx2.mm256_unpackhi_epi32(count2, ZERO), Avx2.mm256_unpackhi_epi32(count3, ZERO));

                            length -= 4L * 8 * uint.MaxValue;

                            longCount = Avx2.mm256_add_epi64(longCount, Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(longSum0, longSum1), Avx2.mm256_add_epi64(longSum2, longSum3)));

                            count0 = Avx.mm256_setzero_si256();
                            count1 = Avx.mm256_setzero_si256();
                            count2 = Avx.mm256_setzero_si256();
                            count3 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely(length >= 32))
                        {
                            do
                            {
                                count0 = Avx2.mm256_sub_epi32(count0, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi32(count1, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi32(count2, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi32(count3, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            } 
                            while (Hint.Likely(length >= 32));

                            v256 longSum0 = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(count0, ZERO), Avx2.mm256_unpacklo_epi32(count1, ZERO));
                            v256 longSum1 = Avx2.mm256_add_epi64(Avx2.mm256_unpackhi_epi32(count0, ZERO), Avx2.mm256_unpackhi_epi32(count1, ZERO));
                            v256 longSum2 = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(count2, ZERO), Avx2.mm256_unpacklo_epi32(count3, ZERO));
                            v256 longSum3 = Avx2.mm256_add_epi64(Avx2.mm256_unpackhi_epi32(count2, ZERO), Avx2.mm256_unpackhi_epi32(count3, ZERO));

                            length -= 32;

                            longCount = Avx2.mm256_add_epi64(longCount, Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(longSum0, longSum1), Avx2.mm256_add_epi64(longSum2, longSum3)));

                            count0 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Avx2.mm256_sub_epi32(count0, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                count0 = Avx2.mm256_sub_epi32(count0, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    count0 = Avx2.mm256_sub_epi32(count0, Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi32(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));
                        v128 longCount128 = Sse2.add_epi64(Avx.mm256_castsi256_si128(longCount), Avx2.mm256_extracti128_si256(longCount, 1));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi32(count128, Compare.UInts128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }

                        count128 = Sse2.add_epi32(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));
                        longCount128 = Sse2.add_epi64(longCount128, Sse2.bsrli_si128(longCount128, 1 * sizeof(ulong)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi32(count128, Compare.UInts128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        count128 = Sse2.add_epi32(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        ulong countTotal = count128.UInt0 + longCount128.ULong0;

                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (ulong)originalLength - 1 - countTotal;
                            }

                            bool comparison = Compare.UInts(*(uint*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (where == Comparison.NotEqualTo)
                            {
                                countTotal = (ulong)originalLength - countTotal;
                            }
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 ZERO = default(v128);

                        v128 broadcast = Sse2.set1_epi32((int)value);
                        v128* ptr_v128 = (v128*)ptr;
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);
                        v128 longCount = default(v128);

                        if (Hint.Likely(length >= 4L * 4 * uint.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Sse2.sub_epi32(count0, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi32(count1, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi32(count2, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi32(count3, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < uint.MaxValue));

                            length -= 4L * 4 * uint.MaxValue;

                            v128 longSum0 = Sse2.add_epi64(Sse2.unpacklo_epi32(count0, ZERO), Sse2.unpacklo_epi32(count1, ZERO));
                            v128 longSum1 = Sse2.add_epi64(Sse2.unpackhi_epi32(count0, ZERO), Sse2.unpackhi_epi32(count1, ZERO));
                            v128 longSum2 = Sse2.add_epi64(Sse2.unpacklo_epi32(count2, ZERO), Sse2.unpacklo_epi32(count3, ZERO));
                            v128 longSum3 = Sse2.add_epi64(Sse2.unpackhi_epi32(count2, ZERO), Sse2.unpackhi_epi32(count3, ZERO));

                            longCount = Sse2.add_epi64(longCount, Sse2.add_epi64(Sse2.add_epi64(longSum0, longSum1), Sse2.add_epi64(longSum2, longSum3)));

                            count0 = Sse2.setzero_si128();
                            count1 = Sse2.setzero_si128();
                            count2 = Sse2.setzero_si128();
                            count3 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely(length >= 16))
                        {
                            do
                            {
                                count0 = Sse2.sub_epi32(count0, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi32(count1, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi32(count2, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi32(count3, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                length -= 16;
                            } 
                            while (Hint.Likely(length >= 16));

                            v128 longSum0 = Sse2.add_epi64(Sse2.unpacklo_epi32(count0, ZERO), Sse2.unpacklo_epi32(count1, ZERO));
                            v128 longSum1 = Sse2.add_epi64(Sse2.unpackhi_epi32(count0, ZERO), Sse2.unpackhi_epi32(count1, ZERO));
                            v128 longSum2 = Sse2.add_epi64(Sse2.unpacklo_epi32(count2, ZERO), Sse2.unpacklo_epi32(count3, ZERO));
                            v128 longSum3 = Sse2.add_epi64(Sse2.unpackhi_epi32(count2, ZERO), Sse2.unpackhi_epi32(count3, ZERO));

                            longCount = Sse2.add_epi64(longCount, Sse2.add_epi64(Sse2.add_epi64(longSum0, longSum1), Sse2.add_epi64(longSum2, longSum3)));

                            count0 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 4))
                            {
                                count0 = Sse2.sub_epi32(count0, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 4))
                                {
                                    count0 = Sse2.sub_epi32(count0, Compare.UInts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi32(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));
                        longCount = Sse2.bsrli_si128(longCount, 1 * sizeof(ulong));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.UInts128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }

                        count0 = Sse2.add_epi32(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        ulong countTotal = count0.UInt0 + longCount.ULong0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (Sse4_1.IsSse41Supported)
                            {
                                if (where == Comparison.NotEqualTo)
                                {
                                    countTotal = (uint)((ulong)originalLength - 1 - countTotal);
                                }
                            }
                            else
                            {  
                                if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                                {
                                    countTotal = (uint)((ulong)originalLength - 1 - countTotal);
                                }
                            }

                            bool comparison = Compare.UInts(*(uint*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (Sse4_1.IsSse41Supported)
                            {
                                if (where == Comparison.NotEqualTo)
                                {
                                    countTotal = (uint)((ulong)originalLength - countTotal);
                                }
                            }
                            else
                            {  
                                if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                                {
                                    countTotal = (uint)((ulong)originalLength - countTotal);
                                }
                            }
                        }

                        return countTotal;
                    }
                    else
                    {
                        ulong count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.UInts(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }

                default: return 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<uint> array, uint value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<uint> array, int index, uint value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<uint> array, int index, int numEntries, uint value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<uint> array, uint value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<uint> array, int index, uint value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<uint> array, int index, int numEntries, uint value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<uint> array, uint value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<uint> array, int index, uint value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<uint> array, int index, int numEntries, uint value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, TypeCode.Int32);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(ulong* ptr, long length, ulong value, Comparison where = Comparison.EqualTo)
        {
Assert.IsNonNegative(length);

            long originalLength = length;

            if (Avx2.IsAvx2Supported)
            {
                v256 broadcast = Avx.mm256_set1_epi64x((long)value);
                v256* ptr_v256 = (v256*)ptr;
                v256 count0 = default(v256);
                v256 count1 = default(v256);
                v256 count2 = default(v256);
                v256 count3 = default(v256);

                while (Hint.Likely(length >= 16))
                {
                    count0 = Avx2.mm256_sub_epi64(count0, Compare.ULongs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                    count1 = Avx2.mm256_sub_epi64(count1, Compare.ULongs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                    count2 = Avx2.mm256_sub_epi64(count2, Compare.ULongs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                    count3 = Avx2.mm256_sub_epi64(count3, Compare.ULongs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                    length -= 16;
                }

                count0 = Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(count0, count1), Avx2.mm256_add_epi64(count2, count3));

                if (Hint.Likely((int)length >= 4))
                {
                    count0 = Avx2.mm256_sub_epi64(count0, Compare.ULongs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        count0 = Avx2.mm256_sub_epi64(count0, Compare.ULongs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            count0 = Avx2.mm256_sub_epi64(count0, Compare.ULongs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                v128 count128 = Sse2.add_epi64(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                if (Hint.Likely((int)length >= 2))
                {
                    count128 = Sse2.sub_epi64(count128, Compare.ULongs128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 2;
                }
                else { }

                count128 = Sse2.add_epi64(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));
                ulong countTotal = count128.ULong0;

                if (Hint.Likely(length != 0))
                {
                    if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                    {
                        countTotal = (ulong)originalLength - 1 - countTotal; 
                    }

                    bool comparison = Compare.ULongs(*(ulong*)ptr_v256, value, where);
                    countTotal += *(byte*)&comparison;
                }
                else 
                {
                    if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                    {
                        countTotal = (ulong)originalLength - countTotal; 
                    }
                }


                return countTotal;
            }
            else if (Sse4_2.IsSse42Supported)
            {
                v128 broadcast = Sse2.set1_epi64x((long)value);
                v128* ptr_v128 = (v128*)ptr;
                v128 count0 = default(v128);
                v128 count1 = default(v128);
                v128 count2 = default(v128);
                v128 count3 = default(v128);

                while (Hint.Likely(length >= 8))
                {
                    count0 = Sse2.sub_epi64(count0, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                    count1 = Sse2.sub_epi64(count1, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                    count2 = Sse2.sub_epi64(count2, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                    count3 = Sse2.sub_epi64(count3, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                    
                    length -= 8;
                }

                count0 = Sse2.add_epi64(Sse2.add_epi64(count0, count1), Sse2.add_epi64(count2, count3));

                if (Hint.Likely((int)length >= 2))
                {
                    count0 = Sse2.sub_epi64(count0, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        count0 = Sse2.sub_epi64(count0, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            count0 = Sse2.sub_epi64(count0, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                count0 = Sse2.add_epi64(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));
                ulong countTotal = count0.ULong0;

                if (Hint.Likely(length != 0))
                {
                    if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                    {
                        countTotal = (ulong)originalLength - 1 - countTotal; 
                    }

                    bool comparison = Compare.ULongs(*(ulong*)ptr_v128, value, where);
                    countTotal += *(byte*)&comparison;
                }
                else 
                {
                    if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                    {
                        countTotal = (ulong)originalLength - countTotal; 
                    }
                }


                return countTotal;
            }
            else if (Sse4_1.IsSse41Supported)
            {
                if (where == Comparison.EqualTo || where == Comparison.NotEqualTo)
                {
                    v128 broadcast = Sse2.set1_epi64x((long)value);
                    v128* ptr_v128 = (v128*)ptr;
                    v128 count0 = default(v128);
                    v128 count1 = default(v128);
                    v128 count2 = default(v128);
                    v128 count3 = default(v128);

                    while (Hint.Likely(length >= 8))
                    {
                        count0 = Sse2.sub_epi64(count0, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                        count1 = Sse2.sub_epi64(count1, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                        count2 = Sse2.sub_epi64(count2, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                        count3 = Sse2.sub_epi64(count3, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                        
                        length -= 8;
                    }

                    count0 = Sse2.add_epi64(Sse2.add_epi64(count0, count1), Sse2.add_epi64(count2, count3));

                    if (Hint.Likely((int)length >= 2))
                    {
                        count0 = Sse2.sub_epi64(count0, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                        if (Hint.Likely((int)length >= 2 * 2))
                        {
                            count0 = Sse2.sub_epi64(count0, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 3 * 2))
                            {
                                count0 = Sse2.sub_epi64(count0, Compare.ULongs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                    count0 = Sse2.add_epi64(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));
                    ulong countTotal = count0.ULong0;

                    if (Hint.Likely(length != 0))
                    {
                        if (where == Comparison.NotEqualTo)
                        {
                            countTotal = (ulong)originalLength - 1 - countTotal; 
                        }

                        bool comparison = Compare.ULongs(*(ulong*)ptr_v128, value, where);
                        countTotal += *(byte*)&comparison;
                    }
                    else 
                    {
                        if (where == Comparison.NotEqualTo)
                        {
                            countTotal = (ulong)originalLength - countTotal; 
                        }
                    }


                    return countTotal;
                }
                else
                {
                    ulong count = 0;

                    for (long i = 0; i < length; i++)
                    {
                        bool comparison = Compare.ULongs(ptr[i], value, where);
                        count += *(byte*)&comparison;
                    }

                    return count;
                }
            }
            else
            {
                ulong count = 0;

                for (long i = 0; i < length; i++)
                {
                    bool comparison = Compare.ULongs(ptr[i], value, where);
                    count += *(byte*)&comparison;
                }

                return count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<ulong> array, ulong value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<ulong> array, int index, ulong value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<ulong> array, int index, int numEntries, ulong value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<ulong> array, ulong value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<ulong> array, int index, ulong value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<ulong> array, int index, int numEntries, ulong value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<ulong> array, ulong value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<ulong> array, int index, ulong value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<ulong> array, int index, int numEntries, ulong value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(sbyte* ptr, long length, sbyte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.UInt64)
        {
Assert.IsBetween((int)returnType, (int)TypeCode.SByte, (int)TypeCode.UInt64);
Assert.IsNonNegative(length);
            
            long originalLength = length;

            switch (returnType)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 ZERO = Avx.mm256_setzero_si256();

                        v256 broadcast = Avx.mm256_set1_epi8((byte)value);
                        v256* ptr_v256 = (v256*)ptr;

                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);
                        
                        while (Hint.Likely(length >= 4 * 32))
                        {
                            count0 = Avx2.mm256_sub_epi8(count0, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count1 = Avx2.mm256_sub_epi8(count1, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count2 = Avx2.mm256_sub_epi8(count2, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count3 = Avx2.mm256_sub_epi8(count3, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            length -= 4 * 32;
                        }

                        count0 = Avx2.mm256_add_epi8(Avx2.mm256_add_epi8(count0, count1), Avx2.mm256_add_epi8(count2, count3));
                        
                        if (Hint.Likely((int)length >= 32))
                        {
                            count0 = Avx2.mm256_sub_epi8(count0, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 32))
                            {
                                count0 = Avx2.mm256_sub_epi8(count0, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 32))
                                {
                                    count0 = Avx2.mm256_sub_epi8(count0, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi8(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                        if (Hint.Likely((int)length >= 16))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.SBytes128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 16;
                        }
                        else { }

                        count128 = Sse2.add_epi8(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.SBytes128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 8;
                        }
                        else { }

                        count128 = Sse2.add_epi8(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.SBytes128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }
                        
                        count128 = Sse2.add_epi8(count128, Sse2.shufflelo_epi16(count128, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.SBytes128(Sse2.insert_epi16(default(v128), *(ushort*)ptr_v256, 0), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((ushort*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }
                        
                        count128 = Sse2.add_epi8(count128, Sse2.bsrli_si128(count128, 1 * sizeof(sbyte)));
                        byte countTotal = count128.Byte0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (byte)((ulong)originalLength - 1 - countTotal);
                            }

                            bool comparison = Compare.SBytes(*(sbyte*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (byte)((ulong)originalLength - countTotal);
                            }
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 ZERO = Sse2.setzero_si128();
                        
                        v128* ptr_v128 = (v128*)ptr;

                        v128 broadcast = Sse2.set1_epi8((sbyte)value);
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);

                        while (Hint.Likely(length >= 4 * 16))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count1 = Sse2.sub_epi8(count1, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count2 = Sse2.sub_epi8(count2, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count3 = Sse2.sub_epi8(count3, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            length -= 4 * 16;
                        }

                        count0 = Sse2.add_epi8(Sse2.add_epi8(count0, count1), Sse2.add_epi8(count2, count3));

                        if (Hint.Likely((int)length >= 16))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi8(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 8;
                        }
                        else { }

                        count0 = Sse2.add_epi8(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                            length -= 4;
                        }
                        else { }
                        
                        count0 = Sse2.add_epi8(count0, Sse2.shufflelo_epi16(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.insert_epi16(ZERO, *(ushort*)ptr_v128, 0), broadcast, where));

                            ptr_v128 = (v128*)((ushort*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }
                        
                        count0 = Sse2.add_epi8(count0, Sse2.bsrli_si128(count0, 1 * sizeof(sbyte)));
                        byte countTotal = count0.Byte0;

                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (byte)((ulong)originalLength - 1 - countTotal);
                            }

                            bool comparison = Compare.SBytes(*(sbyte*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (byte)((ulong)originalLength - countTotal);
                            }
                        }

                        return countTotal;
                    }
                    else
                    {
                        byte count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.SBytes(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 ZERO = Avx.mm256_setzero_si256();

                        v256 broadcast = Avx.mm256_set1_epi8((byte)value);
                        v256* ptr_v256 = (v256*)ptr;

                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);
                        v256 longCount = default(v256);
                        
                        while (Hint.Likely(length >= 4 * 32 * byte.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Avx2.mm256_sub_epi8(count0, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi8(count1, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi8(count2, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi8(count3, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < byte.MaxValue));

                            length -= 4 * 32 * byte.MaxValue;
                            
                            v256 sum0 = Avx2.mm256_sad_epu8(ZERO, count0);
                            v256 sum1 = Avx2.mm256_sad_epu8(ZERO, count1);
                            v256 sum2 = Avx2.mm256_sad_epu8(ZERO, count2);
                            v256 sum3 = Avx2.mm256_sad_epu8(ZERO, count3);

                            sum0 = Avx2.mm256_add_epi64(sum0, sum1);
                            sum1 = Avx2.mm256_add_epi64(sum2, sum3);

                            longCount = Avx2.mm256_add_epi64(longCount, Avx2.mm256_add_epi64(sum0, sum1));

                            count0 = Avx.mm256_setzero_si256();
                            count1 = Avx.mm256_setzero_si256();
                            count2 = Avx.mm256_setzero_si256();
                            count3 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely(length >= 4 * 32))
                        {
                            do
                            {
                                count0 = Avx2.mm256_sub_epi8(count0, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi8(count1, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi8(count2, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi8(count3, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                length -= 4 * 32;
                            }
                            while (Hint.Likely(length >= 4 * 32));

                            v256 sum0 = Avx2.mm256_sad_epu8(ZERO, count0);
                            v256 sum1 = Avx2.mm256_sad_epu8(ZERO, count1);
                            v256 sum2 = Avx2.mm256_sad_epu8(ZERO, count2);
                            v256 sum3 = Avx2.mm256_sad_epu8(ZERO, count3);

                            sum0 = Avx2.mm256_add_epi64(sum0, sum1);
                            sum1 = Avx2.mm256_add_epi64(sum2, sum3);

                            longCount = Avx2.mm256_add_epi64(longCount, Avx2.mm256_add_epi64(sum0, sum1));

                            count0 = Avx.mm256_setzero_si256();
                        }
                        
                        if (Hint.Likely((int)length >= 32))
                        {
                            count0 = Avx2.mm256_sub_epi8(count0, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 32))
                            {
                                count0 = Avx2.mm256_sub_epi8(count0, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 32))
                                {
                                    count0 = Avx2.mm256_sub_epi8(count0, Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi8(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                        if (Hint.Likely((int)length >= 16))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.SBytes128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 16;
                        }
                        else { }

                        count128 = Sse2.add_epi8(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.SBytes128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 8;
                        }
                        else { }

                        v128 longCount128 = Sse2.add_epi64(Avx.mm256_castsi256_si128(longCount), Avx2.mm256_extracti128_si256(longCount, 1));
                        count128 = Sse2.add_epi8(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.SBytes128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }
                        
                        count128 = Sse2.add_epi8(count128, Sse2.shufflelo_epi16(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        longCount128 = Sse2.add_epi64(longCount128, Sse2.bsrli_si128(longCount128, 1 * sizeof(ulong)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi8(count128, Compare.SBytes128(Sse2.insert_epi16(default(v128), *(ushort*)ptr_v256, 0), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((ushort*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }
                        
                        count128 = Sse2.add_epi8(count128, Sse2.bsrli_si128(count128, 1 * sizeof(sbyte)));
                        ulong countTotal = Sse4_1.extract_epi8(count128, 0);
                        countTotal += (ulong)Sse2.cvtsi128_si64x(longCount128);

                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ulong)originalLength - 1 - countTotal;
                            }

                            bool comparison = Compare.SBytes(*(sbyte*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ulong)originalLength - countTotal;
                            }
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 ZERO = Sse2.setzero_si128();
                        
                        v128* ptr_v128 = (v128*)ptr;

                        v128 broadcast = Sse2.set1_epi8((sbyte)value);
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);
                        v128 longCount = default(v128);

                        while (Hint.Likely(length >= 4 * 16 * byte.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi8(count1, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi8(count2, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi8(count3, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < byte.MaxValue));

                            length -= 4 * 16 * byte.MaxValue;
                            
                            v128 sum0 = Sse2.sad_epu8(ZERO, count0);
                            v128 sum1 = Sse2.sad_epu8(ZERO, count1);
                            v128 sum2 = Sse2.sad_epu8(ZERO, count2);
                            v128 sum3 = Sse2.sad_epu8(ZERO, count3);

                            sum0 = Sse2.add_epi64(sum0, sum1);
                            sum1 = Sse2.add_epi64(sum2, sum3);

                            longCount = Sse2.add_epi64(longCount, Sse2.add_epi64(sum0, sum1));

                            count0 = Sse2.setzero_si128();
                            count1 = Sse2.setzero_si128();
                            count2 = Sse2.setzero_si128();
                            count3 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely(length >= 4 * 16))
                        {
                            do
                            {
                                count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi8(count1, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi8(count2, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi8(count3, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                length -= 4 * 16;
                            }
                            while (Hint.Likely(length >= 4 * 16));

                            v128 sum0 = Sse2.sad_epu8(ZERO, count0);
                            v128 sum1 = Sse2.sad_epu8(ZERO, count1);
                            v128 sum2 = Sse2.sad_epu8(ZERO, count2);
                            v128 sum3 = Sse2.sad_epu8(ZERO, count3);

                            sum0 = Sse2.add_epi64(sum0, sum1);
                            sum1 = Sse2.add_epi64(sum2, sum3);

                            longCount = Sse2.add_epi64(longCount, Sse2.add_epi64(sum0, sum1));

                            count0 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 16))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi8(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 8;
                        }
                        else { }

                        count0 = Sse2.add_epi8(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                            length -= 4;
                        }
                        else { }
                        
                        count0 = Sse2.add_epi8(count0, Sse2.shufflelo_epi16(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        longCount = Sse2.add_epi64(longCount, Sse2.bsrli_si128(longCount, 1 * sizeof(ulong)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi8(count0, Compare.SBytes128(Sse2.insert_epi16(ZERO, *(ushort*)ptr_v128, 0), broadcast, where));

                            ptr_v128 = (v128*)((ushort*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }
                        
                        count0 = Sse2.add_epi8(count0, Sse2.bsrli_si128(count0, 1 * sizeof(sbyte)));
                        ulong countTotal;

                        if (Sse4_1.IsSse41Supported)
                        {
                            countTotal = Sse4_1.extract_epi8(count0, 0);
                            countTotal += (ulong)Sse2.cvtsi128_si64x(longCount);
                        }
                        else
                        {
                            count0 = Sse2.and_si128(count0, Sse2.cvtsi32_si128(byte.MaxValue));
                            longCount = Sse2.add_epi64(longCount, count0);
                            countTotal = (ulong)Sse2.cvtsi128_si64x(longCount);
                        }

                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ulong)originalLength - 1 - countTotal;
                            }

                            bool comparison = Compare.SBytes(*(sbyte*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else 
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ulong)originalLength - countTotal;
                            }
                        }

                        return countTotal;
                    }
                    else
                    {
                        ulong count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.SBytes(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }

                default: return 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<sbyte> array, sbyte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<sbyte> array, int index, sbyte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<sbyte> array, int index, int numEntries, sbyte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<sbyte> array, sbyte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<sbyte> array, int index, sbyte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<sbyte> array, int index, int numEntries, sbyte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<sbyte> array, sbyte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<sbyte> array, int index, sbyte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<sbyte> array, int index, int numEntries, sbyte value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, returnType);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(short* ptr, long length, short value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.UInt64)
        {
Assert.IsBetween((int)returnType, (int)TypeCode.SByte, (int)TypeCode.UInt64);
Assert.IsNonNegative(length);

            long originalLength = length;

            switch (returnType)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 broadcast = Avx.mm256_set1_epi16(value);
                        v256* ptr_v256 = (v256*)ptr;
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);

                        while (Hint.Likely(length >= 4 * 16))
                        {
                            count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count1 = Avx2.mm256_sub_epi16(count1, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count2 = Avx2.mm256_sub_epi16(count2, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count3 = Avx2.mm256_sub_epi16(count3, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            length -= 4 * 16;
                        }

                        count0 = Avx2.mm256_add_epi16(count0, count1);
                        count2 = Avx2.mm256_add_epi16(count2, count3);
                        count0 = Avx2.mm256_add_epi16(count0, count2);

                        if (Hint.Likely((int)length >= 16))
                        {
                            count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi16(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.Shorts128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 8;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.Shorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.Shorts128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shufflelo_epi16(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        ushort countTotal = count128.UShort0;

                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ushort)((ulong)originalLength - 1 - countTotal);
                            }

                            bool comparison = Compare.Shorts(*(short*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ushort)((ulong)originalLength - countTotal);
                            } 
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 broadcast = Sse2.set1_epi16(value);
                        v128* ptr_v128 = (v128*)ptr;
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);

                        while (Hint.Likely(length >= 4 * 8))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count1 = Sse2.sub_epi16(count1, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count2 = Sse2.sub_epi16(count2, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count3 = Sse2.sub_epi16(count3, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            length -= 4 * 8;
                        }

                        count0 = Sse2.add_epi16(count0, count1);
                        count2 = Sse2.add_epi16(count2, count3);
                        count0 = Sse2.add_epi16(count0, count2);

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi16(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 4;
                        }
                        else { }

                        count0 = Sse2.add_epi16(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }

                        count0 = Sse2.add_epi16(count0, Sse2.shufflelo_epi16(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        ushort countTotal = count0.UShort0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ushort)((ulong)originalLength - 1 - countTotal);
                            }

                            bool comparison = Compare.Shorts(*(short*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ushort)((ulong)originalLength - countTotal);
                            } 
                        }

                        return countTotal;
                    }
                    else
                    {
                        ushort count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.Shorts(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }
                case TypeCode.Int32:
                case TypeCode.UInt32:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 ZERO = Avx.mm256_setzero_si256();
                        
                        v256 broadcast = Avx.mm256_set1_epi16(value);
                        v256* ptr_v256 = (v256*)ptr;
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);
                        v256 intCount = default(v256);

                        while (Hint.Likely(length >= 4 * 16 * ushort.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi16(count1, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi16(count2, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi16(count3, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < ushort.MaxValue));

                            length -= 4 * 16 * ushort.MaxValue;

                            v256 intSum0 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count0, ZERO), Avx2.mm256_unpacklo_epi16(count1, ZERO));
                            v256 intSum1 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count0, ZERO), Avx2.mm256_unpackhi_epi16(count1, ZERO));
                            v256 intSum2 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count2, ZERO), Avx2.mm256_unpacklo_epi16(count3, ZERO));
                            v256 intSum3 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count2, ZERO), Avx2.mm256_unpackhi_epi16(count3, ZERO));
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum1);
                            intSum2 = Avx2.mm256_add_epi32(intSum2, intSum3);
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum2);

                            intCount = Avx2.mm256_add_epi32(intCount, intSum0);

                            count0 = Avx.mm256_setzero_si256();
                            count1 = Avx.mm256_setzero_si256();
                            count2 = Avx.mm256_setzero_si256();
                            count3 = Avx.mm256_setzero_si256();
                        }
                        
                        if (Hint.Likely(length >= 4 * 16))
                        {
                            do
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi16(count1, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi16(count2, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi16(count3, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                length -= 4 * 16;
                            } 
                            while (Hint.Likely(length >= 4 * 16));

                            v256 intSum0 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count0, ZERO), Avx2.mm256_unpacklo_epi16(count1, ZERO));
                            v256 intSum1 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count0, ZERO), Avx2.mm256_unpackhi_epi16(count1, ZERO));
                            v256 intSum2 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count2, ZERO), Avx2.mm256_unpacklo_epi16(count3, ZERO));
                            v256 intSum3 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count2, ZERO), Avx2.mm256_unpackhi_epi16(count3, ZERO));
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum1);
                            intSum2 = Avx2.mm256_add_epi32(intSum2, intSum3);
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum2);

                            intCount = Avx2.mm256_add_epi32(intCount, intSum0);

                            count0 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely((int)length >= 16))
                        {
                            count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi16(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.Shorts128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 8;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));
                        v128 intCount128 = Sse2.add_epi32(Avx.mm256_castsi256_si128(intCount), Avx2.mm256_extracti128_si256(intCount, 1));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.Shorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }
                        
                        count128 = Sse2.add_epi16(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        intCount128 = Sse2.add_epi32(intCount128, Sse2.bsrli_si128(intCount128, 2 * sizeof(uint)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.Shorts128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shufflelo_epi16(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        count128 = Sse4_1.cvtepu16_epi32(count128);
                        intCount128 = Sse2.add_epi32(intCount128, Sse2.bsrli_si128(intCount128, 1 * sizeof(uint)));
                        uint countTotal = Sse2.add_epi32(intCount128, count128).UInt0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (uint)((ulong)originalLength - 1 - countTotal);
                            }

                            bool comparison = Compare.Shorts(*(short*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (uint)((ulong)originalLength - countTotal);
                            } 
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 ZERO = Sse2.setzero_si128();

                        v128 broadcast = Sse2.set1_epi16(value);
                        v128* ptr_v128 = (v128*)ptr;
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);
                        v128 intCount = default(v128);

                        while (Hint.Likely(length >= 4 * 8 * ushort.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi16(count1, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi16(count2, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi16(count3, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < ushort.MaxValue));

                            length -= 4 * 8 * ushort.MaxValue;

                            v128 intSum0 = Sse2.add_epi32(Sse2.unpacklo_epi16(count0, ZERO), Sse2.unpacklo_epi16(count1, ZERO));
                            v128 intSum1 = Sse2.add_epi32(Sse2.unpackhi_epi16(count0, ZERO), Sse2.unpackhi_epi16(count1, ZERO));
                            v128 intSum2 = Sse2.add_epi32(Sse2.unpacklo_epi16(count2, ZERO), Sse2.unpacklo_epi16(count3, ZERO));
                            v128 intSum3 = Sse2.add_epi32(Sse2.unpackhi_epi16(count2, ZERO), Sse2.unpackhi_epi16(count3, ZERO));

                            intSum0 = Sse2.add_epi32(intSum0, intSum1);
                            intSum2 = Sse2.add_epi32(intSum2, intSum3);
                            intSum0 = Sse2.add_epi32(intSum0, intSum2);

                            intCount = Sse2.add_epi32(intCount, intSum0);

                            count0 = Sse2.setzero_si128();
                            count1 = Sse2.setzero_si128();
                            count2 = Sse2.setzero_si128();
                            count3 = Sse2.setzero_si128();
                        }
                        
                        if (Hint.Likely(length >= 4 * 8))
                        {
                            do
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi16(count1, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi16(count2, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi16(count3, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                length -= 4 * 8;
                            } 
                            while (Hint.Likely(length >= 4 * 8));

                            v128 intSum0 = Sse2.add_epi32(Sse2.unpacklo_epi16(count0, ZERO), Sse2.unpacklo_epi16(count1, ZERO));
                            v128 intSum1 = Sse2.add_epi32(Sse2.unpackhi_epi16(count0, ZERO), Sse2.unpackhi_epi16(count1, ZERO));
                            v128 intSum2 = Sse2.add_epi32(Sse2.unpacklo_epi16(count2, ZERO), Sse2.unpacklo_epi16(count3, ZERO));
                            v128 intSum3 = Sse2.add_epi32(Sse2.unpackhi_epi16(count2, ZERO), Sse2.unpackhi_epi16(count3, ZERO));

                            intSum0 = Sse2.add_epi32(intSum0, intSum1);
                            intSum2 = Sse2.add_epi32(intSum2, intSum3);
                            intSum0 = Sse2.add_epi32(intSum0, intSum2);

                            intCount = Sse2.add_epi32(intCount, intSum0);

                            count0 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi16(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 4;
                        }
                        else { }

                        count0 = Sse2.add_epi16(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        intCount = Sse2.add_epi32(intCount, Sse2.bsrli_si128(intCount, 2 * sizeof(uint)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }

                        count0 = Sse2.add_epi16(count0, Sse2.shufflelo_epi16(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Sse4_1.IsSse41Supported)
                        {
                            count0 = Sse4_1.cvtepu16_epi32(count0);
                        }
                        else
                        {
                            count0 = Sse2.and_si128(count0, Sse2.cvtsi32_si128(ushort.MaxValue));
                        }

                        intCount = Sse2.add_epi32(intCount, Sse2.bsrli_si128(intCount, 1 * sizeof(uint)));
                        uint countTotal = Sse2.add_epi32(intCount, count0).UInt0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (uint)((ulong)originalLength - 1 - countTotal);
                            }

                            bool comparison = Compare.Shorts(*(short*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (uint)((ulong)originalLength - countTotal);
                            } 
                        }

                        return countTotal;
                    }
                    else
                    {
                        uint count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.Shorts(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }
                case TypeCode.Int64:
                case TypeCode.UInt64:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 ZERO = Avx.mm256_setzero_si256();
                        
                        v256 broadcast = Avx.mm256_set1_epi16(value);
                        v256* ptr_v256 = (v256*)ptr;
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);
                        v256 longCount = default(v256);

                        while (Hint.Likely(length >= 4 * 16 * ushort.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi16(count1, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi16(count2, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi16(count3, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < ushort.MaxValue));

                            length -= 4 * 16 * ushort.MaxValue;

                            v256 intSum0 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count0, ZERO), Avx2.mm256_unpacklo_epi16(count1, ZERO));
                            v256 intSum1 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count0, ZERO), Avx2.mm256_unpackhi_epi16(count1, ZERO));
                            v256 intSum2 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count2, ZERO), Avx2.mm256_unpacklo_epi16(count3, ZERO));
                            v256 intSum3 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count2, ZERO), Avx2.mm256_unpackhi_epi16(count3, ZERO));
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum1);
                            intSum2 = Avx2.mm256_add_epi32(intSum2, intSum3);
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum2);

                            v256 longSum = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(intSum0, ZERO), Avx2.mm256_unpackhi_epi32(intSum0, ZERO));
                            longCount = Avx2.mm256_add_epi64(longCount, longSum);

                            count0 = Avx.mm256_setzero_si256();
                            count1 = Avx.mm256_setzero_si256();
                            count2 = Avx.mm256_setzero_si256();
                            count3 = Avx.mm256_setzero_si256();
                        }
                        
                        if (Hint.Likely(length >= 4 * 16))
                        {
                            do
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi16(count1, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi16(count2, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi16(count3, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                length -= 4 * 16;
                            } 
                            while (Hint.Likely(length >= 4 * 16));

                            v256 intSum0 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count0, ZERO), Avx2.mm256_unpacklo_epi16(count1, ZERO));
                            v256 intSum1 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count0, ZERO), Avx2.mm256_unpackhi_epi16(count1, ZERO));
                            v256 intSum2 = Avx2.mm256_add_epi32(Avx2.mm256_unpacklo_epi16(count2, ZERO), Avx2.mm256_unpacklo_epi16(count3, ZERO));
                            v256 intSum3 = Avx2.mm256_add_epi32(Avx2.mm256_unpackhi_epi16(count2, ZERO), Avx2.mm256_unpackhi_epi16(count3, ZERO));
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum1);
                            intSum2 = Avx2.mm256_add_epi32(intSum2, intSum3);
                            intSum0 = Avx2.mm256_add_epi32(intSum0, intSum2);

                            v256 longSum = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(intSum0, ZERO), Avx2.mm256_unpackhi_epi32(intSum0, ZERO));
                            longCount = Avx2.mm256_add_epi64(longCount, longSum);

                            count0 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely((int)length >= 16))
                        {
                            count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 16))
                            {
                                count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 16))
                                {
                                    count0 = Avx2.mm256_sub_epi16(count0, Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi16(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.Shorts128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 8;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.Shorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }
                        
                        v128 longCount128 = Sse2.add_epi64(Avx.mm256_castsi256_si128(longCount), Avx2.mm256_extracti128_si256(longCount, 1));
                        count128 = Sse2.add_epi16(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi16(count128, Compare.Shorts128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        count128 = Sse2.add_epi16(count128, Sse2.shufflelo_epi16(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        count128 = Sse4_1.cvtepu16_epi64(count128);
                        longCount128 = Sse2.add_epi64(longCount128, Sse2.bsrli_si128(longCount128, 1 * sizeof(ulong)));
                        ulong countTotal = Sse2.add_epi64(longCount128, count128).ULong0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ulong)originalLength - 1 - countTotal;
                            }

                            bool comparison = Compare.Shorts(*(short*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ulong)originalLength - countTotal;
                            }
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 ZERO = Sse2.setzero_si128();

                        v128 broadcast = Sse2.set1_epi16(value);
                        v128* ptr_v128 = (v128*)ptr;
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);
                        v128 longCount = default(v128);

                        while (Hint.Likely(length >= 4 * 8 * ushort.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi16(count1, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi16(count2, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi16(count3, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < ushort.MaxValue));

                            length -= 4 * 8 * ushort.MaxValue;

                            v128 intSum0 = Sse2.add_epi32(Sse2.unpacklo_epi16(count0, ZERO), Sse2.unpacklo_epi16(count1, ZERO));
                            v128 intSum1 = Sse2.add_epi32(Sse2.unpackhi_epi16(count0, ZERO), Sse2.unpackhi_epi16(count1, ZERO));
                            v128 intSum2 = Sse2.add_epi32(Sse2.unpacklo_epi16(count2, ZERO), Sse2.unpacklo_epi16(count3, ZERO));
                            v128 intSum3 = Sse2.add_epi32(Sse2.unpackhi_epi16(count2, ZERO), Sse2.unpackhi_epi16(count3, ZERO));

                            intSum0 = Sse2.add_epi32(intSum0, intSum1);
                            intSum2 = Sse2.add_epi32(intSum2, intSum3);
                            intSum0 = Sse2.add_epi32(intSum0, intSum2);

                            v128 longSum = Sse2.add_epi64(Sse2.unpacklo_epi32(intSum0, ZERO), Sse2.unpackhi_epi32(intSum0, ZERO));
                            longCount = Sse2.add_epi64(longCount, longSum);

                            count0 = Sse2.setzero_si128();
                            count1 = Sse2.setzero_si128();
                            count2 = Sse2.setzero_si128();
                            count3 = Sse2.setzero_si128();
                        }
                        
                        if (Hint.Likely(length >= 4 * 8))
                        {
                            do
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi16(count1, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi16(count2, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi16(count3, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                length -= 4 * 8;
                            } 
                            while (Hint.Likely(length >= 4 * 8));

                            v128 intSum0 = Sse2.add_epi32(Sse2.unpacklo_epi16(count0, ZERO), Sse2.unpacklo_epi16(count1, ZERO));
                            v128 intSum1 = Sse2.add_epi32(Sse2.unpackhi_epi16(count0, ZERO), Sse2.unpackhi_epi16(count1, ZERO));
                            v128 intSum2 = Sse2.add_epi32(Sse2.unpacklo_epi16(count2, ZERO), Sse2.unpacklo_epi16(count3, ZERO));
                            v128 intSum3 = Sse2.add_epi32(Sse2.unpackhi_epi16(count2, ZERO), Sse2.unpackhi_epi16(count3, ZERO));

                            intSum0 = Sse2.add_epi32(intSum0, intSum1);
                            intSum2 = Sse2.add_epi32(intSum2, intSum3);
                            intSum0 = Sse2.add_epi32(intSum0, intSum2);

                            v128 longSum = Sse2.add_epi64(Sse2.unpacklo_epi32(intSum0, ZERO), Sse2.unpackhi_epi32(intSum0, ZERO));
                            longCount = Sse2.add_epi64(longCount, longSum);

                            count0 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi16(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 4;
                        }
                        else { }

                        count0 = Sse2.add_epi16(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi16(count0, Compare.Shorts128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }

                        count0 = Sse2.add_epi16(count0, Sse2.shufflelo_epi16(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        if (Sse4_1.IsSse41Supported)
                        {
                            count0 = Sse4_1.cvtepu16_epi64(count0);
                        }
                        else
                        {
                            count0 = Sse2.and_si128(count0, Sse2.cvtsi32_si128(ushort.MaxValue));
                        }

                        longCount = Sse2.add_epi64(longCount, Sse2.bsrli_si128(longCount, 1 * sizeof(ulong)));
                        ulong countTotal = Sse2.add_epi64(longCount, count0).ULong0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ulong)originalLength - 1 - countTotal;
                            }

                            bool comparison = Compare.Shorts(*(short*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ulong)originalLength - countTotal;
                            }
                        }

                        return countTotal;
                    }
                    else
                    {
                        ulong count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.Shorts(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }

                default: return 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<short> array, short value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<short> array, int index, short value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<short> array, int index, int numEntries, short value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<short> array, short value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<short> array, int index, short value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<short> array, int index, int numEntries, short value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<short> array, short value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
            return (int)SIMD_Count((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<short> array, int index, short value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<short> array, int index, int numEntries, short value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.Int32)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, returnType);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(int* ptr, long length, int value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.UInt64)
        {
Assert.IsBetween((int)returnType, (int)TypeCode.SByte, (int)TypeCode.UInt64);
Assert.IsNonNegative(length);

            long originalLength = length;
            
            switch (returnType)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 broadcast = Avx.mm256_set1_epi32(value);
                        v256* ptr_v256 = (v256*)ptr;
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);

                        while (Hint.Likely(length >= 32))
                        {
                            count0 = Avx2.mm256_sub_epi32(count0, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count1 = Avx2.mm256_sub_epi32(count1, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count2 = Avx2.mm256_sub_epi32(count2, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            count3 = Avx2.mm256_sub_epi32(count3, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            
                            length -= 32;
                        }

                        count0 = Avx2.mm256_add_epi32(Avx2.mm256_add_epi32(count0, count1), Avx2.mm256_add_epi32(count2, count3));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Avx2.mm256_sub_epi32(count0, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                count0 = Avx2.mm256_sub_epi32(count0, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    count0 = Avx2.mm256_sub_epi32(count0, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi32(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi32(count128, Compare.Ints128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }

                        count128 = Sse2.add_epi32(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi32(count128, Compare.Ints128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        count128 = Sse2.add_epi32(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        uint countTotal = count128.UInt0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (uint)((ulong)originalLength - 1 - countTotal);
                            }

                            bool comparison = Compare.Ints(*(int*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (uint)((ulong)originalLength - countTotal);
                            }
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 broadcast = Sse2.set1_epi32(value);
                        v128* ptr_v128 = (v128*)ptr;
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);

                        while (Hint.Likely(length >= 16))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count1 = Sse2.sub_epi32(count1, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count2 = Sse2.sub_epi32(count2, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                            count3 = Sse2.sub_epi32(count3, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            length -= 16;
                        }

                        count0 = Sse2.add_epi32(Sse2.add_epi32(count0, count1), Sse2.add_epi32(count2, count3));


                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 4))
                            {
                                count0 = Sse2.sub_epi32(count0, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 4))
                                {
                                    count0 = Sse2.sub_epi32(count0, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi32(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.Ints128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }

                        count0 = Sse2.add_epi32(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        uint countTotal = count0.UInt0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (uint)((ulong)originalLength - 1 - countTotal);
                            }

                            bool comparison = Compare.Ints(*(int*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (uint)((ulong)originalLength - countTotal);
                            }
                        }

                        return countTotal;
                    }
                    else
                    {
                        uint count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.Ints(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }
                case TypeCode.Int64:
                case TypeCode.UInt64:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 ZERO = default(v256);

                        v256 broadcast = Avx.mm256_set1_epi32(value);
                        v256* ptr_v256 = (v256*)ptr;
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);
                        v256 longCount = default(v256);

                        while (Hint.Likely(length >= 4L * 8 * uint.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Avx2.mm256_sub_epi32(count0, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi32(count1, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi32(count2, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi32(count3, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < uint.MaxValue));

                            v256 longSum0 = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(count0, ZERO), Avx2.mm256_unpacklo_epi32(count1, ZERO));
                            v256 longSum1 = Avx2.mm256_add_epi64(Avx2.mm256_unpackhi_epi32(count0, ZERO), Avx2.mm256_unpackhi_epi32(count1, ZERO));
                            v256 longSum2 = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(count2, ZERO), Avx2.mm256_unpacklo_epi32(count3, ZERO));
                            v256 longSum3 = Avx2.mm256_add_epi64(Avx2.mm256_unpackhi_epi32(count2, ZERO), Avx2.mm256_unpackhi_epi32(count3, ZERO));

                            length -= 4L * 8 * uint.MaxValue;

                            longCount = Avx2.mm256_add_epi64(longCount, Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(longSum0, longSum1), Avx2.mm256_add_epi64(longSum2, longSum3)));

                            count0 = Avx.mm256_setzero_si256();
                            count1 = Avx.mm256_setzero_si256();
                            count2 = Avx.mm256_setzero_si256();
                            count3 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely(length >= 32))
                        {
                            do
                            {
                                count0 = Avx2.mm256_sub_epi32(count0, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi32(count1, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi32(count2, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi32(count3, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                            } 
                            while (Hint.Likely(length >= 32));

                            v256 longSum0 = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(count0, ZERO), Avx2.mm256_unpacklo_epi32(count1, ZERO));
                            v256 longSum1 = Avx2.mm256_add_epi64(Avx2.mm256_unpackhi_epi32(count0, ZERO), Avx2.mm256_unpackhi_epi32(count1, ZERO));
                            v256 longSum2 = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(count2, ZERO), Avx2.mm256_unpacklo_epi32(count3, ZERO));
                            v256 longSum3 = Avx2.mm256_add_epi64(Avx2.mm256_unpackhi_epi32(count2, ZERO), Avx2.mm256_unpackhi_epi32(count3, ZERO));

                            length -= 32;

                            longCount = Avx2.mm256_add_epi64(longCount, Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(longSum0, longSum1), Avx2.mm256_add_epi64(longSum2, longSum3)));

                            count0 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Avx2.mm256_sub_epi32(count0, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                count0 = Avx2.mm256_sub_epi32(count0, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    count0 = Avx2.mm256_sub_epi32(count0, Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi32(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));
                        v128 longCount128 = Sse2.add_epi64(Avx.mm256_castsi256_si128(longCount), Avx2.mm256_extracti128_si256(longCount, 1));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi32(count128, Compare.Ints128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }

                        count128 = Sse2.add_epi32(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));
                        longCount128 = Sse2.add_epi64(longCount128, Sse2.bsrli_si128(longCount128, 1 * sizeof(ulong)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi32(count128, Compare.Ints128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        count128 = Sse2.add_epi32(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        ulong countTotal = count128.UInt0 + longCount128.ULong0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ulong)originalLength - 1 - countTotal;
                            }

                            bool comparison = Compare.Ints(*(int*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ulong)originalLength - countTotal;
                            }
                        }

                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 ZERO = default(v128);

                        v128 broadcast = Sse2.set1_epi32(value);
                        v128* ptr_v128 = (v128*)ptr;
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);
                        v128 longCount = default(v128);

                        if (Hint.Likely(length >= 4L * 4 * uint.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Sse2.sub_epi32(count0, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi32(count1, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi32(count2, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi32(count3, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < uint.MaxValue));

                            length -= 4L * 4 * uint.MaxValue;

                            v128 longSum0 = Sse2.add_epi64(Sse2.unpacklo_epi32(count0, ZERO), Sse2.unpacklo_epi32(count1, ZERO));
                            v128 longSum1 = Sse2.add_epi64(Sse2.unpackhi_epi32(count0, ZERO), Sse2.unpackhi_epi32(count1, ZERO));
                            v128 longSum2 = Sse2.add_epi64(Sse2.unpacklo_epi32(count2, ZERO), Sse2.unpacklo_epi32(count3, ZERO));
                            v128 longSum3 = Sse2.add_epi64(Sse2.unpackhi_epi32(count2, ZERO), Sse2.unpackhi_epi32(count3, ZERO));

                            longCount = Sse2.add_epi64(longCount, Sse2.add_epi64(Sse2.add_epi64(longSum0, longSum1), Sse2.add_epi64(longSum2, longSum3)));

                            count0 = Sse2.setzero_si128();
                            count1 = Sse2.setzero_si128();
                            count2 = Sse2.setzero_si128();
                            count3 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely(length >= 16))
                        {
                            do
                            {
                                count0 = Sse2.sub_epi32(count0, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi32(count1, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi32(count2, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi32(count3, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                length -= 16;
                            } 
                            while (Hint.Likely(length >= 16));

                            v128 longSum0 = Sse2.add_epi64(Sse2.unpacklo_epi32(count0, ZERO), Sse2.unpacklo_epi32(count1, ZERO));
                            v128 longSum1 = Sse2.add_epi64(Sse2.unpackhi_epi32(count0, ZERO), Sse2.unpackhi_epi32(count1, ZERO));
                            v128 longSum2 = Sse2.add_epi64(Sse2.unpacklo_epi32(count2, ZERO), Sse2.unpacklo_epi32(count3, ZERO));
                            v128 longSum3 = Sse2.add_epi64(Sse2.unpackhi_epi32(count2, ZERO), Sse2.unpackhi_epi32(count3, ZERO));

                            longCount = Sse2.add_epi64(longCount, Sse2.add_epi64(Sse2.add_epi64(longSum0, longSum1), Sse2.add_epi64(longSum2, longSum3)));

                            count0 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 4))
                            {
                                count0 = Sse2.sub_epi32(count0, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 4))
                                {
                                    count0 = Sse2.sub_epi32(count0, Compare.Ints128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi32(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));
                        longCount = Sse2.bsrli_si128(longCount, 1 * sizeof(ulong));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.Ints128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }

                        count0 = Sse2.add_epi32(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        ulong countTotal = count0.UInt0 + longCount.ULong0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ulong)originalLength - 1 - countTotal;
                            }

                            bool comparison = Compare.Ints(*(int*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else
                        { 
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                countTotal = (ulong)originalLength - countTotal;
                            }
                        }

                        return countTotal;
                    }
                    else
                    {
                        ulong count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.Ints(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }

                default: return 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<int> array, int value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<int> array, int index, int value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<int> array, int index, int numEntries, int value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<int> array, int value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<int> array, int index, int value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<int> array, int index, int numEntries, int value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<int> array, int value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<int> array, int index, int value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<int> array, int index, int numEntries, int value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, TypeCode.Int32);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(long* ptr, long length, long value, Comparison where = Comparison.EqualTo)
        {
Assert.IsNonNegative(length);

            long originalLength = length;

            if (Avx2.IsAvx2Supported)
            {
                v256 broadcast = Avx.mm256_set1_epi64x(value);
                v256* ptr_v256 = (v256*)ptr;
                v256 count0 = default(v256);
                v256 count1 = default(v256);
                v256 count2 = default(v256);
                v256 count3 = default(v256);

                while (Hint.Likely(length >= 16))
                {
                    count0 = Avx2.mm256_sub_epi64(count0, Compare.Longs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                    count1 = Avx2.mm256_sub_epi64(count1, Compare.Longs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                    count2 = Avx2.mm256_sub_epi64(count2, Compare.Longs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));
                    count3 = Avx2.mm256_sub_epi64(count3, Compare.Longs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                    length -= 16;
                }

                count0 = Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(count0, count1), Avx2.mm256_add_epi64(count2, count3));

                if (Hint.Likely((int)length >= 4))
                {
                    count0 = Avx2.mm256_sub_epi64(count0, Compare.Longs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        count0 = Avx2.mm256_sub_epi64(count0, Compare.Longs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            count0 = Avx2.mm256_sub_epi64(count0, Compare.Longs256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                v128 count128 = Sse2.add_epi64(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                if (Hint.Likely((int)length >= 2))
                {
                    count128 = Sse2.sub_epi64(count128, Compare.Longs128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    length -= 2;
                }
                else { }

                count128 = Sse2.add_epi64(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));
                ulong countTotal = count128.ULong0;

                if (Hint.Likely(length != 0))
                {
                    if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                    {
                        countTotal = (ulong)originalLength - 1 - countTotal;
                    }

                    bool comparison = Compare.Longs(*(long*)ptr_v256, value, where);
                    countTotal += *(byte*)&comparison;
                }
                else 
                { 
                    if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                    {
                        countTotal = (ulong)originalLength - countTotal;
                    }
                }


                return countTotal;
            }
            else if (Sse4_2.IsSse42Supported)
            {
                v128 broadcast = Sse2.set1_epi64x(value);
                v128* ptr_v128 = (v128*)ptr;
                v128 count0 = default(v128);
                v128 count1 = default(v128);
                v128 count2 = default(v128);
                v128 count3 = default(v128);

                while (Hint.Likely(length >= 8))
                {
                    count0 = Sse2.sub_epi64(count0, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                    count1 = Sse2.sub_epi64(count1, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                    count2 = Sse2.sub_epi64(count2, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                    count3 = Sse2.sub_epi64(count3, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                    
                    length -= 8;
                }

                count0 = Sse2.add_epi64(Sse2.add_epi64(count0, count1), Sse2.add_epi64(count2, count3));

                if (Hint.Likely((int)length >= 2))
                {
                    count0 = Sse2.sub_epi64(count0, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        count0 = Sse2.sub_epi64(count0, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            count0 = Sse2.sub_epi64(count0, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                count0 = Sse2.add_epi64(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));
                ulong countTotal = count0.ULong0;

                if (Hint.Likely(length != 0))
                {
                    if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                    {
                        countTotal = (ulong)originalLength - 1 - countTotal;
                    }

                    bool comparison = Compare.Longs(*(long*)ptr_v128, value, where);
                    countTotal += *(byte*)&comparison;
                }
                else 
                { 
                    if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                    {
                        countTotal = (ulong)originalLength - countTotal;
                    }
                }


                return countTotal;
            }
            else if (Sse4_1.IsSse41Supported)
            {
                if (where == Comparison.EqualTo || where == Comparison.NotEqualTo)
                {
                    v128 broadcast = Sse2.set1_epi64x(value);
                    v128* ptr_v128 = (v128*)ptr;
                    v128 count0 = default(v128);
                    v128 count1 = default(v128);
                    v128 count2 = default(v128);
                    v128 count3 = default(v128);

                    while (Hint.Likely(length >= 8))
                    {
                        count0 = Sse2.sub_epi64(count0, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                        count1 = Sse2.sub_epi64(count1, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                        count2 = Sse2.sub_epi64(count2, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                        count3 = Sse2.sub_epi64(count3, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));
                        
                        length -= 8;
                    }

                    count0 = Sse2.add_epi64(Sse2.add_epi64(count0, count1), Sse2.add_epi64(count2, count3));

                    if (Hint.Likely((int)length >= 2))
                    {
                        count0 = Sse2.sub_epi64(count0, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                        if (Hint.Likely((int)length >= 2 * 2))
                        {
                            count0 = Sse2.sub_epi64(count0, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 3 * 2))
                            {
                                count0 = Sse2.sub_epi64(count0, Compare.Longs128(Sse2.loadu_si128(ptr_v128++), broadcast, where));

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

                    count0 = Sse2.add_epi64(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));
                    ulong countTotal = count0.ULong0;

                    if (Hint.Likely(length != 0))
                    {
                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            countTotal = (ulong)originalLength - 1 - countTotal;
                        }

                        bool comparison = Compare.Longs(*(long*)ptr_v128, value, where);
                        countTotal += *(byte*)&comparison;
                    }
                    else
                    { 
                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            countTotal = (ulong)originalLength - countTotal;
                        }
                    }


                    return countTotal;
                }
                else
                {
                    ulong count = 0;

                    for (long i = 0; i < length; i++)
                    {
                        bool comparison = Compare.Longs(ptr[i], value, where);
                        count += *(byte*)&comparison;
                    }

                    return count;
                }
            }
            else
            {
                ulong count = 0;

                for (long i = 0; i < length; i++)
                {
                    bool comparison = Compare.Longs(ptr[i], value, where);
                    count += *(byte*)&comparison;
                }

                return count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<long> array, long value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<long> array, int index, long value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<long> array, int index, int numEntries, long value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<long> array, long value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<long> array, int index, long value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<long> array, int index, int numEntries, long value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<long> array, long value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<long> array, int index, long value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<long> array, int index, int numEntries, long value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(float* ptr, long length, float value, Comparison where = Comparison.EqualTo, TypeCode returnType = TypeCode.UInt64)
        {
Assert.IsBetween((int)returnType, (int)TypeCode.SByte, (int)TypeCode.UInt64);
Assert.IsNonNegative(length);
Assert.IsFalse(math.isnan(value));

            switch (returnType)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 broadcast = Avx.mm256_set1_ps(value);
                        v256* ptr_v256 = (v256*)ptr;
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);

                        while (Hint.Likely(length >= 32))
                        {
                            count0 = Avx2.mm256_sub_epi32(count0, Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256++), broadcast, where));
                            count1 = Avx2.mm256_sub_epi32(count1, Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256++), broadcast, where));
                            count2 = Avx2.mm256_sub_epi32(count2, Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256++), broadcast, where));
                            count3 = Avx2.mm256_sub_epi32(count3, Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256++), broadcast, where));
                            
                            length -= 32;
                        }

                        count0 = Avx2.mm256_add_epi32(Avx2.mm256_add_epi32(count0, count1), Avx2.mm256_add_epi32(count2, count3));

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Avx2.mm256_sub_epi32(count0, Compare.Floats256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                count0 = Avx2.mm256_sub_epi32(count0, Compare.Floats256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    count0 = Avx2.mm256_sub_epi32(count0, Compare.Floats256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi32(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi32(count128, Compare.Floats128(Sse.loadu_ps(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }

                        count128 = Sse2.add_epi32(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi32(count128, Compare.Floats128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        count128 = Sse2.add_epi32(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        uint countTotal = count128.UInt0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            bool comparison = Compare.Floats(*(float*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else { }


                        return countTotal;
                    }
                    else if (Avx.IsAvxSupported)
                    {
                        goto case TypeCode.UInt64;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 broadcast = Sse.set1_ps(value);
                        v128* ptr_v128 = (v128*)ptr;
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);

                        while (Hint.Likely(length >= 16))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                            count1 = Sse2.sub_epi32(count1, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                            count2 = Sse2.sub_epi32(count2, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                            count3 = Sse2.sub_epi32(count3, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                            
                            length -= 16;
                        }

                        count0 = Sse2.add_epi32(Sse2.add_epi32(count0, count1), Sse2.add_epi32(count2, count3));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 4))
                            {
                                count0 = Sse2.sub_epi32(count0, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 4))
                                {
                                    count0 = Sse2.sub_epi32(count0, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi32(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.Floats128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }

                        count0 = Sse2.add_epi32(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        uint countTotal = count0.UInt0;

                        if (Hint.Likely(length != 0))
                        {
                            bool comparison = Compare.Floats(*(float*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else { }


                        return countTotal;
                    }
                    else
                    {
                        uint count = 0;

                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.Floats(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }
                case TypeCode.Int64:
                case TypeCode.UInt64:
                {
                    if (Avx2.IsAvx2Supported)
                    {
                        v256 ZERO = default(v256);

                        v256 broadcast = Avx.mm256_set1_ps(value);
                        v256* ptr_v256 = (v256*)ptr;
                        v256 count0 = default(v256);
                        v256 count1 = default(v256);
                        v256 count2 = default(v256);
                        v256 count3 = default(v256);
                        v256 longCount = default(v256);


                        if (Hint.Likely(length >= 4L * 8 * uint.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Avx2.mm256_sub_epi32(count0, Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi32(count1, Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi32(count2, Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi32(count3, Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256++), broadcast, where));
                                
                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < uint.MaxValue));

                            length -= 4L * 8 * uint.MaxValue;

                            v256 longSum0 = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(count0, ZERO), Avx2.mm256_unpacklo_epi32(count1, ZERO));
                            v256 longSum1 = Avx2.mm256_add_epi64(Avx2.mm256_unpackhi_epi32(count0, ZERO), Avx2.mm256_unpackhi_epi32(count1, ZERO));
                            v256 longSum2 = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(count2, ZERO), Avx2.mm256_unpacklo_epi32(count3, ZERO));
                            v256 longSum3 = Avx2.mm256_add_epi64(Avx2.mm256_unpackhi_epi32(count2, ZERO), Avx2.mm256_unpackhi_epi32(count3, ZERO));

                            longCount = Avx2.mm256_add_epi64(longCount, Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(longSum0, longSum1), Avx2.mm256_add_epi64(longSum2, longSum3)));

                            count0 = Avx.mm256_setzero_si256();
                            count1 = Avx.mm256_setzero_si256();
                            count2 = Avx.mm256_setzero_si256();
                            count3 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely(length >= 32))
                        {
                            do
                            {
                                count0 = Avx2.mm256_sub_epi32(count0, Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256++), broadcast, where));
                                count1 = Avx2.mm256_sub_epi32(count1, Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256++), broadcast, where));
                                count2 = Avx2.mm256_sub_epi32(count2, Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256++), broadcast, where));
                                count3 = Avx2.mm256_sub_epi32(count3, Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256++), broadcast, where));
                                
                                length -= 32;
                            }
                            while (Hint.Likely(length >= 32));

                            v256 longSum0 = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(count0, ZERO), Avx2.mm256_unpacklo_epi32(count1, ZERO));
                            v256 longSum1 = Avx2.mm256_add_epi64(Avx2.mm256_unpackhi_epi32(count0, ZERO), Avx2.mm256_unpackhi_epi32(count1, ZERO));
                            v256 longSum2 = Avx2.mm256_add_epi64(Avx2.mm256_unpacklo_epi32(count2, ZERO), Avx2.mm256_unpacklo_epi32(count3, ZERO));
                            v256 longSum3 = Avx2.mm256_add_epi64(Avx2.mm256_unpackhi_epi32(count2, ZERO), Avx2.mm256_unpackhi_epi32(count3, ZERO));

                            longCount = Avx2.mm256_add_epi64(longCount, Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(longSum0, longSum1), Avx2.mm256_add_epi64(longSum2, longSum3)));

                            count0 = Avx.mm256_setzero_si256();
                        }

                        if (Hint.Likely((int)length >= 8))
                        {
                            count0 = Avx2.mm256_sub_epi32(count0, Compare.Floats256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                count0 = Avx2.mm256_sub_epi32(count0, Compare.Floats256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    count0 = Avx2.mm256_sub_epi32(count0, Compare.Floats256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where));

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

                        v128 count128 = Sse2.add_epi32(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));
                        v128 longCount128 = Sse2.add_epi64(Avx.mm256_castsi256_si128(longCount), Avx2.mm256_extracti128_si256(longCount, 1));

                        if (Hint.Likely((int)length >= 4))
                        {
                            count128 = Sse2.sub_epi32(count128, Compare.Floats128(Sse.loadu_ps(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }

                        count128 = Sse2.add_epi32(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));
                        longCount128 = Sse2.add_epi64(longCount128, Sse2.bsrli_si128(longCount128, 1 * sizeof(ulong)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count128 = Sse2.sub_epi32(count128, Compare.Floats128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        count128 = Sse2.add_epi32(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 0, 1)));
                        ulong countTotal = count128.UInt0 + longCount128.ULong0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            bool comparison = Compare.Floats(*(float*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else { }


                        return countTotal;
                    }
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

                            v256 cmpeq0 = Compare.Floats256(load0, broadcast, where);
                            v256 cmpeq1 = Compare.Floats256(load1, broadcast, where);
                            v256 cmpeq2 = Compare.Floats256(load2, broadcast, where);
                            v256 cmpeq3 = Compare.Floats256(load3, broadcast, where);

                            int mask0 = Avx.mm256_movemask_ps(cmpeq0);
                            int mask1 = Avx.mm256_movemask_ps(cmpeq1);
                            int mask2 = Avx.mm256_movemask_ps(cmpeq2);
                            int mask3 = Avx.mm256_movemask_ps(cmpeq3);

                            int tempCount0 = Popcnt.popcnt_u32((uint)mask0);
                            int tempCount1 = Popcnt.popcnt_u32((uint)mask1);
                            int tempCount2 = Popcnt.popcnt_u32((uint)mask2);
                            int tempCount3 = Popcnt.popcnt_u32((uint)mask3);

                            count0 += (uint)tempCount0;
                            count1 += (uint)tempCount1;
                            count2 += (uint)tempCount2;
                            count3 += (uint)tempCount3;
                        }

                        countTotal = (count0 + count1) + (count2 + count3);


                        if (Hint.Likely((int)length >= 8))
                        {
                            countTotal += (uint)Popcnt.popcnt_u32((uint)Avx.mm256_movemask_ps(Compare.Floats256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where)));

                            if (Hint.Likely((int)length >= 2 * 8))
                            {
                                countTotal += (uint)Popcnt.popcnt_u32((uint)Avx.mm256_movemask_ps(Compare.Floats256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where)));

                                if (Hint.Likely((int)length >= 3 * 8))
                                {
                                    countTotal += (uint)Popcnt.popcnt_u32((uint)Avx.mm256_movemask_ps(Compare.Floats256(Avx.mm256_loadu_si256(ptr_v256++), broadcast, where)));
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
                            countTotal += (uint)Popcnt.popcnt_u32((uint)Sse.movemask_ps(Compare.Floats128(Sse.loadu_ps(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where)));

                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 4;
                        }
                        else { }

                        if (Hint.Likely((int)length >= 2))
                        {
                            int mask = Sse.movemask_ps(Compare.Floats128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                            if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                            {
                                ;
                            }
                            else
                            {
                                mask &= 0b0011;
                            }

                            countTotal += (uint)Popcnt.popcnt_u32((uint)mask);

                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 2;
                        }
                        else { }

                        if (Hint.Likely(length != 0))
                        {
                            bool comparison = Compare.Floats(*(float*)ptr_v256, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else { }


                        return countTotal;
                    }
                    else if (Sse2.IsSse2Supported)
                    {
                        v128 ZERO = default(v128);

                        v128 broadcast = Sse.set1_ps(value);
                        v128* ptr_v128 = (v128*)ptr;
                        v128 count0 = default(v128);
                        v128 count1 = default(v128);
                        v128 count2 = default(v128);
                        v128 count3 = default(v128);
                        v128 longCount = default(v128);


                        if (Hint.Likely(length >= 4L * 4 * uint.MaxValue))
                        {
                            uint loopCounter = 0;

                            do
                            {
                                count0 = Sse2.sub_epi32(count0, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi32(count1, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi32(count2, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi32(count3, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                                
                                loopCounter++;
                            } 
                            while (Hint.Likely(loopCounter < uint.MaxValue));

                            length -= 4L * 4 * uint.MaxValue;

                            v128 longSum0 = Sse2.add_epi64(Sse2.unpacklo_epi32(count0, ZERO), Sse2.unpacklo_epi32(count1, ZERO));
                            v128 longSum1 = Sse2.add_epi64(Sse2.unpackhi_epi32(count0, ZERO), Sse2.unpackhi_epi32(count1, ZERO));
                            v128 longSum2 = Sse2.add_epi64(Sse2.unpacklo_epi32(count2, ZERO), Sse2.unpacklo_epi32(count3, ZERO));
                            v128 longSum3 = Sse2.add_epi64(Sse2.unpackhi_epi32(count2, ZERO), Sse2.unpackhi_epi32(count3, ZERO));

                            longCount = Sse2.add_epi64(longCount, Sse2.add_epi64(Sse2.add_epi64(longSum0, longSum1), Sse2.add_epi64(longSum2, longSum3)));

                            count0 = Sse2.setzero_si128();
                            count1 = Sse2.setzero_si128();
                            count2 = Sse2.setzero_si128();
                            count3 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely(length >= 16))
                        {
                            do
                            {
                                count0 = Sse2.sub_epi32(count0, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                                count1 = Sse2.sub_epi32(count1, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                                count2 = Sse2.sub_epi32(count2, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                                count3 = Sse2.sub_epi32(count3, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                                
                                length -= 16;
                            }
                            while (Hint.Likely(length >= 16));

                            v128 longSum0 = Sse2.add_epi64(Sse2.unpacklo_epi32(count0, ZERO), Sse2.unpacklo_epi32(count1, ZERO));
                            v128 longSum1 = Sse2.add_epi64(Sse2.unpackhi_epi32(count0, ZERO), Sse2.unpackhi_epi32(count1, ZERO));
                            v128 longSum2 = Sse2.add_epi64(Sse2.unpacklo_epi32(count2, ZERO), Sse2.unpacklo_epi32(count3, ZERO));
                            v128 longSum3 = Sse2.add_epi64(Sse2.unpackhi_epi32(count2, ZERO), Sse2.unpackhi_epi32(count3, ZERO));

                            longCount = Sse2.add_epi64(longCount, Sse2.add_epi64(Sse2.add_epi64(longSum0, longSum1), Sse2.add_epi64(longSum2, longSum3)));

                            count0 = Sse2.setzero_si128();
                        }

                        if (Hint.Likely((int)length >= 4))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));

                            if (Hint.Likely((int)length >= 2 * 4))
                            {
                                count0 = Sse2.sub_epi32(count0, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));

                                if (Hint.Likely((int)length >= 3 * 4))
                                {
                                    count0 = Sse2.sub_epi32(count0, Compare.Floats128(Sse.loadu_ps(ptr_v128++), broadcast, where));

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

                        count0 = Sse2.add_epi32(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));
                        longCount = Sse2.add_epi64(longCount, Sse2.bsrli_si128(longCount, 1 * sizeof(ulong)));

                        if (Hint.Likely((int)length >= 2))
                        {
                            count0 = Sse2.sub_epi32(count0, Compare.Floats128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 2;
                        }
                        else { }

                        count0 = Sse2.add_epi32(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 0, 1)));
                        ulong countTotal = count0.UInt0 + longCount.ULong0;
                        
                        if (Hint.Likely(length != 0))
                        {
                            bool comparison = Compare.Floats(*(float*)ptr_v128, value, where);
                            countTotal += *(byte*)&comparison;
                        }
                        else { }


                        return countTotal;
                    }
                    else
                    {
                        ulong count = 0;
                        for (long i = 0; i < length; i++)
                        {
                            bool comparison = Compare.Floats(ptr[i], value, where);
                            count += *(byte*)&comparison;
                        }

                        return count;
                    }
                }

                default: return 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<float> array, float value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<float> array, int index, float value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<float> array, int index, int numEntries, float value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<float> array, float value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<float> array, int index, float value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<float> array, int index, int numEntries, float value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<float> array, float value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<float> array, int index, float value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, TypeCode.Int32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<float> array, int index, int numEntries, float value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, TypeCode.Int32);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_Count(double* ptr, long length, double value, Comparison where = Comparison.EqualTo)
        {
Assert.IsNonNegative(length);
Assert.IsFalse(math.isnan(value));

            if (Avx2.IsAvx2Supported)
            {
                v256 broadcast = Avx.mm256_set1_pd(value);
                v256* ptr_v256 = (v256*)ptr;
                v256 count0 = default(v256);
                v256 count1 = default(v256);
                v256 count2 = default(v256);
                v256 count3 = default(v256);

                while (Hint.Likely(length >= 16))
                {
                    count0 = Avx2.mm256_sub_epi64(count0, Compare.Doubles256(Avx.mm256_loadu_pd(ptr_v256++), broadcast, where));
                    count1 = Avx2.mm256_sub_epi64(count1, Compare.Doubles256(Avx.mm256_loadu_pd(ptr_v256++), broadcast, where));
                    count2 = Avx2.mm256_sub_epi64(count2, Compare.Doubles256(Avx.mm256_loadu_pd(ptr_v256++), broadcast, where));
                    count3 = Avx2.mm256_sub_epi64(count3, Compare.Doubles256(Avx.mm256_loadu_pd(ptr_v256++), broadcast, where));
                    
                    length -= 16;
                }

                count0 = Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(count0, count1), Avx2.mm256_add_epi64(count2, count3));

                if (Hint.Likely((int)length >= 4))
                {
                    count0 = Avx2.mm256_sub_epi64(count0, Compare.Doubles256(Avx.mm256_loadu_pd(ptr_v256++), broadcast, where));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        count0 = Avx2.mm256_sub_epi64(count0, Compare.Doubles256(Avx.mm256_loadu_pd(ptr_v256++), broadcast, where));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            count0 = Avx2.mm256_sub_epi64(count0, Compare.Doubles256(Avx.mm256_loadu_pd(ptr_v256++), broadcast, where));

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

                v128 count128 = Sse2.add_epi64(Avx.mm256_castsi256_si128(count0), Avx2.mm256_extracti128_si256(count0, 1));

                if (Hint.Likely((int)length >= 2))
                {
                    count128 = Sse2.sub_epi64(count128, Compare.Doubles128(Sse.loadu_ps(ptr_v256), Avx.mm256_castpd256_pd128(broadcast), where));

                    length -= 2;
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                }
                else { }

                count128 = Sse2.add_epi64(count128, Sse2.shuffle_epi32(count128, Sse.SHUFFLE(0, 0, 3, 2)));
                ulong countTotal = count128.ULong0;

                if (Hint.Likely(length != 0))
                {
                    bool comparison = Compare.Doubles(*(double*)ptr_v256, value, where);
                    countTotal += *(byte*)&comparison;
                }
                else { }


                return countTotal;
            }
            else if (Avx.IsAvxSupported)
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

                    v256 cmpeq0 = Compare.Doubles256(load0, broadcast, where);
                    v256 cmpeq1 = Compare.Doubles256(load1, broadcast, where);
                    v256 cmpeq2 = Compare.Doubles256(load2, broadcast, where);
                    v256 cmpeq3 = Compare.Doubles256(load3, broadcast, where);

                    int mask0 = Avx.mm256_movemask_pd(cmpeq0);
                    int mask1 = Avx.mm256_movemask_pd(cmpeq1);
                    int mask2 = Avx.mm256_movemask_pd(cmpeq2);
                    int mask3 = Avx.mm256_movemask_pd(cmpeq3);

                    int tempCount0 = Popcnt.popcnt_u32((uint)mask0);
                    int tempCount1 = Popcnt.popcnt_u32((uint)mask1);
                    int tempCount2 = Popcnt.popcnt_u32((uint)mask2);
                    int tempCount3 = Popcnt.popcnt_u32((uint)mask3);

                    count0 += (uint)tempCount0;
                    count1 += (uint)tempCount1;
                    count2 += (uint)tempCount2;
                    count3 += (uint)tempCount3;
                }

                countTotal = (count0 + count1) + (count2 + count3);


                if (Hint.Likely((int)length >= 4))
                {
                    countTotal += (uint)Popcnt.popcnt_u32((uint)Avx.mm256_movemask_pd(Compare.Doubles256(Avx.mm256_loadu_pd(ptr_v256++), broadcast, where)));

                    if (Hint.Likely((int)length >= 2 * 4))
                    {
                        countTotal += (uint)Popcnt.popcnt_u32((uint)Avx.mm256_movemask_pd(Compare.Doubles256(Avx.mm256_loadu_pd(ptr_v256++), broadcast, where)));

                        if (Hint.Likely((int)length >= 3 * 4))
                        {
                            countTotal += (uint)Popcnt.popcnt_u32((uint)Avx.mm256_movemask_pd(Compare.Doubles256(Avx.mm256_loadu_pd(ptr_v256++), broadcast, where)));
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
                    countTotal += (uint)Popcnt.popcnt_u32((uint)Sse2.movemask_pd(Compare.Doubles128(Sse.loadu_ps(ptr_v256), Avx.mm256_castpd256_pd128(broadcast), where)));

                    length -= 2;
                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                }
                else { }

                if (Hint.Likely(length != 0))
                {
                    bool comparison = Compare.Doubles(*(double*)ptr_v256, value, where);
                    countTotal += *(byte*)&comparison;
                }
                else { }


                return countTotal;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128 broadcast = Sse2.set1_pd(value);
                v128* ptr_v128 = (v128*)ptr;
                v128 count0 = default(v128);
                v128 count1 = default(v128);
                v128 count2 = default(v128);
                v128 count3 = default(v128);

                while (Hint.Likely(length >= 8))
                {
                    count0 = Sse2.sub_epi64(count0, Compare.Doubles128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                    count1 = Sse2.sub_epi64(count1, Compare.Doubles128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                    count2 = Sse2.sub_epi64(count2, Compare.Doubles128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                    count3 = Sse2.sub_epi64(count3, Compare.Doubles128(Sse.loadu_ps(ptr_v128++), broadcast, where));
                    
                    length -= 8;
                }

                count0 = Sse2.add_epi64(Sse2.add_epi64(count0, count1), Sse2.add_epi64(count2, count3));

                if (Hint.Likely((int)length >= 2))
                {
                    count0 = Sse2.sub_epi64(count0, Compare.Doubles128(Sse.loadu_ps(ptr_v128++), broadcast, where));

                    if (Hint.Likely((int)length >= 2 * 2))
                    {
                        count0 = Sse2.sub_epi64(count0, Compare.Doubles128(Sse.loadu_ps(ptr_v128++), broadcast, where));

                        if (Hint.Likely((int)length >= 3 * 2))
                        {
                            count0 = Sse2.sub_epi64(count0, Compare.Doubles128(Sse.loadu_ps(ptr_v128++), broadcast, where));

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

                count0 = Sse2.add_epi64(count0, Sse2.shuffle_epi32(count0, Sse.SHUFFLE(0, 0, 3, 2)));
                ulong countTotal = count0.ULong0;

                if (Hint.Likely(length != 0))
                {
                    bool comparison = Compare.Doubles(*(double*)ptr_v128, value, where);
                    countTotal += *(byte*)&comparison;
                }
                else { }


                return countTotal;
            }
            else
            {
                ulong count = 0;

                for (long i = 0; i < length; i++)
                {
                    bool comparison = Compare.Doubles(ptr[i], value, where);
                    count += *(byte*)&comparison;
                }

                return count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<double> array, double value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<double> array, int index, double value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeArray<double> array, int index, int numEntries, double value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<double> array, double value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<double> array, int index, double value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeList<double> array, int index, int numEntries, double value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<double> array, double value, Comparison where = Comparison.EqualTo)
        {
            return (int)SIMD_Count((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<double> array, int index, double value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_Count((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_Count(this NativeSlice<double> array, int index, int numEntries, double value, Comparison where = Comparison.EqualTo)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_Count((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where);
        }
    }
}