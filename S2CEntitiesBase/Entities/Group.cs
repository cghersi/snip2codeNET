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
    public enum GroupPolicy
    {
        Personal = 0,   // the fake group composed by a single user, who is also admin of it; needed to state a private snippet
        OpenSource = 1, // a group that can own only public snippets, OBSOLETE
        Company = 2,     // usual group: it pays a periodic fee and is completely responsible of the visibility of its snippets
        Channel = 3     // a group which can be joined by all Snip2Code users. It doesn't own any snippet, but the members may 
                        //be notified upon new snippet publishing onto this channel
    }

    public abstract class Group : BaseEntity
    {
        public const int MIN_NAME_LEN = 3;
        public const int MAX_NAME_LEN = 50;

        public const int MAX_SHORTDESCR_LEN = 128;

        #region Private Members
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        internal ICollection<GroupUserInfo> m_members;
        internal ICollection<int> m_childrenGroupIDs;
        internal ICollection<int> m_parentGroupIDs;
        private IGroupRepository m_repoGroup = null;
        private IUserRepository m_repoUser = null;
        private int m_membersCount = 0;

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        [XmlIgnore]
        public int ID { get; private set; }
       
        public string Name { get; set; }

        public GroupPolicy Policy { get; set; }
        [XmlIgnore]
        public GroupPreferences Preferences { get; private set; }
        [XmlIgnore]
        public string ShortDescription { get; set; }
        [XmlIgnore]
        public string Description { get; set; }
        [XmlIgnore]
        public int CreatorID { get; private set; }

        /// <summary>
        /// Available only for channels; -1 if not available
        /// </summary>
        public int NumOfSnippets { get; set; }
        [XmlIgnore]
        public int MembersCount
        {
            get
            {
                if (m_membersCount == 0)
                {
                    //try to guess from members collection, if available:
                    if (Members != null)
                        m_membersCount = Members.Count;
                }
                return m_membersCount;
            }
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Properties Validity Checks
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Checks whether the given name is valid or not as a group name
        /// </summary>
        /// <param name="candidateName"></param>
        /// <returns></returns>
        public static bool GroupNameIsValid(string candidateName)
        {
            return Utilities.PropValueIsValid(candidateName, MIN_NAME_LEN, false, MAX_NAME_LEN);
        }

        /// <summary>
        /// Checks whether the given input is valid or not as a group short description
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public static bool GroupShortDescrIsValid(string candidate)
        {
            return Utilities.PropValueIsValid(candidate, 0, true, MAX_SHORTDESCR_LEN);
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////

        [XmlIgnore]
        public ICollection<GroupUserInfo> Members
        {
            get
            {
                if ((GroupRepository != null) && (m_members == null) && (this.ID > 0))
                {
                    m_members = GroupRepository.GetUsers(this.ID);
                    if (m_members != null)
                        GroupRepository.UpdateCache(this);
                }
                return m_members;
            }
        }

        [XmlIgnore]
        public ICollection<int> ChildrenGroupIDs
        {
            get
            {
                if ((GroupRepository != null) && (m_childrenGroupIDs == null) && (this.ID > 0))
                {
                    m_childrenGroupIDs = GroupRepository.GetChildrenIDs(this.ID);
                    if (m_childrenGroupIDs != null)
                        GroupRepository.UpdateCache(this);
                }
                return m_childrenGroupIDs;
            }
        }

        [XmlIgnore]
        public ICollection<int> ParentGroupIDs
        {
            get
            {
                if ((GroupRepository != null) && (m_parentGroupIDs == null) && (this.ID > 0))
                { 
                    m_parentGroupIDs = GroupRepository.GetParentIDs(this.ID);
                    if (m_parentGroupIDs != null)
                        GroupRepository.UpdateCache(this);
                }
                return m_parentGroupIDs;
            }
        }

        public GroupUserInfo GetUserInfo(int userID)
        {
            if (Members == null)
                return null;
            
            foreach (GroupUserInfo info in Members)
            {
                if (info.UserID == userID)
                    return info;
            }

            return null;
        }

        [XmlIgnore]
        public User Administrator
        {
            get
            {
                if (!Members.IsNullOrEmpty())
                {
                    foreach (GroupUserInfo memb in Members)
                    {
                        if ((memb != null) && memb.IsAdmin)
                            return memb.User;
                    }
                }

                return null;
            }
        }


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        protected Group() : base() { Preferences = new GroupPreferences(); }

        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        protected Group(string content, SerialFormat format) : base(content, format) { }

        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="policy"></param>
        /// <param name="prefs"></param>
        /// <param name="shortDescr"></param>
        /// <param name="description"></param>
        /// <param name="creatorID"></param>
        /// <param name="numOfSnippets"></param>
        protected void Init(int id, string name, GroupPolicy policy, GroupPreferences prefs, string shortDescr, string description, int creatorID, int numOfSnippets)
        {
            ID = id;
            Name = name;
            Policy = policy;
            Preferences = prefs;
            ShortDescription = shortDescr;
            Description = description;
            CreatorID = creatorID;
            NumOfSnippets = numOfSnippets;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected bool Init(Group objToCopy)
        {
            if (objToCopy == null)
                return false;

            //save the values of the properties with private setters:
            ID = objToCopy.ID;
            Name = objToCopy.Name;
            Policy = objToCopy.Policy;
            Preferences = objToCopy.Preferences;
            ShortDescription = objToCopy.ShortDescription;
            Description = objToCopy.Description;
            CreatorID = objToCopy.CreatorID;
            NumOfSnippets = objToCopy.NumOfSnippets;

            //if the object to copy comes from a JSON deserialization, take the arguments from the helper class:
            if (objToCopy is GroupComm)
            {
                GroupComm jsonObj = (GroupComm)objToCopy;
                ID = (objToCopy.ID > 0) ? objToCopy.ID : jsonObj.CommID;
                Preferences = (objToCopy.Preferences == null) ? objToCopy.Preferences : jsonObj.CommPreferences;
                CreatorID = (objToCopy.CreatorID > 0) ? objToCopy.CreatorID : jsonObj.CommCreatorID;
                m_membersCount = (objToCopy.MembersCount > 0) ? objToCopy.MembersCount : jsonObj.CommMembersCount;
            }

            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override bool ParseFromJson(string content)
        {
            return Init(ParseFromJson<GroupComm>(content));
        }

        public override bool Parse(XElement xml)
        {
            if (xml == null)
                return false;

            int id = 0;
            string name = string.Empty;
            GroupPolicy policy = GroupPolicy.OpenSource;
            string shortDescr = string.Empty;
            string descr = string.Empty;
            int creatorID = 0;
            int numOfSnippets = 0;

            xml.ParseNode("ID", ref id, false);
            xml.ParseNode("Name", ref name, false);
            xml.ParseNode<GroupPolicy>("Policy", ref policy, false);
            xml.ParseNode("ShortDescription", ref shortDescr, false);
            xml.ParseNode("Description", ref descr, false);
            xml.ParseNode("CreatorID", ref creatorID, false);
            xml.ParseNode("NumOfSnippets", ref numOfSnippets, false);
            xml.ParseNode("MembersCount", ref m_membersCount, false);

            GroupPreferences prefs = new GroupPreferences(xml.GetNode("pref", false));

            Init(id, name, policy, prefs, shortDescr, descr, creatorID, numOfSnippets);

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
            XDocument res = Utilities.CreateXMLDoc("Group", false);

            res.Root.Add(
                new XElement(ConfigReader.S2CNS + "ID", ID),
                new XElement(ConfigReader.S2CNS + "Name", Name.FixXML()),
                new XElement(ConfigReader.S2CNS + "Policy", Policy),
                new XElement(ConfigReader.S2CNS + "ShortDescription", ShortDescription.FixXML()),
                new XElement(ConfigReader.S2CNS + "Description", Description.FixXML()),
                new XElement(ConfigReader.S2CNS + "CreatorID", CreatorID),
                new XElement(ConfigReader.S2CNS + "NumOfSnippets", NumOfSnippets),
                new XElement(ConfigReader.S2CNS + "MembersCount", MembersCount),
                Preferences.ToXML(false)
            );

            return res.Root;
        }


        public override BaseEntity ToJSONEntity()
        {
            //set the values for properties with private setters using the helper class:
            GroupComm json = new GroupComm();
            json.CommID = ID;
            json.CommPreferences = Preferences;
            json.CommCreatorID = CreatorID;
            json.CommMembersCount = MembersCount;
            json.Init(this);

            return json;
        }

        /// <summary>
        ///     override object.ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("ID={0};Name={1};Policy={2};Preferences={3};ShortDescription={4};Description={5};CreatorID={6};NumOfSnippets={7}",
              ID, Name, Policy, Preferences, ShortDescription, Description, CreatorID, NumOfSnippets);
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
