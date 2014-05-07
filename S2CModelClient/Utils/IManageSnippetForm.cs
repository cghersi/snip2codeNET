//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using Snip2Code.Model.Client.Entities;
using Snip2Code.Model.Client.WSFramework;
using Snip2Code.Model.Entities;
using System;
using System.Configuration;

namespace Snip2Code.Utils
{
    /// <summary>
    /// This interface collects all the methods exposed by Add/View Snippet form that are used by external components
    /// to set the state of the form
    /// </summary>
    public interface IManageSnippetForm : IAbstractForm
    {
        /// <summary>
        /// Prepares the form to create a new snippet
        /// </summary>
        /// <param name="selectedText">the content selected by the user that is the code of the snippet</param>
        void PrepareAddNewSnippet(string selectedText);

        /// <summary>
        /// Prepares the form to view an existing snippet
        /// </summary>
        /// <param name="snip">the snippet to display</param>
        void PrepareViewSnippet(Snippet snip);
    }
}
