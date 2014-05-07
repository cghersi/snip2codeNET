//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Configuration;

namespace Snip2Code.Utils
{
    public class AppConfig : ConfigReader
    {

        #region PRIVATE STATIC PARAMETERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        static private AppConfig s_current = null;
        static private object s_ObjSync = new object();

        private string m_siteBasePath = "http://localhost:1010/";
        private string m_signupPage = "Access/Signup";
        private string m_retrievePwdPage = "Access/RetrievePassword";
        private string m_contactUsPage = "Static/ContactUs";
        private string m_contactEmail = "info@snip2code.com";
        private string m_s2cHeader = EntityUtils.S2C_HEADER_VS;
        private int m_loginrefreshTimeSec = 800;
        private int m_searchHistoryNumItemToDisplay = 5;
        private int m_searchHistoryNumItemToStore = 50;

        private SerialFormat m_serialFormat = SerialFormat.JSON;
        
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region PUBLIC SERVER PROPERTIES
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public string SiteBasePath { get { return m_siteBasePath; } }
        public string SignupPage { get { return m_signupPage; } }
        public string RetrievePwdPage { get { return m_retrievePwdPage; } }
        public string ContactUsPage { get { return m_contactUsPage; } }
        public string ContactEmail { get { return m_contactEmail; } }
        public string S2CHeader { get { return m_s2cHeader; } }
        public int LoginRefreshTimeSec { get { return m_loginrefreshTimeSec; } }
        public int SearchHistoryNumItemToDisplay { get { return m_searchHistoryNumItemToDisplay; } }
        public int SearchHistoryNumItemToStore { get { return m_searchHistoryNumItemToStore; } }
        public SerialFormat SerialFormat { get { return m_serialFormat; } }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///     This return the one and only config reader (this is a singleton)
        /// </summary>
        static public AppConfig Current 
        {
            get { 
                if (s_current == null)            
                    lock (s_ObjSync)  {
                        if (s_current == null)
                            s_current = new AppConfig();
                    }
                return s_current;
            }
        }


        protected AppConfig() : base() {  }

        public override bool Initialize(string rootPath)
        {
            if (base.Initialize(rootPath))
            {
                GetSetting("SiteBasePath", ref m_siteBasePath);
                GetSetting("SignupPage", ref m_signupPage);
                GetSetting("RetrievePwdPage", ref m_retrievePwdPage);
                GetSetting("ContactUsPage", ref m_contactUsPage);
                GetSetting("ContactEmail", ref m_contactEmail);
                GetSetting("S2CHeader", ref m_s2cHeader);
                GetSetting("LoginrefreshTimeSec", ref m_loginrefreshTimeSec);
                GetSetting("SearchHistoryNumItemToDisplay", ref m_searchHistoryNumItemToDisplay);
                GetSetting("SearchHistoryNumItemToStore", ref m_searchHistoryNumItemToStore);
                GetSetting<SerialFormat>("SerialFormat", ref m_serialFormat);

                return true;
            }
            return false;
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
