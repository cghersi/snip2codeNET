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
using Snip2Code.Model.Comm;

namespace Snip2Code.Model.Client.WSFramework
{
    public class GroupWS : BaseWS, IGroupRepository
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Url to Web Services
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        private const string SEARCH_CHANNELS_URL = "Channel/GetChannels";
        //private const string GET_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Channel/Get";

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public GroupWS() : base() { }

        public GroupWS(string username, string password)
            : base()
        {
            Username = username;
            Password = password;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Get Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public Group GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Group GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<int> GetAdminIDs(int groupID)
        {
            throw new NotImplementedException();
        }

        public IList<GroupUserInfo> GetUsers(int groupID)
        {
            throw new NotImplementedException();
        }

        public IList<Group> GetAllChannels(ChannelSort orderBy, bool orderDesc, out int totNum, 
            int start = 0, int count = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public IList<Group> SearchChannels(string searchText, ChannelSort orderBy, bool orderDesc, out int totNum, 
            int start = 0, int count = int.MaxValue)
        {
            totNum = 0;

            //check for erroneous input:
            if ((count <= 0) || (start < 0))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}, searchText={1}, maxNum={2}, start={3}",
                    CurrentUserID, searchText.PrintNull(), count, start));
                return new List<Group>();
            }

            //send the request and parse the response:
            if (searchText == null)
                searchText = string.Empty;
            string querystring = string.Format("query={0}&field={1}&direction={2}&start={3}&maxNum={4}",
                HttpUtility.UrlEncode(searchText), orderBy, (orderDesc ? SortDirection.Descent : SortDirection.Ascent), 
                start, count);
            S2CResListBaseEntity<GroupComm> resp = SendReqListBaseEntity<GroupComm>(SEARCH_CHANNELS_URL, querystring, false, false);

            //build the result:
            IList<Group> channels = resp.GetListFromResp<GroupComm, Group>(out totNum);
            return channels;
        }

        public IList<int> GetChildrenIDs(int parentGroupID)
        {
            throw new NotImplementedException();
        }

        public IList<int> GetParentIDs(int childGroupID)
        {
            throw new NotImplementedException();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Create/Add/Modify/Delete Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public int CreateGroup(string name, int adminID, GroupPolicy policy, GroupPreferences preferences,
            string shortDescription, string description)
        {
            throw new NotImplementedException();
        }

        public int CreateChannel(string name, int adminID, GroupPreferences preferences, string shortDescription,
            string description)
        {
            throw new NotImplementedException();
        }

        public bool ChangeGroupProfile(Group modified)
        {
            throw new NotImplementedException();
        }

        public bool AddGroupUser(int id, int newUserID, bool isAdmin, GroupJoinStatus status)
        {
            throw new NotImplementedException();
        }

        public bool JoinChannel(int channelID, int newUserID)
        {
            throw new NotImplementedException();
        }

        public bool InviteUsers(int groupID, IList<int> newUserIDs)
        {
            throw new NotImplementedException();
        }

        public bool AddChildGroup(int childID, int parentID)
        {
            throw new NotImplementedException();
        }

        public bool RemoveGroupUser(int groupID, int userID)
        {
            throw new NotImplementedException();
        }

        public bool RemoveChildGroup(int childID, int parentID)
        {
            throw new NotImplementedException();
        }

        public bool DeleteGroup(int groupID, bool onlyData)
        {
            throw new NotImplementedException();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Cache Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateCache(Group group)
        {
            //no cache available on client
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
