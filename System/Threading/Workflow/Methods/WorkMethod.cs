namespace System.Threading.Workflow
{
    using System.Extract;
    using System.Series;
    using System.Uniques;

    public class WorkMethod : Card<IDeputy>
    {
        private ulong key;

        public WorkMethod() { }

        public WorkMethod(IDeputy value)
        {
            Value = value;
        }

        public WorkMethod(long key, IDeputy value) : base(key, value) { }

        public WorkMethod(object key, IDeputy value) : base(key.UniqueKey64(), value) { }

        public override ulong Key
        {
            get => key;
            set => key = value;
        }

        public override int CompareTo(object other)
        {
            return (int)(UniqueKey - other.UniqueKey());
        }

        public override bool Equals(object y)
        {
            return UniqueKey == y.UniqueKey64();
        }

        public override byte[] GetBytes()
        {
            return Key.GetBytes();
        }

        public override int GetHashCode()
        {
            return Key.GetBytes().BitAggregate64to32().ToInt32();
        }

        public override byte[] GetUniqueBytes()
        {
            return Key.GetBytes();
        }

        public override void Set(ICard<IDeputy> card)
        {
            Key = card.Key;
            Value = card.Value;
            Removed = false;
        }

        public override void Set(IDeputy value)
        {
            Value = value;
            Removed = false;
        }

        public override void Set(object key, IDeputy value)
        {
            Key = key.UniqueKey64();
            Value = value;
            Removed = false;
        }
    }
}
