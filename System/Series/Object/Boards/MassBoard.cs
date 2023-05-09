namespace System.Series
{
    using System.Collections.Generic;
    using System.Uniques;

    public class MassBoard<V> : MassBoardBase<V> where V : IUnique
    {
        public MassBoard(
            IEnumerable<ICard<V>> collection,
            int capacity = 9,
            HashBits bits = HashBits.bit64
        ) : this(capacity, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public MassBoard(
            IList<ICard<V>> collection,
            int capacity = 9,
            HashBits bits = HashBits.bit64
        ) : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public MassBoard(int capacity = 9, HashBits bits = HashBits.bit64) : base(capacity, bits)
        { }

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
            return new MassCard<V>(value, value);
        }
    }
}
