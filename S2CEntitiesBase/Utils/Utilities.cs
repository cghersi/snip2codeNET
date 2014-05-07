//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Text;
using System.Linq;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using System.Xml.XPath;
using Snip2Code.Model.Entities;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Management;

namespace Snip2Code.Utils
{
    public enum S2CClientType
    {
        Web,
        MVS,
        Ecl,
        Int,
        Bkm,
        Mac,
        Wi8,
        Npp,
        Api
    }

    /// <summary>
    /// This class makes available a number of general utility methods for XML management, HTTP request/response, 
    /// string management, etc.
    /// </summary>
    public static class Utilities
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        //static public char[] s_separators = new char[] { '@', '.', ',', '!', ' ', '/', '\\', '-', '_', '"', ':', ';', '[', ']', 
        //                                                '{', '}', '(', ')', '<', '>', '=', '!', '?', '\'', '\n', '\t', '\r' };
        static public char[] s_separators = new char[] { '|', '/', '\\', ';', '[', ']', '{', '}', '\'', '\n', '\t', '\r',   //all s_badCharsForSearch 
                                                        '@', '!', ' ', '"', '(', ')', '<', '>', '=' };
        static public char[] s_badChars = new char[] { ',', '!', '|', '*', '/', '\\', ';', '[', ']', '{', '}', '(', ')', '<', 
                                                        '>', '=', '\'', '"', '\n', '\t', '\r' };
        static public char[] s_badCharsForSearch = new char[] { '|', '/', '\\', ';', '[', ']', '{', '}', '\'', '\n', '\t', '\r' };
        
        //static public string[] s_skipKeywords = new string[] { "\"", "more", "less", "here", "there", "on", "off", "the", "this", 
                                                            //"those", "are", "for", "but", "would", "what", "then", "else", "not", 
                                                            //"some", "can", "that", "also", "della", "with", "you", "your", "mine", 
                                                            //"theirs", "being", "gone", "all", "their", "from", "every", "find", 
                                                            //"bring", "take", "back", "it's", "about", "and", "or" };
        static public string[] s_skipKeywords = new string[] { "\"", "the", "those", "are", "not", "it's", "and", "or" }; 
        static public string[] s_skipImages = new string[] { "pheedo", "doubleclick.net", "api.tweetmeme", "feeds.wordpress", 
                                                                "googleadservices", "feedburner", "feedsportal", "addthis", 
                                                                "googlesyndication", "gbuzz-feed.png", "i=", "a=", "u=" };

        static public List<string> SkipKeywords { get { return new List<string>(s_skipKeywords); } }

        static public bool SEARCH_MICROFORMATS = true;
        static public int NUM_META_TAGS = 10;
        static public int NUM_MICROF_TAGS = 100;
        //static public string s_urlRegEx = @"^((([hH][tT][tT][pP][sS]?|[fF][tT][pP])\:\/\/)?([\w\.\-]+(\:[\w\.\&%\$\-]+)*@)?((([^\s\(\)\<\>\\\""\.\[\]\,@;:]+)(\.[^\s\(\)\<\>\\\""\.\[\]\,@;:]+)*(\.[a-zA-Z]{2,4}))|((([01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}([01]?\d{1,2}|2[0-4]\d|25[0-5])))(\b\:(6553[0-5]|655[0-2]\d|65[0-4]\d{2}|6[0-4]\d{3}|[1-5]\d{4}|[1-9]\d{0,3}|0)\b)?((\/[^\/][\w\.\,\?\'\\\/\+&%\$#\=~_\-@]*)*[^\.\,\?\""\'\(\)\[\]!;<>{}\s\x7F-\xFF])?)$";
        //static public string s_urlRegEx = @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";
        static public string s_urlRegEx = @"(file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://[\S]*";
        //static public string s_urlRegEx = @"((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp):?/{0,2}[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?(#[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?";
        //static public string s_urlRegEx = @"(([a-zA-Z][0-9a-zA-Z+\\-\\.]*:)?/{0,2}[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?(#[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?";

        static private string s_nameNoSpaceRegEx = @"^[_a-zA-Z][_a-zA-Z0-9-.]{ç,§}$"; //'ç' is a placeholder for actual minimum length, '§' is a placeholder for actual maximum length
        static private string s_nameRegEx = @"^[_a-zA-Z][_a-zA-Z0-9-. ]{ç,§}$"; //'ç' is a placeholder for actual minimum length, '§' is a placeholder for actual maximum length
        //static private string s_propValueRegEx = @"^[_a-zA-Z0-9-. ]{ç,§}$"; //'ç' is a placeholder for actual minimum length, '§' is a placeholder for actual maximum length
        
        public const string s_emailRegEx = @"^[_a-zA-Z0-9-]+(\.[_a-zA-Z0-9-]+)*@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.(([0-9]{1,3})|([a-zA-Z]{2,3})|(aero|coop|info|museum|name))[\n]?$";

        public const string s_nickNameRegEx = @"^[_a-zA-Z][_a-zA-Z0-9-. ]{" + User.MIN_NICKNAME_LEN_STR + "," + User.MAX_NICKNAME_LEN_STR + "}$"; 

