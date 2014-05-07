//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;

namespace Snip2Code.Utils
{
    public enum TokenKind
    {
        Unknown,
        Word,
        Number,
        QuotedString,
        WhiteSpace,
        Symbol,
        EOL,
        EOF,
        BinaryOp,
        UnaryOp,
        MetaValueOp,
        OpenBrace,
        ClosedBrace
    }


    /// <summary>
    /// Token is the single element in which is broken a string
    /// </summary>
    public class Token
    {
        int line;
        int column;
        string value;
        TokenKind kind;

        public Token(TokenKind kind, string value, int line, int column)
        {
            this.kind = kind;

            //if the token is a QuotedString, remove quotes from the value:
            if (kind == TokenKind.QuotedString)
            {
                if (value[0] == '"' && (value.Length >= 2))
                {
                    int len = value.Length - 2;
                    //particular case when the quoted string doesn't end with '"' (e.g. "san francisco)
                    if (value[value.Length - 1] != '"')
                        len++;
                    this.value = value.Substring(1, len);
                }
                else
                    this.value = value;
            }
            else
                this.value = value;

            this.line = line;
            this.column = column;
        }

        public Token(TokenKind kind, string value) : this (kind, value, 0, 0)
        {
        }

        public int Column
        {
            get { return this.column; }
        }

        public TokenKind Kind
        {
            get { return this.kind; }
            set { kind = value; }
        }

        public int Line
        {
            get { return this.line; }
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public override string ToString()
        {
            string val = value;
            if (kind == TokenKind.QuotedString)
                val = string.Format("\"{0}\"", val);

            return string.Format("value:{0} ;kind:{1}", val, kind);
        }
    }
}
