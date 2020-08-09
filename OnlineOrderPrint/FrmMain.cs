using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GetServerEmail;
using HtmlAgilityPack;
using jmail;
using LumiSoft.Net.Mail;
using LumiSoft.Net.POP3.Client;
using Microsoft.Win32;

namespace OnlineOrderPrint
{
    public partial class FrmMain : Form
    {
        //private static string MAIL_USER_NAME = "chen@milpo.co.uk";
        //private static string MAIL_USER_PWD = "28p9&*Facai";
        //private static string MAIL_POP = "pop.1and1.co.uk";

        private string MAIL_USER_NAME = "";
        private string MAIL_USER_PWD = "";
        private string MAIL_POP = "pop.1and1.co.uk";        

        //private static string MAIL_SENDER = @"noreply@milpoweb.co.uk";
        //private string MAIL_SENDER = @"noreply@internetakeaway.co.uk";
        //private string MAIL_SENDER = @"noreply@dolbynonline.co.uk";
        private string MAIL_SENDER = @"noreply@dolbynonline.co.uk";
        private static string MAIL_RECIVER = @"";

        private string MAIL_TEMPLATE = @"";

        //打印次数
        private string PRT_COUNT = @"";
        //版本
        private string VERSION = @"2";

        //公司名称
        private string COMPANY_NAME = @"";

        private static List<string> textList;       //打印内容行

        private int timer_Int = 60000; // 60 * 1000 = 1 Minute

        private WebBrowser webBrowser = null;

        private string prtFilePath = "";

        private List<MenuItem> lstMi = new List<MenuItem>();

        private string orderId = "";

        private string orderDate = "";

        private string orderType = "";

        //打印内容存储，主要用于二次打印
        private string orderHtmlBody = "";

        public static string wavNewMail = @"\NewMail.wav";

        public static string wavError = @"\ERROR.wav";

        //记录上一次打印内容，用作对比，避免WebBrowser多次打印
        public string strLastHtmlBody = "";
        public bool isPrint = false;

        private System.Threading.AutoResetEvent obj = new System.Threading.AutoResetEvent(false);

        public FrmMain()
        {
            InitializeComponent();
        }

        private void ReciveMail()
        {
            //JMailRecive();
            LumiSoftMail();
        }

        private string GetPrtStr(string htmlText)
        {
            #region 字符串方式
            //Hashtable hashtable = new Hashtable();

            StringBuilder sb = new StringBuilder();

            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlText);

                HtmlNode node;
                node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.HEAD_ORDER_ID);
                //Console.WriteLine(node.InnerText);
                sb.Append(GetSpace((39 - node.InnerText.Length) / 2) + node.InnerText.Replace("&nbsp;", "").Trim());
                orderId = node.InnerText.Replace("&nbsp;", "").Trim().Substring(node.InnerText.Replace("&nbsp;", "").Trim().IndexOf("#"));
                sb.Append(Environment.NewLine);

