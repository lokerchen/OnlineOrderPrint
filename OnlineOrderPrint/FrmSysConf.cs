using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GetServerEmail;

namespace OnlineOrderPrint
{
    public partial class FrmSysConf : Form
    {
        public FrmSysConf()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmSysConf_Load(object sender, EventArgs e)
        {
            User user = SqlHelper.Query("SELECT * FROM User");
            if (user != null) 
            {
                txtUsrName.Text = user.UsrName;
                txtUsrPwd.Text = user.UsrPwd;
                txtMinsInt.Text = user.MinsInt;
                txtMailServer.Text = user.MailServer;
                txtReceiverMail.Text = user.MailSender;

                if (user.PrtCount.Equals(HtmlTextPath.PRT_COUNT_ONE))
                {
                    rbOne.Checked = true;
                    rbTwo.Checked = false;
                    rbThree.Checked = false;
                }
                else if (user.PrtCount.Equals(HtmlTextPath.PRT_COUNT_TWO))
                {
                    rbOne.Checked = false;
                    rbTwo.Checked = true;
                    rbThree.Checked = false;
                }
                else if (user.PrtCount.Equals(HtmlTextPath.PRT_COUNT_THREE))
                {
                    rbOne.Checked = false;
                    rbTwo.Checked = false;
                    rbThree.Checked = true;
                }
                else
                {
                    rbOne.Checked = true;
                    rbTwo.Checked = false;
                    rbThree.Checked = false;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!SqlHelper.QueryId("SELECT * FROM User WHERE UsrName='" + txtUsrName.Text + "'"))

                string prtCount = "";

                if (rbOne.Checked) prtCount = rbOne.Text;
                else if (rbTwo.Checked) prtCount = rbTwo.Text;
                else if (rbThree.Checked) prtCount = rbThree.Text;
                else prtCount = rbOne.Text;

                if (!SqlHelper.QueryId("SELECT * FROM User"))
                {
                    SqlHelper.InsertId("INSERT INTO User VALUES('" + txtUsrName.Text + "','"
                                       + txtUsrPwd.Text + "','"
                                       + txtMinsInt.Text + "','"
                                       + txtMailServer.Text + "','"
                                       + txtReceiverMail.Text +"','"
                                       + prtCount + "')");
                }
                else
                {
                    SqlHelper.InsertId("UPDATE User SET UsrName='" + txtUsrName.Text 
                                                    + "', UsrPwd='" + txtUsrPwd.Text 
                                                    + "', MinsInt='" + txtMinsInt.Text 
                                                    + "', MailServer='" + txtMailServer.Text
                                                    + "', MailSender='" + txtReceiverMail.Text
                                                    + "', PrtCount='" + prtCount + "'");
                }

                MessageBox.Show(@"Save success!", @"DONE", MessageBoxButtons.OK);
            }
            catch (Exception)
            {
                MessageBox.Show(@"Save failure!", @"ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void lblMailServer_DoubleClick(object sender, EventArgs e)
        {
            lblMailServer.Visible = false;
            txtMailServer.Visible = true;
        }

        private void lblReceiverMail_DoubleClick(object sender, EventArgs e)
        {
            lblReceiverMail.Visible = false;
            txtReceiverMail.Visible = true;
        }

        private void rbOne_Click(object sender, EventArgs e)
        {
            rbOne.Checked = true;
            rbTwo.Checked = false;
            rbThree.Checked = false;
        }

        private void rbTwo_Click(object sender, EventArgs e)
        {
            rbOne.Checked = false;
            rbTwo.Checked = true;
            rbThree.Checked = false;
        }

        private void rbThree_Click(object sender, EventArgs e)
        {
            rbOne.Checked = false;
            rbTwo.Checked = false;
            rbThree.Checked = true;
        }
    }
}
