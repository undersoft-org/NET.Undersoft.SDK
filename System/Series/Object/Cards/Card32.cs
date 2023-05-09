namespace System.Series
{
    using System.Runtime.InteropServices;
    using System.Uniques;

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class Card32<V> : CardBase<V>
    {
        private uint _key;

        public Card32() { }

        public Card32(ICard<V> value) : base(value) { }

        public Card32(object key, V value) : base(key, value) { }

        public Card32(ulong key, V value) : base(key, value) { }

        public Card32(V value) : base(value) { }

        public override ulong Key
        {
            get { return _key; }
            set { _key = (uint)value; }
        }

        public override int CompareTo(ICard<V> other)
        {
            return (int)(Key - other.Key);
        }

        public override int CompareTo(object other)
        {
            return (int)(_key - other.UniqueKey32());
        }

        public override int CompareTo(ulong key)
        {
            return (int)(Key - key);
        }

        public override bool Equals(object y)
        {
            return _key.Equals(y.UniqueKey32());
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
            return (int)_key;
        }

        public unsafe override byte[] GetUniqueBytes()
        {
            byte[] b = new byte[4];
            fixed (byte* s = b)
                *(uint*)s = _key;
            return b;
        }

        public override void Set(ICard<V> card)
        {
            this.value = card.Value;
            _key = (uint)card.Key;
        }

        public override void Set(object key, V value)
        {
            this.value = value;
            _key = key.UniqueKey32();
        }

        public override void Set(V value)
        {
            this.value = value;
            _key = value.UniqueKey32();
        }
    }
}
