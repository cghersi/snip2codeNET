//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

using Snip2Code.Utils;

namespace Snip2Code.Model.Entities
{
    /// <summary>
    /// This is a property associated with a particular snippet. It contains the actual value 
    /// of this property which the snippet is actually decorated with.
    /// </summary>
    public class SnippetProperty : BaseEntity
    {
        public const int MAX_NAME_LEN = 50;
        public const int MAX_VALUE_LEN = 50;
        //public const string SPACE_PLACEHOLDER = "++++++";
        public const string NAME_VALUE_SEPARATOR = "---";
        public const string STRANGE_CHAR_REPLACER = "_";

        private static readonly HashSet<string> NUMERIC_PROPS = new HashSet<string> { 
            DefaultProperty.Channel.ToLower(), DefaultProperty.S2CLink.ToLower(), DefaultProperty.S2CNextSnippet.ToLower(), 
            DefaultProperty.S2CPrevSnippet.ToLower() };

        public static readonly HashSet<string> RELATED_PROPS = new HashSet<string> { 
            DefaultProperty.S2CLink.ToLower(), DefaultProperty.S2CNextSnippet.ToLower(), DefaultProperty.S2CPrevSnippet.ToLower() };

        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////

        //[XmlIgnore]
        //public long ID { get; private set; }

        public string Name { get; set; }
        public string Value { get; set; }
        public long SnippetID { get; set; }
        public bool IsVisible { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////

        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        public SnippetProperty() : base() { }

        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        public SnippetProperty(string content, SerialFormat format) : base(content, format) { }


        /// <summary>
        /// Builds a new property not yet owned by any snippet
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public SnippetProperty(string name, string value)
        {
            Init(-1, -1, name, value, true);
        }


        /// <summary>
        /// Builds a new property owned by the given snippet
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public SnippetProperty(string name, string value, long snippetID, bool isVisible)
        {
            Init(-1, snippetID, name, value, isVisible);
        }


        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="snippetID"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void Init(long id, long snippetID, string name, string value, bool isVisible)
        {
            //ID = id;
            SnippetID = snippetID;
            Name = name;
            Value = value;
            IsVisible = isVisible;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected bool Init(SnippetProperty objToCopy)
        {
            if (objToCopy == null)
                return false;

            Init(0 /*objToCopy.ID*/, objToCopy.SnippetID, objToCopy.Name, objToCopy.Value, objToCopy.IsVisible);

            return true;
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Properties Validity Checks
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Checks whether the given property is valid
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool DataAreValid(string name, string value)
        {
            return (Utilities.PropValueIsValid(name, 1, false, MAX_NAME_LEN) &&
                    Utilities.PropValueIsValid(value, 1, false, MAX_VALUE_LEN));
            //return (Utilities.NameIsValid(name, 1, false, MAX_NAME_LEN) &&
            //        Utilities.PropValueIsValid(value, 1, false, MAX_VALUE_LEN));
            //return (Utilities.NameIsValid(name, 1, true, MAX_NAME_LEN) &&
            //        Utilities.PropValueIsValid(value, 1, true, MAX_VALUE_LEN));
        }

        /// <summary>
        /// Checks whether the given property is valid
        /// </summary>
        /// <returns></returns>
        public bool DataAreValid()
        {
            return DataAreValid(Name, Value);
        }

        /// <summary>
        /// Checks and cleans the given input list of properties and returns a new list only with properties:
        /// <ul>
        /// <li> not null</li>
        /// <li> valid</li>
        /// <li> not duplicated</li>
        /// <li> numeric for numeric properties</li>
        /// <li> not self-referring for related properties</li>
        /// </ul>
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static List<SnippetProperty> CleanPropertyList(ICollection<SnippetProperty> properties, long snippetID)
        {
            List<SnippetProperty> cleanProperties = new List<SnippetProperty>();
            foreach (SnippetProperty prop in properties)
            {
                if ((prop == null) || !prop.DataAreValid())
                    continue;

                string name = prop.Name.Trim(); // Utilities.ReplaceStrangeChars(prop.Name, STRANGE_CHAR_REPLACER);
                string value = prop.Value.Trim(); // Utilities.ReplaceStrangeChars(prop.Value, STRANGE_CHAR_REPLACER);
                
                //check for duplicates:
                bool found = false;
                foreach (SnippetProperty cleanProp in cleanProperties)
                {
                    if (cleanProp.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) &&
                        cleanProp.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    continue;

                //check for numeric properties (value should be a number for those properties):
                if (NUMERIC_PROPS.Contains(name.ToLower()))
                {
                    //check that the value is a number:
                    long val = -1;
                    if (!long.TryParse(value, out val))
                        continue;

                    //check for related snippets properties (value should not be the same snippet ID):
                    if ((snippetID > 0) && RELATED_PROPS.Contains(name.ToLower()))
                    {
                        if (val == snippetID)
                            continue;
                    }
                }

                //all checks seems OK, add the input property to the clean list:
                cleanProperties.Add(new SnippetProperty(name, value));
            }

            return cleanProperties;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////

        #region PUBLIC API
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     override object.Equals 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            SnippetProperty other = obj as SnippetProperty;

            if (!Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase))
                return false;
            if (!Value.Equals(other.Value, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;

        }

        /// <summary>
        ///     override object.GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        /// <summary>
        ///     override object.ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Name={0};Value={1};SnippetID={2};IsVisible={3}", Name, Value, SnippetID, IsVisible);
            //return string.Format("ID={0};Name={1};Value={2};SnippetID={3}", ID, Name, Value, SnippetID);
        }

        /// <summary>
        /// Returns the stirng representation of the property as NAME=VALUE,
        /// optionally quoted if spaces are present
        /// </summary>
        /// <returns></returns>
        public string ToFriendlyString()
        {
            return string.Format("{0}={1}", Name.PrintWithQuotes(), Value.PrintWithQuotes());
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////

        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override bool ParseFromJson(string content)
        {
            return Init(ParseFromJson<SnippetProperty>(content));
        }

        public override bool Parse(XElement xml)
        {
            if (xml == null)
                return false;

            string name = xml.Attribute("Name").Value;
            string value = xml.Value;
            Init(-1, 0, name, value, true);

            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Serialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public override XElement ToXML(bool internalPurpose = true)
        {
            return new XElement(ConfigReader.S2CNS + "Property",
                new XAttribute("Name", Name.FixXML()),
                Value.FixXML()
            );
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
