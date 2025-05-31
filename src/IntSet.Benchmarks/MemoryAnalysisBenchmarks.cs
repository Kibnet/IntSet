using BenchmarkDotNet.Attributes;
using Kibnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace IntSet.Benchmarks
{
    public class BenchmarkResult
    {
        public long EstimatedMemoryFootprint { get; set; }
        public int MaxDepth { get; set; } // Maximum depth of the B-tree
        public int NodeCount { get; set; } // Total number of nodes
        public int LeafNodeCount { get; set; } // Number of leaf nodes
        public int InternalNodeCount { get; set; } // Number of internal nodes
        public double AverageKeysPerNode { get; set; } // Average number of keys per node
        public double AverageKeysPerLeafNode { get; set; } // Average number of keys per leaf node
        public double AverageKeysPerInternalNode { get; set; } // Average number of keys per internal node
        public int TotalKeys { get; set; } // Total number of keys stored
    }

    [MemoryDiagnoser]
    public class MemoryAnalysisBenchmarks
    {
        private IntSet _intSet;
        private List<int> _dataToLoad;

        // Parameter to control the size of the IntSet
        [Params(10000, 100000, 1000000)]
        public int N;

        // Parameter to control whether to use the full range of integers or a sparse set
        [Params(true, false)]
        public bool UseFullRangeParameter;


        [GlobalSetup]
        public void GlobalSetup()
        {
            _intSet = new IntSet();
            _dataToLoad = new List<int>(N);

            Console.WriteLine($"GlobalSetup: N={N}, UseFullRange={UseFullRangeParameter}");

            if (UseFullRangeParameter)
            {
                // Load a contiguous range of integers
                for (int i = 0; i < N; i++)
                {
                    _dataToLoad.Add(i);
                }
            }
            else
            {
                // Load a sparse set of integers (e.g., every 10th integer)
                // This creates a more fragmented IntSet, potentially affecting memory and structure.
                Random random = new Random(12345); // Fixed seed for reproducibility
                for (int i = 0; i < N; i++)
                {
                    _dataToLoad.Add(random.Next(0, N * 10));
                }
                // Ensure distinct values if N is small relative to range, though with N*10 duplicates are less likely for large N
                _dataToLoad = _dataToLoad.Distinct().Take(N).ToList();
            }

            Console.WriteLine($"Generated {_dataToLoad.Count} items to load.");

            // Populate the IntSet
            for (int i = 0; i < _dataToLoad.Count; i++)
            {
                _intSet.Add(_dataToLoad[i]);
                if ((i + 1) % (N / 10 == 0 ? 1 : N / 10) == 0) // Log progress every 10%
                {
                    Console.WriteLine($"Loaded {i + 1}/{_dataToLoad.Count} items into IntSet...");
                }
            }
            Console.WriteLine("IntSet populated.");
        }

        [Benchmark(Description = "Analyze Populated IntSet Memory and Structure")]
        public BenchmarkResult AnalyzePopulatedIntSet()
        {
            if (_intSet == null || _intSet.Count == 0)
            {
                // This case should ideally not be hit if GlobalSetup ran correctly and N > 0
                Console.WriteLine("IntSet is null or empty. Returning default BenchmarkResult.");
                return new BenchmarkResult { TotalKeys = 0 };
            }

            var result = new BenchmarkResult();
            result.TotalKeys = _intSet.Count; // Get the actual count from IntSet

            // Access the root node for traversal (assuming internal visibility or a public getter)
            // This part requires that `_intSet.Root` is accessible.
            // If Root is private, this will need to be adjusted (e.g., by making it internal and using InternalsVisibleTo, or adding a public method for analysis)
            var rootNode = _intSet.Root; // This line might require IntSet.Root to be public or internal

            if (rootNode == null)
            {
                 // If root is null (e.g. IntSet is empty after population attempt or N=0)
                return new BenchmarkResult { TotalKeys = _intSet.Count, EstimatedMemoryFootprint = Marshal.SizeOf<IntSet>() };
            }

            long estimatedMemory = 0;
            int maxDepth = 0;
            var nodeQueue = new Queue<Tuple<dynamic, int>>(); // Using dynamic for node, int for depth
            nodeQueue.Enqueue(Tuple.Create((dynamic)rootNode, 1));

            List<int> keysInInternalNodes = new List<int>();
            List<int> keysInLeafNodes = new List<int>();

            // Estimate base size of the IntSet object itself (excluding nodes)
            // This is a rough estimate. A more precise way would be to use a profiler or specific APIs if available.
            estimatedMemory += Marshal.SizeOf<IntSet>(); // Size of the IntSet class shell

            while (nodeQueue.Count > 0)
            {
                var (currentNode, currentDepth) = nodeQueue.Dequeue();
                result.NodeCount++;
                maxDepth = Math.Max(maxDepth, currentDepth);

                // Estimate memory for the node object itself (overhead)
                // This is a very rough approximation. Actual object size depends on CLR layout.
                estimatedMemory += IntPtr.Size * 2; // Approximate overhead for object header + type pointer

                // Estimate memory for keys and children references
                // Assuming Node has a 'Keys' (List<int>) and 'Children' (List<Node>) property or similar structure
                List<int> keys = currentNode.Keys;
                estimatedMemory += (keys.Capacity * sizeof(int)); // Memory for keys list capacity

                if (currentNode.IsLeaf)
                {
                    result.LeafNodeCount++;
                    keysInLeafNodes.AddRange(keys);
                }
                else
                {
                    result.InternalNodeCount++;
                    keysInInternalNodes.AddRange(keys);
                    List<dynamic> children = currentNode.Children;
                    estimatedMemory += (children.Capacity * IntPtr.Size); // Memory for children list capacity (references)
                    foreach (var child in children)
                    {
                        if (child != null)
                        {
                            nodeQueue.Enqueue(Tuple.Create(child, currentDepth + 1));
                        }
                    }
                }
            }

            result.EstimatedMemoryFootprint = estimatedMemory;
            result.MaxDepth = maxDepth;

            if(result.NodeCount > 0)
                result.AverageKeysPerNode = (double)(keysInInternalNodes.Count + keysInLeafNodes.Count) / result.NodeCount;
            if(result.LeafNodeCount > 0)
                result.AverageKeysPerLeafNode = (double)keysInLeafNodes.Count / result.LeafNodeCount;
            if(result.InternalNodeCount > 0)
                result.AverageKeysPerInternalNode = (double)keysInInternalNodes.Count / result.InternalNodeCount;


            Console.WriteLine($"Analysis Complete: N={N}, UseFullRange={UseFullRangeParameter}, EstimatedMemory={result.EstimatedMemoryFootprint}, Nodes={result.NodeCount}, MaxDepth={result.MaxDepth}, TotalKeys={result.TotalKeys}");
            return result;
        }
    }
}
