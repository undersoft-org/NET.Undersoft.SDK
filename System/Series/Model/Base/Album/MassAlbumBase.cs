namespace System.Series
{
    using System.Collections.Generic;
    using System.Series.Basedeck;
    using System.Uniques;

    public abstract class MassAlbumBase<V> : TypedSet<V> where V : IUnique
    {
        protected ICard<V>[] list;
        protected bool repeatable;
        protected int repeated;

        public MassAlbumBase() : this(17, HashBits.bit64) { }

        public MassAlbumBase(
            IEnumerable<IUnique<V>> collection,
            int capacity = 17,
            bool repeatable = false,
            HashBits bits = HashBits.bit64
        ) : this(repeatable, capacity, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public MassAlbumBase(
            IEnumerable<V> collection,
            int capacity = 17,
            bool repeatable = false,
            HashBits bits = HashBits.bit64
        ) : this(repeatable, capacity, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        public MassAlbumBase(bool repeatable, int capacity = 17, HashBits bits = HashBits.bit64)
            : base(capacity, bits)
        {
            this.repeatable = repeatable;
            list = EmptyDeck(capacity);
        }

        public MassAlbumBase(int capacity = 17, HashBits bits = HashBits.bit64)
            : base(capacity, bits)
        {
            list = EmptyDeck(capacity);
        }

        public override ICard<V> First
        {
            get { return first; }
        }

        public override ICard<V> Last
        {
            get { return list[(count + removed) - 1]; }
        }

        public override bool IsRepeatable
        {
            get => repeatable;
        }

        public override void Clear()
        {
            base.Clear();
            list = EmptyDeck(minSize);
        }

        public override void CopyTo(Array array, int index)
        {
            int c = count,
                i = index,
                l = array.Length;

            if (l - i < c)
                c = l - i;

            for (int j = 0; j < c; j++)
                array.SetValue(GetCard(j).Value, i++);
        }

        public override void CopyTo(ICard<V>[] array, int index)
        {
            int c = count,
                i = index,
                l = array.Length;

            if (l - i < c)
                c = l - i;

            for (int j = 0; j < c; j++)
            {
                array[i++] = GetCard(j);
            }
        }

        public override void CopyTo(V[] array, int index)
        {
            int c = count,
                i = index,
                l = array.Length;

            if (l - i < c)
                c = l - i;

            for (int j = 0; j < c; j++)
                array[i++] = GetCard(j).Value;
        }

        public abstract ICard<V>[] EmptyDeck(int size);

        public override void Flush()
        {
            base.Flush();
            list = EmptyDeck(size);
        }

        protected virtual ICard<V> GetCard(ulong key, V item)
        {
            if (key == 0)
                return null;

            ICard<V> mem = table[getPosition(key)];

            while (mem != null)
            {
                if (mem.Equals(key))
                {
                    if (repeatable)
                        while (mem != null || !ValueEquals(mem.Value, item))
                            mem = mem.Next;

                    if (!mem.Removed)
                        return mem;
                    return null;
                }
                mem = mem.Extended;
            }

            return mem;
        }

        public override ICard<V> GetCard(int index)
        {
            if (index < count)
            {
                if (removed > 0)
                    Reindex();

                return list[index];
            }
            throw new IndexOutOfRangeException("Index out of range");
        }

        public override ICard<V> Next(ICard<V> card)
        {
            ICard<V> _card = list[card.Index + 1];
            if (_card != null)
            {
                if (!_card.Removed)
                    return _card;
                return Next(_card);
            }
            return null;
        }

        public override V[] ToArray()
        {
            V[] array = new V[count];
            CopyTo(array, 0);
            return array;
        }

        protected ICard<V> createNew(ICard<V> card)
        {
            int id = count + removed;
            card.Index = id;
            list[id] = card;
            return card;
        }

        protected ICard<V> createNew(ulong key, V value)
        {
            int id = count + removed;
            var newcard = NewCard(key, value);
            newcard.Index = id;
            list[id] = newcard;
            return newcard;
        }

        protected void createRepeated(ICard<V> card, V value)
        {
            var _card = createNew(card.Key, card.Value);
            card.Value = value;
            _card.Next = card.Next;
            card.Next = _card;
            _card.Repeated = true;
        }

        protected void createRepeated(ICard<V> card, ICard<V> newcard)
        {
            var _card = createNew(newcard);
            var val = card.Value;
            card.Value = _card.Value;
            _card.Value = val;
            _card.Next = card.Next;
            card.Next = _card;
            _card.Repeated = true;
        }

        protected ICard<V> swapRepeated(ICard<V> card)
        {
            var value = card.Value;
            var _card = card.Next;
            card.Value = _card.Value;
            _card.Value = value;
            card.Next = _card.Next;
            _card.Next = _card;
            return _card;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    first = null;
                    last = null;
                }
                table = null;
                list = null;

                disposedValue = true;
            }
        }

        protected override bool InnerAdd(ICard<V> value)
        {
            ulong key = value.Key;
            ulong pos = getPosition(key);

            ICard<V> card = table[pos];

            if (card == null)
            {
                table[pos] = createNew(value);
                countIncrement();
                return true;
            }

            for (; ; )
            {
                if (card.Equals(key))
                {
                    if (card.Removed)
                    {
                        card.Removed = false;
                        card.Value = value.Value;
                        removedDecrement();
                        return true;
                    }

                    if (repeatable)
                    {
                        createRepeated(card, value);
                        countIncrement();
                        return true;
                    }
                    return false;
                }

                if (card.Extended == null)
                {
                    card.Extended = createNew(value);
                    conflictIncrement();
                    return true;
                }
                card = card.Extended;
            }
        }

        protected override bool InnerAdd(ulong key, V value)
        {
            ulong pos = getPosition(key);

            ICard<V> card = table[pos];

            if (card == null)
            {
                table[pos] = createNew(key, value);
                countIncrement();
                return true;
            }

            for (; ; )
            {
                if (card.Equals(key))
                {
                    if (card.Removed)
                    {
                        card.Removed = false;
                        card.Value = value;
                        removedDecrement();
                        return true;
                    }

                    if (repeatable)
                    {
                        createRepeated(card, value);
                        countIncrement();
                        return true;
                    }

                    return false;
                }

                if (card.Extended == null)
                {
                    card.Extended = createNew(key, value);
                    conflictIncrement();
                    return true;
                }

                card = card.Extended;
            }
        }

        protected override bool InnerAdd(V value)
        {
            ulong key = unique.Key(value, value.UniqueType);

            ulong pos = getPosition(key);
            ICard<V> card = table[pos];

            if (card == null)
            {
                table[pos] = createNew(key, value);
                countIncrement();
                return true;
            }

            for (; ; )
            {
                if (card.Equals(key))
                {
                    if (card.Removed)
                    {
                        card.Removed = false;
                        card.Value = value;
                        removedDecrement();
                        return true;
                    }

                    if (repeatable)
                    {
                        createRepeated(card, value);
                        countIncrement();
                        return true;
                    }

                    return false;
                }

                if (card.Extended == null)
                {
                    card.Extended = createNew(key, value);
                    conflictIncrement();
                    return true;
                }

                card = card.Extended;
            }
        }

        protected override void InnerInsert(int index, ICard<V> item)
        {
            int c = count - index;
            if (c > 0)
            {
                if (removed > 0)
                    reindexWithInsert(index, item);
                else
                {
                    var replaceCard = GetCard(index);

                    while (replaceCard != null)
                    {
                        int id = ++replaceCard.Index;
                        var _replaceCard = list[id];
                        list[id] = replaceCard;
                        replaceCard = _replaceCard;
                    }

                    item.Index = index;
                    list[index] = item;
                }
            }
            else
            {
                int id = count + removed;
                item.Index = id;
                list[id] = item;
            }
        }

        protected override ICard<V> InnerPut(ICard<V> value)
        {
            ulong key = value.Key;

            ulong pos = getPosition(key);

            ICard<V> card = table[pos];

            if (card == null)
            {
                card = createNew(value);
                table[pos] = card;
                countIncrement();
                return card;
            }

            for (; ; )
            {
                if (card.Equals(key))
                {
                    card.Value = value.Value;

                    if (card.Removed)
                    {
                        card.Removed = false;
                        removedDecrement();
                    }
                    return card;
                }

                if (card.Extended == null)
                {
                    var newcard = createNew(value);
                    card.Extended = newcard;
                    conflictIncrement();
                    return newcard;
                }
                card = card.Extended;
            }
        }

        protected override ICard<V> InnerPut(ulong key, V value)
        {
            ulong pos = getPosition(key);
            ICard<V> card = table[pos];

            if (card == null)
            {
                card = createNew(key, value);
                table[pos] = card;
                countIncrement();
                return card;
            }

            for (; ; )
            {
                if (card.Equals(key))
                {
                    card.Value = value;
                    if (card.Removed)
                    {
                        card.Removed = false;
                        removedDecrement();
                    }

                    return card;
                }

                if (card.Extended == null)
                {
                    var newcard = createNew(key, value);
                    card.Extended = newcard;
                    conflictIncrement();
                    return newcard;
                }

                card = card.Extended;
            }
        }

        protected override ICard<V> InnerPut(V value)
        {
            ulong key = unique.Key(value, value.UniqueType);
            ulong pos = getPosition(key);
            ICard<V> card = table[pos];

            if (card == null)
            {
                card = createNew(key, value);
                table[pos] = card;
                countIncrement();
                return card;
            }

            for (; ; )
            {
                if (card.Equals(key))
                {
                    card.Value = value;
                    if (card.Removed)
                    {
                        card.Removed = false;
                        removedDecrement();
                    }

                    return card;
                }

                if (card.Extended == null)
                {
                    var newcard = createNew(key, value);
                    card.Extended = newcard;
                    conflictIncrement();
                    return newcard;
                }

                card = card.Extended;
            }
        }

        protected override V InnerRemove(ulong key)
        {
            ICard<V> _card = table[getPosition(key)];

            while (_card != null)
            {
                if (_card.Equals(key))
                {
                    if (_card.Removed)
                        return default(V);

                    if (repeatable && _card.Next != null)
                        _card = swapRepeated(_card);

                    _card.Removed = true;
                    removedIncrement();
                    return _card.Value;
                }
                _card = _card.Extended;
            }
            return default(V);
        }

        protected override V InnerRemove(ulong key, V item)
        {
            ICard<V> _card = table[getPosition(key)];

            while (_card != null)
            {
                if (_card.Equals(key))
                {
                    if (_card.Removed)
                        return default(V);
                    do
                    {
                        if (ValueEquals(_card.Value, item))
                        {
                            if (_card.Next != null)
                                _card = swapRepeated(_card);

                            _card.Removed = true;
                            removedIncrement();
                            return _card.Value;
                        }
                        _card = _card.Next;
                    } while (_card != null);
                    return default(V);
                }
                _card = _card.Extended;
            }
            return default(V);
        }

        public virtual bool TryRemove(ulong key, V item)
        {
            var output = InnerRemove(key, item);
            return (output != null) ? true : false;
        }

        public override int IndexOf(V item)
        {
            return IndexOf(unique.Key(item), item);
        }

        protected override int IndexOf(ulong key, V item)
        {
            if (!repeatable)
                return base.IndexOf(key, item);

            var card = GetCard(key);
            if (card == null)
                return -1;

            do
            {
                if (ValueEquals(card.Value, item))
                    return card.Index;

                card = card.Next;
            } while (card != null);

            return -1;
        }

        public override bool Contains(ICard<V> item)
        {
            return IndexOf(item.Key, item.Value) > -1;
        }

        public override bool Contains(IUnique<V> item)
        {
            return IndexOf(unique.Key(item), item.UniqueObject) > -1;
        }

        public override bool Contains(V item)
        {
            return IndexOf(item) > -1;
        }

        public override bool Contains(ulong key, V item)
        {
            return IndexOf(key, item) > -1;
        }

        public override V Dequeue()
        {
            var _output = Next(first);
            if (_output == null)
                return default(V);

            if (repeatable && _output.Next != null)
                _output = swapRepeated(_output);
            else
                first = _output;

            _output.Removed = true;
            removedIncrement();
            return _output.Value;
        }

        public override bool TryDequeue(out V output)
        {
            output = default(V);
            if (count < mincount)
                return false;

            var _output = Next(first);
            if (_output == null)
                return false;

            if (repeatable && _output.Next != null)
                _output = swapRepeated(_output);
            else
                first = _output;

            _output.Removed = true;
            removedIncrement();
            output = _output.Value;
            return true;
        }

        public override bool TryDequeue(out ICard<V> output)
        {
            output = null;
            if (count < mincount)
                return false;

            output = Next(first);
            if (output == null)
                return false;

            if (repeatable && output.Next != null)
                output = swapRepeated(output);
            else
                first = output;

            output.Removed = true;
            removedIncrement();
            return true;
        }

        protected override void Rehash(int newsize)
        {
            int finish = count;
            int _newsize = newsize;
            uint newMaxId = (uint)(_newsize - 1);
            ICard<V>[] newCardTable = EmptyCardTable(_newsize);
            if (removed != 0)
            {
                ICard<V>[] newBaseDeck = EmptyDeck(_newsize);
                rehashAndReindex(newCardTable, newBaseDeck, newMaxId);
                list = newBaseDeck;
            }
            else
            {
                ICard<V>[] newBaseDeck = EmptyDeck(_newsize);
                rehash(newCardTable, newMaxId);
                Array.Copy(list, 0, newBaseDeck, 0, finish);
                list = newBaseDeck;
            }
            table = newCardTable;
            maxId = newMaxId;
            size = newsize;
        }

        protected virtual void Reindex()
        {
            ICard<V> card = null;
            first = EmptyCard();
            int total = count + removed;
            int _counter = 0;
            for (int i = 0; i < total; i++)
            {
                card = list[i];
                if (card != null && !card.Removed)
                {
                    card.Index = _counter;
                    list[_counter++] = card;
                }
            }
            removed = 0;
        }

        protected override void renewClear(int capacity)
        {
            if (capacity != size || count > 0)
            {
                size = capacity;
                maxId = (uint)(capacity - 1);
                conflicts = 0;
                removed = 0;
                count = 0;
                table = EmptyCardTable(size);
                list = EmptyDeck(size);
                first = EmptyCard();
                last = first;
            }
        }

        private void rehash(ICard<V>[] newCardTable, uint newMaxId)
        {
            int _conflicts = 0;
            int total = count + removed;
            uint _newMaxId = newMaxId;
            ICard<V>[] _newCardTable = newCardTable;
            ICard<V> card = null;
            ICard<V> mem = null;
            for (int i = 0; i < total; i++)
            {
                card = list[i];

                if (card == null || card.Removed || (repeatable && card.Repeated))
                    continue;

                ulong pos = getPosition(card.Key, _newMaxId);

                mem = _newCardTable[pos];

                if (mem == null)
                {
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
                        mem = mem.Extended;
                    }
                }
            }
            conflicts = _conflicts;
        }

        private void rehashAndReindex(
            ICard<V>[] newCardTable,
            ICard<V>[] newBaseDeck,
            uint newMaxId
        )
        {
            int _conflicts = 0;
            int _counter = 0;
            int total = count + removed;
            uint _newMaxId = newMaxId;
            ICard<V>[] _newCardTable = newCardTable;
            ICard<V>[] _newBaseDeck = newBaseDeck;
            ICard<V> card = null;
            ICard<V> mem = null;
            for (int i = 0; i < total; i++)
            {
                card = list[i];

                if (card == null || card.Removed)
                    continue;

                if (card.Repeated)
                {
                    card.Index = _counter;
                    _newBaseDeck[_counter++] = card;
                    continue;
                }

                ulong pos = getPosition(card.Key, _newMaxId);

                mem = _newCardTable[pos];

                if (mem == null)
                {
                    card.Extended = null;
                    card.Index = _counter;
                    _newCardTable[pos] = card;
                    _newBaseDeck[_counter++] = card;
                }
                else
                {
                    for (; ; )
                    {
                        if (mem.Extended == null)
                        {
                            card.Extended = null;
                            mem.Extended = card;
                            card.Index = _counter;
                            _newBaseDeck[_counter++] = card;
                            _conflicts++;
                            break;
                        }
                        mem = mem.Extended;
                    }
                }
            }
            first = EmptyCard();
            conflicts = _conflicts;
            removed = 0;
        }

        private void reindexWithInsert(int index, ICard<V> item)
        {
            ICard<V> card = null;
            first = EmptyCard();
            int _counter = 0;
            int total = count + removed;
            for (int i = 0; i < total; i++)
            {
                card = list[i];
                if (card != null && !card.Removed)
                {
                    card.Index = _counter;
                    list[_counter++] = card;
                    if (_counter == index)
                    {
                        item.Index = _counter;
                        list[_counter++] = item;
                    }
                }
            }
            removed = 0;
        }
    }
}
