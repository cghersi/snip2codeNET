//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;

using Snip2Code.Model.Abstract;
using Snip2Code.Model.Entities;
using Snip2Code.Utils;
using Snip2Code.Model.Comm;

namespace Snip2Code.Model.Client.WSFramework
{
    public class UserWS : BaseWS, IUserRepository
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Url to Web Services
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        private const string GET_GROUPS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "User/GetGroups";
        private const string GET_ADMINISTEREDGROUPS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "User/GetAdministeredGroups";
        private const string GET_CHANNELS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "User/GetChannels";
        private const string GET_CHANNELIDS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "User/GetChannelIDs";
        private const string GET_ADMINISTEREDCHANNELS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "User/GetAdministeredChannels";
        private const string GET_BADGES_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "User/GetBadges";
        private const string GET_IMAGE_URL = "Images/Get";

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public UserWS() : base() { }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////



        #region Get Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public User GetById(int id)
        {
            throw new NotSupportedException();
        }

        public User GetByUsername(string userName)
        {
            throw new NotImplementedException();
        }

        public User LoginUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: email={0} or password", email));
                return null;
            }

            Username = email;
            Password = password;
            UseOneAll = false;
            if (CheckLogin(true))
                return CurrentUser;
            else
                return null;
        }

        public User LoginUserOneAll(string oneAllToken)
        {
            if (string.IsNullOrWhiteSpace(oneAllToken))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: oneAllToken={0}", oneAllToken));
                return null;
            }

            IdentToken = oneAllToken;
            UseOneAll = true;
            if (CheckLogin(true))
                return CurrentUser;
            else
                return null;
        }

        public IList<string> CheckUniqueNickName(string nickname, string name, string lastname)
        {
            throw new NotImplementedException();
        }

        public byte[] GetUserImage(int userID, S2CImagesSize size)
        {
            if (userID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}", userID));
                return new byte[0];
            }

            return WebConnector.Current.SendByteRequest(GET_IMAGE_URL, userID + "&size=" + (int)size, false, false);
        }

        public ICollection<Group> GetGroupsOfUser(int userID)
        {
            if (userID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}", userID));
                return new List<Group>();
            }

            //send the request and parse the response:
            string querystring = string.Format("ID={0}", userID);
            S2CResListBaseEntity<GroupComm> resp = SendReqListBaseEntity<GroupComm>(GET_GROUPS_URL, querystring, false);

            //build the result:
            int totNum = 0;
            List<Group> g = resp.GetListFromResp<GroupComm, Group>(out totNum);

            return g;
        }

        public ICollection<Group> GetAdministeredGroupsOfUser(int userID)
        {
            if (userID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}", userID));
                return new List<Group>();
            }

            //send the request and parse the response:
            string querystring = string.Format("ID={0}", userID);
            S2CResListBaseEntity<GroupComm> resp = SendReqListBaseEntity<GroupComm>(GET_ADMINISTEREDGROUPS_URL, querystring, false);

            //build the result:
            int totNum = 0;
            return resp.GetListFromResp<GroupComm, Group>(out totNum);
        }

        public IList<Group> GetChannelsOfUser(int userID)
        {
            if (userID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}", userID));
                return new List<Group>();
            }

            //send the request and parse the response:
            string querystring = string.Format("ID={0}", userID);
            S2CResListBaseEntity<GroupComm> resp = SendReqListBaseEntity<GroupComm>(GET_CHANNELS_URL, querystring, false);

            //build the result:
            int totNum = 0;
            List<Group> g = resp.GetListFromResp<GroupComm, Group>(out totNum);

            return g;
        }

        public IList<int> GetChannelIDsOfUser(int userID)
        {
            if (userID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}", userID));
                return new List<int>();
            }

            //send the request and parse the response:
            string querystring = string.Format("ID={0}", userID);
            S2CResListBaseEntity<GroupComm> resp = SendReqListBaseEntity<GroupComm>(GET_CHANNELIDS_URL, querystring, false);

            //build the result:
            int totNum = 0;
            List<Group> g = resp.GetListFromResp<GroupComm, Group>(out totNum);
            if (g != null)
                return g.Select((group) => group.ID) as IList<int>;
            else
                return new List<int>();
        }

        public IList<Group> GetAdministeredChannelsOfUser(int userID)
        {
            if (userID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}", userID));
                return new List<Group>();
            }

            //send the request and parse the response:
            string querystring = string.Format("ID={0}", userID);
            S2CResListBaseEntity<GroupComm> resp = SendReqListBaseEntity<GroupComm>(GET_ADMINISTEREDCHANNELS_URL, querystring, false);

            //build the result:
            int totNum = 0;
            return resp.GetListFromResp<GroupComm, Group>(out totNum);
        }

        public IList<Group> GetPendingInvitationGroupsOfUser(int userID)
        {
            throw new NotImplementedException();
        }

        public ICollection<OneAllToken> GetTokensOfUser(int userID)
        {
            throw new NotImplementedException();
        }

        public List<Badge> GetBadges(int userID)
        {
            if (userID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}", userID));
                return new List<Badge>();
            }

            //send the request and parse the response:
            string querystring = string.Format("ID={0}", userID);
            S2CResListBaseEntity<Badge> resp = SendReqListBaseEntity<Badge>(GET_BADGES_URL, querystring, false);

            //build the result:
            int totNum = 0;
            return resp.GetListFromResp<Badge, Badge>(out totNum);
        }

        public Dictionary<short, Tuple<int, DateTime>> GetSnippetBadges(int userID)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetAllUsers(int count)
        {
            throw new NotImplementedException();
        }

        public IList<int> GetAllUserIDs(int count)
        {
            throw new NotImplementedException();
        }

        public ICollection<UserBaseInfo> SearchUsers(string query, int maxNum = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Create/Add/Modify/Delete Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public int CreateActiveUser(string email, string password, string name, string lastName, string nickName,
                bool hasValidMail = true, string oneAllToken = "", string oneAllProvider = "", string oneAllNickname = "",
                bool oneAllTokenIsValid = true, string oneAllTokenIdent = "")
        {
            throw new NotImplementedException();
        }

        public int InviteUser(string email, string securityToken)
        {
            throw new NotImplementedException();
        }

        public bool RefreshSecToken(string email, string securityToken)
        {
            throw new NotImplementedException();
        }

        public int ActivateUser(string email, string password, string name, string lastName, string nickName, string securityToken)
        {
            throw new NotImplementedException();
        }

        public bool ChangeUserProfile(User modified)
        {
            throw new NotImplementedException();
        }

        public bool ChangeUserPsw(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool ChangeUserEMail(int id, string email)
        {
            throw new NotImplementedException();
        }

        public bool ChangeUserNickname(int id, string nickname)
        {
            throw new NotImplementedException();
        }

        public bool SaveUserPicture(int userID, byte[] imageStream)
        {
            throw new NotImplementedException();
        }

        public bool AddPointsToUser(int userID, int points)
        {
            throw new NotImplementedException();
        }

        public int RecalcPointsForUser(int userID)
        {
            throw new NotImplementedException();
        }

        public bool SyncBadgeForUser(int userID, BadgeType badgeType, int reachedValue)
        {
            throw new NotImplementedException();
        }

        public bool AddSignupRequest(string email)
        {
            throw new NotImplementedException();
        }

        public bool Add3Party(int userID, User3Party thirdParty)
        {
            throw new NotImplementedException();
        }

        public bool Remove3Party(int userID, User3Party thirdParty)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(int userID)
        {
            throw new NotImplementedException();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Cache Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateCache(User user)
        {
            //anything to do here until we implement the cache on client side...
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
