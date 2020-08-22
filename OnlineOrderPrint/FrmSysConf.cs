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
        public string CompanyName
        {
            get { return txtCompanyName.Text; }
            set { this.txtCompanyName.Text = value; }
        }

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
                txtCompanyName.Text = user.CompanyName;
                txtVersion.Text = user.Version;

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

                string sVersion = @"2";
                if (string.IsNullOrEmpty(txtVersion.Text)) sVersion = @"2";
                else if ("2".Equals(txtVersion.Text)) sVersion = @"2";
                else if ("3".Equals(txtVersion.Text)) sVersion = @"3";
                else sVersion = @"2";

                if (!SqlHelper.QueryId("SELECT * FROM User"))
                {
                    SqlHelper.InsertId("INSERT INTO User VALUES('" + txtUsrName.Text + "','"
                                       + txtUsrPwd.Text + "','"
                                       + txtMinsInt.Text + "','"
                                       + txtMailServer.Text + "','"
                                       + txtReceiverMail.Text + "','"
                                       + prtCount + "','"
                                       + sVersion + "','"
                                       + txtCompanyName.Text + "')");
                }
                else
                {
                    SqlHelper.InsertId("UPDATE User SET UsrName='" + txtUsrName.Text
                                       + "', UsrPwd='" + txtUsrPwd.Text
                                       + "', MinsInt='" + txtMinsInt.Text
                                       + "', MailServer='" + txtMailServer.Text
                                       + "', MailSender='" + txtReceiverMail.Text
                                       + "', PrtCount='" + prtCount
                                       + "', Version='" + sVersion + "'"
                                       + ", CompanyName='" + txtCompanyName.Text + "'");
                }

                MessageBox.Show(@"Save success!", @"DONE", MessageBoxButtons.OK);
            }
            catch (Exception)
            {
                MessageBox.Show(@"Save failure!", @"ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        //private void lblMailServer_DoubleClick(object sender, EventArgs e)
        //{
        //    //lblMailServer.Visible = false;
        //    //txtMailServer.Visible = true;
        //}

        //private void lblReceiverMail_DoubleClick(object sender, EventArgs e)
        //{
        //    //lblReceiverMail.Visible = false;
        //    //txtReceiverMail.Visible = true;
        //}

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

        private void FrmSysConf_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void FrmSysConf_KeyUp(object sender, KeyEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control
                && e.KeyCode == Keys.F8)
            {
                txtCompanyName.Visible = txtCompanyName.Visible == false;
            }

            if (e.KeyCode == Keys.F1)
            {
                txtVersion.Visible = txtVersion.Visible == false;
                lblVersion.Visible = lblVersion.Visible == false;
            }

            if (e.KeyCode == Keys.F2)
            {
                lblMailServer.Visible = lblMailServer.Visible == false;
                txtMailServer.Visible = txtMailServer.Visible == false;
            }

            if (e.KeyCode == Keys.F3)
            {
                lblReceiverMail.Visible = lblReceiverMail.Visible == false;
                txtReceiverMail.Visible = txtReceiverMail.Visible == false;
            }
        }
    }
}
