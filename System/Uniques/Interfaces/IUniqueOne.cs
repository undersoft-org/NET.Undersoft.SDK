using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Uniques;

namespace System.Uniques
{
    public interface IUniqueOne<T>
    {
        IQueryable<T> Queryable { get; }
    }
}
