using FT.Utility.ApiHelper;
using FT.Utility.Helper;
using FT.WinForm.Http;
using FT.WinForm.Login;
using FT.WinForm.Tools;
using MahApps.Metro.Controls;
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

namespace FT.WinForm
{
    /// <summary>
    /// Charge.xaml 的交互逻辑
    /// </summary>
    public partial class SecretCode : MetroWindow
    {
        public SecretCode()
        {
            InitializeComponent();
            tblLoginName.Text = UserLogin.CurrentUser.User.LoginName;
            tblOldPassword.Focus();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tblOldPassword.Password))
                {
                    Tools.Msg.Info(LocalizationHelper.GetRes("str_Require_OldPasswd"));
                    return;
                }
                if (string.IsNullOrEmpty(tblNewPassword.Password))
                {
                    Tools.Msg.Info(LocalizationHelper.GetRes("str_Require_NewPasswd"));
                    return;
                }
                if (string.IsNullOrEmpty(tblNewPassword1.Password))
                {
                    Tools.Msg.Info(LocalizationHelper.GetRes("str_Require_NewPasswd1"));
                    return;
                }
                if (tblNewPassword.Password != tblNewPassword1.Password)
                {
                    Tools.Msg.Info(LocalizationHelper.GetRes("str_Require_NotSame"));
                    return;
                }
                HttpParam param = new HttpParam();
                param.Add("pwd", SecureHelper.Md5(tblOldPassword.Password));
                param.Add("npwd", SecureHelper.Md5(tblNewPassword.Password));
                param.Add("type", 6);
                ProcessRequest.Process(string.Format("User/EditPass").HttpPost(param), (ApiReturn res) =>
                {
                    Msg.Success(LocalizationHelper.GetRes("str_Success_UpdatePassword"));
                    UserLogin.CurrentUser.User.Password = SecureHelper.Md5(tblNewPassword.Password);
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                Msg.Error(ex.Message);
            }
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }
    }
}
