using System;
using BenchmarkDotNet.Running;
using IntSet.Benchmarks; // Namespace where SetOperationsBenchmarks is defined

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting IntSet Benchmarks...");
        // To run all benchmarks from the assembly (if you have multiple benchmark classes)
        // var summary = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

        // To run a specific benchmark class
        var summary = BenchmarkRunner.Run<MemoryAnalysisBenchmarks>(null, args);

        // You can add more summaries or configurations if needed
        Console.WriteLine("IntSet Benchmarks completed.");
    }
}
