namespace System.Series
{
    using System.Collections.Generic;
    using System.Uniques;

    public class Album<V> : AlbumBase<V>
    {
        public Album() : base(17, HashBits.bit64) { }

        public Album(
            IEnumerable<IUnique<V>> collections,
            int capacity = 17,
            bool repeatable = false
        ) : base(collections, capacity, repeatable, HashBits.bit64) { }

        public Album(IEnumerable<V> collections, int capacity = 17, bool repeatable = false)
            : base(collections, capacity, repeatable, HashBits.bit64) { }

        public Album(bool repeatable = false, int capacity = 17)
            : base(repeatable, capacity, HashBits.bit64) { }

        public override ICard<V> EmptyCard()
        {
            return new Card64<V>();
        }

        public override ICard<V>[] EmptyCardTable(int size)
        {
            return new Card64<V>[size];
        }

        public override ICard<V>[] EmptyDeck(int size)
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
