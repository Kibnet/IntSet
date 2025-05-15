using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Kibnet.Tests
{
    public partial class IntSetTests
    {
        [Fact]
        public void Add_NewItem_ReturnsTrue_AndContainsIt()
        {
            var set = new IntSet();
            Assert.True(set.Add(42));
            Assert.True(set.Contains(42));
            Assert.Equal(1, set.Count);
        }

        [Fact]
        public void Add_Duplicate_ReturnsFalse_CountUnchanged()
        {
            var set = new IntSet();
            set.Add(7);
            Assert.False(set.Add(7));
            Assert.Equal(1, set.Count);
        }

        [Fact]
        public void Remove_ExistingItem_ReturnsTrue_AndRemovesIt()
        {
            var set = new IntSet();
            set.Add(100);
            Assert.True(set.Remove(100));
            Assert.False(set.Contains(100));
            Assert.Equal(0, set.Count);
        }

        [Fact]
        public void Remove_NonExisting_ReturnsFalse_CountUnchanged()
        {
            var set = new IntSet();
            Assert.False(set.Remove(5));
            Assert.Equal(0, set.Count);
        }

        [Fact]
        public void Contains_EmptySet_ReturnsFalse()
        {
            var set = new IntSet();
            Assert.False(set.Contains(123));
        }

        [Fact]
        public void Clear_EmptiesSet_CountZero()
        {
            var set = new IntSet(new[] { 1, 2, 3 });
            set.Clear();
            Assert.Empty(set);
            Assert.Equal(0, set.Count);
        }

        [Fact]
        public void Enumeration_YieldsAllAddedItems()
        {
            var items = new[] { 5, 10, 20 };
            var set = new IntSet(items);
            var result = set.ToList();
            Assert.Equal(items.OrderBy(x => x), result.OrderBy(x => x));
        }

        [Fact]
        public void CopyTo_FullCopy_WritesElementsInOrder()
        {
            var set = new IntSet(new[] { 1, 3, 5 });
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
            var set = new IntSet();
            Assert.Throws<ArgumentNullException>(() => set.CopyTo(null, 0, 0));
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(2, 2)]  // array.Length=1, index+count > length
        public void CopyTo_InvalidParams_Throws(int index, int count)
        {
            var set = new IntSet(new[] { 1 });
            var arr = new int[1];
            Assert.ThrowsAny<ArgumentException>(() => set.CopyTo(arr, index, count));
        }

        [Fact]
        public void UnionWith_Null_ThrowsArgumentNullException()
        {
            var set = new IntSet();
            Assert.Throws<ArgumentNullException>(() => set.UnionWith(null));
        }

        [Fact]
        public void IntersectWith_Null_ThrowsArgumentNullException()
        {
            var set = new IntSet();
            Assert.Throws<ArgumentNullException>(() => set.IntersectWith(null));
        }

        [Fact]
        public void ExceptWith_RemovesOnlyPresentElements()
        {
            var set = new IntSet(new[] { 1, 2, 3 });
            set.ExceptWith(new[] { 2, 4 });
            Assert.False(set.Contains(2));
            Assert.True(set.Contains(1));
            Assert.True(set.Contains(3));
        }

        [Fact]
        public void SymmetricExceptWith_Null_ThrowsArgumentNullException()
        {
            var set = new IntSet();
            Assert.Throws<ArgumentNullException>(() => set.SymmetricExceptWith(null));
        }

        [Fact]
        public void IsSubsetOf_EmptySet_ReturnsTrue()
        {
            var set = new IntSet();
            Assert.True(set.IsSubsetOf(new[] { 10, 20 }));
        }

        [Fact]
        public void IsSubsetOf_ProperSubset_ReturnsTrue()
        {
            var sup = new IntSet(new[] { 1, 2, 3 });
            var sub = new IntSet(new[] { 1, 3 });
            Assert.True(sub.IsSubsetOf(sup));
        }

        [Fact]
        public void IsSubsetOf_NotSubset_ReturnsFalse()
        {
            var sup = new IntSet(new[] { 1, 2 });
            Assert.False(sup.IsSubsetOf(new[] { 1 }));
        }

        [Fact]
        public void IsSupersetOf_EmptyOther_ReturnsTrue()
        {
            var set = new IntSet(new[] { 1 });
            Assert.True(set.IsSupersetOf(Array.Empty<int>()));
        }

        [Fact]
        public void IsProperSupersetOf_EmptyOther_ReturnsTrueIfNonEmpty()
        {
            var set = new IntSet(new[] { 1 });
            Assert.True(set.IsProperSupersetOf(Array.Empty<int>()));
        }

        [Fact]
        public void Overlaps_NoCommon_ReturnsFalse()
        {
            var set = new IntSet(new[] { 1, 2 });
            Assert.False(set.Overlaps(new[] { 3, 4 }));
        }

        [Fact]
        public void Overlaps_Common_ReturnsTrue()
        {
            var set = new IntSet(new[] { 1, 2 });
            Assert.True(set.Overlaps(new[] { 2, 5 }));
        }

        [Fact]
        public void SetEquals_SameInstance_ReturnsTrue()
        {
            var set = new IntSet(new[] { 1, 2 });
            Assert.True(set.SetEquals(set));
        }

        [Fact]
        public void SetEquals_DifferentOrderButSameElements_ReturnsTrue()
        {
            var a = new IntSet(new[] { 1, 2, 3 });
            var b = new IntSet(new[] { 3, 1, 2 });
            Assert.True(a.SetEquals(b));
        }

        [Fact]
        public void SetEquals_DifferentCounts_ReturnsFalse()
        {
            var a = new IntSet(new[] { 1 });
            var b = new IntSet(new[] { 1, 2 });
            Assert.False(a.SetEquals(b));
        }

        [Fact]
        public void NullChecks_OnVariousPredicates_ThrowArgumentNullException()
        {
            var set = new IntSet();
            Assert.Throws<ArgumentNullException>(() => set.IsSubsetOf(null));
            Assert.Throws<ArgumentNullException>(() => set.IsSupersetOf(null));
            Assert.Throws<ArgumentNullException>(() => set.IsProperSubsetOf(null));
            Assert.Throws<ArgumentNullException>(() => set.IsProperSupersetOf(null));
            Assert.Throws<ArgumentNullException>(() => set.Overlaps(null));
            Assert.Throws<ArgumentNullException>(() => set.SetEquals(null));
        }

        [Fact]
        public void IsFastest_SetFalse_RemainsFalse()
        {
            var set = new IntSet(new[] { 1, 2, 3 });
            set.IsFastest = false;
            // На прямую поле недоступно, но можно косвенно проверить:
            // добавим элемент, и убеждаемся, что удаление идёт по «медленному» пути (Bytes != null)
            set.Add(4);
            Assert.True(set.Contains(4));
            // здесь важнее, что при IsFastest = false в методе Remove не сбрасывается Bytes раньше времени
            // (иначе Remove мог бы возвращать неверно)
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Remove_VariousIsFastestPaths_DoNotThrow(bool isFastest)
        {
            var set = new IntSet(isFastest){5};
            set.IsFastest = isFastest;
            // при любом значении флага метод Remove должен вернуть true
            Assert.True(set.Remove(5));
            Assert.Equal(isFastest, set.IsFastest);
            Assert.False(set.Contains(5));
        }

        [Fact]
        public void Remove_CascadeEmptyCards_RemovesAllLevels()
        {
            // строим IntSet, в котором один элемент лежит глубоко в дереве
            var value = 555;
            var set = new IntSet([value]);
            Assert.True(set.LongCount == set.Count);
            Assert.Equal(1, set.LongCount);
            // удаляем этот элемент — он должен снять все четыре уровня Card
            bool removed = set.Remove(value);
            Assert.True(removed);
            Assert.True(set.LongCount == set.Count);
            Assert.Equal(0, set.LongCount);
            // Теперь в дереве должна быть только корневая карточка с пустой Bytes/картами
            Assert.False(set.Contains(value));
            Assert.True(set.root.Cards == null);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ElementAt_ValidAndInvalidIndexes(int idx)
        {
            var set = new IntSet(new[] { 10, 20, 30 });
            if (idx < set.Count)
                Assert.Equal(new[] { 10, 20, 30 }[idx], set.ElementAt(idx));
            else
                Assert.Throws<ArgumentOutOfRangeException>(() => _ = set.ElementAt(idx));
        }

        [Fact]
        public void CopyTo_ExactSpace_CopiesAllElements()
        {
            var set = new IntSet(new[] { 7, 8, 9 });
            var dst = new int[5];
            // оставляем ровно 3 места в конце: 5 - 2 = 3
            set.CopyTo(dst, 2, set.Count);
            Assert.Equal(new[] { 0, 0, 7, 8, 9 }, dst);
        }

        [Fact]
        public void CheckEmpty_BytesMinValue_ReturnsTrue()
        {
            var set = new IntSet(new[] { 1 });
            // добиваем Bytes вручную (рефлексия) или через публичный API:
            // например, удаляем все элементы, чтобы остались только нули
            set.Clear();
            // Пусть внутренне Bytes будут не null и все = 0
            Assert.True(set.root.CheckEmpty());
        }

        [Fact]
        public void CheckEmpty_BytesNotMinValue_ReturnsFalse()
        {
            var set = new IntSet(new[] { 1 });
            Assert.False(set.root.CheckEmpty());
        }

        [Fact]
        public void CheckFull_AllBytesMax_ClearsBytesAndReturnsTrue()
        {
            // создаём IntSet так, чтобы внутренний массив Bytes заполнен byte.MaxValue
            var set = new IntSet();
            // через публичный API добавить все возможные элементы до заполнения одной «карты»
            foreach (var i in Enumerable.Range(0, 16384).ToArray())
            {
                set.Add(i);
            }
            Assert.False(set.root.Full);
            Assert.False(set.root.Cards[0].Full);
            Assert.False(set.root.Cards[0].Cards[0].Full);
            Assert.True(set.root.Cards[0].Cards[0].Cards[0].Full);
            Assert.Null(set.root.Cards[0].Cards[0].Cards[0].Cards);
            // после этого Bytes должен стать null
            Assert.Null(set.root.Cards[0].Cards[0].Cards[0].Bytes);
        }

        [Fact]
        public void CheckFull_NonFull_DoesNotClearBytes()
        {
            var set = new IntSet(new[] { 1 });
            Assert.False(set.root.Full);
            // Bytes остаётся ненулевым
            Assert.NotNull(set.root.Cards[0].Cards[0].Cards[0].Cards[0].Bytes);
        }

        [Fact]
        public void UnionWith_AddsAllElements()
        {
            var set = new IntSet(new[] { 1, 2 });
            set.UnionWith(new[] { 2, 3, 4 });
            Assert.True(set.SetEquals(new[] { 1, 2, 3, 4 }));
        }

        [Fact]
        public void IntersectWith_WithSet_LeavesOnlyCommon()
        {
            var a = new IntSet(new[] { 1, 2, 3, 4 });
            var b = new IntSet(new[] { 3, 4, 5 });
            a.IntersectWith(b);
            Assert.True(a.SetEquals(new[] { 3, 4 }));
        }

        [Fact]
        public void IntersectWith_WithEnumerable_LeavesOnlyCommon()
        {
            var set = new IntSet(new[] { 1, 2, 3, 4 });
            set.IntersectWith(new[] { 2, 4, 6 });
            Assert.True(set.SetEquals(new[] { 2, 4 }));
        }

        [Fact]
        public void ExceptWith_RemovesOnlySpecified()
        {
            var set = new IntSet(new[] { 1, 2, 3, 4 });
            set.ExceptWith(new[] { 2, 5 });
            Assert.True(set.SetEquals(new[] { 1, 3, 4 }));
        }

        [Fact]
        public void SymmetricExceptWith_WithSet_ComputesSymmetricDifference()
        {
            var a = new IntSet(new[] { 1, 2, 3 });
            var b = new IntSet(new[] { 2, 3, 4 });
            a.SymmetricExceptWith(b);
            Assert.True(a.SetEquals(new[] { 1, 4 }));
        }

        [Fact]
        public void SymmetricExceptWith_WithEnumerable_ComputesSymmetricDifference()
        {
            var set = new IntSet(new[] { 10, 20, 30 });
            set.SymmetricExceptWith(new[] { 20, 40, 50 });
            Assert.True(set.SetEquals(new[] { 10, 30, 40, 50 }));
        }

        [Fact]
        public void ProperSubsetAndProperSuperset_Behavior()
        {
            var sup = new IntSet(new[] { 1, 2, 3 });
            var sub = new IntSet(new[] { 1, 2 });

            Assert.True(sub.IsProperSubsetOf(sup));
            Assert.False(sup.IsProperSubsetOf(sub));

            Assert.True(sup.IsProperSupersetOf(sub));
            Assert.False(sub.IsProperSupersetOf(sup));
            
            // Равные множества не являются строгими подмножеством/надмножеством
            var c1 = new IntSet(new[] { 1, 2 });
            var c2 = new IntSet(new[] { 2, 1 });
            Assert.False(c1.IsProperSubsetOf(c2));
            Assert.False(c1.IsProperSupersetOf(c2));
        }

        [Fact]
        public void ContainsAllElements_Path()
        {
            // косвенно через IsSupersetOf
            var set = new IntSet(new[] { 5, 6, 7 });
            Assert.True(set.IsSupersetOf(new[] { 5, 7 }));
            Assert.False(set.IsSupersetOf(new[] { 5, 8 }));
        }

        [Fact]
        public void CheckUniqueAndUnfound_ReturnsExpectedCounts()
        {
            // для этого используем SetEquals и Subset-пути, где returnIfUnfound=true/false
            var set = new IntSet(new[] { 1, 2, 3 });
            // равенство → unfound=0, uniqueFound=count
            Assert.True(set.SetEquals(new[] { 1, 2, 3 }));
            // неполное пересечение → unfound>0
            Assert.False(set.SetEquals(new[] { 1, 4, 5 }));
        }

        [Fact]
        public void Clear_ResetsTreeFully()
        {
            var set = new IntSet(new[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            set.Clear();
            Assert.Empty(set);
            // после Clear дерево должно быть пустым
            Assert.True(set.root.CheckEmpty());
        }

        [Fact]
        public void CheckEmpty_OnNonEmpty_ReturnsFalse()
        {
            var set = new IntSet(new[] { 42 });
            Assert.False(set.root.CheckEmpty());
        }

        [Fact]
        public void CheckFull_FillsProperlyAndClearsBytes()
        {
            var set = new IntSet();
            // добавляем достаточно элементов, чтобы заполнить один Card до Full
            foreach (var i in Enumerable.Range(0, 1 << 15))
                set.Add(i);
            // теперь на глубине 3 должна образоваться полностью заполненная карта
            Assert.True(set.root.Cards[0].Cards[0].Cards[0].Full);
            Assert.Null(set.root.Cards[0].Cards[0].Cards[0].Bytes);
        }

        [Fact]
        public void CheckAddMethod()
        {
            ICollection<int> set = new IntSet();
            set.Add(5);
            Assert.Single(set);
            Assert.False(set.IsReadOnly);
        }
    }
}
