namespace System.Instant.Mathset
{
    using System.Runtime.InteropServices;
    using System.Series;
    using System.Uniques;

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class MathRubricCard : Card<MathRubric>
    {
        private ulong _key;

        public MathRubricCard() { }

        public MathRubricCard(ICard<MathRubric> value) : base(value) { }

        public MathRubricCard(long key, MathRubric value) : base(key, value) { }

        public MathRubricCard(MathRubric value) : base(value) { }

        public MathRubricCard(object key, MathRubric value) : base(key.UniqueKey64(), value) { }

        public override ulong Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public override int CompareTo(ICard<MathRubric> other)
        {
            return (int)(_key - other.Key);
        }

        public override int CompareTo(object other)
        {
            return (int)(_key - other.UniqueKey64());
        }

        public override int CompareTo(ulong key)
        {
            return (int)(_key - key);
        }

        public override bool Equals(object y)
        {
            return _key.Equals(y.UniqueKey64());
        }

        public override bool Equals(ulong key)
        {
            return _key == key;
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
            byte[] b = new byte[8];
            fixed (byte* s = b)
                *(ulong*)s = _key;
            return b;
        }

        public override void Set(ICard<MathRubric> card)
        {
            this.value = card.Value;
            _key = card.Key;
        }

        public override void Set(MathRubric value)
        {
            this.value = value;
            _key = value.UniqueKey;
        }

        public override void Set(object key, MathRubric value)
        {
            this.value = value;
            _key = key.UniqueKey64();
        }
    }
}
