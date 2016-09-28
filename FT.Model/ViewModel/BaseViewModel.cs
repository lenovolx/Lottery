using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FT.Model.ViewModel
{
    public class BaseViewModel
    {
        [JsonIgnore]
        public string Cell { get; set; }
    }

    public class BaseLanguageViewModel
    {
        public BaseLanguageViewModel()
        {
            this.Language= "cn";
        }
        public string Language { get; set; }
    }
}
