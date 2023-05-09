namespace System.Series
{
    using System.Collections;
    using System.Collections.Generic;

    public class SpectrumSeries<V> : IEnumerator<CardBase<V>>, IEnumerator
    {
        public CardBase<V> Entry;
        private int iterated = 0;
        private int lastReturned;
        private Spectrum<V> map;

        public SpectrumSeries(Spectrum<V> Map)
        {
            map = Map;
            Entry = new Card64<V>();
        }

        public object Current => Entry;

        public int Key
        {
            get { return (int)Entry.Key; }
        }

        public V Value
        {
            get { return Entry.Value; }
        }

        CardBase<V> IEnumerator<CardBase<V>>.Current => Entry;

        public void Dispose()
        {
            iterated = 0;
            Entry = null;
        }

        public bool HasNext()
        {
            return iterated < map.Count;
        }

        public bool MoveNext()
        {
            return Next();
        }

        public bool Next()
        {
            if (!HasNext())
            {
                return false;
            }

            if (iterated == 0)
            {
                lastReturned = map.IndexMin;
                iterated++;
                Entry.Key = (uint)lastReturned;
                Entry.Value = map.Get(lastReturned);
            }
            else
            {
                lastReturned = map.Next(lastReturned);
                iterated++;
                Entry.Key = (uint)lastReturned;
                Entry.Value = map.Get(lastReturned);
            }
            return true;
        }

        public void Reset()
        {
            Entry = new Card64<V>();
            iterated = 0;
        }
    }
}
