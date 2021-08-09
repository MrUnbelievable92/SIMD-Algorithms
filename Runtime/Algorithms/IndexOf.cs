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
    // further unrolling does NOT break dependencies and/or adds branches
    unsafe public static partial class Algorithms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_IndexOf(bool* ptr, long length, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
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
            return SIMD_IndexOf((byte*)ptr, length, *(byte*)&value, Comparison.EqualTo, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<bool> array, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<bool> array, int index, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<bool> array, int index, int numEntries, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<bool> array, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<bool> array, int index, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<bool> array, int index, int numEntries, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<bool> array, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<bool> array, int index, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<bool> array, int index, int numEntries, bool value, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_IndexOf(byte* ptr, long length, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi8(value);
                    v256* ptr_v256 = (v256*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 32))
                    {
                        int mask = Avx2.mm256_movemask_epi8(Compare.Bytes256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != -1))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }

                        ptr_v256++;
                        length -= 32;
                        index += 32;
                    }


                    if (Hint.Likely((int)length >= 16))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                        length -= 16;
                        index += 16;
                    }
                    else { }


                    v128 cmp = default(v128);

                    if (Hint.Likely((int)length >= 8))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                        length -= 8;
                        index += 8;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                        length -= 4;
                        index += 4;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.insert_epi16(cmp, *(short*)ptr_v256, 0), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0001;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b0010;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((short*)ptr_v256 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.Bytes(*(byte*)ptr_v256, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_epi8((sbyte)value);
                    v128* ptr_v128 = (v128*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 16))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.loadu_si128(ptr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v128++;
                        length -= 16;
                        index += 16;
                    }


                    if (Hint.Likely((int)length >= 8))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                        length -= 8;
                        index += 8;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                        length -= 4;
                        index += 4;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.insert_epi16(default(v128), *(short*)ptr_v128, 0), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0001;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b0010;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }

                        ptr_v128 = (v128*)((short*)ptr_v128 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.Bytes(*(byte*)ptr_v128, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.Bytes(ptr[i], value, where))
                        {
                            return i;
                        }
                        else continue;
                    }

                    return -1;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi8(value);
                    long index = length - 1;
                    long result = -1;
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx2.mm256_movemask_epi8(Compare.Bytes256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(~mask);

                            if (Hint.Unlikely(mask != -1))
                            {
                                result = index - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 32;
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.loadu_si128(endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0xFFFF ^ mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = (index + 16) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 16) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 16;
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 8))
                    {
                        endPtr_v256 = (v256*)((long*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.cvtsi64x_si128(*(long*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111_1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = (index + 24) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 24) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 8;
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 4))
                    {
                        endPtr_v256 = (v256*)((int*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.cvtsi32_si128(*(int*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 2))
                    {
                        endPtr_v256 = (v256*)((ushort*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.cvtsi32_si128(*(ushort*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0001;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b0010;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((byte*)endPtr_v256 - 1);

                        if (Compare.Bytes(*(byte*)endPtr_v256, value, where))
                        {
                            result = 0;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_epi8((sbyte)value);
                    long index = length - 1;
                    long result = -1;
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.loadu_si128(endPtr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0xFFFF ^ mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = (index + 16) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 16) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 16;
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111_1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = (index + 24) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 24) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 8;
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 4))
                    {
                        endPtr_v128 = (v128*)((int*)endPtr_v128 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.cvtsi32_si128(*(int*)endPtr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 2))
                    {
                        endPtr_v128 = (v128*)((ushort*)endPtr_v128 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Bytes128(Sse2.cvtsi32_si128(*(ushort*)endPtr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0001;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b0010;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((byte*)endPtr_v128 - 1);

                        if (Compare.Bytes(*(byte*)endPtr_v128, value, where))
                        {
                            result = 0;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    long i = length - 1;

                    while (i >= 0)
                    {
                        if (Compare.Bytes(ptr[i], value, where))
                        {
                            goto Found;
                        }
                        else
                        {
                            i--;
                        }
                    }

                Found:
                    return i;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<byte> array, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<byte> array, int index, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<byte> array, int index, int numEntries, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<byte> array, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<byte> array, int index, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<byte> array, int index, int numEntries, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<byte> array, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<byte> array, int index, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<byte> array, int index, int numEntries, byte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_IndexOf(ushort* ptr, long length, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi16((short)value);
                    v256* ptr_v256 = (v256*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 16))
                    {
                        int mask = Avx2.mm256_movemask_epi8(Compare.UShorts256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != -1))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        
                        ptr_v256++;
                        length -= 16;
                        index += 16;
                    }


                    if (Hint.Likely((int)length >= 8))
                    {
                        int mask = Sse2.movemask_epi8(Compare.UShorts128(Avx.mm256_castsi256_si128(broadcast), Sse2.loadu_si128(ptr_v256), where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = index + (math.tzcnt(~mask) >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                        length -= 8;
                        index += 8;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Sse2.movemask_epi8(Compare.UShorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                        length -= 4;
                        index += 4;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.UShorts128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0011;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b1100;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.UShorts(*(ushort*)ptr_v256, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_epi16((short)value);
                    v128* ptr_v128 = (v128*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 8))
                    {
                        int mask = Sse2.movemask_epi8(Compare.UShorts128(Sse2.loadu_si128(ptr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        
                        ptr_v128++;
                        length -= 8;
                        index += 8;
                    }


                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Sse2.movemask_epi8(Compare.UShorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                        length -= 4;
                        index += 4;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.UShorts128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0011;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b1100;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        
                        ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.UShorts(*(ushort*)ptr_v128, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.UShorts(ptr[i], value, where))
                        {
                            return i;
                        }
                        else continue;
                    }

                    return -1;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi16((short)value);
                    long index = length - 1;
                    long result = -1;
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx2.mm256_movemask_epi8(Compare.UShorts256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                result = index - (math.lzcnt(~mask) >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        
                        index -= 16;
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.UShorts128(Sse2.loadu_si128(endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0xFFFF ^ mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = (index + 8) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 8) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        
                        index -= 8;
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 8))
                    {
                        endPtr_v256 = (v256*)((long*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.UShorts128(Sse2.cvtsi64x_si128(*(long*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111_1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = (index + 12) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 12) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 4))
                    {
                        endPtr_v256 = (v256*)((int*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.UShorts128(Sse2.cvtsi32_si128(*(int*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0011;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheFirst = mask == 0b1100;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheFirst;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((ushort*)endPtr_v256 - 1);

                        if (Compare.UShorts(*(ushort*)endPtr_v256, value, where))
                        {
                            result = 0;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_epi16((short)value);
                    long index = length - 1;
                    long result = -1;
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Sse2.movemask_epi8(Compare.UShorts128(Sse2.loadu_si128(endPtr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0xFFFF ^ mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = (index + 8) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 8) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        
                        index -= 8;
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Sse2.movemask_epi8(Compare.UShorts128(Sse2.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111_1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = (index + 12) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 12) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 4))
                    {
                        endPtr_v128 = (v128*)((int*)endPtr_v128 - 1);
                        int mask = Sse2.movemask_epi8(Compare.UShorts128(Sse2.cvtsi32_si128(*(int*)endPtr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0011;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b1100;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((ushort*)endPtr_v128 - 1);

                        if (Compare.UShorts(*(ushort*)endPtr_v128, value, where))
                        {
                            result = 0;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    long i = length - 1;

                    while (i >= 0)
                    {
                        if (Compare.UShorts(ptr[i], value, where))
                        {
                            goto Found;
                        }
                        else
                        {
                            i--;
                        }
                    }

                Found:
                    return i;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<ushort> array, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<ushort> array, int index, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<ushort> array, int index, int numEntries, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<ushort> array, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<ushort> array, int index, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<ushort> array, int index, int numEntries, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<ushort> array, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<ushort> array, int index, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<ushort> array, int index, int numEntries, ushort value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_IndexOf(uint* ptr, long length, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi32((int)value);
                    v256* ptr_v256 = (v256*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 8))
                    {
                        int mask = Avx.mm256_movemask_ps(Compare.UInts256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v256++;
                        length -= 8;
                        index += 8;
                    }


                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Sse.movemask_ps(Compare.UInts128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                        length -= 4;
                        index += 4;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse.movemask_ps(Compare.UInts128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0001;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b0010;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.UInts(*(uint*)ptr_v256, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_epi32((int)value);
                    v128* ptr_v128 = (v128*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 4))
                    {
                        int mask = Sse.movemask_ps(Compare.UInts128(Sse2.loadu_si128(ptr_v128), broadcast, where));

                        if (Sse4_1.IsSse41Supported)
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                long nonHits = (uint)math.tzcnt(~mask);

                                if (Hint.Unlikely(mask != 0b1111))
                                {
                                    result = index + nonHits;
                                    goto Found;
                                }
                            }
                            else
                            {
                                long nonHits = (uint)math.tzcnt(mask);

                                if (Hint.Unlikely(mask != 0))
                                {
                                    result = index + nonHits;
                                    goto Found;
                                }
                            }
                        }
                        else
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                long nonHits = (uint)math.tzcnt(~mask);

                                if (Hint.Unlikely(mask != 0b1111))
                                {
                                    result = index + nonHits;
                                    goto Found;
                                }
                            }
                            else
                            {
                                long nonHits = (uint)math.tzcnt(mask);

                                if (Hint.Unlikely(mask != 0))
                                {
                                    result = index + nonHits;
                                    goto Found;
                                }
                            }
                        }
                        
                        ptr_v128++;
                        length -= 4;
                        index += 4;
                    }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.UInts128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                    bool onlyTheSecond = mask == 0x000F;
                                    result = index + *(byte*)&onlyTheSecond;
                                    goto Found;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    bool onlyTheSecond = mask == 0x00F0;
                                    result = index + *(byte*)&onlyTheSecond;
                                    goto Found;
                                }
                            }
                        }
                        else
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0b1111_1111))
                                {
                                    bool onlyTheSecond = mask == 0x000F;
                                    result = index + *(byte*)&onlyTheSecond;
                                    goto Found;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    bool onlyTheSecond = mask == 0x00F0;
                                    result = index + *(byte*)&onlyTheSecond;
                                    goto Found;
                                }
                            }
                        }
                        
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.UInts(*(uint*)ptr_v128, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.UInts(ptr[i], value, where))
                        {
                            return i;
                        }
                        else continue;
                    }

                    return -1;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi32((int)value);
                    long index = length - 1;
                    long result = -1;
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx.mm256_movemask_ps(Compare.UInts256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111_1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = (index + 24) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 24) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 8;
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Sse.movemask_ps(Compare.UInts128(Sse2.loadu_si128(endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 8))
                    {
                        endPtr_v256 = (v256*)((long*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.UInts128(Sse2.cvtsi64x_si128(*(long*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0x000F;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0x00F0;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((uint*)endPtr_v256 - 1);

                        if (Compare.UInts(*(uint*)endPtr_v256, value, where))
                        {
                            result = 0;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_epi32((int)value);
                    long index = length - 1;
                    long result = -1;
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Sse.movemask_ps(Compare.UInts128(Sse2.loadu_si128(endPtr_v128), broadcast, where));

                        if (Sse4_1.IsSse41Supported)
                        {
                            if (where == Comparison.NotEqualTo)
                            {
                                long nonHitsWithOffset = math.lzcnt(0b1111 ^ mask);

                                if (Hint.Unlikely(mask != 0b1111))
                                {
                                    result = (index + 28) - nonHitsWithOffset;
                                    goto Found;
                                }
                            }
                            else
                            { 
                                long nonHitsWithOffset = math.lzcnt(mask);

                                if (Hint.Unlikely(mask != 0))
                                {
                                    result = (index + 28) - nonHitsWithOffset;
                                    goto Found;
                                }
                            }
                        }
                        else
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                long nonHitsWithOffset = math.lzcnt(0b1111 ^ mask);

                                if (Hint.Unlikely(mask != 0b1111))
                                {
                                    result = (index + 28) - nonHitsWithOffset;
                                    goto Found;
                                }
                            }
                            else
                            {
                                long nonHitsWithOffset = math.lzcnt(mask);

                                if (Hint.Unlikely(mask != 0))
                                {
                                    result = (index + 28) - nonHitsWithOffset;
                                    goto Found;
                                }
                            }
                        }
                        
                        index -= 4;
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Sse2.movemask_epi8(Compare.UInts128(Sse2.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                    bool onlyTheSecond = mask == 0x000F;
                                    result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                    goto Found;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    bool onlyTheSecond = mask == 0x00F0;
                                    result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                    goto Found;
                                }
                            }
                        }
                        else
                        {
                            if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                            {
                                if (Hint.Unlikely((byte)mask != 0b1111_1111))
                                {
                                    bool onlyTheSecond = mask == 0x000F;
                                    result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                    goto Found;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    bool onlyTheSecond = mask == 0x00F0;
                                    result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                    goto Found;
                                }
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((uint*)endPtr_v128 - 1);

                        if (Compare.UInts(*(uint*)endPtr_v128, value, where))
                        {
                            result = 0;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    long i = length - 1;

                    while (i >= 0)
                    {
                        if (Compare.UInts(ptr[i], value, where))
                        {
                            goto Found;
                        }
                        else
                        {
                            i--;
                        }
                    }

                Found:
                    return i;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<uint> array, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<uint> array, int index, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<uint> array, int index, int numEntries, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<uint> array, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<uint> array, int index, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<uint> array, int index, int numEntries, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<uint> array, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<uint> array, int index, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<uint> array, int index, int numEntries, uint value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_IndexOf(ulong* ptr, long length, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi64x((long)value);
                    v256* ptr_v256 = (v256*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 4))
                    {
                        int mask = Avx.mm256_movemask_pd(Compare.ULongs256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v256++;
                        length -= 4;
                        index += 4;
                    }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.ULongs128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                bool onlyTheSecond = (ushort)mask == 0b1111_1111;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = (ushort)mask == 0xFF00;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.ULongs(*(ulong*)ptr_v256, value, where))
                        {
                            result = index;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse4_2.IsSse42Supported)
                {
                    v128 broadcast = Sse2.set1_epi64x((long)value);
                    v128* ptr_v128 = (v128*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.ULongs128(Sse2.loadu_si128(ptr_v128), broadcast, where));
                        
                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                bool onlyTheSecond = (ushort)mask == 0b1111_1111;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = (ushort)mask == 0xFF00;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        
                        ptr_v128++;
                        length -= 2;
                        index += 2;
                    }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.ULongs(*(ulong*)ptr_v128, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse4_1.IsSse41Supported)
                {
                    if (where == Comparison.EqualTo || where == Comparison.NotEqualTo)
                    {
                        v128 broadcast = Sse2.set1_epi64x((long)value);
                        v128* ptr_v128 = (v128*)ptr;
                        long index = 0;
                        long result = -1;

                        while (Hint.Likely(length >= 2))
                        {
                            int mask = Sse2.movemask_epi8(Compare.ULongs128(Sse2.loadu_si128(ptr_v128), broadcast, where));
                            
                            if (where == Comparison.NotEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0xFFFF))
                                {
                                    bool onlyTheSecond = (ushort)mask == 0b1111_1111;
                                    result = index + *(byte*)&onlyTheSecond;
                                    goto Found;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    bool onlyTheSecond = (ushort)mask == 0xFF00;
                                    result = index + *(byte*)&onlyTheSecond;
                                    goto Found;
                                }
                            }
                            
                            ptr_v128++;
                            length -= 2;
                            index += 2;
                        }


                        if (Hint.Likely(length != 0))
                        {
                            if (Compare.ULongs(*(ulong*)ptr_v128, value, where))
                            {
                                result = index;
                                goto Found;
                            }
                            else { }
                        }
                        else { }


                    Found:
                        return result;
                    }
                    else
                    {
                        for (long i = 0; i < length; i++)
                        {
                            if (Compare.ULongs(ptr[i], value, where))
                            {
                                return i;
                            }
                            else continue;
                        }

                        return -1;
                    }
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.ULongs(ptr[i], value, where))
                        {
                            return i;
                        }
                        else continue;
                    }

                    return -1;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi64x((long)value);
                    long index = length - 1;
                    long result = -1;
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx.mm256_movemask_pd(Compare.ULongs256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));
                        
                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 4;
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.ULongs128(Sse2.loadu_si128(endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                bool onlyTheSecond = (ushort)mask == 0x00FF;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = (ushort)mask == 0xFF00;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((ulong*)endPtr_v256 - 1);

                        if (Compare.ULongs(*(ulong*)endPtr_v256, value, where))
                        {
                            result = 0;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse4_2.IsSse42Supported)
                {
                    v128 broadcast = Sse2.set1_epi64x((long)value);
                    long index = length - 1;
                    long result = -1;
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Sse2.movemask_epi8(Compare.ULongs128(Sse2.loadu_si128(endPtr_v128), broadcast, where));
                        
                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                bool onlyTheFirst = (ushort)mask == 0xFF00;
                                result = index - *(byte*)&onlyTheFirst;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0)) 
                            {
                                bool onlyTheFirst = (ushort)mask == 0x00FF;
                                result = index - *(byte*)&onlyTheFirst;
                                goto Found;
                            }
                        }

                        index -= 2;
                    }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((ulong*)endPtr_v128 - 1);

                        if (Compare.ULongs(*(ulong*)endPtr_v128, value, where))
                        {
                            result = 0;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                
                else if (Sse4_1.IsSse41Supported)
                {
                    if (where == Comparison.EqualTo || where == Comparison.NotEqualTo)
                    {
                        v128 broadcast = Sse2.set1_epi64x((long)value);
                        long index = length - 1;
                        long result = -1;
                        v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                        while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                        {
                            endPtr_v128--;
                            int mask = Sse2.movemask_epi8(Compare.ULongs128(Sse2.loadu_si128(endPtr_v128), broadcast, where));
                            
                            if (where == Comparison.NotEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0xFFFF))
                                {
                                    bool onlyTheFirst = (ushort)mask == 0xFF00;
                                    result = index - *(byte*)&onlyTheFirst;
                                    goto Found;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0)) 
                                {
                                    bool onlyTheFirst = (ushort)mask == 0x00FF;
                                    result = index - *(byte*)&onlyTheFirst;
                                    goto Found;
                                }
                            }

                            index -= 2;
                        }

                        if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                        {
                            endPtr_v128 = (v128*)((ulong*)endPtr_v128 - 1);

                            if (Compare.ULongs(*(ulong*)endPtr_v128, value, where))
                            {
                                result = 0;
                                goto Found;
                            }
                            else { }
                        }
                        else { }


                    Found:
                        return result;
                    }
                    else
                    {
                        long i = length - 1;

                        while (i >= 0)
                        {
                            if (Compare.ULongs(ptr[i], value, where))
                            {
                                goto Found;
                            }
                            else
                            {
                                i--;
                            }
                        }

                    Found:
                        return i;
                    }
                }
                else
                {
                    long i = length - 1;

                    while (i >= 0)
                    {
                        if (Compare.ULongs(ptr[i], value, where))
                        {
                            goto Found;
                        }
                        else
                        {
                            i--;
                        }
                    }

                Found:
                    return i;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<ulong> array, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<ulong> array, int index, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<ulong> array, int index, int numEntries, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<ulong> array, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<ulong> array, int index, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<ulong> array, int index, int numEntries, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<ulong> array, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<ulong> array, int index, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<ulong> array, int index, int numEntries, ulong value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_IndexOf(sbyte* ptr, long length, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi8((byte)value);
                    v256* ptr_v256 = (v256*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 32))
                    {
                        int mask = Avx2.mm256_movemask_epi8(Compare.SBytes256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != -1))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }

                        ptr_v256++;
                        length -= 32;
                        index += 32;
                    }


                    if (Hint.Likely((int)length >= 16))
                    {
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                        length -= 16;
                        index += 16;
                    }
                    else { }


                    v128 cmp = default(v128);

                    if (Hint.Likely((int)length >= 8))
                    {
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                        length -= 8;
                        index += 8;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                        length -= 4;
                        index += 4;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.insert_epi16(cmp, *(short*)ptr_v256, 0), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0001;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b0010;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((short*)ptr_v256 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.SBytes(*(sbyte*)ptr_v256, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_epi8(value);
                    v128* ptr_v128 = (v128*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 16))
                    {
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.loadu_si128(ptr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v128++;
                        length -= 16;
                        index += 16;
                    }


                    if (Hint.Likely((int)length >= 8))
                    {
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                        length -= 8;
                        index += 8;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                        length -= 4;
                        index += 4;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.insert_epi16(default(v128), *(short*)ptr_v128, 0), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0001;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b0010;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }

                        ptr_v128 = (v128*)((short*)ptr_v128 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.SBytes(*(sbyte*)ptr_v128, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.SBytes(ptr[i], value, where))
                        {
                            return i;
                        }
                        else continue;
                    }

                    return -1;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi8((byte)value);
                    long index = length - 1;
                    long result = -1;
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx2.mm256_movemask_epi8(Compare.SBytes256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(~mask);

                            if (Hint.Unlikely(mask != -1))
                            {
                                result = index - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 32;
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.loadu_si128(endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0xFFFF ^ mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = (index + 16) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 16) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 16;
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 8))
                    {
                        endPtr_v256 = (v256*)((long*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.cvtsi64x_si128(*(long*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111_1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = (index + 24) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 24) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 8;
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 4))
                    {
                        endPtr_v256 = (v256*)((int*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.cvtsi32_si128(*(int*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 2))
                    {
                        endPtr_v256 = (v256*)((ushort*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.cvtsi32_si128(*(ushort*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0001;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b0010;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((byte*)endPtr_v256 - 1);

                        if (Compare.SBytes(*(sbyte*)endPtr_v256, value, where))
                        {
                            result = 0;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_epi8((sbyte)value);
                    long index = length - 1;
                    long result = -1;
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.loadu_si128(endPtr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0xFFFF ^ mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = (index + 16) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 16) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 16;
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111_1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = (index + 24) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 24) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 8;
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 4))
                    {
                        endPtr_v128 = (v128*)((int*)endPtr_v128 - 1);
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.cvtsi32_si128(*(int*)endPtr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111;
                        }

                        if (where == Comparison.NotEqualTo  || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 2))
                    {
                        endPtr_v128 = (v128*)((ushort*)endPtr_v128 - 1);
                        int mask = Sse2.movemask_epi8(Compare.SBytes128(Sse2.cvtsi32_si128(*(ushort*)endPtr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0001;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b0010;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((byte*)endPtr_v128 - 1);

                        if (Compare.SBytes(*(sbyte*)endPtr_v128, value, where))
                        {
                            result = 0;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    long i = length - 1;

                    while (i >= 0)
                    {
                        if (Compare.SBytes(ptr[i], value, where))
                        {
                            goto Found;
                        }
                        else
                        {
                            i--;
                        }
                    }

                Found:
                    return i;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<sbyte> array, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<sbyte> array, int index, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<sbyte> array, int index, int numEntries, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<sbyte> array, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<sbyte> array, int index, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<sbyte> array, int index, int numEntries, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<sbyte> array, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<sbyte> array, int index, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<sbyte> array, int index, int numEntries, sbyte value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_IndexOf(short* ptr, long length, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi16(value);
                    v256* ptr_v256 = (v256*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 16))
                    {
                        int mask = Avx2.mm256_movemask_epi8(Compare.Shorts256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != -1))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        
                        ptr_v256++;
                        length -= 16;
                        index += 16;
                    }


                    if (Hint.Likely((int)length >= 8))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Shorts128(Avx.mm256_castsi256_si128(broadcast), Sse2.loadu_si128(ptr_v256), where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                        length -= 8;
                        index += 8;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Shorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                        length -= 4;
                        index += 4;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Shorts128(Sse2.cvtsi32_si128(*(int*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0011;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b1100;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.Shorts(*(short*)ptr_v256, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_epi16(value);
                    v128* ptr_v128 = (v128*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 8))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Shorts128(Sse2.loadu_si128(ptr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        
                        ptr_v128++;
                        length -= 8;
                        index += 8;
                    }


                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Shorts128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + (nonHits >> 1);
                                goto Found;
                            }
                        }
                        
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                        length -= 4;
                        index += 4;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Shorts128(Sse2.cvtsi32_si128(*(int*)ptr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0011;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b1100;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        
                        ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.Shorts(*(short*)ptr_v128, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.Shorts(ptr[i], value, where))
                        {
                            return i;
                        }
                        else continue;
                    }

                    return -1;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi16(value);
                    long index = length - 1;
                    long result = -1;
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx2.mm256_movemask_epi8(Compare.Shorts256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != -1))
                            {
                                result = index - (math.lzcnt(~mask) >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        
                        index -= 16;
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Shorts128(Sse2.loadu_si128(endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0xFFFF ^ mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = (index + 8) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 8) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        
                        index -= 8;
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 8))
                    {
                        endPtr_v256 = (v256*)((long*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Shorts128(Sse2.cvtsi64x_si128(*(long*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111_1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = (index + 12) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 12) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 4))
                    {
                        endPtr_v256 = (v256*)((int*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Shorts128(Sse2.cvtsi32_si128(*(int*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0011;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b1100;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((short*)endPtr_v256 - 1);

                        if (Compare.Shorts(*(short*)endPtr_v256, value, where))
                        {
                            result = 0;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_epi16(value);
                    long index = length - 1;
                    long result = -1;
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Sse2.movemask_epi8(Compare.Shorts128(Sse2.loadu_si128(endPtr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0xFFFF ^ mask);

                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                result = (index + 8) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 8) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        
                        index -= 8;
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Shorts128(Sse2.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b1111_1111;
                        }

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111_1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = (index + 12) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 12) - (nonHitsWithOffset >> 1);
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 4))
                    {
                        endPtr_v128 = (v128*)((int*)endPtr_v128 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Shorts128(Sse2.cvtsi32_si128(*(int*)endPtr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0b0011;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0b1100;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((short*)endPtr_v128 - 1);

                        if (Compare.Shorts(*(short*)endPtr_v128, value, where))
                        {
                            result = 0;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    long i = length - 1;

                    while (i >= 0)
                    {
                        if (Compare.Shorts(ptr[i], value, where))
                        {
                            goto Found;
                        }
                        else
                        {
                            i--;
                        }
                    }

                Found:
                    return i;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<short> array, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<short> array, int index, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<short> array, int index, int numEntries, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<short> array, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<short> array, int index, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<short> array, int index, int numEntries, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<short> array, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<short> array, int index, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<short> array, int index, int numEntries, short value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_IndexOf(int* ptr, long length, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi32((int)value);
                    v256* ptr_v256 = (v256*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 8))
                    {
                        int mask = Avx.mm256_movemask_ps(Compare.Ints256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v256++;
                        length -= 8;
                        index += 8;
                    }


                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Sse.movemask_ps(Compare.Ints128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                        length -= 4;
                        index += 4;
                    }
                    else { }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Ints128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0x000F;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0x00F0;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.Ints(*(int*)ptr_v256, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_epi32((int)value);
                    v128* ptr_v128 = (v128*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 4))
                    {
                        int mask = Sse.movemask_ps(Compare.Ints128(Sse2.loadu_si128(ptr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v128++;
                        length -= 4;
                        index += 4;
                    }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Ints128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0x000F;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0x00F0;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.Ints(*(int*)ptr_v128, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.Ints(ptr[i], value, where))
                        {
                            return i;
                        }
                        else continue;
                    }

                    return -1;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi32((int)value);
                    long index = length - 1;
                    long result = -1;
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx.mm256_movemask_ps(Compare.Ints256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111_1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111_1111))
                            {
                                result = (index + 24) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 24) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 8;
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Sse.movemask_ps(Compare.Ints128(Sse2.loadu_si128(endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 8))
                    {
                        endPtr_v256 = (v256*)((long*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Ints128(Sse2.cvtsi64x_si128(*(long*)endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0x000F;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0x00F0;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((int*)endPtr_v256 - 1);

                        if (Compare.Ints(*(int*)endPtr_v256, value, where))
                        {
                            result = 0;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_epi32((int)value);
                    long index = length - 1;
                    long result = -1;
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Sse.movemask_ps(Compare.Ints128(Sse2.loadu_si128(endPtr_v128), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        { 
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 4;
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Ints128(Sse2.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));
                        
                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
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
                                bool onlyTheSecond = mask == 0x000F;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = mask == 0x00F0;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((int*)endPtr_v128 - 1);

                        if (Compare.Ints(*(int*)endPtr_v128, value, where))
                        {
                            result = 0;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    long i = length - 1;

                    while (i >= 0)
                    {
                        if (Compare.Ints(ptr[i], value, where))
                        {
                            goto Found;
                        }
                        else
                        {
                            i--;
                        }
                    }

                Found:
                    return i;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<int> array, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<int> array, int index, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<int> array, int index, int numEntries, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<int> array, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<int> array, int index, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<int> array, int index, int numEntries, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<int> array, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<int> array, int index, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<int> array, int index, int numEntries, int value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_IndexOf(long* ptr, long length, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi64x(value);
                    v256* ptr_v256 = (v256*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 4))
                    {
                        int mask = Avx.mm256_movemask_pd(Compare.Longs256(Avx.mm256_loadu_si256(ptr_v256), broadcast, where));

                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHits = (uint)math.tzcnt(~mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHits = (uint)math.tzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = index + nonHits;
                                goto Found;
                            }
                        }
                        
                        ptr_v256++;
                        length -= 4;
                        index += 4;
                    }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Longs128(Sse2.loadu_si128(ptr_v256), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                bool onlyTheSecond = (ushort)mask == 0b1111_1111;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = (ushort)mask == 0xFF00;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                        length -= 2;
                        index += 2;
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.Longs(*(long*)ptr_v256, value, where))
                        {
                            result = index;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse4_2.IsSse42Supported)
                {
                    v128 broadcast = Sse2.set1_epi64x(value);
                    v128* ptr_v128 = (v128*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 2))
                    {
                        int mask = Sse2.movemask_epi8(Compare.Longs128(Sse2.loadu_si128(ptr_v128), broadcast, where));
                        
                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                bool onlyTheSecond = (ushort)mask == 0b1111_1111;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = (ushort)mask == 0xFF00;
                                result = index + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        
                        ptr_v128++;
                        length -= 2;
                        index += 2;
                    }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.Longs(*(long*)ptr_v128, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse4_1.IsSse41Supported)
                {
                    if (where == Comparison.EqualTo || where == Comparison.NotEqualTo)
                    {
                        v128 broadcast = Sse2.set1_epi64x(value);
                        v128* ptr_v128 = (v128*)ptr;
                        long index = 0;
                        long result = -1;

                        while (Hint.Likely(length >= 2))
                        {
                            int mask = Sse2.movemask_epi8(Compare.Longs128(Sse2.loadu_si128(ptr_v128), broadcast, where));
                            
                            if (where == Comparison.NotEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0xFFFF))
                                {
                                    bool onlyTheSecond = (ushort)mask == 0b1111_1111;
                                    result = index + *(byte*)&onlyTheSecond;
                                    goto Found;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0))
                                {
                                    bool onlyTheSecond = (ushort)mask == 0xFF00;
                                    result = index + *(byte*)&onlyTheSecond;
                                    goto Found;
                                }
                            }
                            
                            ptr_v128++;
                            length -= 2;
                            index += 2;
                        }


                        if (Hint.Likely(length != 0))
                        {
                            if (Compare.Longs(*(long*)ptr_v128, value, where))
                            {
                                result = index;
                                goto Found;
                            }
                            else { }
                        }
                        else { }


                    Found:
                        return result;
                    }
                    else
                    {
                        for (long i = 0; i < length; i++)
                        {
                            if (Compare.Longs(ptr[i], value, where))
                            {
                                return i;
                            }
                            else continue;
                        }

                        return -1;
                    }
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.Longs(ptr[i], value, where))
                        {
                            return i;
                        }
                        else continue;
                    }

                    return -1;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_epi64x(value);
                    long index = length - 1;
                    long result = -1;
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx.mm256_movemask_pd(Compare.Longs256(Avx.mm256_loadu_si256(endPtr_v256), broadcast, where));
                        
                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            long nonHitsWithOffset = math.lzcnt(0b1111 ^ mask);

                            if (Hint.Unlikely(mask != 0b1111))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        else
                        {
                            long nonHitsWithOffset = math.lzcnt(mask);

                            if (Hint.Unlikely(mask != 0))
                            {
                                result = (index + 28) - nonHitsWithOffset;
                                goto Found;
                            }
                        }
                        
                        index -= 4;
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_epi8(Compare.Longs128(Sse2.loadu_si128(endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                bool onlyTheSecond = (ushort)mask == 0x00FF;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0))
                            {
                                bool onlyTheSecond = (ushort)mask == 0xFF00;
                                result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                                goto Found;
                            }
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((long*)endPtr_v256 - 1);

                        if (Compare.Longs(*(long*)endPtr_v256, value, where))
                        {
                            result = 0;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse4_2.IsSse42Supported)
                {
                    v128 broadcast = Sse2.set1_epi64x(value);
                    long index = length - 1;
                    long result = -1;
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Sse2.movemask_epi8(Compare.Longs128(Sse2.loadu_si128(endPtr_v128), broadcast, where));
                        
                        if (where == Comparison.NotEqualTo || where == Comparison.GreaterThanOrEqualTo || where == Comparison.LessThanOrEqualTo)
                        {
                            if (Hint.Unlikely(mask != 0xFFFF))
                            {
                                bool onlyTheFirst = (ushort)mask == 0xFF00;
                                result = index - *(byte*)&onlyTheFirst;
                                goto Found;
                            }
                        }
                        else
                        {
                            if (Hint.Unlikely(mask != 0)) 
                            {
                                bool onlyTheFirst = (ushort)mask == 0x00FF;
                                result = index - *(byte*)&onlyTheFirst;
                                goto Found;
                            }
                        }

                        index -= 2;
                    }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);

                        if (Compare.Longs(*(long*)endPtr_v128, value, where))
                        {
                            result = 0;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                
                else if (Sse4_1.IsSse41Supported)
                {
                    if (where == Comparison.EqualTo || where == Comparison.NotEqualTo)
                    {
                        v128 broadcast = Sse2.set1_epi64x(value);
                        long index = length - 1;
                        long result = -1;
                        v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                        while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                        {
                            endPtr_v128--;
                            int mask = Sse2.movemask_epi8(Compare.Longs128(Sse2.loadu_si128(endPtr_v128), broadcast, where));
                            
                            if (where == Comparison.NotEqualTo)
                            {
                                if (Hint.Unlikely(mask != 0xFFFF))
                                {
                                    bool onlyTheFirst = (ushort)mask == 0xFF00;
                                    result = index - *(byte*)&onlyTheFirst;
                                    goto Found;
                                }
                            }
                            else
                            {
                                if (Hint.Unlikely(mask != 0)) 
                                {
                                    bool onlyTheFirst = (ushort)mask == 0x00FF;
                                    result = index - *(byte*)&onlyTheFirst;
                                    goto Found;
                                }
                            }

                            index -= 2;
                        }

                        if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                        {
                            endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);

                            if (Compare.Longs(*(long*)endPtr_v128, value, where))
                            {
                                result = 0;
                                goto Found;
                            }
                            else { }
                        }
                        else { }


                    Found:
                        return result;
                    }
                    else
                    {
                        long i = length - 1;

                        while (i >= 0)
                        {
                            if (Compare.Longs(ptr[i], value, where))
                            {
                                goto Found;
                            }
                            else
                            {
                                i--;
                            }
                        }

                    Found:
                        return i;
                    }
                }
                else
                {
                    long i = length - 1;

                    while (i >= 0)
                    {
                        if (Compare.Longs(ptr[i], value, where))
                        {
                            goto Found;
                        }
                        else
                        {
                            i--;
                        }
                    }

                Found:
                    return i;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<long> array, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<long> array, int index, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<long> array, int index, int numEntries, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<long> array, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<long> array, int index, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<long> array, int index, int numEntries, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<long> array, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<long> array, int index, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<long> array, int index, int numEntries, long value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_IndexOf(float* ptr, long length, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);
Assert.IsFalse(math.isnan(value));

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx.IsAvxSupported)
                {
                    v256 broadcast = Avx.mm256_set1_ps(value);
                    v256* ptr_v256 = (v256*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 8))
                    {
                        int mask = Avx.mm256_movemask_ps(Compare.Floats256(Avx.mm256_loadu_ps(ptr_v256), broadcast, where));
                        
                        long nonHits = (uint)math.tzcnt(mask);

                        if (Hint.Unlikely(mask != 0))
                        {
                            result = index + nonHits;
                            goto Found;
                        }
                        else
                        {
                            ptr_v256++;
                            length -= 8;
                            index += 8;
                        }
                    }


                    if (Hint.Likely((int)length >= 4))
                    {
                        int mask = Sse.movemask_ps(Compare.Floats128(Sse.loadu_ps(ptr_v256), Avx.mm256_castps256_ps128(broadcast), where));
                        
                        long nonHits = (uint)math.tzcnt(mask);

                        if (Hint.Unlikely(mask != 0))
                        {
                            result = index + nonHits;
                            goto Found;
                        }
                        else
                        {
                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 4;
                            index += 4;
                        }
                    }
                    else { }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse.movemask_ps(Compare.Floats128(Sse2.cvtsi64x_si128(*(long*)ptr_v256), Avx.mm256_castps256_ps128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (Hint.Unlikely(mask != 0))
                        {
                            bool onlyTheSecond = mask == 0b0010;
                            result = index + *(byte*)&onlyTheSecond;
                            goto Found;
                        }
                        else
                        {
                            ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                            length -= 2;
                            index += 2;
                        }
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.Floats(*(float*)ptr_v256, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse.set1_ps(value);
                    v128* ptr_v128 = (v128*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 4))
                    {
                        int mask = Sse.movemask_ps(Compare.Floats128(Sse.loadu_ps(ptr_v128), broadcast, where));
                        
                        long nonHits = (uint)math.tzcnt(mask);

                        if (Hint.Unlikely(mask != 0))
                        {
                            result = index + nonHits;
                            goto Found;
                        }
                        else
                        {
                            ptr_v128++;
                            length -= 4;
                            index += 4;
                        }
                    }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse.movemask_ps(Compare.Floats128(Sse2.cvtsi64x_si128(*(long*)ptr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (Hint.Unlikely(mask != 0))
                        {
                            bool onlyTheSecond = mask == 0b0010;
                            result = index + *(byte*)&onlyTheSecond;
                            goto Found;
                        }
                        else
                        {
                            ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                            length -= 2;
                            index += 2;
                        }
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.Floats(*(float*)ptr_v128, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.Floats(ptr[i], value, where))
                        {
                            return i;
                        }
                        else continue;
                    }

                    return -1;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    v256 broadcast = Avx.mm256_set1_ps(value);
                    long index = length - 1;
                    long result = -1;
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx.mm256_movemask_ps(Compare.Floats256(Avx.mm256_loadu_ps(endPtr_v256), broadcast, where));
                        
                        long nonHitsWithOffset = math.lzcnt(mask);

                        if (Hint.Unlikely(mask != 0))
                        {
                            result = (index + 24) - nonHitsWithOffset;
                            goto Found;
                        }
                        else
                        {
                            index -= 8;
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Sse.movemask_ps(Compare.Floats128(Sse.loadu_ps(endPtr_v256), Avx.mm256_castsi256_si128(broadcast), where));
                        
                        long nonHitsWithOffset = math.lzcnt(mask);

                        if (Hint.Unlikely(mask != 0))
                        {
                            result = (index + 28) - nonHitsWithOffset;
                            goto Found;
                        }
                    }
                    else { }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 8))
                    {
                        endPtr_v256 = (v256*)((long*)endPtr_v256 - 1);
                        int mask = Sse.movemask_ps(Compare.Floats128(Sse2.cvtsi64x_si128(*(long*)endPtr_v256), Avx.mm256_castps256_ps128(broadcast), where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (Hint.Unlikely(mask != 0))
                        {
                            bool onlyTheSecond = mask == 0b0010;
                            result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                            goto Found;
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((float*)endPtr_v256 - 1);

                        if (Compare.Floats(*(float*)endPtr_v256, value, where))
                        {
                            result = 0;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse.set1_ps(value);
                    long index = length - 1;
                    long result = -1;
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Sse.movemask_ps(Compare.Floats128(Sse.loadu_ps(endPtr_v128), broadcast, where));
                        
                        long nonHitsWithOffset = math.lzcnt(mask);

                        if (Hint.Unlikely(mask != 0))
                        {
                            result = (index + 28) - nonHitsWithOffset;
                            goto Found;
                        }
                        else
                        {
                            index -= 4;
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v128 - (long)ptr) >= 8))
                    {
                        endPtr_v128 = (v128*)((long*)endPtr_v128 - 1);
                        int mask = Sse.movemask_ps(Compare.Floats128(Sse2.cvtsi64x_si128(*(long*)endPtr_v128), broadcast, where));

                        if (Constant.IsConstantExpression(where) && !PartialVectors.ShouldMask(where, value))
                        {
                            ;
                        }
                        else
                        {
                            mask &= 0b0011;
                        }

                        if (Hint.Unlikely(mask != 0))
                        {
                            bool onlyTheSecond = mask == 0b0010;
                            result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                            goto Found;
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((float*)endPtr_v128 - 1);

                        if (Compare.Floats(*(float*)endPtr_v128, value, where))
                        {
                            result = 0;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    long i = length - 1;

                    while (i >= 0)
                    {
                        if (Compare.Floats(ptr[i], value, where))
                        {
                            goto Found;
                        }
                        else
                        {
                            i--;
                        }
                    }

                Found:
                    return i;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<float> array, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<float> array, int index, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<float> array, int index, int numEntries, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<float> array, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<float> array, int index, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<float> array, int index, int numEntries, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<float> array, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<float> array, int index, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<float> array, int index, int numEntries, float value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SIMD_IndexOf(double* ptr, long length, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(length);
Assert.IsFalse(math.isnan(value));

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx.IsAvxSupported)
                {
                    v256 broadcast = Avx.mm256_set1_pd(value);
                    v256* ptr_v256 = (v256*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 4))
                    {
                        int mask = Avx.mm256_movemask_pd(Compare.Doubles256(Avx.mm256_loadu_pd(ptr_v256), broadcast, where));
                        
                        long nonHits = (uint)math.tzcnt(mask);

                        if (Hint.Unlikely(mask != 0))
                        {
                            result = index + nonHits;
                            goto Found;
                        }
                        else
                        {
                            ptr_v256++;
                            length -= 4;
                            index += 4;
                        }
                    }


                    if (Hint.Likely((int)length >= 2))
                    {
                        int mask = Sse2.movemask_pd(Compare.Doubles128(Sse.loadu_ps(ptr_v256), Avx.mm256_castpd256_pd128(broadcast), where));

                        if (Hint.Unlikely(mask != 0))
                        {
                            bool onlyTheSecond = mask == 0b0010;
                            result = index + *(byte*)&onlyTheSecond;
                            goto Found;
                        }
                        else
                        {
                            ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                            length -= 2;
                            index += 2;
                        }
                    }
                    else { }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.Doubles(*(double*)ptr_v256, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_pd(value);
                    v128* ptr_v128 = (v128*)ptr;
                    long index = 0;
                    long result = -1;

                    while (Hint.Likely(length >= 2))
                    {
                        int mask = Sse2.movemask_pd(Compare.Doubles128(Sse.loadu_ps(ptr_v128), broadcast, where));

                        if (Hint.Unlikely(mask != 0))
                        {
                            bool onlyTheSecond = mask == 0b0010;
                            result = index + *(byte*)&onlyTheSecond;
                            goto Found;
                        }
                        else
                        {
                            ptr_v128++;
                            length -= 2;
                            index += 2;
                        }
                    }


                    if (Hint.Likely(length != 0))
                    {
                        if (Compare.Doubles(*(double*)ptr_v128, value, where))
                        {
                            result = index;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    for (long i = 0; i < length; i++)
                    {
                        if (Compare.Doubles(ptr[i], value, where))
                        {
                            return i;
                        }
                        else continue;
                    }

                    return -1;
                }
            }
            else
            {
                if (Avx.IsAvxSupported)
                {
                    v256 broadcast = Avx.mm256_set1_pd(value);
                    long index = length - 1;
                    long result = -1;
                    v256* endPtr_v256 = (v256*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v256 - (long)ptr >= 32))
                    {
                        endPtr_v256--;
                        int mask = Avx.mm256_movemask_pd(Compare.Doubles256(Avx.mm256_loadu_pd(endPtr_v256), broadcast, where));
                        
                        long nonHitsWithOffset = math.lzcnt(mask);

                        if (Hint.Unlikely(mask != 0))
                        {
                            result = (index + 28) - nonHitsWithOffset;
                            goto Found;
                        }
                        else
                        {
                            index -= 4;
                        }
                    }

                    if (Hint.Likely((int)((long)endPtr_v256 - (long)ptr) >= 16))
                    {
                        endPtr_v256 = (v256*)((v128*)endPtr_v256 - 1);
                        int mask = Sse2.movemask_pd(Compare.Doubles128(Sse.loadu_ps(endPtr_v256), Avx.mm256_castpd256_pd128(broadcast), where));

                        if (Hint.Unlikely(mask != 0))
                        {
                            bool onlyTheSecond = mask == 0b0010;
                            result = ((byte)length & (byte)1) + *(byte*)&onlyTheSecond;
                            goto Found;
                        }
                    }
                    else { }

                    if (Hint.Likely((long)endPtr_v256 != (long)ptr))
                    {
                        endPtr_v256 = (v256*)((double*)endPtr_v256 - 1);

                        if (Compare.Doubles(*(double*)endPtr_v256, value, where))
                        {
                            result = 0;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else if (Sse2.IsSse2Supported)
                {
                    v128 broadcast = Sse2.set1_pd(value);
                    long index = length - 1;
                    long result = -1;
                    v128* endPtr_v128 = (v128*)(ptr + length); // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr_v128 - (long)ptr >= 16))
                    {
                        endPtr_v128--;
                        int mask = Sse2.movemask_pd(Compare.Doubles128(Sse.loadu_ps(endPtr_v128), broadcast, where));

                        if (Hint.Unlikely(mask != 0))
                        {
                            bool onlyTheFirst = mask == 0b0001;
                            result = index - *(byte*)&onlyTheFirst;
                            goto Found;
                        }
                        else
                        {
                            index -= 2;
                        }
                    }

                    if (Hint.Likely((long)endPtr_v128 != (long)ptr))
                    {
                        endPtr_v128 = (v128*)((double*)endPtr_v128 - 1);

                        if (Compare.Doubles(*(double*)endPtr_v128, value, where))
                        {
                            result = 0;
                            goto Found;
                        }
                        else { }
                    }
                    else { }


                Found:
                    return result;
                }
                else
                {
                    long i = length - 1;

                    while (i >= 0)
                    {
                        if (Compare.Doubles(ptr[i], value, where))
                        {
                            goto Found;
                        }
                        else
                        {
                            i--;
                        }
                    }

                Found:
                    return i;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<double> array, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<double> array, int index, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeArray<double> array, int index, int numEntries, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<double> array, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<double> array, int index, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeList<double> array, int index, int numEntries, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<double> array, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
            return (int)SIMD_IndexOf((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<double> array, int index, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index, array.Length);

            return (int)SIMD_IndexOf((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value, where, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SIMD_IndexOf(this NativeSlice<double> array, int index, int numEntries, double value, Comparison where = Comparison.EqualTo, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsWithinArrayBounds(index + numEntries - 1, array.Length);

            return (int)SIMD_IndexOf((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value, where, traversalOrder);
        }
    }
}