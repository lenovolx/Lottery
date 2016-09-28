using com.epson.pos.driver;
using FT.Model;
using FT.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FT.WinForm.Tools
{
    public class PrintHelper
    {
        private const string PRINTER_NAME = "EPSON TM-T82II Receipt";
        private StatusAPI m_objAPI;
        private MatchUserBet userBet;
        public PrintHelper(MatchUserBet _userBet)
        {
            userBet = _userBet;
            m_objAPI = new StatusAPI();
        }
        public void Print()
        {
            Boolean isFinish;
            PrintDocument pdPrint = new PrintDocument();
            pdPrint.PrintPage += new PrintPageEventHandler(pdPrint_PrintPage);
            // Change the printer to the indicated printer.
            pdPrint.PrinterSettings.PrinterName = PRINTER_NAME;

            try
            {
                // Open a printer status monitor for the selected printer.
                if (m_objAPI.OpenMonPrinter(OpenType.TYPE_PRINTER, pdPrint.PrinterSettings.PrinterName) == ErrorCode.SUCCESS || m_objAPI.OpenMonPrinter(OpenType.TYPE_PRINTER, pdPrint.PrinterSettings.PrinterName) == ErrorCode.ERR_OPENED)
                {
                    if (pdPrint.PrinterSettings.IsValid)
                    {
                        pdPrint.DocumentName = "Testing";
                        // Start printing.
                        pdPrint.Print();

                        // Check printing status.
                        isFinish = false;

                        // Perform the status check as long as the status is not ASB_PRINT_SUCCESS.
                        do
                        {
                            if (m_objAPI.Status.ToString().Contains(ASB.ASB_PRINT_SUCCESS.ToString()))
                                isFinish = true;

                        } while (!isFinish);
                        return;
                    }
                }
            }
            catch
            {
                //Tools.Msg.Error(LocalizationHelper.GetRes("str_Error_Print"));
            }
            Tools.Msg.Error(LocalizationHelper.GetRes("str_Error_Print"));
        }
        private void pdPrint_PrintPage(object sender, PrintPageEventArgs e)
        {
            float x, y, lineOffset;
            ////字体颜色
            // Instantiate font objects used in printing.
            //Font printFont = new Font("Microsoft Sans Serif", (float)10, FontStyle, GraphicsUnit.Point); // Substituted to FontA Font
            Font printFont = new Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            //e.Graphics.PageUnit = GraphicsUnit.Point;

            //// Draw the bitmap
            x = 5;
            y = 0;
            lineOffset = printFont.GetHeight(e.Graphics) + 1;
            Graphics g = e.Graphics;
            Brush b = Brushes.Black;
            Font titleFont = new Font("宋体", 16);
            if (userBet != null)
            {
                string title = LocalizationHelper.GetRes("wintitle") + "<" + LocalizationHelper.GetRes("Enum.BetType." + (userBet.BetType.ToString().StartsWith("5") ? 5 : userBet.BetType)) + ">";
                g.DrawString(title, titleFont, b, new PointF((e.PageBounds.Width - g.MeasureString(title, titleFont).Width) / 2, 0));
                //g.DrawString(title, printFont, b, x, y);
                //e.Graphics.DrawString("123xxstreet,xxxcity,xxxxstate", printFont, Brushes.Black, x, y);
                y = y + lineOffset + g.MeasureString(title, titleFont).Height;
                g.DrawString((userBet.BetId + "").PadRight(20, ' '), printFont, Brushes.Black, x, y);
                g.DrawImage(QRCodeHelper.Create(userBet.BetId + "", 80, 80), e.PageBounds.Width - 120, y - 10, 80, 80);
                y += lineOffset + 10;
                g.DrawString(userBet.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"), printFont, Brushes.Black, x, y);
                y += lineOffset + 10;
                g.DrawString("总计" + userBet.BetValue, printFont, Brushes.Black, x, y);
                y += 4;
                g.DrawLine(new Pen(Brushes.Black, 1), x, y + lineOffset - 2, e.PageBounds.Width - x, y + lineOffset - 2);
                foreach (MatchUserBetContent betContent in userBet.MatchUserBetContent)
                {
                    y += lineOffset;
                    g.DrawString(LocalizationHelper.GetRes("Enum.BetType." + betContent.BetType), printFont, Brushes.Black, x, y);
                    y += lineOffset;
                    g.DrawString(string.Format("[{1}]{0}", betContent.LeagueName, betContent.MatchDate), printFont, Brushes.Black, x, y);
                    y += lineOffset;
                    g.DrawString(betContent.BetTeam, printFont, Brushes.Black, x, y); //g.DrawString(dr["名称"].ToString(), myFont2, black, new RectangleF(180, 205, 570, 50));
                    y += g.MeasureString(betContent.BetTeam, printFont).Height + 2;
                    g.DrawString(betContent.BetText, printFont, Brushes.Black, x, y);
                    y += 4;
                    g.DrawLine(new Pen(Brushes.Black, 1), x, y + lineOffset - 2, e.PageBounds.Width - x, y + lineOffset - 2);
                }
                //y += 15;
                //g.DrawImage(QRCodeHelper.Create(userBet.BetId + "", 150, 150), x, y, 150, 150);
            }
            //else
            //{
            //    g.DrawImage(QRCodeHelper.Create("201412423423432423423423", 150, 150), x, y, 150, 150);
            //}
            e.HasMorePages = false;
        }
    }
}
