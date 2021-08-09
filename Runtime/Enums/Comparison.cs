namespace SIMDAlgorithms
{
    public enum Comparison : byte
    {
        /// <summary>       arrayElement == value     </summary>
        EqualTo,
        /// <summary>       arrayElement != value     </summary>
        NotEqualTo,

        /// <summary>       arrayElement > value     </summary>
        GreaterThan,
        /// <summary>       arrayElement < value     </summary>
        LessThan,

        /// <summary>       arrayElement >= value     </summary>
        GreaterThanOrEqualTo,
        /// <summary>       arrayElement <= value     </summary>
        LessThanOrEqualTo
    }
}