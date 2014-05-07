//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System.Collections.Generic;

using Snip2Code.Utils;
using Snip2Code.Model.Entities;
using Snip2Code.Model.Client.Entities;
using System;

namespace Snip2Code.Model.Client.WSFramework
{
    public abstract class BaseWS
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const int MAX_LOGIN_ATTEMPT = 3;
        protected SerialFormat m_serialFormat = SerialFormat.JSON;
        protected AppConfig m_config;
        private ErrorCodes m_lastErrorCode = ErrorCodes.OK;  

        protected const string OAUTH_PREFIX_ACTION = "OAuth";
        public const bool USE_OAUTH = true;

        private string m_lastError = string.Empty;

        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected BaseWS()
        {
            //init environment (e.g log4net, web service communication, etc.)...
            m_config = AppConfig.Current;
            m_serialFormat = m_config.SerialFormat;
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region PROPERTIES
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string Username { get; set; }
        public static string Password { get; set; }
        public static string Server { get; set; }
        public static UserClient CurrentUser { get; protected set; }

        public int CurrentUserID
        {
            get { return (CurrentUser == null ? -1 : CurrentUser.ID); }
            set { throw new NotImplementedException("This operation cannot be done client-side!!"); }
        }

        private static bool useOneAll = false;
        public static bool UseOneAll { 
            get { return useOneAll; }
            set { useOneAll = value; }
        }

        public static string IdentToken { get; set; }

        public string LastErrorMsg { get { return m_lastError; } }
        public ErrorCodes LastErrorCode { get { return m_lastErrorCode; } }

        protected void SetLastError(log4net.ILog logger, ErrorCodes errorCode, string msg)
        {
            if (msg == null)
                m_lastError = string.Empty;
            else
                m_lastError = msg;
            m_lastErrorCode = errorCode;
            logger.Error(m_lastError);
        }

        public SerialFormat SerialFrmt { get; set; }

        public static void Logout()
        {
            WebConnector.Current.Logout();
            
            Username = string.Empty;
            Password = string.Empty;
            IdentToken = string.Empty;
            CurrentUser = null;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Protected Utilities
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Checks that the client has been already logged in, and if this is not the case, automatically logins
        /// </summary>
        /// <returns>true if the login has been successful; false otherwise</returns>
        protected bool CheckLogin(bool forceReLogin = false)
        {
            int count = 1;
            if (forceReLogin)
                WebConnector.Current.ResetLoginStatus();

            while (!WebConnector.Current.IsLogged)
            {
                log.Info("Trying to connect to web server..." + count);

                // Login to site and acquire a valid session
                UserClient loginResp = null;
                if (useOneAll && !IdentToken.IsNullOrWhiteSpaceOrEOF())
                    loginResp = WebConnector.Current.LoginWithOneAll(IdentToken);
                else
                    loginResp = WebConnector.Current.Login(Username, Password);

                //check for timeout:
                if (WebConnector.Current.IsTimeout)
                {
                    SetLastError(log, ErrorCodes.TIMEOUT, "Web server doesn't reply to the requests");
                    return false;
                }

                //check for invalid credentials:
                if ((loginResp != null) && !loginResp.IsValid)
                {
                    SetLastError(log, ErrorCodes.NOT_LOGGED_IN, "Login has retrieved an invalid user");
                    CurrentUser = null;
                    WebConnector.Current.ResetLoginStatus();
                    return false;
                }

                //try to retrieve the current user:
                if (WebConnector.Current.IsLogged && (loginResp != null))
                {
                    CurrentUser = loginResp;

                    //retrieve user groups:
                    //this is very important in order to preload user groups!!
                    //don't remove these lines!!!!
                    log.InfoFormat("Retrieved groups: {0}", CurrentUser.Groups.Print());
                    log.InfoFormat("Retrieved administered groups: {0}", CurrentUser.AdministeredGroups.Print());
                    
                }

                if (count++ > MAX_LOGIN_ATTEMPT)
                {
                    SetLastError(log, ErrorCodes.COMMUNICATION_ERROR, "Cannot login into the web server");
                    return false;
                }
            }

            return true;
        }


        private string PrepareAndSendReq(string action, string data, bool isPost, bool requiresLogin)
        {
            //Check that the client has been already logged in, and if this is not the case, automatically login:
            if (requiresLogin && !CheckLogin())
            {
                //reset login status:
                WebConnector.Current.ResetLoginStatus();

                //retry the login:
                if (!CheckLogin())
                {
                    SetLastError(log, ErrorCodes.NOT_LOGGED_IN, "Cannot login into the system");
                    return null;
                }
            }

            //send the request and parse the response:
            if (isPost)
            {
                string reqPath = string.Format("{0}{1}", Server, action);
                return WebConnector.Current.SendPostRequest(reqPath, data, requiresLogin);
            }
            else
            {
                string reqPath = string.Format("{0}{1}?{2}", Server, action, data);
                return WebConnector.Current.SendGetRequest(reqPath, requiresLogin);
            }
        }


        #region Send Request Utilities
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Sends an HTTP Request to the backend and parse the response (expected as a BaseEntity object)
        /// </summary>
        /// <param name="action">action to query (e.g. "Snippets/Get")</param>
        /// <param name="data">
        ///     if the request is a GET, the parameters to be put in the querystring (e.g. "snippetID=100"),
        ///     otherwise data to be put in the post (e.g. "content=...")
        /// </param>
        /// <param name="isPost">whether this request is a GET(false) or a POST(true)</param>
        /// <param name="requiresLogin">false if this request calls a public web service</param>
        /// <returns>null if any error occurred; the result of the invocation otherwise</returns>
        protected S2CResBaseEntity<T> SendReqBaseEntity<T>(string action, string data, bool isPost, bool requiresLogin = true)
            where T : BaseEntity, new()
        {
            string response = PrepareAndSendReq(action, data, isPost, requiresLogin);
            if (string.IsNullOrEmpty(response))
            {
                ErrorCodes errCode = ErrorCodes.COMMUNICATION_ERROR;
                if (WebConnector.Current.IsTimeout)
                    errCode = ErrorCodes.TIMEOUT;
                SetLastError(log, errCode, S2CRes<string>.GetErrorMsg(errCode));
                return new S2CResBaseEntity<T>(0.0, errCode, null);
            }

            S2CResBaseEntity<T> resp = S2CSerializer.DeserializeBaseEntity<T>(response, m_serialFormat);
            if (!CheckResp<T>(resp))
            {
                PrintRespError<T>(resp);
            
        	    //if the problem is related to user not logged in, reset login status and retry another time:
                if (requiresLogin && (resp != null) && (resp.Status == ErrorCodes.NOT_LOGGED_IN))
                {
                    WebConnector.Current.ResetLoginStatus(); //reset login status

                    //retry the WS call:
                    response = PrepareAndSendReq(action, data, isPost, requiresLogin);
                    resp = S2CSerializer.DeserializeBaseEntity<T>(response, m_serialFormat);
                }
            }

            return resp;
        }


        /// <summary>
        /// Sends an HTTP Request to the backend and parse the response (expected as a list of BaseEntity objects)
        /// </summary>
        /// <param name="action">action to query (e.g. "Snippets/Get")</param>
        /// <param name="data">
        ///     if the request is a GET, the parameters to be put in the querystring (e.g. "snippetID=100"),
        ///     otherwise data to be put in the post (e.g. "content=...")
        /// </param>
        /// <param name="isPost">whether this request is a GET(false) or a POST(true)</param>
        /// <param name="requiresLogin">false if this request calls a public web service</param>
        /// <returns>null if any error occurred; the result of the invocation otherwise</returns>
        protected S2CResListBaseEntity<T> SendReqListBaseEntity<T>(string action, string data, bool isPost, bool requiresLogin = true)
            where T : BaseEntity, new()
        {
            string response = PrepareAndSendReq(action, data, isPost, requiresLogin);
            if (string.IsNullOrEmpty(response))
            {
                ErrorCodes errCode = ErrorCodes.COMMUNICATION_ERROR;
                if (WebConnector.Current.IsTimeout)
                    errCode = ErrorCodes.TIMEOUT;
                SetLastError(log, errCode, S2CRes<string>.GetErrorMsg(errCode));
                return new S2CResListBaseEntity<T>(0.0, errCode, null, 0, null);
            }

            S2CResListBaseEntity<T> resp = S2CSerializer.DeserializeBaseEntityList<T>(response, m_serialFormat);
            if (!CheckResp<T>(resp))
            {
                PrintRespError<T>(resp);

                //if the problem is related to user not logged in, reset login status and retry another time:
                if (requiresLogin && (resp != null) && (resp.Status == ErrorCodes.NOT_LOGGED_IN))
                {
                    WebConnector.Current.ResetLoginStatus(); //reset login status

                    //retry the WS call:
                    response = PrepareAndSendReq(action, data, isPost, requiresLogin);
                    resp = S2CSerializer.DeserializeBaseEntityList<T>(response, m_serialFormat);
                }
            }

            return resp;
        }


        /// <summary>
        /// Sends an HTTP Request to the backend and parse the response (expected as an object)
        /// </summary>
        /// <param name="action">action to query (e.g. "Snippets/Get")</param>
        /// <param name="data">
        ///     if the request is a GET, the parameters to be put in the querystring (e.g. "snippetID=100"),
        ///     otherwise data to be put in the post (e.g. "content=...")
        /// </param>
        /// <param name="isPost">whether this request is a GET(false) or a POST(true)</param>
        /// <param name="requiresLogin">false if this request calls a public web service</param>
        /// <returns>null if any error occurred; the result of the invocation otherwise</returns>
        protected S2CResObj<object> SendReqObj(string action, string data, bool isPost, bool requiresLogin = true)
        {
            string response = PrepareAndSendReq(action, data, isPost, requiresLogin);
            if (string.IsNullOrEmpty(response))
            {
                ErrorCodes errCode = ErrorCodes.COMMUNICATION_ERROR;
                if (WebConnector.Current.IsTimeout)
                    errCode = ErrorCodes.TIMEOUT;
                SetLastError(log, errCode, S2CRes<string>.GetErrorMsg(errCode));
                return new S2CResObj<object>(0.0, errCode, null);
            }

            S2CResObj<object> resp = S2CSerializer.DeserializeObj(response, m_serialFormat);
            if (!CheckResp<object>(resp))
            {
                PrintRespError<object>(resp);

                //if the problem is related to user not logged in, reset login status and retry another time:
                if (requiresLogin && (resp != null) && (resp.Status == ErrorCodes.NOT_LOGGED_IN))
                {
                    WebConnector.Current.ResetLoginStatus(); //reset login status

                    //retry the WS call:
                    response = PrepareAndSendReq(action, data, isPost, requiresLogin);
                    resp = S2CSerializer.DeserializeObj(response, m_serialFormat);
                }
            }

            return resp;
        }


        /// <summary>
        /// Sends an HTTP Request to the backend and parse the response (expected as a list of objects)
        /// </summary>
        /// <param name="action">action to query (e.g. "Snippets/Get")</param>
        /// <param name="data">
        ///     if the request is a GET, the parameters to be put in the querystring (e.g. "snippetID=100"),
        ///     otherwise data to be put in the post (e.g. "content=...")
        /// </param>
        /// <param name="isPost">whether this request is a GET(false) or a POST(true)</param>
        /// <param name="requiresLogin">false if this request calls a public web service</param>
        /// <returns>null if any error occurred; the result of the invocation otherwise</returns>
        protected S2CResListObj<string> SendReqListObj(string action, string data, bool isPost, bool requiresLogin = true)
        {
            string response = PrepareAndSendReq(action, data, isPost, requiresLogin);
            if (string.IsNullOrEmpty(response))
            {
                ErrorCodes errCode = ErrorCodes.COMMUNICATION_ERROR;
                if (WebConnector.Current.IsTimeout)
                    errCode = ErrorCodes.TIMEOUT;
                SetLastError(log, errCode, S2CRes<string>.GetErrorMsg(errCode));
                return new S2CResListObj<string>(0.0, errCode, null, 0);
            }

            S2CResListObj<string> resp = S2CSerializer.DeserializeObjList(response, m_serialFormat);
            if (!CheckResp<string>(resp))
            {
                PrintRespError<string>(resp);

                //if the problem is related to user not logged in, reset login status and retry another time:
                if (requiresLogin && (resp != null) && (resp.Status == ErrorCodes.NOT_LOGGED_IN))
                {
                    WebConnector.Current.ResetLoginStatus(); //reset login status

                    //retry the WS call:
                    response = PrepareAndSendReq(action, data, isPost, requiresLogin);
                    resp = S2CSerializer.DeserializeObjList(response, m_serialFormat);
                }
            }

            return resp;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Checks that the given response is valid and can be safely parsed
        /// </summary>
        /// <param name="resp">response to validate</param>
        /// <returns></returns>
        protected bool CheckResp<T>(S2CRes<T> resp)
        {
            return ((resp != null) && (resp.Status == ErrorCodes.OK)); // && (resp.Data != null));
        }


        /// <summary>
        /// Prints the info regarding the given erroneous response
        /// </summary>
        /// <param name="resp">response to print</param>
        /// <returns></returns>
        protected void PrintRespError<T>(S2CRes<T> resp)
        {
            if ((resp == null) || !(resp is S2CResObj<object>)) 
            {
                SetLastError(log, (resp == null ? ErrorCodes.FAIL : resp.Status), "Generic Error");
                log.Warn("null response");
            } 
            else 
            {
                S2CResObj<T> errMessage = (S2CResObj<T>)resp;
                SetLastError(log, resp.Status, errMessage.Data.PrintNull());
                log.ErrorFormat("Error receiving the response: status={0}; messsage={1}", resp.Status, LastErrorMsg);
            }
        }


        /// <summary>
        /// Try to parse the given response that is expected to return a boolean result
        /// </summary>
        /// <returns></returns>
        protected bool ParseBoolResponse<T>(S2CRes<T> resp)
        {
            if (!CheckResp<T>(resp))
            {
                PrintRespError<T>(resp);
                return false;
            }

            bool result = false;
            S2CResObj<T> value = (S2CResObj<T>)resp;
            if (!bool.TryParse(value.Data.ToString(), out result))
            {
                PrintRespError<T>(resp);
                return false;
            }

            return result;
        }


        /// <summary>
        /// Try to parse the given response that is expected to return a long result
        /// </summary>
        /// <returns></returns>
        protected long ParseLongResponse<T>(S2CRes<T> resp)
        {
            if (!CheckResp<T>(resp))
            {
                PrintRespError<T>(resp);
                return -1;
            }

            long result = -1;
            S2CResObj<T> value = (S2CResObj<T>)resp;
            if (!long.TryParse(value.Data.ToString(), out result))
            {
                PrintRespError<T>(resp);
                return -1;
            }

            return result;
        }


        /// <summary>
        /// Checks the given response and returns the expected collection of BaseEntity objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resp"></param>
        /// <returns>an empty list of Base Entity objects if any error occurred, the expected collection otherwise</returns>
        protected ICollection<T> ParseListBaseEntityResponse<T>(S2CResListBaseEntity<T> resp)
            where T : BaseEntity
        {
            ICollection<T> result = new List<T>();
            if (!CheckResp(resp) || (resp.Data == null))
            {
                PrintRespError(resp);
                return result;
            }

            if (resp.Data != null)
            {
                foreach (BaseEntity be in resp.Data)
                {
                    result.Add((T)be.ToBaseEntity());
                }
            }
            return result;
        }


        /// <summary>
        /// Checks the given response and returns the expected BaseEntity object, if any
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resp">null if any error occurred, the expected object otherwise</param>
        /// <returns></returns>
        protected T ParseBaseEntityResponse<T>(S2CResBaseEntity<T> resp)
            where T : BaseEntity
        {
            //build the result:
            if (!CheckResp(resp))
            {
                PrintRespError(resp);
                return null;
            }
            return (T)resp.Data.ToBaseEntity();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
