//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Snip2Code.Model.Entities;

namespace Snip2Code.Model.Abstract
{
    public interface ISnippetsRepository
    {
        /// <summary>
        /// Presents the last error arosen during the exceution of the class methods
        /// </summary>
        string LastErrorMsg { get; }

        /// <summary>
        /// Gets or Sets the ID of the user that performs all the operations related to the snippets
        /// </summary>
        int CurrentUserID { get; set; }


        #region Get Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Retrieves the snippets (visible to the given user) matching the given search
        /// </summary>
        /// <param name="searchText">text to match</param>
        /// <param name="misSpellings">the eventual dictionary of misspelled words in the search text: 
        ///     keys are wrong words, values are the related correct term</param>
        /// <param name="totNum">returns the total number of items matching the given search 
        ///     (on server-side, without pagination; on client-side it is just the number of returned results)</param>
        /// <param name="totNumWithNonDefOp">returns the total number of items matching the given search with the non-default operator 
        ///     (on server-side, without pagination; on client-side it is just the number of returned results)</param>
        /// <param name="tagsOccurrences">Occurrences of most popular 20 tags in the given resultset</param>
        /// <param name="maxNum">maximum number of items to retrieve</param>
        /// <param name="start">start index for paginated retrieval</param>
        /// <param name="onlyCreated">true to retrieve only the snippets created by the given user</param>
        /// <param name="onlyOfGroup">specify here the ID of a group in order to retrieve only the snippets owned by this group</param>
        /// <param name="field">sorting field</param>
        /// <param name="direction">sorting direction</param>
        /// <returns>the list of found snippets; an empty set if not found</returns>
        IList<Snippet> GetSnippetsForSearch(string searchText, out Dictionary<string, string[]> misSpellings, out int totNum,
            out int totNumWithNonDefOp, out SortedDictionary<string, int> tagsOccurrences, int maxNum = int.MaxValue, int start = 0, bool onlyCreated = false, int onlyOfGroup = 0,
            ItemSortField field = ItemSortField.Relevance, SortDirection direction = SortDirection.Descent);

        /// <summary>
        /// Retrieves the snippets (visible to the given groups) matching the given search
        /// </summary>
        /// <param name="searchText">text to match</param>
        /// <param name="misSpellings">the eventual dictionary of misspelled words in the search text: 
        ///     keys are wrong words, values are the related correct term</param>
        /// <param name="groups">specify here the ID of the groups in order to retrieve only the snippets owned by these groups</param>
        /// <param name="totNum">returns the total number of items matching the given search 
        ///     (on server-side, without pagination; on client-side it is just the number of returned results)</param>
        /// <param name="totNumWithNonDefOp">returns the total number of items matching the given search with the non-default operator 
        ///     (on server-side, without pagination; on client-side it is just the number of returned results)</param>
        /// <param name="tagsOccurrences">Occurrences of most popular 20 tags in the given resultset</param>
        /// <param name="maxNum">maximum number of items to retrieve</param>
        /// <param name="start">start index for paginated retrieval</param>
        /// <param name="field">sorting field</param>
        /// <param name="direction">sorting direction</param>
        /// <returns>the list of found snippets; an empty set if not found</returns>
        IList<Snippet> GetSnippetsForSearch(string searchText, int[] groups, out Dictionary<string, string[]> misSpellings, out int totNum,
            out int totNumWithNonDefOp, out SortedDictionary<string, int> tagsOccurrences, int maxNum = int.MaxValue, int start = 0, ItemSortField field = ItemSortField.Relevance, 
            SortDirection direction = SortDirection.Descent);

        /// <summary>
        /// Retrieves the given snippet if it is visible to the given user
        /// </summary>
        /// <param name="snippetID">ID of the snippet to retrieve</param>
        /// <returns>null if the user is not allowed to see the snippet or the snippet hasn't been found</returns>
        Snippet GetSnippetByID(long snippetID);

        /// <summary>
        ///     Finds a snippet visible to the given user matching the given name
        /// </summary>
        /// <param name="name">name of the snippet</param>
        /// <returns> null if the snippet cannot be found or is inaccessible by the user</returns>
        Snippet GetSnippetByName(string name);

        /// <summary>
        ///     Finds a snippet ID visible to the given user matching the given itemName
        /// </summary>
        /// <param name="itemName">name of the snippet</param>
        /// <returns>the snippet id, or -1 in case of not found or exception</returns>
        long GetSnippetIDByName(string itemName);

