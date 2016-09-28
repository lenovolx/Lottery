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
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

namespace FT.WinForm
{
    /// <summary>
    /// BetInfo.xaml 的交互逻辑
    /// </summary>
    public partial class SettleRecord : MetroWindow
    {
        private XElement xeRoot;
        private int pageSize = 5;
        public SettleRecord()
        {
            InitializeComponent();
            DateTime sdate = DateTime.Now.AddDays(-1);
            DateTime edate = DateTime.Now;
            dpEndDate.BlackoutDates.Add(new CalendarDateRange(edate.AddDays(1), DateTime.MaxValue));
            dpStartDate.SelectedDate = sdate;
            dpEndDate.SelectedDate = edate;
            pgSettle.PageChanged += new PageComponent.PageChangedHandle(LoadPage);
            LoadData(1);
        }
        void LoadPage(object sender, EventArgs e)
        {
            LoadData(Convert.ToInt32((sender as PageComponent).CurrentPage));
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
            //pgSettle. = pageSize + "";
            pgSettle.CurrentPage = "1";
            LoadData(1);
        }
        private int GetTotalPage(int totalcount)
        {
            if (totalcount % pageSize == 0)
            {
                return totalcount / pageSize;
            }
            else
            {
                return (totalcount / pageSize) + 1;
            }
        }
        private void LoadData(int pageIndex)
        {
            bool isOk = false;
            try
            {
                HttpParam param = new HttpParam();
                param.Add("sdate", dpStartDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
                param.Add("edate", dpEndDate.SelectedDate.Value.ToString("yyyy-MM-dd"));
                param.Add("page", pageIndex);
                param.Add("size", pageSize);
                //TaskAsyncHelper.RunAsync(() =>
                //{
                ProcessRequest.Process(string.Format("User/BetRecord").HttpPost(param), (ApiReturn res) =>
                {
                    App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        pgSettle.TotalPage = GetTotalPage(res.total) + "";
                    }));
                    if (!string.IsNullOrWhiteSpace(res.data + ""))
                    {
                        XmlDocument xmldata = JsonConvert.DeserializeXmlNode(res.data + "", "Root");
                        xeRoot = XElement.Parse(xmldata.OuterXml.Replace("&lt;", "<").Replace("&gt;", ">"));
                        LoadBetInfos();
                    }
                }, false);
                //}, () =>
                //{
                //    if (isOk)
                //    {
                //        LoadBetInfos();
                //    }
                //});

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
            foreach (XElement xeUserBet in xeRoot.Element("data").Elements("rows"))
            {
                Grid grid = new Grid();
                grid.Margin = new Thickness(0);
                #region 初始化列
                ColumnDefinition col_1 = new ColumnDefinition();//编号
                ColumnDefinition col_2 = new ColumnDefinition();//注单号
                ColumnDefinition col_3 = new ColumnDefinition();//投注类型
                ColumnDefinition col_4 = new ColumnDefinition();//投注信息
                ColumnDefinition col_5 = new ColumnDefinition();//投注额
                ColumnDefinition col_6 = new ColumnDefinition();//可赢金额
                ColumnDefinition col_7 = new ColumnDefinition();//注单状态
                col_1.Width = new GridLength(0, GridUnitType.Pixel);
                col_2.Width = new GridLength(140, GridUnitType.Pixel);
                col_3.Width = new GridLength(140, GridUnitType.Pixel);
                col_4.Width = new GridLength(240, GridUnitType.Pixel);
                col_5.Width = new GridLength(80, GridUnitType.Pixel);
                col_6.Width = new GridLength(80, GridUnitType.Pixel);
                col_7.Width = new GridLength(80, GridUnitType.Pixel);
                grid.ColumnDefinitions.Add(col_1);
                grid.ColumnDefinitions.Add(col_2);
                grid.ColumnDefinitions.Add(col_3);
                grid.ColumnDefinitions.Add(col_4);
                grid.ColumnDefinitions.Add(col_5);
                grid.ColumnDefinitions.Add(col_6);
                grid.ColumnDefinitions.Add(col_7);
                #endregion
                #region 添加边框线
                GridHelper.SetShowBorder(grid, true);
                #endregion
                dymSP.Children.Add(grid);
                decimal betior = 1;//可赢赔率
                #region 加载投注明细
                int i = 0;
                foreach (XElement xeDetail in xeUserBet.Elements("Detail"))
                {
                    LoadBetDetail(xeDetail, grid, out betior);
                    i++;
                }
                #endregion
                #region 订单号
                TextBlock tblOrderId = new TextBlock();
                tblOrderId.Text = xeUserBet.Element("OrderId").GetValue();
                Grid.SetRow(tblOrderId, 0);
                Grid.SetRowSpan(tblOrderId, i);
                Grid.SetColumn(tblOrderId, 1);
                grid.Children.Add(tblOrderId);
                #endregion
                #region 投注类型
                TextBlock tblBetTypeName = new TextBlock();
                tblBetTypeName.Text = LocalizationHelper.GetRes("Enum.BetType." + (xeUserBet.Element("BetType").GetValue().StartsWith("5") ? 5 : xeUserBet.Element("BetType").GetValue(0)));
                Grid.SetRow(tblBetTypeName, 0);
                Grid.SetRowSpan(tblBetTypeName, i);
                Grid.SetColumn(tblBetTypeName, 2);
                grid.Children.Add(tblBetTypeName);
                #endregion
                #region 投注额
                TextBlock tblBetValue = new TextBlock();
                tblBetValue.Text = xeUserBet.Element("BetValue").GetValue();
                Grid.SetRow(tblBetValue, 0);
                Grid.SetRowSpan(tblBetValue, i);
                Grid.SetColumn(tblBetValue, 4);
                grid.Children.Add(tblBetValue);
                #endregion
                #region 可赢金额
                TextBlock tblBetBonus = new TextBlock();
                tblBetBonus.Text = xeUserBet.Element("AchieveAmount").GetValue(0M).ToString("F2");// (xeUserBet.Element("BetValue").GetValue(0) * xeUserBet.Elements("Detail").Select(c => c.Element("BetIor").GetValue(0M)).Aggregate((x, y) => x * y)).ToString("F2");
                Grid.SetRow(tblBetBonus, 0);
                Grid.SetRowSpan(tblBetBonus, i);
                Grid.SetColumn(tblBetBonus, 5);
                grid.Children.Add(tblBetBonus);
                #endregion
                #region 结算状态
                TextBlock tblIsSettle = new TextBlock();
                tblIsSettle.Text = LocalizationHelper.GetRes("Enum.IsSettle." + xeUserBet.Element("IsSettle").GetValue(0));
                Grid.SetRow(tblIsSettle, 0);
                Grid.SetRowSpan(tblIsSettle, i);
                Grid.SetColumn(tblIsSettle, 6);
                grid.Children.Add(tblIsSettle);
                #endregion
            }
            #endregion

