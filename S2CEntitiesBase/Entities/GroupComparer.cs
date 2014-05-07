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
    /// Provides the implementation class to sort a list of groups
    /// by number of snippets and then number of members
    /// </summary>
    public class GroupComparer : Comparer<Group>
    {
        public override int Compare(Group x, Group y)
        {
            if (x == null)
                return -1;
            else if (y == null)
                return 1;
            else if ((x.NumOfSnippets >= 0) || (y.NumOfSnippets >= 0)) //check number of snippets:
            {
                if (x.NumOfSnippets < y.NumOfSnippets)
                    return -1;
                else if (x.NumOfSnippets > y.NumOfSnippets)
                    return 1;
            }

            //otherwise, check number of members / Followers:
            if (x.Members == null)
                return -1;
            else if (y.Members == null)
                return 1;
            else if (x.Members.Count < y.Members.Count)
                return -1;
            else if (x.Members.Count > y.Members.Count)
                return 1;

            return 0;
        }
    }


    /// <summary>
    /// Provides the implementation class to sort a list of groups by name
    /// </summary>
    public class GroupComparerByName : Comparer<Group>
    {
        public override int Compare(Group x, Group y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;

            string name1 = x.Name;
            if (name1 == null)
            {
                if (y.Name == null)
                    return 0;
                else
                    return -1;
            }
            return name1.ToLower().CompareTo(y.Name.ToLower());
        }
    }
}


