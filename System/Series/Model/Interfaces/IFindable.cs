using System.Collections;
using System.Collections.Generic;

namespace System.Series
{
    public interface IFindable<V> : IFindable, IEnumerable<V>, IList<V>
    {
        new V this[object key] { get; set; }
    }

    public interface IFindable : IEnumerable
    {
        object this[object key] { get; set; }
    }
}
