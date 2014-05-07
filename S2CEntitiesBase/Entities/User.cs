//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

using Snip2Code.Model.Abstract;
using Snip2Code.Model.Comm;
using Snip2Code.Utils;

namespace Snip2Code.Model.Entities
{
    /// <summary>
    /// Possible roles of a user (maps the column "Role" in dbo.Users table)
    /// </summary>
    public enum UserRole
    {
        Normal = 0,
        Admin = 1,
        S2CAdmin = 100  //only S2C team meambers!!
    }


    public enum User3Party
    {
        Unknown = 0,
        Github = 1,
        Stackoverflow = 2,
        Google = 3
    }


    /// <summary>
    /// This class models a single Snip2Code user, which is able to collect his own snippets and to receive the
    /// snippets shared by other users.
    /// A user may become administrator of a group. In this case he can change the preferences of the group
    /// from the administration panel.
    /// </summary>
    public abstract class User : BaseEntity, IBadgeable
    {
        public const int MIN_FIRSTNAME_LEN = 2;
        public const int MIN_LASTNAME_LEN = 0;
        public const int MIN_PSW_LEN = 4;
        public const int MIN_NICKNAME_LEN = 3;
        public const string MIN_NICKNAME_LEN_STR = "2"; //just use it only for Utilities Regex!!!
        public const int MAX_FIRSTNAME_LEN = 50;
        public const int MAX_LASTNAME_LEN = 50;
        public const int MAX_PSW_LEN = 40;
        public const int MAX_NICKNAME_LEN = 50;
        public const string MAX_NICKNAME_LEN_STR = "49"; //just use it only for Utilities Regex!!!

        public const string ONEALL_FAKE_USERNAME = "$$$ONEALL$$$";

        #region Private Members
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        internal ICollection<Group> m_groups = null;
        internal ICollection<Group> m_administeredGroups = null;
        internal IList<Group> m_pendingInvitationGroups = null;
        internal IList<Group> m_channels = null;
        internal IList<int> m_channelIDs = null;
        internal IList<Group> m_administeredChannels = null;
        internal List<Badge> m_badges = null;
        internal Dictionary<short, Tuple<int, DateTime>> m_snippetBadges = null;
        internal ICollection<OneAllToken> m_oneAllTokens = null;
        private IUserRepository m_repo = null;

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Primitive Properties

        [XmlIgnore]
        public long BadgeableID { get { return ID; } }

        [XmlIgnore]
        public int ID { get; private set; }
        [XmlIgnore]
        public string EMail { get; private set; }
        //[XmlIgnore]
        //public string Password { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }
        [XmlIgnore]
        public string NickName { get; private set; }
        [XmlIgnore]
        public bool Active { get; private set; }
        [XmlIgnore]
        public DateTime AcctCreated { get; private set; }
        [XmlIgnore]
        public DateTime LoginFirst { get; private set; }
        [XmlIgnore]
        public DateTime LoginLast { get; private set; }
        [XmlIgnore]
        public UserPreferences Preferences { get; private set; }

        public long PictureID { get; set; }     //0 means no picture
        [XmlIgnore]
        public bool HasValidMail { get; private set; }

        public int Points { get; set; }
        [XmlIgnore]
        public UserRole Role { get; private set; }
        [XmlIgnore]
        public int PersonalGroupID { get; private set; }

        public int DefaultGroupID { get; set; }


        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region Properties Validity Checks
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Checks whether the given name is valid or not as a first name
        /// </summary>
        /// <param name="candidateName"></param>
        /// <returns></returns>
        public static bool FirstnameIsValid(string candidateName)
        {
            return Utilities.NameIsValid(candidateName, MIN_FIRSTNAME_LEN, true, MAX_FIRSTNAME_LEN);
        }

        /// <summary>
        /// Checks whether the given name is valid or not as a last name
        /// </summary>
        /// <param name="candidateName"></param>
        /// <returns></returns>
        public static bool LastnameIsValid(string candidateName)
        {
            return Utilities.NameIsValid(candidateName, MIN_LASTNAME_LEN, true, MAX_LASTNAME_LEN);
        }

