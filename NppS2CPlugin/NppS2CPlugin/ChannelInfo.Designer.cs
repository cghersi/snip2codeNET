//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
namespace NppPluginNet
{
    partial class ChannelInfo
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.channelImageViewer = new System.Windows.Forms.PictureBox();
            this.ChanName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.channelImageViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // channelImageViewer
            // 
            this.channelImageViewer.Location = new System.Drawing.Point(6, 7);
            this.channelImageViewer.Margin = new System.Windows.Forms.Padding(2, 2, 6, 2);
            this.channelImageViewer.Name = "channelImageViewer";
            this.channelImageViewer.Size = new System.Drawing.Size(25, 25);
            this.channelImageViewer.TabIndex = 0;
            this.channelImageViewer.TabStop = false;
            this.channelImageViewer.MouseEnter += new System.EventHandler(this.ChannelInfo_MouseEnter);
            this.channelImageViewer.MouseLeave += new System.EventHandler(this.ChannelInfo_MouseLeave);
            this.channelImageViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChannelInfo_MouseUp);
            // 
            // ChanName
            // 
            this.ChanName.AutoSize = true;
            this.ChanName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChanName.Location = new System.Drawing.Point(40, 13);
            this.ChanName.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.ChanName.Name = "ChanName";
            this.ChanName.Size = new System.Drawing.Size(0, 13);
            this.ChanName.TabIndex = 2;
            this.ChanName.MouseEnter += new System.EventHandler(this.ChannelInfo_MouseEnter);
            this.ChanName.MouseLeave += new System.EventHandler(this.ChannelInfo_MouseLeave);
            this.ChanName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChannelInfo_MouseUp);
            // 
            // ChannelInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.ChanName);
            this.Controls.Add(this.channelImageViewer);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.Name = "ChannelInfo";
            this.Size = new System.Drawing.Size(400, 40);
            this.MouseEnter += new System.EventHandler(this.ChannelInfo_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ChannelInfo_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChannelInfo_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.channelImageViewer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox channelImageViewer;
        private System.Windows.Forms.Label ChanName;
    }
}
