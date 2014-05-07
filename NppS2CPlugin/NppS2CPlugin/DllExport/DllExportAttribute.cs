//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;

namespace NppPlugin.DllExport
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    partial class DllExportAttribute : Attribute
    {
        public DllExportAttribute()
        {
        }
        public DllExportAttribute(string exportName)
            : this(exportName, CallingConvention.StdCall)
        {
        }
        public DllExportAttribute(string exportName, CallingConvention callingConvention)
        {
            ExportName = exportName;
            CallingConvention = callingConvention;
        }
        CallingConvention _callingConvention;
        public CallingConvention CallingConvention
        {
            get { return _callingConvention; }
            set { _callingConvention = value; }
        }
        string _exportName;
        public string ExportName
        {
            get { return _exportName; }
            set { _exportName = value; }
        }
    }
}