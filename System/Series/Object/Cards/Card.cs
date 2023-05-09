namespace System.Series
{
    using System.Runtime.InteropServices;
    using System.Uniques;

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class Card<V> : CardBase<V>
    {
        private ulong _key;

        public Card() { }

        public Card(ICard<V> value) : base(value) { }

        public Card(object key, V value) : base(key, value) { }

        public Card(ulong key, V value) : base(key, value) { }

        public Card(V value) : base(value) { }

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
            this.value = card.Value;
            _key = card.Key;
        }

        public override void Set(object key, V value)
        {
            this.value = value;
            _key = key.UniqueKey64();
        }

        public override void Set(V value)
        {
            this.value = value;
            _key = value.UniqueKey64();
        }
    }
}
