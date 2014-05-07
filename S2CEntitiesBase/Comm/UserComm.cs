//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;

using Snip2Code.Model.Entities;
using Snip2Code.Model.Abstract;

namespace Snip2Code.Model.Comm
{
    /// <summary>
    /// This class is used to help in the serialization and deserialization of the base entity.
    /// It is just a container for those properties having a private setter.
    /// </summary>
    public class UserComm : User
    {
        #region Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public int CommID { get; set; }
        public string CommEMail { get; set; }
        public string CommNickName { get; set; }
        public UserPreferences CommPreferences { get; set; }
        //public long CommPictureID { get; set; }  
        public UserRole CommRole { get; set; }
        public int CommPersonalGroupID { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public UserComm() : base() { }

        protected override IUserRepository RetrieveRepository() { return null; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
