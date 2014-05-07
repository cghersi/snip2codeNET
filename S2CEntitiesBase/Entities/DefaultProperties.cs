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
    /// This class represents a common property that a user may add to a snippet, like
    /// language, os, platform, etc.
    /// the property has a list of possible values and some links with children properties.
    /// e.g. 
    /// language = c# => .net framework
    /// language = java => jvm version
    /// </summary>
    public class DefaultProperty : BaseEntity
    {
        #region Default Properties Names
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public const string OS = "OS";
        public const string WindowsOS = "Windows";
        public const string MacOS = "Mac OS X";
        public const string Extension = "Extension";
        public const string Language = "Language";
        public const string NETFramework = ".NETFramework";
        public const string Framework = "Framework";
        public const string JVM = "JVM";
        public const string Browser = "Browser";
        public const string Technology = "Technology";
        public const string Architecture = "Architecture";
        public const string WindowsVersion = "Windows Version";
        public const string LinuxVersion = "Linux Version";
        public const string MacOSVersion = "MacOS Version";
        public const string AndroidVersion = "Android Version";
        public const string ChromeVersion = "Chrome Version";
        public const string IEVersion = "IE Version";
        public const string SafariVersion = "Safari Version";
        public const string OperaVersion = "Opera Version";
        public const string FirefoxVersion = "Firefox Version";
        public const string DBMS = "DBMS";
        public const string Source = "Source";
        public const string MyGists = "MyGists";
        public const string Channel = "S2CChannel";
        public const string S2CLink = "Related Snippet";
        public const string S2CNextSnippet = "Next Snippet";
        public const string S2CPrevSnippet = "Previous Snippet";
        public const string OwnerGroupID = "ownerGroupID";
        public const string User = "user";
        public const string CreatorID = "CreatorID";
        public const string VisitedBy = "VisitedBy";
        public const string Badge = "Badge";
        public const string LastEditingUser = "LastEditUser";

        public static Dictionary<string, string> nicePropNames = new Dictionary<string, string>()
        {
          { OS.ToLower(), "Operating System" },
          { WindowsOS.ToLower(), "Windows" },
          { MacOS.ToLower(),  "Mac OS X" },
          { Extension.ToLower(),  "Extension" },
          { Language.ToLower(),  "Language" },
          { NETFramework.ToLower(),  ".NET Framework" },
          { Framework.ToLower(),  "Framework" },
          { JVM.ToLower(),  "JVM" },
          { Browser.ToLower(),  "Browser" },
          { Technology.ToLower(),  "Technology" },
          { Architecture.ToLower(),  "Architecture" },
          { WindowsVersion.ToLower(),  "Windows Version" },
          { LinuxVersion.ToLower(),  "Linux Version" },
          { MacOSVersion.ToLower(),  "MacOS Version" },
          { AndroidVersion.ToLower(),  "Android Version" },
          { ChromeVersion.ToLower(),  "Chrome Version" },
          { IEVersion.ToLower(),  "IE Version" },
          { SafariVersion.ToLower(),  "Safari Version" },
          { OperaVersion.ToLower(),  "Opera Version" },
          { FirefoxVersion.ToLower(),  "Firefox Version" },
          { DBMS.ToLower(),  "DBMS" },
          { Source.ToLower(),  "Source" },
          { MyGists.ToLower(),  "MyGists" },
          { Channel.ToLower(),  "Channel" },
          { S2CLink.ToLower(),  "Related Snippet" },
          { S2CNextSnippet.ToLower(),  "Next Snippet" },
          { S2CPrevSnippet.ToLower(),  "Previous Snippet" },
          { OwnerGroupID.ToLower(),  "Group" },
          { User.ToLower(),  "User"  },
          { CreatorID.ToLower(),  "Creator"  },
          { VisitedBy.ToLower(),  "Visited By" },
          { Badge.ToLower(),  "Badge" }
        };

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////

        public string Name { get; set; }

        public int Order { get; set; }

        public List<string> PossibleValues { get; set; }

        /// <summary>
        /// Represents the valued properties that allow this default property to make sense.
        /// Null if this default property always makes sense 
        /// </summary>
        public List<SnippetProperty> DependsOn { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        public DefaultProperty() : base() { }

        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        public DefaultProperty(string content, SerialFormat format) : base(content, format) { }


        public DefaultProperty(string name, int order, List<string> possibleValues)
        {
            Init(name, order, possibleValues, null);
        }

        public DefaultProperty(string name, List<string> possibleValues)
        {
            Init(name, 0, possibleValues, null);
        }

        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param param name="name"></param>
        /// <param name="possibleValues"></param>
        /// <param name="dependsOn"></param>
        protected void Init(string name, int order, List<string> possibleValues, List<SnippetProperty> dependsOn)
        {
            Name = name;
            PossibleValues = possibleValues;
            DependsOn = dependsOn;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected bool Init(DefaultProperty objToCopy)
        {
            if (objToCopy == null)
                return false;

            Init(objToCopy.Name, objToCopy.Order, objToCopy.PossibleValues, objToCopy.DependsOn);
            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region EQUALITY_COMPARER
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Equals(object obj)
        {
            //if obj is not a DefaultProperty, it is different from the current object:
            DefaultProperty prop = obj as DefaultProperty;
            if (prop == null)
                return false;

            return this.Name.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////


        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override bool ParseFromJson(string content)
        {
            return Init(ParseFromJson<DefaultProperty>(content));
        }

        public override bool Parse(XElement xml)
        {
            if (xml == null)
                return false;

            string name = string.Empty;
            int order = 0;
            List<string> possibleValues = new List<string>();
            List<SnippetProperty> dependsOn = new List<SnippetProperty>();

            xml.ParseNode("Name", ref name, false);
            xml.ParseNode("Order", ref order, false);

            XElement possValues = xml.GetNode("PossibleValues", false);
            if (possValues != null)
            {
                List<XElement> elems = possValues.GetNodes("Value", false);    
                foreach (XElement el in elems)
                {
                    possibleValues.Add(el.Value);
                }
            }

            XElement dependsOnElem = xml.GetNode("DependsOn", false);
            if (dependsOnElem != null)
            {
                List<XElement> elems = dependsOnElem.GetNodes("Property", false);
                foreach (XElement el in elems)
                {
                    dependsOn.Add(new SnippetProperty(el.ToString(), SerialFormat.XML));
                }
            }

            Init(name, order, possibleValues, dependsOn);

            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Serialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public override XElement ToXML(bool internalPurpose = true)
        {
            XDocument res = Utilities.CreateXMLDoc("DefaultProperty", false);

            res.Root.Add(
                new XElement(ConfigReader.S2CNS + "Name", Name.FixXML()),
                new XElement(ConfigReader.S2CNS + "Order", Order)
            );

            if (PossibleValues != null)
                res.Root.Add(ToDefValuesXML());

            if (DependsOn != null)
                res.Root.Add(ToDependsOnXML());

            return res.Root;
        }


        public XElement ToDefValuesXML()
        {
            XElement res = new XElement(ConfigReader.S2CNS + "PossibleValues");
            foreach (string defValue in PossibleValues)
            {
                res.Add(new XElement(ConfigReader.S2CNS + "Value", defValue.FixXML()));
            }
            return res;
        }

        public XElement ToDependsOnXML()
        {
            XElement res = new XElement(ConfigReader.S2CNS + "DependsOn");
            foreach (SnippetProperty prop in DependsOn)
            {
                res.Add(prop.ToXML());
            }
            return res;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Utilities
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GetAvgSnippetVisitName(int topPercentage)
        {
            return string.Format("Avg{0}TopPercSnippetVisits", topPercentage);
        }

        public override string ToString()
        {
            return string.Format("Name={0};Order={1};Values={2};DependsOn={3}",
                        Name, Order, PossibleValues.Print<string>(), DependsOn.Print<SnippetProperty>());
        }


        /// <summary>
        /// Filters the given collection of default properties providing only the properties that depends 
        /// on the given current property.
        /// The keys of the dictionary are the names of the dependent properties, 
        /// while the values of this dictionary are the lists of allowed values for each property
        /// </summary>
        /// <param name="defaultProperties">list of all available default properties, i.e. the result of 
        ///     IPropertiesRepository.GetDefaultProperties() method</param>
        /// <param name="property">current property whose dependencies are to be discovered</param>
        /// <returns>an empty dictionary if any error occurred</returns>
        public static IDictionary<string, List<string>> FilterDependentProperties(
            ICollection<DefaultProperty> defaultProperties, SnippetProperty property)
        {
            Dictionary<string, List<string>> res = new Dictionary<string, List<string>>();
            if ((property == null) || (defaultProperties == null))
                return res;

            foreach (DefaultProperty defProp in defaultProperties)
            {
                if ((defProp == null) || (defProp.DependsOn.IsNullOrEmpty()))
                    continue;

                //check if this default property depends on the current property:
                if (defProp.DependsOn.Contains(property))
                {
                    //add a new entry in the resulting dictionary:
                    res.Add(defProp.Name, defProp.PossibleValues);
                }
            }

            return res;
        }


        /// <summary>
        /// Filters the given collection of default properties providing only the properties that depends 
        /// on the given current properties.
        /// The keys of the dictionary are the names of the dependent properties, 
        /// while the values of this dictionary are the lists of allowed values for each property
        /// </summary>
        /// <param name="defaultProperties">list of all available default properties, i.e. the result of 
        ///     IPropertiesRepository.GetDefaultProperties() method</param>
        /// <param name="properties">current properties whose dependencies are to be discovered</param>
        /// <returns>an empty dictionary if any error occurred</returns>
        public static IDictionary<string, List<string>> FilterDependentProperties(
            ICollection<DefaultProperty> defaultProperties, ICollection<SnippetProperty> properties)
        {
            Dictionary<string, List<string>> res = new Dictionary<string, List<string>>();
            if ((properties == null) || (defaultProperties == null))
                return res;

            foreach (SnippetProperty prop in properties)
            {
                if (prop == null)
                    continue;

                IDictionary<string, List<string>> resForSingleProp = FilterDependentProperties(defaultProperties, prop);
                foreach (string resKey in resForSingleProp.Keys)
                {
                    if (!res.ContainsKey(resKey))
                        res.Add(resKey, resForSingleProp[resKey]);
                }
            }

            return res;
        }


        /// <summary>
        /// Filters the given collection of default properties providing only the properties that 
        /// does not have any parent dependency.
        /// The keys of the dictionary are the names of the dependent properties, 
        /// while the values of this dictionary are the lists of allowed values for each property
        /// </summary>
        /// <param name="defaultProperties">list of all available default properties, i.e. the result of 
        ///     IPropertiesRepository.GetDefaultProperties() method</param>
        /// <returns>an empty dictionary if any error occurred</returns>
        public static IDictionary<string, List<string>> FilterBasicProperties(ICollection<DefaultProperty> defaultProperties)
        {
            Dictionary<string, List<string>> res = new Dictionary<string, List<string>>();
            if (defaultProperties == null)
                return res;

            foreach (DefaultProperty defProp in defaultProperties)
            {
                if ((defProp != null) && (defProp.DependsOn.IsNullOrEmpty()))
                    //add a new entry in the resulting dictionary:
                    res.Add(defProp.Name, defProp.PossibleValues);                
            }

            return res;
        }


        /// <summary>
        /// Filters the given collection of default properties providing only the distinct names of the properties.
        /// </summary>
        /// <param name="defaultProperties">list of all available default properties, i.e. the result of 
        ///     IPropertiesRepository.GetDefaultProperties() method</param>
        /// <returns>an empty list if any error occurred</returns>
        public static IList<string> GetPropertiesNames(ICollection<DefaultProperty> defaultProperties)
        {
            List<string> res = new List<string>();
            if (defaultProperties == null)
                return res;

            foreach (DefaultProperty defProp in defaultProperties)
            {
                if (defProp != null)
                    res.Add(defProp.Name);
            }

            return res;
        }


        /// <summary>
        /// Filters the given collection of default properties providing the possible values of the default property
        /// with the given name
        /// </summary>
        /// <param name="defaultProperties">list of all available default properties, i.e. the result of 
        ///     IPropertiesRepository.GetDefaultProperties() method</param>
        /// <param name="propName">the name of the property to look for</param>
        /// <returns>an empty list if any error occurred</returns>
        public static IList<string> GetPossibleValues(ICollection<DefaultProperty> defaultProperties, string propName)
        {
            if ((defaultProperties == null) || string.IsNullOrWhiteSpace(propName))
                return new List<string>();

            foreach (DefaultProperty defProp in defaultProperties)
            {
                if (defProp.Name.Equals(propName, StringComparison.InvariantCultureIgnoreCase))
                    return defProp.PossibleValues;
            }

            return new List<string>();
        }


        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
