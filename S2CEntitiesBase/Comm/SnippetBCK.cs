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

namespace Snip2Code.Model.Entities
{
    /// <summary>
    /// This is an utility class to serialize a snippet for backup
    /// </summary>
    public class SnippetBCK : BaseEntity
    {
        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public long ID { get; set; }
        public string CreatorName { get; set; }
        public string OwnerGroupName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public ShareOption Visibility { get; set; }
 
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Navigation Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<SnippetProperty> Properties { get; set; }
        public List<Tag> Tags { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Default constructor
        /// </summary>
        public SnippetBCK() : base() { }

        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        protected SnippetBCK(string content, SerialFormat format) : base(content, format) { }

        /// <summary>
        /// This is the init/copy constructor for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        public SnippetBCK(Snippet objToCopy)
        {
            if (objToCopy == null)
                return;

            string ownerGroupName = objToCopy.OwnerGroupName;
            if ((objToCopy.OwnerGroup != null) && (objToCopy.OwnerGroup.Policy == GroupPolicy.Personal))
                ownerGroupName = string.Format("{0}'s Personal", objToCopy.CreatorName);
            Init(objToCopy.ID, objToCopy.CreatorName, ownerGroupName, objToCopy.Name, objToCopy.Description, objToCopy.Code, 
                    objToCopy.Created, objToCopy.Modified, objToCopy.Visibility, new List<SnippetProperty>(objToCopy.Properties), 
                    new List<Tag>(objToCopy.Tags));
        }

        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="creatorName"></param>
        /// <param name="ownerGroupName"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="code"></param>
        /// <param name="created"></param>
        /// <param name="modified"></param>
        /// <param name="visibility"></param>
        /// <param name="properties"></param>
        /// <param name="tags"></param>
        protected void Init(long id, string creatorName, string ownerGroupName, string title, string description, string code,
            DateTime created, DateTime modified, ShareOption visibility, List<SnippetProperty> properties, List<Tag> tags)
        {
            ID = id;
            CreatorName = creatorName;
            OwnerGroupName = ownerGroupName;
            Title = title;
            Description = description;
            Code = code;
            Created = created;
            Modified = modified;
            Visibility = visibility;
            Properties = properties;
            Tags = tags;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected bool Init(SnippetBCK objToCopy)
        {
            if (objToCopy == null)
                return false;

            //save the values of the properties with private setters:
            ID = objToCopy.ID;
            CreatorName = objToCopy.CreatorName;
            OwnerGroupName = objToCopy.OwnerGroupName;
            Title = objToCopy.OwnerGroupName;
            Description = objToCopy.Description;
            Code = objToCopy.Code;
            Created = objToCopy.Created;
            Modified = objToCopy.Modified;
            Visibility = objToCopy.Visibility;
            Properties = objToCopy.Properties;
            Tags = objToCopy.Tags;

            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override bool ParseFromJson(string content)
        {
            return Init(ParseFromJson<SnippetBCK>(content));
        }

        public override bool Parse(XElement xml)
        {
            if (xml == null)
                return false;

            long id = 0;
            string creatorName = string.Empty;
            string ownerGroupName = string.Empty;
            string title = string.Empty;
            string description = string.Empty;
            string code = string.Empty;
            DateTime created = DateTime.MinValue;
            DateTime modified = DateTime.MinValue;
            ShareOption visib = ShareOption.Private;

            xml.ParseNode("ID", ref id, false);
            xml.ParseNode("CreatorName", ref creatorName, false);
            xml.ParseNode("OwnerGroupName", ref ownerGroupName, false);
            xml.ParseNode("Title", ref title, false);
            xml.ParseNode("Description", ref description, false);
            xml.ParseNode("Code", ref code, false);
            xml.ParseNode("Created", ref created, false);
            xml.ParseNode("Modified", ref modified, false);
            xml.ParseNode("Visibility", ref visib, false);

            Init(id, creatorName, ownerGroupName, title, description, code, created, modified, visib, null, null);

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
            Properties = new List<SnippetProperty>();
            foreach (XElement el in elems)
            {
                SnippetProperty prop = new SnippetProperty(el.ToString(), SerialFormat.XML);
                prop.SnippetID = this.ID;
                Properties.Add(prop);
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
            Tags = new List<Tag>();
            foreach (XElement el in elems)
            {
                Tags.Add(new Tag(el.Value));
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
                new XElement(ConfigReader.S2CNS + "CreatorName", CreatorName.FixXML()),
                new XElement(ConfigReader.S2CNS + "OwnerGroupName", OwnerGroupName.FixXML()),
                new XElement(ConfigReader.S2CNS + "Title", Title.FixXML()),
                new XElement(ConfigReader.S2CNS + "Description", Description.FixXML()),
                new XElement(ConfigReader.S2CNS + "Code", Code.FixXML()),
                new XElement(ConfigReader.S2CNS + "Created", Created),
                new XElement(ConfigReader.S2CNS + "Modified", Modified),
                new XElement(ConfigReader.S2CNS + "Visibility", Visibility)
            );

            res.Root.Add(ToPropertiesXML());
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
            return this;
        }


        public override string ToString()
        {
            return string.Format("ID={0};OwnerGroupName={1};CreatorName={2};Title={3}",
                ID, OwnerGroupName, CreatorName, Title);
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}


