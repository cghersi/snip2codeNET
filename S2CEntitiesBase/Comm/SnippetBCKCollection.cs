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
    /// This is an utility class to serialize all the snippets of a user account for backup
    /// </summary>
    public class SnippetBCKCollection : BaseEntity
    {
        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<SnippetBCK> Snippets { get; set; }
        public int UserID { get; set; }
 
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Default constructor
        /// </summary>
        public SnippetBCKCollection() : base() { }

        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        public SnippetBCKCollection(string content, SerialFormat format) : base(content, format) { }

        /// <summary>
        /// This is the init/copy constructor for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        public SnippetBCKCollection(SnippetBCKCollection objToCopy)
        {
            if (objToCopy == null)
                return;

            Init(objToCopy.Snippets, objToCopy.UserID);
        }

        /// <summary>
        /// Builds a new object from the given information
        /// </summary>
        /// <param name="snippets"></param>
        /// <param name="userID"></param>
        public SnippetBCKCollection(ICollection<Snippet> snippets, int userID)
        {
            Snippets = new List<SnippetBCK>();
            if (snippets != null)
            {
                foreach (Snippet s in snippets)
                {
                    Snippets.Add(new SnippetBCK(s));
                }
            }
            UserID = userID;
        }

        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="snippets"></param>
        /// <param name="userID"></param>
        protected void Init(List<SnippetBCK> snippets, int userID)
        {
            Snippets = snippets;
            UserID = userID;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected bool Init(SnippetBCKCollection objToCopy)
        {
            if (objToCopy == null)
                return false;

            //save the values of the properties with private setters:
            Snippets = objToCopy.Snippets;
            UserID = objToCopy.UserID;

            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override bool ParseFromJson(string content)
        {
            return Init(ParseFromJson<SnippetBCKCollection>(content));
        }

        public override bool Parse(XElement xml)
        {
            if (xml == null)
                return false;

            List<SnippetBCK> snippets = new List<SnippetBCK>();
            int userID = -1;

            xml.ParseNode("UserID", ref userID, false);

            List<XElement> snips = xml.GetNodes("Snippet", false);         
            foreach (XElement snip in snips)
            {
                SnippetBCK s = new SnippetBCK();
                s.Parse(snip);
                snippets.Add(s);
            }

            Init(snippets, userID);

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
            XDocument res = Utilities.CreateXMLDoc("SnippetCollection", false);
            
            res.Root.Add(
                new XElement(ConfigReader.S2CNS + "UserID", UserID)
            );

            foreach (SnippetBCK s in Snippets)
            {
                res.Root.Add(s.ToXML(internalPurpose));
            }
            
            return res.Root;
        }

        public override BaseEntity ToJSONEntity()
        {
            return this;
        }


        public override string ToString()
        {
            return string.Format("UserID={0};SnippetsCount={1}",
                UserID, Snippets.IsNullOrEmpty() ? 0 : Snippets.Count);
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}


