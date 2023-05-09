using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Uniques;

namespace System.Instant
{
    [DataContract]
    [StructLayout(LayoutKind.Sequential)]
    public abstract class ValueProxy : Origin, IValueProxy
    {
        [JsonIgnore]
        [IgnoreDataMember]
        private ISleeve sleeve;

        public virtual object this[string propertyName]
        {
            get { return sleeve[propertyName]; }
            set { sleeve[propertyName] = value; }
        }
        public virtual object this[int id]
        {
            get { return sleeve[id]; }
            set { sleeve[id] = value; }
        }

        [JsonIgnore]
        [IgnoreDataMember]
        object[] IFigure.ValueArray
        {
            get => sleeve.ValueArray;
            set => sleeve.ValueArray = value;
        }

        [JsonIgnore]
        [IgnoreDataMember]
        Uscn IFigure.UniqueCode
        {
            get => sleeve.UniqueCode;
            set => sleeve.UniqueCode = value;
        }

        [JsonIgnore]
        [IgnoreDataMember]
        IRubrics IValueProxy.Rubrics => sleeve.Rubrics;

        [JsonIgnore]
        [IgnoreDataMember]
        ISleeve IValueProxy.Valuator
        {
            get => sleeve;
            set => sleeve = value;
        }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ulong UniqueKey
        {
            get => sleeve.UniqueKey;
            set => sleeve.UniqueKey = value;
        }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ulong UniqueType
        {
            get => sleeve.UniqueType;
            set => sleeve.UniqueType = value;
        }

        protected virtual void CompileValuator(Action<IValueProxy> compileAction)
        {
            compileAction.Invoke(this);
        }

        protected virtual void CompileValuator()
        {
            sleeve = SleeveFactory.Create(this.GetType(), (uint)UniqueType).Combine(this);
        }

        public virtual bool Equals(IUnique? other)
        {
            return sleeve.Equals(other);
        }

        public virtual int CompareTo(IUnique? other)
        {
            return sleeve.CompareTo(other);
        }

        public virtual byte[] GetBytes()
        {
            return sleeve.GetBytes();
        }

        public virtual byte[] GetUniqueBytes()
        {
            return sleeve.GetUniqueBytes();
        }
    }
}
