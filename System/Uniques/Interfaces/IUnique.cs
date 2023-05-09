namespace System
{
    public interface IUnique : IEquatable<IUnique>, IComparable<IUnique>
    {
        ulong UniqueKey { get; set; }

        ulong UniqueType { get; set; }

        byte[] GetBytes();

        byte[] GetUniqueBytes();
    }
}
