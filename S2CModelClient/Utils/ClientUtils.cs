//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using Snip2Code.Model.Client.Entities;
using Snip2Code.Model.Client.WSFramework;
using System;
using System.Configuration;
using log4net;
using Snip2Code.Model.Entities;
using System.Collections.Generic;

namespace Snip2Code.Utils
{
    public class ClientUtils
    {
        public static System.Threading.Timer LoginTimer;
        private static ILog log = null;

        /// <summary>
        /// Performs all the operations needed to init the plugin/App
        /// </summary>
        /// <param name="user">singleton instance of UserPlugin</param>
        /// <param name="rootPath">path where the config files can be found</param>
        /// <returns>true if the user is already logged in; false if still logged out</returns>
        public static bool InitApp(UserClient user, string rootPath, IPluginManager pluginManager)
        {
            if (user == null)
                return false;

            // initialize the config file
            Snip2Code.Utils.AppConfig.Current.Initialize(rootPath);
            log = LogManager.GetLogger("ClientUtils");
 
            // login the user
            bool res = user.RetrieveUserPreferences();
            bool loggedIn = false;
            if (res && ((!BaseWS.Username.IsNullOrWhiteSpaceOrEOF()) || (!BaseWS.IdentToken.IsNullOrWhiteSpaceOrEOF())))
            {
                LoginPoller poller = new LoginPoller(BaseWS.Username, BaseWS.Password, BaseWS.IdentToken, BaseWS.UseOneAll,
                                                        pluginManager);
                LoginTimer = new System.Threading.Timer(poller.RecallLogin, null, 0, AppConfig.Current.LoginRefreshTimeSec * 1000);
                loggedIn = true;
            }

            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                user.LoadSearchHistoryFromFile();
            }, null);

            //set the empty profile picture for search list results:
            PictureManager.SetEmptyProfilePic(rootPath);

