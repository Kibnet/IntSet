using System;
using System.Linq;
using Xunit;

namespace IntSet.Tests
{
    public partial class IntSetTests
    {
        [Fact]
        public void Add_NewItem_ReturnsTrue_AndContainsIt()
        {
            var set = new Kibnet.IntSet();
            Assert.True(set.Add(42));
            Assert.True(set.Contains(42));
            Assert.Equal(1, set.Count);
        }

        [Fact]
        public void Add_Duplicate_ReturnsFalse_CountUnchanged()
        {
            var set = new Kibnet.IntSet();
            set.Add(7);
            Assert.False(set.Add(7));
            Assert.Equal(1, set.Count);
        }

        [Fact]
        public void Remove_ExistingItem_ReturnsTrue_AndRemovesIt()
        {
            var set = new Kibnet.IntSet();
            set.Add(100);
            Assert.True(set.Remove(100));
            Assert.False(set.Contains(100));
            Assert.Equal(0, set.Count);
        }

        [Fact]
        public void Remove_NonExisting_ReturnsFalse_CountUnchanged()
        {
            var set = new Kibnet.IntSet();
            Assert.False(set.Remove(5));
            Assert.Equal(0, set.Count);
        }

        [Fact]
        public void Contains_EmptySet_ReturnsFalse()
        {
            var set = new Kibnet.IntSet();
            Assert.False(set.Contains(123));
        }

        [Fact]
        public void Clear_EmptiesSet_CountZero()
        {
            var set = new Kibnet.IntSet(new[] { 1, 2, 3 });
            set.Clear();
            Assert.Empty(set);
            Assert.Equal(0, set.Count);
        }

        [Fact]
        public void Enumeration_YieldsAllAddedItems()
        {
            var items = new[] { 5, 10, 20 };
            var set = new Kibnet.IntSet(items);
            var result = set.ToList();
            Assert.Equal(items.OrderBy(x => x), result.OrderBy(x => x));
        }

        [Fact]
        public void CopyTo_FullCopy_WritesElementsInOrder()
        {
            var set = new Kibnet.IntSet(new[] { 1, 3, 5 });
            var dst = new int[5];
            set.CopyTo(dst, 1, set.Count);
            Assert.Equal(0, dst[0]);
            Assert.Equal(1, dst[1]);
            Assert.Equal(3, dst[2]);
            Assert.Equal(5, dst[3]);
            Assert.Equal(0, dst[4]);
        }

        [Fact]
        public void CopyTo_NullArray_ThrowsArgumentNullException()
        {
            var set = new Kibnet.IntSet();
            Assert.Throws<ArgumentNullException>(() => set.CopyTo(null, 0, 0));
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(2, 2)]  // array.Length=1, index+count > length
        public void CopyTo_InvalidParams_Throws(int index, int count)
        {
            var set = new Kibnet.IntSet(new[] { 1 });
            var arr = new int[1];
            Assert.ThrowsAny<ArgumentException>(() => set.CopyTo(arr, index, count));
        }

        [Fact]
        public void UnionWith_Null_ThrowsArgumentNullException()
        {
            var set = new Kibnet.IntSet();
            Assert.Throws<ArgumentNullException>(() => set.UnionWith(null));
        }

        [Fact]
        public void IntersectWith_Null_ThrowsArgumentNullException()
        {
            var set = new Kibnet.IntSet();
            Assert.Throws<ArgumentNullException>(() => set.IntersectWith(null));
        }

        [Fact]
        public void ExceptWith_RemovesOnlyPresentElements()
        {
            var set = new Kibnet.IntSet(new[] { 1, 2, 3 });
            set.ExceptWith(new[] { 2, 4 });
            Assert.False(set.Contains(2));
            Assert.True(set.Contains(1));
            Assert.True(set.Contains(3));
        }

        [Fact]
        public void SymmetricExceptWith_Null_ThrowsArgumentNullException()
        {
            var set = new Kibnet.IntSet();
            Assert.Throws<ArgumentNullException>(() => set.SymmetricExceptWith(null));
        }

        [Fact]
        public void IsSubsetOf_EmptySet_ReturnsTrue()
        {
            var set = new Kibnet.IntSet();
            Assert.True(set.IsSubsetOf(new[] { 10, 20 }));
        }

        [Fact]
        public void IsSubsetOf_ProperSubset_ReturnsTrue()
        {
            var sup = new Kibnet.IntSet(new[] { 1, 2, 3 });
            var sub = new Kibnet.IntSet(new[] { 1, 3 });
            Assert.True(sub.IsSubsetOf(sup));
        }

        [Fact]
        public void IsSubsetOf_NotSubset_ReturnsFalse()
        {
            var sup = new Kibnet.IntSet(new[] { 1, 2 });
            Assert.False(sup.IsSubsetOf(new[] { 1 }));
        }

        [Fact]
        public void IsSupersetOf_EmptyOther_ReturnsTrue()
        {
            var set = new Kibnet.IntSet(new[] { 1 });
            Assert.True(set.IsSupersetOf(Array.Empty<int>()));
        }

        [Fact]
        public void IsProperSupersetOf_EmptyOther_ReturnsTrueIfNonEmpty()
        {
            var set = new Kibnet.IntSet(new[] { 1 });
            Assert.True(set.IsProperSupersetOf(Array.Empty<int>()));
        }

        [Fact]
        public void Overlaps_NoCommon_ReturnsFalse()
        {
            var set = new Kibnet.IntSet(new[] { 1, 2 });
            Assert.False(set.Overlaps(new[] { 3, 4 }));
        }

        [Fact]
        public void Overlaps_Common_ReturnsTrue()
        {
            var set = new Kibnet.IntSet(new[] { 1, 2 });
            Assert.True(set.Overlaps(new[] { 2, 5 }));
        }

        [Fact]
        public void SetEquals_SameInstance_ReturnsTrue()
        {
            var set = new Kibnet.IntSet(new[] { 1, 2 });
            Assert.True(set.SetEquals(set));
        }

        [Fact]
        public void SetEquals_DifferentOrderButSameElements_ReturnsTrue()
        {
            var a = new Kibnet.IntSet(new[] { 1, 2, 3 });
            var b = new Kibnet.IntSet(new[] { 3, 1, 2 });
            Assert.True(a.SetEquals(b));
        }

        [Fact]
        public void SetEquals_DifferentCounts_ReturnsFalse()
        {
            var a = new Kibnet.IntSet(new[] { 1 });
            var b = new Kibnet.IntSet(new[] { 1, 2 });
            Assert.False(a.SetEquals(b));
        }

        [Fact]
        public void NullChecks_OnVariousPredicates_ThrowArgumentNullException()
        {
            var set = new Kibnet.IntSet();
            Assert.Throws<ArgumentNullException>(() => set.IsSubsetOf(null));
            Assert.Throws<ArgumentNullException>(() => set.IsSupersetOf(null));
            Assert.Throws<ArgumentNullException>(() => set.IsProperSubsetOf(null));
            Assert.Throws<ArgumentNullException>(() => set.IsProperSupersetOf(null));
            Assert.Throws<ArgumentNullException>(() => set.Overlaps(null));
            Assert.Throws<ArgumentNullException>(() => set.SetEquals(null));
        }
    }
}
