//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Xml.Linq;

using Snip2Code.Utils;

namespace Snip2Code.Model.Entities
{
    public class PropValueInfo : BaseEntity
    {
        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        public string PropValue { get; set; }

        public string TwitterHashTag { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        public PropValueInfo() 
        { 
        }

        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="propValue"></param>
        /// <param name="twitterHashTag"></param>
        protected void Init(string propValue, string twitterHashTag)
        {
            PropValue = propValue;
            TwitterHashTag = twitterHashTag;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected void Init(PropValueInfo objToCopy)
        {
            if (objToCopy != null)
                Init(objToCopy.PropValue, objToCopy.TwitterHashTag);
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Serialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public override XElement ToXML(bool internalPurpose = true)
        {
            return null; //TODO???
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        ///     override object.ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("PropValue={0};TwitterHashTag={1}", PropValue, TwitterHashTag);
        }
    }
}
