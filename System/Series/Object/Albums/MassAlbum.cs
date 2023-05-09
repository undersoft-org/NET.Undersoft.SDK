namespace System.Series
{
    using System.Collections.Generic;
    using System.Uniques;

    public class MassAlbum<V> : MassAlbumBase<V> where V : IUnique
    {
        public MassAlbum() : base(17, HashBits.bit64) { }

        public MassAlbum(
            IEnumerable<IUnique<V>> collection,
            int capacity = 17,
            HashBits bits = HashBits.bit64
        ) : this(capacity, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public MassAlbum(
            IEnumerable<V> collection,
            int capacity = 17,
            HashBits bits = HashBits.bit64
        ) : this(capacity, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public MassAlbum(
            IList<IUnique<V>> collection,
            int capacity = 17,
            HashBits bits = HashBits.bit64
        ) : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public MassAlbum(IList<V> collection, int capacity = 17, HashBits bits = HashBits.bit64)
            : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public MassAlbum(int capacity = 17, HashBits bits = HashBits.bit64) : base(capacity, bits)
        { }

        public override ICard<V> EmptyCard()
        {
            return new MassCard<V>();
        }

        public override ICard<V>[] EmptyCardTable(int size)
        {
            return new MassCard<V>[size];
        }

        public override ICard<V>[] EmptyDeck(int size)
        {
            return new MassCard<V>[size];
        }

        public override ICard<V> NewCard(ICard<V> card)
        {
            return new MassCard<V>(card);
        }

        public override ICard<V> NewCard(object key, V value)
        {
            return new MassCard<V>(key, value);
        }

        public override ICard<V> NewCard(ulong key, V value)
        {
            return new MassCard<V>(key, value);
        }

        public override ICard<V> NewCard(V value)
        {
            return new MassCard<V>(value);
        }
    }
}
