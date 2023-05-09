namespace System.Series
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Uniques;

    [StructLayout(LayoutKind.Sequential)]
    public abstract class CardBase<V> : ICard<V>
    {
        protected V value;
        protected bool? isUnique;
        private bool disposedValue = false;
        private ICard<V> extended;
        private ICard<V> next;
        private IDeck<V> deck;

        public CardBase() { }

        public CardBase(ICard<V> value) : base()
        {
            Set(value);
        }

        public CardBase(object key, V value) : base()
        {
            Set(key, value);
        }

        public CardBase(ulong key, V value) : base()
        {
            Set(key, value);
        }

        public CardBase(V value) : base()
        {
            Set(value);
        }

        public virtual IUnique Empty => throw new NotImplementedException();

        public virtual ICard<V> Extended
        {
            get => extended;
            set => extended = value;
        }

        public virtual int Index { get; set; } = -1;

        public abstract ulong Key { get; set; }

        public virtual ICard<V> Next
        {
            get => next;
            set => next = value;
        }

        public virtual bool Removed { get; set; }

        public virtual bool Repeated { get; set; }

        public virtual bool IsUnique
        {
            get => isUnique ??= typeof(V).IsAssignableTo(typeof(IUnique));
            set => isUnique = value;
        }

        public virtual ulong UniqueKey
        {
            get => Key;
            set => Key = value;
        }

        public virtual V UniqueObject
        {
            get => value;
            set => this.value = value;
        }

        public virtual ulong UniqueType
        {
            get
            {
                if (IsUnique)
                {
                    var uniqueValue = (IUnique)UniqueObject;
                    if (uniqueValue.UniqueType == 0)
                        uniqueValue.UniqueType = typeof(V).UniqueKey32();
                    return uniqueValue.UniqueType;
                }
                return typeof(V).UniqueKey32();
            }
            set
            {
                if (IsUnique)
                {
                    var uniqueValue = (IUnique)UniqueObject;
                    uniqueValue.UniqueType = value;
                }
            }
        }

        public virtual V Value
        {
            get => value;
            set => this.value = value;
        }

        public virtual ulong CompactKey()
        {
            return (IsUnique) ? ((IUnique)UniqueObject).UniqueKey : Key;
        }

        public virtual int CompareTo(ICard<V> other)
        {
            return (int)(Key - other.Key);
        }

        public virtual int CompareTo(IUnique other)
        {
            return (int)(Key - other.UniqueKey);
        }

        public abstract int CompareTo(object other);

        public virtual int CompareTo(ulong key)
        {
            return (int)(Key - key);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public virtual bool Equals(ICard<V> y)
        {
            return this.Equals(y.Key);
        }

        public virtual bool Equals(IUnique other)
        {
            return Key == other.UniqueKey;
        }

        public override abstract bool Equals(object y);

        public virtual bool Equals(ulong key)
        {
            return Key == key;
        }

        public abstract byte[] GetBytes();

        public override abstract int GetHashCode();

        public abstract byte[] GetUniqueBytes();

        public virtual Type GetUniqueType()
        {
            return typeof(V);
        }

        public abstract void Set(ICard<V> card);

        public abstract void Set(object key, V value);

        public virtual void Set(ulong key, V value)
        {
            this.value = value;
            Key = key;
        }

        public abstract void Set(V value);

        public virtual int[] UniqueOrdinals()
        {
            return null;
        }

        public virtual object[] UniqueValues()
        {
            return new object[] { Key };
        }

        public virtual ICard<V> MoveNext(ICard<V> card)
        {
            ICard<V> _card = card.Next;
            if (_card != null)
            {
                if (!_card.Removed)
                    return _card;
                return MoveNext(_card);
            }
            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Value = default(V);
                }

                disposedValue = true;
            }
        }

        IEnumerator<V> IEnumerable<V>.GetEnumerator()
        {
            return new CardSubSeries<V>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CardSubSeries<V>(this);
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
            return new CardSubSeries<V>(this);
        }
    }
}
