//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Snip2Code.Utils
{
    /// <summary>
    /// StringTokenizer tokenizes string (or stream) into a list of Tokens, inserting a EOF,EOL Token at the end of the list.
    /// </summary>
    public class StringTokenizer
    {
        const char EOF = (char)0;

        int line;
        int column;
        int pos;	// position within data

        string data;

        List<Token> tokens;

        bool ignoreWhiteSpace;
        bool ignoreEqualChar;
        bool ignoreDigits;
        bool tokenSeparatedBySpace;
        char[] symbolChars;
        char[] symbolAsLetter;  //array of symbols treated as common letters inside words

        int saveLine;
        int saveCol;
        int savePos;

        bool tokenListCreated = false;

        public StringTokenizer(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            data = reader.ReadToEnd();

            Reset();
        }

        public StringTokenizer(string data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            this.data = data;

            Reset();
        }


        /// <summary>
        /// gets the list of tokens representing the input
        /// </summary>
        public List<Token> Tokens
        {
            get
            {
                if (!tokenListCreated)
                    this.CreateArrayList();
                return this.tokens;
            }
        }


        /// <summary>
        /// gets or sets which characters are part of TokenKind.Symbol
        /// </summary>
        public char[] SymbolChars
        {
            get { return this.symbolChars; }
            set { this.symbolChars = value; }
        }


        /// <summary>
        /// gets or sets which special characters (symbols) are treated as common letters inside words.
        /// Default is empty list.
        /// </summary>
        public char[] SymbolAsLetter
        {
            get { return this.symbolAsLetter; }
            set { this.symbolAsLetter = value; }
        }


        /// <summary>
        /// if set to true, white space characters will be ignored,
        /// but EOL and whitespace inside of string will still be tokenized.
        /// Default is false.
        /// </summary>
        public bool IgnoreWhiteSpace
        {
            get { return this.ignoreWhiteSpace; }
            set { this.ignoreWhiteSpace = value; }
        }

        /// <summary>
        /// if set to true, '=' character will be ignored,
        /// but EOL and whitespace inside of string will still be tokenized.
        /// Default is false.
        /// </summary>
        public bool IgnoreEqualChar
        {
            get { return this.ignoreEqualChar; }
            set { this.ignoreEqualChar = value; }
        }

        /// <summary>
        /// if set to true, digits are treated as normal characters, like literals.
        /// Default is false.
        /// </summary>
        public bool IgnoreDigits
        {
            get { return this.ignoreDigits; }
            set { this.ignoreDigits = value; }
        }

        /// <summary>
        /// if set to true, tokens must be separated by separators (spaces, tabs, etc).
        /// Default is false.
        /// </summary>
        public bool TokenSeparatedBySpace
        {
            get { return this.tokenSeparatedBySpace; }
            set { this.tokenSeparatedBySpace = value; }
        }

        private void Reset()
        {
            this.ignoreWhiteSpace = false;
            this.ignoreEqualChar = false;
            this.ignoreDigits = false;
            this.tokenSeparatedBySpace = false;
            this.symbolChars = new char[]{  '+', '-', '/', '*', '~', '@', '#', '%', '^',
                                            '=', '<', '>', '!',
                                            ',', '.', ':', ';', '_',
                                            '$', '€', '£', '&', '?', '|', '\\', '\'', '§', '°', 
                                            'ç', 'ì', 'è', 'é', 'ò', 'à', 'ù',    
                                            '(', ')', '{', '}', '[', ']'};
            this.symbolAsLetter = new char[] { };

            this.tokens = new List<Token>();

            line = 1;
            column = 1;
            pos = 0;
            tokenListCreated = false;
        }

        protected char LA(int count)
        {
            if (pos + count >= data.Length)
                return EOF;
            else
                return data[pos + count];
        }

        protected char Consume()
        {
            char ret = data[pos];
            pos++;
            column++;

            return ret;
        }

        protected Token CreateToken(TokenKind kind, string value)
        {
            Token tmp = new Token(kind, value, line, column);
            tokens.Add(tmp);
            return tmp;
        }

        protected Token CreateToken(TokenKind kind)
        {
            string tokenData = data.Substring(savePos, pos - savePos);
            Token tmp = new Token(kind, tokenData, saveLine, saveCol);
            tokens.Add(tmp);
            return tmp;
        }

        /* populate the arraylist. */
        protected void CreateArrayList()
        {
            //initialization of the positions for the scrolling of the input string:
            line = 1;
            column = 1;
            pos = 0;
            tokens.Clear();

            //sliding input in order to fill the arraylist:
            Token tmp = Next();
            while ((tmp.Kind != TokenKind.EOF) && (tmp.Kind != TokenKind.EOL))
                tmp = Next();

            tokenListCreated = true;
        }

        public Token Next()
        {
        ReadToken:

            char ch = LA(0);
            switch (ch)
            {
                case EOF:
                    return CreateToken(TokenKind.EOF, string.Empty);

                case ' ':
                case '\t':
                    {
                        if (this.ignoreWhiteSpace)
                        {
                            Consume();
                            goto ReadToken;
                        }
                        else
                            return ReadWhitespace();
                    }

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    if (ignoreDigits)
                        return ReadWord();
                    else
                        return ReadNumber();

                case '\r':
                    {
                        StartRead();
                        Consume();
                        if (LA(0) == '\n')
                            Consume();	// on DOS/Windows we have \r\n for new line

                        line++;
                        column = 1;

                        return CreateToken(TokenKind.EOL);
                    }
                case '\n':
                    {
                        StartRead();
                        Consume();
                        line++;
                        column = 1;

                        return CreateToken(TokenKind.EOL);
                    }

                case '"':
                    {
                        return ReadString();
                    }

                default:
                    {
                        if (Char.IsLetter(ch) || (ignoreDigits && Char.IsDigit(ch)) ||
                            (ch == '_') || IsSymbolAsLetter(ch))
                            return ReadWord();
                        else if (IsSymbol(ch))
                        {
                            string symbol = new string(ch, 1);
                            char firstChar = ch;

                            StartRead();
                            Consume();
                            ch = LA(0);

                            //case of symbols composed by more than 1 char:
                            //symbols: "<>" or "!=" or ">=" or "<=":
                            switch (firstChar)
                            {
                                case '<':
                                    if ((ch == '>') || (ch == '='))
                                    {
                                        StartRead();
                                        Consume();
                                        symbol = string.Format("{0}{1}", symbol, ch);
                                    }
                                    break;

                                case '>':
                                    if ((ch == '<') || (ch == '='))
                                    {
                                        StartRead();
                                        Consume();
                                        symbol = string.Format("{0}{1}", symbol, ch);
                                    }
                                    break;

                                case '!':
                                    if (ch == '=')
                                    {
                                        StartRead();
                                        Consume();
                                        symbol = string.Format("{0}{1}", symbol, ch);
                                    }
                                    break;

                                default: break;
                            }

                            //if user wants tokens separated only by separators (spaces, tabs, etc.), check if 
                            //symbol is alone or within a word:
                            if (tokenSeparatedBySpace && (ch != EOF) && (ch != ' ') && (ch != '\t') &&
                                (ch != '\n') && (ch != '\r'))
                            {
                                return ReadWord(string.Format("{0}{1}", firstChar, ch));
                            }

                            return CreateToken(TokenKind.Symbol, symbol);
                        }
                        else
                        {
                            StartRead();
                            Consume();
                            return CreateToken(TokenKind.Unknown);
                        }
                    }

            }
        }

        /// <summary>
        /// save read point positions so that CreateToken can use those
        /// </summary>
        private void StartRead()
        {
            saveLine = line;
            saveCol = column;
            savePos = pos;
        }

        /// <summary>
        /// reads all whitespace characters (does not include newline)
        /// </summary>
        /// <returns></returns>
        protected Token ReadWhitespace()
        {
            StartRead();

            Consume(); // consume the looked-ahead whitespace char

            while (true)
            {
                char ch = LA(0);
                if (ch == '\t' || ch == ' ')
                    Consume();
                else
                    break;
            }

            return CreateToken(TokenKind.WhiteSpace);

        }

        /// <summary>
        /// reads number. Number is: DIGIT+ ("." DIGIT*)?
        /// </summary>
        /// <returns></returns>
        protected Token ReadNumber()
        {
            StartRead();

            bool hadDot = false;

            Consume(); // read first digit

            while (true)
            {
                char ch = LA(0);
                if (Char.IsDigit(ch))
                    Consume();
                else if (ch == '.' && !hadDot)
                {
                    hadDot = true;
                    Consume();
                }
                else
                    break;
            }

            return CreateToken(TokenKind.Number);
        }

        /// <summary>
        /// reads word. Word contains any alpha character or _
        /// </summary>
        protected Token ReadWord()
        {
            StartRead();

            Consume(); // consume first character of the word

            while (true)
            {
                char ch = LA(0);
                if (Char.IsLetter(ch) || (ignoreDigits && Char.IsDigit(ch)) || ch == '_' ||
                    (ignoreEqualChar && (ch == '=')) || IsSymbolAsLetter(ch) ||
                    (tokenSeparatedBySpace && IsSymbol(ch)))
                    Consume();
                else
                    break;
            }

            return CreateToken(TokenKind.Word);
        }

        /// <summary>
        /// reads word. Word contains any alpha character or _ and is appended to input string
        /// </summary>
        /// <param name="prefix">string which word is appended to</param>
        protected Token ReadWord(string prefix)
        {
            StartRead();

            //bring forward the position cursor of the nmber of chars in prefix:
            savePos = savePos - prefix.Length + 1;

            while (true)
            {
                char ch = LA(0);
                if (Char.IsLetter(ch) || (ignoreDigits && Char.IsDigit(ch)) || ch == '_' ||
                    (ignoreEqualChar && (ch == '=')) || IsSymbolAsLetter(ch) ||
                    (tokenSeparatedBySpace && IsSymbol(ch)))
                    Consume();
                else
                    break;
            }

            return CreateToken(TokenKind.Word);
        }

        /// <summary>
        /// reads all characters until next " is found.
        /// If "" (2 quotes) are found, then they are consumed as
        /// part of the string
        /// </summary>
        /// <returns></returns>
        protected Token ReadString()
        {
            StartRead();

            Consume(); // read "

            while (true)
            {
                char ch = LA(0);
                if (ch == EOF)
                    break;
                else if (ch == '\r')	// handle CR in strings
                {
                    Consume();
                    if (LA(0) == '\n')	// for DOS & windows
                        Consume();

                    line++;
                    column = 1;
                }
                else if (ch == '\n')	// new line in quoted string
                {
                    Consume();

                    line++;
                    column = 1;
                }
                else if (ch == '"')
                {
                    Consume();
                    if (LA(0) != '"')
                        break;	// done reading, and this quotes does not have escape character
                    else
                        Consume(); // consume second ", because first was just an escape
                }
                else
                    Consume();
            }

            return CreateToken(TokenKind.QuotedString);
        }

        /// <summary>
        /// checks whether c is a symbol character.
        /// </summary>
        protected bool IsSymbol(char c)
        {
            for (int i = 0; i < symbolChars.Length; i++)
                if (symbolChars[i] == c)
                    return true;

            return false;
        }

        /// <summary>
        /// checks whether c is a symbol character treated as common letter inside words.
        /// </summary>
        protected bool IsSymbolAsLetter(char c)
        {
            for (int i = 0; i < symbolAsLetter.Length; i++)
                if (symbolAsLetter[i] == c)
                    return true;

            return false;
        }
    }
}
