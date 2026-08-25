using System.Runtime.CompilerServices;
using Unity.Burst;
using MaxMath;
using MaxMath.CompilerServices;

namespace SIMDAlgorithms
{
    unsafe public static partial class Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemSet<T>(T* dest, T value, long count, bool unrolled = true) 
            where T : unmanaged
        {
            for (long i = 0; i < count; i++)
            {
                dest[i] = value;
            }

            //byte* d = (byte*)dest;
            //long n = count * sizeof(T);
            //
            //switch (sizeof(T))
            //{
            //    case 1:
            //    {
            //        FillCascade(d, n, new byte32(value.Reinterpret<T, byte>()), unrolled);
            //        return;
            //    }
            //    case 2:
            //    {
            //        FillCascade(d, n, new ushort16(value.Reinterpret<T, ushort>()).Reinterpret<ushort16, byte32>(), unrolled);
            //        return;
            //    }
            //    case 4:
            //    {
            //        FillCascade(d, n, new uint8(value.Reinterpret<T, uint>()).Reinterpret<uint8, byte32>(), unrolled);
            //        return;
            //    }
            //    case 8:
            //    {
            //        FillCascade(d, n, new ulong4(value.Reinterpret<T, ulong>()).Reinterpret<ulong4, byte32>(), unrolled);
            //        return;
            //    }
            //    case 16:
            //    {
            //        byte16 lo = new byte16(value.Reinterpret<T, byte>());
            //        FillCascade(d, n, new byte32(lo, lo), unrolled);
            //        return;
            //    }
            //    case 32:
            //    {
            //        FillCascade(d, n, value.Reinterpret<T, byte32>(), unrolled);
            //        return;
            //    }
            //    default:
            //    {
            //        long filled = 0;
            //        while (filled < n)
            //        {
            //            long step = filled == 0 ? sizeof(T) : (filled < n - filled ? filled : n - filled);
            //            if (filled == 0) 
            //            {
            //                *(T*)d = value; 
            //            } 
            //            else 
            //            {
            //                MemCpy(d + filled, d, step, unrolled);
            //            }
            //            filled += step;
            //        }
            //        return;
            //    }
            //}
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemClear(void* dest, long count, bool unrolled = true)
        {
            MemSet<byte>((byte*)dest, 0, count, unrolled);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FillCascade(byte* d, long n, byte32 p32, bool unrolled = true)
        {
            unrolled &= COMPILATION_OPTIONS.OPTIMIZE_FOR == OptimizeFor.Performance;
            int chunkSize = unrolled ? 128 : 32;

            long mainBytes = n & ~(chunkSize - 1L);
            long off = 0;

            long r = n - mainBytes;
            byte* td = d + mainBytes;

            while (off < mainBytes)
            {
                *(byte32*)(d + off) = p32;
                if (unrolled)
                {
                    *(byte32*)(d + off + 32) = p32;
                    *(byte32*)(d + off + 64) = p32;
                    *(byte32*)(d + off + 96) = p32;
                }

                off += chunkSize;
            }

            byte16 p16 = p32.v16_0;
            ulong p8 = p32.Reinterpret<byte32, ulong4>().x;
            uint p4 = p32.Reinterpret<byte32, uint8>().x0;
            ushort p2 = p32.Reinterpret<byte32, ushort16>().x0;
            byte p1 = p32.x0;

            if (unrolled)
            {
                if ((r & 64) != 0)
                {
                    *(byte32*)(td + 0)  = p32;
                    *(byte32*)(td + 32) = p32;
                }

                long o32 = r & ~63L;
                if ((r & 32) != 0) *(byte32*)(td + o32) = p32;
            }

            long o16 = r & ~31L;
            if ((r & 16) != 0) *(byte16*)(td + o16) = p16;

            long o8 = r & ~15L;
            if ((r & 8) != 0)  *(ulong*)  (td + o8) = p8;

            long o4 = r & ~7L;
            if ((r & 4) != 0)   *(uint*)  (td + o4) = p4;

            long o2 = r & ~3L;
            if ((r & 2) != 0) *(ushort*)  (td + o2) = p2;

            long o1 = r & ~1L;
            if ((r & 1) != 0)   *(byte*)  (td + o1) = p1;
        }
    }
}
