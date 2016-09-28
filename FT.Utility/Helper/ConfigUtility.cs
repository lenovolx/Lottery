using System.IO;
using System.Xml.Serialization;

namespace FT.Utility.Helper
{
    public static class ConfigUtility<T> where T : new()
    {
        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        /// <returns></returns>
        public static T GetConfig(string configPath)
        {
            T config;
            using (var fs = new FileStream(configPath, FileMode.Open))
            {
                var xs = new XmlSerializer(typeof(T));
                config = (T)xs.Deserialize(fs);
            }
            return config;
        }
    }
}
