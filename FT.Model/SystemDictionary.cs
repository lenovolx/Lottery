

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class SystemDictionary
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public string DictionaryName { get; set; }
        public string DictionaryNameEn { get; set; }
        public string DictionaryNamePt { get; set; }
        public int Sort { get; set; }
        public LockEnum IsLock { get; set; }
        public double DictionaryValue { get; set; }
    }
}
