using System.Uniques;

namespace System
{
    public interface IUniqueCode : IUnique
    {
        Uscn UniqueCode { get; }
    }
}
