using FT.WinForm.Tools;
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
using System.Xml.Linq;
using FT.Utility.Helper;
namespace FT.WinForm.Login
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class CLogin : Window
    {
        public CLogin()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Login_Loaded);
        }
        void Login_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LoginCookies.GetLoginUser().Count > 0)
                {
                    XElement xeUser = LoginCookies.GetLoginUser().First();
                    CobUserNo.Text = xeUser.Element("LoginName").Value;
                    txtPassword.Password = xeUser.Element("IsRememberPwd").GetValue(false) == true ? xeUser.Element("Password").GetValue() : "";
                    ChkRememberPwd.IsChecked = xeUser.Element("IsRememberPwd").GetValue(false) == true;

                }
                if (string.IsNullOrEmpty(CobUserNo.Text))
                    CobUserNo.Focus();
                else
                    txtPassword.Focus();
            }
            catch (Exception ex)
            {
                //Twi.COMMON.WPF.TwiMsg.Error(ex.Message);
            }
        }

        //点击登录按钮，登录
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;  //按钮失效
            sLogin();
            btnLogin.IsEnabled = true; //按钮有效
        }
        //回车登录
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin.IsEnabled = false;  //按钮失效
                sLogin();
                btnLogin.IsEnabled = true; //按钮有效
            }
        }

        /// <summary>
        /// 登录（带进度条）
        /// </summary>
        private void LoginWithMask()
        {
            //LoadingPage loadingPage = new LoadingPage(this, "正在验证身份，请稍后...");
            //LoadingPage.LoadingCallback doSomthing = new SendInfo.Client.LoadingPage.LoadingCallback(this.sLogin);
            //loadingPage.CallbackDoSomething = doSomthing;
            //loadingPage.ShowDialog();
        }

        /// <summary>
        /// 登录
        /// </summary>
        private void sLogin()
        {

            if (this.CobUserNo.Text.Trim() == "")
            {
                Msg.Info(LocalizationHelper.GetRes("str_Require_LoginName"));
                CobUserNo.Focus();
                return;
            }
            if (this.txtPassword.Password.Trim() == "")
            {
                Msg.Info(LocalizationHelper.GetRes("str_Require_Password"));
                txtPassword.Focus();
                return;
            }
            if (UserLogin.Login(this.CobUserNo.Text.Trim(), this.txtPassword.Password.Trim(), this.ChkRememberPwd.IsChecked.Value))
            {
                MainWindow main = new MainWindow();
                main.Show();

                //main.UpdateLayout();
                //main.WindowState = WindowState.Maximized;
                //全屏模式
                //main.GoFullscreen();
                this.Close();
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //设置可以拖动
            this.DragMove();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void ChangeLanguage(object sender, RoutedEventArgs e)
        {
            try
            {
                Pop_lang.IsOpen = false;
                Button btnLangSel = sender as Button;
                if (btnLangSel == null) return;
                string selLang = (btnLangSel.Tag + "").Split('|')[0];
                foreach (Button btn in UIHelper.GetChildObjects<Button>(spLang, typeof(Button)))
                {
                    btn.Visibility = Visibility.Visible;
                }
                btnLangSel.Visibility = Visibility.Collapsed;
                UserLogin.LANG = selLang;
                BitmapImage image = new BitmapImage(new Uri(string.Format("/Image/head_{0}BIG.png", selLang), UriKind.RelativeOrAbsolute));
                imgLang.Source = image;
                tblLang.Text = (btnLangSel.Tag + "").Split('|')[1];
                LocalizationHelper.ApplyLanguage(selLang);
            }
            catch (Exception ex)
            {
                Tools.Msg.Error(LocalizationHelper.GetRes("str_Error_ChangeLang") + ex.Message);
            }
        }

        private void btnLang_Click(object sender, RoutedEventArgs e)
        {
            Pop_lang.IsOpen = true;
        }
    }
}
