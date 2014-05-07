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
    public class S2CResListBaseEntity<T> : S2CRes<T> where T : BaseEntity
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Snip2Code.Utils.S2CResListBaseEntity"); //otherwise it cannot be addresses in log4net config


        #region Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        private T[] m_data;

        public T[] Data
        {
            get { return m_data; }
            set 
            {
                if (value == null)
                    m_data = null;
                else
                {
                    m_data = new T[value.Length];
                    for (int i = 0; i < value.Length; i++)
                    {
                        m_data[i] = (T)value[i].ToBaseEntity();
                    }
                }
            }
        }


        /// <summary>
        /// Miscellaneous data that can be attached to this object
        /// </summary>
        public Dictionary<string, string> Misc { get; set; } 

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

               
        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public S2CResListBaseEntity() { }

        public S2CResListBaseEntity(double execTime, ErrorCodes res, BaseEntity[] objectToSerialize, int totNum, Dictionary<string, string> misc)
            : base(execTime, res, totNum)
        {
            log.Debug("Starting S2CResListBaseEntity ctor");
            if (objectToSerialize != null)
            {
                m_data = new T[objectToSerialize.Length];
                for (int i = 0; i < objectToSerialize.Length; i++)
                {
                    //if (i % 50 == 0)
                    //    log.Debug("S2CResListBaseEntity tor, item #" + i);
                    m_data[i] = (T)objectToSerialize[i].ToBaseEntity();
                }
            }
            log.Debug("Ending S2CResListBaseEntity ctor");
            Misc = misc;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}