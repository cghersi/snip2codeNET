//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

using Snip2Code.Model.Abstract;
using Snip2Code.Model.Comm;
using Snip2Code.Utils;

namespace Snip2Code.Model.Entities
{
    /// <summary>
    /// This class models the very basic proeprties of a single Snip2Code user
    /// </summary>
    public class UserBaseInfo
    {
        #region Primitive Properties

        public int ID { get; set; }

        public string EMail { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return Name + ' ' + LastName; } }

        public string NickName { get; private set; }

        public long PictureID { get; set; }     //0 means no picture

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////

        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        public UserBaseInfo() { }

        /// <summary>
        /// This is the complete constructor for this class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <param name="lastname"></param>
        /// <param name="nickname"></param>
        /// <param name="pictureID"></param>
        public UserBaseInfo(int id, string email, string name, string lastname, string nickname, long pictureID)
        {
            ID = id;
            EMail = email;
            Name = name;
            LastName = lastname;
            NickName = nickname;
            PictureID = pictureID;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
