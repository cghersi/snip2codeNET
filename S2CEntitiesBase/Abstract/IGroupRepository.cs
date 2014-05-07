//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Snip2Code.Model.Entities;

namespace Snip2Code.Model.Abstract
{
    public interface IGroupRepository
    {
        /// <summary>
        /// Presents the last error arosen during the exceution of the class methods
        /// </summary>
        string LastErrorMsg { get; }

        #region Get Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns an existing group given its ID. 
        /// </summary>
        /// <param name="id">group's id</param>
        /// <returns>the group with the given Id, or null </returns>
        Group GetById(int id);

        /// <summary>
        /// Returns an existing group given its name. 
        /// </summary>
        /// <param name="name">group's name</param>
        /// <returns>the group with the given Name, or null </returns>
        Group GetByName(string name);

        /// <summary>
        /// Returns all the IDs of the administrators of the given group
        /// </summary>
        /// <param name="groupID">ID of the group which administrators are to be retrieved</param>
        /// <returns>the list of admin IDs if no eror occurred, an empty set otherwise</returns>
        IList<int> GetAdminIDs(int groupID);

        /// <summary>
        /// Retrieves the users part of the given group
        /// </summary>
        /// <param name="groupID">the id of the group</param>
        /// <returns> A list of couples userId-isAdmin </returns>
        IList<GroupUserInfo> GetUsers(int groupID);

        /// <summary>
        /// Retrieves all the existing channels
        /// </summary>
        /// <param name="orderBy">type of ordering of the results</param>
        /// <param name="orderDesc">true for descendant order, false otherwise</param>
        /// <param name="totNum">total number of found items with no pagination</param>
        /// <param name="start">first element to consider</param>
        /// <param name="count">max number of items in output</param>
        /// <returns>the list of existing channels, an empty set otherwise</returns>
        IList<Group> GetAllChannels(ChannelSort orderBy, bool orderDesc, out int totNum, int start = 0, int count = int.MaxValue);

        /// <summary>
        /// Retrieves all the existing channels matching the given search text.
        /// Search is performed on Name and Description fields.
        /// </summary>
        /// <param name="searchText">searchable among name and description of the group</param>
        /// <param name="orderBy">type of ordering of the results</param>
        /// <param name="orderDesc">true for descendant order, false otherwise</param>
        /// <param name="totNum">total number of found items with no pagination</param>
        /// <param name="start">first element to consider</param>
        /// <param name="count">max number of items in output</param>
        /// <returns>the list of existing channels matching the search, an empty set otherwise</returns>
        IList<Group> SearchChannels(string searchText, ChannelSort orderBy, bool orderDesc, out int totNum, int start = 0, int count = int.MaxValue);

        /// <summary>
        /// Retrieves all the children of the given group.
        /// </summary>
        /// <param name="parentGroupID">ID of the group whose children are being searched</param>
        /// <returns>the list of children of the given group, an empty set otherwise</returns>
        IList<int> GetChildrenIDs(int parentGroupID);

        /// <summary>
        /// Retrieves all the parents of the given group.
        /// </summary>
        /// <param name="childGroupID">ID of the group whose parents are being searched</param>
        /// <returns>the list of parents of the given group, an empty set otherwise</returns>
        IList<int> GetParentIDs(int childGroupID);

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Create/Add/Modify/Delete Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Creates a new group with the given info. Such info should consist in a valid name and the actual information
        /// provided by the admin when she creates the account. The group will be active from the very beginning.
        /// Before creating the new group, it checks that the name is not yet present in the DB.
        /// </summary>
        /// <param name="name">The unique name of the group</param>
        /// <param name="adminID">the ID of the first user that administer the group</param>
        /// <param name="policy">manages the way in which the snippet owned by this group are published 
        /// and/or available to other users</param>
        /// <param name="preferences">miscellaneous preferences for this group</param>
        /// <param name="shortDescription">a short description of the group, used to introduce this group to other users (MAX 128 chars)</param>
        /// <param name="description">a long description of the group, used to better explain and present deep features or aspects of the group to other users</param>
        /// <returns>the ID of the newly created group; -1 if any error occurred</returns>
        /// <exception cref="NicknameAlreadyPresentException"></exception>
        int CreateGroup(string name, int adminID, GroupPolicy policy, GroupPreferences preferences, string shortDescription, string description);

