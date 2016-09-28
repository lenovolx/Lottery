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
    public partial class Prize : MetroWindow
    {
        public Prize(decimal betBonus)
        {
            InitializeComponent();
            txtUserBetGet.Text = string.Format(LocalizationHelper.GetRes("str_Msg_UserBetGet"), betBonus);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