        /// <summary>
        ///     Checks whether the snippet with the given name and code, created by the current user, already exists or not
        /// </summary>
        /// <param name="itemName">name of the snippet to check</param>
        /// <param name="code">content of the snippet to check</param>
        /// <returns>true if the snippet already exists, false otherwise</returns>
        bool CheckExistenceByNameAndCode(string itemName, string code);

        /// <summary>
        ///     Efficient FindItems function
        ///     Retrieve all items which IDs are in the given array. 
        ///     Note: uses the Global cache to retrieve items already in memory.
        /// </summary>
        /// <param name="snippetIDs">list of IDs of the snippets to be retrieved</param>
        /// <param name="keepOrdering">true to have the result ordered exactly as the input list;
        ///     false to have the result ordered from the newer to the older snippet</param>
        /// <returns>the list of snippets accessible to the user; an empty list if an error occurred</returns>
        IList<Snippet> FindItems(long[] snippetIDs, bool keepOrdering = false);

        /// <summary>
        ///		Retrieves all the items visible to a given user, paginated according to the given parameters
        /// </summary>
        /// <param name="totNum">returns the total number of items matching the given parameters 
        ///     (on server-side, without pagination; on client-side it is just the number of returned results)</param>
        /// <param name="start">the initial index of the returned list</param>
        /// <param name="count">the maximum number of wanted snippets</param>
        /// <param name="onlyOwned">true to retrieve only the owned items (private and visible to the group)</param>
        /// <returns>the list of snippets accessible to the user; an empty list if an error occurred</returns>
        IList<Snippet> FindItems(out int totNum, int start = 0, int count = int.MaxValue, bool onlyOwned = false);

        /// <summary>
        ///		Retrieves all the IDs of the items visible to a given user, paginated according to the given parameters
        /// </summary>
        /// <param name="start">the initial index of the returned list</param>
        /// <param name="count">the maximum number of wanted snippets</param>
        /// <param name="onlyOwned">true to retrieve only the owned items (private and visible to the group)</param>
        /// <param name="totNum">returns the total number of items matching the given parameters 
        ///     (on server-side, without pagination; on client-side it is just the number of returned results)</param>
        /// <returns>the list of IDs of the snippets accessible to the user; an empty list if an error occurred</returns>
        IList<long> FindItemsID(int start, int count, bool onlyOwned, out int totNum);

        /// <summary>
        ///		Retrieves all the IDs of the public items (visible to everyone)
        /// </summary>
        /// <param name="start">the initial index of the returned list</param>
        /// <param name="count">the maximum number of wanted snippets</param>
        /// <param name="minRelevance">minimum relevance of the retrieved snippets (check SnippetRelevance)</param>
        /// <param name="totNum">returns the total number of items 
        ///     (on server-side, without pagination; on client-side it is just the number of returned results)</param>
        /// <returns>the list of IDs of the public snippets; an empty list if an error occurred</returns>
        IList<long> FindPublicItemsID(int start, int count, int minRelevance, out int totNum);

        /// <summary>
        ///		Retrieves all the public items (visible to everyone)
        /// </summary>
        /// <param name="start">the initial index of the returned list</param>
        /// <param name="count">the maximum number of wanted snippets</param>
        /// <param name="totNum">returns the total number of items 
        ///     (on server-side, without pagination; on client-side it is just the number of returned results)</param>
        /// <returns>the list of the public snippets; an empty list if an error occurred</returns>
        IList<Snippet> FindPublicItems(int start, int count, out int totNum);

        /// <summary>
        ///		Retrieves all the private items (created by the current user and not shared to anybody)
        /// </summary>
        /// <param name="start">the initial index of the returned list</param>
        /// <param name="count">the maximum number of wanted snippets</param>
        /// <param name="totNum">returns the total number of items 
        ///     (on server-side, without pagination; on client-side it is just the number of returned results)</param>
        /// <returns>the list of the private snippets; an empty list if an error occurred</returns>
        IList<Snippet> FindPersonalItems(int start, int count, out int totNum);

