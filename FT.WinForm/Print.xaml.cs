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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using FT.Utility.Helper;
using FT.WinForm.Login;
using FT.WinForm.Http;
using FT.WinForm.Tools;
using FT.Utility.ApiHelper;
using System.Drawing.Printing;
using com.epson.pos.driver;
using System.Drawing;
using System.Xml.Linq;
using FT.Model;
namespace FT.WinForm
{
    /// <summary>
    /// Trade.xaml 的交互逻辑
    /// </summary>
    public partial class Print : MetroWindow
    {
        private MatchUserBet userBet;
        public Print(MatchUserBet _userBet)
        {
            InitializeComponent(); 
            userBet = _userBet;
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }
        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintHelper p = new PrintHelper(userBet);
            p.Print();
        }

        private void BtnContinue_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}