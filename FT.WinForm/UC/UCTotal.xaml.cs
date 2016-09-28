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
    public partial class UCTotal : UserControl
    {
        public List<XElement> xeSelectBet = new List<XElement>();
        private XElement xeRoot;
        Action<List<XElement>> ItemSelect;
        public UCTotal(Action<List<XElement>> _ItemSelect)
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
                param.Add("bettype", "2");
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
                ColumnDefinition col_3 = new ColumnDefinition();//
                ColumnDefinition col_4 = new ColumnDefinition();//
                ColumnDefinition col_5 = new ColumnDefinition();//
                ColumnDefinition col_6 = new ColumnDefinition();//
                col_1.Width = new GridLength(50, GridUnitType.Pixel);
                col_2.Width = new GridLength(140, GridUnitType.Pixel);
                col_3.Width = new GridLength(100, GridUnitType.Pixel);
                col_4.Width = new GridLength(100, GridUnitType.Pixel);
                col_5.Width = new GridLength(100, GridUnitType.Pixel);
                col_6.Width = new GridLength(100, GridUnitType.Pixel);
                grid.ColumnDefinitions.Add(col_1);
                grid.ColumnDefinitions.Add(col_2);
                grid.ColumnDefinitions.Add(col_3);
                grid.ColumnDefinitions.Add(col_4);
                grid.ColumnDefinitions.Add(col_5);
                grid.ColumnDefinitions.Add(col_6);
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
            Grid.SetColumn(tbl_MatchDate, 0);
            grid.Children.Add(tbl_MatchDate);
            #endregion

            #region 比赛双方
            TextBlock tbl_MatchTeamH = new TextBlock();
            TextBlock tbl_MatchTeamC = new TextBlock();
            tbl_MatchTeamH.Height = 25;
            tbl_MatchTeamC.Height = 25;
            tbl_MatchTeamH.Text = xeMatch.Element("HTeam").GetValue();
            tbl_MatchTeamH.ToolTip = xeMatch.Element("HTeam").GetValue();
            tbl_MatchTeamC.Text = xeMatch.Element("CTeam").GetValue();
            tbl_MatchTeamC.ToolTip = xeMatch.Element("CTeam").GetValue();
            StackPanel spTeam = new StackPanel();
            spTeam.HorizontalAlignment = HorizontalAlignment.Left;
            spTeam.Orientation = Orientation.Vertical;
            spTeam.Children.Add(tbl_MatchTeamH);
            spTeam.Children.Add(tbl_MatchTeamC);
            //球队
            Grid.SetRow(spTeam, rowIndex);
            Grid.SetColumn(spTeam, 1);
            grid.Children.Add(spTeam);
            #endregion
            #region 比分
            XElement xeBetInfo = xeMatch.Element("Odds").Element("MatchContent").Elements("BetInfo").FirstOrDefault(c => c.Element("BetType").GetValue() == "20");
            if (xeBetInfo != null)
            {
                int _col = 2;
                Dictionary<string, string> dic = BetInfoHelper.GetIOR_Total(xeBetInfo);
                foreach (string key in dic.Keys)
                {
                    string ior_total = dic[key];
                    ToggleButton tgbtn = new ToggleButton();
                    tgbtn.Content = ior_total;
                    XElement xeBetContent = BetInfoHelper.GetBetContent(xeMatch, xeBetInfo);
                    xeBetContent.SetElementValue("BetText", string.Format("{0} @{1}", key, ior_total));
                    xeBetContent.SetElementValue("BetContent", string.Format("{0}@{1}", key, ior_total));
                    xeBetContent.SetElementValue("BetIor", ior_total);
                    tgbtn.Tag = xeBetContent;
                    tgbtn.Height = 50;
                    tgbtn.Click += new RoutedEventHandler(TgButton_Click);
                    Grid.SetRow(tgbtn, rowIndex);
                    Grid.SetColumn(tgbtn, _col++);
                    grid.Children.Add(tgbtn);
                }
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
