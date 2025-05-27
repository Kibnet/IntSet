using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Kibnet; // Assuming IntSet is in this namespace

namespace IntSet.Benchmarks
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    // Optional: Add [SimpleJob(RuntimeMoniker.Net80)] or other job configurations if needed
    public class SetOperationsBenchmarks
    {
        // Parameters for benchmark variations
        [Params(0, 100, 10000, 1000000)] // Added 0 to test edge cases
        public int Size;

        [Params("Dense", "Sparse")]
        public string DataType;

        private List<int> _dataA;
        private List<int> _dataB; // For binary set operations

        private Kibnet.IntSet _intSetA; // Fully qualify to avoid ambiguity if any
        private HashSet<int> _hashSetA;

        private Kibnet.IntSet _intSetB; // For binary set operations
        private HashSet<int> _hashSetB; // For binary set operations

        private int _itemToAdd;
        private int _itemToRemove;
        private int _itemToContain;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Random rand = new Random(42); // Use a fixed seed for reproducibility
            if (DataType == "Dense")
            {
                _dataA = Enumerable.Range(0, Size).ToList();
                // Ensure _dataB is also scaled by Size for dense, and has some overlap and some difference
                _dataB = Enumerable.Range(Size / 2, Size).ToList(); 
            }
            else // Sparse
            {
                // Generate Size unique random numbers for sparse data for _dataA
                var sparseA = new HashSet<int>();
                while(sparseA.Count < Size)
                {
                    sparseA.Add(rand.Next(0, Size * 10));
                }
                _dataA = sparseA.ToList();
                _dataA.Sort(); // Optional: Sort if order matters for setup, though not for sets

                // Generate Size unique random numbers for sparse data for _dataB
                var sparseB = new HashSet<int>();
                while(sparseB.Count < Size)
                {
                    // Ensure some potential overlap and difference with _dataA
                    sparseB.Add(rand.Next(0, Size * 10) + (Size * 5)); 
                }
                _dataB = sparseB.ToList();
                _dataB.Sort(); // Optional
            }

            // Determine items for Add, Remove, Contains operations
            if (Size > 0)
            {
                // Item not in _dataA for Add
                _itemToAdd = DataType == "Dense" ? Size : _dataA.Max() + 1; 
                if (_dataA.Contains(_itemToAdd)) // Ensure it's truly not in for sparse random
                {
                    _itemToAdd = _dataA.Max() + rand.Next(1,100);
                     while(_dataA.Contains(_itemToAdd)) { // find one not in the set
                        _itemToAdd++;
                    }
                }

                _itemToRemove = _dataA[Size / 2]; // Item in _dataA
                _itemToContain = _dataA[Size / 2]; // Item in _dataA
            }
            else
            {
                _itemToAdd = 0; // Item to add to an empty set
                _itemToRemove = 0; // Item to attempt to remove from an empty set
                _itemToContain = 0; // Item to check in an empty set
            }
        }

        [IterationSetup]
        public void IterationSetup()
        {
            // Initialize sets for each iteration to ensure a clean state
            // Pass empty list if _dataA or _dataB is null (e.g. if Size is 0 and GlobalSetup didn't init them)
            _intSetA = new Kibnet.IntSet(_dataA ?? new List<int>());
            _hashSetA = new HashSet<int>(_dataA ?? new List<int>());

            _intSetB = new Kibnet.IntSet(_dataB ?? new List<int>());
            _hashSetB = new HashSet<int>(_dataB ?? new List<int>());
        }

        // --- Add Operation ---
        [BenchmarkCategory("Add"), Benchmark]
        public void IntSet_Add() => _intSetA.Add(_itemToAdd);

        [BenchmarkCategory("Add"), Benchmark]
        public void HashSet_Add() => _hashSetA.Add(_itemToAdd);

        // --- Contains Operation ---
        [BenchmarkCategory("Contains"), Benchmark]
        public bool IntSet_Contains() => _intSetA.Contains(_itemToContain);

        [BenchmarkCategory("Contains"), Benchmark]
        public bool HashSet_Contains() => _hashSetA.Contains(_itemToContain);

        // --- Remove Operation ---
        [BenchmarkCategory("Remove"), Benchmark]
        public bool IntSet_Remove() // Return bool for consistency with HashSet.Remove
        {
            if (Size > 0) return _intSetA.Remove(_itemToRemove);
            return false; // Or handle as appropriate for empty set
        }

        [BenchmarkCategory("Remove"), Benchmark]
        public bool HashSet_Remove()
        {
            if (Size > 0) return _hashSetA.Remove(_itemToRemove);
            return false;
        }

        // --- UnionWith Operation ---
        [BenchmarkCategory("Union"), Benchmark]
        public void IntSet_UnionWith() => _intSetA.UnionWith(_intSetB);

        [BenchmarkCategory("Union"), Benchmark]
        public void HashSet_UnionWith() => _hashSetA.UnionWith(_hashSetB);

        // --- IntersectWith Operation ---
        [BenchmarkCategory("Intersection"), Benchmark]
        public void IntSet_IntersectWith() => _intSetA.IntersectWith(_intSetB);

        [BenchmarkCategory("Intersection"), Benchmark]
        public void HashSet_IntersectWith() => _hashSetA.IntersectWith(_hashSetB);

        // --- ExceptWith Operation ---
        [BenchmarkCategory("Except"), Benchmark]
        public void IntSet_ExceptWith() => _intSetA.ExceptWith(_intSetB);

        [BenchmarkCategory("Except"), Benchmark]
        public void HashSet_ExceptWith() => _hashSetA.ExceptWith(_hashSetB);

        // --- SymmetricExceptWith Operation ---
        [BenchmarkCategory("SymmetricExcept"), Benchmark]
        public void IntSet_SymmetricExceptWith() => _intSetA.SymmetricExceptWith(_intSetB);

        [BenchmarkCategory("SymmetricExcept"), Benchmark]
        public void HashSet_SymmetricExceptWith() => _hashSetA.SymmetricExceptWith(_hashSetB);
    }
}
