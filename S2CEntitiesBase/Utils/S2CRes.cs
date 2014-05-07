//------------------------------------------------------------------------------
// (c) 2011 snip2code inc.
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
    /// This enum describes all the possible status codes for the response of a web service
    /// </summary>
    public enum ErrorCodes
    {
        OK = 0,
        FAIL = -1
    }


    /// <summary>
    /// This enum describes the possible type of contents contained in Data property of an S2CRes
    /// </summary>
    public enum S2CContentType
    {
        Obj = 0,
        ListObj = 1,
        S2CEntity = 2,
        ListS2CEntity = 3
    }

    /// <summary>
    /// This class describes the result provided by a generic S2C web service
    /// </summary>
    public class S2CRes : S2CSerializable
    {
        #region Members
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected static fastJSON.JSON jsonUtils = EntityUtils.InitJSONUtils();

        //only one of these members will get a value != null:
        private object m_objectToSerialize = null;
        private ICollection<object> m_objectsToSerialize = null;
        private BaseEntity m_entityToSerialize = null;
        private ICollection<BaseEntity> m_entitiesToSerialize = null;

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        [XmlIgnore]
        public SerialFormat Format { get; private set; }

        public ErrorCodes Status { get; set; }
        public S2CContentType ContentType { get; set; }
        public double ExecTime { get; set; }
        public object Data { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

               
        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public S2CRes() { }

        private S2CRes(SerialFormat format, double execTime, ErrorCodes res)
        {
            Format = format;
            ExecTime = execTime;
            Status = res;
        }

        public S2CRes(SerialFormat format, double execTime, ErrorCodes res, object objectToSerialize) 
            : this(format, execTime, res)
        {
            this.m_objectToSerialize = objectToSerialize;
            Data = objectToSerialize;
            ContentType = S2CContentType.Obj;
        }

        public S2CRes(SerialFormat format, double execTime, ErrorCodes res, ICollection<object> objectsToSerialize)
            : this(format, execTime, res)
        {
            ContentType = S2CContentType.ListObj;

            //guess the content of the enumeration:
            if (objectsToSerialize != null)
            {
                foreach (object obj in objectsToSerialize)
                {
                    if (obj is BaseEntity)
                        ContentType = S2CContentType.ListS2CEntity;

                    break;
                }
            }
            if (ContentType == S2CContentType.ListObj)
                this.m_objectsToSerialize = objectsToSerialize;
            else
            {
                //build the list of BaseEntities:
                List<BaseEntity> tmp = new List<BaseEntity>();
                foreach (object obj in objectsToSerialize)
                {
                    tmp.Add((BaseEntity)obj);
                }
                this.m_entitiesToSerialize = tmp;
            }
            Data = objectsToSerialize;
        }

        public S2CRes(SerialFormat format, double execTime, ErrorCodes res, BaseEntity entityToSerialize)
            : this(format, execTime, res)
        {
            this.m_entityToSerialize = entityToSerialize;
            Data = entityToSerialize;
            ContentType = S2CContentType.S2CEntity;
        }

        public S2CRes(SerialFormat format, double execTime, ErrorCodes res, ICollection<BaseEntity> entitiesToSerialize)
            : this(format, execTime, res)
        {
            this.m_entitiesToSerialize = entitiesToSerialize;
            Data = entitiesToSerialize;
            ContentType = S2CContentType.ListS2CEntity;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Public API
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public string Serialize()
        {
            string resp = string.Empty;
            switch (Format)
            {
                case SerialFormat.JSON:
                    //set content:
                    if (m_entityToSerialize != null)
                        Data = m_entityToSerialize.ToJSONEntity();
                    else if (m_entitiesToSerialize != null)
                    {
                        List<BaseEntity> dataList = new List<BaseEntity>();
                        foreach (BaseEntity be in m_entitiesToSerialize)
                        {
                            dataList.Add(be.ToJSONEntity());
                        }
                        Data = dataList;
                    }

                    return jsonUtils.ToJSON(this);

                case SerialFormat.XML:
                    //set content:
                    XElement data = new XElement(ConfigReader.S2CNS + "Data", new XAttribute("ContType", ContentType));

                    switch(ContentType)
                    {
                        case S2CContentType.S2CEntity:
                            if (m_entityToSerialize != null)
                                data.Add(m_entityToSerialize.ToXML(false));
                            break;

                        case S2CContentType.ListS2CEntity:
                            if (m_entitiesToSerialize != null)
                            {
                                foreach (BaseEntity be in m_entitiesToSerialize)
                                {
                                    data.Add(be.ToXML(false));
                                }
                            }
                            break;

                        case S2CContentType.Obj:
                            if (m_objectToSerialize != null)
                                data.SetValue(m_objectToSerialize);
                            break;

                        case S2CContentType.ListObj:
                            if (m_objectsToSerialize != null)
                            {
                                foreach (object toSer in m_objectsToSerialize)
                                {
                                    data.Add(new XElement(ConfigReader.S2CNS + "Obj", toSer));
                                }
                            }
                            break;
                    }

                    XDocument result = Utilities.CreateXMLDoc("S2CResponse", false);

                    result.Root.Add(
                        new XElement(ConfigReader.S2CNS + "Status", Status),
                        new XElement(ConfigReader.S2CNS + "ExecTime", ExecTime),
                        data
                    );

                    return result.ToString();

                case SerialFormat.HTML:
                    return string.Empty;
                    //TODO???

            }

            return string.Empty;
        }


        public static S2CRes Deserialize(string content, SerialFormat format)
        {
            if (string.IsNullOrEmpty(content))
            {
                log.Error("Content is empty");
                return new S2CRes(format, -1.0, ErrorCodes.FAIL, content);
            }

            switch (format)
            {
                case SerialFormat.JSON:
                    return jsonUtils.ToObject<S2CRes>(content);

                case SerialFormat.XML:
                    //parse content:
                    XElement data = null;
                    try
                    {
                        data = XElement.Parse(content);
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat("Cannot parse XML:{0} due to {1}", content, e.Message);
                        return null;
                    }

                    if (data == null)
                    {
                        log.ErrorFormat("Cannot parse XML:{0}", content);
                        return null;
                    }

                    //parse single elements:
                    double execTime = -1;
                    data.ParseNode("ExecTime", ref execTime, false);

                    ErrorCodes status = ErrorCodes.FAIL;
                    data.ParseNode<ErrorCodes>("Status", ref status, false);

                    S2CContentType contType = S2CContentType.Obj;
                    data.ParseNode<S2CContentType>("ContType", ref contType, false);

                    //parse actual data depending on content type:
                    XElement dataContent = data.GetNode("Data", false);
                    switch (contType)
                    {
                        case S2CContentType.S2CEntity:
                            S2CRes singleEntity =  new S2CRes(SerialFormat.XML, execTime, status, dataContent.ToString());
                            singleEntity.ContentType = S2CContentType.S2CEntity;
                            return singleEntity;

                        case S2CContentType.ListS2CEntity:
                            S2CRes multEntity = new S2CRes(SerialFormat.XML, execTime, status, dataContent.ToString());
                            multEntity.ContentType = S2CContentType.S2CEntity;
                            return multEntity;

                        case S2CContentType.Obj:
                            return new S2CRes(SerialFormat.XML, execTime, status, dataContent.Value);

                        case S2CContentType.ListObj:
                            List<object> objs = new List<object>();
                            List<XElement> nodes = dataContent.GetNodes("Obj", false);
                            if (nodes != null)
                            {
                                foreach (XElement node in nodes)
                                {
                                    objs.Add(node.Value);
                                }
                            }
                            return new S2CRes(SerialFormat.XML, execTime, status, objs);
                    }

                    log.Error("Unknown S2CContentType");
                    return new S2CRes(format, -1.0, ErrorCodes.FAIL, "Unknown S2CContentType");

                case SerialFormat.HTML:
                    return new S2CRes(format, -1.0, ErrorCodes.FAIL, "Unsupported SerialFormat");
                    //TODO???
            }

            log.Error("Unknown SerialFormat");
            return new S2CRes(format, -1.0, ErrorCodes.FAIL, "Unknown SerialFormat");
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
