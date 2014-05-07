//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
namespace NppPluginNet
{
    partial class frmPublish
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPublish));
            this.SearchBtn = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.ErrorTxt = new System.Windows.Forms.Label();
            this.ResultsRecap = new System.Windows.Forms.Label();
            this.listChannelWiew = new NppPluginNet.SelectableFlowLayoutPanel();
            this.TooltipTxt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // SearchBtn
            // 
            this.SearchBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchBtn.BackColor = System.Drawing.Color.Transparent;
            this.SearchBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SearchBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SearchBtn.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchBtn.ForeColor = System.Drawing.Color.White;
            this.SearchBtn.Image = global::NppPluginNet.Properties.Resources.publishSnippet;
            this.SearchBtn.Location = new System.Drawing.Point(335, 455);
            this.SearchBtn.Name = "SearchBtn";
            this.SearchBtn.Size = new System.Drawing.Size(107, 31);
            this.SearchBtn.TabIndex = 2;
            this.SearchBtn.UseVisualStyleBackColor = false;
            this.SearchBtn.Click += new System.EventHandler(this.PublishBtn_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(16, 455);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(313, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 11;
            this.progressBar.Visible = false;
            // 
            // ErrorTxt
            // 
            this.ErrorTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorTxt.BackColor = System.Drawing.Color.White;
            this.ErrorTxt.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrorTxt.ForeColor = System.Drawing.Color.Red;
            this.ErrorTxt.Location = new System.Drawing.Point(16, 454);
            this.ErrorTxt.Name = "ErrorTxt";
            this.ErrorTxt.Size = new System.Drawing.Size(313, 32);
            this.ErrorTxt.TabIndex = 12;
            this.ErrorTxt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ErrorTxt.Visible = false;
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
            // listChannelWiew
            // 
            this.listChannelWiew.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listChannelWiew.AutoScroll = true;
            this.listChannelWiew.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listChannelWiew.Location = new System.Drawing.Point(17, 59);
            this.listChannelWiew.Name = "listChannelWiew";
            this.listChannelWiew.Size = new System.Drawing.Size(425, 390);
            this.listChannelWiew.TabIndex = 0;
            this.listChannelWiew.TabStop = true;
            // 
            // TooltipTxt
            // 
            this.TooltipTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TooltipTxt.BackColor = System.Drawing.Color.White;
            this.TooltipTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TooltipTxt.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TooltipTxt.ForeColor = System.Drawing.Color.Black;
            this.TooltipTxt.Location = new System.Drawing.Point(16, 12);
            this.TooltipTxt.Multiline = true;
            this.TooltipTxt.Name = "TooltipTxt";
            this.TooltipTxt.ReadOnly = true;
            this.TooltipTxt.Size = new System.Drawing.Size(426, 41);
            this.TooltipTxt.TabIndex = 16;
            this.TooltipTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // frmPublish
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(456, 490);
            this.Controls.Add(this.TooltipTxt);
            this.Controls.Add(this.listChannelWiew);
            this.Controls.Add(this.ResultsRecap);
            this.Controls.Add(this.ErrorTxt);
            this.Controls.Add(this.SearchBtn);
            this.Controls.Add(this.progressBar);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPublish";
            this.Text = "Search Snippets";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SearchBtn;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label ErrorTxt;
        private System.Windows.Forms.Label ResultsRecap;
        private SelectableFlowLayoutPanel listChannelWiew;
        private System.Windows.Forms.TextBox TooltipTxt;





    }
}