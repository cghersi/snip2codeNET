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
using System.Text;
using System.Windows.Forms;

namespace NppPluginNet
{
    partial class frmAddSnippet : Form, IManageSnippetForm
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int PROP_NAME_MAX_LENGTH = 50;
        private const int PROP_VALUE_MAX_LENGTH = 50;

        static Bitmap removePropImg = Properties.Resources.removeProperty;

        private Dictionary<string, List<string>> m_userSelectionProperties;

        public string LinkToSource { get; set; }
        public long SnippetId { get; set; }
        public int TargetGroupId { get; set; }

        // used by background add thread to communicate with the UI
        private BackgroundWorker bgw = null;

        private BackgroundWorker loadProperties = null;

        private class SnippetSaveResult
        {
            public SnippetWrongField validity = SnippetWrongField.OK;
            public long id = -1;
            public int targetGroupId = -1;
            public string errorMsg = string.Empty;
        }

        // semaphore on add snippet btn
        private bool AddClicked = false;

        public frmAddSnippet()
        {
            InitializeComponent();
            ResetFields();

            SnipTxt.Text = Properties.Resources.SnippetTextBox;
        }

        public void SetSelectedText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            SnipTxt.Text = EntityUtils.TrimStartingTabsOrSpaces(text);
        }

        private void frmAddSnippet_Load(object sender, EventArgs e)
        {
            SetHighlighting("cs");
            //try
            //{
            //    log.Debug("Setting lang 'cs' in Sync execution from Load");
            //    SnipTxt.ConfigurationManager.Language = "cs";
            //    SnipTxt.ConfigurationManager.Configure();
            //}
            //catch { }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (AddClicked)
                return; // multiple click, exit!!

            AddClicked = true;

            string name = string.Empty;
            string snip = string.Empty;
            string description = string.Empty;
            string tags = string.Empty;

            // Check consistency
            if (string.IsNullOrWhiteSpace(SnipTxt.Text) ||
                SnipTxt.Text.Equals(Properties.Resources.SnippetTextBox))
            {
                SetErrorString("Please insert a valid Snippet!");
                AddClicked = false;
                return;
            }
            else
            {
                snip = EntityUtils.TrimStartingTabsOrSpaces(SnipTxt.Text);
            }
            if (string.IsNullOrWhiteSpace(NameTxt.Text) || (!CheckTxtChanged(NameTxt)))
            {
                SetErrorString("Please insert a valid Title!");
                NameTxt.ForeColor = Color.Red;
                AddClicked = false;
                return;
            }
            else
            {
                name = NameTxt.Text;
            }


            if (DescriptionTxt.Text != Properties.Resources.DescriptionTextBox)
                description = DescriptionTxt.Text;

            if (TagsTxt.Text != Properties.Resources.TagsTextBox)
                tags = TagsTxt.Text;

            // create snippet
            SnippetClient snipp = new SnippetClient();
            snipp.Name = name.Trim();
            snipp.Description = description.Trim();
            snipp.Code = snip;
            snipp.StringTags = tags.Trim();

            // get selected group
            if ((visibilityDDL != null) && (visibilityDDL.SelectedItem != null))
            {
                GroupComboItem sel = visibilityDDL.SelectedItem as GroupComboItem;
                if (sel != null)
                    snipp.TargetGroupID = sel.GroupID;
            }

            // add properties
            List<SnippetProperty> pList = BuildPropertiesList(propertiesPanel.Controls, false);
            foreach (SnippetProperty newProp in pList)
                snipp.AddProperty(newProp);

            // save snippet in a background thread
            ResetErrors();
            progressBar.Visible = true;

            bgw = new BackgroundWorker();
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            bgw.WorkerReportsProgress = true;
            if (!bgw.IsBusy)
                bgw.RunWorkerAsync(snipp);
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            SnippetSaveResult sresult = new SnippetSaveResult();
            sresult.id = -1;
            sresult.validity = SnippetWrongField.OK;
            sresult.targetGroupId = -1;

            SnippetClient mysnippet = e.Argument as SnippetClient;

            if (mysnippet != null)
            {
                // check if snippet is valid
                sresult.validity = mysnippet.AreFieldsValid();
                if (sresult.validity == SnippetWrongField.OK)
                {
                    SnippetsWS s_snippetRepo = new SnippetsWS();

                    // save it
                    sresult.id = s_snippetRepo.SaveSnippet(mysnippet);
                    if (sresult.id <= 0)
                    {
                        try
                        {
                            sresult.errorMsg = Utilities.GetResourceLocalizedMsg<ErrorCodes>(s_snippetRepo.LastErrorCode);
                        }
                        catch (Exception)
                        {
                            sresult.errorMsg = null;
                        }
                        if (string.IsNullOrEmpty(sresult.errorMsg))
                            sresult.errorMsg = Properties.Resources.ErrorCodes_UNKNOWN;
                    }
                    else
                    {
                        sresult.targetGroupId = mysnippet.TargetGroupID;
                    }
                }
            }

            e.Result = sresult;
        }


        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Visible = false;

            SnippetSaveResult sres = (SnippetSaveResult)e.Result;

            if (sres.id > 0)
            {
                // Success: reset the fields and close the current window
                ResetFields();

                // if snippet is set to public display publish mask (only if the channels are available for this user)
                if ((sres.targetGroupId == 0) && (BaseWS.CurrentUser != null) &&
                    !BaseWS.CurrentUser.JoinedChannels.IsNullOrEmpty())
                {
                    PluginBase.PublishSnip(sres.id, sres.targetGroupId);
                }

                PluginBase.CloseForm(this);

                //AddClicked = false; //relase add button
            }
            else
            {
                // snippet was invalid
                if (sres.validity != SnippetWrongField.OK)
                {
                    string resError = string.Empty;
                    try
                    {
                        resError = Utilities.GetResourceLocalizedMsg<SnippetWrongField>(sres.validity);
                    }
                    catch (Exception)
                    {
                        resError = null;
                    }
                    if (string.IsNullOrEmpty(resError))
                        resError = Properties.Resources.SnippetWrongField_default;
                    SetErrorString(resError);
                }
                else
                {
                    // Failure: show error creating snippet
                    SetErrorString(string.IsNullOrEmpty(sres.errorMsg) ?
                                        Properties.Resources.GenericSnippetWSError :
                                        sres.errorMsg);

                    // login error customized
                    if (!WebConnector.Current.IsLogged)
                    {
                        // in this case we don't need to show other messages
                        SetErrorString(string.Empty);
                        log.DebugFormat("bgw_RunWorkerCompleted didn't save snippet; seems logged:{0}", WebConnector.Current.SeemsLogged);
                        LoginErr.Visible = true;
                        LoginBtn.Visible = true;
                        s2cLogoImg.Visible = false;
                    }
                }
            }

            AddClicked = false; //relase add button
        }


        private void bgw_CheckLoginAndConnection(object sender, DoWorkEventArgs e)
        {
            ErrorCodes sresult = ErrorCodes.OK;

            if (!WebConnector.Current.SeemsLogged || (BaseWS.CurrentUser == null))
            {
                if (WebConnector.Current.PingS2CServer())
                    sresult = ErrorCodes.NOT_LOGGED_IN; // probably wrong credentials
                else
                    sresult = ErrorCodes.COMMUNICATION_ERROR;    // network down
            }

            e.Result = sresult;
        }


        private void bgw_CheckLoginAndConnectionCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ErrorCodes sres = (ErrorCodes)e.Result;
            SetErrorString(string.Empty);

            log.DebugFormat("bgw_CheckLoginAndConnectionCompleted returned {0}", sres);
            switch (sres)
            {
                case ErrorCodes.NOT_LOGGED_IN:                    
                    LoginErr.Visible = true;
                    LoginBtn.Visible = true;
                    s2cLogoImg.Visible = false;
                    break;
                case ErrorCodes.OK:
                    // nothing to do
                    break;
                case ErrorCodes.COMMUNICATION_ERROR:
                default:
                    SetErrorString(Properties.Resources.NetworkError);
                    break;
            }
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Return)
            {
                saveBtn.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyData == Keys.Escape)
            {
                try
                {
                    Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_GRABFOCUS, 0, 0);
                }
                catch (Exception exc)
                {
                    if (log != null)
                        log.ErrorFormat("Cannot grab focus due to {0}", exc.Message);
                }
            }
            //else if (e.KeyCode == Keys.Tab)
            //{
            //    Control next = GetNextControl((Control)sender, !e.Shift);
            //    while ((next == null) || (!next.TabStop)) next = GetNextControl(next, !e.Shift);
            //    next.Focus();
            //    e.Handled = true;
            //}
        }

        // Copy all the snippet Text into clipboard
        private void copyBox_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(SnipTxt.Text);
        }

        //---------------------------------------------------------------
        #region PROPERTIES and GROUPS

        /// <summary>
        /// Populate visibility section with groups belonging to current user
        /// <param name="selectedGroupID">Default selected group.
        /// 0 :                                 Everyone
        /// -1 or CurrentUser.PersonalGroupID : Only Me
        /// GroupID :                           a specific group
        /// </param>
        /// <param name="ownerGroupName">Owner group name of current snippet.
        /// Empty if snippet is to be added.
        /// The name of the owner in case we want to display a specific snippet
        /// </param>
        /// </summary>
        public void PopulateGroups(int selectedGroupID, string ownerGroupName = "")
        {
            visibilityDDL.Items.Clear();
            string selectedGroupName = "";

            if (string.IsNullOrWhiteSpace(ownerGroupName))
            {
                // check for login error 
                if (!WebConnector.Current.IsLogged || (BaseWS.CurrentUser == null))
                {
                    bgw = new BackgroundWorker();
                    bgw.DoWork += new DoWorkEventHandler(bgw_CheckLoginAndConnection);
                    bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_CheckLoginAndConnectionCompleted);
                    bgw.WorkerReportsProgress = true;
                    if (!bgw.IsBusy)
                        bgw.RunWorkerAsync();

                    // add only Everyone group (selected), we cannot add other groups
                    GroupComboItem cbeveryone = new GroupComboItem();
                    cbeveryone.IsSelected = true;
                    selectedGroupName = Commons.GroupNames.Public; // "Everyone";
                    cbeveryone.GroupID = 0;
                    cbeveryone.Text = Commons.GroupNames.Public; // "Everyone";
                    AddGroupToVisibilityDLL(cbeveryone);
                }
                else
                {
                    // add OnlyMe group
                    GroupComboItem cbonlyme = new GroupComboItem();
                    if ((selectedGroupID == BaseWS.CurrentUser.PersonalGroupID) || (selectedGroupID < 0))
                    {
                        cbonlyme.IsSelected = true;
                        selectedGroupName = Commons.GroupNames.MyOwn; // "Only Me";
                    }
                    cbonlyme.GroupID = BaseWS.CurrentUser.PersonalGroupID;
                    cbonlyme.Text = Commons.GroupNames.MyOwn; // "Only Me";
                    AddGroupToVisibilityDLL(cbonlyme);

                    ICollection<Snip2Code.Model.Entities.Group> groupsList = BaseWS.CurrentUser.Groups;

                    foreach (Snip2Code.Model.Entities.Group g in groupsList)
                    {
                        if (g.ID == BaseWS.CurrentUser.PersonalGroupID)
                            continue;   // this is already pre-loaded !

                        GroupComboItem cb = new GroupComboItem();
                        if (g.ID == selectedGroupID)
                        {
                            cb.IsSelected = true;
                            selectedGroupName = g.Name;
                        }
                        cb.GroupID = g.ID;
                        cb.Text = g.Name;
                        AddGroupToVisibilityDLL(cb);
                    }

                    // add Everyone group
                    GroupComboItem cbeveryone = new GroupComboItem();
                    if (selectedGroupID == 0)
                    {
                        cbeveryone.IsSelected = true;
                        selectedGroupName = Commons.GroupNames.Public; // "Everyone";
                    }
                    cbeveryone.GroupID = 0;
                    cbeveryone.Text = Commons.GroupNames.Public; // "Everyone";
                    AddGroupToVisibilityDLL(cbeveryone);
                }
            }
            else
            {
                GroupComboItem cb = new GroupComboItem();
                cb.GroupID = 0;
                cb.IsSelected = true;
                cb.Text = ownerGroupName;
                AddGroupToVisibilityDLL(cb);
            }
        }


        public class GroupComboItem
        {
            public string Text { get; set; }
            public int GroupID { get; set; }
            public bool IsSelected { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }


        private void SetHighlighting(string lang)
        {
            if (string.IsNullOrEmpty(lang))
                return;

            try
            {
                if (SnipTxt.InvokeRequired)
                    SnipTxt.Invoke(new Action(() =>
                    {
                        try
                        {
                            log.DebugFormat("Setting lang {0} in ActionInvoked", lang);
                            SnipTxt.ConfigurationManager.Language = lang;
                            SnipTxt.ConfigurationManager.Configure();
                        }
                        catch { }
                    }));
                else
                {
                    log.DebugFormat("Setting lang {0} in Sync execution", lang);
                    SnipTxt.ConfigurationManager.Language = lang;
                    SnipTxt.ConfigurationManager.Configure();
                }
            }
            catch { }
        }

        /// <summary>
        /// Detect properties of the current project and populate extended properties 
        /// area and tags with autodected values.
        /// </summary>
        public void PrepareProperties()
        {
            System.Threading.Thread.Sleep(100); // A small delay to facilitate in memory loading of the class.

            //Dictionary<string, string> detectedProperties = new Dictionary<string, string>();
            List<SnippetProperty> detectedProperties = new List<SnippetProperty>();
            
            // Get edited document properties
            string docName = PluginBase.GetCurrentFilename();
            bool languageSet = false;

            string extension = string.Empty;
            string[] filenameTokens = docName.Split('.');
            if (!filenameTokens.IsNullOrEmpty())
                extension = filenameTokens[filenameTokens.Length - 1];
            try
            {
                if (!string.IsNullOrEmpty(extension))
                {
                    detectedProperties.Add(new SnippetProperty(DefaultProperty.Extension, extension));

                    // Overwrite language when uncorrectly detected.
                    // These languages have been notified by users.
                    if (extension.ToLower().Equals(".vbs"))
                    {
                        detectedProperties.Add(new SnippetProperty(DefaultProperty.Language, "VBscript"));
                        languageSet = true;
                    }
                    else if (extension.ToLower().Equals(".asp"))
                    {
                        detectedProperties.Add(new SnippetProperty(DefaultProperty.Language, "ASP"));
                        languageSet = true;
                    }
                }
            }
            catch { }

            if (!languageSet)   // for some languages we have already set this property using extension
            {
                string docLang = PluginBase.GetCurrentLanguage();
                if (!string.IsNullOrEmpty(docLang))
                {
                    detectedProperties.Add(new SnippetProperty(DefaultProperty.Language, docLang));
                    SetHighlighting(docLang);
                }
            }

            // O.S. properties
            // Determine whether the current operating system is a 64 bit operating system.
            bool f64bitOS = Snip2Code.Utils.Utilities.Is64BitOperatingSystem();
            string architecture = f64bitOS ? "x86_64" : "x86";

            string winVer = Snip2Code.Utils.Utilities.getOSInfo();
            if (!string.IsNullOrEmpty(winVer) && (winVer.ToLower().StartsWith("windows")))
            {
                detectedProperties.Add(new SnippetProperty(DefaultProperty.OS, DefaultProperty.WindowsOS));
                detectedProperties.Add(new SnippetProperty(DefaultProperty.WindowsVersion, winVer));
            }
            detectedProperties.Add(new SnippetProperty(DefaultProperty.Architecture, architecture));

            //populate detected properties
            PopulateProperties(detectedProperties);
        }


        /// <summary>
        /// Populate properties section based on the Dictionary in input
        /// </summary>
        /// <param name="prop"></param>
        public void PopulateProperties(ICollection<SnippetProperty> prop, bool editable = true)
        {
            loadProperties = new BackgroundWorker();
            loadProperties.DoWork += new DoWorkEventHandler(loadProperties_DoWork);
            if (!loadProperties.IsBusy)
                loadProperties.RunWorkerAsync(new LoadPropArgs() { Prop = prop, Editable = editable });
        } 

        public class LoadPropArgs
        {
            public ICollection<SnippetProperty> Prop { get; set; }
            public bool Editable { get; set; }
        }

        private void loadProperties_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadPropArgs args = e.Argument as LoadPropArgs;
            bool networkAvailable = false;
            if (args.Editable)
            {
                // Read suggested values from DB
                PropertiesWS propRepo = new PropertiesWS();
                networkAvailable = WebConnector.Current.PingS2CServer();

                // check if connection is available
                if (networkAvailable)
                {
                    // get default properties from server
                    ICollection<DefaultProperty> defProperties = propRepo.GetDefaultProperties();
                    log.Debug("Found " + defProperties.Count + " properties");
                    IDictionary<string, List<string>> basicProperties = DefaultProperty.FilterBasicProperties(defProperties);
                    log.DebugFormat("basicProperties count: " + basicProperties.Count);
                    m_userSelectionProperties = new Dictionary<string, List<string>>(basicProperties);  // shallow copy!!

                    // fetch dependent properties from DB and add to our list to be shown 
                    // for user selection
                    IDictionary<string, List<string>> dependentProperties =
                                DefaultProperty.FilterDependentProperties(defProperties, args.Prop);
                    foreach (var d in dependentProperties)
                    {
                        if (!m_userSelectionProperties.ContainsKey(d.Key.ToLower()))
                            m_userSelectionProperties.Add(d.Key, d.Value);
                    }
                }
                else
                {
                    log.Debug("Network is down");

                    // network down: we cannot read them from DB let's populate anyway using whta we have detected
                    m_userSelectionProperties = new Dictionary<string, List<string>>();
                }
            }

            // Populate detected properties
            Random rdn = new Random();
            bool highlightingDone = false;

            // clean up the panel from previous execution
            log.Debug("Clean up propertiesPanel");
            if (propertiesPanel.InvokeRequired)
                this.Invoke(new Action(() => propertiesPanel.Controls.Clear()));
            else
                propertiesPanel.Controls.Clear();

            foreach (SnippetProperty p in args.Prop)
            {
                if (p == null)
                    continue;
                string numRdn = rdn.Next().ToString();

                if (args.Editable)
                {
                    CreateComboBox(m_userSelectionProperties.Keys.ToObjArray(), p, false, numRdn, false);
                    ComboBox cbV = CreateComboBox(m_userSelectionProperties.ContainsKey(p.Name) ? 
                                    m_userSelectionProperties[p.Name].ToObjArray() : null,
                                        p, true, numRdn, false) as ComboBox;

                    CreateDelPropImage(cbV.Height, numRdn);
                }
                else
                {
                    CreateComboBox(null, p, false, numRdn, true);
                    CreateComboBox(null, p, true, numRdn, true);
                }


                // set syntax highlighting
                if (!highlightingDone && p.Name.Equals(DefaultProperty.Language, StringComparison.InvariantCultureIgnoreCase))
                {
                    string docLang = p.Value;
                    // rename the detected language name in order to make it understandable by avalon syntax highlighter
                    // See: DB Script.PostDeployment.sql
                    // See: ICSharpCode.AvalonEdit.Highlighting.Resources.cs
                    if (docLang.ToLower().Equals("csharp"))
                        docLang = "C#";
                    else if (docLang.ToLower().Equals("c"))
                        docLang = "C++";
                    else if (docLang.ToLower().Equals("vb") || docLang.ToLower().Equals("vbscript"))
                        docLang = "VBNET";
                    else if (docLang.ToLower().Equals("asp") || docLang.ToLower().Equals("asp.net"))
                        docLang = "ASP/XHTML";
                    else if (docLang.ToLower().Equals("xsl") || docLang.ToLower().Equals("dtd") || docLang.ToLower().Equals("xsd"))
                        docLang = "XML";

                    SetHighlighting(docLang);
                    highlightingDone = true;
                }

            }

            // Populate empty checkbox properties
            if (args.Editable)
            {
                CreateComboBox(m_userSelectionProperties.Keys.ToObjArray(), null, false, rdn.Next().ToString(), false);

                if (!networkAvailable)
                    SetErrorString(Properties.Resources.NetworkError);
            }

            //remove highlighting from properties comboboxes:
            if (args.Editable)
                RemoveHighlightingFromProperties();
        }

        private void RemoveHighlightingFromProperties()
        {
            foreach (Control c in propertiesPanel.Controls)
            {
                if (c is ComboBox)
                {
                    if (c.InvokeRequired)
                        this.Invoke(new Action(() => (c as ComboBox).SelectionLength = 0));
                    else
                        (c as ComboBox).SelectionLength = 0;
                }
            }
        }


        private Control CreateComboBox(object[] items, SnippetProperty p, bool isValue, string numRdn, bool onlyRead)
        {
            Control cbN = null;
            if (onlyRead)
            {
                cbN = new TextBox();
                ((TextBox)cbN).Enabled = false;
            }
            else
            {
                cbN = new ComboBox();
                ((ComboBox)cbN).MaxLength = PROP_NAME_MAX_LENGTH;
                ((ComboBox)cbN).Margin = new Padding(1);
                ((ComboBox)cbN).SelectionChangeCommitted += new EventHandler(ComboBox_SelectedValueChanged);
                ((ComboBox)cbN).LostFocus += new EventHandler(ComboBox_LostFocus);
                ((ComboBox)cbN).KeyUp += new KeyEventHandler(ComboBox_KeyUp);
            }

            if (isValue)
            {
                if (p != null)
                {
                    if (!onlyRead)
                        ((ComboBox)cbN).SelectedValue = p.Value;
                    cbN.Text = p.Value;
                }
                cbN.Name = "Value_" + numRdn;
            }
            else
            {
                if (p != null)
                {
                    if (!onlyRead)
                        ((ComboBox)cbN).SelectedValue = p;
                    cbN.Text = p.Name;
                }
                cbN.Name = "Name_" + numRdn;
            }
            
            if (!onlyRead && (items != null))
                ((ComboBox)cbN).Items.AddRange(items);
            
            cbN.Width = 115;
            cbN.Font = Utilities.s_regularFont;
            cbN.BackColor = Color.White;
           
            if (propertiesPanel.InvokeRequired)
                this.Invoke(new Action(() => propertiesPanel.Controls.Add(cbN)));
            else
                propertiesPanel.Controls.Add(cbN);

            return cbN;
        }


        private void AddGroupToVisibilityDLL(GroupComboItem cb)
        {
            if (visibilityDDL.InvokeRequired)
                this.Invoke(new Action(() => AddGroupToVisibilityDLLAction(cb)));
            else
                AddGroupToVisibilityDLLAction(cb);
        }

        private void AddGroupToVisibilityDLLAction(GroupComboItem cb)
        {
            visibilityDDL.Items.Add(cb);
            if (cb.IsSelected)
                visibilityDDL.SelectedIndex = (visibilityDDL.Items.Count - 1);
        }


        private void CreateDelPropImage(int height, string numRdn)
        {
            PictureBox delProp = new PictureBox();
            delProp.Image = removePropImg;
            delProp.Height = height;
            delProp.Size = new Size(26, 26);
            delProp.Margin = new Padding(1);
            delProp.Name = "Del_" + numRdn;
            delProp.Click += new System.EventHandler(delProp_MouseLeftButtonUp);

            if (propertiesPanel.InvokeRequired)
                this.Invoke(new Action(() => propertiesPanel.Controls.Add(delProp)));
            else
                propertiesPanel.Controls.Add(delProp);
        }

        private void ComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb == null)
                return;
            string currentText = cb.SelectedItem as string;
            ComboBox_NameChangeUpdate(cb, currentText, true, true);
        }

        private void ComboBox_LostFocus(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb == null)
                return;
            if ((cb.Name.StartsWith("Value_")) && (string.IsNullOrEmpty(cb.Text)))
                return;
            ComboBox_NameChangeUpdate(cb, cb.Text, false, false);
        }

        private void ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            // traps Return/Enter sent
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                ComboBox cb = sender as ComboBox;
                if (cb == null)
                    return;
                ComboBox_NameChangeUpdate(cb, cb.Text, true, false);
            }
        }


        private void ComboBox_NameChangeUpdate(ComboBox cb, string currentValue, bool needsUpdate, bool getSelectedValue)
        {
            log.Debug("Init ComboBox_NameChangeUpdate, currentValue=" + currentValue.PrintNull());
            if ((cb == null) || (currentValue == null))
                return;

            // detect if the next combobox has already been created
            int cbNameIdx = propertiesPanel.Controls.IndexOf(cb);
            if (cbNameIdx < 0)  // this should not occur: current combobox not found!
                return;

            //---------
            // DELETE
            if (currentValue.IsNullOrWhiteSpaceOrEOF() && cb.Name.StartsWith("Name_"))
            {
                log.Debug("Delete ComboBox_NameChangeUpdate, cbName=" + cb.Name + ";cbNameIdx=" + cbNameIdx);
                if (cbNameIdx >= propertiesPanel.Controls.Count - 1)
                {
                    // nothing to do: it is already the latest combobox in the panel
                }
                else
                {
                    // delete the value
                    log.Debug("Removing control at " + (cbNameIdx + 1));
                    propertiesPanel.Controls.RemoveAt(cbNameIdx + 1);

                    // delete the "X"
                    if (propertiesPanel.Controls.Count >= cbNameIdx + 2)
                        propertiesPanel.Controls.RemoveAt(cbNameIdx + 1);

                    // if it is not the latest element delete the name itself
                    if (cbNameIdx < propertiesPanel.Controls.Count - 1)
                        propertiesPanel.Controls.RemoveAt(cbNameIdx);
                }
            }
            else
            {
                log.Debug("Modify ComboBox_NameChangeUpdate, cbName=" + cb.Name);
                //---------
                // MODIFY
                if (cbNameIdx == propertiesPanel.Controls.Count - 1)  // current combobox is the latest element in the panel
                {
                    // no combobox found: add a new one
                    if (cb.Name.StartsWith("Name_"))    // current cb is a Name combobox
                    {
                        string rdnNum = GetCustomIndex(cb);
                        ComboBox cbNew = CreateComboBox(m_userSelectionProperties.ContainsKey(currentValue) ?
                            m_userSelectionProperties[currentValue].ToObjArray() : null,
                            null, true, rdnNum, false) as ComboBox;

                        CreateDelPropImage(cbNew.Height, rdnNum);

                        // Populate empty combobox properties
                        CreateComboBox(m_userSelectionProperties.Keys.ToObjArray(), null, false, new Random().Next().ToString(),
                                        false);
                    }
                    else    // current cb is a Value combobox
                    {
                        CreateComboBox(m_userSelectionProperties.Keys.ToObjArray(), null, false, new Random().Next().ToString(),
                                        false);
                    }
                }
                else
                {
                    if (cb.Name.StartsWith("Name_"))
                    {
                        // update combobox with new values
                        ComboBox cbV = propertiesPanel.Controls[cbNameIdx + 1] as ComboBox;
                        if (cbV == null)
                            return;
                        if (m_userSelectionProperties.ContainsKey(currentValue))
                        {
                            cbV.Items.Clear();
                            cbV.Items.AddRange(m_userSelectionProperties[currentValue].ToObjArray());
                        }
                        if (needsUpdate)
                            cbV.Text = "";
                    }
                }
            }

            if (needsUpdate)
            {
                //refresh the item source of the properties combos:
                foreach (Control c in propertiesPanel.Controls)
                {
                    ComboBox combo = c as ComboBox;
                    if (combo == null)
                        continue;
                    if (combo.Name.StartsWith("Name"))
                    {
                        string selItem = string.Empty;
                        if (combo.SelectedItem != null)
                            selItem = combo.SelectedItem.ToString();
                        combo.Items.Clear();
                        combo.Items.AddRange(m_userSelectionProperties.Keys.ToObjArray());
                        if (!string.IsNullOrEmpty(selItem) && !m_userSelectionProperties.ContainsKey(selItem))
                            combo.Items.Add(selItem);
                        combo.SelectedItem = selItem;
                    }
                    else if (combo.Name.StartsWith("Value"))
                    {
                        string selItem = string.Empty;
                        if (combo.SelectedItem != null)
                            selItem = combo.SelectedItem.ToString();

                        //retrieve the property name related to the value of the current combo:
                        int curIdx = propertiesPanel.Controls.IndexOf(combo);
                        if (curIdx > 0)
                        {
                            ComboBox propNameCombo = propertiesPanel.Controls[curIdx - 1] as ComboBox;
                            if (propNameCombo != null)
                            {
                                string actualName = string.Empty;
                                if (propNameCombo.SelectedItem != null)
                                    actualName = propNameCombo.SelectedItem.ToString();
                                combo.Items.Clear();
                                if (m_userSelectionProperties.ContainsKey(actualName))
                                    combo.Items.AddRange(m_userSelectionProperties[actualName].ToObjArray());
                                if (!string.IsNullOrEmpty(selItem) && !combo.Items.Contains(selItem))
                                    combo.Items.Add(selItem);
                                combo.SelectedItem = selItem;
                            }
                        }
                    }
                }
            }

            //scroll to the correct position:
            propertiesPanel.AutoScrollPosition = new Point(cb.Left, cb.Top);
        }

        private string GetCustomIndex(ComboBox cbName)
        {
            if (cbName == null)
                return string.Empty;

            string[] tmp = cbName.Name.Split('_');
            if ((tmp == null) || (tmp.Length < 2))
                return string.Empty;

            return tmp[1];
        }

        private List<SnippetProperty> BuildPropertiesList(Control.ControlCollection comboxesList, bool includeEmptyValues, bool getSelectedValue = false)
        {
            List<SnippetProperty> pList = new List<SnippetProperty>();

            // retrieve the properties:
            foreach (Control elem in comboxesList)
            {
                ComboBox comb = elem as ComboBox;
                if (comb == null)
                    continue;
                if ((comb.SelectedItem == null) && (string.IsNullOrEmpty(comb.Text)))
                    continue;

                // Search only for combobox having properties name:
                if (comb.Name.StartsWith("Name_"))
                {
                    string idx = GetCustomIndex(comb);
                    if (!string.IsNullOrEmpty(idx))
                    {
                        string propVal = string.Empty;

                        // Search for related value:
                        foreach (Control elemVal in propertiesPanel.Controls)
                        {
                            ComboBox combVal = elemVal as ComboBox;
                            if ((combVal != null) && combVal.Name.Equals("Value_" + idx))
                            {
                                string selValue = combVal.SelectedItem as string;
                                string combText = combVal.Text;

                                if ((selValue == null) && (combText == null))
                                    continue;   // this property is invalid...

                                if (getSelectedValue)
                                    propVal = string.IsNullOrEmpty(selValue) ? combText : selValue.ToString();
                                else
                                    propVal = combText;
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(propVal) || includeEmptyValues)
                        {
                            SnippetProperty newProp = new SnippetProperty(comb.SelectedItem == null ? comb.Text.Trim() : comb.SelectedItem.ToString(), propVal);
                            pList.Add(newProp);
                        }
                    }
                }
            }

            return pList;
        }

        private void delProp_MouseLeftButtonUp(object sender, EventArgs e)
        {
            log.Debug("Deleting property");
            PictureBox xImage = sender as PictureBox;
            if (xImage == null)
                return;
            //int xIdx = wrapPanelProp.Children.IndexOf(xImage);
            int xIdx = propertiesPanel.Controls.IndexOf(xImage);
            if (xIdx < 0)  // this should not occur: current image not found!
                return;
            log.Debug("Deleting property at index " + xIdx);
            propertiesPanel.Controls.RemoveAt(xIdx); //image with "X"
            propertiesPanel.Controls.RemoveAt(xIdx - 1); //combo with property value
            propertiesPanel.Controls.RemoveAt(xIdx - 2); //combo with property name

            RemoveHighlightingFromProperties();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Reset all the textboxes and errors 
        /// </summary>
        public void ResetFields()
        {
            ResetErrors();

            NameTxt.Text = Properties.Resources.NameTextBox;
            TxtNotEditMode(NameTxt, new EventArgs());
            DescriptionTxt.Text = Properties.Resources.DescriptionTextBox;
            TxtNotEditMode(DescriptionTxt, new EventArgs());
            SnipTxt.Text = string.Empty;
            TxtNotEditMode(SnipTxt, new EventArgs());
            TagsTxt.Text = Properties.Resources.TagsTextBox;
            TxtNotEditMode(TagsTxt, new EventArgs());

            SnippetId = -1;
            TargetGroupId = -1;
            LinkToSource = string.Empty;
            GoToOriginalBtn.Visible = false;
            GoToOriginalBtn.Text = string.Empty;
        }


        /// <summary>
        /// Set textbox in normal edit mode (remove italic and special formatting of default help text)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtInEditMode(object sender, EventArgs e)
        {
            TextBox t1 = sender as TextBox;
            ScintillaNET.Scintilla t2 = sender as ScintillaNET.Scintilla;

            if ((t1 != null) && (!t1.ReadOnly))
            {
                if (!CheckTxtChanged(t1))
                {
                    t1.Text = string.Empty;
                    t1.Font = Utilities.s_regularFont;
                    t1.ForeColor = Color.Black;
                }
            }
            else if ((t2 != null) && (!t2.IsReadOnly))  // this is the case of the snipTxt
            {
                snipSuggestion.Visible = false;
            }
        }

        /// <summary>
        /// Set textbox in help mode (set italic and special formatting of default help text)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtNotEditMode(object sender, EventArgs e)
        {
            TextBox t1 = sender as TextBox;
            ScintillaNET.Scintilla t2 = sender as ScintillaNET.Scintilla;

            if ((t1 != null) && (!t1.ReadOnly))
            {
                if (string.IsNullOrWhiteSpace(t1.Text) || !CheckTxtChanged(t1))
                {
                    t1.ForeColor = Utilities.s_lightGrayS2C;

                    switch (t1.Name)
                    {
                        case "NameTxt":
                            t1.Text = Properties.Resources.NameTextBox;
                            break;
                        case "DescriptionTxt":
                            t1.Text = Properties.Resources.DescriptionTextBox;
                            break;
                        case "SnipTxt":
                            t1.Text = Properties.Resources.SnippetTextBox;
                            break;
                        case "TagsTxt":
                            t1.Text = Properties.Resources.TagsTextBox;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    t1.ForeColor = Color.Black;
                }
            }
            else if ((t2 != null) && (!t2.IsReadOnly))  // this is the case of the snipTxt 
            {
                snipSuggestion.Visible = string.IsNullOrWhiteSpace(t2.Text);
                if (snipSuggestion.Visible)
                    snipSuggestion.BringToFront();
            }
        }

        /// <summary>
        /// Reset error fields
        /// </summary>
        public void ResetErrors()
        {
            // reset error fields
            SetErrorString(string.Empty);

            SnipTxt.ForeColor = Color.Black;
            DescriptionTxt.ForeColor = Color.Black;
            NameTxt.ForeColor = Color.Black;
            msgTxt.ForeColor = Color.Red;

            // adjust formatting of text fields (cleans up read validator messages on text fields)
            TxtNotEditMode(SnipTxt, new EventArgs());
            TxtNotEditMode(NameTxt, new EventArgs());
            TxtNotEditMode(DescriptionTxt, new EventArgs());

            // enable submit button
            saveBtn.Enabled = true;
        }

        /// <summary>
        /// Set the error message text with the given string
        /// </summary>
        /// <param name="errorMsg"></param>
        private void SetErrorString(string errorMsg)
        {
            LoginErr.Visible = false;
            LoginBtn.Visible = false;

            if (string.IsNullOrWhiteSpace(errorMsg))
            {
                msgTxt.Text = string.Empty;
                msgTxt.Visible = false;
                s2cLogoImg.Visible = true;
            }
            else
            {
                s2cLogoImg.Visible = false;
                msgTxt.Text = errorMsg;
                msgTxt.Visible = true;
            }
        }

        /// <summary>
        /// This procedure will make all the textboxes of the window writable/"read-only"
        /// </summary>
        public void SetEditableFields(bool enabled)
        {
            NameTxt.ReadOnly = !enabled;
            NameTxt.BackColor = Color.White;
            DescriptionTxt.ReadOnly = !enabled;
            DescriptionTxt.BackColor = Color.White;
            SnipTxt.IsReadOnly = !enabled;
            TagsTxt.ReadOnly = !enabled;
            TagsTxt.BackColor = Color.White;
            visibilityDDL.ReadOnly = !enabled;
        }

        private void NameTxt_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox t = sender as TextBox;

            if (t == null)
                return;

            if (!CheckTxtChanged(t))
            {
                t.Text = string.Empty;
                t.ForeColor = Color.Black;
            }
        }

        private void GoToOriginal_Click(object sender, EventArgs e)
        {
            if ((!string.IsNullOrWhiteSpace(LinkToSource)) && (Snip2Code.Utils.Utilities.UrlIsValid(LinkToSource)))
            {
                // redirect to original source page
                try
                {
                    System.Diagnostics.Process.Start(LinkToSource);
                }
                catch { }
            }
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            if ((BaseWS.CurrentUser == null) || !WebConnector.Current.IsLogged)
            {
                PluginBase.Login_Logout();
                PluginBase.CloseForm(this);
            }
            else
            {
                PopulateGroups(BaseWS.CurrentUser.DefaultGroupID);
                SetErrorString(string.Empty);
            }
        }

        private void publishBtn_Click(object sender, EventArgs e)
        {
            if (SnippetId > 0)
                PluginBase.PublishSnip(SnippetId, TargetGroupId);
        }

        /// <summary>
        /// Check if textbox has changed form its default value
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool CheckTxtChanged(TextBox t)
        {
            switch (t.Name)
            {
                case "NameTxt":
                    if (t.Text != Properties.Resources.NameTextBox)
                        return true;
                    break;
                case "DescriptionTxt":
                    if (t.Text != Properties.Resources.DescriptionTextBox)
                        return true;
                    break;
                case "SnipTxt":
                    if (t.Text != Properties.Resources.SnippetTextBox)
                        return true;
                    break;
                case "TagsTxt":
                    if (t.Text != Properties.Resources.TagsTextBox)
                        return true;
                    break;
                default:
                    return false;
            }
            return false;
        }


        #region IManageSnippetForm Implementation
        /////////////////////////////////////////////////////////////////////////////////

        public void PrepareAddNewSnippet(string selectedText)
        {
            // cleanup fields from a previous execution
            ResetFields();

            // reset snippet id / target group
            SnippetId = -1;
            TargetGroupId = -1;

            // put the selected text into the snippet text field of the new window
            if (selectedText != string.Empty)
            {
                SnipTxt.Text = EntityUtils.TrimStartingTabsOrSpaces(selectedText);
                snipSuggestion.Visible = false;
            }
            else
            {
                // display the snipTxt suggestion
                snipSuggestion.Visible = true;
                snipSuggestion.BringToFront();
            }

            // populate user groups
            if (BaseWS.CurrentUser != null)
                PopulateGroups(BaseWS.CurrentUser.DefaultGroupID, "");
            else
                PopulateGroups(-1, "");

            // autodetect properties
            System.Threading.Thread th = new System.Threading.Thread(PrepareProperties);
            th.Start();

            // disable publish button
            publishBtn.Visible = false;
            visibilityDDL.Width = 175;

            this.Icon = Properties.Resources.addSnipIco;

            // enable all the textboxes
            SetEditableFields(true);
        }

        public void PrepareViewSnippet(Snippet snip)
        {
            // cleanup error messages from previous execution
            ResetErrors();

            // save info needed for publishing
            SnippetId = snip.ID;
            TargetGroupId = snip.TargetGroupID;

            // hide the snipTxt suggestion
            snipSuggestion.Visible = false;

            // populate the snippet fields
            SnipTxt.Text = EntityUtils.TrimStartingTabsOrSpaces(snip.Code);
            TagsTxt.Text = snip.StringTags;
            NameTxt.Text = snip.Name;
            DescriptionTxt.Text = snip.Description;

            // select the visibility
            publishBtn.Visible = false;
            switch (snip.Visibility)
            {
                case Snip2Code.Model.Entities.ShareOption.Protected:
                    PopulateGroups(snip.OwnerGroupID, snip.OwnerGroupName);
                    break;
                case Snip2Code.Model.Entities.ShareOption.Public:
                    PopulateGroups(0, Commons.GroupNames.Public);
                    break;
                case Snip2Code.Model.Entities.ShareOption.Private:
                default:
                    PopulateGroups(-1, Commons.GroupNames.MyOwn);    // only to creator
                    break;
            }

            // display publish button
            UserClient currUser = BaseWS.CurrentUser;
            if ((currUser != null) && (snip.IsAllowedToChangeVisibility(currUser)))
            {
                // enable publish button
                publishBtn.Visible = true;
            }

            // display creator
            if (!string.IsNullOrWhiteSpace(snip.CreatorName))
            {
                CreatedByLabel.Text = string.Format("{0}: {1}", Properties.Resources.CreatedBy, snip.CreatorName.WithEllipsis(26));
                CreatedByLabel.Visible = true;
                CreatedByLabel.ForeColor = Color.Black;
            }

            // original source
            if (!String.IsNullOrWhiteSpace(snip.LinkToSource))
            {
                LinkToSource = snip.LinkToSource;
                GoToOriginalBtn.Visible = true;    
            }
            else
            {
                LinkToSource = string.Empty;
                GoToOriginalBtn.Visible = false;
            }
            GoToOriginalBtn.Text = LinkToSource;

            // display properties
            log.DebugFormat("Populating properties for snippet {0}", SnippetId);
            PopulateProperties(snip.Properties, false);
            log.DebugFormat("Populated properties for snippet {0}", SnippetId);

            saveBtn.Visible = false;
            copyCodeBtn.Visible = true;

            this.Icon = Properties.Resources.searchIco;

            // disable all the textboxes
            SetEditableFields(false);

            copyCodeBtn.Focus();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////
    }
}
