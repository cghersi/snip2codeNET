//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Serialization;

using Snip2Code.Utils;

namespace Snip2Code.Model.Entities
{
    /// ===============================================================================
    /// <summary>
    /// Stores the information regarding the meta information of the snippet, saved in the 
    /// "ItemMeta" field in the "Snippets" table of the DB.
    /// </summary>
    /// ===============================================================================
    public class ItemMetaInfo : BaseEntity
    {
        #region STATIC VARIABLES
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private const string s_rootName = "meta";

        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        #region PROPERTIES
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        //public string Description { get; set; } //TODO this is just a trial, place here the real content!!

        [XmlIgnore]
        static public XName XMLRootName { get { return s_rootName; } }

        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////


        #region CONSTRUCTORS
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public ItemMetaInfo()
        {
            setDefaults();
        }


        /// <summary>
        ///     Creates an instance reading the xml and applying the defaults where 
        ///     there are no information available
        /// </summary>
        /// <param name="xml">xml fragment to parse</param>
        /// --------------------------------------------------------------------------------------------------
        public ItemMetaInfo(XElement xml)
            : this()
        {
            if (xml == null)
                return;

            bool internalPurpose = !xml.Name.Namespace.Equals(ConfigReader.S2CNS);

            //string desc = string.Empty;
            //xml.ParseNode("description", ref desc, internalPurpose);
            //Description = desc;
        }

        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////


        #region UTILITIES
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///     Set the defaults in case of errors.
        /// </summary>
        private void setDefaults()
        {
            //Description = string.Empty;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Serialization
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override XElement ToXML(bool internalPurpose = true)
        {
            XElement xml = new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + s_rootName);
            //xml.Add(
            //    new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "Description", Description)
            //);

            return xml;
        }
        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        ///     override object.ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToXML().ToString();
        }
    }
}
