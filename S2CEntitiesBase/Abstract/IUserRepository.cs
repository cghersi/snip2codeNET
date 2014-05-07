//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using Snip2Code.Model.Entities;

namespace Snip2Code.Model.Abstract
{
    public interface IUserRepository
    {
        #region Get Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns an existing user given its ID. 
        /// </summary>
        /// <param name="id">user's id</param>
        /// <returns>the user with the given Id, or null </returns>
        User GetById(int id);

        /// <summary>
        ///	Returns an existing user by its username (nickname or email)
        /// </summary>
        /// <param name="userName">username or email of the given user</param>
        /// <returns>the user with the given username, or null</returns>
        User GetByUsername(string userName);

        /// <summary>
        /// Checks the given credentials searching for a user: if found, returns the User built with the info
        /// retrieved from DB
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>null if any error occurred or the credentials don't match</returns>
        User LoginUser(string email, string password);

        /// <summary>
        /// Checks the given credentials searching for a user: if found, returns the User built with the info
        /// retrieved from DB
        /// </summary>
        /// <param name="oneAllToken"></param>
        /// <returns>null if any error occurred or the credentials don't match</returns>
        User LoginUserOneAll(string oneAllToken);

        /// <summary>
        /// Checks if given nickname is unique, and if not tries to give some suggestions based on name and lastname...
        /// </summary>
        /// <param name="nickname">nickname to be checked</param>
        /// <param name="name">name (for suggestions)</param>
        /// <param name="lastname">lastname (for suggestions)</param>
        /// <returns>List with 0 elements if the nickname is unique, 
        ///         otherwise (if the nickname is already present in the DB) some suggestions; null if any error occurred</returns>
        IList<string> CheckUniqueNickName(string nickname, string name, string lastname);

        /// <summary>
        /// Retrieves the image of the given user as binary content (mime type is PNG)
        /// </summary>
        /// <param name="userID">ID of the owner of the wanted image</param>
        /// <param name="size">size of the image (it should be a square picture)</param>
        /// <returns>null if any error occurred or the credentials don't match</returns>
        byte[] GetUserImage(int userID, S2CImagesSize size);

        /// <summary>
        /// Retrieves the groups which the given user belongs to
        /// </summary>
        /// <param name="userID">user's id</param>
        /// <returns>the collection of user's groups if no error occurred; an empty collection otherwise</returns>
        ICollection<Group> GetGroupsOfUser(int userID);

        /// <summary>
        /// Retrieves the groups which the given user administers
        /// </summary>
        /// <param name="userID">user's id</param>
        /// <returns>the collection of user's groups if no error occurred; an empty collection otherwise</returns>
        ICollection<Group> GetAdministeredGroupsOfUser(int userID);

        /// <summary>
        /// Retrieves the channels which the given user is subscribed to
        /// </summary>
        /// <param name="userID">user's id</param>
        /// <returns>the collection of user's channels if no error occurred; an empty collection otherwise</returns>
        IList<Group> GetChannelsOfUser(int userID);

        /// <summary>
        /// Retrieves the IDs of the channels which the given user is subscribed to
        /// </summary>
        /// <param name="userID">user's id</param>
        /// <returns>the collection of user's channels if no error occurred; an empty collection otherwise</returns>
        IList<int> GetChannelIDsOfUser(int userID);

        /// <summary>
        /// Retrieves the channels which the given user administers
        /// </summary>
        /// <param name="userID">user's id</param>
        /// <returns>the collection of user's groups if no error occurred; an empty collection otherwise</returns>
        IList<Group> GetAdministeredChannelsOfUser(int userID);

        /// <summary>
        /// Retrieves the groups which the given user has a pending invitation for
        /// </summary>
        /// <param name="userID">user's id</param>
        /// <returns>the collection of user's groups if no error occurred; an empty collection otherwise</returns>
        IList<Group> GetPendingInvitationGroupsOfUser(int userID);

        /// <summary>
        /// Retrieves the OneAll tokens provided by the given user
        /// </summary>
        /// <param name="userID">user's id</param>
        /// <returns>the collection of user's tokens if no error occurred; an empty collection otherwise</returns>
        ICollection<OneAllToken> GetTokensOfUser(int userID);

