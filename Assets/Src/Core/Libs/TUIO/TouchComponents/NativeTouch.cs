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

using UnityEngine;
using System.Collections;

namespace Tuio.Native
{
	/// <summary>
	/// Used as temporary storage for all Touch objects.  Exact copy of the UnityEgine.Touch struct
	/// allowing it to be Marshalled into a UnityEngine.Touch object.
	/// NOTE: This class will need to be changed if the UnityEngine.Touch class changes.
	/// </summary>
	public struct Touch
	{
		private int m_FingerId;
		private Vector2 m_Position;
		private Vector2 m_PositionDelta;
		private float m_TimeDelta;
		private int m_TapCount;
		private TouchPhase m_Phase;
		public int fingerId
		{
			get
			{
				return this.m_FingerId;
			}
		}
		public Vector2 position
		{
			get
			{
				return this.m_Position;
			}
		}
		public Vector2 deltaPosition
		{
			get
			{
				return this.m_PositionDelta;
			}
		}
		public float deltaTime
		{
			get
			{
				return this.m_TimeDelta;
			}
		}
		public int tapCount
		{
			get
			{
				return this.m_TapCount;
			}
		}
		public TouchPhase phase
		{
			get
			{
				return this.m_Phase;
			}
		}
		
		public Touch (int Id, Vector2 pos, Vector2 posDelta, float timeDelta, int taps, TouchPhase p)
		{
			m_FingerId = Id;
			m_Position = pos;
			m_PositionDelta = posDelta;
			m_TimeDelta = timeDelta;
			m_TapCount = taps;
			m_Phase = p;
		}
	}
}