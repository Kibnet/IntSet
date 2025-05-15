using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Kibnet.Tests
{
    public static class TestHelper
    {
        public static IEnumerable<int> GetEnumerable(int start, int finish)
        {
            if (start == finish)
            {
                yield return start;
            }
            else if (start > finish)
            {
                for (int i = start; i >= finish && i > Int32.MinValue; i--)
                {
                    yield return i;
                }
            }
            else
            {
                for (int i = start; i <= finish && i < Int32.MaxValue; i++)
                {
                    yield return i;
                }
            }
        }

        public static void EqualRange(this Kibnet.IntSet intSet, int start, int finish)
        {
            var enumerable = intSet as IEnumerable;
            var last = start - 1;
            Assert.Equal(start, intSet.First());
            foreach (int i in enumerable)
            {
                Assert.Equal(last + 1, i);
                last = i;
            }
            Assert.Equal(finish, intSet.Last());
        }
    }
}