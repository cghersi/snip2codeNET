//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

using Snip2Code.Model.Abstract;
using Snip2Code.Model.Entities;

namespace Snip2Code.Model.Comm
{
    /// <summary>
    /// This class is used to help in the serialization and deserialization of the base entity.
    /// It is just a container for those properties having a private setter.
    /// </summary>
    public class SnippetComm : Snippet
    {
        #region Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public long CommID { get; set; }

        public int CommCreatorID { get; set; }

        public int CommOwnerGroupID { get; set; }

        public DateTime CommCreated { get; set; }

        public DateTime CommModified { get; set; }

        public ShareOption CommVisibility { get; set; }

        public float CommAvgRating { get; set; }

        public int CommNumVote { get; set; }

        public List<string> CommTags { get; set; }

        //public Dictionary<string, string> CommProperties { get; set; }
        public List<SnippetProperty> CommProperties { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public SnippetComm() : base() { }

        protected override ISnippetsRepository RetrieveRepository() { return null; }

        protected override IGroupRepository RetrieveGroupRepository() { return null; }

        protected override IUserRepository RetrieveUserRepository() { return null; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