        /// <summary>
        ///		Retrieves all the items owned by the given group
        /// </summary>
        /// <param name="start">the initial index of the returned list</param>
        /// <param name="count">the maximum number of wanted snippets</param>
        /// <param name="groupID">
        ///     the ID of the group which snippets should be retrieved for; 
        ///     -1 to retrieve the snippets of all the groups of the current user
        /// </param>
        /// <param name="totNum">returns the total number of items for the given group 
        ///     (on server-side, without pagination; on client-side it is just the number of returned results)</param>
        /// <returns>the list of the protected snippets for the given group; an empty list if an error occurred</returns>
        IList<Snippet> FindGroupItems(int start, int count, int groupID, out int totNum);

        /// <summary>
        ///		Retrieves all the items owned by the given groups
        /// </summary>
        /// <param name="start">the initial index of the returned list</param>
        /// <param name="count">the maximum number of wanted snippets</param>
        /// <param name="groupIDs">the IDs of the groups which snippets should be retrieved for</param>
        /// <param name="totNum">returns the total number of items for the given groups 
        ///     (on server-side, without pagination; on client-side it is just the number of returned results)</param>
        /// <param name="field">sorting field</param>
        /// <param name="direction">sorting direction</param>
        /// <returns>the list of the protected snippets for the given groups; an empty list if an error occurred</returns>
        IList<Snippet> FindGroupItems(int start, int count, int[] groupIDs, out int totNum,
            ItemSortField field = ItemSortField.Relevance, SortDirection direction = SortDirection.Descent);

        /// <summary>
        ///		Retrieves all the items published in the given channels
        /// </summary>
        /// <param name="start">the initial index of the returned list</param>
        /// <param name="count">the maximum number of wanted snippets</param>
        /// <param name="channelsIDs">the IDs of the channels which snippets should be retrieved for</param>
        /// <param name="totNum">returns the total number of items for the given channels 
        ///     (on server-side, without pagination; on client-side it is just the number of returned results)</param>
        /// <param name="field">sorting field</param>
        /// <param name="direction">sorting direction</param>
        /// <param name="findAllChannels">true to find snippets published on at least one channel (overrides the list in channelIDs)</param>
        /// <returns>the list of the snippets published on the given channels; an empty list if an error occurred</returns>
        IList<Snippet> FindChannelsItems(int start, int count, ICollection<int> channelsIDs, out int totNum,
                ItemSortField field = ItemSortField.Relevance, SortDirection direction = SortDirection.Descent,
                bool findAllChannels = false);

        /// <summary>
        /// Retrieves the IDs of the items created by the current user
        /// </summary>
        /// <param name="totNum">returns the total number of items for the given groups 
        ///     (on server-side, without pagination; on client-side it is just the number of returned results)</param>
        /// <param name="start">the initial index of the returned list</param>
        /// <param name="count">the maximum number of wanted snippets</param>
        /// <returns></returns>
        IList<long> FindCreatedItemsID(out int totNum, int start = 0, int count = int.MaxValue);

        /// <summary>
        /// Retrieves the comment with the given ID
        /// </summary>
        /// <param name="commentID">ID of the comment</param>
        /// <returns>the wanted comment; null if not found or any error occurred</returns>
        SnippetComment GetComment(long commentID);

        /// <summary>
        /// Retrieves the comments related to the given snippet
        /// </summary>
        /// <param name="snippetID">ID of the snippet</param>
        /// <returns>the list of comments related to this snippet; an empty list if not found or any error occurred</returns>
        ICollection<SnippetComment> GetComments(long snippetID);

        /// <summary>
        /// Retrieves the number of comments related to the given snippet
        /// </summary>
        /// <param name="snippetID">ID of the snippet</param>
        /// <returns>the number of comments related to this snippet; -1 if not found or any error occurred</returns>
        int GetCommentsCount(long snippetID);

        /// <summary>
        /// Retrieves the tags related to the given snippet
        /// </summary>
        /// <param name="snippetID">ID of the snippet</param>
        /// <returns>the list of tags related to this snippet; an empty list if not found or any error occurred</returns>
        ICollection<Tag> GetTags(long snippetID);

        /// <summary>
        /// Retrieves the properties related to the given snippet
        /// </summary>
        /// <param name="snippetID">ID of the snippet</param>
        /// <returns>the list of properties related to this snippet; an empty list if not found or any error occurred</returns>
        ICollection<SnippetProperty> GetProperties(long snippetID);

        /// <summary>
        /// Retrieves the IDs of the channels where the given snippet has been published to
        /// </summary>
        /// <param name="snippetID">ID of the snippet</param>
        /// <returns>the list of IDs of the channels where the given snippet has been published to; an empty list if not found or any error occurred</returns>
        ICollection<int> GetChannels(long snippetID);

