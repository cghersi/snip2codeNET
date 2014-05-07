//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Xml.Linq;
using System.Xml.Serialization;

using Snip2Code.Utils;

namespace Snip2Code.Model.Entities
{
    public class RankingAction : BaseEntity
    {
        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////

        public short ID { get; set; }

        public string Name { get; set; }

        public RankingActionCategory Category { get; set; }

        public int Threshold { get; set; }

        public short Points { get; set; }


        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        public RankingAction() : base() { }

        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        public RankingAction(string content, SerialFormat format) : base(content, format) { }


        /// <summary>
        ///     Creates a new action guessing all the non-given values from the type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="threshold"></param>
        /// <param name="points"></param>
        /// --------------------------------------------------------------------------------------------------
        public RankingAction(RankingActionType type, string name, int threshold, short points)
        {
            int category = ((short)type) / 1000;
            Init((short)type, name, (RankingActionCategory)category, threshold, points);
        }

        /// <summary>
        ///   This create a complete RankingAction
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value">Value of the TAG</param>
        /// --------------------------------------------------------------------------------------------------
        protected RankingAction(short id, string name, RankingActionCategory category, int threshold, short points)
        {
            Init(id, name, category, threshold, points);
        }


        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <param name="threshold"></param>
        /// <param name="points"></param>
        protected void Init(short id, string name, RankingActionCategory category, int threshold, short points)
        {
            ID = id;
            Name = name;
            Category = category;
            Threshold = threshold;
            Points = points;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected bool Init(RankingAction objToCopy)
        {
            if (objToCopy == null)
                return false;

            Init(objToCopy.ID, objToCopy.Name, objToCopy.Category, objToCopy.Threshold, objToCopy.Points);
            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override bool ParseFromJson(string content)
        {
            return Init(ParseFromJson<RankingAction>(content));
        }

        public override bool Parse(XElement xml)
        {
            if (xml == null)
                return false;

            short id = 0;
            string name = string.Empty;
            RankingActionCategory cat = RankingActionCategory.DirectAction;
            int threshold = 0;
            short points = 0;

            xml.ParseNode("ID", ref id, false);
            xml.ParseNode("Name", ref name, false);
            xml.ParseNode<RankingActionCategory>("Category", ref cat, false);
            xml.ParseNode("Threshold", ref threshold, false);
            xml.ParseNode("Points", ref points, false);

            Init(id, name, cat, threshold, points);

            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Serialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public override XElement ToXML(bool internalPurpose = true)
        {
            XDocument res = Utilities.CreateXMLDoc("RankingAction", false);

            res.Root.Add(
                new XElement(ConfigReader.S2CNS + "ID", ID),
                new XElement(ConfigReader.S2CNS + "Name", Name.FixXML()),
                new XElement(ConfigReader.S2CNS + "Category", Category),
                new XElement(ConfigReader.S2CNS + "Threshold", Threshold),
                new XElement(ConfigReader.S2CNS + "Points", Points)
            );

            return res.Root;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        public override string ToString()
        {
            return string.Format("ID={0};Name={1};Points={2};Threshold={3}", ID, Name, Points, Threshold);
        }
    }

    public enum RankingActionType
    {
        //Direct Actions:
        PrivateSnippetCreation = 1001,
        ProtectedSnippetCreation = 1002,
        PublicSnippetCreation = 1003,
        CommentSnippet = 1004,
        RateSnippet = 1005,
        ChannelCreation = 1006,
        GroupCreation = 1007,

        //Events:
        VotedSnippet = 2001,
        CommentedSnippet = 2002,
        VisitedSnippet = 2003,
        SharedSocialSnippet = 2004,
        MemberJoiningGroup = 2005,
        FollowerOfChannel = 2006,
        SnippetPubInChannel = 2007
    }

    public enum RankingActionCategory
    {
        DirectAction = 1,
        Event = 2
    }
}
