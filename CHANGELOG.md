## Additions

- Added scoped registry support
- Added `SIMD_Reverse` functions for any contiguous native container - particularly optimized for structs with size 8 or less
- Added `SIMD_IsSorted` functions for any contiguous native container of primitive numeric types
- Added `SIMD_BitsEqual` functions for any pair of contiguous native containers
- Added `MemoryAccessOptions` enum, controling how many bytes may be overread when accessing memory
- Added `Compare` extension methods to `Comparison`
- Added the first installation of `SIMD_Sort` algorithms, currently only available for contiguous `(s)byte` native containers

## Improvements

- 32/64 bit `SIMD_Contains` overloads have had their residuals loop optimized to 1 branch if compiling for AVX2
- Improved performance of all `SIMD_CountBits` overloads by unrolling the main loop, removing a nested loop and by mixing scalar- and SIMD instructions
- MaxMath updates now enable full support for ARM CPU SIMD instructions (!), enhance constant folding, and add SSE2 fallback for `SIMD_CountBits`

## Changes

- This package now depends on [MaxMath](https://github.com/MrUnbelievable92/MaxMath) version 2.9.9
- Bumped C# Dev Tools dependency to version 1.0.10
- Bumped Unity.Burst dependency to version 1.8.26
