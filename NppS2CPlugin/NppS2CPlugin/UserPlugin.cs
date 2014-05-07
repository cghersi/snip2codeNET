//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Snip2Code.Model.Client.Entities;
using Snip2Code.Model.Client.WSFramework;
using Snip2Code.Utils;
using System.Globalization;


namespace NppPluginNet
{
    public class UserPlugin : UserClient
    {
        static private UserPlugin s_current = null;
        static private object s_ObjSync = new object();

        protected UserPlugin()
            : base()
        {
        }


        /// <summary>
        ///     This return the one and only user plugin (this is a singleton)
        /// </summary>
        static public UserPlugin Current
        {
            get
            {
                if (s_current == null)
                {
                    lock (s_ObjSync)
                    {
                        if (s_current == null)
                            s_current = new UserPlugin();
                    }
                }
                return s_current;
            }
        }
    }
}   
