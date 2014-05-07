//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
namespace NppPluginNet
{
    partial class frmAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbout));
            this.S2cLogo = new System.Windows.Forms.PictureBox();
            this.VersionLbl = new System.Windows.Forms.Label();
            this.SiteLinkBtn = new System.Windows.Forms.LinkLabel();
            this.ContactUsBtn = new System.Windows.Forms.LinkLabel();
            this.ConnLbl = new System.Windows.Forms.Label();
            this.separator = new System.Windows.Forms.Panel();
            this.ServerLbl = new System.Windows.Forms.Label();
            this.ServerTxt = new System.Windows.Forms.TextBox();
            this.ChangeBtn = new System.Windows.Forms.Button();
            this.CopyrightLbl = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.ErrorLbl = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.S2cLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // S2cLogo
            // 
            this.S2cLogo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.S2cLogo.Image = global::NppPluginNet.Properties.Resources.Snip2Code_40x240;
            this.S2cLogo.Location = new System.Drawing.Point(57, 29);
            this.S2cLogo.Name = "S2cLogo";
            this.S2cLogo.Size = new System.Drawing.Size(231, 51);
            this.S2cLogo.TabIndex = 0;
            this.S2cLogo.TabStop = false;
            // 
            // VersionLbl
            // 
            this.VersionLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VersionLbl.AutoSize = true;
            this.VersionLbl.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.VersionLbl.Location = new System.Drawing.Point(109, 99);
            this.VersionLbl.Name = "VersionLbl";
            this.VersionLbl.Size = new System.Drawing.Size(130, 18);
            this.VersionLbl.TabIndex = 1;
            this.VersionLbl.Text = "Version: 1.4.0";
            this.VersionLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SiteLinkBtn
            // 
            this.SiteLinkBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SiteLinkBtn.AutoSize = true;
            this.SiteLinkBtn.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SiteLinkBtn.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(145)))), ((int)(((byte)(27)))));
            this.SiteLinkBtn.Location = new System.Drawing.Point(87, 135);
            this.SiteLinkBtn.Name = "SiteLinkBtn";
            this.SiteLinkBtn.Size = new System.Drawing.Size(174, 18);
            this.SiteLinkBtn.TabIndex = 2;
            this.SiteLinkBtn.TabStop = true;
            this.SiteLinkBtn.Text = "Visit Snip2Code site";
            this.SiteLinkBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SiteLinkBtn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SiteLinkBtn_LinkClicked);
            // 
            // ContactUsBtn
            // 
            this.ContactUsBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ContactUsBtn.AutoSize = true;
            this.ContactUsBtn.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContactUsBtn.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(145)))), ((int)(((byte)(27)))));
            this.ContactUsBtn.Location = new System.Drawing.Point(122, 172);
            this.ContactUsBtn.Name = "ContactUsBtn";
            this.ContactUsBtn.Size = new System.Drawing.Size(104, 18);
            this.ContactUsBtn.TabIndex = 3;
            this.ContactUsBtn.TabStop = true;
            this.ContactUsBtn.Text = "Contact Us!";
            this.ContactUsBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ContactUsBtn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ContactUsBtn_LinkClicked);
            // 
            // ConnLbl
            // 
            this.ConnLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConnLbl.AutoSize = true;
            this.ConnLbl.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.ConnLbl.Location = new System.Drawing.Point(83, 223);
            this.ConnLbl.Name = "ConnLbl";
            this.ConnLbl.Size = new System.Drawing.Size(182, 18);
            this.ConnLbl.TabIndex = 4;
            this.ConnLbl.Text = "Connection Settings";
            this.ConnLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // separator
            // 
            this.separator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.separator.CausesValidation = false;
            this.separator.Location = new System.Drawing.Point(0, 208);
            this.separator.Name = "separator";
            this.separator.Size = new System.Drawing.Size(358, 2);
            this.separator.TabIndex = 5;
            // 
            // ServerLbl
            // 
            this.ServerLbl.AutoSize = true;
            this.ServerLbl.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerLbl.Location = new System.Drawing.Point(4, 257);
            this.ServerLbl.Name = "ServerLbl";
            this.ServerLbl.Size = new System.Drawing.Size(81, 13);
            this.ServerLbl.TabIndex = 6;
            this.ServerLbl.Text = "Server URL: ";
            // 
            // ServerTxt
            // 
            this.ServerTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ServerTxt.Enabled = false;
            this.ServerTxt.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(145)))), ((int)(((byte)(27)))));
            this.ServerTxt.Location = new System.Drawing.Point(83, 251);
            this.ServerTxt.Name = "ServerTxt";
            this.ServerTxt.Size = new System.Drawing.Size(263, 27);
            this.ServerTxt.TabIndex = 7;
            // 
            // ChangeBtn
            // 
            this.ChangeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ChangeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.ChangeBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ChangeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChangeBtn.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChangeBtn.ForeColor = System.Drawing.Color.White;
            this.ChangeBtn.Location = new System.Drawing.Point(251, 285);
            this.ChangeBtn.Name = "ChangeBtn";
            this.ChangeBtn.Size = new System.Drawing.Size(94, 28);
            this.ChangeBtn.TabIndex = 8;
            this.ChangeBtn.Text = "CHANGE";
            this.ChangeBtn.UseVisualStyleBackColor = false;
            this.ChangeBtn.Click += new System.EventHandler(this.ChangeBtn_Click);
            // 
            // CopyrightLbl
            // 
            this.CopyrightLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CopyrightLbl.AutoSize = true;
            this.CopyrightLbl.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CopyrightLbl.Location = new System.Drawing.Point(60, 359);
            this.CopyrightLbl.Name = "CopyrightLbl";
            this.CopyrightLbl.Size = new System.Drawing.Size(298, 16);
            this.CopyrightLbl.TabIndex = 9;
            this.CopyrightLbl.Text = "© 2014 Snip2Code Inc. All Rights Reserved.";
            this.CopyrightLbl.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(15, 319);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(329, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 11;
            this.progressBar.Visible = false;
            // 
            // ErrorLbl
            // 
            this.ErrorLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorLbl.BackColor = System.Drawing.Color.White;
            this.ErrorLbl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ErrorLbl.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrorLbl.ForeColor = System.Drawing.Color.Red;
            this.ErrorLbl.Location = new System.Drawing.Point(16, 322);
            this.ErrorLbl.Name = "ErrorLbl";
            this.ErrorLbl.ReadOnly = true;
            this.ErrorLbl.Size = new System.Drawing.Size(328, 16);
            this.ErrorLbl.TabIndex = 12;
            this.ErrorLbl.Visible = false;
            // 
            // frmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(358, 377);
            this.Controls.Add(this.ErrorLbl);
            this.Controls.Add(this.CopyrightLbl);
            this.Controls.Add(this.ChangeBtn);
            this.Controls.Add(this.ServerTxt);
            this.Controls.Add(this.ServerLbl);
            this.Controls.Add(this.separator);
            this.Controls.Add(this.ConnLbl);
            this.Controls.Add(this.ContactUsBtn);
            this.Controls.Add(this.SiteLinkBtn);
            this.Controls.Add(this.VersionLbl);
            this.Controls.Add(this.S2cLogo);
            this.Controls.Add(this.progressBar);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAbout";
            this.Text = "About Snip2Code";
            ((System.ComponentModel.ISupportInitialize)(this.S2cLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox S2cLogo;
        private System.Windows.Forms.Label VersionLbl;
        private System.Windows.Forms.LinkLabel SiteLinkBtn;
        private System.Windows.Forms.LinkLabel ContactUsBtn;
        private System.Windows.Forms.Label ConnLbl;
        private System.Windows.Forms.Panel separator;
        private System.Windows.Forms.Label ServerLbl;
        private System.Windows.Forms.TextBox ServerTxt;
        private System.Windows.Forms.Button ChangeBtn;
        private System.Windows.Forms.Label CopyrightLbl;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox ErrorLbl;





    }
}