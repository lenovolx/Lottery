using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT.Utility.UEditor
{
    public class NotSupportedHandler : IUEditorHandle
    {
        public NotSupportedHandler() { }

        public object Process()
        {
            return (new
            {
                state = "action 参数为空或者 action 不被支持。"
            });
        }
    }
}