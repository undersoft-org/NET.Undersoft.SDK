namespace System.Instant
{
    using Series;
    using Uniques;

    public static class SleeveFactory
    {
        public static IDeck<Sleeve> Cache = new Catalog<Sleeve>();

        public static Sleeve Create<T>()
        {
            return Create(typeof(T));
        }

        public static Sleeve Create(Type type)
        {
            return Create(type, type.UniqueKey32());
        }

        public static Sleeve Create(Type type, uint key)
        {
            if (!Cache.TryGet(key, out Sleeve sleeve))
            {
                Cache.Add(key, sleeve = new Sleeve(type));
            }
            return sleeve;
        }

        public static Sleeve GetSleeve(this object item)
        {
            var t = item.GetType();
            var key = t.UniqueKey32();
            if (!Cache.TryGet(key, out Sleeve sleeve))
            {
                Cache.Add(key, sleeve = new Sleeve(t));
            }
            return sleeve;
        }

        public static Sleeve GetSleeve<T>(this T item)
        {
            var t = typeof(T);
            var key = t.UniqueKey32();
            if (!Cache.TryGet(key, out Sleeve sleeve))
            {
                Cache.Add(key, sleeve = new Sleeve(t));
            }
            return sleeve;
        }

        public static Sleeve Generate<T>()
        {
            var sleeve = Create<T>();
            sleeve.Combine();
            return sleeve;
        }

        public static Sleeve Generate(Type type)
        {
            var sleeve = Create(type);
            sleeve.Combine();
            return sleeve;
        }

        public static Sleeve Generate(object item)
        {
            var sleeve = GetSleeve(item);
            sleeve.Combine(item);
            return sleeve;
        }

        public static Sleeve Generate<T>(T item)
        {
            var sleeve = GetSleeve<T>(item);
            sleeve.Combine(item);
            return sleeve;
        }

        public static ISleeve ToSleeve(this object item)
        {
            return Combine(item);
        }

        public static ISleeve ToSleeve<T>(this T item)
        {
            Type t = typeof(T);
            if (t.IsInterface)
                return Combine((object)item);

            return Combine(item);
        }

        public static ISleeve ToSleeve(this Type type)
        {
            return Combine(type.New());
        }

        public static ISleeve Combine(object item)
        {
            var t = item.GetType();
            if (t.IsAssignableTo(typeof(ISleeve)))
                return (ISleeve)item;
            else if (t.IsAssignableTo(typeof(IValueProxy)))
                return ((IValueProxy)item).Valuator;

            var key = t.UniqueKey32();
            if (!Cache.TryGet(key, out Sleeve sleeve))
                Cache.Add(key, sleeve = new Sleeve(t));

            return sleeve.Combine(item);
        }

        public static ISleeve Combine<T>(T item)
        {
            var t = typeof(T);
            if (t.IsAssignableTo(typeof(ISleeve)))
                return (ISleeve)item;
            else if (t.IsAssignableTo(typeof(IValueProxy)))
                return ((IValueProxy)item).Valuator;

            var key = t.UniqueKey32();
            if (!Cache.TryGet(key, out Sleeve sleeve))
                Cache.Add(key, sleeve = new Sleeve<T>());

            return sleeve.Combine(item);
        }
    }
}
