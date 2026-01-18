using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst.Intrinsics;
using Unity.Burst.CompilerServices;
using MaxMath;
using MaxMath.Intrinsics;
using DevTools;

using static Unity.Burst.Intrinsics.X86;

namespace SIMDAlgorithms
{
    // further unrolling does NOT break dependencies and/or adds branches
    unsafe public static partial class Algorithms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(bool* ptr, long length, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
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
            return SIMD_Contains((byte*)ptr, length, *(byte*)&value, Comparison.EqualTo, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<bool> array, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<bool> array, int index, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<bool> array, int index, int numEntries, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<bool> array, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<bool> array, int index, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<bool> array, int index, int numEntries, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<bool> array, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<bool> array, int index, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<bool> array, int index, int numEntries, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(byte* ptr, long length, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi8(value);
                    v256* ptr_v256 = (v256*)ptr;

                    while (Hint.Likely(length >= 32))
                    {
                        int mask = Avx2.mm256_movemask_epi8(Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v256++;
                        length -= 32;
                    }

                    if (Hint.Likely((int)length >= 16))
                    {
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 16;
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    }

                    v128 cmp = Xse.setzero_si128();

                    if (Hint.Likely((int)length >= 8))
                    {
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 8;
                        ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    }

                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 4;
                        ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    }

                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.insert_epi16(cmp, *(ushort*)ptr_v256, 0), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b0011))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 2;
                        ptr_v256 = (v256*)((short*)ptr_v256 + 1);
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.Bytes(*(byte*)ptr_v256, value, where);
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_epi8((sbyte)value);
                    v128* ptr_v128 = (v128*)ptr;

                    while (Hint.Likely(length >= 16))
                    {
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.loadu_si128(ptr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v128++;
                        length -= 16;
                    }


                    if (Hint.Likely((int)length >= 8))
                    {
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 8;
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    }

                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 4;
                        ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    }

                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.insert_epi16(Xse.setzero_si128(), *(ushort*)ptr_v128, 0), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b0011))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 2;
                        ptr_v128 = (v128*)((short*)ptr_v128 + 1);
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.Bytes(*(byte*)ptr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.Bytes(ptr[i], value, where))
                        {
                            return true;
                        }
                        else continue;
                    }

                    return false;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi8(value);
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx2.mm256_movemask_epi8(Compare.Bytes256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.loadu_si128(endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 8))
                    {
                        endPtr_v256 = (v256*)((long*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.cvtsi64x_si128(*(long*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 4))
                    {
                        endPtr_v256 = (v256*)((int*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.cvtsi32_si128(*(int*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 2))
                    {
                        endPtr_v256 = (v256*)((ushort*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.cvtsi32_si128(*(ushort*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b0011))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((byte*)endPtr_v256 - 1);

                        return Compare.Bytes(*(byte*)endPtr_v256, value, where);
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_epi8((sbyte)value);
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.loadu_si128(endPtr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 4))
                    {
                        endPtr_v128 = (v128*)((int*)endPtr_v128 - 1);
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.cvtsi32_si128(*(int*)endPtr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 2))
                    {
                        endPtr_v128 = (v128*)((ushort*)endPtr_v128 - 1);
                        int mask = Xse.movemask_epi8(Compare.Bytes128(Xse.cvtsi32_si128(*(ushort*)endPtr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b0011))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((byte*)endPtr_v128 - 1);

                        return Compare.Bytes(*(byte*)endPtr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    length--;

                    while (length >= 0)
                    {
                        if (Compare.Bytes(ptr[length], value, where))
                        {
                            return true;
                        }
                        else
                        {
                            length--;
                        }
                    }

                    return false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<byte> array, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<byte> array, int index, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<byte> array, int index, int numEntries, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<byte> array, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<byte> array, int index, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<byte> array, int index, int numEntries, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<byte> array, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<byte> array, int index, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<byte> array, int index, int numEntries, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(ushort* ptr, long length, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi16((short)value);
                    v256* ptr_v256 = (v256*)ptr;

                    while (Hint.Likely(length >= 16))
                    {
                        int mask = Avx2.mm256_movemask_epi8(Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v256++;
                        length -= 16;
                    }

                    if (Hint.Likely((int)length >= 8))
                    {
                        int mask = Xse.movemask_epi8(Compare.UShorts128(Xse.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 8;
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    }

                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Xse.movemask_epi8(Compare.UShorts128(Xse.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 4;
                        ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    }

                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Xse.movemask_epi8(Compare.UShorts128(Xse.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 2;
                        ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.UShorts(*(ushort*)ptr_v256, value, where);
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_epi16((short)value);
                    v128* ptr_v128 = (v128*)ptr;

                    while (Hint.Likely(length >= 8))
                    {
                        int mask = Xse.movemask_epi8(Compare.UShorts128(Xse.loadu_si128(ptr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v128++;
                        length -= 8;
                    }

                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Xse.movemask_epi8(Compare.UShorts128(Xse.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 4;
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    }

                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Xse.movemask_epi8(Compare.UShorts128(Xse.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 2;
                        ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.UShorts(*(ushort*)ptr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.UShorts(ptr[i], value, where))
                        {
                            return true;
                        }
                        else continue;
                    }

                    return false;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi16((short)value);
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx2.mm256_movemask_epi8(Compare.UShorts256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.UShorts128(Xse.loadu_si128(endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 8))
                    {
                        endPtr_v256 = (v256*)((long*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.UShorts128(Xse.cvtsi64x_si128(*(long*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 4))
                    {
                        endPtr_v256 = (v256*)((int*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.UShorts128(Xse.cvtsi32_si128(*(int*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((ushort*)endPtr_v256 - 1);

                        return Compare.UShorts(*(ushort*)endPtr_v256, value, where);
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_epi16((short)value);
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Xse.movemask_epi8(Compare.UShorts128(Xse.loadu_si128(endPtr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Xse.movemask_epi8(Compare.UShorts128(Xse.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 4))
                    {
                        endPtr_v128 = (v128*)((int*)endPtr_v128 - 1);
                        int mask = Xse.movemask_epi8(Compare.UShorts128(Xse.cvtsi32_si128(*(int*)endPtr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((ushort*)endPtr_v128 - 1);

                        return Compare.UShorts(*(ushort*)endPtr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    length--;

                    while (length >= 0)
                    {
                        if (Compare.UShorts(ptr[length], value, where))
                        {
                            return true;
                        }
                        else
                        {
                            length--;
                        }
                    }

                    return false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<ushort> array, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<ushort> array, int index, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<ushort> array, int index, int numEntries, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<ushort> array, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<ushort> array, int index, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<ushort> array, int index, int numEntries, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<ushort> array, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<ushort> array, int index, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<ushort> array, int index, int numEntries, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(uint* ptr, long length, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi32((int)value);
                    v256* ptr_v256 = (v256*)ptr;

                    while (Hint.Likely(length >= 8))
                    {
                        int mask = Avx2.mm256_movemask_epi8(Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v256++;
                        length -= 8;
                    }

                    if (Hint.Likely((int)length != 0))
                    {
                        v256 offsets = new v256(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u);
                        offsets = Avx2.mm256_min_epi32(offsets, Avx.mm256_set1_epi32((int)length - 1));
                        v256 lastLoad = Avx2.mm256_i32gather_epi32(ptr_v256, offsets, sizeof(int));

                        int mask = Avx2.mm256_movemask_epi8(Compare.UInts256(lastLoad, broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_epi32((int)value);
                    v128* ptr_v128 = (v128*)ptr;

                    while (Hint.Likely(length >= 4))
                    {
                        int mask = Xse.movemask_epi8(Compare.UInts128(Xse.loadu_si128(ptr_v128), broadcast, where));

                        if (Sse4_1.IsSse41Supported)
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0xFFFF))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0xFFFF))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    return true;
                                }
                            }
                        }

                        ptr_v128++;
                        length -= 4;
                    }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Xse.movemask_epi8(Compare.UInts128(Xse.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (Sse4_1.IsSse41Supported)
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0b1111_1111))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0b1111_1111))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    return true;
                                }
                            }
                        }

                        length -= 2;
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.UInts(*(uint*)ptr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.UInts(ptr[i], value, where))
                        {
                            return true;
                        }
                        else continue;
                    }

                    return false;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi32((int)value);
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx2.mm256_movemask_epi8(Compare.UInts256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        v256 offsets = new v256(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u);
                        offsets = Avx2.mm256_min_epi32(offsets, Avx.mm256_set1_epi32((int)length - 1));
                        v256 lastLoad = Avx2.mm256_i32gather_epi32(ptr, offsets, sizeof(int));

                        int mask = Avx2.mm256_movemask_epi8(Compare.UInts256(lastLoad, broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_epi32((int)value);
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Xse.movemask_epi8(Compare.UInts128(Xse.loadu_si128(endPtr_v128), broadcast, where));

                        if (Sse4_1.IsSse41Supported)
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0xFFFF))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0xFFFF))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Xse.movemask_epi8(Compare.UInts128(Xse.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (Sse4_1.IsSse41Supported)
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0b1111_1111))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0b1111_1111))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((uint*)endPtr_v128 - 1);

                        return Compare.UInts(*(uint*)endPtr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    length--;

                    while (length >= 0)
                    {
                        if (Compare.UInts(ptr[length], value, where))
                        {
                            return true;
                        }
                        else
                        {
                            length--;
                        }
                    }

                    return false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<uint> array, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<uint> array, int index, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<uint> array, int index, int numEntries, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<uint> array, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<uint> array, int index, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<uint> array, int index, int numEntries, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<uint> array, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<uint> array, int index, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<uint> array, int index, int numEntries, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(ulong* ptr, long length, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi64x((long)value);
                    v256* ptr_v256 = (v256*)ptr;

                    while (Hint.Likely(length >= 4))
                    {
                        int mask = Avx2.mm256_movemask_epi8(Compare.ULongs256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v256++;
                        length -= 4;
                    }

                    if (Hint.Likely((int)length != 0))
                    {
                        v256 offsets = new v256(0ul, 1ul, 2ul, 3ul);
                        offsets = Xse.mm256_min_epi64(offsets, Avx.mm256_set1_epi64x(length - 1));
                        v256 lastLoad = Avx2.mm256_i64gather_epi64(ptr_v256, offsets, sizeof(long));

                        int mask = Avx2.mm256_movemask_epi8(Compare.ULongs256(lastLoad, broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
                else if (BurstArchitecture.IsCMP64Supported)
                {
                    v128 broadcast = Xse.set1_epi64x((long)value);
                    v128* ptr_v128 = (v128*)ptr;

                    while (Hint.Likely(length >= 2))
                    {
                        int mask = Xse.movemask_epi8(Compare.ULongs128(Xse.loadu_si128(ptr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v128++;
                        length -= 2;
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.ULongs(*(ulong*)ptr_v128, value, where);
                    }

                    return false;
                }
                else if (Sse4_1.IsSse41Supported)
                {
                    if (where == Comparison.EqualTo || where == Comparison.NotEqualTo)
                    {
                        v128 broadcast = Xse.set1_epi64x((long)value);
                        v128* ptr_v128 = (v128*)ptr;

                        while (Hint.Likely(length >= 2))
                        {
                            int mask = Xse.movemask_epi8(Compare.ULongs128(Xse.loadu_si128(ptr_v128), broadcast, where));

                            if (where == Comparison.NotEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0xFFFF))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    return true;
                                }
                            }

                            ptr_v128++;
                            length -= 2;
                        }

                        if (Hint.Likely(length != 0))
                        {
                            return Compare.ULongs(*(ulong*)ptr_v128, value, where);
                        }

                        return false;
                    }
                    else
                    {
                        for (long i = 0; i < length; i++)
                        {
                            if (Compare.ULongs(ptr[i], value, where))
                            {
                                return true;
                            }
                            else continue;
                        }

                        return false;
                    }
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.ULongs(ptr[i], value, where))
                        {
                            return true;
                        }
                        else continue;
                    }

                    return false;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi64x((long)value);
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx2.mm256_movemask_epi8(Compare.ULongs256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        v256 offsets = new v256(0ul, 1ul, 2ul, 3ul);
                        offsets = Xse.mm256_min_epi64(offsets, Avx.mm256_set1_epi64x(length - 1));
                        v256 lastLoad = Avx2.mm256_i64gather_epi64(ptr, offsets, sizeof(long));

                        int mask = Avx2.mm256_movemask_epi8(Compare.ULongs256(lastLoad, broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
                else if (BurstArchitecture.IsCMP64Supported)
                {
                    v128 broadcast = Xse.set1_epi64x((long)value);
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Xse.movemask_epi8(Compare.ULongs128(Xse.loadu_si128(endPtr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((ulong*)endPtr_v128 - 1);

                        return Compare.ULongs(*(ulong*)endPtr_v128, value, where);
                    }

                    return false;
                }
                else if (Sse4_1.IsSse41Supported)
                {
                    if (where == Comparison.EqualTo || where == Comparison.NotEqualTo)
                    {
                        v128 broadcast = Xse.set1_epi64x((long)value);
                        v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                        while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                        {
                            endPtr_v128--;
                            int mask = Xse.movemask_epi8(Compare.ULongs128(Xse.loadu_si128(endPtr_v128), broadcast, where));

                            if (where == Comparison.NotEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0xFFFF))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    return true;
                                }
                            }
                        }

                        if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                        {
                            endPtr_v128 = (v128*)((ulong*)endPtr_v128 - 1);

                            return Compare.ULongs(*(ulong*)endPtr_v128, value, where);
                        }

                        return false;
                    }
                    else
                    {
                        length--;

                        while (length >= 0)
                        {
                            if (Compare.ULongs(ptr[length], value, where))
                            {
                                return true;
                            }
                            else
                            {
                                length--;
                            }
                        }

                        return false;
                    }
                }
                else
                {
                    length--;

                    while (length >= 0)
                    {
                        if (Compare.ULongs(ptr[length], value, where))
                        {
                            return true;
                        }
                        else
                        {
                            length--;
                        }
                    }

                    return false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<ulong> array, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<ulong> array, int index, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<ulong> array, int index, int numEntries, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<ulong> array, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<ulong> array, int index, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<ulong> array, int index, int numEntries, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<ulong> array, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<ulong> array, int index, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<ulong> array, int index, int numEntries, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(sbyte* ptr, long length, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi8((byte)value);
                    v256* ptr_v256 = (v256*)ptr;

                    while (Hint.Likely(length >= 32))
                    {
                        int mask = Avx2.mm256_movemask_epi8(Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v256++;
                        length -= 32;
                    }

                    if (Hint.Likely((int)length >= 16))
                    {
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 16;
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    }

                    v128 cmp = Xse.setzero_si128();

                    if (Hint.Likely((int)length >= 8))
                    {
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 8;
                        ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    }

                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 4;
                        ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    }

                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.insert_epi16(cmp, *(ushort*)ptr_v256, 0), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b0011))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 2;
                        ptr_v256 = (v256*)((short*)ptr_v256 + 1);
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.SBytes(*(sbyte*)ptr_v256, value, where);
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_epi8(value);
                    v128* ptr_v128 = (v128*)ptr;

                    while (Hint.Likely(length >= 16))
                    {
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.loadu_si128(ptr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v128++;
                        length -= 16;
                    }

                    if (Hint.Likely((int)length >= 8))
                    {
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 8;
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    }

                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 4;
                        ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    }

                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.insert_epi16(Xse.setzero_si128(), *(ushort*)ptr_v128, 0), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b0011))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 2;
                        ptr_v128 = (v128*)((short*)ptr_v128 + 1);
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.SBytes(*(sbyte*)ptr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.SBytes(ptr[i], value, where))
                        {
                            return true;
                        }
                        else continue;
                    }

                    return false;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi8((byte)value);
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx2.mm256_movemask_epi8(Compare.SBytes256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.loadu_si128(endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 8))
                    {
                        endPtr_v256 = (v256*)((long*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.cvtsi64x_si128(*(long*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 4))
                    {
                        endPtr_v256 = (v256*)((int*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.cvtsi32_si128(*(int*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 2))
                    {
                        endPtr_v256 = (v256*)((ushort*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.cvtsi32_si128(*(ushort*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b0011))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((sbyte*)endPtr_v256 - 1);

                        return Compare.SBytes(*(sbyte*)endPtr_v256, value, where);
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_epi8(value);
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.loadu_si128(endPtr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 4))
                    {
                        endPtr_v128 = (v128*)((int*)endPtr_v128 - 1);
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.cvtsi32_si128(*(int*)endPtr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 2))
                    {
                        endPtr_v128 = (v128*)((ushort*)endPtr_v128 - 1);
                        int mask = Xse.movemask_epi8(Compare.SBytes128(Xse.cvtsi32_si128(*(ushort*)endPtr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b0011))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((sbyte*)endPtr_v128 - 1);

                        return Compare.SBytes(*(sbyte*)endPtr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    length--;

                    while (length >= 0)
                    {
                        if (Compare.SBytes(ptr[length], value, where))
                        {
                            return true;
                        }
                        else
                        {
                            length--;
                        }
                    }

                    return false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<sbyte> array, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<sbyte> array, int index, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<sbyte> array, int index, int numEntries, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<sbyte> array, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<sbyte> array, int index, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<sbyte> array, int index, int numEntries, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<sbyte> array, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<sbyte> array, int index, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<sbyte> array, int index, int numEntries, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(short* ptr, long length, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi16(value);
                    v256* ptr_v256 = (v256*)ptr;

                    while (Hint.Likely(length >= 16))
                    {
                        int mask = Avx2.mm256_movemask_epi8(Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v256++;
                        length -= 16;
                    }

                    if (Hint.Likely((int)length >= 8))
                    {
                        int mask = Xse.movemask_epi8(Compare.Shorts128(Xse.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 8;
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    }

                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Xse.movemask_epi8(Compare.Shorts128(Xse.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 4;
                        ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    }

                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Xse.movemask_epi8(Compare.Shorts128(Xse.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 2;
                        ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.Shorts(*(short*)ptr_v256, value, where);
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_epi16(value);
                    v128* ptr_v128 = (v128*)ptr;

                    while (Hint.Likely(length >= 8))
                    {
                        int mask = Xse.movemask_epi8(Compare.Shorts128(Xse.loadu_si128(ptr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v128++;
                        length -= 8;
                    }

                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Xse.movemask_epi8(Compare.Shorts128(Xse.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 4;
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    }

                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Xse.movemask_epi8(Compare.Shorts128(Xse.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 2;
                        ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.Shorts(*(short*)ptr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.Shorts(ptr[i], value, where))
                        {
                            return true;
                        }
                        else continue;
                    }

                    return false;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi16(value);
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx2.mm256_movemask_epi8(Compare.Shorts256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.Shorts128(Xse.loadu_si128(endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 8))
                    {
                        endPtr_v256 = (v256*)((long*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.Shorts128(Xse.cvtsi64x_si128(*(long*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 4))
                    {
                        endPtr_v256 = (v256*)((int*)endPtr_v256 - 1);
                        int mask = Xse.movemask_epi8(Compare.Shorts128(Xse.cvtsi32_si128(*(int*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((short*)endPtr_v256 - 1);

                        return Compare.Shorts(*(short*)endPtr_v256, value, where);
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_epi16(value);
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Xse.movemask_epi8(Compare.Shorts128(Xse.loadu_si128(endPtr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Xse.movemask_epi8(Compare.Shorts128(Xse.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 4))
                    {
                        endPtr_v128 = (v128*)((int*)endPtr_v128 - 1);
                        int mask = Xse.movemask_epi8(Compare.Shorts128(Xse.cvtsi32_si128(*(int*)endPtr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((short*)endPtr_v128 - 1);

                        return Compare.Shorts(*(short*)endPtr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    length--;

                    while (length >= 0)
                    {
                        if (Compare.Shorts(ptr[length], value, where))
                        {
                            return true;
                        }
                        else
                        {
                            length--;
                        }
                    }

                    return false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<short> array, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<short> array, int index, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<short> array, int index, int numEntries, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<short> array, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<short> array, int index, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<short> array, int index, int numEntries, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<short> array, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<short> array, int index, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<short> array, int index, int numEntries, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(int* ptr, long length, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi32(value);
                    v256* ptr_v256 = (v256*)ptr;

                    while (Hint.Likely(length >= 8))
                    {
                        int mask = Avx2.mm256_movemask_epi8(Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v256++;
                        length -= 8;
                    }

                    if (Hint.Likely((int)length != 0))
                    {
                        v256 offsets = new v256(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u);
                        offsets = Avx2.mm256_min_epi32(offsets, Avx.mm256_set1_epi32((int)length - 1));
                        v256 lastLoad = Avx2.mm256_i32gather_epi32(ptr_v256, offsets, sizeof(int));

                        int mask = Avx2.mm256_movemask_epi8(Compare.Ints256(lastLoad, broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_epi32(value);
                    v128* ptr_v128 = (v128*)ptr;

                    while (Hint.Likely(length >= 4))
                    {
                        int mask = Xse.movemask_epi8(Compare.Ints128(Xse.loadu_si128(ptr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v128++;
                        length -= 4;
                    }

                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Xse.movemask_epi8(Compare.Ints128(Xse.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        length -= 2;
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.Ints(*(int*)ptr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.Ints(ptr[i], value, where))
                        {
                            return true;
                        }
                        else continue;
                    }

                    return false;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi32(value);
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx2.mm256_movemask_epi8(Compare.Ints256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        v256 offsets = new v256(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u);
                        offsets = Avx2.mm256_min_epi32(offsets, Avx.mm256_set1_epi32((int)length - 1));
                        v256 lastLoad = Avx2.mm256_i32gather_epi32(ptr, offsets, sizeof(int));

                        int mask = Avx2.mm256_movemask_epi8(Compare.Ints256(lastLoad, broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_epi32(value);
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Xse.movemask_epi8(Compare.Ints128(Xse.loadu_si128(endPtr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Xse.movemask_epi8(Compare.Ints128(Xse.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((int*)endPtr_v128 - 1);

                        return Compare.Ints(*(int*)endPtr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    length--;

                    while (length >= 0)
                    {
                        if (Compare.Ints(ptr[length], value, where))
                        {
                            return true;
                        }
                        else
                        {
                            length--;
                        }
                    }

                    return false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<int> array, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<int> array, int index, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<int> array, int index, int numEntries, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<int> array, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<int> array, int index, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<int> array, int index, int numEntries, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<int> array, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<int> array, int index, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<int> array, int index, int numEntries, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(long* ptr, long length, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi64x(value);
                    v256* ptr_v256 = (v256*)ptr;

                    while (Hint.Likely(length >= 4))
                    {
                        int mask = Avx2.mm256_movemask_epi8(Compare.Longs256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v256++;
                        length -= 4;
                    }

                    if (Hint.Likely((int)length != 0))
                    {
                        v256 offsets = new v256(0ul, 1ul, 2ul, 3ul);
                        offsets = Xse.mm256_min_epi64(offsets, Avx.mm256_set1_epi64x(length - 1));
                        v256 lastLoad = Avx2.mm256_i64gather_epi64(ptr_v256, offsets, sizeof(long));

                        int mask = Avx2.mm256_movemask_epi8(Compare.Longs256(lastLoad, broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
                else if (BurstArchitecture.IsCMP64Supported)
                {
                    v128 broadcast = Xse.set1_epi64x(value);
                    v128* ptr_v128 = (v128*)ptr;

                    while (Hint.Likely(length >= 2))
                    {
                        int mask = Xse.movemask_epi8(Compare.Longs128(Xse.loadu_si128(ptr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        ptr_v128++;
                        length -= 2;
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.Longs(*(long*)ptr_v128, value, where);
                    }

                    return false;
                }
                else if (Sse4_1.IsSse41Supported)
                {
                    if (where == Comparison.EqualTo || where == Comparison.NotEqualTo)
                    {
                        v128 broadcast = Xse.set1_epi64x(value);
                        v128* ptr_v128 = (v128*)ptr;

                        while (Hint.Likely(length >= 2))
                        {
                            int mask = Xse.movemask_epi8(Compare.Longs128(Xse.loadu_si128(ptr_v128), broadcast, where));

                            if (where == Comparison.NotEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0xFFFF))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    return true;
                                }
                            }

                            ptr_v128++;
                            length -= 2;
                        }

                        if (Hint.Likely(length != 0))
                        {
                            return Compare.Longs(*(long*)ptr_v128, value, where);
                        }

                        return false;
                    }
                    else
                    {
                        for (long i = 0; i < length; i++)
                        {
                            if (Compare.Longs(ptr[i], value, where))
                            {
                                return true;
                            }
                            else continue;
                        }

                        return false;
                    }
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.Longs(ptr[i], value, where))
                        {
                            return true;
                        }
                        else continue;
                    }

                    return false;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi64x(value);
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx2.mm256_movemask_epi8(Compare.Longs256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        v256 offsets = new v256(0ul, 1ul, 2ul, 3ul);
                        offsets = Xse.mm256_min_epi64(offsets, Avx.mm256_set1_epi64x(length - 1));
                        v256 lastLoad = Avx2.mm256_i64gather_epi64(ptr, offsets, sizeof(long));

                        int mask = Avx2.mm256_movemask_epi8(Compare.Longs256(lastLoad, broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
                else if (BurstArchitecture.IsCMP64Supported)
                {
                    v128 broadcast = Xse.set1_epi64x(value);
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Xse.movemask_epi8(Compare.Longs128(Xse.loadu_si128(endPtr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);

                        return Compare.Longs(*(long*)endPtr_v128, value, where);
                    }

                    return false;
                }
                else if (Sse4_1.IsSse41Supported)
                {
                    if (where == Comparison.EqualTo || where == Comparison.NotEqualTo)
                    {
                        v128 broadcast = Xse.set1_epi64x(value);
                        v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                        while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                        {
                            endPtr_v128--;
                            int mask = Xse.movemask_epi8(Compare.Longs128(Xse.loadu_si128(endPtr_v128), broadcast, where));

                            if (where == Comparison.NotEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0xFFFF))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    return true;
                                }
                            }
                        }

                        if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                        {
                            endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);

                            return Compare.Longs(*(long*)endPtr_v128, value, where);
                        }

                        return false;
                    }
                    else
                    {
                        length--;

                        while (length >= 0)
                        {
                            if (Compare.Longs(ptr[length], value, where))
                            {
                                return true;
                            }
                            else
                            {
                                length--;
                            }
                        }

                        return false;
                    }
                }
                else
                {
                    length--;

                    while (length >= 0)
                    {
                        if (Compare.Longs(ptr[length], value, where))
                        {
                            return true;
                        }
                        else
                        {
                            length--;
                        }
                    }

                    return false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<long> array, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<long> array, int index, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<long> array, int index, int numEntries, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<long> array, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<long> array, int index, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<long> array, int index, int numEntries, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<long> array, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<long> array, int index, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<long> array, int index, int numEntries, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(float* ptr, long length, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);
Assert.IsFalse(math.isnan(value));

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx.IsAvxSupported)
                {
                    v256 broadcast = Avx.mm256_set1_ps(value);
                    v256* ptr_v256 = (v256*)ptr;

                    while (Hint.Likely(length >= 8))
                    {
                        int mask = Avx.mm256_movemask_ps(Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256), broadcast, where));

                        if (Hint.Unlikely(mask != 0))
                        {
                            return true;
                        }
                        else
                        {
                            ptr_v256++;
                            length -= 8;
                        }
                    }

                    if (Avx2.IsAvx2Supported)
                    {
                        if (Hint.Likely((int)length != 0))
                        {
                            v256 offsets = new v256(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u);
                            offsets = Avx2.mm256_min_epi32(offsets, Avx.mm256_set1_epi32((int)length - 1));
                            v256 lastLoad = Avx2.mm256_i32gather_ps(ptr_v256, offsets, sizeof(float));

                            int mask = Avx2.mm256_movemask_epi8(Compare.Floats256(lastLoad, broadcast, where));

                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (Hint.Likely((int)length >= 4))
                        {
                            int mask = Xse.movemask_ps(Compare.Floats128(*(v128*)ptr_v256, Avx.mm256_castps256_ps128(broadcast), where));

                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                            else
                            {
                                length -= 4;
                                ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            }
                        }

                        if (Hint.Likely((int)length >= 2))
                        {
                            int mask = Xse.movemask_ps(Compare.Floats128(Xse.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castps256_ps128(broadcast), where));

                            if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                            {
                                ;
                            }
                            else
                            {
                                mask &= 0b0011;
                            }

                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                            else
                            {
                                length -= 2;
                                ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            }
                        }

                        if (Hint.Likely(length != 0))
                        {
                            return Compare.Floats(*(float*)ptr_v256, value, where);
                        }
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_ps(value);
                    v128* ptr_v128 = (v128*)ptr;

                    while (Hint.Likely(length >= 4))
                    {
                        int mask = Xse.movemask_ps(Compare.Floats128(*ptr_v128, broadcast, where));

                        if (Hint.Unlikely(mask != 0))
                        {
                            return true;
                        }
                        else
                        {
                            ptr_v128++;
                            length -= 4;
                        }
                    }

                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Xse.movemask_ps(Compare.Floats128(Xse.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (Hint.Unlikely(mask != 0))
                        {
                            return true;
                        }
                        else
                        {
                            length -= 2;
                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                        }
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.Floats(*(float*)ptr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.Floats(ptr[i], value, where))
                        {
                            return true;
                        }
                        else continue;
                    }

                    return false;
                }
            }
            else
            {
                if (Avx.IsAvxSupported)
                {
                    v256 broadcast = Avx.mm256_set1_ps(value);
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx.mm256_movemask_ps(Compare.Floats256(Avx.mm256_loadu_ps(endPtr_v256), broadcast, where));

                        if (Hint.Unlikely(mask != 0))
                        {
                            return true;
                        }
                    }

                    if (Avx2.IsAvx2Supported)
                    {
                        if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                        {
                            v256 offsets = new v256(0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u);
                            offsets = Avx2.mm256_min_epi32(offsets, Avx.mm256_set1_epi32((int)length - 1));
                            v256 lastLoad = Avx2.mm256_i32gather_ps(ptr, offsets, sizeof(float));

                            int mask = Avx2.mm256_movemask_epi8(Compare.Floats256(lastLoad, broadcast, where));

                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                        {
                            endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                            int mask = Xse.movemask_ps(Compare.Floats128(*(v128*)endPtr_v256, Avx.mm256_castps256_ps128(broadcast), where));

                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 8))
                        {
                            endPtr_v256 = (v256*)((long*)endPtr_v256 - 1);
                            int mask = Xse.movemask_ps(Compare.Floats128(Xse.cvtsi64x_si128(*(long*)endPtr_v256), Avx.mm256_castps256_ps128(broadcast), where));

                            if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                            {
                                ;
                            }
                            else
                            {
                                mask &= 0b0011;
                            }

                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                        {
                            endPtr_v256 = (v256*)((float*)endPtr_v256 - 1);

                            return Compare.Floats(*(float*)endPtr_v256, value, where);
                        }
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_ps(value);
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Xse.movemask_ps(Compare.Floats128(*endPtr_v128, broadcast, where));

                        if (Hint.Unlikely(mask != 0))
                        {
                            return true;
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Xse.movemask_ps(Compare.Floats128(Xse.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));

                        if (constexpr.IS_CONST(where) && constexpr.IS_CONST(value) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (Hint.Unlikely(mask != 0))
                        {
                            return true;
                        }
                    }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((float*)endPtr_v128 - 1);

                        return Compare.Floats(*(float*)endPtr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    length--;

                    while (length >= 0)
                    {
                        if (Compare.Floats(ptr[length], value, where))
                        {
                            return true;
                        }
                        else
                        {
                            length--;
                        }
                    }

                    return false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<float> array, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<float> array, int index, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<float> array, int index, int numEntries, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<float> array, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<float> array, int index, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<float> array, int index, int numEntries, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<float> array, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<float> array, int index, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<float> array, int index, int numEntries, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(double* ptr, long length, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);
Assert.IsFalse(math.isnan(value));

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx.IsAvxSupported)
                {
                    v256 broadcast = Avx.mm256_set1_pd(value);
                    v256* ptr_v256 = (v256*)ptr;

                    while (Hint.Likely(length >= 4))
                    {
                        int mask = Avx.mm256_movemask_pd(Compare.Doubles256(Avx.mm256_loadu_pd(ptr_v256), broadcast, where));

                        if (Hint.Unlikely(mask != 0))
                        {
                            return true;
                        }
                        else
                        {
                            ptr_v256++;
                            length -= 4;
                        }
                    }

                    if (Avx2.IsAvx2Supported)
                    {
                        if (Hint.Likely((int)length != 0))
                        {
                            v256 offsets = new v256(0ul, 1ul, 2ul, 3ul);
                            offsets = Xse.mm256_min_epi64(offsets, Avx.mm256_set1_epi64x(length - 1));
                            v256 lastLoad = Avx2.mm256_i64gather_pd(ptr_v256, offsets, sizeof(double));

                            int mask = Avx2.mm256_movemask_epi8(Compare.Doubles256(lastLoad, broadcast, where));

                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (Hint.Likely((int)length >= 2))
                        {
                            int mask = Xse.movemask_pd(Compare.Doubles128(*(v128*)ptr_v256, Avx.mm256_castpd256_pd128(broadcast), where));

                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                            else
                            {
                                ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                                length -= 2;
                            }
                        }

                        if (Hint.Likely(length != 0))
                        {
                            return Compare.Doubles(*(double*)ptr_v256, value, where);
                        }
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_pd(value);
                    v128* ptr_v128 = (v128*)ptr;

                    while (Hint.Likely(length >= 2))
                    {
                        int mask = Xse.movemask_pd(Compare.Doubles128(*ptr_v128, broadcast, where));

                        if (Hint.Unlikely(mask != 0))
                        {
                            return true;
                        }
                        else
                        {
                            ptr_v128++;
                            length -= 2;
                        }
                    }

                    if (Hint.Likely(length != 0))
                    {
                        return Compare.Doubles(*(double*)ptr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.Doubles(ptr[i], value, where))
                        {
                            return true;
                        }
                        else continue;
                    }

                    return false;
                }
            }
            else
            {
                if (Avx.IsAvxSupported)
                {
                    v256 broadcast = Avx.mm256_set1_pd(value);
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx.mm256_movemask_pd(Compare.Doubles256(Avx.mm256_loadu_pd(endPtr_v256), broadcast, where));

                        if (Hint.Unlikely(mask != 0))
                        {
                            return true;
                        }
                    }

                    if (Avx2.IsAvx2Supported)
                    {
                        if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                        {
                            v256 offsets = new v256(0ul, 1ul, 2ul, 3ul);
                            offsets = Xse.mm256_min_epi64(offsets, Avx.mm256_set1_epi64x(length - 1));
                            v256 lastLoad = Avx2.mm256_i64gather_pd(ptr, offsets, sizeof(double));

                            int mask = Avx2.mm256_movemask_epi8(Compare.Doubles256(lastLoad, broadcast, where));

                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                        {
                            endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                            int mask = Xse.movemask_pd(Compare.Doubles128(*(v128*)endPtr_v256, Avx.mm256_castpd256_pd128(broadcast), where));

                            if (Hint.Unlikely(mask != 0))
                            {
                                return true;
                            }
                        }

                        if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                        {
                            endPtr_v256 = (v256*)((double*)endPtr_v256 - 1);

                            return Compare.Doubles(*(double*)endPtr_v256, value, where);
                        }
                    }

                    return false;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    v128 broadcast = Xse.set1_pd(value);
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Xse.movemask_pd(Compare.Doubles128(*endPtr_v128, broadcast, where));

                        if (Hint.Unlikely(mask != 0))
                        {
                            return true;
                        }
                    }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((double*)endPtr_v128 - 1);

                        return Compare.Doubles(*(double*)endPtr_v128, value, where);
                    }

                    return false;
                }
                else
                {
                    length--;

                    while (length >= 0)
                    {
                        if (Compare.Doubles(ptr[length], value, where))
                        {
                            return true;
                        }
                        else
                        {
                            length--;
                        }
                    }

                    return false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<double> array, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<double> array, int index, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<double> array, int index, int numEntries, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<double> array, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<double> array, int index, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<double> array, int index, int numEntries, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<double> array, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<double> array, int index, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<double> array, int index, int numEntries, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsValidSubarray(index, numEntries, array.Length);

            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }
    }
}