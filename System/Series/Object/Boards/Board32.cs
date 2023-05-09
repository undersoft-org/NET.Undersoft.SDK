namespace System.Series
{
    using System.Uniques;

    public class Board32<V> : BoardBase<V>
    {
        public Board32(int _deckSize = 9, HashBits bits = HashBits.bit64) : base(_deckSize, bits)
        { }

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
