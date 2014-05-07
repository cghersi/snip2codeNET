using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NppPlugin;
using NppPluginNet;

namespace TestNppForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Demo.Init();
        }

        private void addSnippetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Demo.AddSnippet();
        }

        private void searchSnippetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Demo.SearchSnippets();
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Demo.Login_Logout();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Demo.About();
        }
    }
}
