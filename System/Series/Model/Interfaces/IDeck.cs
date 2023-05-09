namespace System.Series
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public interface IDeck<V>
        : IEnumerable<V>,
            IEnumerable,
            ICollection,
            ICollection<V>,
            IList<V>,
            IProducerConsumerCollection<V>,
            IDisposable,
            IUnique,
            IFindable<V>
    {
        ICard<V> First { get; }
        ICard<V> Last { get; }

        bool IsRepeatable { get; }

        ICard<V> Next(ICard<V> card);

        new int Count { get; }
        int MinCount { get; set; }

        bool ContainsKey(ulong key);
        bool ContainsKey(object key);
        bool ContainsKey(IUnique key);

        bool Contains(ICard<V> item);
        bool Contains(IUnique<V> item);
        bool Contains(ulong key, V item);

        V Get(object key);
        V Get(ulong key);
        V Get(IUnique key);
        V Get(IUnique<V> key);

        bool TryGet(object key, out ICard<V> output);
        bool TryGet(object key, out V output);
        bool TryGet(ulong key, out V output);
        bool TryGet(IUnique key, out ICard<V> output);
        bool TryGet(IUnique<V> key, out ICard<V> output);

        ICard<V> GetCard(object key);
        ICard<V> GetCard(ulong key);
        ICard<V> GetCard(IUnique key);
        ICard<V> GetCard(IUnique<V> key);

        ICard<V> Set(object key, V value);
        ICard<V> Set(ulong key, V value);
        ICard<V> Set(IUnique key, V value);
        ICard<V> Set(IUnique<V> key, V value);
        ICard<V> Set(V value);
        ICard<V> Set(IUnique<V> value);
        ICard<V> Set(ICard<V> value);
        int Set(IEnumerable<V> values);
        int Set(IList<V> values);
        int Set(IEnumerable<ICard<V>> values);
        int Set(IEnumerable<IUnique<V>> values);

        ICard<V> EnsureGet(object key, Func<ulong, V> ensureaction);
        ICard<V> EnsureGet(ulong key, Func<ulong, V> ensureaction);
        ICard<V> EnsureGet(IUnique key, Func<ulong, V> ensureaction);
        ICard<V> EnsureGet(IUnique<V> key, Func<ulong, V> ensureaction);

        ICard<V> New();
        ICard<V> New(ulong key);
        ICard<V> New(object key);

        bool Add(object key, V value);
        bool Add(ulong key, V value);
        void Add(ICard<V> card);
        void Add(IList<ICard<V>> cardList);
        void Add(IEnumerable<ICard<V>> cards);
        void Add(IList<V> cards);
        void Add(IEnumerable<V> cards);
        void Add(IUnique<V> cards);
        void Add(IList<IUnique<V>> cards);
        void Add(IEnumerable<IUnique<V>> cards);

        bool Enqueue(object key, V value);
        void Enqueue(ICard<V> card);
        bool Enqueue(V card);

        V Dequeue();
        bool TryDequeue(out ICard<V> item);
        bool TryDequeue(out V item);
        new bool TryTake(out V item);

        bool TryPick(int skip, out V output);

        ICard<V> Put(object key, V value);
        ICard<V> Put(ulong key, V value);
        ICard<V> Put(ICard<V> card);
        void Put(IList<ICard<V>> cardList);
        void Put(IEnumerable<ICard<V>> cards);
        void Put(IList<V> cards);
        void Put(IEnumerable<V> cards);
        ICard<V> Put(V value);
        ICard<V> Put(IUnique<V> cards);
        void Put(IList<IUnique<V>> cards);
        void Put(IEnumerable<IUnique<V>> cards);

        V Remove(object key);
        bool Remove(object key, V item);
        bool Remove(ICard<V> item);
        bool Remove(IUnique<V> item);
        bool TryRemove(object key);

        void Renew(IEnumerable<V> cards);
        void Renew(IList<V> cards);
        void Renew(IList<ICard<V>> cards);
        void Renew(IEnumerable<ICard<V>> cards);

        new V[] ToArray();

        IEnumerable<ICard<V>> AsCards();

        IEnumerable<V> AsValues();

        new void CopyTo(Array array, int arrayIndex);

        new bool IsSynchronized { get; set; }
        new object SyncRoot { get; set; }

        ICard<V> NewCard(V value);
        ICard<V> NewCard(object key, V value);
        ICard<V> NewCard(ulong key, V value);
        ICard<V> NewCard(ICard<V> card);

        void CopyTo(ICard<V>[] array, int destIndex);
        void CopyTo(IUnique<V>[] array, int arrayIndex);

        new void Clear();

        void Resize(int size);

        void Flush();
    }
}
