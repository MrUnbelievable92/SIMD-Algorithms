namespace SIMDAlgorithms
{
    public enum BitwiseOperation : byte
    {
        /// <summary>       countbits(arrayElement)     </summary>
        None,

        /// <summary>       countbits(~arrayElement)     </summary>
        NOT,

        /// <summary>       countbits(arrayElement & mask)     </summary>
        AND,
        /// <summary>       countbits(arrayElement | mask)     </summary>
        OR,
        /// <summary>       countbits(arrayElement ^ mask)     </summary>
        XOR,

        /// <summary>       countbits(~(arrayElement & mask))     </summary>
        NAND,
        /// <summary>       countbits(~(arrayElement | mask))     </summary>
        NOR,
        /// <summary>       countbits(~(arrayElement ^ mask))     </summary>
        XNOR,

        /// <summary>       countbits(~arrayElement & mask)     </summary>
        ANDNOT,
        /// <summary>       countbits(~arrayElement | mask)     </summary>
        ORNOT,
        /// <summary>       countbits(~arrayElement ^ mask)     </summary>
        XORNOT = XNOR
    }
}