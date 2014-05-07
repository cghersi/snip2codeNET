//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Xml.Linq;
using System.Xml.Serialization;

using Snip2Code.Utils;
using System.Collections.Generic;

namespace Snip2Code.Model.Entities
{
    public class Badge : BaseEntity
    {
        public const int SUBJECT_DIVIDER = 10000;
        public const int LEVEL_DIVIDER = 10;

        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// ID is a composite number:
        /// - the units determine the Level
        /// - the other 
        /// </summary>
        public short ID { get; set; }

        public string Name { get; set; }

        public string Descr { get; set; }

        public BadgeSubject RefersTo { get; set; }

        public int Threshold { get; set; }

        public BadgeLevel Level { get; set; }

        public string IconPath { get; set; }

        public DateTime Timestamp { get; set; }

        public string ActiveDescr { get; set; }

        [XmlIgnore]
        public BadgeType Type 
        { 
            get 
            { 
                int val = ID - (short)Level;
                if (Enum.IsDefined(typeof(BadgeType), val))
                    return (BadgeType)val;
                else
                    return BadgeType.Custom;
            } 
        }

        [XmlIgnore]
        public short TypeAsNumber { get { return (short)(ID - (short)Level); } }


        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        public Badge() : base() { }

        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        public Badge(string content, SerialFormat format) : base(content, format) { }


        /// <summary>
        ///     Creates a new badge guessing all the non-given values from the type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="descr"></param>
        /// <param name="threshold"></param>
        /// <param name="iconPath"></param>
        /// <param name="timestamp"></param>
        /// <param name="activeDescr"></param>
        /// --------------------------------------------------------------------------------------------------
        public Badge(BadgeType type, string name, string descr, int threshold, string iconPath, DateTime timestamp, string activeDescr)
        {
            Init((short)type, name, descr, GetSubject(type), threshold, GetLevel(type), iconPath, timestamp, activeDescr);
        }

