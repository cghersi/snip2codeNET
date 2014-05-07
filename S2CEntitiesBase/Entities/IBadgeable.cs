//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Xml.Linq;
using System.Xml.Serialization;

using Snip2Code.Utils;
using System.Collections.Generic;

namespace Snip2Code.Model.Entities
{
    public interface IBadgeable
    {
        long BadgeableID { get; }

        List<Badge> Badges { get; }

        /// <summary>
        /// Adds the given content to the list of badges.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        /// <param name="addingBadge"></param>
        void AddBadge(Badge addingBadge);

        /// <summary>
        /// Replaces the given content in the list of badges, if not yet present.
        /// This procedure modifies only the local in-memory content of the snippet.
        /// </summary>
        /// <param name="existingBadge"></param>
        /// <param name="newBadge"></param>
        void ReplaceBadge(Badge existingBadge, Badge newBadge);
    }
}
