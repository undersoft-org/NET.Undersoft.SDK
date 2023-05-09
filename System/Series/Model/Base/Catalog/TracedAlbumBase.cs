namespace System.Series
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Instant;
    using System.Uniques;

    public class TracedAlbumBase<V>
        : AlbumBase<V>,
            ITraceable,
            INotifyPropertyChanged,
            INotifyPropertyChanging,
            INotifyCollectionChanged where V : class, ITraceable
    {
        int readers;

        public TracedAlbumBase() : this(false, 17, HashBits.bit64) { }

        public TracedAlbumBase(int capacity = 17, HashBits bits = HashBits.bit64)
            : base(capacity, bits)
        {
            Initialize();
        }

        public TracedAlbumBase(bool repeatable, int capacity = 17, HashBits bits = HashBits.bit64)
            : base(repeatable, capacity, bits)
        {
            Initialize();
        }

        public TracedAlbumBase(
            IEnumerable<IUnique<V>> collection,
            int capacity = 17,
            bool repeatable = false,
            HashBits bits = HashBits.bit64
        ) : this(repeatable, capacity, bits)
        {
            Initialize();
            if (collection != null)
                foreach (IUnique<V> c in collection)
                    Add(c);
        }

        public TracedAlbumBase(
            IEnumerable<V> collection,
            int capacity = 17,
            bool repeatable = false,
            HashBits bits = HashBits.bit64
        ) : this(repeatable, capacity, bits)
        {
            Initialize();
            if (collection != null)
                foreach (V c in collection)
                    Add(c);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        protected void AddNotifier(V itemAdded)
        {
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemAdded)
            );
        }

        protected void AddNotifier(IEnumerable<V> itemsAdded)
        {
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemsAdded)
            );
        }

        protected void ChangeNotiifer(V newItem, V oldItem)
        {
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace,
                    newItem,
                    oldItem
                )
            );
        }

        protected override ICard<V> GetCard(ulong key, V item)
        {
            ICard<V> card = base.GetCard(key, item);

            return card;
        }

        protected override int IndexOf(ulong key, V item)
        {
            int id = 0;

            id = base.IndexOf(key, item);

            return id;
        }

        protected override V InnerGet(ulong key)
        {
            V v = base.InnerGet(key);

            return v;
        }

        protected override ICard<V> InnerGetCard(ulong key)
        {
            return base.InnerGetCard(key);
        }

        protected override ICard<V> InnerPut(ICard<V> value)
        {
            int _count = count;

            ICard<V> temp = base.InnerPut(value);

            return temp;
        }

        protected override ICard<V> InnerPut(V value)
        {
            ICard<V> temp = base.InnerPut(value);

            return temp;
        }

        protected override ICard<V> InnerPut(ulong key, V value)
        {
            ICard<V> temp = base.InnerPut(key, value);

            return temp;
        }

        protected override V InnerRemove(ulong key)
        {
            V temp = base.InnerRemove(key);
            RemoveNotifier(temp);
            return temp;
        }

        protected override ICard<V> InnerSet(ICard<V> value)
        {
            ICard<V> card = InnerGetCard(value.Key);
            if (card != null)
            {
                V oldItem = card.Value;
                V newItem = value.Value;
                card.Value = newItem;
                ChangeNotiifer(newItem, oldItem);
            }
            return card;
        }

        protected override ICard<V> InnerSet(V value)
        {
            return InnerSet(unique.Key(value), value);
        }

        protected override ICard<V> InnerSet(ulong key, V value)
        {
            ICard<V> card = InnerGetCard(key);
            if (card != null)
            {
                V oldItem = card.Value;
                V newItem = value;
                card.Value = newItem;
                ChangeNotiifer(newItem, oldItem);
            }
            return card;
        }

        protected override bool InnerTryGet(ulong key, out ICard<V> output)
        {
            return base.InnerTryGet(key, out output);
        }

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged.Invoke(this, e);
        }

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged.Invoke(sender, e);
        }

        protected void OnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            PropertyChanging.Invoke(sender, e);
        }

        protected override void Rehash(int newsize)
        {
            base.Rehash(newsize);
        }

        protected override void Reindex()
        {
            base.Reindex();
        }

        protected void RemoveNotifier(V itemRemoved)
        {
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    itemRemoved
                )
            );
        }

        protected void ReplaceNotifier(V itemsMoved)
        {
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, itemsMoved)
            );
        }

        protected void ResetNotifier(IEnumerable<V> itemsReset)
        {
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset,
                    itemsReset
                )
            );
        }

        protected override bool InnerAdd(ICard<V> value)
        {
            bool temp = base.InnerAdd(value);
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, temp)
            );

            return temp;
        }

        protected override bool InnerAdd(V value)
        {
            bool temp = base.InnerAdd(value);
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, temp)
            );

            return temp;
        }

        protected override bool InnerAdd(ulong key, V value)
        {
            bool temp = base.InnerAdd(key, value);
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, temp)
            );

            return temp;
        }

        public override void Clear()
        {
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
            );
            base.Clear();
        }

        public override void CopyTo(Array array, int index)
        {
            base.CopyTo(array, index);
        }

        public override void CopyTo(ICard<V>[] array, int index)
        {
            base.CopyTo(array, index);
        }

        public override void CopyTo(V[] array, int index)
        {
            base.CopyTo(array, index);
        }

        public override V Dequeue()
        {
            V temp = base.Dequeue();
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, temp)
            );

            return temp;
        }

        public override ICard<V> EmptyCard()
        {
            return new TracedCard<V>();
        }

        public override ICard<V>[] EmptyCardTable(int size)
        {
            return new TracedCard<V>[size];
        }

        public override ICard<V>[] EmptyDeck(int size)
        {
            return new TracedCard<V>[size];
        }

        public override ICard<V> GetCard(int index)
        {
            if (index < count)
            {
                if (removed > 0)
                {
                    Reindex();
                }

                ICard<V> temp = list[index];

                return temp;
            }
            throw new IndexOutOfRangeException("Index out of range");
        }

        public override int IndexOf(ICard<V> item)
        {
            int id = 0;

            id = base.IndexOf(item);

            return id;
        }

        public void Initialize()
        {
            NoticeChange = new Deputy<TracedAlbumBase<V>>(this, a => nameof(a.OnPropertyChanged));
            NoticeChanging = new Deputy<TracedAlbumBase<V>>(
                this,
                a => nameof(a.OnPropertyChanging)
            );
        }

        public override void Insert(int index, ICard<V> item)
        {
            InnerInsert(index, item);
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item.Value)
            );
        }

        public override ICard<V> NewCard(ICard<V> card)
        {
            return new TracedCard<V>(card);
        }

        public override ICard<V> NewCard(V value)
        {
            return new TracedCard<V>(value);
        }

        public override ICard<V> NewCard(object key, V value)
        {
            return new TracedCard<V>(key, value);
        }

        public override ICard<V> NewCard(ulong key, V value)
        {
            return new TracedCard<V>(key, value);
        }

        public override V[] ToArray()
        {
            V[] array = base.ToArray();

            return array;
        }

        public override bool TryDequeue(out ICard<V> output)
        {
            bool temp = base.TryDequeue(out output);
            RemoveNotifier(output.Value);
            return temp;
        }

        public override bool TryDequeue(out V output)
        {
            bool temp = base.TryDequeue(out output);
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, temp)
            );

            return temp;
        }

        public override bool TryPick(int skip, out V output)
        {
            bool temp = base.TryPick(skip, out output);

            return temp;
        }

        public IDeputy NoticeChange { get; set; }

        public IDeputy NoticeChanging { get; set; }

        public IVariety Variator { get; set; }
    }
}
