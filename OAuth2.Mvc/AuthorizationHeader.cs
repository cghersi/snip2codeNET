using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OAuth2.Mvc
{
    public class AuthorizationHeader
    {
        public AuthorizationHeader()
        {
            Parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public AuthorizationHeader(string headerText)
            : this()
        {
            Parse(headerText);
        }

        public string Scheme { get; set; }
        public IDictionary<string, string> Parameters { get; private set; }

        private string _parameterText;

        public string ParameterText
        {
            get { return _parameterText; }
            set { _parameterText = value; }
        }

        public void Parse(string header)
        {
            var buffer = new StringBuilder();
            var reader = new StringReader(header);
            var parameterText = new StringBuilder();

            var withinQuotes = false;

            do
            {
                char c = (char)reader.Read();

                if (!string.IsNullOrEmpty(Scheme))
                    parameterText.Append(c);

                if (c == '"')
                {
                    withinQuotes = !withinQuotes;
                    continue;
                }

                if (char.IsWhiteSpace(c) && buffer.Length == 0 && !withinQuotes)
                    continue;

                if ((c == ',' || char.IsWhiteSpace(c)) && buffer.Length > 0 && !withinQuotes)
                {
                    // end of token                    
                    ReadToken(buffer);
                    continue;
                }

                buffer.Append(c);
            }
            while (reader.Peek() != -1);

            ReadToken(buffer);

            _parameterText = parameterText.ToString();
        }

        private void ReadToken(StringBuilder buffer)
        {
            if (buffer.Length == 0)
                return;

            var text = buffer.ToString();
            if (string.IsNullOrEmpty(Scheme))
            {
                Scheme = text.Trim();
                buffer.Length = 0;
                return;
            }

            var parts = text.Split('=');
            if (parts.Length == 2)
                Parameters[parts[0]] = parts[1];

            buffer.Length = 0;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append(Scheme);
            buffer.Append(" ");

            bool haveParam = false;
            foreach (var p in Parameters)
            {
                if (haveParam)
                    buffer.Append(",");

                buffer.Append(p.Key);
                buffer.Append("=");
                buffer.Append("\"");
                buffer.Append(p.Value);
                buffer.Append("\"");

                haveParam = true;
            }

            if (!haveParam && !string.IsNullOrEmpty(ParameterText))
                buffer.Append(ParameterText);

            return buffer.ToString();
        }
    }
}