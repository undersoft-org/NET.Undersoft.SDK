namespace System.Series
{
    using System.Collections.Generic;
    using System.Uniques;

    public class Deck<V> : DeckBase<V>
    {
        public Deck() : base(17, HashBits.bit64) { }

        public Deck(IEnumerable<V> collection, int capacity = 17, HashBits bits = HashBits.bit64)
            : this(capacity, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public Deck(IList<ICard<V>> collection, int capacity = 9) : base(collection, capacity) { }

        public Deck(IList<IUnique<V>> collection, int capacity = 17, HashBits bits = HashBits.bit64)
            : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public Deck(IList<IUnique<V>> collection, int capacity = 9) : base(collection, capacity) { }

        public Deck(IList<V> collection, int capacity = 17, HashBits bits = HashBits.bit64)
            : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public Deck(int capacity = 17, HashBits bits = HashBits.bit64) : base(capacity, bits) { }

        public Deck(int capacity = 9) : base(capacity) { }

        public override ICard<V> EmptyCard()
        {
            return new Card<V>();
        }

        public override ICard<V>[] EmptyCardTable(int size)
        {
            return new Card<V>[size];
        }

        public override ICard<V> NewCard(ICard<V> card)
        {
            return new Card<V>(card);
        }

        public override ICard<V> NewCard(object key, V value)
        {
            return new Card<V>(key, value);
        }

        public override ICard<V> NewCard(ulong key, V value)
        {
            return new Card<V>(key, value);
        }

        public override ICard<V> NewCard(V value)
        {
            return new Card<V>(value);
        }
    }
}
