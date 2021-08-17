# SIMD-Algorithms

This package contains some of the most common array algorithms which are either auto-vectorized very poorly (like summing up unsigned bytes) or not auto vectorized at all (Contains, IndexOf, etc.).

All included functions have a "SIMD_" prefix added to their name.

The main way to access them is via extension methods to NativeArray<T>, NativeSlice<T> and NativeList<T>, where T is a primitive C# numeric type although "CountBits" takes in any kind of array. You may need to use (for example) "(NativeArray<Health>.Reinterpret<byte>).SIMD_Max();", althouth there is a base version of each algorithm which takes in a pointer in the static "SIMDAlgorithms.Algorithms" class, aswell.

Note: 
- [C Sharp Dev Tools](https://github.com/MrUnbelievable92/C-Sharp-Dev-Tools) (conditionally compiled runtime checks) is required. Unit tests for this library are included in this repository.

# How To Install This Library

Disclaimer: I firmly believe in open source - being able to copy/modify/understand other people's code is great :)
I also want people to be able to step through code with a debugger.
For these reasons I usually don't distribute DLLs.

- Download the package and unzip it into your "LocalPackages" folder, which is located at the root folder of your Unity project (where your "Assets" folder resides at).
- Start up Unity. Usually Unity detects new packages and will generate .meta files for you.
- In case that doesn't work, open up the package manager from within Unity and click on the '+' symbol at the upper left corner of the window, further clicking on "Add package from disk..." - "Add package from git URL" should also work.

![alt text](https://i.imgur.com/QcqF96e.png)

- Locate the library's "package.json" file
- DONE! 
