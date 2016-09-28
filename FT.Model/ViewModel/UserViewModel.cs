using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FT.Model.ViewModel
{
    [JsonObject(MemberSerialization.OptOut)]
    public class UserViewModel : BaseViewModel
    {
        public long Id { get; set; }
        public string LoginName { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public string PasswordSalt { get; set; }
        public decimal? BalanceAmount { get; set; }
        [JsonIgnore]
        public int? Status { get; set; }
        [JsonIgnore]
        public string Phone { get; set; }
        [JsonIgnore]
        public string email { get; set; }
        [JsonIgnore]
        public DateTime? CreateDate { get; set; }
        [JsonIgnore]
        public string Type { get; set; }
        [JsonIgnore]
        public string Level { get; set; }
        [JsonIgnore]
        public string StatusName { get; set; }
        public int? TypeName { get; set; }
        public int? LevelName { get; set; }
        [JsonIgnore]
        public string ParentPath { get; set; }
        [JsonIgnore]
        public decimal? CreditLimit { get; set; }
        [JsonIgnore]
        public int? ReturnRate { get; set; }
    }
}
