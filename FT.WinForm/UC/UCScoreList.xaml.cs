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
    /// UCScoreList.xaml 的交互逻辑
    /// </summary>
    public partial class UCScoreList : UserControl
    {
        public List<XElement> xeSelectBet = new List<XElement>();
        private XElement xeRoot;
        Action<List<XElement>> ItemSelect;
        public UCScoreList(Action<List<XElement>> _ItemSelect)
        {
            InitializeComponent();
            ItemSelect = _ItemSelect;
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
        public void ClearSelect()
        {
            xeSelectBet = new List<XElement>();
            UIHelper.UnCheckToggleButton(dymSP);
        }
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
                param.Add("bettype", "3");
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
                ColumnDefinition col_3 = new ColumnDefinition();//1:0
                ColumnDefinition col_4 = new ColumnDefinition();//
                ColumnDefinition col_5 = new ColumnDefinition();//
                ColumnDefinition col_6 = new ColumnDefinition();//
                ColumnDefinition col_7 = new ColumnDefinition();//
                ColumnDefinition col_8 = new ColumnDefinition();//
                ColumnDefinition col_9 = new ColumnDefinition();//
                ColumnDefinition col_10 = new ColumnDefinition();//
                ColumnDefinition col_11 = new ColumnDefinition();//
                ColumnDefinition col_12 = new ColumnDefinition();//
                ColumnDefinition col_13 = new ColumnDefinition();//
                ColumnDefinition col_14 = new ColumnDefinition();//
                ColumnDefinition col_15 = new ColumnDefinition();//
                ColumnDefinition col_16 = new ColumnDefinition();//
                ColumnDefinition col_17 = new ColumnDefinition();//
                ColumnDefinition col_18 = new ColumnDefinition();//其他
                col_1.Width = new GridLength(50, GridUnitType.Pixel);
                col_2.Width = new GridLength(140, GridUnitType.Pixel);
                col_3.Width = new GridLength(45, GridUnitType.Pixel);
                col_4.Width = new GridLength(45, GridUnitType.Pixel);
                col_5.Width = new GridLength(45, GridUnitType.Pixel);
                col_6.Width = new GridLength(45, GridUnitType.Pixel);
                col_7.Width = new GridLength(45, GridUnitType.Pixel);
                col_8.Width = new GridLength(45, GridUnitType.Pixel);
                col_9.Width = new GridLength(45, GridUnitType.Pixel);
                col_10.Width = new GridLength(45, GridUnitType.Pixel);
                col_11.Width = new GridLength(45, GridUnitType.Pixel);
                col_12.Width = new GridLength(45, GridUnitType.Pixel);
                col_13.Width = new GridLength(45, GridUnitType.Pixel);
                col_14.Width = new GridLength(45, GridUnitType.Pixel);
                col_15.Width = new GridLength(45, GridUnitType.Pixel);
                col_16.Width = new GridLength(45, GridUnitType.Pixel);
                col_17.Width = new GridLength(45, GridUnitType.Pixel);
                col_18.Width = new GridLength(45, GridUnitType.Pixel);
                grid.ColumnDefinitions.Add(col_1);
                grid.ColumnDefinitions.Add(col_2);
                grid.ColumnDefinitions.Add(col_3);
                grid.ColumnDefinitions.Add(col_4);
                grid.ColumnDefinitions.Add(col_5);
                grid.ColumnDefinitions.Add(col_6);
                grid.ColumnDefinitions.Add(col_7);
                grid.ColumnDefinitions.Add(col_8);
                grid.ColumnDefinitions.Add(col_9);
                grid.ColumnDefinitions.Add(col_10);
                grid.ColumnDefinitions.Add(col_11);
                grid.ColumnDefinitions.Add(col_12);
                grid.ColumnDefinitions.Add(col_13);
                grid.ColumnDefinitions.Add(col_14);
                grid.ColumnDefinitions.Add(col_15);
                grid.ColumnDefinitions.Add(col_16);
                grid.ColumnDefinitions.Add(col_17);
                grid.ColumnDefinitions.Add(col_18);
                #endregion
                #region 初始化行数，一场比赛占2行
                RowDefinition row_1 = new RowDefinition();
                RowDefinition row_2 = new RowDefinition();
                grid.RowDefinitions.Add(row_1);
                grid.RowDefinitions.Add(row_2);
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
            Grid.SetRowSpan(tbl_MatchDate, 2);
            Grid.SetColumn(tbl_MatchDate, 0);
            grid.Children.Add(tbl_MatchDate);
            #endregion

            #region 比赛双方
            TextBlock tbl_MatchTeamH = new TextBlock();
            TextBlock tbl_MatchTeamC = new TextBlock();
            tbl_MatchTeamH.Height = 30;
            tbl_MatchTeamC.Height = 30;
            tbl_MatchTeamH.Text = xeMatch.Element("HTeam").GetValue();
            tbl_MatchTeamH.ToolTip = xeMatch.Element("HTeam").GetValue();
            tbl_MatchTeamC.Text = xeMatch.Element("CTeam").GetValue();
            tbl_MatchTeamC.ToolTip = xeMatch.Element("CTeam").GetValue();
            //主
            Grid.SetRow(tbl_MatchTeamH, rowIndex);
            Grid.SetColumn(tbl_MatchTeamH, 1);
            grid.Children.Add(tbl_MatchTeamH);

            //客
            Grid.SetRow(tbl_MatchTeamC, rowIndex + 1);
            Grid.SetColumn(tbl_MatchTeamC, 1);
            grid.Children.Add(tbl_MatchTeamC);
            #endregion
            #region 比分
            XElement xeBetInfo = xeMatch.Element("Odds").Element("MatchContent").Elements("BetInfo").FirstOrDefault(c => c.Element("BetType").GetValue() == "30");
            if (xeBetInfo != null)
            {
                Dictionary<string, string> dic = BetInfoHelper.GetIOR_Score(xeBetInfo);
                //胜
                int _col = 2;
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        string key = i + ":" + j;
                        string ior_scoreH = dic[key];
                        ToggleButton tgbtn = new ToggleButton();
                        tgbtn.Content = ior_scoreH;
                        XElement xeBetContentH = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                        xeBetContentH.SetElementValue("BetText", string.Format("{0} @{1}", key, ior_scoreH));
                        xeBetContentH.SetElementValue("BetContent", string.Format("{0}@{1}", key, ior_scoreH));
                        xeBetContentH.SetElementValue("BetIor", ior_scoreH);
                        tgbtn.Tag = xeBetContentH;
                        tgbtn.Height = 30;
                        tgbtn.Click += new RoutedEventHandler(TgButton_Click);
                        Grid.SetRow(tgbtn, rowIndex);
                        Grid.SetColumn(tgbtn, _col++);
                        grid.Children.Add(tgbtn);
                    }
                }

                //负
                _col = 2;
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        string key = j + ":" + i;
                        string ior_scoreC = dic[key];
                        ToggleButton tgbtn = new ToggleButton();
                        tgbtn.Content = ior_scoreC;
                        XElement xeBetContentH = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                        xeBetContentH.SetElementValue("BetText", string.Format("{0} @{1}", key, ior_scoreC));
                        xeBetContentH.SetElementValue("BetContent", string.Format("{0}@{1}", key, ior_scoreC));
                        xeBetContentH.SetElementValue("BetIor", ior_scoreC);
                        tgbtn.Tag = xeBetContentH;
                        tgbtn.Height = 30;
                        tgbtn.Click += new RoutedEventHandler(TgButton_Click);
                        Grid.SetRow(tgbtn, rowIndex + 1);
                        Grid.SetColumn(tgbtn, _col++);
                        grid.Children.Add(tgbtn);
                    }
                }
                //平
                _col = 12;
                for (int i = 0; i < 5; i++)
                {
                    string key = i + ":" + i;
                    string ior_scoreN = dic[key];
                    ToggleButton tgbtn = new ToggleButton();
                    tgbtn.Content = ior_scoreN;
                    XElement xeBetContentH = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                    xeBetContentH.SetElementValue("BetText", string.Format("{0} @{1}", key, ior_scoreN));
                    xeBetContentH.SetElementValue("BetContent", string.Format("{0}@{1}", key, ior_scoreN));
                    xeBetContentH.SetElementValue("BetIor", ior_scoreN);
                    tgbtn.Tag = xeBetContentH;
                    tgbtn.Height = 30;
                    tgbtn.Click += new RoutedEventHandler(TgButton_Click);
                    Grid.SetRow(tgbtn, rowIndex);
                    Grid.SetRowSpan(tgbtn, 2);
                    Grid.SetColumn(tgbtn, _col++);
                    grid.Children.Add(tgbtn);
                }
                //其他
                string ior_scoreOVH = dic["OVH"];
                ToggleButton tgbtnOVH = new ToggleButton();
                tgbtnOVH.Content = ior_scoreOVH;
                XElement xeBetContentOVH = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                xeBetContentOVH.SetElementValue("BetText", string.Format("{0} @{1}", LocalizationHelper.GetRes("Enum.Score.OVH"), ior_scoreOVH));
                xeBetContentOVH.SetElementValue("BetContent", string.Format("{0}@{1}", "OVH", ior_scoreOVH));
                xeBetContentOVH.SetElementValue("BetIor", ior_scoreOVH);
                tgbtnOVH.Tag = xeBetContentOVH;
                tgbtnOVH.Height = 30;
                tgbtnOVH.Click += new RoutedEventHandler(TgButton_Click);
                Grid.SetRow(tgbtnOVH, rowIndex);
                Grid.SetRowSpan(tgbtnOVH, 2);
                Grid.SetColumn(tgbtnOVH, _col++);
                grid.Children.Add(tgbtnOVH);
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
