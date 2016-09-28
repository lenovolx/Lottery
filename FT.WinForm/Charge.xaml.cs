using FT.Utility.ApiHelper;
using FT.Utility.Helper;
using FT.WinForm.Http;
using FT.WinForm.Login;
using FT.WinForm.Tools;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

namespace FT.WinForm
{
    /// <summary>
    /// Charge.xaml 的交互逻辑
    /// </summary>
    public partial class Charge : MetroWindow
    {
        public Charge()
        {
            InitializeComponent();
            tblLoginName.Text = UserLogin.CurrentUser.User.LoginName;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string cardnum = txtCardNum.Text.Trim();
                if (string.IsNullOrWhiteSpace(cardnum))
                {
                    Tools.Msg.Info(LocalizationHelper.GetRes("str_Require_CardNum"));
                    txtCardNum.Focus();
                    return;
                }
                HttpParam param = new HttpParam();
                param.Add("card", cardnum);
                param.Add("tuser", UserLogin.CurrentUser.User.Id);
                param.Add("type", 2);//充值
                ProcessRequest.Process(string.Format("User/Trade").HttpPost(param), (ApiReturn res) =>
                {
                    UserLogin.GetBalance();
                    Msg.Success(LocalizationHelper.GetRes("str_Success_Charge"));
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        private void btnValid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string cardnum = txtCardNum.Text.Trim();
                if (string.IsNullOrWhiteSpace(cardnum))
                {
                    Tools.Msg.Info(LocalizationHelper.GetRes("str_Require_CardNum"));
                    txtCardNum.Focus();
                    return;
                }
                HttpParam param = new HttpParam();
                param.Add("username", UserLogin.CurrentUser.User.LoginName);
                param.Add("card", cardnum);
                ProcessRequest.Process(string.Format("System/Card").HttpPost(param), (ApiReturn res) =>
                {
                    if (!string.IsNullOrWhiteSpace(res.data + ""))
                    {
                        XmlDocument xmldata = JsonConvert.DeserializeXmlNode(res.data + "", "Root");
                        XElement xeRoot = XElement.Parse(xmldata.OuterXml.Replace("&lt;", "<").Replace("&gt;", ">"));
                        if (xeRoot.Element("IsUsed").GetValue(0) == 1)
                        {
                            Msg.Error(LocalizationHelper.GetRes("str_Error_IsUsed"));
                            return;
                        }
                        if (xeRoot.Element("Status").GetValue(0) == 1)
                        {
                            Msg.Error(LocalizationHelper.GetRes("str_Error_UnValidCard"));
                            return;
                        }
                        App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            tblAmount.Text = xeRoot.Element("Amount").GetValue(0) + "";
                        }));
                    }
                    else
                    {
                        Msg.Error(LocalizationHelper.GetRes("str_Error_UnExists"));
                        return;
                    }
                });
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
