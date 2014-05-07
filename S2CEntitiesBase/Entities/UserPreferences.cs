//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;

using System.Configuration;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Collections.Generic;

using Snip2Code.Utils;

namespace Snip2Code.Model.Entities
{
    //public enum PreferenceType
    //{
    //    undefined = -1,
    //    Description = 2,
    //    WebSite = 3,
    //    RedirectToWebSite = 4,
    //    DisplayProfileInCommunity = 5,

    //    HomeTopLeftPanel = 10,
    //    HomeMidLeftPanel = 11,
    //    HomeBotLeftPanel = 12,
    //    HomeTopRightPanel = 13,
    //    HomeMidRightPanel = 14,
    //    HomeBotRightPanel = 15,
    //    DisplayQuickLink = 17,

    //    nItems = 20,
    //    DisplayAvgRating = 21,

    //    alertDeliveryType = 30,
    //    ShareAlert = 34,
    //    CommentAlert = 36,
    //    AlertEmail = 40,
    //    AlertFormatHtml = 41,
 
    //    ForeignID = 100,
    //    GravatarEmail = 101,
    //    ThumbUploadNum = 61,

    //    //1000 or more are reserved for preferences that can be changed only by Admin
    //    MaxPublishedItems = 1002,
    //    FeaturedUser = 1006,
    //}

    public enum HomePageLeftPanel
    {
        NoPanel = 0,
        IntroductionHelpPanel = 1,
        MyTagMapPanel = 3,
        ShortcutsPanel = 4,
        UpgradeHelpPanel = 100,
    }

    public enum HomePageRightPanel
    {
        NoPanel = 0,
        MyTagMapPanel = 1,
        ShortcutsPanel = 2,
    }

    public enum AlertPeriod
    {
        Never = -1,
        ASAP = 0,
        Daily = 1,
        Weekly = 7,
        Monthly = 30
    }

    public enum AlertType
    {
        All = 0,
        ChannelsNews = 1,
        CustomFromAdmin = 2,
        BadgeNews = 3,
        NewSnippetInGroup = 4
    }

    /// <summary>
    /// Stores the information regarding the preferences of the user, saved in the 
    /// "Preferences" field in the "Users" table of the DB.
    /// </summary>
    public class UserPreferences : BaseEntity
    {
        #region STATIC VARIABLES
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /* Available premium features settings: */
        public static int[] s_MaxPublishingItems = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 400 };
        /* END Available premium features settings. */

        private const string s_rootName = "Preferences";

        /* Default constants settings: */
        public const int NITEMS = 10; //this should be the same number of defaultSettings.pageSize in s2clist.ts
        public const int DEF_MAXPUBITEMS = 100;
        public const bool DEF_FEATUREDUSER = false;
        public const int DEF_NUMOFTAGS_INFILTER = 30; 
        /* END Default constants settings. */

        /* Picture: */
        private string[] m_allowedType = { "gif", "png", "jpeg", "jpg" };
        private const int THUMB_DIM = 80;
        private const int IMAGE_DIM = 250;
        /* END Picture. */

        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        #region PROPERTIES
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /* Personal Info: */
        public string Description { get; set; }
        public string Location { get; set; }
        public string WebSite { get; set; }
        public bool RedirectToWebSite { get; set; }

        [XmlIgnore]
        public bool DisplayProfileInCommunity { get; private set; }

        [XmlIgnore]
        public bool DisplayReputation { get; set; }
        /* END Personal Info. */

        /* Home Page Settings: */
        [XmlIgnore]
        public HomePageLeftPanel HomeTopLeftPanel { get; set; }
        [XmlIgnore]
        public HomePageLeftPanel HomeMiddleLeftPanel { get; set; }
        [XmlIgnore]
        public HomePageLeftPanel HomeBottomLeftPanel { get; set; }

        [XmlIgnore]
        public HomePageRightPanel HomeTopRightPanel { get; set; }
        [XmlIgnore]
        public HomePageRightPanel HomeMiddleRightPanel { get; set; }
        [XmlIgnore]
        public HomePageRightPanel HomeBottomRightPanel { get; set; }

        [XmlIgnore]
        public bool DisplayQuickLink { get; set; }
        /* END Home Page Settings. */

