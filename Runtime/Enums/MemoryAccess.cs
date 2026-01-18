namespace SIMDAlgorithms
{
    /// <summary>   Controls how a memory read may access bytes beyond the requested range.    </summary>
    public enum MemoryAccess
    {
        /// <summary>   No more than the requested amount of bytes will be read from memory.    </summary>
        Strict = 0,

        /// <summary>   Allows reading of up to 2 contiguous bytes from memory, including the requested bytes.    </summary>
        Allow2ByteRead = 2,

        /// <summary>   Allows reading of up to 4 contiguous bytes from memory, including the requested bytes.    </summary>
        Allow4ByteRead = 4,

        /// <summary>   Allows reading of up to 8 contiguous bytes from memory, including the requested bytes.    </summary>
        Allow8ByteRead = 8,

        /// <summary>   Allows reading of up to 16 contiguous bytes from memory, including the requested bytes.    </summary>
        Allow16ByteRead = 16,

        /// <summary>   Allows reading of up to 32 contiguous bytes from memory, including the requested bytes.    </summary>
        Allow32ByteRead = 32,

        /// <summary>   Allows reading of up to 64 contiguous bytes from memory, including the requested bytes.    </summary>
        Allow64ByteRead = 64,

        /// <summary>   Allows reading of up to 128 contiguous bytes from memory, including the requested bytes.    </summary>
        Allow128ByteRead = 128,

        /// <summary>   Allows reading of up to 256 contiguous bytes from memory, including the requested bytes.    </summary>
        Allow256ByteRead = 256
    }
}
