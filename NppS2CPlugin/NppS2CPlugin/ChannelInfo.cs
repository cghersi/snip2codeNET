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
    public partial class ChannelInfo : UserControl
    {
        public ChannelInfo()
        {
            InitializeComponent();
            ManageMouseEnterLeaveEvents = true;
            ManageMouseClickEvents = false;
        }

        public ChannelInfo(string channelName, System.Drawing.Image thumbnail, int id)
        {
            InitializeComponent();
            ManageMouseEnterLeaveEvents = true;
            ManageMouseClickEvents = false;

            ChannelName = channelName;
            Thumbnail = thumbnail;
            ChannelId = id;
        }

        public string ChannelName
        {
            get { return m_channelName; }
            set 
            { 
                m_channelName = (value != null) ? value.GetFirstLines(1) : string.Empty;
                ChanName.Text = m_channelName;
            }
        }
        public System.Drawing.Image Thumbnail
        {
            get { return m_thumbnail; }
            set
            {
                m_thumbnail = value;
                LoadPicture();
            }
        }
        public int ChannelId { get; set; }

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

        private string m_channelName;
        private System.Drawing.Image m_thumbnail;

        private void ChannelInfo_MouseEnter(object sender, EventArgs e)
        {
            if (ManageMouseEnterLeaveEvents)
            {
                // change background
                BackColor = Utilities.s_orangeS2C;
                ChanName.BackColor = Utilities.s_orangeS2C;
            }
        }

        private void ChannelInfo_MouseLeave(object sender, EventArgs e)
        {
            if (ManageMouseEnterLeaveEvents)
            {
                // reset background to white
                BackColor = Color.White;
                ChanName.BackColor = Color.White;
            }
        }



        private void LoadPicture()
        {
            if (Thumbnail != null)
            {
                channelImageViewer.Image = Thumbnail;
            }
        }

        private void ChannelInfo_MouseUp(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                if (ManageMouseClickEvents)
                {
                    m_selected = !m_selected;
                    UpdateItemBackground();
                }
            }
        }


        private void UpdateItemBackground()
        {
            if (m_selected)
            {
                // change background
                BackColor = Utilities.s_orangeS2C;
                ChanName.BackColor = Utilities.s_orangeS2C;
            }
            else
            {
                // reset background to white
                BackColor = Color.White;
                ChanName.BackColor = Color.White;
            }
        }
    }
}
