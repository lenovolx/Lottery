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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using FT.Utility.Helper;
using System.Windows.Controls.Primitives;
using FT.WinForm.Http;
using FT.Utility.ApiHelper;
using Newtonsoft.Json;
using FT.WinForm.Tools;
using FT.WinForm.Login;
using System.Xml;
using MahApps.Metro.Controls;
using System.Windows.Threading;
using System.Threading;
namespace FT.WinForm.UC
{
    /// <summary>
    /// UCStandardGrid.xaml 的交互逻辑
    /// </summary>
    public partial class UCStandardGrid : UserControl
    {
        public List<XElement> xeSelectBet = new List<XElement>();
        private XElement xeRoot;
        Action<List<XElement>> ItemSelect;
        public UCStandardGrid(Action<List<XElement>> _ItemSelect)
        {
            InitializeComponent();
            ItemSelect = _ItemSelect;
            //LoadData();
        }
        /// <summary>
        /// 点击赔率的时候挂单到左侧的下注区域
        /// 同一个选项选中两次即当做取消选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TgButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tgBtn = sender as ToggleButton;
            if (tgBtn == null) return;
            XElement xeSelTmp = tgBtn.Tag as XElement;
            if (xeSelTmp == null) return;
            if (xeSelectBet.Contains(xeSelTmp))
            {
                xeSelectBet.Remove(xeSelTmp);
            }
            else if (xeSelectBet != null && xeSelectBet.Count > 0)
            {
                UIHelper.UnCheckToggleButton(dymSP);
                tgBtn.IsChecked = true;
                xeSelectBet.Clear();
                xeSelectBet.Add(xeSelTmp);
            }
            else
            {
                xeSelectBet.Add(xeSelTmp);
            }
            ItemSelect(xeSelectBet);
        }
        public void ClearSelect()
        {
            xeSelectBet = new List<XElement>();
            UIHelper.UnCheckToggleButton(dymSP);
        }
        private void SelectExists()
        {
            if (xeSelectBet != null && xeSelectBet.Count > 0)
            {
                List<ToggleButton> tgList = UIHelper.GetChildObjectsLogic<ToggleButton>(dymSP, typeof(ToggleButton));
                foreach (ToggleButton tg in tgList)
                {
                    XElement xetg = tg.Tag as XElement;
                    if (xeSelectBet.Any(c => (c + "") == (xetg + "")))
                    {
                        tg.IsChecked = true;
                    }
                }
            }
        }
        #region 服务端查询数据
        /// <summary>
        /// 从服务端获取数据
        /// </summary>
        public void LoadData(bool clearSelect = false)
        {
            if (clearSelect)
            {
                xeSelectBet = new List<XElement>();
            }
            try
            {
                HttpParam param = new HttpParam();
                param.Add("bettype", "1");
                param.Add("oddbettype", "");
                ProcessRequest.Process(string.Format("Match/List").HttpPost(param), (ApiReturn res) =>
                {
                    if (!string.IsNullOrWhiteSpace(res.data + ""))
                    {
                        XmlDocument xmldata = JsonConvert.DeserializeXmlNode(res.data + "", "Root");
                        xeRoot = XElement.Parse(xmldata.OuterXml.Replace("&lt;", "<").Replace("&gt;", ">"));
                        pagetotal = xeRoot.Elements().Count() / pagesize;
                        pageindex = 0;
                        if (xeRoot.Elements().Count() % pagesize > 0)
                        {
                            pagetotal += 1;
                        }
                        LoadLeagues();
                    }
                });
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
        #endregion

        private int pagetotal = 0;
        private int pagesize = 10;
        private int pageindex = 0;
        /// <summary>
        /// 按联赛展示
        /// </summary>
        /// <param name="xeRoot"></param>
        private void LoadLeagues()
        {
            if (xeRoot == null) return;
            dymSP.Children.Clear();
            #region 加载联赛
            foreach (XElement xeLeague in xeRoot.Elements("Leagues").Take(pagesize))
            {
                //App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                //{
                LoadLeague(xeLeague);
                //}));
            }
            SelectExists();
            pageindex += 1;
            #endregion
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0 && (e.ExtentHeight - e.ViewportHeight - e.VerticalOffset) < 5 && pageindex < pagetotal)
            {
                Next();
            }
        }
        private void Next()
        {
            List<XElement> xePage = xeRoot.Elements("Leagues").Skip(pagesize * pageindex).Take(pagesize).ToList();
            foreach (XElement xeLeague in xePage)
            {
                LoadLeague(xeLeague);
            }
            SelectExists();
            pageindex += 1;
        }

        private void LoadLeague(XElement xeLeague)
        {
            List<XElement> xeList = xeLeague.Elements("Match").ToList();
            Expander exp = new Expander();
            exp.IsExpanded = true;
            TextBlock tblHeader = new TextBlock();
            tblHeader.Text = xeLeague.Element("League").GetValue();
            exp.Header = tblHeader;
            exp.Padding = new Thickness(0);
            StackPanel mainSP = new StackPanel();
            mainSP.Orientation = Orientation.Vertical;
            mainSP.Margin = new Thickness(0);
            exp.Content = mainSP;
            dymSP.Children.Add(exp);
            #region  新增内容Grid
            foreach (XElement xeMatch in xeList)
            {
                Grid grid = new Grid();
                //grid.ShowGridLines = true;
                grid.Margin = new Thickness(0);
                #region 初始化列
                ColumnDefinition col_1 = new ColumnDefinition();//时间
                ColumnDefinition col_2 = new ColumnDefinition();//赛事
                ColumnDefinition col_3 = new ColumnDefinition();//独赢
                ColumnDefinition col_4 = new ColumnDefinition();//全场让球
                ColumnDefinition col_5 = new ColumnDefinition();//全场大小
                ColumnDefinition col_6 = new ColumnDefinition();//单双
                ColumnDefinition col_7 = new ColumnDefinition();//半场独赢
                ColumnDefinition col_8 = new ColumnDefinition();//半场让球
                ColumnDefinition col_9 = new ColumnDefinition();//半场大小
                col_1.Width = new GridLength(50, GridUnitType.Pixel);
                col_2.Width = new GridLength(140, GridUnitType.Pixel);
                col_3.Width = new GridLength(50, GridUnitType.Pixel);
                col_4.Width = new GridLength(100, GridUnitType.Pixel);
                col_5.Width = new GridLength(130, GridUnitType.Pixel);
                col_6.Width = new GridLength(100, GridUnitType.Pixel);
                col_7.Width = new GridLength(80, GridUnitType.Pixel);
                col_8.Width = new GridLength(100, GridUnitType.Pixel);
                col_9.Width = new GridLength(130, GridUnitType.Pixel);
                grid.ColumnDefinitions.Add(col_1);
                grid.ColumnDefinitions.Add(col_2);
                grid.ColumnDefinitions.Add(col_3);
                grid.ColumnDefinitions.Add(col_4);
                grid.ColumnDefinitions.Add(col_5);
                grid.ColumnDefinitions.Add(col_6);
                grid.ColumnDefinitions.Add(col_7);
                grid.ColumnDefinitions.Add(col_8);
                grid.ColumnDefinitions.Add(col_9);
                #endregion
                #region 初始化行数，一场比赛占3行
                RowDefinition row_1 = new RowDefinition();
                RowDefinition row_2 = new RowDefinition();
                RowDefinition row_3 = new RowDefinition();
                grid.RowDefinitions.Add(row_1);
                grid.RowDefinitions.Add(row_2);
                grid.RowDefinitions.Add(row_3);
                #endregion
                #region 添加边框线
                GridHelper.SetShowBorder(grid, true);
                #endregion

                //把比赛场次加到联赛下面
                mainSP.Children.Add(grid);
                //App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                //{
                LoadMatch(xeMatch, grid);
                //}));
            }
            #endregion
        }
        private void LoadMatch(XElement xeMatch, Grid grid)
        {
            int rowIndex = 0;
            #region 比赛时间
            TextBlock tbl_MatchDate = new TextBlock();
            tbl_MatchDate.TextAlignment = TextAlignment.Center;
            tbl_MatchDate.Text = xeMatch.Element("MatchDate").GetValue(DateTime.Now).ToString("MM-dd\rHH:mm");
            Grid.SetRow(tbl_MatchDate, rowIndex);
            Grid.SetRowSpan(tbl_MatchDate, 3);
            Grid.SetColumn(tbl_MatchDate, 0);
            grid.Children.Add(tbl_MatchDate);
            #endregion

            #region 比赛双方
            TextBlock tbl_MatchTeamH = new TextBlock();
            TextBlock tbl_MatchTeamC = new TextBlock();
            TextBlock tbl_MatchTeam = new TextBlock();
            tbl_MatchTeamH.Height = 30;
            tbl_MatchTeamC.Height = 30;
            tbl_MatchTeam.Height = 30;
            tbl_MatchTeamH.Text = xeMatch.Element("HTeam").GetValue();
            tbl_MatchTeamH.ToolTip = xeMatch.Element("HTeam").GetValue();
            tbl_MatchTeamC.Text = xeMatch.Element("CTeam").GetValue();
            tbl_MatchTeamC.ToolTip = xeMatch.Element("CTeam").GetValue();
            tbl_MatchTeam.SetResourceReference(TextBlock.TextProperty, "Enum.HCN.N");
            //主 
            Grid.SetRow(tbl_MatchTeamH, rowIndex);
            Grid.SetColumn(tbl_MatchTeamH, 1);
            grid.Children.Add(tbl_MatchTeamH);

            //客
            Grid.SetRow(tbl_MatchTeamC, rowIndex + 1);
            Grid.SetColumn(tbl_MatchTeamC, 1);
            grid.Children.Add(tbl_MatchTeamC);

            //平
            Grid.SetRow(tbl_MatchTeam, rowIndex + 2);
            Grid.SetColumn(tbl_MatchTeam, 1);
            grid.Children.Add(tbl_MatchTeam);
            #endregion

            #region 全场独赢
            XElement xeBetInfo = xeMatch.Element("Odds").Element("MatchContent").Elements("BetInfo").FirstOrDefault(c => c.Element("BetType").GetValue() == "10");
            if (xeBetInfo != null)
            {
                //胜
                ToggleButton tgbtn_R1_C3 = new ToggleButton();
                string ior_MH = BetInfoHelper.GetIOR_MH(xeBetInfo).GetValue();
                tgbtn_R1_C3.Content = ior_MH;
                XElement xeBetContentH = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentH.SetElementValue("BetText", string.Format("{0} @{1}", xeMatch.Element("HTeam").GetValue(), ior_MH));
                xeBetContentH.SetElementValue("BetContent", string.Format("{0}@{1}", "H", ior_MH));
                xeBetContentH.SetElementValue("BetIor", ior_MH);
                //tbl_MatchDate.Text = Utils.ConvertInt2DateTime(xeMatch.Element("MatchDate").GetValue(0D)).ToString("HH:mm");
                tgbtn_R1_C3.Tag = xeBetContentH;
                tgbtn_R1_C3.Height = 30;
                tgbtn_R1_C3.Click += new RoutedEventHandler(TgButton_Click);
                Grid.SetRow(tgbtn_R1_C3, rowIndex);
                Grid.SetColumn(tgbtn_R1_C3, 2);
                grid.Children.Add(tgbtn_R1_C3);

                //负
                ToggleButton tgbtn_R2_C3 = new ToggleButton();
                string ior_MC = BetInfoHelper.GetIOR_MC(xeBetInfo).GetValue();
                tgbtn_R2_C3.Content = ior_MC;
                XElement xeBetContentC = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentC.SetElementValue("BetText", string.Format("{0} @{1}", xeMatch.Element("CTeam").GetValue(), ior_MC));
                xeBetContentC.SetElementValue("BetContent", string.Format("{0}@{1}", "C", ior_MC));
                xeBetContentC.SetElementValue("BetIor", ior_MC);
                //tbl_MatchDate.Text = Utils.ConvertInt2DateTime(xeMatch.Element("MatchDate").GetValue(0D)).ToString("HH:mm");
                tgbtn_R2_C3.Tag = xeBetContentC;
                tgbtn_R2_C3.Height = 30;
                tgbtn_R2_C3.Click += new RoutedEventHandler(TgButton_Click);

                Grid.SetRow(tgbtn_R2_C3, rowIndex + 1);
                Grid.SetColumn(tgbtn_R2_C3, 2);
                grid.Children.Add(tgbtn_R2_C3);

                //平

                ToggleButton tgbtn_R3_C3 = new ToggleButton();
                string ior_MN = BetInfoHelper.GetIOR_MN(xeBetInfo).GetValue();
                tgbtn_R3_C3.Content = ior_MN;
                XElement xeBetContent = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContent.SetElementValue("BetText", string.Format("{0} @{1}", LocalizationHelper.GetRes("Enum.HCN.N"), ior_MN));
                xeBetContent.SetElementValue("BetContent", string.Format("{0}@{1}", "N", ior_MN));
                xeBetContent.SetElementValue("BetIor", ior_MN);
                //tbl_MatchDate.Text = Utils.ConvertInt2DateTime(xeMatch.Element("MatchDate").GetValue(0D)).ToString("HH:mm");
                tgbtn_R3_C3.Tag = xeBetContent;
                tgbtn_R3_C3.Height = 30;
                tgbtn_R3_C3.Click += new RoutedEventHandler(TgButton_Click);
                Grid.SetRow(tgbtn_R3_C3, rowIndex + 2);
                Grid.SetColumn(tgbtn_R3_C3, 2);
                grid.Children.Add(tgbtn_R3_C3);
            }
            else
            {
                //空的
                //胜
                Border grid_R1_C3 = new Border();
                Grid.SetRow(grid_R1_C3, rowIndex);
                Grid.SetColumn(grid_R1_C3, 2);
                grid.Children.Add(grid_R1_C3);

                //负
                Border grid_R2_C3 = new Border();
                Grid.SetRow(grid_R2_C3, rowIndex + 1);
                Grid.SetColumn(grid_R2_C3, 2);
                grid.Children.Add(grid_R2_C3);

                //平
                Border grid_R3_C3 = new Border();
                Grid.SetRow(grid_R3_C3, rowIndex + 2);
                Grid.SetColumn(grid_R3_C3, 2);
                grid.Children.Add(grid_R3_C3);
            }
            #endregion
            #region 全场让球
            xeBetInfo = xeMatch.Element("Odds").Element("MatchContent").Elements("BetInfo").FirstOrDefault(c => c.Element("BetType").GetValue() == "12");
            if (xeBetInfo != null)
            {
                //让球方
                string strong = BetInfoHelper.GetStrong(xeBetInfo).GetValue();
                //让球数
                string ratio = BetInfoHelper.GetRatio(xeBetInfo).GetValue();
                //胜
                StackPanel sp_R1_C4 = new StackPanel();
                //sp_R1_C4.Background = new SolidColorBrush(Colors.LightGray);
                sp_R1_C4.Orientation = Orientation.Horizontal;
                sp_R1_C4.FlowDirection = FlowDirection.RightToLeft;
                sp_R1_C4.Width = 90;
                TextBlock tbl_RH = new TextBlock();
                string ior_RH = BetInfoHelper.GetIOR_RH(xeBetInfo).GetValue();
                tbl_RH.Text = ior_RH;
                tbl_RH.Width = 35;
                sp_R1_C4.Children.Add(tbl_RH);
                if (!string.IsNullOrWhiteSpace(ratio) && strong == "H")
                {
                    TextBlock tbl_ratio = new TextBlock();
                    tbl_ratio.Text = ratio;
                    tbl_ratio.Width = 55;
                    tbl_ratio.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A4B9"));
                    sp_R1_C4.Children.Add(tbl_ratio);
                }
                XElement xeBetContentH = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentH.SetElementValue("BetText", string.Format("{0} @{1}", xeMatch.Element("HTeam").GetValue(), ior_RH));
                xeBetContentH.SetElementValue("BetContent", string.Format("{0}@{1}", "H", ior_RH));
                xeBetContentH.SetElementValue("RatioH", strong == "H" ? ratio : "");
                xeBetContentH.SetElementValue("RatioC", strong == "C" ? ratio : "");
                xeBetContentH.SetElementValue("BetIor", ior_RH);
                //tbl_MatchDate.Text = Utils.ConvertInt2DateTime(xeMatch.Element("MatchDate").GetValue(0D)).ToString("HH:mm");

                ToggleButton tgbtn_R1_C4 = new ToggleButton();
                tgbtn_R1_C4.Tag = xeBetContentH;
                tgbtn_R1_C4.Height = 30;
                tgbtn_R1_C4.Content = sp_R1_C4;
                tgbtn_R1_C4.Click += new RoutedEventHandler(TgButton_Click);
                Grid.SetRow(tgbtn_R1_C4, rowIndex);
                Grid.SetColumn(tgbtn_R1_C4, 3);
                grid.Children.Add(tgbtn_R1_C4);

                //负
                StackPanel sp_R2_C4 = new StackPanel();
                //sp_R2_C4.Background = new SolidColorBrush(Colors.LightGray);
                sp_R2_C4.Orientation = Orientation.Horizontal;
                sp_R2_C4.FlowDirection = FlowDirection.RightToLeft;
                sp_R2_C4.Width = 90;
                TextBlock tbl_RC = new TextBlock();
                string ior_RC = BetInfoHelper.GetIOR_RC(xeBetInfo).GetValue();
                tbl_RC.Text = ior_RC;
                tbl_RC.Width = 35;
                sp_R2_C4.Children.Add(tbl_RC);

                if (!string.IsNullOrWhiteSpace(ratio) && strong == "C")
                {
                    TextBlock tbl_ratio = new TextBlock();
                    tbl_ratio.Text = ratio;
                    tbl_ratio.Width = 55;
                    tbl_ratio.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A4B9"));
                    sp_R2_C4.Children.Add(tbl_ratio);
                }
                XElement xeBetContentC = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentC.SetElementValue("BetText", string.Format("{0} @{1}", xeMatch.Element("CTeam").GetValue(), ior_RC));
                xeBetContentC.SetElementValue("BetContent", string.Format("{0}@{1}", "C", ior_RC));
                xeBetContentC.SetElementValue("RatioH", strong == "H" ? ratio : "");
                xeBetContentC.SetElementValue("RatioC", strong == "C" ? ratio : "");
                xeBetContentC.SetElementValue("BetIor", ior_RC);
                ToggleButton tgbtn_R2_C4 = new ToggleButton();
                tgbtn_R2_C4.Tag = xeBetContentC;
                tgbtn_R2_C4.Height = 30;
                tgbtn_R2_C4.Content = sp_R2_C4;
                tgbtn_R2_C4.Click += new RoutedEventHandler(TgButton_Click);
                Grid.SetRow(tgbtn_R2_C4, rowIndex + 1);
                Grid.SetColumn(tgbtn_R2_C4, 3);
                grid.Children.Add(tgbtn_R2_C4);

                //更多
                Border grid_R3_C4 = new Border();
                Grid.SetRow(grid_R3_C4, rowIndex + 2);
                Grid.SetColumn(grid_R3_C4, 3);
                Grid.SetColumnSpan(grid_R3_C4, 3);
                grid.Children.Add(grid_R3_C4);
            }
            else
            {
                //空的
                //胜
                Border grid_R1_C4 = new Border();
                Grid.SetRow(grid_R1_C4, rowIndex);
                Grid.SetColumn(grid_R1_C4, 3);
                grid.Children.Add(grid_R1_C4);

                //负
                Border grid_R2_C4 = new Border();
                Grid.SetRow(grid_R2_C4, rowIndex + 1);
                Grid.SetColumn(grid_R2_C4, 3);
                grid.Children.Add(grid_R2_C4);

                //更多
                Border grid_R3_C4 = new Border();
                Grid.SetRow(grid_R3_C4, rowIndex + 2);
                Grid.SetColumn(grid_R3_C4, 3);
                Grid.SetColumnSpan(grid_R3_C4, 3);
                grid.Children.Add(grid_R3_C4);
            }
            #endregion
            #region 全场大小球
            xeBetInfo = xeMatch.Element("Odds").Element("MatchContent").Elements("BetInfo").FirstOrDefault(c => c.Element("BetType").GetValue() == "14");
            if (xeBetInfo != null)
            {
                //大球
                StackPanel sp_R1_C5 = new StackPanel();
                //sp_R1_C4.Background = new SolidColorBrush(Colors.LightGray);
                sp_R1_C5.Orientation = Orientation.Horizontal;
                sp_R1_C5.FlowDirection = FlowDirection.RightToLeft;
                sp_R1_C5.Width = 120;
                //大球赔率
                TextBlock tbl_OUO = new TextBlock();
                string ior_OUO = BetInfoHelper.GetIOR_OUO(xeBetInfo).GetValue();
                tbl_OUO.Text = ior_OUO;
                tbl_OUO.Width = 35;
                sp_R1_C5.Children.Add(tbl_OUO);
                //大球数
                TextBlock tbl_ratio_O = new TextBlock();
                string ratio_O = BetInfoHelper.GetRatio_O(xeBetInfo).GetValue();
                tbl_ratio_O.Text = LocalizationHelper.GetRes("Enum.OU.O") + ratio_O;
                tbl_ratio_O.Width = 85;
                tbl_ratio_O.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A4B9"));
                sp_R1_C5.Children.Add(tbl_ratio_O);

                XElement xeBetContentO = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentO.SetElementValue("BetText", string.Format("{2} {0} @{1}", ratio_O, ior_OUO, LocalizationHelper.GetRes("Enum.OU.O")));
                xeBetContentO.SetElementValue("BetContent", string.Format("{0}@{1}", "O", ior_OUO));
                xeBetContentO.SetElementValue("BetIor", ior_OUO);
                //tbl_MatchDate.Text = Utils.ConvertInt2DateTime(xeMatch.Element("MatchDate").GetValue(0D)).ToString("HH:mm");

                ToggleButton tgbtn_R1_C5 = new ToggleButton();
                tgbtn_R1_C5.Tag = xeBetContentO;
                tgbtn_R1_C5.Height = 30;
                tgbtn_R1_C5.Content = sp_R1_C5;
                tgbtn_R1_C5.Click += new RoutedEventHandler(TgButton_Click);
                Grid.SetRow(tgbtn_R1_C5, rowIndex);
                Grid.SetColumn(tgbtn_R1_C5, 4);
                grid.Children.Add(tgbtn_R1_C5);

                //小球
                StackPanel sp_R2_C5 = new StackPanel();
                sp_R2_C5.Orientation = Orientation.Horizontal;
                sp_R2_C5.FlowDirection = FlowDirection.RightToLeft;
                sp_R2_C5.Width = 120;
                //小球赔率
                TextBlock tbl_OUU = new TextBlock();
                string ior_OUU = BetInfoHelper.GetIOR_OUU(xeBetInfo).GetValue();
                tbl_OUU.Text = ior_OUU;
                tbl_OUU.Width = 35;
                sp_R2_C5.Children.Add(tbl_OUU);
                //小球数
                TextBlock tbl_ratio_U = new TextBlock();
                string ratio_U = BetInfoHelper.GetRatio_O(xeBetInfo).GetValue();
                tbl_ratio_U.Text = LocalizationHelper.GetRes("Enum.OU.U") + ratio_U;
                tbl_ratio_U.Width = 85;
                tbl_ratio_U.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A4B9"));
                sp_R2_C5.Children.Add(tbl_ratio_U);
                XElement xeBetContentU = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentU.SetElementValue("BetText", string.Format("{2} {0} @{1}", ratio_U, ior_OUU, LocalizationHelper.GetRes("Enum.OU.U")));
                xeBetContentU.SetElementValue("BetContent", string.Format("{0}@{1}", "U", ior_OUU));
                xeBetContentU.SetElementValue("BetIor", ior_OUU);
                ToggleButton tgbtn_R2_C5 = new ToggleButton();
                tgbtn_R2_C5.Tag = xeBetContentU;
                tgbtn_R2_C5.Height = 30;
                tgbtn_R2_C5.Content = sp_R2_C5;
                tgbtn_R2_C5.Click += new RoutedEventHandler(TgButton_Click);
                Grid.SetRow(tgbtn_R2_C5, rowIndex + 1);
                Grid.SetColumn(tgbtn_R2_C5, 4);
                grid.Children.Add(tgbtn_R2_C5);
            }
            else
            {
                //空的
                //大
                Border grid_R1_C5 = new Border();
                Grid.SetRow(grid_R1_C5, rowIndex);
                Grid.SetColumn(grid_R1_C5, 4);
                grid.Children.Add(grid_R1_C5);

                //下
                Border grid_R2_C5 = new Border();
                Grid.SetRow(grid_R2_C5, rowIndex + 1);
                Grid.SetColumn(grid_R2_C5, 4);
                grid.Children.Add(grid_R2_C5);
            }
            #endregion
            #region 单双
            xeBetInfo = xeMatch.Element("Odds").Element("MatchContent").Elements("BetInfo").FirstOrDefault(c => c.Element("BetType").GetValue() == "16");
            if (xeBetInfo != null)
            {
                //单
                StackPanel sp_R1_C6 = new StackPanel();
                sp_R1_C6.Orientation = Orientation.Horizontal;
                sp_R1_C6.FlowDirection = FlowDirection.RightToLeft;
                sp_R1_C6.Width = 98;
                //单赔率
                TextBlock tbl_EOO = new TextBlock();
                string ior_EOO = BetInfoHelper.GetIOR_EOO(xeBetInfo).GetValue();
                tbl_EOO.Text = ior_EOO;
                tbl_EOO.Width = 35;
                sp_R1_C6.Children.Add(tbl_EOO);
                //单
                TextBlock tbl_strEOO = new TextBlock();
                tbl_strEOO.Text = LocalizationHelper.GetRes("Enum.EO.O");
                tbl_strEOO.Width = 60;
                tbl_strEOO.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A4B9"));
                sp_R1_C6.Children.Add(tbl_strEOO);

                XElement xeBetContentO = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentO.SetElementValue("BetText", string.Format("{0} @{1}", LocalizationHelper.GetRes("Enum.EO.O"), ior_EOO));
                xeBetContentO.SetElementValue("BetContent", string.Format("{0}@{1}", "O", ior_EOO));
                xeBetContentO.SetElementValue("BetIor", ior_EOO);

                ToggleButton tgbtn_R1_C6 = new ToggleButton();
                tgbtn_R1_C6.Tag = xeBetContentO;
                tgbtn_R1_C6.Height = 30;
                tgbtn_R1_C6.Content = sp_R1_C6;
                tgbtn_R1_C6.Click += new RoutedEventHandler(TgButton_Click);
                Grid.SetRow(tgbtn_R1_C6, rowIndex);
                Grid.SetColumn(tgbtn_R1_C6, 5);
                grid.Children.Add(tgbtn_R1_C6);

                //双
                StackPanel sp_R2_C6 = new StackPanel();
                sp_R2_C6.Orientation = Orientation.Horizontal;
                sp_R2_C6.FlowDirection = FlowDirection.RightToLeft;
                sp_R2_C6.Width = 98;
                //双赔率
                TextBlock tbl_EOE = new TextBlock();
                string ior_EOE = BetInfoHelper.GetIOR_EOE(xeBetInfo).GetValue();
                tbl_EOE.Text = ior_EOE;
                tbl_EOE.Width = 35;
                sp_R2_C6.Children.Add(tbl_EOE);
                //双
                TextBlock tbl_strEOE = new TextBlock();
                tbl_strEOE.Text = LocalizationHelper.GetRes("Enum.EO.E");
                tbl_strEOE.Width = 60;
                tbl_strEOE.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A4B9"));
                sp_R2_C6.Children.Add(tbl_strEOE);
                XElement xeBetContentU = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentU.SetElementValue("BetText", string.Format("{0} @{1}", LocalizationHelper.GetRes("Enum.EO.E"), ior_EOE));
                xeBetContentU.SetElementValue("BetContent", string.Format("{0}@{1}", "E", ior_EOE));
                xeBetContentU.SetElementValue("BetIor", ior_EOE);
                ToggleButton tgbtn_R2_C6 = new ToggleButton();
                tgbtn_R2_C6.Tag = xeBetContentU;
                tgbtn_R2_C6.Height = 30;
                tgbtn_R2_C6.Content = sp_R2_C6;
                tgbtn_R2_C6.Click += new RoutedEventHandler(TgButton_Click);
                Grid.SetRow(tgbtn_R2_C6, rowIndex + 1);
                Grid.SetColumn(tgbtn_R2_C6, 5);
                grid.Children.Add(tgbtn_R2_C6);
            }
            else
            {
                //空的
                //单
                Border grid_R1_C6 = new Border();
                Grid.SetRow(grid_R1_C6, rowIndex);
                Grid.SetColumn(grid_R1_C6, 5);
                grid.Children.Add(grid_R1_C6);

                //双
                Border grid_R2_C6 = new Border();
                Grid.SetRow(grid_R2_C6, rowIndex + 1);
                Grid.SetColumn(grid_R2_C6, 5);
                grid.Children.Add(grid_R2_C6);
            }
            #endregion
            #region 半场独赢
            xeBetInfo = xeMatch.Element("Odds").Element("MatchContent").Elements("BetInfo").FirstOrDefault(c => c.Element("BetType").GetValue() == "11");
            if (xeBetInfo != null)
            {
                //胜
                ToggleButton tgbtn_R1_C7 = new ToggleButton();

                string ior_HMH = BetInfoHelper.GetIOR_HMH(xeBetInfo).GetValue();
                tgbtn_R1_C7.Content = ior_HMH;
                XElement xeBetContentH = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentH.SetElementValue("BetText", string.Format("{0} @{1}", xeMatch.Element("HTeam").GetValue(), ior_HMH));
                xeBetContentH.SetElementValue("BetContent", string.Format("{0}@{1}", "H", ior_HMH));
                xeBetContentH.SetElementValue("BetIor", ior_HMH);
                //tbl_MatchDate.Text = Utils.ConvertInt2DateTime(xeMatch.Element("MatchDate").GetValue(0D)).ToString("HH:mm");
                tgbtn_R1_C7.Tag = xeBetContentH;
                tgbtn_R1_C7.Height = 30;
                tgbtn_R1_C7.Click += new RoutedEventHandler(TgButton_Click);

                Grid.SetRow(tgbtn_R1_C7, rowIndex);
                Grid.SetColumn(tgbtn_R1_C7, 6);
                grid.Children.Add(tgbtn_R1_C7);

                //负
                ToggleButton tgbtn_R2_C7 = new ToggleButton();
                string ior_HMC = BetInfoHelper.GetIOR_HMC(xeBetInfo).GetValue();
                tgbtn_R2_C7.Content = ior_HMC;
                XElement xeBetContentC = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentC.SetElementValue("BetText", string.Format("{0} @{1}", xeMatch.Element("CTeam").GetValue(), ior_HMC));
                xeBetContentC.SetElementValue("BetContent", string.Format("{0}@{1}", "C", ior_HMC));
                xeBetContentC.SetElementValue("BetIor", ior_HMC);
                //tbl_MatchDate.Text = Utils.ConvertInt2DateTime(xeMatch.Element("MatchDate").GetValue(0D)).ToString("HH:mm");
                tgbtn_R2_C7.Tag = xeBetContentC;
                tgbtn_R2_C7.Height = 30;
                tgbtn_R2_C7.Click += new RoutedEventHandler(TgButton_Click);

                Grid.SetRow(tgbtn_R2_C7, rowIndex + 1);
                Grid.SetColumn(tgbtn_R2_C7, 6);
                grid.Children.Add(tgbtn_R2_C7);

                //平
                ToggleButton tgbtn_R3_C7 = new ToggleButton();
                string ior_HMN = BetInfoHelper.GetIOR_HMN(xeBetInfo).GetValue();
                tgbtn_R3_C7.Content = ior_HMN;
                XElement xeBetContent = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContent.SetElementValue("BetText", string.Format("{0} @{1}", LocalizationHelper.GetRes("Enum.HCN.N"), ior_HMN));
                xeBetContent.SetElementValue("BetContent", string.Format("{0}@{1}", "N", ior_HMN));
                xeBetContent.SetElementValue("BetIor", ior_HMN);
                //tbl_MatchDate.Text = Utils.ConvertInt2DateTime(xeMatch.Element("MatchDate").GetValue(0D)).ToString("HH:mm");
                tgbtn_R3_C7.Tag = xeBetContent;
                tgbtn_R3_C7.Height = 30;
                tgbtn_R3_C7.Click += new RoutedEventHandler(TgButton_Click);

                Grid.SetRow(tgbtn_R3_C7, rowIndex + 2);
                Grid.SetColumn(tgbtn_R3_C7, 6);
                grid.Children.Add(tgbtn_R3_C7);
            }
            else
            {
                //空的
                //胜
                Border grid_R1_C7 = new Border();
                Grid.SetRow(grid_R1_C7, rowIndex);
                Grid.SetColumn(grid_R1_C7, 6);
                grid.Children.Add(grid_R1_C7);

                //负
                Border grid_R2_C7 = new Border();
                Grid.SetRow(grid_R2_C7, rowIndex + 1);
                Grid.SetColumn(grid_R2_C7, 6);
                grid.Children.Add(grid_R2_C7);

                //平
                Border grid_R3_C7 = new Border();
                Grid.SetRow(grid_R3_C7, rowIndex + 2);
                Grid.SetColumn(grid_R3_C7, 6);
                grid.Children.Add(grid_R3_C7);
            }
            #endregion
            #region 半场让球
            xeBetInfo = xeMatch.Element("Odds").Element("MatchContent").Elements("BetInfo").FirstOrDefault(c => c.Element("BetType").GetValue() == "13");
            if (xeBetInfo != null)
            {
                //让球方
                string hstrong = BetInfoHelper.GetHStrong(xeBetInfo).GetValue();
                //让球数
                string hratio = BetInfoHelper.GetHRatio(xeBetInfo).GetValue();
                //胜
                StackPanel sp_R1_C8 = new StackPanel();
                //sp_R1_C4.Background = new SolidColorBrush(Colors.LightGray);
                sp_R1_C8.Orientation = Orientation.Horizontal;
                sp_R1_C8.FlowDirection = FlowDirection.RightToLeft;
                sp_R1_C8.Width = 90;
                TextBlock tbl_HRH = new TextBlock();
                string ior_HRH = BetInfoHelper.GetIOR_HRH(xeBetInfo).GetValue();
                tbl_HRH.Text = ior_HRH;
                tbl_HRH.Width = 35;
                sp_R1_C8.Children.Add(tbl_HRH);
                if (!string.IsNullOrWhiteSpace(hratio) && hstrong == "H")
                {
                    TextBlock tbl_ratio = new TextBlock();
                    tbl_ratio.Text = hratio;
                    tbl_ratio.Width = 55;
                    tbl_ratio.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A4B9"));
                    sp_R1_C8.Children.Add(tbl_ratio);
                }
                XElement xeBetContentH = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentH.SetElementValue("BetText", string.Format("{0} @{1}", xeMatch.Element("HTeam").GetValue(), ior_HRH));
                xeBetContentH.SetElementValue("BetContent", string.Format("{0}@{1}", "H", ior_HRH));
                xeBetContentH.SetElementValue("RatioH", hstrong == "H" ? hratio : "");
                xeBetContentH.SetElementValue("RatioC", hstrong == "C" ? hratio : "");
                xeBetContentH.SetElementValue("BetIor", ior_HRH);
                //tbl_MatchDate.Text = Utils.ConvertInt2DateTime(xeMatch.Element("MatchDate").GetValue(0D)).ToString("HH:mm");

                ToggleButton tgbtn_R1_C8 = new ToggleButton();
                tgbtn_R1_C8.Tag = xeBetContentH;
                tgbtn_R1_C8.Height = 30;
                tgbtn_R1_C8.Content = sp_R1_C8;
                tgbtn_R1_C8.Click += new RoutedEventHandler(TgButton_Click);
                Grid.SetRow(tgbtn_R1_C8, rowIndex);
                Grid.SetColumn(tgbtn_R1_C8, 7);
                grid.Children.Add(tgbtn_R1_C8);

                //负
                StackPanel sp_R2_C8 = new StackPanel();
                //sp_R2_C4.Background = new SolidColorBrush(Colors.LightGray);
                sp_R2_C8.Orientation = Orientation.Horizontal;
                sp_R2_C8.FlowDirection = FlowDirection.RightToLeft;
                sp_R2_C8.Width = 90;
                TextBlock tbl_HRC = new TextBlock();
                string ior_HRC = BetInfoHelper.GetIOR_HRC(xeBetInfo).GetValue();
                tbl_HRC.Text = ior_HRC;
                tbl_HRC.Width = 35;
                sp_R2_C8.Children.Add(tbl_HRC);

                if (!string.IsNullOrWhiteSpace(hratio) && hstrong == "C")
                {
                    TextBlock tbl_ratio = new TextBlock();
                    tbl_ratio.Text = hratio;
                    tbl_ratio.Width = 55;
                    tbl_ratio.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A4B9"));
                    sp_R2_C8.Children.Add(tbl_ratio);
                }
                XElement xeBetContentC = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentC.SetElementValue("BetText", string.Format("{0} @{1}", xeMatch.Element("CTeam").GetValue(), ior_HRC));
                xeBetContentC.SetElementValue("BetContent", string.Format("{0}@{1}", "C", ior_HRC));
                xeBetContentC.SetElementValue("RatioH", hstrong == "H" ? hratio : "");
                xeBetContentC.SetElementValue("RatioC", hstrong == "C" ? hratio : "");
                xeBetContentC.SetElementValue("BetIor", ior_HRC);
                ToggleButton tgbtn_R2_C8 = new ToggleButton();
                tgbtn_R2_C8.Tag = xeBetContentC;
                tgbtn_R2_C8.Height = 30;
                tgbtn_R2_C8.Content = sp_R2_C8;
                tgbtn_R2_C8.Click += new RoutedEventHandler(TgButton_Click);
                Grid.SetRow(tgbtn_R2_C8, rowIndex + 1);
                Grid.SetColumn(tgbtn_R2_C8, 7);
                grid.Children.Add(tgbtn_R2_C8);

                //更多
                Grid grid_R3_C8 = new Grid();
                Grid.SetRow(grid_R3_C8, rowIndex + 2);
                Grid.SetColumn(grid_R3_C8, 7);
                Grid.SetColumnSpan(grid_R3_C8, 2);
                grid.Children.Add(grid_R3_C8);
            }
            else
            {
                //空的
                //胜
                Border grid_R1_C8 = new Border();
                Grid.SetRow(grid_R1_C8, rowIndex);
                Grid.SetColumn(grid_R1_C8, 7);
                grid.Children.Add(grid_R1_C8);

                //负
                Border grid_R2_C8 = new Border();
                Grid.SetRow(grid_R2_C8, rowIndex + 1);
                Grid.SetColumn(grid_R2_C8, 7);
                grid.Children.Add(grid_R2_C8);

                //更多
                Border grid_R3_C8 = new Border();
                Grid.SetRow(grid_R3_C8, rowIndex + 2);
                Grid.SetColumn(grid_R3_C8, 7);
                Grid.SetColumnSpan(grid_R3_C8, 2);
                grid.Children.Add(grid_R3_C8);
            }
            #endregion
            #region 半场大小球
            xeBetInfo = xeMatch.Element("Odds").Element("MatchContent").Elements("BetInfo").FirstOrDefault(c => c.Element("BetType").GetValue() == "15");
            if (xeBetInfo != null)
            {
                //大球
                StackPanel sp_R1_C9 = new StackPanel();
                //sp_R1_C4.Background = new SolidColorBrush(Colors.LightGray);
                sp_R1_C9.Orientation = Orientation.Horizontal;
                sp_R1_C9.FlowDirection = FlowDirection.RightToLeft;
                sp_R1_C9.Width = 120;
                //大球赔率
                TextBlock tbl_HOUO = new TextBlock();
                string ior_HOUO = BetInfoHelper.GetIOR_OUO(xeBetInfo).GetValue();
                tbl_HOUO.Text = ior_HOUO;
                tbl_HOUO.Width = 35;
                sp_R1_C9.Children.Add(tbl_HOUO);
                //大球数
                TextBlock tbl_hratio_O = new TextBlock();
                string hratio_O = BetInfoHelper.GetRatio_O(xeBetInfo).GetValue();
                tbl_hratio_O.Text = LocalizationHelper.GetRes("Enum.OU.O") + hratio_O;
                tbl_hratio_O.Width = 85;
                tbl_hratio_O.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A4B9"));
                sp_R1_C9.Children.Add(tbl_hratio_O);

                XElement xeBetContentO = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentO.SetElementValue("BetText", string.Format("{2} {0} @{1}", hratio_O, ior_HOUO, LocalizationHelper.GetRes("Enum.OU.O")));
                xeBetContentO.SetElementValue("BetContent", string.Format("{0}@{1}", "O", ior_HOUO));
                xeBetContentO.SetElementValue("BetIor", ior_HOUO);
                //tbl_MatchDate.Text = Utils.ConvertInt2DateTime(xeMatch.Element("MatchDate").GetValue(0D)).ToString("HH:mm");

                ToggleButton tgbtn_R1_C9 = new ToggleButton();
                tgbtn_R1_C9.Tag = xeBetContentO;
                tgbtn_R1_C9.Height = 30;
                tgbtn_R1_C9.Content = sp_R1_C9;
                tgbtn_R1_C9.Click += new RoutedEventHandler(TgButton_Click);
                Grid.SetRow(tgbtn_R1_C9, rowIndex);
                Grid.SetColumn(tgbtn_R1_C9, 8);
                grid.Children.Add(tgbtn_R1_C9);

                //小球
                StackPanel sp_R2_C9 = new StackPanel();
                sp_R2_C9.Orientation = Orientation.Horizontal;
                sp_R2_C9.FlowDirection = FlowDirection.RightToLeft;
                sp_R2_C9.Width = 120;
                //小球赔率
                TextBlock tbl_HOUU = new TextBlock();
                string ior_HOUU = BetInfoHelper.GetIOR_OUU(xeBetInfo).GetValue();
                tbl_HOUU.Text = ior_HOUU;
                tbl_HOUU.Width = 35;
                sp_R2_C9.Children.Add(tbl_HOUU);
                //小球数
                TextBlock tbl_hratio_U = new TextBlock();
                string hratio_U = BetInfoHelper.GetRatio_O(xeBetInfo).GetValue();
                tbl_hratio_U.Text = LocalizationHelper.GetRes("Enum.OU.U") + hratio_U;
                tbl_hratio_U.Width = 85;
                tbl_hratio_U.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A4B9"));
                sp_R2_C9.Children.Add(tbl_hratio_U);
                XElement xeBetContentU = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentU.SetElementValue("BetText", string.Format("{2} {0} @{1}", hratio_U, ior_HOUU, LocalizationHelper.GetRes("Enum.OU.U")));
                xeBetContentU.SetElementValue("BetContent", string.Format("{0}@{1}", "U", ior_HOUU));
                xeBetContentU.SetElementValue("BetIor", ior_HOUU);
                ToggleButton tgbtn_R2_C9 = new ToggleButton();
                tgbtn_R2_C9.Tag = xeBetContentU;
                tgbtn_R2_C9.Height = 30;
                tgbtn_R2_C9.Content = sp_R2_C9;
                tgbtn_R2_C9.Click += new RoutedEventHandler(TgButton_Click);
                Grid.SetRow(tgbtn_R2_C9, rowIndex + 1);
                Grid.SetColumn(tgbtn_R2_C9, 8);
                grid.Children.Add(tgbtn_R2_C9);
            }
            else
            {
                //空的
                //大
                Border grid_R1_C9 = new Border();
                Grid.SetRow(grid_R1_C9, rowIndex);
                Grid.SetColumn(grid_R1_C9, 8);
                grid.Children.Add(grid_R1_C9);

                //下
                Border grid_R2_C9 = new Border();
                Grid.SetRow(grid_R2_C9, rowIndex + 1);
                Grid.SetColumn(grid_R2_C9, 8);
                grid.Children.Add(grid_R2_C9);
            }
            #endregion
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Scrollbar.VerticalOffset == 0 && e.Delta < 0)
            {
                Next();
            }
        }
    }
}
