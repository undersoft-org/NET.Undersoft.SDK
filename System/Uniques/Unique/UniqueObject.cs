using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Extract;
using System.Instant;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Uniques
{
    [DataContract]
    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public class UniqueObject : ValueProxy, IUniqueObject
    {
        private Uscn uniquecode;

        public UniqueObject() : this(true) { }

        public UniqueObject(bool autoId)
        {
            if (!autoId)
                return;

            uniquecode.UniqueKey = Unique.New;
            uniquecode.UniqueType = this.GetType().UniqueKey();
        }

        [Required]
        [StringLength(32)]
        [ConcurrencyCheck]
        [DataMember(Order = 0)]
        [FigureAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public virtual Uscn UniqueCode
        {
            get => uniquecode;
            set => uniquecode = value;
        }

        [FigureKey]
        [DataMember(Order = 1)]
        public virtual long Id
        {
            get => (long)uniquecode.UniqueKey;
            set => uniquecode.UniqueKey = (ulong)value;
        }

        [DataMember(Order = 2)]
        public virtual int Ordinal
        {
            get => (int)uniquecode.ValueFromXYZ(10, 25 * 1000);
            set => uniquecode.ValueToXYZ(10, 25 * 1000, (ulong)value);
        }

        [FigureIdentity]
        [NotMapped]
        public virtual int TypeKey
        {
            get => (int)UniqueType;
            set => UniqueType = (uint)value;
        }

        [FigureKey]
        [DataMember(Order = 3)]
        public virtual long SourceId { get; set; } = -1;

        [DataMember(Order = 4)]
        [FigureAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public virtual string SourceType { get; set; }

        [FigureKey]
        [DataMember(Order = 5)]
        public virtual long TargetId { get; set; } = -1;

        [DataMember(Order = 6)]
        [FigureAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public virtual string TargetType { get; set; }

        [DataMember(Order = 7)]
        [FigureAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public virtual string Label { get; set; }

        [NotMapped]
        public override int OriginKey
        {
            get { return (int)uniquecode.UniqueOrigin; }
            set { uniquecode.UniqueOrigin = (uint)value; }
        }

        [FigureKey]
        [DataMember(Order = 199)]
        [Column(Order = 199)]
        [FigureAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public virtual string CodeNumber { get; set; }

        public long AutoId()
        {
            ulong key = uniquecode.UniqueKey;
            if (key != 0)
                return (long)key;

            ulong id = Unique.New;
            uniquecode.UniqueKey = id;
            uniquecode.UniqueType = this.GetType().UniqueKey();
            return (long)id;
        }

        public int CompareTo(IUnique other)
        {
            return uniquecode.CompareTo(other);
        }

        public bool Equals(IUnique other)
        {
            return uniquecode.Equals(other);
        }

        public byte[] GetBytes()
        {
            return this.GetStructureBytes();
        }

        public byte[] GetUniqueBytes()
        {
            return uniquecode.GetUniqueBytes();
        }
    }
}
