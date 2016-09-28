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
    /// Charge.xaml 的交互逻辑
    /// </summary>
    public partial class Cash : MetroWindow
    {
        List<XElement> xeList = new List<XElement>();
        public Cash()
        {
            InitializeComponent();
            tblBalance.Text = UserLogin.CurrentUser.User.BalanceAmount + "";
            GetAccountData();
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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
                if (string.IsNullOrEmpty(tblSecretCode.Password))
                {
                    Tools.Msg.Info(LocalizationHelper.GetRes("str_Require_SecretCode"));
                    return;
                }
                decimal amount = 0;
                if (!decimal.TryParse(tblAmount.Text.Replace(",", "").Trim(), out amount) || amount == 0)
                {
                    Msg.Info(LocalizationHelper.GetRes("str_Require_Amount"));
                    return;
                }
                //判断余额是否不足
                if (amount > UserLogin.CurrentUser.User.BalanceAmount)
                {
                    Tools.Msg.Info(LocalizationHelper.GetRes("str_Valid_NoMoney"));
                    tblAmount.Focus();
                    return;
                }
                HttpParam param = new HttpParam();
                param.Add("realname", realname);
                param.Add("banktype", banktype);
                param.Add("bankcardnum", bankcardnum);
                param.Add("bankbranch", bankbranch);
                param.Add("amount", amount);
                param.Add("code", SecureHelper.Md5(tblSecretCode.Password));
                ProcessRequest.Process(string.Format("User/Cash").HttpPost(param), (ApiReturn res) =>
                {
                    UserLogin.GetBalance();
                    //UserLogin.CurrentUser.User.BalanceAmount -= amount;
                    Msg.Success(LocalizationHelper.GetRes("str_Success_Cash"));
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }
        private List<XElement> GetAccountData()
        {
            HttpParam param = new HttpParam();
            //bool isOk = false;
            ProcessRequest.Process(string.Format("User/UserBank").HttpPost(param), (ApiReturn res) =>
            {
                if (!string.IsNullOrWhiteSpace(res.data + ""))
                {
                    XmlDocument xmldata = JsonConvert.DeserializeXmlNode(res.data + "", "Root");
                    XElement xeRoot = XElement.Parse(xmldata.OuterXml.Replace("&lt;", "<").Replace("&gt;", ">"));
                    if (xeRoot.HasElements)
                    {
                        xeList = xeRoot.Elements().ToList();
                        LoadAccount();
                    }
                }
            }, false);
            return xeList;
        }
        void ChangeBank(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            XElement xe = btn.Tag as XElement;
            txtBankCardNum.Text = xe.Element("BankCardNum").GetValue();
            txtBankType.Text = xe.Element("BankType").GetValue();
            txtRealName.Text = xe.Element("RealName").GetValue();
            Pop_account.IsOpen = false;
        }
        private void LoadAccount()
        {
            spBank.Children.Clear();
            foreach (XElement xe in xeList)
            {
                Button btn = new Button();
                btn.Click += new RoutedEventHandler(ChangeBank);
                btn.Margin = new Thickness(0);
                btn.Style = (Style)this.FindResource("PopButtonStyle");
                //btn.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Vertical;
                TextBlock tblName = new TextBlock();
                tblName.Text = xe.Element("RealName").GetValue();
                TextBlock tblBank = new TextBlock();
                tblBank.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#969696"));
                tblBank.Margin = new Thickness(0, 3, 0, 0);
                tblBank.Text = string.Format("{0} {1}", xe.Element("BankCardNum").GetValue(), xe.Element("BankType").GetValue());
                sp.Children.Add(tblName);
                sp.Children.Add(tblBank);
                btn.Content = sp;
                btn.Tag = xe;
                spBank.Children.Add(btn);
            }
        }

        private void btnMyAccount_Click(object sender, RoutedEventArgs e)
        {
            Pop_account.IsOpen = true;
        }

        private void tblAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal amount = 0;
            if (!decimal.TryParse(tblAmount.Text.Replace(",", "").Trim(), out amount))
            {
                Msg.Info(LocalizationHelper.GetRes("str_Require_Amount"));
                return;
            }
        }
    }
}
