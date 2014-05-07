//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

using Snip2Code.Model.Abstract;
using Snip2Code.Model.Comm;
using Snip2Code.Utils;
using System.Diagnostics;

namespace Snip2Code.Model.Entities
{
    public enum ShareOption
    {
        Private = 0,
        Protected = 10,
        Public = 100
    }


    /// <summary>
    /// This is the most important entity of the system.
    /// Regarding the visibility of a snippet:
    /// 1) if the snippet is private:
    ///     a) The snippet is visible only to the user that created it.
    ///     b) The snippet is manageable only by the user that created it.
    ///         - the OwnerGroupID is the personal group of the creator
    ///         - Visibility is set to 0 (Private)
    /// 2) if the snippet is protected:
    ///     a) The snippet is visible only to the belongers of the group specified in OwnerGroupID.
    ///     b) The snippet is manageable only by the admins of the group specified in OwnerGroupID and to the creator 
    ///             (if he is a member of the group specified in OwnerGroupID)
    ///         - the OwnerGroupID is the group that owns the snippet (at the creation, it is specified by the creator in the 
    ///                             TargetGroupID field of the snippet)
    ///         - Visibility is set to Visibility is set to 10 (Protected)
    /// 3) if the snippet is public:
    ///     a) The snippet is visible to everybody.
    ///     b) The snippet is manageable only by the admins of the group specified in OwnerGroupID and to the creator 
    ///             (if he is a member of the group specified in OwnerGroupID)
    ///         - the OwnerGroupID is the group that owns the snippet: its value depends on when the snippet has become public.
    ///             IF the snippet has been directly created as public, OwnerGroupID=personal group of the creator.
    ///             IF the snippet has been published after its creation, OwnerGroupID keeps the value it had when it was published.
    ///         - Visibility is set to 100 (Public)
    /// </summary>
    public abstract class Snippet : BaseEntity, IBadgeable
    {
        public const int MIN_NAME_LEN = 3;
        public const int MAX_NAME_LEN = 255;
        public const int MIN_DESCR_LEN = 0;
        public const int MAX_DESCR_LEN = int.MaxValue;
        public const int MIN_CODE_LEN = 3;

        #region Private Members
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        internal int m_commentsNum = -1;
        internal ICollection<SnippetComment> m_comments = null;
        internal ICollection<Snippet> m_correlatedSnippets = null;
        internal ICollection<SnippetProperty> m_properties = null;
        internal ICollection<int> m_channelIDs = null;
        internal ICollection<Tag> m_tags = null;
        internal List<Badge> m_badges = null;
        private ISnippetsRepository m_repo = null;
        private IGroupRepository m_repoGroup = null;
        private IUserRepository m_repoUser = null;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Stopwatch stopWatch = new Stopwatch();

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        [XmlIgnore]
        public long BadgeableID { get { return ID; } }
        [XmlIgnore]
        public long ID { get; protected set; }
        [XmlIgnore]
        public int OwnerGroupID { get; protected set; }
        [XmlIgnore]
        public int CreatorID { get; protected set; }

        // This is a denormalization of the model
        public string CreatorName { get; set; }
        public string OwnerGroupName { get; set; }
        public int CreatorImageNum { get; set; }
        public int CreatorPoints { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }
        [XmlIgnore]
        public DateTime Created { get; protected set; }
        [XmlIgnore]
        public DateTime Modified { get; protected set; }
        [XmlIgnore]
        public ShareOption Visibility { get; protected set; }

        //public ItemMetaInfo ItemMeta { get; set; }

        public int Rating { get; set; }
        [XmlIgnore]
        public float AvgRating { get; protected set; }
        [XmlIgnore]
        public int NumVote { get; protected set; }
        [XmlIgnore]
        public bool Selected { get; set; }
        [XmlIgnore]
        public bool IsTemporary { get; set; }

        public string LinkToSource { get; set; }

        public string ExtAuthor { get; set; }

        public decimal PricePerView { get; set; }

        /// <summary>
        /// This is the group that will become the owner of this snippet.
        /// IT should be:
        /// - the personal group of the creator, if the snippet would be private
        /// - the target group, if the snippet would be protected
        /// - 0 if the snippet would be public
        /// </summary>
        public int TargetGroupID { get; set; }

        [XmlIgnore]
        public byte Relevance { get; set; }

        [XmlIgnore]
        public int NumOfVisits { get; set; }

        [XmlIgnore]
        public long VisitsRanking { get; set; }

        [XmlIgnore]
        public int NumOfCopyActions { get; set; }

