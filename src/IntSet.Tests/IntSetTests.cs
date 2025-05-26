
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Kibnet.Tests
{
    public partial class IntSetTests
    {
        [Fact]
        public void GetEnumerableTest()
        {
            Assert.Equal(1, TestHelper.GetEnumerable(1, 1).Single());
            Assert.Equal(100, TestHelper.GetEnumerable(1, 100).Count());
            Assert.Equal(100, TestHelper.GetEnumerable(100, 1).Count());
        }

        [Fact]
        public void SimpleContainsTest()
        {
            var intSet = new Kibnet.IntSet();
            for (int i = 0; i < 100000; i++)
            {
                intSet.Add(i);
            }
            for (int i = 0; i < 100000; i++)
            {
                Assert.True(intSet.Contains(i));
            }
            for (int i = 100000; i < 200000; i++)
            {
                Assert.False(intSet.Contains(i));
            }
        }

        [Fact]
        public void SimpleDeleteTest()
        {
            var intSet = new Kibnet.IntSet();

            for (int i = 100000; i < 200000; i++)
            {
                intSet.Add(i);
                Assert.True(intSet.Contains(i));
                intSet.Remove(i);
                Assert.False(intSet.Contains(i));
            }
        }

        [Theory]
        [InlineData(1007, 1009)]
        [InlineData(int.MinValue, int.MinValue + 1000)]
        [InlineData(int.MaxValue - 1000, int.MaxValue)]
        [InlineData(-500, 500)]
        [InlineData(-256, 256)]
        public void IterationDeleteTest(int from, int to)
        {
            var intSet = new Kibnet.IntSet();

            for (int i = from; i < to; i++)
            {
                intSet.Add(i);
            }

            for (int i = from; i < to; i++)
            {
                Assert.True(intSet.Contains(i));
                intSet.Remove(i);
                for (int j = from; j < to; j++)
                {
                    if (i >= j)
                        Assert.False(intSet.Contains(j));
                    else
                        Assert.True(intSet.Contains(j));
                }
            }
        }

        [Fact]
        public void DoubleAddContainsTest()
        {
            var intSet = new Kibnet.IntSet();
            for (int i = 0; i < 100000; i++)
            {
                intSet.Add(i);
            }
            for (int i = 0; i < 100000; i++)
            {
                intSet.Add(i);
            }
            for (int i = 0; i < 100000; i++)
            {
                Assert.True(intSet.Contains(i));
            }
            for (int i = 100000; i < 200000; i++)
            {
                Assert.False(intSet.Contains(i));
            }
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(0)]
        public void BoundContainsTest(int value)
        {
            var intSet = new Kibnet.IntSet();
            {
                intSet.Add(value);
            }
            Assert.True(intSet.Contains(value));
        }

        [Fact]
        public void OrderedEnumerateTest()
        {
            var intSet = new Kibnet.IntSet(TestHelper.GetEnumerable(100000, 0));

            var last = -1;
            foreach (var i in intSet)
            {
                Assert.Equal(last + 1, i);
                last = i;
            }
        }


        [Theory]
        [InlineData(int.MinValue, int.MinValue + 100000)]
        [InlineData(int.MaxValue - 100000, int.MaxValue)]
        [InlineData(-50000, 50000)]
        [InlineData((1<<3)-50000, (1 << 3)+50000)]
        [InlineData((1<<6)-50000, (1 << 6)+50000)]
        [InlineData((1<<9)-50000, (1 << 9)+50000)]
        [InlineData((1<<12)-50000, (1 << 12)+50000)]
        [InlineData((1<<15)-50000, (1 << 15)+50000)]
        [InlineData((1<<18)-50000, (1 << 18)+50000)]
        [InlineData((1<<21)-50000, (1 << 21)+50000)]
        [InlineData((1<<24)-50000, (1 << 24)+50000)]
        [InlineData((1<<27)-50000, (1 << 27)+50000)]
        [InlineData((1<<30)-50000, (1 << 30)+50000)]
        [InlineData(-5, 5)]
        public void OrderedIEnumerateTest(int from, int to)
        {
            var intSet = new Kibnet.IntSet(TestHelper.GetEnumerable(to, from));

            int? last = null;
            var enumerable = intSet as IEnumerable;
            foreach (int i in enumerable)
            {
                if (last == null)
                {
                    last = i;
                    continue;
                }
                Assert.Equal(last+1, i);
                last = i;
            }
        }

        [Theory]
        [InlineData(int.MinValue, int.MinValue + 100000)]
        [InlineData(int.MaxValue - 100000, int.MaxValue)]
        [InlineData(-50000, 50000)]
        public void EvensTest(int from, int to)
        {
            var intSet = new Kibnet.IntSet();
            foreach (var i in TestHelper.GetEnumerable(from, to))
            {
                if (i % 2 == 0)
                    intSet.Add(i);
            }

            foreach (var i in TestHelper.GetEnumerable(from, to))
            {
                if (i % 2 == 0)
                    Assert.True(intSet.Contains(i));
                else
                    Assert.False(intSet.Contains(i));
            }
        }

        [Theory]
        [InlineData(int.MinValue, int.MinValue + 100000)]
        [InlineData(int.MaxValue - 100000, int.MaxValue)]
        [InlineData(-50000, 50000)]
        public void NotEvensTest(int from, int to)
        {
            var intSet = new Kibnet.IntSet();
            foreach (var i in TestHelper.GetEnumerable(from, to))
            {
                if (i % 2 != 0)
                    intSet.Add(i);
            }

            foreach (var i in TestHelper.GetEnumerable(from, to))
            {
                if (i % 2 != 0)
                    Assert.True(intSet.Contains(i));
                else
                    Assert.False(intSet.Contains(i));
            }
        }

        [Theory]
        [InlineData(int.MinValue, int.MinValue + 100000)]
        [InlineData(int.MaxValue - 100000, int.MaxValue)]
        [InlineData(-50000, 50000)]
        public void FullSetTest(int from, int to)
        {
            var intSet = new Kibnet.IntSet(false, true);
            intSet.Add(0);

            foreach (var i in TestHelper.GetEnumerable(from, to))
            {
                Assert.True(intSet.Contains(i));
            }
        }

        [Theory]
        [InlineData(int.MinValue, int.MinValue + 100000, 997)]
        [InlineData(int.MaxValue - 100000, int.MaxValue, 997)]
        [InlineData(-500000, 500000, 997)]
        [InlineData(int.MinValue, int.MaxValue, 214747777)]
        public void FullSteppedSetTest(int from, int to, int step)
        {
            var intSet = new Kibnet.IntSet(false, true);
            intSet.Add(0);

            for (int i = from; i < to; i += step)
            {
                Assert.True(intSet.Contains(i));
            }
        }

        [Theory]
        [InlineData(int.MinValue, int.MinValue + 100000)]
        [InlineData(int.MaxValue - 100000, int.MaxValue)]
        [InlineData(-50000, 50000)]
        public void EmptySetTest(int from, int to)
        {
            var intSet = new Kibnet.IntSet();

            foreach (var i in TestHelper.GetEnumerable(from, to))
            {
                Assert.False(intSet.Contains(i));
            }
        }

        [Fact]
        public void Card0FullTest()
        {
            var intSet = new Kibnet.IntSet();
            intSet.Add(0);
            var card0 = intSet.root.Cards.FirstOrDefault(card => card != null);
            card0.Full = true;
            card0.Cards = null;
            intSet.Add(0);

            Assert.True(intSet.Contains(0));
        }


        [Fact]
        public void FullEnumeratorTest()
        {
            var intSet = new Kibnet.IntSet(false, true);

            var last = int.MinValue;
            var first = true;
            foreach (var i in intSet.Take(100000))
            {
                if (first)
                {
                    Assert.Equal(last, i);
                    first = false;
                }

                else
                    Assert.Equal(last + 1, i);
                Assert.True(intSet.Contains(i));
                last = i;
            }
        }

        [Fact]
        public void FullSetToStringTest()
        {
            var intSet = new Kibnet.IntSet(false, true);
            var rootString = intSet.root.ToString();

            Assert.Equal("1111111111111111111111111111111111111111111111111111111111111111", rootString);
        }

        [Fact]
        public void FiveLevelSetToStringTest()
        {
            var card = new Kibnet.IntSet.Card(5) { Full = true };

            var cardString = card.ToString();

            Assert.Equal("1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111", cardString);
        }

        [Fact]
        public void OneSetToStringTest()
        {
            var intSet = new Kibnet.IntSet();
            intSet.Add(1);
            foreach (int indexCard0 in intSet.root)
            {
                var card0 = intSet.root.Cards[indexCard0];
                Assert.Equal("1000000000000000000000000000000000000000000000000000000000000000", card0.ToString());
                foreach (int indexCard1 in card0)
                {
                    var card1 = card0.Cards[indexCard1];
                    Assert.Equal("1000000000000000000000000000000000000000000000000000000000000000", card1.ToString());
                    foreach (int indexCard2 in card1)
                    {
                        var card2 = card1.Cards[indexCard2];
                        Assert.Equal("1000000000000000000000000000000000000000000000000000000000000000", card2.ToString());
                        foreach (int indexCard3 in card2)
                        {
                            var card3 = card2.Cards[indexCard3];
                            Assert.Equal("0100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", card3.ToString());
                            foreach (byte byteValue in card3)
                            {
                                Assert.Equal("2", byteValue.ToString());
                                break;
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public void IntersectWithTest()
        {
            var intSet = new Kibnet.IntSet(TestHelper.GetEnumerable(100000, 100));
            
            var intSet2 = new Kibnet.IntSet(TestHelper.GetEnumerable(10000, 0));
            
            intSet.IntersectWith(intSet2);

            var last = 99;
            var enumerable = intSet as IEnumerable;
            Assert.Equal(100, intSet.First());
            foreach (int i in enumerable)
            {
                Assert.Equal(last + 1, i);
                last = i;
            }
            Assert.Equal(10000, intSet.Last());
        }

        [Fact]
        public void IntersectWithIEnumerableTest()
        {
            var intSet = new Kibnet.IntSet(TestHelper.GetEnumerable(100000, 100));
            
            var intSet2 = TestHelper.GetEnumerable(10000, 0);

            intSet.IntersectWith(intSet2);

            var last = 99;
            var enumerable = intSet as IEnumerable;
            Assert.Equal(100, intSet.First());
            foreach (int i in enumerable)
            {
                Assert.Equal(last + 1, i);
                last = i;
            }
            Assert.Equal(10000, intSet.Last());
        }

        [Fact]
        public void CopyToTest()
        {
            var intSet = new Kibnet.IntSet(TestHelper.GetEnumerable(100000, 1));
            
            var array = new int[intSet.Count];
            intSet.CopyTo(array);

            var last = 0;
            Assert.Equal(1, intSet.First());
            foreach (int i in array)
            {
                Assert.Equal(last + 1, i);
                last = i;
            }
            Assert.Equal(100000, intSet.Last());
        }

        [Fact]
        public void IsProperSubsetOfTest()
        {
            var intSet = new Kibnet.IntSet(TestHelper.GetEnumerable(100000, 100));

            var intSet2 = new Kibnet.IntSet(TestHelper.GetEnumerable(10000, 0));

            var intSet3 = new Kibnet.IntSet(TestHelper.GetEnumerable(100000, 0));

            Assert.False(intSet.IsProperSubsetOf(intSet2));
            Assert.True(intSet.IsProperSubsetOf(intSet3));
        }

        [Fact]
        public void IsProperSubsetOfIEnumerableTest()
        {
            var intSet = new Kibnet.IntSet(TestHelper.GetEnumerable(100000, 100));
            
            var intSet2 = TestHelper.GetEnumerable(10000, 0);

            var intSet3 = TestHelper.GetEnumerable(100000, 0);

            Assert.False(intSet.IsProperSubsetOf(intSet2));
            Assert.True(intSet.IsProperSubsetOf(intSet3));
        }        
        
        [Fact]
        public void ExceptWithTest()
        {
            var intSet = new Kibnet.IntSet(TestHelper.GetEnumerable(100000, 100));

            var intSet2 = new Kibnet.IntSet(TestHelper.GetEnumerable(10000, 0));

            intSet.ExceptWith(intSet2);

            intSet.EqualRange(10001, 100000);
        }

        [Fact]
        public void Contains_DefaultValue_ReturnsFalse()
        {
            var set = new Kibnet.IntSet();          // изначально все биты = 0
            Assert.False(set.Contains(0));   // должно вернуть false
            Assert.False(set.Contains(42));  // любое значение
        }

        [Fact]
        public void CopyTo_MatchExactSpace_CopiesAll()
        {
            var set = new Kibnet.IntSet();
            set.Add(1);
            set.Add(2);
            set.Add(3);
            var dst = new int[5];
            set.CopyTo(dst, 2);
            Assert.Equal(new[] { 0, 0, 1, 2, 3 }, dst);
        }

        #region GetElementsInRange Tests

        [Fact]
        public void GetElementsInRange_EmptySet()
        {
            var set = new Kibnet.IntSet();
            Assert.Equal(new List<int>(), set.GetElementsInRange(1, 10).ToList());
            Assert.Equal(new List<int>(), set.GetElementsInRange(10, 1).ToList());
        }

        [Fact]
        public void GetElementsInRange_PopulatedSet_Ascending()
        {
            var set = new Kibnet.IntSet(new[] { 1, 5, 10, 15, 20, 25 });
            Assert.Equal(new List<int> { 1, 5, 10, 15, 20, 25 }, set.GetElementsInRange(1, 25).ToList());
            Assert.Equal(new List<int> { 10, 15, 20 }, set.GetElementsInRange(10, 20).ToList());
            Assert.Equal(new List<int> { 1, 5 }, set.GetElementsInRange(0, 5).ToList());
            Assert.Equal(new List<int> { 20, 25 }, set.GetElementsInRange(20, 30).ToList());
            Assert.Equal(new List<int>(), set.GetElementsInRange(11, 14).ToList());
            Assert.Equal(new List<int> { 1, 5, 10, 15, 20, 25 }, set.GetElementsInRange(0, 30).ToList());
            Assert.Equal(new List<int> { 5, 10, 15 }, set.GetElementsInRange(5, 15).ToList());
        }

        [Fact]
        public void GetElementsInRange_PopulatedSet_Descending()
        {
            var set = new Kibnet.IntSet(new[] { 1, 5, 10, 15, 20, 25 });
            Assert.Equal(new List<int> { 25, 20, 15, 10, 5, 1 }, set.GetElementsInRange(25, 1).ToList());
            Assert.Equal(new List<int> { 20, 15, 10 }, set.GetElementsInRange(20, 10).ToList());
            Assert.Equal(new List<int> { 25, 20 }, set.GetElementsInRange(30, 20).ToList());
            Assert.Equal(new List<int> { 5, 1 }, set.GetElementsInRange(5, 0).ToList());
            Assert.Equal(new List<int>(), set.GetElementsInRange(14, 11).ToList());
            Assert.Equal(new List<int> { 25, 20, 15, 10, 5, 1 }, set.GetElementsInRange(30, 0).ToList());
            Assert.Equal(new List<int> { 15, 10, 5 }, set.GetElementsInRange(15, 5).ToList());
        }

        [Fact]
        public void GetElementsInRange_PopulatedSet_SingleElementRange()
        {
            var set = new Kibnet.IntSet(new[] { 1, 5, 10, 15, 20 });
            Assert.Equal(new List<int> { 10 }, set.GetElementsInRange(10, 10).ToList());
            Assert.Equal(new List<int>(), set.GetElementsInRange(11, 11).ToList());
            Assert.Equal(new List<int> { 1 }, set.GetElementsInRange(1, 1).ToList());
            Assert.Equal(new List<int> { 20 }, set.GetElementsInRange(20, 20).ToList());
        }

        [Fact]
        public void GetElementsInRange_ComplexScenarios_Gaps()
        {
            var set = new Kibnet.IntSet(new[] { 1, 2, 3, 10, 11, 12, 20, 21, 22 });
            Assert.Equal(new List<int> { 3, 10, 11 }, set.GetElementsInRange(3, 11).ToList());
            Assert.Equal(new List<int> { 11, 10, 3 }, set.GetElementsInRange(11, 3).ToList());
            Assert.Equal(new List<int> { 3, 10 }, set.GetElementsInRange(3, 10).ToList()); // Test case name was 'Asc across gap'
            Assert.Equal(new List<int> { 10, 3 }, set.GetElementsInRange(10, 3).ToList()); // Test case name was 'Desc across gap'
        }

        [Fact]
        public void GetElementsInRange_ComplexScenarios_NegativeNumbers()
        {
            var negSet = new Kibnet.IntSet(new[] { -10, -5, 0, 5, 10 });
            Assert.Equal(new List<int> { -5, 0, 5 }, negSet.GetElementsInRange(-5, 5).ToList());
            Assert.Equal(new List<int> { 5, 0, -5 }, negSet.GetElementsInRange(5, -5).ToList());
            Assert.Equal(new List<int> { -10, -5 }, negSet.GetElementsInRange(-15, -5).ToList());
            Assert.Equal(new List<int> { -5, -10 }, negSet.GetElementsInRange(-5, -15).ToList());
            Assert.Equal(new List<int> { -10 }, negSet.GetElementsInRange(-10, -10).ToList());
            Assert.Equal(new List<int> { 0 }, negSet.GetElementsInRange(0, 0).ToList());
             Assert.Equal(new List<int> { 10 }, negSet.GetElementsInRange(10, 10).ToList());
        }

        [Fact]
        public void GetElementsInRange_FullRange_MinMaxInt()
        {
            var set = new Kibnet.IntSet(new[] { int.MinValue, 0, int.MaxValue });
            Assert.Equal(new List<int> { int.MinValue, 0, int.MaxValue }, set.GetElementsInRange(int.MinValue, int.MaxValue).ToList());
            Assert.Equal(new List<int> { int.MaxValue, 0, int.MinValue }, set.GetElementsInRange(int.MaxValue, int.MinValue).ToList());
        }
        
        [Fact]
        public void GetElementsInRange_LargeNumbers()
        {
            var set = new Kibnet.IntSet(new[] { 1000000, 2000000, 3000000, int.MaxValue - 5, int.MaxValue });
            Assert.Equal(new List<int> { 1000000, 2000000 }, set.GetElementsInRange(1000000, 2500000).ToList());
            Assert.Equal(new List<int> { 2000000, 1000000 }, set.GetElementsInRange(2500000, 1000000).ToList());
            Assert.Equal(new List<int> { int.MaxValue - 5, int.MaxValue }, set.GetElementsInRange(int.MaxValue - 10, int.MaxValue).ToList());
            Assert.Equal(new List<int> { int.MaxValue, int.MaxValue -5 }, set.GetElementsInRange(int.MaxValue, int.MaxValue - 10).ToList());
        }

        [Fact]
        public void GetElementsInRange_RangeOutsidePopulatedData()
        {
            var set = new Kibnet.IntSet(new[] { 10, 20, 30 });
            Assert.Equal(new List<int>(), set.GetElementsInRange(1, 5).ToList()); // Range before
            Assert.Equal(new List<int>(), set.GetElementsInRange(5, 1).ToList());
            Assert.Equal(new List<int>(), set.GetElementsInRange(35, 40).ToList()); // Range after
            Assert.Equal(new List<int>(), set.GetElementsInRange(40, 35).ToList());
            Assert.Equal(new List<int>(), set.GetElementsInRange(22, 28).ToList()); // Range between elements
            Assert.Equal(new List<int>(), set.GetElementsInRange(28, 22).ToList());
        }

        [Fact]
        public void GetElementsInRange_SingleElementInSet_MatchingRange()
        {
            var set = new Kibnet.IntSet(new[] { 42 });
            Assert.Equal(new List<int> { 42 }, set.GetElementsInRange(42, 42).ToList());
            Assert.Equal(new List<int> { 42 }, set.GetElementsInRange(40, 45).ToList());
            Assert.Equal(new List<int> { 42 }, set.GetElementsInRange(45, 40).ToList());
        }

        [Fact]
        public void GetElementsInRange_SingleElementInSet_NonMatchingRange()
        {
            var set = new Kibnet.IntSet(new[] { 42 });
            Assert.Equal(new List<int>(), set.GetElementsInRange(10, 20).ToList());
            Assert.Equal(new List<int>(), set.GetElementsInRange(50, 60).ToList());
        }
        
        // Test with a set that forces traversal through multiple levels of cards
        [Fact]
        public void GetElementsInRange_DeepTraversal()
        {
            // These numbers are chosen to likely span different i0, i1, i2, i3 blocks
            var data = new List<int> { 0, 1, 2, 
                                       (1 << 8) + 5, (1 << 8) + 10, // Different i3
                                       (1 << 14) + 7, (1 << 14) + 15, // Different i2
                                       (1 << 20) + 3, (1 << 20) + 9,  // Different i1
                                       (1 << 26) + 100, (1 << 26) + 200, // Different i0
                                       int.MaxValue - 10, int.MaxValue -1, int.MaxValue};
            var set = new Kibnet.IntSet(data);

            var expectedAsc = data.Where(x => x >= ((1 << 8) + 5) && x <= ((1 << 26) + 100)).OrderBy(x => x).ToList();
            Assert.Equal(expectedAsc, set.GetElementsInRange((1 << 8) + 5, (1 << 26) + 100).ToList());

            var expectedDesc = data.Where(x => x >= ((1 << 14) + 7) && x <= ((1 << 20) + 9)).OrderByDescending(x => x).ToList();
            Assert.Equal(expectedDesc, set.GetElementsInRange((1 << 20) + 9, (1 << 14) + 7).ToList());
            
            // Range including int.MaxValue
            Assert.Equal(new List<int> { (1 << 26) + 200, int.MaxValue - 10, int.MaxValue -1, int.MaxValue }, set.GetElementsInRange((1 << 26) + 150, int.MaxValue).ToList());
            Assert.Equal(new List<int> { int.MaxValue, int.MaxValue -1, int.MaxValue - 10, (1 << 26) + 200 }, set.GetElementsInRange(int.MaxValue, (1 << 26) + 150).ToList());

            // Range including int.MinValue (if 0 is considered MinValue for positive range)
            // If IntSet can store negative numbers, this test would be more relevant with actual int.MinValue
             var dataWithNeg = new List<int>(data);
             dataWithNeg.Add(int.MinValue);
             dataWithNeg.Add(int.MinValue+1);
             var setWithNeg = new Kibnet.IntSet(dataWithNeg);
             dataWithNeg.Sort(); // For expected list

            Assert.Equal(dataWithNeg.Where(x => x >= int.MinValue && x <= 2).ToList(), setWithNeg.GetElementsInRange(int.MinValue, 2).ToList());
            Assert.Equal(dataWithNeg.Where(x => x >= int.MinValue && x <= 2).OrderByDescending(x=>x).ToList(), setWithNeg.GetElementsInRange(2, int.MinValue).ToList());

        }

        #endregion
    }
}