        /// <summary>
        /// Retrieves the badges achieved by the given user
        /// </summary>
        /// <param name="snippetID">ID of the user</param>
        /// <returns>the list of badges related to this user; an empty list if not found or any error occurred</returns>
        List<Badge> GetBadges(int userID);

        /// <summary>
        /// Retrieves the badges achieved by the snippets created by the given user
        /// </summary>
        /// <param name="snippetID">ID of the user</param>
        /// <returns>the list of badges related to the snippets created by this user; an empty list if not found or any error occurred</returns>
        Dictionary<short, Tuple<int, DateTime>> GetSnippetBadges(int userID);

        /// <summary>
        /// Adds an email address to the table of requests for Beta subscription
        /// </summary>
        /// <param name="email">email to add</param>
        /// <returns>true if no error occurred, false otherwise</returns>
        bool AddSignupRequest(string email);

        /// <summary>
        /// Retrieve the list of all users, up to the given threshold
        /// </summary>
        /// <param name="count">max number of results</param>
        /// <returns>the collection of users if no error occurred; an empty collection otherwise</returns>
        IList<User> GetAllUsers(int count);

        /// <summary>
        /// Retrieve the list of IDs of all users, up to the given threshold
        /// </summary>
        /// <param name="count">max number of results</param>
        /// <returns>the collection of IDs of users if no error occurred; an empty collection otherwise</returns>
        IList<int> GetAllUserIDs(int count);

        /// <summary>
        /// Retrieves all the basic info of the users matching the given query
        /// </summary>
        /// <param name="query">part of nickname, email, first or last name</param>
        /// <param name="maxNum">max number of results</param>
        /// <returns>the collection of basic info of found users if no error occurred; an empty collection otherwise</returns>
        ICollection<UserBaseInfo> SearchUsers(string query, int maxNum = int.MaxValue);

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Create/Add/Modify/Delete Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Creates a new user with the given info. Such info should consist in a valid email and the actual information
        /// provided by the user when she creates the account. The user will be active from the very beginning.
        /// Before creating the new user, it checks that the email and the nickname are not yet present in the DB.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password">plain password (the encryption is done from the DB)</param>
        /// <param name="name"></param>
        /// <param name="lastName"></param>
        /// <param name="nickName"></param>
        /// <param name="hasValidMail">false if the user is created via third party login (e.g. Github, Facebook, etc.)</param>
        /// <param name="oneAllToken">user token if created via oneAll APIs</param>
        /// <param name="oneAllProvider">login provider if created via oneAll APIs</param>
        /// <param name="oneAllNickname">nickname used from thirdparty account if created via oneAll APIs</param>
        /// <param name="oneAllTokenIsValid">whether the input token is generated by OneAll if created via oneAll APIs (false if autogenerated by Snip2Code)</param>
        /// <param name="oneAllTokenIdent">unique token provided by the plugins to use OneAll system for the login</param>
        /// <returns>the ID of the newly created user; -1 if any error occurred</returns>
        /// <exception cref="EmailAlreadyPresentException"></exception>
        /// <exception cref="NicknameAlreadyPresentException"></exception>
        int CreateActiveUser(string email, string password, string name, string lastName, string nickName,
                                bool hasValidMail = true, string oneAllToken = "", string oneAllProvider = "",
                                string oneAllNickname = "", bool oneAllTokenIsValid = true, string oneAllTokenIdent = "");

        /// <summary>
        /// Creates a new (inactive) user with the given info. Such info should consist in a valid email and a security token. 
        /// The user will not be active until she will signed up.
        /// The nickname of the temporary user will be the email with the '@' replaced by '.'.
        /// The security token will be stored in the password field of the user.
        /// Before creating the new user, it checks that the email and the nickname are not yet present in the DB.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="securityToken">A random string to be matched when the user accepts the invitation</param>
        /// <returns></returns>
        int InviteUser(string email, string securityToken);

        /// <summary>
        /// Refreshes the security token for a not-yet-active user
        /// </summary>
        /// <param name="email">email fo the user to refresh</param>
        /// <param name="securityToken">new value of security token</param>
        /// <returns></returns>
        bool RefreshSecToken(string email, string securityToken);

