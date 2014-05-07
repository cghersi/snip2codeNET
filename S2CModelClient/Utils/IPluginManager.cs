//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using Snip2Code.Model.Client.Entities;
using Snip2Code.Model.Client.WSFramework;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Snip2Code.Utils
{
    /// <summary>
    /// This interface collects all the methods exposed by the manager of the plugin/app life cycle
    /// </summary>
    public interface IPluginManager
    {
        /// <summary>
        /// Retrieves the text selected by the user in the current environment
        /// </summary>
        /// <returns></returns>
        string RetrieveSelectedText();

        #region Display Toolbar Buttons
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Make the UI in "LoggedOut" mode, i.e.
        /// - Add Snippet (grayed)
        /// - Search Snippets
        /// - Login
        /// - About
        /// </summary>
        void DisplayLoggedOutButtons();

        /// <summary>
        /// Make the UI in "LoggedIn" mode, i.e.
        /// - Add Snippet
        /// - Search Snippets
        /// - Logout [username]
        /// - About
        /// </summary>
        void DisplayLoggedInButtons();

        #endregion
        /////////////////////////////////////////////////////////////////////////////////


        #region Close Forms
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Closes the Publish Snippet form
        /// </summary>
        void ClosePublishSnippetWindow();

        /// <summary>
        /// Closes the Add Snippet form
        /// </summary>
        void CloseAddSnippetWindow();

        #endregion
        /////////////////////////////////////////////////////////////////////////////////


        #region Forms Clean Up
        /////////////////////////////////////////////////////////////////////////////////

        ///// <summary>
        ///// Cleans up login form
        ///// </summary>
        //void LoginPageCleanup();

        ///// <summary>
        ///// Cleans up Search Snippets form
        ///// </summary>
        //void SearchPageCleanup();


        #endregion
        /////////////////////////////////////////////////////////////////////////////////


        #region Forms Creation (Factory)
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Factory to create a form to add a new snippet
        /// </summary>
        /// <returns></returns>
        IManageSnippetForm CreateAddSnippetForm();

        /// <summary>
        /// Factory to create a form to view an existing snippet
        /// </summary>
        /// <returns></returns>
        IManageSnippetForm CreateViewSnippetForm();

        /// <summary>
        /// Factory to create a form to publish an existing snippet
        /// </summary>
        /// <returns></returns>
        IPublishSnippetForm CreatePublishSnippetForm();

        /// <summary>
        /// Factory to create a form to login the user
        /// </summary>
        /// <returns></returns>
        ILoginForm CreateLoginForm();

        ///// <summary>
        ///// Factory to create a form to search snippets
        ///// </summary>
        ///// <returns></returns>
        //ISearchSnippetForm CreateSearchSnippetsForm();

        /// <summary>
        /// Retrieves, if available, the window of the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T FindWindow<T>() where T : IAbstractForm;

        /// <summary>
        /// Retrieves, if available, all the windows of the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> FindWindows<T>() where T : IAbstractForm;

        #endregion
        /////////////////////////////////////////////////////////////////////////////////
    }
}
