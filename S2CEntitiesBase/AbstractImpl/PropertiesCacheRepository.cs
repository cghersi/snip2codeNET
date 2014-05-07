//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Snip2Code.Model.Entities;
using Snip2Code.Model.Utils;

namespace Snip2Code.Model.Abstract.Impl
{
    /// <summary>
    /// This class exposes extension metods used to collect the stuff and utilities that can be shared among the implementations
    /// of IPropertiesRepository interface
    /// </summary>
    public static class PropertiesCacheRepository
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Internal Static Caches
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Map of default properties: keys are the name of default properties, values are the list of possible values for the given name
        /// </summary> 
        private static Dictionary<string, List<string>> s_defPropertyValues = null;

        /// <summary>
        /// Set of default properties names and values: the complete collection of names and values that constitutes the
        /// default properties managed in Snip2Code
        /// </summary>
        private static HashSet<string> s_defPropertyItemsSet = null;

        /// <summary>
        /// Map of default BASIC properties, that is, the default properties with no dependencies on other default properties;
        /// Keys are the name of default properties, values are the list of possible values for the given name
        /// </summary>
        private static IDictionary<string, List<string>> s_basicProps = null;

        /// <summary>
        /// Map of values related to a single name for default property.
        /// It can be used to guess the use of a default property instead of simple tag
        /// </summary>
        private static IDictionary<string, string> s_defValuesRelatedToSingleKey = null;

        //default properties sets to be used in the creation of new snippets
        private static List<string> s_browsers = null;
        private static List<string> s_architectures = null;
        private static List<string> s_osTypes = null;
        private static List<string> s_windowsVersions = null;
        private static List<string> s_languages = null;

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Get Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns all the default properties that can be associated to a snippet.
        /// The first time they are fetched from DB, the other ones from in-memory cache.
        /// </summary>
        /// <returns>an empty map if any error occurred; the default properties otherwise, 
        /// where the keys are the properties names and the values are the actual possible values of that property</returns>
        public static Dictionary<string, List<string>> GetDefaultPropertiesValues(this IPropertiesRepository repo)
        {
            if (repo == null)
                return new Dictionary<string, List<string>>();
            if (s_defPropertyValues == null)
            {
                //init default properties if needed:
                ICollection<DefaultProperty> defProperties = repo.GetDefaultProperties();

                //fill all the list with the proper values:
                s_defPropertyValues = new Dictionary<string, List<string>>();
                foreach (DefaultProperty defProp in defProperties)
                {
                    s_defPropertyValues.Add(defProp.Name, defProp.PossibleValues);
                }
            }
            return s_defPropertyValues;
        }

        /// <summary>
        /// Returns the set of default properties names and values: the complete collection of names and values that constitutes the
        /// default properties managed in Snip2Code.
        /// The first time they are fetched from DB, the other ones from in-memory cache.
        /// </summary>
        /// <returns>an empty set if any error occurred; the default properties names and values otherwise</returns>
        public static HashSet<string> GetDefaultPropertiesItems(this IPropertiesRepository repo)
        {
            if (repo == null)
                return new HashSet<string>();
            if (s_defPropertyItemsSet == null)
            {
                //init default properties if needed:
                Dictionary<string, List<string>> defPropertyValues = repo.GetDefaultPropertiesValues();

                //fill all the list with the proper values:
                s_defPropertyItemsSet = new HashSet<string>(defPropertyValues.Keys, new IgnoreCaseComparer());
                foreach (string key in defPropertyValues.Keys)
                {
                    s_defPropertyItemsSet.UnionWith(defPropertyValues[key]);
                }
            }
            return s_defPropertyItemsSet;
        }


        /// <summary>
        /// Returns all the default properties that don't have any dependency from other default properties.
        /// The first time they are fetched from DB, the other ones from in-memory cache.
        /// </summary>
        /// <returns>an empty map if any error occurred; the default basic properties otherwise, 
        /// where the keys are the properties names and the values are the actual possible values of that basic property</returns>
        public static IDictionary<string, List<string>> GetBasicProperties(this IPropertiesRepository repo)
        {
            if (s_basicProps == null)
            {
                //init default properties if needed:
                ICollection<DefaultProperty> defProperties = repo.GetDefaultProperties();

                s_basicProps = DefaultProperty.FilterBasicProperties(defProperties);
            }
            return s_basicProps;
        }


        /// <summary>
        /// Retrieves the list of default Architectures supported by Snip2Code
        /// </summary>
        /// <returns>an empty list if any error occurred, the actual list otherwise</returns>
        public static List<string> GetDefaultArchitectures(this IPropertiesRepository repo)
        {
            if (!LoadDefaultProperties(repo))
                return new List<string>();
            return s_architectures;
        }


