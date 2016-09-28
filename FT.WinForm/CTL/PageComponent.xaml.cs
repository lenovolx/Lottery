using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace FT.WinForm
{
    /// <summary>
    /// ucPage.xaml 的交互逻辑
    /// </summary>
    public partial class PageComponent : UserControl
    {
        //定义一个委托
        public delegate void PageChangedHandle(object sender, EventArgs e);
        //定义一个事件
        public event PageChangedHandle PageChanged;

        public PageComponent()
        {
            InitializeComponent();
        }
        //总页数
        private int totalPage = 1;
        /// <summary>
        /// 当前页
        /// </summary>
        private int currentPage = 1;

        #region 当前页
        /// <summary>
        /// 注册当前页
        /// </summary>
        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(String),
        typeof(PageComponent), new FrameworkPropertyMetadata("1", FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnCurrentPageChanged)), new ValidateValueCallback(CurrentPageValidation));

        /// <summary>
        /// 验证当前页
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool CurrentPageValidation(object value)
        {
            return true;
        }
        /// <summary>
        /// 当前页
        /// </summary>
        public string CurrentPage
        {
            get { return GetValue(PageComponent.CurrentPageProperty).ToString(); }
            set
            {
                SetValue(PageComponent.CurrentPageProperty, value);

                lblCurrentPage.Content = value;
            }
        }


        #endregion

        #region 总页数
        /// <summary>
        /// 总页数
        /// </summary>
        public static readonly DependencyProperty TotalPageProperty = DependencyProperty.Register("TotalPage", typeof(String), typeof(PageComponent), new FrameworkPropertyMetadata("1", FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnTotalPageChanged)), new ValidateValueCallback(TotalPageValidation));

        /// <summary>
        /// 总页数进行验证
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TotalPageValidation(object value)
        {
            return true;
        }
        /// <summary>
        /// 总页数
        /// </summary>
        public string TotalPage
        {
            get { return GetValue(PageComponent.TotalPageProperty).ToString(); }
            set
            {
                SetValue(PageComponent.TotalPageProperty, value);
                lblTotalPage.Content = value;
            }
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 值改变方法将由此方法来引发事件
        /// </summary>
        private void PageChangedFunc()
        {
            if (PageChanged != null)
            {
                ///引发事件
                PageChanged(this, new EventArgs());
            }
        }


        #endregion

        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFrist_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = "1";

            PageChangedFunc();

        }
        /// <summary>
        /// 前一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRew_Click(object sender, RoutedEventArgs e)
        {
            totalPage = GetIntVal(TotalPage);
            currentPage = GetIntVal(CurrentPage);
            if (currentPage > 1)
            {
                currentPage = currentPage - 1;
                CurrentPage = currentPage.ToString();
            }
            PageChangedFunc();
        }
        /// <summary>
        /// 后一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFF_Click(object sender, RoutedEventArgs e)
        {
            currentPage = GetIntVal(CurrentPage);
            totalPage = GetIntVal(TotalPage);
            if (currentPage < totalPage)
            {
                currentPage = currentPage + 1;
                CurrentPage = currentPage.ToString();
            }
            PageChangedFunc();
        }
        //尾页
        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            currentPage = GetIntVal(TotalPage);
            CurrentPage = currentPage.ToString();
            PageChangedFunc();
        }

        private int GetIntVal(string val)
        {
            int temp = 0;
            if (!int
                .TryParse(val, out temp))
            {
                temp = 1;
            }
            return temp;
        }
        /// <summary>
        /// 当当前页值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTotalPageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            
        }
        /// <summary>
        /// 当当前页值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnCurrentPageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

            //MyButton hsb = (MyButton)sender;
            //  SetValue(NextPageControl.CurrentPageProperty, "1");
            // ShowMsg("event");
            //Image image = hsb.tehImage;
            //CurrentPage = "1";
            //image.Source = new BitmapImage((Uri)e.NewValue);
        }

    }
}
