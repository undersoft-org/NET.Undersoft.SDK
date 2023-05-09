using System.Series;

namespace System.Instant.Linking
{
    public static class Linker
    {
        public static MassCache<IUnique> Maps = new MassCache<IUnique>();

        public static Catalog<Type> Types = new Catalog<Type>();

    }
}
