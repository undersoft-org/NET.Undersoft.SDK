using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace System.Instant
{
    [DataContract]
    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public abstract class Origin : IOrigin
    {
        [JsonIgnore]
        public virtual int OriginKey { get; set; }

        [StringLength(32)]
        [DataMember(Order = 8)]
        [FigureAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public virtual string OriginName { get; set; }

        [Column(TypeName = "timestamp")]
        [DataMember(Order = 9)]
        [FigureAs(UnmanagedType.I8, SizeConst = 8)]
        public virtual DateTime Modified { get; set; } = DateTime.Now;

        [StringLength(32)]
        [DataMember(Order = 10)]
        [FigureAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public virtual string Modifier { get; set; }

        [Column(TypeName = "timestamp")]
        [DataMember(Order = 11)]
        [FigureAs(UnmanagedType.I8, SizeConst = 8)]
        public virtual DateTime Created { get; set; }

        [StringLength(32)]
        [DataMember(Order = 12)]
        [FigureAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public virtual string Creator { get; set; }
    }
}
