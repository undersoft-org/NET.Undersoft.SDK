namespace System.Series
{
    using System.Collections.Generic;
    using System.Uniques;

    public class Board<V> : BoardBase<V>
    {
        public Board(
            IEnumerable<ICard<V>> collection,
            int capacity = 9,
            HashBits bits = HashBits.bit64
        ) : this(capacity, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public Board(IList<ICard<V>> collection, int capacity = 9, HashBits bits = HashBits.bit64)
            : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public Board(int capacity = 9, HashBits bits = HashBits.bit64) : base(capacity, bits) { }

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
            return new Card64<V>(value, value);
        }
    }
}
