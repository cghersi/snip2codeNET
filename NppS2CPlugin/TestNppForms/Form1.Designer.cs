namespace TestNppForms
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.snip2CodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSnippetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchSnippetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.snip2CodeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(284, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // snip2CodeToolStripMenuItem
            // 
            this.snip2CodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSnippetToolStripMenuItem,
            this.searchSnippetsToolStripMenuItem,
            this.loginToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.snip2CodeToolStripMenuItem.Name = "snip2CodeToolStripMenuItem";
            this.snip2CodeToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.snip2CodeToolStripMenuItem.Text = "Snip2Code";
            // 
            // addSnippetToolStripMenuItem
            // 
            this.addSnippetToolStripMenuItem.Name = "addSnippetToolStripMenuItem";
            this.addSnippetToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.addSnippetToolStripMenuItem.Text = "Add Snippet";
            this.addSnippetToolStripMenuItem.Click += new System.EventHandler(this.addSnippetToolStripMenuItem_Click);
            // 
            // searchSnippetsToolStripMenuItem
            // 
            this.searchSnippetsToolStripMenuItem.Name = "searchSnippetsToolStripMenuItem";
            this.searchSnippetsToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.searchSnippetsToolStripMenuItem.Text = "Search Snippets";
            this.searchSnippetsToolStripMenuItem.Click += new System.EventHandler(this.searchSnippetsToolStripMenuItem_Click);
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.loginToolStripMenuItem.Text = "Login";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.loginToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem snip2CodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSnippetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchSnippetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

