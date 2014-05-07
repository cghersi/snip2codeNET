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
    partial class frmSearch : Form, ISearchSnippetForm
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      // used by background login thread to communicate with the UI
        private BackgroundWorker bgw = null;

        // semaphore on Search snippet btn
        private bool SearchClicked = false;

        // stores current Search text
        private string currentSearchText = string.Empty;
        // stores suggested Search text
        private static string suggestedSearchText = string.Empty;

        private const int INIT_HEIGHT_SEARCH_COMBO = 24;

        public frmSearch()
        {
            InitializeComponent();
            ResetErrors();
            ResetFields();

            SuggestedStr.Text = Properties.Resources.SearchMispellingText;
            SuggestedStr.Visible = false;
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            PluginBase.CloseForm(this);
        }


        private void SearchBtn_Click(object sender, EventArgs e)
        {
            if (SearchClicked)
                return; // multiple click, exit!!

            SearchClicked = true;

            SearchCombobox.DroppedDown = false; //hide suggestions for search history
            SearchCombobox.Size = new Size(SearchCombobox.Size.Width, INIT_HEIGHT_SEARCH_COMBO);

            // empty string means *
            if (string.IsNullOrWhiteSpace(SearchCombobox.Text) || SearchCombobox.Text.Equals(Properties.Resources.SearchSuggestion))
            {
                SearchCombobox.Text = "*";
                SearchCombobox.ForeColor = Color.Black;
            }

            // cleanup previous results
            log.Debug("Search: before cleaning results, Search text is:" + SearchCombobox.Text.Trim());
            ResetResults();
            currentSearchText = SearchCombobox.Text.Trim();
            log.Debug("currentSearchText:" + currentSearchText);

            Search();

            // save Search history into file
            if (!currentSearchText.IsNullOrWhiteSpaceOrEOF() && !currentSearchText.Equals("*") && !currentSearchText.Equals(Properties.Resources.SearchSuggestion))
            {
                UserPlugin.Current.AddSearchHistoryEntry(currentSearchText);
            }
            LoadSearchHistory(null);
        }


        public void ResetErrors()
        {
            // reset error fields
            ErrMsg.Visible = false;
            ErrMsg.Text = "";

            // enable submit button
            SearchBtn.Enabled = true;
        }

        public void ResetResults()
        {
            // cleans up the list view
            listSnippetWiew.Controls.Clear();

            ResultsRecap.Text = "";
            currentSearchText = string.Empty;
            suggestedSearchText = string.Empty;
            ResultsRecap.Visible = false;
            showMoreBox.Visible = false;

            SuggestedStr.Visible = false;
            SetSuggestedVisibility(false);
        }


        public void DisplayCleanForm()
        {
            ResetResults();
            SearchCombobox.Text = string.Empty;
        }

        public void ResetFields()
        {
            SearchCombobox.Text = Properties.Resources.SearchSuggestion;
            SearchCombobox_LostFocus(SearchCombobox, null);

            ResetErrors();
            ResetResults();

            LoadSearchHistory(null);
        }


        private void LoadSearchHistory(string prefix)
        {
            //SearchCombobox.Items.Clear();
            List<string> historyEntries = UserPlugin.Current.GetTopHistoryEntries(AppConfig.Current.SearchHistoryNumItemToDisplay, prefix);
            SearchCombobox.Items.Clear();
            SearchCombobox.Items.AddRange(historyEntries.ToObjArray());
            if (!string.IsNullOrEmpty(prefix))
                SearchCombobox.SelectionStart = prefix.Length;
        }

        private void showMore_Click(object sender, EventArgs e)
        {
            if (SearchClicked)
                return; // multiple click, exit!!

            SearchClicked = true;

            // detect if someone has changed Search text in the meanwhile
            if (!currentSearchText.Equals(SearchCombobox.Text.Trim()))
            {
                ResetResults();

                // empty string means *
                if (string.IsNullOrWhiteSpace(SearchCombobox.Text) || SearchCombobox.Text.Equals(Properties.Resources.SearchSuggestion))
                {
                    SearchCombobox.Text = "*";
                    SearchCombobox.ForeColor = Color.Black;
                }

                currentSearchText = SearchCombobox.Text.Trim();
            }

            Search();
        }

        private void Search()
        {
            ResetErrors();

            // Search snippets in a background thread
            progressBar.Visible = true;
            KeyValuePair<string, int> search_count = new KeyValuePair<string, int>(currentSearchText, listSnippetWiew.Controls.Count);

            bgw = new BackgroundWorker();
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            bgw.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            bgw.WorkerReportsProgress = true;
            if (!bgw.IsBusy)
                bgw.RunWorkerAsync(search_count);

        }


        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            SnippetSearchResult result = new SnippetSearchResult();
            e.Result = result;

            KeyValuePair<string, int> search_count = (KeyValuePair<string, int>)e.Argument;

            // read snippets and populate the List
            SnippetsWS snippetRepo = new SnippetsWS();
            int searchBlock = 20;   // length of each block of snippets read via web call
            int startIndex = search_count.Value;      // current block of snippets
            int numReturnedSnippets = 0;
            int numReturnedSnippetsWithNonDefOperator = 0;

            Dictionary<string, string[]> mispelling = null;
            SortedDictionary<string, int> tags = null;
            IList<Snippet> snipList = snippetRepo.GetSnippetsForSearch(search_count.Key, out mispelling, out numReturnedSnippets,
                out numReturnedSnippetsWithNonDefOperator, out tags, searchBlock, startIndex);

            //TODO: display results with non default operator

            result.snippetNum = numReturnedSnippets;
            if (result.snippetNum <= 0)
            {
                try
                {
                    result.errorMsg = Utilities.GetResourceLocalizedMsg<ErrorCodes>(snippetRepo.LastErrorCode);
                }
                catch (Exception)
                {
                    result.errorMsg = null;
                }
                if (string.IsNullOrEmpty(result.errorMsg))
                    result.errorMsg = Properties.Resources.ErrorCodes_UNKNOWN;
            }
            int p = 1;

            foreach (Snippet s in snipList)
            {
                SnipInfo sinfo = new SnipInfo();
                if (s != null)
                {
                    sinfo.snip_image = PictureManager.GetImageOfUser(s.CreatorID, new System.Drawing.Size(50, 50), s.CreatorImageNum);
                    sinfo.snip_name = s.Name;
                    sinfo.snip_description = s.Description;
                    sinfo.snip_creator = s.CreatorName;
                    sinfo.snip_ID = s.ID;

                    bgw.ReportProgress(p++, sinfo);
                }
            }

            e.Result = new KeyValuePair<SnippetSearchResult, Dictionary<string, string[]>>(result, mispelling);
        }

        private void SetErrorMsg(string errorMsg)
        {
            ErrMsg.Visible = true;
            ErrMsg.Text = errorMsg;
            showMoreBox.Visible = false;
        }


        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Visible = false;
            KeyValuePair<SnippetSearchResult, Dictionary<string, string[]>> returnedValue = (KeyValuePair<SnippetSearchResult, Dictionary<string, string[]>>)e.Result;

            SnippetSearchResult result = returnedValue.Key as SnippetSearchResult;
            Dictionary<string, string[]> mispellings = returnedValue.Value as Dictionary<string, string[]>;
            if (result == null)
                result = new SnippetSearchResult();
            int numReturnedSnippets = result.snippetNum;

            //  log.Debug("bgw_RunWorkerCompleted - currentSearchText: " + currentSearchText + " - SearchCombobox.Text: " + SearchCombobox.Text);
            SearchCombobox.Text = currentSearchText;

            if (numReturnedSnippets == 0)
            {
                if (result.errorMsg.CompareTo(Utilities.GetResourceLocalizedMsg<ErrorCodes>(ErrorCodes.OK)) == 0)
                {
                    ResultsRecap.Text = Properties.Resources.NoResults;
                }
                else
                {
                    SetErrorMsg(result.errorMsg);
                }
            }
            else
            {
                if (listSnippetWiew.Controls.Count == 0)
                {
                    ResultsRecap.Text = Properties.Resources.NoResults;
                }
                else
                {
                    ResultsRecap.Text = string.Format(Properties.Resources.SearchSnippetRecap1, numReturnedSnippets);

                    if (numReturnedSnippets > listSnippetWiew.Controls.Count)
                    {  // there are more snippets available
                        ResultsRecap.Text += string.Format(": " + Properties.Resources.SearchSnippetRecap2, listSnippetWiew.Controls.Count);
                        showMoreBox.Visible = true;
                    }
                    else
                    {  // we are displaying everything
                        showMoreBox.Visible = false;
                    }
                }
            }

            // cleanup the panel containing mispelling labels
            SuggestedPanel.Controls.Clear();

            // populate mispellings
            if ((mispellings != null) && (mispellings.Count > 0))
            {
                // split current Search terms by words
                List<string> mispellingArr = currentSearchText.SplitWords();

                for (int i = 0; i < mispellingArr.Count; i++)
                {
                    // prepare the label to be added to the panel TODO
                    Label suggestedLbl = new Label();
                    suggestedLbl.TextAlign = ContentAlignment.TopLeft; 
                    suggestedLbl.Margin = new Padding(2, 6, 0, 0);
                    suggestedLbl.Name = "SuggestedLbl" + i;
                    suggestedLbl.Padding = new Padding(0, 0, 0, 0);
                    suggestedLbl.Cursor = Cursors.Hand;
                    suggestedLbl.Visible = true;

                    bool foundMispelling = false;

                    // Search if the word must be replaced by a suggestion
                    foreach (string key in mispellings.Keys)
                    {
                        if ((mispellingArr[i].Equals(key, StringComparison.InvariantCultureIgnoreCase)) &&
                            (!mispellings[key].IsNullOrEmpty()))
                        {
                            mispellingArr[i] = mispellings[key][0];
                            foundMispelling = true;
                            break;
                        }
                    }

                    // style and add the label to the panel
                    suggestedLbl.Text = mispellingArr[i];

                    if (foundMispelling)
                        suggestedLbl.Font = new Font(new FontFamily("Verdana"), 11, FontStyle.Bold);
                    else
                        suggestedLbl.Font = new Font(new FontFamily("Verdana"), 11, FontStyle.Italic);

                    SuggestedPanel.Controls.Add(suggestedLbl);
                }

                suggestedSearchText = string.Join(" ", mispellingArr);
                SuggestedStr.Visible = true;
            }

            ResultsRecap.Visible = true;
            SearchClicked = false;
        }

        // This event handler updates the UI
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SnipInfo sinfo = e.UserState as SnipInfo;

            if (sinfo != null)
            {
                SnippetInfo resItem = new SnippetInfo(sinfo.snip_name,
                                            sinfo.snip_description,
                                            sinfo.snip_creator,
                                            sinfo.snip_image,
                                            sinfo.snip_ID);

                listSnippetWiew.Controls.Add(resItem);
            }
        }


        private void SearchCombobox_KeyDown(object sender, KeyEventArgs e)
        {
            //    log.Debug("SearchCombobox_KeyDown " + e.Key);
            ComboBox t = sender as ComboBox;

            if (t == null)
                return;

            if (!CheckTxtChanged(t))
            {
                t.Text = string.Empty;
                t.ForeColor = Color.Black;
            }
        }


        private void SearchCombobox_LostFocus(object sender, EventArgs e)
        {
            ComboBox t1 = sender as ComboBox;

            if (t1 != null)
            {
                if (string.IsNullOrWhiteSpace(t1.Text) || !CheckTxtChanged(t1))
                {
                    t1.ForeColor = Utilities.s_lightGrayS2C;
                    t1.Text = Properties.Resources.SearchSuggestion;
                }
                else
                {
                    t1.ForeColor = Color.Black;
                }
            }
            SearchCombobox.DroppedDown = false;
            SearchCombobox.Size = new Size(SearchCombobox.Size.Width, INIT_HEIGHT_SEARCH_COMBO);
        }

        /// <summary>
        /// Check if textbox has changed form its default value
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool CheckTxtChanged(ComboBox t)
        {
            if (t.Text != Properties.Resources.SearchSuggestion)
                return true;
            else
                return false;
        }

        private void SearchCombobox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    SearchBtn_Click(SearchBtn, new EventArgs()); 
                    break;
                case Keys.Down:
                case Keys.Up:
                    //nothing to do here...
                    break;
                default:
                    LoadSearchHistory(SearchCombobox.Text);
                    int newHeight = INIT_HEIGHT_SEARCH_COMBO;
                    if (SearchCombobox.Items != null)
                        newHeight += SearchCombobox.Items.Count * 20;
                    SearchCombobox.Size = new Size(SearchCombobox.Size.Width, newHeight);
                    SearchCombobox.DroppedDown = true;
                    SearchCombobox.BringToFront();
                    break;
            }
        }

        /// <summary>
        /// Click on suggested string button: populate Search string and launch a Search with the new terms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SuggestedStr_MouseUp(object sender, MouseEventArgs e)
        {
            SearchCombobox.Text = suggestedSearchText;

            SuggestedStr.Visible = false; 
            SetSuggestedVisibility(false); 
            SearchBtn_Click(sender, e);
        }


        private void SetSuggestedVisibility(bool visible)
        {
            bool vis = false;
            if (visible)
                vis = true;
       
            foreach (Control el in SuggestedPanel.Controls)
            {
                el.Visible = vis;
            }
        }

        private void listSnippetWiew_MouseDown(object sender, MouseEventArgs e)
        {
            if (!listSnippetWiew.Focused)
                listSnippetWiew.Focus();
        }

        private void listSnippetWiew_MouseEnter(object sender, EventArgs e)
        {
            if (!listSnippetWiew.Focused)
                listSnippetWiew.Focus();
        }
    }
}
