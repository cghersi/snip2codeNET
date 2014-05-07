//------------------------------------------------------------------------------
// (c) 2011-2014 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NppPluginNet
{
    public static class Utilities
    {
        public static Color s_lightGrayS2C = Color.FromArgb(0x9c, 0x9b, 0x9b); //#9c9b9b
        public static Color s_orangeS2C = Color.FromArgb(0xdc, 0x91, 0x1b);
        public static Font s_regularFont = new Font("Verdana", 10, FontStyle.Regular);

        public static string GetResourceLocalizedMsg<T>(T enumCode) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            string res = string.Format("{0}_{1}", typeof(T).Name, enumCode);
            return Properties.Resources.ResourceManager.GetString(res, CultureInfo.CurrentCulture);
        }
    }
}
