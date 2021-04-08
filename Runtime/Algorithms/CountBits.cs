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
    // Wojciech Mula's algorithm
    unsafe public static partial class Algorithms
    {
        private static v256 SHUFFLE_MASK_AVX   => new v256(0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4,
                                                           0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4);
        private static v128 SHUFFLE_MASK_Ssse3 => new v128(0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4);

        private static v256 NIBBLE_MASK_AVX => Avx.mm256_set1_epi8(0x0F);
        private static v128 NIBBLE_MASK_Ssse3 => Sse2.set1_epi8(0x0F);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static v256 Popcnt_AVX(v256 vector, v256 SHUFFLE_MASK, v256 NIBBLE_MASK) 
        {
            if (Avx2.IsAvx2Supported)
            {
                v256 countLo = Avx2.mm256_shuffle_epi8(SHUFFLE_MASK, Avx2.mm256_and_si256(vector, NIBBLE_MASK));
                v256 countHi = Avx2.mm256_shuffle_epi8(SHUFFLE_MASK, Avx2.mm256_and_si256(Avx2.mm256_srli_epi16(vector, 4), NIBBLE_MASK));

                return Avx2.mm256_add_epi8(countLo, countHi);
            }
            else
            {
                return default(v256);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static v128 Popcnt_Ssse3(v128 vector, v128 SHUFFLE_MASK, v128 NIBBLE_MASK)
        {
            if (Ssse3.IsSsse3Supported)
            {
                v128 countLo = Ssse3.shuffle_epi8(SHUFFLE_MASK, Sse2.and_si128(vector, NIBBLE_MASK));
                v128 countHi = Ssse3.shuffle_epi8(SHUFFLE_MASK, Sse2.and_si128(Sse2.srli_epi16(vector, 4), NIBBLE_MASK));

                return Sse2.add_epi8(countLo, countHi);
            }
            else
            {
                return default(v128);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong SIMD_CountBitsWithMask<T>(void* ptr, long length, BitwiseOperation operation, T operand)
            where T : unmanaged
        {
Assert.IsNonNegative(length);
Assert.IsNotGreater(length * sizeof(T), long.MaxValue);

            if (Avx2.IsAvx2Supported)
            {
                long bytes = length * sizeof(T);
                v256 mask = sizeof(T) == 1 ? Avx.mm256_set1_epi8(*(byte*)&operand)   :
                            sizeof(T) == 2 ? Avx.mm256_set1_epi16(*(short*)&operand) :
                            sizeof(T) == 4 ? Avx.mm256_set1_epi32(*(int*)&operand)   :
                                             Avx.mm256_set1_epi64x(*(long*)&operand);

                v256 ALL_ONES = Avx2.mm256_cmpeq_epi32(default(v256), default(v256));

                v256 SHUFFLE_MASK = SHUFFLE_MASK_AVX;
                v256 NIBBLE_MASK = NIBBLE_MASK_AVX;
                v256 ZERO = default(v256);

                v256 longSum = default(v256);
                v256 sum = default(v256);
                v256* ptr_v256 = (v256*)ptr;


                while (bytes >= 31 * 32)
                {
                    sum = operation == BitwiseOperation.AND    ? Popcnt_AVX(Avx2.mm256_and_si256(Avx.mm256_loadu_si256(ptr_v256++), mask), SHUFFLE_MASK, NIBBLE_MASK)                                 :
                          operation == BitwiseOperation.OR     ? Popcnt_AVX(Avx2.mm256_or_si256(Avx.mm256_loadu_si256(ptr_v256++), mask), SHUFFLE_MASK, NIBBLE_MASK)                                  :
                          operation == BitwiseOperation.XOR    ? Popcnt_AVX(Avx2.mm256_xor_si256(Avx.mm256_loadu_si256(ptr_v256++), mask), SHUFFLE_MASK, NIBBLE_MASK)                                 :
                          operation == BitwiseOperation.ANDNOT ? Popcnt_AVX(Avx2.mm256_andnot_si256(Avx.mm256_loadu_si256(ptr_v256++), mask), SHUFFLE_MASK, NIBBLE_MASK)                              :
                          operation == BitwiseOperation.ORNOT  ? Popcnt_AVX(Avx2.mm256_or_si256(Avx2.mm256_xor_si256(ALL_ONES, Avx.mm256_loadu_si256(ptr_v256++)), mask), SHUFFLE_MASK, NIBBLE_MASK)  :
                          operation == BitwiseOperation.NAND   ? Popcnt_AVX(Avx2.mm256_xor_si256(ALL_ONES, Avx2.mm256_and_si256(Avx.mm256_loadu_si256(ptr_v256++), mask)), SHUFFLE_MASK, NIBBLE_MASK) :
                          operation == BitwiseOperation.NOR    ? Popcnt_AVX(Avx2.mm256_xor_si256(ALL_ONES, Avx2.mm256_or_si256(Avx.mm256_loadu_si256(ptr_v256++), mask)), SHUFFLE_MASK, NIBBLE_MASK)  :
                          operation == BitwiseOperation.XNOR   ? Popcnt_AVX(Avx2.mm256_xor_si256(ALL_ONES, Avx2.mm256_xor_si256(Avx.mm256_loadu_si256(ptr_v256++), mask)), SHUFFLE_MASK, NIBBLE_MASK) :
                          throw new System.Exception($"Internal error: Invalid bitwise operation { operation }.");

                    for (int i = 1; i < byte.MaxValue / 8; i++)
                    {
                        sum = Avx2.mm256_add_epi8(sum, operation == BitwiseOperation.AND    ? Popcnt_AVX(Avx2.mm256_and_si256(Avx.mm256_loadu_si256(ptr_v256++), mask), SHUFFLE_MASK, NIBBLE_MASK)                                 :
                                                       operation == BitwiseOperation.OR     ? Popcnt_AVX(Avx2.mm256_or_si256(Avx.mm256_loadu_si256(ptr_v256++), mask), SHUFFLE_MASK, NIBBLE_MASK)                                  :
                                                       operation == BitwiseOperation.XOR    ? Popcnt_AVX(Avx2.mm256_xor_si256(Avx.mm256_loadu_si256(ptr_v256++), mask), SHUFFLE_MASK, NIBBLE_MASK)                                 :
                                                       operation == BitwiseOperation.ANDNOT ? Popcnt_AVX(Avx2.mm256_andnot_si256(Avx.mm256_loadu_si256(ptr_v256++), mask), SHUFFLE_MASK, NIBBLE_MASK)                              :
                                                       operation == BitwiseOperation.ORNOT  ? Popcnt_AVX(Avx2.mm256_or_si256(Avx2.mm256_xor_si256(ALL_ONES, Avx.mm256_loadu_si256(ptr_v256++)), mask), SHUFFLE_MASK, NIBBLE_MASK)  :
                                                       operation == BitwiseOperation.NAND   ? Popcnt_AVX(Avx2.mm256_xor_si256(ALL_ONES, Avx2.mm256_and_si256(Avx.mm256_loadu_si256(ptr_v256++), mask)), SHUFFLE_MASK, NIBBLE_MASK) :
                                                       operation == BitwiseOperation.NOR    ? Popcnt_AVX(Avx2.mm256_xor_si256(ALL_ONES, Avx2.mm256_or_si256(Avx.mm256_loadu_si256(ptr_v256++), mask)), SHUFFLE_MASK, NIBBLE_MASK)  :
                                                       operation == BitwiseOperation.XNOR   ? Popcnt_AVX(Avx2.mm256_xor_si256(ALL_ONES, Avx2.mm256_xor_si256(Avx.mm256_loadu_si256(ptr_v256++), mask)), SHUFFLE_MASK, NIBBLE_MASK) :
                                                       throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));
                    }

                    longSum = Avx2.mm256_add_epi64(longSum, Avx2.mm256_sad_epu8(sum, ZERO));
                    bytes -= 31 * 32;
                }


                sum = ZERO;


                for (int i = 0; i < 30; i++)
                {
                    if (Hint.Likely(bytes >= 32))
                    {
                        sum = Avx2.mm256_add_epi8(sum, operation == BitwiseOperation.AND    ? Popcnt_AVX(Avx2.mm256_and_si256(Avx.mm256_loadu_si256(ptr_v256++), mask), SHUFFLE_MASK, NIBBLE_MASK)                                 :
                                                       operation == BitwiseOperation.OR     ? Popcnt_AVX(Avx2.mm256_or_si256(Avx.mm256_loadu_si256(ptr_v256++), mask), SHUFFLE_MASK, NIBBLE_MASK)                                  :
                                                       operation == BitwiseOperation.XOR    ? Popcnt_AVX(Avx2.mm256_xor_si256(Avx.mm256_loadu_si256(ptr_v256++), mask), SHUFFLE_MASK, NIBBLE_MASK)                                 :
                                                       operation == BitwiseOperation.ANDNOT ? Popcnt_AVX(Avx2.mm256_andnot_si256(Avx.mm256_loadu_si256(ptr_v256++), mask), SHUFFLE_MASK, NIBBLE_MASK)                              :
                                                       operation == BitwiseOperation.ORNOT  ? Popcnt_AVX(Avx2.mm256_or_si256(Avx2.mm256_xor_si256(ALL_ONES, Avx.mm256_loadu_si256(ptr_v256++)), mask), SHUFFLE_MASK, NIBBLE_MASK)  :
                                                       operation == BitwiseOperation.NAND   ? Popcnt_AVX(Avx2.mm256_xor_si256(ALL_ONES, Avx2.mm256_and_si256(Avx.mm256_loadu_si256(ptr_v256++), mask)), SHUFFLE_MASK, NIBBLE_MASK) :
                                                       operation == BitwiseOperation.NOR    ? Popcnt_AVX(Avx2.mm256_xor_si256(ALL_ONES, Avx2.mm256_or_si256(Avx.mm256_loadu_si256(ptr_v256++), mask)), SHUFFLE_MASK, NIBBLE_MASK)  :
                                                       operation == BitwiseOperation.XNOR   ? Popcnt_AVX(Avx2.mm256_xor_si256(ALL_ONES, Avx2.mm256_xor_si256(Avx.mm256_loadu_si256(ptr_v256++), mask)), SHUFFLE_MASK, NIBBLE_MASK) :
                                                       throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));

                        bytes -= 32;
                    }
                    else break;
                }


                longSum = Avx2.mm256_add_epi64(longSum, Avx2.mm256_sad_epu8(sum, ZERO));
                v128 csum = Sse2.add_epi64(Avx.mm256_castsi256_si128(longSum), Avx2.mm256_extracti128_si256(longSum, 1));

                if (Hint.Likely(bytes >= 16))
                {
                    v128 popcnt = operation == BitwiseOperation.AND    ? Popcnt_Ssse3(Sse2.and_si128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(mask)), Avx.mm256_castsi256_si128(SHUFFLE_MASK), Avx.mm256_castsi256_si128(NIBBLE_MASK))                                                      :
                                  operation == BitwiseOperation.OR     ? Popcnt_Ssse3(Sse2.or_si128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(mask)), Avx.mm256_castsi256_si128(SHUFFLE_MASK), Avx.mm256_castsi256_si128(NIBBLE_MASK))                                                       :
                                  operation == BitwiseOperation.XOR    ? Popcnt_Ssse3(Sse2.xor_si128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(mask)), Avx.mm256_castsi256_si128(SHUFFLE_MASK), Avx.mm256_castsi256_si128(NIBBLE_MASK))                                                      :
                                  operation == BitwiseOperation.ANDNOT ? Popcnt_Ssse3(Sse2.andnot_si128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(mask)), Avx.mm256_castsi256_si128(SHUFFLE_MASK), Avx.mm256_castsi256_si128(NIBBLE_MASK))                                                   :
                                  operation == BitwiseOperation.ORNOT  ? Popcnt_Ssse3(Sse2.or_si128(Sse2.xor_si128(Avx.mm256_castsi256_si128(ALL_ONES), Sse2.loadu_si128(ptr_v256)), Avx.mm256_castsi256_si128(mask)), Avx.mm256_castsi256_si128(SHUFFLE_MASK), Avx.mm256_castsi256_si128(NIBBLE_MASK))  :
                                  operation == BitwiseOperation.NAND   ? Popcnt_Ssse3(Sse2.xor_si128(Avx.mm256_castsi256_si128(ALL_ONES), Sse2.and_si128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(mask))), Avx.mm256_castsi256_si128(SHUFFLE_MASK), Avx.mm256_castsi256_si128(NIBBLE_MASK)) :
                                  operation == BitwiseOperation.NOR    ? Popcnt_Ssse3(Sse2.xor_si128(Avx.mm256_castsi256_si128(ALL_ONES), Sse2.or_si128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(mask))), Avx.mm256_castsi256_si128(SHUFFLE_MASK), Avx.mm256_castsi256_si128(NIBBLE_MASK))  :
                                  operation == BitwiseOperation.XNOR   ? Popcnt_Ssse3(Sse2.xor_si128(Avx.mm256_castsi256_si128(ALL_ONES), Sse2.xor_si128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(mask))), Avx.mm256_castsi256_si128(SHUFFLE_MASK), Avx.mm256_castsi256_si128(NIBBLE_MASK)) :
                                  throw new System.Exception($"Internal error: Invalid bitwise operation { operation }.");

                    csum = Sse2.add_epi64(csum, Sse2.sad_epu8(popcnt, Avx.mm256_castsi256_si128(ZERO)));

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    bytes -= 16;
                }

                csum = Sse2.add_epi64(csum, Sse2.shuffle_epi32(csum, Sse.SHUFFLE(0, 0, 3, 2)));
                ulong bits = csum.ULong0;
                ulong maskScalar = mask.ULong0;


                if (Hint.Likely(bytes >= 8))
                {
                    bits += (uint)math.countbits(operation == BitwiseOperation.AND    ? *(ulong*)ptr_v256 & maskScalar    :
                                                 operation == BitwiseOperation.OR     ? *(ulong*)ptr_v256 | maskScalar    :
                                                 operation == BitwiseOperation.XOR    ? *(ulong*)ptr_v256 ^ maskScalar    :
                                                 operation == BitwiseOperation.ANDNOT ? Bmi1.IsBmi1Supported ? Bmi1.andn_u64(*(ulong*)ptr_v256, maskScalar) : ~*(ulong*)ptr_v256 & maskScalar :
                                                 operation == BitwiseOperation.ORNOT  ? ~*(ulong*)ptr_v256 | maskScalar   :
                                                 operation == BitwiseOperation.NAND   ? ~(*(ulong*)ptr_v256 & maskScalar) :
                                                 operation == BitwiseOperation.NOR    ? ~(*(ulong*)ptr_v256 | maskScalar) :
                                                 operation == BitwiseOperation.XNOR   ? ~(*(ulong*)ptr_v256 ^ maskScalar) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));

                    ptr_v256 = (v256*)((ulong*)ptr_v256 + 1);
                    bytes -= 8;
                }


                if (Hint.Likely(bytes >= 4))
                {
                    bits += (uint)math.countbits(operation == BitwiseOperation.AND    ? *(uint*)ptr_v256 & (uint)maskScalar    :
                                                 operation == BitwiseOperation.OR     ? *(uint*)ptr_v256 | (uint)maskScalar    :
                                                 operation == BitwiseOperation.XOR    ? *(uint*)ptr_v256 ^ (uint)maskScalar    :
                                                 operation == BitwiseOperation.ANDNOT ? Bmi1.IsBmi1Supported ? Bmi1.andn_u32(*(uint*)ptr_v256, (uint)maskScalar) : ~*(uint*)ptr_v256 & (uint)maskScalar :
                                                 operation == BitwiseOperation.ORNOT  ? ~*(uint*)ptr_v256 | (uint)maskScalar   :
                                                 operation == BitwiseOperation.NAND   ? ~(*(uint*)ptr_v256 & (uint)maskScalar) :
                                                 operation == BitwiseOperation.NOR    ? ~(*(uint*)ptr_v256 | (uint)maskScalar) :
                                                 operation == BitwiseOperation.XNOR   ? ~(*(uint*)ptr_v256 ^ (uint)maskScalar) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));

                    ptr_v256 = (v256*)((uint*)ptr_v256 + 1);
                    bytes -= 4;
                }


                if (Hint.Likely(bytes >= 2))
                {
                    bits += (uint)math.countbits(operation == BitwiseOperation.AND    ? (uint)(*(ushort*)ptr_v256 & (ushort)maskScalar)    :
                                                 operation == BitwiseOperation.OR     ? (uint)(*(ushort*)ptr_v256 | (ushort)maskScalar)    :
                                                 operation == BitwiseOperation.XOR    ? (uint)(*(ushort*)ptr_v256 ^ (ushort)maskScalar)    :
                                                 operation == BitwiseOperation.ANDNOT ? (uint)(Bmi1.IsBmi1Supported ? Bmi1.andn_u32(*(ushort*)ptr_v256, (ushort)maskScalar) : ~(uint)*(ushort*)ptr_v256 & (uint)(ushort)maskScalar) :
                                                 operation == BitwiseOperation.ORNOT  ? (uint)(0x0000_FFFF & (~*(ushort*)ptr_v256 | (ushort)maskScalar))   :
                                                 operation == BitwiseOperation.NAND   ? (uint)(0x0000_FFFF & (~(*(ushort*)ptr_v256 & (ushort)maskScalar))) :
                                                 operation == BitwiseOperation.NOR    ? (uint)(0x0000_FFFF & (~(*(ushort*)ptr_v256 | (ushort)maskScalar))) :
                                                 operation == BitwiseOperation.XNOR   ? (uint)(0x0000_FFFF & (~(*(ushort*)ptr_v256 ^ (ushort)maskScalar))) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));

                    ptr_v256 = (v256*)((ushort*)ptr_v256 + 1);
                    bytes -= 2;
                }


                if (Hint.Likely(bytes != 0))
                {
                    bits += (uint)math.countbits(operation == BitwiseOperation.AND    ? (uint)(*(byte*)ptr_v256 & (byte)maskScalar)    :
                                                 operation == BitwiseOperation.OR     ? (uint)(*(byte*)ptr_v256 | (byte)maskScalar)    :
                                                 operation == BitwiseOperation.XOR    ? (uint)(*(byte*)ptr_v256 ^ (byte)maskScalar)    :
                                                 operation == BitwiseOperation.ANDNOT ? (uint)(Bmi1.IsBmi1Supported ? Bmi1.andn_u32(*(byte*)ptr_v256, (byte)maskScalar) : ~(uint)*(byte*)ptr_v256 & (uint)(byte)maskScalar) :
                                                 operation == BitwiseOperation.ORNOT  ? (uint)(0x0000_00FF & (~*(byte*)ptr_v256 | (byte)maskScalar))   :
                                                 operation == BitwiseOperation.NAND   ? (uint)(0x0000_00FF & (~(*(byte*)ptr_v256 & (byte)maskScalar))) :
                                                 operation == BitwiseOperation.NOR    ? (uint)(0x0000_00FF & (~(*(byte*)ptr_v256 | (byte)maskScalar))) :
                                                 operation == BitwiseOperation.XNOR   ? (uint)(0x0000_00FF & (~(*(byte*)ptr_v256 ^ (byte)maskScalar))) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));
                }


                return bits;
            }
            else if (Ssse3.IsSsse3Supported)
            {
                long bytes = length * sizeof(T);
                v128 mask = sizeof(T) == 1 ? Sse2.set1_epi8(*(sbyte*)&operand)  :
                            sizeof(T) == 2 ? Sse2.set1_epi16(*(short*)&operand) :
                            sizeof(T) == 4 ? Sse2.set1_epi32(*(int*)&operand)   :
                                             Sse2.set1_epi64x(*(long*)&operand);

                v128 ALL_ONES = Sse2.cmpeq_epi32(default(v128), default(v128));

                v128 SHUFFLE_MASK = SHUFFLE_MASK_Ssse3;
                v128 NIBBLE_MASK = NIBBLE_MASK_Ssse3;
                v128 ZERO = default(v128);

                v128 longSum = default(v128);
                v128 sum = default(v128);
                v128* ptr_v128 = (v128*)ptr;


                while (bytes >= 31 * 16)
                {
                    sum = operation == BitwiseOperation.AND    ? Popcnt_Ssse3(Sse2.and_si128(Sse2.loadu_si128(ptr_v128++), mask), SHUFFLE_MASK, NIBBLE_MASK)                           :
                          operation == BitwiseOperation.OR     ? Popcnt_Ssse3(Sse2.or_si128(Sse2.loadu_si128(ptr_v128++), mask), SHUFFLE_MASK, NIBBLE_MASK)                            :
                          operation == BitwiseOperation.XOR    ? Popcnt_Ssse3(Sse2.xor_si128(Sse2.loadu_si128(ptr_v128++), mask), SHUFFLE_MASK, NIBBLE_MASK)                           :
                          operation == BitwiseOperation.ANDNOT ? Popcnt_Ssse3(Sse2.andnot_si128(Sse2.loadu_si128(ptr_v128++), mask), SHUFFLE_MASK, NIBBLE_MASK)                        :
                          operation == BitwiseOperation.ORNOT  ? Popcnt_Ssse3(Sse2.or_si128(Sse2.xor_si128(ALL_ONES, Sse2.loadu_si128(ptr_v128++)), mask), SHUFFLE_MASK, NIBBLE_MASK)  :
                          operation == BitwiseOperation.NAND   ? Popcnt_Ssse3(Sse2.xor_si128(ALL_ONES, Sse2.and_si128(Sse2.loadu_si128(ptr_v128++), mask)), SHUFFLE_MASK, NIBBLE_MASK) :
                          operation == BitwiseOperation.NOR    ? Popcnt_Ssse3(Sse2.xor_si128(ALL_ONES, Sse2.or_si128(Sse2.loadu_si128(ptr_v128++), mask)), SHUFFLE_MASK, NIBBLE_MASK)  :
                          operation == BitwiseOperation.XNOR   ? Popcnt_Ssse3(Sse2.xor_si128(ALL_ONES, Sse2.xor_si128(Sse2.loadu_si128(ptr_v128++), mask)), SHUFFLE_MASK, NIBBLE_MASK) :
                          throw new System.Exception($"Internal error: Invalid bitwise operation { operation }.");

                    for (int i = 1; i < byte.MaxValue / 8; i++)
                    {
                        sum = Sse2.add_epi8(sum, operation == BitwiseOperation.AND    ? Popcnt_Ssse3(Sse2.and_si128(Sse2.loadu_si128(ptr_v128++), mask), SHUFFLE_MASK, NIBBLE_MASK)                           :
                                                 operation == BitwiseOperation.OR     ? Popcnt_Ssse3(Sse2.or_si128(Sse2.loadu_si128(ptr_v128++), mask), SHUFFLE_MASK, NIBBLE_MASK)                            :
                                                 operation == BitwiseOperation.XOR    ? Popcnt_Ssse3(Sse2.xor_si128(Sse2.loadu_si128(ptr_v128++), mask), SHUFFLE_MASK, NIBBLE_MASK)                           :
                                                 operation == BitwiseOperation.ANDNOT ? Popcnt_Ssse3(Sse2.andnot_si128(Sse2.loadu_si128(ptr_v128++), mask), SHUFFLE_MASK, NIBBLE_MASK)                        :
                                                 operation == BitwiseOperation.ORNOT  ? Popcnt_Ssse3(Sse2.or_si128(Sse2.xor_si128(ALL_ONES, Sse2.loadu_si128(ptr_v128++)), mask), SHUFFLE_MASK, NIBBLE_MASK)  :
                                                 operation == BitwiseOperation.NAND   ? Popcnt_Ssse3(Sse2.xor_si128(ALL_ONES, Sse2.and_si128(Sse2.loadu_si128(ptr_v128++), mask)), SHUFFLE_MASK, NIBBLE_MASK) :
                                                 operation == BitwiseOperation.NOR    ? Popcnt_Ssse3(Sse2.xor_si128(ALL_ONES, Sse2.or_si128(Sse2.loadu_si128(ptr_v128++), mask)), SHUFFLE_MASK, NIBBLE_MASK)  :
                                                 operation == BitwiseOperation.XNOR   ? Popcnt_Ssse3(Sse2.xor_si128(ALL_ONES, Sse2.xor_si128(Sse2.loadu_si128(ptr_v128++), mask)), SHUFFLE_MASK, NIBBLE_MASK) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));
                    }

                    longSum = Sse2.add_epi64(longSum, Sse2.sad_epu8(sum, ZERO));
                    bytes -= 31 * 16;
                }


                sum = ZERO;


                for (int i = 0; i < 30; i++)
                {
                    if (Hint.Likely(bytes >= 16))
                    {
                        sum = Sse2.add_epi8(sum, operation == BitwiseOperation.AND    ? Popcnt_Ssse3(Sse2.and_si128(Sse2.loadu_si128(ptr_v128++), mask), SHUFFLE_MASK, NIBBLE_MASK)                           :
                                                 operation == BitwiseOperation.OR     ? Popcnt_Ssse3(Sse2.or_si128(Sse2.loadu_si128(ptr_v128++), mask), SHUFFLE_MASK, NIBBLE_MASK)                            :
                                                 operation == BitwiseOperation.XOR    ? Popcnt_Ssse3(Sse2.xor_si128(Sse2.loadu_si128(ptr_v128++), mask), SHUFFLE_MASK, NIBBLE_MASK)                           :
                                                 operation == BitwiseOperation.ANDNOT ? Popcnt_Ssse3(Sse2.andnot_si128(Sse2.loadu_si128(ptr_v128++), mask), SHUFFLE_MASK, NIBBLE_MASK)                        :
                                                 operation == BitwiseOperation.ORNOT  ? Popcnt_Ssse3(Sse2.or_si128(Sse2.xor_si128(ALL_ONES, Sse2.loadu_si128(ptr_v128++)), mask), SHUFFLE_MASK, NIBBLE_MASK)  :
                                                 operation == BitwiseOperation.NAND   ? Popcnt_Ssse3(Sse2.xor_si128(ALL_ONES, Sse2.and_si128(Sse2.loadu_si128(ptr_v128++), mask)), SHUFFLE_MASK, NIBBLE_MASK) :
                                                 operation == BitwiseOperation.NOR    ? Popcnt_Ssse3(Sse2.xor_si128(ALL_ONES, Sse2.or_si128(Sse2.loadu_si128(ptr_v128++), mask)), SHUFFLE_MASK, NIBBLE_MASK)  :
                                                 operation == BitwiseOperation.XNOR   ? Popcnt_Ssse3(Sse2.xor_si128(ALL_ONES, Sse2.xor_si128(Sse2.loadu_si128(ptr_v128++), mask)), SHUFFLE_MASK, NIBBLE_MASK) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));

                        bytes -= 16;
                    }
                    else break;
                }


                longSum = Sse2.add_epi64(longSum, Sse2.sad_epu8(sum, ZERO));

                v128 csum = Sse2.add_epi64(longSum, Sse2.shuffle_epi32(longSum, Sse.SHUFFLE(0, 0, 3, 2)));
                ulong bits = csum.ULong0;
                ulong maskScalar = mask.ULong0;


                if (Hint.Likely(bytes >= 8))
                {
                    bits += (uint)math.countbits(operation == BitwiseOperation.AND    ? *(ulong*)ptr_v128 & maskScalar    :
                                                 operation == BitwiseOperation.OR     ? *(ulong*)ptr_v128 | maskScalar    :
                                                 operation == BitwiseOperation.XOR    ? *(ulong*)ptr_v128 ^ maskScalar    :
                                                 operation == BitwiseOperation.ANDNOT ? Bmi1.IsBmi1Supported ? Bmi1.andn_u64(*(ulong*)ptr_v128, maskScalar) : ~*(ulong*)ptr_v128 & maskScalar :
                                                 operation == BitwiseOperation.ORNOT  ? ~*(ulong*)ptr_v128 | maskScalar   :
                                                 operation == BitwiseOperation.NAND   ? ~(*(ulong*)ptr_v128 & maskScalar) :
                                                 operation == BitwiseOperation.NOR    ? ~(*(ulong*)ptr_v128 | maskScalar) :
                                                 operation == BitwiseOperation.XNOR   ? ~(*(ulong*)ptr_v128 ^ maskScalar) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));

                    ptr_v128 = (v128*)((ulong*)ptr_v128 + 1);
                    bytes -= 8;
                }


                if (Hint.Likely(bytes >= 4))
                {
                    bits += (uint)math.countbits(operation == BitwiseOperation.AND    ? *(uint*)ptr_v128 & (uint)maskScalar    :
                                                 operation == BitwiseOperation.OR     ? *(uint*)ptr_v128 | (uint)maskScalar    :
                                                 operation == BitwiseOperation.XOR    ? *(uint*)ptr_v128 ^ (uint)maskScalar    :
                                                 operation == BitwiseOperation.ANDNOT ? Bmi1.IsBmi1Supported ? Bmi1.andn_u32(*(uint*)ptr_v128, (uint)maskScalar) : ~*(uint*)ptr_v128 & (uint)maskScalar :
                                                 operation == BitwiseOperation.ORNOT  ? ~*(uint*)ptr_v128 | (uint)maskScalar   :
                                                 operation == BitwiseOperation.NAND   ? ~(*(uint*)ptr_v128 & (uint)maskScalar) :
                                                 operation == BitwiseOperation.NOR    ? ~(*(uint*)ptr_v128 | (uint)maskScalar) :
                                                 operation == BitwiseOperation.XNOR   ? ~(*(uint*)ptr_v128 ^ (uint)maskScalar) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));

                    ptr_v128 = (v128*)((uint*)ptr_v128 + 1);
                    bytes -= 4;
                }


                if (Hint.Likely(bytes >= 2))
                {
                    bits += (uint)math.countbits(operation == BitwiseOperation.AND    ? (uint)(*(ushort*)ptr_v128 & (ushort)maskScalar)    :
                                                 operation == BitwiseOperation.OR     ? (uint)(*(ushort*)ptr_v128 | (ushort)maskScalar)    :
                                                 operation == BitwiseOperation.XOR    ? (uint)(*(ushort*)ptr_v128 ^ (ushort)maskScalar)    :
                                                 operation == BitwiseOperation.ANDNOT ? (uint)(Bmi1.IsBmi1Supported ? Bmi1.andn_u32(*(ushort*)ptr_v128, (ushort)maskScalar) : ~(uint)*(ushort*)ptr_v128 & (uint)(ushort)maskScalar) :
                                                 operation == BitwiseOperation.ORNOT  ? (uint)(0x0000_FFFF & (~*(ushort*)ptr_v128 | (ushort)maskScalar))   :
                                                 operation == BitwiseOperation.NAND   ? (uint)(0x0000_FFFF & (~(*(ushort*)ptr_v128 & (ushort)maskScalar))) :
                                                 operation == BitwiseOperation.NOR    ? (uint)(0x0000_FFFF & (~(*(ushort*)ptr_v128 | (ushort)maskScalar))) :
                                                 operation == BitwiseOperation.XNOR   ? (uint)(0x0000_FFFF & (~(*(ushort*)ptr_v128 ^ (ushort)maskScalar))) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));

                    ptr_v128 = (v128*)((ushort*)ptr_v128 + 1);
                    bytes -= 2;
                }


                if (Hint.Likely(bytes != 0))
                {
                    bits += (uint)math.countbits(operation == BitwiseOperation.AND    ? (uint)(*(byte*)ptr_v128 & (byte)maskScalar)    :
                                                 operation == BitwiseOperation.OR     ? (uint)(*(byte*)ptr_v128 | (byte)maskScalar)    :
                                                 operation == BitwiseOperation.XOR    ? (uint)(*(byte*)ptr_v128 ^ (byte)maskScalar)    :
                                                 operation == BitwiseOperation.ANDNOT ? (uint)(Bmi1.IsBmi1Supported ? Bmi1.andn_u32(*(byte*)ptr_v128, (byte)maskScalar) : ~(uint)*(byte*)ptr_v128 & (uint)(byte)maskScalar) :
                                                 operation == BitwiseOperation.ORNOT  ? (uint)(0x0000_00FF & (~*(byte*)ptr_v128 | (byte)maskScalar))   :
                                                 operation == BitwiseOperation.NAND   ? (uint)(0x0000_00FF & (~(*(byte*)ptr_v128 & (byte)maskScalar))) :
                                                 operation == BitwiseOperation.NOR    ? (uint)(0x0000_00FF & (~(*(byte*)ptr_v128 | (byte)maskScalar))) :
                                                 operation == BitwiseOperation.XNOR   ? (uint)(0x0000_00FF & (~(*(byte*)ptr_v128 ^ (byte)maskScalar))) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));
                }


                return bits;
            }
            else
            {
                long bytes = length * sizeof(T);
                ulong mask = 0;
                ulong bits = 0;

                switch (sizeof(T))
                {
                    case 1:
                    {
                        uint x = *(byte*)&operand;
                        uint xx = x | (x << 8);
                        uint xxxx = xx | (xx << 16);
                        mask = xxxx | ((ulong)xxxx << 32);

                        break;
                    }
                    case 2:
                    {
                        uint x = *(ushort*)&operand;
                        uint xx = x | (x << 16);
                        mask = xx | ((ulong)xx << 32);
                    
                        break;
                    }
                    case 4:
                    {
                        uint x = *(uint*)&operand;
                        mask = x | ((ulong)x << 32);
                    
                        break;
                    }
                    case 8:
                    {
                        mask = *(ulong*)&operand;
                    
                        break;
                    }
                }


                while (Hint.Likely(bytes >= 8))
                {
                    bits += (uint)math.countbits(operation == BitwiseOperation.AND    ? *(ulong*)ptr & mask    :
                                                 operation == BitwiseOperation.OR     ? *(ulong*)ptr | mask    :
                                                 operation == BitwiseOperation.XOR    ? *(ulong*)ptr ^ mask    :
                                                 operation == BitwiseOperation.ANDNOT ? ~*(ulong*)ptr & mask   :
                                                 operation == BitwiseOperation.ORNOT  ? ~*(ulong*)ptr | mask   :
                                                 operation == BitwiseOperation.NAND   ? ~(*(ulong*)ptr & mask) :
                                                 operation == BitwiseOperation.NOR    ? ~(*(ulong*)ptr | mask) :
                                                 operation == BitwiseOperation.XNOR   ? ~(*(ulong*)ptr ^ mask) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));

                    ptr = (ulong*)ptr + 1;
                    bytes -= 8;
                }


                if (Hint.Likely(bytes >= 4))
                {
                    bits += (uint)math.countbits(operation == BitwiseOperation.AND    ? *(uint*)ptr & (uint)mask    :
                                                 operation == BitwiseOperation.OR     ? *(uint*)ptr | (uint)mask    :
                                                 operation == BitwiseOperation.XOR    ? *(uint*)ptr ^ (uint)mask    :
                                                 operation == BitwiseOperation.ANDNOT ? ~*(uint*)ptr & (uint)mask   :
                                                 operation == BitwiseOperation.ORNOT  ? ~*(uint*)ptr | (uint)mask   :
                                                 operation == BitwiseOperation.NAND   ? ~(*(uint*)ptr & (uint)mask) :
                                                 operation == BitwiseOperation.NOR    ? ~(*(uint*)ptr | (uint)mask) :
                                                 operation == BitwiseOperation.XNOR   ? ~(*(uint*)ptr ^ (uint)mask) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));

                    ptr = (uint*)ptr + 1;
                    bytes -= 4;
                }


                if (Hint.Likely(bytes >= 2))
                {
                    bits += (uint)math.countbits(operation == BitwiseOperation.AND    ? (uint)(*(ushort*)ptr & (ushort)mask)    :
                                                 operation == BitwiseOperation.OR     ? (uint)(*(ushort*)ptr | (ushort)mask)    :
                                                 operation == BitwiseOperation.XOR    ? (uint)(*(ushort*)ptr ^ (ushort)mask)    :
                                                 operation == BitwiseOperation.ANDNOT ? (uint)(~(uint)*(ushort*)ptr & (uint)(ushort)mask)       :
                                                 operation == BitwiseOperation.ORNOT  ? (uint)(0x0000_FFFF & (~*(ushort*)ptr | (ushort)mask))   :
                                                 operation == BitwiseOperation.NAND   ? (uint)(0x0000_FFFF & (~(*(ushort*)ptr & (ushort)mask))) :
                                                 operation == BitwiseOperation.NOR    ? (uint)(0x0000_FFFF & (~(*(ushort*)ptr | (ushort)mask))) :
                                                 operation == BitwiseOperation.XNOR   ? (uint)(0x0000_FFFF & (~(*(ushort*)ptr ^ (ushort)mask))) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));

                    ptr = (ushort*)ptr + 1;
                    bytes -= 2;
                }


                if (Hint.Likely(bytes != 0))
                {
                    bits += (uint)math.countbits(operation == BitwiseOperation.AND    ? (uint)(*(byte*)ptr & (byte)mask)    :
                                                 operation == BitwiseOperation.OR     ? (uint)(*(byte*)ptr | (byte)mask)    :
                                                 operation == BitwiseOperation.XOR    ? (uint)(*(byte*)ptr ^ (byte)mask)    :
                                                 operation == BitwiseOperation.ANDNOT ? (uint)(~(uint)*(byte*)ptr & (uint)(byte)mask)       :
                                                 operation == BitwiseOperation.ORNOT  ? (uint)(0x0000_00FF & (~*(byte*)ptr | (byte)mask))   :
                                                 operation == BitwiseOperation.NAND   ? (uint)(0x0000_00FF & (~(*(byte*)ptr & (byte)mask))) :
                                                 operation == BitwiseOperation.NOR    ? (uint)(0x0000_00FF & (~(*(byte*)ptr | (byte)mask))) :
                                                 operation == BitwiseOperation.XNOR   ? (uint)(0x0000_00FF & (~(*(byte*)ptr ^ (byte)mask))) :
                                                 throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));
                }


                return bits;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(void* ptr, long bytes)
        {
Assert.IsNonNegative(bytes);

            if (Avx2.IsAvx2Supported)
            {
                v256 SHUFFLE_MASK = SHUFFLE_MASK_AVX;
                v256 NIBBLE_MASK = NIBBLE_MASK_AVX;
                v256 ZERO = default(v256);

                v256 longSum = default(v256);
                v256 sum;
                v256* ptr_v256 = (v256*)ptr;


                while (bytes >= 31 * 32)
                {
                    sum = Popcnt_AVX(Avx.mm256_loadu_si256(ptr_v256++), SHUFFLE_MASK, NIBBLE_MASK);

                    for (int i = 1; i < byte.MaxValue / 8; i++)
                    {
                        sum = Avx2.mm256_add_epi8(sum, Popcnt_AVX(Avx.mm256_loadu_si256(ptr_v256++), SHUFFLE_MASK, NIBBLE_MASK));
                    }

                    longSum = Avx2.mm256_add_epi64(longSum, Avx2.mm256_sad_epu8(sum, ZERO));
                    bytes -= 31 * 32;
                }


                sum = ZERO;


                for (int i = 0; i < 30; i++)
                {
                    if (Hint.Likely(bytes >= 32))
                    {
                        sum = Avx2.mm256_add_epi8(sum, Popcnt_AVX(Avx.mm256_loadu_si256(ptr_v256++), SHUFFLE_MASK, NIBBLE_MASK));
                        bytes -= 32;
                    }
                    else break;
                }


                longSum = Avx2.mm256_add_epi64(longSum, Avx2.mm256_sad_epu8(sum, ZERO));
                v128 csum = Sse2.add_epi64(Avx.mm256_castsi256_si128(longSum), Avx2.mm256_extracti128_si256(longSum, 1));

                if (Hint.Likely(bytes >= 16))
                {
                    v128 popcnt = Popcnt_Ssse3(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(SHUFFLE_MASK), Avx.mm256_castsi256_si128(NIBBLE_MASK));
                    csum = Sse2.add_epi64(csum, Sse2.sad_epu8(popcnt, Avx.mm256_castsi256_si128(ZERO)));

                    ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    bytes -= 16;
                }

                csum = Sse2.add_epi64(csum, Sse2.shuffle_epi32(csum, Sse.SHUFFLE(0, 0, 3, 2)));
                ulong bits = csum.ULong0;


                if (Hint.Likely(bytes >= 8))
                {
                    bits += (uint)math.countbits(*(ulong*)ptr_v256);

                    ptr_v256 = (v256*)((ulong*)ptr_v256 + 1);
                    bytes -= 8;
                }


                if (Hint.Likely(bytes >= 4))
                {
                    bits += (uint)math.countbits(*(uint*)ptr_v256);

                    ptr_v256 = (v256*)((uint*)ptr_v256 + 1);
                    bytes -= 4;
                }


                if (Hint.Likely(bytes >= 2))
                {
                    bits += (uint)math.countbits((uint)*((ushort*)ptr_v256));

                    ptr_v256 = (v256*)((ushort*)ptr_v256 + 1);
                    bytes -= 2;
                }


                if (Hint.Likely(bytes != 0))
                {
                    bits += (uint)math.countbits((uint)*((byte*)ptr_v256));
                }


                return bits;
            }
            else if (Ssse3.IsSsse3Supported)
            {
                v128 SHUFFLE_MASK = SHUFFLE_MASK_Ssse3;
                v128 NIBBLE_MASK = NIBBLE_MASK_Ssse3;
                v128 ZERO = default(v128);

                v128 longSum= default(v128);
                v128 sum;
                v128* ptr_v128 = (v128*)ptr;


                while (bytes >= 31 * 16)
                {
                    sum = Popcnt_Ssse3(Sse2.loadu_si128(ptr_v128++), SHUFFLE_MASK, NIBBLE_MASK);

                    for (int i = 1; i < byte.MaxValue / 8; i++)
                    {
                        sum = Sse2.add_epi64(sum, Popcnt_Ssse3(Sse2.loadu_si128(ptr_v128++), SHUFFLE_MASK, NIBBLE_MASK));
                    }


                    longSum = Sse2.add_epi64(longSum, Sse2.sad_epu8(ZERO, sum));
                    bytes -= 31 * 16;
                }


                sum = ZERO;

                for (int i = 0; i < 30; i++)
                {
                    if (Hint.Likely(bytes >= 16))
                    {
                        sum = Sse2.add_epi64(sum, Popcnt_Ssse3(Sse2.loadu_si128(ptr_v128++), SHUFFLE_MASK, NIBBLE_MASK));

                        bytes -= 16;
                    }
                    else break;
                }

                longSum = Sse2.add_epi64(longSum, Sse2.sad_epu8(ZERO, sum));
                longSum = Sse2.add_epi64(longSum, Sse2.shuffle_epi32(longSum, Sse.SHUFFLE(0, 0, 3, 2)));
                ulong bits = longSum.ULong0;


                if (Hint.Likely(bytes >= 8))
                {
                    bits += (uint)math.countbits(*(ulong*)ptr_v128);

                    ptr_v128 = (v128*)((ulong*)ptr_v128 + 1);
                    bytes -= 8;
                }


                if (Hint.Likely(bytes >= 4))
                {
                    bits += (uint)math.countbits(*(uint*)ptr_v128);

                    ptr_v128 = (v128*)((uint*)ptr_v128 + 1);
                    bytes -= 4;
                }


                if (Hint.Likely(bytes >= 2))
                {
                    bits += (uint)math.countbits((uint)*((ushort*)ptr_v128));

                    ptr_v128 = (v128*)((ushort*)ptr_v128 + 1);
                    bytes -= 2;
                }


                if (Hint.Likely(bytes != 0))
                {
                    bits += (uint)math.countbits((uint)*((byte*)ptr_v128));
                }


                return bits;
            }
            else
            {
                long longs = bytes / 8;
                long residuals = bytes % 8;
                ulong bits = 0;


                for (long i = 0; i < longs; i++)
                {
                    bits += (ulong)math.countbits(*((ulong*)ptr));
                    ptr = (ulong*)ptr + 1;
                }

                while (residuals != 0)
                {
                    bits += (ulong)math.countbits((uint)(*(byte*)ptr));

                    ptr = (byte*)ptr + 1;
                    residuals--;
                }

                return bits;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeArray<T> array)
            where T : unmanaged
        {
            return SIMD_CountBits((T*)array.GetUnsafeReadOnlyPtr(), array.Length * sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeArray<T> array, int index)
            where T : unmanaged
        {
            return SIMD_CountBits((T*)array.GetUnsafeReadOnlyPtr() + index, (array.Length * sizeof(T) - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeArray<T> array, int index, int numEntries)
            where T : unmanaged
        {
            return SIMD_CountBits((T*)array.GetUnsafeReadOnlyPtr() + index, numEntries * sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeList<T> array)
            where T : unmanaged
        {
            return SIMD_CountBits((T*)array.GetUnsafeReadOnlyPtr(), array.Length * sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeList<T> array, int index)
            where T : unmanaged
        {
            return SIMD_CountBits((T*)array.GetUnsafeReadOnlyPtr() + index, (array.Length * sizeof(T) - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeList<T> array, int index, int numEntries)
            where T : unmanaged
        {
            return SIMD_CountBits((T*)array.GetUnsafeReadOnlyPtr() + index, numEntries * sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeSlice<T> array)
            where T : unmanaged
        {
            return SIMD_CountBits((T*)array.GetUnsafeReadOnlyPtr(), array.Length * sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeSlice<T> array, int index)
            where T : unmanaged
        {
            return SIMD_CountBits((T*)array.GetUnsafeReadOnlyPtr() + index, (array.Length * sizeof(T) - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeSlice<T> array, int index, int numEntries)
            where T : unmanaged
        {
            return SIMD_CountBits((T*)array.GetUnsafeReadOnlyPtr() + index, numEntries * sizeof(T));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(byte* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.None:
                {
                    return SIMD_CountBits((void*)ptr, length * sizeof(byte));
                }
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(byte))) - SIMD_CountBits((void*)ptr, length * sizeof(byte));
                }
                default:
                {
                    return SIMD_CountBitsWithMask<byte>(ptr, length, operation, operand);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<byte> array, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
            return SIMD_CountBits((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<byte> array, int index, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
            return SIMD_CountBits((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<byte> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
            return SIMD_CountBits((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<byte> array, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
            return SIMD_CountBits((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<byte> array, int index, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
            return SIMD_CountBits((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<byte> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
            return SIMD_CountBits((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<byte> array, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
            return SIMD_CountBits((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<byte> array, int index, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
            return SIMD_CountBits((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<byte> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
            return SIMD_CountBits((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(ushort* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.None:
                {
                    return SIMD_CountBits((void*)ptr, length * sizeof(ushort));
                }
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(ushort))) - SIMD_CountBits((void*)ptr, length * sizeof(ushort));
                }
                default:
                {
                    return SIMD_CountBitsWithMask<ushort>(ptr, length, operation, operand);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<ushort> array, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
            return SIMD_CountBits((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<ushort> array, int index, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
            return SIMD_CountBits((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<ushort> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
            return SIMD_CountBits((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<ushort> array, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
            return SIMD_CountBits((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<ushort> array, int index, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
            return SIMD_CountBits((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<ushort> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
            return SIMD_CountBits((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<ushort> array, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
            return SIMD_CountBits((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<ushort> array, int index, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
            return SIMD_CountBits((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<ushort> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
            return SIMD_CountBits((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(uint* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.None:
                {
                    return SIMD_CountBits((void*)ptr, length * sizeof(uint));
                }
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(uint))) - SIMD_CountBits((void*)ptr, length * sizeof(uint));
                }
                default:
                {
                    return SIMD_CountBitsWithMask<uint>(ptr, length, operation, operand);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<uint> array, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
            return SIMD_CountBits((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<uint> array, int index, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
            return SIMD_CountBits((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<uint> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
            return SIMD_CountBits((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<uint> array, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
            return SIMD_CountBits((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<uint> array, int index, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
            return SIMD_CountBits((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<uint> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
            return SIMD_CountBits((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<uint> array, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
            return SIMD_CountBits((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<uint> array, int index, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
            return SIMD_CountBits((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<uint> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
            return SIMD_CountBits((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(ulong* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.None:
                {
                    return SIMD_CountBits((void*)ptr, length * sizeof(ulong));
                }
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(ulong))) - SIMD_CountBits((void*)ptr, length * sizeof(ulong));
                }
                default:
                {
                    return SIMD_CountBitsWithMask<ulong>(ptr, length, operation, operand);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<ulong> array, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
            return SIMD_CountBits((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<ulong> array, int index, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
            return SIMD_CountBits((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<ulong> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
            return SIMD_CountBits((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<ulong> array, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
            return SIMD_CountBits((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<ulong> array, int index, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
            return SIMD_CountBits((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<ulong> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
            return SIMD_CountBits((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<ulong> array, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
            return SIMD_CountBits((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<ulong> array, int index, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
            return SIMD_CountBits((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<ulong> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
            return SIMD_CountBits((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(sbyte* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.None:
                {
                    return SIMD_CountBits((void*)ptr, length * sizeof(sbyte));
                }
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(sbyte))) - SIMD_CountBits((void*)ptr, length * sizeof(sbyte));
                }
                default:
                {
                    return SIMD_CountBitsWithMask<sbyte>(ptr, length, operation, operand);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<sbyte> array, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
            return SIMD_CountBits((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<sbyte> array, int index, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
            return SIMD_CountBits((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<sbyte> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
            return SIMD_CountBits((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<sbyte> array, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
            return SIMD_CountBits((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<sbyte> array, int index, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
            return SIMD_CountBits((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<sbyte> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
            return SIMD_CountBits((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<sbyte> array, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
            return SIMD_CountBits((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<sbyte> array, int index, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
            return SIMD_CountBits((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<sbyte> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
            return SIMD_CountBits((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(short* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.None:
                {
                    return SIMD_CountBits((void*)ptr, length * sizeof(short));
                }
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(short))) - SIMD_CountBits((void*)ptr, length * sizeof(short));
                }
                default:
                {
                    return SIMD_CountBitsWithMask<short>(ptr, length, operation, operand);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<short> array, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
            return SIMD_CountBits((short*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<short> array, int index, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
            return SIMD_CountBits((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<short> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
            return SIMD_CountBits((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<short> array, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
            return SIMD_CountBits((short*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<short> array, int index, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
            return SIMD_CountBits((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<short> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
            return SIMD_CountBits((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<short> array, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
            return SIMD_CountBits((short*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<short> array, int index, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
            return SIMD_CountBits((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<short> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
            return SIMD_CountBits((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(int* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.None:
                {
                    return SIMD_CountBits((void*)ptr, length * sizeof(int));
                }
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(int))) - SIMD_CountBits((void*)ptr, length * sizeof(int));
                }
                default:
                {
                    return SIMD_CountBitsWithMask<int>(ptr, length, operation, operand);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<int> array, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
            return SIMD_CountBits((int*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<int> array, int index, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
            return SIMD_CountBits((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<int> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
            return SIMD_CountBits((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<int> array, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
            return SIMD_CountBits((int*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<int> array, int index, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
            return SIMD_CountBits((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<int> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
            return SIMD_CountBits((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<int> array, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
            return SIMD_CountBits((int*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<int> array, int index, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
            return SIMD_CountBits((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<int> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
            return SIMD_CountBits((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(long* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.None:
                {
                    return SIMD_CountBits((void*)ptr, length * sizeof(long));
                }
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(long))) - SIMD_CountBits((void*)ptr, length * sizeof(long));
                }
                default:
                {
                    return SIMD_CountBitsWithMask<long>(ptr, length, operation, operand);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<long> array, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
            return SIMD_CountBits((long*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<long> array, int index, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
            return SIMD_CountBits((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<long> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
            return SIMD_CountBits((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<long> array, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
            return SIMD_CountBits((long*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<long> array, int index, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
            return SIMD_CountBits((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<long> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
            return SIMD_CountBits((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<long> array, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
            return SIMD_CountBits((long*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<long> array, int index, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
            return SIMD_CountBits((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<long> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
            return SIMD_CountBits((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(float* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.None:
                {
                    return SIMD_CountBits((void*)ptr, length * sizeof(float));
                }
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(float))) - SIMD_CountBits((void*)ptr, length * sizeof(float));
                }
                default:
                {
                    return SIMD_CountBitsWithMask<float>(ptr, length, operation, operand);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<float> array, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
            return SIMD_CountBits((float*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<float> array, int index, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
            return SIMD_CountBits((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<float> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
            return SIMD_CountBits((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<float> array, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
            return SIMD_CountBits((float*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<float> array, int index, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
            return SIMD_CountBits((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<float> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
            return SIMD_CountBits((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<float> array, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
            return SIMD_CountBits((float*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<float> array, int index, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
            return SIMD_CountBits((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<float> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
            return SIMD_CountBits((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(double* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.None:
                {
                    return SIMD_CountBits((void*)ptr, length * sizeof(double));
                }
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(double))) - SIMD_CountBits((void*)ptr, length * sizeof(double));
                }
                default:
                {
                    return SIMD_CountBitsWithMask<double>(ptr, length, operation, operand);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<double> array, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
            return SIMD_CountBits((double*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<double> array, int index, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
            return SIMD_CountBits((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<double> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
            return SIMD_CountBits((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<double> array, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
            return SIMD_CountBits((double*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<double> array, int index, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
            return SIMD_CountBits((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<double> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
            return SIMD_CountBits((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<double> array, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
            return SIMD_CountBits((double*)array.GetUnsafeReadOnlyPtr(), array.Length, operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<double> array, int index, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
            return SIMD_CountBits((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<double> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
            return SIMD_CountBits((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }
    }
}