        /// <summary>
        ///   This create a complete Badge
        /// </summary>
        /// <param name="id">Tag ID (from the dictionary)</param>
        /// <param name="name"></param>
        /// <param name="refersTo"></param>
        /// <param name="threshold"></param>
        /// <param name="level"></param>
        /// <param name="iconPath"></param>
        /// <param name="timestamp"></param>
        /// <param name="activeDescr"></param>
        /// --------------------------------------------------------------------------------------------------
        protected Badge(short id, string name, string descr, BadgeSubject refersTo, int threshold, BadgeLevel level,
            string iconPath, DateTime timestamp, string activeDescr)
        {
            Init(id, name, descr, refersTo, threshold, level, iconPath, timestamp, activeDescr);
        }


        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="refersTo"></param>
        /// <param name="threshold"></param>
        /// <param name="level"></param>
        /// <param name="iconPath"></param>
        /// <param name="timestamp"></param>
        protected void Init(short id, string name, string descr, BadgeSubject refersTo, int threshold, BadgeLevel level,
            string iconPath, DateTime timestamp, string activeDescr)
        {
            ID = id;
            Name = name;
            Descr = descr;
            RefersTo = refersTo;
            Threshold = threshold;
            Level = level;
            IconPath = iconPath;
            Timestamp = timestamp;
            ActiveDescr = activeDescr;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected bool Init(Badge objToCopy)
        {
            if (objToCopy == null)
                return false;

            Init(objToCopy.ID, objToCopy.Name, objToCopy.Descr, objToCopy.RefersTo, objToCopy.Threshold, objToCopy.Level, 
                    objToCopy.IconPath, objToCopy.Timestamp, objToCopy.ActiveDescr);
            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override bool ParseFromJson(string content)
        {
            return Init(ParseFromJson<Badge>(content));
        }

        public override bool Parse(XElement xml)
        {
            if (xml == null)
                return false;

            short id = 0;
            string name = string.Empty;
            string descr = string.Empty;
            BadgeSubject refersTo = BadgeSubject.User;
            int threshold = 0;
            BadgeLevel level = BadgeLevel.Junior;
            string iconPath = string.Empty;
            DateTime timestamp = DateTime.Now;
            string activedescr = string.Empty;

            xml.ParseNode("ID", ref id, false);
            xml.ParseNode("Name", ref name, false);
            xml.ParseNode("Descr", ref descr, false);
            xml.ParseNode<BadgeSubject>("RefersTo", ref refersTo, false);
            xml.ParseNode("Threshold", ref threshold, false);
            xml.ParseNode<BadgeLevel>("Level", ref level, false);
            xml.ParseNode("IconPath", ref iconPath, false);
            xml.ParseNode("Timestamp", ref timestamp, false);
            xml.ParseNode("ActiveDescr", ref activedescr, false);

            Init(id, name, descr, refersTo, threshold, level, iconPath, timestamp, activedescr);

            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Serialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public override XElement ToXML(bool internalPurpose = true)
        {
            XDocument res = Utilities.CreateXMLDoc("Badge", false);

            res.Root.Add(
                new XElement(ConfigReader.S2CNS + "ID", ID),
                new XElement(ConfigReader.S2CNS + "Name", Name.FixXML()),
                new XElement(ConfigReader.S2CNS + "Descr", Descr.FixXML()),
                new XElement(ConfigReader.S2CNS + "RefersTo", RefersTo),
                new XElement(ConfigReader.S2CNS + "Threshold", Threshold),
                new XElement(ConfigReader.S2CNS + "Level", Level),
                new XElement(ConfigReader.S2CNS + "IconPath", IconPath.FixXML()),
                new XElement(ConfigReader.S2CNS + "Timestamp", Timestamp),
                new XElement(ConfigReader.S2CNS + "ActiveDescr", ActiveDescr.FixXML())
            );

            return res.Root;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        public static BadgeLevel GetLevel(short id)
        {
            return (BadgeLevel)(id % LEVEL_DIVIDER);
        }

        public static BadgeLevel GetLevel(BadgeType type)
        {
            return (BadgeLevel)((short)type % LEVEL_DIVIDER);
        }

        public static BadgeSubject GetSubject(short id)
        {
            return (BadgeSubject)(id / SUBJECT_DIVIDER);
        }

        public static BadgeSubject GetSubject(BadgeType type)
        {
            return (BadgeSubject)((short)type / SUBJECT_DIVIDER);
        }

        public override string ToString()
        {
            return string.Format("ID={0};Name={1};Threshold={2}", ID, Name, Threshold);
        }
    }

    public enum BadgeType 
    {
        Custom = 1,

        //User Badges:
        SnippetCreation = 10010,
        PublishOnChannel = 10020,
        ChannelCreation = 10030,
        ChannelWithFollowers = 10040,
        ChannelWithSnippets = 10050,
        GroupCreation = 10060,
        PublishOnSocial = 10070,
        SnippetComment = 10080,
        SnippetRating = 10090,
        CreatorOfVisitedSnippet = 10100,
        ProfileComplete = 10110,
        ActiveUser = 10120,
        GroupWithMembers = 10130,
        CreatorOfVeryVisitedSnippet = 10140,
        CreatorOfHugeVisitedSnippet = 10150,

        //Snippet Badges:
        VisitedSnippet = 20010,
        VotedSnippet = 20020,
        TaggedSnippet = 20030,
        PropertySnippet = 20040,
        CommentedSnippet = 20050,
        PublishedSnippet = 20060
    }

    public enum BadgeSubject
    {
        User = 1,
        Snippet = 2
    }

    public enum BadgeLevel
    {
        None = 0,
        Junior = 1,
        Serious = 2,
        Senior = 3,
        Master = 4,
        Guru = 5
    }

    public class BadgeNameLevelPair
    {
        public string Name { get; set; }
        public BadgeLevel Level { get; set; }
        public BadgeNameLevelPair(string name, BadgeLevel level)
        {
            Name = name;
            Level = level;
        }

        public override bool Equals(object obj)
        {
            BadgeNameLevelPair y = obj as BadgeNameLevelPair;
            if (y == null)
                return false;

            return ToString().Equals(y.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}_{1}", Name.PrintNull(), Level);
        }
    }

    public class BadgeNameLevelPairComparer : Comparer<BadgeNameLevelPair>
    {
        public override int Compare(BadgeNameLevelPair x, BadgeNameLevelPair y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;

            string name1 = x.Name;
            if (name1 == null)
            {
                if (y.Name == null)
                    return 0;
                else
                    return -1;
            }
            
            int nameCompare = name1.ToLower().CompareTo(y.Name.ToLower());
            if (nameCompare != 0)
                return nameCompare;
            else
                return x.Level.CompareTo(y.Level);
        }
    }
}
