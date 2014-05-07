//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using System.Configuration;
using System.Web;
//using System.Web.Configuration;
//using System.Web.Security;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using log4net;
using log4net.Config;

namespace Snip2Code.Utils
{
    /// <summary>
    /// This class provides some useful methods to read the configuration of the app (from Web.config or App.config).
    /// It should be extended by a concrete class in order to be instantiated as a singleton.
    /// In the private constructor the method Init() should be invoked.
    /// </summary>
    public abstract class ConfigReader
    {
        #region PROTECTED PARAMETERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string m_lastErrorMessage = string.Empty;

        protected XDocument m_customConfig = null;  //custom config XML file
        protected XDocument m_mimeTypes = null;  //myme type doc
        // protected string m_appPath = string.Empty;
        protected string m_proto = string.Empty;
        protected string m_name = string.Empty;
        protected string m_apiProto = string.Empty;
        protected string m_apiName = string.Empty;
        //protected string m_root = string.Empty;
        // protected string m_configuration = string.Empty; 

        // protected string m_logPath;
        // protected int m_keepMeLoggedInDays = 14;
        protected bool m_logWarnings = false;
        protected bool m_logMessages = false;

        protected string m_rootLocalPath;

        private static XmlNamespaceManager m_namespaceManager = null;
        
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region PUBLIC PROPERTIES
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public string LastError { get { return m_lastErrorMessage;  } }

        public static XNamespace S2CNS { get { return "http://www.snip2code.com/ns/snip"; } }
        public const string S2CPrefix = "s2c";

        public static XmlNamespaceManager S2CNSManager
        {
            get
            {
                if (m_namespaceManager == null)
                {
                    m_namespaceManager = new XmlNamespaceManager(new NameTable());
                    m_namespaceManager.AddNamespace(ConfigReader.S2CPrefix, ConfigReader.S2CNS.NamespaceName);
                }
                return m_namespaceManager;
            }
        }

        // public string AppPath { get { return m_appPath; } }

        /// <summary>
        /// This is the public path of Snip2Code service, i.e. http://www.snip2code.com/
        /// Please note the string ends with the '/'
        /// </summary>
        public string PublicPath { get { return Protocol + Name + "/"; } }
        public string Protocol { get { return m_proto + "://"; } }
        public string Name { get { return m_name; } }

        /// <summary>
        /// This is the public path of Snip2Code API service, i.e. http://api.snip2code.com/
        /// Please note the string ends with the '/'
        /// </summary>
        public string ApiPublicPath { get { return ApiProtocol + ApiName + "/"; } }
        public string ApiProtocol { get { return m_apiProto + "://"; } }
        public string ApiName { get { return m_apiName; } }
        //public string Root { get { return m_root; } }
        // public string Configuration { get { return m_configuration; } }

        // public string VersionString { get { return String.Format("{0}.{1}.{2}", m_majorVer, m_minoVer, m_buildNum); } }
        // public int KeepMeLoggedInDays { get { return m_keepMeLoggedInDays; } }
        public bool LogWarnings { get { return m_logWarnings; } }
        public bool LogMessages { get { return m_logMessages; } }
        public string SitePath { get { return m_rootLocalPath; } }
        
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors

        protected ConfigReader() 
        {
        }

