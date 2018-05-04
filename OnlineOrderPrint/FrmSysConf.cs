﻿using System;
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
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!SqlHelper.QueryId("SELECT * FROM User WHERE UsrName='" + txtUsrName.Text + "'"))
                if (!SqlHelper.QueryId("SELECT * FROM User"))
                {
                    SqlHelper.InsertId("INSERT INTO User VALUES('" + txtUsrName.Text + "','"
                                       + txtUsrPwd.Text + "','"
                                       + txtMinsInt.Text + "')");
                }
                else
                {
                    SqlHelper.InsertId("UPDATE User SET UsrName='" + txtUsrName.Text + "', UsrPwd='" + txtUsrPwd.Text + "', MinsInt='" + txtMinsInt.Text + "'");
                }

                MessageBox.Show("Save success!");
            }
            catch (Exception)
            {
                MessageBox.Show("Save failure!");
                throw;
            }
        }

        private void lblMailServer_DoubleClick(object sender, EventArgs e)
        {
            lblMailServer.Visible = false;
            txtMailServer.Visible = true;
        }
    }
}
