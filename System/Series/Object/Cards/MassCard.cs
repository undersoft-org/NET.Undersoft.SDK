namespace System.Series
{
    using System.Extract;
    using System.Runtime.InteropServices;
    using System.Uniques;

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class MassCard<V> : CardBase<V>
    {
        private ulong _key;

        public MassCard() : base() { }

        public MassCard(ICard<V> value) : base(value) { }

        public MassCard(object key, V value) : base(key, value) { }

        public MassCard(ulong key, V value) : base(key, value) { }

        public MassCard(V value) : base(value) { }

        public override ulong Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public override int CompareTo(ICard<V> other)
        {
            return (int)(Key - other.Key);
        }

        public override int CompareTo(object other)
        {
            return (int)(Key - other.UniqueKey64(UniqueType));
        }

        public override int CompareTo(ulong key)
        {
            return (int)(Key - key);
        }

        public override bool Equals(object y)
        {
            return Key.Equals(y.UniqueKey64(UniqueType));
        }

        public override bool Equals(ulong key)
        {
            return Key == key;
        }

        public override byte[] GetBytes()
        {
            return this.value.GetBytes();
        }

        public override int GetHashCode()
        {
            return (int)Key.UniqueKey32();
        }

        public unsafe override byte[] GetUniqueBytes()
        {
            byte[] b = new byte[8];
            fixed (byte* s = b)
                *(ulong*)s = _key;
            return b;
        }

        public override void Set(ICard<V> card)
        {
            value = card.Value;
            _key = card.Key;
        }

        public override void Set(object key, V value)
        {
            this.value = value;
            _key = key.UniqueKey64(UniqueType);
        }

        public override void Set(V value)
        {
            this.value = value;
            if (this.value is IUnique<V>)
                _key = ((IUnique<V>)value).CompactKey();
        }
    }
}
