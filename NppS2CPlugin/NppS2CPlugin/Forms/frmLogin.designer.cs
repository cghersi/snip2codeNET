//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
namespace NppPluginNet
{
    partial class frmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            this.S2cLogo = new System.Windows.Forms.PictureBox();
            this.retrievePwdLbl = new System.Windows.Forms.Label();
            this.retrievePwdBtn = new System.Windows.Forms.LinkLabel();
            this.usernameLbl = new System.Windows.Forms.Label();
            this.usernameTxt = new System.Windows.Forms.TextBox();
            this.LoginBtn = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.errorTxt = new System.Windows.Forms.TextBox();
            this.passwrodLbl = new System.Windows.Forms.Label();
            this.passwordTxt = new System.Windows.Forms.TextBox();
            this.signupBtn = new System.Windows.Forms.LinkLabel();
            this.signupLbl = new System.Windows.Forms.Label();
            this.oneAllbrowser = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.S2cLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // S2cLogo
            // 
            this.S2cLogo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.S2cLogo.Image = global::NppPluginNet.Properties.Resources.Snip2Code_40x240;
            this.S2cLogo.Location = new System.Drawing.Point(58, 18);
            this.S2cLogo.Name = "S2cLogo";
            this.S2cLogo.Size = new System.Drawing.Size(229, 51);
            this.S2cLogo.TabIndex = 0;
            this.S2cLogo.TabStop = false;
            // 
            // retrievePwdLbl
            // 
            this.retrievePwdLbl.AutoSize = true;
            this.retrievePwdLbl.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.retrievePwdLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.retrievePwdLbl.Location = new System.Drawing.Point(23, 269);
            this.retrievePwdLbl.Name = "retrievePwdLbl";
            this.retrievePwdLbl.Size = new System.Drawing.Size(141, 16);
            this.retrievePwdLbl.TabIndex = 1000;
            this.retrievePwdLbl.Text = "Problem loggin in?";
            this.retrievePwdLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // retrievePwdBtn
            // 
            this.retrievePwdBtn.AutoSize = true;
            this.retrievePwdBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.retrievePwdBtn.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.retrievePwdBtn.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(145)))), ((int)(((byte)(27)))));
            this.retrievePwdBtn.Location = new System.Drawing.Point(166, 269);
            this.retrievePwdBtn.Name = "retrievePwdBtn";
            this.retrievePwdBtn.Size = new System.Drawing.Size(163, 16);
            this.retrievePwdBtn.TabIndex = 4;
            this.retrievePwdBtn.TabStop = true;
            this.retrievePwdBtn.Text = "Retrieve your password";
            this.retrievePwdBtn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.retrievePwdBtn_LinkClicked);
            // 
            // usernameLbl
            // 
            this.usernameLbl.AutoSize = true;
            this.usernameLbl.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.usernameLbl.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.usernameLbl.Location = new System.Drawing.Point(40, 91);
            this.usernameLbl.Name = "usernameLbl";
            this.usernameLbl.Size = new System.Drawing.Size(72, 16);
            this.usernameLbl.TabIndex = 1002;
            this.usernameLbl.Text = "Username";
            this.usernameLbl.Click += new System.EventHandler(this.suggestionLabel_MouseLeftButtonUp);
            // 
            // usernameTxt
            // 
            this.usernameTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.usernameTxt.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameTxt.ForeColor = System.Drawing.Color.Black;
            this.usernameTxt.Location = new System.Drawing.Point(35, 86);
            this.usernameTxt.Name = "usernameTxt";
            this.usernameTxt.Size = new System.Drawing.Size(272, 27);
            this.usernameTxt.TabIndex = 1;
            this.usernameTxt.Enter += new System.EventHandler(this.TxtInEditMode);
            this.usernameTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.usernameTxt.Leave += new System.EventHandler(this.TxtNotEditMode);
            // 
            // LoginBtn
            // 
            this.LoginBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.LoginBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.LoginBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LoginBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LoginBtn.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginBtn.ForeColor = System.Drawing.Color.White;
            this.LoginBtn.Location = new System.Drawing.Point(123, 163);
            this.LoginBtn.Name = "LoginBtn";
            this.LoginBtn.Size = new System.Drawing.Size(94, 28);
            this.LoginBtn.TabIndex = 3;
            this.LoginBtn.Text = "LOGIN";
            this.LoginBtn.UseVisualStyleBackColor = false;
            this.LoginBtn.Click += new System.EventHandler(this.LoginBtn_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(25, 218);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(300, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 11;
            this.progressBar.Visible = false;
            // 
            // errorTxt
            // 
            this.errorTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.errorTxt.BackColor = System.Drawing.Color.White;
            this.errorTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.errorTxt.Cursor = System.Windows.Forms.Cursors.Default;
            this.errorTxt.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errorTxt.ForeColor = System.Drawing.Color.Red;
            this.errorTxt.Location = new System.Drawing.Point(23, 197);
            this.errorTxt.Multiline = true;
            this.errorTxt.Name = "errorTxt";
            this.errorTxt.ReadOnly = true;
            this.errorTxt.Size = new System.Drawing.Size(300, 64);
            this.errorTxt.TabIndex = 12;
            this.errorTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.errorTxt.Visible = false;
            // 
            // passwrodLbl
            // 
            this.passwrodLbl.AutoSize = true;
            this.passwrodLbl.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.passwrodLbl.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwrodLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.passwrodLbl.Location = new System.Drawing.Point(40, 129);
            this.passwrodLbl.Name = "passwrodLbl";
            this.passwrodLbl.Size = new System.Drawing.Size(70, 16);
            this.passwrodLbl.TabIndex = 1003;
            this.passwrodLbl.Text = "Password";
            this.passwrodLbl.Click += new System.EventHandler(this.suggestionLabel_MouseLeftButtonUp);
            // 
            // passwordTxt
            // 
            this.passwordTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordTxt.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordTxt.ForeColor = System.Drawing.Color.Black;
            this.passwordTxt.Location = new System.Drawing.Point(35, 124);
            this.passwordTxt.Name = "passwordTxt";
            this.passwordTxt.PasswordChar = '*';
            this.passwordTxt.Size = new System.Drawing.Size(272, 27);
            this.passwordTxt.TabIndex = 2;
            this.passwordTxt.UseSystemPasswordChar = true;
            this.passwordTxt.Enter += new System.EventHandler(this.TxtInEditMode);
            this.passwordTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.passwordTxt.Leave += new System.EventHandler(this.TxtNotEditMode);
            // 
            // signupBtn
            // 
            this.signupBtn.AutoSize = true;
            this.signupBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.signupBtn.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.signupBtn.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(145)))), ((int)(((byte)(27)))));
            this.signupBtn.Location = new System.Drawing.Point(262, 294);
            this.signupBtn.Name = "signupBtn";
            this.signupBtn.Size = new System.Drawing.Size(57, 16);
            this.signupBtn.TabIndex = 5;
            this.signupBtn.TabStop = true;
            this.signupBtn.Text = "Signup!";
            this.signupBtn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.signupBtn_LinkClicked);
            // 
            // signupLbl
            // 
            this.signupLbl.AutoSize = true;
            this.signupLbl.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.signupLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.signupLbl.Location = new System.Drawing.Point(23, 294);
            this.signupLbl.Name = "signupLbl";
            this.signupLbl.Size = new System.Drawing.Size(237, 16);
            this.signupLbl.TabIndex = 1001;
            this.signupLbl.Text = "Don\'t have already an account?";
            this.signupLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // oneAllbrowser
            // 
            this.oneAllbrowser.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.oneAllbrowser.Location = new System.Drawing.Point(25, 315);
            this.oneAllbrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.oneAllbrowser.Name = "oneAllbrowser";
            this.oneAllbrowser.ScrollBarsEnabled = false;
            this.oneAllbrowser.Size = new System.Drawing.Size(298, 155);
            this.oneAllbrowser.TabIndex = 6;
            this.oneAllbrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.oneAllbrowser_DocumentCompleted);
            this.oneAllbrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.oneAllbrowser_Navigating);
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(344, 475);
            this.Controls.Add(this.oneAllbrowser);
            this.Controls.Add(this.signupBtn);
            this.Controls.Add(this.signupLbl);
            this.Controls.Add(this.passwrodLbl);
            this.Controls.Add(this.passwordTxt);
            this.Controls.Add(this.usernameLbl);
            this.Controls.Add(this.errorTxt);
            this.Controls.Add(this.LoginBtn);
            this.Controls.Add(this.retrievePwdBtn);
            this.Controls.Add(this.retrievePwdLbl);
            this.Controls.Add(this.S2cLogo);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.usernameTxt);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmLogin";
            this.Text = "Login into Snip2Code";
            this.Activated += new System.EventHandler(this.frmLogin_Load);
            this.Load += new System.EventHandler(this.frmLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.S2cLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox S2cLogo;
        private System.Windows.Forms.Label retrievePwdLbl;
        private System.Windows.Forms.LinkLabel retrievePwdBtn;
        private System.Windows.Forms.Label usernameLbl;
        private System.Windows.Forms.TextBox usernameTxt;
        private System.Windows.Forms.Button LoginBtn;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox errorTxt;
        private System.Windows.Forms.Label passwrodLbl;
        private System.Windows.Forms.TextBox passwordTxt;
        private System.Windows.Forms.LinkLabel signupBtn;
        private System.Windows.Forms.Label signupLbl;
        private System.Windows.Forms.WebBrowser oneAllbrowser;





    }
}