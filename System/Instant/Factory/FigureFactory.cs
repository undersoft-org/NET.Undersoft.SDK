namespace System.Instant
{
    using Series;
    using Uniques;

    public static class FigureFactory
    {
        public static IDeck<Figure> Cache = new Catalog<Figure>();

        private static Figure Create<T>(FigureMode mode = FigureMode.Derived)
        {
            return Create(typeof(T), mode);
        }

        private static Figure Create(Type type, FigureMode mode = FigureMode.Derived)
        {
            return Create(type, type.UniqueKey32(), mode);
        }

        private static Figure Create(Type type, uint key, FigureMode mode = FigureMode.Derived)
        {
            if (!Cache.TryGet(key, out Figure figure))
            {
                Cache.Add(key, figure = new Figure(type, mode));
            }
            return figure;
        }

        public static Figure GetFigure(this object item, FigureMode mode = FigureMode.Derived)
        {
            var t = item.GetType();
            var key = t.UniqueKey32();
            if (!Cache.TryGet(key, out Figure figure))
            {
                Cache.Add(key, figure = new Figure(t, mode));
            }
            figure.Combine();
            return figure;
        }

        public static Figure GetFigure<T>(this T item, FigureMode mode = FigureMode.Derived)
        {
            var t = typeof(T);
            var key = t.UniqueKey32();
            if (!Cache.TryGet(key, out Figure figure))
            {
                Cache.Add(key, figure = new Figure(t, mode));
            }
            figure.Combine();
            return figure;
        }

        public static Figure Generate<T>(FigureMode mode = FigureMode.Derived)
        {
            var figure = Create<T>(mode);
            figure.Combine();
            return figure;
        }

        public static Figure Generate(Type type, FigureMode mode = FigureMode.Derived)
        {
            var figure = Create(type);
            figure.Combine();
            return figure;
        }

        public static Figure Generate(object item, FigureMode mode = FigureMode.Derived)
        {
            var figure = GetFigure(item);
            figure.Combine();
            return figure;
        }

        public static Figure Generate<T>(T item, FigureMode mode = FigureMode.Derived)
        {
            var figure = GetFigure<T>(item);
            figure.Combine();
            return figure;
        }

        public static IFigure ToFigure(this object item, FigureMode mode = FigureMode.Derived)
        {
            return Combine(item);
        }

        public static IFigure ToFigure<T>(this T item, FigureMode mode = FigureMode.Derived)
        {
            Type t = typeof(T);
            if (t.IsInterface)
                return Combine((object)item);

            return Combine(item);
        }

        public static IFigure ToFigure(this Type type, FigureMode mode = FigureMode.Derived)
        {
            return Combine(type.New());
        }

        public static IFigure Combine(object item, FigureMode mode = FigureMode.Derived)
        {
            var t = item.GetType();
            if (t.IsAssignableTo(typeof(IFigure)))
                return (IFigure)item;

            var key = t.UniqueKey32();
            if (!Cache.TryGet(key, out Figure figure))
                Cache.Add(key, figure = new Figure(t, mode));

            return figure.Combine();
        }

        public static IFigure Combine<T>(T item, FigureMode mode = FigureMode.Derived)
        {
            var t = typeof(T);
            if (t.IsAssignableTo(typeof(IFigure)))
                return (IFigure)item;

            var key = t.UniqueKey32();
            if (!Cache.TryGet(key, out Figure figure))
                Cache.Add(key, figure = new Figure(t, mode));

            return figure.Combine();
        }
    }
}
