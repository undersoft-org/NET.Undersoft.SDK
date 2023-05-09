using System.Linq;
using System.Uniques;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace System.Series.Basedeck
{
    public abstract class TetraSet<V>
        : Uniqueness,
            ICollection<V>,
            IList<V>,
            IDeck<V>,
            ICollection<ICard<V>>,
            IList<ICard<V>>,
            IProducerConsumerCollection<V>,
            IDisposable,
            ICollection<IUnique<V>>
    {
        static protected readonly float CONFLICTS_PERCENT_LIMIT = 0.25f;
        static protected readonly float REMOVED_PERCENT_LIMIT = 0.15f;

        protected Usid serialcode;
        protected ICard<V> first,
            last;
        protected TetraTable<V> table;
        protected TetraSize tsize;
        protected TetraCount tcount;
        protected int count,
            conflicts,
            removed,
            size,
            minSize;

        protected void countIncrement(uint tid)
        {
            count++;
            if ((tcount.Increment(tid) + 3) > size)
                Rehash(tsize.NextSize(tid), tid);
        }

        protected void conflictIncrement(uint tid)
        {
            countIncrement(tid);
            if (++conflicts > (size * CONFLICTS_PERCENT_LIMIT))
                Rehash(tsize.NextSize(tid), tid);
        }

        protected void removedIncrement(uint tid)
        {
            int _tsize = tsize[tid];
            --count;
            tcount.Decrement(tid);
            if (++removed > (_tsize * REMOVED_PERCENT_LIMIT))
            {
                if (_tsize < _tsize / 2)
                    Rehash(tsize.PreviousSize(tid), tid);
                else
                    Rehash(_tsize, tid);
            }
        }

        protected void removedDecrement()
        {
            ++count;
            --removed;
        }

        public TetraSet(int capacity = 16, HashBits bits = HashBits.bit64) : base(bits)
        {
            size = capacity;
            minSize = capacity;
            tsize = new TetraSize(capacity);
            tcount = new TetraCount();
            table = new TetraTable<V>(this, capacity);
            first = EmptyCard();
            last = first;
            ValueEquals = getValueComparer();
        }

        public TetraSet(
            IList<ICard<V>> collection,
            int capacity = 16,
            HashBits bits = HashBits.bit64
        ) : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            this.Add(collection);
        }

        public TetraSet(
            IEnumerable<ICard<V>> collection,
            int capacity = 16,
            HashBits bits = HashBits.bit64
        ) : this(capacity, bits)
        {
            this.Add(collection);
        }

        public virtual ICard<V> First => first;
        public virtual ICard<V> Last => last;

        public virtual int Size => size;
        public virtual int Count => count;
        public virtual int MinCount { get; set; }
        public virtual bool IsReadOnly { get; set; }
        public virtual bool IsSynchronized { get; set; }
        public virtual bool IsRepeatable
        {
            get => false;
        }
        public virtual object SyncRoot { get; set; }
        public virtual Func<V, V, bool> ValueEquals { get; }

        ICard<V> IList<ICard<V>>.this[int index]
        {
            get => GetCard(index);
            set => GetCard(index).Set(value);
        }
        public virtual V this[int index]
        {
            get => GetCard(index).Value;
            set => GetCard(index).Value = value;
        }
        protected V this[ulong hashkey]
        {
            get { return InnerGet(hashkey); }
            set { InnerPut(hashkey, value); }
        }
        public virtual V this[object key]
        {
            get { return InnerGet(unique.Key(key)); }
            set { InnerPut(unique.Key(key), value); }
        }
        object IFindable.this[object key]
        {
            get => InnerGet(unique.Key(key));
            set => InnerPut(unique.Key(key), (V)value);
        }

        public virtual V InnerGet(ulong key)
        {
            uint tid = getTetraId(key);
            int _size = tsize[tid];
            uint pos = (uint)getPosition(key);

            ICard<V> mem = table[tid, pos];

            while (mem != null)
            {
                if (mem.Equals(key))
                {
                    if (!mem.Removed)
                        return mem.Value;
                    return default(V);
                }
                mem = mem.Extended;
            }

            return default(V);
        }

        public V Get(ulong key)
        {
            return InnerGet(key);
        }

        public virtual V Get(object key)
        {
            return InnerGet(unique.Key(key));
        }

        public virtual V Get(IUnique<V> key)
        {
            return InnerGet(unique.Key(key));
        }

        public virtual V Get(IUnique key)
        {
            return InnerGet(unique.Key(key));
        }

        public virtual bool InnerTryGet(ulong key, out ICard<V> output)
        {
            output = null;
            uint tid = getTetraId(key);
            int _size = tsize[tid];
            uint pos = (uint)getPosition(key);

            ICard<V> mem = table[tid, pos];
            while (mem != null)
            {
                if (mem.Equals(key))
                {
                    if (!mem.Removed)
                    {
                        output = mem;
                        return true;
                    }
                    return false;
                }
                mem = mem.Extended;
            }
            return false;
        }

        public virtual bool TryGet(object key, out ICard<V> output)
        {
            return InnerTryGet(unique.Key(key), out output);
        }

        public virtual bool TryGet(object key, out V output)
        {
            output = default(V);
            ICard<V> card = null;
            if (InnerTryGet(unique.Key(key), out card))
            {
                output = card.Value;
                return true;
            }
            return false;
        }

        public bool TryGet(ulong key, out V output)
        {
            output = default(V);
            ICard<V> card = null;
            if (InnerTryGet(key, out card))
            {
                output = card.Value;
                return true;
            }
            return false;
        }

        public bool TryGet(IUnique key, out ICard<V> output)
        {
            return deckImplementation.TryGet(key, out output);
        }

        public bool TryGet(IUnique<V> key, out ICard<V> output)
        {
            return deckImplementation.TryGet(key, out output);
        }

        public virtual ICard<V> InnerGetCard(ulong key)
        {
            uint tid = getTetraId(key);
            int _size = tsize[tid];
            uint pos = (uint)getPosition(key);

            ICard<V> mem = table[tid, pos];
            while (mem != null)
            {
                if (mem.Equals(key))
                {
                    if (!mem.Removed)
                        return mem;
                    return null;
                }
                mem = mem.Extended;
            }

            return null;
        }

        public abstract ICard<V> GetCard(int index);

        public virtual ICard<V> GetCard(object key)
        {
            return InnerGetCard(unique.Key(key));
        }

        public ICard<V> GetCard(ulong key)
        {
            return InnerGetCard(key);
        }

        public ICard<V> GetCard(IUnique key)
        {
            return deckImplementation.GetCard(key);
        }

        public ICard<V> GetCard(IUnique<V> key)
        {
            return deckImplementation.GetCard(key);
        }

        public ICard<V> Set(object key, V value)
        {
            return deckImplementation.Set(key, value);
        }

        public ICard<V> Set(ulong key, V value)
        {
            return deckImplementation.Set(key, value);
        }

        public ICard<V> Set(IUnique key, V value)
        {
            return deckImplementation.Set(key, value);
        }

        public ICard<V> Set(IUnique<V> key, V value)
        {
            return deckImplementation.Set(key, value);
        }

        public ICard<V> Set(V value)
        {
            return deckImplementation.Set(value);
        }

        public ICard<V> Set(IUnique<V> value)
        {
            return deckImplementation.Set(value);
        }

        public ICard<V> Set(ICard<V> value)
        {
            return deckImplementation.Set(value);
        }

        public int Set(IEnumerable<V> values)
        {
            return deckImplementation.Set(values);
        }

        public int Set(IList<V> values)
        {
            return deckImplementation.Set(values);
        }

        public int Set(IEnumerable<ICard<V>> values)
        {
            return deckImplementation.Set(values);
        }

        public int Set(IEnumerable<IUnique<V>> values)
        {
            return deckImplementation.Set(values);
        }

        public ICard<V> EnsureGet(object key, Func<ulong, V> sureaction)
        {
            return deckImplementation.EnsureGet(key, sureaction);
        }

        public ICard<V> EnsureGet(ulong key, Func<ulong, V> sureaction)
        {
            return deckImplementation.EnsureGet((object)key, sureaction);
        }

        public ICard<V> EnsureGet(IUnique key, Func<ulong, V> sureaction)
        {
            return deckImplementation.EnsureGet((object)key, sureaction);
        }

        public ICard<V> EnsureGet(IUnique<V> key, Func<ulong, V> sureaction)
        {
            return deckImplementation.EnsureGet((object)key, sureaction);
        }

        protected abstract ICard<V> InnerPut(ulong key, V value);
        protected abstract ICard<V> InnerPut(V value);
        protected abstract ICard<V> InnerPut(ICard<V> value);

        public virtual ICard<V> Put(ulong key, object value)
        {
            return InnerPut(key, (V)value);
        }

        public virtual ICard<V> Put(ulong key, V value)
        {
            return InnerPut(key, value);
        }

        protected virtual Func<V, V, bool> getValueComparer()
        {
            if (typeof(V).IsValueType)
                return (o1, o2) => o1.Equals(o2);
            return (o1, o2) => ReferenceEquals(o1, o2);
        }

        public virtual ICard<V> Put(object key, V value)
        {
            return InnerPut(unique.Key(key), value);
        }

        public virtual ICard<V> Put(object key, object value)
        {
            if (value is V)
                return InnerPut(unique.Key(key), (V)value);
            return null;
        }

        public virtual ICard<V> Put(ICard<V> _card)
        {
            return InnerPut(_card);
        }

        public virtual void Put(IList<ICard<V>> cards)
        {
            int c = cards.Count;
            for (int i = 0; i < c; i++)
            {
                InnerPut(cards[i]);
            }
        }

        public virtual void Put(IEnumerable<ICard<V>> cards)
        {
            foreach (ICard<V> card in cards)
                InnerPut(card);
        }

        public virtual ICard<V> Put(V value)
        {
            return InnerPut(value);
        }

        public virtual void Put(object value)
        {
            if (value is IUnique<V>)
                Put((IUnique<V>)value);
            if (value is V)
                Put((V)value);
            else if (value is ICard<V>)
                Put((ICard<V>)value);
        }

        public virtual void Put(IList<V> cards)
        {
            int c = cards.Count;
            for (int i = 0; i < c; i++)
            {
                Put(cards[i]);
            }
        }

        public virtual void Put(IEnumerable<V> cards)
        {
            foreach (V card in cards)
                Put(card);
        }

        public virtual ICard<V> Put(IUnique<V> value)
        {
            if (value is ICard<V>)
                return InnerPut((ICard<V>)value);
            return InnerPut(value.CompactKey(), value.UniqueObject);
        }

        public virtual void Put(IList<IUnique<V>> value)
        {
            foreach (IUnique<V> item in value)
            {
                Put(item);
            }
        }

        public virtual void Put(IEnumerable<IUnique<V>> value)
        {
            foreach (IUnique<V> item in value)
            {
                Put(item);
            }
        }

        protected abstract bool InnerAdd(ulong key, V value);
        protected abstract bool InnerAdd(V value);
        protected abstract bool InnerAdd(ICard<V> value);

        public virtual bool Add(ulong key, object value)
        {
            return InnerAdd(key, (V)value);
        }

        public virtual bool Add(ulong key, V value)
        {
            return InnerAdd(key, value);
        }

        public virtual bool Add(object key, V value)
        {
            return InnerAdd(unique.Key(key), value);
        }

        public virtual void Add(ICard<V> card)
        {
            InnerAdd(card);
        }

        public virtual void Add(IList<ICard<V>> cardList)
        {
            int c = cardList.Count;
            for (int i = 0; i < c; i++)
            {
                InnerAdd(cardList[i]);
            }
        }

        public virtual void Add(IEnumerable<ICard<V>> cardTable)
        {
            foreach (ICard<V> card in cardTable)
                Add(card);
        }

        public virtual void Add(V value)
        {
            InnerAdd(value);
        }

        public virtual void Add(IList<V> cards)
        {
            int c = cards.Count;
            for (int i = 0; i < c; i++)
            {
                Add(cards[i]);
            }
        }

        public virtual void Add(IEnumerable<V> cards)
        {
            foreach (V card in cards)
                Add(card);
        }

        public virtual void Add(IUnique<V> value)
        {
            if (value is ICard<V>)
                InnerAdd((ICard<V>)value);
            InnerAdd(unique.Key(value), value.UniqueObject);
        }

        public virtual void Add(IList<IUnique<V>> value)
        {
            foreach (IUnique<V> item in value)
            {
                Add(item);
            }
        }

        public virtual void Add(IEnumerable<IUnique<V>> value)
        {
            foreach (IUnique<V> item in value)
            {
                Add(item);
            }
        }

        public virtual bool TryAdd(V value)
        {
            return InnerAdd(value);
        }

        public virtual ICard<V> New()
        {
            ICard<V> newCard = NewCard(Unique.New, default(V));
            if (InnerAdd(newCard))
                return newCard;
            return null;
        }

        public virtual ICard<V> New(ulong key)
        {
            ICard<V> newCard = NewCard(key, default(V));
            if (InnerAdd(newCard))
                return newCard;
            return null;
        }

        public virtual ICard<V> New(object key)
        {
            ICard<V> newCard = NewCard(unique.Key(key), default(V));
            if (InnerAdd(newCard))
                return newCard;
            return null;
        }

        protected abstract void InnerInsert(int index, ICard<V> item);

        public virtual void Insert(int index, ICard<V> item)
        {
            ulong key = item.Key;
            uint tid = getTetraId(key);
            int _size = tsize[tid];
            ulong pos = getPosition(key);
            var _table = table[tid];
            ICard<V> card = _table[pos];
            if (card == null)
            {
                card = NewCard(item);
                _table[pos] = card;
                InnerInsert(index, card);
                countIncrement(tid);
                return;
            }

            for (; ; )
            {
                if (card.Equals(key))
                {
                    if (card.Removed)
                    {
                        var newcard = NewCard(item);
                        card.Extended = newcard;
                        InnerInsert(index, newcard);
                        conflictIncrement(tid);
                        return;
                    }
                    throw new Exception("Item exist");
                }

                if (card.Extended == null)
                {
                    var newcard = NewCard(item);
                    card.Extended = newcard;
                    InnerInsert(index, newcard);
                    conflictIncrement(tid);
                    return;
                }
                card = card.Extended;
            }
        }

        public virtual void Insert(int index, V item)
        {
            Insert(index, NewCard(unique.Key(item), item));
        }

        public virtual bool Enqueue(V value)
        {
            return InnerAdd(unique.Key(value), value);
        }

        public virtual bool Enqueue(object key, V value)
        {
            return InnerAdd(unique.Key(key), value);
        }

        public virtual void Enqueue(ICard<V> card)
        {
            InnerAdd(card.Key, card.Value);
        }

        public virtual bool TryTake(out V output)
        {
            return TryDequeue(out output);
        }

        public bool TryPick(int skip, out V output)
        {
            return deckImplementation.TryPick(skip, out output);
        }

        public virtual V Dequeue()
        {
            V card = default(V);
            TryDequeue(out card);
            return card;
        }

        public virtual bool TryDequeue(out V output)
        {
            var _output = Next(first);
            if (_output != null)
            {
                _output.Removed = true;
                removedIncrement(getTetraId(_output.Key));
                output = _output.Value;
                return true;
            }
            output = default(V);
            return false;
        }

        public virtual bool TryDequeue(out ICard<V> output)
        {
            output = Next(first);
            if (output != null)
            {
                output.Removed = true;
                removedIncrement(getTetraId(output.Key));
                return true;
            }
            return false;
        }

        private void renewClear(int capacity)
        {
            if (capacity != size || count > 0)
            {
                size = capacity;
                conflicts = 0;
                removed = 0;
                count = 0;
                tcount.ResetAll();
                tsize.ResetAll();
                table = new TetraTable<V>(this, size);
                first = EmptyCard();
                last = first;
            }
        }

        public virtual void Renew(IEnumerable<V> cards)
        {
            renewClear(minSize);
            Put(cards);
        }

        public virtual void Renew(IList<V> cards)
        {
            int capacity = cards.Count;
            capacity += (int)(capacity * CONFLICTS_PERCENT_LIMIT);
            renewClear(capacity);
            Put(cards);
        }

        public virtual void Renew(IList<ICard<V>> cards)
        {
            int capacity = cards.Count;
            capacity += (int)(capacity * CONFLICTS_PERCENT_LIMIT);
            renewClear(capacity);
            Put(cards);
        }

        public virtual void Renew(IEnumerable<ICard<V>> cards)
        {
            renewClear(minSize);
            Put(cards);
        }

        protected bool InnerContainsKey(ulong key)
        {
            uint tid = getTetraId(key);
            int _size = tsize[tid];
            uint pos = (uint)getPosition(key);

            ICard<V> mem = table[tid, pos];

            while (mem != null)
            {
                if (!mem.Removed && mem.Equals(key))
                {
                    return true;
                }
                mem = mem.Extended;
            }

            return false;
        }

        public virtual bool ContainsKey(object key)
        {
            return InnerContainsKey(unique.Key(key));
        }

        public virtual bool ContainsKey(ulong key)
        {
            return InnerContainsKey(key);
        }

        public virtual bool ContainsKey(IUnique key)
        {
            return InnerContainsKey(unique.Key(key));
        }

        public virtual bool Contains(ICard<V> item)
        {
            return InnerContainsKey(item.Key);
        }

        public virtual bool Contains(IUnique<V> item)
        {
            return InnerContainsKey(unique.Key(item));
        }

        public virtual bool Contains(V item)
        {
            return InnerContainsKey(unique.Key(item));
        }

        public virtual bool Contains(ulong key, V item)
        {
            return InnerContainsKey(key);
        }

        public virtual V InnerRemove(ulong key)
        {
            uint tid = getTetraId(key);
            int _size = tsize[tid];
            uint pos = (uint)getPosition(key);

            ICard<V> mem = table[tid, pos];

            while (mem != null)
            {
                if (mem.Equals(key))
                {
                    if (!mem.Removed)
                    {
                        mem.Removed = true;
                        removedIncrement(getTetraId(mem.Key));
                        return mem.Value;
                    }
                    return default(V);
                }

                mem = mem.Extended;
            }
            return default(V);
        }

        protected virtual V InnerRemove(ulong key, V item)
        {
            uint tid = getTetraId(key);
            int _size = tsize[tid];
            uint pos = (uint)getPosition(key);

            ICard<V> mem = table[tid, pos];

            while (mem != null)
            {
                if (mem.Equals(key))
                {
                    if (mem.Removed)
                        return default(V);

                    if (ValueEquals(mem.Value, item))
                    {
                        mem.Removed = true;
                        removedIncrement(getTetraId(mem.Key));
                        return mem.Value;
                    }
                    return default(V);
                }
                mem = mem.Extended;
            }
            return default(V);
        }

        public virtual bool Remove(V item)
        {
            return InnerRemove(unique.Key(item)).Equals(default(V)) ? false : true;
        }

        public virtual V Remove(object key)
        {
            return InnerRemove(unique.Key(key));
        }

        public virtual bool Remove(ICard<V> item)
        {
            return InnerRemove(item.Key).Equals(default(V)) ? false : true;
        }

        public virtual bool Remove(IUnique<V> item)
        {
            return TryRemove(unique.Key(item));
        }

        public virtual bool TryRemove(object key)
        {
            return InnerRemove(unique.Key(key)).Equals(default(V)) ? false : true;
        }

        public virtual void RemoveAt(int index)
        {
            InnerRemove(GetCard(index).Key);
        }

        public virtual bool Remove(object key, V item)
        {
            return InnerRemove(unique.Key(key), item).Equals(default(V)) ? false : true;
        }

        public virtual void Clear()
        {
            size = minSize;
            conflicts = 0;
            removed = 0;
            count = 0;
            tcount.ResetAll();
            tsize.ResetAll();
            table = new TetraTable<V>(this, size);
            first = EmptyCard();
            last = first;
        }

        public void Resize(int size)
        {
            deckImplementation.Resize(size);
        }

        public virtual void Flush()
        {
            conflicts = 0;
            removed = 0;
            count = 0;
            tcount.ResetAll();
            tsize.ResetAll();
            table = new TetraTable<V>(this, size);
            first = EmptyCard();
            last = first;
        }

        public virtual void CopyTo(ICard<V>[] array, int index)
        {
            int c = count,
                i = index,
                l = array.Length;
            if (l - i < c)
            {
                c = l - i;
                foreach (ICard<V> ves in this.AsCards().Take(c))
                    array[i++] = ves;
            }
            else
                foreach (ICard<V> ves in this)
                    array[i++] = ves;
        }

        public virtual void CopyTo(Array array, int index)
        {
            int c = count,
                i = index,
                l = array.Length;
            if (l - i < c)
            {
                c = l - i;
                foreach (V ves in this.AsValues().Take(c))
                    array.SetValue(ves, i++);
            }
            else
                foreach (V ves in this.AsValues())
                    array.SetValue(ves, i++);
        }

        public virtual void CopyTo(V[] array, int index)
        {
            int c = count,
                i = index,
                l = array.Length;
            if (l - i < c)
            {
                c = l - i;
                foreach (V ves in this.AsValues().Take(c))
                    array[i++] = ves;
            }
            else
                foreach (V ves in this.AsValues())
                    array[i++] = ves;
        }

        public virtual void CopyTo(IUnique<V>[] array, int arrayIndex)
        {
            int c = count,
                i = arrayIndex,
                l = array.Length;
            if (l - i < c)
            {
                c = l - i;
                foreach (ICard<V> ves in this.AsCards().Take(c))
                    array[i++] = ves;
            }
            else
                foreach (ICard<V> ves in this)
                    array[i++] = ves;
        }

        public virtual V[] ToArray()
        {
            return this.AsValues().ToArray();
        }

        public virtual ICard<V> Next(ICard<V> card)
        {
            ICard<V> _card = card.Next;
            if (_card != null)
            {
                if (!_card.Removed)
                    return _card;
                return Next(_card);
            }
            return null;
        }

        public virtual void Resize(int size, uint tid)
        {
            Rehash(size, tid);
        }

        public abstract ICard<V> EmptyCard();

        public abstract ICard<V> NewCard(ulong key, V value);
        public abstract ICard<V> NewCard(object key, V value);
        public abstract ICard<V> NewCard(ICard<V> card);
        public abstract ICard<V> NewCard(V card);

        public abstract ICard<V>[] EmptyCardTable(int size);

        public virtual int IndexOf(ICard<V> item)
        {
            return GetCard(item).Index;
        }

        public virtual int IndexOf(V item)
        {
            return GetCard(item).Index;
        }

        public virtual IEnumerable<V> AsValues()
        {
            return (IEnumerable<V>)this;
        }

        public virtual IEnumerable<ICard<V>> AsCards()
        {
            return (IEnumerable<ICard<V>>)this;
        }

        public virtual IEnumerable<IUnique<V>> AsIdentifiers()
        {
            return (IEnumerable<IUnique<V>>)this;
        }

        public virtual IEnumerator<ICard<V>> GetEnumerator()
        {
            return new CardSeries<V>(this);
        }

        IEnumerator<V> IEnumerable<V>.GetEnumerator()
        {
            return new CardSeries<V>(this);
        }

        IEnumerator<IUnique<V>> IEnumerable<IUnique<V>>.GetEnumerator()
        {
            return new CardKeySeries<V>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CardSeries<V>(this);
        }

        protected static uint getTetraId(ulong key)
        {
            return (uint)(((long)key & 1L) - (((long)key & -1L) * 2));
        }

        protected ulong getPosition(ulong key)
        {
            return (key % (uint)(size - 1));
        }

        protected static ulong getPosition(ulong key, int newsize)
        {
            return (key % (uint)(newsize - 1));
        }

        protected virtual void Rehash(int newsize, uint tid)
        {
            int finish = tcount[tid];
            int _tsize = tsize[tid];
            ICard<V>[] newCardTable = EmptyCardTable(newsize);
            ICard<V> card = first;
            card = card.Next;
            if (removed > 0)
            {
                rehashAndReindex(card, newCardTable, newsize, tid);
            }
            else
            {
                rehashOnly(card, newCardTable, newsize, tid);
            }

            table[tid] = newCardTable;
            size = newsize - _tsize;
        }

        private void rehashAndReindex(ICard<V> card, ICard<V>[] newCardTable, int newsize, uint tid)
        {
            int _conflicts = 0;
            int _oldconflicts = 0;
            int _removed = 0;
            ICard<V>[] _newCardTable = newCardTable;
            ICard<V> _firstcard = EmptyCard();
            ICard<V> _lastcard = _firstcard;
            do
            {
                if (!card.Removed)
                {
                    ulong pos = getPosition(card.Key, newsize);

                    ICard<V> mem = _newCardTable[pos];

                    if (mem == null)
                    {
                        if (card.Extended != null)
                            _oldconflicts++;

                        card.Extended = null;
                        _newCardTable[pos] = _lastcard = _lastcard.Next = card;
                    }
                    else
                    {
                        for (; ; )
                        {
                            if (mem.Extended == null)
                            {
                                card.Extended = null;
                                ;
                                _lastcard = _lastcard.Next = mem.Extended = card;
                                _conflicts++;
                                break;
                            }
                            else
                                mem = mem.Extended;
                        }
                    }
                }
                else
                    _removed++;

                card = card.Next;
            } while (card != null);
            conflicts -= _oldconflicts;
            removed -= _removed;
            first = _firstcard;
            last = _lastcard;
        }

        private void rehashOnly(ICard<V> card, ICard<V>[] newCardTable, int newsize, uint tid)
        {
            int _conflicts = 0;
            int _oldconflicts = 0;
            ICard<V>[] _newCardTable = newCardTable;
            do
            {
                if (!card.Removed)
                {
                    ulong pos = getPosition(card.Key, newsize);

                    ICard<V> mem = _newCardTable[pos];

                    if (mem == null)
                    {
                        if (card.Extended != null)
                            _oldconflicts++;

                        card.Extended = null;
                        _newCardTable[pos] = card;
                    }
                    else
                    {
                        for (; ; )
                        {
                            if (mem.Extended == null)
                            {
                                card.Extended = null;
                                mem.Extended = card;
                                _conflicts++;
                                break;
                            }
                            else
                                mem = mem.Extended;
                        }
                    }
                }

                card = card.Next;
            } while (card != null);
            conflicts -= _oldconflicts;
        }

        private bool disposedValue = false;
        private IDeck<V> deckImplementation;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    first = null;
                    last = null;
                }

                table.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public bool Equals(IUnique? other)
        {
            return serialcode.Equals(other);
        }

        public int CompareTo(IUnique? other)
        {
            return serialcode.CompareTo(other);
        }

        public IUnique Empty => Usid.Empty;

        public ulong UniqueKey
        {
            get => serialcode.UniqueKey;
            set => serialcode.UniqueKey = value;
        }

        public ulong UniqueType
        {
            get => serialcode.UniqueType;
            set => serialcode.UniqueType = value;
        }

        public byte[] GetBytes()
        {
            return serialcode.GetBytes();
        }

        public byte[] GetUniqueBytes()
        {
            return serialcode.GetUniqueBytes();
        }

        public Type ElementType
        {
            get { return typeof(V); }
        }
        public Expression Expression { get; set; }
        public IQueryProvider Provider { get; protected set; }
    }
}
