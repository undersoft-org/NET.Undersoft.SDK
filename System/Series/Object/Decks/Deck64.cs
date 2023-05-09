namespace System.Series
{
    using System.Collections.Generic;
    using System.Uniques;

    public class Deck64<V> : DeckBase<V>
    {
        public Deck64(IEnumerable<ICard<V>> collection, int capacity = 9) : this(capacity)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public Deck64(IEnumerable<IUnique<V>> collection, int capacity = 9) : this(capacity)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public Deck64(IList<ICard<V>> collection, int capacity = 9)
            : this(capacity > collection.Count ? capacity : collection.Count)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public Deck64(IList<IUnique<V>> collection, int capacity = 9)
            : this(capacity > collection.Count ? capacity : collection.Count)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public Deck64(int capacity = 9) : base(capacity, HashBits.bit64) { }

        public override ICard<V> EmptyCard()
        {
            return new Card64<V>();
        }

        public override ICard<V>[] EmptyCardTable(int size)
        {
            return new Card64<V>[size];
        }

        public override ICard<V> NewCard(ICard<V> card)
        {
            return new Card64<V>(card);
        }

        public override ICard<V> NewCard(object key, V value)
        {
            return new Card64<V>(key, value);
        }

        public override ICard<V> NewCard(ulong key, V value)
        {
            return new Card64<V>(key, value);
        }

        public override ICard<V> NewCard(V value)
        {
            return new Card64<V>(value);
        }
    }
}