        public const string s_wordNoStrangeCharsRegEx = @"[^\p{L}\p{N} ._-]";
        public static Regex s_wordNoStrangeCharsRegularExpression = new Regex(s_wordNoStrangeCharsRegEx);
        private static Regex s_badHtmlTags = new Regex("<[/]*(abbr|acronym|address|applet|area|base|basefont|bdo|big|body|button|caption|center|cite|code|col|colgroup|dd|del|dir|div|dfn|dl|dt|embed|fieldset|font|form|frame|frameset|head|html|iframe|input|ins|isindex|kbd|label|legend|link|map|menu|meta|noframes|noscript|object|optgroup|option|param|pre|q|s|samp|script|select|small|span|strike|style|table|tbody|td|textarea|tfoot|th|thead|title|tr|tt|var|xmp)[.]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled );


        static public long s_imgQuality = 70L;

        public static string RdnRefresh { get { return DateTime.Today.Month.ToString(); } }

        public static void CheckBrowser(System.Web.UI.Page page, HttpContext context)
        {
            try
            {
                string pageName = page.GetType().ToString().Replace("ASP.", "").Replace("_aspx", ".aspx");

                /*
                if (context.Request.UserAgent.ToLower().Contains("iphone"))
                {
                    page.Response.Redirect(String.Format("iphone/{0}?{1}", pageName, context.Request.QueryString.ToString()), true);
                    return;
                }
                 */
            }
            catch
            {
            }
        }

        static public string ReplaceFirst(string sourceString, string findString, string replacementString)
        {
            int pos = sourceString.IndexOf(findString);
            if (pos < 0) return sourceString;

            return sourceString.Substring(0, pos) + replacementString + sourceString.Substring(pos + findString.Length);
        }


        /// <summary>
        /// Strips non-printable ascii characters 
        /// Refer to http://www.w3.org/TR/xml11/#charsets for XML 1.1
        /// Refer to http://www.w3.org/TR/2006/REC-xml-20060816/#charsets for XML 1.0
        /// </summary>
        /// <param name="input">string to be checked</param>
        /// <returns>the string cleaned by the illegal characters</returns>
        static public string StripIllegalXMLChars(string input)
        {
            ////remove XML 1.1 illegal characters
            ////(USE @"#x((10?|[2-F])FFF[EF]|FDD[0-9A-F]|7F|8[0-46-9A-F]9[0-9A-F])" for XML 1.0)
            //Regex regex = new Regex(@"#x((10?|[2-F])FFF[EF]|FDD[0-9A-F]|[19][0-9A-F]|7F|8[0-46-9A-F]|0?[1-8BCEF])", RegexOptions.IgnoreCase);
            //Regex regex2 = new Regex(@"[\u0000-\u0008]|[\u000B-\u000C]|[\u000E-\u0019]|[\u007F-\u009F]", RegexOptions.IgnoreCase);
            //return regex2.Replace(regex.Replace(inputXML, string.Empty), string.Empty);

            StringBuilder output = new StringBuilder(); // Used to hold the output.
            char current; // Used to reference the current character.

            if (string.IsNullOrEmpty(input))
                return string.Empty; // vacancy test.
            for (int i = 0; i < input.Length; i++)
            {
                current = input[i]; // NOTE: No IndexOutOfBoundsException caught here; it should not happen.
                if ((current == 0x9) ||
                    (current == 0xA) ||
                    (current == 0xD) ||
                    (current == 0x1A) ||
                    ((current >= 0x20) && (current <= 0xD7FF)) ||
                    ((current >= 0xE000) && (current <= 0xFFFD))) // ||
                    //((current >= 0x10000) && (current <= 0x10FFFF)))
                    output.Append(current);
            }
            return output.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        /// ---------------------------------------------------------------------------------------------------------------------------
        public static string ConvertExtensionToMimeType(string extension)
        {
            string ext = extension.ToLower().Trim(new char[] { ' ', '.' });
            switch (ext)
            {
                case "ai": return "application/postscript";
                case "aif": return "audio/x-aiff";
                case "aifc": return "audio/x-aiff";
                case "aiff": return "audio/x-aiff";
                case "asc": return "text/plain";
                case "asf": return "video/x-ms-asf";
                case "au": return "audio/basic";
                case "avi": return "video/x-msvideo";
                case "bcpio": return "application/x-bcpio";
                case "bin": return "application/octet-stream";
                case "c": return "text/plain";
                case "cc": return "text/plain";
                case "ccad": return "application/clariscad";
                case "cdf": return "application/x-netcdf";
                case "class": return "application/octet-stream";
                case "cpio": return "application/x-cpio";
                case "cpp": return "text/plain";
                case "cpt": return "application/mac-compactpro";
                case "cs": return "text/plain";
                case "csh": return "application/x-csh";
                case "css": return "text/css";
                case "dcr": return "application/x-director";
                case "dir": return "application/x-director";
                case "dms": return "application/octet-stream";
                case "doc": return "application/msword";
                case "drw": return "application/drafting";
                case "dvi": return "application/x-dvi";
                case "dwg": return "application/acad";
                case "dxf": return "application/dxf";
                case "dxr": return "application/x-director";
                case "eps": return "application/postscript";
                case "etx": return "text/x-setext";
                case "exe": return "application/octet-stream";
                case "ez": return "application/andrew-inset";
                case "f": return "text/plain";
                case "f90": return "text/plain";
                case "fli": return "video/x-fli";
                case "flv": return "video/x-flv";
                case "gif": return "image/gif";
                case "gtar": return "application/x-gtar";
                case "gz": return "application/x-gzip";
                case "h": return "text/plain";
                case "hdf": return "application/x-hdf";
                case "hh": return "text/plain";
                case "hqx": return "application/mac-binhex40";
                case "htm": return "text/html";
                case "html": return "text/html";
                case "ice": return "x-conference/x-cooltalk";
                case "ief": return "image/ief";
                case "iges": return "model/iges";
                case "igs": return "model/iges";
                case "ips": return "application/x-ipscript";
                case "ipx": return "application/x-ipix";
                case "jpe": return "image/jpeg";
                case "jpeg": return "image/jpeg";
                case "jpg": return "image/jpeg";
                case "js": return "application/x-javascript";
                case "kar": return "audio/midi";
                case "latex": return "application/x-latex";
                case "lha": return "application/octet-stream";
                case "lsp": return "application/x-lisp";
                case "lzh": return "application/octet-stream";
                case "m": return "text/plain";
                case "man": return "application/x-troff-man";
                case "me": return "application/x-troff-me";
                case "mesh": return "model/mesh";
                case "mid": return "audio/midi";
                case "midi": return "audio/midi";
                case "mime": return "www/mime";
                case "mov": return "video/quicktime";
                case "movie": return "video/x-sgi-movie";
                case "mp2": return "audio/mpeg";
                case "mp3": return "audio/mpeg";
                case "mpe": return "video/mpeg";
                case "mpeg": return "video/mpeg";
                case "mpg": return "video/mpeg";
                case "mpga": return "audio/mpeg";
                case "ms": return "application/x-troff-ms";
                case "msh": return "model/mesh";
                case "nc": return "application/x-netcdf";
                case "oda": return "application/oda";
                case "pbm": return "image/x-portable-bitmap";
                case "pdb": return "chemical/x-pdb";
                case "pdf": return "application/pdf";
                case "pgm": return "image/x-portable-graymap";
                case "pgn": return "application/x-chess-pgn";
                case "png": return "image/png";
                case "pnm": return "image/x-portable-anymap";
                case "pot": return "application/mspowerpoint";
                case "ppm": return "image/x-portable-pixmap";
                case "pps": return "application/mspowerpoint";
                case "ppt": return "application/mspowerpoint";
                case "ppz": return "application/mspowerpoint";
                case "pre": return "application/x-freelance";
                case "prt": return "application/pro_eng";
                case "ps": return "application/postscript";
                case "qt": return "video/quicktime";
                case "ra": return "audio/x-realaudio";
                case "ram": return "audio/x-pn-realaudio";
                case "ras": return "image/cmu-raster";
                case "rgb": return "image/x-rgb";
                case "rm": return "audio/x-pn-realaudio";
                case "roff": return "application/x-troff";
                case "rpm": return "audio/x-pn-realaudio-plugin";
                case "rtf": return "text/rtf";
                case "rtx": return "text/richtext";
                case "scm": return "application/x-lotusscreencam";
                case "set": return "application/set";
                case "sgm": return "text/sgml";
                case "sgml": return "text/sgml";
                case "sh": return "application/x-sh";
                case "shar": return "application/x-shar";
                case "silo": return "model/mesh";
                case "sit": return "application/x-stuffit";
                case "skd": return "application/x-koan";
                case "skm": return "application/x-koan";
                case "skp": return "application/x-koan";
                case "skt": return "application/x-koan";
                case "smi": return "application/smil";
                case "smil": return "application/smil";
                case "snd": return "audio/basic";
                case "sol": return "application/solids";
                case "spl": return "application/x-futuresplash";
                case "src": return "application/x-wais-source";
                case "step": return "application/STEP";
                case "stl": return "application/SLA";
                case "stp": return "application/STEP";
                case "sv4cpio": return "application/x-sv4cpio";
                case "sv4crc": return "application/x-sv4crc";
                case "swf": return "application/x-shockwave-flash";
                case "t": return "application/x-troff";
                case "tar": return "application/x-tar";
                case "tcl": return "application/x-tcl";
                case "tex": return "application/x-tex";
                case "tif": return "image/tiff";
                case "tiff": return "image/tiff";
                case "tr": return "application/x-troff";
                case "tsi": return "audio/TSP-audio";
                case "tsp": return "application/dsptype";
                case "tsv": return "text/tab-separated-values";
                case "txt": return "text/plain";
                case "unv": return "application/i-deas";
                case "ustar": return "application/x-ustar";
                case "vcd": return "application/x-cdlink";
                case "vda": return "application/vda";
                case "vrml": return "model/vrml";
                case "wav": return "audio/x-wav";
                case "wm": return "video/x-ms-wm";
                case "wma": return "audio/x-ms-wma";
                case "wmv": return "video/x-ms-wmv";
                case "wrl": return "model/vrml";
                case "xbm": return "image/x-xbitmap";
                case "xlc": return "application/vnd.ms-excel";
                case "xll": return "application/vnd.ms-excel";
                case "xlm": return "application/vnd.ms-excel";
                case "xls": return "application/vnd.ms-excel";
                case "xlw": return "application/vnd.ms-excel";
                case "xml": return "text/xml";
                case "xpm": return "image/x-xpixmap";
                case "xwd": return "image/x-xwindowdump";
                case "xyz": return "chemical/x-pdb";
                case "zip": return "application/zip";
                case "docm": return "application/vnd.ms-word.document.macroEnabled.12";
                case "docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case "dotm": return "application/vnd.ms-word.template.macroEnabled.12";
                case "dotx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                case "potm": return "application/vnd.ms-powerpoint.template.macroEnabled.12";
                case "potx": return "application/vnd.openxmlformats-officedocument.presentationml.template";
                case "ppam": return "application/vnd.ms-powerpoint.addin.macroEnabled.12";
                case "ppsm": return "application/vnd.ms-powerpoint.slideshow.macroEnabled.12";
                case "ppsx": return "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
                case "pptm": return "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
                case "pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case "xlam": return "application/vnd.ms-excel.addin.macroEnabled.12";
                case "xlsb": return "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
                case "xlsm": return "application/vnd.ms-excel.sheet.macroEnabled.12";
                case "xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "xltm": return "application/vnd.ms-excel.template.macroEnabled.12";
                case "xltx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
                default: return "text/plain";
            }
        }

        /// Capitalize the first letter of a string
        /// ----------------------------------------------------------------------------------------------------------------------------
        public static string Capitalize(string str)
        {
            string temp = str.Substring(0, 1);
            return temp.ToUpper() + str.Remove(0, 1);
        }
        /// <summary>
        ///		Output the input string encoded as HTML and truncate the content
        ///		to a max lenght. Add the ... if truncation occurs.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        /// ---------------------------------------------------------------------------------------------------------------------------
        static public string DisplayHTML(string message, int maxLength) { return DisplayHTML(message, maxLength, false); }
        static public string DisplayHTML(string message, int maxLength, bool strict)
        {
            string resMsg = message.Trim();
            if (resMsg.Length <= maxLength)
                return HttpUtility.HtmlEncode(resMsg);     //MAT Mar 2009: Encode HTML when short msg

            if (strict)
            {
                resMsg = resMsg.WithEllipsis(maxLength);
            }
            else
            {

                int counter = maxLength - 4;
                while (resMsg[counter] != ' ')
                {
                    counter++;
                    if (counter > maxLength + 6 || counter >= resMsg.Length)
                        break;
                }
                maxLength = counter;
                if (maxLength < resMsg.Length - 4)
                    resMsg = resMsg.Remove(maxLength) + " ...";
            }


            // MAT 03/10: Make sure we are not cutting a html tag
            resMsg = Regex.Replace(resMsg, @"<[^>]*$", "");

            return HttpUtility.HtmlEncode(resMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="minLenght"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        /// ---------------------------------------------------------------------------------------------------------------------------
        static public string WrapText(string inputText, int minLenght, int maxLength)
        {
            // bool ignoreWrap = false;
            string wrappedText = string.Empty;

            string[] lines = inputText.Split(new char[] { '\n' });
            foreach (string line in lines)
            {
                string[] words = line.Split(new char[] { ' ', '\r' });
                int lineCounter = 0;
                foreach (string word in words)
                {
                    if (word.Trim().Length == 0)
                        continue;
#if false
          if (word.StartsWith("<a") || word.StartsWith("<p")) //Assume is an HTML TAG so no space taken
          {
            ignoreWrap = true;
            wrappedText += word + " ";
            continue;
          }
          if ( word.StartsWith(">") && ignoreWrap) // if it starts with a close < then keep the rest for wrapping
          {
            ignoreWrap = false;
          }
          else if ( word.EndsWith(">") && ignoreWrap)
          {
            ignoreWrap = false;
            wrappedText += word + " ";
            continue;
          }
          if (ignoreWrap)
          {
            wrappedText += word + " ";
            continue;
          }
#endif
                    // Next word will pass the max line length => add a newLine
                    if (lineCounter + word.Length + 1 >= maxLength)
                    {
                        // Split a single word that is longer than the line
                        if (word.Length >= maxLength)
                        {
                            int fragments = word.Length / maxLength + 1;
                            int fragmentStart = 0;
                            int fragmentLength = 0;
                            for (int t = 0; t < fragments; t++)
                            {
                                fragmentLength = (word.Length - fragmentStart > maxLength) ? maxLength : word.Length - fragmentStart;
                                if (lineCounter > 0)
                                    wrappedText += Environment.NewLine + word.Substring(fragmentStart, fragmentLength);
                                else
                                    wrappedText += word.Substring(fragmentStart, fragmentLength);
                                fragmentStart += fragmentLength;
                            }
                            if (fragmentLength < maxLength)
                            {
                                wrappedText += " ";
                                lineCounter = fragmentLength + 1;
                            }
                            else
                            {
                                wrappedText += Environment.NewLine;
                                lineCounter = 0;
                            }
                            continue;
                        }
                        wrappedText += Environment.NewLine;
                        lineCounter = 0;
                    }
                    wrappedText += word;
                    wrappedText += " ";
                    lineCounter += word.Length + 1;
                }
                wrappedText += Environment.NewLine;
            }
            return wrappedText;
        }

        /// <summary>
        ///  Replace each Regex match with the HTML code that creates an hyperlink 
        /// </summary>
        /// <param name="matchingURL"></param>
        /// <returns></returns>
        /// ---------------------------------------------------------------------------------------------------------------------------
        static public string HiperlinkAUrl(Match matchingURL)
        {
            return String.Format(" <a class=\"CommentLink\" target=\"_blank\" title=\"{1}\" href=\"{0}\" >{0}</a> ", matchingURL.Value, HttpUtility.HtmlAttributeEncode(matchingURL.Value));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        /// ---------------------------------------------------------------------------------------------------------------------------
        static public string RenderAsHtml(string inputText)
        {
            if (inputText.Contains("\r\n"))
                return inputText.Replace("\r\n", "<br/>");
            else if (inputText.Contains("\n"))
                return inputText.Replace("\n", "<br/>");
            else
                return inputText;
        }


        /// <summary>
        /// Sanitize an input user string replacing illegal chars with '_'
        /// </summary>
        /// <param name="s"></param>
        /// Date: July 2007
        /// Author: Matteo Porru
        /// -----------------------------------------------------------------------------------------------------
        static public string Sanitize(string s) { return Utilities.Sanitize(s, '_', false); }
        static public string Sanitize(string s, char subchar) { return Utilities.Sanitize(s, subchar, false); }
        static public string Sanitize(string s, char subchar, bool skip)
        {
            string admitted = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!-_+*°§ùàòèéìç£@#$^.()[] ";
            StringBuilder output = new StringBuilder(s.Length);
            bool found = false;

            foreach (char c in s)
            {
                found = false;
                foreach (char adm in admitted)
                {
                    if (c == adm)
                    {
                        found = true;
                        output.Append(c);
                    }
                }

                if (found == false)
                {
                    if (!skip)
                        output.Append(subchar);
                }
            }

            return output.ToString();
        }


        /// <summary>
        /// Sanitize an url removing illegal characters
        /// </summary>
        /// <param name="s"></param>
        /// Date: November 2008
        /// Author: Cristiano Ghersi
        /// -----------------------------------------------------------------------------------------------------
        static public string SanitizeUrl(string url)
        {
            //replace spaces with dashes
            string s = url.Replace(' ', '-');

            string admitted = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!-_+°§£@#$^()[]";
            StringBuilder output = new StringBuilder(s.Length);
            bool found = false;

            //skip unadmitted chars
            foreach (char c in s)
            {
                found = false;
                foreach (char adm in admitted)
                {
                    if (c == adm)
                    {
                        found = true;
                        output.Append(c);
                    }

                    if (found)
                        break;
                }
            }

            return HttpUtility.HtmlAttributeEncode(HttpUtility.UrlEncode(output.ToString()));
        }


        /// <summary>
        /// Remove non ASCII chars
        /// </summary>
        /// <param name="s"></param>
        /// Date: March 2007
        /// Author: Matteo Porru
        /// -----------------------------------------------------------------------------------------------------
        static public string RemoveNonAscii(string s)
        {
            StringBuilder output = new StringBuilder(s.Length);

            foreach (char c in s)
            {
                if ((int)c <= 127)
                {
                    output.Append(c);
                }
            }

            return output.ToString();
        }


        /// <summary>
        /// Encode an input user string with http encoding
        /// </summary>
        /// <param name="s"></param>
        /// Date: July 2007
        /// Author: Matteo Porru
        /// -----------------------------------------------------------------------------------------------------
        static public string Encode(string s)
        {
            string admitted = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!-_+*°§ùàòèéìç£@#$%^.: ";
            StringBuilder output = new StringBuilder(s.Length);
            bool found = false;

            foreach (char c in s)
            {
                found = false;
                foreach (char adm in admitted)
                {
                    if (c == adm)
                    {
                        found = true;
                        output.Append(c);
                    }
                }

                if (found == false)
                {
                    switch (c)
                    {
                        case '\'': output.Append("&rsquo;"); break; // single quote
                        case '"': output.Append("&quot;"); break; // double quote
                        case '&': output.Append("&amp;"); break;
                        case '<': output.Append("&lt;"); break;
                        case '>': output.Append("&gt;"); break;
                        case '»': output.Append("&raquo;"); break;
                        case '«': output.Append("&laquo;"); break;
                        default: output.AppendFormat("&#x{0:x};", (long)c); break;
                    }
                }
            }

            return output.ToString();
        }


        /// <summary>
        /// Decode an input user string with html encoding
        /// </summary>
        /// <param name="s"></param>
        /// Date: July 2007
        /// Author: Matteo Porru
        /// -----------------------------------------------------------------------------------------------------
        static public string Decode(string s)
        {
            StringWriter strWriter = new StringWriter();
            HttpUtility.HtmlDecode(s, strWriter);
            return strWriter.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------------------------------------
        static public string RemoveHtml(this string source)
        {
          // Regex regex = new Regex("(<(.+?)>)|([&](.+?)[;])", RegexOptions.IgnoreCase | RegexOptions.Multiline);
          //             return regex.Replace(s, " ").Trim();

          // much faster alternative: see http://www.dotnetperls.com/remove-html-tags
            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------------------------------------
        static public string RemoveHtmlComments(this string s)
        {
            Regex regex = new Regex(@"<!--(.|\s)*?-->", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regex.Replace(s, " ").Trim();
        }


        /// <summary>
        ///		Remove some but not all HTML tags.
        ///		Here we remove scripts and forms. (we should remove iframe as well ?)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------------------------------------
        static public string RemoveSomeHtml(this string s)
        {
            return RemoveHtmlForms(RemoveHtmlTag(s, "script"));
            /*
            Regex regex = new Regex("(<(.+?)>)|([&](.+?)[;])", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regex.Replace(s, string.Empty);*/
        }

        // Remove the <form> and </form> tags from an HTML input
        // -----------------------------------------------------------------------------------------------------
        static public string RemoveHtmlForms(string s)
        {
            Regex regex = new Regex("(<(form)([^>]*)>)|(</form>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regex.Replace(s, string.Empty);
        }


        /// <summary>
        /// Replace all the escaped chars (like \n, \r, \t etc.) with blanks.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static public string RemoveEscapedChars(this string input)
        {
            // The list of escaped chars:
            // http://msdn.microsoft.com/en-us/library/4edbef7e%28VS.80%29.aspx

            // removes standard escaped chars
            input = input.Replace('\a', ' ');
            input = input.Replace('\b', ' ');
            input = input.Replace('\t', ' ');
            input = input.Replace('\r', ' ');
            input = input.Replace('\v', ' ');
            input = input.Replace('\f', ' ');
            input = input.Replace('\n', ' ');
            input = input.Replace('\\', ' ');

            return input;
        }


        /// <summary>
        /// Remove all the occorences of '\0' in a string except the ending one
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        // -----------------------------------------------------------------------------------------------------
        static public string RemoveStringTerminations(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != '\0')
                    sb.Append(input[i]);
            }
            return sb.ToString();
        }


        // this method will remove *most* malicious code leaving allowed 
        // HTML intact (not working)
        // -----------------------------------------------------------------------------------------------------
        static public string RemoveMostHtml(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string output = "";
            // break the comments so someone cannot add an open comment
            input = input.Replace("<!--", "");

            // strip out comments and doctype
            Regex docType = new Regex("<!DOCTYPE[.]*>");
            output = docType.Replace(input, "");

            // add target="_blank" to hrefs and remove parts that are 
            // not supported
            //output = Regex.Replace(output, "(.*)", @"$5");

            // strip out most known tags except (a|b|br|blockquote|em|h1|h2|h3|h4|h5|h6|hr|i|img||li|ol|p|u|ul|strong|sub|sup)
            return s_badHtmlTags.Replace(output, "");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlFragment"></param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------------------------------------
        static public string FindPicturesInHtml(string htmlFragment)
        {
            //string html = htmlFragment.ToLower();
            int imageStart = 0;
            int nextStart = 0;
            int imageEnd = 0;
            imageStart = htmlFragment.IndexOf("<img", nextStart, StringComparison.OrdinalIgnoreCase);
            if (imageStart >= 0)
                imageStart = htmlFragment.IndexOf("src", imageStart, StringComparison.OrdinalIgnoreCase);
            while (imageStart >= 0 && imageStart < 2000)
            {
                imageEnd = htmlFragment.IndexOf('>', imageStart);
                nextStart = imageStart + 5;

                string imageHtmlFragment = htmlFragment.Substring(imageStart, imageEnd - imageStart + 1);
                // Regex imageUrlFinder = new Regex(@"(http|ftp|https|HTTP|FTP|HTTPS):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");
                Regex imageUrlFinder = new Regex(@"(?:""|')(http|ftp|https|HTTP|FTP|HTTPS):([^(""|')]*)(?:""|')");
                Match urlMatch = imageUrlFinder.Match(imageHtmlFragment);
                bool blacklisted = false;
                string imgUrl = urlMatch.Value.Trim(new char[] { ' ', '"', '\'' });
                foreach (string blackListImg in s_skipImages)
                {
                    if (imgUrl.Contains(blackListImg))
                    {
                        blacklisted = true;
                        break;
                    }
                }
                if (blacklisted)
                {
                    imageStart = htmlFragment.IndexOf("<img", nextStart, StringComparison.OrdinalIgnoreCase);
                    if (imageStart >= 0)
                        imageStart = htmlFragment.IndexOf("src", imageStart, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    string thumbData = CheckUrlContent(imgUrl);
                    if (string.IsNullOrEmpty(thumbData) && !thumbData.Contains("JFIF") && !thumbData.Contains("PNG"))
                    {
                        imageStart = htmlFragment.IndexOf("<img", nextStart, StringComparison.OrdinalIgnoreCase);
                        if (imageStart >= 0)
                            imageStart = htmlFragment.IndexOf("src", imageStart, StringComparison.OrdinalIgnoreCase);

                        continue;
                    }
                    else
                        return imgUrl;
                }
            }
            return string.Empty;

        }


        /// <summary>
        /// Removes "script" tags
        /// </summary>
        /// <param name="pageString"> The HTML source to parse.</param>
        /// <returns> A string with the cleaned HTML.</returns>
        /// -----------------------------------------------------------------------------------------------------
        static public string RemoveScripts(string pageString)
        {
            return RemoveHtmlTag(pageString, "script");
        }


        /// <summary>
        /// Removes HTML tags
        /// </summary>
        /// <param name="pageString"> The HTML source to parse.</param>
        /// <param name="tagName"> The name of the tag to remove </param>
        /// <returns> A string with the cleaned HTML.</returns>
        /// -----------------------------------------------------------------------------------------------------
        static public string RemoveHtmlTag(string pageString, string tagName)
        {
            string output = pageString;
            int exprIni = 0;
            int firstChar = 0;
            int lastChar = 0;

            // find html element
            exprIni = output.IndexOf("<" + tagName, 0, output.Length,
                                            StringComparison.InvariantCultureIgnoreCase);
            while (exprIni > 0)
            {
                // extract the html
                firstChar = exprIni;

                // find <tag ... />
                lastChar = output.IndexOf("/>", (firstChar + 1)) + 1;
                int nextCloseTag = output.IndexOf('>', (firstChar + 1));

                // closed with </tag> marker..
                if ((lastChar <= 0) || (nextCloseTag < lastChar))
                {
                    // find </tag>
                    lastChar = output.IndexOf("</" + tagName, (firstChar + 1), (output.Length - firstChar - 1),
                                                                    StringComparison.InvariantCultureIgnoreCase);
                    lastChar = output.IndexOf('>', (lastChar + 1));
                }

                if ((firstChar < 0) || (lastChar <= 0) || (firstChar == lastChar))
                    return output.Substring(0, firstChar);
                else
                {
                    output = String.Format("{0} {1}",
                                output.Substring(0, firstChar).Trim(),
                                output.Substring((lastChar + 1), (output.Length - lastChar - 1)).Trim());
                }
                exprIni = output.IndexOf("<" + tagName, 0, output.Length,
                                            StringComparison.InvariantCultureIgnoreCase);
            }

            return output;
        }


        /// <summary>
        ///    Converts a sostring given in a particular encoding to a destination encoding
        /// </summary>
        /// <param name="sourceEnc"></param>
        /// <param name="destEnc"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------------------------------------
        static public string EncodingConvert(Encoding sourceEnc, Encoding destEnc, string data)
        {
            return destEnc.GetString(Encoding.Convert(sourceEnc, destEnc, sourceEnc.GetBytes(data)));
        }


        /// <summary>
        /// Convert a Dictionary of keys, values into a valid querystring
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToQueryString(this IDictionary<string, string> dict)
        {
            if (dict.Count == 0) return string.Empty;

            var buffer = new StringBuilder();
            int count = 0;
            bool end = false;

            foreach (var key in dict.Keys)
            {
                if (count == dict.Count - 1) end = true;

                if (end)
                    buffer.AppendFormat("{0}={1}", key, dict[key]);
                else
                    buffer.AppendFormat("{0}={1}&", key, dict[key]);

                count++;
            }

            return buffer.ToString();
        }

        // This function downloads just the few first bytes out of an URL to verify is pointing to some real data
        /// -----------------------------------------------------------------------------------------------------
        static public string CheckUrlContent(string url)
        {
            const int buffSize = 1024;// 1 KB max
            char[] pageBuffer = new char[buffSize + 100];
            string content = string.Empty;

            WebClient client = new WebClient();

            // Add a user agent header in case the 
            // requested URI contains a query.
            client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*;q=0.8");
            /*
            client.Headers.Add( "User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.1.7) Gecko/20091221 Firefox/3.5.7 (.NET CLR 3.5.30729)" );
            client.Headers.Add( "Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7" );
            client.Headers.Add( "Accept-Encoding", "gzip,deflate" );
            client.Headers.Add( "Keep-Alive", "300" );
            client.Headers.Add( "Connection", "keep-alive" );
            */

            Stream data = null;
            StreamReader reader = null;
            try
            {
                data = client.OpenRead(url);
                reader = new StreamReader(data);
                int effectiveReadCount = reader.ReadBlock(pageBuffer, 0, buffSize); // ReadToEnd(); is dangerous because the link could be pointing to a huge file 

                //if( effectiveReadCount == buffSize )
                //	incompleteRead = true;

                content = new string(pageBuffer);
                data.Close();
                reader.Close();
            }
            catch
            {
                //client.Dispose();
                return string.Empty;
            }
            finally
            {
                if (data != null)
                    data.Close();
                if (reader != null)
                    reader.Close();
            }

            client.Dispose();
            return content;

        }

        static public string ExtractXMLFragment(string sourceData, string tag, ref int startPoint, out int endFragmentPosition)
        {
            if (startPoint < 0)       // invalid input
            {
                endFragmentPosition = sourceData.Length;
                return string.Empty;
            }
            string pageFragment = string.Empty;

            if (sourceData.Length <= startPoint + 3 + tag.Length)
            {
                endFragmentPosition = sourceData.Length;
                return string.Empty;
            }

            //search every tag
            startPoint = sourceData.IndexOf(string.Format("<{0} ", tag),
                                    (startPoint + 2 + tag.Length),
                                    StringComparison.InvariantCultureIgnoreCase);
            if (startPoint < 0)       // tag not found
            {
                endFragmentPosition = sourceData.Length;
                return string.Empty;
            }

            //copy of startPoint to avoid unwanted changes on reference param
            int tagIni = startPoint;

            int nextTag = -1;
            int numberOfQuotes = 0;
            int numberOfApos = 0;
            int startSearchNextTag = tagIni + 3;

            // Find the next < not enclosed between quotes or apos.
            do
            {
                nextTag = sourceData.IndexOf('<', startSearchNextTag);
                if (nextTag < 0)
                    break;

                numberOfQuotes = Regex.Matches(sourceData.Substring(tagIni, nextTag - tagIni + 1), "[\"].").Count;
                numberOfApos = Regex.Matches(sourceData.Substring(tagIni, nextTag - tagIni + 1), "['].").Count;
                startSearchNextTag = nextTag + 1;
            } while ((numberOfQuotes % 2 == 1) || (numberOfApos % 2 == 1));

            if (nextTag < 0)
                nextTag = sourceData.Length - 1;        // it's the latest tag

            endFragmentPosition = sourceData.IndexOf("/>", tagIni, nextTag - tagIni);
            if (endFragmentPosition < 0)
            {
                int count = nextTag + 3 + tag.Length - tagIni;
                if (count < 0)
                    count = 1;
                if (count + tagIni >= sourceData.Length)
                    count = sourceData.Length - tagIni - 1;

                endFragmentPosition = sourceData.IndexOf(string.Format("</{0}>", tag),
                                        tagIni, count,
                                        StringComparison.InvariantCultureIgnoreCase);
                if (endFragmentPosition < 0)      // malformed xml
                {
                    // malformed xml, try fixing it

                    int nextValidGt = tagIni;

                    // Find the next > not enclosed between quotes or apos.
                    do
                    {
                        endFragmentPosition = sourceData.IndexOf('>', nextValidGt, nextTag - nextValidGt);
                        if (endFragmentPosition < 0)
                            break;

                        numberOfQuotes = Regex.Matches(sourceData.Substring(tagIni, endFragmentPosition - tagIni + 1), "[\"].").Count;
                        numberOfApos = Regex.Matches(sourceData.Substring(tagIni, endFragmentPosition - tagIni + 1), "['].").Count;
                        nextValidGt = endFragmentPosition + 1;
                    } while ((numberOfQuotes % 2 == 1) || (numberOfApos % 2 == 1));


                    //endFragmentPosition = sourceData.IndexOf(">", tagIni, nextTag - tagIni);
                    if (endFragmentPosition > 0)
                    {
                        int blank = sourceData.IndexOfAny(new char[2] { ' ', '>' }, tagIni + 1);
                        string baseTag = sourceData.Substring(blank, endFragmentPosition - blank + 1);
                        pageFragment = string.Format("<{0}{1}</{0}>", tag, baseTag);
                    }
                    else
                    {
                        return string.Empty;    // unrecoverable wrong xml
                    }
                }
                else
                {
                    endFragmentPosition += tag.Length + 2;
                    pageFragment = sourceData.Substring(tagIni, endFragmentPosition - tagIni + 1);
                }
            }
            else
            {
                endFragmentPosition += 1;
                pageFragment = sourceData.Substring(tagIni, endFragmentPosition - tagIni + 1);
            }

            //-------------
            // workaround to patch malformed xhtml: no double quotes after =
            //-------------
            int firstGt = pageFragment.IndexOf("/>");
            if (firstGt < 0)
                firstGt = pageFragment.IndexOf(">");
            if (firstGt < 0)
                firstGt = endFragmentPosition;

            string endTagSegm = pageFragment.Substring(firstGt);
            string[] tagSegm = pageFragment.Substring(0, (pageFragment.Length - endTagSegm.Length)).Split(' ');
            for (int i = 0; i < tagSegm.Length; i++)
            {
                int eqPos = tagSegm[i].IndexOf('=');
                if (eqPos < 0)
                    continue;

                if (tagSegm[i].Length > (eqPos + 1) && tagSegm[i][eqPos + 1] != '"')
                //char.Parse("\""))
                {
                    if (tagSegm[i][eqPos + 1] != '\'')
                    {
                        tagSegm[i] = tagSegm[i].Substring(0, eqPos) + "=\"" + tagSegm[i].Substring(eqPos + 1) + "\"";
                    }
                    else
                    {       // substitute '
                        numberOfApos = Regex.Matches(tagSegm[i] + " ", "['].").Count;

                        // join the strings until we have even apos
                        while (((numberOfApos % 2) == 1) && (tagSegm.Length > i + 1))
                        {
                            tagSegm[i + 1] = tagSegm[i] + " " + tagSegm[i + 1];
                            tagSegm[i] = string.Empty;
                            i++;
                            numberOfApos = Regex.Matches(tagSegm[i] + " ", "['].").Count;
                        }

                        tagSegm[i] = string.Format("{0}=\"{1}\"",
                                            tagSegm[i].Substring(0, eqPos),
                                            (tagSegm[i].Substring(eqPos + 2)).TrimEnd('\''));
                    }
                }
            }
            pageFragment = string.Join(" ", tagSegm) + endTagSegm;

            return pageFragment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="resX"></param>
        /// <param name="resY"></param>
        /// <returns></returns>
        static public bool DetectImageSize(string url, out int resX, out int resY)
        {
            resX = 0;
            resY = 0;
            try
            {
                WebClient client = new WebClient();
                Stream data = client.OpenRead(url);
                Image remoteImage = Image.FromStream(data);
                resX = (int)remoteImage.Width;
                resY = (int)remoteImage.Height;
                data.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Converts a generic Url to absolute Uri
        /// </summary>
        /// <param name="baseUrl">The main url of the website</param>
        /// <param name="extractedUrl">The url to be converted</param>
        /// <returns>An absolute Uri | null</returns>
        /// -----------------------------------------------------------------------------------------------------
        static public Uri UriAbsoluteFromUrl(string baseUrl, string extractedUrl)
        {
            // The URI is in a HTTP page and could have encoded HTML char ...
            string link = HttpUtility.HtmlDecode(extractedUrl.Replace("\n", ""));
            Uri baseUri = new Uri(baseUrl);
            string siteRoot = string.Empty;

            // create absolute link
            if (link.StartsWith("/"))
            {
                //absolute replace
                int firstSlashPos = baseUrl.IndexOf("://");
                string protocol = baseUrl.Substring(0, firstSlashPos + 3);
                Uri siteRef = new Uri(baseUrl.Trim());
                link = string.Concat(protocol, siteRef.Host, link.Trim());
            }
            else if (link.StartsWith("./"))
            {
                //relative replace - same level
                siteRoot = baseUrl.Trim();
                siteRoot = siteRoot.TrimEnd('/');
                link = string.Format("{0}/{1}", siteRoot, link.TrimStart('.', '/'));
            }
            else if (link.StartsWith("../"))
            {
                //relative replace - multi level                                
                siteRoot = baseUrl.Trim();
                string clean = link;

                while (clean.StartsWith("../"))
                {
                    clean = clean.Remove(0, 3);
                    siteRoot = siteRoot.TrimEnd('/');
                    siteRoot = siteRoot.Substring(0, siteRoot.LastIndexOf('/'));
                }

                link = string.Format("{0}/{1}", siteRoot, clean.TrimStart('/'));
            }
            else if (!link.Contains("://"))
            {
                //relative replace - same level
                baseUrl += "/";
                int lastSlash = baseUrl.LastIndexOf('/');
                if (lastSlash > 0 && lastSlash < baseUrl.Length)
                    siteRoot = baseUrl.Substring(0, lastSlash);
                siteRoot = siteRoot.TrimEnd('/');
                link = string.Format("{0}/{1}", siteRoot, link.TrimStart('.', '/'));
            }

            // escape the link
            if (!Uri.IsWellFormedUriString(link, UriKind.Absolute))
                link = Uri.EscapeDataString(link);

            if (Uri.IsWellFormedUriString(link, UriKind.Absolute))
                return new Uri(link);
            else
                return null;
        }


        /// <summary>
        /// Find out if the input string is a valid (well formed) url
        /// </summary>
        /// <param name="url"></param>
        /// <returns>false if the given input is null, empty or doesn't match the required Regex</returns>
        static public bool UrlIsValid(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            Regex urlExpr = new Regex(s_urlRegEx);
            return urlExpr.IsMatch(url);
        }

        /// <summary>
        /// Find out if the input string is a valid (well formed) email
        /// </summary>
        /// <param name="url"></param>
        /// <returns>false if the given input is null, empty or doesn't match the required Regex</returns>
        static public bool EmailIsValid(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            Regex mailExpr = new Regex(s_emailRegEx);
            return mailExpr.IsMatch(email);
        }

        /// <summary>
        /// Find out if the input string is a valid (well formed) name, i.e. starts with a letter and contains only letters, digits and 
        /// '_', '-', spaces
        /// </summary>
        /// <param name="candidateName"></param>
        /// <param name="minimumLen">the minimum number of chars allowed (included)</param>
        /// <param name="checkOnlyLen">true to check only the minimum length;
        ///     false to check also the allowed characters inside the input string</param>
        /// <param name="maxLen">the maximum number of chars allowed (included)</param>
        /// <returns>false if the given input is null, empty or doesn't match the required Regex</returns>
        public static bool NameIsValid(string candidateName, int minimumLen, bool checkOnlyLen, int maxLen = int.MaxValue)
        {
            if (candidateName == null)
                return (minimumLen == 0); //return false if there's a minimum len
            string trimmedName = candidateName.Trim();
            if ((trimmedName.Length < minimumLen) || (trimmedName.Length > maxLen))
                return false;
            if (checkOnlyLen)
                return true;
            Regex nameExpr = new Regex(s_nameRegEx.Replace("ç", (minimumLen - 1) + "").Replace("§", (maxLen - 1) + ""));
            return nameExpr.IsMatch(trimmedName);
        }

        /// <summary>
        /// Find out if the input string is a valid (well formed) name, i.e. contains everything but special chars that can affect our search.
        /// Check s_badChars for unallowed chars.
        /// </summary>
        /// <param name="candidateName"></param>
        /// <param name="minimumLen">the minimum number of chars allowed (included)</param>
        /// <param name="checkOnlyLen">true to check only the minimum length;
        ///     false to check also the allowed characters inside the input string</param>
        /// <param name="maxLen">the maximum number of chars allowed (included)</param>
        /// <returns>false if the given input is null, empty or doesn't match the required Regex</returns>
        public static bool PropValueIsValid(string candidateName, int minimumLen, bool checkOnlyLen, int maxLen = int.MaxValue)
        {
            if (candidateName == null)
                return false;
            string trimmedName = candidateName.Trim();
            if ((trimmedName.Length < minimumLen) || (trimmedName.Length > maxLen))
                return false;
            if (checkOnlyLen)
                return true;
            return (trimmedName.IndexOfAny(s_badChars) == -1);
            //Regex nameExpr = new Regex(s_propValueRegEx.Replace("ç", minimumLen + "").Replace("§", (maxLen - 1) + ""));
            //return nameExpr.IsMatch(trimmedName);
        }


        /// <summary>
        /// Find out if the input string is a valid (well formed, without spaces) name 
        /// </summary>
        /// <param name="candidateName"></param>
        /// <param name="minimumLen">the minimum number of chars allowed (included)</param>
        /// <param name="checkOnlyLen">true to check only the minimum length;
        ///     false to check also the allowed characters inside the input string</param>
        /// <param name="maxLen">the maximum number of chars allowed (included)</param>
        /// <returns>false if the given input is null, empty or doesn't match the required Regex</returns>
        public static bool NoSpaceNameIsValid(string candidateName, int minimumLen, bool checkOnlyLen, int maxLen = int.MaxValue)
        {
            if (candidateName == null)
                return false;
            string trimmedName = candidateName.Trim();
            if ((trimmedName.Length < minimumLen) || (trimmedName.Length > maxLen))
                return false;
            if (checkOnlyLen)
                return true;
            Regex mailExpr = new Regex(s_nameNoSpaceRegEx.Replace("ç", (minimumLen - 1) + "").Replace("§", (maxLen - 1) + ""));
            return mailExpr.IsMatch(candidateName);
        }

        /// <summary>
        /// Replaces the characters that are not digit, nor alphabetical characters, not dash ('-'), space or underscore ('_')
        /// with the given replacer character
        /// </summary>
        /// <param name="input"></param>
        /// <param name="replacer"></param>
        /// <returns></returns>
        public static string ReplaceStrangeChars(string input, string replacer)
        {
            return s_wordNoStrangeCharsRegularExpression.Replace(input, replacer);
        }

        public static string HtmlToXhtml(string content)
        {
            //TODO
            return content;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        /// --------------------------------------------------------------------------------------------------------
        static public ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            } return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        /// --------------------------------------------------------------------------------------------------------
        static public Stream ResizeImage(System.Drawing.Image source, int maxWidth, int maxHeight, string contentType)
        {
            double scale = 1.0;
            //int width, height;
            int new_width, new_height;

            new_width = source.Width;
            new_height = source.Height;

            if (new_width > maxWidth)
            {
                scale = (double)maxWidth / (double)new_width;
                new_width = maxWidth;
                new_height = (int)(new_height * scale);
            }
            if (new_height > maxHeight)
            {
                scale = (double)maxHeight / (double)new_height;
                new_height = maxHeight;
                new_width = (int)(new_width * scale);
            }

            // Creates the resized image
            Bitmap resizedImg = new Bitmap(new_width, new_height);

            // Draw into it
            Graphics graphics = Graphics.FromImage(resizedImg);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            graphics.DrawImage(source, new Rectangle(0, 0, resizedImg.Width, resizedImg.Height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel);

            graphics.Dispose();

            //create the output stream and save the resulting image into it:
            Stream output = new MemoryStream();
            ImageFormat frmt;
            switch (contentType)
            {
                case "image/gif":
                    frmt = ImageFormat.Gif;
                    resizedImg.Save(output, frmt);
                    break;
                case "image/png":
                    {
                        frmt = ImageFormat.Png;
                        EncoderParameter encPar = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, s_imgQuality);
                        EncoderParameters encParList = new EncoderParameters(1);
                        ImageCodecInfo cropEncoder = GetEncoderInfo("image/png");
                        encParList.Param[0] = encPar;

                        resizedImg.Save(output, cropEncoder, encParList);
                    }
                    break;
                case "image/jpeg":
                case "image/jpg":
                default:
                    {
                        frmt = ImageFormat.Jpeg;
                        EncoderParameter encPar = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, s_imgQuality);
                        EncoderParameters encParList = new EncoderParameters(1);
                        ImageCodecInfo cropEncoder = GetEncoderInfo("image/jpeg");
                        encParList.Param[0] = encPar;
                        resizedImg.Save(output, cropEncoder, encParList);
                    }
                    break;
            }

            return output;
        }


        /// <summary>
        ///     Crop the source image to the input width & height, taking the minimum dimension
        ///     and deleting the external parts of the maximum dimension.
        /// </summary>
        /// <param name="source">image to crop</param>
        /// <param name="width">width of the resulting image</param>
        /// <param name="height">height of the resulting image</param>
        /// <returns>a stream containing the image</returns>
        /// --------------------------------------------------------------------------------------------------------
        static public Stream Crop(System.Drawing.Image source, int width, int height, string contentType)
        {
            //initialize internal variables for dimensions:
            int sourceWidth = source.Width;
            int sourceHeight = source.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;
            float nPercent = 0;
            float nPercentW = ((float)width / (float)sourceWidth);
            float nPercentH = ((float)height / (float)sourceHeight);


            //take the greater dimension as starting point to resize the image, cropping 
            //the other dimension:
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                destY = (int)((height - (sourceHeight * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentH;
                destX = (int)((width - (sourceWidth * nPercent)) / 2);
            }

            //set the final dimensions of the image
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            //create the resulting image:
            Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            //enhance quality of the resulting image:
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(source,
                                new Rectangle(destX, destY, destWidth, destHeight),
                                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                                GraphicsUnit.Pixel);

            grPhoto.Dispose();

            //create the output stream and save the resulting image into it:
            Stream output = new MemoryStream();
            ImageFormat frmt;
            switch (contentType)
            {
                case "image/gif":
                    frmt = ImageFormat.Gif;
                    bmPhoto.Save(output, frmt);
                    break;
                case "image/png":
                    {
                        frmt = ImageFormat.Png;
                        EncoderParameter encPar = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, s_imgQuality);
                        EncoderParameters encParList = new EncoderParameters(1);
                        ImageCodecInfo cropEncoder = GetEncoderInfo("image/png");
                        encParList.Param[0] = encPar;

                        bmPhoto.Save(output, cropEncoder, encParList);
                    }
                    break;
                case "image/jpeg":
                case "image/jpg":
                default:
                    {
                        frmt = ImageFormat.Jpeg;
                        EncoderParameter encPar = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, s_imgQuality);
                        EncoderParameters encParList = new EncoderParameters(1);
                        ImageCodecInfo cropEncoder = GetEncoderInfo("image/jpeg");
                        encParList.Param[0] = encPar;
                        bmPhoto.Save(output, cropEncoder, encParList);
                    }
                    break;
            }

            return output;
        }




        /// <summary>
        ///     Resize the source image to the input width & height, resizing the maximum dimension
        ///     and adding transparents as external parts of the minimum dimension.
        ///     It returns a PNG image.
        /// </summary>
        /// <param name="originalImg">image to resize</param>
        /// <param name="maxWidth">width of the resulting image</param>
        /// <param name="maxHeight">height of the resulting image</param>
        /// <returns>a stream containing the image</returns>
        /// -----------------------------------------------------------------------------------------------------
        public static Stream ExactResizeImage(System.Drawing.Image originalImg, int width, int height)
        {
            float tempDimension = 0F;
            float newWidth = 0F;
            float newHeight = 0F;
            float xCorner = 0F;
            float yCorner = 0F;
            try
            {
                //  work out the width/height for the thumbnail. Preserve aspect ratio and honor max width/height
                //  Note: if the original is smaller than the thumbnail size it will be scaled up
                if ((originalImg.Width / width) > (originalImg.Height / height))
                {
                    //original image: width > height
                    tempDimension = originalImg.Width;
                    newWidth = width;
                    newHeight = originalImg.Height * (width / tempDimension);
                    if (newHeight > height)
                    {
                        newWidth = (float)Math.Round(newWidth * (height / newHeight));
                        newHeight = height;
                    }
                    yCorner = (float)Math.Round((height - newHeight) / 2);
                }
                else
                {
                    //original image: height > width
                    tempDimension = originalImg.Height;
                    newHeight = height;
                    newWidth = (originalImg.Width * (width / tempDimension));
                    if (newWidth > width)
                    {
                        newHeight = (float)Math.Round(newHeight * (width / newWidth));
                        newWidth = width;
                    }
                    xCorner = (float)Math.Round((width - newWidth) / 2);
                }
                Bitmap resizedImage = new Bitmap(width, height);

                // Create a graphics object
                Graphics graphics = Graphics.FromImage(resizedImage);
                graphics.FillRectangle(Brushes.Transparent, 0F, 0F, width, height);
                graphics.DrawImage(originalImg, xCorner, yCorner, newWidth, newHeight);
                graphics.Dispose();

                //create the output stream and save the resulting image into it:
                Stream output = new MemoryStream();
                EncoderParameter encPar = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, s_imgQuality);
                EncoderParameters encParList = new EncoderParameters(1);
                ImageCodecInfo cropEncoder = GetEncoderInfo("image/png");
                encParList.Param[0] = encPar;

                resizedImage.Save(output, cropEncoder, encParList);

                return output;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// retrieve from Web the content of a page
        /// </summary>
        /// <param name="url">url of the page</param>
        /// <returns>the content of the page; string.Empty if any error occurred</returns>
        /// -----------------------------------------------------------------------------------------------------
        public static string RetrieveContentFromWeb(string url, out string mimeType)
        {
            mimeType = "unknown";
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            string pageString = string.Empty;

            try
            {
                Stream data = client.OpenRead(url);

                StreamReader reader = new StreamReader(data);
                pageString = reader.ReadToEnd();

                if (client.ResponseHeaders["Content-type"] != null)
                    mimeType = client.ResponseHeaders["Content-type"].ToLowerInvariant();

                data.Close();
                reader.Close();
            }
            catch
            {
                return string.Empty;
            }

            return pageString;
        }


        /// <summary>
        /// fill the response stream with an image containing error message. It could be:
        /// - a blank image with a string written inside
        /// - an image ready for the prompt
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errorMsg">the text to be written in the blank image; string.Empty to display an error image without text</param>
        /// <param name="imagePath">the path of the error image to be displayed; string.Empty to display a text in the blank image</param>
        /// -----------------------------------------------------------------------------------------------------
        public static void DrawErrorResponse(HttpContext context, string errorMsg, string imagePath)
        {
            //retrieve the background of the text
            string bckImage = string.Empty;
            if (string.IsNullOrEmpty(errorMsg))
                bckImage = context.Server.MapPath(imagePath);
            else
                bckImage = context.Server.MapPath("sPressResource/blankErrorMessage.png");
            Bitmap bitMapImage = new Bitmap(bckImage);
            Graphics graphicImage = Graphics.FromImage(bitMapImage);
            graphicImage.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //write the error message to the image
            if (!string.IsNullOrEmpty(errorMsg))
                graphicImage.DrawString(errorMsg, new Font("Arial", 11, FontStyle.Regular), Brushes.Black, 110F, 90F);

            //place the image in output
            MemoryStream io = new MemoryStream();
            bitMapImage.Save(io, ImageFormat.Png);
            context.Response.ContentType = "image/png";
            context.Response.BinaryWrite(io.GetBuffer());

            //release resources
            graphicImage.Dispose();
            bitMapImage.Dispose();
            io.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateToPrint">Date in Universal Standard Time</param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------------------------------------
        static public string SmartDate(this DateTime dateToPrint, bool longDate = false)
        {
            DateTime now = DateTime.Now.ToUniversalTime();
            TimeSpan deltaTime = now - dateToPrint;
            if (deltaTime.TotalDays >= 7.0)
              return string.Format("on {0}", (longDate) ? dateToPrint.ToString("MMM d, yyyy") : dateToPrint.ToShortDateString());
            if (deltaTime.TotalDays >= 2.0)
              return string.Format("on {0} at {1}", dateToPrint.DayOfWeek.ToString(), dateToPrint.ToLongTimeString());
            if (deltaTime.TotalDays >= 1.0)
                return string.Format("yesterday at {0}", (longDate) ? dateToPrint.ToLongTimeString() : dateToPrint.ToShortTimeString());
            if (deltaTime.TotalHours >= 2.0)
              return string.Format("{0} hours ago", (int)(deltaTime.TotalHours));
            if (deltaTime.TotalHours >= 1.0)
              return string.Format("{0} hour ago", (int)(deltaTime.TotalHours));
            if (deltaTime.TotalMinutes > 1.0)
              return string.Format("{0} minutes ago", (int)(deltaTime.TotalMinutes));

            return "just now";

        }

        /// <summary>
        /// retrieve the TinyURL, given an url
        /// </summary>
        /// <param name="url"></param>
        /// <returns>the tiny url</returns>
        /// -----------------------------------------------------------------------------------------------------
        static public string GetShortUrl(string url)
        {
            return ShortenURL.DoShortenURL(url, ShortenURL.ShortURLProvider.Tinyurl);
        }


        /// <summary>
        /// take the input value and compute the hash value
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetHash(string source)
        {
            //translate input string into byte array
            byte[] tmpSource = ASCIIEncoding.ASCII.GetBytes(source);

            //compute the hash
            byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

            //create the resulting output
            StringBuilder result = new StringBuilder(tmpHash.Length);
            for (int i = 0; i < tmpHash.Length; i++)
            {
                result.Append(tmpHash[i].ToString("X2"));
            }

            return result.ToString();
        }

        /// <summary>
        /// Calculates the SHA1 hash of a string
        /// </summary>
        /// <param name="item">data="input string"</param>
        /// <returns>string</returns>
        /// Author: Matteo Porru
        /// Date: April 2007
        /// -------------------------------------------------------------------------
        public static string StrSHA1(string data)
        {
            SHA1 md = new SHA1CryptoServiceProvider();
            string encr = string.Empty;

            byte[] digest = md.ComputeHash(Encoding.Default.GetBytes(data));

            foreach (byte i in digest)
            {
                encr += i.ToString("x2");
            }
            return encr;
        }


        /// <summary>
        /// Removes a cookie.
        /// </summary>
        /// <param name="cookieName">Name of the cookie</param>
        /// <param name="currentPage">Page handler</param>
        static public void RemoveCookie(string cookieName, Page currentPage)
        {
            //remove KeepMeLoggedIn Cookie, if present
            HttpCookie c1 = currentPage.Request.Cookies[cookieName];
            currentPage.Request.Cookies.Remove(cookieName);

            if (c1 != null)
            {
                c1.Expires = DateTime.Now.AddDays(-10);
                c1.Value = null;
                currentPage.Response.SetCookie(c1);
            }
        }

        /// <summary>
        /// Reads the contents of the stream into a byte array.
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>A byte array containing the contents of the stream. NULL if any error happen</returns>
        /// ------------------------------------------------------------------------------------------------------------------------------
        public static byte[] ReadAllBytes(Stream source)
        {
            //long originalPosition = source.Position;
            //source.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];
                int totalBytesRead = 0;
                int bytesRead;


                while ((bytesRead = source.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = source.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }


                byte[] buffer = readBuffer;

                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }

                return buffer;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// read a Stream describing an XML and return the corresponding XML Document (correctly encoded)
        /// </summary>
        /// <param name="data"></param>
        /// <returns>null if any error occurred</returns>
        /// ------------------------------------------------------------------------------------------------------
        public static XmlDocument GetEncodedXML(Stream data)
        {
            string pageString = string.Empty;

            byte[] dataBytes = ReadAllBytes(data);
            if ((dataBytes == null) || (dataBytes.Length == 0))
                return null;
            MemoryStream memData = new MemoryStream(dataBytes);

            StreamReader reader = new StreamReader(memData);

            // Guess the encoding of the xml stream
            Encoding enc = Encoding.UTF8;
            string encoding = enc.EncodingName;
            string declaration = string.Empty;
            int index = -1;
            do
            {
                declaration = reader.ReadLine();
                index = declaration.IndexOf("?xml", StringComparison.InvariantCultureIgnoreCase);
            }
            while ((index <= 0) && (declaration.Length < 10) && (!reader.EndOfStream));

            if (index > 0)
            {
                declaration = declaration.Replace('\'', '"');
                index = declaration.IndexOf("encoding", StringComparison.InvariantCultureIgnoreCase);

                if (index > 0)
                {
                    int iniQuotes = declaration.IndexOf('"', index);
                    if (iniQuotes > 0)
                    {
                        int endQuotes = declaration.IndexOf('"', iniQuotes + 1);
                        encoding = declaration.Substring(iniQuotes + 1, endQuotes - iniQuotes - 1);
                    }
                }
            }

            //retrieve the encoding
            try
            {
                enc = Encoding.GetEncoding(encoding);
            }
            catch
            {
                enc = Encoding.UTF8;
            }

            // Rewind the stream
            memData.Seek(0, SeekOrigin.Begin);

            StreamReader reader1 = new StreamReader(memData, enc, true);
            pageString = reader1.ReadToEnd();
            reader1.Close();

            if (pageString == string.Empty)
                return null;

            //remove non valid chars:
            pageString = StripIllegalXMLChars(pageString);

            // Parse the loaded file as XML document
            // ------------------------------------------------------------------------
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(pageString);
            }
            catch
            {
                return null;
            }

            return xmlDoc;
        }

        /// <summary>
        /// Concatenates a GUID to a fixed and known string and returns random string
        /// </summary>
        /// <param name="fixedString">Fixed string</param>
        /// <returns>Random string GUID</returns>
        /// --------------------------------------------------------------------------------------------------------------
        public static string GenerateUniqueId(string fixedString)
        {
            Guid random_Guid_id = new Guid();
            random_Guid_id = Guid.NewGuid();
            return fixedString + random_Guid_id.ToString("N");
        }


        /// <summary>
        /// Admitted chars are lower case letters, upper case letters and digits
        /// </summary>
        /// <param name="size"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        /// --------------------------------------------------------------------------------------------------------------
        public static string GenerateRandomString(int size)
        {
            //int seed = (int)(DateTime.Now.Ticks & 0x00000000FFFFFFFF);
            int seed = int.Parse(Guid.NewGuid().ToString().Remove(8).Replace("-", string.Empty), System.Globalization.NumberStyles.HexNumber);

            Random r = new Random(seed);
            string legalChars = "abcdefghijklmnopqrstuvwxzyABCDEFGHIJKLMNOPQRSTUVWXZY0123456789";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++)
                sb.Append(legalChars.Substring(r.Next(0, legalChars.Length - 1), 1));
            return sb.ToString();
        }



        /// <summary>
        /// This routine splits a whole word at every Uppercase character, 
        ///  i.e. "getAllItemsByID" would be "Get All Items By ID".
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        static public string SplitUppercaseWords(string words)
        {
            StringBuilder SB = new System.Text.StringBuilder();

            foreach (Char C in words)
            {
                if (Char.IsUpper(C))
                    SB.Append(' ');
                SB.Append(C);
            }

            return SB.ToString();
        }


        #region XML Management
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This routine writes the given value onto the given node. If the node doesn't exists, it creates it and append
        /// it to the parentNode
        /// </summary>
        /// <param name="parentNode">parent node of the XML node to be written</param>
        /// <param name="nodeName">name of the node to be created/updated</param>
        /// <param name="nodeValue">value of the node to be written</param>
        /// <param name="overwrite">false to create another node, true to overwrite the existing one, if any</param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns>The created XML node</returns>
        static public XElement WriteXMLNode(this XElement parentNode, string nodeName, string nodeValue, bool overwrite = true,
            bool internalPurpose = true)
        {
            XElement node = null;
            if (overwrite)
                node = parentNode.GetNode(nodeName);
            if (node == null)
            {
                XName name = null;
                if (internalPurpose)
                    name = nodeName;
                else
                    name = ConfigReader.S2CNS + nodeName;
                node = new XElement(name);
                parentNode.Add(node);
            }
            node.Value = nodeValue;
            return node;
        }


        /// <summary>
        /// Builds an empty XML document with the specified root element
        /// </summary>
        /// <param name="rootNodeName"></param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns></returns>
        static public XDocument CreateXMLDoc(string rootNodeName, bool internalPurpose = true)
        {
            XName name = null;
            if (internalPurpose)
                name = rootNodeName;
            else
                name = ConfigReader.S2CNS + rootNodeName;
            XDocument result = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement(name));
            if (!internalPurpose)
                result.Root.Add(new XAttribute(XNamespace.Xmlns + ConfigReader.S2CPrefix, ConfigReader.S2CNS));
            return result;
        }

        /// <summary>
        /// This routine returns an XElement matching the given Xpath query
        /// </summary>
        /// <param name="xmlDoc">XML document where the node is contained</param>
        /// <param name="xpath">XPath query</param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns>The read XML node</returns>
        static public XElement FindNode(this XContainer xmlDoc, string xpath, bool internalPurpose = true)
        {
            if (internalPurpose)
                return xmlDoc.XPathSelectElement(xpath);
            else
                return xmlDoc.XPathSelectElement("//" + ConfigReader.S2CPrefix + ":" + xpath, ConfigReader.S2CNSManager);
        }


        /// <summary>
        /// This routine returns a child XElement of the given parent with the given name
        /// </summary>
        /// <param name="parent">XML element which the node is child of</param>
        /// <param name="nodeName">name of the node to retrieve</param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns>The read XML node</returns>
        static public XElement GetNode(this XContainer parent, string nodeName, bool internalPurpose = true)
        {
            return parent.Element((internalPurpose ? "" : ConfigReader.S2CNS) + nodeName);
        }


        /// <summary>
        /// This routine returns a list of XElements matching the given xpath query
        /// </summary>
        /// <param name="xmlDoc">container of the searched elements</param>
        /// <param name="xpath">XPath query</param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns>the list with the found elements, an empty list otherwise</returns>
        static public List<XElement> GetNodes(this XContainer xmlDoc, string xpath, bool internalPurpose = true)
        {
            if (internalPurpose)
                return new List<XElement>(xmlDoc.XPathSelectElements(xpath));
            else
                return new List<XElement>(xmlDoc.XPathSelectElements(ConfigReader.S2CPrefix + ":" + xpath, ConfigReader.S2CNSManager));
        }



        /// <summary>
        /// This routine reads an XElement into the given member variable
        /// </summary>
        /// <param name="xmlDoc">XML document where the node is contained</param>
        /// <param name="xpath">XPath query</param>
        /// <param name="member">member variable to be assigned</param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns>The read XML node</returns>
        static public XElement ParseNode(this XContainer xmlDoc, string xpath, ref long member, bool internalPurpose = true)
        {
            XElement node = xmlDoc.GetNode(xpath, internalPurpose);
            if (node != null)
                long.TryParse(node.Value, out member);

            return node;
        }


        /// <summary>
        /// This routine reads an XElement into the given member variable
        /// </summary>
        /// <param name="xmlDoc">XML document where the node is contained</param>
        /// <param name="xpath">XPath query</param>
        /// <param name="member">member variable to be assigned</param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns>The read XML node</returns>
        static public XElement ParseNode(this XContainer xmlDoc, string xpath, ref int member, bool internalPurpose = true)
        {
            XElement node = xmlDoc.GetNode(xpath, internalPurpose);
            if (node != null)
                int.TryParse(node.Value, out member);

            return node;
        }


        /// <summary>
        /// This routine reads an XElement into the given member variable
        /// </summary>
        /// <param name="xmlDoc">XML document where the node is contained</param>
        /// <param name="xpath">XPath query</param>
        /// <param name="member">member variable to be assigned</param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns>The read XML node</returns>
        static public XElement ParseNode(this XContainer xmlDoc, string xpath, ref short member, bool internalPurpose = true)
        {
            XElement node = xmlDoc.GetNode(xpath, internalPurpose);
            if (node != null)
                short.TryParse(node.Value, out member);

            return node;
        }

        /// <summary>
        /// This routine reads an XElement into the given member variable
        /// </summary>
        /// <param name="xmlDoc">XML document where the node is contained</param>
        /// <param name="xpath">XPath query</param>
        /// <param name="member">member variable to be assigned</param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns>The read XML node</returns>
        static public XElement ParseNode(this XContainer xmlDoc, string xpath, ref bool member, bool internalPurpose = true)
        {
            XElement node = xmlDoc.GetNode(xpath, internalPurpose);
            if (node != null)
                bool.TryParse(node.Value, out member);

            return node;
        }

        /// <summary>
        /// This routine reads an XElement into the given member variable
        /// </summary>
        /// <param name="xmlDoc">XML document where the node is contained</param>
        /// <param name="xpath">XPath query</param>
        /// <param name="member">member variable to be assigned</param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns>The read XML node</returns>
        static public XElement ParseNode(this XContainer xmlDoc, string xpath, ref float member, bool internalPurpose = true)
        {
            XElement node = xmlDoc.GetNode(xpath, internalPurpose);
            if (node != null)
                float.TryParse(node.Value, out member);

            return node;
        }

        /// <summary>
        /// This routine reads an XElement into the given member variable
        /// </summary>
        /// <param name="xmlDoc">XML document where the node is contained</param>
        /// <param name="xpath">XPath query</param>
        /// <param name="member">member variable to be assigned</param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns>The read XML node</returns>
        static public XElement ParseNode(this XContainer xmlDoc, string xpath, ref decimal member, bool internalPurpose = true)
        {
            XElement node = xmlDoc.GetNode(xpath, internalPurpose);
            if (node != null)
                decimal.TryParse(node.Value, out member);

            return node;
        }

        /// <summary>
        /// This routine reads an XElement into the given member variable
        /// </summary>
        /// <param name="xmlDoc">XML document where the node is contained</param>
        /// <param name="xpath">XPath query</param>
        /// <param name="member">member variable to be assigned</param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns>The read XML node</returns>
        static public XElement ParseNode(this XContainer xmlDoc, string xpath, ref string member, bool internalPurpose = true)
        {
            XElement node = xmlDoc.GetNode(xpath, internalPurpose);
            if (node != null)
                member = node.Value;

            return node;
        }

        /// <summary>
        /// This routine reads an XElement into the given member variable of Enum type
        /// </summary>
        /// <typeparam name="T">only Enum types are allowed</typeparam>
        /// <param name="xmlDoc">XML document where the node is contained</param>
        /// <param name="xpath">XPath query</param>
        /// <param name="member">member variable to be assigned</param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns>The read XML node</returns>
        static public XElement ParseNode<T>(this XContainer xmlDoc, string xpath, ref T member, bool internalPurpose = true)
        {
            XElement node = xmlDoc.GetNode(xpath, internalPurpose);
            if (node != null)
                member = (T)Enum.Parse(typeof(T), node.Value, true);

            return node;
        }


        /// <summary>
        /// This routine reads an XElement into the given member variable
        /// </summary>
        /// <param name="xmlDoc">XML document where the node is contained</param>
        /// <param name="xpath">XPath query</param>
        /// <param name="member">member variable to be assigned</param>
        /// <param name="internalPurpose">
        ///     if the XML is for internal purpose (e.g. store on DB, search compilation, it doesn't have the namespace s2c,
        ///     otherwise it will have s2c namespace
        /// </param>
        /// <returns>The read XML node</returns>
        static public XElement ParseNode(this XContainer xmlDoc, string xpath, ref DateTime member, bool internalPurpose = true)
        {
            XElement node = xmlDoc.GetNode(xpath, internalPurpose);
            if (node != null)
            {
                try
                {
                    member = DateTime.Parse(node.Value);
                }
                catch
                {
                    member = DateTime.Now - TimeSpan.FromDays(1.0);
                }
            }

            return node;
        }

        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Merges the given list of IDs into a comma separated string
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="removeZerosOrNegatives">true to remove the elements with value less than or equal to 0 
        ///     from the resulting string</param>
        /// <param name="separator">the separator of the resulting string, default ','</param>
        /// <returns></returns>
        public static string MergeIntoCommaSeparatedString<T>(ICollection<T> idList, bool removeZerosOrNegatives, char separator = ',')
               where T : struct
        {
            string list_ids = string.Empty;
            if (idList != null)
            {
                foreach (T id in idList)
                {
                    if (removeZerosOrNegatives && (Convert.ToInt64(id) <= 0))
                        continue;
                    list_ids += string.Format("{0}{1}", separator, id);
                }
                list_ids = list_ids.TrimStart(separator);
            }
            return list_ids;
        }


        /// <summary>
        /// Merges the given array of strings into a comma separated string
        /// </summary>
        /// <param name="list"></param>
        /// <param name="separator">the separator of the resulting string, default ','</param>
        /// <returns></returns>
        public static string MergeIntoCommaSeparatedString(string[] list, char separator = ',')
        {
            string list_ids = string.Empty;
            if (list != null)
            {
                foreach (string id in list)
                {
                    list_ids += string.Format("{0}{1}", separator, id);
                }
                list_ids = list_ids.TrimStart(separator);
            }
            return list_ids;
        }


        /// <summary>
        /// Returns a string containing the result of the ToString() method applied to all the
        /// objects of the given enumeration, separated by commas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Print<T>(this IEnumerable<T> list)
        {
            if (list == null)
                return "null";
            string res = string.Join(",", list.ToArray().Select(c => c.PrintNull()));
            res = res.TrimEnd(',');
            return res;
        }


        /// <summary>
        /// Returns true if the given collection is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> list)
        {
            return ((list == null) || (list.Count <= 0));
        }


        /// <summary>
        /// Returns the string in input if it is non null, "null" otherwise
        /// </summary>
        /// <param name="arg"></param>
        public static string PrintNull(this object arg)
        {
            return (arg == null ? "null" : arg.ToString());
        }


        /// <summary>
        /// Returns a list of longs contained in the given string separated by the given separator
        /// </summary>
        /// <param name="content"></param>
        /// <param name="separator"></param>
        /// <returns>an empty list if any error occurred; the list with ids otherwise</returns>
        public static List<long> ParseIntoListOfLongs(string content, char separator = ' ')
        {
            List<long> ids = new List<long>();

            if (string.IsNullOrEmpty(content))
                return ids;

            string[] strIds = content.Split(separator);
            foreach (string str in strIds)
            {
                long id = -1;
                if (long.TryParse(str, out id))
                {
                    if (id > 0)
                        ids.Add(id);
                }
            }

            return ids;
        }


        /// <summary>
        /// Returns a list of int contained in the given string separated by the given separator
        /// </summary>
        /// <param name="content"></param>
        /// <param name="separator"></param>
        /// <returns>an empty list if any error occurred; the list with ids otherwise</returns>
        public static List<int> ParseIntoListOfInt(this string content, char separator = ' ')
        {
            List<int> ids = new List<int>();

            if (string.IsNullOrEmpty(content))
                return ids;

            string[] strIds = content.Split(separator);
            foreach (string str in strIds)
            {
                int id = -1;
                if (int.TryParse(str, out id))
                {
                    if (id >= 0)
                        ids.Add(id);
                }
            }

            return ids;
        }


        ///// <summary>
        ///// Transforms a collection into an array
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="list"></param>
        ///// <returns></returns>
        //public static T[] ToArray<T>(this ICollection<T> list)
        //{
        //    if (list == null)
        //        return null;
        //    List<T> resp = new List<T>();
        //    foreach (T elem in list)
        //    {
        //        resp.Add(elem);
        //    }
        //    return resp.ToArray();
        //}


        /// <summary>
        /// Transforms a collection into an array of object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static object[] ToObjArray<T>(this ICollection<T> list)
        {
            if (list == null)
                return null;
            List<object> resp = new List<object>();
            foreach (T elem in list)
            {
                resp.Add(elem);
            }
            return resp.ToArray();
        }


        /// <summary>
        /// Function to get byte array from a file
        /// </summary>
        /// <param name="filename">File name to get byte array</param>
        /// <returns>an empty byte array if any error occurred; the content of the image otherwise</returns>
        public static byte[] FileToByteArray(string filename)
        {
            byte[] buffer = null;

            try
            {
                // Open file for reading
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

                // attach filestream to binary reader
                BinaryReader br = new BinaryReader(fs);

                // get total byte length of the file
                long totBytes = new FileInfo(filename).Length;

                // read entire file into buffer
                buffer = br.ReadBytes((int)totBytes);

                // close file reader
                fs.Close();
                fs.Dispose();
                br.Close();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Exception caught in process: {0}", ex.ToString());
                buffer = null;
            }

            return buffer;
        }


        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            var col = new ObservableCollection<T>();
            foreach (var cur in enumerable)
            {
                col.Add(cur);
            }
            return col;
        }

        public static int HashCode(this string input)
        {
          int hash = 0;
          if (string.IsNullOrEmpty(input)) 
                return hash;
          for (int i = 0; i < input.Length; i++) {
                char c = input[i];
            hash = ((hash<<5)-hash) + c;
            hash = hash & hash; // Convert to 32bit integer
          }
          return hash;
        }

        /// <summary>
        /// Get a substring of the first N characters, optionally using the last 3 chars available for trailing with "..."
        /// </summary>
        public static string Truncate(this string source, int length, bool threeDots=false)
        {
            if (source.Length > length)
            {
                if ((threeDots) && (length > 3))
                    source = source.Substring(0, length-3)+"...";
                else
                    source = source.Substring(0, length);
            }
            return source;
        }


        /// <summary>
        /// Trims the given sequence if present at the beginning and at the end of the given string.
        /// Note: it just trims only one instance of the sequence, differently from Trim() method.
        /// </summary>
        public static string TrimOne(this string source, string sequenceToTrim = " ")
        {
            if (string.IsNullOrEmpty(source))
                return source;
            int seqLen = sequenceToTrim.Length;
            if (source.StartsWith(sequenceToTrim))
                source = source.Substring(seqLen, source.Length - seqLen);
            if (source.EndsWith(sequenceToTrim))
                source = source.Substring(0, source.Length - seqLen);
            return source;
        }

        /// <summary>
        /// Sends an HTTP request to the given service url
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="data"></param>
        /// <param name="isPost">true to send a POST request, false to send a GET request</param>
        /// <returns>string.Empty if any error occurred, the server response otherwise</returns>
        public static string SendHttpRequest(string serviceUrl, string data, bool isPost)
        {
            // send out cookie along with a request for the protected page
            HttpWebRequest webRequest = null;
            try
            {
                webRequest = WebRequest.Create(serviceUrl) as HttpWebRequest;
            }
            catch (UriFormatException ufe)
            {
                log.ErrorFormat("Cannot connect to {0} due to {1} ", serviceUrl, ufe.Message);
                return string.Empty;
            }

            if (isPost)
            {
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                if (data != null)
                    webRequest.ContentLength = data.Length;
            }

            StreamReader responseReader = null;
            string responseData = string.Empty;
            try
            {
                if (isPost)
                {
                    // write the form values into the request message
                    StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
                    requestWriter.Write(data);
                    requestWriter.Close();
                }
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());

                //read the response:
                responseData = responseReader.ReadToEnd();
            }
            catch (WebException wex)
            {
                log.ErrorFormat("Cannot connect to {0} due to {1} ", serviceUrl, wex.Message);
                return string.Empty;
            }
            catch (IOException ioex)
            {
                log.ErrorFormat("Cannot read data from to {0} due to {1} ", serviceUrl, ioex.Message);
                return string.Empty;
            }
            finally
            {
                if (responseReader != null)
                    responseReader.Close();
            }

            return responseData;
        }


        /// <summary>
        /// Sends an HTTP request to the given service url and retrieve the response as byte array
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="data"></param>
        /// <param name="isPost">true to send a POST request, false to send a GET request</param>
        /// <returns>null if any error occurred, the server response otherwise</returns>
        public static byte[] GetHttpContent(string serviceUrl, string data, bool isPost)
        {
            // send out cookie along with a request for the protected page
            HttpWebRequest webRequest = null;
            try
            {
                webRequest = WebRequest.Create(serviceUrl) as HttpWebRequest;
            }
            catch (UriFormatException ufe)
            {
                log.ErrorFormat("Cannot connect to {0} due to {1} ", serviceUrl, ufe.Message);
                return null;
            }

            if (isPost)
            {
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                if (data != null)
                    webRequest.ContentLength = data.Length;
            }

            StreamReader responseReader = null;
            byte[] responseData = null;
            try
            {
                if (isPost)
                {
                    // write the form values into the request message
                    StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
                    requestWriter.Write(data);
                    requestWriter.Close();
                }
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());

                //read the response:
                responseData = ReadAllBytes(responseReader.BaseStream);
            }
            catch (WebException wex)
            {
                log.ErrorFormat("Cannot connect to {0} due to {1} ", serviceUrl, wex.Message);
                return null;
            }
            catch (IOException ioex)
            {
                log.ErrorFormat("Cannot read data from to {0} due to {1} ", serviceUrl, ioex.Message);
                return null;
            }
            finally
            {
                if (responseReader != null)
                    responseReader.Close();
            }

            return responseData;
        }


        /// <summary>
        /// Return the first "maxLines" of the input string.
        /// </summary>
        public static string GetFirstLines(this string input, int maxLines)
        {
            string output = string.Empty;
            string[] lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; (i<maxLines)&&(i<lines.Length); i++) {
                if ((i < maxLines - 1) && (i < lines.Length - 1))
                    output += lines[i] + Environment.NewLine;
                else
                    output += lines[i];
            }

            return output; 
        }


        /// <summary>
        /// Returns true if the given string is empty, only whitespaces, or \r or \n
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpaceOrEOF(this string input)
        {
            return (string.IsNullOrWhiteSpace(input) || input.Equals("\r") || input.Equals("\n") || input.Equals("\r\n") ||
                    input.Equals("\n\r"));
        }


        private const int PRIME = 5;

        /// <summary>
        /// Returns an hash value from the given input string
        /// </summary>
        /// <param name="input">string to hash</param>
        /// <returns> if the string is null or empty, the hash value otherwise</returns>
        public static int Hash(this string input)
        {
            //if (string.IsNullOrEmpty(input))
            //    return 0;
            //int result = PRIME; //a prime number
            //for (int i = 0; i < input.Length; i++)
            //{
            //    result += PRIME * input[i];
            //}
            //return result;

            if (string.IsNullOrEmpty(input))
                return 0;
            int hash = 0;
            for (int i = 0; i < input.Length; i++) 
            {
                hash = ((hash << PRIME) - hash) + input[i];
                hash = hash & hash; // Convert to 32bit integer
            }
            return hash;
        }


        /// <summary>
        /// Resamples an image to square, padding with transparent background
        /// </summary>
        /// <param name="originalImage"></param>
        /// <returns></returns>
        public static Image PadImage(Image originalImage)
        {
            int largestDimension = Math.Max(originalImage.Height, originalImage.Width);
            Size squareSize = new Size(largestDimension, largestDimension);
            Bitmap squareImage = new Bitmap(squareSize.Width, squareSize.Height);
            using (Graphics graphics = Graphics.FromImage(squareImage))
            {
                graphics.FillRectangle(Brushes.Transparent, 0, 0, squareSize.Width, squareSize.Height);
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                graphics.DrawImage(originalImage, 
                                    (squareSize.Width / 2) - (originalImage.Width / 2), 
                                    (squareSize.Height / 2) - (originalImage.Height / 2), 
                                    originalImage.Width, originalImage.Height);
            }
            return squareImage;
        }


        /// <summary>
        /// Resize a square image to the new given size
        /// </summary>
        /// <param name="fullsizeImage"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        public static Image ResizeSquareImage(Image fullsizeImage, int newSize)
        {
            if ((fullsizeImage == null) || (newSize <= 0))
                return null;

            // Prevent using images internal thumbnail
            fullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            fullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            return fullsizeImage.GetThumbnailImage(newSize, newSize, null, IntPtr.Zero);
        }


        /// <summary>
        /// Crops the input list with respect to the given pagination parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">the list to be paginated</param>
        /// <param name="maxNum">max number of items in output</param>
        /// <param name="start">first element to consider</param>
        /// <returns>an empty list if any error occurred; the paginated list otherwise</returns>
        public static IList<T> Paginate<T>(IList<T> input, int maxNum = int.MaxValue, int start = 0)
        {
            //perform the pagination, if required:
            if ((maxNum != int.MaxValue) || (start != 0))
            {
                int counter = 0;
                int buildCounter = 0;
                IList<T> resultPaged = new List<T>();
                foreach (T result in input)
                {
                    if ((counter >= start) && (buildCounter < maxNum))
                    {
                        resultPaged.Add(result);
                        buildCounter++;
                    }
                    else
                    {
                        if (buildCounter >= maxNum)
                            break;
                    }
                    counter++;
                }
                return resultPaged;
            }
            else
                return input;
        }

        /// <summary>
        /// Returns the number of milliseconds from Epoch
        /// </summary>
        /// <returns></returns>
        public static long UnixTimeNow()
        {
            TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }


        /// <summary>
        /// Returns the given string truncated to maxChars (with the addition of '...' ellipsis)
        /// </summary>
        /// <param name="input">stirng to truncate</param>
        /// <param name="maxChars">max number of allowed characters for the result</param>
        /// <returns></returns>
        public static string WithEllipsis(this string input, int maxChars)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return (input.Length > maxChars ? input.Substring(0, maxChars-3) + "..." : input);
        }


        /// <summary>
        /// Same as String.Replace but with case-insensitive matches
        /// </summary>
        /// <param name="original"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceEx(string original, string pattern, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) *
                      (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern,
                                              position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                    chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i)
                chars[count++] = original[i];
            return new string(chars, 0, count);
        }


        /// <summary>
        /// Same as String.Replace but with case-insensitive matches and search for the exact word.
        /// E.g. searching in "Java and Javascript with Java." the word 'Java', and replacing it with 'Python'
        /// results in "Python and Javascript with Python."
        /// </summary>
        /// <param name="stringToSearch"></param>
        /// <param name="find"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceExactWord(this string stringToSearch, string find, string replace)
        {
            string pattern = @"\b" + find + @"\b";
            return Regex.Replace(stringToSearch, pattern, replace, RegexOptions.IgnoreCase);
        }


        private static Lazy<Regex> s_ControlChars = new Lazy<Regex>(() => new Regex("[\x00-\x1f]", RegexOptions.Compiled));

        private static string FixDataReplace(Match match)
        {
            if ((match.Value.Equals("\t")) || (match.Value.Equals("\n")) || (match.Value.Equals("\r")))
                return match.Value;

            return "&#" + ((int)match.Value[0]).ToString("X4") + ";";
        }

        /// <summary>
        /// Escapes unprintable characters with UNICODE XML correct character
        /// </summary>
        /// <param name="data"></param>
        /// <param name="replacer"></param>
        /// <returns></returns>
        public static string FixXML(this object data, MatchEvaluator replacer = null)
        {
            if (data == null) 
                return string.Empty;
            if (replacer != null) 
                return s_ControlChars.Value.Replace(data.ToString(), replacer);
            else 
                return s_ControlChars.Value.Replace(data.ToString(), FixDataReplace);
        }


        /// <summary>
        /// Prints the number and the label together, detecting the plural / singular inflection.
        /// E.g. 10 Snippet(s)
        /// </summary>
        /// <param name="number">number of items to display</param>
        /// <param name="label">label to display, optionally with a final 's' if number != 1</param>
        /// <param name="printNum">true to actually print the number, false only to print the correct label</param>
        /// <returns></returns>
        public static string PrintNumAndLabel(long? number, string label, bool printNum = true)
        {
            string result = string.Empty;
            if (printNum)
            {
                result = "0";
                if (number.HasValue)
                    result = number.ToString();

                result += " ";
            }

            result += label;
            if (!(number.HasValue && number == 1))
                result += "s";
            return result;
        }


        /// <summary>
        /// Prints the english ordinal for the given number.
        /// E.g. 3rd place, 5th place, etc.
        /// </summary>
        /// <param name="number">the number to display</param>
        /// <returns></returns>
        public static string PrintEnglishOrdinal(long? number)
        {
            if (!number.HasValue)
                return string.Empty;

            switch (number % 10)
            {
                case 1: return number + "st";
                case 2: return number + "nd";
                case 3: return number + "rd";
                default: return number + "th";
            }
        }


        /// <summary>
        /// Returns the input string trimmed of spaces, checking its null-ness
        /// </summary>
        /// <param name="input"></param>
        /// <returns>the trimmed input string, empty string if it is null</returns>
        public static string CheckNullAndTrim(this string input)
        {
            return (input == null ? string.Empty : input.Trim());
        }


        public static long LongRandom(long min, long max, Random rand)
        {
            long result = rand.Next((Int32)(min >> 32), (Int32)(max >> 32));
            result = (result << 32);
            result = result | (long)rand.Next((Int32)min, (Int32)max);
            return result;
        }

        public static long CurrentTimeInMillis()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Returns all cokkies inside a CookieContainer
        /// </summary>
        /// <param name="cookieJar"></param>
        /// <returns></returns>
        public static CookieCollection GetAllCookies(CookieContainer cookieJar)
        {
            CookieCollection cookieCollection = new CookieCollection();

            Hashtable table = (Hashtable)cookieJar.GetType().InvokeMember("m_domainTable",
                                                                            BindingFlags.NonPublic |
                                                                            BindingFlags.GetField |
                                                                            BindingFlags.Instance,
                                                                            null,
                                                                            cookieJar,
                                                                            new object[] { });

            foreach (var tableKey in table.Keys)
            {
                String str_tableKey = (string)tableKey;

                if (str_tableKey[0] == '.')
                {
                    str_tableKey = str_tableKey.Substring(1);
                }

                SortedList list = (SortedList)table[tableKey].GetType().InvokeMember("m_list",
                                                                            BindingFlags.NonPublic |
                                                                            BindingFlags.GetField |
                                                                            BindingFlags.Instance,
                                                                            null,
                                                                            table[tableKey],
                                                                            new object[] { });

                foreach (var listKey in list.Keys)
                {
                    String url = "https://" + str_tableKey + (string)listKey;
                    cookieCollection.Add(cookieJar.GetCookies(new Uri(url)));
                }
            }

            return cookieCollection;
        }


        #region ARCHITECTURE AND OS
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This is the obsolete detection method
        /// </summary>
        /// <returns></returns>
        public static int getOSArchitecture()
        {
            string pa = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            return ((String.IsNullOrEmpty(pa) || String.Compare(pa, 0, "x86", 0, 3, true) == 0) ? 32 : 64);
        }


        /// <summary>
        /// Detects Windows Version
        /// </summary>
        /// <returns></returns>
        public static string getOSInfo()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "ME";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else if (vs.Minor == 1)
                            operatingSystem = "7";
                        else
                            operatingSystem = "8";
                        break;
                    default:
                        break;
                }
            }
            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.
            if (operatingSystem != "")
            {
                //Got something.  Let's prepend "Windows" and get more info.
                operatingSystem = DefaultProperty.WindowsOS + operatingSystem;
                ////See if there's a service pack installed.
                //if (os.ServicePack != "")
                //{
                //    //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                //    operatingSystem += " " + os.ServicePack;
                //}
                ////Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                //operatingSystem += " " + getOSArchitecture() + "-bit";
            }
            //Return the information we've gathered.
            return operatingSystem;
        }


        /// <summary>
        /// The function determines whether the current operating system is a 
        /// 64-bit operating system.
        /// </summary>
        /// <returns>
        /// The function returns true if the operating system is 64-bit; 
        /// otherwise, it returns false.
        /// </returns>
        public static bool Is64BitOperatingSystem()
        {
            if (IntPtr.Size == 8)  // 64-bit programs run only on Win64
            {
                return true;
            }
            else  // 32-bit programs run on both 32-bit and 64-bit Windows
            {
                // Detect whether the current process is a 32-bit process 
                // running on a 64-bit system.
                bool flag;
                return ((DoesWin32MethodExist("kernel32.dll", "IsWow64Process") &&
                    IsWow64Process(GetCurrentProcess(), out flag)) && flag);
            }
        }

        /// <summary>
        /// The function determins whether a method exists in the export 
        /// table of a certain module.
        /// </summary>
        /// <param name="moduleName">The name of the module</param>
        /// <param name="methodName">The name of the method</param>
        /// <returns>
        /// The function returns true if the method specified by methodName 
        /// exists in the export table of the module specified by moduleName.
        /// </returns>
        static bool DoesWin32MethodExist(string moduleName, string methodName)
        {
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            if (moduleHandle == IntPtr.Zero)
            {
                return false;
            }
            return (GetProcAddress(moduleHandle, methodName) != IntPtr.Zero);
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)]string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

        /// <summary>
        /// The function determines whether the operating system of the 
        /// current machine of any remote machine is a 64-bit operating 
        /// system through Windows Management Instrumentation (WMI).
        /// </summary>
        /// <param name="machineName">
        /// The full computer name or IP address of the target machine. "." 
        /// or null means the local machine.
        /// </param>
        /// <param name="domain">
        /// NTLM domain name. If the parameter is null, NTLM authentication 
        /// will be used and the NTLM domain of the current user will be used.
        /// </param>
        /// <param name="userName">
        /// The user name to be used for the connection operation. If the 
        /// user name is from a domain other than the current domain, the 
        /// string may contain the domain name and user name, separated by a 
        /// backslash: string 'username' = "DomainName\\UserName". If the 
        /// parameter is null, the connection will use the currently logged-
        /// on user
        /// </param>
        /// <param name="password">
        /// The password for the specified user.
        /// </param>
        /// <returns>
        /// The function returns true if the operating system is 64-bit; 
        /// otherwise, it returns false.
        /// </returns>
        /// <exception cref="System.Management.ManagementException">
        /// The ManagementException exception is generally thrown with the  
        /// error code: System.Management.ManagementStatus.InvalidParameter.
        /// You need to check whether the parameters for ConnectionOptions 
        /// (e.g. user name, password, domain) are set correctly.
        /// </exception>
        /// <exception cref="System.Runtime.InteropServices.COMException">
        /// A common error accompanied with the COMException is "The RPC 
        /// server is unavailable. (Exception from HRESULT: 0x800706BA)". 
        /// This is usually caused by the firewall on the target machine that 
        /// blocks the WMI connection or some network problem.
        /// </exception>
        public static bool Is64BitOperatingSystem(string machineName,
            string domain, string userName, string password)
        {
            ConnectionOptions options = null;
            if (!string.IsNullOrEmpty(userName))
            {
                // Build a ConnectionOptions object for the remote connection 
                // if you plan to connect to the remote with a different user 
                // name and password than the one you are currently using.
                options = new ConnectionOptions();
                options.Username = userName;
                options.Password = password;
                options.Authority = "NTLMDOMAIN:" + domain;
            }
            // Else the connection will use the currently logged-on user

            // Make a connection to the target computer.
            ManagementScope scope = new ManagementScope("\\\\" +
                (string.IsNullOrEmpty(machineName) ? "." : machineName) +
                "\\root\\cimv2", options);
            scope.Connect();

            // Query Win32_Processor.AddressWidth which dicates the current 
            // operating mode of the processor (on a 32-bit OS, it would be 
            // "32"; on a 64-bit OS, it would be "64").
            // Note: Win32_Processor.DataWidth indicates the capability of 
            // the processor. On a 64-bit processor, it is "64".
            // Note: Win32_OperatingSystem.OSArchitecture tells the bitness
            // of OS too. On a 32-bit OS, it would be "32-bit". However, it 
            // is only available on Windows Vista and newer OS.
            ObjectQuery query = new ObjectQuery(
                "SELECT AddressWidth FROM Win32_Processor");

            // Perform the query and get the result.
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject queryObj in queryCollection)
            {
                if (queryObj["AddressWidth"].ToString() == "64")
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////
    }
}
