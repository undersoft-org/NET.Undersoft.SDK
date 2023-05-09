using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Extract;
using System.Instant;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Uniques;

namespace System.Series
{
    public class Listing<TDto> : KeyedCollection<long, TDto>, IFindable where TDto : IUniqueObject
    {
        protected override long GetKeyForItem(TDto item)
        {
            return (item.Id == 0) ? (long)item.AutoId() : item.Id;
        }
         
        public TDto Single
        {
            get => this.FirstOrDefault();
        }

        public object this[object key]
        {
            get
            {
                TryGetValue((long)(key.UniqueKey()), out TDto result);
                return result;
            }
            set
            {
                Dictionary[(long)(key.UniqueKey())] = (TDto)value;
            }
        }
    }
}
