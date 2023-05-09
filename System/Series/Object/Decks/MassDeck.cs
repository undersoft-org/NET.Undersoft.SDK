namespace System.Series
{
    using System.Collections.Generic;

    public class MassDeck<V> : MassDeckBase<V> where V : IUnique
    {
        public MassDeck(IEnumerable<ICard<V>> collection, int capacity = 9)
            : base(collection, capacity) { }

        public MassDeck(IList<ICard<V>> collection, int capacity = 9) : base(collection, capacity)
        { }

        public MassDeck(IList<IUnique<V>> collection, int capacity = 9) : base(collection, capacity)
        { }

        public MassDeck(int capacity = 9) : base(capacity) { }

        public override ICard<V> EmptyCard()
        {
            return new MassCard<V>();
        }

        public override ICard<V>[] EmptyCardTable(int size)
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
