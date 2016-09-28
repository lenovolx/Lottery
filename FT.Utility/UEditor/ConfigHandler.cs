using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT.Utility.UEditor
{
    public class ConfigHandler : IUEditorHandle
    {
        public ConfigHandler() : base() { }
        public Object Process()
        {
            return (Config.Items);
        }
    }
}