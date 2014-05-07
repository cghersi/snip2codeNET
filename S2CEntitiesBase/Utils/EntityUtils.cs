//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Snip2Code.Model.Entities;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Snip2Code.Utils
{
    public enum SerialFormat
    {
        JSON,
        XML,
        HTML
    }


    /// <summary>
    /// This class makes available a number of general utility methods for Entities classes,
    /// usually in the form of extension methods
    /// </summary>
    public static class EntityUtils
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public const string JSON_MIMETYPE = "application/json";
        public const string XML_MIMETYPE = "text/xml";
        public const string HTML_MIMETYPE = "application/xhtml+xml"; //"text/html";

        public const string S2C_HEADER = "S2cHead";
        public const string S2C_HEADER_VS = "VS";
        public const string S2C_HEADER_ECLIPSE = "Ecl";
        public const string S2C_HEADER_INTELLIJ = "Int";
        public const string S2C_HEADER_METRO = "Metro";
        public const string S2C_HEADER_MAC = "MacOSX";
        public const string S2C_HEADER_NPP = "Npp";

        public const string TOT_NUM_WITH_NON_DEF_OP = "TOT_NUM_WITH_NON_DEF_OP";

        public static char[] SymbolAsLetterForCompilation = new char[] {'+', '-', '/', '*', '~', '@', '#', '%', '^',
            ',', '.', ':', ';', '_', '$', '€', '£', '&', '?', '|', '\\', '\'', '§', '°', 'ç', 'ì', 'è', 'é', 'ò', 'à', 'ù',    
            '{', '}', '[', ']'};


        /// <summary>
        ///     Build a tag list from the given string. Uses common separator to
        ///     distinguish each tag
        /// </summary>
        /// ------------------------------------------------------------------------------------------
        public static SortedList<string, Tag> BuildTagList(this string tags)
        {
            SortedList<string, Tag> tagList = new SortedList<string, Tag>();

            if (string.IsNullOrEmpty(tags))
                return tagList;

            //split tags also by comma(',') and semi-colon(';'):
            tags = tags.Replace(',', ' ').Replace(';', ' ');

            // 1 - Tokenize the list of incoming tags:
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            StringTokenizer tok = new StringTokenizer(tags);
            tok.IgnoreWhiteSpace = true;
            tok.TokenSeparatedBySpace = true;
            tok.IgnoreDigits = true;
            tok.IgnoreEqualChar = true;
            tok.SymbolAsLetter = SymbolAsLetterForCompilation;
            List<Token> tokens = tok.Tokens;

            // 2 - Parse the tokens:
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            foreach (Token token in tokens)
            {
                if ((token == null) || (token.Kind == TokenKind.EOL) || (token.Kind == TokenKind.EOF))
                    continue;

                string tokValue = token.Value.Trim().ToLower();

                // 2.1 - Skip blacklisted tags:
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                if (Utilities.SkipKeywords.Contains(tokValue))
                    continue;

                // 2.2 - Allow tags of less than 3 chars only if quoted:
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                if ((tokValue.Length <= 2) && (token.Kind != TokenKind.QuotedString))
                    continue;

                // 2.3 - Trim tags of more than 50 chars:
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                if (tokValue.Length >= 48)
                    tokValue = tokValue.Substring(0, 47);

                // 2.4 - Skip duplicated tags:
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                if (tagList.ContainsKey(tokValue))
                    continue;

                tagList.Add(tokValue, new Tag(tokValue));
            }

            return tagList;
        }


        public static string FormatS2CTag(this string inputTag)
        {
            if (string.IsNullOrEmpty(inputTag))
                return string.Empty;
            return PrintWithQuotes(inputTag);
        }


        /// <summary>
        /// Removes the duplicates from the given string
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static string RemoveDuplicates(this string tags)
        {
            string result = string.Empty;
            SortedList<string, Tag> tagList = tags.BuildTagList();
            foreach (String key in tagList.Keys)
            {
                result = string.Format("{0} {1}", result, FormatS2CTag(key));
            }
            return result;
        }


        /// <summary>
        /// Returns the given input between quotes ('"') if it contains spaces
        /// </summary>
        /// <param name="input">string to manage</param>
        /// <returns>the quoted string if it contains spaces, the string as is otherwise</returns>
        public static string PrintWithQuotes(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            if (input.IndexOfAny(Utilities.s_separators) != -1)
                return string.Format("\"{0}\"", input);
            else
                return input;
        }


        /// <summary>
        /// Splits the given input string in words delimited by spaces or the following simbols:
        /// '+', '-', '/', '*', '~', '@', '#', '%', '^', ',', '.', ':', ';', '_', '$', '€', '£', '&amp;', '?', '|', '\\', 
        /// '\'', '§', '°', 'ç', 'ì', 'è', 'é', 'ò', 'à', 'ù', '{', '}', '[', ']'
        /// </summary>
        /// <param name="s">string to be splitted</param>
        /// <returns>the list of splitted words; an empty list if any error occurred</returns>
        public static List<string> SplitWords(this string s)
        {
            List<string> wordsList = new List<string>();

            if (string.IsNullOrWhiteSpace(s))
                return wordsList;

            // Tokenize the list of incoming words
            StringTokenizer tok = new StringTokenizer(s);
            tok.IgnoreWhiteSpace = true;
            tok.TokenSeparatedBySpace = true;
            tok.IgnoreDigits = true;
            tok.IgnoreEqualChar = true;
            tok.SymbolAsLetter = SymbolAsLetterForCompilation;
            List<Token> tokens = tok.Tokens;

            // Add the tokens
            foreach (Token token in tokens)
            {
                if ((token == null) || (token.Kind == TokenKind.EOL) || (token.Kind == TokenKind.EOF))
                    continue;

                string tokValue = token.Value.Trim().ToLower();

                if (token.Kind == TokenKind.QuotedString)
                    tokValue = string.Format("\"{0}\"", tokValue);

                wordsList.Add(tokValue);
            }

            return wordsList;
        }


        /// <summary>
        /// Initializes the helper object for the serialization of content in JSON
        /// </summary>
        /// <returns>the helper object correctly initialized</returns>
        public static fastJSON.JSON InitJSONUtils()
        {
            fastJSON.JSON res = fastJSON.JSON.Instance;
            res.SerializeNullValues = true;
            res.ShowReadOnlyProperties = true;
            res.UseUTCDateTime = true;
            res.IndentOutput = false;
            res.UsingGlobalTypes = true;
            res.UseSerializerExtension = false;
            return res;
        }


        /// <summary>
        /// Retrieves the MIME type associated to the given format
        /// </summary>
        /// <param name="format">format which the MIME type should be taken for</param>
        /// <returns></returns>
        public static string GetMIMEType(this SerialFormat format)
        {
            switch (format)
            {
                case SerialFormat.JSON:
                    return JSON_MIMETYPE;
                case SerialFormat.HTML:
                    return HTML_MIMETYPE;
                case SerialFormat.XML:
                    return XML_MIMETYPE;
            }
            return string.Empty;
        }


        /// <summary>
        /// Builds a list of BaseEntities from the serialized given object
        /// </summary>
        /// <typeparam name="T">subtype of BaseEntity</typeparam>
        /// <param name="resp">serialized object which the list should be retrieved from</param>
        /// <returns>an empty list if any error occurred; the parsed list otherwise</returns>
        public static List<U> GetListFromResp<T, U>(this S2CResListBaseEntity<T> resp, out int totNum)
            where T : BaseEntity
            where U : BaseEntity
        {
            totNum = 0;
            List<U> result = new List<U>(); 
            if ((resp == null) || (resp.Data == null) || (!(resp.Data is object[])))
                return result;

            object[] respData = (object[])resp.Data;
            totNum = resp.TotNum;
            foreach (object obj in respData)
            {
                T typedObj = (T)obj;
                result.Add((U)typedObj.ToBaseEntity());
            }

            return result;
        }


        /// <summary>
        /// Retrieves the Misc field (if available) from the serialized given object
        /// </summary>
        /// <typeparam name="T">subtype of BaseEntity</typeparam>
        /// <param name="resp">serialized object which the miscellaneous should be retrieved from</param>
        /// <returns>null if any error occurred; the miscellaneous field otherwise</returns>
        public static Dictionary<string, string[]> GetMiscFromResp<T>(this S2CResListBaseEntity<T> resp, out int totNumWithNonDefOp)
            where T : BaseEntity
        {
            totNumWithNonDefOp = 0;
            if ((resp == null) || (resp.Misc == null))
                return null;
            else
            {
                Dictionary<string, string[]> res = new Dictionary<string, string[]>();
                foreach (string key in resp.Misc.Keys)
                {
                    if (string.IsNullOrEmpty(key) || (resp.Misc[key] == null))
                        continue;
                    if (key.Equals(TOT_NUM_WITH_NON_DEF_OP, StringComparison.InvariantCultureIgnoreCase))
                        int.TryParse(resp.Misc[key], out totNumWithNonDefOp);
                    else
                        res.Add(key, resp.Misc[key].Split(','));
                }
                return res;
            }
        }


        /// <summary>
        /// Builds a list of longs from the serialized given object
        /// </summary>
        /// <typeparam name="T">subtype of BaseEntity</typeparam>
        /// <param name="resp">serialized object which the list should be retrieved from</param>
        /// <returns>an empty list if any error occurred; the parsed list otherwise</returns>
        public static List<long> GetListOfLongsFromResp(this S2CResListObj<string> resp, out int totNum)
        {
            totNum = 0;
            if ((resp == null) || (resp.Status != ErrorCodes.OK) || (resp.Data == null) || (!(resp.Data is string[])))
                return new List<long>();

            List<long> result = new List<long>();
            totNum = resp.TotNum;
            object[] respData = (object[])resp.Data;
            foreach (string obj in respData)
            {
                if (!string.IsNullOrEmpty(obj))
                    result.Add(long.Parse(obj));
            }

            return result;
        }


        /// <summary>
        /// Builds a list of int from the serialized given object
        /// </summary>
        /// <typeparam name="T">subtype of BaseEntity</typeparam>
        /// <param name="resp">serialized object which the list should be retrieved from</param>
        /// <returns>an empty list if any error occurred; the parsed list otherwise</returns>
        public static List<int> GetListOfIntFromResp(this S2CResListObj<string> resp, out int totNum)
        {
            totNum = 0;
            if ((resp == null) || (resp.Status != ErrorCodes.OK) || (resp.Data == null) || (!(resp.Data is string[])))
                return new List<int>();

            List<int> result = new List<int>();
            totNum = resp.TotNum;
            object[] respData = (object[])resp.Data;
            foreach (string obj in respData)
            {
                if (!string.IsNullOrEmpty(obj))
                    result.Add(int.Parse(obj));
            }

            return result;
        }


        /// <summary>
        /// Removes the starting tabs or spaces from each line of the given text:
        /// first of all this methods computes the common number of tabs or spaces for all the single lines of the
        /// given input text; then trims the common number of such characters, if needed. 
        /// </summary>
        /// <param name="text">text to trim</param>
        /// <returns>the trimmed text</returns>
        public static string TrimStartingTabsOrSpaces(string text) 
        {
		    if (string.IsNullOrWhiteSpace(text))
			    return text;
		
		    //split into lines:
            log.DebugFormat("Splitting: {0}", text);
            int posOfSlashN = text.IndexOf(LINE_SEPARATOR);
            if (posOfSlashN < 0)
            {
                //cannot find line separator, check if at least '\r' is found:
                log.Debug("Cannot find \\n, trying to find \\r");
                int posOfSlashR = text.IndexOf('\r');
                if (posOfSlashR >= 0)
                {
                    log.Debug("Found \\r, replacing with line correct separator");
                    text = text.Replace("\r", "\r\n");
                }
            }

		    string[] lines = text.Split(LINE_SEPARATOR);
            log.DebugFormat("found {0} lines", lines.Length);

		    //count the number of tabs and spaces at the start of each line:
		    int numOfTabs = int.MaxValue;
            int numOfSpaces = int.MaxValue;
		    foreach (string line in lines) 
            {
                if (line.IsNullOrWhiteSpaceOrEOF())
                    continue;

			    //check the number of tabs in the current line:
			    int curNumOfTabs = CountStartingChars(line, '\t');
			
			    //refresh the minimum number of tabs of the entire text:
			    if (curNumOfTabs < numOfTabs)
				    numOfTabs = curNumOfTabs;
			
			    //check the number of spaces in the current line:
			    int curNumOfSpaces = CountStartingChars(line, ' ');
			
			    //refresh the minimum number of spaces of the entire text:
			    if (curNumOfSpaces < numOfSpaces)
				    numOfSpaces = curNumOfSpaces;			
			
			    if ((numOfTabs == 0) && (numOfSpaces == 0))
				    break;
		    }
		
		    //if a positive number of tabs have been found in all the lines, 
		    //trim all the lines of the actual minimum common number of tabs:
		    if (numOfTabs > 0) 
			    return TrimStartingChar(lines, numOfTabs);
		
		    //if a positive number of spaces have been found in all the lines, 
		    //trim all the lines of the actual minimum common number of spaces:
		    if (numOfSpaces > 0) 
			    return TrimStartingChar(lines, numOfSpaces);
		
		    //otherwise, leave the given input untouched:
		    return text;
	    }
        private static char LINE_SEPARATOR = '\n';
        private static int CountStartingChars(string line, char c)
        {
            if (string.IsNullOrWhiteSpace(line))
                return 0;
            for (int pos = 0; pos < line.Length; pos++)
            {
                if (line[pos] != c)
                    return pos;
            }
            return line.Length;
        }
        private static string TrimStartingChar(string[] lines, int numOfCharsToTrim) 
        {
		    StringBuilder str = new StringBuilder();
		    foreach (string line in lines) 
            {
                if (line.IsNullOrWhiteSpaceOrEOF())
                    str.Append(line);
                else
                {
                    str.Append(line.Substring(numOfCharsToTrim));
                    log.DebugFormat("end of line: {0} {1}", PrintChar(line[line.Length - 2]), PrintChar(line[line.Length - 1]));
                }
                str.Append(LINE_SEPARATOR);
		    }
            return str.ToString();
	    }
        private static char PrintChar(char c)
        {
            switch (c)
            {
                case '\r': return 'r';
                case '\n': return 'n';
                default: return c;
            }
        }


        /// <summary>
        /// Removes (if present) the wrapping double quotes from the given string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveWrappingQuotes(this string input)
        {
            if (input != null)
            {
                if (input.StartsWith("\""))
                    input = input.TrimStart('"');
                if (input.EndsWith("\""))
                    input = input.TrimEnd('"');
            }
            return input;
        }


        /// <summary>
        /// Returns an hash (computed with MD5 algorithm) of the given string
        /// </summary>
        /// <param name="input">string to hash</param>
        /// <param name="printLowerCase">true to print the hexadecimal form in lower case</param>
        /// <returns></returns>
        public static string ComputeMD5Hash(this string input, bool printLowerCase = false, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.ASCII;
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] binaryInput = encoding.GetBytes(input);
            byte[] hashOfInput = md5.ComputeHash(binaryInput);
            return hashOfInput.PrintHexByteArray(printLowerCase);
        }


        /// <summary>
        /// Returns an hash (computed with MD5 algorithm) of the given string
        /// </summary>
        /// <param name="input">string to hash</param>
        /// <param name="printLowerCase">true to print the hexadecimal form in lower case</param>
        /// <returns></returns>
        public static string ComputeMD5HashSQLServerCompliant(this string password, bool printLowerCase = false)
        {
            if (password.IsNullOrWhiteSpaceOrEOF())
                password = string.Empty;
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] binaryInput = Encoding.Unicode.GetBytes(password);
            byte[] hashOfInput = md5.ComputeHash(binaryInput);
            return hashOfInput.PrintHexByteArray(printLowerCase);
        }


        /// <summary>
        /// Returns a string representation of input byte array as hexadecimal digits
        /// </summary>
        /// <param name="input">byte array to print</param>
        /// <param name="printLowerCase">true to print the hexadecimal form in lower case</param>
        /// <returns>string.Empty if input is null, the hexadecimal representation otherwise</returns>
        public static string PrintHexByteArray(this byte[] input, bool printLowerCase = false)
        {
            if (input == null)
                return string.Empty;
            StringBuilder sbs = new StringBuilder();
            string format = (printLowerCase ? "x2" : "X2");
            for (int i = 0; i < input.Length; i++)
            {
                sbs.Append(input[i].ToString(format));
            }
            return sbs.ToString();
        }

        private static BadgeComparer s_badgeComparer = new BadgeComparer();

        /// <summary>
        /// Retrieves just only the latest badges of each type from the complete list of badges 
        /// of the given object
        /// </summary>
        /// <param name="badgeable"></param>
        /// <returns></returns>
        public static List<Badge> GetLatestBadges(this IBadgeable badgeable)
        {
            List<Badge> res = new List<Badge>();
            if (badgeable.Badges.IsNullOrEmpty())
                return res;

            //sort by ID and then by timestamp:
            List<Badge> allbadges = badgeable.Badges;
            allbadges.Sort(s_badgeComparer);

            short lastBadgeType = -1;
            foreach (Badge b in allbadges)
            {
                //if the type is the same, skip it as we want only the very last one:
                if (lastBadgeType != b.TypeAsNumber)
                {
                    res.Add(b);
                }
                lastBadgeType = b.TypeAsNumber;
            }

            return res;
        }

        /// <summary>
        /// Returns true if the given object already has the given badge.
        /// The comparison is made on the badge ID
        /// </summary>
        /// <param name="badgeable"></param>
        /// <param name="badgeID"></param>
        /// <returns></returns>
        public static bool HasBadge(this IBadgeable badgeable, short badgeID)
        {
            if (badgeable == null)
                return false;
            List<Badge> availBadges = badgeable.Badges;
            if (availBadges.IsNullOrEmpty())
                return false;

            foreach (Badge badge in availBadges)
            {
                if (badge.ID == badgeID)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the actual badge that matches the given id, if available.
        /// The comparison is made on the badge ID
        /// </summary>
        /// <param name="badgeable"></param>
        /// <param name="badgeID"></param>
        /// <returns>null if not available</returns>
        public static Badge RetrieveBadge(this IBadgeable badgeable, short badgeID)
        {
            if (badgeable == null)
                return null;
            List<Badge> availBadges = badgeable.Badges;
            if (availBadges.IsNullOrEmpty())
                return null;

            foreach (Badge badge in availBadges)
            {
                if (badge.ID == badgeID)
                    return badge;
            }

            return null;
        }


        /// <summary>
        /// Returns the code specified between startLine and endLine.
        /// The split of lines is done using both '\r' and '\n'.
        /// </summary>
        /// <param name="code">actual code to chop and return</param>
        /// <param name="startLine">starting line (included) of code to return. First row is 1. Specify 1 to start from the very beginning</param>
        /// <param name="endLine">ending line (included) of code to return. Specify int.MaxValue to return code until the end of the snippet is reached</param>
        /// <returns>The code specified between startLine and endLine</returns>
        public static string GetCodeInLines(string code, int startLine = 1, int endLine = int.MaxValue)
        {
            if (endLine < startLine)
                endLine = int.MaxValue;
            if (startLine < 1)
                startLine = 1;
            if ((startLine > 1) || (endLine < int.MaxValue))
            {
                //split into lines:
                string[] lines = Regex.Split(code, "\r\n|\r|\n");
                if (lines.IsNullOrEmpty())
                    return code;

                //cap the end line to the actual max length of the code:
                if (endLine > lines.Length)
                    endLine = lines.Length;

                StringBuilder res = new StringBuilder();
                for (int actualLine = startLine - 1; actualLine < endLine; actualLine++)
                {
                    res.AppendLine(lines[actualLine]);
                }
                return res.ToString();
            }

            return code;
        }
    }
}
