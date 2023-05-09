namespace System.Series
{
    using System.Collections;
    using System.Collections.Generic;

    public class CardKeySeries<V> : IEnumerator<IUnique<V>>, IEnumerator
    {
        public ICard<V> Entry;
        private IDeck<V> map;

        public CardKeySeries(IDeck<V> Map)
        {
            map = Map;
            Entry = map.First;
        }

        public object Current => Entry.Key;

        public ulong Key
        {
            get { return Entry.Key; }
        }

        public V Value
        {
            get { return Entry.Value; }
        }

        IUnique<V> IEnumerator<IUnique<V>>.Current => Entry;

        public void Dispose()
        {
            Entry = map.First;
        }

        public bool MoveNext()
        {
            Entry = Entry.Next;
            if (Entry != null)
            {
                if (Entry.Removed)
                    return MoveNext();
                return true;
            }
            return false;
        }

        public void Reset()
        {
            Entry = map.First;
        }
    }
}
