# IntSet <img src ="https://github.com/Kibnet/IntSet/raw/master/resources/IntSet.jpg" width="80px" alt="IntSet" align ="right">
![](https://github.com/Kibnet/IntSet/workflows/NuGet%20Generation/badge.svg?branch=master)
![](https://img.shields.io/github/issues/Kibnet/IntSet.svg?label=Issues)
![](https://img.shields.io/github/tag/Kibnet/IntSet.svg?label=Last%20Version)
![GitHub last commit](https://img.shields.io/github/last-commit/kibnet/IntSet)

![GitHub search hit counter](https://img.shields.io/github/search/kibnet/IntSet/IntSet?label=GitHub%20Search%20Hits)
![Nuget](https://img.shields.io/nuget/dt/IntSet?label=IntSet%20Downloads)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/kibnet/IntSet?label=Code%20Size)

## What is it?
It is a specialized data structure for storing a set of integers of type Int32.

Unlike the universal HashSet<int>, numbers are not stored explicitly, but are packed into compact ordered structures.
This allows you to save on the used RAM for large set sizes. 
In the worst case, the structure takes up ~525 MB in memory, while it can contain the full range of Int32 numbers. A hashset with such volumes consumes tens of gigabytes.

The structure implements the standard ISet<int> interface, and can be used in all scenarios instead of HashSet<int>.
Also IntSet implements the IEnumerable<int> interface, which enumerate numbers in ascending order without overhead.

## How to use?
The structure is designed as a Net Standard 2.1 library that can be used in any compatible projects.

To use it, install the [nuget package](https://www.nuget.org/packages/IntSet/):
```
Install-Package IntSet
```
### Usage Example
```
using Kibnet;
var intSet = new IntSet();
intSet.Add(123);
var isContains = intSet.Contains(123);
intSet.Remove(123);
```

## What is your proof?
The repository has tests that show that everything works as it should.

I also have a code with benchmarks that shows superiority over HashSet in the operations of adding, deleting, and contains.
I will publish it in this repository after I bring it to an acceptable form.

## Benchmarks

This project includes benchmarks to compare the performance and memory usage of `IntSet` against the standard `System.Collections.Generic.HashSet<int>`. The benchmarks are implemented using [BenchmarkDotNet](https://benchmarkdotnet.org/).

### Running the Benchmarks

To run the benchmarks:

1.  Navigate to the benchmark project directory:
    ```bash
    cd src/IntSet.Benchmarks
    ```
2.  Run the benchmark project:
    ```bash
    dotnet run -c Release
    ```
    It is highly recommended to run benchmarks in `Release` configuration for accurate results.

### Benchmark Results

The benchmark results will be displayed in the console after the run completes. Additionally, BenchmarkDotNet will generate detailed reports (including markdown files, CSV files, and plots) in a `BenchmarkDotNet.Artifacts` directory within `src/IntSet.Benchmarks/bin/Release/netX.X/` (where `netX.X` is the target framework, e.g., `net9.0`).
