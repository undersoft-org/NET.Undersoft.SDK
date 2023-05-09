namespace System.Series
{
    using System.Collections.Generic;
    using System.Uniques;

    public class Album64<V> : AlbumBase<V>
    {
        public Album64() : base(17, HashBits.bit64) { }

        public Album64(IEnumerable<IUnique<V>> collections, int _deckSize = 17)
            : base(collections, _deckSize) { }

        public Album64(IEnumerable<V> collections, int _deckSize = 17)
            : base(collections, _deckSize) { }

        public Album64(int _deckSize = 17) : base(_deckSize, HashBits.bit64) { }

        public override ICard<V>[] EmptyDeck(int size)
        {
            return new Card64<V>[size];
        }

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
