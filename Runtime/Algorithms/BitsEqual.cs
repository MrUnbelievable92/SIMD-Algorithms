using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst.Intrinsics;
using Unity.Burst.CompilerServices;
using MaxMath.Intrinsics;
using DevTools;

using static Unity.Burst.Intrinsics.X86;

namespace SIMDAlgorithms
{
    unsafe public static partial class Algorithms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual(void* ptr0, void* ptr1, long bytes, TraversalOrder traversalOrder = TraversalOrder.Ascending)
        {
Assert.IsNonNegative(bytes);

            if (traversalOrder == TraversalOrder.Ascending)
            {
                if (Avx2.IsAvx2Supported)
                {
                    while (Hint.Likely(bytes >= sizeof(v256)))
                    {
                        if (Xse.mm256_notalltrue_epi256<byte>(Avx2.mm256_cmpeq_epi8(*(v256*)ptr0, *(v256*)ptr1)))
                        {
                            return false;
                        }

                        ptr0 = (v256*)ptr0 + 1;
                        ptr1 = (v256*)ptr1 + 1;
                        bytes -= 32;
                    }

                    if (Hint.Likely(bytes >= sizeof(v128)))
                    {
                        if (Xse.notalltrue_epi128<byte>(Xse.cmpeq_epi8(*(v128*)ptr0, *(v128*)ptr1)))
                        {
                            return false;
                        }

                        ptr0 = (v128*)ptr0 + 1;
                        ptr1 = (v128*)ptr1 + 1;
                        bytes -= 16;
                    }

                    if (Hint.Likely(bytes >= sizeof(long)))
                    {
                        if (*(long*)ptr0 != *(long*)ptr1)
                        {
                            return false;
                        }

                        ptr0 = (long*)ptr0 + 1;
                        ptr1 = (long*)ptr1 + 1;
                        bytes -= 8;
                    }

                    if (Hint.Likely(bytes >= sizeof(int)))
                    {
                        if (*(int*)ptr0 != *(int*)ptr1)
                        {
                            return false;
                        }

                        ptr0 = (int*)ptr0 + 1;
                        ptr1 = (int*)ptr1 + 1;
                        bytes -= 4;
                    }

                    if (Hint.Likely(bytes >= sizeof(ushort)))
                    {
                        if (*(ushort*)ptr0 != *(ushort*)ptr1)
                        {
                            return false;
                        }

                        ptr0 = (ushort*)ptr0 + 1;
                        ptr1 = (ushort*)ptr1 + 1;
                        bytes -= 2;
                    }

                    if (Hint.Likely(bytes != 0))
                    {
                        return *(byte*)ptr0 == *(byte*)ptr1;
                    }

                    return true;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    while (Hint.Likely(bytes >= sizeof(v128)))
                    {
                        if (Xse.notalltrue_epi128<byte>(Xse.cmpeq_epi8(*(v128*)ptr0, *(v128*)ptr1)))
                        {
                            return false;
                        }

                        ptr0 = (v128*)ptr0 + 1;
                        ptr1 = (v128*)ptr1 + 1;
                        bytes -= 16;
                    }

                    if (Hint.Likely(bytes >= sizeof(long)))
                    {
                        if (*(long*)ptr0 != *(long*)ptr1)
                        {
                            return false;
                        }

                        ptr0 = (long*)ptr0 + 1;
                        ptr1 = (long*)ptr1 + 1;
                        bytes -= 8;
                    }

                    if (Hint.Likely(bytes >= sizeof(int)))
                    {
                        if (*(int*)ptr0 != *(int*)ptr1)
                        {
                            return false;
                        }

                        ptr0 = (int*)ptr0 + 1;
                        ptr1 = (int*)ptr1 + 1;
                        bytes -= 4;
                    }

                    if (Hint.Likely(bytes >= sizeof(ushort)))
                    {
                        if (*(ushort*)ptr0 != *(ushort*)ptr1)
                        {
                            return false;
                        }

                        ptr0 = (ushort*)ptr0 + 1;
                        ptr1 = (ushort*)ptr1 + 1;
                        bytes -= 2;
                    }

                    if (Hint.Likely(bytes != 0))
                    {
                        return *(byte*)ptr0 == *(byte*)ptr1;
                    }

                    return true;
                }
                else
                {
                    while (Hint.Likely(bytes >= sizeof(long)))
                    {
                        if (*(long*)ptr0 != *(long*)ptr1)
                        {
                            return false;
                        }

                        ptr0 = (long*)ptr0 + 1;
                        ptr1 = (long*)ptr1 + 1;
                        bytes -= 8;
                    }

                    if (Hint.Likely(bytes >= sizeof(int)))
                    {
                        if (*(int*)ptr0 != *(int*)ptr1)
                        {
                            return false;
                        }

                        ptr0 = (int*)ptr0 + 1;
                        ptr1 = (int*)ptr1 + 1;
                        bytes -= 4;
                    }

                    if (Hint.Likely(bytes >= sizeof(ushort)))
                    {
                        if (*(ushort*)ptr0 != *(ushort*)ptr1)
                        {
                            return false;
                        }

                        ptr0 = (ushort*)ptr0 + 1;
                        ptr1 = (ushort*)ptr1 + 1;
                        bytes -= 2;
                    }

                    if (Hint.Likely(bytes != 0))
                    {
                        return *(byte*)ptr0 == *(byte*)ptr1;
                    }

                    return true;
                }
            }
            else
            {
                if (Avx2.IsAvx2Supported)
                {
                    void* endPtr0 = (byte*)ptr0 + bytes; // this points to the "element" right after the last element. THIS IS INTENTIONAL
                    void* endPtr1 = (byte*)ptr1 + bytes; // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr0 - (long)ptr0 >= sizeof(v256)))
                    {
                        endPtr0 = (v256*)endPtr0 - 1;
                        endPtr1 = (v256*)endPtr1 - 1;

                        if (Xse.mm256_notalltrue_epi256<byte>(Avx2.mm256_cmpeq_epi8(*(v256*)endPtr0, *(v256*)endPtr1)))
                        {
                            return false;
                        }
                    }

                    if (Hint.Likely((long)endPtr0 - (long)ptr0 >= sizeof(v128)))
                    {
                        endPtr0 = (v128*)endPtr0 - 1;
                        endPtr1 = (v128*)endPtr1 - 1;

                        if (Xse.notalltrue_epi128<byte>(Xse.cmpeq_epi8(*(v128*)endPtr0, *(v128*)endPtr1)))
                        {
                            return false;
                        }
                    }

                    if (Hint.Likely((long)endPtr0 - (long)ptr0 >= sizeof(long)))
                    {
                        endPtr0 = (long*)endPtr0 - 1;
                        endPtr1 = (long*)endPtr1 - 1;

                        if (*(long*)endPtr0 != *(long*)endPtr1)
                        {
                            return false;
                        }
                    }

                    if (Hint.Likely((long)endPtr0 - (long)ptr0 >= sizeof(int)))
                    {
                        endPtr0 = (int*)endPtr0 - 1;
                        endPtr1 = (int*)endPtr1 - 1;

                        if (*(int*)endPtr0 != *(int*)endPtr1)
                        {
                            return false;
                        }
                    }

                    if (Hint.Likely((long)endPtr0 - (long)ptr0 >= sizeof(ushort)))
                    {
                        endPtr0 = (ushort*)endPtr0 - 1;
                        endPtr1 = (ushort*)endPtr1 - 1;

                        if (*(ushort*)endPtr0 != *(ushort*)endPtr1)
                        {
                            return false;
                        }
                    }

                    if (Hint.Likely((long)endPtr0 != (long)ptr0))
                    {
                        return *(byte*)ptr0 == *(byte*)ptr1;
                    }

                    return true;
                }
                else if (BurstArchitecture.IsSIMDSupported)
                {
                    void* endPtr0 = (byte*)ptr0 + bytes; // this points to the "element" right after the last element. THIS IS INTENTIONAL
                    void* endPtr1 = (byte*)ptr1 + bytes; // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr0 - (long)ptr0 >= sizeof(v128)))
                    {
                        endPtr0 = (v128*)endPtr0 - 1;
                        endPtr1 = (v128*)endPtr1 - 1;

                        if (Xse.notalltrue_epi128<byte>(Xse.cmpeq_epi8(*(v128*)endPtr0, *(v128*)endPtr1)))
                        {
                            return false;
                        }
                    }

                    if (Hint.Likely((long)endPtr0 - (long)ptr0 >= sizeof(long)))
                    {
                        endPtr0 = (long*)endPtr0 - 1;
                        endPtr1 = (long*)endPtr1 - 1;

                        if (*(long*)endPtr0 != *(long*)endPtr1)
                        {
                            return false;
                        }
                    }

                    if (Hint.Likely((long)endPtr0 - (long)ptr0 >= sizeof(int)))
                    {
                        endPtr0 = (int*)endPtr0 - 1;
                        endPtr1 = (int*)endPtr1 - 1;

                        if (*(int*)endPtr0 != *(int*)endPtr1)
                        {
                            return false;
                        }
                    }

                    if (Hint.Likely((long)endPtr0 - (long)ptr0 >= sizeof(ushort)))
                    {
                        endPtr0 = (ushort*)endPtr0 - 1;
                        endPtr1 = (ushort*)endPtr1 - 1;

                        if (*(ushort*)endPtr0 != *(ushort*)endPtr1)
                        {
                            return false;
                        }
                    }

                    if (Hint.Likely((long)endPtr0 != (long)ptr0))
                    {
                        return *(byte*)ptr0 == *(byte*)ptr1;
                    }

                    return true;
                }
                else
                {
                    void* endPtr0 = (byte*)ptr0 + bytes; // this points to the "element" right after the last element. THIS IS INTENTIONAL
                    void* endPtr1 = (byte*)ptr1 + bytes; // this points to the "element" right after the last element. THIS IS INTENTIONAL

                    while (Hint.Likely((long)endPtr0 - (long)ptr0 >= sizeof(long)))
                    {
                        endPtr0 = (long*)endPtr0 - 1;
                        endPtr1 = (long*)endPtr1 - 1;

                        if (*(long*)endPtr0 != *(long*)endPtr1)
                        {
                            return false;
                        }
                    }

                    if (Hint.Likely((long)endPtr0 - (long)ptr0 >= sizeof(int)))
                    {
                        endPtr0 = (int*)endPtr0 - 1;
                        endPtr1 = (int*)endPtr1 - 1;

                        if (*(int*)endPtr0 != *(int*)endPtr1)
                        {
                            return false;
                        }
                    }

                    if (Hint.Likely((long)endPtr0 - (long)ptr0 >= sizeof(ushort)))
                    {
                        endPtr0 = (ushort*)endPtr0 - 1;
                        endPtr1 = (ushort*)endPtr1 - 1;

                        if (*(ushort*)endPtr0 != *(ushort*)endPtr1)
                        {
                            return false;
                        }
                    }

                    if (Hint.Likely((long)endPtr0 != (long)ptr0))
                    {
                        return *(byte*)ptr0 == *(byte*)ptr1;
                    }

                    return true;
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeArray<T> array, NativeArray<T> cmpArray, int count, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
Assert.IsSmaller(indexLHS, array.Length);
Assert.IsSmaller(indexRHS, cmpArray.Length);
Assert.AreEqual(array.Length - indexLHS, cmpArray.Length - indexRHS);
Assert.IsValidSubarray(indexLHS, count, array.Length);
Assert.IsValidSubarray(indexRHS, count, cmpArray.Length);

            return SIMD_BitsEqual((T*)array.GetUnsafeReadOnlyPtr() + indexLHS, (T*)cmpArray.GetUnsafeReadOnlyPtr() + indexRHS, sizeof(T) * count, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeArray<T> array, NativeList<T> cmpArray, int count, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
Assert.IsSmaller(indexLHS, array.Length);
Assert.IsSmaller(indexRHS, cmpArray.Length);
Assert.AreEqual(array.Length - indexLHS, cmpArray.Length - indexRHS);
Assert.IsValidSubarray(indexLHS, count, array.Length);
Assert.IsValidSubarray(indexRHS, count, cmpArray.Length);

            return SIMD_BitsEqual((T*)array.GetUnsafeReadOnlyPtr() + indexLHS, (T*)cmpArray.GetUnsafeReadOnlyPtr() + indexRHS, sizeof(T) * count, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeArray<T> array, NativeSlice<T> cmpArray, int count, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
Assert.IsSmaller(indexLHS, array.Length);
Assert.IsSmaller(indexRHS, cmpArray.Length);
Assert.AreEqual(array.Length - indexLHS, cmpArray.Length - indexRHS);
Assert.IsValidSubarray(indexLHS, count, array.Length);
Assert.IsValidSubarray(indexRHS, count, cmpArray.Length);

            return SIMD_BitsEqual((T*)array.GetUnsafeReadOnlyPtr() + indexLHS, (T*)cmpArray.GetUnsafeReadOnlyPtr() + indexRHS, sizeof(T) * count, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeArray<T> array, NativeArray<T> cmpArray, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
            return SIMD_BitsEqual(array, cmpArray, array.Length - indexLHS, indexLHS, indexRHS, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeArray<T> array, NativeList<T> cmpArray, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
            return SIMD_BitsEqual(array, cmpArray, array.Length - indexLHS, indexLHS, indexRHS, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeArray<T> array, NativeSlice<T> cmpArray, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
            return SIMD_BitsEqual(array, cmpArray, array.Length - indexLHS, indexLHS, indexRHS, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeList<T> array, NativeArray<T> cmpArray, int count, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
Assert.IsSmaller(indexLHS, array.Length);
Assert.IsSmaller(indexRHS, cmpArray.Length);
Assert.AreEqual(array.Length - indexLHS, cmpArray.Length - indexRHS);
Assert.IsValidSubarray(indexLHS, count, array.Length);
Assert.IsValidSubarray(indexRHS, count, cmpArray.Length);

            return SIMD_BitsEqual((T*)array.GetUnsafeReadOnlyPtr() + indexLHS, (T*)cmpArray.GetUnsafeReadOnlyPtr() + indexRHS, sizeof(T) * count, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeList<T> array, NativeList<T> cmpArray, int count, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
Assert.IsSmaller(indexLHS, array.Length);
Assert.IsSmaller(indexRHS, cmpArray.Length);
Assert.AreEqual(array.Length - indexLHS, cmpArray.Length - indexRHS);
Assert.IsValidSubarray(indexLHS, count, array.Length);
Assert.IsValidSubarray(indexRHS, count, cmpArray.Length);

            return SIMD_BitsEqual((T*)array.GetUnsafeReadOnlyPtr() + indexLHS, (T*)cmpArray.GetUnsafeReadOnlyPtr() + indexRHS, sizeof(T) * count, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeList<T> array, NativeSlice<T> cmpArray, int count, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
Assert.IsSmaller(indexLHS, array.Length);
Assert.IsSmaller(indexRHS, cmpArray.Length);
Assert.AreEqual(array.Length - indexLHS, cmpArray.Length - indexRHS);
Assert.IsValidSubarray(indexLHS, count, array.Length);
Assert.IsValidSubarray(indexRHS, count, cmpArray.Length);

            return SIMD_BitsEqual((T*)array.GetUnsafeReadOnlyPtr() + indexLHS, (T*)cmpArray.GetUnsafeReadOnlyPtr() + indexRHS, sizeof(T) * count, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeList<T> array, NativeArray<T> cmpArray, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
            return SIMD_BitsEqual(array, cmpArray, array.Length - indexLHS, indexLHS, indexRHS, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeList<T> array, NativeList<T> cmpArray, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
            return SIMD_BitsEqual(array, cmpArray, array.Length - indexLHS, indexLHS, indexRHS, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeList<T> array, NativeSlice<T> cmpArray, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
            return SIMD_BitsEqual(array, cmpArray, array.Length - indexLHS, indexLHS, indexRHS, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeSlice<T> array, NativeArray<T> cmpArray, int count, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
Assert.IsSmaller(indexLHS, array.Length);
Assert.IsSmaller(indexRHS, cmpArray.Length);
Assert.AreEqual(array.Length - indexLHS, cmpArray.Length - indexRHS);
Assert.IsValidSubarray(indexLHS, count, array.Length);
Assert.IsValidSubarray(indexRHS, count, cmpArray.Length);

            return SIMD_BitsEqual((T*)array.GetUnsafeReadOnlyPtr() + indexLHS, (T*)cmpArray.GetUnsafeReadOnlyPtr() + indexRHS, sizeof(T) * count, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeSlice<T> array, NativeList<T> cmpArray, int count, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
Assert.IsSmaller(indexLHS, array.Length);
Assert.IsSmaller(indexRHS, cmpArray.Length);
Assert.AreEqual(array.Length - indexLHS, cmpArray.Length - indexRHS);
Assert.IsValidSubarray(indexLHS, count, array.Length);
Assert.IsValidSubarray(indexRHS, count, cmpArray.Length);

            return SIMD_BitsEqual((T*)array.GetUnsafeReadOnlyPtr() + indexLHS, (T*)cmpArray.GetUnsafeReadOnlyPtr() + indexRHS, sizeof(T) * count, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeSlice<T> array, NativeSlice<T> cmpArray, int count, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
Assert.IsSmaller(indexLHS, array.Length);
Assert.IsSmaller(indexRHS, cmpArray.Length);
Assert.AreEqual(array.Length - indexLHS, cmpArray.Length - indexRHS);
Assert.IsValidSubarray(indexLHS, count, array.Length);
Assert.IsValidSubarray(indexRHS, count, cmpArray.Length);

            return SIMD_BitsEqual((T*)array.GetUnsafeReadOnlyPtr() + indexLHS, (T*)cmpArray.GetUnsafeReadOnlyPtr() + indexRHS, sizeof(T) * count, traversalOrder);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeSlice<T> array, NativeArray<T> cmpArray, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
            return SIMD_BitsEqual(array, cmpArray, array.Length - indexLHS, indexLHS, indexRHS, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeSlice<T> array, NativeList<T> cmpArray, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
            return SIMD_BitsEqual(array, cmpArray, array.Length - indexLHS, indexLHS, indexRHS, traversalOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SIMD_BitsEqual<T>(this NativeSlice<T> array, NativeSlice<T> cmpArray, int indexLHS = 0, int indexRHS = 0, TraversalOrder traversalOrder = TraversalOrder.Ascending)
            where T : unmanaged
        {
            return SIMD_BitsEqual(array, cmpArray, array.Length - indexLHS, indexLHS, indexRHS, traversalOrder);
        }
    }
}