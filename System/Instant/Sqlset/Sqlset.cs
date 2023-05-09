using System.Series;

namespace System.Instant.Sqlset
{
    public class Sqlset<T> : CatalogBase<IVariety<T>>
    {
        public Sqlset() { }

        public Sqlset(SqlContext sqlcontext)
        {
            Context = sqlcontext;
        }
     
        public IFigures Figures { get; private set; }

        public SqlContext Context { get; }
    }

    public class Sqlset : CatalogBase<IVariety>
    {
        public Sqlset() { }

        public Sqlset(SqlContext sqlcontext)
        {
            Context = sqlcontext;
        }

        public IFigures Figures { get; private set; }

        public SqlContext Context { get; }
    }
}
