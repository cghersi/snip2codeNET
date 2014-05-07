//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Snip2Code.Model.Abstract;
using Snip2Code.Model.Client.WSFramework;
using Snip2Code.Model.Entities;
using Snip2Code.Model.Comm;
using Snip2Code.Utils;

namespace Snip2Code.Model.Client.Entities
{
    public class SnippetClient : SnippetComm
    {
        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Default constructor for JSON parsing
        /// </summary>
        public SnippetClient() : base() { }

        ///// <summary>
        ///// Builds a new object from a string formatted in one 
        ///// of the SerialFormat enum values
        ///// </summary>
        ///// <param name="content"></param>
        ///// <param name="format"></param>
        //public SnippetClient(string content, SerialFormat format)
        //    : base(content, format) { }

        ///// <summary>
        ///// Builds a new object from a result obtained from Web service
        ///// </summary>
        ///// <param name="wsResult"></param>
        //public SnippetClient(S2CResBaseEntity<SnippetComm> wsResult)
        //    : base() 
        //{
        //    if ((wsResult == null) || (wsResult.Status != ErrorCodes.OK) || (wsResult.Data == null))
        //    {
        //        IsValid = false;
        //        return;
        //    }
        //    switch (wsResult.Format)
        //    {
        //        case SerialFormat.JSON:
        //            Init(wsResult.Data);
        //            break;

        //        case SerialFormat.XML:
        //            if (wsResult.XMLData == null)
        //            {
        //                IsValid = false;
        //                return;
        //            }

        //            IsValid = Parse(wsResult.XMLData);
        //            break;
        //    }
        //}

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Repository
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override ISnippetsRepository RetrieveRepository()
        {
            return new SnippetsWS();
        }

        protected override IGroupRepository RetrieveGroupRepository()
        {
            return null; 
        }

        protected override IUserRepository RetrieveUserRepository()
        {
            return new UserWS();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