        /// <summary>
        /// Checks whether the given name is valid or not as a nick name
        /// </summary>
        /// <param name="candidateName"></param>
        /// <returns></returns>
        public static bool NicknameIsValid(string candidateName)
        {
            return Utilities.NoSpaceNameIsValid(candidateName, MIN_NICKNAME_LEN, false, MAX_NICKNAME_LEN);
        }

        /// <summary>
        /// Cleans the given suggestion from invalid chars and returns a valid nickname
        /// </summary>
        /// <param name="suggestion"></param>
        /// <returns></returns>
        public static string CleanNickname(string suggestion)
        {
            if (string.IsNullOrEmpty(suggestion))
                return DateTime.Now.Ticks + "";

            StringBuilder sugg = new StringBuilder();
            for (int i = 0; i < suggestion.Length; i++)
            {
                if (Char.IsLetterOrDigit(suggestion[i]))
                    sugg.Append(suggestion[i]);
            }
            string res = sugg.ToString();
            if (!NicknameIsValid(res))
                res += DateTime.Now.Ticks;

            return res;
        }

        /// <summary>
        /// Checks whether the given string is valid or not as a password
        /// </summary>
        /// <param name="candidateName"></param>
        /// <returns></returns>
        public static bool PswIsValid(string candidatePsw)
        {
            return Utilities.NameIsValid(candidatePsw, MIN_PSW_LEN, true, MAX_PSW_LEN);
        }


        /// <summary>
        /// Checks whether the fields of the current user are valid or not:
        /// the check is performed on first name, last name and nick name.
        /// </summary>
        /// <returns></returns>
        public UserWrongField AreFieldsValid()
        {
            if (!FirstnameIsValid(this.Name))
                return UserWrongField.FIRSTNAME;
            if (!LastnameIsValid(this.LastName))
                return UserWrongField.LASTNAME;
            if (!NicknameIsValid(this.NickName))
                return UserWrongField.NICKNAME;
            return UserWrongField.OK;
        }

