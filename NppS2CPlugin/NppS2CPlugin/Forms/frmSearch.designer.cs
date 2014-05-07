//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
namespace NppPluginNet
{
    partial class frmSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSearch));
            this.SearchBtn = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.ErrMsg = new System.Windows.Forms.TextBox();
            this.SearchCombobox = new System.Windows.Forms.ComboBox();
            this.ResultsRecap = new System.Windows.Forms.Label();
            this.showMoreBox = new System.Windows.Forms.Button();
            this.SuggestedPanel = new System.Windows.Forms.Panel();
            this.SuggestedStr = new System.Windows.Forms.Label();
            this.listSnippetWiew = new NppPluginNet.SelectableFlowLayoutPanel();
            this.SuspendLayout();
            // 
            // SearchBtn
            // 
            this.SearchBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.SearchBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SearchBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SearchBtn.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchBtn.ForeColor = System.Drawing.Color.White;
            this.SearchBtn.Image = global::NppPluginNet.Properties.Resources.search_btn;
            this.SearchBtn.Location = new System.Drawing.Point(412, 12);
            this.SearchBtn.Name = "SearchBtn";
            this.SearchBtn.Size = new System.Drawing.Size(30, 31);
            this.SearchBtn.TabIndex = 2;
            this.SearchBtn.UseVisualStyleBackColor = false;
            this.SearchBtn.Click += new System.EventHandler(this.SearchBtn_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(16, 455);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(426, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 11;
            this.progressBar.Visible = false;
            // 
            // ErrMsg
            // 
            this.ErrMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrMsg.BackColor = System.Drawing.Color.White;
            this.ErrMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ErrMsg.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrMsg.ForeColor = System.Drawing.Color.Red;
            this.ErrMsg.Location = new System.Drawing.Point(16, 425);
            this.ErrMsg.Multiline = true;
            this.ErrMsg.Name = "ErrMsg";
            this.ErrMsg.ReadOnly = true;
            this.ErrMsg.Size = new System.Drawing.Size(426, 53);
            this.ErrMsg.TabIndex = 12;
            this.ErrMsg.Visible = false;
            // 
            // SearchCombobox
            // 
            this.SearchCombobox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchCombobox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.SearchCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.SearchCombobox.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchCombobox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.SearchCombobox.FormattingEnabled = true;
            this.SearchCombobox.Location = new System.Drawing.Point(16, 14);
            this.SearchCombobox.Name = "SearchCombobox";
            this.SearchCombobox.Size = new System.Drawing.Size(390, 24);
            this.SearchCombobox.TabIndex = 1;
            this.SearchCombobox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchCombobox_KeyDown);
            this.SearchCombobox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SearchCombobox_KeyUp);
            this.SearchCombobox.Leave += new System.EventHandler(this.SearchCombobox_LostFocus);
            // 
            // ResultsRecap
            // 
            this.ResultsRecap.AutoSize = true;
            this.ResultsRecap.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResultsRecap.Location = new System.Drawing.Point(16, 59);
            this.ResultsRecap.Name = "ResultsRecap";
            this.ResultsRecap.Size = new System.Drawing.Size(0, 16);
            this.ResultsRecap.TabIndex = 15;
            // 
            // showMoreBox
            // 
            this.showMoreBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.showMoreBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.showMoreBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.showMoreBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showMoreBox.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showMoreBox.ForeColor = System.Drawing.Color.White;
            this.showMoreBox.Location = new System.Drawing.Point(181, 422);
            this.showMoreBox.Margin = new System.Windows.Forms.Padding(0);
            this.showMoreBox.Name = "showMoreBox";
            this.showMoreBox.Size = new System.Drawing.Size(110, 28);
            this.showMoreBox.TabIndex = 3;
            this.showMoreBox.Text = "SHOW MORE";
            this.showMoreBox.UseVisualStyleBackColor = false;
            this.showMoreBox.Visible = false;
            this.showMoreBox.Click += new System.EventHandler(this.showMore_Click);
            // 
            // SuggestedPanel
            // 
            this.SuggestedPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SuggestedPanel.Location = new System.Drawing.Point(152, 73);
            this.SuggestedPanel.Name = "SuggestedPanel";
            this.SuggestedPanel.Size = new System.Drawing.Size(290, 23);
            this.SuggestedPanel.TabIndex = 19;
            this.SuggestedPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SuggestedStr_MouseUp);
            // 
            // SuggestedStr
            // 
            this.SuggestedStr.AutoSize = true;
            this.SuggestedStr.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SuggestedStr.Location = new System.Drawing.Point(16, 75);
            this.SuggestedStr.Name = "SuggestedStr";
            this.SuggestedStr.Size = new System.Drawing.Size(136, 16);
            this.SuggestedStr.TabIndex = 20;
            this.SuggestedStr.Text = "Search instead for:";
            this.SuggestedStr.Visible = false;
            // 
            // listSnippetWiew
            // 
            this.listSnippetWiew.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listSnippetWiew.AutoScroll = true;
            this.listSnippetWiew.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listSnippetWiew.Location = new System.Drawing.Point(17, 102);
            this.listSnippetWiew.Name = "listSnippetWiew";
            this.listSnippetWiew.Size = new System.Drawing.Size(425, 317);
            this.listSnippetWiew.TabIndex = 0;
            this.listSnippetWiew.TabStop = true;
            this.listSnippetWiew.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listSnippetWiew_MouseDown);
            this.listSnippetWiew.MouseEnter += new System.EventHandler(this.listSnippetWiew_MouseEnter);
            // 
            // frmSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(456, 490);
            this.Controls.Add(this.SuggestedStr);
            this.Controls.Add(this.SuggestedPanel);
            this.Controls.Add(this.showMoreBox);
            this.Controls.Add(this.listSnippetWiew);
            this.Controls.Add(this.ResultsRecap);
            this.Controls.Add(this.SearchCombobox);
            this.Controls.Add(this.ErrMsg);
            this.Controls.Add(this.SearchBtn);
            this.Controls.Add(this.progressBar);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSearch";
            this.Text = "Search Snippets";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SearchCombobox_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SearchBtn;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox ErrMsg;
        private System.Windows.Forms.ComboBox SearchCombobox;
        private System.Windows.Forms.Label ResultsRecap;
        private SelectableFlowLayoutPanel listSnippetWiew;
        private System.Windows.Forms.Button showMoreBox;
        private System.Windows.Forms.Panel SuggestedPanel;
        private System.Windows.Forms.Label SuggestedStr;





    }
}