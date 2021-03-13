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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraPrinting;
using System.Runtime.InteropServices;
using DevExpress.Office;

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

        [DllImport("kernel32.dll", EntryPoint = "GetSystemDefaultLCID")]
        public static extern int GetSystemDefaultLCID();
        [DllImport("kernel32.dll", EntryPoint = "SetLocaleInfoA")]
        public static extern int SetLocaleInfo(int Locale, int LCType, string lpLCData);
        public const int LOCALE_SLONGDATE = 0x20;
        public const int LOCALE_SSHORTDATE = 0x1F;
        public const int LOCALE_STIME = 0x1003;

        private string strSource = "";

        public FrmMain()
        {
            InitializeComponent();
        }

        private void ReciveMail()
        {
            //JMailRecive();
            LumiSoftMail();
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

            if (frmSysConf.ShowDialog() == DialogResult.OK)
            {
                QueryUser();
                if (!string.IsNullOrEmpty(frmSysConf.CompanyName)) lblCompanyName.Text = frmSysConf.CompanyName;
            }
        }

        private void timerOrder_Tick(object sender, EventArgs e)
        {
            //Recive mail
            //string strHtml = ReciveMail();
            timerOrder.Enabled = false;
            btnRetrieveOrder.Enabled = false;
            btnRetrieveOrder.BackColor = Color.DimGray;

            ReciveMail();

            btnRetrieveOrder.BackColor = Color.MidnightBlue;
            timerOrder.Enabled = true;
            btnRetrieveOrder.Enabled = true;

            //重组打印
            //Hashtable hs = GetPrtStr(strHtml);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            #region Timer
            try
            {
                //SetDateTimeFormat();

                //网络连接判断
                if (!IsNetConnect()) return;

                QueryUser();

                string strFileName = Environment.CurrentDirectory + @"\Login.jpg";
                if (File.Exists(strFileName)) pictureBoxLogo.Load(strFileName);
                else pictureBoxLogo.Visible = false;
            }
            catch (Exception)
            {
                throw;
            }
            #endregion
        }
        
        private void btnRetrieveOrder_Click(object sender, EventArgs e)
        {
            timerOrder.Enabled = false;
            btnRetrieveOrder.Enabled = false;
            btnRetrieveOrder.BackColor = Color.DimGray;

            ReciveMail();

            btnRetrieveOrder.BackColor = Color.MidnightBlue;
            timerOrder.Enabled = true;
            btnRetrieveOrder.Enabled = true;
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
                
                if (string.IsNullOrEmpty(MAIL_USER_NAME) || string.IsNullOrEmpty(MAIL_USER_PWD) || string.IsNullOrEmpty(MAIL_POP) || string.IsNullOrEmpty(MAIL_SENDER) ||
                    string.IsNullOrEmpty(PRT_COUNT) || string.IsNullOrEmpty(COMPANY_NAME))
                {
                    QueryUser();
                }

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
                //SetRichTextValue("MAIL_USER_NAME:" + MAIL_USER_NAME + "MAIL_USER_PWD:" + MAIL_USER_PWD + "MAIL_POP:" + MAIL_POP + "MAIL_SENDER:" + MAIL_SENDER + "PRT_COUNT:" + PRT_COUNT + "COMPANY_NAME:" + COMPANY_NAME);
                SetRichTextValue(DateTime.Now.ToString("o") + @"######Can not connect email server######");
                return;
            }

            Console.Out.WriteLine(DateTime.Now.ToString("o"));
            SetRichTextValue(DateTime.Now.ToString("o"));
            POP3_ClientMessageCollection messagesCollection = popMail.Messages;

            POP3_ClientMessage message = null;

            //存放需删除的邮件
            List<POP3_ClientMessage> lstMessage = new List<POP3_ClientMessage>();

            string strMessage = "";

            if (0 < messagesCollection.Count)
            {
                //for (int i = messagesCollection.Count - 1; i >= 0; i--)
                for (var index = 0; index < popMail.Messages.Count; index++)
                {
                    POP3_ClientMessage mail = popMail.Messages[index];
                    
                    try
                    {
                        message = mail;
                    }
                    catch (Exception)
                    {
                        popMail.Timeout = HtmlTextPath.EMAIL_TIME_OUT;
                        popMail.Connect(MAIL_POP, HtmlTextPath.EMAIL_PORT, true);
                        popMail.Login(MAIL_USER_NAME, MAIL_USER_PWD);
                    }

                    Mail_Message mailMessage = null;

                    try
                    {
                        if (message != null)
                        {
                            byte[] messBytes = message.MessageToByte();
                            mailMessage = Mail_Message.ParseFromByte(messBytes);
                        }
                        else
                        {
                            continue;
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
                    
                    if (!sendmail.Equals(MAIL_SENDER))
                    {
                        message.MarkForDeletion();
                        SetRichTextValue(DateTime.Now.ToString("o") + @"######===" + sendmail + "===Message discarded#####");
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
                        SetRichTextValue(DateTime.Now.ToString("o") + @"######===" + orderId + "===OLD Message discarded#####");
                        continue;
                    }

                    ////DataGridView中的订单号不能重复
                    //for (int j = 0; j < dgvOrder.RowCount; j++)
                    //{
                    //    if (dgvOrder.Rows[j].Cells[0].Value != null)
                    //    {
                    //        if (!string.IsNullOrEmpty(dgvOrder.Rows[j].Cells[0].Value.ToString()))
                    //        {
                    //            if (dgvOrder.Rows[j].Cells[0].Value.ToString().Equals(orderId))
                    //            {
                    //                message.MarkForDeletion();
                    //                SetRichTextValue(DateTime.Now.ToString("o") + @"######===" + orderId + "===DGV Message discarded#####");
                    //                continue;
                    //            }
                    //        }
                    //    }
                    //}

                    //存在订单时不打印
                    //if (SqlHelper.QueryId(@"SELECT mailID FROM Mail_ID WHERE orderID='" + orderId + "'"))
                    //{
                    //    SetRichTextValue(DateTime.Now.ToString("o") + @"######Duplicate order ID######");
                    //    continue;
                    //}

                    if (VERSION.Equals("2"))
                    {
                        HtmlBody = HtmlBody.Replace("h1", "h4").Replace("<p>", "").Replace("</p>", "<br />")
                            .Replace("<p style=\"width:94%;\">", "").Replace("<strong>", "").Replace("</strong>", "");
                        //HtmlBody = HtmlBody.Replace("h1", "h5");
                        HtmlBody = HtmlBody.Replace("<h4>", "").Replace("</h4>", "").Replace("<b>", "")
                            .Replace("</b>", "")
                            .Replace("border-top:hidden;", "").Replace("style=\"border-top:hidden;\"", "");
                    }
                    else if (VERSION.Equals("3"))
                    {
                        //中文字体更大一号字体
                        HtmlBody = HtmlBody.Replace("<span style=\"font-size:18px;\">",
                            "<span style=\"font-size:24px;\">");
                        HtmlBody = HtmlBody.Replace("h1", "h4").Replace("<p>", "").Replace("</p>", "<br />")
                            .Replace("<p style=\"width:94%;\">", "").Replace("<strong>", "").Replace("</strong>", "");
                        //HtmlBody = HtmlBody.Replace("h1", "h5");
                        HtmlBody = HtmlBody.Replace("<h4>", "").Replace("</h4>", "").Replace("<b>", "")
                            .Replace("</b>", "")
                            .Replace("border-top:hidden;", "").Replace("style=\"border-top:hidden;\"", "");
                    }

                    
                    //打印完成后插入数据
                    if (!SqlHelper.InsertId(
                        @"INSERT INTO Mail_ID(mailID, orderID, orderType, orderTime, orderHtmlBody) VALUES('"
                        + mail.UID + "', '"
                        + orderId + "', '"
                        + orderType + "', '"
                        + orderDate + "', '')"))
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

                    
                    SetRichTextValue(@"#Time Printing order number=" + orderId);
                    
                    PrtContent(HtmlBody);
                    
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
                    SetRichTextValue(DateTime.Now.ToString("o") + @"#No mail received#");
                    popMail.Disconnect();
                }
                catch (Exception)
                {
                    //Console.Out.WriteLine("Error else");
                    SetRichTextValue(@"Error ELSE");
                }
            }  
        }

        private void PrtContent(string strText)
        {
            RichEditDocumentServer server = new RichEditDocumentServer();
            //server.LoadDocument(strFileName, DocumentFormat.Html);
            server.BeginUpdate();
            server.Document.HtmlText = strText;

            server.Document.Unit = DocumentUnit.Point;

            foreach (Section section in server.Document.Sections)
            {
                //section.Page.PaperKind = PaperKind.Custom;
                section.Page.Landscape = false;
                section.Page.Width = 200;
                section.Margins.Left = 1f;
                section.Margins.Right = 1f;
                section.Margins.Top = 0f;
                section.Margins.Bottom = 0f;
            }

            server.Document.DefaultParagraphProperties.Alignment = ParagraphAlignment.Center;

            PrintableComponentLink link = new PrintableComponentLink();
            PrintingSystem ps = new PrintingSystem();
            ps.Links.Add(link);
            link.Component = server;
            link.PrintingSystem.ShowMarginsWarning = false;
            link.PrintingSystem.ShowPrintStatusDialog = false;

            link.CreateDocument();

            //PrinterSettings pSet = new PrinterSettings();
            //pSet.Copies = 2;
            //pSet.PrinterName = strDefaultPrintName;
            //ps.PreviewFormEx.Show();
            //link.ShowPreview();
            SetRichTextValue(DateTime.Now.ToString("o") + "###Print Count = 1###");
            server.Print();
            //ps.PreviewFormEx.Show();
            //link.ShowPreview();
            //link.Print();
            //ps.Print();
        }

        private bool GetPrtInfo(string htmlText)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlText);

                if (!"2".Equals(VERSION))
                {
                    try
                    {
                        HtmlNode node;
                        node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.HEAD_ORDER_ID);
                        orderId = node.InnerText.Replace("&nbsp;", "").Trim().Substring(node.InnerText.Replace("&nbsp;", "").Trim().IndexOf("#"));

                        node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.HEAD_ORDER_TYPE);
                        orderType = node.InnerText.Replace("&nbsp;", "").Trim().Substring(0, node.InnerText.Replace("&nbsp;", "").Trim().IndexOf("ORDER")).ToUpper();

                        node = doc.DocumentNode.SelectSingleNode(orderType.Trim().Equals(HtmlTextPath.ORDER_TYPE_COLLECTION) ? HtmlTextPath.BODY_COLLECTION_ORDER_TIME : HtmlTextPath.BODY_DELIVER_ORDER_TIME);
                        orderDate = node.InnerText.Replace("&nbsp;", "").Trim().Substring(node.InnerText.Replace("&nbsp;", "").Trim().IndexOf(":") + 1);

                        return true;
                        //return Convert.ToDateTime(orderDate) >= Convert.ToDateTime(DateTime.Now.AddDays(-2).ToShortDateString());
                    }
                    catch (Exception)
                    {
                        return false;
                    } 
                }
                else
                {
                    try
                    {
                        HtmlNode node;
                        node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.HEAD_ORDER_ID);
                        orderId = node.InnerText.Replace("&nbsp;", "").Trim().Substring(node.InnerText.Replace("&nbsp;", "").Trim().IndexOf("#"));

                        node = doc.DocumentNode.SelectSingleNode(HtmlTextPath.HEAD_ORDER_TYPE);
                        orderType = node.InnerText.Replace("&nbsp;", "").Trim().Substring(0, node.InnerText.Replace("&nbsp;", "").Trim().IndexOf("ORDER")).ToUpper();

                        node = doc.DocumentNode.SelectSingleNode(orderType.Trim().Equals(HtmlTextPath.ORDER_TYPE_COLLECTION) ? HtmlTextPath.BODY_COLLECTION_ORDER_TIME : HtmlTextPath.BODY_DELIVER_ORDER_TIME);
                        orderDate = node.InnerText.Replace("&nbsp;", "").Trim().Substring(node.InnerText.Replace("&nbsp;", "").Trim().IndexOf(":") + 1);

                        return true;
                        //return Convert.ToDateTime(orderDate) >= Convert.ToDateTime(DateTime.Now.AddDays(-2).ToShortDateString());
                    }
                    catch (Exception)
                    {
                        return false;
                    }
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
                SetRichTextValue(@"#Double Click Printing order number=" + dgvOrder.CurrentRow.Cells[0].Value.ToString());

                PrtContent(dgvOrder.CurrentRow.Cells[3].Value.ToString());
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

        private void QueryUser()
        {
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
                }
            }
        }

        private void btnReprint_Click(object sender, EventArgs e)
        {
            //if (dgvOrder.CurrentRow != null)
            //{
            //    //Print(dgvOrder.CurrentRow.Cells[3].Value.ToString());
            //    SetRichTextValue(@"#Reprinting order number=" + dgvOrder.CurrentRow.Cells[0].Value.ToString());
            //    webBrowser1.DocumentText = dgvOrder.CurrentRow.Cells[3].Value.ToString();

            //    webBrowser1.DocumentCompleted += wb_DocumentCompleted;
            //    obj.Reset();
            //    Application.DoEvents();
            //    obj.Set();
            //    webBrowser1.DocumentCompleted -= wb_DocumentCompleted;
            //}
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //删除数据表中的数据
            SqlHelper.ClearData(@"DELETE FROM Mail_ID");
        }
    }
}
