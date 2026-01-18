using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst.Intrinsics;
using Unity.Burst.CompilerServices;
using MaxMath;
using MaxMath.Intrinsics;
using DevTools;

using static Unity.Burst.Intrinsics.X86;
using static MaxMath.maxmath;
using static Unity.Mathematics.math;

namespace SIMDAlgorithms
{
    unsafe public static partial class Algorithms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static v256 DecodeOp256(BitwiseOperation operation, v256 value, v256 mask)
        {
            if (Avx2.IsAvx2Supported)
            {
                return operation == BitwiseOperation.AND    ? Xse.mm256_popcnt_epi8(Avx2.mm256_and_si256(value, mask))                                      :
                       operation == BitwiseOperation.OR     ? Xse.mm256_popcnt_epi8(Avx2.mm256_or_si256(value, mask))                                       :
                       operation == BitwiseOperation.XOR    ? Xse.mm256_popcnt_epi8(Avx2.mm256_xor_si256(value, mask))                                      :
                       operation == BitwiseOperation.ANDNOT ? Xse.mm256_popcnt_epi8(Avx2.mm256_andnot_si256(value, mask))                                   :
                       operation == BitwiseOperation.ORNOT  ? Xse.mm256_popcnt_epi8(Xse.mm256_ternarylogic_si256(mask, value, mask, TernaryOperation.OxF3)) :
                       operation == BitwiseOperation.NAND   ? Xse.mm256_popcnt_epi8(Xse.mm256_ternarylogic_si256(mask, value, mask, TernaryOperation.Ox3F)) :
                       operation == BitwiseOperation.NOR    ? Xse.mm256_popcnt_epi8(Xse.mm256_ternarylogic_si256(mask, value, mask, TernaryOperation.OxO3)) :
                       operation == BitwiseOperation.XNOR   ? Xse.mm256_popcnt_epi8(Xse.mm256_ternarylogic_si256(mask, value, mask, TernaryOperation.OxC3)) :
                       operation == BitwiseOperation.None   ? Xse.mm256_popcnt_epi8(value)                                                                  :
                       throw new System.Exception($"Internal error: Invalid bitwise operation { operation }.");
            }
            else throw new IllegalInstructionException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static v128 DecodeOp128(BitwiseOperation operation, v128 value, v128 mask)
        {
            if (BurstArchitecture.IsSIMDSupported)
            {
                return operation == BitwiseOperation.AND    ? Xse.popcnt_epi8(Xse.and_si128(value, mask))                                       :
                       operation == BitwiseOperation.OR     ? Xse.popcnt_epi8(Xse.or_si128(value, mask))                                        :
                       operation == BitwiseOperation.XOR    ? Xse.popcnt_epi8(Xse.xor_si128(value, mask))                                       :
                       operation == BitwiseOperation.ANDNOT ? Xse.popcnt_epi8(Xse.andnot_si128(value, mask))                                    :
                       operation == BitwiseOperation.ORNOT  ? Xse.popcnt_epi8(Xse.ternarylogic_si128(mask, value, mask, TernaryOperation.OxF3)) :
                       operation == BitwiseOperation.NAND   ? Xse.popcnt_epi8(Xse.ternarylogic_si128(mask, value, mask, TernaryOperation.Ox3F)) :
                       operation == BitwiseOperation.NOR    ? Xse.popcnt_epi8(Xse.ternarylogic_si128(mask, value, mask, TernaryOperation.OxO3)) :
                       operation == BitwiseOperation.XNOR   ? Xse.popcnt_epi8(Xse.ternarylogic_si128(mask, value, mask, TernaryOperation.OxC3)) :
                       operation == BitwiseOperation.None   ? Xse.popcnt_epi8(value)                                                            :
                       throw new System.Exception($"Internal error: Invalid bitwise operation { operation }.");
            }
            else throw new IllegalInstructionException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint DecodeOp64(BitwiseOperation operation, ulong value, ulong maskScalar)
        {
            return (uint)countbits(operation == BitwiseOperation.AND    ? value & maskScalar        :
                                   operation == BitwiseOperation.OR     ? value | maskScalar        :
                                   operation == BitwiseOperation.XOR    ? value ^ maskScalar        :
                                   operation == BitwiseOperation.ANDNOT ? andnot(maskScalar, value) :
                                   operation == BitwiseOperation.ORNOT  ? ~value | maskScalar       :
                                   operation == BitwiseOperation.NAND   ? ~(value & maskScalar)     :
                                   operation == BitwiseOperation.NOR    ? ~(value | maskScalar)     :
                                   operation == BitwiseOperation.XNOR   ? ~(value ^ maskScalar)     :
                                   operation == BitwiseOperation.None   ? value                     :
                                   throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint DecodeOp32(BitwiseOperation operation, uint value, ulong maskScalar)
        {
            return (uint)countbits(operation == BitwiseOperation.AND    ? value & (uint)maskScalar        :
                                   operation == BitwiseOperation.OR     ? value | (uint)maskScalar        :
                                   operation == BitwiseOperation.XOR    ? value ^ (uint)maskScalar        :
                                   operation == BitwiseOperation.ANDNOT ? andnot((uint)maskScalar, value) :
                                   operation == BitwiseOperation.ORNOT  ? ~value | (uint)maskScalar       :
                                   operation == BitwiseOperation.NAND   ? ~(value & (uint)maskScalar)     :
                                   operation == BitwiseOperation.NOR    ? ~(value | (uint)maskScalar)     :
                                   operation == BitwiseOperation.XNOR   ? ~(value ^ (uint)maskScalar)     :
                                   operation == BitwiseOperation.None   ? value                           :
                                   throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint DecodeOp16(BitwiseOperation operation, ushort value, ulong maskScalar)
        {
            return (uint)countbits(operation == BitwiseOperation.AND    ? (uint)(value & (ushort)maskScalar)                    :
                                   operation == BitwiseOperation.OR     ? (uint)(value | (ushort)maskScalar)                    :
                                   operation == BitwiseOperation.XOR    ? (uint)(value ^ (ushort)maskScalar)                    :
                                   operation == BitwiseOperation.ANDNOT ? andnot((ushort)maskScalar, value)                     :
                                   operation == BitwiseOperation.ORNOT  ? (uint)(0x0000_FFFF & (~value | (ushort)maskScalar))   :
                                   operation == BitwiseOperation.NAND   ? (uint)(0x0000_FFFF & (~(value & (ushort)maskScalar))) :
                                   operation == BitwiseOperation.NOR    ? (uint)(0x0000_FFFF & (~(value | (ushort)maskScalar))) :
                                   operation == BitwiseOperation.XNOR   ? (uint)(0x0000_FFFF & (~(value ^ (ushort)maskScalar))) :
                                   operation == BitwiseOperation.None   ? (uint)value                                           :
                                   throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint DecodeOp8(BitwiseOperation operation, byte value, ulong maskScalar)
        {
            return (uint)countbits(operation == BitwiseOperation.AND    ? (uint)(value & (byte)maskScalar)                    :
                                   operation == BitwiseOperation.OR     ? (uint)(value | (byte)maskScalar)                    :
                                   operation == BitwiseOperation.XOR    ? (uint)(value ^ (byte)maskScalar)                    :
                                   operation == BitwiseOperation.ANDNOT ? andnot((byte)maskScalar, value)                     :
                                   operation == BitwiseOperation.ORNOT  ? (uint)(0x0000_00FF & (~value | (byte)maskScalar))   :
                                   operation == BitwiseOperation.NAND   ? (uint)(0x0000_00FF & (~(value & (byte)maskScalar))) :
                                   operation == BitwiseOperation.NOR    ? (uint)(0x0000_00FF & (~(value | (byte)maskScalar))) :
                                   operation == BitwiseOperation.XNOR   ? (uint)(0x0000_00FF & (~(value ^ (byte)maskScalar))) :
                                   operation == BitwiseOperation.None   ? (uint)value                                         :
                                   throw new System.Exception($"Internal error: Invalid bitwise operation { operation }."));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong SIMD_CountBitsWithMask<T>(T* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, T operand = default)
            where T : unmanaged
        {

Assert.IsNonNegative(length);
Assert.IsNonNegative(length * sizeof(T));
Assert.IsNotGreater((ulong)length * (ulong)sizeof(T), (ulong)long.MaxValue);

            if (Avx2.IsAvx2Supported)
            {
                long bytes = length * sizeof(T);
                v256 mask = sizeof(T) == 1 ? Avx.mm256_set1_epi8(*(byte*)&operand)   :
                            sizeof(T) == 2 ? Avx.mm256_set1_epi16(*(short*)&operand) :
                            sizeof(T) == 4 ? Avx.mm256_set1_epi32(*(int*)&operand)   :
                                             Avx.mm256_set1_epi64x(*(long*)&operand);
                ulong maskScalar = mask.ULong0;

                v256 ZERO = Avx.mm256_setzero_si256();

                v256 acc0 = Avx.mm256_setzero_si256();
                v256 acc1 = Avx.mm256_setzero_si256();
                v256 acc2 = Avx.mm256_setzero_si256();
                v256 acc3 = Avx.mm256_setzero_si256();
                ulong acc4 = 0;
                ulong acc5 = 0;
                ulong acc6 = 0;
                ulong acc7 = 0;
                v256* ptr_v256 = (v256*)ptr;

                while (Hint.Likely(bytes >= 4 * BYTES_IN_LONG + 4 * BYTES_IN_V256))
                {
                    acc0 = Avx2.mm256_add_epi64(acc0, Avx2.mm256_sad_epu8(DecodeOp256(operation, ptr_v256[0], mask), ZERO));
                    acc1 = Avx2.mm256_add_epi64(acc1, Avx2.mm256_sad_epu8(DecodeOp256(operation, ptr_v256[1], mask), ZERO));
                    acc2 = Avx2.mm256_add_epi64(acc2, Avx2.mm256_sad_epu8(DecodeOp256(operation, ptr_v256[2], mask), ZERO));
                    acc3 = Avx2.mm256_add_epi64(acc3, Avx2.mm256_sad_epu8(DecodeOp256(operation, ptr_v256[3], mask), ZERO));
                    acc4 += DecodeOp64(operation, ((ulong*)ptr_v256)[16], maskScalar);
                    acc5 += DecodeOp64(operation, ((ulong*)ptr_v256)[17], maskScalar);
                    acc6 += DecodeOp64(operation, ((ulong*)ptr_v256)[18], maskScalar);
                    acc7 += DecodeOp64(operation, ((ulong*)ptr_v256)[19], maskScalar);

                    ptr_v256 += 4 + 4 / LONGS_IN_V256;
                    bytes -= 4 * BYTES_IN_LONG + 4 * BYTES_IN_V256;
                }

                v256 longSum = Avx2.mm256_add_epi64(Avx2.mm256_add_epi64(acc0, acc1), Avx2.mm256_add_epi64(acc2, acc3));

                if (Hint.Likely(bytes >= BYTES_IN_V256))
                {
                    v256 lastSum = Avx2.mm256_sad_epu8(DecodeOp256(operation, *ptr_v256++, mask), ZERO);

                    if (Hint.Likely(bytes >= 2 * BYTES_IN_V256))
                    {
                        lastSum = Avx2.mm256_add_epi64(lastSum, Avx2.mm256_sad_epu8(DecodeOp256(operation, *ptr_v256++, mask), ZERO));

                        if (Hint.Likely(bytes >= 3 * BYTES_IN_V256))
                        {
                            lastSum = Avx2.mm256_add_epi64(lastSum, Avx2.mm256_sad_epu8(DecodeOp256(operation, *ptr_v256++, mask), ZERO));

                            if (Hint.Likely(bytes >= 4 * BYTES_IN_V256))
                            {
                                lastSum = Avx2.mm256_add_epi64(lastSum, Avx2.mm256_sad_epu8(DecodeOp256(operation, *ptr_v256++, mask), ZERO));

                                bytes -= 4 * BYTES_IN_V256;
                            }
                            else
                            {
                                bytes -= 3 * BYTES_IN_V256;
                            }
                        }
                        else
                        {
                            bytes -= 2 * BYTES_IN_V256;
                        }
                    }
                    else
                    {
                        bytes -= BYTES_IN_V256;
                    }

                    longSum = Avx2.mm256_add_epi64(longSum, lastSum);
                }

                v256 vsum = Xse.mm256_vsum_epi64(longSum);
                ulong bits = Xse.add_epi64(vsum.Lo128, vsum.Hi128).ULong0 + (acc4 + acc5 + acc6 + acc7);


                if (Hint.Likely((sizeof(T) % 32) != 0 && (int)bytes >= 16))
                {
                    v128* ptr128 = (v128*)ptr_v256;
                    bits += Xse.sad_epu8(Xse.vsum_epi64(DecodeOp128(operation, *ptr128, mask.Lo128)), ZERO.Lo128).ULong0;

                    ptr_v256 = (v256*)(ptr128 + 1);
                    bytes -= 16;
                }


                if (Hint.Likely((sizeof(T) % 16) != 0 && (int)bytes >= 8))
                {
                    ulong* ptr64 = (ulong*)ptr_v256;
                    bits += DecodeOp64(operation, *ptr64, maskScalar);

                    ptr_v256 = (v256*)(ptr64 + 1);
                    bytes -= 8;
                }


                if (Hint.Likely((sizeof(T) % 8) != 0 && (int)bytes >= 4))
                {
                    uint* ptr32 = (uint*)ptr_v256;
                    bits += DecodeOp32(operation, *ptr32, maskScalar);

                    ptr_v256 = (v256*)(ptr32 + 1);
                    bytes -= 4;
                }


                if (Hint.Likely((sizeof(T) % 4) != 0 && (int)bytes >= 2))
                {
                    ushort* ptr16 = (ushort*)ptr_v256;
                    bits += DecodeOp16(operation, *ptr16, maskScalar);

                    ptr_v256 = (v256*)(ptr16 + 1);
                    bytes -= 2;
                }


                if (Hint.Likely((sizeof(T) % 2) != 0 && bytes != 0))
                {
                    byte* ptr8 = (byte*)ptr_v256;
                    bits += DecodeOp8(operation, *ptr8, maskScalar);
                }


                return bits;
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                v128 ZERO = Xse.setzero_si128();

                long bytes = length * sizeof(T);
                v128 mask = sizeof(T) == 1 ? Xse.set1_epi8(*(sbyte*)&operand)  :
                            sizeof(T) == 2 ? Xse.set1_epi16(*(short*)&operand) :
                            sizeof(T) == 4 ? Xse.set1_epi32(*(int*)&operand)   :
                                             Xse.set1_epi64x(*(long*)&operand);
                ulong maskScalar = mask.ULong0;

                v128 acc0 = Xse.setzero_si128();
                v128 acc1 = Xse.setzero_si128();
                v128 acc2 = Xse.setzero_si128();
                v128 acc3 = Xse.setzero_si128();
                ulong acc4 = 0;
                ulong acc5 = 0;
                ulong acc6 = 0;
                ulong acc7 = 0;
                v128* ptr_v128 = (v128*)ptr;

                while (Hint.Likely(bytes >= 4 * BYTES_IN_LONG + 4 * BYTES_IN_V128))
                {
                    acc0 = Xse.add_epi64(acc0, Xse.sad_epu8(DecodeOp128(operation, ptr_v128[0], mask), ZERO));
                    acc1 = Xse.add_epi64(acc1, Xse.sad_epu8(DecodeOp128(operation, ptr_v128[1], mask), ZERO));
                    acc2 = Xse.add_epi64(acc2, Xse.sad_epu8(DecodeOp128(operation, ptr_v128[2], mask), ZERO));
                    acc3 = Xse.add_epi64(acc3, Xse.sad_epu8(DecodeOp128(operation, ptr_v128[3], mask), ZERO));
                    acc4 += DecodeOp64(operation, ((ulong*)ptr_v128)[8], maskScalar);
                    acc5 += DecodeOp64(operation, ((ulong*)ptr_v128)[9], maskScalar);
                    acc6 += DecodeOp64(operation, ((ulong*)ptr_v128)[10], maskScalar);
                    acc7 += DecodeOp64(operation, ((ulong*)ptr_v128)[11], maskScalar);

                    ptr_v128 += 4 + 4 / LONGS_IN_V128;
                    bytes -= 4 * BYTES_IN_LONG + 4 * BYTES_IN_V128;
                }

                v128 longSum = Xse.add_epi64(Xse.add_epi64(acc0, acc1), Xse.add_epi64(acc2, acc3));

                if (Hint.Likely(bytes >= BYTES_IN_V128))
                {
                    v128 lastSum = Xse.sad_epu8(DecodeOp128(operation, *ptr_v128++, mask), ZERO);

                    if (Hint.Likely(bytes >= 2 * BYTES_IN_V128))
                    {
                        lastSum = Xse.add_epi64(lastSum, Xse.sad_epu8(DecodeOp128(operation, *ptr_v128++, mask), ZERO));

                        if (Hint.Likely(bytes >= 3 * BYTES_IN_V128))
                        {
                            lastSum = Xse.add_epi64(lastSum, Xse.sad_epu8(DecodeOp128(operation, *ptr_v128++, mask), ZERO));

                            if (Hint.Likely(bytes >= 4 * BYTES_IN_V128))
                            {
                                lastSum = Xse.add_epi64(lastSum, Xse.sad_epu8(DecodeOp128(operation, *ptr_v128++, mask), ZERO));

                                if (Hint.Likely(bytes >= 5 * BYTES_IN_V128))
                                {
                                    lastSum = Xse.add_epi64(lastSum, Xse.sad_epu8(DecodeOp128(operation, *ptr_v128++, mask), ZERO));

                                    bytes -= 5 * BYTES_IN_V128;
                                }
                                else
                                {
                                    bytes -= 4 * BYTES_IN_V128;
                                }
                            }
                            else
                            {
                                bytes -= 3 * BYTES_IN_V128;
                            }
                        }
                        else
                        {
                            bytes -= 2 * BYTES_IN_V128;
                        }
                    }
                    else
                    {
                        bytes -= BYTES_IN_V128;
                    }

                    longSum = Xse.add_epi64(longSum, lastSum);
                }

                ulong bits = Xse.vsum_epi64(longSum).ULong0 + (acc4 + acc5 + acc6 + acc7);


                if (Hint.Likely((sizeof(T) % 16) != 0 && (int)bytes >= 8))
                {
                    ulong* ptr64 = (ulong*)ptr_v128;
                    bits += DecodeOp64(operation, *ptr64, maskScalar);

                    ptr_v128 = (v128*)(ptr64 + 1);
                    bytes -= 8;
                }


                if (Hint.Likely((sizeof(T) % 8) != 0 && (int)bytes >= 4))
                {
                    uint* ptr32 = (uint*)ptr_v128;
                    bits += DecodeOp32(operation, *ptr32, maskScalar);

                    ptr_v128 = (v128*)(ptr32 + 1);
                    bytes -= 4;
                }


                if (Hint.Likely((sizeof(T) % 4) != 0 && (int)bytes >= 2))
                {
                    ushort* ptr16 = (ushort*)ptr_v128;
                    bits += DecodeOp16(operation, *ptr16, maskScalar);

                    ptr_v128 = (v128*)(ptr16 + 1);
                    bytes -= 2;
                }


                if (Hint.Likely((sizeof(T) % 2) != 0 && bytes != 0))
                {
                    byte* ptr8 = (byte*)ptr_v128;
                    bits += DecodeOp8(operation, *ptr8, maskScalar);

                    ptr_v128 = (v128*)(ptr8 + 1);
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


                while (Hint.Likely((int)bytes >= 8))
                {
                    ulong* ptr64 = (ulong*)ptr;
                    bits += DecodeOp64(operation, *ptr64, mask);

                    ptr = (T*)(ptr64 + 1);
                    bytes -= 8;
                }

                if (Hint.Likely((int)bytes >= 4))
                {
                    uint* ptr32 = (uint*)ptr;
                    bits += DecodeOp32(operation, *ptr32, mask);

                    ptr = (T*)(ptr32 + 1);
                    bytes -= 4;
                }

                if (Hint.Likely((int)bytes >= 2))
                {
                    ushort* ptr16 = (ushort*)ptr;
                    bits += DecodeOp16(operation, *ptr16, mask);

                    ptr = (T*)(ptr16 + 1);
                    bytes -= 2;
                }

                if (Hint.Likely(bytes != 0))
                {
                    byte* ptr8 = (byte*)ptr;
                    bits += DecodeOp8(operation, *ptr8, mask);

                    ptr = (T*)(ptr8 + 1);
                }


                return bits;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeArray<T> array)
            where T : unmanaged
        {
            return SIMD_CountBitsWithMask((T*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeArray<T> array, int index)
            where T : unmanaged
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBitsWithMask((T*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeArray<T> array, int index, int numEntries)
            where T : unmanaged
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_CountBitsWithMask((T*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeList<T> array)
            where T : unmanaged
        {
            return SIMD_CountBitsWithMask((T*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeList<T> array, int index)
            where T : unmanaged
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBitsWithMask((T*)array.GetUnsafeReadOnlyPtr() + index, array.Length - index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeList<T> array, int index, int numEntries)
            where T : unmanaged
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_CountBitsWithMask((T*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeSlice<T> array)
            where T : unmanaged
        {
            return SIMD_CountBitsWithMask((T*)array.GetUnsafeReadOnlyPtr(), array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeSlice<T> array, int index)
            where T : unmanaged
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBitsWithMask((T*)array.GetUnsafeReadOnlyPtr() + index, array.Length - index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits<T>(this NativeSlice<T> array, int index, int numEntries)
            where T : unmanaged
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_CountBitsWithMask((T*)array.GetUnsafeReadOnlyPtr() + index, numEntries);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(byte* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(byte))) - SIMD_CountBitsWithMask(ptr, length);
                }
                default:
                {
                    return SIMD_CountBitsWithMask(ptr, length, operation, operand);
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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<byte> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<byte> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<byte> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, byte operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_CountBits((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(ushort* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(ushort))) - SIMD_CountBitsWithMask(ptr, length);
                }
                default:
                {
                    return SIMD_CountBitsWithMask(ptr, length, operation, operand);
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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<ushort> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<ushort> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<ushort> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, ushort operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_CountBits((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(uint* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(uint))) - SIMD_CountBitsWithMask(ptr, length);
                }
                default:
                {
                    return SIMD_CountBitsWithMask(ptr, length, operation, operand);
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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<uint> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<uint> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<uint> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, uint operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_CountBits((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(ulong* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(ulong))) - SIMD_CountBitsWithMask(ptr, length);
                }
                default:
                {
                    return SIMD_CountBitsWithMask(ptr, length, operation, operand);
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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<ulong> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<ulong> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<ulong> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, ulong operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_CountBits((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(sbyte* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(sbyte))) - SIMD_CountBitsWithMask(ptr, length);
                }
                default:
                {
                    return SIMD_CountBitsWithMask(ptr, length, operation, operand);
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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<sbyte> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<sbyte> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<sbyte> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, sbyte operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_CountBits((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(short* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(short))) - SIMD_CountBitsWithMask(ptr, length);
                }
                default:
                {
                    return SIMD_CountBitsWithMask(ptr, length, operation, operand);
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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<short> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<short> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<short> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, short operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_CountBits((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(int* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(int))) - SIMD_CountBitsWithMask(ptr, length);
                }
                default:
                {
                    return SIMD_CountBitsWithMask(ptr, length, operation, operand);
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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<int> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<int> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<int> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, int operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_CountBits((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(long* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(long))) - SIMD_CountBitsWithMask(ptr, length);
                }
                default:
                {
                    return SIMD_CountBitsWithMask(ptr, length, operation, operand);
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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<long> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<long> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<long> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, long operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_CountBits((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(float* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(float))) - SIMD_CountBitsWithMask(ptr, length);
                }
                default:
                {
                    return SIMD_CountBitsWithMask(ptr, length, operation, operand);
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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<float> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<float> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<float> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, float operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_CountBits((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(double* ptr, long length, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
            switch (operation)
            {
                case BitwiseOperation.NOT:
                {
                    return ((ulong)length * (8 * sizeof(double))) - SIMD_CountBitsWithMask(ptr, length);
                }
                default:
                {
                    return SIMD_CountBitsWithMask(ptr, length, operation, operand);
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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeArray<double> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeList<double> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

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
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_CountBits((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), operation, operand);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SIMD_CountBits(this NativeSlice<double> array, int index, int numEntries, BitwiseOperation operation = BitwiseOperation.None, double operand = 0)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_CountBits((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, operation, operand);
        }
    }
}