                node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.HEAD_ORDER_TYPE);
                //Console.WriteLine(node.InnerText);
                sb.Append(GetSpace((39 - node.InnerText.Length) / 2) + node.InnerText.Replace("&nbsp;", "").Trim().ToUpper());
                orderType = node.InnerText.Replace("&nbsp;", "").Trim().Substring(0, node.InnerText.Replace("&nbsp;", "").Trim().IndexOf("ORDER")).ToUpper();
                sb.Append(Environment.NewLine);

                node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.BODY_NAME);
                //Console.WriteLine(node.InnerText);
                sb.Append(node.InnerText.Replace("&nbsp;", "").Trim());
                sb.Append(Environment.NewLine);

                node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.BODY_PHONE);
                //Console.WriteLine(node.InnerText);
                sb.Append(node.InnerText.Replace("&nbsp;", ""));
                sb.Append(Environment.NewLine);

                node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.BODY_COLLECTION_ORDER_TIME);
                //Order Time:&nbsp;05/04/2018 - 22:01
                //Console.WriteLine(node.InnerText);
                sb.Append(node.InnerText.Replace("&nbsp;", "").Trim());
                orderDate = node.InnerText.Replace("&nbsp;", "").Trim().Substring(node.InnerText.Replace("&nbsp;", "").Trim().IndexOf(":") + 1);
                sb.Append(Environment.NewLine);

                //for (int i = 1; i < 10; i++)
                //{
                //    //MenuItem mi = new MenuItem();
                //    try
                //    {
                //        node = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/center[1]/table[1]/tbody[1]/tr[" + i + "]");
                //        //Console.WriteLine(node.InnerText);
                //        sb.Append(GetTab(
                //            doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/center[1]/table[1]/tbody[1]/tr[" + i + "]/td[1]/h1[1]").InnerText,
                //            doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/center[1]/table[1]/tbody[1]/tr[" + i + "]/td[2]/h1[1]").InnerText,
                //            doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/center[1]/table[1]/tbody[1]/tr[" + i + "]/td[3]/h1[1]").InnerText,
                //            doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/center[1]/table[1]/tbody[1]/tr[" + i + "]/td[4]/h1[1]").InnerText
                //            ));
                //        sb.Append(Environment.NewLine);
                //    }
                //    catch (Exception)
                //    {
                //        break;
                //        //throw;
                //    }

                //}

                sb.Append("Code" + GetSpace(2) + "Qty" + GetSpace(2) + "Name" + GetSpace(19) + "Price");
                sb.Append(Environment.NewLine);

                HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/center[1]/table[1]/tbody[1]").ChildNodes;

                foreach (var n in nodeCollection)
                {
                    string[] sTmp = n.InnerText.Trim().Replace("\r\n", "*").Split('*');

                    sTmp = sTmp.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                    if (sTmp.Length >= 4) //菜品类
                    {
                        sb.Append(GetTab(sTmp[0].Trim(), sTmp[1].Trim().Replace("&nbsp;", ""), sTmp[2].Trim(), sTmp[3].Trim()));
                        sb.Append(Environment.NewLine);
                    }
                    else
                    {
                        if (n.InnerText.Replace("\r\n", "").Trim().IndexOf("-") >= 0)
                        {
                            sb.Append(GetSpace(37 - n.InnerText.Replace("\r\n", "").Trim().Length) + n.InnerText.Replace("\r\n", "").Trim().Replace("-", GetSpace(2) + "-"));
                        }
                        else
                        {
                            sb.Append(GetSpace(39 - n.InnerText.Replace("\r\n", "").Trim().Length) + n.InnerText.Replace("\r\n", "").Trim());
                        }
                        sb.Append(Environment.NewLine);
                    }
                }

                try
                {
                    HtmlNodeCollection hnc = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/center[1]/table[1]/tfoot[1]").ChildNodes;

                    foreach (var n in hnc.Where(s => s.InnerText.Replace("\r\n", "").Trim().Replace("&nbsp;", "").Length > 0))
                    {
                        if (n.InnerText.Replace("\r\n", "").Trim().Replace("&nbsp;", "").Substring(0, 4).ToUpper().Equals("FREE"))
                        {
                            sb.Append(GetSpace((39 - n.InnerText.Replace("\r\n", "").Trim().Replace("&nbsp;", "").Length) / 2) + n.InnerText.Replace("\r\n", "").Trim().Replace("&nbsp;", ""));
                            sb.Append(Environment.NewLine);
                        }
                        else
                        {
                            sb.Append(GetSpace(39 - n.InnerText.Replace("\r\n", "").Trim().Replace("&nbsp;", "").Length) + n.InnerText.Replace("\r\n", "").Trim().Replace("&nbsp;", ""));
                            sb.Append(Environment.NewLine);
                        }
                    }
                }
                catch (Exception)
                {
                    //throw;
                }

                //HtmlNode node1 = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/tbody[1]/tr[2]/td[1]");
                ////Items(1)
                ////Console.WriteLine(node.InnerText);
                //sb.Append(GetSpace(39 - node.InnerText.Length / 2) + node.InnerText);
                //sb.Append(Environment.NewLine);

                //node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/tbody[1]/tr[2]/td[3]");
                ////0.10
                //Console.WriteLine(node.InnerText);

                //node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/tbody[1]/tr[3]/td[1]/b[1]");
                ////Card Fee:
                //Console.WriteLine(node.InnerText);

                //node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/tbody[1]/tr[3]/td[2]");
                ////£0.50
                //Console.WriteLine(node.InnerText);

                //node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/tfoot[1]/tr[1]/td[1]/b[1]");
                ////Total:
                //Console.WriteLine(node.InnerText);

                //node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/tfoot[1]/tr[1]/td[2]");
                ////£0.60
                //Console.WriteLine(node.InnerText);

                //node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/strong[1]/strong[1]/p[3]");
                ////Remarks:&nbsp;test
                //Console.WriteLine(node.InnerText);

                node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/p[4]");
                //Payment Status:&nbsp;PAID by Paypal 
                sb.Append(node.InnerText.Replace("\r\n", "").Replace(@"&nbsp;", "").Trim());
                sb.Append(Environment.NewLine);

                //node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/tbody[1]/tr[3]/td[2]");
                ////£0.50
                //Console.WriteLine(node.InnerText);

                node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/p[5]");
                //Payment Status:&nbsp;PAID by Paypal 
                sb.Append(node.InnerText.Replace(@"&nbsp;", ""));
                sb.Append(Environment.NewLine);

                node = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/p[6]");
                //Payment Status:&nbsp;PAID by Paypal 
                sb.Append(node.InnerText.Replace(@"&nbsp;", ""));
                sb.Append(Environment.NewLine);

                string str = @"Powered by Dolbyn Technologies";
                sb.Append(GetSpace((39 - str.Length) / 2) + str);
                sb.Append(Environment.NewLine);
                str = @"http://www.dolbyncomputers.com";
                sb.Append(GetSpace((39 - str.Length) / 2) + str);
                sb.Append(Environment.NewLine);

                //node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/strong[1]/strong[1]/p[5]");
                //// Collection Time: 05/04/2018 - 22:30
                //Console.WriteLine(node.InnerText);

                //node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[2]/p[1]");
                ////Powered by Milpo Technologieshttp://www.milpo.co.uk
                //Console.WriteLine(node.InnerText);

                //node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[2]/p[1]/a[1]");
                ////http://www.milpo.co.uk
                //Console.WriteLine(node.InnerText);
            }
            catch (Exception ex)
            {

                throw;
            }

            //return hashtable;

            return sb.ToString();

            #endregion

            //webBrowser = new WebBrowser();

            ////是否显式滚动条
            //webBrowser.ScrollBarsEnabled = false;

            ////加载 html
            //webBrowser.DocumentText = htmlText;

            ////页面加载完成执行事件
            //webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);

            //if (!string.IsNullOrEmpty(prtFilePath))
            //{
            //    PrintDocument pd = new PrintDocument();
            //    pd.PrintPage += (sender, args) =>
            //    {
            //        Image i = Image.FromFile(prtFilePath);
            //        args.Graphics.DrawImage(i, args.MarginBounds);
            //    };
            //    pd.Print();
            //}

        }

        private void FrmMain_SizeChanged(object sender, EventArgs e)
        {

        }

        private void btnMini_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //this.Close();
            timerOrder.Enabled = false;

            FrmExit frmExit = new FrmExit();
            frmExit.ShowDialog();
        }

        private void btnSysConf_Click(object sender, EventArgs e)
        {
            FrmSysConf frmSysConf = new FrmSysConf();
            frmSysConf.ShowDialog();
        }

        private void timerOrder_Tick(object sender, EventArgs e)
        {
            //Recive mail
            //string strHtml = ReciveMail();
            ReciveMail();

            //重组打印
            //Hashtable hs = GetPrtStr(strHtml);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            #region Timer
            try
            {
                //网络连接判断
                if (!IsNetConnect()) return;

                User user = SqlHelper.Query("SELECT * FROM User");
                if (user != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(user.UsrName) || string.IsNullOrEmpty(user.UsrPwd) || string.IsNullOrEmpty(user.MailServer) || string.IsNullOrEmpty(user.MinsInt) ||
                            string.IsNullOrEmpty(user.MailSender) || string.IsNullOrEmpty(user.PrtCount))
                        {
                            //默认版本为2
                            if (string.IsNullOrEmpty(user.Version)) VERSION = "2";

                            timerOrder.Enabled = false;
                            //MessageBox.Show(@"Mail Server ERROR!", @"ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            if (File.Exists(Environment.CurrentDirectory + wavError))
                            {
                                SoundPlayer player = new SoundPlayer(Environment.CurrentDirectory + wavError);
                                player.Play();
                            }

                            panelErrorMsg.Show();

                            //FrmSysConf frmSysConf = new FrmSysConf();
                            //frmSysConf.ShowDialog();

                            return;
                        }

                        MAIL_USER_NAME = user.UsrName;
                        MAIL_USER_PWD = user.UsrPwd;
                        MAIL_POP = user.MailServer;
                        MAIL_SENDER = user.MailSender;
                        PRT_COUNT = user.PrtCount;
                        VERSION = user.Version;
                        COMPANY_NAME = user.CompanyName;

                        lblCompanyName.Text = COMPANY_NAME;

                        if (string.IsNullOrEmpty(PRT_COUNT)) PRT_COUNT = @"ONE";

                        Console.WriteLine(PubCommon.GetRadioBtnValue(PRT_COUNT));

                        timer_Int = Convert.ToInt32(user.MinsInt) * 60 * 1000;

                        timerOrder.Interval = timer_Int;
                    }
                    catch (Exception)
                    {
                        timerOrder.Enabled = false;
                        MessageBox.Show("Mail Server ERROR!");

                        if (File.Exists(Environment.CurrentDirectory + wavError))
                        {
                            SoundPlayer player = new SoundPlayer(Environment.CurrentDirectory + wavError);
                            player.Play();
                        }

                        //FrmSysConf frmSysConf = new FrmSysConf();
                        //frmSysConf.ShowDialog();
                        return;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            #endregion
        }

        private void webBrowser_DocumentCompleted(object sender, EventArgs e)
        {
            System.Drawing.Rectangle rectangle = webBrowser.Document.Body.ScrollRectangle;
            int width = rectangle.Width;
            int height = rectangle.Height;

            //设置解析后HTML的可视区域
            webBrowser.Width = width;
            webBrowser.Height = height;

            Bitmap bitmap = new System.Drawing.Bitmap(width, height);
            webBrowser.DrawToBitmap(bitmap, new System.Drawing.Rectangle(0, 0, width, height));

            //设置图片文件保存路径和图片格式，格式可以自定义
            string filePath = prtFilePath = AppDomain.CurrentDomain.BaseDirectory + "/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
        }

        //打印事件处理  
        private void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            //int x = e.MarginBounds.X;
            //int y = e.MarginBounds.Y;
            //int width = bmp.Width;
            //int height = bmp.Height;
            //Rectangle destRect = new Rectangle(x, y, width, height);
            //e.Graphics.DrawImage(bmp, destRect, 0, 0, bmp.Width, bmp.Height, System.Drawing.GraphicsUnit.Pixel);
        }

        private StringBuilder GetSpace(int i)
        {
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < i; j++)
            {
                sb.Append(' ');
            }
            return sb;
        }

        private StringBuilder GetTab(string sCode, string sQty, string sName, string sPrice)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Environment.NewLine);
            sb.Append(sCode + GetSpace(HtmlTextPath.PRT_CODE - sCode.Length) + sQty + GetSpace(HtmlTextPath.PRT_QTY - sQty.Length));

            //sb.Append(GetSpace(1) + sPrice);

            if (sName.Length > HtmlTextPath.PRT_MENUITEM_NUM)
            {
                //sb.Append(sName.Substring(0, HtmlTextPath.PRT_MENUITEM_NUM - 1));
                //sb.Append(GetSpace(HtmlTextPath.PRT_WORD_LOWER_NUM - sb.Length - sPrice.Length + 1 + HtmlTextPath.PRT_OFFSET) + sPrice);
                //sb.Append(Environment.NewLine);
                //sb.Append(GetSpace(HtmlTextPath.PRT_CODE + HtmlTextPath.PRT_QTY + 3) + sName.Substring(HtmlTextPath.PRT_MENUITEM_NUM - 1));

                string[] s = sName.Split(' ');
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                bool isAdd = false;

                foreach (string s1 in s.Where(s1 => !string.IsNullOrEmpty(s1)))
                {
                    if (isAdd)
                    {
                        sb2.Append(s1);
                    }
                    else
                    {
                        if ((sb1.Length + s1.Length) < HtmlTextPath.PRT_MENUITEM_NUM)
                        {
                            sb1.Append(s1 + " ");
                        }
                        else
                        {
                            isAdd = true;
                            sb2.Append(s1);
                        }
                    }
                }

                //sb.Replace(" ", sb1.ToString(), (HtmlTextPath.PRT_CODE + HtmlTextPath.PRT_QTY), sb1.Length);
                //sb.Append(Environment.NewLine);
                //sb.Append(sb2);

                sb.Append(sb1);
                sb.Append(GetSpace(HtmlTextPath.PRT_WORD_LOWER_NUM - sb.Length + 5 - sPrice.Length) + sPrice);
                sb.Append(Environment.NewLine);
                sb.Append(GetSpace(HtmlTextPath.PRT_CODE + HtmlTextPath.PRT_QTY + 4) + sb2.ToString());

                //string[] s = sName.Split(' ');
                //StringBuilder sbTmp = new StringBuilder();
                //int i = 0;
                //bool isAddPrice = false;

                //foreach (string s1 in s)
                //{
                //    if (string.IsNullOrEmpty(s1)) continue;

                //    if (i == 0)
                //    {
                //        sb.Append(s1);
                //        sbTmp.Append(s1);
                //    }
                //    else
                //    {
                //        if ((sbTmp.Length + s1.Length + 1) < HtmlTextPath.PRT_MENUITEM_NUM)
                //        {
                //            sb.Append(" " + s1);
                //            sbTmp.Append(s1);
                //        }
                //        else
                //        {
                //            //sb.Append(sbTmp);
                //            if (!isAddPrice)
                //            {
                //                sb.Append(GetSpace(HtmlTextPath.PRT_WORD_LOWER_NUM - sb.Length + 2) + sPrice);
                //                isAddPrice = true;
                //                sb.Append(Environment.NewLine);
                //                sb.Append(GetSpace(HtmlTextPath.PRT_CODE + HtmlTextPath.PRT_QTY + 4));
                //            }
                //            else
                //            {
                //                sb.Append(s1 + " ");
                //                sbTmp.Append(s1 + " ");
                //            }
                //        }
                //    }
                //    i++;

                //    //sb.Append(sbTmp.ToString());
                //}
            }
            else
            {
                sb.Append(sName + GetSpace(HtmlTextPath.PRT_MENUITEM_NUM - sName.Length));
                sb.Append(sPrice);
            }

            return sb;
        }

        public StringBuilder GetItem(string sItem, string sSubTotal)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Items:(" + sItem + ")" + GetSpace(39 - sItem.Length - sSubTotal.Length - 1) + sSubTotal);

            return sb;
        }

        public StringBuilder GetTotal(string sTotal)
        {
            StringBuilder sb = new StringBuilder();
            //string s1 = "Total:" + sTotal;
            sb.Append(GetSpace((25 - sTotal.Length)) + sTotal);
            return sb;
        }

        private static void Prt_Order(object sender, PrintPageEventArgs e)
        {
            var mark = 0;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            foreach (var item in textList)
            {
                e.Graphics.DrawString(item, new Font(new FontFamily("宋体"), 10), Brushes.Black, 0, (mark + 1) * 18);
                mark++;
            }
        }

        private void PrtOrder(string htmlText)
        {
            if (string.IsNullOrWhiteSpace(GetPrtStr(htmlText)))
            {
                return;
            }

            //原文字行或者段落内容
            var sourceTexts = GetPrtStr(htmlText).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            //重新把文字进行分行树立
            textList = new List<string>();
            foreach (var item in sourceTexts)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    if (item.Length > 39)
                    {
                        textList.AddRange(GetArr(39, item));
                    }
                    else
                    {
                        textList.Add(item);
                    }
                }
            }


            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(Prt_Order);
            //纸张设置默认
            PaperSize pageSize = new PaperSize("自定义纸张", (textList.Count * (int)(58 / 25.4 * 100)), 700);
            pd.DefaultPageSettings.PaperSize = pageSize;
            //pd.PrinterSettings.PrinterName
            try
            {
                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("打印失败." + ex.Message, @"ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static List<string> GetArr(int linelen, string text)
        {
            var list = new List<string>();
            int listcount = text.Length % linelen == 0 ? text.Length / linelen : (text.Length / linelen) + 1;
            for (int j = 0; j < listcount; j++)
            {
                try
                {
                    list.Add(text.Substring(j * linelen, linelen));
                }
                catch (Exception)
                {
                    list.Add(text.Substring(j * linelen));
                }
            }
            return list;
        }

        private void btnRetrieveOrder_Click(object sender, EventArgs e)
        {
            timerOrder.Enabled = false;
            btnRetrieveOrder.Enabled = false;
            btnRetrieveOrder.BackColor = Color.DimGray;

            ReciveMail();

            btnRetrieveOrder.BackColor = Color.Gold;
            timerOrder.Enabled = true;
            btnRetrieveOrder.Enabled = true;
        }

        private void JMailRecive()
        {
            //string poptity, senders, sendmail, subject, HtmlBody, TextBody, date, size;
            string sendmail, HtmlBody = "", date, reciver;

            POP3Class popMail = new POP3Class();
            jmail.Message mailMessage;

            try
            {
                popMail.Connect(MAIL_USER_NAME, MAIL_USER_PWD, MAIL_POP);
                popMail.Timeout = 45000;
            }
            catch (Exception ex)
            {
                //popMail = null;
                //popMail.Disconnect();
                Console.Out.WriteLine("ex:" + ex);
                SetRichTextValue("ex:" + ex);
                //MessageBox.Show("Can not connect email server");
                Console.Out.WriteLine("Can not connect email server" + DateTime.Now.ToString("o"));
                SetRichTextValue("Can not connect email server:" + DateTime.Now.ToString("o"));
                
                return;
            }

            Console.Out.WriteLine(DateTime.Now.ToString("o"));
            SetRichTextValue(DateTime.Now.ToString("o"));
            
            if (0 < popMail.Count)
            {
                for (int i = popMail.Count; i <= popMail.Count; i--)
                {

                    if (!SqlHelper.QueryId(@"SELECT mailID FROM Mail_ID WHERE mailID='" + popMail.GetMessageUID(i) + "'"))
                    {
                        try
                        {
                            mailMessage = popMail.Messages[i];
                        }
                        catch (Exception)
                        {
                            popMail.Connect(MAIL_USER_NAME, MAIL_USER_PWD, MAIL_POP);
                            mailMessage = popMail.Messages[i];
                        }
                    }
                    else
                    {
                        return;
                    }


                    mailMessage.Charset = "utf-8";
                    mailMessage.Silent = true;
                    mailMessage.EnableCharsetTranslation = true;
                    mailMessage.ContentTransferEncoding = "Base64";
                    //mailMessage.Encoding = "Base64";
                    mailMessage.Encoding = @"Base64";
                    mailMessage.ISOEncodeHeaders = false;

                    //邮件优先级：
                    //poptity = mailMessage.Priority.ToString();
                    //发件人
                    //senders = mailMessage.FromName;
                    //发件人地址
                    sendmail = mailMessage.From.ToString();
                    //主题
                    //subject = mailMessage.Subject;
                    //内容
                    HtmlBody = mailMessage.HTMLBody;
                    //TextBody = mailMessage.Body;
                    //发送日期时间
                    date = mailMessage.Date.ToString("d");
                    if (Convert.ToDateTime(date) < Convert.ToDateTime(DateTime.Now.ToShortDateString()))
                    {
                        break;
                    }
                    //size = mailMessage.Size.ToString();
                    ////收件人地址
                    //reciver = mailMessage.RecipientsString.ToString();

                    //删除邮件
                    //popMail.DeleteSingleMessage(i);

                    if (!sendmail.Equals(MAIL_SENDER))
                    {
                        continue;
                    }

                    //if (!reciver.Replace("\r\n", "").Equals(MAIL_RECIVER))
                    //{
                    //    continue;
                    //}

                    //Console.WriteLine("主题：" + subject + "<br>");
                    //Console.WriteLine("发件人：" + senders + "<" + sendmail + "><br>");
                    //Console.WriteLine("发送时间：" + date + "<br>");
                    //Console.WriteLine("邮件大小：" + size + "<br>");
                    //Console.WriteLine("邮件优先级：" + poptity + "<br>");
                    //Console.WriteLine("TextBody：" + TextBody + "<br>");
                    //Console.WriteLine("内容：<br>" + HtmlBody + "<hr>");

                    //GetPrtStr(HtmlBody);

                    //播放语音提示

                    if (File.Exists(Environment.CurrentDirectory + wavNewMail))
                    {
                        SoundPlayer player = new SoundPlayer(Environment.CurrentDirectory + wavNewMail);
                        player.Play();
                    }

                    PrtOrder(HtmlBody.Replace("脳", "×").Replace("拢", "£"));

                    //打印完成后插入数据
                    if (!SqlHelper.InsertId(@"INSERT INTO Mail_ID(mailID, orderID, orderType, orderTime) VALUES('"
                                           + popMail.GetMessageUID(i) + "', '"
                                           + orderId + "', '"
                                           + orderType + "', '"
                                           + orderDate + "', '"
                                           + HtmlBody + "')"))
                    {
                        MessageBox.Show(@"WRITE Data Error!", @"ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //int index = this.dgvOrder.Rows.Add();
                        this.dgvOrder.Rows.Insert(0, dgvOrder.Rows);
                        dgvOrder.Rows[0].Cells[0].Value = orderId;
                        dgvOrder.Rows[0].Cells[1].Value = orderDate;
                        dgvOrder.Rows[0].Cells[2].Value = orderType;

                        dgvOrder.Refresh();
                    }
                }

                if (popMail != null)
                {
                    //popMail = null;
                    try
                    {
                        popMail.Disconnect();
                    }
                    catch (Exception)
                    {
                        Console.Out.WriteLine("Error if (popMail != null)");
                        SetRichTextValue(@"Error popMail is not null)");
                    }
                }
                //return HtmlBody.Replace("脳", "×").Replace("拢", "£");
            }
            else
            {
                //popMail = null;
                try
                {
                    popMail.Disconnect();
                }
                catch (Exception)
                {
                    Console.Out.WriteLine("Error else");

                    SetRichTextValue(@"Error else");    
                }

                //return "";
            }
        }

        private void LumiSoftMail()
        {
            string sendmail, HtmlBody = "", date;

            //网络连接判断
            if (!IsNetConnect()) return; 

            POP3_Client popMail = new POP3_Client();
            
            try
            {
                popMail.Timeout = HtmlTextPath.EMAIL_TIME_OUT;

                try
                {
                    popMail.Connect(MAIL_POP, HtmlTextPath.EMAIL_PORT, false);
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("ex:" + ex);
                    SetRichTextValue(DateTime.Now.ToString("o") + @"######Internet connection failed######");
                    return;
                    //throw;
                }

                //登录邮箱地址
                popMail.Login(MAIL_USER_NAME, MAIL_USER_PWD);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("ex:" + ex);
                //richTextBox1.Text += System.Environment.NewLine + "ex:" + ex;
                //Console.Out.WriteLine("Can not connect email server" + DateTime.Now.ToString("o"));
                //richTextBox1.Text += System.Environment.NewLine + "Can not connect email server:" + DateTime.Now.ToString("o");
                SetRichTextValue(DateTime.Now.ToString("o") + @"######Can not connect email server######");
                return;
            }

            Console.Out.WriteLine(DateTime.Now.ToString("o"));
            SetRichTextValue(DateTime.Now.ToString("o"));
            POP3_ClientMessageCollection messagesCollection = popMail.Messages;

            POP3_ClientMessage message = null;

            //存放需删除的邮件
            List<POP3_ClientMessage> lstMessage = new List<POP3_ClientMessage>();

            if (0 < messagesCollection.Count)
            {
                for (int i = messagesCollection.Count - 1; i >= 0; i--)
                {

                    if (!SqlHelper.QueryId(@"SELECT mailID FROM Mail_ID WHERE mailID='" + messagesCollection[i].UID + "'"))
                    {
                        try
                        {
                            message = popMail.Messages[i];
                        }
                        catch (Exception)
                        {
                            popMail.Timeout = HtmlTextPath.EMAIL_TIME_OUT;
                            popMail.Connect(MAIL_POP, HtmlTextPath.EMAIL_PORT, true);
                            popMail.Login(MAIL_USER_NAME, MAIL_USER_PWD);
                        }
                    }
                    else
                    {
                        //return;
                        break;
                    }

                    Mail_Message mailMessage = null;

                    try
                    {
                        if (message != null)
                        {
                            byte[] messBytes = message.MessageToByte();
                            mailMessage = Mail_Message.ParseFromByte(messBytes);
                        }
                    }
                    catch (Exception)
                    {
                        SetRichTextValue(DateTime.Now.ToString("o") + @"ERR GET MSG TO BYTE");
                        continue;
                        //Console.WriteLine(@"ERR GET MSG TO BYTE:" + DateTime.Now.ToString("o"));
                        //throw;
                    }
                    
                    
                    sendmail = mailMessage.From[0].Address;
                    HtmlBody = mailMessage.BodyHtmlText;
                    //date = mailMessage.Date.ToString("d");
                    //if (Convert.ToDateTime(date) < Convert.ToDateTime(DateTime.Now.ToShortDateString()))
                    //{
                    //    break;
                    //}
                    //发送日期时间
                    date = mailMessage.Date.ToString("d");
                    if (Convert.ToDateTime(date) < Convert.ToDateTime(DateTime.Now.AddDays(-1).ToShortDateString()))
                    {
                        break;
                    }
                    
                    if (!sendmail.Equals(MAIL_SENDER))
                    {
                        message.MarkForDeletion();
                        SetRichTextValue(DateTime.Now.ToString("o") + @"######Message discarded#####");
                        continue;
                    }
                    
                    //PrtOrder(HtmlBody.Replace("脳", "×").Replace("拢", "£"));
                    //PrtOrderWithTemplate(HtmlBody);
                    if (VERSION.Equals("2"))
                    {
                        HtmlBody = HtmlBody.Replace("<body style=\"", "<body style=\"font-family:Arial; ");
                    }
                    else if (VERSION.Equals("3"))
                    {
                        HtmlBody = HtmlBody.Replace("<body", "<body style=\"font-family:Arial; \"");
                    }

                    //读取Html失败时，跳出循环
                    if (!GetPrtInfo(HtmlBody))
                    {
                        message.MarkForDeletion();
                        continue;
                    }
                    //存在订单时不打印
                    if (SqlHelper.QueryId(@"SELECT mailID FROM Mail_ID WHERE orderID='" + orderId + "'"))
                    {
                        continue;
                    }

                    if (VERSION.Equals("2"))
                    {
                        HtmlBody = HtmlBody.Replace("h1", "h4").Replace("<p>", "").Replace("</p>", "<br />").Replace("<p style=\"width:94%;\">", "").Replace("<strong>", "").Replace("</strong>", "");
                        //HtmlBody = HtmlBody.Replace("h1", "h5");
                        HtmlBody = HtmlBody.Replace("<h4>", "").Replace("</h4>", "").Replace("<b>", "").Replace("</b>", "")
                                           .Replace("border-top:hidden;", "").Replace("style=\"border-top:hidden;\"", "");
                    }
                    else if (VERSION.Equals("3"))
                    {
                        //中文字体更大一号字体
                        HtmlBody = HtmlBody.Replace("<span style=\"font-size:18px;\">", "<span style=\"font-size:24px;\">");
                        HtmlBody = HtmlBody.Replace("h1", "h4").Replace("<p>", "").Replace("</p>", "<br />").Replace("<p style=\"width:94%;\">", "").Replace("<strong>", "").Replace("</strong>", "");
                        //HtmlBody = HtmlBody.Replace("h1", "h5");
                        HtmlBody = HtmlBody.Replace("<h4>", "").Replace("</h4>", "").Replace("<b>", "").Replace("</b>", "")
                                           .Replace("border-top:hidden;", "").Replace("style=\"border-top:hidden;\"", "");
                    }

                        //Print(HtmlBody);
                    webBrowser1.DocumentText = HtmlBody;
                    
                    //打印完成后插入数据
                    if (!SqlHelper.InsertId(@"INSERT INTO Mail_ID(mailID, orderID, orderType, orderTime, orderHtmlBody) VALUES('"
                                           + messagesCollection[i].UID + "', '"
                                           + orderId + "', '"
                                           + orderType + "', '"
                                           + orderDate + "', '"
                                           + HtmlBody + "')"))
                    {
                        MessageBox.Show(@"WRITE Data Error!", @"ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //int index = this.dgvOrder.Rows.Add();
                        this.dgvOrder.Rows.Insert(0, dgvOrder.Rows);
                        dgvOrder.Rows[0].Cells[0].Value = orderId;
                        dgvOrder.Rows[0].Cells[1].Value = orderDate;
                        dgvOrder.Rows[0].Cells[2].Value = orderType;
                        dgvOrder.Rows[0].Cells[3].Value = HtmlBody;

                        dgvOrder.Refresh();
                    }

                    //播放语音提示
                    if (File.Exists(Environment.CurrentDirectory + wavNewMail))
                    {
                        SoundPlayer player = new SoundPlayer(Environment.CurrentDirectory + wavNewMail);
                        player.Play();
                    }

                    for (int j = 0; j < PubCommon.GetRadioBtnValue(PRT_COUNT); j++)
                    {
                        webBrowser1.DocumentCompleted += wb_DocumentCompleted;

                        Console.Out.WriteLine("Wait:" + DateTime.Now.ToString("o"));
                        obj.Reset();
                        while (obj.WaitOne(1000, false) == false)
                        {
                            Application.DoEvents();
                            if (isPrint) obj.Set();
                        }
                        Console.Out.WriteLine("Finish:" + DateTime.Now.ToString("o"));
                    }

                    //完成后添加删除邮件
                    lstMessage.Add(message);
                }

                for (int i = lstMessage.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        lstMessage[i].MarkForDeletion();
                        Console.WriteLine(@"DELETE MAIL DONE:" + DateTime.Now.ToString("o"));
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(@"DELETE MAIL FAILURE:" + DateTime.Now.ToString("o"));
                        //throw;
                    }
                }

                if (popMail != null)
                {
                    //popMail = null;
                    try
                    {
                        popMail.Disconnect();
                    }
                    catch (Exception)
                    {
                        //Console.Out.WriteLine("Error if (popMail != null)");
                        SetRichTextValue(@"Error POPMail != null");
                    }
                }
            }
            else
            {
                try
                {
                    popMail.Disconnect();
                }
                catch (Exception)
                {
                    //Console.Out.WriteLine("Error else");
                    SetRichTextValue(@"Error ELSE");
                }
            }  
        }

        private void PrtOrderWithTemplate(string htmlText)
        {
            if (string.IsNullOrWhiteSpace(GetPrtStr(htmlText)))
            {
                return;
            }

            PrtTemplate prtTemplate = GetPrtStrWithTemplate(htmlText);
            List<string> lst = new List<string>();
            lst = PrtReplaceTemplate(prtTemplate);
            Print(lst);
        }

        private PrtTemplate GetPrtStrWithTemplate(string htmlText)
        {
            #region 字符串方式
            StringBuilder sb = new StringBuilder();

            PrtTemplate prtTemplate = new PrtTemplate();

            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlText);

                HtmlNode node;
                node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.HEAD_ORDER_ID);
                //sb.Append(GetSpace((39 - node.InnerText.Length) / 2) + node.InnerText.Replace("&nbsp;", "").Trim());
                orderId = node.InnerText.Replace("&nbsp;", "").Trim().Substring(node.InnerText.Replace("&nbsp;", "").Trim().IndexOf("#"));
                prtTemplate.OrderId = orderId;

                node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.HEAD_ORDER_TYPE);
                //sb.Append(GetSpace((39 - node.InnerText.Length) / 2) + node.InnerText.Replace("&nbsp;", "").Trim().ToUpper());
                orderType = node.InnerText.Replace("&nbsp;", "").Trim().Substring(0, node.InnerText.Replace("&nbsp;", "").Trim().IndexOf("ORDER")).ToUpper();
                prtTemplate.OrderType = orderType;

                node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.BODY_NAME);
                prtTemplate.Name = node.InnerText.Replace("&nbsp;", "").Trim();

                node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.BODY_PHONE);
                prtTemplate.Phone = node.InnerText.Replace("&nbsp;", "");

                node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.BODY_COLLECTION_ORDER_TIME);
                sb.Append(node.InnerText.Replace("&nbsp;", "").Trim());
                orderDate = node.InnerText.Replace("&nbsp;", "").Trim().Substring(node.InnerText.Replace("&nbsp;", "").Trim().IndexOf(":") + 1);
                prtTemplate.OrderTime = orderDate;

                prtTemplate.OrderItem = "Code" + GetSpace(2) + "Qty" + GetSpace(2) + "Name" + GetSpace(HtmlTextPath.PRT_MENUITEM_NUM + HtmlTextPath.PRT_OFFSET) + "Price" + "\n";
                
                HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/center[1]/table[1]/tbody[1]").ChildNodes;

                foreach (var n in nodeCollection)
                {
                    string[] sTmp = n.InnerText.Trim().Replace("\r\n", "*").Split('*');

                    sTmp = sTmp.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                    if (sTmp.Length >= 4) //菜品类
                    {
                        prtTemplate.OrderItem += GetTab(sTmp[0].Trim(), sTmp[1].Trim().Replace("&nbsp;", ""), sTmp[2].Trim(), sTmp[3].Trim());
                    }
                    else
                    {
                        //if (n.InnerText.Replace("\r\n", "").Trim().IndexOf("-") >= 0)
                        //{
                        //    prtTemplate.OrderItem += GetSpace(37 - n.InnerText.Replace("\r\n", "").Trim().Length) + n.InnerText.Replace("\r\n", "").Trim().Replace("-", GetSpace(2) + "-");
                        //}
                        //else
                        //{
                            //Item/SubTotal
                        if (n.InnerText.Replace("\r\n", "").Trim().Contains("Items"))
                        {
                            string[] s = n.InnerText.Replace("\r\n", "").Trim().Split(' ');
                            if (s.Length >= 2)
                            {
                                prtTemplate.ItemCount = s[0].Trim();
                                prtTemplate.SubTotal = s[s.Length - 1].Trim();
                            }
                        }
                        else if (n.InnerText.Replace("\r\n", "").Trim().Contains("Off"))
                        {
                            prtTemplate.Discount = n.InnerText.Replace("\r\n", "").Trim();
                        }
                        else if (n.InnerText.Replace("\r\n", "").Trim().Contains("Card Fee"))
                        {
                            prtTemplate.CardFee = n.InnerText.Replace("\r\n", "").Trim();
                        }

                            //prtTemplate.OrderItem += GetSpace(39 - n.InnerText.Replace("\r\n", "").Trim().Length) + n.InnerText.Replace("\r\n", "").Trim();
                        //}
                        //prtTemplate.OrderItem += "\n";
                    }
                }

                try
                {
                    HtmlNodeCollection hnc = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/center[1]/table[1]/tfoot[1]").ChildNodes;

                    foreach (var n in hnc.Where(s => s.InnerText.Replace("\r\n", "").Trim().Replace("&nbsp;", "").Length > 0))
                    {
                        if (n.InnerText.Replace("\r\n", "").Trim().Replace("&nbsp;", "").Substring(0, 4).ToUpper().Equals("FREE"))
                        {
                            //sb.Append(GetSpace((39 - n.InnerText.Replace("\r\n", "").Trim().Replace("&nbsp;", "").Length) / 2) + n.InnerText.Replace("\r\n", "").Trim().Replace("&nbsp;", ""));
                            //sb.Append(Environment.NewLine);

                            prtTemplate.FreeMenuItem = n.InnerText.Replace("\r\n", "").Trim().Replace("&nbsp;", "");
                        }
                        else
                        {
                            prtTemplate.Total = n.InnerText.Replace("\r\n", "").Trim().Replace("&nbsp;", "");
                        }
                    }
                }
                catch (Exception)
                {
                    //throw;
                }
                
                node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/p[4]");
                //Payment Status:&nbsp;PAID by Paypal 
                prtTemplate.PaymentStatus = node.InnerText.Replace("\r\n", "").Replace(@"&nbsp;", "").Trim();

                node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/p[5]");
                //Payment Status:&nbsp;PAID by Paypal 
                sb.Append(node.InnerText.Replace(@"&nbsp;", ""));
                //sb.Append(Environment.NewLine);
                prtTemplate.Remarks = node.InnerText.Replace(@"&nbsp;", "").Trim();

                node = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/p[6]");
                //Payment Status:&nbsp;PAID by Paypal 
                sb.Append(node.InnerText.Replace(@"&nbsp;", ""));
                prtTemplate.DeliveryTime = node.InnerText.Replace(@"&nbsp;", "").Trim();

                node = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/center[1]/table[1]/tbody[1]");
                Console.WriteLine(node.InnerText.Replace(@"&nbsp;", ""));

            }
            catch (Exception ex)
            {
                SetRichTextValue(@"Template ERROR:" + ex.InnerException);
            }

            return prtTemplate;

            #endregion
        }

        private List<string> PrtReplaceTemplate(PrtTemplate template)
        {
            try
            {
                if (!string.IsNullOrEmpty(MAIL_TEMPLATE))
                {
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{OrderId}", template.OrderId);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{OrderType}", template.OrderType);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{Name}", template.Name);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{Phone}", template.Phone);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{Address}", template.Address);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{City}", template.City);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{Postcode}", template.Postcode);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{OrderTime}", template.OrderTime);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{OrderItem}", template.OrderItem);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{ItemCount}", template.ItemCount);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{SubTotal}", template.SubTotal);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{Discount}", template.Discount);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{DeliveryPrice}", template.DeliveryPrice);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{ServiceCharge}", template.ServiceCharge);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{CardFee}", template.CardFee);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{Total}", template.Total);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{FreeMenuItem}", template.FreeMenuItem);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{Remarks}", template.Remarks);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{PaymentStatus}", template.PaymentStatus);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{DeliveryTime}", template.DeliveryTime);
                    MAIL_TEMPLATE = MAIL_TEMPLATE.Replace("{PayType}", template.PayType);

                    List<string> lst = MAIL_TEMPLATE.Split('\n').ToList();

                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (string.IsNullOrEmpty(lst[i])) lst.Remove(lst[i]);
                        else if (lst[i].Equals("\r")) lst.Remove(lst[i]);
                        else if (lst[i].Equals("\n")) lst.Remove(lst[i]);
                        else if (lst[i].Equals("\r\n")) lst.Remove(lst[i]);
                        else if (lst[i].Equals("\n\r")) lst.Remove(lst[i]);
                    }

                    return lst;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                SetRichTextValue(@"ReplaceTemplate ERROR:" + ex.InnerException);
                return null;
            }
        }

        #region 打印主体方法
        /// <summary>
        /// 打印主体方法
        /// </summary>
        /// <param name="lstStr">打印内容</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="strPrinterName">打印机名称</param>
        public void Print(List<string> lstStr)
        {
            PrintDocument printDocument = new PrintDocument();

            printDocument.PrintPage += (sender, e) =>
            {
                int fontheight = 0;
                foreach (var item in lstStr)
                {
                    e.Graphics.DrawString(item, new Font(HtmlTextPath.PRT_FONT, HtmlTextPath.PRT_FONT_SIZE), Brushes.Black, new Point(0, fontheight));
                    fontheight += new Font(HtmlTextPath.PRT_FONT, HtmlTextPath.PRT_FONT_SIZE).Height;
                }
            };
            printDocument.Print();
        }
        #endregion

        private void Print(string str)
        {
            webBrowser1.DocumentText = str;
            webBrowser1.DocumentCompleted += wb_DocumentCompleted;
            //webBrowser1.Dispose();
        }

        private void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.ReadyState < WebBrowserReadyState.Complete) return;

            string keyName = @"Software\Microsoft\Internet Explorer\PageSetup\";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
            {
                if (key != null)
                {
                    key.SetValue("footer", ""); //设置页脚为空
                    key.SetValue("header", ""); //设置页眉为空
                    //key.SetValue("Print_Background", true); //设置打印背景颜色
                    key.SetValue("margin_bottom", 0); //设置下页边距为0
                    key.SetValue("margin_left", 0); //设置左页边距为0
                    key.SetValue("margin_right", 0); //设置右页边距为0
                    key.SetValue("margin_top", 0); //设置上页边距为0
                    
                    webBrowser1.Print();
                    isPrint = true;

                    webBrowser1.DocumentCompleted -= wb_DocumentCompleted;
                }
            }
        }

        private bool GetPrtInfo(string htmlText)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlText);

                if (!"2".Equals(VERSION))
                {
                    HtmlNode node;
                    node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.HEAD_ORDER_ID);
                    orderId = node.InnerText.Replace("&nbsp;", "").Trim().Substring(node.InnerText.Replace("&nbsp;", "").Trim().IndexOf("#"));

                    node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.HEAD_ORDER_TYPE);
                    orderType = node.InnerText.Replace("&nbsp;", "").Trim().Substring(0, node.InnerText.Replace("&nbsp;", "").Trim().IndexOf("ORDER")).ToUpper();

                    node = doc.DocumentNode.SelectSingleNode(orderType.Trim().Equals(HtmlTextPath.ORDER_TYPE_COLLECTION) ? HtmlTextPath.BODY_COLLECTION_ORDER_TIME : HtmlTextPath.BODY_DELIVER_ORDER_TIME);
                    orderDate = node.InnerText.Replace("&nbsp;", "").Trim().Substring(node.InnerText.Replace("&nbsp;", "").Trim().IndexOf(":") + 1);

                    return true;
                }
                else
                {
                    HtmlNode node;
                    node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.HEAD_ORDER_ID);
                    orderId = node.InnerText.Replace("&nbsp;", "").Trim().Substring(node.InnerText.Replace("&nbsp;", "").Trim().IndexOf("#"));

                    node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.HEAD_ORDER_TYPE);
                    orderType = node.InnerText.Replace("&nbsp;", "").Trim().Substring(0, node.InnerText.Replace("&nbsp;", "").Trim().IndexOf("ORDER")).ToUpper();

                    node = doc.DocumentNode.SelectSingleNode(orderType.Trim().Equals(HtmlTextPath.ORDER_TYPE_COLLECTION) ? HtmlTextPath.BODY_COLLECTION_ORDER_TIME : HtmlTextPath.BODY_DELIVER_ORDER_TIME);
                    orderDate = node.InnerText.Replace("&nbsp;", "").Trim().Substring(node.InnerText.Replace("&nbsp;", "").Trim().IndexOf(":") + 1);

                    return true;
                } 

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);

                SetRichTextValue(DateTime.Now.ToString("o") + @"######ERR Message#####");
                
                return false;
            }
        }

        #region 判断网络是否为连接状态
        /// <summary>
        /// 判断网络是否为连接状态
        /// </summary>
        /// <returns></returns>
        private bool IsNetConnect()
        {
            bool isConnect = NetworkInterface.GetIsNetworkAvailable();
            if (!isConnect)
            {
                //是否存在语音文件
                if (File.Exists(Environment.CurrentDirectory + wavError))
                {
                    SoundPlayer player = new SoundPlayer(Environment.CurrentDirectory + wavError);
                    player.Play();
                }

                //增加提示
                SetRichTextValue(DateTime.Now.ToString("o") + @"######Network connection failed!######");
            }

            return isConnect;
        }
        #endregion

        private void dgvOrder_DoubleClick(object sender, EventArgs e)
        {
            if (dgvOrder.CurrentRow != null)
            {
                //Print(dgvOrder.CurrentRow.Cells[3].Value.ToString());
                webBrowser1.DocumentText = dgvOrder.CurrentRow.Cells[3].Value.ToString();

                for (int i = 0; i < PubCommon.GetRadioBtnValue(PRT_COUNT); i++)
                {
                    webBrowser1.DocumentCompleted += wb_DocumentCompleted;

                    Console.Out.WriteLine("Wait1:" + DateTime.Now.ToString("o"));
                    obj.Reset();
                    while (obj.WaitOne(1000, false) == false)
                    {
                        Application.DoEvents();
                        if (isPrint) obj.Set();
                    }
                    Console.Out.WriteLine("Finish1:" + DateTime.Now.ToString("o"));
                }
            }
        }

        private void panelErrorMsg_Click(object sender, EventArgs e)
        {
            panelErrorMsg.Hide();
        }

        private void SetRichTextValue(string sValue)
        {
            richTextBox1.Text = sValue + Environment.NewLine + richTextBox1.Text;
            richTextBox1.ScrollToCaret();
        }
    }
}
