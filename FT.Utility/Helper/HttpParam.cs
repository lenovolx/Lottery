namespace FT.Utility.Helper
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class HttpParam : Dictionary<string, object>
    {
        public HttpParam()
        {
        }

        public HttpParam(IEnumerable<KeyValuePair<string, object>> data)
        {
            foreach (KeyValuePair<string, object> pair in data)
            {
                base.Add(pair.Key, pair.Value);
            }
        }

        public virtual string Format()
        {
            IEnumerable<string> values = from e in this select string.Format("{0}={1}", e.Key, Uri.EscapeDataString(Convert.ToString(e.Value)));
            return string.Join("&", values);
        }

        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

