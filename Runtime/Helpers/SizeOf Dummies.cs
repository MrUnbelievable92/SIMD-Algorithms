using System.Runtime.InteropServices;

namespace SIMDAlgorithms
{
    [StructLayout(LayoutKind.Sequential, Size = 3)]
    internal struct Byte3
    {
        internal byte a, b, c;
    }

    [StructLayout(LayoutKind.Sequential, Size = 5)]
    internal struct Byte5
    {
        internal byte a, b, c, d, e;
    }

    [StructLayout(LayoutKind.Sequential, Size = 6)]
    internal struct Byte6
    {
        internal byte a, b, c, d, e, f;
    }

    [StructLayout(LayoutKind.Sequential, Size = 7)]
    internal struct Byte7
    {
        internal byte a, b, c, d, e, f, g;
    }
}
