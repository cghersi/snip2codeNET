//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
namespace NppPluginNet
{
    partial class frmAddSnippet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddSnippet));
            this.saveBtn = new System.Windows.Forms.Button();
            this.NameTxt = new System.Windows.Forms.TextBox();
            this.DescriptionTxt = new System.Windows.Forms.TextBox();
            this.copyCodeBtn = new System.Windows.Forms.Button();
            this.VisibleToLabel = new System.Windows.Forms.Label();
            this.TagsTxt = new System.Windows.Forms.TextBox();
            this.s2cLogoImg = new System.Windows.Forms.PictureBox();
            this.spacer = new System.Windows.Forms.Panel();
            this.SnipTxt = new ScintillaNET.Scintilla();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.LoginErr = new System.Windows.Forms.Label();
            this.LoginBtn = new System.Windows.Forms.LinkLabel();
            this.msgTxt = new System.Windows.Forms.TextBox();
            this.publishBtn = new System.Windows.Forms.Button();
            this.snipSuggestion = new System.Windows.Forms.Label();
            this.GoToOriginalBtn = new System.Windows.Forms.LinkLabel();
            this.CreatedByLabel = new System.Windows.Forms.Label();
            this.propertiesPanel = new NppPluginNet.SelectableFlowLayoutPanel();
            this.visibilityDDL = new NppPluginNet.ComboBoxReadOnly();
            ((System.ComponentModel.ISupportInitialize)(this.s2cLogoImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SnipTxt)).BeginInit();
            this.SuspendLayout();
            // 
            // saveBtn
            // 
            this.saveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveBtn.ForeColor = System.Drawing.SystemColors.Window;
            this.saveBtn.Image = global::NppPluginNet.Properties.Resources.saveSnippet;
            this.saveBtn.Location = new System.Drawing.Point(441, 595);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(105, 23);
            this.saveBtn.TabIndex = 6;
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            this.saveBtn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            // 
            // NameTxt
            // 
            this.NameTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NameTxt.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.NameTxt.Location = new System.Drawing.Point(15, 6);
            this.NameTxt.Multiline = true;
            this.NameTxt.Name = "NameTxt";
            this.NameTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.NameTxt.Size = new System.Drawing.Size(531, 39);
            this.NameTxt.TabIndex = 1;
            this.NameTxt.Enter += new System.EventHandler(this.TxtInEditMode);
            this.NameTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            this.NameTxt.Leave += new System.EventHandler(this.TxtNotEditMode);
            // 
            // DescriptionTxt
            // 
            this.DescriptionTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DescriptionTxt.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.DescriptionTxt.Location = new System.Drawing.Point(15, 57);
            this.DescriptionTxt.Multiline = true;
            this.DescriptionTxt.Name = "DescriptionTxt";
            this.DescriptionTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DescriptionTxt.Size = new System.Drawing.Size(531, 59);
            this.DescriptionTxt.TabIndex = 2;
            this.DescriptionTxt.Enter += new System.EventHandler(this.TxtInEditMode);
            this.DescriptionTxt.Leave += new System.EventHandler(this.TxtNotEditMode);
            // 
            // copyCodeBtn
            // 
            this.copyCodeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.copyCodeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.copyCodeBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.copyCodeBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.copyCodeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.copyCodeBtn.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copyCodeBtn.ForeColor = System.Drawing.SystemColors.Window;
            this.copyCodeBtn.Location = new System.Drawing.Point(441, 393);
            this.copyCodeBtn.Name = "copyCodeBtn";
            this.copyCodeBtn.Size = new System.Drawing.Size(105, 26);
            this.copyCodeBtn.TabIndex = 5;
            this.copyCodeBtn.Text = "COPY CODE";
            this.copyCodeBtn.UseVisualStyleBackColor = false;
            this.copyCodeBtn.Visible = false;
            this.copyCodeBtn.Click += new System.EventHandler(this.copyBox_Click);
            // 
            // VisibleToLabel
            // 
            this.VisibleToLabel.AutoSize = true;
            this.VisibleToLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VisibleToLabel.Location = new System.Drawing.Point(12, 448);
            this.VisibleToLabel.Name = "VisibleToLabel";
            this.VisibleToLabel.Size = new System.Drawing.Size(64, 13);
            this.VisibleToLabel.TabIndex = 7;
            this.VisibleToLabel.Text = "Visible to:";
            // 
            // TagsTxt
            // 
            this.TagsTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TagsTxt.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TagsTxt.Location = new System.Drawing.Point(12, 481);
            this.TagsTxt.Multiline = true;
            this.TagsTxt.Name = "TagsTxt";
            this.TagsTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TagsTxt.Size = new System.Drawing.Size(244, 68);
            this.TagsTxt.TabIndex = 5;
            this.TagsTxt.Enter += new System.EventHandler(this.TxtInEditMode);
            this.TagsTxt.Leave += new System.EventHandler(this.TxtNotEditMode);
            // 
            // s2cLogoImg
            // 
            this.s2cLogoImg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.s2cLogoImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.s2cLogoImg.Image = global::NppPluginNet.Properties.Resources.Snip2Code_40x240;
            this.s2cLogoImg.Location = new System.Drawing.Point(0, 590);
            this.s2cLogoImg.Name = "s2cLogoImg";
            this.s2cLogoImg.Size = new System.Drawing.Size(178, 33);
            this.s2cLogoImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.s2cLogoImg.TabIndex = 11;
            this.s2cLogoImg.TabStop = false;
            // 
            // spacer
            // 
            this.spacer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spacer.BackgroundImage = global::NppPluginNet.Properties.Resources.spacerAddSnippet;
            this.spacer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.spacer.Location = new System.Drawing.Point(15, 408);
            this.spacer.Name = "spacer";
            this.spacer.Size = new System.Drawing.Size(531, 30);
            this.spacer.TabIndex = 12;
            // 
            // SnipTxt
            // 
            this.SnipTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SnipTxt.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SnipTxt.Location = new System.Drawing.Point(15, 129);
            this.SnipTxt.Margins.Margin0.Width = 25;
            this.SnipTxt.Margins.Margin1.Width = 5;
            this.SnipTxt.Name = "SnipTxt";
            this.SnipTxt.Size = new System.Drawing.Size(531, 258);
            this.SnipTxt.Styles.BraceBad.FontName = "Verdana\0\0\0\0";
            this.SnipTxt.Styles.BraceLight.FontName = "Verdana\0\0\0\0";
            this.SnipTxt.Styles.CallTip.FontName = "Segoe UI\0\0\0";
            this.SnipTxt.Styles.ControlChar.FontName = "Verdana\0\0\0\0";
            this.SnipTxt.Styles.Default.BackColor = System.Drawing.SystemColors.Window;
            this.SnipTxt.Styles.Default.FontName = "Verdana\0\0\0\0";
            this.SnipTxt.Styles.IndentGuide.FontName = "Verdana\0\0\0\0";
            this.SnipTxt.Styles.LastPredefined.FontName = "Verdana\0\0\0\0";
            this.SnipTxt.Styles.LineNumber.FontName = "Verdana\0\0\0\0";
            this.SnipTxt.Styles.Max.FontName = "Verdana\0\0\0\0";
            this.SnipTxt.TabIndex = 3;
            this.SnipTxt.Enter += new System.EventHandler(this.TxtInEditMode);
            this.SnipTxt.Leave += new System.EventHandler(this.TxtNotEditMode);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(196, 595);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(239, 23);
            this.progressBar.TabIndex = 14;
            this.progressBar.Visible = false;
            // 
            // LoginErr
            // 
            this.LoginErr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LoginErr.AutoSize = true;
            this.LoginErr.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginErr.ForeColor = System.Drawing.Color.Red;
            this.LoginErr.Location = new System.Drawing.Point(12, 598);
            this.LoginErr.Name = "LoginErr";
            this.LoginErr.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LoginErr.Size = new System.Drawing.Size(171, 16);
            this.LoginErr.TabIndex = 15;
            this.LoginErr.Text = "Invalid login detected:";
            this.LoginErr.Visible = false;
            // 
            // LoginBtn
            // 
            this.LoginBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LoginBtn.AutoSize = true;
            this.LoginBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LoginBtn.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginBtn.Location = new System.Drawing.Point(184, 598);
            this.LoginBtn.Name = "LoginBtn";
            this.LoginBtn.Size = new System.Drawing.Size(96, 16);
            this.LoginBtn.TabIndex = 16;
            this.LoginBtn.TabStop = true;
            this.LoginBtn.Text = "Click to Login";
            this.LoginBtn.Visible = false;
            this.LoginBtn.Click += new System.EventHandler(this.LoginBtn_Click);
            // 
            // msgTxt
            // 
            this.msgTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.msgTxt.BackColor = System.Drawing.SystemColors.Window;
            this.msgTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.msgTxt.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.msgTxt.ForeColor = System.Drawing.Color.Red;
            this.msgTxt.Location = new System.Drawing.Point(0, 590);
            this.msgTxt.Multiline = true;
            this.msgTxt.Name = "msgTxt";
            this.msgTxt.ReadOnly = true;
            this.msgTxt.Size = new System.Drawing.Size(435, 33);
            this.msgTxt.TabIndex = 17;
            this.msgTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.msgTxt.Visible = false;
            // 
            // publishBtn
            // 
            this.publishBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.publishBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.publishBtn.ForeColor = System.Drawing.SystemColors.Window;
            this.publishBtn.Image = global::NppPluginNet.Properties.Resources.publish;
            this.publishBtn.Location = new System.Drawing.Point(231, 443);
            this.publishBtn.Name = "publishBtn";
            this.publishBtn.Size = new System.Drawing.Size(25, 25);
            this.publishBtn.TabIndex = 18;
            this.publishBtn.UseVisualStyleBackColor = true;
            this.publishBtn.Click += new System.EventHandler(this.publishBtn_Click);
            // 
            // snipSuggestion
            // 
            this.snipSuggestion.AutoSize = true;
            this.snipSuggestion.BackColor = System.Drawing.Color.White;
            this.snipSuggestion.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.snipSuggestion.Location = new System.Drawing.Point(47, 133);
            this.snipSuggestion.Name = "snipSuggestion";
            this.snipSuggestion.Size = new System.Drawing.Size(268, 18);
            this.snipSuggestion.TabIndex = 19;
            this.snipSuggestion.Text = "Place the snippet in this frame ";
            this.snipSuggestion.Click += new System.EventHandler(this.TxtInEditMode);
            // 
            // GoToOriginalBtn
            // 
            this.GoToOriginalBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GoToOriginalBtn.AutoSize = true;
            this.GoToOriginalBtn.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GoToOriginalBtn.Location = new System.Drawing.Point(9, 570);
            this.GoToOriginalBtn.Name = "GoToOriginalBtn";
            this.GoToOriginalBtn.Size = new System.Drawing.Size(0, 18);
            this.GoToOriginalBtn.TabIndex = 20;
            this.GoToOriginalBtn.Click += new System.EventHandler(this.GoToOriginal_Click);
            // 
            // CreatedByLabel
            // 
            this.CreatedByLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CreatedByLabel.AutoSize = true;
            this.CreatedByLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreatedByLabel.Location = new System.Drawing.Point(9, 557);
            this.CreatedByLabel.Name = "CreatedByLabel";
            this.CreatedByLabel.Size = new System.Drawing.Size(0, 13);
            this.CreatedByLabel.TabIndex = 21;
            // 
            // propertiesPanel
            // 
            this.propertiesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesPanel.AutoScroll = true;
            this.propertiesPanel.Location = new System.Drawing.Point(263, 444);
            this.propertiesPanel.Name = "propertiesPanel";
            this.propertiesPanel.Size = new System.Drawing.Size(283, 124);
            this.propertiesPanel.TabIndex = 7;
            this.propertiesPanel.TabStop = true;
            // 
            // visibilityDDL
            // 
            this.visibilityDDL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.visibilityDDL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.visibilityDDL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.visibilityDDL.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.visibilityDDL.FormattingEnabled = true;
            this.visibilityDDL.Location = new System.Drawing.Point(81, 444);
            this.visibilityDDL.Name = "visibilityDDL";
            this.visibilityDDL.ReadOnly = false;
            this.visibilityDDL.Size = new System.Drawing.Size(144, 22);
            this.visibilityDDL.TabIndex = 4;
            // 
            // frmAddSnippet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(563, 625);
            this.Controls.Add(this.msgTxt);
            this.Controls.Add(this.CreatedByLabel);
            this.Controls.Add(this.GoToOriginalBtn);
            this.Controls.Add(this.snipSuggestion);
            this.Controls.Add(this.publishBtn);
            this.Controls.Add(this.LoginBtn);
            this.Controls.Add(this.LoginErr);
            this.Controls.Add(this.SnipTxt);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.propertiesPanel);
            this.Controls.Add(this.TagsTxt);
            this.Controls.Add(this.visibilityDDL);
            this.Controls.Add(this.VisibleToLabel);
            this.Controls.Add(this.copyCodeBtn);
            this.Controls.Add(this.DescriptionTxt);
            this.Controls.Add(this.NameTxt);
            this.Controls.Add(this.spacer);
            this.Controls.Add(this.s2cLogoImg);
            this.Controls.Add(this.progressBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAddSnippet";
            this.Text = "Add Snippet";
            this.Load += new System.EventHandler(this.frmAddSnippet_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.s2cLogoImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SnipTxt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveBtn;
        internal System.Windows.Forms.TextBox NameTxt;
        internal System.Windows.Forms.TextBox DescriptionTxt;
        private System.Windows.Forms.Button copyCodeBtn;
        private System.Windows.Forms.Label VisibleToLabel;
        private ComboBoxReadOnly visibilityDDL;
        internal System.Windows.Forms.TextBox TagsTxt;
        private SelectableFlowLayoutPanel propertiesPanel;
        private System.Windows.Forms.PictureBox s2cLogoImg;
        private System.Windows.Forms.Panel spacer;
        private ScintillaNET.Scintilla SnipTxt;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label LoginErr;
        private System.Windows.Forms.LinkLabel LoginBtn;
        private System.Windows.Forms.TextBox msgTxt;
        private System.Windows.Forms.Button publishBtn;
        private System.Windows.Forms.Label snipSuggestion;
        private System.Windows.Forms.LinkLabel GoToOriginalBtn;
        private System.Windows.Forms.Label CreatedByLabel;




    }
}