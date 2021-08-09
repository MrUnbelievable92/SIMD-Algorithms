using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;

using static Unity.Burst.Intrinsics.X86;

namespace SIMDAlgorithms
{
    internal static class Fallback
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static v128 blendv_epi8_SSE2(v128 a, v128 b, v128 mask)
        {
            if (Sse2.IsSse2Supported)
            {
                return Sse2.or_si128(Sse2.and_si128(mask, b),
                                     Sse2.andnot_si128(mask, a));
            }
            else
            {
                return default(v128);
            }
        }
    }
}