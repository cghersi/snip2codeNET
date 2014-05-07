//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

using Snip2Code.Model.Abstract;
using Snip2Code.Model.Comm;
using Snip2Code.Utils;

namespace Snip2Code.Model.Entities
{
    /// <summary>
    /// Provides the implementation class to sort a list of snippets
    /// by modified date
    /// </summary>
    public class SnippetComparer : Comparer<Snippet>
    {
        public override int Compare(Snippet x, Snippet y)
        {
            if (x == null)
                return -1;
            else if (y == null)
                return 1;
            else
                return x.Modified.CompareTo(y.Modified);
        }
    }
}