            #region 加载总计
            tblTotal.Text = xeRoot.Element("data").Elements("rows").Sum(c => c.Element("BetValue").GetValue(0D)) + "";
            #endregion
        }
        private void LoadBetDetail(XElement xeDetail, Grid grid, out decimal betior)
        {
            betior = 1;
            #region 初始化行数
            RowDefinition row = new RowDefinition();
            grid.RowDefinitions.Add(row);
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            Grid.SetColumn(sp, 3);
            //最后一行
            Grid.SetRow(sp, grid.RowDefinitions.Count - 1);
            grid.Children.Add(sp);
            #region 投注项
            TextBlock tblBetType = new TextBlock();
            tblBetType.Text = LocalizationHelper.GetRes("Enum.BetType." + xeDetail.Element("BetType").GetValue(1));
            sp.Children.Add(tblBetType);

            TextBlock tblLeagueName = new TextBlock();
            tblLeagueName.Text = xeDetail.Element("LeagueName").GetValue();
            tblLeagueName.ToolTip = xeDetail.Element("LeagueName").GetValue();
            sp.Children.Add(tblLeagueName);

            //<WrapPanel Orientation="Horizontal" Margin="10,0,0,0">
            //    <TextBlock Margin="0,10,0,0" Name="tblBetHTeam" Text="{Binding Element[HTeam].Value}" Foreground="#012C4C"></TextBlock>
            //    <TextBlock Margin="0,10,0,0" Name="tblRatioH" Text="{Binding Element[RatioH].Value}"></TextBlock>
            //    <TextBlock Margin="0,10,0,0" Name="tblVS" Text="{Binding Element[VS].Value}" Foreground="#012C4C"></TextBlock>
            //    <TextBlock Margin="0,10,0,0" Name="tblRatioC" Text="{Binding Element[RatioC].Value}"></TextBlock>
            //    <TextBlock Margin="0,10,0,0" Name="tblBetCTeam" Text="{Binding Element[CTeam].Value}" Foreground="#012C4C"></TextBlock>
            //</WrapPanel>
            //StackPanel spTeam = new StackPanel();
            //spTeam.Orientation = Orientation.Horizontal;

            TextBlock tblTeamVs = new TextBlock();
            tblTeamVs.Text = xeDetail.Element("TeamVs").GetValue();
            tblTeamVs.ToolTip = xeDetail.Element("TeamVs").GetValue();
            sp.Children.Add(tblTeamVs);

            TextBlock tblBetContent = new TextBlock();
            tblBetContent.Text = xeDetail.Element("BetDescription").GetValue();
            tblBetContent.ToolTip = xeDetail.Element("BetDescription").GetValue();
            sp.Children.Add(tblBetContent);
            #endregion


            #endregion
        }
    }
}