        /// <summary>
        /// Creates a new channel with the given info. Such info should consist in a valid name and the actual information
        /// provided by the admin when she creates the account. The channel will be active from the very beginning.
        /// Before creating the new channel, it checks that the name is not yet present in the DB.
        /// </summary>
        /// <param name="name">The unique name of the channel</param>
        /// <param name="adminID">the ID of the first user that administer the channel</param>
        /// <param name="preferences">miscellaneous preferences for this channel</param>
        /// <param name="shortDescription">a short description of the channel, used to introduce this channel to other users (MAX 128 chars)</param>
        /// <param name="description">a long description of the channel, used to better explain and present deep features or aspects of the channel to other users</param>
        /// <returns>the ID of the newly created channel; -1 if any error occurred</returns>
        /// <exception cref="NicknameAlreadyPresentException"></exception>
        int CreateChannel(string name, int adminID, GroupPreferences preferences, string shortDescription, string description);

        /// <summary>
        /// Changes the profile of the group, checking that the name, if changed, continues to be unique
        /// </summary>
        /// <param name="modified">record to be modified</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        /// <exception cref="NicknameAlreadyPresentException"></exception>
        bool ChangeGroupProfile(Group modified);

        /// <summary>
        /// Adds another user to the given group; if this user already belongs to this group, 
        /// changes the "IsAdmin" property in order to allow or revoke the administrative credentials for this group 
        /// and/or the "Status" property
        /// </summary>
        /// <param name="id">Id of the group to change</param>
        /// <param name="newUserID">ID of the new user</param>
        /// <param name="isAdmin">whether this user is an administrator for this group or not</param>
        /// <param name="status">the joining status of the user to be added</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool AddGroupUser(int id, int newUserID, bool isAdmin, GroupJoinStatus status);

        /// <summary>
        /// Subscribes the given user to the given channel
        /// </summary>
        /// <param name="channelID">ID of the channel to join</param>
        /// <param name="newUserID">ID of the user that wants to subscribe</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool JoinChannel(int channelID, int newUserID);

        /// <summary>
        /// Adds the given list of users in Pending status to the given group. All these users will be normal users,
        /// not admin
        /// </summary>
        /// <param name="groupID">Id of the group to change</param>
        /// <param name="newUserIDs">List of IDs of the new users that will join the group</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool InviteUsers(int groupID, IList<int> newUserIDs);

        /// <summary>
        /// Adds the parent/child relationship between the given groups
        /// </summary>
        /// <param name="childID">Child group of the relationship</param>
        /// <param name="parentID">Parent group of the relationship</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool AddChildGroup(int childID, int parentID);

        /// <summary>
        /// Removes the given user from the given group
        /// </summary>
        /// <param name="groupID">Id of the group to change</param>
        /// <param name="userID"></param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool RemoveGroupUser(int groupID, int userID);

        /// <summary>
        /// Removes the parent/child relationship between the given groups
        /// </summary>
        /// <param name="childID">Child group of the relationship</param>
        /// <param name="parentID">Parent group of the relationship</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool RemoveChildGroup(int childID, int parentID);

        /// <summary>
        /// Removes the given group and all related data
        /// </summary>
        /// <param name="groupID">Id of the group to remove</param>
        /// <param name="onlyData">true to remove only snippets owned by this group; false to remove also the group record from DB</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool DeleteGroup(int groupID, bool onlyData);

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Cache Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Updates the internal cache with the given object
        /// </summary>
        /// <param name="group">content to update</param>
        void UpdateCache(Group group);

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }



    public enum ChannelSort
    {
        alphabetically = 0,
        snippetsNum = 1,
        followersNum = 2,
        lastModifiedSnippet = 3
    }
}
