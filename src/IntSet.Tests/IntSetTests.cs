using System;
using System.Collections;
using System.Linq;
using Xunit;

namespace IntSet.Tests
{
    public class IntSetTests
    {
        [Fact]
        public void SimpleContainsTest()
        {
            var intSet = new Kibnet.IntSet.IntSet();
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
            var intSet = new Kibnet.IntSet.IntSet();

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
            var intSet = new Kibnet.IntSet.IntSet();

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
            var intSet = new Kibnet.IntSet.IntSet();
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
            var intSet = new Kibnet.IntSet.IntSet();
            {
                intSet.Add(value);
            }
            Assert.True(intSet.Contains(value));
        }

        [Fact]
        public void OrderedEnumerateTest()
        {
            var intSet = new Kibnet.IntSet.IntSet();
            for (int i = 100000; i >= 0; i--)
            {
                intSet.Add(i);
            }

            var last = -1;
            foreach (var i in intSet)
            {
                Assert.Equal(last + 1, i);
                last = i;
            }
        }

        [Fact]
        public void OrderedIEnumerateTest()
        {
            var intSet = new Kibnet.IntSet.IntSet();
            for (int i = 100000; i >= 0; i--)
            {
                intSet.Add(i);
            }

            var last = -1;
            var enumerable = intSet as IEnumerable;
            foreach (int i in enumerable)
            {
                Assert.Equal(last + 1, i);
                last = i;
            }
        }

        [Theory]
        [InlineData(int.MinValue, int.MinValue + 100000)]
        [InlineData(int.MaxValue - 100000, int.MaxValue)]
        [InlineData(-50000, 50000)]
        public void EvensTest(int from, int to)
        {
            var intSet = new Kibnet.IntSet.IntSet();
            for (int i = from; i < to; i++)
            {
                if (i % 2 == 0)
                    intSet.Add(i);
            }

            for (int i = from; i < to; i++)
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
            var intSet = new Kibnet.IntSet.IntSet();
            for (int i = from; i < to; i++)
            {
                if (i % 2 != 0)
                    intSet.Add(i);
            }

            for (int i = from; i < to; i++)
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
            var intSet = new Kibnet.IntSet.IntSet(false, true);
            intSet.Add(0);

            for (int i = from; i < to; i++)
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
            var intSet = new Kibnet.IntSet.IntSet();

            for (int i = from; i < to; i++)
            {
                Assert.False(intSet.Contains(i));
            }
        }

        [Fact]
        public void Card0FullTest()
        {
            var intSet = new Kibnet.IntSet.IntSet();
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
            var intSet = new Kibnet.IntSet.IntSet(false, true);

            var last = -1;
            foreach (var i in intSet.Take(100000))
            {
                Assert.Equal(last + 1, i);
                Assert.True(intSet.Contains(i));
                last = i;
            }
        }

        [Fact]
        public void FullSetToStringTest()
        {
            var intSet = new Kibnet.IntSet.IntSet(false, true);
            var rootString = intSet.root.ToString();

            Assert.Equal("1111111111111111111111111111111111111111111111111111111111111111", rootString);
        }

        [Fact]
        public void FiveLevelSetToStringTest()
        {
            var card = new Kibnet.IntSet.IntSet.Card(5) { Full = true };

            var cardString = card.ToString();

            Assert.Equal("1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111", cardString);
        }

        [Fact]
        public void OneSetToStringTest()
        {
            var intSet = new Kibnet.IntSet.IntSet();
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
    }
}
