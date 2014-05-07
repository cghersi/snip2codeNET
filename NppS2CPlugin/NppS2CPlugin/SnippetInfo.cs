//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Snip2Code.Utils;

using System.IO;
using System.Globalization;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Drawing;

namespace NppPluginNet
{
    public partial class SnippetInfo : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SnippetInfo()
        {
            InitializeComponent();
            ManageMouseEnterLeaveEvents = true;
            ManageMouseClickEvents = false;
        }

        public SnippetInfo(string snippetName, string snippetDescription, string creator, System.Drawing.Image creatorThumbnail, long id = 0)
        {
            InitializeComponent();
            ManageMouseEnterLeaveEvents = true;
            ManageMouseClickEvents = false;

            SnipName = snippetName;
            SnipDescription = snippetDescription;
            CreatorName = creator;
            CreatorThumbnail = creatorThumbnail;
            SnippetId = id;
        }

        public string SnipName
        {
            get { return m_snipName; }
            set 
            { 
                m_snipName = (value != null) ? value.GetFirstLines(1) : string.Empty;
                SnippetName.Text = m_snipName;
            }
        }
        public string SnipDescription
        {
            get { return m_snipDescription; }
            set 
            { 
                m_snipDescription = (value != null) ? value.GetFirstLines(2) : string.Empty;
                SnippetDescription.Text = m_snipDescription;
            }
        }
        public string CreatorName 
        {
            get { return m_creatorName; }
            set
            {
                m_creatorName = value;
                Creator.Text = m_creatorName;
            }
        }
        public System.Drawing.Image CreatorThumbnail
        {
            get { return m_creatorThumbnail; }
            set
            {
                m_creatorThumbnail = value;
                LoadCreatorPicture();
            }
        }
        public long SnippetId { get; set; }

        public bool ManageMouseEnterLeaveEvents { get; set; }
        public bool ManageMouseClickEvents { get; set; }

        private bool m_selected = false;
        public bool Selected
        {
            get { return m_selected; }
            set
            {
                lock (this)
                {
                    if (ManageMouseClickEvents)
                    {
                        m_selected = value;
                        UpdateItemBackground();
                    }
                }
            }
        }

        private string m_snipName;
        private string m_snipDescription;
        private string m_creatorName;
        private System.Drawing.Image m_creatorThumbnail;



        private void SnippetInfo_MouseEnter(object sender, EventArgs e)
        {
            if (ManageMouseEnterLeaveEvents && PointerIsInsideControl())
            {
                // change background
                BackColor = Utilities.s_orangeS2C;
                Creator.BackColor = Utilities.s_orangeS2C;
                SnippetName.BackColor = Utilities.s_orangeS2C;
                SnippetDescription.BackColor = Utilities.s_orangeS2C;
            }
        }

        private bool PointerIsInsideControl()
        {
            return ClientRectangle.Contains(PointToClient(Control.MousePosition));
        }

        private void SnippetInfo_MouseLeave(object sender, EventArgs e)
        {
            if (ManageMouseEnterLeaveEvents && !PointerIsInsideControl())
            {
                // reset background to white
                BackColor = Color.White;
                Creator.BackColor = Color.White;
                SnippetName.BackColor = Color.White;
                SnippetDescription.BackColor = Color.White;
            }
        }



        private void LoadCreatorPicture()
        {
            if (CreatorThumbnail != null)
            {
                creatorImageViewer.Image = CreatorThumbnail;
            }
        }

        private void SnippetInfo_MouseUp(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                if (ManageMouseClickEvents)
                {
                    m_selected = !m_selected;
                    UpdateItemBackground();
                }
            }

            frmAddSnippet window = ClientUtils.PrepareViewSnippetForm(PluginBase.Current, SnippetId) as frmAddSnippet;

#if DOCKED
            PluginBase.OpenForm(window, PluginBase.VIEWSNIP_MENU_LABEL, PluginBase.ID_ADD_SNIPPET_CMD);
#else
            window.Show();
#endif
        }


        private void UpdateItemBackground()
        {
            if (m_selected)
            {
                // change background
                BackColor = Utilities.s_orangeS2C;
                Creator.BackColor = Utilities.s_orangeS2C;
                SnippetName.BackColor = Utilities.s_orangeS2C;
                SnippetDescription.BackColor = Utilities.s_orangeS2C;
            }
            else
            {
                // reset background to white
                BackColor = Color.White;
                Creator.BackColor = Color.White;
                SnippetName.BackColor = Color.White;
                SnippetDescription.BackColor = Color.White;
            }
        }
    }
}
