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
using MahApps.Metro.Controls;
using FT.Utility.Helper;
using FT.WinForm.Login;
using FT.WinForm.Http;
using FT.WinForm.Tools;
using FT.Utility.ApiHelper;
namespace FT.WinForm
{
    /// <summary>
    /// Trade.xaml 的交互逻辑
    /// </summary>
    public partial class Trade : MetroWindow
    {
        public Trade()
        {
            InitializeComponent();
            tblBalance.Text = UserLogin.CurrentUser.User.BalanceAmount + "";
            tblUserName.Text = UserLogin.CurrentUser.User.LoginName;
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
                decimal amount = 0;
                if (!decimal.TryParse(tblAmount.Text.Replace(",", "").Trim(), out amount) || amount == 0)
                {
                    Msg.Info(LocalizationHelper.GetRes("str_Require_Amount"));
                    tblAmount.Focus();
                    return;
                }
                if (amount > UserLogin.CurrentUser.User.BalanceAmount)
                {
                    tblNoMoneyMsg.Visibility = Visibility.Visible;
                    tblAmount.Focus();
                    return;
                }
                string tuser = txtToUserName.Text.Trim();
                if (string.IsNullOrWhiteSpace(tuser))
                {
                    txtToUserName.Focus();
                    Msg.Info(LocalizationHelper.GetRes("str_Require_ToUserName"));
                    return;
                }
                if (string.IsNullOrEmpty(tblSecretCode.Password))
                {
                    Tools.Msg.Info(LocalizationHelper.GetRes("str_Require_SecretCode"));
                    return;
                }
                HttpParam param = new HttpParam();
                param.Add("fuser", UserLogin.CurrentUser.User.Id);
                param.Add("tuser", tuser);
                param.Add("amount", tblAmount.Text.Replace(",", "").Trim());
                param.Add("type", 1);//转账
                param.Add("code", SecureHelper.Md5(tblSecretCode.Password));
                ProcessRequest.Process(string.Format("User/Trade").HttpPost(param), (ApiReturn res) =>
                {
                    UserLogin.GetBalance();
                    //UserLogin.CurrentUser.User.BalanceAmount -= amount;
                    Msg.Success(LocalizationHelper.GetRes("str_Success_Trade"));
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }

        private void tblAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal amount = 0;
            if (!decimal.TryParse(tblAmount.Text.Replace(",", "").Trim(), out amount))
            {
                Msg.Info(LocalizationHelper.GetRes("str_Require_Amount"));
                return;
            }
            //判断余额是否不足
            if (amount > UserLogin.CurrentUser.User.BalanceAmount)
            {
                tblNoMoneyMsg.Visibility = Visibility.Visible;
            }
            else
            {
                tblNoMoneyMsg.Visibility = Visibility.Collapsed;
            }
        }
    }
}
