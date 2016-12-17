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

using System.Collections;
using UnityEngine;
using System.Linq;

/// <summary>
/// Provides mouse input as UnityEngine.Touch objects for receiving touch information.
/// Must be attached to a GameObject in the Hierachy to be used.
/// 
/// Provides exactly the same interface as UnityEngine.Input regarding touch data
/// allowing any code using UnityEngine.Input to use TuioInput instead.
/// </summary>
public class MouseInput : MonoBehaviour
{
	static TuioComponentBase mouseSim;
	
	static Touch[] frameTouches = new Touch[0];
	
	public static readonly bool multiTouchEnabled = true;
	
	public static int touchCount
	{
		get;
		private set;
	}
	
	void Awake()
	{
		mouseSim = InitTracking(new MouseTrackingComponent());
	}
	
	void Update()
	{
		TuioComponentBase tr = mouseSim;
		UpdateTouches(tr);
	}
	
	void UpdateTouches(TuioComponentBase tr)
	{
		tr.BuildTouchDictionary();
		frameTouches = tr.AllTouches.Values.Select(t => t.ToUnityTouch()).ToArray();
		touchCount = frameTouches.Length;
	}
	
	TuioComponentBase InitTracking(TuioComponentBase tr)
	{
		tr.ScreenWidth = Camera.main.pixelWidth;
		tr.ScreenHeight = Camera.main.pixelHeight;
		return tr;
	}
	
	public static Touch GetTouch(int index)
	{
		return frameTouches[index];		
	}
	
	public static Touch[] touches
	{
		get
		{
			return frameTouches;
		}
	}
	
	void OnApplicationQuit()
	{
		if (mouseSim != null) mouseSim.Close();
	}
}