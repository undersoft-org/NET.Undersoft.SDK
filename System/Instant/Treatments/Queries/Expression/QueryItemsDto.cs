using System;
using System.Collections.Generic;

namespace System.Instant.Treatments
{
    [Serializable]
    public class QueryItems
    {
        #region Properties

        public List<FilterItem> Filter { get; set; } = new();

        public List<SortItem> Sort { get; set; } = new(); 

        #endregion

    }
}
