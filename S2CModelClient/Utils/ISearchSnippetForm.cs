//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using Snip2Code.Model.Client.Entities;
using Snip2Code.Model.Client.WSFramework;
using System;
using System.Configuration;

namespace Snip2Code.Utils
{
    /// <summary>
    /// This interface collects all the methods exposed by Search Snippet form that are used by external components
    /// to set the state of the form
    /// </summary>
    public interface ISearchSnippetForm : IAbstractForm
    {
        /// <summary>
        /// Resets the fields of the form and prepares for a clean display
        /// </summary>
        void DisplayCleanForm();
    }
}
