using System;

namespace FT.Utility.Helper
{
    /// <summary>
    /// 基元类型帮助类
    /// </summary>
    public static class BaseTypeHelper
    {
        #region 常用类型的TryParse
        /// <summary>
        /// 将Object转换为Int
        /// </summary>
        /// <param name="value"></param>
        /// <returns>成功返回具体值，失败返回0。</returns>
        public static Int32 ToInt(this Object value)
        {
            if (value == null) return 0;
            Int32 i = 0;
            if (Int32.TryParse(value.ToString(), out i))
                return i;
            try
            {
                return Convert.ToInt32(value.ToString());
            }
            catch (Exception)
            {
                return 0;
            }
        }
        /// <summary>
        /// 将Object转换为Int。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>成功返回具体值，失败则返回默认值。</returns>
        public static Int32 ToInt(this Object value, int defaultValue)
        {
            if (value == null) return defaultValue;
            Int32 i = defaultValue;
            if (Int32.TryParse(value.ToString(), out i))
                return i;
            try
            {
                return Convert.ToInt32(value.ToString());
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 将Object转换为Int
        /// </summary>
        /// <param name="value"></param>
        /// <returns>成功则返回具体值，失败则返回null。</returns>
        public static Int32? ToIntOrNull(this Object value)
        {
            if (value == null) return null;
            Int32 i = 0;
            if (Int32.TryParse(value.ToString(), out i))
                return i;
            else
                return null;
        }
        /// <summary>
        /// 将Object转换为Guid
        /// </summary>
        /// <param name="value"></param>
        /// <returns>成功返回具体值，失败返回Empty。</returns>
        public static Guid ToGuid(this Object value)
        {
            if (value == null) return Guid.Empty;
            Guid guid = Guid.Empty;
            Guid.TryParse(value.ToString(), out guid);
            return guid;
        }
        /// <summary>
        /// 将Object转换为Guid
        /// </summary>
        /// <param name="value"></param>
        /// <returns>成功返回具体值，失败返回null。</returns>
        public static Guid? ToGuidOrNull(this Object value)
        {
            if (value == null) return null;
            Guid guid = Guid.Empty;
            if (Guid.TryParse(value.ToString(), out guid))
                return guid;
            else
                return null;
        }
        /// <summary>
        /// 将Object转化为Double
        /// </summary>
        /// <param name="value"></param>
        /// <returns>成功返回具体值，失败返回0</returns>
        public static Double ToDouble(this Object value)
        {
            if (value == null) return 0;
            Double db = 0;
            Double.TryParse(value.ToString(), out db);
            return db;
        }
        /// <summary>
        /// 将Object转化为Double
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>成功返回具体值，失败返回0</returns>
        public static Double ToDouble(this Object value, double defaultValue)
        {
            if (value == null) return defaultValue;
            Double db = defaultValue;
            Double.TryParse(value.ToString(), out db);
            return db;
        }
        /// <summary>
        /// 将Object转化为Double
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits">处理小数位数</param>
        /// <returns>成功返回具体值，失败返回0</returns>
        public static Double ToDouble(this Object value, int digits)
        {
            return Math.Round(value.ToDouble(), digits);
        }
        /// <summary>
        /// 将Object转化为可空Double
        /// </summary>
        /// <param name="value"></param>
        /// <returns>成功返回具体值，失败返回Null</returns>
        public static Double? ToDoubleOrNull(this Object value)
        {
            if (value == null) return null;
            Double db = 0;
            if (Double.TryParse(value.ToString(), out db))
                return db;
            else
                return null;
        }
        /// <summary>
        /// 将Object转化为Decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns>成功返回具体值，失败返回0</returns>
        public static Decimal ToDecimal(this Object value)
        {
            if (value == null) return 0;
            Decimal db = 0;
            Decimal.TryParse(value.ToString(), out db);
            return db;
        }
        /// <summary>
        /// 将Object转化为Decimal
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultVallue"></param>
        /// <returns>成功返回具体值，失败返回0</returns>
        public static Decimal ToDecimal(this Object value, Decimal defaultVallue)
        {
            if (value == null) return 0;
            Decimal db = defaultVallue;
            Decimal.TryParse(value.ToString(), out db);
            return db;
        }
        /// <summary>
        /// 将Object转化为可空Decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns>成功返回具体值，失败返回Null</returns>
        public static Decimal? ToDecimalOrNull(this Object value)
        {
            if (value == null) return null;
            Decimal db = 0;
            if (Decimal.TryParse(value.ToString(), out db))
                return db;
            else
                return null;
        }
        /// <summary>
        ///  将Object转化为DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <returns>成功返回具体值，失败返回DateTime.MinValue</returns>
        public static DateTime ToDateTime(this Object value)
        {
            if (value == null) return DateTime.MinValue;
            DateTime dt = DateTime.MinValue;
            DateTime.TryParse(value.ToString(), out dt);
            return dt;
        }
        /// <summary>
        ///  将Object转化为DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <returns>成功返回具体值，失败返回DateTime.MinValue</returns>
        public static DateTime ToDateTime(this Object value, DateTime defaultDateTime)
        {
            if (value == null) return DateTime.MinValue;
            DateTime dt = defaultDateTime;
            DateTime.TryParse(value.ToString(), out dt);
            return dt;
        }
        /// <summary>
        ///  将Object转化为可空DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <returns>成功返回具体值，失败返回Null</returns>
        public static DateTime? ToDateTimeOrNull(this Object value)
        {
            if (value == null) return DateTime.MinValue;
            DateTime dt = DateTime.MinValue;
            if (DateTime.TryParse(value.ToString(), out dt))
                return dt;
            else
                return null;
        }
        /// <summary>
        /// 将Object转化为Boolean
        /// </summary>
        /// <param name="value"></param>
        /// <returns>成功返回具体值，失败返回false</returns>
        public static Boolean ToBoolean(this Object value)
        {
            if (value == null) return false;
            Boolean bl = false;
            Boolean.TryParse(value.ToString(), out bl);
            return bl;
        }
        /// <summary>
        /// 将Object转化为Boolean
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns>成功返回具体值，失败返回false</returns>
        public static Boolean ToBoolean(this Object value, bool defaultValue)
        {
            if (value == null) return false;
            Boolean bl = defaultValue;
            Boolean.TryParse(value.ToString(), out bl);
            return bl;
        }
        /// <summary>
        /// 将Object转换为可空boolean
        /// </summary>
        /// <param name="value"></param>
        /// <returns>成功返回具体值，失败返回Null</returns>
        public static Boolean? ToBooleanOrNull(this Object value)
        {
            if (value == null) return null;
            Boolean bl = false;
            if (Boolean.TryParse(value.ToString(), out bl))
                return bl;
            else
                return null;
        }
        /// <summary>
        /// 将特殊字符串转化为Boolean
        /// </summary>
        /// <param name="value">[0,1,是,否,yes,no,true,false]</param>
        /// <returns>返回bool</returns>
        public static Boolean ToBooleanBySpecicalString(this String value)
        {
            if (String.IsNullOrEmpty(value)) return false;
            switch (value.Trim().ToLower())
            {
                case "0":
                    return false;
                case "1":
                    return true;
                case "是":
                    return true;
                case "否":
                    return false;
                case "yes":
                    return true;
                case "no":
                    return false;
                case "true":
                    return true;
                case "false":
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 泛型转换
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">数据</param>
        /// <returns>成功返回具体值，失败返回default(T)</returns>
        public static T To<T>(this Object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return default(T);
            Type type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            try
            {
                if (type.Name.ToLower() == "guid")
                    return (T)(object)new Guid(value.ToString());
                if (value is IConvertible)
                    return (T)Convert.ChangeType(value, type);
                return (T)value;
            }
            catch
            {
                return default(T);
            }
        }

        #endregion
    }
}