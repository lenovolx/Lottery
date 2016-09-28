using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace FT.Utility.Helper
{
    public static class EnumHelper
    {
        private static Hashtable _enumDesciption = GetDescriptionContainer();

        private static void AddToEnumDescription(Type enumType)
        {
            _enumDesciption.Add(enumType, GetEnumDic(enumType));
        }

        private static string GetDescription(Type enumType, string enumText)
        {
            if (string.IsNullOrEmpty(enumText))
                return null;
            if (!_enumDesciption.ContainsKey(enumType))
                AddToEnumDescription(enumType);
            var obj2 = _enumDesciption[enumType];
            if ((obj2 == null) || string.IsNullOrEmpty(enumText))
                throw new ApplicationException("不存在枚举的描述");
            var dictionary = (Dictionary<string, string>) obj2;
            return dictionary[enumText];
        }

        private static string GetDescription(Type enumType, string enumText,string language="cn")
        {
            if (string.IsNullOrEmpty(enumText))
                return null;
            if (!_enumDesciption.ContainsKey(enumType))
                AddToEnumDescription(enumType);
            var obj2 = _enumDesciption[enumType];
            if ((obj2 == null) || string.IsNullOrEmpty(enumText))
                throw new ApplicationException("不存在枚举的描述");
            var dictionary = (Dictionary<string, string>)obj2;
            var descrit = dictionary[enumText];
            switch (language.ToLower())
            {
                case "pt":
                    descrit = descrit.Split('|')[2];
                    break;
                case "en":
                    descrit = descrit.Split('|')[1];
                    break;
                default:
                    descrit = descrit.Split('|')[0];
                    break;
            }
            return descrit;
        }

        private static Hashtable GetDescriptionContainer()
        {
            _enumDesciption = new Hashtable();
            return _enumDesciption;
        }

        private static Dictionary<string, string> GetEnumDic(Type enumType)
        {
            var dictionary = new Dictionary<string, string>();
            var fields = enumType.GetFields();
            foreach (var info in fields)
            {
                if (!info.FieldType.IsEnum) continue;
                var customAttributes = info.GetCustomAttributes(typeof(DescriptionAttribute), false);
                dictionary.Add(info.Name, ((DescriptionAttribute)customAttributes[0]).Description);
            }
            return dictionary;
        }

        /// <summary>
        /// 根据语言获取枚举对应语言描述
        /// </summary>
        /// <param name="value"></param>
        /// <param name="language">语言</param>
        /// <returns></returns>
        public static string ToDescription(this Enum value, string language = "cn")
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            var descrit = GetDescription(enumType, name);
            switch (language.ToLower())
            {
                case "pt":
                    descrit = descrit.Split('|')[2];
                    break;
                case "en":
                    descrit = descrit.Split('|')[1];
                    break;
                default:
                    descrit = descrit.Split('|')[0];
                    break;
            }
            return descrit;
        }

        public static string ToDescription(this Enum value)
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return GetDescription(enumType, name).Split('|')[0];
        }

        public static Dictionary<int, string> ToDescriptionDictionary<TEnum>()
        {
            var enumType = typeof(TEnum);
            var values = Enum.GetValues(enumType);
            return values.Cast<Enum>().ToDictionary(Convert.ToInt32, enum2 => enum2.ToDescription());
        }

        public static Dictionary<int, string> ToDictionary<TEnum>()
        {
            var enumType = typeof(TEnum);
            var values = Enum.GetValues(enumType);
            return values.Cast<Enum>().ToDictionary(Convert.ToInt32, enum2 => enum2.ToString());
        }
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj, bool perfix = true, bool onlyFlag = false)
        {
            var items = (from e in Enum.GetValues(typeof(TEnum)).Cast<TEnum>() select new { Id = Convert.ToInt32(e), Name = GetDescription(typeof(TEnum), e.ToString()) }).ToList();
            if (perfix)
            {
                items.Add(new
                {
                    Id = -99,
                    Name = "----"
                });
            }
            return new SelectList(items, "Id", "Name", enumObj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumObj"></param>
        /// <param name="language"></param>
        /// <param name="perfix"></param>
        /// <param name="onlyFlag"></param>
        /// <returns></returns>
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj, string language = "cn", bool perfix = true,
            bool onlyFlag = false)
        {
            var items = (from e in Enum.GetValues(typeof (TEnum)).Cast<TEnum>()
                select new {Id = Convert.ToInt32(e), Name = GetDescription(typeof (TEnum), e.ToString(), language)})
                .ToList();
            if (perfix)
            {
                items.Add(new
                {
                    Id = -99,
                    Name = "----"
                });
            }
            return new SelectList(items, "Id", "Name", enumObj);
        }
    }
}
