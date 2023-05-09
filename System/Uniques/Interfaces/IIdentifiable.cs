using System.Collections.Specialized;
using System.Instant;
using System.Uniques;

namespace RadicalR
{
    public interface IIdentifiable : IUniqueObject, IEquatable<IIdentifiable>, IComparable<IIdentifiable>, IEquatable<BitVector32>,
                                     IEquatable<DateTime>, IEquatable<ISerialNumber>, IValueProxy, IOrigin
    {
        bool Obsolete { get; set; }
        byte Priority { get; set; }
        bool Inactive { get; set; }
        bool Locked { get; set; }

        byte Flags { get; set; }

        DateTime Time { get; set; }

        void SetFlag(ushort position);
        void ClearFlag(ushort position);
        void SetFlag(bool flag, ushort position);
        bool GetFlag(ushort position);

        long AutoId();
        long SetId(long id);
        long SetId(object id);

        IIdentifiable Sign(object id);
        IIdentifiable Sign();
        IIdentifiable Stamp();

        TEntity Sign<TEntity>(object id) where TEntity : class, IIdentifiable;
        TEntity Sign<TEntity>() where TEntity : class, IIdentifiable;
        TEntity Stamp<TEntity>() where TEntity : class, IIdentifiable;

        TEntity Sign<TEntity>(TEntity entity, object id) where TEntity : class, IIdentifiable;
        TEntity Sign<TEntity>(TEntity entity) where TEntity : class, IIdentifiable;
        TEntity Stamp<TEntity>(TEntity entity) where TEntity : class, IIdentifiable;

        byte GetPriority();
        byte SetPriority(byte priority);
        byte ComparePriority(IIdentifiable entity);
    }
}