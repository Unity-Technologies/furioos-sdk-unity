/*
Unity3d-TUIO connects touch tracking from a TUIO to objects in Unity3d.

Copyright 2011 - Mindstorm Limited (reg. 05071596)

Author - Simon Lerpiniere

This file is part of Unity3d-TUIO.

Unity3d-TUIO is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Unity3d-TUIO is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser Public License for more details.

You should have received a copy of the GNU Lesser Public License
along with Unity3d-TUIO.  If not, see <http://www.gnu.org/licenses/>.

If you have any questions regarding this library, or would like to purchase 
a commercial licence, please contact Mindstorm via www.mindstorm.com.
*/

using System;
using System.Collections.Generic;
using OSC;

/// <summary>
/// Parses TUIO messages into TUIO objects.
/// </summary>
namespace Tuio
{
	public class TuioParser
	{
	    public static int GetSequenceNumber(OSCBundle pack)
	    {
	        int result = -1;
	
	        List<OSCMessage> msgs = FindByCommand(pack, "/tuio/2Dcur","fseq");
	        if (msgs.Count > 0)
	        {
	            result = Convert.ToInt32((int)msgs[0].Values[1]);
	        }
	
	        return result;
	    }
	
	    public static Dictionary<int, Tuio2DCursor> GetCursors(OSCBundle pack)
	    {
	        Dictionary<int, Tuio2DCursor> result = new Dictionary<int, Tuio2DCursor>();
	
	        List<OSCMessage> cursors = FindByCommand(pack, "/tuio/2Dcur", "set");
	        foreach (OSCMessage msg in cursors)
	        {
	            Tuio2DCursor cur = new Tuio2DCursor(msg);
	            result.Add(cur.SessionID, cur);
	        }
	        return result;
	    }
		
		public static bool ContainsCursors(OSCBundle bundle)
		{
			foreach (object o in bundle.Values)
	        {
	            OSCMessage msg = (OSCMessage)o;
	            if (msg.Address == "/tuio/2Dcur")
				{
					return true;
				}
			}
			return false;
		}
	
	    public static List<int> GetAliveCursors(OSCBundle pack)
	    {
	        List<int> result = new List<int>();
	
	        List<OSCMessage> alivecmd = FindByCommand(pack, "/tuio/2Dcur", "alive");
	        if (alivecmd.Count > 0)
	        {
	            for (int i = 1; i < alivecmd[0].Values.Count; i++)
	            {
	                int id = (int)alivecmd[0].Values[i];
	                result.Add(id);
	
	            }
	        }
	
	        return result;
	    }
	
	    static List<OSCMessage> FindByCommand(OSCBundle bundle, string address, string cmd)
	    {
	        List<OSCMessage> result = new List<OSCMessage>();
	        foreach (object o in bundle.Values)
	        {
	            OSCMessage msg = (OSCMessage)o;
	            if (msg.Address == address)
	            {
	                if (msg.Values.Count > 0)
	                {
	                    if (msg.Values[0].ToString() == cmd)
	                    {
	                        result.Add(msg);
	                    }
	                }
	            }
	        }
	        return result;
	    }
	
	}
}