        /* Browsing Options: */
        [XmlIgnore]
        public int NItems { get; set; }
        [XmlIgnore]
        public bool DisplayAvgRating { get; set; }
        [XmlIgnore]
        public bool CodeWrap { get; set; }
        [XmlIgnore]
        public int TagsInFilters { get; set; }
        /* END Browsing Options. */

        /* Alerting System: */
        [XmlIgnore]
        public bool NewShareAlert { get; set; }
        [XmlIgnore]
        public bool NewCommentAlert { get; set; }
        [XmlIgnore]
        public bool AlertFormatHtml { get; set; }
        [XmlIgnore]
        public bool AlertEmail { get; set; }
        [XmlIgnore]
        public Dictionary<AlertType, AlertPeriod> AlertPeriodicity { get; set; }
        [XmlIgnore]
        public List<int> UnsubscribedChannels { get; set; }
        /* END Alerting System. */

        /* Premium Services: */
        /* Limit properties. These cannot be changed directly by the user,
         * but may be modified by acquired services or by admin 
         */
        [XmlIgnore]
        public int MaxPublishedItems { get; set; }
        [XmlIgnore]
        public bool IsFeaturedUser { get; set; }
        /* END Premium Services. */

        /* Picture: */
        public int ThumbUploadNum { get; set; }
        public string GravatarEmail { get; set; }
        /* END Picture. */

        /* Third Parties Credentials */
        [XmlIgnore]
        public bool HasValidPsw { get; set; }

        [XmlIgnore]
        public string StackoverflowAccessToken { get; set; }
        [XmlIgnore]
        public string GithubAccessToken { get; set; }
        [XmlIgnore]
        public string GithubUsername { get; set; }
        [XmlIgnore]
        public string TwitterUsername { get; set; }
        /* END Third Parties Credentials */

        //static public XName XMLRootName { get { return ConfigReader.S2CNS + s_rootName; } } internal XML doesn't have namespaces
        [XmlIgnore]
        static public XName XMLRootName { get; private set; }

        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////


        #region CONSTRUCTORS
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public UserPreferences()
        {
            setDefaults();
        }