        /// <summary>
        /// Turns active a previously invited user with the given info. 
        /// Such info should consist in a valid email and the actual information
        /// provided by the user when she creates the account.
        /// Before creating the new user, it checks that the nickname is not yet present in the DB, while the record
        /// with the given email and securityToken are already stored into the DB.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password">plain password (the encryption is done from the DB)</param>
        /// <param name="name"></param>
        /// <param name="lastName"></param>
        /// <param name="nickName"></param>
        /// <param name="securityToken"></param>
        /// <returns>the ID of the newly created user; -1 if any error occurred</returns>
        /// <exception cref="NicknameAlreadyPresentException"></exception>
        int ActivateUser(string email, string password, string name, string lastName, string nickName, string securityToken);

        /// <summary>
        /// Changes the profile of the user, checking that the nickname and the email, if changed, continues to be unique
        /// </summary>
        /// <param name="modified">record to be modified</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        /// <exception cref="EmailAlreadyPresentException"></exception>
        /// <exception cref="NicknameAlreadyPresentException"></exception>
        /// <exception cref="WrongDefaultGroupException"></exception>
        bool ChangeUserProfile(User modified);

        /// <summary>
        /// Changes the password of the given user
        /// </summary>
        /// <param name="username">can be both email or nickname</param>
        /// <param name="password">plain password (the encryption is done from the DB)</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool ChangeUserPsw(string username, string password);

        /// <summary>
        /// Changes the email of the given user, checking that the email, if changed, continues to be unique
        /// </summary>
        /// <param name="id">Id of the user to change</param>
        /// <param name="email">new email</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        /// <exception cref="EmailAlreadyPresentException"></exception>
        bool ChangeUserEMail(int id, string email);

        /// <summary>
        /// Changes the nickname of the given user, checking that the nickname, if changed, continues to be unique
        /// </summary>
        /// <param name="id">Id of the user to change</param>
        /// <param name="nickname">new nickname</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        /// <exception cref="NicknameAlreadyPresentException"></exception>
        bool ChangeUserNickname(int id, string nickname);

        /// <summary>
        /// Adds (or updates, if the user laready has a picture) the given image for the given user
        /// </summary>
        /// <param name="userID">ID of the user who the picture is related to</param>
        /// <param name="imageStream">content of the image to save</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        /// <exception cref="ArgumentException"></exception>
        bool SaveUserPicture(int userID, byte[] imageStream); // Stream imageStream);

        /// <summary>
        /// Adds the given amount of points to the given user
        /// </summary>
        /// <param name="userID">ID of the user gaining points</param>
        /// <param name="points">amount of points to add; it must be positive</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool AddPointsToUser(int userID, int points);

        /// <summary>
        /// Recalculates the total amount of points gained by the user
        /// </summary>
        /// <param name="userID">ID of the user gaining points</param>
        /// <returns>the total number of points of the user if no error occurred; -1 otherwise</returns>
        int RecalcPointsForUser(int userID);

        /// <summary>
        /// Synchronize the badge status of the given user
        /// </summary>
        /// <param name="userID">user to calculate badge status</param>
        /// <param name="badgeType">type of badge to calculate</param>
        /// <param name="reachedValue">amount to be compared to the badge threshold to determine, if any, 
        ///     the level of the gathered badge</param>
        /// <returns>true if the badge status changed, false otherwise</returns>
        bool SyncBadgeForUser(int userID, BadgeType badgeType, int reachedValue);

        /// <summary>
        /// Adds the given 3rd party connection to the given user
        /// </summary>
        /// <param name="userID">ID of the user connecting to the 3rd party service</param>
        /// <param name="thirdParty">3rd party service connected</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool Add3Party(int userID, User3Party thirdParty);

        /// <summary>
        /// Removes the given 3rd party connection from the given user
        /// </summary>
        /// <param name="userID">ID of the user un-connecting to the 3rd party service</param>
        /// <param name="thirdParty">3rd party service un-connected</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool Remove3Party(int userID, User3Party thirdParty);

        /// <summary>
        /// Removes the given user from the DB
        /// </summary>
        /// <param name="userID">ID of the user to remove</param>
        /// <returns>true if no error occurred; false otherwise</returns>
        bool DeleteUser(int userID);


        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Cache Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Updates the internal cache with the given object
        /// </summary>
        /// <param name="user">content to update</param>
        void UpdateCache(User user);

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}

