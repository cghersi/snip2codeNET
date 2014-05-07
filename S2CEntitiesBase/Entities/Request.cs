//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Xml.Linq;

using Snip2Code.Utils;

namespace Snip2Code.Model.Entities
{
    public abstract class Request : BaseEntity
    {
        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        public long ID { get; private set;  }

        public int AskerID { get; set; }

        public string AskerEmail { get; set; }

        public string Question { get; set; }

        public DateTime RequestDate { get; private set; }

        public DateTime Expiration { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        public Request() 
        { 
        }

        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="askerID"></param>
        /// <param name="askerEmail"></param>
        /// <param name="question"></param>
        /// <param name="reqDate"></param>
        /// <param name="expiration"></param>
        protected void Init(long id, int askerID, string askerEmail, string question, DateTime reqDate, DateTime expiration)
        {
            ID = id;
            AskerID = askerID;
            AskerEmail = AskerEmail;
            Question = question;
            RequestDate = reqDate;
            Expiration = expiration;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected void Init(Request objToCopy)
        {
            if (objToCopy != null)
                Init(objToCopy.ID, objToCopy.AskerID, objToCopy.AskerEmail, objToCopy.Question,
                    objToCopy.RequestDate, objToCopy.Expiration);
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
            return string.Format("ID={0};AskerID={1};AskerEmail={2};Question={3};Date={4};Expiration={5}",
                ID, AskerID, AskerEmail, Question, RequestDate, Expiration);
        }
    }
}
