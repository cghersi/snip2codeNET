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
using System.Drawing;

namespace Snip2Code.Model.Entities
{
    /// ===============================================================================
    /// <summary>
    /// Stores the information regarding the preferences of the group, saved in the 
    /// "Preferences" field in the "Groups" table of the DB.
    /// </summary>
    /// ===============================================================================
    public class GroupPreferences : BaseEntity
    {
        #region STATIC VARIABLES
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public const string GROUP_ACCENT = "LightGreen";
        private const string s_rootName = "Preferences";

        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        #region PROPERTIES
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        [XmlIgnore]
        static public XName XMLRootName { get { return s_rootName; } }

        /* GUI */
        public string ColorCode { get; set; }

        public string ColorCodeRGB { get { return ToRGB(ColorCode); } }
        /* END GUI */

        /* Third Parties Credentials */
        [XmlIgnore]
        public string GithubLogin { get; set; }
        /* END Third Parties Credentials */

        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////


        #region CONSTRUCTORS
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public GroupPreferences() : base()
        {
            setDefaults();
        }


        /// <summary>
        ///     Creates an instance reading the xml and applying the defaults where 
        ///     there are no information available
        /// </summary>
        /// <param name="xml">xml fragment to parse</param>
        /// --------------------------------------------------------------------------------------------------
        public GroupPreferences(XElement xml)
            : this()
        {
            if (xml == null)
                return;

            bool internalPurpose = !xml.Name.Namespace.Equals(ConfigReader.S2CNS);

            /* GUI */
            string colorcode = GROUP_ACCENT;
            xml.ParseNode("ColorCode", ref colorcode, internalPurpose);
            ColorCode = colorcode;
            /* END GUI */

            /* Third Parties Credentials */
            string githubLogin = string.Empty;
            xml.ParseNode("GithubLogin", ref githubLogin, internalPurpose);
            GithubLogin = githubLogin;
            /* END Third Parties Credentials */
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
            /* GUI */
            ColorCode = GROUP_ACCENT;
            /* END GUI */

            /* Third Parties Credentials */
            GithubLogin = string.Empty;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Serialization
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override XElement ToXML(bool internalPurpose = true)
        {
            XElement xml = new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + s_rootName);
            xml.Add(
                /* GUI */
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "ColorCode", ColorCode),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "ColorCodeRGB", ColorCodeRGB),
                /* END GUI */

                /* Third Parties Credentials */
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "GithubLogin", GithubLogin.FixXML())
                /* Parties Credentials */
            );

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


        private static string ToRGB(string colorName)
        {
            Color c = Color.FromName(colorName);
            if (c != null)
                return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
            else
                return "#90EE90"; //lightgreen
        }
    }


    public enum PricePlans
    {
        Personal = 0,
        Pro = 10,
        Enterprise = 100
    }
}
