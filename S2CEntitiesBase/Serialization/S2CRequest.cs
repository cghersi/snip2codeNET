//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

using Snip2Code.Model.Entities;

namespace Snip2Code.Utils
{
    /// <summary>
    /// This class describes the content of a request provided by a client to a generic S2C web service.
    /// It consist in an id (userID, snippetID, etc.) and a payload that can be a single entity or a list of objects/BaseEntities
    /// </summary>
    public abstract class S2CRequest<T>
    {
        #region Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        //[XmlIgnore]
        //public SerialFormat Format { get; set; }

        public long ID { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

               
        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public S2CRequest() { }

        public S2CRequest(long id) 
        {
            ID = id;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
