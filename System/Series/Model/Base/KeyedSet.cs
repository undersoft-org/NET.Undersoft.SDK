
namespace System.Series.Basedeck
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Uniques;


    public abstract class KeyedSet<V> : Uniqueness, IDeck<V>, ISet<V>, IAsyncDisposable
    {

        private const float RESIZING_VECTOR = 1.766F;
        private const float CONFLICTS_PERCENT_LIMIT = 0.222F;
        private const float REMOVED_PERCENT_LIMIT = 0.15F;

        protected Uscn serialcode;
        protected ICard<V> first, last;
        protected ICard<V>[] table;
        protected readonly int minSize;
        protected int count, conflicts, removed, size, mincount;
        protected uint maxId;

        private int nextSize()
        {
            return (((int)(size * RESIZING_VECTOR)) | 3);
        }

        private int previousSize()
        {

            return (int)(size * (1 - REMOVED_PERCENT_LIMIT)) | 3;
        }

        protected void countIncrement()
        {
            if ((++count + 3) > size)
                Rehash(nextSize());
        }
        protected void conflictIncrement()
        {
            countIncrement();
            if (++conflicts > (size * CONFLICTS_PERCENT_LIMIT))
                Rehash(nextSize());
        }
        protected void removedIncrement()
        {
            --count;
            if (++removed > ((size * REMOVED_PERCENT_LIMIT) - 1))
            {
                if (size < (size * 0.5))
                    Rehash(previousSize());
                else
                    Rehash(size);
            }
        }
        protected void removedDecrement()
        {
            ++count;
            --removed;
        }

        protected KeyedSet(int capacity = 17, HashBits bits = HashBits.bit64) : base(bits)
        {
            size = capacity;
            minSize = capacity;
            maxId = (uint)(capacity - 1);
            table = EmptyCardTable(capacity);
            first = EmptyCard();
            last = first;
            ValueEquals = getValueComparer();
            serialcode = new Uscn(typeof(V).UniqueKey64());
        }
        protected KeyedSet(IList<ICard<V>> collection, int capacity = 17, HashBits bits = HashBits.bit64) : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            if (collection != null)
                this.Add(collection);
        }
        protected KeyedSet(IList<IUnique<V>> collection, int capacity = 17, HashBits bits = HashBits.bit64) : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            if (collection != null)
                foreach (var c in collection)
                    this.Add(c);
        }
        protected KeyedSet(IEnumerable<ICard<V>> collection, int capacity = 17, HashBits bits = HashBits.bit64) : this(capacity, bits)
        {
            if (collection != null)
                this.Add(collection);
        }
        protected KeyedSet(IEnumerable<IUnique<V>> collection, int capacity = 17, HashBits bits = HashBits.bit64) : this(capacity, bits)
        {
            if (collection != null)
                foreach (var c in collection)
                    this.Add(c);
        }

        public virtual ICard<V> First
        { get { return first; } }
        public virtual ICard<V> Last
        { get { return last; } }

        public virtual int Size => size;
        public virtual int Count => count;
        public virtual int MinCount
        {
            get => mincount;
            set => mincount = value;
        }
        public virtual bool IsReadOnly { get; set; }
        public virtual bool IsSynchronized { get; set; }
        public virtual bool IsRepeatable { get => false; }
        public virtual object SyncRoot { get; set; }
        public virtual Func<V, V, bool> ValueEquals { get; }

        public virtual V this[int index]
        {
            get => GetCard(index).Value;
            set => GetCard(index).Value = value;
        }
        protected V this[ulong hashkey]
        {
            get => InnerGet(hashkey);
            set => InnerPut(hashkey, value);
        }
        public virtual V this[object key]
        {
            get => InnerGet(unique.Key(key));
            set => InnerPut(unique.Key(key), value);
        }
        public virtual V this[IUnique key]
        {
            get => InnerGet(unique.Key(key));
            set => InnerPut(unique.Key(key), value);
        }
        object IFindable.this[object key]
        {
            get => InnerGet(unique.Key(key));
            set => InnerPut(unique.Key(key), (V)value);
        }

        protected virtual V InnerGet(ulong key)
        {
            ICard<V> mem = table[getPosition(key)];

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
        public virtual V Get(ulong key)
        {
            return InnerGet(key);
        }
        public virtual V Get(object key)
        {
            return InnerGet(unique.Key(key));
        }
        public virtual V Get(IUnique key)
        {
            return InnerGet(unique.Key(key));
        }
        public virtual V Get(IUnique<V> key)
        {
            return InnerGet(key.CompactKey());
        }

        protected virtual bool InnerTryGet(ulong key, out ICard<V> output)
        {
            output = null;

            ICard<V> mem = table[getPosition(key)];
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
        public virtual bool TryGet(ulong key, out ICard<V> output)
        {
            return InnerTryGet(key, out output);
        }
        public virtual bool TryGet(object key, out ICard<V> output)
        {
            return InnerTryGet(unique.Key(key), out output);
        }
        public virtual bool TryGet(IUnique key, out ICard<V> output)
        {
            return InnerTryGet(unique.Key(key), out output);
        }
        public virtual bool TryGet(IUnique<V> key, out ICard<V> output)
        {
            return InnerTryGet(key.CompactKey(), out output);
        }

        public virtual bool TryGet(object key, out V output)
        {
            output = default(V);
            if (InnerTryGet(unique.Key(key), out ICard<V> card))
            {
                output = card.Value;
                return true;
            }
            return false;
        }
        public virtual bool TryGet(ulong key, out V output)
        {
            if (InnerTryGet(key, out ICard<V> card))
            {
                output = card.Value;
                return true;
            }
            output = default(V);
            return false;
        }
        public virtual bool TryGet(IUnique key, out V output)
        {
            output = default;
            if (!InnerTryGet(unique.Key(key), out ICard<V> _output))
                return false;
            output = _output.Value;
            return true;
        }
        public virtual bool TryGet(IUnique<V> key, out V output)
        {
            output = default;
            if (!InnerTryGet(key.CompactKey(), out ICard<V> _output))
                return false;
            output = _output.Value;
            return true;
        }

        protected virtual ICard<V> InnerGetCard(ulong key)
        {
            ICard<V> mem = table[getPosition(key)];

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

            return mem;
        }
        public virtual ICard<V> GetCard(ulong key)
        {
            return InnerGetCard(key);
        }
        public virtual ICard<V> GetCard(object key)
        {
            return InnerGetCard(unique.Key(key));
        }
        public virtual ICard<V> GetCard(IUnique key)
        {
            return InnerGetCard(unique.Key(key, key.UniqueType));
        }
        public virtual ICard<V> GetCard(IUnique<V> key)
        {
            return InnerGetCard(key.CompactKey());
        }
        public abstract ICard<V> GetCard(int index);

        public virtual ICard<V> EnsureGet(object key, Func<ulong, V> sureaction)
        {
            ulong _key = unique.Key(key);
            return (!TryGet(_key, out ICard<V> item)) ?
                Put(key, sureaction.Invoke(_key)) : item;
        }
        public virtual ICard<V> EnsureGet(ulong key, Func<ulong, V> sureaction)
        {
            return (!TryGet(key, out ICard<V> item)) ?
                Put(key, sureaction.Invoke(key)) : item;
        }
        public virtual ICard<V> EnsureGet(IUnique key, Func<ulong, V> sureaction)
        {
            ulong _key = unique.Key(key);
            return (!TryGet(_key, out ICard<V> item)) ?
                Put(key, sureaction.Invoke(_key)) : item;
        }
        public virtual ICard<V> EnsureGet(IUnique<V> key, Func<ulong, V> sureaction)
        {
            ulong _key = key.CompactKey();
            return (!TryGet(_key, out ICard<V> item)) ?
                Put(key, sureaction.Invoke(_key)) : item;
        }

        protected virtual ICard<V> InnerSet(ulong key, V value)
        {
            var card = InnerGetCard(key);
            if (card != null) card.Value = value;
            return card;
        }
        protected virtual ICard<V> InnerSet(ICard<V> value)
        {
            var card = InnerGetCard(value.Key);
            if (card != null) card.Value = value.Value;
            return card;
        }
        protected virtual ICard<V> InnerSet(V value)
        {
            var card = InnerGetCard(unique.Key(value));
            if (card != null) card.Value = value;
            return card;
        }
        public virtual ICard<V> Set(object key, V value)
        {
            return InnerSet(unique.Key(key), value);
        }
        public virtual ICard<V> Set(ulong key, V value)
        {
            return InnerSet(key, value);
        }
        public virtual ICard<V> Set(IUnique key, V value)
        {
            return InnerSet(unique.Key(key), value);
        }
        public virtual ICard<V> Set(IUnique<V> key, V value)
        {
            return InnerSet(key.CompactKey(), value);
        }
        public virtual ICard<V> Set(V value)
        {
            return InnerSet(value);
        }
        public virtual ICard<V> Set(IUnique<V> value)
        {
            return InnerSet(value.CompactKey(), value.UniqueObject);
        }
        public virtual ICard<V> Set(ICard<V> value)
        {
            return InnerSet(value);
        }
        public virtual int Set(IEnumerable<V> values)
        {
            int count = 0;
            foreach (var value in values)
            {
                if (Set(value) != null)
                    count++;
            }

            return count;
        }
        public virtual int Set(IList<V> values)
        {
            int i = 0, c = values.Count;
            while (--c > -1)
            {
                if (Set(values[c]) != null)
                    i++;
            }
            return i;
        }
        public virtual int Set(IEnumerable<ICard<V>> values)
        {
            int count = 0;
            foreach (var value in values)
            {
                if (InnerSet(value) != null)
                    count++;
            }

            return count;
        }
        public virtual int Set(IEnumerable<IUnique<V>> values)
        {
            int count = 0;
            foreach (var value in values)
            {
                if (Set(value) != null)
                    count++;
            }

            return count;
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
        public virtual ICard<V> Put(ICard<V> card)
        {
            return InnerPut(card);
        }
        public virtual void Put(IList<ICard<V>> cards)
        {
            int i = 0, c = cards.Count;
            while (i < c)
                InnerPut(cards[i++]);
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
            int i = 0, c = cards.Count;
            while (i < c)
                InnerPut(cards[i++]);
        }
        public virtual void Put(IEnumerable<V> cards)
        {
            foreach (V card in cards)
                InnerPut(card);
        }
        public virtual ICard<V> Put(IUnique<V> value)
        {
            return InnerPut(unique.Key(value), value.UniqueObject);
        }
        public virtual void Put(IList<IUnique<V>> value)
        {
            int i = 0, c = value.Count;
            while (i < c)
                Put(value[i++]);
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
            int i = 0, c = cardList.Count;
            while (i < c)
                InnerAdd(cardList[i++]);
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
        bool ISet<V>.Add(V value)
        {
            return InnerAdd(value);
        }
        public virtual void Add(IList<V> cards)
        {
            int i = 0, c = cards.Count;
            while (i < c)
                InnerAdd(cards[i++]);
        }
        public virtual void Add(IEnumerable<V> cards)
        {
            foreach (V card in cards)
                Add(card);
        }
        public virtual void Add(IUnique<V> value)
        {
            InnerAdd(unique.Key(value), value.UniqueObject);
        }
        public virtual void Add(IList<IUnique<V>> value)
        {
            int i = 0, c = value.Count;
            while (i < c)
                Add(value[i++]);
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
        public virtual bool TryAdd(object key, V value)
        {
            return InnerAdd(unique.Key(key), value);
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
            ulong pos = getPosition(key);

            ICard<V> card = table[pos];

            if (card == null)
            {
                card = NewCard(item);
                table[pos] = card;
                InnerInsert(index, card);
                countIncrement();
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
                        conflictIncrement();
                        return;
                    }
                    throw new Exception("Item exist");

                }

                if (card.Extended == null)
                {
                    var newcard = NewCard(item);
                    card.Extended = newcard;
                    InnerInsert(index, newcard);
                    conflictIncrement();
                    return;
                }
                card = card.Extended;
            }
        }
        public virtual void Insert(int index, V item)
        {

            ulong key = unique.Key(item);
            ulong pos = getPosition(key);

            ICard<V> card = table[pos];

            if (card == null)
            {
                card = NewCard(item);
                table[pos] = card;
                InnerInsert(index, card);
                countIncrement();
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
                        conflictIncrement();
                        return;
                    }
                    throw new Exception("Item exist");

                }

                if (card.Extended == null)
                {
                    var newcard = NewCard(item);
                    card.Extended = newcard;
                    InnerInsert(index, newcard);
                    conflictIncrement();
                    return;
                }
                card = card.Extended;
            }
        }

        public virtual bool Enqueue(V value)
        {
            return InnerAdd(value);
        }
        public virtual bool Enqueue(object key, V value)
        {
            return InnerAdd(unique.Key(key), value);
        }
        public virtual void Enqueue(ICard<V> card)
        {
            InnerAdd(card);
        }

        public virtual void SetMinCount(int minCount)
        {
            mincount = minCount;
        }

        public virtual V Dequeue()
        {
            var card = Next(first);
            if (card != null)
            {
                card.Removed = true;
                removedIncrement();
                first = card;
                return card.Value;
            }
            return default(V);
        }
        public virtual bool TryDequeue(out V output)
        {
            output = default(V);
            if (count < mincount)
                return false;

            var card = Next(first);
            if (card != null)
            {
                card.Removed = true;
                removedIncrement();
                first = card;
                output = card.Value;
                return true;
            }
            return false;
        }
        public virtual bool TryDequeue(out ICard<V> output)
        {
            output = null;
            if (count < mincount)
                return false;

            output = Next(first);
            if (output != null)
            {
                output.Removed = true;
                removedIncrement();
                first = output;
                return true;
            }
            return false;
        }

        public virtual bool TryPick(int skip, out V output)
        {
            output = this.Skip(skip).FirstOrDefault();
            if (output != null)
            {
                return true;
            }
            return false;
        }

        public virtual bool TryTake(out V output)
        {
            return TryDequeue(out output);
        }

        protected virtual void renewClear(int capacity)
        {
            if (capacity != size || count > 0)
            {
                size = capacity;
                maxId = (uint)(capacity - 1);
                conflicts = 0;
                removed = 0;
                count = 0;
                table = EmptyCardTable(size);
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
            ICard<V> mem = table[getPosition(key)];

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

        protected virtual Func<V, V, bool> getValueComparer()
        {
            if (typeof(V).IsValueType)
                return (o1, o2) => o1.Equals(o2);
            return (o1, o2) => ReferenceEquals(o1, o2);
        }

        protected virtual V InnerRemove(ulong key)
        {
            ICard<V> mem = table[getPosition(key)];

            while (mem != null)
            {
                if (mem.Equals(key))
                {
                    if (mem.Removed)
                        return default(V);

                    mem.Removed = true;
                    removedIncrement();
                    return mem.Value;
                }

                mem = mem.Extended;
            }
            return default(V);
        }
        protected virtual V InnerRemove(ulong key, V item)
        {
            ICard<V> mem = table[getPosition(key)];

            while (mem != null)
            {
                if (mem.Equals(key))
                {
                    if (mem.Removed)
                        return default(V);

                    if (ValueEquals(mem.Value, item))
                    {
                        mem.Removed = true;
                        removedIncrement();
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
            return InnerRemove(unique.Key(item)).Equals(default(V)) ? false : true;
        }
        public virtual bool TryRemove(object key)
        {
            V result = InnerRemove(unique.Key(key));
            if (result != null &&
                !result.Equals(default(V)))
                return true;
            return false;
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
            maxId = (uint)(minSize - 1);
            conflicts = 0;
            removed = 0;
            count = 0;
            table = EmptyCardTable(size);
            first = EmptyCard();
            last = first;
        }

        public virtual void Flush()
        {
            conflicts = 0;
            removed = 0;
            count = 0;
            table = null;
            table = EmptyCardTable(size);
            first = EmptyCard();
            last = first;
        }

        public virtual void CopyTo(ICard<V>[] array, int index)
        {
            int c = count, i = index, l = array.Length;
            if (l - i < c)
            {
                c = l - i;
                foreach (ICard<V> ves in this.Take(c))
                    array[i++] = ves;
            }
            else
                foreach (ICard<V> ves in this)
                    array[i++] = ves;
        }
        public virtual void CopyTo(IUnique<V>[] array, int arrayIndex)
        {
            int c = count, i = arrayIndex, l = array.Length;
            if (l - i < c)
            {
                c = l - i;
                foreach (ICard<V> ves in this.Take(c))
                    array[i++] = ves;
            }
            else
                foreach (ICard<V> ves in this)
                    array[i++] = ves;
        }
        public virtual void CopyTo(Array array, int index)
        {
            int c = count, i = index, l = array.Length;
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
            int c = count, i = index, l = array.Length;
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

        public virtual V[] ToArray()
        {
            return this.AsValues().ToArray();
        }
        public virtual object[] ToObjectArray()
        {
            return AsValues().Cast<object>().ToArray();
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

        public virtual void Resize(int size)
        {
            Rehash(size);
        }

        public abstract ICard<V> EmptyCard();

        public abstract ICard<V> NewCard(ulong key, V value);
        public abstract ICard<V> NewCard(object key, V value);
        public abstract ICard<V> NewCard(ICard<V> card);
        public abstract ICard<V> NewCard(V card);

        public abstract ICard<V>[] EmptyCardTable(int size);

        public virtual int IndexOf(ICard<V> item)
        {
            if (item != null)
                return item.Index;
            return -1;
        }
        public virtual int IndexOf(V item)
        {
            var card = GetCard(item);
            if (card != null)
                return card.Index;
            return -1;
        }
        protected virtual int IndexOf(ulong key, V item)
        {
            var card = GetCard(key);
            if (card != null && ValueEquals(card.Value, item))
                return card.Index;
            return -1;
        }

        public virtual IEnumerable<V> AsValues()
        {
            return this;
        }
        public virtual IEnumerable<ICard<V>> AsCards()
        {
            foreach (ICard<V> card in this)
            {
                yield return card;
            }
        }
        public virtual IEnumerator<ICard<V>> GetEnumerator()
        {
            return new CardSeries<V>(this);
        }
        public virtual IAsyncEnumerator<V> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new CardSeriesAsync<V>(this);
        }
        public virtual IEnumerator<ulong> GetKeyEnumerator()
        {
            return new CardUniqueKeySeries<V>(this);
        }

        IEnumerator<V> IEnumerable<V>.GetEnumerator()
        {
            return new CardSeries<V>(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CardSeries<V>(this);
        }

        protected ulong getPosition(ulong key)
        {

            return (key % maxId);
        }
        protected static ulong getPosition(ulong key, uint tableMaxId)
        {
            return (key % tableMaxId);
        }

        protected virtual void Rehash(int newSize)
        {
            int finish = count;
            int newsize = newSize;
            uint newMaxId = (uint)(newsize - 1);
            ICard<V>[] newcardTable = EmptyCardTable(newsize);
            ICard<V> card = first;
            card = card.Next;
            if (removed > 0)
            {
                rehashAndReindex(card, newcardTable, newMaxId);
            }
            else
            {
                rehash(card, newcardTable, newMaxId);
            }

            table = newcardTable;
            maxId = newMaxId;
            size = newsize;
        }

        private void rehashAndReindex(ICard<V> card, ICard<V>[] newCardTable, uint newMaxId)
        {
            int _conflicts = 0;
            uint _newMaxId = newMaxId;
            ICard<V>[] _newCardTable = newCardTable;
            ICard<V> _firstcard = EmptyCard();
            ICard<V> _lastcard = _firstcard;
            do
            {
                if (!card.Removed)
                {
                    ulong pos = getPosition(card.Key, _newMaxId);

                    ICard<V> ex_card = _newCardTable[pos];

                    if (ex_card == null)
                    {
                        card.Extended = null;
                        _newCardTable[pos] = _lastcard = _lastcard.Next = card;
                    }
                    else
                    {
                        for (; ; )
                        {
                            if (ex_card.Extended == null)
                            {
                                card.Extended = null; ;
                                _lastcard = _lastcard.Next = ex_card.Extended = card;
                                _conflicts++;
                                break;
                            }
                            else
                                ex_card = ex_card.Extended;
                        }
                    }
                }

                card = card.Next;

            } while (card != null);

            conflicts = _conflicts;
            removed = 0;
            first = _firstcard;
            last = _lastcard;
        }

        private void rehash(ICard<V> card, ICard<V>[] newCardTable, uint newMaxId)
        {
            int _conflicts = 0;
            uint _newMaxId = newMaxId;
            ICard<V>[] _newCardTable = newCardTable;
            do
            {
                if (!card.Removed)
                {
                    ulong pos = getPosition(card.Key, _newMaxId);

                    ICard<V> ex_card = _newCardTable[pos];

                    if (ex_card == null)
                    {
                        card.Extended = null;
                        _newCardTable[pos] = card;
                    }
                    else
                    {
                        for (; ; )
                        {
                            if (ex_card.Extended == null)
                            {
                                card.Extended = null;
                                ex_card.Extended = card;
                                _conflicts++;
                                break;
                            }
                            else
                                ex_card = ex_card.Extended;
                        }
                    }
                }

                card = card.Next;

            } while (card != null);
            conflicts = _conflicts;
        }

        protected bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    first = null;
                    last = null;
                    table = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual async ValueTask DisposeAsyncCore()
        {
            await new ValueTask(Task.Run(() =>
            {
                first = null;
                last = null;
                table = null;

            })).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(false);

            GC.SuppressFinalize(this);
        }

        public virtual bool Equals(IUnique? other)
        {
            return serialcode.Equals(other);
        }

        public virtual int CompareTo(IUnique? other)
        {
            return serialcode.CompareTo(other);
        }

        public IUnique Empty => Usid.Empty;

        public virtual ulong UniqueKey
        {
            get => serialcode.UniqueKey;
            set => serialcode.UniqueKey = value;
        }

        public virtual ulong UniqueType
        {
            get => serialcode.UniqueType;
            set => serialcode.UniqueType = value;
        }

        public virtual byte[] GetBytes()
        {
            return serialcode.GetBytes();
        }

        public virtual byte[] GetUniqueBytes()
        {
            return serialcode.GetUniqueBytes();
        }

        public virtual void ExceptWith(IEnumerable<V> other)
        {
            throw new NotImplementedException();
        }
        public virtual void IntersectWith(IEnumerable<V> other)
        {
            throw new NotImplementedException();
        }
        public virtual bool IsProperSubsetOf(IEnumerable<V> other)
        {
            throw new NotImplementedException();
        }
        public virtual bool IsProperSupersetOf(IEnumerable<V> other)
        {
            throw new NotImplementedException();
        }
        public virtual bool IsSubsetOf(IEnumerable<V> other)
        {
            throw new NotImplementedException();
        }
        public virtual bool IsSupersetOf(IEnumerable<V> other)
        {
            throw new NotImplementedException();
        }
        public virtual bool Overlaps(IEnumerable<V> other)
        {
            throw new NotImplementedException();
        }
        public virtual bool SetEquals(IEnumerable<V> other)
        {
            throw new NotImplementedException();
        }
        public virtual void SymmetricExceptWith(IEnumerable<V> other)
        {
            other.ForOnly(e => Contains(e), (e) => Remove(e));
        }
        public virtual void UnionWith(IEnumerable<V> other)
        {
            this.Add(other);
        }
    }
}
