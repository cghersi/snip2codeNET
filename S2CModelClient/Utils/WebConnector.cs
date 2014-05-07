#define USE_OAUTH

//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
using System.Net;
using System.IO;

using Snip2Code.Model.Entities;
using Snip2Code.Model.Comm;
using Snip2Code.Model.Client.Entities;
using Snip2Code.Model.Client.Utils;
using OAuth2.Mvc;
using Snip2Code.Model.Client.WSFramework;

namespace Snip2Code.Utils
{
    public class WebConnector
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region PRIVATE MEMBERS
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        static private WebConnector s_current = null;
        static private object s_ObjSync = new object();

        private CookieContainer m_cookies = new CookieContainer();    //cookie container ready to receive the forms auth cookie
        private string m_accessToken = null;
        private DateTime m_expirationAccessToken = DateTime.MinValue;
        private SerialFormat m_requestFormat = SerialFormat.JSON;

        public const string OAUTH_LOGIN_URL = "OAuthUser/GetUser";
        public const string LOGIN_URL = "Access/Login";
        public const string LOGOUT_URL = "Access/Logout";
        private const string OAUTH_ONEALL_LOGIN_URL = "OAuthUser/LoginWithOneAll";
        private const string LOGIN_ONEALL_URL = "User/LoginWithOneAll";
        public const string PING_URL = "Access/Ping";
        private const string USERNAME_TEXTBOX_NAME = "Username"; 
        private const string PASSWORD_TEXTBOX_NAME = "Password"; 
        private const string LOGIN_BUTTON_NAME = "LoginBtn"; 
        private const string LOGIN_BUTTON_VALUE = "Login"; 
        private const bool NEED_VIEW_STATE = false;

        private const String TIMEOUT_RESP = "TIMEOUT";

        private bool m_isLogged = false;

        protected static fastJSON.JSON jsonUtils = EntityUtils.InitJSONUtils();

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region PROPERTIES
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Tells whether the current user is logged in AND the access token is still valid 
        /// (if using OAUTH authentication)
        /// </summary>
        public bool IsLogged {
            get 
            { 
#if USE_OAUTH
                return m_isLogged && !AccessTokenInvalid(); 
#else
                return m_isLogged; 
#endif
            }
        }

        /// <summary>
        /// Tells whether the current user is logged in from client side.
        /// Note: This doesn't take into account the validity of the acces token (if using OAUTH authentication)
        /// </summary>
        public bool SeemsLogged { get { return m_isLogged; } }

        public bool IsTimeout { get; private set; }
        //public SerialFormat RequestFormat { get { return m_requestFormat; } set { m_requestFormat = value; } }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///     This return the one and only web connector (this is a singleton)
        /// </summary>
        /// 
        static public WebConnector Current
        {
            get
            {
                if (s_current == null)
                {
                    lock (s_ObjSync)
                    {
                        if (s_current == null)
                            s_current = new WebConnector();
                    }
                }
                return s_current;
            }
        }


        private WebConnector()
        {
            m_isLogged = false;
            m_requestFormat = AppConfig.Current.SerialFormat;
        }


        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Public API
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Reset the status of authentication, force the user to re-perform the login process
        /// </summary>
        public void ResetLoginStatus()
        {
            m_accessToken = "";
            m_expirationAccessToken = DateTime.MinValue;
            m_isLogged = false;
            m_cookies = new CookieContainer();
        }

