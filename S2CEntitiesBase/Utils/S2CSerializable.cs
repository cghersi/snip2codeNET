//------------------------------------------------------------------------------
// (c) 2011 snip2code inc.
// This software is property of snip2code inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Snip2Code.Utils
{
    /// <summary>
    /// This interface represents an object used to pass serialized content between client and server 
    /// for S2C web service communication
    /// </summary>
    public interface S2CSerializable
    {
        object Data { get; set; }
    }
}
