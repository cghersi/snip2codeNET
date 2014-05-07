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
    /// This enum describes all the possible status codes for the response of a web service
    /// </summary>
    public enum ErrorCodes
    {
        OK = 0,
        FAIL = -1,
        NOT_LOGGED_IN = -2,
        TARGET_GROUP_DELETED = -3,
        WRONG_INPUT = -4,
        NOT_ALLOWED = -5,
        INVALID_INPUT_FORMAT = -6, 
        COMMUNICATION_ERROR = -7,
        TIMEOUT = -8,
        UNKNOWN = -100
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
    public abstract class S2CRes<T>
    {
        #region Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        //[XmlIgnore]
        //public SerialFormat Format { get; set; }

        /// <summary>
        /// The result of the request
        /// </summary>
        public ErrorCodes Status { get; set; }

        /// <summary>
        /// The total amount of time needed to evaluate the request [in milliseconds]
        /// </summary>
        public double ExecTime { get; set; }

        /// <summary>
        /// The total number of results that would be sent if no pagination would be selected
        /// </summary>
        public int TotNum { get; set; }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

               
        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public S2CRes() { }

        public S2CRes(double execTime, ErrorCodes res, int totNum) 
        {
            ExecTime = execTime;
            Status = res;
            TotNum = totNum;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        public static string GetErrorMsg(ErrorCodes err)
        {
            switch (err)
            {
                case ErrorCodes.OK:
                    return "Done!";
                case ErrorCodes.FAIL:
                    return "Ooops!! Something was wrong in the last operation. Please, retry!";
                case ErrorCodes.NOT_LOGGED_IN:
                    return "Ooops!! Seems that you are not logged in. Please, login and retry!";
                case ErrorCodes.TARGET_GROUP_DELETED:
                    return "Ooops!! Seems that the target group has been deleted! Please, check with the group administrator and retry!";
                case ErrorCodes.WRONG_INPUT:
                    return "Acch!! Seems that some input value are not correct. Please, check the content and retry!";
                case ErrorCodes.NOT_ALLOWED:
                    return "Mmmmhhh, seems you are not allowed to perform this operation. Check your permissions!";
                case ErrorCodes.INVALID_INPUT_FORMAT:
                    return "Ooops!! There was an error in the communication with the remote server. Please retry!";
                case ErrorCodes.COMMUNICATION_ERROR:
                    return "Ooops!! There was an error in the communication with the remote server. Please retry!";
                case ErrorCodes.TIMEOUT:
                    return "Acch!! The remote server is taking too long to answer. Please retry in a while!";
                case ErrorCodes.UNKNOWN:
                default:
                    return "Ooops!! Something was wrong in the last action. Please, retry!";
            }
        }
    }
}
