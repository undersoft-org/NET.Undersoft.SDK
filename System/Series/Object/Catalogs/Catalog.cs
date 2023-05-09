using System.Collections.Generic;

namespace System.Series
{


    public class Catalog<V> : CatalogBase<V>
    {
        public Catalog(IEnumerable<V> collection, int capacity = 17, bool repeatable = false)
            : base(collection, capacity) { }

        public Catalog(bool repeatable = false, int capacity = 17) : base(repeatable, capacity) { }

        public override ICard<V> EmptyCard()
        {
            return new Card<V>();
        }

        public override ICard<V>[] EmptyCardTable(int size)
        {
            return new Card<V>[size];
        }

        public override ICard<V>[] EmptyDeck(int size)
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
