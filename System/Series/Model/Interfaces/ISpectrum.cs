namespace System.Series
{
    using System.Collections.Generic;

    public interface ISpectrum<V> : IEnumerable<CardBase<V>>
    {
        int Count { get; }

        int Size { get; }

        bool Add(int key, V value);

        bool Contains(int key);

        int Next(int key);

        int Previous(int key);

        bool Remove(int key);

        bool Set(int key, V value);
    }
}