        /// <summary>
        ///     Creates a user preference reading the xml and applying the defaults where 
        ///     there are no information available
        /// </summary>
        /// <param name="xmlPref">xml fragment to parse</param>
        /// --------------------------------------------------------------------------------------------------
        public UserPreferences(XElement xmlPref)
            : this()
        {
            if (xmlPref == null)
                return;

            bool internalPurpose = !xmlPref.Name.Namespace.Equals(ConfigReader.S2CNS);

            /* Personal Info: */
            string descr = Description;
            xmlPref.ParseNode("description", ref descr, internalPurpose);
            Description = descr;

            string location = Location;
            xmlPref.ParseNode("location", ref location, internalPurpose);
            Location = descr;

            string webSite = WebSite;
            XElement node = xmlPref.ParseNode("webSite", ref webSite, internalPurpose);
            WebSite = webSite;
            if (node != null)
            {
                if (node.Attribute("redirect") != null)
                {
                    bool redirToWS = RedirectToWebSite;
                    bool.TryParse(node.Attribute("redirect").Value, out redirToWS);
                    RedirectToWebSite = redirToWS;
                }
            }

            bool displayInComm = DisplayProfileInCommunity;
            xmlPref.ParseNode("displayProfileInCommunity", ref displayInComm, internalPurpose);
            DisplayProfileInCommunity = displayInComm;

            bool displayReputation = DisplayReputation;
            xmlPref.ParseNode("displayReputation", ref displayReputation, internalPurpose);
            DisplayReputation = displayReputation;
            /* END Personal Info. */

            /* Browsing Options: */
            int items = NItems;
            xmlPref.ParseNode("n_items", ref items, internalPurpose);
            NItems = items;

            bool displayAvgRating = DisplayAvgRating;
            xmlPref.ParseNode("displayAvgRating", ref displayAvgRating, internalPurpose);
            DisplayAvgRating = displayAvgRating;

            bool codeWrap = CodeWrap;
            xmlPref.ParseNode("codeWrap", ref codeWrap, internalPurpose);
            CodeWrap = codeWrap;

            int tagsInFilters = TagsInFilters;
            xmlPref.ParseNode("tagsInFilters", ref tagsInFilters, internalPurpose);
            TagsInFilters = tagsInFilters;
            /* END Browsing Options. */

            /* Home Page Settings: */
            bool displayQuickLink = DisplayQuickLink;
            xmlPref.ParseNode("displayQuickLink", ref displayQuickLink, internalPurpose);
            DisplayQuickLink = displayQuickLink;

            // Left Panels
            HomePageLeftPanel tmpHTLP = HomeTopLeftPanel;
            xmlPref.ParseNode<HomePageLeftPanel>("homeTopLeftPanel", ref tmpHTLP, internalPurpose);
            HomeTopLeftPanel = tmpHTLP;

            HomePageLeftPanel tmpHMLP = HomeMiddleLeftPanel;
            xmlPref.ParseNode<HomePageLeftPanel>("homeMidLeftPanel", ref tmpHMLP, internalPurpose);
            HomeMiddleLeftPanel = tmpHMLP;

            HomePageLeftPanel tmpHBLP = HomeBottomLeftPanel;
            xmlPref.ParseNode<HomePageLeftPanel>("homeBotLeftPanel", ref tmpHBLP, internalPurpose);
            HomeBottomLeftPanel = tmpHBLP;

            // Right Panels
            HomePageRightPanel tmpHTRP = HomeTopRightPanel;
            xmlPref.ParseNode<HomePageRightPanel>("homeTopRightPanel", ref tmpHTRP, internalPurpose);
            HomeTopRightPanel = tmpHTRP;

            HomePageRightPanel tmpHMRP = HomeMiddleRightPanel;
            xmlPref.ParseNode<HomePageRightPanel>("homeMiddleRightPanel", ref tmpHMRP, internalPurpose);
            HomeMiddleRightPanel = tmpHMRP;

            HomePageRightPanel tmpHBRP = HomeBottomRightPanel;
            xmlPref.ParseNode<HomePageRightPanel>("homeBotRightPanel", ref tmpHBRP, internalPurpose);
            HomeBottomRightPanel = tmpHBRP;
            /* END Home Page Settings. */

            /* Alerting System:  */
            XElement alertNode = xmlPref.GetNode("alerts", internalPurpose);
            if (alertNode != null)
            {
                bool newShareAlert = NewShareAlert;
                alertNode.ParseNode("newShareAlert", ref newShareAlert, internalPurpose);
                NewShareAlert = newShareAlert;

                bool newCommentAlert = NewCommentAlert;
                alertNode.ParseNode("newCommentAlert", ref newCommentAlert, internalPurpose);
                NewCommentAlert = newCommentAlert;

                bool formatHtml = AlertFormatHtml;
                alertNode.ParseNode("formatHtml", ref formatHtml, internalPurpose);
                AlertFormatHtml = formatHtml;

                bool alertEmail = AlertEmail;
                alertNode.ParseNode("alertEmail", ref alertEmail, internalPurpose);
                AlertEmail = alertEmail;

                //ALert Periodicity:
                if (AlertPeriodicity == null)
                    AlertPeriodicity = new Dictionary<AlertType, AlertPeriod>();

                SetAlertPeriodicity(AlertPeriod.Weekly, AlertType.ChannelsNews, internalPurpose, alertNode);
                SetAlertPeriodicity(AlertPeriod.ASAP, AlertType.CustomFromAdmin, internalPurpose, alertNode);
                SetAlertPeriodicity(AlertPeriod.Weekly, AlertType.BadgeNews, internalPurpose, alertNode);
                SetAlertPeriodicity(AlertPeriod.ASAP, AlertType.NewSnippetInGroup, internalPurpose, alertNode);

                string unsubChannels = string.Empty;
                alertNode.ParseNode("unsubChannels", ref unsubChannels, internalPurpose);
                UnsubscribedChannels = unsubChannels.ParseIntoListOfInt(',');
            }
            /* END Alerting System */

            /* Premium Services: */
            XElement premiumNode = xmlPref.GetNode("premium", internalPurpose);
            if (premiumNode != null)
            {
                int maxPublishedItems = MaxPublishedItems;
                premiumNode.ParseNode("maxPublishedItems", ref maxPublishedItems, internalPurpose);
                if (maxPublishedItems < DEF_MAXPUBITEMS)
                    maxPublishedItems = DEF_MAXPUBITEMS;
                MaxPublishedItems = maxPublishedItems;

                bool featuredUser = IsFeaturedUser;
                premiumNode.ParseNode("featuredUser", ref featuredUser, internalPurpose);
                IsFeaturedUser = featuredUser;
            }
            /* END Premium Services. */

            /* Picture: */
            int thumbUploadNum = ThumbUploadNum;
            xmlPref.ParseNode("thumbUploadNum", ref thumbUploadNum, internalPurpose);
            ThumbUploadNum = thumbUploadNum;

            string gravatarEmail = GravatarEmail;
            xmlPref.ParseNode("gravatarEmail", ref gravatarEmail, internalPurpose);
            GravatarEmail = gravatarEmail;
            /* END Picture. */

            /* Third Parties Credentials */
            bool hasValidPsw = HasValidPsw;
            xmlPref.ParseNode("hasValidPsw", ref hasValidPsw, internalPurpose);
            HasValidPsw = hasValidPsw;

            string stackoverflowAccessToken = StackoverflowAccessToken;
            xmlPref.ParseNode("stackoverflowAccessToken", ref stackoverflowAccessToken, internalPurpose);
            StackoverflowAccessToken = stackoverflowAccessToken;

            string githubAccessToken = GithubAccessToken;
            xmlPref.ParseNode("githubAccessToken", ref githubAccessToken, internalPurpose);
            GithubAccessToken = githubAccessToken;

            string githubUsername = GithubUsername;
            xmlPref.ParseNode("githubUsername", ref githubUsername, internalPurpose);
            GithubUsername = githubUsername;

            string twitterUsername = TwitterUsername;
            xmlPref.ParseNode("twitterUsername", ref twitterUsername, internalPurpose);
            TwitterUsername = twitterUsername;
            /* END Third Parties Credentials */
        }

