//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

using Snip2Code.Model.Comm;
using Snip2Code.Utils;
using Snip2Code.Model.Abstract;

namespace Snip2Code.Model.Entities
{
    
    public enum GroupJoinStatus
    {
        Pending = 0, 
        Active = 1, 
        Banned = -1,
        Unknown = -2
    }


    /// <summary>
    /// This is a POCO representing the association of a user to a given group
    /// </summary>
    public abstract class GroupUserInfo : BaseEntity
    {
        #region Private Members
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        private IGroupRepository m_repoGroup = null;
        private IUserRepository m_repoUser = null;

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////

        public int GroupID { get; set; }
        public int UserID { get; set; }
        public bool IsAdmin { get; set; }
        public GroupJoinStatus Status { get; set; }

        public User User
        {
            get
            {
                if ((UserID > 0) && (UserRepository != null))
                    return UserRepository.GetById(UserID);
                else
                    return null;
            }
        }

        public Group Group
        {
            get
            {
                if ((GroupID > 0) && (GroupRepository != null))
                    return GroupRepository.GetById(GroupID);
                else
                    return null;
            }
        }

        public bool IsCreator 
        { 
            get 
            { 
                int groupCreatorID = -2;
                if (Group != null)
                    groupCreatorID = Group.CreatorID;
                return (groupCreatorID == UserID);
            } 
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////

        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        public GroupUserInfo() 
        {
            GroupID = -1;
            UserID = -1;
            IsAdmin = false;
            Status = GroupJoinStatus.Unknown;
        }

        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        protected GroupUserInfo(string content, SerialFormat format) : base(content, format) { }

        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="userID"></param>
        /// <param name="isAdmin"></param>
        /// <param name="status"></param>
        protected void Init(int groupID, int userID, bool isAdmin, GroupJoinStatus status)
        {
            GroupID = groupID;
            UserID = userID;
            IsAdmin = isAdmin;
            Status = status;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected bool Init(GroupUserInfo objToCopy)
        {
            if (objToCopy == null)
                return false;

            //save the values of the properties with private setters:
            GroupID = objToCopy.GroupID;
            UserID = objToCopy.UserID;
            IsAdmin = objToCopy.IsAdmin;
            Status = objToCopy.Status;

            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override bool ParseFromJson(string content)
        {
            return Init(ParseFromJson<GroupUserInfoComm>(content));
        }

        public override bool Parse(XElement xml)
        {
            if (xml == null)
                return false;

            int groupID = 0;
            int userID = 0;
            bool isAdmin = false;
            GroupJoinStatus status = GroupJoinStatus.Unknown;

            xml.ParseNode("GroupID", ref groupID);
            xml.ParseNode("UserID", ref userID);        
            xml.ParseNode("IsAdmin", ref isAdmin);
            xml.ParseNode<GroupJoinStatus>("Status", ref status);

            Init(groupID, userID, isAdmin, status);

            return true;
        }

        public override BaseEntity ToBaseEntity()
        {
            Init(this);
            return this;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Serialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public override XElement ToXML(bool internalPurpose = true)
        {
            XDocument res = Utilities.CreateXMLDoc("GroupUserInfo", false);

            res.Root.Add(
                new XElement(ConfigReader.S2CNS + "GroupID", GroupID),
                new XElement(ConfigReader.S2CNS + "UserID", UserID),
                new XElement(ConfigReader.S2CNS + "IsAdmin", IsAdmin),
                new XElement(ConfigReader.S2CNS + "Status", Status)
            );

            return res.Root;
        }


        public override BaseEntity ToJSONEntity()
        {
            //set the values for properties with private setters using the helper class:
            GroupUserInfoComm json = new GroupUserInfoComm();
            json.Init(this);

            return json;
        }

        /// <summary>
        ///     override object.ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("GroupID={0};UserID={1};IsAdmin={2};Status={3}", GroupID, UserID, IsAdmin, Status);
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Repository
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected IUserRepository UserRepository
        {
            get
            {
                if (m_repoUser == null)
                    m_repoUser = RetrieveUserRepository();
                return m_repoUser;
            }
        }

        protected IGroupRepository GroupRepository
        {
            get
            {
                if (m_repoGroup == null)
                    m_repoGroup = RetrieveGroupRepository();
                return m_repoGroup;
            }
        }

        /// <summary>
        /// Retrieves the repository of the groups, used to lazy load info related to the members of this group.
        /// </summary>
        /// <returns>null if any error occurred or not found</returns>
        protected abstract IGroupRepository RetrieveGroupRepository();

        /// <summary>
        /// Retrieves the repository of the user, used to lazy load info related to the members of this group.
        /// </summary>
        /// <returns>null if any error occurred or not found</returns>
        protected abstract IUserRepository RetrieveUserRepository();

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}
