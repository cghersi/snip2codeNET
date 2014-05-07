//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snip2Code.Model.Utils
{
    /// <summary>
    /// This comparer provides the comparison of two strings ignoring the case of them
    /// </summary>
    public class IgnoreCaseComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (x == null)
                return (y == null);
            return x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            if (obj == null)
                return 0;
            return obj.ToLowerInvariant().GetHashCode();
        }
    }
}
