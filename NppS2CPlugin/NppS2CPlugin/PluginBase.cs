//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using log4net.Config;
using log4net;
using Snip2Code.Utils;
using Snip2Code.Model.Client.WSFramework;
using NppPluginNet.Properties;
using System.ComponentModel;
using System.Reflection;


namespace NppPluginNet
{
    partial class PluginBase : IPluginManager
    {
        #region " Fields "
        /////////////////////////////////////////////////////////////////////////////////

        private static log4net.ILog log = null;

        internal const string PluginName = "Snip2Code";
        static string s_iniFilePath = null;
        static string s_generalConfigPath = string.Empty;

        public const int ID_ADD_SNIPPET_CMD = 1;
        public const int ID_SEARCH_SNIPPET_CMD = 2;
        public const int ID_LOGIN_LOGOUT_CMD = 3;
        public const int ID_ABOUT_CMD = 4;
        public const int ID_PUB_SNIPPET_CMD = 5;

        public const string LOGIN_MENU_LABEL = "Login";
        public const string LOGOUT_MENU_LABEL = "Logout";
        public const string ABOUT_MENU_LABEL = "About";
        public const string ADDSNIP_MENU_LABEL = "Add Snippet";
        public const string VIEWSNIP_MENU_LABEL = "View Snippet";
        public const string PUBSNIP_MENU_LABEL = "Publish Snippet";
        public const string SEARCHSNIP_MENU_LABEL = "Search Snippets";

        static Bitmap addSnipBmp = Properties.Resources.addSnip;
        static Bitmap searchSnipBmp = Properties.Resources.search;

        static System.Threading.Timer s_loginStatus = null;

        #endregion
        /////////////////////////////////////////////////////////////////////////////////

        #region " Startup/CleanUp "
        /////////////////////////////////////////////////////////////////////////////////
        