        private void SetAlertPeriodicity(AlertPeriod defaultPeriod, AlertType type, bool internalPurpose, XElement xmlPref)
        {
            AlertPeriod period = defaultPeriod;
            xmlPref.ParseNode<AlertPeriod>("alertPeriod_" + type, ref period, internalPurpose);
            if (AlertPeriodicity.ContainsKey(type))
                AlertPeriodicity[type] = period;
            else
                AlertPeriodicity.Add(type, period);
        }

        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////


        #region UTILITIES
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///     Set the defaults of the Preferences in case of errors.
        /// </summary>
        /// --------------------------------------------------------------------------------------------------
        private void setDefaults()
        {
            /* Personal Info: */
            Description = string.Empty;
            Location = string.Empty;
            WebSite = string.Empty;
            RedirectToWebSite = false;
            DisplayProfileInCommunity = true;
            DisplayReputation = true;
            /* END Personal Info. */

            /* Home Page Settings: */
            HomeTopLeftPanel = HomePageLeftPanel.MyTagMapPanel;
            HomeMiddleLeftPanel = HomePageLeftPanel.ShortcutsPanel;
            HomeBottomLeftPanel = HomePageLeftPanel.NoPanel;

            HomeTopRightPanel = HomePageRightPanel.NoPanel;
            HomeMiddleRightPanel = HomePageRightPanel.NoPanel;
            HomeBottomRightPanel = HomePageRightPanel.NoPanel;

            DisplayQuickLink = true;
            /* END Home Page Settings. */

            /* Browsing Options: */
            NItems = NITEMS;
            DisplayAvgRating = true;
            CodeWrap = true;
            TagsInFilters = DEF_NUMOFTAGS_INFILTER;
            /* END Browsing Options. */

            /* Alerting System: */
            NewShareAlert = true;
            NewCommentAlert = true;
            AlertEmail = true;
            AlertFormatHtml = true;
            AlertPeriodicity = new Dictionary<AlertType, AlertPeriod>();
            AlertPeriodicity.Add(AlertType.ChannelsNews, AlertPeriod.Weekly);
            AlertPeriodicity.Add(AlertType.BadgeNews, AlertPeriod.Weekly);
            AlertPeriodicity.Add(AlertType.NewSnippetInGroup, AlertPeriod.ASAP);
            UnsubscribedChannels = new List<int>();
            /* END Alerting System. */

            /* Premium Services: */
            MaxPublishedItems = DEF_MAXPUBITEMS;
            IsFeaturedUser = false;
            /* END Premium Services. */

            /* Picture: */
            ThumbUploadNum = 1;
            GravatarEmail = string.Empty;
            /* END Picture. */

            /* Third Parties Credentials */
            HasValidPsw = true;
            StackoverflowAccessToken = string.Empty;
            GithubAccessToken = string.Empty;
            GithubUsername = string.Empty;
            TwitterUsername = string.Empty;
            /* END Third Parties Credentials */
        }

