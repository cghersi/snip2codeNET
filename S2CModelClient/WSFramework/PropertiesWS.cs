//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Web;

using Snip2Code.Model.Abstract;
using Snip2Code.Model.Entities;
using Snip2Code.Model.Client.Entities;
using Snip2Code.Utils;

namespace Snip2Code.Model.Client.WSFramework
{
    public class PropertiesWS : BaseWS, IPropertiesRepository
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Static caches:
        /// <summary>
        /// List of default properties
        /// </summary>
        private static ICollection<DefaultProperty> s_defProperties = null;

        #region Url to Web Services
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        private const string GET_DEFAULTS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Properties/GetDefaults";
        private const string SUGGEST_DEFAULTS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Properties/SuggestDefaults";

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public PropertiesWS() : base() { }

        public PropertiesWS(string username, string password)
            : base()
        {
            Username = username;
            Password = password;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Get Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public ICollection<DefaultProperty> GetDefaultProperties()
        {
            if (s_defProperties != null)
                return s_defProperties;

            //send the request and parse the response:
            string querystring = string.Empty;
            S2CResListBaseEntity<DefaultProperty> resp = SendReqListBaseEntity<DefaultProperty>(GET_DEFAULTS_URL, querystring, false);
            log.DebugFormat("GetDefaultProperties: resp in {0} ms", resp.ExecTime);

            //build the result:
            if (!CheckResp(resp))
            {
                PrintRespError(resp);
                return new List<DefaultProperty>();
            }

            s_defProperties = resp.Data; // resp.GetListFromResp<DefaultProperty>();
            return s_defProperties;
        }

        public IDictionary<string, PropValueInfo> GetPropValuesInfo()
        {
            throw new NotImplementedException();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Add Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool SuggestDefaultProperty(DefaultProperty defProp, int userID)
        {
            //send the request and parse the response:
            string contentToSend = "content=" + HttpUtility.UrlEncode(defProp.Serialize(m_serialFormat));
            S2CResObj<object> resp = SendReqObj(SUGGEST_DEFAULTS_URL, contentToSend, true);

            //build the result:
            if (!CheckResp(resp))
            {
                PrintRespError(resp);
                return false;
            }

            return ParseBoolResponse(resp);
        }

        public bool AddDefaultProperty(DefaultProperty defProp)
        {
            throw new NotImplementedException();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Reset Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ResetCaches()
        {
            s_defProperties = null;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
