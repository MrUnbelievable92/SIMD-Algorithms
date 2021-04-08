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
        public static bool SIMD_Contains(bool* ptr, long length, bool value)
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
            return SIMD_Contains((byte*)ptr, length, *(byte*)&value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<bool> array, bool value)
        {
            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<bool> array, int index, bool value)
        {
            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<bool> array, int index, int numEntries, bool value)
        {
            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<bool> array, bool value)
        {
            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<bool> array, int index, bool value)
        {
            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<bool> array, int index, int numEntries, bool value)
        {
            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<bool> array, bool value)
        {
            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<bool> array, int index, bool value)
        {
            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<bool> array, int index, int numEntries, bool value)
        {
            return SIMD_Contains((bool*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(byte* ptr, long length, byte value)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256 broadcast = Avx.mm256_set1_epi8(value);
                v256* ptr_v256 = (v256*)ptr;

                while (Hint.Likely(length >= 32))
                {
                    int mask = Avx2.mm256_movemask_epi8(Avx2.mm256_cmpeq_epi8(broadcast, Avx.mm256_loadu_si256(ptr_v256)));

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        ptr_v256++;
                        length -= 32;
                    }
                }


                if (length >= 16)
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(Avx.mm256_castsi256_si128(broadcast), Sse2.loadu_si128(ptr_v256)));

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        length -= 16;
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    }
                }
                else { }


                v128 cmp = default(v128);

                if (length >= 8)
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(Avx.mm256_castsi256_si128(broadcast), Sse2.cvtsi64x_si128(*(long*)ptr_v256)));
                    mask &= 0b1111_1111;

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        length -= 8;
                        ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    }
                }
                else { }


                if (length >= 4)
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(Avx.mm256_castsi256_si128(broadcast), Sse2.cvtsi32_si128(*(int*)ptr_v256)));
                    mask &= 0b1111;

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        length -= 4;
                        ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    }
                }
                else { }


                if (length >= 2)
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(Avx.mm256_castsi256_si128(broadcast), Sse2.insert_epi16(cmp, *(short*)ptr_v256, 0)));
                    mask &= 0b0011;

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        length -= 2;
                        ptr_v256 = (v256*)((short*)ptr_v256 + 1);
                    }
                }
                else { }


                if (length != 0)
                {
                    return *(byte*)ptr_v256 == value;
                }
                else { }


                return false;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128 broadcast = Sse2.set1_epi8((sbyte)value);
                v128* ptr_v128 = (v128*)ptr;

                while (Hint.Likely(length >= 16))
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(broadcast, Sse2.loadu_si128(ptr_v128)));

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        ptr_v128++;
                        length -= 16;
                    }
                }


                if (length >= 8)
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(broadcast, Sse2.cvtsi64x_si128(*(long*)ptr_v128)));
                    mask &= 0b1111_1111;

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        length -= 8;
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    }
                }
                else { }


                if (length >= 4)
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(broadcast, Sse2.cvtsi32_si128(*(int*)ptr_v128)));
                    mask &= 0b1111;

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        length -= 4;
                        ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    }
                }
                else { }


                if (length >= 2)
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi8(broadcast, Sse2.insert_epi16(default(v128), *(short*)ptr_v128, 0)));
                    mask &= 0b0011;

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        length -= 2;
                        ptr_v128 = (v128*)((short*)ptr_v128 + 1);
                    }
                }
                else { }


                if (length != 0)
                {
                    return *(byte*)ptr_v128 == value;
                }
                else { }


                return false;
            }
            else
            {
                for (long i = 0; i < length; i++)
                {
                    if (ptr[i] == value)
                    {
                        return true;
                    }
                    else continue;
                }

                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<byte> array, byte value)
        {
            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<byte> array, int index, byte value)
        {
            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<byte> array, int index, int numEntries, byte value)
        {
            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<byte> array, byte value)
        {
            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<byte> array, int index, byte value)
        {
            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<byte> array, int index, int numEntries, byte value)
        {
            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<byte> array, byte value)
        {
            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<byte> array, int index, byte value)
        {
            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<byte> array, int index, int numEntries, byte value)
        {
            return SIMD_Contains((byte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(ushort* ptr, long length, ushort value)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256 broadcast = Avx.mm256_set1_epi16((short)value);
                v256* ptr_v256 = (v256*)ptr;

                while (Hint.Likely(length >= 16))
                {
                    int mask = Avx2.mm256_movemask_epi8(Avx2.mm256_cmpeq_epi16(broadcast, Avx.mm256_loadu_si256(ptr_v256)));

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        ptr_v256++;
                        length -= 16;
                    }
                }


                if (length >= 8)
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi16(Avx.mm256_castsi256_si128(broadcast), Sse2.loadu_si128(ptr_v256)));

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else 
                    {
                        length -= 8;
                        ptr_v256 = (v256*)((v128*)ptr_v256 + 1);
                    }
                }
                else { }


                if (length >= 4)
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi16(Avx.mm256_castsi256_si128(broadcast), Sse2.cvtsi64x_si128(*(long*)ptr_v256)));
                    mask &= 0b1111_1111;

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        length -= 4;
                        ptr_v256 = (v256*)((long*)ptr_v256 + 1);
                    }
                }
                else { }


                if (length >= 2)
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi16(Avx.mm256_castsi256_si128(broadcast), Sse2.cvtsi32_si128(*(int*)ptr_v256)));
                    mask &= 0b1111;

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        length -= 2;
                        ptr_v256 = (v256*)((int*)ptr_v256 + 1);
                    }
                }
                else { }


                if (length != 0)
                {
                    return *(ushort*)ptr_v256 == value;
                }
                else { }


                return false;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128 broadcast = Sse2.set1_epi16((short)value);
                v128* ptr_v128 = (v128*)ptr;

                while (Hint.Likely(length >= 8))
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi16(broadcast, Sse2.loadu_si128(ptr_v128)));

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        ptr_v128++;
                        length -= 8;
                    }
                }


                if (length >= 4)
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi16(broadcast, Sse2.cvtsi64x_si128(*(long*)ptr_v128)));
                    mask &= 0b1111_1111;

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        length -= 4;
                        ptr_v128 = (v128*)((long*)ptr_v128 + 1);
                    }
                }
                else { }


                if (length >= 2)
                {
                    int mask = Sse2.movemask_epi8(Sse2.cmpeq_epi16(broadcast, Sse2.cvtsi32_si128(*(int*)ptr_v128)));
                    mask &= 0b1111;

                    if (Hint.Unlikely(mask != 0))
                    {
                        return true;
                    }
                    else
                    {
                        length -= 2;
                        ptr_v128 = (v128*)((int*)ptr_v128 + 1);
                    }
                }
                else { }


                if (length != 0)
                {
                    return *(ushort*)ptr_v128 == value;
                }
                else { }


                return false;
            }
            else
            {
                for (long i = 0; i < length; i++)
                {
                    if (ptr[i] == value)
                    {
                        return true;
                    }
                    else continue;
                }

                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<ushort> array, ushort value)
        {
            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<ushort> array, int index, ushort value)
        {
            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<ushort> array, int index, int numEntries, ushort value)
        {
            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<ushort> array, ushort value)
        {
            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<ushort> array, int index, ushort value)
        {
            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<ushort> array, int index, int numEntries, ushort value)
        {
            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<ushort> array, ushort value)
        {
            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<ushort> array, int index, ushort value)
        {
            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<ushort> array, int index, int numEntries, ushort value)
        {
            return SIMD_Contains((ushort*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(uint* ptr, long length, uint value)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256 broadcast = Avx.mm256_set1_epi32((int)value);
                v256* ptr_v256 = (v256*)ptr;

                while (Hint.Likely(length >= 8))
                {
                    int mask = Avx.mm256_movemask_ps(Avx2.mm256_cmpeq_epi32(broadcast, Avx.mm256_loadu_si256(ptr_v256)));

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


                if (length >= 4)
                {
                    int mask = Sse.movemask_ps(Sse2.cmpeq_epi32(Avx.mm256_castsi256_si128(broadcast), Sse2.loadu_si128(ptr_v256)));

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
                else { }


                if (length >= 2)
                {
                    int mask = Sse.movemask_ps(Sse2.cmpeq_epi32(Avx.mm256_castsi256_si128(broadcast), Sse2.cvtsi64x_si128(*(long*)ptr_v256)));
                    mask &= 0b0011;

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
                else { }


                if (length != 0)
                {
                    return *(uint*)ptr_v256 == value;
                }
                else { }


                return false;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128 broadcast = Sse2.set1_epi32((int)value);
                v128* ptr_v128 = (v128*)ptr;

                while (Hint.Likely(length >= 4))
                {
                    int mask = Sse.movemask_ps(Sse2.cmpeq_epi32(broadcast, Sse2.loadu_si128(ptr_v128)));

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


                if (length >= 2)
                {
                    int mask = Sse.movemask_ps(Sse2.cmpeq_epi32(broadcast, Sse2.cvtsi64x_si128(*(long*)ptr_v128)));
                    mask &= 0b0011;

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
                else { }


                if (length != 0)
                {
                    return *(uint*)ptr_v128 == value;
                }
                else { }


                return false;
            }
            else
            {
                for (long i = 0; i < length; i++)
                {
                    if (ptr[i] == value)
                    {
                        return true;
                    }
                    else continue;
                }

                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<uint> array, uint value)
        {
            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<uint> array, int index, uint value)
        {
            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<uint> array, int index, int numEntries, uint value)
        {
            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<uint> array, uint value)
        {
            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<uint> array, int index, uint value)
        {
            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<uint> array, int index, int numEntries, uint value)
        {
            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<uint> array, uint value)
        {
            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<uint> array, int index, uint value)
        {
            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<uint> array, int index, int numEntries, uint value)
        {
            return SIMD_Contains((uint*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(ulong* ptr, long length, ulong value)
        {
Assert.IsNonNegative(length);

            if (Avx2.IsAvx2Supported)
            {
                v256 broadcast = Avx.mm256_set1_epi64x((long)value);
                v256* ptr_v256 = (v256*)ptr;

                while (Hint.Likely(length >= 4))
                {
                    int mask = Avx.mm256_movemask_pd(Avx2.mm256_cmpeq_epi64(broadcast, Avx.mm256_loadu_si256(ptr_v256)));

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


                if (length >= 2)
                {
                    int mask = Sse2.movemask_pd(Sse4_1.cmpeq_epi64(Avx.mm256_castsi256_si128(broadcast), Sse2.loadu_si128(ptr_v256)));

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
                else { }


                if (length != 0)
                {
                    return *(ulong*)ptr_v256 == value;
                }
                else { }


                return false;
            }
            else if (Sse4_1.IsSse41Supported)
            {
                v128 broadcast = Sse2.set1_epi64x((long)value);
                v128* ptr_v128 = (v128*)ptr;

                while (Hint.Likely(length >= 2))
                {
                    int mask = Sse2.movemask_pd(Sse4_1.cmpeq_epi64(broadcast, Sse2.loadu_si128(ptr_v128)));

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


                if (length != 0)
                {
                    return *(ulong*)ptr_v128 == value;
                }
                else { }


                return false;
            }
            else
            {
                for (long i = 0; i < length; i++)
                {
                    if (ptr[i] == value)
                    {
                        return true;
                    }
                    else continue;
                }

                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<ulong> array, ulong value)
        {
            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<ulong> array, int index, ulong value)
        {
            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<ulong> array, int index, int numEntries, ulong value)
        {
            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<ulong> array, ulong value)
        {
            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<ulong> array, int index, ulong value)
        {
            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<ulong> array, int index, int numEntries, ulong value)
        {
            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<ulong> array, ulong value)
        {
            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<ulong> array, int index, ulong value)
        {
            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<ulong> array, int index, int numEntries, ulong value)
        {
            return SIMD_Contains((ulong*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(sbyte* ptr, long length, sbyte value)
        {
Assert.IsNonNegative(length);

            return SIMD_Contains((byte*)ptr, length, (byte)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<sbyte> array, sbyte value)
        {
            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<sbyte> array, int index, sbyte value)
        {
            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<sbyte> array, int index, int numEntries, sbyte value)
        {
            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<sbyte> array, sbyte value)
        {
            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<sbyte> array, int index, sbyte value)
        {
            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<sbyte> array, int index, int numEntries, sbyte value)
        {
            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<sbyte> array, sbyte value)
        {
            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<sbyte> array, int index, sbyte value)
        {
            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<sbyte> array, int index, int numEntries, sbyte value)
        {
            return SIMD_Contains((sbyte*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(short* ptr, long length, short value)
        {
Assert.IsNonNegative(length);

            return SIMD_Contains((ushort*)ptr, length, (ushort)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<short> array, short value)
        {
            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<short> array, int index, short value)
        {
            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<short> array, int index, int numEntries, short value)
        {
            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<short> array, short value)
        {
            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<short> array, int index, short value)
        {
            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<short> array, int index, int numEntries, short value)
        {
            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<short> array, short value)
        {
            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<short> array, int index, short value)
        {
            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<short> array, int index, int numEntries, short value)
        {
            return SIMD_Contains((short*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(int* ptr, long length, int value)
        {
Assert.IsNonNegative(length);

            return SIMD_Contains((uint*)ptr, length, (uint)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<int> array, int value)
        {
            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<int> array, int index, int value)
        {
            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<int> array, int index, int numEntries, int value)
        {
            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<int> array, int value)
        {
            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<int> array, int index, int value)
        {
            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<int> array, int index, int numEntries, int value)
        {
            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<int> array, int value)
        {
            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<int> array, int index, int value)
        {
            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<int> array, int index, int numEntries, int value)
        {
            return SIMD_Contains((int*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(long* ptr, long length, long value)
        {
Assert.IsNonNegative(length);

            return SIMD_Contains((ulong*)ptr, length, (ulong)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<long> array, long value)
        {
            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<long> array, int index, long value)
        {
            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<long> array, int index, int numEntries, long value)
        {
            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<long> array, long value)
        {
            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<long> array, int index, long value)
        {
            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<long> array, int index, int numEntries, long value)
        {
            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<long> array, long value)
        {
            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<long> array, int index, long value)
        {
            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<long> array, int index, int numEntries, long value)
        {
            return SIMD_Contains((long*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(float* ptr, long length, float value)
        {
Assert.IsNonNegative(length);
Assert.IsFalse(math.isnan(value));

            if (Avx.IsAvxSupported)
            {
                v256 broadcast = Avx.mm256_set1_ps(value);
                v256* ptr_v256 = (v256*)ptr;

                while (Hint.Likely(length >= 8))
                {
                    int mask = Avx.mm256_movemask_ps(Avx.mm256_cmp_ps(broadcast, Avx.mm256_loadu_ps(ptr_v256), (int)Avx.CMP.EQ_OQ));

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


                if (length >= 4)
                {
                    int mask = Sse.movemask_ps(Sse.cmpeq_ps(Avx.mm256_castps256_ps128(broadcast), Sse.loadu_ps(ptr_v256)));

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
                else { }


                if (length >= 2)
                {
                    int mask = Sse.movemask_ps(Sse.cmpeq_ps(Avx.mm256_castps256_ps128(broadcast), Sse2.cvtsi64x_si128(*(long*)ptr_v256)));
                    mask &= 0b0011;

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
                else { }


                if (length != 0)
                {
                    return *(float*)ptr_v256 == value;
                }
                else { }


                return false;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128 broadcast = Sse.set1_ps(value);
                v128* ptr_v128 = (v128*)ptr;

                while (Hint.Likely(length >= 4))
                {
                    int mask = Sse.movemask_ps(Sse.cmpeq_ps(broadcast, Sse.loadu_ps(ptr_v128)));

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


                if (length >= 2)
                {
                    int mask = Sse.movemask_ps(Sse.cmpeq_ps(broadcast, Sse2.cvtsi64x_si128(*(long*)ptr_v128)));
                    mask &= 0b0011;

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
                else { }


                if (length != 0)
                {
                    return *(float*)ptr_v128 == value;
                }
                else { }


                return false;
            }
            else
            {
                for (long i = 0; i < length; i++)
                {
                    if (ptr[i] == value)
                    {
                        return true;
                    }
                    else continue;
                }

                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<float> array, float value)
        {
            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<float> array, int index, float value)
        {
            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<float> array, int index, int numEntries, float value)
        {
            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<float> array, float value)
        {
            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<float> array, int index, float value)
        {
            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<float> array, int index, int numEntries, float value)
        {
            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<float> array, float value)
        {
            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<float> array, int index, float value)
        {
            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<float> array, int index, int numEntries, float value)
        {
            return SIMD_Contains((float*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(double* ptr, long length, double value)
        {
Assert.IsNonNegative(length);
Assert.IsFalse(math.isnan(value));

            if (Avx.IsAvxSupported)
            {
                v256 broadcast = Avx.mm256_set1_pd(value);
                v256* ptr_v256 = (v256*)ptr;

                while (Hint.Likely(length >= 4))
                {
                    int mask = Avx.mm256_movemask_pd(Avx.mm256_cmp_pd(broadcast, Avx.mm256_loadu_pd(ptr_v256), (int)Avx.CMP.EQ_OQ));

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


                if (length >= 2)
                {
                    int mask = Sse2.movemask_pd(Sse2.cmpeq_pd(Avx.mm256_castpd256_pd128(broadcast), Sse.loadu_ps(ptr_v256)));

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
                else { }


                if (length != 0)
                {
                    return *(double*)ptr_v256 == value;
                }
                else { }


                return false;
            }
            else if (Sse2.IsSse2Supported)
            {
                v128 broadcast = Sse2.set1_pd(value);
                v128* ptr_v128 = (v128*)ptr;

                while (Hint.Likely(length >= 2))
                {
                    int mask = Sse2.movemask_pd(Sse2.cmpeq_pd(broadcast, Sse.loadu_ps(ptr_v128)));

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


                if (length != 0)
                {
                    return *(double*)ptr_v128 == value;
                }
                else { }


                return false;
            }
            else
            {
                for (long i = 0; i < length; i++)
                {
                    if (ptr[i] == value)
                    {
                        return true;
                    }
                    else continue;
                }

                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<double> array, double value)
        {
            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<double> array, int index, double value)
        {
            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeArray<double> array, int index, int numEntries, double value)
        {
            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<double> array, double value)
        {
            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<double> array, int index, double value)
        {
            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeList<double> array, int index, int numEntries, double value)
        {
            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<double> array, double value)
        {
            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<double> array, int index, double value)
        {
            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr() + index, (array.Length - index), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_Contains(this NativeSlice<double> array, int index, int numEntries, double value)
        {
            return SIMD_Contains((double*)array.GetUnsafeReadOnlyPtr() + index, numEntries, value);
        }
    }
}