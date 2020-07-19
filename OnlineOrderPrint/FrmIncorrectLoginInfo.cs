using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnlineOrderPrint
{
    public partial class FrmIncorrectLoginInfo : Form
    {
        public FrmIncorrectLoginInfo()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            FrmSysConf frmSysConf = new FrmSysConf();
            frmSysConf.ShowDialog();
        }
    }
}
