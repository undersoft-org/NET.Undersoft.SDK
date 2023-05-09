namespace System.Series
{
    using System.Instant;
    using System.Runtime.InteropServices;
    using System.Uniques;

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class TracedCard<V> : CardBase<V> where V : class, ITraceable
    {
        private ulong _key;
        private Variety<V> _proxy;

        public TracedCard() { }

        public TracedCard(ICard<V> value) : base(value) { }

        public TracedCard(object key, V value) : base(key, value) { }

        public TracedCard(ulong key, V value) : base(key, value) { }

        public TracedCard(V value) : base(value) { }

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
            return (int)(Key - other.UniqueKey64());
        }

        public override int CompareTo(ulong key)
        {
            return (int)(Key - key);
        }

        public override bool Equals(object y)
        {
            return Key.Equals(y.UniqueKey64());
        }

        public override bool Equals(ulong key)
        {
            return Key == key;
        }

        public override byte[] GetBytes()
        {
            return GetUniqueBytes();
        }

        public override int GetHashCode()
        {
            return (int)Key;
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
            if (this.value == null)
            {
                _proxy = new Variety<V>(card.Value);
                value = _proxy.Preset;
            }
            else
            {
                value.PatchTo(_proxy.EntryProxy);
            }

            _key = card.Key;
        }

        public override void Set(object key, V value)
        {
            if (this.value == null)
            {
                _proxy = new Variety<V>(value);
                this.value = _proxy.Entry;
            }
            else
            {
                value.PatchTo(_proxy.EntryProxy);
            }

            _key = key.UniqueKey64();
        }

        public override void Set(V value)
        {
            Set(value.UniqueKey64(), value);
        }

        public override V UniqueObject
        {
            get => base.UniqueObject;
            set => value.PatchTo(_proxy.EntryProxy);
        }

        public override V Value
        {
            get => base.Value;
            set => value.PatchTo(_proxy.EntryProxy);
        }
    }
}
