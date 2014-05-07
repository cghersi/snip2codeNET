//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

using Snip2Code.Model.Abstract;
using Snip2Code.Model.Client.WSFramework;
using Snip2Code.Model.Comm;
using Snip2Code.Utils;
using System.Text;
using System.Linq;
using System.Globalization;

namespace Snip2Code.Model.Client.Entities
{
    public class UserClient : UserComm
    {
        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Default constructor for JSON parsing
        /// </summary>
        public UserClient() : base() { }

        /// <summary>
        /// Constructor to set validity
        /// </summary>
        public UserClient(bool isValid) : base() 
        {
            IsValid = isValid;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Repository
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override IUserRepository RetrieveRepository()
        {
            return new UserWS();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        public static string UserAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string Snip2CodeBaseDir = System.IO.Path.Combine(UserAppData, "Snip2Code_" + AppConfig.Current.S2CHeader);
        public static string UserPreferencesFile = System.IO.Path.Combine(Snip2CodeBaseDir, "userPref.conf");
        public static string SearchHistoryFile = System.IO.Path.Combine(Snip2CodeBaseDir, "search.history");
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, SearchHistoryEntry> m_searchEntries = new Dictionary<string,SearchHistoryEntry>();


        #region USER PREFERENCES
        //-------------------------------

        public bool RetrieveUserPreferences()
        {
            BaseWS.Server = AppConfig.Current.SiteBasePath; //default value

            // populate username / pwd
            if (System.IO.File.Exists(UserPreferencesFile))
            {
                try
                {
                    string decodedPref = System.IO.File.ReadAllText(UserPreferencesFile, Encoding.Default);
                    if (decodedPref.IsNullOrWhiteSpaceOrEOF())
                        return false;
                    string savedCredentials = Snip2Code.Utils.Encryptor.Decrypt(decodedPref);
                    string[] splittedCredentials = savedCredentials.Split(',');
                   
                    if ((splittedCredentials == null) || (splittedCredentials.Length == 0))     // no credentials saved
                        return false;
                    if (splittedCredentials.Length >= 2)
                    {    // username and password
                        BaseWS.Username = splittedCredentials[0];
                        BaseWS.Password = splittedCredentials[1];
                        if ((splittedCredentials.Length >= 3) && (splittedCredentials[2] != string.Empty))
                        {
                            BaseWS.Server = splittedCredentials[2];
                        }
                        if (splittedCredentials.Length >= 5)
                        {
                            BaseWS.IdentToken = splittedCredentials[3];
                            BaseWS.UseOneAll = bool.Parse(splittedCredentials[4]);
                        }
                        return true;
                    }
                    else
                        log.ErrorFormat("User preferences cannot be parsed. DecodedContent={0} ", savedCredentials);
                }
                catch (Exception ioex)
                {
                    log.ErrorFormat("Cannot read user preferences: {0} ", ioex.Message);
                }
            }
            return false;
        }


        public void SaveUserPreferences()
        {
            string encryptedPref = string.Empty;

         //   if (!string.IsNullOrEmpty(BaseWS.Username) && !string.IsNullOrEmpty(BaseWS.Password))
         //   {
                // Save credentials
                encryptedPref = Snip2Code.Utils.Encryptor.Encrypt(string.Format("{0},{1},{2},{3},{4}",
                    BaseWS.Username.CheckNullAndTrim(), BaseWS.Password.CheckNullAndTrim(),
                    BaseWS.Server.CheckNullAndTrim(), BaseWS.IdentToken.CheckNullAndTrim(), BaseWS.UseOneAll));
         //   }

            try
            {
                // create Snip2Code Base Directory for current user, if does not exist on filesystem
                if (!System.IO.Directory.Exists(Snip2CodeBaseDir))
                    System.IO.Directory.CreateDirectory(Snip2CodeBaseDir);

                // Create the file (if not found) and write to it.
                System.IO.File.WriteAllText(UserPreferencesFile, encryptedPref, Encoding.Default);
            }
            catch (Exception ioex)
            {
                log.ErrorFormat("Cannot save user preferences: {0} ", ioex.Message);
            }
        }

        #endregion


        #region SEARCH HISTORY
        //-------------------------------

        public List<string> GetTopHistoryEntries(int numOfEntries, string prefix) {
            List<string> retList = new List<string>();

            IEnumerable<KeyValuePair<string, SearchHistoryEntry>> sortedDict = null;
            if (prefix.IsNullOrWhiteSpaceOrEOF() || prefix.Equals("*"))
                sortedDict = m_searchEntries.OrderByDescending(entry => entry.Value.HitCount)
                         .Take(numOfEntries)
                         .ToList();
            else
                sortedDict = m_searchEntries.Where(entry => entry.Key.ToLower().Contains(prefix.ToLower()))
                         .OrderByDescending(entry => entry.Value.HitCount)
                         .Take(numOfEntries)
                         .ToList();

            if (sortedDict == null)
                return retList;
            foreach (KeyValuePair<string, SearchHistoryEntry> kvp in sortedDict)
            {
                retList.Add(kvp.Key);
            }

            return retList;
        }

        /// <summary>
        /// Update Search History 
        /// </summary>
        public void AddSearchHistoryEntry(string searchText)
        {
            // update history entries            
            if (string.IsNullOrEmpty(searchText))
                return;

            if (m_searchEntries.ContainsKey(searchText))
            {
                m_searchEntries[searchText].UpdateEntry();
            }
            else
            {
                m_searchEntries.Add(searchText, new SearchHistoryEntry(searchText));
            }

            // prepare the object to be written into file
            StringBuilder sb = new StringBuilder();
            var sortedDict = m_searchEntries.OrderByDescending(entry => entry.Value.HitCount)
                             .Take(AppConfig.Current.SearchHistoryNumItemToStore)
                             .ToList();

            foreach (KeyValuePair<string, SearchHistoryEntry> kvp in sortedDict)
            {
                sb.AppendFormat("{0},{1},{2}{3}",
                    kvp.Value.HitCount, kvp.Value.LastSearchTime.ToString(SearchHistoryEntry.SEARCH_TIME_DATE_FORMAT),
                    kvp.Value.SearchText, Environment.NewLine);
            }


            // write to file
            try
            {
                // create Snip2Code Base Directory for current user, if does not exist on filesystem
                if (!System.IO.Directory.Exists(Snip2CodeBaseDir))
                    System.IO.Directory.CreateDirectory(Snip2CodeBaseDir);

                // Create the file (if not found) and write to it.
                System.IO.File.WriteAllText(SearchHistoryFile, sb.ToString(), Encoding.Default);
            }
            catch (Exception ioex)
            {
                log.ErrorFormat("Cannot save search history: {0} ", ioex.Message);
            }
        }


        public void LoadSearchHistoryFromFile()
        {
            m_searchEntries.Clear();

            // populate the dictionary with the history of searches:
            if (System.IO.File.Exists(SearchHistoryFile))
            {
                try
                {
                    IEnumerable<string> savedHistory = System.IO.File.ReadLines(SearchHistoryFile, Encoding.Default);
                    if (savedHistory != null)
                    {
                        foreach (string line in savedHistory)
                        {
                            string[] entries = line.Split(',');

                            if (entries.Length < 3)
                                log.ErrorFormat("Search History cannot be parsed. DecodedContent={0} ", line);

                            SearchHistoryEntry she = new SearchHistoryEntry();
                            she.HitCount = int.Parse(entries[0]);
                            she.LastSearchTime = DateTime.ParseExact(entries[1], SearchHistoryEntry.SEARCH_TIME_DATE_FORMAT, CultureInfo.InvariantCulture);
                            she.SearchText = string.Join(",", entries, 2, entries.Length - 2);
                            m_searchEntries.Add(she.SearchText, she);
                        }
                    }
                }
                catch (Exception ioex)
                {
                    log.ErrorFormat("Cannot read Search History: {0} ", ioex.Message);
                }
            } 

            if (m_searchEntries.IsNullOrEmpty()) 
            {
                m_searchEntries.Add("singleton", new SearchHistoryEntry("singleton"));
                m_searchEntries.Add("replace character in string", new SearchHistoryEntry("replace character in string"));
                m_searchEntries.Add("create excel file c#", new SearchHistoryEntry("create excel file c#"));
                m_searchEntries.Add("Open URL in external browser \"Windows Version\"=Windows8", new SearchHistoryEntry("Open URL in external browser \"Windows Version\"=Windows8"));
                m_searchEntries.Add("metro style file exist*", new SearchHistoryEntry("metro style file exist*"));
            }
        }

        #endregion
    }
}
