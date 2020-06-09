using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;

namespace Kibnet.IntSet
{
    [Serializable]
    public class IntSet : IEnumerable<int>
    {
        public Card root = new Card(0);
        public long Count { get; set; } = 0;

        public bool Contain(int value)
        {
            var card = root;
            for (int i = 0; i < 5; i++)
            {
                if (card.Full)
                {
                    return true;
                }

                var index = Card.GetIndex(value, i);

                if (i < 4)
                {
                    if (card.Cards[index] == null)
                        return false;
                    card = card.Cards[index];
                }
                else
                {
                    var bindex = index >> 3;
                    var mask = (byte)(1 << (index & 7));
                    return (card.Bytes[bindex] & mask) != 0;
                }
            }
            return false;
        }

        public void Add(int value)
        {
            var card = root;
            for (int i = 0; i < 5; i++)
            {
                if (card.Full)
                {
                    return;
                }
                
                var index = Card.GetIndex(value, i);
                if (i < 4)
                {
                    if (card.Cards[index] == null)
                    {
                        var newcard = new Card(i + 1);
                        card.Cards[index] = newcard;
                        card = newcard;
                    }
                    else
                    {
                        card = card.Cards[index];
                    }
                }
                else
                {
                    var bindex = index >> 3;
                    var mask = (byte)(1 << (index & 7));
                    if ((card.Bytes[bindex] & mask) == 0)
                    {
                        card.Bytes[bindex] |= mask;
                        Count++;
                        if (card.Bytes[bindex] == byte.MaxValue)
                        {
                            var full = card.CheckFull();
                            if (full)
                            {
                                var parentCard0 = root.Cards[Card.GetIndex(value, 0)];
                                var parentCard1 = parentCard0.Cards[Card.GetIndex(value,1)];
                                var parentCard2 = parentCard1.Cards[Card.GetIndex(value,2)];
                                var parentCard3 = parentCard2.Cards[Card.GetIndex(value,3)];
                                //var parentCard4 = parentCard3.Cards[indexes[4]];
                                //if (parentCard4.CheckFull())
                                    if (parentCard3.CheckFull())
                                        if (parentCard2.CheckFull())
                                            if (parentCard1.CheckFull())
                                                if (parentCard0.CheckFull())
                                                    return;
                            }
                        }
                    }
                }
            }
        }

        public IEnumerator<int> GetEnumerator()
        {
            foreach (var i0 in (IEnumerable<int>)root)
            {
                var cards1 = root.Full ? root : root.Cards[i0];
                if (cards1 != null)
                {
                    foreach (var i1 in (IEnumerable<int>)cards1)
                    {
                        var cards2 = cards1.Full ? cards1 : cards1.Cards[i1];
                        if (cards2 != null)
                        {
                            foreach (var i2 in (IEnumerable<int>)cards2)
                            {
                                var cards3 = cards2.Full ? cards2 : cards2.Cards[i2];
                                if (cards3 != null)
                                {
                                    foreach (var i3 in (IEnumerable<int>)cards3)
                                    {
                                        var bytes = cards3.Full ? cards3 : cards3.Cards[i3];
                                        if (bytes != null)
                                        {
                                            var bytecount = 0;
                                            foreach (var i4 in (IEnumerable<byte>)bytes)
                                            {
                                                for (int j = 0; j < 8; j++)
                                                {
                                                    var tail = i4 & (1 << j);
                                                    if (tail != 0)
                                                    {
                                                        var value = i0 << 26;
                                                        value |= i1 << 20;
                                                        value |= i2 << 14;
                                                        value |= i3 << 8;
                                                        value |= bytecount << 3;
                                                        value |= j;
                                                        yield return value;
                                                    }
                                                }

                                                bytecount++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class Card : IEnumerable<int>, IEnumerable<byte>
        {
            public Card() { }

            public Card(int level)
            {
                if (level < 4)
                {
                    Cards = new Card[64];
                }
                else
                {
                    Bytes = new byte[32];
                }
            }

            public Card[] Cards;
            public byte[] Bytes;
            public bool Full;

            [SecuritySafeCritical]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static unsafe int GetIndex(int value, int level)
            {
                var temp = (uint)(uint*)(value);
                var index = level switch
                {
                    0 => (temp) >> 26,
                    1 => (temp << 6) >> 26,
                    2 => (temp << 12) >> 26,
                    3 => (temp << 18) >> 26,
                    4 => (temp << 24) >> 24,
                };
                //TODO переписать на получение указателей
                return (int)(int*)index;
            }
            
            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            {
                if (Cards != null)
                {
                    for (int i = 0; i < 64; i++)
                    {
                        if (Cards[i] != null)
                        {
                            yield return i;
                        }
                    }
                }
                else if (Full)
                {
                    for (int i = 0; i < 64; i++)
                    {
                        yield return i;
                    }
                }
            }

            IEnumerator<byte> IEnumerable<byte>.GetEnumerator()
            {
                if (Bytes != null)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        yield return Bytes[i];
                    }
                }
                else if (Full)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        yield return byte.MaxValue;
                    }
                }
            }

            public override string ToString()
            {
                if (Bytes != null)
                {
                    return String.Create(256, this, GetMask);
                }
                else
                {
                    return String.Create(64, this, GetMask);
                }
            }

            public IEnumerator GetEnumerator()
            {
                if (Bytes != null)
                {
                    return ((IEnumerable<byte>)this).GetEnumerator();
                }
                else
                {
                    return ((IEnumerable<int>)this).GetEnumerator();
                }
            }

            public bool CheckFull()
            {
                if (Bytes != null)
                {
                    Full = Bytes.All(b => b == byte.MaxValue);
                    if (Full == true) Bytes = null;
                }
                if (Cards != null)
                {
                    Full = Cards.All(c => c!=null && c.Full);
                    if (Full == true) Cards = null;
                }
                return Full;
            }

            public static void GetMask(Span<char> span, Card arg)
            {
                if (arg.Full == true)
                {
                    span.Fill('1');
                }
                else
                {
                    if (arg.Cards != null)
                    {
                        var count = arg.Cards.Length;
                        for (int i = 0; i < count; i++)
                        {
                            span[i] = arg.Cards[i] != null ? '1' : '0';
                        }
                        return;
                    }
                    if (arg.Bytes != null)
                    {
                        var count = arg.Bytes.Length;
                        var bytesize = 8;
                        var p = 0;
                        for (int i = 0; i < count; i++)
                        {
                            var b = arg.Bytes[i];
                            for (int j = 0; j < bytesize; j++)
                            {
                                span[p++] = ((b & 1) == 1) ? '1' : '0';
                                b >>= 1;
                            }
                        }
                    }
                }
            }
        }
    }
}