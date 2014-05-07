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
    partial class frmAbout : Form, IAboutForm
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      // used by background login thread to communicate with the UI
        private BackgroundWorker bgw = null;

        // semaphore on update btn
        private bool UpdateClicked = false;

        public frmAbout()
        {
            InitializeComponent();
       
            // read assembly version as per Properties/AssemblyInfo.cs
            string assemblyVersionStr = ClientUtils.GetPluginVersion();

            VersionLbl.Text = String.Format("Version: {0}", assemblyVersionStr);
            ChangeBtn.Text = Properties.Resources.ChangeBtnText.ToUpper();
            ConnLbl.Text = Properties.Resources.ConnectionSettingsText;
            ServerTxt.Text = BaseWS.Server;
            SetError(string.Empty);
        }

        private void SetError(string error)
        {
            ErrorLbl.Text = error;
            ErrorLbl.Visible = !string.IsNullOrEmpty(error);
        }

        private void SiteLinkBtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClientUtils.OpenSiteLink();
        }

        private void ContactUsBtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClientUtils.ContactUsLink();
        }

        private void ChangeBtn_Click(object sender, EventArgs e)
        {
            if (UpdateClicked)
                return; // multiple click, exit!!

            UpdateClicked = true;

            if (ServerTxt.Enabled)
            {
                if (ServerTxt.Text.IsNullOrWhiteSpaceOrEOF())
                {
                    // invalid input!
                    SetError(Properties.Resources.IncompleteInput);
                    UpdateClicked = false;
                    return;
                }

                ServerTxt.Enabled = false;
                ChangeBtn.Text = Properties.Resources.ChangeBtnText.ToUpper();

                // change the server name in memory and in user preference file
                SetError(string.Empty);
                progressBar.Visible = true;
                string serv = ServerTxt.Text.Trim(new char[] { ' ', '/' });
                if (!serv.IsNullOrWhiteSpaceOrEOF())
                {
                    string oldServer = BaseWS.Server;
                    bgw = new BackgroundWorker();
                    bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
                    bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
                    bgw.WorkerReportsProgress = true;
                    if (!bgw.IsBusy)
                        bgw.RunWorkerAsync(serv + "/");
                }
            }
            else
            {
                ServerTxt.Enabled = true;
                ChangeBtn.Text = Properties.Resources.UpdateBtnText.ToUpper();
                UpdateClicked = false;
            }
        }


        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AboutResultReport res = e.Result as AboutResultReport;
            progressBar.Visible = false;

            if (!res.newServer.Equals(res.oldServer, StringComparison.InvariantCultureIgnoreCase)) // we have changed something
            {
                User us = res.currentUser;
                IPluginManager thisPackage = PluginBase.Current;
                ClientUtils.CloseObsoleteFormsFromAbout(thisPackage);   // cleanup and close obsolete objects

                if ((us == null) || (us.ID <= 0))
                {
                    string errorMsg = res.error;
                    if (!errorMsg.IsNullOrWhiteSpaceOrEOF())
                    {
                        SetError(errorMsg); // only display the message 
                    }
                    else
                    {
                        // invalid login on the new server!
                        ClientUtils.ManageInvalidLoginOnNewServerFromAbout(thisPackage, UserPlugin.Current,
                            res.oldServer, res.newServer);

                        // Close the window
                        PluginBase.CloseForm(this);
                    }

                    UpdateClicked = false;
                    return;
                }

                // save new server path
                UserPlugin.Current.SaveUserPreferences();
            }
            UpdateClicked = false;
        }


        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            string newserv = (string)e.Argument;
            AboutResultReport res = ClientUtils.TryChangeServerURL(newserv);
            if (!string.IsNullOrEmpty(res.error))
                res.error = Properties.Resources.ServerUnreacheableOrInvalid;
            e.Result = res;
        }

        private void ServerTxt_LostFocus(object sender, EventArgs e)
        {
            ServerTxt.SelectionStart = 0;
        }
    }
}
