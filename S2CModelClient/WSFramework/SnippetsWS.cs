//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Web;

using Snip2Code.Model.Abstract;
using Snip2Code.Model.Entities;
using Snip2Code.Model.Client.Entities;
using Snip2Code.Utils;
using Snip2Code.Model.Comm;

namespace Snip2Code.Model.Client.WSFramework
{
    public class SnippetsWS : BaseWS, ISnippetsRepository
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Url to Web Services
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        private const string FIND_SNIPPETS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/FindItems";
        private const string GET_BYNAME_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/GetItemByName";
        private const string GETID_BYNAME_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/GetItemIDByName";
        private const string CHECKEXIST_BYNAME_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/CheckExistByNameAndCode";  
        private const string FINDPAGED_SNIPPETS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/FindItemsPaged";
        private const string FINDPAGED_SNIPPETIDS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/FindItemsIDPaged";
        private const string FINDPUB_SNIPPETS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/FindPubItemsID";
        private const string FINDGROUP_SNIPPETS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/FindGroupItems";
        private const string FINDGROUPS_SNIPPETS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/FindGroupsItems";
        private const string GET_SNIPPET_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/Get";
        private const string GET_TAGS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/GetTags";
        private const string GET_PROPERTIES_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/GetProperties";
        private const string GET_CHANNELS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/GetChannels";
        private const string GET_MOSTCORRELATED_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/GetMostCorrelated";
        private const string GET_BADGES_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/GetBadges";
        private const string GET_COMMENTS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/GetComments";
        private const string GET_COMMENT_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/GetComment";
        private const string GET_COMMENTS_COUNT_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/GetCommentCount"; 
        private const string COUNT_SNIPPET_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/Count";
        private const string GET_SNIPPET_VOTE_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/GetSnippetVote";
        private const string CREATE_SNIPPET_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/Add";
        //private const string CHANGE_SNIPPET_URL = (useOAuth ? OAUTH_PREFIX_ACTION : "") + "Snippets/Change";
        private const string RATE_SNIPPET_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/Rate";
        private const string SHARE_SNIPPET_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/Share";
        private const string PUBLISH_SNIPPET_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/Publish";
        private const string ADD_TAGS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/AddTags";
        private const string ADD_PROPERTY_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/AddProperty";
        private const string ADD_PROPERTIES_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/AddProperties";
        private const string ADD_COMMENT_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/AddComment";
        private const string CLEAR_TAGS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/ClearTags";
        private const string DELETE_TAGS_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/DeleteTags";
        private const string DELETE_SNIPPET_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/Delete";
        private const string CLEAR_PROPERTIES_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/ClearProperties";
        private const string DELETE_PROPERTY_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/DeleteProperty";
        private const string DELETE_PROPERTY_BYNAME_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/DeletePropertyByName";
        private const string DELETE_COMMENT_URL = (USE_OAUTH ? OAUTH_PREFIX_ACTION : "") + "Snippets/DeleteComment";
        private const string SEARCH_SNIPPET_URL = "Search/Get";
        private const string SEARCH_SNIPPET_OFGROUPS_URL = "Search/GetForGroups";

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public SnippetsWS() : base() { }

