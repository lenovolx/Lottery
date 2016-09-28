using FT.Utility.ApiHelper;
using FT.Utility.Helper;
using FT.WinForm.Http;
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
    /// BetInfo.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryAccount : MetroWindow
    {
        private XElement xeRoot;
        public HistoryAccount()
        {
            InitializeComponent();
            DateTime sdate = DateTime.Now.AddDays(-7);
            DateTime edate = DateTime.Now;
            dpStartDate.BlackoutDates.Add(new CalendarDateRange(new DateTime(), sdate.AddDays(-1)));
            dpStartDate.BlackoutDates.Add(new CalendarDateRange(edate.AddDays(1), DateTime.MaxValue));
            dpEndDate.BlackoutDates.Add(new CalendarDateRange(edate.AddDays(1), DateTime.MaxValue));
            dpStartDate.SelectedDate = sdate;
            dpEndDate.SelectedDate = edate;
            //dpEndDate.BlackoutDates.Add(new CalendarDateRange(edate, DateTime.MaxValue));
            LoadData();
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!dpStartDate.SelectedDate.HasValue || !dpEndDate.SelectedDate.HasValue)
            {
                Tools.Msg.Info(LocalizationHelper.GetRes("str_Require_Date"));
                return;
            }
            LoadData();
        }

        private void LoadData()
        {
            bool isOk = false;
            try
            {
                HttpParam param = new HttpParam();
                param.Add("sdate", dpStartDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
                param.Add("edate", dpEndDate.SelectedDate.Value.ToString("yyyy-MM-dd"));

                ProcessRequest.Process(string.Format("User/History").HttpPost(param), (ApiReturn res) =>
                {
                    if (!string.IsNullOrWhiteSpace(res.data + ""))
                    {
                        XmlDocument xmldata = JsonConvert.DeserializeXmlNode(res.data + "", "Root");
                        xeRoot = XElement.Parse(xmldata.OuterXml.Replace("&lt;", "<").Replace("&gt;", ">"));
                        LoadBetInfos();
                    }
                }, false);
            }
            catch (Exception ex)
            {
                try
                {
                    Msg.Error(ex.Message);
                }
                catch
                {
                }
            }
        }
        private void LoadBetInfos()
        {
            if (xeRoot == null) { return; }
            dymSP.Children.Clear();
            #region 加载交易单
            foreach (XElement xeUserBet in xeRoot.Element("data").Elements("Grid"))
            {
                Grid grid = new Grid();
                grid.Margin = new Thickness(0);
                #region 初始化列
                ColumnDefinition col_1 = new ColumnDefinition();//日期
                ColumnDefinition col_2 = new ColumnDefinition();//投注额
                ColumnDefinition col_3 = new ColumnDefinition();//有效
                ColumnDefinition col_4 = new ColumnDefinition();//输赢
                col_1.Width = new GridLength(140, GridUnitType.Pixel);
                col_2.Width = new GridLength(180, GridUnitType.Pixel);
                col_3.Width = new GridLength(180, GridUnitType.Pixel);
                col_4.Width = new GridLength(180, GridUnitType.Pixel);
                grid.ColumnDefinitions.Add(col_1);
                grid.ColumnDefinitions.Add(col_2);
                grid.ColumnDefinitions.Add(col_3);
                grid.ColumnDefinitions.Add(col_4);
                #endregion
                #region 添加边框线
                GridHelper.SetShowBorder(grid, true);
                #endregion
                dymSP.Children.Add(grid);
                #region 日期
                TextBlock tblDate = new TextBlock();
                tblDate.Text = xeUserBet.Element("DateTimes").GetValue();
                Grid.SetRow(tblDate, 0);
                Grid.SetColumn(tblDate, 0);
                grid.Children.Add(tblDate);
                #endregion
                #region 投注额
                TextBlock tblBetValue = new TextBlock();
                tblBetValue.Text = xeUserBet.Element("BetAmount").GetValue();
                Grid.SetRow(tblBetValue, 0);
                Grid.SetColumn(tblBetValue, 1);
                grid.Children.Add(tblBetValue);
                #endregion
                #region 有效金额
                TextBlock tblValidValue = new TextBlock();
                tblValidValue.Text = xeUserBet.Element("EffectiveAmount").GetValue(0D) + "";
                Grid.SetRow(tblValidValue, 0);
                Grid.SetColumn(tblValidValue, 2);
                grid.Children.Add(tblValidValue);
                #endregion
                #region 输赢
                TextBlock tblBetBonus = new TextBlock();
                tblBetBonus.Text = xeUserBet.Element("WinOrLose").GetValue(0D) + "";
                Grid.SetRow(tblBetBonus, 0);
                Grid.SetColumn(tblBetBonus, 3);
                grid.Children.Add(tblBetBonus);
                #endregion
            }
            #endregion

            #region 加载总计
            tblTotalBetValue.Text = xeRoot.Element("data").Elements("Grid").Sum(c => c.Element("BetAmount").GetValue(0D)) + "";
            tblTotalValidValue.Text = xeRoot.Element("data").Elements("Grid").Sum(c => c.Element("EffectiveAmount").GetValue(0D)) + "";
            tblTotalBetBonus.Text = xeRoot.Element("data").Elements("Grid").Sum(c => c.Element("WinOrLose").GetValue(0D)) + "";
            #endregion
        }
    }
}
