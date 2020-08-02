namespace OnlineOrderPrint
{
    partial class FrmSysConf
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtReceiverMail = new System.Windows.Forms.TextBox();
            this.lblReceiverMail = new System.Windows.Forms.Label();
            this.txtMailServer = new System.Windows.Forms.TextBox();
            this.txtUsrPwd = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUsrName = new System.Windows.Forms.TextBox();
            this.lblMailServer = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtCompanyName = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbThree = new System.Windows.Forms.RadioButton();
            this.rbTwo = new System.Windows.Forms.RadioButton();
            this.rbOne = new System.Windows.Forms.RadioButton();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtMinsInt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtReceiverMail);
            this.groupBox1.Controls.Add(this.lblReceiverMail);
            this.groupBox1.Controls.Add(this.txtMailServer);
            this.groupBox1.Controls.Add(this.txtUsrPwd);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtUsrName);
            this.groupBox1.Controls.Add(this.lblMailServer);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(8, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(491, 235);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MailBox Setting";
            // 
            // txtReceiverMail
            // 
            this.txtReceiverMail.Location = new System.Drawing.Point(23, 187);
            this.txtReceiverMail.Name = "txtReceiverMail";
            this.txtReceiverMail.Size = new System.Drawing.Size(446, 38);
            this.txtReceiverMail.TabIndex = 7;
            this.txtReceiverMail.Text = "noreply@dolbynonline.co.uk";
            this.txtReceiverMail.Visible = false;
            // 
            // lblReceiverMail
            // 
            this.lblReceiverMail.AutoSize = true;
            this.lblReceiverMail.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblReceiverMail.Location = new System.Drawing.Point(16, 187);
            this.lblReceiverMail.Name = "lblReceiverMail";
            this.lblReceiverMail.Size = new System.Drawing.Size(209, 39);
            this.lblReceiverMail.TabIndex = 6;
            this.lblReceiverMail.Text = "Receiver Mail";
            this.lblReceiverMail.DoubleClick += new System.EventHandler(this.lblReceiverMail_DoubleClick);
            // 
            // txtMailServer
            // 
            this.txtMailServer.Location = new System.Drawing.Point(23, 139);
            this.txtMailServer.Name = "txtMailServer";
            this.txtMailServer.Size = new System.Drawing.Size(446, 38);
            this.txtMailServer.TabIndex = 5;
            this.txtMailServer.Text = "pop.1and1.co.uk";
            this.txtMailServer.Visible = false;
            // 
            // txtUsrPwd
            // 
            this.txtUsrPwd.Location = new System.Drawing.Point(154, 87);
            this.txtUsrPwd.Name = "txtUsrPwd";
            this.txtUsrPwd.PasswordChar = '*';
            this.txtUsrPwd.Size = new System.Drawing.Size(315, 38);
            this.txtUsrPwd.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 31);
            this.label3.TabIndex = 3;
            this.label3.Text = "Password";
            // 
            // txtUsrName
            // 
            this.txtUsrName.Location = new System.Drawing.Point(154, 37);
            this.txtUsrName.Name = "txtUsrName";
            this.txtUsrName.Size = new System.Drawing.Size(315, 38);
            this.txtUsrName.TabIndex = 2;
            // 
            // lblMailServer
            // 
            this.lblMailServer.AutoSize = true;
            this.lblMailServer.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMailServer.Location = new System.Drawing.Point(16, 139);
            this.lblMailServer.Name = "lblMailServer";
            this.lblMailServer.Size = new System.Drawing.Size(199, 39);
            this.lblMailServer.TabIndex = 0;
            this.lblMailServer.Text = "Mail SERVER";
            this.lblMailServer.DoubleClick += new System.EventHandler(this.lblMailServer_DoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 31);
            this.label2.TabIndex = 1;
            this.label2.Text = "User Name";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtCompanyName);
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(8, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(542, 553);
            this.panel1.TabIndex = 1;
            // 
            // txtCompanyName
            // 
            this.txtCompanyName.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtCompanyName.Location = new System.Drawing.Point(8, 419);
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.Size = new System.Drawing.Size(505, 38);
            this.txtCompanyName.TabIndex = 8;
            this.txtCompanyName.Visible = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbThree);
            this.groupBox4.Controls.Add(this.rbTwo);
            this.groupBox4.Controls.Add(this.rbOne);
            this.groupBox4.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox4.Location = new System.Drawing.Point(8, 321);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(505, 83);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Copy of printing";
            // 
            // rbThree
            // 
            this.rbThree.AutoSize = true;
            this.rbThree.Location = new System.Drawing.Point(367, 39);
            this.rbThree.Name = "rbThree";
            this.rbThree.Size = new System.Drawing.Size(83, 27);
            this.rbThree.TabIndex = 2;
            this.rbThree.TabStop = true;
            this.rbThree.Text = "THREE";
            this.rbThree.UseVisualStyleBackColor = true;
            this.rbThree.Click += new System.EventHandler(this.rbThree_Click);
            // 
            // rbTwo
            // 
            this.rbTwo.AutoSize = true;
            this.rbTwo.Location = new System.Drawing.Point(182, 39);
            this.rbTwo.Name = "rbTwo";
            this.rbTwo.Size = new System.Drawing.Size(72, 27);
            this.rbTwo.TabIndex = 1;
            this.rbTwo.TabStop = true;
            this.rbTwo.Text = "TWO";
            this.rbTwo.UseVisualStyleBackColor = true;
            this.rbTwo.Click += new System.EventHandler(this.rbTwo_Click);
            // 
            // rbOne
            // 
            this.rbOne.AutoSize = true;
            this.rbOne.Location = new System.Drawing.Point(23, 39);
            this.rbOne.Name = "rbOne";
            this.rbOne.Size = new System.Drawing.Size(68, 27);
            this.rbOne.TabIndex = 0;
            this.rbOne.TabStop = true;
            this.rbOne.Text = "ONE";
            this.rbOne.UseVisualStyleBackColor = true;
            this.rbOne.Click += new System.EventHandler(this.rbOne_Click);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExit.Location = new System.Drawing.Point(334, 482);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(124, 53);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "EXIT";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSave.Location = new System.Drawing.Point(69, 482);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(124, 53);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "SAVE";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtMinsInt);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(8, 248);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(507, 67);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            // 
            // txtMinsInt
            // 
            this.txtMinsInt.Location = new System.Drawing.Point(260, 20);
            this.txtMinsInt.Name = "txtMinsInt";
            this.txtMinsInt.Size = new System.Drawing.Size(73, 38);
            this.txtMinsInt.TabIndex = 5;
            this.txtMinsInt.Text = "1";
            this.txtMinsInt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(339, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(166, 31);
            this.label5.TabIndex = 1;
            this.label5.Text = "mins. interval";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(5, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(249, 31);
            this.label6.TabIndex = 0;
            this.label6.Text = "Auto download freq:";
            // 
            // FrmSysConf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 577);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "FrmSysConf";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmSysConf";
            this.Load += new System.EventHandler(this.FrmSysConf_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FrmSysConf_KeyUp);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtUsrPwd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUsrName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblMailServer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtMinsInt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMailServer;
        private System.Windows.Forms.Label lblReceiverMail;
        private System.Windows.Forms.TextBox txtReceiverMail;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbThree;
        private System.Windows.Forms.RadioButton rbTwo;
        private System.Windows.Forms.RadioButton rbOne;
        private System.Windows.Forms.TextBox txtCompanyName;
    }
}