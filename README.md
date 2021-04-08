# SIMD-Algorithms

This package contains some of the most common array algorithms which are either auto-vectorized very poorly (like summing up unsigend bytes) or not auto vectorized at all (Contains, IndexOf, etc.).

All included functions have a "SIMD_" prefix added to their name.

There is a base version of each algorithm which takes in a pointer in the static "SIMDAlgorithms.Algorithms" class. There are extension methods of each algorithm for Unity.Collections' NativeArray, NativeSlice and NativeList aswell.

Note: 
- [C Sharp Dev Tools](https://github.com/MrUnbelievable92/C-Sharp-Dev-Tools) (conditionally compiled runtime checks) is required. Unit tests for this library are included in this repository.
