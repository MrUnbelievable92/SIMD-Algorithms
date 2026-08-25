using System.Runtime.CompilerServices;
using Unity.Burst;
using MaxMath;
using MaxMath.CompilerServices;

namespace SIMDAlgorithms
{
    unsafe public static partial class Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemCpy(void* dest, void* src, long n, bool unrolled = true)
        {
            Unity.Collections.LowLevel.Unsafe.UnsafeUtility.MemCpy(dest, src, n);

            //unrolled &= COMPILATION_OPTIONS.OPTIMIZE_FOR == OptimizeFor.Performance;
            //int chunkSize = unrolled ? 128 : 32;
            //
            //byte* d = (byte*)dest;
            //byte* s = (byte*)src;
            //
            //long mainBytes = n & ~(chunkSize - 1L);
            //long off = 0;
            //
            //long r = n - mainBytes;
            //byte* td = d + mainBytes;
            //byte* ts = s + mainBytes;
            //
            //while (off < mainBytes)
            //{
            //    *(byte32*)(d + off) = *(byte32*)(s + off);
            //    if (unrolled)
            //    {
            //        *(byte32*)(d + off + 32) = *(byte32*)(s + off + 32);
            //        *(byte32*)(d + off + 64) = *(byte32*)(s + off + 64);
            //        *(byte32*)(d + off + 96) = *(byte32*)(s + off + 96);
            //    }
            //
            //    off += chunkSize;
            //}
            //
            //if (unrolled)
            //{
            //    if ((r & 64) != 0)
            //    {
            //        *(byte32*)(td +  0) = *(byte32*)(ts +  0);
            //        *(byte32*)(td + 32) = *(byte32*)(ts + 32);
            //    }
            //    
            //    long o = r & ~63L;
            //    if ((r & 32) != 0) *(byte32*)(td + o) = *(byte32*)(ts + o);
            //}
            //
            //long o16 = r & ~31L;
            //if ((r & 16) != 0) *(byte16*)(td + o16) = *(byte16*)(ts + o16);
            //
            //long o8 = r & ~15L;
            //if ((r & 8) != 0)   *(ulong*)(td + o8)  = *(ulong*)(ts + o8);
            //                                        
            //long o4 = r & ~7L;               
            //if ((r & 4) != 0)    *(uint*)(td + o4)  = *(uint*)(ts + o4);
            //                                        
            //long o2 = r & ~3L;               
            //if ((r & 2) != 0)  *(ushort*)(td + o2)  = *(ushort*)(ts + o2);
            //                                        
            //long o1 = r & ~1L;               
            //if ((r & 1) != 0)    *(byte*)(td + o1)  = *(byte*)(ts + o1);
        }
    }
}
