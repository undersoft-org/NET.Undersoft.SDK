namespace System
{
    public interface IUnique<V> : IUnique
    {
        V UniqueObject { get; set; }

        int[] UniqueOrdinals();

        ulong CompactKey();

        object[] UniqueValues();
    }
}
