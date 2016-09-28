using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Xml.Linq;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Media;
using MahApps.Metro.Controls;
using System.Windows.Controls.Primitives;
using FT.WinForm.Login;
using FT.WinForm.UC;
using FT.Utility.Helper;
using FT.WinForm.Http;
using FT.Utility.ApiHelper;
using Newtonsoft.Json;
using System.Windows.Threading;
using FT.WinForm.Tools;
using System.Xml;

namespace FT.WinForm
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private UCStandardGrid ucStandardGrid;
        private UCScoreList ucScoreGrid;
        private UCHalfFinal ucHalfFinal;
        private UCTotal ucTotal;
        private UCMultiList ucMultiList;
        private const int BETSECOND = 15;
        //计时器
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }
        #region 初始化
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dockManager"></param>
        /// <param name="tebName"></param>
        /// <param name="tecNowTime"></param>
        /// <param name="openIndex">有首页权限是否默认打开通用首页</param>
        public void Initialize()
        {
            #region 绑定本地化资源字典
            //tblstrBalance.SetResourceReference(TextBlock.TextProperty, "Index.MyBalance");
            #endregion
            tblUserName.Text = UserLogin.CurrentUser.User.LoginName;
            tblBalance.Text = UserLogin.CurrentUser.User.BalanceAmount + "";
            InitLanguage();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);

            #region 升级程序自更新
            //Thread trdDownload = new Thread(new ThreadStart(LaunchUpdate.LauchUpdateFileInfo));
            //trdDownload.Start();
            #endregion
        }
        //void Rendered(object sender, EventArgs e)
        //{
        //    MessageBox.Show("123");
        //}
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //计时开始
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(10000000);   //时间间隔为一秒
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            IniMatchControl();
            ReloadData();
        }
        #endregion

        #region 注销系统
        //注销系统
        protected void MenuLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(LocalizationHelper.GetRes("str_Msg_Exit"), LocalizationHelper.GetRes("str_Msg_Msg"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                isExitBtn = true;
                Assembly asm = Assembly.GetEntryAssembly();
                string exeDir = System.IO.Path.GetDirectoryName(asm.Location);
                string startupUri = exeDir + @"\" + ConfigurationManager.AppSettings["StartupProgram"];
                Process.Start(startupUri);
                App.Current.Shutdown();
            }
        }
        #endregion
        #region 修改密码
        protected void MenuResetPwd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //UpdatePassword ue = new UpdatePassword();
                //ue.Show();
            }
            catch //(Exception ex)
            { }
        }
        #endregion
        #region 退出系统
        public static bool isExitBtn = false; //是否点击退出按钮退出
        protected void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isExitBtn)
            {
                if (MessageBox.Show(LocalizationHelper.GetRes("str_Msg_Exit"), LocalizationHelper.GetRes("str_Msg_Msg"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                    }
                    catch //(Exception ex)
                    { }
                    e.Cancel = false;
                    App.Current.Shutdown();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 加载比赛场次
        //private List<XElement> xeBetContent;
        /// <summary>
        /// 选择投注项
        /// </summary>
        /// <param name="xe"></param>
        public void ItemSelect(List<XElement> xe)
        {
            if (xe == null || xe.Count == 0)
            {
                //BetSP.DataContext = null;
                LbBet.ItemsSource = null;
                BetSP.Visibility = Visibility.Collapsed;
                bdMsg.Visibility = Visibility.Visible;
                btnBetRefresh.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (xe.Count == 1 && BetSP.Visibility == Visibility.Collapsed)
                {
                    //第一次选择赔率时重新计时刷新按钮
                    betsecond = BETSECOND;
                    BetCountDown();
                }
                ClearSelect(tabMatch.SelectedIndex);
                btnBetRefresh.Visibility = Visibility.Visible;
                //BetSP.DataContext = xe;
                LbBet.ItemsSource = null;
                LbBet.ItemsSource = xe;
                if (xe.Count >= xe.Max(c => c.Element("MinLimit").GetValue(0)) && xe.Count <= xe.Min(c => c.Element("MaxLimit").GetValue(10)))
                {
                    spBetValue.Visibility = Visibility.Visible;
                    BtnSubmit.IsEnabled = true;
                }
                else
                {
                    spBetValue.Visibility = Visibility.Collapsed;
                    BtnSubmit.IsEnabled = false;
                }
                BetSP.Visibility = Visibility.Visible;
                bdMsg.Visibility = Visibility.Collapsed;
                tblBetMin.Text = UserLogin.CurrentUser.System.MinBetAmount + "";
                tblBetMax.Text = GetBetMax().ToString("F2");
                txtBetValue_TextChanged(null, null);
                txtBetValue.Focus();
            }
        }
        private decimal GetBetMax()
        {
            int maxBetValue = ((int)(UserLogin.CurrentUser.System.MaxBetAmount / GetSelectedIor() / 10) * 10);
            return maxBetValue < 50 ? 50 : maxBetValue;
        }
        private void ClearSelect(int index = -1)
        {
            if (ucStandardGrid != null && index != 0)
            {
                ucStandardGrid.ClearSelect();
            }
            if (ucScoreGrid != null && index != 1)
            {
                ucScoreGrid.ClearSelect();
            }
            if (ucTotal != null && index != 2)
            {
                ucTotal.ClearSelect();
            }
            if (ucHalfFinal != null && index != 3)
            {
                ucHalfFinal.ClearSelect();
            }
            if (ucMultiList != null && index != 4)
            {
                ucMultiList.ClearSelect();
            }
            ItemSelect(null);
            txtBetValue.Text = "";
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            UserLogin.GetBalance();
            tblBalance.Text = UserLogin.CurrentUser.User.BalanceAmount + "";
            ReloadData();
            totalsecond = 180;
        }
        /// <summary>
        /// 刷新下注区赔率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBetRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadBetData();
            betsecond = BETSECOND;
        }
        private void IniMatchControl()
        {
            ucStandardGrid = new UCStandardGrid(ItemSelect);
            StandardBD.Child = ucStandardGrid;
            ucScoreGrid = new UCScoreList(ItemSelect);
            ScoreBD.Child = ucScoreGrid;
            ucHalfFinal = new UCHalfFinal(ItemSelect);
            HalfFinalBD.Child = ucHalfFinal;
            ucTotal = new UCTotal(ItemSelect);
            TotalScoreBD.Child = ucTotal;
            ucMultiList = new UCMultiList(ItemSelect);
            MultiBD.Child = ucMultiList;
        }
        void ReloadData(bool clearSelect = false)
        {
            if (ucStandardGrid != null)
            {
                ucStandardGrid.LoadData(clearSelect);
            }
            if (ucScoreGrid != null)
            {
                ucScoreGrid.LoadData(clearSelect);
            }
            if (ucHalfFinal != null)
            {
                ucHalfFinal.LoadData(clearSelect);
            }
            if (ucTotal != null)
            {
                ucTotal.LoadData(clearSelect);
            }
            if (ucMultiList != null)
            {
                ucMultiList.LoadData(clearSelect);
            }
        }
        private void ReloadBetData()
        {
            bool isOk = false;
            try
            {
                List<XElement> xeBetList = LbBet.ItemsSource as List<XElement>;
                List<XElement> xeMatchs = new List<XElement>();
                if (xeBetList == null || xeBetList.Count == 0)
                {
                    return;
                }
                string matchids = xeBetList.Select(c => c.Element("MatchID").GetValue()).Aggregate((x, y) => x + "," + y);
                HttpParam param = new HttpParam();
                param.Add("matchids", matchids);
                //TaskAsyncHelper.RunAsync(() =>
                //{
                ProcessRequest.Process(string.Format("Match/Detail").HttpPost(param), (ApiReturn res) =>
                {
                    if (!string.IsNullOrWhiteSpace(res.data + ""))
                    {
                        XmlDocument xmldata = JsonConvert.DeserializeXmlNode(res.data + "", "Root");
                        xeMatchs = XElement.Parse(xmldata.OuterXml.Replace("&lt;", "<").Replace("&gt;", ">")).Elements("data").ToList();
                        isOk = true;
                        if (isOk)
                        {
                            foreach (XElement xeBet in xeBetList)
                            {
                                XElement _xeMatch = xeMatchs.FirstOrDefault(c => c.Element("MatchId").GetValue(0) == xeBet.Element("MatchID").GetValue(0));
                                string betkey = xeBet.Element("BetContent").GetValue().Split('@')[0];
                                string bettext = xeBet.Element("BetText").GetValue().Split('@')[0];
                                string betior = BetInfoHelper.GetIOR_All(_xeMatch.Element("Odds").Element("MatchContent"))[xeBet.Element("BetType").GetValue() + "." + betkey];
                                xeBet.SetElementValue("BetText", bettext + "@" + betior);
                                xeBet.SetElementValue("BetIor", betior);
                                xeBet.SetElementValue("BetContent", betkey + "@" + betior);
                            }
                            LbBet.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                            {
                                LbBet.ItemsSource = null;
                                LbBet.ItemsSource = xeBetList;
                            }));
                        }
                    }
                }, false);
                //}, () =>
                //{
                //    if (isOk)
                //    {
                //        foreach (XElement xeBet in xeBetList)
                //        {
                //            XElement _xeMatch = xeMatchs.FirstOrDefault(c => c.Element("MatchId").GetValue(0) == xeBet.Element("MatchID").GetValue(0));
                //            string betkey = xeBet.Element("BetContent").GetValue().Split('@')[0];
                //            string bettext = xeBet.Element("BetText").GetValue().Split('@')[0];
                //            string betior = BetInfoHelper.GetIOR_All(_xeMatch.Element("Odds").Element("MatchContent"))[xeBet.Element("BetType").GetValue() + "." + betkey];
                //            xeBet.SetElementValue("BetText", bettext + "@" + betior);
                //            xeBet.SetElementValue("BetIor", betior);
                //            xeBet.SetElementValue("BetContent", betkey + "@" + betior);
                //        }
                //        LbBet.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                //        {
                //            LbBet.ItemsSource = null;
                //            LbBet.ItemsSource = xeBetList;
                //        }));
                //    }
                //});

            }
            catch (Exception ex)
            {
                try
                {
                    Tools.Msg.Error(ex.Message);
                }
                catch
                {
                }
            }
        }
        #region 按钮倒计时
        private int totalsecond = 180;
        private int betsecond = BETSECOND;
        private void timer_Tick(object sender, EventArgs e)
        {
            tecNowTime.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                tecNowTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            }));
            if (CountDown() <= 0)
            {
                ReloadData();
                totalsecond = 180;
            }
            if (BetCountDown() <= 0)
            {
                ReloadBetData();
                betsecond = BETSECOND;
            }
        }
        private int CountDown()
        {
            totalsecond -= 1;
            tblSecond.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                tblSecond.Text = totalsecond + "";
            }));
            return totalsecond;
        }
        private int BetCountDown()
        {
            betsecond -= 1;
            tblBetSecond.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                tblBetSecond.Text = betsecond + "";
            }));
            return betsecond;
        }
        #endregion
        #endregion
        /// <summary>
        /// 提交投注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<XElement> xeBetList = LbBet.ItemsSource as List<XElement>;
                if (xeBetList == null || xeBetList.Count == 0)
                {
                    Tools.Msg.Error(LocalizationHelper.GetRes("str_Require_SelectOne"));
                    return;
                }
                decimal betvalue = 0;
                if (!decimal.TryParse(txtBetValue.Text.Replace(",", ""), out betvalue))
                {
                    Tools.Msg.Error(LocalizationHelper.GetRes("str_Require_BetValue"));
                    return;
                }
                if (betvalue > UserLogin.CurrentUser.User.BalanceAmount)
                {
                    Tools.Msg.Error(LocalizationHelper.GetRes("str_Valid_NoMoney"));
                    return;
                }
                if (betvalue < UserLogin.CurrentUser.System.MinBetAmount)
                {
                    Tools.Msg.Error(LocalizationHelper.GetRes("str_Valid_BetMin") + UserLogin.CurrentUser.System.MinBetAmount);
                    return;
                }
                if (betvalue > GetBetMax())
                {
                    Tools.Msg.Error(LocalizationHelper.GetRes("str_Valid_BetMax") + GetBetMax());
                    return;
                }
                HttpParam param = new HttpParam();
                Model.MatchUserBet userBet = new Model.MatchUserBet();
                userBet.UserId = UserLogin.CurrentUser.User.Id;
                userBet.BetValue = betvalue;
                int _betType = xeBetList.First().Element("BetType").GetValue(1);
                userBet.BetType = (_betType + "").Substring(0, 1) == "5" ? 5 : _betType;
                foreach (XElement xeBet in xeBetList)
                {
                    Model.MatchUserBetContent userBetContent = new Model.MatchUserBetContent();
                    userBetContent.MatchDate = xeBet.Element("MatchDate").GetValue(DateTime.Now).ToString("MM-dd HH:mm");
                    userBetContent.BetType = xeBet.Element("BetType").GetValue(0);
                    userBetContent.MatchID = xeBet.Element("MatchID").GetValue(0);
                    userBetContent.BetContent = xeBet.Element("BetContent").GetValue();
                    userBetContent.BetKey = xeBet.Element("BetKey").GetValue();
                    userBetContent.BetText = xeBet.Element("BetText").GetValue();
                    userBetContent.BetTeam = string.Format("{0}{1} VS {2}{3}"
                        , xeBet.Element("HTeam").GetValue()
                        , xeBet.Element("RatioH").GetValue()
                        , xeBet.Element("RatioC").GetValue()
                        , xeBet.Element("CTeam").GetValue());
                    userBetContent.LeagueName = xeBet.Element("LeagueName").GetValue();
                    userBet.MatchUserBetContent.Add(userBetContent);
                }
                string json = JsonConvert.SerializeObject(userBet);
                param.Add("matchuserbet", json);
                ProcessRequest.Process(string.Format("User/Bet").HttpPost(param), (ApiReturn res) =>
                {
                    try
                    {
                        var _userBet = JsonConvert.DeserializeObject<dynamic>(res.data + "");
                        userBet.BetId = _userBet.BetId;
                        userBet.CreateDate = _userBet.CreateDate;
                        UserLogin.GetBalance();
                        tblBalance.Text = UserLogin.CurrentUser.User.BalanceAmount + "";
                        //Tools.Msg.Success(LocalizationHelper.GetRes("str_Success_Bet"));
                        ClearSelect();
                        PrintHelper ph = new PrintHelper(userBet);
                        ph.Print();
                    }
                    catch { }
                    Print p = new Print(userBet);
                    p.Topmost = true;
                    p.ShowDialog();
                });
            }
            catch (Exception ex)
            {
                Tools.Msg.Error(ex.Message);
            }
        }
        /// <summary>
        /// 取消投注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearSelect();
        }

        private void BtnBetValue_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            txtBetValue.Text = btn.Tag + "";
        }
        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCharge_Click(object sender, RoutedEventArgs e)
        {
            Charge frm = new Charge();
            this.Opacity = 0.5;
            frm.Topmost = true;
            frm.ShowDialog();
            tblBalance.Text = UserLogin.CurrentUser.User.BalanceAmount + "";
            this.Opacity = 1;
        }
        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuTrade_Click(object sender, RoutedEventArgs e)
        {
            Trade frm = new Trade();
            this.Opacity = 0.5;
            frm.Topmost = true;
            frm.ShowDialog();
            tblBalance.Text = UserLogin.CurrentUser.User.BalanceAmount + "";
            this.Opacity = 1;
        }
        /// <summary>
        /// 提现
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCash_Click(object sender, RoutedEventArgs e)
        {
            Cash frm = new Cash();
            this.Opacity = 0.5;
            frm.Topmost = true;
            frm.ShowDialog();
            tblBalance.Text = UserLogin.CurrentUser.User.BalanceAmount + "";
            this.Opacity = 1;
        }
        /// <summary>
        /// 我的账户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuAccount_Click(object sender, RoutedEventArgs e)
        {
            MyAccount frm = new MyAccount();
            this.Opacity = 0.5;
            frm.Topmost = true;
            frm.ShowDialog();
            this.Opacity = 1;
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUpdatePasswd_Click(object sender, RoutedEventArgs e)
        {
            UpdatePasswd frm = new UpdatePasswd();
            this.Opacity = 0.5;
            frm.Topmost = true;
            frm.ShowDialog();
            this.Opacity = 1;
        }
        /// <summary>
        /// 安全密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUpdateSecretCode_Click(object sender, RoutedEventArgs e)
        {
            SecretCode frm = new SecretCode();
            this.Opacity = 0.5;
            frm.Topmost = true;
            frm.ShowDialog();
            this.Opacity = 1;
        }
        /// <summary>
        /// 交易状况
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSettleRecord_Click(object sender, RoutedEventArgs e)
        {
            SettleRecord frm = new SettleRecord();
            this.Opacity = 0.5;
            frm.Topmost = true;
            frm.ShowDialog();
            this.Opacity = 1;
        }
        /// <summary>
        /// 账户历史
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHistoryAccount_Click(object sender, RoutedEventArgs e)
        {
            HistoryAccount frm = new HistoryAccount();
            this.Opacity = 0.5;
            frm.Topmost = true;
            frm.ShowDialog();
            this.Opacity = 1;
        }
        /// <summary>
        /// 添加账户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddAccount_Click(object sender, RoutedEventArgs e)
        {

        }
        private decimal GetSelectedIor()
        {
            List<XElement> _xeBetList = LbBet.ItemsSource as List<XElement>;
            if (_xeBetList == null || _xeBetList.Count == 0)
            {
                return 1;
            }
            return _xeBetList.Select(c => c.Element("BetIor").GetValue(0M)).Aggregate((i, j) => i * j);

        }
        private void txtBetValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            string betValue = txtBetValue.Text;
            decimal betBonus = 0;
            if (decimal.TryParse(betValue, out betBonus))
            {
                List<XElement> _xeBetList = LbBet.ItemsSource as List<XElement>;
                if (_xeBetList != null)
                {
                    betBonus = betBonus * (GetSelectedIor() - 1);
                }
            }
            tblBetBonus.Text = betBonus.ToString("F2");
        }
        /// <summary>
        /// 拖动窗体头部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        private void btnMyCenter_Click(object sender, RoutedEventArgs e)
        {
            Pop_bg.IsOpen = true;
        }

        private void btnLang_Click(object sender, RoutedEventArgs e)
        {
            Pop_lang.IsOpen = true;
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
                ItemSelect(null);
                ReloadData(true);
            }
            catch (Exception ex)
            {
                Tools.Msg.Error(LocalizationHelper.GetRes("str_Error_ChangeLang") + ex.Message);
            }
        }
        /// <summary>
        /// 初始化用户登录时选的语言
        /// </summary>
        /// <param name="lang"></param>
        private void InitLanguage()
        {
            Button btnLangSel = UIHelper.FindChild<Button>(spLang, "btnLang_" + UserLogin.LANG);
            btnLangSel.Visibility = Visibility.Collapsed;
            BitmapImage image = new BitmapImage(new Uri(string.Format("/Image/head_{0}BIG.png", UserLogin.LANG), UriKind.RelativeOrAbsolute));
            imgLang.Source = image;
            tblLang.Text = (btnLangSel.Tag + "").Split('|')[1];
        }
        /// <summary>
        /// 扫描条码的回车
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBetId_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                long betid = 0;
                if (long.TryParse(txtBetId.Text.Trim(), out betid))
                {
                    UserBetGet(betid);
                }
                txtBetId.Text = "";
            }
        }

        private void txtBetValue_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSubmit_Click(null, null);
            }
        }
        private void UserBetGet(long betid)
        {
            try
            {
                HttpParam param = new HttpParam();
                param.Add("betid", betid);
                ProcessRequest.Process(string.Format("User/UserBetGet").HttpPost(param), (ApiReturn res) =>
                {
                    try
                    {
                        var _userBet = JsonConvert.DeserializeObject<dynamic>(res.data + "");
                        Prize p = new Prize((decimal)_userBet.BetBonus);
                        p.Topmost = true;
                        p.ShowDialog();
                    }
                    catch { }
                });
            }
            catch (Exception ex)
            {
                Tools.Msg.Error(ex.Message);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long betid = 0;
                if (long.TryParse(txtBetId.Text.Trim(), out betid))
                {
                    UserBetGet(betid);
                }
                txtBetId.Text = "";
            }
            catch { 
            }
        }

    }
}
