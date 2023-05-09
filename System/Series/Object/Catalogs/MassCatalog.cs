namespace System.Series
{
    using System.Collections.Generic;

    public class MassCatalog<V> : MassCatalogBase<V> where V : IUnique
    {
        public MassCatalog(IEnumerable<V> collection, int capacity = 17)
            : base(collection, capacity) { }

        public MassCatalog(IList<IUnique<V>> collection, int capacity = 17)
            : base(collection, capacity) { }

        public MassCatalog(IList<V> collection, int capacity = 17) : base(collection, capacity) { }

        public MassCatalog(int capacity = 17) : base(capacity) { }

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
