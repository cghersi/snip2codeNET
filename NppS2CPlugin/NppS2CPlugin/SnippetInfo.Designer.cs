//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
namespace NppPluginNet
{
    partial class SnippetInfo
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
            this.creatorImageViewer = new System.Windows.Forms.PictureBox();
            this.Creator = new System.Windows.Forms.Label();
            this.SnippetName = new System.Windows.Forms.Label();
            this.SnippetDescription = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.creatorImageViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // creatorImageViewer
            // 
            this.creatorImageViewer.Location = new System.Drawing.Point(6, 7);
            this.creatorImageViewer.Margin = new System.Windows.Forms.Padding(2, 2, 6, 2);
            this.creatorImageViewer.Name = "creatorImageViewer";
            this.creatorImageViewer.Size = new System.Drawing.Size(50, 50);
            this.creatorImageViewer.TabIndex = 0;
            this.creatorImageViewer.TabStop = false;
            this.creatorImageViewer.MouseEnter += new System.EventHandler(this.SnippetInfo_MouseEnter);
            this.creatorImageViewer.MouseLeave += new System.EventHandler(this.SnippetInfo_MouseLeave);
            this.creatorImageViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SnippetInfo_MouseUp);
            // 
            // Creator
            // 
            this.Creator.AutoSize = true;
            this.Creator.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Creator.Location = new System.Drawing.Point(62, 2);
            this.Creator.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.Creator.Name = "Creator";
            this.Creator.Size = new System.Drawing.Size(0, 12);
            this.Creator.TabIndex = 1;
            this.Creator.MouseEnter += new System.EventHandler(this.SnippetInfo_MouseEnter);
            this.Creator.MouseLeave += new System.EventHandler(this.SnippetInfo_MouseLeave);
            this.Creator.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SnippetInfo_MouseUp);
            // 
            // SnippetName
            // 
            this.SnippetName.AutoSize = true;
            this.SnippetName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SnippetName.Location = new System.Drawing.Point(61, 16);
            this.SnippetName.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.SnippetName.Name = "SnippetName";
            this.SnippetName.Size = new System.Drawing.Size(0, 13);
            this.SnippetName.TabIndex = 2;
            this.SnippetName.MouseEnter += new System.EventHandler(this.SnippetInfo_MouseEnter);
            this.SnippetName.MouseLeave += new System.EventHandler(this.SnippetInfo_MouseLeave);
            this.SnippetName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SnippetInfo_MouseUp);
            // 
            // SnippetDescription
            // 
            this.SnippetDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SnippetDescription.BackColor = System.Drawing.Color.White;
            this.SnippetDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SnippetDescription.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SnippetDescription.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SnippetDescription.Location = new System.Drawing.Point(62, 31);
            this.SnippetDescription.Multiline = true;
            this.SnippetDescription.Name = "SnippetDescription";
            this.SnippetDescription.ReadOnly = true;
            this.SnippetDescription.Size = new System.Drawing.Size(335, 34);
            this.SnippetDescription.TabIndex = 3;
            this.SnippetDescription.MouseEnter += new System.EventHandler(this.SnippetInfo_MouseEnter);
            this.SnippetDescription.MouseLeave += new System.EventHandler(this.SnippetInfo_MouseLeave);
            this.SnippetDescription.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SnippetInfo_MouseUp);
            // 
            // SnippetInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.SnippetDescription);
            this.Controls.Add(this.SnippetName);
            this.Controls.Add(this.Creator);
            this.Controls.Add(this.creatorImageViewer);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.Name = "SnippetInfo";
            this.Size = new System.Drawing.Size(400, 65);
            this.MouseEnter += new System.EventHandler(this.SnippetInfo_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.SnippetInfo_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SnippetInfo_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.creatorImageViewer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox creatorImageViewer;
        private System.Windows.Forms.Label Creator;
        private System.Windows.Forms.Label SnippetName;
        private System.Windows.Forms.TextBox SnippetDescription;
    }
}
