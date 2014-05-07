//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Snip2Code.Model.Entities;

namespace Snip2Code.Model.Abstract
{
    public interface IPropertiesRepository
    {
        /// <summary>
        /// Presents the last error arosen during the exceution of the class methods
        /// </summary>
        string LastErrorMsg { get; }

        #region Get Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns all the default properties that can be associated to a snippet.
        /// The first time they are fetched from DB, the other ones from in-memory cache.
        /// </summary>
        /// <returns>an empty list if any error occurred; the default properties list otherwise</returns>
        ICollection<DefaultProperty> GetDefaultProperties();

        /// <summary>
        /// Returns all the additional information related to specific default properties values.
        /// The first time they are fetched from DB, the other ones from in-memory cache.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, PropValueInfo> GetPropValuesInfo();

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Save Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Adds the given default property in the table of suggestions for new default properties: after reaching
        /// a certain threshold, such suggestions gain the title of default properties.
        /// </summary>
        /// <param name="defProp">object containing the info of the suggestion:
        ///     Name) Name of the suggested default property
        ///     PossibleValues) Collection of values for the suggested default property: it should contain at 
        ///     least one valid string
        ///     DependsOn) Collection of default properties which the suggestion depends on: each element of this
        ///     collection should have a valid name and a valid value
        /// </param>
        /// <param name="userID">user that gives the feedback</param>
        /// <returns>true if no error occurred, false otherwise</returns>
        bool SuggestDefaultProperty(DefaultProperty defProp, int userID);

        /// <summary>
        /// Adds the given default property in the related tables.
        /// </summary>
        /// <param name="defProp">object containing the info of the suggestion:
        ///     Name) Name of the suggested default property
        ///     PossibleValues) Collection of values for the suggested default property: it should contain at 
        ///     least one valid string
        ///     DependsOn) Collection of default properties which the suggestion depends on: each element of this
        ///     collection should have a valid name and a valid value
        /// </param>
        /// <returns>true if no error occurred, false otherwise</returns>
        bool AddDefaultProperty(DefaultProperty defProp);

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Reset Methods
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Resets the inmemory caches so that at the next invocation the GetDefaultProperties() method will re-fetch 
        /// the default properties from DB.
        /// </summary>
        void ResetCaches();

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