        /// <summary>
        /// Retrieves the badges related to the given snippet
        /// </summary>
        /// <param name="snippetID">ID of the snippet</param>
        /// <returns>the list of badges related to this snippet; an empty list if not found or any error occurred</returns>
        List<Badge> GetBadges(long snippetID);

        /// <summary>
        /// Retrieves the list of the most correlated snippets with the given one, ordered by correlation rank
        /// </summary>
        /// <param name="snippetID">ID of the snippet</param>
        /// <returns>the list of most correlated snippets; an empty list if not found or any error occurred</returns>
        ICollection<Snippet> GetMostCorrelatedSnippets(long snippetID);

        /// <summary>
        /// Retrieves the total number of visible snippets by the current user owned by the given group
        /// </summary>
        /// <param name="groupID">-1 to have the total count of the snippets of the system; 0 to have the total count of the snippets visible to the given user</param>
        /// <returns>-1 if any error occurred; the snippet count otherwise</returns>
        int CountSnippets(int groupID = -1);

        /// <summary>
        /// Returns the vote given by the current user to the given snippet
        /// </summary>
        /// <param name="snippetID">ID of the snippet to check</param>
        /// <returns>the actual vote if the user already voted this snippet, 0 otherwise</returns>
        int GetSnippetVote(long snippetID);

        #endregion 
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        /////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Create/Add/Modify Methods

        /// <summary>
        /// Adds a new snippet to the DB with the given information, or update an existing one if it has ID > 0.
        /// The creator is the current user.
        /// </summary>
        /// <param name="snippet">snippet to add/update</param>
        /// <returns>the ID of the newly created/updated snippet; -1 if any error occurred</returns>
        long SaveSnippet(Snippet snippet);

        /// <summary>
        /// Adds the given tags to the given snippet. The current tags are preserved.
        /// </summary>
        /// <param name="snippetID">ID of the snippet which the tags refer to</param>
        /// <param name="tags">collection of tags to be added</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool AddSnippetTags(long snippetID, ICollection<string> tags);

        /// <summary>
        /// Adds the given properties to the given snippet
        /// </summary>
        /// <param name="snippetID">ID of the snippet which the tags refer to</param>
        /// <param name="properties">collection of properties to be added, they needs only the name and the value</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool AddSnippetProperties(long snippetID, ICollection<SnippetProperty> properties);

        /// <summary>
        /// Adds the given property to the given snippet.
        /// If an existing property is found (see oldPropValue parameter), it is updated with the new value,
        /// otherwise a new entry is added to the snippet.
        /// </summary>
        /// <param name="snippetID">ID of the snippet which the property refer to</param>
        /// <param name="propName">name of the property</param>
        /// <param name="propValue">value of the property</param>
        /// <param name="oldPropValue">previous value of the property, if any. 
        ///     If specified, the method tries to update the given old property with the new values.</param>
        /// <returns>the ID of the property in the DB; -1 if any error occurred</returns>
        long AddSnippetProperty(long snippetID, string propName, string propValue, string oldPropValue = "");

        /// <summary>
        /// Adds the given comment to the given snippet
        /// </summary>
        /// <param name="snippetID">ID of the snippet which the comment refer to</param>
        /// <param name="comment">text of the comment</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool AddSnippetComment(long snippetID, string comment);

        ///// <summary>
        ///// Changes the profile of the snippet
        ///// </summary>
        ///// <param name="snippet">record to be modified</param>
        ///// <returns>true if no error occurred; false otherwise</returns>
        //bool ChangeSnippet(Snippet snippet);

        /// <summary>
        /// Votes the given snippet: the "Rating" property is the algebric sum of all the votes, 
        /// while "NumVotes" property is the total amount of votes given by the users
        /// </summary>
        /// <param name="snippetID">ID of the snippet to vote</param>
        /// <param name="rating">the actual value of the vote</param>
        /// <param name="previousRating">the previous vale of the vote (if available) that the user already gave 
        ///     to the snippet. The user can change his vote (e.g. from negative to positive or viceversa)
        ///     but he cannot vote twice with the same sign.</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool RateSnippet(long snippetID, int rating, int previousRating = 0);

