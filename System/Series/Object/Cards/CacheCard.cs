using System.Extract;
using System.Logs;
using System.Runtime.InteropServices;
using System.Uniques;

namespace System.Series
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class CacheCard<V> : CardBase<V>
    {
        private ulong _key;
        private TimeSpan duration;
        private DateTime expiration;
        private IDeputy callback;

        public void SetupExpiration(TimeSpan? lifetime, IDeputy callback = null)
        {
            duration = (lifetime != null) ? lifetime.Value : TimeSpan.FromMinutes(15);
            expiration = Log.Clock + duration;
            this.callback = callback;
        }

        private void setupExpiration()
        {
            expiration = Log.Clock + duration;
        }

        public CacheCard() : base()
        {
            SetupExpiration(TimeSpan.FromMinutes(15));
        }

        public CacheCard(ICard<V> value, TimeSpan? lifeTime = null, IDeputy deputy = null)
            : base(value)
        {
            SetupExpiration(lifeTime, deputy);
        }

        public CacheCard(object key, V value, TimeSpan? lifeTime = null, IDeputy deputy = null)
            : base(key, value)
        {
            SetupExpiration(lifeTime, deputy);
        }

        public CacheCard(ulong key, V value, TimeSpan? lifeTime = null, IDeputy deputy = null)
            : base(key, value)
        {
            SetupExpiration(lifeTime, deputy);
        }

        public CacheCard(V value, TimeSpan? lifeTime = null, IDeputy deputy = null) : base(value)
        {
            SetupExpiration(lifeTime, deputy);
        }

        public override ulong Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public override V Value
        {
            get
            {
                if (Log.Clock > expiration)
                {
                    Removed = true;
                    if (callback != null)
                        _ = callback.ExecuteAsync(value);
                    return default(V);
                }
                return value;
            }
            set
            {
                setupExpiration();
                this.value = value;
            }
        }

        public override V UniqueObject
        {
            get => this.Value;
            set => this.Value = value;
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
            Value = card.Value;
            _key = card.Key;
        }

        public override void Set(object key, V value)
        {
            this.Value = value;
            _key = key.UniqueKey64(UniqueType);
        }

        public override void Set(V value)
        {
            this.Value = value;
            if (this.value is IUnique<V>)
                _key = ((IUnique<V>)value).CompactKey();
        }
    }
}
