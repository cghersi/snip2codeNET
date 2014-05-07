//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

using Snip2Code.Model.Comm;
using Snip2Code.Utils;
using Snip2Code.Model.Abstract;

namespace Snip2Code.Model.Entities
{  
    public enum OneAllProvider
    {
        unknown = 0,
        facebook = 1, 
        github = 2, 
        google = 3,
        linkedin = 4, 
        stackexchange = 5, 
        twitter = 6,
        windowslive = 7,
        disqus = 8
    }


    /// <summary>
    /// This is a POCO representing the association of a user to a given group
    /// </summary>
    public abstract class OneAllToken : BaseEntity
    {
        #region Private Members
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        private IUserRepository m_repoUser = null;

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Primitive Properties
        /////////////////////////////////////////////////////////////////////////////////////////////////

        public int UserID { get; set; }
        public Guid Token { get; set; }
        public OneAllProvider Provider { get; set; }
        public string OneAllNickname { get; set; }
        public bool TokenIsValid { get; set; }

        public User User
        {
            get
            {
                if ((UserID > 0) && (UserRepository != null))
                    return UserRepository.GetById(UserID);
                else
                    return null;
            }
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////

        #region Constructors
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor, builds an empty object.
        /// </summary>
        public OneAllToken() 
        {
            UserID = -1;
            Token = Guid.Empty;
            Provider = OneAllProvider.unknown;
        }

        /// <summary>
        /// Builds a new object from a string formatted in one 
        /// of the SerialFormat enum values
        /// </summary>
        /// <param name="content"></param>
        /// <param name="format"></param>
        protected OneAllToken(string content, SerialFormat format) : base(content, format) { }

        /// <summary>
        /// This is the complete init method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="token"></param>
        /// <param name="provider"></param>
        /// <param name="oneAllNickname"></param>
        /// <param name="tokenIsValid"></param>
        protected void Init(int userID, Guid token, OneAllProvider provider, string oneAllNickname, bool tokenIsValid)
        {
            UserID = userID;
            Token = token;
            Provider = provider;
            OneAllNickname = oneAllNickname;
            TokenIsValid = tokenIsValid;
        }

        /// <summary>
        /// This is the init/copy method for this class.
        /// It should be used by the children classes in the constructor in order to correctly fill the properties of the object.
        /// </summary>
        /// <param name="objToCopy"></param>
        protected bool Init(OneAllToken objToCopy)
        {
            if (objToCopy == null)
                return false;

            //save the values of the properties with private setters:
            UserID = objToCopy.UserID;
            Token = objToCopy.Token;
            Provider = objToCopy.Provider;
            OneAllNickname = objToCopy.OneAllNickname;
            TokenIsValid = objToCopy.TokenIsValid;

            return true;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region DeSerialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override bool ParseFromJson(string content)
        {
            return Init(ParseFromJson<OneAllToken>(content));
        }

        public override bool Parse(XElement xml)
        {
            if (xml == null)
                return false;

            int userID = 0;
            Guid token = Guid.Empty;
            OneAllProvider provider = OneAllProvider.unknown;
            string oneAllNickname = string.Empty;
            bool tokenIsValid = true;

            xml.ParseNode("UserID", ref userID);
            xml.ParseNode("Token", ref token);
            xml.ParseNode<OneAllProvider>("Provider", ref provider);
            xml.ParseNode("OneAllNickname", ref oneAllNickname);
            xml.ParseNode("TokenIsValid", ref tokenIsValid);

            Init(userID, token, provider, oneAllNickname, tokenIsValid);

            return true;
        }

        public override BaseEntity ToBaseEntity()
        {
            Init(this);
            return this;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Serialization
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public override XElement ToXML(bool internalPurpose = true)
        {
            XDocument res = Utilities.CreateXMLDoc("OneAllToken", false);

            res.Root.Add(
                new XElement(ConfigReader.S2CNS + "UserID", UserID),
                new XElement(ConfigReader.S2CNS + "Token", Token.FixXML()),
                new XElement(ConfigReader.S2CNS + "Provider", Provider),
                new XElement(ConfigReader.S2CNS + "OneAllNickname", OneAllNickname.FixXML()),
                new XElement(ConfigReader.S2CNS + "TokenIsValid", TokenIsValid)
            );

            return res.Root;
        }


        /// <summary>
        ///     override object.ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("UserID={0};Token={1};Provider={2};OneAllNickname={3};TokenIsValid={4}", 
                UserID, Token, Provider, OneAllNickname, TokenIsValid);
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Repository
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        protected IUserRepository UserRepository
        {
            get
            {
                if (m_repoUser == null)
                    m_repoUser = RetrieveUserRepository();
                return m_repoUser;
            }
        }

        /// <summary>
        /// Retrieves the repository of the user, used to lazy load info related to the members of this group.
        /// </summary>
        /// <returns>null if any error occurred or not found</returns>
        protected abstract IUserRepository RetrieveUserRepository();

        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}