        /// <summary>
        /// Updates onto DB the number of visits the snippet received
        /// </summary>
        /// <param name="snippetID">ID of the snippet to update</param>
        /// <param name="numVisits">the actual value of the visits</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool UpdateNumVisits(long snippetID, int numVisits);

        /// <summary>
        /// Updates onto DB the number of copy actions the snippet received
        /// </summary>
        /// <param name="snippetID">ID of the snippet to update</param>
        /// <param name="numCopyActions">the actual value of the copy actions</param>
        /// <param name="embedded">true to update the number of copies of iframe code for embedded widget; 
        ///     false to signal the number of copies of actual content</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool UpdateNumCopyActions(long snippetID, int numCopyActions, bool embedded);

        /// <summary>
        /// Shares the given snippet with the given group.
        /// </summary>
        /// <param name="snippetID">ID of the snippet to share</param>
        /// <param name="groupID">ID of the group receiving the share</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool ShareSnippet(long snippetID, int groupID);

        /// <summary>
        /// Publishes the given snippet in the given channels.
        /// </summary>
        /// <param name="snippetID">ID of the snippet to publish</param>
        /// <param name="channelIDs">list of IDs of the channels receiving the public snippet</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool PublishSnippet(long snippetID, ICollection<int> channelIDs);

        /// <summary>
        /// Un-Publishes the given snippet from the given channels.
        /// </summary>
        /// <param name="snippetID">ID of the snippet to unpublish</param>
        /// <param name="channelIDs">list of IDs of the channels removed from the public snippet</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool UnPublishSnippet(long snippetID, ICollection<int> channelIDs);

        /// <summary>
        /// Adds a +1 to the number of visits received by the snippet.
        /// </summary>
        /// <param name="snippet">snippet to count a hit for</param>
        /// <returns>false if any error occurred; true otherwise</returns>
        bool StoreHit(Snippet snippet);

        /// <summary>
        /// Adds a +1 to the number of copy actions performed on the snippet.
        /// </summary>
        /// <param name="snippet">snippet to count a hit for</param>
        /// <param name="embedded">true to update the number of copies of iframe code for embedded widget; 
        ///     false to signal the number of copies of actual content</param>
        /// <returns>false if any error occurred; true otherwise</returns>
        bool StoreCopyAction(Snippet snippet, bool embedded);

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        /////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Delete Methods

        /// <summary>
        /// Removes all the tags from the given snippet
        /// </summary>
        /// <param name="snippetID">ID of the snippet which the tags refer to</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool ClearSnippetTags(long snippetID);

        /// <summary>
        /// Removes the specified tags from the given snippet
        /// </summary>
        /// <param name="snippetID">ID of the snippet which the tags refer to</param>
        /// <param name="tagsToDelete">tags to remove</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool DeleteSnippetTags(long snippetID, ICollection<string> tagsToDelete);

        /// <summary>
        /// Removes the given snippet
        /// </summary>
        /// <param name="snippetID">ID of the snippet to remove</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool DeleteSnippet(long snippetID);

        /// <summary>
        /// Removes all the porperties from the given snippet
        /// </summary>
        /// <param name="snippetID">ID of the snippet which the properties refer to</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool ClearSnippetProperties(long snippetID);

        /// <summary>
        /// Removes the given property from the related snippet
        /// </summary>
        /// <param name="propertyID">ID of the property name to remove</param>
        /// <param name="snippetID">ID of the snippet which the property should be removed from</param>
        /// <param name="value">value of the property to remove</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool DeleteSnippetProperty(long propertyID, long snippetID, string value);

        /// <summary>
        /// Removes the given property from the related snippet
        /// </summary>
        /// <param name="prop">property to remove</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool DeleteSnippetProperty(SnippetProperty prop);

        /// <summary>
        /// Removes the given comment
        /// </summary>
        /// <param name="snippetID">ID of the comment to remove</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool DeleteComment(long commentID);

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Cache Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Updates the internal cache with the given object
        /// </summary>
        /// <param name="snippet">content to update</param>
        void UpdateCache(Snippet snippet);

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }


    public enum ItemSortField
    {
        Relevance = 0,
        Created = 1,
        Modified = 2,
        Name = 3,
        Rating = 4,
        AvgRating = 5,
        Visibility = 6,
        AuthorRanking = 7,
        NumVote = 8,
        NumOfVisits = 9,
        NumOfCopies = 10,
    }

    public enum SortDirection
    {
        Ascent,
        Descent,
        NoDir
    }
}
