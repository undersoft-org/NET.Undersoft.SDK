namespace System.Instant
{
    public static class ValueOfFactory
    {
        public static R ValueOf<T, R>(this T item, string name) where T : class
        {
            return (R)SleeveFactory.Combine(item)[name];
        }

        public static R ValueOf<R>(this object item, string name)
        {
            return (R)SleeveFactory.Combine(item)[name];
        }

        public static object ValueOf(this object item, string name)
        {
            return SleeveFactory.Combine(item)[name];
        }

        public static R ValueOf<T, R>(this T item, int index) where T : class
        {
            return (R)SleeveFactory.Combine(item)[index];
        }

        public static R ValueOf<R>(this object item, int index)
        {
            return (R)SleeveFactory.Combine(item)[index];
        }

        public static object ValueOf(this object item, int index)
        {
            return SleeveFactory.Combine(item)[index];
        }

        public static R ValueOf<T, R>(this T item, string name, R value) where T : class
        {
            return (R)(SleeveFactory.Combine(item)[name] = value);
        }

        public static R ValueOf<R>(this object item, string name, R value)
        {
            return (R)(SleeveFactory.Combine(item)[name] = value);
        }

        public static object ValueOf(this object item, string name, object value)
        {
            return (SleeveFactory.Combine(item)[name] = value);
        }

        public static R ValueOf<T, R>(this T item, int index, R value) where T : class
        {
            return (R)(SleeveFactory.Combine(item)[index] = value);
        }

        public static R ValueOf<R>(this object item, int index, R value)
        {
            return (R)(SleeveFactory.Combine(item)[index] = value);
        }

        public static object ValueOf(this object item, int index, object value)
        {
            return (SleeveFactory.Combine(item)[index] = value);
        }
    }
}