            return loggedIn;
        }


        #region Manage Snippet Form
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Creates a new AddSnippet Form and precompiles this form with the selected text
        /// </summary>
        /// <param name="pluginManager"></param>
        public static IManageSnippetForm PrepareAddNewSnippetForm(IPluginManager pluginManager)
        {
            if (pluginManager == null)
                return null;

            // Manage editor selection
            string selText = pluginManager.RetrieveSelectedText();

            // Open "Add Snippet" Window
            pluginManager.ClosePublishSnippetWindow();
            IManageSnippetForm addSnipForm = pluginManager.CreateAddSnippetForm();
            if (addSnipForm != null)
                addSnipForm.PrepareAddNewSnippet(selText);

            return addSnipForm;
        }


        /// <summary>
        /// Creates a new ViewSnippet Form and precompiles this form with the content of the given snippet
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="snippetId">ID of the snppet to display</param>
        public static IManageSnippetForm PrepareViewSnippetForm(IPluginManager pluginManager, long snippetId)
        {
            if (pluginManager == null)
                return null;

            if (snippetId <= 0)
                return null;

            // read snippets and populate the List
            SnippetsWS snippetRepo = new SnippetsWS();
            log.DebugFormat("Reading snippet {0}", snippetId);
            Snippet snip = snippetRepo.GetSnippetByID(snippetId);

            if (snip == null)
            {
                log.WarnFormat("Snippet {0} is null", snippetId);
                return null;
            }

            //CG: Add +1 to the view stats
            System.Threading.Tasks.Task.Factory.StartNew(() => snippetRepo.StoreHit(snip));

            // Open "Add Snippet" Window
            log.DebugFormat("Loading snippet {0}", snippetId);
            pluginManager.ClosePublishSnippetWindow();

            IManageSnippetForm viewSnipForm = pluginManager.CreateViewSnippetForm();
            viewSnipForm.PrepareViewSnippet(snip);

            return viewSnipForm;
        }

        /// <summary>
        /// Show "Publish snippet" window 
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="snippetId">ID of the snippet to display</param>
        /// <param name="targetGroupId"></param>
        public static IPublishSnippetForm PreparePublishSnippetForm(IPluginManager pluginManager, long snippetId, int targetGroupId)
        {
            if (pluginManager == null)
                return null;

            IPublishSnippetForm pubForm = pluginManager.CreatePublishSnippetForm();
            if (pubForm != null)
                pubForm.DisplayCleanForm(snippetId, targetGroupId);

            return pubForm;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////

        #region Search Snippets Form
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Clean up search window
        /// </summary>
        /// <param name="pluginManager"></param>
        public static void SearchPageCleanup(IPluginManager pluginManager)
        {
            // clean up previous search contents (privacy concern)
            List<ISearchSnippetForm> searchForms = pluginManager.FindWindows<ISearchSnippetForm>();
            if (searchForms != null)
            {
                foreach (ISearchSnippetForm searchForm in searchForms)
                {
                    searchForm.DisplayCleanForm();
                }
            }
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////

        #region Login Form
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Creates a new Login Form
        /// </summary>
        /// <param name="pluginManager"></param>
        public static ILoginForm PrepareLoginForm(IPluginManager pluginManager)
        {
            if (pluginManager == null)
                return null;

            ILoginForm loginForm = pluginManager.CreateLoginForm();

            // reset oneAll page
            LoginPageCleanup(pluginManager);

            // Give focus to login button
            loginForm.InitialFocus();

            return loginForm;
        }

        /// <summary>
        /// Clean up login window
        /// </summary>
        public static void LoginPageCleanup(IPluginManager pluginManager)
        {
            ILoginForm loginForm = pluginManager.FindWindow<ILoginForm>();
            if (loginForm != null)
                loginForm.DisplayCleanForm();
        }

        /// <summary>
        /// Opens the default browser on the signup URL for the web app
        /// </summary>
        public static void OpenSignupPage()
        {
            string signupUrl = string.Format("{0}{1}", BaseWS.Server, AppConfig.Current.SignupPage);
            try
            {
                System.Diagnostics.Process.Start(signupUrl);
            }
            catch { }
        }

        /// <summary>
        /// Opens the default browser on the "retrieve password" URL for the web app
        /// </summary>
        public static void OpenRetrievePwdPage()
        {
            string retrievePwdUrl = string.Format("{0}{1}", BaseWS.Server, AppConfig.Current.RetrievePwdPage);
            try
            {
                System.Diagnostics.Process.Start(retrievePwdUrl);
            }
            catch { }
        }


        /// <summary>
        /// Try to login into the remote erver with the given credentials
        /// </summary>
        /// <param name="user"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>true if the login has been successful; false otherwise</returns>
        public static ErrorCodes TryLogin(UserClient user, string username, string password, IPluginManager pluginManager)
        {
            // Test login
            UserWS repoUser = new UserWS();
            Snip2Code.Model.Entities.User loggedUser = repoUser.LoginUser(username, password);

            if ((loggedUser != null) && (loggedUser.ID > 0))
            {
                // Save credentials
                BaseWS.Username = username;
                BaseWS.Password = password;
                BaseWS.IdentToken = string.Empty;
                user.SaveUserPreferences();

                // change the logged user on the package
                LoginPoller poller = new LoginPoller(username, password, string.Empty, false, pluginManager);
                if (ClientUtils.LoginTimer != null)
                    ClientUtils.LoginTimer.Dispose();
                ClientUtils.LoginTimer = new System.Threading.Timer(poller.RecallLogin, null, 0, AppConfig.Current.LoginRefreshTimeSec * 1000);
                return ErrorCodes.OK;
            }

            if (repoUser.LastErrorCode == ErrorCodes.OK)
                return ErrorCodes.NOT_LOGGED_IN;
            else
                return repoUser.LastErrorCode;
        }


        public static ErrorCodes TryLoginOneAll(UserClient user, IPluginManager pluginManager)
        {
            UserWS repoUser = new UserWS();
            string identToken = BaseWS.IdentToken;
            Snip2Code.Model.Entities.User loggedUser = repoUser.LoginUserOneAll(identToken);

            if ((loggedUser != null) && (loggedUser.ID > 0))
            {
                // Save credentials
                BaseWS.Username = string.Empty;
                BaseWS.Password = string.Empty;
                user.SaveUserPreferences();

                // change the logged user on the package
                LoginPoller poller = new LoginPoller(string.Empty, string.Empty, identToken, true, pluginManager);
                if (ClientUtils.LoginTimer != null)
                    ClientUtils.LoginTimer.Dispose();
                ClientUtils.LoginTimer = new System.Threading.Timer(poller.RecallLogin, null, 0, AppConfig.Current.LoginRefreshTimeSec * 1000);

                return ErrorCodes.OK;
            }
            else
            {
                // something went wrong on the login: reload the oneAllPage
                BaseWS.IdentToken = string.Empty;

                if (repoUser.LastErrorCode == ErrorCodes.OK)
                    return ErrorCodes.NOT_LOGGED_IN;
                else
                    return repoUser.LastErrorCode;
            }
        }


        public static string GenerateOneallIdentToken()
        {
            if (BaseWS.IdentToken.IsNullOrWhiteSpaceOrEOF())
                BaseWS.IdentToken = Utilities.CurrentTimeInMillis() + "_" + Utilities.LongRandom(0, 100000000000000000, new Random()) +
                            "_" + AppConfig.Current.S2CHeader;
            return BaseWS.Server + "Access/LoginOneAll?identToken=" + BaseWS.IdentToken;
        }


        #endregion
        /////////////////////////////////////////////////////////////////////////////////

        #region About Form
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Closes the obsolete forms when the server is changed in the About form
        /// </summary>
        /// <param name="pluginManager"></param>
        public static void CloseObsoleteFormsFromAbout(IPluginManager pluginManager)
        {
            SearchPageCleanup(pluginManager);
            pluginManager.ClosePublishSnippetWindow();
            pluginManager.CloseAddSnippetWindow();
        }


        /// <summary>
        /// Do all the actions needed when a wrong login is detected after changing the server URL
        /// from About form
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="user"></param>
        /// <param name="oldServer">previous URL of the server</param>
        /// <param name="newServer">new URL of the server</param>
        public static void ManageInvalidLoginOnNewServerFromAbout(IPluginManager pluginManager, UserClient user,
            string oldServer, string newServer)
        {
            // change the visibility of buttons of top menu and right-click
            BaseWS.Server = oldServer;    // this is needed to logout the old session on the old server
            LogoutUser(pluginManager, user);
            BaseWS.Server = newServer;    // from now on, go ahead with the new server
            // save new server path
            user.SaveUserPreferences();

            // update the login page to address the one-all form to the new server
            LoginPageCleanup(pluginManager);
        }


        /// <summary>
        /// Try to connect to the provided server URL
        /// </summary>
        /// <param name="newserv"></param>
        /// <returns>a structure to represent the results of this action</returns>
        public static AboutResultReport TryChangeServerURL(string newserv)
        {
            string oldserv = BaseWS.Server;

            AboutResultReport res = new AboutResultReport();
            res.newServer = newserv;
            res.oldServer = oldserv;

            if (newserv != oldserv) // we have changed something
            {
                // Ping the new server, if unreacheable display message
                if (WebConnector.Current.PingS2CServer(newserv + WebConnector.PING_URL))
                {
                    // test the login: if not successful, logout the user and display the window
                    BaseWS.Server = newserv;
                    UserWS usRepo = new UserWS();
                    User currUser = usRepo.LoginUser(BaseWS.Username, BaseWS.Password);
                    res.currentUser = currUser;
                }
                else
                {
                    // network down / invalid server
                    res.error = "The server inserted is unreacheable now!";
                }
            }

            return res;
        }

        /// <summary>
        /// Opens the default browser on the base URL for the web app
        /// </summary>
        public static void OpenSiteLink()
        {
            string homepageUrl = BaseWS.Server;
            try
            {
                System.Diagnostics.Process.Start(homepageUrl);
            }
            catch { }
        }

        /// <summary>
        /// Opens the default mail client to send email
        /// </summary>
        public static void ContactUsLink()
        {
            string contactUri = string.Format("{0}{1}", BaseWS.Server, AppConfig.Current.ContactUsPage);
            try
            {
                System.Diagnostics.Process.Start(contactUri);
            }
            catch { }
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////

        #region Logout Actions
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Performs the logout action for the current user
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="user"></param>
        public static void LogoutUser(IPluginManager pluginManager, UserClient user)
        {
            System.Threading.Thread th = new System.Threading.Thread(
                new System.Threading.ThreadStart(delegate { DoLogout(user); })
            );
            th.Start();

            // change the buttons 
            pluginManager.DisplayLoggedOutButtons();

            // clean up previous search contents (privacy concern)
            ISearchSnippetForm searchForm = pluginManager.FindWindow<ISearchSnippetForm>();
            if (searchForm != null)
                searchForm.DisplayCleanForm();

            // clean up previous publish info
            IPublishSnippetForm publishForm = pluginManager.FindWindow<IPublishSnippetForm>();
            if (publishForm != null)
                publishForm.ResetResults();

            // close publish window
            pluginManager.ClosePublishSnippetWindow();

            // close addSnippet Window
            pluginManager.CloseAddSnippetWindow();

            // cleanup previous login from login page
            ILoginForm loginForm = pluginManager.FindWindow<ILoginForm>();
            if (loginForm != null)
                loginForm.ResetFields();

            // show login page 
            PrepareLoginForm(pluginManager);
        }


        private static void DoLogout(UserClient user)
        {
            log.Debug("Logging out current user...");
            // deletes the stored credentials
            BaseWS.Logout();
            if (user != null)
                user.SaveUserPreferences();

            // disconnect the user and stop the periodic login thread
            WebConnector.Current.Logout();
            if (LoginTimer != null)
                LoginTimer.Dispose();
            log.Debug("Logout completed!");
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns the version of the current assembly
        /// </summary>
        /// <returns>default to 1.0.0 if any error occurred; the actual executing assembly version otherwise</returns>
        public static string GetPluginVersion()
        {
            string assemblyVersionStr = "1.0.0";
            Version assVers = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (assVers != null)
                assemblyVersionStr = string.Format("{0}.{1}.{2}", assVers.Major, assVers.Minor, assVers.Build);
            return assemblyVersionStr;
        }
    }

    public class AboutResultReport
    {
        public string error = "";
        public User currentUser = null;
        public string newServer = "";
        public string oldServer = "";
    }


    public class SnippetSaveResult
    {
        public SnippetWrongField validity = SnippetWrongField.OK;
        public long id = -1;
        public int targetGroupId = -1;
        public string errorMsg = string.Empty;
    }

    public class SnippetSearchResult
    {
        public int snippetNum = 0;
        public string errorMsg = string.Empty;
    }

    public class SnipInfo
    {
        public System.Drawing.Image snip_image = null;
        public string snip_name = string.Empty;
        public string snip_description = string.Empty;
        public string snip_creator = string.Empty;
        public long snip_ID = -1;
    }
}
