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
/// Handles all data required for a TUIO cursor ready to send or receive on OSC.
/// </summary>
namespace Tuio
{
	public class Tuio2DCursor
	{
	    int sessionID;
	    float posX, posY, velX, velY, acc;
	
	    #region Constructor
	    public Tuio2DCursor()
	    {
	
	    }
	
	    public Tuio2DCursor(OSCMessage message)
	    {
	        this.SessionID = Convert.ToInt32(message.Values[1]);
	        this.PositionX = Convert.ToSingle(message.Values[2]);
	        this.PositionY = Convert.ToSingle(message.Values[3]);
	        this.VelocityX = Convert.ToSingle(message.Values[4]);
	        this.VelocityY = Convert.ToSingle(message.Values[5]);
	        this.Acceleration = Convert.ToSingle(message.Values[6]);
	    }
	    #endregion
	
	    #region Get OSC Message
	    public OSCMessage GetMessage()
	    {
	        OSCMessage msg = new OSCMessage("/tuio/2Dcur");
	        msg.Append("set");
	        msg.Append(this.SessionID);
	        msg.Append(this.PositionX);
	        msg.Append(this.PositionY);
	        msg.Append(this.VelocityX);
	        msg.Append(this.VelocityY);
	        msg.Append(this.Acceleration);
	        return msg;
	    }
	    #endregion
	
	    #region Session ID
	    public int SessionID
	    {
	        get { return sessionID; }
	        set { sessionID = value; }
	    }
	    #endregion
	
	    #region Position
	    public float PositionX
	    {
	        get { return posX; }
	        set { posX = value; }
	    }
	
	    public float PositionY
	    {
	        get { return posY; }
	        set { posY = value; }
	    }
	    #endregion
	
	    #region Velocity
	    public float VelocityX
	    {
	        get { return velX; }
	        set { velX = value; }
	    }
	
	    public float VelocityY
	    {
	        get { return velY; }
	        set { velY = value; }
	    }
	    #endregion
	
	    #region Acceleration
	    public float Acceleration
	    {
	        get { return acc; }
	        set { acc = value; }
	    }
	    #endregion
	
	    public bool IsEqual(Tuio2DCursor cursor)
	    {
	        return this.Acceleration == cursor.Acceleration
	            && this.PositionX == cursor.PositionX
	            && this.PositionY == cursor.PositionY
	            && this.SessionID == cursor.SessionID
	            && this.VelocityX == cursor.VelocityX
	            && this.VelocityY == cursor.VelocityY;
	    }
	}
}