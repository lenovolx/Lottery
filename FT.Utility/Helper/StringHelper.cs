namespace FT.Utility.Helper
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    public class StringHelper
    {
        public static int GetStringLength(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return Encoding.Default.GetBytes(s).Length;
            }
            return 0;
        }

        public static int IndexOf(string s, int order)
        {
            return IndexOf(s, '-', order);
        }

        public static int IndexOf(string s, char c, int order)
        {
            int length = s.Length;
            for (int i = 0; i < length; i++)
            {
                if (c == s[i])
                {
                    if (order == 1)
                    {
                        return i;
                    }
                    order--;
                }
            }
            return -1;
        }

        public static string[] SplitString(string sourceStr)
        {
            return SplitString(sourceStr, ",");
        }

        public static string[] SplitString(string sourceStr, string splitStr)
        {
            if (string.IsNullOrEmpty(sourceStr) || string.IsNullOrEmpty(splitStr))
            {
                return new string[0];
            }
            if (sourceStr.IndexOf(splitStr) == -1)
            {
                return new string[] { sourceStr };
            }
            if (splitStr.Length == 1)
            {
                return sourceStr.Split(new char[] { splitStr[0] });
            }
            return Regex.Split(sourceStr, Regex.Escape(splitStr), RegexOptions.IgnoreCase);
        }

        public static string SubString(string sourceStr, int length)
        {
            return SubString(sourceStr, 0, length);
        }

        public static string SubString(string sourceStr, int startIndex, int length)
        {
            if (!string.IsNullOrEmpty(sourceStr))
            {
                if (sourceStr.Length >= (startIndex + length))
                {
                    return sourceStr.Substring(startIndex, length);
                }
                return sourceStr.Substring(startIndex);
            }
            return "";
        }

        public static string Trim(string sourceStr, string trimStr)
        {
            return Trim(sourceStr, trimStr, true);
        }

        public static string Trim(string sourceStr, string trimStr, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(sourceStr))
            {
                return string.Empty;
            }
            if (!string.IsNullOrEmpty(trimStr))
            {
                if (sourceStr.StartsWith(trimStr, ignoreCase, CultureInfo.CurrentCulture))
                {
                    sourceStr = sourceStr.Remove(0, trimStr.Length);
                }
                if (sourceStr.EndsWith(trimStr, ignoreCase, CultureInfo.CurrentCulture))
                {
                    sourceStr = sourceStr.Substring(0, sourceStr.Length - trimStr.Length);
                }
            }
            return sourceStr;
        }

        public static string TrimEnd(string sourceStr, string trimStr)
        {
            return TrimEnd(sourceStr, trimStr, true);
        }

        public static string TrimEnd(string sourceStr, string trimStr, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(sourceStr))
            {
                return string.Empty;
            }
            if (!(!string.IsNullOrEmpty(trimStr) && sourceStr.EndsWith(trimStr, ignoreCase, CultureInfo.CurrentCulture)))
            {
                return sourceStr;
            }
            return sourceStr.Substring(0, sourceStr.Length - trimStr.Length);
        }

        public static string TrimStart(string sourceStr, string trimStr)
        {
            return TrimStart(sourceStr, trimStr, true);
        }

        public static string TrimStart(string sourceStr, string trimStr, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(sourceStr))
            {
                return string.Empty;
            }
            if (!(!string.IsNullOrEmpty(trimStr) && sourceStr.StartsWith(trimStr, ignoreCase, CultureInfo.CurrentCulture)))
            {
                return sourceStr;
            }
            return sourceStr.Remove(0, trimStr.Length);
        }

        
    }
}

