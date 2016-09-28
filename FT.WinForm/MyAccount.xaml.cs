using FT.Utility.ApiHelper;
using FT.Utility.Helper;
using FT.WinForm.Http;
using FT.WinForm.Login;
using FT.WinForm.Tools;
using MahApps.Metro.Controls;
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
using System.Xml;
using System.Xml.Linq;

namespace FT.WinForm
{
    /// <summary>
    /// MyAccount.xaml 的交互逻辑
    /// </summary>
    public partial class MyAccount : MetroWindow
    {
        public MyAccount()
        {
            InitializeComponent();
            LoadAccount();
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }
        private void LoadAccount()
        {
            List<XElement> xeList = GetAccountData().Select(
                c => new XElement("Account"
                    , c.Element("Id")
                    , c.Element("RealName")
                    , c.Element("BankType")
                    , c.Element("BankCardNum")
                    , new XElement("BankCardInfo", string.Format("{0}({1})", c.Element("BankType").GetValue(), c.Element("BankCardNum").GetValue()))
                    )).ToList();
            dgAccount.ItemsSource = xeList;
        }
        private List<XElement> GetAccountData()
        {
            List<XElement> xeList = new List<XElement>();
            try
            {
                HttpParam param = new HttpParam();
                ProcessRequest.Process(string.Format("User/UserBank").HttpPost(param), (ApiReturn res) =>
                {
                    if (!string.IsNullOrWhiteSpace(res.data + ""))
                    {
                        XmlDocument xmldata = JsonConvert.DeserializeXmlNode(res.data + "", "Root");
                        XElement xeRoot = XElement.Parse(xmldata.OuterXml.Replace("&lt;", "<").Replace("&gt;", ">"));
                        if (xeRoot.HasElements)
                        {
                            xeList = xeRoot.Elements().ToList();
                        }
                    }
                }, false);
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
            return xeList;
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            gdList.Visibility = Visibility.Collapsed;
            gdDetail.Visibility = Visibility.Visible;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgAccount.SelectedIndex < 0)
                {
                    Tools.Msg.Info(LocalizationHelper.GetRes("str_Require_DeleteOne"));
                    return;
                }
                if (MessageBox.Show(LocalizationHelper.GetRes("str_Msg_DeleteAccount"), LocalizationHelper.GetRes("str_Msg_Msg"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    XElement xeSel = dgAccount.SelectedItem as XElement;
                    HttpParam param = new HttpParam();
                    param.Add("bankid", xeSel.Element("Id").GetValue(0));
                    ProcessRequest.Process(string.Format("User/DelUserBank").HttpPost(param), (ApiReturn res) =>
                    {
                        Tools.Msg.Success(LocalizationHelper.GetRes("str_Success_DeleteAccount")); 
                        LoadAccount();
                    });
                }
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region 验证
                string banktype = txtBankType.Text.Trim();
                string bankcardnum = txtBankCardNum.Text.Trim();
                string realname = txtRealName.Text.Trim();
                string bankbranch = "";
                if (string.IsNullOrWhiteSpace(realname))
                {
                    Tools.Msg.Info(LocalizationHelper.GetRes("str_Require_RealName"));
                    txtRealName.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(banktype))
                {
                    Tools.Msg.Info(LocalizationHelper.GetRes("str_Require_BankType"));
                    txtBankType.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(bankcardnum))
                {
                    Tools.Msg.Info(LocalizationHelper.GetRes("str_Require_BankCardNum"));
                    txtBankCardNum.Focus();
                    return;
                }
                #endregion
                HttpParam param = new HttpParam();
                param.Add("realname", realname);
                param.Add("banktype", banktype);
                param.Add("bankcardnum", bankcardnum);
                param.Add("bankbranch", bankbranch);
                ProcessRequest.Process(string.Format("User/AddUserBank").HttpPost(param), (ApiReturn res) =>
                {
                    Tools.Msg.Success(LocalizationHelper.GetRes("str_Success_AddAccount"));
                    LoadAccount();
                    CleanForm();
                });
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            CleanForm();
        }

        private void CleanForm()
        {
            gdList.Visibility = Visibility.Visible;
            gdDetail.Visibility = Visibility.Collapsed;
            txtBankCardNum.Text = "";
            txtBankType.Text = "";
            txtRealName.Text = "";
        }
    }
}
