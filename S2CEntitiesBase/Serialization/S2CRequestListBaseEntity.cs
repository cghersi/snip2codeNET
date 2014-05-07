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
    public class S2CRequestListBaseEntity<T> : S2CRequest<T> where T : BaseEntity
    {
        #region Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        public T[] Data { get; set; }

        [XmlIgnore]
        public XElement XMLData { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

               
        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public S2CRequestListBaseEntity() { }

        public S2CRequestListBaseEntity(long id, BaseEntity[] objectToSerialize) 
            : base(id)
        {
            if (objectToSerialize != null)
            {
                Data = new T[objectToSerialize.Length];
                for (int i = 0; i < objectToSerialize.Length; i++)
                {
                    Data[i] = (T)objectToSerialize[i];
                }
            }
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
