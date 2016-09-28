namespace FT.Utility.Helper
{
    using System;
    public class TypeHelper
    {
        public static bool ObjectToBool(object o)
        {
            return ObjectToBool(o, false);
        }

        public static bool ObjectToBool(object o, bool defaultValue)
        {
            if (o != null)
            {
                return StringToBool(o.ToString(), defaultValue);
            }
            return defaultValue;
        }

        public static DateTime ObjectToDateTime(object o)
        {
            return ObjectToDateTime(o, DateTime.Now);
        }

        public static DateTime ObjectToDateTime(object o, DateTime defaultValue)
        {
            if (o != null)
            {
                return StringToDateTime(o.ToString(), defaultValue);
            }
            return defaultValue;
        }

        public static decimal ObjectToDecimal(object o)
        {
            return ObjectToDecimal(o, 0M);
        }

        public static decimal ObjectToDecimal(object o, decimal defaultValue)
        {
            if (o != null)
            {
                return StringToDecimal(o.ToString(), defaultValue);
            }
            return defaultValue;
        }

        public static int ObjectToInt(object o)
        {
            return ObjectToInt(o, 0);
        }

        public static int ObjectToInt(object o, int defaultValue)
        {
            if (o != null)
            {
                return StringToInt(o.ToString(), defaultValue);
            }
            return defaultValue;
        }

        public static bool StringToBool(string s, bool defaultValue)
        {
            if (s == "false")
            {
                return false;
            }
            return ((s == "true") || defaultValue);
        }

        public static DateTime StringToDateTime(string s)
        {
            return StringToDateTime(s, DateTime.Now);
        }

        public static DateTime StringToDateTime(string s, DateTime defaultValue)
        {
            DateTime time;
            if (!string.IsNullOrWhiteSpace(s) && DateTime.TryParse(s, out time))
            {
                return time;
            }
            return defaultValue;
        }

        public static decimal StringToDecimal(string s)
        {
            return StringToDecimal(s, 0M);
        }

        public static decimal StringToDecimal(string s, decimal defaultValue)
        {
            decimal num;
            if (!string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, out num))
            {
                return num;
            }
            return defaultValue;
        }

        public static int StringToInt(string s)
        {
            return StringToInt(s, 0);
        }

        public static int StringToInt(string s, int defaultValue)
        {
            int num;
            if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out num))
            {
                return num;
            }
            return defaultValue;
        }

        public static bool ToBool(string s)
        {
            return StringToBool(s, false);
        }
    }
}

