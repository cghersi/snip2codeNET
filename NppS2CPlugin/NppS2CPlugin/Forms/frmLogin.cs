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
    partial class frmLogin : Form, ILoginForm
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // used by background login thread to communicate with the UI
        private BackgroundWorker bgw = null;

        // semaphore on login btn
        private bool LoginClicked = false;

        public frmLogin()
        {
            InitializeComponent();

            // populate username / pwd
            if (UserPlugin.Current.RetrieveUserPreferences())
            {
                TxtInEditMode(usernameTxt, new EventArgs());
                TxtInEditMode(passwordTxt, new EventArgs());
                usernameTxt.Text = BaseWS.Username;
                passwordTxt.Text = BaseWS.Password;
                TxtNotEditMode(usernameTxt, new EventArgs());
                TxtNotEditMode(passwordTxt, new EventArgs());
            }

            // launche one-all browser
            ResetAccessTokenAndOneAllPage();

            // Simulate ENTER clicks "Login"
            KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    LoginBtn_Click(LoginBtn, new EventArgs());
                    break;
                case Keys.Tab:
                    //override of tab function 'cause inside notepad++ it doesn't work in the regular way!
                    Control ctr = sender as Control;
                    if (ctr != null)
                    {
                        switch (ctr.Name)
                        {
                            case "usernameTxt":
                                passwordTxt.Focus();
                                break;
                            case "passwordTxt":
                                LoginBtn.Focus();
                                break;
                        }
                    }
                    break;
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            LoginBtn.Focus();
        }

        private void retrievePwdBtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClientUtils.OpenRetrievePwdPage();
        }

        private void signupBtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClientUtils.OpenSignupPage();
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            if (LoginClicked)
                return; // multiple click, exit!!

            if (string.IsNullOrWhiteSpace(usernameTxt.Text) || string.IsNullOrWhiteSpace(passwordTxt.Text))
            {
                errorTxt.Visible = true;
                errorTxt.Text = Properties.Resources.IncompleteInput;
                return;
            }

            LoginClicked = true;

            errorTxt.Visible = false;
            errorTxt.Text = string.Empty;
            progressBar.Visible = true;
            KeyValuePair<string, string> credentials = new KeyValuePair<string, string>(usernameTxt.Text.Trim(), passwordTxt.Text);

            bgw = new BackgroundWorker();
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            bgw.WorkerReportsProgress = true;
            if (!bgw.IsBusy)
                bgw.RunWorkerAsync(credentials);
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string errorMsg = e.Result as string;
            progressBar.Visible = false;

            if (string.IsNullOrEmpty(errorMsg))
            {
                errorTxt.Visible = false;
                errorTxt.Text = string.Empty;

                // change the visibility of buttons of top menu and right-click
                PluginBase.Current.DisplayLoggedInButtons();

                // Close the window
                LoginClicked = false;
                PluginBase.CloseForm(this);
            }
            else
            {
                errorTxt.Text = errorMsg;
                errorTxt.Visible = true;
                LoginClicked = false;
            }
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = string.Empty;

            KeyValuePair<string, string> credentials = (KeyValuePair<string, string>)e.Argument;
            string username = credentials.Key;
            string password = credentials.Value;

            ErrorCodes error = ClientUtils.TryLogin(UserPlugin.Current, username, password, PluginBase.Current);
            if (error != ErrorCodes.OK)
                LoginBtn.Invoke(new Action(() => e.Result = CheckResult(error)));
        }


        private static string CheckResult(ErrorCodes lastErrorCode)
        {
            switch (lastErrorCode)
            {
                case ErrorCodes.NOT_LOGGED_IN:
                case ErrorCodes.FAIL:
                    // Login failed: display error
                    return Properties.Resources.LoginError;
                default:
                    string errorRes = string.Empty;
                    try
                    {
                        errorRes = Utilities.GetResourceLocalizedMsg<ErrorCodes>(lastErrorCode);
                    }
                    catch (Exception)
                    {
                        errorRes = null;
                    }
                    if (string.IsNullOrEmpty(errorRes))
                        errorRes = Properties.Resources.ErrorCodes_UNKNOWN;
                    return errorRes;
            }
        }

        /// <summary>
        /// Hide username / password label suggestion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtInEditMode(object sender, EventArgs e)
        {
            TextBox t1 = sender as TextBox;

            if (t1 != null)
            {
                if (t1.Name.Equals("usernameTxt", StringComparison.InvariantCultureIgnoreCase))
                    usernameLbl.Visible = false;
                else if (t1.Name.Equals("passwordTxt", StringComparison.InvariantCultureIgnoreCase))
                    passwrodLbl.Visible = false;
            }
        }


        /// <summary>
        /// Show username / password label suggestion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtNotEditMode(object sender, EventArgs e)
        {
            TextBox t1 = sender as TextBox;

            if (t1 != null)
            {
                if (t1.Name.Equals("usernameTxt", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(t1.Text))
                    {
                        usernameLbl.Visible = true;
                        t1.Text = string.Empty;
                    }
                }
                else if (t1.Name.Equals("passwordTxt", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(t1.Text))
                    {
                        passwrodLbl.Visible = true;
                        t1.Text = string.Empty;
                    }
                }
            }
        }

        private void suggestionLabel_MouseLeftButtonUp(object sender, EventArgs e)
        {
            Label l = sender as Label;

            if (l == null)
                return;

            switch (l.Name)
            {
                case "usernameLbl":
                    usernameTxt.Focus();
                    break;
                case "passwrodLbl":
                    passwordTxt.Focus();
                    break;
            }
        }

        private void bgw_DoWorkForOneAll(object sender, DoWorkEventArgs e)
        {
            e.Result = string.Empty;

            ErrorCodes error = ClientUtils.TryLoginOneAll(UserPlugin.Current, PluginBase.Current);
            if (error != ErrorCodes.OK)
                LoginBtn.Invoke(new Action(() => e.Result = CheckResult(error)));
        }

        private void oneAllbrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (oneAllbrowser.Document == null)
                return;
            Uri currentUri = oneAllbrowser.Document.Url;
            if ((currentUri != null) && (currentUri.AbsoluteUri.Contains("provider_connection_token")))
            {
                bgw = new BackgroundWorker();
                bgw.DoWork += new DoWorkEventHandler(bgw_DoWorkForOneAll);
                bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
                bgw.WorkerReportsProgress = true;
                if (!bgw.IsBusy)
                    bgw.RunWorkerAsync();
            }
        }


        private void ResetAccessTokenAndOneAllPage()
        {
            string oneAllUrl = ClientUtils.GenerateOneallIdentToken();
            oneAllbrowser.Navigate(new Uri(oneAllUrl));
        }

        /// <summary>
        /// Prevent the scrollbars to be displayed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void oneAllbrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string script = "document.body.style.overflow ='hidden'";
            WebBrowser wb = (WebBrowser)sender;
            wb.Document.InvokeScript("execScript", new Object[] { script, "JavaScript" });
            //wb.InvokeScript("execScript", new Object[] { script, "JavaScript" });
        }



        #region ILoginForm implementation
        /////////////////////////////////////////////////////////////////////////////////

        public void ResetFields()
        {
            usernameTxt.Text = string.Empty;
            usernameLbl.Visible = true;
            passwordTxt.Text = string.Empty;
            passwrodLbl.Visible = true;
        }

        public void DisplayCleanForm()
        {
            ResetAccessTokenAndOneAllPage();
            errorTxt.Text = string.Empty;
            errorTxt.Visible = false;
            usernameTxt.Text = string.Empty;
            passwordTxt.Text = string.Empty;
        }

        public void InitialFocus()
        {
            LoginBtn.Focus();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////
    }
}
