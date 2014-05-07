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
    /// This class provides methods an utilities to serialize/deserialize the payload exhanged between client and server
    /// in S2C communication through web services
    /// </summary>
    public class S2CSerializer
    {
        #region Members
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected static fastJSON.JSON jsonUtils = EntityUtils.InitJSONUtils();

        private bool m_isClientSide = false;
        private long m_id = -1;
        private double m_execTime = 0.0F;
        private ErrorCodes m_res = ErrorCodes.FAIL;
        private SerialFormat m_format = SerialFormat.JSON;
        private int m_totNum = 0;

        //only one of these members will get a value != null:
        //private object m_objectToSerialize = null;
        //private object[] m_objectsToSerialize = null;
        //private BaseEntity m_entityToSerialize = null;
        //private BaseEntity[] m_entitiesToSerialize = null;

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

               
        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public S2CSerializer(SerialFormat format, long id)
        {
            m_isClientSide = true;
            m_format = format;
            m_id = id;
        }

        public S2CSerializer(SerialFormat format, double execTime, ErrorCodes res, int totNum) 
        {
            m_isClientSide = false;
            m_format = format;
            m_execTime = execTime;
            m_res = res;
            m_totNum = totNum;
        }


        //public S2CSerializer(SerialFormat format, double execTime, ErrorCodes res, BaseEntity entityToSerialize)
        //    : this(format, execTime, res)
        //{
        //    this.m_entityToSerialize = entityToSerialize;
        //}

        //public S2CSerializer(SerialFormat format, double execTime, ErrorCodes res, object[] objectsToSerialize)
        //    : this(format, execTime, res)
        //{
        //    this.m_objectsToSerialize = objectsToSerialize;
        //}

        //public S2CSerializer(SerialFormat format, double execTime, ErrorCodes res, BaseEntity[] entitiesToSerialize)
        //    : this(format, execTime, res)
        //{
        //    this.m_entitiesToSerialize = entitiesToSerialize;
        //}

        //public S2CSerializer(SerialFormat format, double execTime, ErrorCodes res, object objectToSerialize)
        //    : this(format, execTime, res)
        //{
        //    this.m_objectToSerialize = objectToSerialize;
        //}

        //public S2CSerializer(SerialFormat format, long id, object objectToSerialize) 
        //    : this(format, id)
        //{
        //    this.m_objectToSerialize = objectToSerialize;
        //}

        //public S2CSerializer(SerialFormat format, long id, object[] objectsToSerialize)
        //    : this(format, id)
        //{
        //    this.m_objectsToSerialize = objectsToSerialize;
        //}

        //public S2CSerializer(SerialFormat format, long id, BaseEntity entityToSerialize)
        //    : this(format, id)
        //{
        //    this.m_entityToSerialize = entityToSerialize;
        //}

        //public S2CSerializer(SerialFormat format, long id, BaseEntity[] entitiesToSerialize)
        //    : this(format, id)
        //{
        //    this.m_entitiesToSerialize = entitiesToSerialize;
        //}

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Serialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public string SerializeBaseEntity<T>(T entityToSerialize) where T : BaseEntity
        {
            string resp = string.Empty;
            switch (m_format)
            {
                case SerialFormat.JSON:
                    //set content:
                    if (entityToSerialize == null)
                        return string.Empty;

                    BaseEntity m_entityToSerialize = entityToSerialize.ToJSONEntity();
                    if (m_isClientSide)
                        return jsonUtils.ToJSON(new S2CRequestBaseEntity<T>(m_id, m_entityToSerialize));
                    else
                        return jsonUtils.ToJSON(new S2CResBaseEntity<T>(m_execTime, m_res, m_entityToSerialize));

                case SerialFormat.XML:
                    //set content:
                    XElement data = new XElement(ConfigReader.S2CNS + "Data", new XAttribute("ContType", S2CContentType.S2CEntity));
                    if (entityToSerialize != null)
                        data.Add(entityToSerialize.ToXML(false));
                    XDocument result = PrepareXML(data);
                    return result.ToString();
            }

            return string.Empty;
        }

        public string SerializeBaseEntityList<T>(T[] entitiesToSerialize, Dictionary<string, string> misc) where T : BaseEntity
        {
            string resp = string.Empty;
            switch (m_format)
            {
                case SerialFormat.JSON:
                    //set content:
                    if (entitiesToSerialize == null)
                        return string.Empty;

                    log.Debug("Starting serializing entitiesToSerialize");
                    List<BaseEntity> dataList = new List<BaseEntity>();
                    foreach (BaseEntity be in entitiesToSerialize)
                    {
                        if (be == null)
                            continue;
                        dataList.Add(be.ToJSONEntity());
                    }
                    log.Debug("Starting serializing m_entitiesToSerialize");
                    BaseEntity[] m_entitiesToSerialize = dataList.ToArray();
                    if (m_isClientSide)
                        return jsonUtils.ToJSON(new S2CRequestListBaseEntity<T>(m_id, m_entitiesToSerialize));
                    else
                        return jsonUtils.ToJSON(new S2CResListBaseEntity<T>(m_execTime, m_res, m_entitiesToSerialize, m_totNum, misc));

                case SerialFormat.XML:
                    //set content:
                    XElement data = new XElement(ConfigReader.S2CNS + "Data", new XAttribute("ContType", S2CContentType.ListS2CEntity));
                    if (entitiesToSerialize != null)
                    {
                        foreach (BaseEntity be in entitiesToSerialize)
                        {
                            if (be == null)
                                continue;
                            data.Add(be.ToXML(false));
                        }
                    }
                    XElement miscElem = null;
                    if (misc != null)
                    {
                        miscElem = new XElement(ConfigReader.S2CNS + "Misc");
                        foreach (string key in misc.Keys)
                        {
                            if (string.IsNullOrEmpty(key))
                                continue;
                            miscElem.Add(new XElement(ConfigReader.S2CNS + "MiscVal", new XAttribute("key", key.FixXML()), misc[key].FixXML()));
                        }
                    }
                    XDocument result = PrepareXML(data, miscElem);
                    return result.ToString();
            }

            return string.Empty;
        }

        public string SerializeObj<T>(object objectToSerialize) 
        {
            string resp = string.Empty;
            switch (m_format)
            {
                case SerialFormat.JSON:
                    //set content:
                    if (objectToSerialize == null)
                        return string.Empty;

                    if (m_isClientSide)
                        return jsonUtils.ToJSON(new S2CRequestObj<T>(m_id, objectToSerialize));
                    else
                        return jsonUtils.ToJSON(new S2CResObj<T>(m_execTime, m_res, objectToSerialize));

                case SerialFormat.XML:
                    //set content:
                    XElement data = new XElement(ConfigReader.S2CNS + "Data", new XAttribute("ContType", S2CContentType.Obj));
                    if (objectToSerialize != null)
                        data.SetValue(objectToSerialize);
                    XDocument result = PrepareXML(data);
                    return result.ToString();
            }

            return string.Empty;
        }

        public string SerializeListObj<T>(T[] objectsToSerialize)
        {
            string resp = string.Empty;
            switch (m_format)
            {
                case SerialFormat.JSON:
                    //set content:
                    if (objectsToSerialize == null)
                        return string.Empty;

                    if (m_isClientSide)
                        return jsonUtils.ToJSON(new S2CRequestListObj<T>(m_id, objectsToSerialize));
                    else
                        return jsonUtils.ToJSON(new S2CResListObj<T>(m_execTime, m_res, objectsToSerialize, m_totNum));

                case SerialFormat.XML:
                    //set content:
                    XElement data = new XElement(ConfigReader.S2CNS + "Data", new XAttribute("ContType", S2CContentType.ListObj));
                    if (objectsToSerialize != null)
                    {
                        foreach (object toSer in objectsToSerialize)
                        {
                            if (toSer == null)
                                continue;
                            data.Add(new XElement(ConfigReader.S2CNS + "Obj", toSer));
                        }
                    }
                    XDocument result = PrepareXML(data);
                    return result.ToString();
            }

            return string.Empty;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region DeSerialization S2CRes
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public static S2CResBaseEntity<T> DeserializeBaseEntity<T>(string content, SerialFormat format) where T : BaseEntity, new()
        {
            if (string.IsNullOrEmpty(content))
            {
                log.Error("Content is empty");
                return null;
            }

            switch (format)
            {
                case SerialFormat.JSON:
                    try
                    {
                        return jsonUtils.ToObject<S2CResBaseEntity<T>>(content);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Error while parsing content {0} due to {1}", content.PrintNull(), ex.Message);
                        return new S2CResBaseEntity<T>(0.0F, ErrorCodes.FAIL, null);
                    }

                case SerialFormat.XML:
                    //parse content:
                    XElement data = ParseContent(content);
                    if (data == null)
                        return null;

                    //parse single elements:
                    float execTime = -1;
                    data.ParseNode("ExecTime", ref execTime, false);
                    ErrorCodes status = ErrorCodes.FAIL;
                    data.ParseNode<ErrorCodes>("Status", ref status, false);

                    //parse actual data:
                    XElement dataContent = data.GetNode("Data", false);
                    T tmp = new T();
                    tmp.Parse((XElement)dataContent.FirstNode);
                    return new S2CResBaseEntity<T>(execTime, status, tmp);
            }

            log.Error("Unknown SerialFormat");
            return null;
        }

        public static S2CResListBaseEntity<T> DeserializeBaseEntityList<T>(string content, SerialFormat format) where T : BaseEntity, new()
        {
            if (string.IsNullOrEmpty(content))
            {
                log.Error("Content is empty");
                return null;
            }

            switch (format)
            {
                case SerialFormat.JSON:
                    try
                    {
                        return jsonUtils.ToObject<S2CResListBaseEntity<T>>(content);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Error while parsing content {0} due to {1}", content.PrintNull(), ex.Message);
                        return new S2CResListBaseEntity<T>(0.0F, ErrorCodes.WRONG_INPUT, null, 0, null);
                    }

                case SerialFormat.XML:
                    //parse content:
                    XElement data = ParseContent(content);
                    if (data == null)
                        return null;

                    //parse single elements:
                    float execTime = -1;
                    data.ParseNode("ExecTime", ref execTime, false);
                    ErrorCodes status = ErrorCodes.FAIL;
                    data.ParseNode<ErrorCodes>("Status", ref status, false);
                    int totNum = 0;
                    data.ParseNode("TotNum", ref totNum, false);

                    //parse actual data:
                    XElement dataContent = data.GetNode("Data", false);
                    IEnumerable<XElement> nodes = dataContent.Elements();
                    List<BaseEntity> entities = new List<BaseEntity>();
                    if (nodes != null)
                    {
                        foreach (XElement node in nodes)
                        {
                            T tmp = new T();
                            tmp.Parse(node);
                            entities.Add(tmp);
                        }
                    }

                    //parse eventual miscellaneous arguments:
                    XElement miscRoot = data.GetNode("Misc", false);
                    Dictionary<string, string> misc = null;
                    if (miscRoot != null)
                    {
                        List<XElement> miscContent = miscRoot.GetNodes("MiscVal", false);
                        if (!miscContent.IsNullOrEmpty())
                        {
                            misc = new Dictionary<string, string>();
                            foreach (XElement miscElem in miscContent)
                            {
                                if (miscElem == null)
                                    continue;
                                XAttribute keyAttr = miscElem.Attribute("key");
                                if ((keyAttr == null) || string.IsNullOrEmpty(keyAttr.Value))
                                    continue;

                                //List<string> values = new List<string>();
                                //foreach (XElement valElem in miscElem.Elements())
                                //{
                                //    string val = string.Empty;
                                //    valElem.ParseNode("MiscValue", ref val, false);
                                //    if (!string.IsNullOrEmpty(val))
                                //        values.Add(val);
                                //}
                                misc.Add(keyAttr.Value, miscElem.Value); // Utilities.MergeIntoCommaSeparatedString(values));
                            }
                        }
                    }
                    return new S2CResListBaseEntity<T>(execTime, status, entities.ToArray(), totNum, misc);
            }

            log.Error("Unknown SerialFormat");
            return null;
        }

        public static S2CResObj<object> DeserializeObj(string content, SerialFormat format)
        {
            if (string.IsNullOrEmpty(content))
            {
                log.Error("Content is empty");
                return null;
            }

            switch (format)
            {
                case SerialFormat.JSON:
                    try 
                    {
                        return jsonUtils.ToObject<S2CResObj<object>>(content);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Error while parsing content {0} due to {1}", content.PrintNull(), ex.Message);
                        return new S2CResObj<object>(0.0F, ErrorCodes.FAIL, null);
                    }

                case SerialFormat.XML:
                    //parse content:
                    XElement data = ParseContent(content);
                    if (data == null)
                        return null;

                    //parse single elements:
                    float execTime = -1;
                    data.ParseNode("ExecTime", ref execTime, false);
                    ErrorCodes status = ErrorCodes.FAIL;
                    data.ParseNode<ErrorCodes>("Status", ref status, false);

                    //parse actual data:
                    XElement dataContent = data.GetNode("Data", false);
                    return new S2CResObj<object>(execTime, status, dataContent.Value);
            }

            log.Error("Unknown SerialFormat");
            return null;
        }

        public static S2CResListObj<string> DeserializeObjList(string content, SerialFormat format)
        {
            if (string.IsNullOrEmpty(content))
            {
                log.Error("Content is empty");
                return null;
            }

            switch (format)
            {
                case SerialFormat.JSON:
                    try
                    {
                        return jsonUtils.ToObject<S2CResListObj<string>>(content);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Error while parsing content {0} due to {1}", content.PrintNull(), ex.Message);
                        return new S2CResListObj<string>(0.0F, ErrorCodes.FAIL, new string[] { "Generic Error" }, 0); 
                    }

                case SerialFormat.XML:
                    //parse content:
                    XElement data = ParseContent(content);
                    if (data == null)
                        return null;

                    //parse single elements:
                    float execTime = -1;
                    data.ParseNode("ExecTime", ref execTime, false);
                    ErrorCodes status = ErrorCodes.FAIL;
                    data.ParseNode<ErrorCodes>("Status", ref status, false);
                    int totNum = 0;
                    data.ParseNode("TotNum", ref totNum, false);

                    //parse actual data depending on content type:
                    XElement dataContent = data.GetNode("Data", false);
                    List<string> objs = new List<string>();
                    List<XElement> nodes = dataContent.GetNodes("Obj", false);
                    if (nodes != null)
                    {
                        foreach (XElement node in nodes)
                        {
                            objs.Add(node.Value);
                        }
                    }
                    return new S2CResListObj<string>(execTime, status, objs.ToArray(), totNum);
            }

            log.Error("Unknown SerialFormat");
            return null;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region DeSerialization S2CRequest
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public static S2CRequestBaseEntity<T> DeserializeReqBaseEntity<T>(string content, SerialFormat format) 
            where T : BaseEntity, new()
        {
            if (string.IsNullOrEmpty(content))
            {
                log.Error("Content is empty");
                return null;
            }

            switch (format)
            {
                case SerialFormat.JSON:
                    try
                    {
                        return jsonUtils.ToObject<S2CRequestBaseEntity<T>>(content);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Error while parsing content {0} due to {1}", content.PrintNull(), ex.Message);
                        return new S2CRequestBaseEntity<T>(-1, null);
                    }

                case SerialFormat.XML:
                    //parse content:
                    XElement data = ParseContent(content);
                    if (data == null)
                        return null;

                    //parse single elements:
                    long id = -1;
                    data.ParseNode("ID", ref id, false);

                    //parse actual data:
                    XElement dataContent = data.GetNode("Data", false);
                    T tmp = new T();
                    tmp.Parse((XElement)dataContent.FirstNode);
                    return new S2CRequestBaseEntity<T>(id, tmp);
            }

            log.Error("Unknown SerialFormat");
            return null;
        }

        public static S2CRequestListBaseEntity<T> DeserializeReqBaseEntityList<T>(string content, SerialFormat format) 
            where T : BaseEntity, new()
        {
            if (string.IsNullOrEmpty(content))
            {
                log.Error("Content is empty");
                return null;
            }

            switch (format)
            {
                case SerialFormat.JSON:
                    try
                    {
                        return jsonUtils.ToObject<S2CRequestListBaseEntity<T>>(content);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Error while parsing content {0} due to {1}", content.PrintNull(), ex.Message);
                        return new S2CRequestListBaseEntity<T>(-1, null);
                    }

                case SerialFormat.XML:
                    //parse content:
                    XElement data = ParseContent(content);
                    if (data == null)
                        return null;

                    //parse single elements:
                    long id = -1;
                    data.ParseNode("ID", ref id, false);

                    //parse actual data:
                    XElement dataContent = data.GetNode("Data", false);
                    IEnumerable<XElement> nodes = dataContent.Elements();
                    List<BaseEntity> entities = new List<BaseEntity>();
                    if (nodes != null)
                    {
                        foreach (XElement node in nodes)
                        {
                            T tmp = new T();
                            tmp.Parse(node);
                            entities.Add(tmp);
                        }
                    }
                    return new S2CRequestListBaseEntity<T>(id, entities.ToArray());
            }

            log.Error("Unknown SerialFormat");
            return null;
        }

        public static S2CRequestObj<string> DeserializeReqObj(string content, SerialFormat format)
        {
            if (string.IsNullOrEmpty(content))
            {
                log.Error("Content is empty");
                return null;
            }

            switch (format)
            {
                case SerialFormat.JSON:
                    try 
                    {
                        return jsonUtils.ToObject<S2CRequestObj<string>>(content);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Error while parsing content {0} due to {1}", content.PrintNull(), ex.Message);
                        return new S2CRequestObj<string>(-1, null);
                    }
                case SerialFormat.XML:
                    //parse content:
                    XElement data = ParseContent(content);
                    if (data == null)
                        return null;

                    //parse single elements:
                    long id = -1;
                    data.ParseNode("ID", ref id, false);

                    //parse actual data:
                    XElement dataContent = data.GetNode("Data", false);
                    return new S2CRequestObj<string>(id, dataContent.Value);
            }

            log.Error("Unknown SerialFormat");
            return null;
        }

        public static S2CRequestListObj<string> DeserializeReqObjList(string content, SerialFormat format)
        {
            if (string.IsNullOrEmpty(content))
            {
                log.Error("Content is empty");
                return null;
            }

            switch (format)
            {
                case SerialFormat.JSON:
                    try
                    {
                        return jsonUtils.ToObject<S2CRequestListObj<string>>(content);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Error while parsing content {0} due to {1}", content.PrintNull(), ex.Message);
                        return new S2CRequestListObj<string>(-1, null);
                    }
                case SerialFormat.XML:
                    //parse content:
                    XElement data = ParseContent(content);
                    if (data == null)
                        return null;

                    //parse single elements:
                    long id = -1;
                    data.ParseNode("ID", ref id, false);

                    //parse actual data depending on content type:
                    XElement dataContent = data.GetNode("Data", false);
                    List<string> objs = new List<string>();
                    List<XElement> nodes = dataContent.GetNodes("Obj", false);
                    if (nodes != null)
                    {
                        foreach (XElement node in nodes)
                        {
                            objs.Add(node.Value);
                        }
                    }
                    return new S2CRequestListObj<string>(id, objs.ToArray());
            }

            log.Error("Unknown SerialFormat");
            return null;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Private Utilities
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected XDocument PrepareXML(XElement data, XElement misc = null)
        {
            XDocument result = null;
            if (m_isClientSide)
            {
                result = Utilities.CreateXMLDoc("S2CRequest", false);
                result.Root.Add(
                    new XElement(ConfigReader.S2CNS + "ID", m_id),
                    data
                );
            }
            else
            {
                result = Utilities.CreateXMLDoc("S2CResponse", false);
                result.Root.Add(
                    new XElement(ConfigReader.S2CNS + "Status", m_res),
                    new XElement(ConfigReader.S2CNS + "ExecTime", m_execTime),
                    new XElement(ConfigReader.S2CNS + "TotNum", m_totNum),
                    data
                );
                if (misc != null)
                    result.Root.Add(misc);
            }

            return result;
        }

        protected static XElement ParseContent(string content)
        {
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

            return data;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
