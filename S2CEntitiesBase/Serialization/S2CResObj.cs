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
    public class S2CResObj<T> : S2CRes<T> 
    {
        #region Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        public T Data { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

               
        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public S2CResObj() { }

        public S2CResObj(double execTime, ErrorCodes res, object objectToSerialize) 
            : base(execTime, res, 1)
        {
            if (objectToSerialize != null)
                Data = (T)objectToSerialize;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
