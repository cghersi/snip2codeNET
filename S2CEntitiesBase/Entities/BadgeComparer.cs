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
    /// Provides the implementation class to sort a list of badges
    /// by their ID and then by the timestamp
    /// </summary>
    public class BadgeComparer : Comparer<Badge>
    {
        public override int Compare(Badge x, Badge y)
        {
            if (x == null)
                return -1;
            else if (y == null)
                return 1;
            else if ((x.ID >= 0) || (y.ID >= 0)) //check IDs:
            {
                if (x.ID < y.ID)
                    return -1;
                else if (x.ID > y.ID)
                    return 1;
            }

            //otherwise, check timestamps:
            return x.Timestamp.CompareTo(y.Timestamp);
        }
    }


    /// <summary>
    /// Provides the implementation class to sort a list of badges by timestamp
    /// </summary>
    public class BadgeComparerByTimestamp : Comparer<Badge>
    {
        public override int Compare(Badge x, Badge y)
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

            return x.Timestamp.CompareTo(y.Timestamp);
        }
    }
}


