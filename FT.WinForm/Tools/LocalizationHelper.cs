using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FT.WinForm
{
    public class LocalizationHelper
    {
        public static void ApplyLanguage(string culture)
        {
            string requestedCulture = string.Format("/Resources/Language/String_{0}.xaml", culture);

            Collection<ResourceDictionary> mergedDicts = Application.Current.Resources.MergedDictionaries;
            //判断app是否包含本地化资源文件
            ResourceDictionary resourceDictionary = mergedDicts.FirstOrDefault(d => d.Source.OriginalString.Contains("/Resources/Language/String_"));
            if (resourceDictionary != null)
            {
                //如果有了，就切换资源
                resourceDictionary.Source = new Uri(requestedCulture, UriKind.Relative);
            }
            else
            {
                //如果没有，就重新加载默认的
                resourceDictionary = new ResourceDictionary();
                try
                {
                    resourceDictionary.Source = new Uri(requestedCulture, UriKind.Relative);
                }
                catch { }
                if (resourceDictionary.Source == null)
                {
                    resourceDictionary.Source = new Uri("/Resources/Language/String_cn.xaml", UriKind.Relative);
                }
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
        public static string GetRes(string reskey)
        {
            string res = "";
            try
            {
                res = Application.Current.FindResource(reskey).ToString();
            }
            catch (Exception e)
            {
                return res;
            }
            return res;
        }
    }
}
