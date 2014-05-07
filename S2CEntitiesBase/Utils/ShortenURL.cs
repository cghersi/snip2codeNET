//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;


namespace Snip2Code.Utils
{
    public class ShortenURL
    {
        public enum ShortURLProvider
        {
            Bitly = 0,
            Isgd = 1,
            Tinyurl = 2,
            Trim = 3,
        }


        /// <summary>
        /// try to use different services to shorten the given url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="eService"></param>
        /// <returns></returns>
        public static string DoShortenURL(string url, ShortURLProvider eService)
        {
            // return empty strings if not valid  
            if (!IsValidURL(url))
                return string.Empty;

            string requestUrl = string.Format(GetRequestTemplate(eService), url);
            WebRequest request = HttpWebRequest.Create(requestUrl);
            request.Proxy = null;
            string strResult = null;
            try
            {
                using (Stream responseStream = request.GetResponse().GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.ASCII);
                    strResult = reader.ReadToEnd();
                    if (!IsValidURL(strResult))
                        return string.Empty;
                }
            }
            catch (Exception)
            {
                strResult = url;
            }

            // if converted is longer than original, return original  
            if (strResult.Length > url.Length)
                strResult = url;

            //if the previous service didn't work, try Tr.im:
            if (strResult.CompareTo(url) == 0)
                strResult = GetShortURLbyXML(url);

            return strResult;
        }


        /// <summary>
        /// validation of the url
        /// </summary>
        /// <param name="strurl"></param>
        /// <returns></returns>
        public static bool IsValidURL(string strurl)
        {
            return (strurl.ToLower().StartsWith("http://") || strurl.StartsWith("https://"));
        }


        /// <summary>
        /// Request template for URL
        /// </summary>
        /// <param name="eService"></param>
        /// <returns></returns>
        private static string GetRequestTemplate(ShortURLProvider eService)
        {
            string strRequest = string.Empty;
            switch (eService)
            {
                case ShortURLProvider.Isgd:
                    strRequest = "http://is.gd/api.php?longurl={0}";
                    break;
                case ShortURLProvider.Bitly:
                    strRequest = "http://bit.ly/api?url={0}"; ;
                    break;
                case ShortURLProvider.Tinyurl:
                    strRequest = "http://tinyurl.com/api-create.php?url={0}";
                    break;
                case ShortURLProvider.Trim:
                    strRequest = "http://tr.im/api/trim_url.xml?url={0}";
                    break;
                default:
                    break;
            }

            return strRequest;
        }


        /// <summary>
        /// shorten the url using Tr.im service
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected static string GetShortURLbyXML(string url)
        {
            //prepare the request to the remote service
            //Note: Tr.im wants the url NOT HttpEncoded!!!
            string trimUrl = string.Format(GetRequestTemplate(ShortURLProvider.Trim), url);
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            string resultUrl = string.Empty;
            XmlDocument result = new XmlDocument();

            try
            {
                //send a request to tr.im
                Stream data = client.OpenRead(trimUrl);

                //load the response in an XML document
                result.Load(data);

                data.Close();
            }
            catch
            {
                return resultUrl;
            }

            //parse the XML response
            XmlNode statusNode = result.SelectSingleNode("/trim/status");
            string status = string.Empty;
            if (statusNode != null)
                status = statusNode.Attributes["result"].InnerText;

            //retrieve the tinyurl
            if (status.CompareTo("OK") == 0)
            {
                XmlNode urlNode = result.SelectSingleNode("/trim/url");
                if (urlNode != null)
                    resultUrl = urlNode.InnerText;
            }

            return resultUrl;
        }
    }
}