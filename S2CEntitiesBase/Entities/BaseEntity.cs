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
    /// <summary>
    /// Represents the base class for all the entities in the model
    /// </summary>
    public abstract class BaseEntity : IBaseEntity
    {
        #region Members and Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected static fastJSON.JSON jsonUtils = EntityUtils.InitJSONUtils();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [XmlIgnore]
        public bool IsValid { get; protected set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        public BaseEntity() 
        {
            IsValid = true;
        }


        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        protected BaseEntity(string content, SerialFormat format)
            : base()
        {
            switch (format)
            {
                case SerialFormat.JSON:
                    IsValid = ParseFromJson(content); 
                    break;
                case SerialFormat.XML:
                    IsValid = Parse(XElement.Parse(content));
                    break;
                default:
                    IsValid = false;
                    break;
            }
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Deserialize a JSON fragment into a new object
        /// </summary>
        /// <param name="xml"></param>
        protected virtual bool ParseFromJson(string content) { return true; }

        /// <summary>
        /// Deserialize an XML fragment into a new object
        /// </summary>
        /// <param name="xml"></param>
        public virtual bool Parse(XElement xml) { return true; }

        /// <summary>
        /// Deserialize a JSON fragment into a new object
        /// </summary>
        /// <param name="xml"></param>
        public static T ParseFromJson<T>(string json)
        {
            try
            {
                return jsonUtils.ToObject<T>(json);
            }
            catch (Exception e)
            {
                log.ErrorFormat("Cannot parse from json string:{0} due to {1}", json.PrintNull(), e);
                return default(T);
            }
        }

        /// <summary>
        /// Creates and object with the characteristics of a base entity.
        /// This means that e.g. an object of Snip2Code.Model.Comm namespace is turned into a plain BaseEntity
        /// </summary>
        /// <returns></returns>
        public virtual BaseEntity ToBaseEntity()
        {
            return this;
        }

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
        public abstract XElement ToXML(bool internalPurpose = true);

        /// <summary>
        /// Creates an object ready to be serialized in JSON
        /// </summary>
        /// <returns></returns>
        public virtual BaseEntity ToJSONEntity()
        {
            return this;
        }

        /// <summary>
        /// Builds the string serialization of this object depending on the given format
        /// </summary>
        /// <param name="format">serialization format</param>
        /// <returns>string serialization of this object if no error occurred; string.Empty otherwise</returns>
        public string Serialize(SerialFormat format)
        {
            switch (format)
            {
                case SerialFormat.JSON:
                    return jsonUtils.ToJSON(ToJSONEntity());
                case SerialFormat.XML:
                    return ToXML(false).ToString();
                default:
                    return string.Empty;
            }
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