        /// <summary>
        /// Retrieves the list of default Browsers supported by Snip2Code
        /// </summary>
        /// <returns>an empty list if any error occurred, the actual list otherwise</returns>
        public static List<string> GetDefaultBrowsers(this IPropertiesRepository repo)
        {
            if (!LoadDefaultProperties(repo))
                return new List<string>();
            return s_browsers;
        }

        /// <summary>
        /// Retrieves the list of default OS Types supported by Snip2Code
        /// </summary>
        /// <returns>an empty list if any error occurred, the actual list otherwise</returns>
        public static List<string> GetDefaultOsTypes(this IPropertiesRepository repo)
        {
            if (!LoadDefaultProperties(repo))
                return new List<string>();
            return s_osTypes;
        }

        /// <summary>
        /// Retrieves the list of default Windows Versions supported by Snip2Code
        /// </summary>
        /// <returns>an empty list if any error occurred, the actual list otherwise</returns>
        public static List<string> GetDefaultWinVersions(this IPropertiesRepository repo)
        {
            if (!LoadDefaultProperties(repo))
                return new List<string>();
            return s_windowsVersions;
        }

        /// <summary>
        /// Retrieves the list of default Languages supported by Snip2Code
        /// </summary>
        /// <returns>an empty list if any error occurred, the actual list otherwise</returns>
        public static List<string> GetDefaultLanguages(this IPropertiesRepository repo)
        {
            if (!LoadDefaultProperties(repo))
                return new List<string>();
            return s_languages;
        }


        private static bool LoadDefaultProperties(this IPropertiesRepository repo)
        {
            //check whether the lists have been already retrieved
            if (s_osTypes != null)
                return true;

            //get the list of default values:
            ICollection<DefaultProperty> defaultProps = repo.GetDefaultProperties();
            if (defaultProps == null)
            {
                log.ErrorFormat("Cannot retrieve default properties");
                return false;
            }

            //fill all the list with the proper values:
            foreach (DefaultProperty defProp in defaultProps)
            {
                switch (defProp.Name)
                {
                    case DefaultProperty.Architecture:
                        s_architectures = defProp.PossibleValues;
                        break;
                    case DefaultProperty.Browser:
                        s_browsers = defProp.PossibleValues;
                        break;
                    case DefaultProperty.OS:
                        s_osTypes = defProp.PossibleValues;
                        break;
                    case DefaultProperty.WindowsVersion:
                        s_windowsVersions = defProp.PossibleValues;
                        break;
                    case DefaultProperty.Language:
                        s_languages = defProp.PossibleValues;
                        break;
                }
            }

            return true;
        }


        /// <summary>
        /// Map of values related to a single name for default property.
        /// It can be used to guess the use of a default property instead of simple tag
        /// </summary>
        /// <returns>an mepty map if any error occurred; the map with keys: default properties values (lower case); values, the related property name </returns>
        public static IDictionary<string, string> GetDefaultValuesRelatedToSingleKey(this IPropertiesRepository repo)
        {
            if (repo == null)
                return new Dictionary<string, string>();

            if (s_defValuesRelatedToSingleKey == null)
            {
                s_defValuesRelatedToSingleKey = new Dictionary<string, string>();
                Dictionary<string, List<string>> defPropVal = repo.GetDefaultPropertiesValues();
                foreach (string defPropKey in defPropVal.Keys)
                {
                    List<string> values = defPropVal[defPropKey];
                    if (values != null)
                    {
                        foreach (string value in values)
                        {
                            string valueLower = value.ToLower();

                            // put an empty value to signal that the property value is linked to multiple keys
                            if (s_defValuesRelatedToSingleKey.ContainsKey(valueLower))
                                s_defValuesRelatedToSingleKey[valueLower] = string.Empty;
                            else
                                s_defValuesRelatedToSingleKey.Add(valueLower, defPropKey);
                        }
                    }
                }

                //purge eventual values with multiple keys:
                List<string> keyToRemove = new List<string>(s_defValuesRelatedToSingleKey.Keys);
                foreach (string key in keyToRemove)
                {
                    if (string.IsNullOrEmpty(s_defValuesRelatedToSingleKey[key]))
                        s_defValuesRelatedToSingleKey.Remove(key);
                }
            }

            return s_defValuesRelatedToSingleKey;
        }


        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Reset Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Resets the inmemory caches so that at the next invocation the methods will re-fetch 
        /// the default properties from DB.
        /// </summary>
        public static void Reset(this IPropertiesRepository repo)
        {
            if (repo != null)
                repo.ResetCaches();
            s_defPropertyValues = null;
            s_defPropertyItemsSet = null;
            s_basicProps = null;
            s_browsers = null;
            s_architectures = null;
            s_osTypes = null;
            s_windowsVersions = null;
            s_languages = null;
            s_defValuesRelatedToSingleKey = null;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