        public SnippetsWS(string username, string password) : base()
        {
            Username = username;
            Password = password;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Get Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public IList<Snippet> GetSnippetsForSearch(string searchText, out Dictionary<string, string[]> misSpellings, out int totNum,
            out int totNumWithNonDefOp, out SortedDictionary<string, int> tagsOccurrences, int maxNum = int.MaxValue, int start = 0, bool onlyCreated = false, int onlyOfGroup = 0,
            ItemSortField field = ItemSortField.Relevance, SortDirection direction = SortDirection.Descent)
        {
            //check for erroneous input:
            misSpellings = null;
            totNum = 0;
            totNumWithNonDefOp = 0;
            tagsOccurrences = null;
            if ((maxNum <= 0) || (start < 0))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}, searchText={1}, maxNum={2}, start={3}",
                    CurrentUserID, searchText.PrintNull(), maxNum, start));
                return new List<Snippet>();
            }

            //send the request and parse the response:
            string querystring = string.Format("query={0}&maxNum={1}&start={2}&onlyCreated={3}&onlyOfGroup={4}&field={5}&direction={6}",
                                    HttpUtility.UrlEncode(searchText), maxNum, start, onlyCreated, onlyOfGroup, field, direction);
            S2CResListBaseEntity<SnippetComm> resp = SendReqListBaseEntity<SnippetComm>(SEARCH_SNIPPET_URL, querystring, false, false);

            //build the result:
            IList<Snippet> snips = resp.GetListFromResp<SnippetComm, Snippet>(out totNum);
            misSpellings = resp.GetMiscFromResp<SnippetComm>(out totNumWithNonDefOp);
            return snips;
        }

        public IList<Snippet> GetSnippetsForSearch(string searchText, int[] groups, out Dictionary<string, string[]> misSpellings, out int totNum,
            out int totNumWithNonDefOp, out SortedDictionary<string, int> tagsOccurrences, int maxNum = int.MaxValue, int start = 0, 
            ItemSortField field = ItemSortField.Relevance, SortDirection direction = SortDirection.Descent)
        {
            //check for erroneous input:
            misSpellings = null;
            totNum = 0;
            totNumWithNonDefOp = 0;
            tagsOccurrences = null;
            if ((groups == null) || (groups.Length <= 0) || (maxNum <= 0) || (start < 0))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}, searchText={1}, groups={2}, maxNum={3}, start={4}",
                    CurrentUserID, searchText.PrintNull(), groups.PrintNull(), maxNum, start));
                return new List<Snippet>();
            }

            //send the request and parse the response:
            string groupString = Utilities.MergeIntoCommaSeparatedString(groups, false, ' ');
            if (string.IsNullOrEmpty(groupString))
                return new List<Snippet>();
            string querystring = string.Format("query={0}&groups={1}&maxNum={2}&start={3}&field={4}&direction={5}",
                                    HttpUtility.UrlEncode(searchText), HttpUtility.UrlEncode(groupString), maxNum, start, field, direction);
            S2CResListBaseEntity<SnippetComm> resp = SendReqListBaseEntity<SnippetComm>(SEARCH_SNIPPET_OFGROUPS_URL, querystring, false, false);

            //build the result:
            IList<Snippet> snips = resp.GetListFromResp<SnippetComm, Snippet>(out totNum);
            misSpellings = resp.GetMiscFromResp<SnippetComm>(out totNumWithNonDefOp);
            return snips;
        }

        public Snippet GetSnippetByID(long snippetID)
        {
            //check for erroneous input:
            if (snippetID <= 0)
                return null;

            //send the request and parse the response:
            S2CResBaseEntity<SnippetComm> resp = SendReqBaseEntity<SnippetComm>(GET_SNIPPET_URL, "snippetID=" + snippetID, false, false);
            log.DebugFormat("Snippet {0} taken in {1} ms", snippetID, (resp == null ? -1 : resp.ExecTime));

            //build the result:
            return ParseBaseEntityResponse<SnippetComm>(resp);
        }

        public Snippet GetSnippetByName(string name)
        {
            //check for erroneous input:
            if (string.IsNullOrWhiteSpace(name))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}, name={1}", CurrentUserID, name.PrintNull()));
                return null;
            }

            //send the request and parse the response:
            S2CResBaseEntity<SnippetComm> resp = SendReqBaseEntity<SnippetComm>(GET_BYNAME_URL, HttpUtility.UrlEncode(name), false, false);

            //build the result:
            return ParseBaseEntityResponse<SnippetComm>(resp);
        }

        public long GetSnippetIDByName(string name)
        {
            //check for erroneous input:
            if (string.IsNullOrWhiteSpace(name))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}, name={1}", CurrentUserID, name.PrintNull()));
                return -1;
            }

            //send the request and parse the response:
            S2CResObj<object> resp = SendReqObj(GETID_BYNAME_URL, HttpUtility.UrlEncode(name), false, false);

            //build the result:
            return ParseLongResponse(resp);
        }

        public bool CheckExistenceByNameAndCode(string itemName, string code)
        {
            //check for erroneous input:
            if (string.IsNullOrWhiteSpace(itemName) || string.IsNullOrWhiteSpace(code))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}, name={1}, code={2}",
                    CurrentUserID, itemName.PrintNull(), code.PrintNull()));
                return false;
            }

            //send the request and parse the response:
            string querystring = string.Format("itemName={0}&code={1}", HttpUtility.UrlEncode(itemName), HttpUtility.UrlEncode(code));
            S2CResObj<object> resp = SendReqObj(CHECKEXIST_BYNAME_URL, querystring, true, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        public IList<Snippet> FindItems(long[] snippetIDs, bool keepOrdering = false)
        {
            //check for erroneous input:
            if ((snippetIDs == null) || (snippetIDs.Length == 0))
                return new List<Snippet>();

            //send the request and parse the response:
            string content = Utilities.MergeIntoCommaSeparatedString(snippetIDs, true, ' ');
            if (string.IsNullOrEmpty(content))
                return new List<Snippet>();
            string querystring = string.Format("content={0}", HttpUtility.UrlEncode(content));
            S2CResListBaseEntity<SnippetComm> resp = SendReqListBaseEntity<SnippetComm>(FIND_SNIPPETS_URL, querystring, true, false);

            //build the result:
            int totNum = 0;
            return resp.GetListFromResp<SnippetComm, Snippet>(out totNum);
        }

        public IList<Snippet> FindItems(out int totNum, int start = 0, int count = int.MaxValue, bool onlyOwned = false)
        {
            //check for erroneous input:
            totNum = 0;
            if ((count <= 0) || (start < 0))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, 
                    string.Format("Input error: userID={0}, count={1}, start={2}", CurrentUserID, count, start));
                return new List<Snippet>();
            }

            //send the request and parse the response:
            string querystring = string.Format("start={0}&count={1}&onlyOwned={2}", start, count, onlyOwned);
            S2CResListBaseEntity<SnippetComm> resp = SendReqListBaseEntity<SnippetComm>(FINDPAGED_SNIPPETS_URL, querystring, false, false);

            //build the result:
            IList<Snippet> snips = resp.GetListFromResp<SnippetComm, Snippet>(out totNum);
            return snips;
        }

        public IList<long> FindItemsID(int start, int count, bool onlyOwned, out int totNum)
        {
            //check for erroneous input:
            totNum = 0;
            if ((count <= 0) || (start < 0))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, 
                    string.Format("Input error: userID={0}, count={1}, start={2}", CurrentUserID, count, start));
                return new List<long>();
            }

            //send the request and parse the response:
            string querystring = string.Format("start={0}&count={1}&onlyOwned={2}", start, count, onlyOwned);
            S2CResListObj<string> resp = SendReqListObj(FINDPAGED_SNIPPETIDS_URL, querystring, false, false);

            //build the result:
            IList<long> snipIDs = resp.GetListOfLongsFromResp(out totNum);
            return snipIDs;
        }

        public IList<long> FindPublicItemsID(int start, int count, int minRelevance, out int totNum)
        {
            //check for erroneous input:
            totNum = 0;
            if ((count <= 0) || (start < 0))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, 
                    string.Format("Input error: userID={0}, count={1}, start={2}", CurrentUserID, count, start));
                return new List<long>();
            }

            //send the request and parse the response:
            string querystring = string.Format("start={0}&count={1}&minRelevance={2}", start, count, minRelevance);
            S2CResListObj<string> resp = SendReqListObj(FINDPUB_SNIPPETS_URL, querystring, false, false);

            //build the result:
            IList<long> snipIDs = resp.GetListOfLongsFromResp(out totNum);
            return snipIDs;
        }

        public IList<Snippet> FindPublicItems(int start, int count, out int totNum)
        {
            return FindItems(out totNum, start, count, false);
        }

        public IList<Snippet> FindPersonalItems(int start, int count, out int totNum)
        {
            return FindGroupItems(start, count, CurrentUser.PersonalGroupID, out totNum);
        }

        public IList<Snippet> FindGroupItems(int start, int count, int groupID, out int totNum)
        {
            //check for erroneous input:
            totNum = 0;
            if ((count <= 0) || (start < 0))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, 
                    string.Format("Input error: userID={0}, count={1}, start={2}", CurrentUserID, count, start));
                return new List<Snippet>();
            }

            //send the request and parse the response:
            string querystring = string.Format("groupID={0}&start={1}&count={2}", groupID, start, count);
            S2CResListBaseEntity<SnippetComm> resp = SendReqListBaseEntity<SnippetComm>(FINDGROUP_SNIPPETS_URL, querystring, false, false);

            //build the result:
            IList<Snippet> snips = resp.GetListFromResp<SnippetComm, Snippet>(out totNum);
            return snips;
        }

        public IList<Snippet> FindGroupItems(int start, int count, int[] groupIDs, out int totNum,
            ItemSortField field = ItemSortField.Relevance, SortDirection direction = SortDirection.Descent)
        {
            //check for erroneous input:
            totNum = 0;
            if ((count <= 0) || (start < 0) || (groupIDs == null) || (groupIDs.Length == 0))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}, count={1}, start={2}, groupIDs={3}",
                    CurrentUserID, count, start, groupIDs.Print()));
                return new List<Snippet>();
            }

            //send the request and parse the response:
            string content = Utilities.MergeIntoCommaSeparatedString(groupIDs, true, ' ');
            if (string.IsNullOrEmpty(content))
                return new List<Snippet>();
            string querystring = string.Format("content={0}&start={1}&count={2}&field={3}&direction={4}",
                HttpUtility.UrlEncode(content), start, count, field, direction);
            S2CResListBaseEntity<SnippetComm> resp = SendReqListBaseEntity<SnippetComm>(FINDGROUPS_SNIPPETS_URL, querystring, true, false);

            //build the result:
            IList<Snippet> snips = resp.GetListFromResp<SnippetComm, Snippet>(out totNum);
            return snips;
        }

        public IList<Snippet> FindChannelsItems(int start, int count, ICollection<int> channelsIDs, out int totNum,
                ItemSortField field = ItemSortField.Relevance, SortDirection direction = SortDirection.Descent,
                bool findAllChannels = false)
        {
            //check for erroneous input:
            totNum = 0;
            if ((count <= 0) || (start < 0) || (channelsIDs.IsNullOrEmpty() && !findAllChannels))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: userID={0}, count={1}, start={2}, channelsIDs={3}",
                    CurrentUserID, count, start, channelsIDs.Print()));
                return new List<Snippet>();
            }

            //exec the SP:
            string search = string.Format("{0}={1}", DefaultProperty.Channel,
                (findAllChannels ? "*" : Utilities.MergeIntoCommaSeparatedString(channelsIDs, true)));
            Dictionary<string, string[]> misSpellings = null;
            int totNumWithNonDefOp = 0;
            SortedDictionary<string, int> tagsOccurrences = null; 
            return GetSnippetsForSearch(search, out misSpellings, out totNum, out totNumWithNonDefOp, out tagsOccurrences,
                        count, start, false, 0, field, direction);
        }

        public IList<long> FindCreatedItemsID(out int totNum, int start = 0, int count = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public SnippetComment GetComment(long commentID)
        {
            //check for erroneous input:
            if (commentID <= 0)
                return null;

            //send the request and parse the response:
            S2CResBaseEntity<SnippetCommentComm> resp = 
                SendReqBaseEntity<SnippetCommentComm>(GET_COMMENT_URL, "commentID=" + commentID, false);

            //build the result:
            return ParseBaseEntityResponse<SnippetCommentComm>(resp);
        }

        public ICollection<SnippetComment> GetComments(long snippetID)
        {
            //check for erroneous input:
            if (snippetID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}", snippetID));
                return new List<SnippetComment>();
            }

            //send the request and parse the response:
            S2CResListBaseEntity<SnippetCommentComm> resp = 
                SendReqListBaseEntity<SnippetCommentComm>(GET_COMMENTS_URL, "id=" + snippetID, false);

            //build the result:
            int totNum = 0;
            return resp.GetListFromResp<SnippetCommentComm, SnippetComment>(out totNum);
        }

        public int GetCommentsCount(long snippetID)
        {
            if (snippetID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}", snippetID));
                return -1;
            }

            //send the request and parse the response:
            S2CResObj<object> resp = SendReqObj(GET_COMMENTS_COUNT_URL, "id=" + snippetID, false);

            //build the result:
            return (int)ParseLongResponse(resp);
        }

        public ICollection<Tag> GetTags(long snippetID)
        {
            //check for erroneous input:
            if (snippetID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}", snippetID));
                return new List<Tag>();
            }

            //send the request and parse the response:
            S2CResListBaseEntity<Tag> resp = SendReqListBaseEntity<Tag>(GET_TAGS_URL, "id=" + snippetID, false, false);

            //build the result:
            return ParseListBaseEntityResponse<Tag>(resp);
        }

        public ICollection<SnippetProperty> GetProperties(long snippetID)
        {
            //check for erroneous input:
            if (snippetID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}", snippetID));
                return new List<SnippetProperty>();
            }

            //send the request and parse the response:
            S2CResListBaseEntity<SnippetProperty> resp =
                SendReqListBaseEntity<SnippetProperty>(GET_PROPERTIES_URL, "id=" + snippetID, false, false);

            //build the result:
            return ParseListBaseEntityResponse<SnippetProperty>(resp);
        }

        public ICollection<int> GetChannels(long snippetID)
        {
            //check for erroneous input:
            if (snippetID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}", snippetID));
                return new List<int>();
            }

            //send the request and parse the response:
            S2CResListObj<string> resp = SendReqListObj(GET_CHANNELS_URL, "id=" + snippetID, false, false);

            //build the result:
            int totNum = 0;
            return resp.GetListOfIntFromResp(out totNum);
        }

        public List<Badge> GetBadges(long snippetID)
        {
            //check for erroneous input:
            if (snippetID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}", snippetID));
                return new List<Badge>();
            }

            //send the request and parse the response:
            S2CResListBaseEntity<Badge> resp = SendReqListBaseEntity<Badge>(GET_BADGES_URL, "id=" + snippetID, false, false);

            //build the result:
            int totNum = 0;
            List<Badge> badges = resp.GetListFromResp<Badge, Badge>(out totNum);
            return badges;
        }

        public ICollection<Snippet> GetMostCorrelatedSnippets(long snippetID)
        {
            //check for erroneous input:
            if (snippetID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}", snippetID));
                return new List<Snippet>();
            }

            //send the request and parse the response:
            S2CResListBaseEntity<SnippetComm> resp = SendReqListBaseEntity<SnippetComm>(GET_MOSTCORRELATED_URL, "id=" + snippetID, false, false);

            //build the result:
            int totNum = 0;
            IList<Snippet> snips = resp.GetListFromResp<SnippetComm, Snippet>(out totNum);
            return snips;
        }

        public int CountSnippets(int groupID = -1)
        {
            //send the request and parse the response:
            S2CResObj<object> resp = SendReqObj(COUNT_SNIPPET_URL, "groupID=" + groupID, false, true);

            //build the result:
            return (int)ParseLongResponse(resp);
        }

        public int GetSnippetVote(long snippetID)
        {
            //send the request and parse the response:
            S2CResObj<object> resp = SendReqObj(GET_SNIPPET_VOTE_URL, "snippetID=" + snippetID, false, false);

            //build the result:
            return (int)ParseLongResponse(resp);
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Create/Add/Modify Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>
        ///// Create a Snippet having Owner and Creator = CurrentUser
        ///// </summary>
        //public long SaveSnippet(Snippet snippet)
        //{
        //    //Check that the client has been already logged in, and if this is not the case, automatically login:
        //    if (!CheckLogin())
        //    {
        //        SetLastError(log, "Cannot login into the system");
        //        return -1;
        //    }

        //    snippet.TargetGroupID = CurrentUser.PersonalGroupID;
        //    return CreateSnippet(snippet, CurrentUser);
        //}


        public long SaveSnippet(Snippet snippet)
        {
            if ((snippet == null) || (snippet.AreFieldsValid() != SnippetWrongField.OK))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, 
                    string.Format("Input error: snippet={0}, creator={1}", snippet.PrintNull(), CurrentUser.PrintNull()));
                return -1;
            }

            //Check that the client has been already logged in, and if this is not the case, automatically login:
            if (!CheckLogin())
            {
                SetLastError(log, ErrorCodes.NOT_LOGGED_IN, "Cannot login into the system");
                return -1;
            }

            //send the request and parse the response:
            string contentToSend = "content=" + HttpUtility.UrlEncode(snippet.Serialize(m_serialFormat));
            S2CResObj<object> resp = SendReqObj(CREATE_SNIPPET_URL, contentToSend, true);

            //build the result:
            long result = ParseLongResponse(resp);
            if (result == -1)
            {
                //check if the Status is TARGET_GROUP_INVALID:
                if ((resp != null) && (resp.Status == ErrorCodes.TARGET_GROUP_DELETED))
                    return -2;
            }
            return result;
        }

        public bool AddSnippetTags(long snippetID, ICollection<string> tags)
        {
            if ((snippetID <= 0) || tags.IsNullOrEmpty())
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}, tags={1}", snippetID, tags.Print<string>()));
                return false;
            }

            //send the request and parse the response:
            S2CSerializer ser = new S2CSerializer(m_serialFormat, snippetID);
            string contentToSend = "content=" + HttpUtility.UrlEncode(ser.SerializeListObj<string>(tags.ToArray()));
            S2CResObj<object> resp = SendReqObj(ADD_TAGS_URL, contentToSend, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        public bool AddSnippetProperties(long snippetID, ICollection<SnippetProperty> properties)
        {
            if ((snippetID <= 0) || properties.IsNullOrEmpty())
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, 
                    string.Format("Input error: snippetID={0}, properties={1}", snippetID, properties.Print<SnippetProperty>()));
                return false;
            }

            //check input properties and clean them if not valid:
            List<SnippetProperty> cleanProperties = SnippetProperty.CleanPropertyList(properties, snippetID);

            //fastly return if no properties are to be added:
            if (cleanProperties.IsNullOrEmpty())
                return true;

            //send the request and parse the response:
            S2CSerializer ser = new S2CSerializer(m_serialFormat, snippetID);
            string contentToSend = "content=" + HttpUtility.UrlEncode(ser.SerializeBaseEntityList<SnippetProperty>(cleanProperties.ToArray(), null));
            S2CResObj<object> resp = SendReqObj(ADD_PROPERTIES_URL, contentToSend, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        public long AddSnippetProperty(long snippetID, string propName, string propValue, string oldPropValue = "")
        {
            if ((snippetID <= 0) || !SnippetProperty.DataAreValid(propName, propValue))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}, propName={1}, propValue={2}",
                                snippetID, propName.PrintNull(), propValue.PrintNull()));
                return -1;
            }

            //send the request and parse the response:
            string contentToSend = string.Format("snippetID={0}&propName={1}&propValue={2}&oldPropValue={3}", snippetID,
                    HttpUtility.UrlEncode(propName), HttpUtility.UrlEncode(propValue), HttpUtility.UrlEncode(oldPropValue));
            S2CResObj<object> resp = SendReqObj(ADD_PROPERTY_URL, contentToSend, true);

            //build the result:
            return ParseLongResponse(resp);
        }

        public bool AddSnippetComment(long snippetID, string comment)
        {
            if ((snippetID <= 0) || string.IsNullOrEmpty(comment))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, 
                    string.Format("Input error: snippetID={0}, comment={1}", snippetID, comment.PrintNull()));
                return false;
            }

            //send the request and parse the response:
            string contentToSend = string.Format("snippetID={0}&comment={1}", snippetID, HttpUtility.UrlEncode(comment));
            S2CResObj<object> resp = SendReqObj(ADD_COMMENT_URL, contentToSend, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        //public bool ChangeSnippet(Snippet snippet)
        //{
        //    if (snippet == null)
        //        return false;

        //    //send the request and parse the response:
        //    string contentToSend = "content=" + snippet.Serialize(m_serialFormat);
        //    S2CRes resp = SendPostReq(CHANGE_SNIPPET_URL, contentToSend);

        //    //build the result:
        //    return ParseBoolResponse(resp);
        //}

        public bool UpdateNumVisits(long snippetID, int numVisits)
        {
            throw new NotImplementedException();
        }

        public bool UpdateNumCopyActions(long snippetID, int numCopyActions, bool embedded)
        {
            throw new NotImplementedException();
        }

        public bool RateSnippet(long snippetID, int rating, int previousRating = 0)
        {
            if ((snippetID <= 0) || (rating < 0))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}; rating={1}", snippetID, rating));
                return false;
            }

            //send the request and parse the response:
            string contentToSend = string.Format("snippetID={0}&rating={1}&previousRating={2}", snippetID, rating, previousRating);
            S2CResObj<object> resp = SendReqObj(RATE_SNIPPET_URL, contentToSend, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        public bool ShareSnippet(long snippetID, int groupID)
        {
            if ((snippetID <= 0) || (groupID < 0))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}; groupID={1}", snippetID, groupID));
                return false;
            }

            //send the request and parse the response:
            string contentToSend = string.Format("snippetID={0}&groupID={1}", snippetID, groupID);
            S2CResObj<object> resp = SendReqObj(SHARE_SNIPPET_URL, contentToSend, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        public bool PublishSnippet(long snippetID, ICollection<int> channelIDs)
        {
            if ((snippetID <= 0) || channelIDs.IsNullOrEmpty())
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}; channelIDs={1}", snippetID, channelIDs.PrintNull()));
                return false;
            }

            //send the request and parse the response:
            string content = Utilities.MergeIntoCommaSeparatedString(channelIDs, true);
            string contentToSend = string.Format("snippetID={0}&channelsIDs={1}", snippetID, content);
            S2CResObj<object> resp = SendReqObj(PUBLISH_SNIPPET_URL, contentToSend, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        public bool UnPublishSnippet(long snippetID, ICollection<int> channelIDs)
        {
            // 1) check input validity:
            if ((snippetID <= 0) || channelIDs.IsNullOrEmpty())
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}; channelIDs={1}", snippetID, channelIDs.PrintNull()));
                return false;
            }

            // 2) remove the channels ID as a property of the snippet:
            foreach (int channelID in channelIDs)
            {
                if (!DeleteSnippetProperty(new SnippetProperty(DefaultProperty.Channel, channelID + "", snippetID, false)))
                    return false;
            }

            return true;
        }

        public bool StoreHit(Snippet snippet)
        {
            //check input validity:
            if (snippet == null)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippet={0}", snippet.PrintNull()));
                return false;
            }

            //send the request and parse the response:
            S2CResObj<object> resp = SendReqObj("SnippetHit/" + snippet.ID, string.Empty, true, false);

            //build the result:
            return ParseBoolResponse(resp);
        }

        public bool StoreCopyAction(Snippet snippet, bool embedded)
        {
            //check input validity:
            if (snippet == null)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippet={0}", snippet.PrintNull()));
                return false;
            }

            //send the request and parse the response:
            S2CResObj<object> resp = SendReqObj("Explore/WhoCopy?snippetId=" + snippet.ID + "&embedded=" + embedded, 
                                string.Empty, true, false);

            //build the result:
            return ParseBoolResponse(resp);
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Delete Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool ClearSnippetTags(long snippetID)
        {
            if (snippetID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}", snippetID));
                return false;
            }

            //send the request and parse the response:
            string contentToSend = string.Format("snippetID={0}", snippetID);
            S2CResObj<object> resp = SendReqObj(CLEAR_TAGS_URL, contentToSend, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        public bool DeleteSnippetTags(long snippetID, ICollection<string> tagsToDelete)
        {
            if ((snippetID <= 0) || tagsToDelete.IsNullOrEmpty())
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, 
                    string.Format("Input error: snippetID={0}, tags={1}", snippetID, tagsToDelete.Print<string>()));
                return false;
            }

            //send the request and parse the response:
            S2CSerializer ser = new S2CSerializer(m_serialFormat, snippetID);
            string contentToSend = "content=" + HttpUtility.UrlEncode(ser.SerializeListObj<string>(tagsToDelete.ToArray()));
            S2CResObj<object> resp = SendReqObj(DELETE_TAGS_URL, contentToSend, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        public bool DeleteSnippet(long snippetID)
        {
            if (snippetID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}", snippetID));
                return false;
            }

            //send the request and parse the response:
            string contentToSend = string.Format("snippetID={0}", snippetID);
            S2CResObj<object> resp = SendReqObj(DELETE_SNIPPET_URL, contentToSend, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        public bool ClearSnippetProperties(long snippetID)
        {
            if (snippetID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: snippetID={0}", snippetID));
                return false;
            }

            //send the request and parse the response:
            string contentToSend = string.Format("snippetID={0}", snippetID);
            S2CResObj<object> resp = SendReqObj(CLEAR_PROPERTIES_URL, contentToSend, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        public bool DeleteSnippetProperty(long propertyID, long snippetID, string value)
        {
            if ((propertyID <= 0) || (snippetID <= 0) || string.IsNullOrEmpty(value))
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: property SnippetID={0}, ID={1}, Value={2}",
                                            snippetID, propertyID, value));
                return false;
            }

            //send the request and parse the response:
            string contentToSend = string.Format("propertyID={0}&snippetID={1}&value={2}",
                                    propertyID, snippetID, HttpUtility.UrlEncode(value));
            S2CResObj<object> resp = SendReqObj(DELETE_PROPERTY_URL, contentToSend, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        public bool DeleteSnippetProperty(SnippetProperty prop)
        {
            if ((prop == null) || !prop.DataAreValid())
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: property={0}", prop.PrintNull()));
                return false;
            }

            //send the request and parse the response:
            string contentToSend = string.Format("snippetID={0}&name={1}&value={2}",
                                        prop.SnippetID, HttpUtility.UrlEncode(prop.Name), HttpUtility.UrlEncode(prop.Value));
            S2CResObj<object> resp = SendReqObj(DELETE_PROPERTY_BYNAME_URL, contentToSend, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        public bool DeleteComment(long commentID)
        {
            if (commentID <= 0)
            {
                SetLastError(log, ErrorCodes.WRONG_INPUT, string.Format("Input error: commentID={0}", commentID));
                return false;
            }

            //send the request and parse the response:
            string contentToSend = string.Format("commentID={0}", commentID);
            S2CResObj<object> resp = SendReqObj(DELETE_COMMENT_URL, contentToSend, true);

            //build the result:
            return ParseBoolResponse(resp);
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Cache Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateCache(Snippet snippet)
        {
            //anything to do here until we implement the cache on client side...
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
