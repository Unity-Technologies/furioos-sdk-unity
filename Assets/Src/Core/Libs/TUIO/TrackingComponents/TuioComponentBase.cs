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
/// Provides a base implementation for any component wanting to process or simulate TUIO tracking data.
/// </summary>
public abstract class TuioComponentBase : ITrackingComponent
{
	protected Dictionary<int, Tuio.Touch> TuioTouches =  new Dictionary<int, Tuio.Touch>();
	
	public double ScreenWidth = 1.0;
    public double ScreenHeight = 1.0;
	
	protected TuioComponentBase()
	{
		initialize();
	}
    
	public List<Tuio.Touch> getNewTouches() 
	{
		return TuioTouches.Values.Where(t => t.Status == TouchStatus.Began).ToList();	
	}
	
	public Dictionary<int, Tuio.Touch> AllTouches
	{
		get
		{
			return TuioTouches;
		}
	}
	
	public void BuildTouchDictionary()
	{
		deleteNonCurrentTouches();
		
		updateAllTouchesAsTemp();
		
		updateTouches();
		
		updateEndedTouches();
	}
	
	/// <summary>
	/// Deletes all old non-current touches from the last frame 
	/// </summary>
	protected void deleteNonCurrentTouches()
	{
		int[] deadTouches = (from Tuio.Touch t in TuioTouches.Values
				where !t.IsCurrent
				select t.TouchId).ToArray();
		foreach (int touchId in deadTouches) TuioTouches.Remove(touchId);
	}
	
	/// <summary>
	/// Update all remaining touches as temp (setting new points will reset this) 
	/// </summary>
	protected void updateAllTouchesAsTemp()
	{
		foreach (Tuio.Touch t in TuioTouches.Values) t.SetTemp();
	}
	
	/// <summary>
	/// Updates all touches with the latest TUIO received data 
	/// </summary>
	public abstract void updateTouches();
	
	/// <summary>
	/// Update non-current touches as ended 
	/// </summary>
	protected void updateEndedTouches()
	{
		var nonCurrent = from Tuio.Touch t in TuioTouches.Values
				where !t.IsCurrent
				select t;
		foreach (Tuio.Touch t in nonCurrent) t.Status = TouchStatus.Ended;
	}
	
	public abstract void initialize();
	
	public abstract void Close();
}