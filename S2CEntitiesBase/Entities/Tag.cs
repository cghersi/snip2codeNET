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
    public class Tag : BaseEntity
    {
        protected const string BLACK_LISTED_TAGS = "&'<>=!\"/()[]#.:";

        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////

        [XmlIgnore]
        public long ID { get; private set; }

        public string Value { get; set; }

        [XmlIgnore]
        public static string BlackListedTags { get { return BLACK_LISTED_TAGS; } }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////

        #region EQUALITY_COMPARER
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Equals(object obj)
        {
            //if obj is not a Tag, it is different from the current object:
            Tag tagObj = (Tag)obj;
            if (tagObj == null)
                return false;

            //if both have negative ID, check the values:
            if ((this.ID <= 0) && (tagObj.ID <= 0))
                return Value.Equals(tagObj.Value, StringComparison.InvariantCultureIgnoreCase);

            //if obj has the same ID, they are equal!!
            if (this.ID == tagObj.ID)
                return true;

            return false;
        }


        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        public Tag() : base() { }

        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        public Tag(string content, SerialFormat format) : base(content, format) { }


        /// <summary>
        ///     Creates a new tag with value given by the parameter content.
        ///     Note: This is a in memory only tag. Use updateDictionary() to assign this tag with
        ///     an unique ID from the database
        /// </summary>
        /// <param name="content"></param>
        /// --------------------------------------------------------------------------------------------------
        public Tag(string content)
        {
            Init(-1, content);
        }

        /// <summary>
        ///   This create a TAG that exists in the dictionary. Needs a valid ID
        /// </summary>
        /// <param name="id">Tag ID (from the dictionary)</param>
        /// <param name="value">Value of the TAG</param>
        /// --------------------------------------------------------------------------------------------------
        protected Tag(long id, string content)
        {
            Init(id, content);
        }


        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="snippetID"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void Init(long id, string value)
        {
            ID = id;
            Value = value;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected bool Init(Tag objToCopy)
        {
            if (objToCopy == null)
                return false;

            Init(objToCopy.ID, objToCopy.Value);
            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override bool ParseFromJson(string content)
        {
            return Init(ParseFromJson<Tag>(content));
        }

        public override bool Parse(XElement xml)
        {
            if (xml == null)
                return false;

            string value = xml.Value;
            Init(-1, value);

            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Serialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public override XElement ToXML(bool internalPurpose = true)
        {
            return new XElement(ConfigReader.S2CNS + "Tag", Value.FixXML());
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        public override string ToString()
        {
            return string.Format("ID={0};Value={1}", ID, Value);
        }
    }
}