        public virtual bool Initialize(string rootPath)
        {
            m_rootLocalPath = rootPath;

            //Load custom config file if it is available:
            LoadCustomConfigXMLFile();

            // GetSetting("AppPath", ref m_appPath);
            GetSetting("Proto", ref m_proto);
            GetSetting("Server", ref m_name);
            GetSetting("ApiProto", ref m_apiProto);
            GetSetting("ApiServer", ref m_apiName);
            //GetSetting("Root", ref m_root);
            // GetSetting("Configuration", ref m_configuration);
            // GetSetting("MajorVersion", ref m_majorVer);
            // GetSetting("keepMeLoggedInDays", ref m_keepMeLoggedInDays);

            /// TODO: This should be removed....
            /*
            if (!GetSetting("LogPath", out m_logPath))
            {
                m_lastErrorMessage = "";
                return false;
            }*/

            if ( !GetSetting("LogWarning", ref m_logWarnings) )
                m_logWarnings = false;
            if ( !GetSetting("LogMessages", ref m_logMessages) )
                m_logMessages = false;

            // load mime type document
            Load_MimeTypeXML();

            // Log4Net initialization:
            FileInfo log4netFile = new FileInfo(string.Format("{0}\\Snip2Code.config", rootPath));
            if (log4netFile.Exists)
                XmlConfigurator.Configure(log4netFile);
            else
                XmlConfigurator.Configure();
            ILog log = log4net.LogManager.GetLogger(this.GetType());

            log.Info("Log4net correctly started");

            return true;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Public API

        public bool SetProperty(string propName, string proValue)
        {
            return false;
        }

        public string GetSetting(string propName)
        {
            try
            {
                if (ConfigurationManager.AppSettings.Get(propName) != null)
                    return ConfigurationManager.AppSettings.Get(propName).ToString();
                //else if (WebConfigurationManager.AppSettings.Get(propName) != null)
                //    return WebConfigurationManager.AppSettings.Get(propName).ToString();
                else if (m_customConfig != null)
                {
                    string res = string.Empty;
                    m_customConfig.Root.ParseNode(propName, ref res);
                    return res;
                }
                else
                    return string.Empty;
            }
            catch { }

            return string.Empty;
        }

        protected bool GetSetting(string propName, ref string property)
        {
            // property = string.Empty;
            try
            {
                if (ConfigurationManager.AppSettings.Get(propName) != null)
                    property = ConfigurationManager.AppSettings.Get(propName).ToString();
                //else if (WebConfigurationManager.AppSettings.Get(propName) != null)
                //    property = WebConfigurationManager.AppSettings.Get(propName).ToString();
                else if (m_customConfig != null)
                    m_customConfig.Root.ParseNode(propName, ref property);
                else
                    return false;
                return true;
            }
            catch 
            {
                return false;
            }
        }

        protected bool GetSetting(string propName, ref bool property)
        {
            string prop = string.Empty;
            if ( GetSetting(propName, ref prop) )
                return bool.TryParse(prop, out property);
            else
                return false;
        }

        protected bool GetSetting(string propName, ref int property)
        {
            string prop = string.Empty;
            if (GetSetting(propName, ref prop))
                return int.TryParse(prop, out property);
            else
                return false;
        }

        protected bool GetSetting<T>(string propName, ref T property)
        {
            string prop = string.Empty;
            if (GetSetting(propName, ref prop))
            {
                try
                {
                    property = (T)Enum.Parse(typeof(T), prop, true);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            else
                return false;
        }
        #endregion

        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Protected internal utility functions

        /// <summary>
        /// Load in memory the xml describing the configuration, if any
        /// </summary>
        /// ------------------------------------------------------------------------------------------
        protected bool LoadCustomConfigXMLFile()
        {
            string path = string.Format("{0}\\Snip2Code.config", m_rootLocalPath);
            try
            {
                m_customConfig = XDocument.Load(path);
            }
            catch
            {
                // error loading document: Not found
                m_customConfig = null;
                return false;
            }

            return true;
        }


        /// <summary>
        /// Load in memory the xml describing the mime types
        /// </summary>
        /// ------------------------------------------------------------------------------------------
        protected bool Load_MimeTypeXML()
        {
            string path = GetSetting("MimeTypesXML_Path");
            if (string.IsNullOrEmpty(path))
            {
                m_mimeTypes = null;
                return false;
            }

            try
            {
                m_mimeTypes = XDocument.Load(string.Format("{0}\\S2CMimeTypes.xml", path));
            }
            catch
            {
                // error loading document
                m_mimeTypes = null;
                return false;
            }

            return true;
        }


        /// <summary>
        /// Retrieve the properties for a given mime type
        /// </summary>
        /// <param name="mimeTypeName"></param>
        /// <param name="itemContent"></param>
        /// <param name="itemExtension"></param>
        /// ------------------------------------------------------------------------------------------
        protected void Find_MymeTypeProperties(string mimeTypeName, out string itemContent, out string itemExtension)
        {
            itemContent = string.Empty;
            itemExtension = string.Empty;

            if (m_mimeTypes == null)
            {
                if (!Load_MimeTypeXML())
                    return;
            }

            if (m_mimeTypes == null)
                return;

            //find the mime type from the name
            m_mimeTypes.ParseNode(string.Format("mimeTypes/mimeType[@name='{0}']/itemContent", mimeTypeName), ref itemContent);
            m_mimeTypes.ParseNode(string.Format("mimeTypes/mimeType[@name='{0}']/extension", mimeTypeName), ref itemExtension);
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
