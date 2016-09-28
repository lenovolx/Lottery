namespace FT.Utility.Helper
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    public class XmlHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filePath">xml文件路径</param>
        /// <returns></returns>
        public static object DeserializeFromXml(Type type, string filePath)
        {
            FileStream stream = null;
            object obj2;
            try
            {
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                obj2 = new XmlSerializer(type).Deserialize(stream);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return obj2;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filePath">xml文件路径</param>
        /// <returns></returns>
        public static bool SerializeToXml(object obj, string filePath)
        {
            var flag = false;
            FileStream stream = null;
            try
            {
                stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                new XmlSerializer(obj.GetType()).Serialize((Stream) stream, obj);
                flag = true;
            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return flag;
        }
        #region 反序列化

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static object Deserialize(Type type, string xml)
        {
            try
            {
                using (var sr = new StringReader(xml))
                {
                    var xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("反序列化对象{0}出错,原因:{1}", type.Name, e));
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static object Deserialize(Type type, Stream stream)
        {
            var xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }
        #endregion

        #region 序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer(Type type, object obj)
        {
            var Stream = new MemoryStream();
            var xml = new XmlSerializer(type);
            try
            {
                //序列化对象
                xml.Serialize(Stream, obj);
            }
            catch (InvalidOperationException i)
            {
                throw new Exception(string.Format("序列化对象{0}出错,原因:{1}", type.Name, i));
            }
            Stream.Position = 0;
            var sr = new StreamReader(Stream);
            var str = sr.ReadToEnd();

            sr.Dispose();
            Stream.Dispose();

            return str;
        }
        #endregion
    }
}