        /// <summary>
        /// Returns the name of the personal channel, created at the signup of the user 
        /// </summary>
        /// <returns></returns>
        public string GetPersChannelName()
        {
            //keep aligned with [dbo].[getPersChannelName]!!
            string res = null;
            if (NickName.Length > 37)
		        res = res.Substring(0, 36);
            else
                res = NickName;
            return "Cool stuff by " + res;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region Navigation Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        //[XmlIgnore]
        //public string PictureRelativePath { get { return System.IO.Path.Combine("Content", "Images", "Users", ID + ".png"); } }

        /// <summary>
        /// Retrieves the name of the picture for this user, related to the given size
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string PictureRelativePath(S2CImagesSize size) 
        {
            return string.Format("{0}_{1}.png", ID, size); 
        }

        [XmlIgnore]
        public ICollection<Group> Groups
        {
            get
            {
                if (m_groups.IsNullOrEmpty())
                {
                    if ((ID > 0) && (GetRepository != null))
                    {
                        m_groups = GetRepository.GetGroupsOfUser(this.ID);
                        if (m_groups != null)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_groups = new List<Group>();
                }
                return m_groups;
            }
        }


        /// <summary>
        /// Tells whether this user is part of the given group (both as a simple viewer or administrator)
        /// </summary>
        /// <param name="groupID">group to check</param>
        /// <returns></returns>
        public bool BelongsToGroup(int groupID)
        {
            if (Groups != null)
            {
                foreach (Group g in Groups)
                {
                    if ((g != null) && (g.ID == groupID))
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Tells whether this user is part of the given group (both as a simple viewer or administrator)
        /// </summary>
        /// <param name="groupName">group to check</param>
        /// <returns>-1 if the user doesn't belong to this group; the ID of the group matching the given name</returns>
        public int BelongsToGroup(string groupName)
        {
            if (Groups != null)
            {
                foreach (Group g in Groups)
                {
                    if ((g != null) && g.Name.Equals(groupName, StringComparison.InvariantCultureIgnoreCase))
                        return g.ID;
                }
            }
            return -1;
        }


        /// <summary>
        /// The groups administered by the current user, i.e. where she can edit snippets, add/remove members, etc.
        /// </summary>
        [XmlIgnore]
        public ICollection<Group> AdministeredGroups
        {
            get
            {
                if (m_administeredGroups.IsNullOrEmpty())
                {
                    if ((ID > 0) && (GetRepository != null)) 
                    {
                        m_administeredGroups = GetRepository.GetAdministeredGroupsOfUser(this.ID);
                        if (m_administeredGroups != null)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_administeredGroups = new List<Group>();
                }
                return m_administeredGroups;
            }
        }

        /// <summary>
        /// The groups of the user with Policy=Open
        /// </summary>
        [XmlIgnore]
        public ICollection<Group> OpenGroups
        {
            get
            {
                ICollection<Group> retVal = new List<Group>();
                if (Groups != null)
                {
                    foreach (Group g in Groups)
                    {
                        if ((g != null) && (g.Policy == GroupPolicy.OpenSource))
                            retVal.Add(g);
                    }
                }
                return retVal;
            }
        }

        /// <summary>
        /// The groups of the user with Policy=Protected
        /// </summary>
        [XmlIgnore]
        public ICollection<Group> ProtectedGroups
        {
            get
            {
                ICollection<Group> retVal = new List<Group>();
                if (Groups != null)
                {
                    foreach (Group g in Groups)
                    {
                        if ((g != null) && (g.Policy == GroupPolicy.Company))
                            retVal.Add(g);
                    }
                }
                return retVal;
            }
        }

        /// <summary>
        /// The channels of the user
        /// </summary>
        [XmlIgnore]
        public IList<Group> JoinedChannels
        {
            get
            {
                if (m_channels.IsNullOrEmpty())
                {
                    if ((ID > 0) && (GetRepository != null))
                    {
                        m_channels = GetRepository.GetChannelsOfUser(this.ID);
                        if (m_channels != null)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_channels = new List<Group>();
                }
                return m_channels;
            }
        }

        /// <summary>
        /// The channels of the user
        /// </summary>
        [XmlIgnore]
        public IList<int> JoinedChannelIDs
        {
            get
            {
                if (m_channelIDs.IsNullOrEmpty())
                {
                    if ((ID > 0) && (GetRepository != null))
                        m_channelIDs = GetRepository.GetChannelIDsOfUser(this.ID);
                    else
                        m_channelIDs = new List<int>();
                }
                return m_channelIDs;
            }
        }

        /// <summary>
        /// Tells whether this user is subscribed to the given channel (both as a simple viewer or administrator)
        /// </summary>
        /// <param name="channelID">channel to check</param>
        /// <returns></returns>
        public bool IsSubscribedToChannel(int channelID)
        {
            if (JoinedChannels != null)
            {
                foreach (Group channel in JoinedChannels)
                {
                    if ((channel != null) && (channel.ID == channelID))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// The channels administered by the current user, i.e. where she can add/remove members, etc.
        /// </summary>
        [XmlIgnore]
        public ICollection<Group> AdministeredChannels
        {
            get
            {
                if (m_administeredChannels.IsNullOrEmpty())
                {
                    if ((ID > 0) && (GetRepository != null))
                    {
                        m_administeredChannels = GetRepository.GetAdministeredChannelsOfUser(this.ID);
                        if (m_administeredChannels != null)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_administeredChannels = new List<Group>();
                }
                return m_administeredChannels;
            }
        }

        /// <summary>
        /// Tells whether this user is an administrator of the given group
        /// </summary>
        /// <param name="groupID">group to check</param>
        /// <returns></returns>
        public bool AdministerGroup(int groupID)
        {
            if (AdministeredGroups != null)
            {
                foreach (Group g in AdministeredGroups)
                {
                    if ((g != null) && (g.ID == groupID))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Tells whether this user is an administrator of the given group
        /// </summary>
        /// <param name="groupID">group to check</param>
        /// <returns></returns>
        public bool AdministerChannel(int channelID)
        {
            if (AdministeredChannels != null)
            {
                foreach (Group channel in AdministeredChannels)
                {
                    if ((channel != null) && (channel.ID == channelID))
                        return true;
                }
            }
            return false;
        }

        [XmlIgnore]
        public IList<Group> PendingInvitationGroups
        {
            get
            {
                if (m_pendingInvitationGroups.IsNullOrEmpty())
                {
                    if ((ID > 0) && (GetRepository != null))
                    {
                        m_pendingInvitationGroups = GetRepository.GetPendingInvitationGroupsOfUser(this.ID);
                        if (m_pendingInvitationGroups != null)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_pendingInvitationGroups = new List<Group>();
                }
                return m_pendingInvitationGroups;
            }
        }

        [XmlIgnore]
        public ICollection<OneAllToken> OneAllTokens
        {
            get
            {
                if (m_oneAllTokens.IsNullOrEmpty())
                {
                    if ((ID > 0) && (GetRepository != null))
                    {
                        m_oneAllTokens = GetRepository.GetTokensOfUser(this.ID);
                        if (m_oneAllTokens != null)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_oneAllTokens = new List<OneAllToken>();
                }
                return m_oneAllTokens;
            }
        }

        /// <summary>
        /// Retrieves the token linked to the given provider for this user
        /// </summary>
        /// <param name="provider">provider to check</param>
        /// <returns>Guid.Empty if not found, the actual token if present</returns>
        public Guid GetToken(OneAllProvider provider)
        {
            if (OneAllTokens != null)
            {
                foreach (OneAllToken tok in OneAllTokens)
                {
                    if ((tok != null) && (tok.Provider == provider))
                        return tok.Token;
                }
            }
            return Guid.Empty;
        }

        [XmlIgnore]
        public string CompleteName
        {
            get
            {
                string res = Name;
                if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(LastName))
                    res += " ";
                res += LastName;
                return res;
            }
        }


        [XmlIgnore]
        public List<Badge> Badges
        {
            get
            {
                if (m_badges == null)
                {
                    if ((ID > 0) && (GetRepository != null))
                    {
                        m_badges = GetRepository.GetBadges(ID);
                        if (m_badges != null)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_badges = new List<Badge>();
                }
                return m_badges;
            }
        }
      
        [XmlIgnore]
        public Dictionary<short, DateTime> BadgeIDs
        {
            get 
            {
                Dictionary<short, DateTime> res = new Dictionary<short, DateTime>();
                List<Badge> badges = Badges;
                if (badges != null)
                    badges.ForEach((b) => res.Add(b.ID, b.Timestamp));
                return res;
            }
        }

        [XmlIgnore]
        public Dictionary<short, Tuple<int, DateTime>> SnippetBadges
        {
            get
            {
                if (m_snippetBadges == null)
                {
                    if ((ID > 0) && (GetRepository != null))
                    {
                        m_snippetBadges = GetRepository.GetSnippetBadges(ID);
                        if (m_snippetBadges != null)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_snippetBadges = new Dictionary<short, Tuple<int, DateTime>>();
                }
                return m_snippetBadges;
            }
        }


        /// <summary>
        /// Adds the given content to the list of badges.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        /// <param name="addingBadge"></param>
        public void AddBadge(Badge addingBadge)
        {
            if (addingBadge == null)
                return;

            //if the badge is not yet present in the list, add it:
            if (m_badges == null)
                m_badges = new List<Badge>();
            else
            {
                if (m_badges.Contains(addingBadge))
                    return;
            }

            m_badges.Add(addingBadge);
        }

        /// <summary>
        /// Replaces the given content in the list of badges, if not yet present.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        /// <param name="addingBadge"></param>
        /// <param name="existingBadge"></param>
        public void ReplaceBadge(Badge existingBadge, Badge newBadge)
        {
            if (m_badges == null)
                m_badges = new List<Badge>();
            if (existingBadge != null)
                m_badges.Remove(existingBadge);
            if ((newBadge != null) && !m_badges.Contains(newBadge))
                m_badges.Add(newBadge);
        }

        /// <summary>
        /// Clears the list of badges.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        public void ClearBadges()
        {
            m_badges = new List<Badge>();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        protected User() : base() { }

        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        protected User(string content, SerialFormat format) : base(content, format) { }


        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <param name="lastname"></param>
        /// <param name="nickname"></param>
        /// <param name="active"></param>
        /// <param name="created"></param>
        /// <param name="loginFirst"></param>
        /// <param name="loginLast"></param>
        /// <param name="preferences"></param>
        /// <param name="pictureID"></param>
        /// <param name="hasValidMail"></param>
        /// <param name="points"></param>
        /// <param name="role"></param>
        /// <param name="personalGroupID"></param>
        /// <param name="defaultGroupID"></param>
        protected void Init(int id, string email, string name, string lastname, string nickname, bool active, DateTime created, 
            DateTime loginFirst, DateTime loginLast, UserPreferences preferences, long pictureID, bool hasValidMail, int points,
            UserRole role, int personalGroupID, int defaultGroupID)
        {
            ID = id;
            EMail = email;
            Name = name;
            LastName = lastname;
            NickName = nickname;
            Active = active;
            AcctCreated = created;
            LoginFirst = loginFirst;
            LoginLast = loginLast;
            Preferences = preferences;
            PictureID = pictureID;
            HasValidMail = hasValidMail;
            Points = points;
            Role = role;
            PersonalGroupID = personalGroupID;
            DefaultGroupID = defaultGroupID;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected bool Init(User objToCopy)
        {
            if (objToCopy == null)
                return false;

            //save the values of the properties with private setters:
            ID = objToCopy.ID;
            EMail = objToCopy.EMail;
            Name = objToCopy.Name;
            LastName = objToCopy.LastName;
            NickName = objToCopy.NickName;
            Active = objToCopy.Active;
            AcctCreated = objToCopy.AcctCreated;
            LoginFirst = objToCopy.LoginFirst;
            LoginLast = objToCopy.LoginLast;
            Preferences = objToCopy.Preferences;
            PictureID = objToCopy.PictureID;
            HasValidMail = objToCopy.HasValidMail;
            Points = objToCopy.Points;
            Role = objToCopy.Role;
            PersonalGroupID = objToCopy.PersonalGroupID;
            DefaultGroupID = objToCopy.DefaultGroupID;

            //if the object to copy comes from a JSON deserialization, tke the arguments from the helper class:
            if (objToCopy is UserComm)
            {
                UserComm jsonObj = (UserComm)objToCopy;
                ID = (objToCopy.ID > 0) ? objToCopy.ID : jsonObj.CommID;
                EMail = (!string.IsNullOrEmpty(objToCopy.EMail)) ? objToCopy.EMail : jsonObj.CommEMail;
                NickName = (!string.IsNullOrEmpty(objToCopy.NickName)) ? objToCopy.NickName : jsonObj.CommNickName;
                Preferences = (objToCopy.Preferences == null) ? objToCopy.Preferences : jsonObj.CommPreferences;
                //PictureID = (objToCopy.PictureID > 0) ? objToCopy.PictureID : jsonObj.CommPictureID;
                Role = (objToCopy.Role > 0) ? objToCopy.Role : jsonObj.CommRole;
                PersonalGroupID = (objToCopy.PersonalGroupID > 0) ? objToCopy.PersonalGroupID : jsonObj.CommPersonalGroupID;
            }

            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Repository
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected IUserRepository GetRepository
        {
            get
            {
                if (m_repo == null)
                    m_repo = RetrieveRepository();
                return m_repo;
            }
        }

        /// <summary>
        /// Retrieves the repository of the user, used to lazy load info related to this user.
        /// Depending on the environment where the concrete class is instantiated, the retrieval
        /// will be performed from the DB, from Web service, etc.
        /// </summary>
        /// <returns>null if any error occurred; if no comments are found, returns an empty set (not null)</returns>
        protected abstract IUserRepository RetrieveRepository();

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override bool ParseFromJson(string content)
        {
            return Init(ParseFromJson<UserComm>(content));
        }

        public override bool Parse(XElement xml)
        {
            if (xml == null)
                return false;

            int id = 0;
            string email = string.Empty;
            string name = string.Empty;
            string lastname = string.Empty;
            string nickname = string.Empty;
            bool active = false;
            DateTime created = DateTime.MinValue;
            DateTime loginFirst = DateTime.MinValue;
            DateTime loginLast = DateTime.MinValue;
            long pictureID = 0;
            bool hasValidMail = false;
            int points = 0;
            UserRole role = UserRole.Normal;
            int personalGroupID = 0;
            int defaultGroupID = 0;

            xml.ParseNode("ID", ref id, false);
            xml.ParseNode("EMail", ref email, false);
            xml.ParseNode("Name", ref name, false);
            xml.ParseNode("LastName", ref lastname, false);
            xml.ParseNode("NickName", ref nickname, false);
            //xml.ParseNode("Active", ref active, false);
            //xml.ParseNode("AcctCreated", ref created, false);
            //xml.ParseNode("LoginFirst", ref loginFirst, false);
            //xml.ParseNode("LoginLast", ref loginLast, false);
            xml.ParseNode("PictureID", ref pictureID, false);
            //xml.ParseNode("HasValidMail", ref hasValidMail, false);
            xml.ParseNode("Points", ref points, false);
            xml.ParseNode<UserRole>("Role", ref role, false);
            xml.ParseNode("PersonalGroupID", ref personalGroupID, false);
            xml.ParseNode("DefaultGroupID", ref defaultGroupID, false);

            UserPreferences prefs = new UserPreferences(xml.GetNode("pref", false));

            Init(id, email, name, lastname, nickname, active, created, loginFirst, loginLast, prefs,
                pictureID, hasValidMail, points, role, personalGroupID, defaultGroupID);

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
            XDocument res = Utilities.CreateXMLDoc("User", false);

            res.Root.Add(
                new XElement(ConfigReader.S2CNS + "ID", ID),
                new XElement(ConfigReader.S2CNS + "EMail", EMail),
                new XElement(ConfigReader.S2CNS + "Name", Name.FixXML()),
                new XElement(ConfigReader.S2CNS + "LastName", LastName.FixXML()),
                new XElement(ConfigReader.S2CNS + "NickName", NickName.FixXML()),
                //new XElement(ConfigReader.S2CNS + "Active", Active),
                //new XElement(ConfigReader.S2CNS + "AcctCreated", AcctCreated),
                //new XElement(ConfigReader.S2CNS + "LoginFirst", LoginFirst),
                //new XElement(ConfigReader.S2CNS + "LoginLast", LoginLast),
                new XElement(ConfigReader.S2CNS + "PictureID", PictureID),
                //new XElement(ConfigReader.S2CNS + "HasValidMail", HasValidMail),
                new XElement(ConfigReader.S2CNS + "Points", Points),
                new XElement(ConfigReader.S2CNS + "Role", Role),
                new XElement(ConfigReader.S2CNS + "PersonalGroupID", PersonalGroupID),
                new XElement(ConfigReader.S2CNS + "DefaultGroupID", DefaultGroupID),
                Preferences.ToXML(false)
            );

            return res.Root;
        }

        public override BaseEntity ToJSONEntity()
        {
            //set the values for properties with private setters using the helper class:
            UserComm json = new UserComm();
            json.CommID = ID;
            json.CommEMail = EMail;
            json.CommNickName = NickName;
            json.CommPreferences = Preferences;
            //json.CommPictureID = PictureID;
            json.CommRole = Role;
            json.CommPersonalGroupID = PersonalGroupID;

            json.Init(this);

            return json;
        }


        public override string ToString()
        {
            return string.Format("ID={0};EMail={1};Name={2};NickName={3};PictureID={4};PersonalGroupID={5}",
                ID, EMail, Name + ' ' + LastName, NickName, PictureID, PersonalGroupID);
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }


    public enum UserWrongField
    {
        OK,
        FIRSTNAME,
        LASTNAME,
        NICKNAME
    }


    public enum S2CImagesSize
    {
        tiny = 16,
        small = 50,
        med = 80,
        large = 300
    }
}
