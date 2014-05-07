//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Threading;

using Snip2Code.Model.Client.WSFramework;
using Snip2Code.Model.Entities;

namespace Snip2Code.Utils
{
    public class LoginPoller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool RecallIsLogged { get { return WebConnector.Current.IsLogged; } }
        
        private string username = "";
        private string password = "";
        private string identToken = "";
        private bool useOneAll = false;
        private IPluginManager pluginManager = null;

        private static UserWS m_userRepo = new UserWS();

        public LoginPoller(string username, string password, string identToken, bool useOneAll, IPluginManager pluginManager) 
        {
            this.username = username;
            this.password = password;
            this.identToken = identToken;
            this.useOneAll = useOneAll;
            this.pluginManager = pluginManager;
        }

        // This method is called by the timer delegate.
        public void RecallLogin(Object stateInfo)
        {
            log.DebugFormat("Calling Login from Poller; LoginRefreshTimeSec={0}", AppConfig.Current.LoginRefreshTimeSec);
            User user = null;
            if (useOneAll)
                user = m_userRepo.LoginUserOneAll(identToken);
            else
                user = m_userRepo.LoginUser(username, password);

            log.DebugFormat("Called Login from Poller: user is {0}", user.PrintNull());
            if ((user != null) && user.IsValid && (user.ID > 0))
                pluginManager.DisplayLoggedInButtons();
            else
                pluginManager.DisplayLoggedOutButtons();           
        }

     /*   public void RecallLogout(Object stateInfo)
        {
            WebConnector.Current.Logout();          
        }*/
        
    }
}