        /// <summary>
        /// Perform a logout operation on the server for the current user
        /// </summary>
        /// <returns></returns>
        public bool Logout()
        {
            string logoutUrl = string.Format("{0}{1}", BaseWS.Server, LOGOUT_URL);
            string response = SendGetRequest(logoutUrl, true);
            if (string.IsNullOrEmpty(response))
                return false;

            S2CResObj<object> resp = S2CSerializer.DeserializeObj(response, m_requestFormat);
            if ((resp != null) && (resp.Status == ErrorCodes.OK))
            {
                ResetLoginStatus();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Login into the web server in order to be ready to send web service requests
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public UserClient Login(string username, string password)
        {
            string viewStateString = string.Empty;

#if NEED_VIEW_STATE
                // first, request the login form to get the viewstate value
                HttpWebRequest viewStateRequest = WebRequest.Create(loginUrl) as HttpWebRequest;
                viewStateRequest.Accept = "application/xhtml+xml";

                StreamReader responseReader = new StreamReader(viewStateRequest.GetResponse().GetResponseStream());
                string responseData = responseReader.ReadToEnd();
                responseReader.Close();

                // extract the viewstate value and build out POST data
                string viewState = ExtractViewState(responseData);
                if (viewState != string.Empty)
                    viewStateString = string.Format("__VIEWSTATE={0}&", viewState);
#endif
            string encryptedPsw = password.ComputeMD5HashSQLServerCompliant();
            string postData = string.Format("{0}{1}={2}&{3}={4}&{5}={6}",
                viewStateString, 
                USERNAME_TEXTBOX_NAME, HttpUtility.UrlEncode(username),
                PASSWORD_TEXTBOX_NAME, HttpUtility.UrlEncode(encryptedPsw), 
                LOGIN_BUTTON_NAME, LOGIN_BUTTON_VALUE);

            // now post to the login form
#if USE_OAUTH
            //check the validity of Access Token:
            if (AccessTokenInvalid())
            {
                //maybe in the future we can also refresh the token instead of creating a new one after the expiration...
                if (!ReceiveAccessToken(username, encryptedPsw))
                    return new UserClient(false);
            }

            string loginUrl = string.Format("{0}{1}?{2}", BaseWS.Server, OAUTH_LOGIN_URL, postData);
            string response = SendGetRequest(loginUrl, true);
#else
            string loginUrl = string.Format("{0}{1}", BaseWS.Server, LOGIN_URL);
            string response = SendPostRequest(loginUrl, postData, false);
#endif

            S2CResBaseEntity<UserClient> resp = null;
            if (string.IsNullOrEmpty(response))
                resp = new S2CResBaseEntity<UserClient>(-1, ErrorCodes.COMMUNICATION_ERROR, null);
            else
                resp = S2CSerializer.DeserializeBaseEntity<UserClient>(response, m_requestFormat);

            //check that the authentication request has been correctly received:
            m_isLogged = ((m_cookies != null) && (m_cookies.Count > 0) && (resp != null) && 
                        (resp.Status == ErrorCodes.OK) && (resp.Data != null));

            if ((resp != null) && (resp.Status == ErrorCodes.FAIL))
                return new UserClient(false);

            if (resp == null)
                return null;
            else
                return resp.Data;
        }


        /// <summary>
        /// Login into the web server using OneAll system in order to be ready to send web service requests
        /// </summary>
        /// <param name="identToken"></param>
        public UserClient LoginWithOneAll(string identToken)
        {
            if (identToken.IsNullOrWhiteSpaceOrEOF())
                return new UserClient(false);

            string postData = "identToken=" + identToken;

            // now post to the login form
#if USE_OAUTH
            //check the validity of Access Token:
            if (AccessTokenInvalid())
            {
                //maybe in the future we can also refresh the token instead of creating a new one after the expiration...
                if (!ReceiveAccessToken(User.ONEALL_FAKE_USERNAME, identToken))
                    return new UserClient(false);
            }

            string loginUrl = string.Format("{0}{1}?{2}", BaseWS.Server, OAUTH_ONEALL_LOGIN_URL, postData);
            string response = SendGetRequest(loginUrl, true);
#else
            string loginUrl = string.Format("{0}{1}", BaseWS.Server, LOGIN_ONEALL_URL);
            string response = SendPostRequest(loginUrl, postData, false);
#endif

            S2CResBaseEntity<UserClient> resp = null;
            if (string.IsNullOrEmpty(response))
                resp = new S2CResBaseEntity<UserClient>(-1, ErrorCodes.COMMUNICATION_ERROR, null);
            else
                resp = S2CSerializer.DeserializeBaseEntity<UserClient>(response, m_requestFormat);

            //check that the authentication request has been correctly received:
            m_isLogged = ((m_cookies != null) && (m_cookies.Count > 0) && (resp != null) &&
                        (resp.Status == ErrorCodes.OK) && (resp.Data != null));

            if ((resp != null) && (resp.Status == ErrorCodes.FAIL))
                return new UserClient(false);

            if (resp == null)
                return null;
            else
                return resp.Data;
        }


        private bool AccessTokenInvalid()
        {
            return (string.IsNullOrEmpty(m_accessToken) || (m_expirationAccessToken <= DateTime.Now));
        }

        private bool ReceiveAccessToken(string username, string password)
        {
            // 1) Request Token:
            string requestTokenData = SendGetRequest(BaseWS.Server + "OAuth/RequestToken", false);
            if (string.IsNullOrEmpty(requestTokenData))
            {
                log.Error("Cannot retrieve request Token");
                return false;
            }

            OAuthResponse resp = null;
            try
            {
                resp = jsonUtils.ToObject<OAuthResponse>(requestTokenData);
            }
            catch (Exception e)
            {
                log.ErrorFormat("Cannot parse from json string:{0} due to {1}", requestTokenData.PrintNull(), e);
                return false;
            }
            string reqToken = (resp == null ? null : resp.RequestToken);
            if (string.IsNullOrEmpty(reqToken))
            {
                log.Error("Cannot retrieve request Token");
                return false;
            }

            // 2) Access Token:
            string accessTokenData = SendPostRequest(BaseWS.Server + "OAuth/AccessToken",
                    "oauth_token=" + reqToken + "&grant_type=unused&username=" + username + "&password=" + password, false);
            if (string.IsNullOrEmpty(accessTokenData))
            {
                log.Error("Cannot retrieve Access Token");
                return false;
            }

            resp = null; //re-init resp
            try
            {
                resp = jsonUtils.ToObject<OAuthResponse>(accessTokenData);
            }
            catch (Exception e)
            {
                log.ErrorFormat("Cannot parse from json string:{0} due to {1}", accessTokenData.PrintNull(), e);
                return false;
            }
            if (resp == null)
            {
                log.Error("Cannot retrieve Access Token");
                return false;
            }

            m_accessToken = resp.AccessToken;
            if (resp.Expires > 0)
                m_expirationAccessToken = DateTime.Now.AddSeconds(resp.Expires);
            else
            {
                log.Error("Cannot retrieve Expiration of Access Token");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sends a POST request to the given service url with the given data
        /// </summary>
        /// <param name="serviceUrl">URL of the service complete of querystring parameters</param>
        /// <param name="addAccessToken">true if the request should be authenticated</param>
        /// <param name="data"></param>
        /// <returns>string.Empty if any error occurred, the server response otherwise</returns>
        public string SendPostRequest(string serviceUrl, string data, bool addAccessToken)
        {
            return SendRequest(serviceUrl, data, true, addAccessToken);
        }

        /// <summary>
        /// Sends a GET request to the given service url
        /// </summary>
        /// <param name="serviceUrl">URL of the service complete of querystring parameters</param>
        /// <param name="addAccessToken">true if the request should be authenticated</param>
        /// <returns></returns>
        public string SendGetRequest(string serviceUrl, bool addAccessToken)
        {
            return SendRequest(serviceUrl, string.Empty, false, addAccessToken); 
        }


        /// <summary>
        /// Detects whether the Snip2Code web service is alive or not
        /// </summary>
        /// <returns></returns>
        public bool PingS2CServer()
        {
            return PingS2CServer(BaseWS.Server + PING_URL);
        }


        /// <summary>
        /// Detects whether the Snip2Code web service is alive or not
        /// </summary>
        /// <returns></returns>
        public bool PingS2CServer(string pingUrl)
        {
            bool res = false;
            using (S2cWebClient client = new S2cWebClient(10000, this.m_requestFormat))
            {
                try
                {
                    string retStr = client.DownloadString(pingUrl);
                    S2CResObj<object> boolResp = S2CSerializer.DeserializeObj(retStr, this.m_requestFormat);
                    if ((boolResp != null) && (boolResp.Status == ErrorCodes.OK))
                    {
                        if (!bool.TryParse(boolResp.Data.ToString(), out res))
                            res = false;
                    }        
                }
                catch (Exception e)
                {
                    //log.DebugFormat("Cannot connect due to {0}; response is {1}", e, (response == null ? "null" : response.StatusCode.ToString()));
                    log.DebugFormat("Cannot connect due to {0}", e);
                }
            }
            log.Debug("CheckConnections result=" + res);

            return res;
        }


        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Private Utilities
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Sends a request to the given service url and returns a byte[] as response
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="data"></param>
        /// <param name="isPost">true to send a POST request, false to send a GET request</param>
        /// <param name="addAccessToken">true if the request should be authenticated</param>
        /// <returns>empty byte array if any error occurred, the server response otherwise</returns>
        public byte[] SendByteRequest(string serviceUrl, string data, bool isPost, bool addAccessToken)
        {
            // send out cookie along with a request for the protected page
            HttpWebRequest webRequest = null;
            try
            {
                webRequest = WebRequest.Create(serviceUrl) as HttpWebRequest;
            }
            catch (UriFormatException ufe)
            {
                log.ErrorFormat("Cannot connect to {0} due to {1} ", serviceUrl, ufe.Message);
                return new byte[0];
            }

            if (isPost)
            {
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
            }
            webRequest.Accept = m_requestFormat.GetMIMEType();
            webRequest.CookieContainer = m_cookies;
#if USE_OAUTH
            if (addAccessToken)
            {
                if (m_accessToken != null)
                {
                    Cookie authCookie = new Cookie("oauth_token", m_accessToken, "/");
                    webRequest.CookieContainer.Add(new Uri(BaseWS.Server), authCookie);
                }
            }
#endif
            webRequest.Headers.Add(EntityUtils.S2C_HEADER, AppConfig.Current.S2CHeader);
            if (isPost && (data != null))
                webRequest.ContentLength = data.Length;

            byte[] responseData = new byte[0];
            try
            {
                if (isPost)
                {
                    // write the form values into the request message
                    StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
                    requestWriter.Write(data);
                    requestWriter.Close();
                }

                //read the response:
                Stream resp = webRequest.GetResponse().GetResponseStream();
                long len = resp.Length;
                responseData = new byte[len];
                resp.Read(responseData, 0, (int)len);
            }
            catch (WebException wex)
            {
                log.ErrorFormat("Cannot connect to {0} due to {1} ", serviceUrl, wex.Message);
                return new byte[0];
            }
            catch (IOException ioex)
            {
                log.ErrorFormat("Cannot read data from to {0} due to {1} ", serviceUrl, ioex.Message);
                return new byte[0];
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Cannot read data from to {0} due to {1} ", serviceUrl, ex.Message);
                return new byte[0];
            }

            return responseData;
        }


        /// <summary>
        /// Sends a request to the given service url
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="data"></param>
        /// <param name="isPost">true to send a POST request, false to send a GET request</param>
        /// <param name="addAccessToken">true if the request should be authenticated</param>
        /// <returns>string.Empty if any error occurred, the server response otherwise</returns>
        private string SendRequest(string serviceUrl, string data, bool isPost, bool addAccessToken)
        {
            IsTimeout = false; //reset timeout status flag

            HttpWebRequest webRequest = null;
            log.DebugFormat("Creating request {0}", serviceUrl);
            try
            {
                webRequest = WebRequest.Create(serviceUrl) as HttpWebRequest;
            }
            catch (UriFormatException ufe)
            {
                log.ErrorFormat("Cannot connect to {0} due to {1} ", serviceUrl, ufe.Message);
                return string.Empty;
            }

            if (isPost)
            {
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
            }

            webRequest.Accept = m_requestFormat.GetMIMEType();
            webRequest.ServicePoint.ConnectionLimit = 10;
            webRequest.ServicePoint.Expect100Continue = false; //this would prevent waiting infinite time for this HTTP 100 CONTINUE (unuseful)
            webRequest.CookieContainer = m_cookies;             // send out cookie along with a request for the protected page
#if USE_OAUTH
            if (addAccessToken)
            {
                if (m_accessToken != null)
                {
                    Cookie authCookie = new Cookie("oauth_token", m_accessToken, "/");
                    webRequest.CookieContainer.Add(new Uri(BaseWS.Server), authCookie);
                }
            }
#endif
            webRequest.Headers.Add(EntityUtils.S2C_HEADER, AppConfig.Current.S2CHeader);
            if (isPost && (data != null))
                webRequest.ContentLength = data.Length;

            StreamReader responseReader = null;
            string responseData = string.Empty;
            WebResponse webResp = null;
            try
            {
                if (isPost)
                {
                    log.DebugFormat("Posting request {0}", serviceUrl);

                    // write the form values into the request message
                    StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
                    requestWriter.Write(data);
                    requestWriter.Flush();
                    requestWriter.Close();
                    requestWriter.Dispose();
                }

                log.DebugFormat("Number of active connections: {0}", webRequest.ServicePoint.CurrentConnections);
                log.DebugFormat("Getting web response for request {0}", serviceUrl);
                webResp = webRequest.GetResponse();
                log.DebugFormat("Getting web response stream for request {0}", serviceUrl);
                Stream webStream = webResp.GetResponseStream();
                log.DebugFormat("Creating reader for request {0}", serviceUrl);
                responseReader = new StreamReader(webStream);

                //read the response:
                log.DebugFormat("Reading response for request {0}", serviceUrl);
                responseData = responseReader.ReadToEnd();
            }
            catch (WebException wex)
            {
                log.ErrorFormat("Cannot connect to {0} due to {1} ", serviceUrl, wex.Message);
                if ((webRequest != null) && (webRequest.ServicePoint != null))
                    log.ErrorFormat("Number of active connections: {0}", webRequest.ServicePoint.CurrentConnections);
                IsTimeout = (wex.Status == WebExceptionStatus.Timeout);
                return string.Empty;
            }
            catch (IOException ioex)
            {
                log.ErrorFormat("Cannot read data from to {0} due to {1} ", serviceUrl, ioex.Message);
                if ((webRequest != null) && (webRequest.ServicePoint != null))
                    log.ErrorFormat("Number of active connections: {0}", webRequest.ServicePoint.CurrentConnections);
                return string.Empty;
            }
            finally
            {
                if (responseReader != null)
                {
                    responseReader.Close();
                    responseReader.Dispose();
                }
                if (webResp != null)
                    webResp.Close();
                webRequest = null;
            }

            log.DebugFormat("Response read for request {0}", serviceUrl);

            return responseData;
        }


        private string ExtractViewState(string s)
        {
            string viewStateNameDelimiter = "__VIEWSTATE";
            string valueDelimiter = "value=\"";

            int viewStateNamePos = s.IndexOf(viewStateNameDelimiter);
            if (viewStateNamePos < 0)
                return string.Empty;

            int viewStatePos = s.IndexOf(valueDelimiter, viewStateNamePos);

            int viewStateStartPos = viewStatePos + valueDelimiter.Length;
            int viewStateEndPos = s.IndexOf("\"", viewStateStartPos);

            return HttpUtility.UrlEncodeUnicode(s.Substring(viewStateStartPos, viewStateEndPos - viewStateStartPos));
        }


        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
