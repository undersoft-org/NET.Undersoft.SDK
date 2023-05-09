namespace System.Series
{
    using System.Collections;
    using System.Collections.Generic;

    public class CardSubSeries<V> : IEnumerator<ICard<V>>, IEnumerator<V>, IEnumerator
    {
        public ICard<V> Entry;
        private ICard<V> map;

        public CardSubSeries(ICard<V> map)
        {
            this.map = map;
            Entry = map;
        }

        public object Current => Entry.Value;

        public int Index => Entry.Index;

        public ulong Key => Entry.Key;

        public V Value => Entry.Value;

        ICard<V> IEnumerator<ICard<V>>.Current => Entry;

        V IEnumerator<V>.Current => Entry.Value;

        public void Dispose()
        {
            Entry = map;
        }

        public bool MoveNext()
        {
            Entry = map.MoveNext(Entry);
            if (Entry != null)
            {
                return true;
            }
            return false;
        }

        public void Reset()
        {
            Entry = map;
        }
    }
}
