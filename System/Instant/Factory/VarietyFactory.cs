namespace System.Instant
{
    public static class VarietyFactory
    {
        public static E PatchTo<T, E>(this T item, E target, IDeputy traceChanges = null)
            where T : class
            where E : class
        {
            return new Variety<T>(item, traceChanges).Patch(target);
        }

        public static object PatchTo(this object item, object target, IDeputy traceChanges = null)
        {
            return new Variety(item, traceChanges).Patch(target);
        }

        public static E PatchTo<T, E>(this T item, IDeputy traceChanges = null)
            where T : class
            where E : class
        {
            return new Variety<T>(item, traceChanges).Patch<E>();
        }

        public static object PatchSelf(this object item, IDeputy traceChanges = null)
        {
            return new Variety(item, traceChanges).PatchSelf();
        }

        public static E PutTo<T, E>(this T item, E target, IDeputy traceChanges = null)
            where T : class
            where E : class
        {
            return new Variety<T>(item, traceChanges).Put(target);
        }

        public static object PutTo(this object item, object target, IDeputy traceChanges = null)
        {
            return new Variety(item, traceChanges).Put(target);
        }

        public static E PutTo<T, E>(this T item, IDeputy traceChanges = null)
            where T : class
            where E : class
        {
            return new Variety<T>(item, traceChanges).Put<E>();
        }
    }
}
