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
    public interface IBaseEntity
    {
        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Creates and object with the characteristics of a base entity.
        /// This means that e.g. an object of Snip2Code.Model.Comm namespace is turned into a plain BaseEntity
        /// </summary>
        /// <returns></returns>
        BaseEntity ToBaseEntity();

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Serialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Serializes this object in XML fragment
        /// </summary>
        /// <param name="internalPurpose">true if this XML is used to store the object inside the DB, 
        /// false if the XML representation is going to pe presented outside the backend</param>
        /// <returns></returns>
        XElement ToXML(bool internalPurpose = true);

        /// <summary>
        /// Creates and object ready to be serialized in JSON
        /// </summary>
        /// <returns></returns>
        BaseEntity ToJSONEntity();

        /// <summary>
        /// Builds the string serialization of this object depending on the given format
        /// </summary>
        /// <param name="format">serialization format</param>
        /// <returns>string serialization of this object if no error occurred; string.Empty otherwise</returns>
        string Serialize(SerialFormat format);

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
