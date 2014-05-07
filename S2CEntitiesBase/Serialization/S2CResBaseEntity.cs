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
    public class S2CResBaseEntity<T> : S2CRes<T> where T : BaseEntity
    {
        #region Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        private T m_data;

        public T Data 
        { 
            get { return m_data; }
            set { m_data = (value == null ? null : (T)value.ToBaseEntity()); }
        }

        [XmlIgnore]
        public XElement XMLData { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

               
        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public S2CResBaseEntity() { }

        public S2CResBaseEntity(double execTime, ErrorCodes res, BaseEntity objectToSerialize) 
            : base(execTime, res, 1)
        {
            if (objectToSerialize != null)
                m_data = (T)objectToSerialize.ToBaseEntity();
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
