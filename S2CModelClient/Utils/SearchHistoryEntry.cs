//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snip2Code.Utils
{
    public class SearchHistoryEntry : IComparable
    {
        public string SearchText { get; set; }
        public DateTime LastSearchTime { get; set; }
        public int HitCount { get; set; }
        public const string SEARCH_TIME_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

        public SearchHistoryEntry()
        {
            SearchText = string.Empty;
            LastSearchTime = DateTime.Now;
            HitCount = 0;
        }

        public SearchHistoryEntry(string text) : this()
        {
            SearchText = text;
            HitCount = 1;
        }

        public void UpdateEntry()
        {
            HitCount++;
            LastSearchTime = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2}",
                    SearchText, LastSearchTime.ToString(SEARCH_TIME_DATE_FORMAT), HitCount);
        }

        int IComparable.CompareTo(object obj)
        {
            SearchHistoryEntry b = obj as SearchHistoryEntry;

            if (b == null)
                return 1;
            else
            {
                if (this.HitCount > b.HitCount)
                    return 1;
                else if (this.HitCount < b.HitCount)
                    return -1;
                else
                    return this.LastSearchTime.CompareTo(b.LastSearchTime);
            }
        }
    }


    /// <summary>
    /// Compare two SearchHistoryEntry objects
    /// </summary>
    //public class SearchHistoryEntryComparer : IComparer<SearchHistoryEntry>
    //{
        //    public int CompareTo(SearchHistoryEntry a, SearchHistoryEntry b)
        //{
        //    if (a == null)
        //        return (b == null) ? 0 : -1;
        //    else if (b == null)
        //        return 1;
        //    else
        //    {
        //        if (a.HitCount > b.HitCount)
        //            return 1;
        //        else if (a.HitCount < b.HitCount)
        //            return -1;
        //        else
        //            return a.LastSearchTime.CompareTo(b.LastSearchTime);
        //    }
        //}
    //}

}
