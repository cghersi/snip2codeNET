//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using Snip2Code.Model.Client.Entities;
using Snip2Code.Model.Client.WSFramework;
using Snip2Code.Model.Entities;
using Snip2Code.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NppPluginNet
{
    partial class frmPublish : Form, IPublishSnippetForm
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private BackgroundWorker bgw = null;
        private BackgroundWorker pubishw = null;

        private bool publishClicked = false; // semaphore for publish button
        private List<int> pubChList = new List<int>();
        private ICollection<int> selectedChList = new List<int>();
        private static GroupComparerByName channelSorter = new GroupComparerByName();

        public long SnippetId { get; set; }

        public frmPublish()
        {
            InitializeComponent();
            ResetErrors();
            ResetResults();
        }

        public void ResetResults()
        {
            // cleans up the list view
            listChannelWiew.Controls.Clear();
            pubChList = new List<int>();
        }

        public void DisplayCleanForm(long snippetId, int targetGroupId)
        {
            SnippetId = snippetId;

            if (targetGroupId == 0)   // 0=everyone
                TooltipTxt.Text = Properties.Resources.PublishSnippetTooltipSimple;
            else
                TooltipTxt.Text = Properties.Resources.PublishSnippetTooltipFull;

            PopulateChannels();
        }

        public void ResetErrors()
        {
            ErrorTxt.Text = string.Empty;
            ErrorTxt.Visible = false;
        }

        public void PopulateChannels()
        {
            publishClicked = true;
            ResetErrors();
            ResetResults();

            progressBar.Visible = true;

            bgw = new BackgroundWorker();
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            bgw.WorkerReportsProgress = true;
            int? userId = (BaseWS.CurrentUser != null) ? (int?)BaseWS.CurrentUser.ID : null;
            if (!bgw.IsBusy)
                bgw.RunWorkerAsync(userId);
        }

        protected void PublishBtn_Click(object sender, EventArgs e)
        {
            if (publishClicked)
                return;

            publishClicked = true;

            ResetErrors();
            progressBar.Visible = true;
            foreach (Control si in listChannelWiew.Controls)
            {
                ChannelInfo channel = si as ChannelInfo;
                if ((channel != null) && channel.Selected)
                    pubChList.Add(channel.ChannelId); 
            }

            pubishw = new BackgroundWorker();
            pubishw.DoWork += new DoWorkEventHandler(publishw_DoWork);
            pubishw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(publishw_RunWorkerCompleted);
            pubishw.WorkerReportsProgress = true;
            if (!pubishw.IsBusy)
                pubishw.RunWorkerAsync(pubChList);
        }


        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string errorMsg = e.Result as string;
            progressBar.Visible = false;

            if (string.IsNullOrEmpty(errorMsg))
            {
                ErrorTxt.Text = string.Empty;
                ErrorTxt.Visible = false;
            }
            else
            {
                ErrorTxt.Text = errorMsg;
                ErrorTxt.Visible = true;
            }
            publishClicked = false;
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = string.Empty;

            int? userId = e.Argument as int?;

            if (userId != null)
            {
                // read channels
                UserWS userRepo = new UserWS();
                List<Group> chList = userRepo.GetChannelsOfUser((int)userId) as List<Group>;
                if (chList != null)
                    chList.Sort(channelSorter);

                SnippetsWS snipRepo = new SnippetsWS();
                lock (selectedChList)   // we don't want that someone is updating this while we read it and vice-versa
                {
                    selectedChList = snipRepo.GetChannels(this.SnippetId);

                    if (chList != null)
                    {
                        // populate the control
                        foreach (Group c in chList)
                        {
                            // if the snippet is already published on the channel, mark it as selected
                            bool sel = false;
                            if (selectedChList != null)
                            {
                                foreach (int cs in selectedChList)
                                {
                                    if (c.ID == cs)
                                    {
                                        sel = true;
                                        break;
                                    }
                                }
                            }

                            // populate the control
                            listChannelWiew.Invoke(new Action(() => addItemToList(c.Name, c.ID, sel)));
                        }
                    }
                }
                if ((chList == null) || (chList.Count == 0))
                    e.Result = Properties.Resources.PublishNoChannelFollowed;
            }
            else
            {
                e.Result = Properties.Resources.ErrorCodes_UNKNOWN;
            }
        }

        private void publishw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string errorMsg = e.Result as string;
            progressBar.Visible = false;

            if (string.IsNullOrEmpty(errorMsg))
            {
                ErrorTxt.Text = string.Empty;
                ErrorTxt.Visible = false;

                // Close the window
                PluginBase.CloseForm(this);
            }
            else
            {
                ErrorTxt.Text = errorMsg;
                ErrorTxt.Visible = true;
            }
            publishClicked = false;
        }

        private void publishw_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = string.Empty;

            List<int> pubChList = e.Argument as List<int>;

            // build the list of items to handle
            List<int> unpubList = new List<int>();
            List<int> newPubList = new List<int>();
            lock (selectedChList)
            {
                if ((selectedChList != null) && (pubChList != null))
                {
                    foreach (int p in pubChList)
                    {
                        if (!selectedChList.Contains(p))
                            newPubList.Add(p);
                    }

                    foreach (int p in selectedChList)
                    {
                        if (!pubChList.Contains(p))
                            unpubList.Add(p);
                    }
                }
            }

            bool res = true;

            // UI error handling
            if ((unpubList.Count == 0) && ((pubChList == null) || (pubChList.Count == 0)))
            {
                if (pubChList == null)
                    e.Result = Properties.Resources.ErrorCodes_UNKNOWN;
                else
                    e.Result = Properties.Resources.NoItemsSelected;
            }

            // remove from publishing the unnecessary items
            if (unpubList.Count > 0)
            {
                SnippetsWS snipPrepo = new SnippetsWS();
                res &= snipPrepo.UnPublishSnippet(SnippetId, unpubList);
            }

            // publish the new items
            if (newPubList.Count > 0)
            {
                SnippetsWS snipPrepo = new SnippetsWS();
                res &= snipPrepo.PublishSnippet(SnippetId, newPubList);
            }

            if (!res)
                e.Result = Properties.Resources.PublishErrorUnpublishing;

        }


        private void addItemToList(string name, int chId, bool selected)
        {
            ChannelInfo resItem = new ChannelInfo(name, Properties.Resources.channel, chId);
            resItem.ManageMouseEnterLeaveEvents = false;
            resItem.ManageMouseClickEvents = true;
         
            listChannelWiew.Controls.Add(resItem);
            if (selected)
                selectedChList.Add(chId);
            resItem.Selected = selected;
        }
    }
}
