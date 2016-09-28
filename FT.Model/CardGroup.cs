

namespace FT.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class CardGroup
    {
        public CardGroup()
        {
            this.Card = new HashSet<Card>();
        }
    
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public int Amount { get; set; }
        public int Number { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual ICollection<Card> Card { get; set; }
    }
}
