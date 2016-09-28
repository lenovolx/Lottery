using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace FT.Utility.Helper
{
    public static class XElementExtensions
    {
        public static void MapTo(this XElement me, XElement xeTarget, string xpath)
        {
            //ElementName[ChildElmentName = value] 唯一可识别的条件判断

            string[] nodeNames = xpath.Trim('/').Split('/');  //xpath所表示的各级节点
            XElement xeParent = xeTarget;
            for (int i = 0; i < nodeNames.Length; i++)
            {
                if (nodeNames[i].Contains('['))
                {
                    string[] splitted = nodeNames[i].Split('[', ']');
                    string eleName = splitted[0];
                    string currentXPath = "/" + nodeNames.Take(i + 1).Aggregate((x, y) => x + "/" + y);
                    XElement xeXPathSelected = xeParent.XPathSelectElement(currentXPath);
                    if (xeXPathSelected == null) //未能找到指定节点
                    {
                        XElement xeNew = new XElement(eleName);
                        xeParent.Add(xeNew);
                        var splitted_criteria = splitted[1].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray(); //查询条件的表达式分解
                        xeNew.Add(new XElement(splitted_criteria[0], splitted_criteria[1].Trim('\'')));
                        xeParent = xeNew;
                    }
                    else
                    {
                        xeParent = xeXPathSelected;
                    }
                }
                else
                {
                    if (xeParent.Element(nodeNames[i]) == null)
                    {
                        XElement xeNew = new XElement(nodeNames[i]);
                        xeParent.Add(xeNew);
                        xeParent = xeNew;
                    }
                    else
                    {
                        xeParent = xeParent.Element(nodeNames[i]);
                    }
                    if (i == nodeNames.Length - 1) //叶子节点， 需要设置Element的值
                    {
                        xeParent.SetValue(me.Value);
                    }
                }

            }
        }

        //获得XElement值
        public static T GetValue<T>(this XElement me, T defaultValue)
        {
            if (me == null || me.Value.Trim() == "")
            {
                return defaultValue;
            }
            if (typeof(T) == typeof(Guid))
            {
                return (T)Convert.ChangeType(new Guid(me.Value), typeof(T));
            }
            else if (typeof(T) == typeof(int))
            {
                return (T)Convert.ChangeType(int.Parse(me.Value), typeof(T));
            }
            else if (typeof(T) == typeof(DateTime))
            {
                return (T)Convert.ChangeType(DateTime.Parse(me.Value), typeof(T));
            }
            else if (typeof(T) == typeof(decimal))
            {
                return (T)Convert.ChangeType(decimal.Parse(me.Value), typeof(T));
            }
            else if (typeof(T) == typeof(float))
            {
                return (T)Convert.ChangeType(float.Parse(me.Value), typeof(T));
            }
            else if (typeof(T) == typeof(bool))
            {
                return (T)Convert.ChangeType(bool.Parse(me.Value), typeof(T));
            }
            else
            {
                return (T)Convert.ChangeType(me.Value.Trim(), typeof(T));
            }
        }

        /// <summary>
        /// 获取xml节点对象值
        /// </summary>
        /// <param name="xe">节点对象</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetValue(this XElement xe, string defaultValue = "")
        {
            if (xe != null)
            {
                return xe.Value.Trim();
            }
            else
            {
                return defaultValue;
            }
        }

        public static string GetValue(this XAttribute me, string defaultValue = "")
        {
            if (me != null)
            {
                return me.Value;
            }
            else
            {
                return defaultValue;
            }
        }

        public static string GetNotEmptyFormattedValue(this XElement me, string format)
        {
            if (me == null || me.Value == "")
            {
                return "";
            }
            return string.Format(format, me.Value.Trim());
        }

        //从另外一个Element中获取相匹配的值
        public static void SetMatchingElementValues(this XElement me, XElement source, params string[] exceptElementNames)
        {
            if (me == null)
                return;
            foreach (XElement x in me.Elements())
            {
                if (exceptElementNames != null && exceptElementNames.Contains(x.Name.LocalName))
                {
                    continue;
                }
                if (source.Element(x.Name.LocalName) != null)
                {
                    x.SetValue(source.Element(x.Name.LocalName).Value);
                }
            }
        }

        public static void RemoveSubElement(this XElement me, string subElementName)
        {
            if (me.Element(subElementName) != null)
            {
                me.Element(subElementName).Remove();
            }
        }

        /// <summary>
        /// 拿一个业务对象与相应的对象比对，获取服务器端能识别的比对格式
        /// </summary>
        /// <param name="me"></param>
        /// <param name="xeCounterpart">副本</param>
        /// <returns></returns>
        public static XElement CompareTo(this XElement me, XElement xeCounterpart, XElement xeCounterpartDesc, params string[] reserverdElementNames)
        {
            if (me == null)
                return null;

            XElement xeDiff = new XElement(me.Name, new XAttribute("flag", "update"));
            foreach (XElement x in me.Elements())  //只限于当前表的字段
            {
                if (xeCounterpart.Element(x.Name) != null)
                {
                    if (x.Element("NewValue") != null || x.Value != xeCounterpart.Element(x.Name).Value)
                    {
                        var desc = "";
                        xeDiff.Add(new XElement(x.Name, new XElement("Desc", desc), new XElement("NewValue", x.Value), new XElement("OldValue", xeCounterpart.Element(x.Name).Value)));
                    }
                }
            }
            if (xeDiff.HasElements)
            {
                foreach (string x in reserverdElementNames)
                {
                    if (xeDiff.Element(x) == null) //如果保留的字段已经是更新字段，就不必添加了
                    {
                        xeDiff.Add(xeCounterpart.Element(x));
                    }
                }
                return xeDiff;
            }
            else
            {
                return null;
            }
        }

      
        
    }
}