        public override XElement ToXML(bool internalPurpose = true)
        {
            XElement xml = new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + s_rootName);

            xml.Add(
                /* Personal Info: */
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "description", Description.FixXML()),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "location", Location.FixXML()),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "webSite", WebSite.FixXML(),
                    new XAttribute("redirect", RedirectToWebSite)),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "displayProfileInCommunity", DisplayProfileInCommunity),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "displayReputation", DisplayReputation),
                /* END Personal Info. */

                /* Home Page Settings: */
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "homeTopLeftPanel", HomeTopLeftPanel),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "homeMidLeftPanel", HomeMiddleLeftPanel),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "homeBotLeftPanel", HomeBottomLeftPanel),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "homeTopRightPanel", HomeTopRightPanel),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "homeMiddleRightPanel", HomeMiddleRightPanel),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "homeBotRightPanel", HomeBottomRightPanel),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "displayQuickLink", DisplayQuickLink),
                /* END Home Page Settings. */

                /* Browsing Options: */
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "n_items", NItems),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "displayAvgRating", DisplayAvgRating),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "codeWrap", CodeWrap),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "tagsInFilters", TagsInFilters),
                /* END Browsing Options. */

                /* Alerting System: */
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "alerts", 
                    new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "newShareAlert", NewShareAlert),
                    new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "newCommentAlert", NewCommentAlert),
                    new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "alertEmail", AlertEmail),
                    new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "formatHtml", AlertFormatHtml),
                    GetXElementForPeriodicity(internalPurpose, AlertType.ChannelsNews, AlertPeriod.Weekly),
                    GetXElementForPeriodicity(internalPurpose, AlertType.CustomFromAdmin, AlertPeriod.ASAP),
                    GetXElementForPeriodicity(internalPurpose, AlertType.BadgeNews, AlertPeriod.Weekly),
                    GetXElementForPeriodicity(internalPurpose, AlertType.NewSnippetInGroup, AlertPeriod.ASAP), 
                    new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "unsubChannels", Utilities.MergeIntoCommaSeparatedString(UnsubscribedChannels, true))
                ),
                /* END Alerting System. */

                /* Premium Preferences */
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "premium",
                    new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "maxPublishedItems", MaxPublishedItems),
                    new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "featuredUser", IsFeaturedUser)),
                /* END Premium Preferences */

                /* Picture: */
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "thumbUploadNum", ThumbUploadNum),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "gravatarEmail", GravatarEmail),
                /* END Picture. */

                /* Third Parties Credentials */
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "hasValidPsw", HasValidPsw),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "stackoverflowAccessToken", StackoverflowAccessToken.FixXML()),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "githubAccessToken", GithubAccessToken.FixXML()),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "githubUsername", GithubUsername.FixXML()),
                new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + "twitterUsername", TwitterUsername.FixXML())
                /* END Third Parties Credentials */
            );

            return xml;
        }

        private XElement GetXElementForPeriodicity(bool internalPurpose, AlertType type, AlertPeriod defaultVal)
        {
            if (AlertPeriodicity.ContainsKey(type))
                defaultVal = AlertPeriodicity[type];
            return new XElement((internalPurpose ? "" : ConfigReader.S2CNS) + ("alertPeriod_" + type), defaultVal);
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

