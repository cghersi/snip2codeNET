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
    public class GroupUserInfoComm : GroupUserInfo
    {
        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public GroupUserInfoComm() : base() { }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override IGroupRepository RetrieveGroupRepository() { return null; }
        protected override IUserRepository RetrieveUserRepository() { return null; }
    }
}
