namespace System.Series
{
    using System.Collections.Generic;
    using System.Uniques;

    public class Catalog32<V> : CatalogBase<V>
    {
        public Catalog32(
            IEnumerable<IUnique<V>> collection,
            int capacity = 17,
            bool repeatable = false
        ) : this(repeatable, capacity)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public Catalog32(IEnumerable<V> collection, int capacity = 17, bool repeatable = false)
            : this(repeatable, capacity)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public Catalog32(bool repeatable = false, int capacity = 17)
            : base(repeatable, capacity, HashBits.bit32) { }

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