        static internal void CommandMenuInit()
        {
	        // get path of plugin configuration
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            try
            {
                Win32.SendMessage(nppData._nppHandle, NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            }
            catch { }
            s_iniFilePath = sbIniFilePath.ToString();
#if TEST_NPP
            s_iniFilePath = Application.UserAppDataPath;
#endif
            CheckConfigFileExistence(s_iniFilePath + "\\Snip2Code.config");
            ClientUtils.InitApp(UserPlugin.Current, s_iniFilePath, PluginBase.Current);

            s_generalConfigPath = s_iniFilePath.Replace("plugins\\Config", "config.xml");

            //start a timertask to periodically check for the login status (10 times faster than the periodic check of connection):
            //ALREADY DONE BY LOGIN POLLER
            //s_loginStatus = new System.Threading.Timer(delegate { CheckLoginStatus(); }, null, 1000, AppConfig.Current.LoginRefreshTimeSec * 100);

            log = log4net.LogManager.GetLogger("PluginBase");
            log.Info("Starting Snip2Code from this directory:" + s_iniFilePath);

	        // if config path doesn't exist, we create it
            if (!Directory.Exists(s_iniFilePath))
            {
                try
                {
                    Directory.CreateDirectory(s_iniFilePath);
                }
                catch { }
            }

	        // make your plugin config file full file path name
            s_iniFilePath = Path.Combine(s_iniFilePath, PluginName + ".config");

            // with function :
            // SetCommand(int index,                            // zero based number to indicate the order of command
            //            string commandName,                   // the command name that you want to see in plugin menu
            //            NppFuncItemDelegate functionPointer,  // the symbol of function (function pointer) associated with this command. The body should be defined below. See Step 4.
            //            ShortcutKey *shortcut,                // optional. Define a shortcut to trigger this command
            //            bool check0nInit                      // optional. Make this menu item be checked visually
            //            );
            SetCommand(ID_ADD_SNIPPET_CMD, ADDSNIP_MENU_LABEL, AddSnip, new ShortcutKey(true, true, true, Keys.A));
            SetCommand(ID_SEARCH_SNIPPET_CMD, SEARCHSNIP_MENU_LABEL, SearchSnip, new ShortcutKey(true, true, true, Keys.S));
            SetCommand(ID_LOGIN_LOGOUT_CMD, LOGIN_MENU_LABEL, Login_Logout);
            SetCommand(ID_ABOUT_CMD, ABOUT_MENU_LABEL, About);
        }

        //static internal void CheckLoginStatus()
        //{
        //    if (WebConnector.Current.IsLogged)
        //        PluginBase.Current.DisplayLoggedInButtons();
        //    else
        //        PluginBase.Current.DisplayLoggedOutButtons();
        //}

        static internal void SetToolBarIcon()
        {
            SetSingleToolbarIcon(addSnipBmp, ID_ADD_SNIPPET_CMD);
            SetSingleToolbarIcon(searchSnipBmp, ID_SEARCH_SNIPPET_CMD);
        }

        private static void SetSingleToolbarIcon(Bitmap bmp, int cmdID)
        {
            try
            {
                toolbarIcons tbIcons = new toolbarIcons();
                tbIcons.hToolbarBmp = bmp.GetHbitmap();
                IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
                Marshal.StructureToPtr(tbIcons, pTbIcons, false);
                Win32.SendMessage(nppData._nppHandle, NppMsg.NPPM_ADDTOOLBARICON, _funcItems.Items[cmdID - 1]._cmdID, pTbIcons);
                Marshal.FreeHGlobal(pTbIcons);
            }
            catch (Exception e)
            {
                if (log != null)
                    log.ErrorFormat("Cannot set toolbar with cmdID={0} due to {1}", cmdID, e.Message);
            }
        }

        static internal void PluginCleanUp()
        {
	        //nothing to do here...
        }

        static internal void CheckConfigFileExistence(string path)
        {
            if (File.Exists(path))
                return;

            try
            {
                File.WriteAllText(path, Properties.Resources.ConfigContent);
            }
            catch (Exception e)
            {
                if (log != null)
                    log.ErrorFormat("Cannot create config file here:{0} due to {1}", path, e.Message);
            }
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////

        public static string GetCurrentFilename()
        {
            StringBuilder path = new StringBuilder(Win32.MAX_PATH);
            try
            {
                Win32.SendMessage(nppData._nppHandle, NppMsg.NPPM_GETFILENAME, 0, path);
            }
            catch (Exception e)
            {
                if (log != null)
                    log.ErrorFormat("Cannot get current filename due to {0}", e.Message);
            }

            return path.ToString();
        }


        public static string GetCurrentLanguage()
        {
            LangType docType = LangType.L_TEXT;
            try
            {
                Win32.SendMessage(nppData._nppHandle, NppMsg.NPPM_GETCURRENTLANGTYPE, 0, ref docType);
            }
            catch (Exception exc)
            {
                if (log != null)
                    log.ErrorFormat("Cannot get current language due to {0}", exc.Message);
            }
            switch (docType)
            {
                case LangType.L_TEXT: return "Text";
                case LangType.L_PHP: return "PHP";
                case LangType.L_C: return "C";
                case LangType.L_CPP: return "C++";
                case LangType.L_CS: return "CSharp";
                case LangType.L_OBJC: return "ObjectiveC";
                case LangType.L_JAVA: return "Java";
                case LangType.L_RC: return "R";
                case LangType.L_HTML: return "HTML";
                case LangType.L_XML: return "XML";
                case LangType.L_MAKEFILE: return "makefile";
                case LangType.L_PASCAL: return "Pascal";
                case LangType.L_BATCH: return "Batch";
                case LangType.L_INI: return "ini";
                case LangType.L_ASCII: return "ASCII";
                case LangType.L_USER: return "Text";
                case LangType.L_ASP: return "ASP";
                case LangType.L_SQL: return "SQL";
                case LangType.L_VB: return "VB";
                case LangType.L_JS: return "Javascript";
                case LangType.L_CSS: return "CSS";
                case LangType.L_PERL: return "PERL";
                case LangType.L_PYTHON: return "Python";
                case LangType.L_LUA: return "Lua";
                case LangType.L_TEX: return "Tex";
                case LangType.L_FORTRAN: return "Fortran";
                case LangType.L_BASH: return "BASH";
                case LangType.L_FLASH: return "Flash";
                case LangType.L_NSIS: return "Nsis";
                case LangType.L_TCL: return "TCL";
                case LangType.L_LISP: return "Lisp";
                case LangType.L_SCHEME: return "Scheme";
                case LangType.L_ASM: return "Assembly_x86";
                case LangType.L_DIFF: return "Diff";
                case LangType.L_PROPS: return "Properties";
                case LangType.L_PS: return "PowerShell";
                case LangType.L_RUBY: return "Ruby";
                case LangType.L_SMALLTALK: return "Smalltalk";
                case LangType.L_VHDL: return "VHDL";
                case LangType.L_KIX: return "Kix";
                case LangType.L_AU3: return "Au3";
                case LangType.L_CAML: return "Caml";
                case LangType.L_ADA: return "ADA";
                case LangType.L_VERILOG: return "Verilog";
                case LangType.L_MATLAB: return "MATLab";
                case LangType.L_HASKELL: return "Haskell";
                case LangType.L_INNO: return "SQL";
                case LangType.L_SEARCHRESULT: return "";
                case LangType.L_CMAKE: return "makefile";
                case LangType.L_YAML: return "Yaml";
                case LangType.L_COBOL: return "COBOL";
                case LangType.L_GUI4CLI: return "";
                case LangType.L_D: return "D";
                case LangType.L_POWERSHELL: return "PowerShell";
                case LangType.L_R: return "R";
                case LangType.L_JSP: return "JSP";
                case LangType.L_EXTERNAL:
                default: return "";
            }
        }

        public static void CloseForm(Form form)
        {     
#if DOCKED
            try
            {
                Win32.SendMessage(nppData._nppHandle, NppMsg.NPPM_DMMHIDE, 0, form.Handle);
            }
            catch (Exception exc)
            {
                if (log != null)
                    log.ErrorFormat("Cannot open form due to {0}", exc.Message);
            }
#else
            form.Close();
#endif
        }

        public static void OpenForm(Form form, string title, int dialogID)
        {
            NppTbData _nppTbData = new NppTbData();
            _nppTbData.hClient = form.Handle;
            _nppTbData.pszName = title;

            // the dlgDlg should be the index of funcItem where the current function pointer is in
            _nppTbData.dlgID = dialogID;

            // define the default docking behaviour
            _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;

            Icon icon = null;
            if (ADDSNIP_MENU_LABEL.Equals(title))
            {
                if (addSnipBmp != null)
                    icon = Icon.FromHandle(addSnipBmp.GetHicon());
            }
            else if (SEARCHSNIP_MENU_LABEL.Equals(title))
            {
                if (addSnipBmp != null)
                    icon = Icon.FromHandle(searchSnipBmp.GetHicon());
            }
            if (icon != null)
                _nppTbData.hIconTab = (uint)icon.Handle;

            _nppTbData.pszModuleName = PluginName;
            IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
            Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

            //try to programmatically change the config.xml of Notepad++ in order to set the initial docking layout:
            SetInitialSizeOfDockedForm(form.Size.Width);

            try
            {
                Win32.SendMessage(nppData._nppHandle, NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            }
            catch (Exception exc)
            {
                if (log != null)
                    log.ErrorFormat("Cannot open form due to {0}", exc.Message);
            }
        }

        private static void SetInitialSizeOfDockedForm(int initialWidth)
        {
            //Try to open the general configuration file of Notepad++:
            XDocument generalConfig = null;
            try
            {
                generalConfig = XDocument.Load(s_generalConfigPath);
            }
            catch (Exception exc)
            {
                if (log != null)
                    log.ErrorFormat("Cannot open general configuration file at path {1} due to {0}", exc.Message, s_generalConfigPath);
                return;
            }

            if (generalConfig == null)
            {
                if (log != null)
                    log.ErrorFormat("Cannot open general configuration file at path {0}", s_generalConfigPath);
                return;
            }

            //Go to the DockingManager node of the XML:
            XElement guiConfigsElem = generalConfig.Root.XPathSelectElement("/NotepadPlus/GUIConfigs");
            if (guiConfigsElem == null)
            {
                if (log != null)
                    log.ErrorFormat("Cannot find GUIConfigs on general configuration file at path {0}", s_generalConfigPath);
                return;
            }
            XElement elem = null;
            foreach (XElement e in guiConfigsElem.Descendants("GUIConfig"))
            {
                XAttribute attr = e.Attribute("name");
                if (attr == null)
                    continue;
                if (attr.Value.Equals("DockingManager", StringComparison.InvariantCultureIgnoreCase))
                {
                    elem = e;
                    break;
                }
            }
            if (elem == null)
            {
                if (log != null)
                    log.ErrorFormat("Cannot find DockingManager on general configuration file at path {0}", s_generalConfigPath);
                return;
            }

            //Modify the attribute of this element and save the file:
            try
            {
                XAttribute attr = elem.Attribute("rightWidth");
                if (attr == null)
                {
                    if (log != null)
                        log.ErrorFormat("Cannot find DockingManager.rightWidth on general configuration file at path {0}", s_generalConfigPath);
                    return;
                }

                attr.SetValue(initialWidth);

                generalConfig.Save(s_generalConfigPath);
            }
            catch(Exception e)
            {
                if (log != null)
                    log.ErrorFormat("Cannot save general configuration file at path {0}", s_generalConfigPath);
            }
        }

        #region " Menu functions "
        /////////////////////////////////////////////////////////////////////////////////

        public static void AddSnip()
        {
            frmAddSnippet window = ClientUtils.PrepareAddNewSnippetForm(PluginBase.Current) as frmAddSnippet;
            if (window == null)
                return;
#if DOCKED
            OpenForm(window, ADDSNIP_MENU_LABEL, ID_ADD_SNIPPET_CMD);
#else
            window.Show();
#endif
        }

        public static void SearchSnip()
        {
            frmSearch window = new frmSearch();

#if DOCKED
            OpenForm(window, SEARCHSNIP_MENU_LABEL, ID_SEARCH_SNIPPET_CMD);
#else
            window.Show();
#endif
        }

        public static void PublishSnip(long snippetID, int targetGroupID)
        {
            frmPublish window = ClientUtils.PreparePublishSnippetForm(PluginBase.Current, snippetID, targetGroupID) as frmPublish;
            if (window == null)
                return;

#if DOCKED
            OpenForm(window, PUBSNIP_MENU_LABEL, ID_PUB_SNIPPET_CMD);
#else
            window.Show();
#endif
        }

        public static void Login_Logout()
        {
            if ((BaseWS.CurrentUser == null) || !WebConnector.Current.IsLogged)
            {
                log.Info("Login");
                frmLogin window = ClientUtils.PrepareLoginForm(PluginBase.Current) as frmLogin;
                if (window == null)
                    return;

#if DOCKED
                OpenForm(window, LOGIN_MENU_LABEL, ID_LOGIN_LOGOUT_CMD);
#else
                window.Show();
#endif
            }
            else
            {
                ClientUtils.LogoutUser(PluginBase.Current, UserPlugin.Current);
            }
        }

        public static void About()
        {
            frmAbout window = new frmAbout();

#if DOCKED
            OpenForm(window, ABOUT_MENU_LABEL, ID_ABOUT_CMD);
#else
            window.Show();
#endif
        }

        private static void ChangeMenuItem(string oldLabel, string newLabel, bool enabled)
        {
            try
            {
                IntPtr hMenu = Win32.SendMessage(nppData._nppHandle, NppMsg.NPPM_GETMENUHANDLE, (int)NppMsg.NPPPLUGINMENU, 0);

                //Try to find the Snip2Code menu
                MenuItemInfoEnlarged s2cMenu = ManageMenu(hMenu, PluginName, "", true, true);
                if (s2cMenu != null)
                {
                    //in this Menu, find the item that matches the old label and change it:
                    ManageMenu(s2cMenu.mif.hSubMenu, oldLabel, newLabel, enabled, false);
                }
            }
            catch(Exception e)
            {
                if (log != null)
                    log.ErrorFormat("Cannot change menu item {0} to {1} due to {2}", oldLabel.PrintNull(), newLabel.PrintNull(), e.Message);
            }
        }

        class MenuItemInfoEnlarged
        {
            public MENUITEMINFO mif;
            public int pos;
        }

        private static MenuItemInfoEnlarged ManageMenu(IntPtr parentMenu, string oldMenuName, string newMenuName, bool enable, bool onlyRead)
        {
            if ((parentMenu.ToInt32() == 0) || string.IsNullOrEmpty(oldMenuName))
            {
                if (log != null) 
                    log.WarnFormat("Cannot manage menu with parent={0} and oldMenuName={1}", parentMenu.ToInt32(), oldMenuName.PrintNull());
                return null;
            }

            for (int i = Win32.GetMenuItemCount(parentMenu) - 1; i >= 0; i--)
            {
                try
                {
                    //guess the label of the menu in two steps:

                    //1. get the length of the string:
                    MENUITEMINFO mif = new MENUITEMINFO();
                    mif.fMask = Win32.MIIM_STRING | Win32.MIIM_SUBMENU | Win32.MIIM_ID;
                    mif.fType = Win32.MFT_STRING;
                    mif.dwTypeData = IntPtr.Zero;
                    bool res = Win32.GetMenuItemInfo(parentMenu, i, true, mif);
                    if (!res)
                    {
                        uint error = Win32.GetLastError(); //just for debug purpose
                        if (log != null) 
                            log.WarnFormat("Cannot get menu info with parent={0} and # {1} due to error={2}", parentMenu.ToInt32(), i, error);
                        continue;
                    }
                    mif.cch++;
                    mif.dwTypeData = Marshal.AllocHGlobal((IntPtr)(mif.cch * 2));

                    //2. get the actual string:
                    try
                    {
                        res = Win32.GetMenuItemInfo(parentMenu, i, true, mif);
                        if (!res)
                        {
                            uint error = Win32.GetLastError(); //just for debug purpose
                            if (log != null) 
                                log.WarnFormat("Cannot get menu info with parent={0} and # {1} due to error={2}", parentMenu.ToInt32(), i, error);
                            continue;
                        }
                        string caption = Marshal.PtrToStringUni(mif.dwTypeData);
                        if (caption.ToLowerInvariant().StartsWith(oldMenuName.ToLowerInvariant()))
                        {
                            if (!onlyRead && !string.IsNullOrEmpty(newMenuName))
                            {
                                mif.fMask = Win32.MIIM_STRING | Win32.MIIM_STATE;
                                if (enable)
                                    mif.fState = Win32.MFS_ENABLED;
                                else
                                    mif.fState = Win32.MFS_DISABLED;
                                mif.dwTypeData = Marshal.StringToHGlobalUni(newMenuName);
                                res = Win32.SetMenuItemInfo(parentMenu, i, true, mif);
                                if (res)
                                {
                                    if (!Win32.DrawMenuBar(nppData._nppHandle))
                                    {
                                        uint error = Win32.GetLastError(); //just for debug purpose
                                        if (log != null)
                                            log.WarnFormat("Cannot draw menu info with parent={0} and # {1} due to error={2}; newManuName was:{3}",
                                                        parentMenu.ToInt32(), i, error, newMenuName.PrintNull());
                                        continue;
                                    }
                                }
                                else
                                {
                                    uint error = Win32.GetLastError(); //just for debug purpose
                                    if (log != null)
                                        log.WarnFormat("Cannot get menu info with parent={0} and # {1} due to error={2}; newManuName was:{3}",
                                                    parentMenu.ToInt32(), i, error, newMenuName.PrintNull());
                                    continue;
                                }
                            }

                            MenuItemInfoEnlarged result = new MenuItemInfoEnlarged();
                            result.mif = mif;
                            result.pos = i;
                            return result;
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(mif.dwTypeData);
                    }
                }
                catch { }
            }

            return null;            
        }

        static internal void doInsertHtmlCloseTag(char newChar)
        {
            LangType docType = LangType.L_TEXT;
            Win32.SendMessage(nppData._nppHandle, NppMsg.NPPM_GETCURRENTLANGTYPE, 0, ref docType);
            bool isDocTypeHTML = (docType == LangType.L_HTML || docType == LangType.L_XML || docType == LangType.L_PHP);
            if (isDocTypeHTML)
            {
                if (newChar == '>')
                {
                    int bufCapacity = 512;
                    IntPtr hCurrentEditView = GetCurrentScintilla();
                    int currentPos = (int)Win32.SendMessage(hCurrentEditView, SciMsg.SCI_GETCURRENTPOS, 0, 0);
                    int beginPos = currentPos - (bufCapacity - 1);
                    int startPos = (beginPos > 0) ? beginPos : 0;
                    int size = currentPos - startPos;

                    if (size >= 3)
                    {
                        using (Sci_TextRange tr = new Sci_TextRange(startPos, currentPos, bufCapacity))
                        {
                            Win32.SendMessage(hCurrentEditView, SciMsg.SCI_GETTEXTRANGE, 0, tr.NativePointer);
                            string buf = tr.lpstrText;

                            if (buf[size - 2] != '/')
                            {
                                StringBuilder insertString = new StringBuilder("</");

                                int pCur = size - 2;
                                for (; (pCur > 0) && (buf[pCur] != '<') && (buf[pCur] != '>'); )
                                    pCur--;

                                if (buf[pCur] == '<')
                                {
                                    pCur++;

                                    Regex regex = new Regex(@"[\._\-:\w]");
                                    while (regex.IsMatch(buf[pCur].ToString()))
                                    {
                                        insertString.Append(buf[pCur]);
                                        pCur++;
                                    }
                                    insertString.Append('>');

                                    if (insertString.Length > 3)
                                    {
                                        Win32.SendMessage(hCurrentEditView, SciMsg.SCI_BEGINUNDOACTION, 0, 0);
                                        Win32.SendMessage(hCurrentEditView, SciMsg.SCI_REPLACESEL, 0, insertString);
                                        Win32.SendMessage(hCurrentEditView, SciMsg.SCI_SETSEL, currentPos, currentPos);
                                        Win32.SendMessage(hCurrentEditView, SciMsg.SCI_ENDUNDOACTION, 0, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////



        #region IPluginManager implementation
        /////////////////////////////////////////////////////////////////////////////////

        public string RetrieveSelectedText()
        {
            string selectedText = string.Empty;

            try
            {
                IntPtr hCurrScintilla = PluginBase.GetCurrentScintilla();
                int textLen = (int)Win32.SendMessage(hCurrScintilla, SciMsg.SCI_GETSELTEXT, 0, 0);
                IntPtr ptrText = Marshal.AllocHGlobal(textLen);
                Win32.SendMessage(hCurrScintilla, SciMsg.SCI_GETSELTEXT, 0, ptrText);
                selectedText = Marshal.PtrToStringAnsi(ptrText);
                Marshal.FreeHGlobal(ptrText);
            }
            catch (Exception exc)
            {
                if (log != null)
                    log.ErrorFormat("Cannot retrieve selected text due to {0}", exc.Message);
            }

            log.Info("Selected Text is: " + selectedText.PrintNull());

            return selectedText;
        }

        #region Display Toolbar Buttons
        /////////////////////////////////////////////////////////////////////////////////

        public void DisplayLoggedOutButtons()
        {
            log.Info("Changing to logged out buttons");
            ChangeMenuItem(LOGOUT_MENU_LABEL, LOGIN_MENU_LABEL, true);
            //ChangeMenuItem(ADDSNIP_MENU_LABEL, ADDSNIP_MENU_LABEL, false);
            log.Info("Changed to logged out buttons");
        }

        public void DisplayLoggedInButtons()
        {
            log.Info("Changing to logged in buttons");
            string currentUsername = string.Empty;

            // Add current username name to Logout button
            if (!string.IsNullOrWhiteSpace(BaseWS.Username))
                currentUsername = BaseWS.Username;
            else if ((BaseWS.CurrentUser != null) && (!string.IsNullOrWhiteSpace(BaseWS.CurrentUser.NickName)))
                currentUsername = BaseWS.CurrentUser.NickName;
            else if ((BaseWS.CurrentUser != null) && (!string.IsNullOrWhiteSpace(BaseWS.CurrentUser.Name)))
                currentUsername = BaseWS.CurrentUser.Name;

            string logoutTxt = LOGOUT_MENU_LABEL;
            if (!string.IsNullOrWhiteSpace(currentUsername))
                logoutTxt += string.Format(" [{0}]", currentUsername.Truncate(25, true));

            ChangeMenuItem(LOGIN_MENU_LABEL, logoutTxt, true);
            //ChangeMenuItem(ADDSNIP_MENU_LABEL, ADDSNIP_MENU_LABEL, true);
            log.Info("Changed to logged in buttons");
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////


        #region Close Forms
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Close "Publish snippet" window 
        /// </summary>
        public void ClosePublishSnippetWindow()
        {
            // close any obsolete publish window
            List<frmPublish> forms = FindWindows<frmPublish>();
            if (forms != null)
            {
                foreach (frmPublish form in forms)
                {
                    CloseForm(form);
                }
            }
        }


        /// <summary>
        /// Close "Add snippet" window 
        /// </summary>
        public void CloseAddSnippetWindow()
        {
            // close any obsolete add snippet window
            List<frmAddSnippet> forms = FindWindows<frmAddSnippet>();
            if (forms != null)
            {
                foreach (frmAddSnippet form in forms)
                {
                    CloseForm(form);
                }
            }
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////


        #region Forms Creation (Factory)
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Factory to create a form to add a new snippet
        /// </summary>
        /// <returns></returns>
        public IManageSnippetForm CreateAddSnippetForm()
        {
            return CreateManageSnippetForm(false);
        }

        /// <summary>
        /// Factory to create a form to view an existing snippet
        /// </summary>
        /// <returns></returns>
        public IManageSnippetForm CreateViewSnippetForm()
        {
            return CreateManageSnippetForm(true);
        }

        private IManageSnippetForm CreateManageSnippetForm(bool viewSnip)
        {
            frmAddSnippet window = new frmAddSnippet();
            if (viewSnip)
                window.Text = Resources.ViewSnippetWindowTitle;
            else
                window.Text = Resources.AddSnippetWindowTitle;

            return window;
        }

        /// <summary>
        /// Factory to create a form to publish a snippet
        /// </summary>
        /// <returns></returns>
        public IPublishSnippetForm CreatePublishSnippetForm()
        {
            frmPublish window = new frmPublish();
            window.Text = Resources.PublishSnippetWindowTitle;
            return window;
        }

        /// <summary>
        /// Factory to create a form to login the user
        /// </summary>
        /// <returns></returns>
        public ILoginForm CreateLoginForm()
        {
            frmLogin window = new frmLogin();
            window.Text = Resources.UserWindowTitle;
            return window;
        }

        ///// <summary>
        ///// Factory to create a form to Search snippets
        ///// </summary>
        ///// <returns></returns>
        //ISearchSnippetForm CreateSearchSnippetsForm();

        public T FindWindow<T>() where T : IAbstractForm
        {
            try
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                foreach (Form form in Application.OpenForms)
                {
                    Type formType = form.GetType();
                    Type tType = typeof(T);
                    if ((formType.Assembly == currentAssembly) && tType.IsInstanceOfType(form))
                    {
                        IAbstractForm retType = null;
                        if (tType == typeof(ISearchSnippetForm))
                            retType = (frmSearch)form;
                        else if (tType == typeof(ILoginForm))
                            retType = (frmLogin)form;
                        else if (tType == typeof(IManageSnippetForm))
                            retType = (frmAddSnippet)form;
                        else if (tType == typeof(IAboutForm))
                            retType = (frmAbout)form;
                        return (T)retType;
                    }
                }
            }
            catch { }

            return default(T);
        }

        public List<T> FindWindows<T>() where T : IAbstractForm
        {
            List<T> result = new List<T>();
            try
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                foreach (Form form in Application.OpenForms)
                {
                    Type formType = form.GetType();
                    Type tType = typeof(T);
                    if ((formType.Assembly == currentAssembly) && tType.IsInstanceOfType(form))
                    {
                        IAbstractForm retType = null;
                        if (tType == typeof(ISearchSnippetForm))
                            retType = (frmSearch)form;
                        else if (tType == typeof(ILoginForm))
                            retType = (frmLogin)form;
                        else if (tType == typeof(IManageSnippetForm))
                            retType = (frmAddSnippet)form;
                        else if (tType == typeof(IAboutForm))
                            retType = (frmAbout)form;
                        else
                            retType = (IAbstractForm)form;
                        if (retType != null)
                            result.Add((T)retType);
                    }
                }
            }
            catch { }

            return result;
        }

        #endregion
        /////////////////////////////////////////////////////////////////////////////////

        #endregion
        /////////////////////////////////////////////////////////////////////////////////


        #region Singleton Implementation
        /////////////////////////////////////////////////////////////////////////////////
        static private PluginBase s_current = null;
        static private object s_ObjSync = new object();


        /// <summary>
        ///     This return the one and only PluginBase (this is a singleton)
        /// </summary>
        static public PluginBase Current
        {
            get
            {
                if (s_current == null)
                {
                    lock (s_ObjSync)
                    {
                        if (s_current == null)
                            s_current = new PluginBase();
                    }
                }
                return s_current;
            }
        }
        
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
    }

    public class Demo
    {
        public static void Init()
        {
            PluginBase.CommandMenuInit();
        }

        public static void AddSnippet()
        {
            PluginBase.AddSnip();
        }

        public static void SearchSnippets()
        {
            PluginBase.SearchSnip();
        }

        public static void Login_Logout()
        {
            PluginBase.Login_Logout();
        }

        public static void About()
        {
            PluginBase.About();
        }
    }
}   
