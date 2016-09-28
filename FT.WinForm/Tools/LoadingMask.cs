using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.WinForm
{
    public class LoadingMask
    {
        private static LoadingPage loadingPage;

        public static void Show()
        {
            if (loadingPage == null)
            {
                loadingPage = new LoadingPage();
            }
            loadingPage.Show();
        }

        public static void Show(string msg)
        {
            if (loadingPage == null)
            {
                loadingPage = new LoadingPage();
            }
            loadingPage.Msg = msg;
            loadingPage.Show();
        }
        public static void Hide()
        {
            if (loadingPage == null)
            {
                return;
            }
            loadingPage.Close();
        }
    }
}
