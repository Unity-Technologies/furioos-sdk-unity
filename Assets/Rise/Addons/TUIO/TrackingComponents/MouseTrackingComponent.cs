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
/// Uses mouse input to simulate TUIO using the mouse.  Used mostly for testing purposes.
/// </summary>
public class MouseTrackingComponent : TuioComponentBase, ITrackingComponent
{
	public static int[] MOUSE_BUTTONS = new int[] {0, 1, 2};
    
	/// <summary>
	/// Updates all touches with the latest TUIO received data 
	/// </summary>
	public override void updateTouches()
	{
		int[] mouseButtons = (from i in MOUSE_BUTTONS
		                    where Input.GetMouseButton(i)
		                    select i
		               ).ToArray();
		
		// Update touches in current collection
		foreach (int i in mouseButtons)
		{
			// Get the touch relating to the key
			Tuio.Touch t = null;
			if (TuioTouches.ContainsKey(i))
			{
				// It's not a new one
				t = TuioTouches[i];
				// Update it's position
				t.SetNewTouchPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
			}
			else
			{
				// It's a new one
				t = buildTouch(i);
				TuioTouches.Add(i, t);
			}
		}
	}
	
	Tuio.Touch buildTouch(int ID)
    {
        TouchProperties prop;
        prop.Acceleration = 0f;
        prop.VelocityX = 0f;
        prop.VelocityY = 0f;

        Vector2 p = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        Tuio.Touch t = new Tuio.Touch(ID, p);
        t.Properties = prop;

        return t;
    }
	
	public override void initialize()
	{
	}
	
	public override void Close()
	{
    }
}