using System.Collections.Generic;

namespace System.Series
{
    public interface ICard<V>
        : IComparable<ulong>,
            IComparable<ICard<V>>,
            IEquatable<ICard<V>>,
            IEquatable<object>,
            IEquatable<ulong>,
            IComparable<object>,
            IUnique<V>,
            IDisposable,
            IEnumerable<V>
    {
        bool Repeated { get; set; }

        ICard<V> Extended { get; set; }

        int Index { get; set; }

        ulong Key { get; set; }

        ICard<V> Next { get; set; }

        bool Removed { get; set; }

        V Value { get; set; }

        int GetHashCode();

        Type GetUniqueType();

        void Set(ICard<V> card);

        void Set(object key, V value);

        void Set(ulong key, V value);

        void Set(V value);

        ICard<V> MoveNext(ICard<V> card);

        IEnumerable<ICard<V>> AsCards();

        IEnumerable<V> AsValues();
    }
}
