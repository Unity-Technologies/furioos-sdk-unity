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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Tuio
{
    /// <summary>
    /// Handles simple data about a touch including status, properties and ID.
    /// Specific to TUIO Input and not used outside of the TUIO code.
    /// </summary>
    public class Touch
    {
        public readonly int TouchId;
		
		long deltaTime = 0;
		Vector2 deltaDistance = Vector2.zero;
		
		Stopwatch sw;
		
        /// <summary>
        /// Holds detailed information about this touch (e.g. Velocity).
        /// </summary>
        public TouchProperties Properties;

        public Touch(int Id, Vector2 point)
        {
            TouchId = Id;
            TouchPoint = point;
			RawPoint = point;
			IsCurrent = true;
			sw = Stopwatch.StartNew();
			TimeAdded = Time.time;
        }
		
		public Touch(int Id, Vector2 point, Vector2 rawPoint)
        {
            TouchId = Id;
            TouchPoint = point;
			RawPoint = rawPoint;
			IsCurrent = true;
			sw = Stopwatch.StartNew();
			TimeAdded = Time.time;
        }
		
		public UnityEngine.Touch ToUnityTouch()
		{
			Native.Touch t = this.ToNativeTouch();
			UnityEngine.Touch uT;
			
			IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(t));
			
			try
			{
				Marshal.StructureToPtr(t, pnt, false);
				uT = (UnityEngine.Touch)Marshal.PtrToStructure(pnt, typeof(UnityEngine.Touch));
			}
			finally
			{
				Marshal.FreeHGlobal(pnt);
			}
			
			return uT;
		}
		
		Native.Touch ToNativeTouch()
		{
			TouchPhase p;
			switch (Status)
			{
			case TouchStatus.Began:
				p = TouchPhase.Began;
				break;
			case TouchStatus.Moved:
				p = TouchPhase.Moved;
				break;
			case TouchStatus.Ended:
				p = TouchPhase.Ended;
				break;
			case TouchStatus.Stationary:
				p = TouchPhase.Stationary;
				break;
			default:
				p = TouchPhase.Canceled;
				break;
			}
			
			Native.Touch t = new Native.Touch (	
				TouchId,
				TouchPoint,
				DeltaDistance,
				DeltaTime,
				1,
				p
			);
			
			return t;
		}
		
		public Vector2 DeltaDistance
		{
			get
			{
				return deltaDistance;
			}
		}
		
		public long DeltaTime
		{
			get
			{
				return deltaTime;
			}
		}
		
		public TouchStatus Status
		{
			get;
			set;
		}

        public Vector2 TouchPoint
        {
            get;
            set;
        }
		
		public Vector2 RawPoint
        {
            get;
            set;
        }
		
		public bool IsCurrent
		{
			get;
			set;
		}
		
		public float TimeAdded
		{
			get;
			set;
		}

        public void SetMoving()
        {
            Status = TouchStatus.Moved;
        }
        public void SetHeld()
        {
            Status = TouchStatus.Stationary;
        }
		
		/// <summary>
		/// Use this function before updating all touches to know which ones have not been updated 
		/// </summary>
		public void SetTemp()
		{
			IsCurrent = false;
		}
        
        /// <summary>
        /// Updates the touch point to a new one without messing up other internal variables.
        /// Use this function when a touch point has moved.
        /// </summary>
        /// <param name="p">New point of the touch location.</param>
        public void SetNewTouchPoint(Vector2 p)
        {
			// We're being updated so must be current
			IsCurrent = true;
			
			deltaTime = sw.ElapsedTicks;
			sw.Reset();
			sw.Start();
			
			// If we've not moved then we're Held
			if (p != TouchPoint) 
			{
				SetMoving();
				deltaDistance = p - TouchPoint;
				TouchPoint = p;
				RawPoint = p;
			}
			else 
			{
				SetHeld();
			}
        }
		
		public void SetNewTouchPoint(Vector2 p, Vector2 rawPoint)
        {
			// We're being updated so must be current
			IsCurrent = true;
			
			deltaTime = sw.ElapsedTicks;
			sw.Reset();
			sw.Start();
			
			// If we've not moved then we're Held
			if (p != TouchPoint) 
			{
				SetMoving();
				deltaDistance = p - TouchPoint;
				TouchPoint = p;
				RawPoint = rawPoint;
			}
			else 
			{
				SetHeld();
			}
        }
    }
}
