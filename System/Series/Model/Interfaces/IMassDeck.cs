namespace System.Series
{
    using System.Collections.Generic;

    public interface IMassDeck<V> : IDeck<V> where V : IUnique
    {
        V this[object key, ulong seed] { get; set; }
        V this[IUnique key, ulong seed] { get; set; }

        bool ContainsKey(object key, ulong seed);

        bool Contains(V item, ulong seed);

        V Get(object key, ulong seed);

        bool TryGet(object key, ulong seed, out ICard<V> output);
        bool TryGet(object key, ulong seed, out V output);

        ICard<V> GetCard(object key, ulong seed);

        ICard<V> New(object key, ulong seed);

        bool Add(object key, ulong seed, V value);
        bool Add(V value, ulong seed);
        void Add(IList<V> cards, ulong seed);
        void Add(IEnumerable<V> cards, ulong seed);

        bool Enqueue(object key, ulong seed, V value);
        bool Enqueue(V card, ulong seed);

        ICard<V> Put(object key, ulong seed, V value);
        ICard<V> Put(object key, ulong seed, object value);
        void Put(IList<V> cards, ulong seed);
        void Put(IEnumerable<V> cards, ulong seed);
        ICard<V> Put(V value, ulong seed);

        V Remove(object key, ulong seed);
        bool TryRemove(object key, ulong seed);

        ICard<V> NewCard(V value, ulong seed);
        ICard<V> NewCard(object key, ulong seed, V value);
        ICard<V> NewCard(ulong key, ulong seed, V value);
    }
}