        [XmlIgnore]
        public int NumOfCopyEmbedActions { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Navigation Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        [XmlIgnore]
        public int NumComments
        {
            get
            {
                if (m_commentsNum < 0)
                {
                    if ((ID > 0) && (GetRepository != null))
                    {
                        m_commentsNum = GetRepository.GetCommentsCount(ID);
                        if (m_commentsNum >= 0)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_commentsNum = 0;
                }
                return m_commentsNum;
            }
        }

        [XmlIgnore]
        public ICollection<SnippetComment> Comments
        {
            get
            {
                if (m_comments == null)
                {
                    if ((ID > 0) && (GetRepository != null))
                    {
                        m_comments = GetRepository.GetComments(ID);
                        if (m_comments != null)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_comments = new List<SnippetComment>();
                }
                return m_comments;
            }
        }

        [XmlIgnore]
        public ICollection<Snippet> CorrelatedSnippets
        {
            get
            {
                if (m_correlatedSnippets == null)
                {
                    if ((ID > 0) && (GetRepository != null))
                    {
                        m_correlatedSnippets = GetRepository.GetMostCorrelatedSnippets(ID);
                        if (m_correlatedSnippets != null)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_correlatedSnippets = new List<Snippet>();
                }
                return m_correlatedSnippets;
            }
        }

        [XmlIgnore]
        public ICollection<SnippetProperty> Properties
        {
            get
            {
                if (m_properties == null)
                {
                    if ((ID > 0) && (GetRepository != null))
                    {
                        m_properties = GetRepository.GetProperties(ID);
                        if (m_properties != null)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_properties = new List<SnippetProperty>();
                }
                return m_properties;
            }
        }

        [XmlIgnore]
        public ICollection<Tag> Tags
        {
            get
            {
                if (m_tags == null)
                {
                    if ((ID > 0) && (GetRepository != null))
                    {
                        m_tags = GetRepository.GetTags(ID);
                        if (m_tags != null)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_tags = new List<Tag>();
                }
                return m_tags;
            }
        }

        /// <summary>
        ///  Gets or sets tags for this item as a list of strings
        /// </summary>
        [XmlIgnore]
        public List<string> TagList
        {
            get
            {
                List<string> result = new List<string>();
                if (Tags != null)
                {
                    foreach (Tag t in Tags)
                    {
                        result.Add(t.Value);
                    }
                }
                return result;
            }
            set
            {
                if (m_tags != null)
                    ClearTags();
                if (value != null)
                {
                    foreach (string tag in value)
                    {
                        AddTag(new Tag(tag));
                    }
                }
            }
        }


        /// <summary>
        ///  Gets or sets tags for this item as a single space separated string
        /// </summary>
        [XmlIgnore]
        public string StringTags
        {
            get
            {
                string result = "";
                if (Tags == null)
                    return "";
                foreach (Tag tag in Tags)
                {
                    if ((tag == null) || string.IsNullOrWhiteSpace(tag.Value))
                        continue;
                    if ((tag.Value.IndexOfAny(Utilities.s_separators) != -1) || (tag.Value.Length <= 2))    //allow tags with length<=2 if quoted
                        result += string.Format("\"{0}\" ", tag.Value);
                    else
                        result += string.Format("{0} ", tag.Value);
                }
                return result;
            }
            set
            {
                if (Tags != null)
                    ClearTags();
                SortedList<string, Tag> sortedTags = value.BuildTagList();
                foreach (Tag tag in sortedTags.Values)
                {
                    AddTag(tag);
                }
            }
        }

        ///// <summary>
        /////  Return properties for this item as a dictionary with key = property name and value = property value
        /////   WE CANNOT USE A DICTIONARY BECAUSE A SNIPPET MAY HAVE TWO PROPERTIES WITH THE SAME NAME BUT DIFFERENT VALUES:
        /////   E.G. OS=WIND & OS=LINUX
        ///// </summary>
        //[XmlIgnore]
        //public Dictionary<string, string> PropertyList
        //{
        //    get
        //    {
        //        Dictionary<string, string> result = new Dictionary<string, string>();
        //        ICollection<SnippetProperty> props = Properties;
        //        if (props != null)
        //        {
        //            foreach (SnippetProperty prop in props)
        //            {
        //                result.Add(prop.Name, prop.Value);
        //            }
        //        }
        //        return result;
        //    }
        //    set
        //    {
        //        if (m_properties != null)
        //            ClearProperties();
        //        if (value != null)
        //        {
        //            foreach (string propName in value.Keys)
        //            {
        //                AddProperty(new SnippetProperty(propName, value[propName], ID));
        //            }
        //        }
        //    }
        //}

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
        public Group OwnerGroup 
        {
            get
            {
                if ((OwnerGroupID > 0) && (GetGroupRepository != null))
                    return GetGroupRepository.GetById(OwnerGroupID);
                return null;
            }
        }

        [XmlIgnore]
        public ICollection<Group> Channels
        {
            get
            {
                ICollection<Group> retVal = new List<Group>();
                ICollection<int> channels = ChannelsIDs;
                if ((channels != null) && (GetGroupRepository != null))
                {
                    foreach (int channelID in channels)
                    {
                        Group channel = GetGroupRepository.GetById(channelID);
                        if (channel != null)
                            retVal.Add(channel);
                    }
                }
                return retVal;
            }
        }

        [XmlIgnore]
        public ICollection<string> ChannelsNames
        {
            get
            {
                ICollection<string> retVal = new List<string>();
                ICollection<int> channels = ChannelsIDs;
                if ((channels != null) && (GetGroupRepository != null))
                {
                    foreach (int channelID in channels)
                    {
                        Group channel = GetGroupRepository.GetById(channelID);
                        if (channel != null)
                            retVal.Add(channel.Name);
                    }
                }
                return retVal;
            }
        }

        [XmlIgnore]
        public ICollection<int> ChannelsIDs
        {
            get
            {
                if (m_channelIDs == null)
                {
                    if ((ID > 0) && (GetRepository != null))
                    {
                        m_channelIDs = GetRepository.GetChannels(ID);
                        if (m_channelIDs != null)
                            GetRepository.UpdateCache(this);
                    }
                    else
                        m_channelIDs = new List<int>();
                }
                return m_channelIDs;
            }
        }


        private IList<long> GetSnippetsIDsWithProperty(string propName)
        {
            IList<long> retVal = new List<long>();
            if ((Properties != null) && (GetRepository != null))
            {
                foreach (SnippetProperty prop in Properties)
                {
                    if ((prop != null) && propName.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        long snipID = 0;
                        long.TryParse(prop.Value, out snipID);
                        if (snipID > 0)
                            retVal.Add(snipID);
                    }
                }
            }
            return retVal;
        }

        private IList<Snippet> GetSnippetsWithProperty(string propName)
        {
            IList<Snippet> retVal = new List<Snippet>();
            if ((Properties != null) && (GetRepository != null))
            {
                foreach (SnippetProperty prop in Properties)
                {
                    if ((prop != null) && propName.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        long snipID = 0;
                        long.TryParse(prop.Value, out snipID);
                        Snippet snip = GetRepository.GetSnippetByID(snipID);
                        if (snip != null)
                            retVal.Add(snip);
                    }
                }
            }
            return retVal;
        }

        [XmlIgnore]
        public IList<long> LinkedSnippetsIDs
        {
            get { return GetSnippetsIDsWithProperty(DefaultProperty.S2CLink); }
        }

        [XmlIgnore]
        public IList<Snippet> LinkedSnippets
        {
            get { return GetSnippetsWithProperty(DefaultProperty.S2CLink); }
        }

        [XmlIgnore]
        public IList<long> PreviousSnippetsIDs
        {
            get { return GetSnippetsIDsWithProperty(DefaultProperty.S2CPrevSnippet); }
        }

        [XmlIgnore]
        public IList<Snippet> PreviousSnippets
        {
            get { return GetSnippetsWithProperty(DefaultProperty.S2CPrevSnippet); }
        }

        [XmlIgnore]
        public IList<long> NextSnippetsIDs
        {
            get { return GetSnippetsIDsWithProperty(DefaultProperty.S2CNextSnippet); }
        }

        [XmlIgnore]
        public IList<Snippet> NextSnippets
        {
            get { return GetSnippetsWithProperty(DefaultProperty.S2CNextSnippet); }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Default constructor
        /// </summary>
        protected Snippet() : base() 
        {
            IsTemporary = false;
            Relevance = (byte)SnippetRelevance.S2C;
            NumOfVisits = 1;
            VisitsRanking = 0;
            NumOfCopyActions = 0;
            NumOfCopyEmbedActions = 0;
        }

        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        protected Snippet(string content, SerialFormat format) : base(content, format) 
        {
            IsTemporary = false;
            Relevance = (byte)SnippetRelevance.S2C;
            NumOfVisits = 1;
            VisitsRanking = 0;
            VisitsRanking = 0;
            NumOfCopyActions = 0;
            NumOfCopyEmbedActions = 0;
        }

        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ownerID"></param>
        /// <param name="creatorID"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="code"></param>
        /// <param name="created"></param>
        /// <param name="modified"></param>
        /// <param name="visibility"></param>
        /// <param name="rating"></param>
        /// <param name="avgRating"></param>
        /// <param name="numVote"></param>
        /// <param name="selected"></param>
        /// <param name="pricePerView"></param>
        /// <param name="linkToSource"></param>
        /// <param name="extAuthor"></param>
        /// <param name="targetGroupID"></param>
        /// <param name="creatorName"></param>
        /// <param name="ownerGroupName"></param>
        /// <param name="creatorImageNum"></param>
        /// <param name="creatorPoints"></param>
        /// <param name="isTemporary"></param>
        /// <param name="relevance"></param>
        /// <param name="numOfVisits"></param>
        /// <param name="visitsRanking"></param>
        /// <param name="copyActions"></param>
        /// <param name="copyEmbedActions"></param>
        protected void Init(long id, int ownerID, int creatorID, string name, string description, string code,
            DateTime created, DateTime modified, ShareOption visibility, int rating, float avgRating,
            int numVote, bool selected, decimal pricePerView, string linkToSource, string extAuthor, int targetGroupID, string creatorName, 
            string ownerGroupName, int creatorImageNum, int creatorPoints, bool isTemporary, byte relevance, int numOfVisits,
            long visitsRanking, int copyActions, int copyEmbedActions)
        {
            ID = id;
            OwnerGroupID = ownerID;
            CreatorID = creatorID;
            Name = name;
            Description = description;
            Code = code;
            Created = created;
            Modified = modified;
            Visibility = visibility;
            //ItemMeta = itemMeta;
            Rating = rating;
            AvgRating = avgRating;
            NumVote = numVote;
            Selected = selected;
            PricePerView = pricePerView;
            LinkToSource = linkToSource;
            ExtAuthor = extAuthor;
            TargetGroupID = targetGroupID;
            CreatorName = creatorName;
            OwnerGroupName = ownerGroupName;
            CreatorImageNum = creatorImageNum;
            CreatorPoints = creatorPoints;
            IsTemporary = isTemporary;
            Relevance = relevance;
            NumOfVisits = numOfVisits;
            VisitsRanking = visitsRanking;
            NumOfCopyActions = copyActions;
            NumOfCopyEmbedActions = copyEmbedActions;
        }


        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected bool Init(Snippet objToCopy)
        {
            if (objToCopy == null)
                return false;

            //save the values of the properties with private setters:
            ID = objToCopy.ID;
            OwnerGroupID = objToCopy.OwnerGroupID;
            CreatorID = objToCopy.CreatorID;
            Name = objToCopy.Name;
            Description = objToCopy.Description;
            Code = objToCopy.Code;
            Created = objToCopy.Created;
            Modified = objToCopy.Modified;
            Visibility = objToCopy.Visibility;
            //ItemMeta = objToCopy.ItemMeta;
            Rating = objToCopy.Rating;
            AvgRating = objToCopy.AvgRating;
            NumVote = objToCopy.NumVote;
            Selected = objToCopy.Selected;
            IsTemporary = objToCopy.IsTemporary;
            PricePerView = objToCopy.PricePerView;
            LinkToSource = objToCopy.LinkToSource;
            ExtAuthor = objToCopy.ExtAuthor;
            TargetGroupID = objToCopy.TargetGroupID;
            CreatorName = objToCopy.CreatorName;
            OwnerGroupName = objToCopy.OwnerGroupName;
            CreatorImageNum = objToCopy.CreatorImageNum;
            CreatorPoints = objToCopy.CreatorPoints;
            Relevance = objToCopy.Relevance;
            NumOfVisits = objToCopy.NumOfVisits;
            VisitsRanking = objToCopy.VisitsRanking;
            NumOfCopyActions = objToCopy.NumOfCopyActions;
            NumOfCopyEmbedActions = objToCopy.NumOfCopyEmbedActions;

            //if the object to copy comes from a JSON deserialization, take the arguments from the helper class:
            if (objToCopy is SnippetComm)
            {
                SnippetComm jsonObj = (SnippetComm)objToCopy;
                ID = (objToCopy.ID > 0) ? objToCopy.ID : jsonObj.CommID;
                CreatorID = (objToCopy.CreatorID > 0) ? objToCopy.CreatorID : jsonObj.CommCreatorID;
                OwnerGroupID = (objToCopy.OwnerGroupID > 0) ? objToCopy.OwnerGroupID : jsonObj.CommOwnerGroupID;
                Created = (objToCopy.ID > 0) ? objToCopy.Created : jsonObj.CommCreated;
                Modified = (objToCopy.ID > 0) ? objToCopy.Modified : jsonObj.CommModified;
                Visibility = (objToCopy.Visibility > 0) ? objToCopy.Visibility : jsonObj.CommVisibility;
                AvgRating = (objToCopy.AvgRating > 0) ? objToCopy.AvgRating : jsonObj.CommAvgRating;
                NumVote = (objToCopy.NumVote > 0) ? objToCopy.NumVote : jsonObj.CommNumVote;
                TagList = (!objToCopy.TagList.IsNullOrEmpty()) ? objToCopy.TagList : jsonObj.CommTags;
                //PropertyList = (!objToCopy.PropertyList.IsNullOrEmpty()) ? objToCopy.PropertyList : jsonObj.CommProperties;
                if (!jsonObj.CommProperties.IsNullOrEmpty())
                {
                    ClearProperties();
                    foreach (SnippetProperty prop in jsonObj.CommProperties)
                    {
                        AddProperty(prop);
                    }
                }
            }

            //copy the navigation properties:
            if (this.m_tags.IsNullOrEmpty())
                this.m_tags = objToCopy.m_tags;
            if (this.m_properties.IsNullOrEmpty())
                this.m_properties = objToCopy.m_properties;
            if (this.m_comments.IsNullOrEmpty())
                this.m_comments = objToCopy.m_comments;
            if (this.m_correlatedSnippets.IsNullOrEmpty())
                this.m_correlatedSnippets = objToCopy.m_correlatedSnippets;
            if (this.m_badges.IsNullOrEmpty())
                this.m_badges = objToCopy.m_badges;

            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Properties Validity Checks
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Checks whether the given name is valid or not as a snippet name
        /// </summary>
        /// <param name="candidateName"></param>
        /// <returns></returns>
        public static bool SnippetNameIsValid(string candidateName)
        {
            return Utilities.NameIsValid(candidateName, MIN_NAME_LEN, true, MAX_NAME_LEN);
        }


        /// <summary>
        /// Checks whether the given text is valid or not as a snippet code
        /// </summary>
        /// <param name="candidateName"></param>
        /// <returns></returns>
        public static bool SnippetCodeIsValid(string candidateText)
        {
            return Utilities.NameIsValid(candidateText, MIN_CODE_LEN, true);
        }


        /// <summary>
        /// Checks whether the given text is valid or not as a snippet description
        /// </summary>
        /// <param name="candidateName"></param>
        /// <returns></returns>
        public static bool SnippetDescrIsValid(string candidateText)
        {
            return Utilities.NameIsValid(candidateText, MIN_DESCR_LEN, true, MAX_DESCR_LEN);
        }


        /// <summary>
        /// Checks whether the fields of the current snippet are valid or not:
        /// the check is performed on name, description and code.
        /// </summary>
        /// <returns></returns>
        public SnippetWrongField AreFieldsValid()
        {
            if (!SnippetNameIsValid(this.Name))
                return SnippetWrongField.NAME;
            if (!SnippetDescrIsValid(this.Description))
                return SnippetWrongField.DESCR;
            if (!SnippetCodeIsValid(this.Code))
                return SnippetWrongField.CODE;
            if (PricePerView < 0)
                return SnippetWrongField.PRICE_PER_VIEW;
            foreach (SnippetProperty prop in Properties)
            {
                if ((prop != null) && !prop.DataAreValid())
                    return SnippetWrongField.PROPERTIES;
            }
            return SnippetWrongField.OK;
        }


        /// <summary>
        /// Tells whether the given user is allowed to view the content of the current snippet
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsAllowedToView(User user)
        {
            if (Visibility >= ShareOption.Public)
                return true;
            if (user == null)
                return false;

            return user.BelongsToGroup(OwnerGroupID);
        }


        /// <summary>
        /// Tells whether the given user is allowed to change visibility of the current snippet
        /// </summary>
        /// <param name="user"></param>
        /// <param name="toChangeGroupOrUnshare">true if this method is used to check if the user is able to change the group or unshare the snippet</param>
        /// <returns></returns>
        public bool IsAllowedToChangeVisibility(User user, bool toChangeGroupOrUnshare = false)
        {
            if (user == null)
                return false;

            if (toChangeGroupOrUnshare)
                return (CreatorID == user.ID) && user.BelongsToGroup(OwnerGroupID); //the user should be the creator and belong to the owner group
            else
                return user.AdministerGroup(OwnerGroupID); //the user should be an admin of the group that owns the snippet
        }


        /// <summary>
        /// Tells whether the given user is allowed to modify the content of the current snippet and/or to delete it
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsAllowedToEdit(User user)
        {
            if (user == null)
                return false;

            //the user should be an admin of the group that owns the snippet:
            if (user.AdministerGroup(OwnerGroupID))
                return true;
            
            //or can be a simple user, if he is the creator of the snippet:
            return ((user.ID == CreatorID) && user.BelongsToGroup(OwnerGroupID));         
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region Utility methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Checks if this snippet has at least one property with the given name
        /// </summary>
        /// <param name="name">name of the property to check for</param>
        /// <returns></returns>
        public bool HasProperty(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            foreach (SnippetProperty prop in Properties)
            {
                if ((prop == null) || (prop.Name == null))
                    continue;
                if (prop.Name.Trim().Equals(name.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if this snippet has at least one property with the given value
        /// </summary>
        /// <param name="name">value of the property to check for</param>
        /// <returns></returns>
        public bool HasPropertyValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            foreach (SnippetProperty prop in Properties)
            {
                if ((prop == null) || (prop.Value == null))
                    continue;
                if (prop.Value.Trim().Equals(value.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if this snippet has the property with the given name and the given value
        /// </summary>
        /// <param name="name">name of the property to check for</param>
        /// <param name="value">value of the property to check for</param>
        /// <returns></returns>
        public bool HasProperty(string name, string value)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
                return false;
            foreach (SnippetProperty prop in Properties)
            {
                if ((prop == null) || (prop.Name == null) || (prop.Value == null))
                    continue;
                if (prop.Name.Trim().Equals(name.Trim(), StringComparison.InvariantCultureIgnoreCase) &&
                    prop.Value.Trim().Equals(value.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Retrieves the list of values of the properties matching the given name
        /// </summary>
        /// <param name="name">name of the properties to check for</param>
        /// <returns></returns>
        public List<string> GetPropertyValues(string name)
        {
            List<string> res = new List<string>();
            if (string.IsNullOrEmpty(name))
                return res;
            foreach (SnippetProperty prop in Properties)
            {
                if ((prop == null) || (prop.Name == null) || (prop.Value == null))
                    continue;
                if (prop.Name.Trim().Equals(name.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    res.Add(prop.Value);
            }
            return res;
        }


        /// <summary>
        /// Checks the given list of properties, searching for duplicates and returning only the properties that are
        /// not duplicated
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public List<SnippetProperty> RemoveDuplicates(ICollection<SnippetProperty> properties)
        {
            List<SnippetProperty> cleanProperties = new List<SnippetProperty>();
            if (properties == null)
                return cleanProperties;
            foreach (SnippetProperty prop in properties)
            {
                if (!HasProperty(prop.Name, prop.Value))
                    cleanProperties.Add(prop);
            }

            return cleanProperties;
        }


        /// <summary>
        /// Returns the ID of the user that last edited the snippet
        /// </summary>
        /// <returns>-1 if not found, the last editor ID otherwise</returns>
        public int GetLastEditor()
        {
            List<string> lastEditorProps = GetPropertyValues(DefaultProperty.LastEditingUser);
            int lastEditor = -1;
            if (!lastEditorProps.IsNullOrEmpty())
                int.TryParse(lastEditorProps[0], out lastEditor);
            return lastEditor;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region Repository
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected ISnippetsRepository GetRepository
        {
            get
            {
                if (m_repo == null)
                    m_repo = RetrieveRepository();
                return m_repo;
            }
        }

        /// <summary>
        /// Retrieves the repository of the snippet, used to lazy load info related to this snippet.
        /// Depending on the environment where the concrete class is instantiated, the retrieval
        /// will be performed from the DB, from Web service, etc.
        /// </summary>
        /// <returns>null if any error occurred or not found</returns>
        protected abstract ISnippetsRepository RetrieveRepository();

        protected IUserRepository GetUserRepository
        {
            get
            {
                if (m_repoUser == null)
                    m_repoUser = RetrieveUserRepository();
                return m_repoUser;
            }
        }

        /// <summary>
        /// Retrieves the repository of the users, used to lazy load info related to the creator of this snippet.
        /// Depending on the environment where the concrete class is instantiated, the retrieval
        /// will be performed from the DB, from Web service, etc.
        /// </summary>
        /// <returns>null if any error occurred or not found</returns>
        protected abstract IUserRepository RetrieveUserRepository();

        protected IGroupRepository GetGroupRepository
        {
            get
            {
                if (m_repoGroup == null)
                    m_repoGroup = RetrieveGroupRepository();
                return m_repoGroup;
            }
        }

        /// <summary>
        /// Retrieves the repository of the groups, used to lazy load info related to the owner of this snippet.
        /// Depending on the environment where the concrete class is instantiated, the retrieval
        /// will be performed from the DB, from Web service, etc.
        /// </summary>
        /// <returns>null if any error occurred or not found</returns>
        protected abstract IGroupRepository RetrieveGroupRepository();

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Public Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Adds the given content to the list of tags, if not yet present.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        /// <param name="addingTag"></param>
        public void AddTag(Tag addingTag)
        {
            m_tags = Tags;
            AddTagToCollection(addingTag);
        }

        /// <summary>
        /// This method should be used only by SnippetSP to load the tags into the object and safely store it into the cache
        /// </summary>
        /// <param name="addingTag"></param>
        protected void AddTagToCollection(Tag addingTag)
        {
            //if the tag is not yet present in the list, add it:
            if (m_tags == null)
                m_tags = new List<Tag>();
            else
            {
                foreach (Tag tag in m_tags)
                {
                    if (tag == null)
                        continue;
                    if ((addingTag == null) || tag.Value.Equals(addingTag.Value, StringComparison.InvariantCultureIgnoreCase))
                        return;
                }
            }

            m_tags.Add(addingTag);
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
        /// Clears the list of tags.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        public void ClearTags()
        {
            m_tags = new List<Tag>();
        }


        /// <summary>
        /// Clears the list of badges.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        public void ClearBadges()
        {
            m_badges = new List<Badge>();
        }


        /// <summary>
        /// Adds the given property to the list of properties, if not yet present.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        /// <param name="addingProp"></param>
        public void AddProperty(SnippetProperty addingProp)
        {
            if (addingProp == null)
                return;

            string propName = addingProp.Name;
            string propValue = addingProp.Value;

            //skip the property if not valid:
            if (!SnippetProperty.DataAreValid(propName, propValue))
                return;
            //foreach (SnippetProperty prop in Properties)
            //{
            //    if (prop == null)
            //        continue;
            //    if (prop.Name.Equals(propName, StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        prop.Value = propValue;
            //        return;
            //    }
            //}

            //if the property is not yet present in the list, add it:
            if (m_properties == null)
                m_properties = new List<SnippetProperty>();

            m_properties.Add(new SnippetProperty(propName, propValue, ID, addingProp.IsVisible));
        }


        /// <summary>
        /// Adds the given property to the list of properties, if not yet present.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        /// <param name="addingProp"></param>
        public void AddOrReplaceProperty(SnippetProperty addingProp)
        {
            if (addingProp == null)
                return;

            string propName = addingProp.Name;
            string propValue = addingProp.Value;

            //skip the property if not valid:
            if (!SnippetProperty.DataAreValid(propName, propValue))
                return;

            //replace the value of the property with the existing value, if it can be found:
            foreach (SnippetProperty prop in Properties)
            {
                if (prop == null)
                    continue;
                if (prop.Name.Equals(propName, StringComparison.InvariantCultureIgnoreCase))
                {
                    prop.Value = propValue;
                    return;
                }
            }

            //if the property is not yet present in the list, add it:
            if (m_properties == null)
                m_properties = new List<SnippetProperty>();
            m_properties.Add(new SnippetProperty(propName, propValue, ID, addingProp.IsVisible));
        }


        /// <summary>
        /// Clears the list of properties.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        public void ClearProperties()
        {
            m_properties = new List<SnippetProperty>();
        }


        /// <summary>
        /// Removes the tags with un-allowed characters
        /// </summary>
        public void RemoveDirtyTags()
        {
            SortedList<string, Tag> tagList = StringTags.BuildTagList();
            m_tags = new List<Tag>();
            if (tagList != null)
            {
                foreach (string t in tagList.Keys)
                {
                    if (!string.IsNullOrWhiteSpace(t))
                        m_tags.Add(new Tag(GetCleanWord(t)));
                }
            }
        }

        private string GetCleanWord(string wordToClean)
        {
            string clean = wordToClean;
            if (wordToClean.IndexOfAny(Utilities.s_badChars) != -1)
            {
                foreach (char badChar in Utilities.s_badChars)
                {
                    clean = clean.Replace(badChar, '_');
                }
            }
            clean = clean.Replace("\"\"", ""); //fix bug#668
            return clean;
        }


        /// <summary>
        /// Removes the properties with un-allowed characters in name or value
        /// </summary>
        public void RemoveDirtyProperties()
        {
            if (Properties != null)
            {
                List<SnippetProperty> propsToRemove = new List<SnippetProperty>();
                List<SnippetProperty> propsToAdd = new List<SnippetProperty>();
                foreach (SnippetProperty prop in Properties)
                {
                    if (prop == null)
                        continue;

                    bool toClean = ((prop.Name.IndexOfAny(Utilities.s_badChars) != -1) ||
                                    (prop.Value.IndexOfAny(Utilities.s_badChars) != -1));
                    if (string.IsNullOrWhiteSpace(prop.Name) || string.IsNullOrWhiteSpace(prop.Value) || toClean)
                        propsToRemove.Add(prop);

                    if (toClean && !string.IsNullOrWhiteSpace(prop.Name) && !string.IsNullOrWhiteSpace(prop.Value))
                    {
                        propsToAdd.Add(new SnippetProperty(GetCleanWord(prop.Name), GetCleanWord(prop.Value)));
                    }
                }

                foreach (SnippetProperty prop in propsToRemove)
                {
                    m_properties.Remove(prop);
                }

                foreach (SnippetProperty prop in propsToAdd)
                {
                    AddProperty(prop);
                }
            }
        }

        /// <summary>
        /// Adds the given comment to the list of comments, if not yet present.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        /// <param name="addingComment"></param>
        public void AddComment(SnippetComment addingComment)
        {
            if (addingComment == null)
                return;

            //if the comment is not yet present in the list, add it:
            if (m_comments == null)
                m_comments = new List<SnippetComment>();

            m_comments.Add(addingComment);
        }


        /// <summary>
        /// Clears the list of comments.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        public void ClearComments()
        {
            m_comments = new List<SnippetComment>();
        }

        /// <summary>
        /// Adds the given correlated snippet to the list of correlated snippets, if not yet present.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        /// <param name="addingCorrelatedSnippet"></param>
        public void AddComment(Snippet addingCorrelatedSnippet)
        {
            if (addingCorrelatedSnippet == null)
                return;

            //if the comment is not yet present in the list, add it:
            if (m_correlatedSnippets == null)
                m_correlatedSnippets = new List<Snippet>();

            m_correlatedSnippets.Add(addingCorrelatedSnippet);
        }


        /// <summary>
        /// Clears the list of correlated snippets.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        public void ClearCorrelatedSnippets()
        {
            m_correlatedSnippets = new List<Snippet>();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override bool ParseFromJson(string content)
        {
            return Init(ParseFromJson<SnippetComm>(content));
        }

        public override bool Parse(XElement xml)
        {
            if (xml == null)
                return false;

            long id = 0;
            int ownerGroupID = 0;
            int creatorID = 0;
            string name = string.Empty;
            string description = string.Empty;
            string code = string.Empty;
            DateTime created = DateTime.MinValue;
            DateTime modified = DateTime.MinValue;
            ShareOption visibility = ShareOption.Private;
            int rating = -1;
            float avgRating = 0.0F;
            int numVote = 0;
            decimal pricePerView = new decimal(0.0);
            string linkToSource = string.Empty;
            string extAuthor = string.Empty;
            int targetGroupID = -1;
            string creatorName = string.Empty;
            string ownerGroupName = string.Empty;
            int creatorImageNum = 0;
            int creatorPoints = 0;

            xml.ParseNode("ID", ref id, false);
            xml.ParseNode("OwnerGroupID", ref ownerGroupID, false);
            xml.ParseNode("CreatorID", ref creatorID, false);
            xml.ParseNode("Name", ref name, false);
            xml.ParseNode("Description", ref description, false);
            xml.ParseNode("Code", ref code, false);
            xml.ParseNode("Created", ref created, false);
            xml.ParseNode("Modified", ref modified, false);
            xml.ParseNode<ShareOption>("Visibility", ref visibility, false);
            xml.ParseNode("Rating", ref rating, false);
            xml.ParseNode("AvgRating", ref avgRating, false);
            xml.ParseNode("NumVote", ref numVote, false);
            xml.ParseNode("PricePerView", ref pricePerView, false);
            xml.ParseNode("LinkToSource", ref linkToSource, false);
            xml.ParseNode("ExtAuthor", ref extAuthor, false);
            xml.ParseNode("TargetGroupID", ref targetGroupID, false);
            xml.ParseNode("CreatorName", ref creatorName, false);
            xml.ParseNode("OwnerGroupName", ref ownerGroupName, false);
            xml.ParseNode("CreatorImageNum", ref creatorImageNum, false);
            xml.ParseNode("CreatorPoints", ref creatorPoints, false); 

            //ItemMetaInfo itemMeta = new ItemMetaInfo(xml.GetNode(ItemMetaInfo.XMLRootName.LocalName, false));

            Init(id, ownerGroupID, creatorID, name, description, code, created, modified, visibility, //itemMeta,
                rating, avgRating, numVote, false, pricePerView, linkToSource, extAuthor, targetGroupID, creatorName, 
                ownerGroupName, creatorImageNum, creatorPoints, false, (byte)SnippetRelevance.S2C, 1, 0, 0, 0);

            XElement props = xml.GetNode("Properties", false);
            ParseProperties(props);

            XElement tags = xml.GetNode("Tags", false);
            ParseTags(tags);

            return true;
        }


        /// <summary>
        /// Deserialize an XML fragment into a list of properties for this snippet
        /// </summary>
        /// <param name="xml"></param>
        protected void ParseProperties(XElement xml)
        {
            if (xml == null)
                return;

            List<XElement> elems = xml.GetNodes("Property", false);
            foreach (XElement el in elems)
            {
                SnippetProperty prop = new SnippetProperty(el.ToString(), SerialFormat.XML);
                prop.SnippetID = this.ID;
                AddProperty(prop);
            }
        }


        /// <summary>
        /// Deserialize an XML fragment into a list of tags for this snippet
        /// </summary>
        /// <param name="xml"></param>
        protected void ParseTags(XElement xml)
        {
            if (xml == null)
                return;

            List<XElement> elems = xml.GetNodes("Tag", false);
            foreach (XElement el in elems)
            {
                AddTag(new Tag(el.Value));
            }
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
            XDocument res = Utilities.CreateXMLDoc("Snippet", false);
            
            res.Root.Add(
                new XElement(ConfigReader.S2CNS + "ID", ID),
                new XElement(ConfigReader.S2CNS + "OwnerGroupID", OwnerGroupID),
                new XElement(ConfigReader.S2CNS + "CreatorID", CreatorID),
                new XElement(ConfigReader.S2CNS + "Name", Name.FixXML()),
                new XElement(ConfigReader.S2CNS + "Description", Description.FixXML()),
                new XElement(ConfigReader.S2CNS + "Code", Code.FixXML()),
                new XElement(ConfigReader.S2CNS + "Created", Created),
                new XElement(ConfigReader.S2CNS + "Modified", Modified),
                new XElement(ConfigReader.S2CNS + "Visibility", Visibility),
                new XElement(ConfigReader.S2CNS + "Rating", Rating),
                new XElement(ConfigReader.S2CNS + "AvgRating", AvgRating),
                new XElement(ConfigReader.S2CNS + "NumVote", NumVote),
                new XElement(ConfigReader.S2CNS + "PricePerView", PricePerView),
                new XElement(ConfigReader.S2CNS + "LinkToSource", LinkToSource.FixXML()),
                new XElement(ConfigReader.S2CNS + "ExtAuthor", ExtAuthor.FixXML()), 
                new XElement(ConfigReader.S2CNS + "TargetGroupID", TargetGroupID),
                new XElement(ConfigReader.S2CNS + "CreatorName", CreatorName.FixXML()),
                new XElement(ConfigReader.S2CNS + "OwnerGroupName", OwnerGroupName.FixXML()),
                new XElement(ConfigReader.S2CNS + "CreatorImageNum", CreatorImageNum), 
                new XElement(ConfigReader.S2CNS + "CreatorPoints", CreatorPoints)
                //ItemMeta.ToXML(false)
            );

            if (m_properties != null)
                res.Root.Add(ToPropertiesXML());

            if (m_tags != null)
                res.Root.Add(ToTagsXML());

            return res.Root;
        }


        /// <summary>
        /// Serializes the list of properties related to this object in XML fragment
        /// </summary>
        /// <returns></returns>
        public XElement ToPropertiesXML()
        {
            XElement res = new XElement(ConfigReader.S2CNS + "Properties", new XAttribute("ID", ID));
            foreach (SnippetProperty prop in Properties)
            {
                if (prop.IsVisible)
                    res.Add(prop.ToXML());
            }
            return res;
        }


        /// <summary>
        /// Serializes the list of tags related to this object in XML fragment
        /// </summary>
        /// <returns></returns>
        public XElement ToTagsXML()
        {
            XElement res = new XElement(ConfigReader.S2CNS + "Tags", new XAttribute("ID", ID));
            foreach (Tag tag in Tags)
            {
                if ((tag == null) || tag.Value.Equals(CreatorName, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                res.Add(tag.ToXML());
            }
            return res;
        }


        public override BaseEntity ToJSONEntity()
        {
            //set the values for properties with private setters using the helper class:
            SnippetComm json = new SnippetComm();
            json.CommID = ID;
            json.CommCreatorID = CreatorID;
            json.CommOwnerGroupID = OwnerGroupID;
            json.CommCreated = Created;
            json.CommModified = Modified;
            json.CommVisibility = Visibility;
            json.CommAvgRating = AvgRating;
            json.CommNumVote = NumVote;
            json.CommTags = TagList; // VisibleTagList;

            //json.CommProperties = PropertyList;
            json.CommProperties = new List<SnippetProperty>();
            foreach (SnippetProperty prop in Properties)
            {
                if (prop.IsVisible)
                    json.CommProperties.Add(prop);
            }

            json.Init(this);

            return json;
        }


        public override string ToString()
        {
            return string.Format("ID={0};OwnerGroupID={1};CreatorID={2};Name={3};Visibility={4};TargetGroupID={5}", 
                ID, OwnerGroupID, CreatorID, Name, Visibility, TargetGroupID);
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }


    public enum SnippetWrongField
    {
        OK,
        NAME,
        DESCR,
        CODE,
        PRICE_PER_VIEW,
        PROPERTIES
    }

    public enum SnippetRelevance
    {
        S2C = 10,
        Github = 5,
        StackOverflow = 1
    }
}


