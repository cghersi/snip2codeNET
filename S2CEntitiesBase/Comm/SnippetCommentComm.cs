//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;

using Snip2Code.Model.Entities;

namespace Snip2Code.Model.Comm
{
    /// <summary>
    /// This class is used to help in the serialization and deserialization of the base entity.
    /// It is just a container for those properties having a private setter.
    /// </summary>
    public class SnippetCommentComm : SnippetComment
    {
        #region Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public long CommID { get; set; }

        public DateTime CommSnippetCommentDate { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public SnippetCommentComm() : base() { }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
