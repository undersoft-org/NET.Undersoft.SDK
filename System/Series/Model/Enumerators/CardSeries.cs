namespace System.Series
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CardSeries<V> : IEnumerator<ICard<V>>, IEnumerator<V>, IEnumerator
    {
        public ICard<V> Entry;
        private IDeck<V> map;

        public CardSeries(IDeck<V> Map)
        {
            map = Map;
            Entry = map.First;
        }

        public object Current => Entry.Value;

        public int Index => Entry.Index;

        public ulong Key => Entry.Key;

        public V Value => Entry.Value;

        ICard<V> IEnumerator<ICard<V>>.Current => Entry;

        V IEnumerator<V>.Current => Entry.Value;

        public void Dispose()
        {
            Entry = map.First;
        }

        public bool MoveNext()
        {
            Entry = map.Next(Entry);
            if (Entry != null)
            {
                return true;
            }
            return false;
        }

        public void Reset()
        {
            Entry = map.First;
        }
    }

    public class CardSeriesAsync<V> : IAsyncEnumerator<ICard<V>>, IAsyncEnumerator<V>, IEnumerator
    {
        public ICard<V> Entry;
        private IDeck<V> map;

        public CardSeriesAsync(IDeck<V> Map)
        {
            map = Map;
            Entry = map.First;
        }

        public object Current => Entry.Value;

        public int Index => Entry.Index;

        public ulong Key => Entry.Key;

        public V Value => Entry.Value;

        ICard<V> IAsyncEnumerator<ICard<V>>.Current => Entry;

        V IAsyncEnumerator<V>.Current => Entry.Value;

        public ValueTask DisposeAsync()
        {            
            Entry = map.First;
            return ValueTask.CompletedTask;
        }

        public bool MoveNext()
        {
            Entry = map.Next(Entry);
            if (Entry != null)
            {
                return true;
            }
            return false;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            
            Entry = map.Next(Entry);
            if (Entry != null)
            {
                return ValueTask.FromResult(true);
            }
            return ValueTask.FromResult(false);
        }

        public void Reset()
        {
            Entry = map.First;
        }
    }
}
