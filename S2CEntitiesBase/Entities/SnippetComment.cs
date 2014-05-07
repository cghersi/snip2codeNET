//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Xml.Linq;
using System.Xml.Serialization;

using Snip2Code.Model.Comm;
using Snip2Code.Utils;

namespace Snip2Code.Model.Entities
{
    /// <summary>
    /// Summary description for SnippetComment
    /// </summary>
    public abstract class SnippetComment : BaseEntity
    {
        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        [XmlIgnore]
        public long CommentID { get; private set; }

        public long SnippetID { get; set; }

        public int CommenterID { get; set; }

        public string CommenterFullName { get; set; }

        public string Content { get; set; }
        [XmlIgnore]
        public DateTime SnippetCommentDate { get; private set; }

        public long DisqusPostId { get; set; }

        public string DisqusAuthorUID { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        protected SnippetComment() : base() { }

        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        protected SnippetComment(string content, SerialFormat format) : base(content, format) { }

        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="commentID"></param>
        /// <param name="snippetID"></param>
        /// <param name="commenterID"></param>
        /// <param name="commenterFullName"></param>
        /// <param name="text"></param>
        /// <param name="date"></param>
        /// <param name="disqusPostId"></param>
        /// <param name="disqusAuthorUID"></param>
        protected void Init(long commentID, long snippetID, int commenterID, string commenterFullName, string text, DateTime date,
            long disqusPostId, string disqusAuthorUID)
        {
            CommentID = commentID;
            SnippetID = snippetID;
            CommenterID = commenterID;
            CommenterFullName = commenterFullName;
            Content = text;
            SnippetCommentDate = date;
            DisqusPostId = disqusPostId;
            DisqusAuthorUID = disqusAuthorUID;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected bool Init(SnippetComment objToCopy)
        {
            if (objToCopy == null)
                return false;

            //save the values of the properties with private setters:
            CommentID = objToCopy.CommentID;
            SnippetID = objToCopy.SnippetID;
            CommenterID = objToCopy.CommenterID;
            CommenterFullName = objToCopy.CommenterFullName;
            Content = objToCopy.Content;
            SnippetCommentDate = objToCopy.SnippetCommentDate;
            DisqusPostId = objToCopy.DisqusPostId;
            DisqusAuthorUID = objToCopy.DisqusAuthorUID;

            //if the object to copy comes from a JSON deserialization, tke the arguments from the helper class:
            if (objToCopy is SnippetCommentComm)
            {
                SnippetCommentComm jsonObj = (SnippetCommentComm)objToCopy;
                CommentID = (objToCopy.CommentID > 0) ? objToCopy.CommentID : jsonObj.CommID;
                SnippetCommentDate = (objToCopy.CommentID > 0) ? objToCopy.SnippetCommentDate : jsonObj.CommSnippetCommentDate;
            }

            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override bool ParseFromJson(string content)
        {
            return Init(ParseFromJson<SnippetCommentComm>(content));
        }

        public override bool Parse(XElement xml)
        {
            if (xml == null)
                return false;

            long commentID = 0;
            long snippetID = 0;
            int userID = 0;
            string userFullName = string.Empty;
            string text = string.Empty;
            DateTime date = DateTime.MinValue;
            long disqusPostId = -1;
            string disqusAuthorUID = string.Empty;

            xml.ParseNode("CommentID", ref commentID, false);
            xml.ParseNode("SnippetID", ref snippetID, false);
            xml.ParseNode("CommenterID", ref userID, false);
            xml.ParseNode("CommenterFullName", ref userFullName, false);
            xml.ParseNode("Content", ref text, false);
            xml.ParseNode("SnippetCommentDate", ref date, false);
            xml.ParseNode("DisqusPostId", ref disqusPostId, false);
            xml.ParseNode("DisqusAuthorUID", ref disqusAuthorUID, false);

            Init(commentID, snippetID, userID, userFullName, text, date, disqusPostId, disqusAuthorUID);

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
            XDocument res = Utilities.CreateXMLDoc("SnippetComment", false);
            res.Root.Add(
                new XElement(ConfigReader.S2CNS + "CommentID", CommentID),
                new XElement(ConfigReader.S2CNS + "SnippetID", SnippetID),
                new XElement(ConfigReader.S2CNS + "CommenterID", CommenterID),
                new XElement(ConfigReader.S2CNS + "CommenterFullName", CommenterFullName.FixXML()),
                new XElement(ConfigReader.S2CNS + "Content", Content.FixXML()),
                new XElement(ConfigReader.S2CNS + "SnippetCommentDate", SnippetCommentDate),
                new XElement(ConfigReader.S2CNS + "DisqusPostId", DisqusPostId),
                new XElement(ConfigReader.S2CNS + "DisqusAuthorUID", DisqusAuthorUID)
            );

            return res.Root;
        }


        public override BaseEntity ToJSONEntity()
        {
            //set the values for properties with private setters using the helper class:
            SnippetCommentComm json = new SnippetCommentComm();
            json.CommID = CommentID;
            json.CommSnippetCommentDate = SnippetCommentDate;
            json.Init(this);

            return json;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        ///     override object.ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("ID={0};SnippetID={1};CommenterID={2};CommenterFullName={3};Content={4};Date={5};DisqusPostId={6};DisqusAuthorUID={7}",
                CommentID, SnippetID, CommenterID, CommenterFullName, Content, SnippetCommentDate, DisqusPostId, DisqusAuthorUID);
        }
    }
}
