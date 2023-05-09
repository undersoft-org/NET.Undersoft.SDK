namespace System.Series
{
    using System.Collections.Generic;
    using System.Uniques;

    public class Album32<V> : AlbumBase<V>
    {
        public Album32() : base(17, HashBits.bit32) { }

        public Album32(
            IEnumerable<IUnique<V>> collections,
            int _deckSize = 17,
            bool repeatable = false
        ) : base(collections, _deckSize, repeatable, HashBits.bit32) { }

        public Album32(IEnumerable<V> collections, int _deckSize = 17, bool repeatable = false)
            : base(collections, _deckSize, repeatable, HashBits.bit32) { }

        public Album32(bool repeatable = false, int _deckSize = 17)
            : base(repeatable, _deckSize, HashBits.bit32) { }

        public override ICard<V>[] EmptyDeck(int size)
        {
            return new Card32<V>[size];
        }

        public override ICard<V> EmptyCard()
        {
            return new Card32<V>();
        }

        public override ICard<V>[] EmptyCardTable(int size)
        {
            return new Card32<V>[size];
        }

        public override ICard<V> NewCard(ICard<V> card)
        {
            return new Card32<V>(card);
        }

        public override ICard<V> NewCard(object key, V value)
        {
            return new Card32<V>(key, value);
        }

        public override ICard<V> NewCard(ulong key, V value)
        {
            return new Card32<V>(key, value);
        }

        public override ICard<V> NewCard(V value)
        {
            return new Card32<V>(value);
        }
    }
}
