using Snip2Code.Utils;
//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Snip2Code.Model.Client.Utils
{
    public class S2cWebClient : WebClient
    {
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout { get; set; }
        public SerialFormat AcceptType { get; set; }

        public S2cWebClient() : this(60000) { }

        public S2cWebClient(int timeout)
        {
            this.Timeout = timeout;
            this.AcceptType = SerialFormat.JSON;
        }

        public S2cWebClient(int timeout, SerialFormat acceptType)
        {
            this.Timeout = timeout;
            this.AcceptType = acceptType;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
                HttpWebRequest r = request as HttpWebRequest;
                if (r != null)
                    r.Accept = AcceptType.GetMIMEType();
            }
            return request;
        }


    }
}
