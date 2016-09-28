using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Xml.Linq;

namespace FT.WinForm
{
    [ValueConversion(typeof(string), typeof(string))]
    public class CommonConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.ToString() == "Reverse")
            {
                return value.ToString().ToLower() == "false";
            }

            switch (parameter.ToString())
            {
                case "BoolEN":
                    if (value.ToString() == "1")
                        return "true";
                    else
                        return "false";
                case "BoolEN2":
                    if (value.ToString() == "0")
                        return "true";
                    else
                        return "false";
                case "BoolCN":
                    if (value.ToString() == "1")
                        return "是";
                    else
                        return "否";
                case "BoolCN1":
                    if (value.ToString() == "1")
                        return "是";
                    else
                        return "否";
                case "BoolCNNULL":
                    if (value.ToString() == "1")
                        return "是";
                    else
                        return "";
                case "BoolCNNULL2":
                    if (!string.IsNullOrEmpty(value + ""))
                        return "有";
                    else
                        return "";
                case "Flag":
                    if (value.ToString() == "1")
                        return "有";
                    else
                        return "无";
                case "Sex_Male":
                    return value.ToString() == "男" ? "true" : "false";
                case "Sex_Female":
                    return value.ToString() == "女" ? "true" : "false";
                case "RemainAmount":
                    if (value.ToString().StartsWith("-"))
                        return "0";
                    else
                        return value.ToString();
                case "Visibility":
                    switch (value.ToString())
                    {
                        case "0": return "显示";
                        //  case "1": return "隐藏";
                        case "2": return "不显示";
                        default: return "";
                    }
                case "IsReadOnly":
                    switch (value.ToString())
                    {
                        case "0": return "可编辑";
                        case "1": return "只读";
                        default: return "";
                    }
                case "DefaultFlag":
                    switch (value.ToString())
                    {
                        case "1": return "默认使用";
                        case "0": return "选用";
                        default: return "";
                    }
                case "ColumnType":
                    switch (value.ToString())
                    {
                        case "1": return "普通列";
                        case "2": return "界面已定义列";
                        default: return "";
                    }
                case "MoneyN2":
                    if (value.ToString() == "")
                    {
                        return "0";
                    }

                    double moneyN2;
                    if (double.TryParse(value.ToString(), out moneyN2))
                    {
                        if (moneyN2 == 0)
                        {
                            return "0";
                        }
                        else
                        {
                            return string.Format("{0:N2}", moneyN2);
                        }
                    }
                    else
                    {
                        return value.ToString();
                    }
                case "NumRound":
                    decimal numRound;
                    decimal.TryParse(value.ToString(), out numRound);
                    return string.Format("{0:#0.#}", Math.Round(numRound, 1));
                case "Discount":
                    if (value.ToString() == "")
                    {
                        return string.Format("{0:P}", 0); //比例
                    }
                    decimal discount;
                    decimal.TryParse(value.ToString(), out discount);
                    return string.Format("{0:P}", discount); //比例 
                default:
                    return "";

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (parameter.ToString())
            {
                case "BoolEN":
                    return value.ToString().ToLower() == "true" ? "1" : "0";
                case "BoolEN2":
                    return value.ToString().ToLower() == "true" ? "0" : "1";
                case "BoolCN":
                    return value.ToString() == "是" ? "1" : "0";
                case "Sex_Male":
                    return value.ToString().ToLower() == "true" ? "男" : "女";
                case "Sex_Female":
                    return value.ToString().ToLower() == "true" ? "女" : "男";
                case "Money":
                    return value.ToString() == "" ? "0" : value;
                case "Money2":
                    return value.ToString() == "" ? "0" : value;
                case "MoneyN2":
                    return value.ToString() == "" ? "0" : value;
                case "Discount":
                    return decimal.Parse(value.ToString().Replace("%", "")) / 100;
                case "NumRound":
                    return value.ToString() == "" ? "0" : value;
                default:
                    return "";
            }
        }

        #endregion
    }
}
