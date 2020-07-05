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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GetServerEmail;
using HtmlAgilityPack;
using jmail;
using LumiSoft.Net.Mail;
using LumiSoft.Net.POP3.Client;

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
        private string MAIL_SENDER = @"noreply@dolbynonline.co.uk";
        private static string MAIL_RECIVER = @"";

        private static List<string> textList;       //打印内容行

        private int timer_Int = 60000; // 60 * 1000 = 1 Minute

        private WebBrowser webBrowser = null;

        private string prtFilePath = "";

        private List<MenuItem> lstMi = new List<MenuItem>();

        private string orderId = "";

        private string orderDate = "";

        private string orderType = "";

        public static string wavNewMail = @"\NewMail.wav";

        public static string wavError = @"\ERROR.wav";

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

                node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.BODY_ORDER_TIME);
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
                User user = SqlHelper.Query("SELECT * FROM User");
                if (user != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(user.UsrName) ||
                            string.IsNullOrEmpty(user.UsrPwd) ||
                            string.IsNullOrEmpty(user.MailServer) ||
                            string.IsNullOrEmpty(user.MinsInt))
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
                        MAIL_USER_NAME = user.UsrName;
                        MAIL_USER_PWD = user.UsrPwd;
                        MAIL_POP = user.MailServer;
                        MAIL_SENDER = user.MailSender;

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

            sb.Append(sCode + GetSpace(6 - sCode.Length) + sQty + GetSpace(4 - sQty.Length));
            if (sName.Length > 22)
            {
                sb.Append(sName.Substring(0, 21));
                sb.Append(GetSpace(2) + sPrice);
                sb.Append(Environment.NewLine);
                sb.Append(GetSpace(11) + sName.Substring(21, sName.Length - 21));
                sb.Append(Environment.NewLine);
            }
            else
            {
                sb.Append(sName + GetSpace(22 - sName.Length));
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
                MessageBox.Show("打印失败." + ex.Message);
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

            ReciveMail();

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
                richTextBox1.Text += System.Environment.NewLine + "ex:" + ex;
                //MessageBox.Show("Can not connect email server");
                Console.Out.WriteLine("Can not connect email server" + DateTime.Now.ToString("o"));
                richTextBox1.Text += System.Environment.NewLine + "Can not connect email server:" + DateTime.Now.ToString("o");
                //if (File.Exists(Environment.CurrentDirectory + wavError))
                //{
                //    SoundPlayer player = new SoundPlayer(Environment.CurrentDirectory + wavError);
                //    player.Play();
                //}
                //return "";

                //popMail = null;

                //popMail.Connect(MAIL_USER_NAME, MAIL_USER_PWD, MAIL_POP);

                return;
            }

            Console.Out.WriteLine(DateTime.Now.ToString("o"));
            richTextBox1.Text += System.Environment.NewLine + DateTime.Now.ToString("o");
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
                                           + orderDate + "')"))
                    {
                        MessageBox.Show("Insert Data Error!");
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
                        richTextBox1.Text += System.Environment.NewLine + @"Error if (popMail != null)";
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
                    richTextBox1.Text += System.Environment.NewLine + @"Error else";
                }

                //return "";
            }
        }

        private void LumiSoftMail()
        {
            string sendmail, HtmlBody = "", date;

            POP3_Client popMail = new POP3_Client();
            
            try
            {
                popMail.Connect(MAIL_POP, 110, false);
                popMail.Login(MAIL_USER_NAME, MAIL_USER_PWD);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("ex:" + ex);
                richTextBox1.Text += System.Environment.NewLine + "ex:" + ex;
                Console.Out.WriteLine("Can not connect email server" + DateTime.Now.ToString("o"));
                richTextBox1.Text += System.Environment.NewLine + "Can not connect email server:" + DateTime.Now.ToString("o");
                return;
            }

            Console.Out.WriteLine(DateTime.Now.ToString("o"));
            richTextBox1.Text += System.Environment.NewLine + DateTime.Now.ToString("o");
            POP3_ClientMessageCollection messagesCollection = popMail.Messages;

            POP3_ClientMessage message = null;

            if (0 < messagesCollection.Count)
            {
                for (int i = messagesCollection.Count - 1; i < messagesCollection.Count; i--)
                {

                    if (!SqlHelper.QueryId(@"SELECT mailID FROM Mail_ID WHERE mailID='" + messagesCollection[i].UID + "'"))
                    {
                        try
                        {
                            message = popMail.Messages[i];
                        }
                        catch (Exception)
                        {
                            popMail.Connect(MAIL_POP, 587, true);
                            popMail.Login(MAIL_USER_NAME, MAIL_USER_PWD);
                        }
                    }
                    else
                    {
                        return;
                    }

                    Mail_Message mailMessage = null;
                    if (message != null)
                    {
                        byte[] messBytes = message.MessageToByte();
                        mailMessage = Mail_Message.ParseFromByte(messBytes);
                    }
                    
                    sendmail = mailMessage.From[0].Address;
                    HtmlBody = mailMessage.BodyHtmlText;
                    date = mailMessage.Date.ToString("d");
                    if (Convert.ToDateTime(date) < Convert.ToDateTime(DateTime.Now.ToShortDateString()))
                    {
                        break;
                    }

                    if (!sendmail.Equals(MAIL_SENDER))
                    {
                        continue;
                    }

                    //播放语音提示

                    if (File.Exists(Environment.CurrentDirectory + wavNewMail))
                    {
                        SoundPlayer player = new SoundPlayer(Environment.CurrentDirectory + wavNewMail);
                        player.Play();
                    }

                    PrtOrder(HtmlBody.Replace("脳", "×").Replace("拢", "£"));

                    //打印完成后插入数据
                    if (!SqlHelper.InsertId(@"INSERT INTO Mail_ID(mailID, orderID, orderType, orderTime) VALUES('"
                                           + messagesCollection[i].UID + "', '"
                                           + orderId + "', '"
                                           + orderType + "', '"
                                           + orderDate + "')"))
                    {
                        MessageBox.Show("Insert Data Error!");
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
                        richTextBox1.Text += System.Environment.NewLine + @"Error if (popMail != null)";
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
                    Console.Out.WriteLine("Error else");
                    richTextBox1.Text += System.Environment.NewLine + @"Error else";
                }
            }
        }
    }
}
