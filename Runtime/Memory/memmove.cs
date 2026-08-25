using System.Runtime.CompilerServices;
using Unity.Burst;
using MaxMath;
using MaxMath.CompilerServices;

namespace SIMDAlgorithms
{
    unsafe public static partial class Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemMove(void* dest, void* src, long n, bool unrolled = true)
        {
            Unity.Collections.LowLevel.Unsafe.UnsafeUtility.MemMove(dest, src, n);

            //unrolled &= COMPILATION_OPTIONS.OPTIMIZE_FOR == OptimizeFor.Performance;
            //int chunkSize = unrolled ? 128 : 32;
            //
            //byte* d = (byte*)dest;
            //byte* s = (byte*)src;
            //
            //if (d < s) 
            //{
            //    MemCpy(dest, src, n, unrolled);
            //    return;
            //}
            //
            //long mainBytes = n & ~(chunkSize - 1L);
            //long r = n - mainBytes;
            //
            //long off = mainBytes;
            //while (off > 0)
            //{
            //    off -= chunkSize;
            //    long p = r + off;
            //
            //    *(byte32*)(d + p) = *(byte32*)(s + p);
            //    if (unrolled)
            //    {
            //        *(byte32*)(d + p + 32) = *(byte32*)(s + p + 32);
            //        *(byte32*)(d + p + 64) = *(byte32*)(s + p + 64);
            //        *(byte32*)(d + p + 96) = *(byte32*)(s + p + 96);
            //    }
            //}
            //
            //long o1 = r & ~1L;
            //if ((r & 1) != 0) *(byte*)(d + o1) = *(byte*)(s + o1);
            //
            //long o2 = r & ~3L;
            //if ((r & 2) != 0) *(ushort*)(d + o2) = *(ushort*)(s + o2);
            //
            //long o4 = r & ~7L;
            //if ((r & 4) != 0) *(uint*)(d + o4) = *(uint*)(s + o4);
            //
            //long o8 = r & ~15L;
            //if ((r & 8) != 0) *(ulong*)(d + o8) = *(ulong*)(s + o8);
            //
            //long o16 = r & ~31L;
            //if ((r & 16) != 0) *(byte16*)(d + o16) = *(byte16*)(s + o16);
            //
            //if (unrolled)
            //{
            //    long o32 = r & ~63L;
            //    if ((r & 32) != 0) *(byte32*)(d + o32) = *(byte32*)(s + o32);
            //    
            //    if ((r & 64) != 0)
            //    {
            //        *(byte32*)d = *(byte32*)s;
            //        *(byte32*)(d + 32) = *(byte32*)(s + 32);
            //    }
            //}
        }
    }
}
