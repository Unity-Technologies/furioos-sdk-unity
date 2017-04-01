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
using UnityEngine;
using Tuio;
using System.Linq;

/// <summary>
/// Uses the TUIO Tracking component to receive touch information over TUIO.
/// </summary>
public class WM_TrackingComponent : TuioComponentBase, ITrackingComponent
{
	private static WM_Tracking tracking = null;
	
	public WM_TrackingComponent()
	{
	}
	
	/// <summary>
	/// Updates all touches with the latest TUIO received data 
	/// </summary>
	public override void updateTouches()
	{
		WM_Tracking.TOUCHINPUT[] cursors = tracking.GetTouchArray();
		
		// Update touches in current collection
		foreach (WM_Tracking.TOUCHINPUT cursor in cursors)
		{
			// Get the touch relating to the key
			Tuio.Touch t = null;
			if (TuioTouches.ContainsKey(cursor.dwID))
			{
				// It's not a new one
				t = TuioTouches[cursor.dwID];
				// Update it's position
				t.SetNewTouchPoint(getScreenPoint(cursor), getRawPoint(cursor));
			}
			else
			{
				// It's a new one
				t = buildTouch(cursor);
				TuioTouches.Add(cursor.dwID, t);
			}
		}
	}
	
	Tuio.Touch buildTouch(WM_Tracking.TOUCHINPUT cursor)
    {
        Vector2 p = getScreenPoint(cursor);
		Vector2 raw = getRawPoint(cursor);

        Tuio.Touch t = new Tuio.Touch(cursor.dwID, p, raw);

        return t;
    }

    Vector2 getRawPoint(WM_Tracking.TOUCHINPUT data)
    {
		WM_Tracking.POINT p = tracking.getClientPoint(data);
		Vector2 position = new Vector2(p.X, p.Y);
        return position;
    }
	
	Vector2 getScreenPoint(WM_Tracking.TOUCHINPUT data)
    {
		WM_Tracking.POINT p = tracking.getClientPoint(data);
		Vector2 position = new Vector2(p.X, p.Y);
		
		float x1 = getScreenPoint(position.x,
            ScreenWidth, false);
        float y1 = getScreenPoint(position.y,
            ScreenHeight, true);

        Vector2 t = new Vector2(x1, y1);
        return t;
    }
	
	float getScreenPoint(float xOrY, double screenDimension, bool flip)
    {
		if (flip) xOrY = (float)screenDimension - xOrY;
        return xOrY;
    }
	
	public override void initialize()
	{		
		if (tracking == null) tracking = new WM_Tracking();
		tracking.Start();
	}
	
    public override void Close()
	{
		tracking.Close();
    }
}