using UnityEngine;
using System.Collections;

namespace Rise.Core {
	public class RSInputTouch {
		private Vector2 pos;
		private Vector2 lastPos;
		private Vector2 delta;
		private float distance;
		
		private Vector2 normalizedPos;
		private Vector2 normalizedDelta;
		
		private bool isActive;

		public int Id {
			get;
			set;
		}
		
		public float Distance {
			get {
				return distance;
			}
		}
		
		public Vector2 Position {
			get {
				return pos;
			}
		}
		
		public Vector2 NormalizedPosition {
			get {
				return normalizedPos;
			}
		}
		
		public Vector2 Delta {
			get {
				return delta;
			}
		}
		
		public Vector2 NormalizedDelta {
			get {
				return normalizedDelta;
			}
		}
		
		public bool IsActive {
			get {
				return isActive;
			}
			set {
				if(value && !isActive) {
					HasGoneActive = true;
					distance = 0;
				}

				if(!value && isActive) {
					HasGoneInactive = true;
				}

				isActive = value;
			}
		}
		
		public bool HasGoneActive {
			get;
			private set;
		}
		
		public bool HasGoneInactive {
			get;
			private set;
		}
		
		public bool IsMoving {
			get { 
				return IsActive && !HasGoneActive;
			}
		}

		public RSInputTouch() {
			Id = -1;
		}

		public void UpdatePosition(float px, float py) {
			pos = new Vector2(Mathf.Clamp(px, 0, Screen.width), Mathf.Clamp(py, 0, Screen.height));
			normalizedPos = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
			delta = (pos - lastPos);
			normalizedDelta = new Vector2(delta.x / Screen.width, delta.y / Screen.height);
			if(IsMoving) {
				distance += delta.magnitude;
			}
		}

		public void Move(float deltaX, float deltaY) {
			delta = new Vector2(Mathf.Clamp(deltaX, -pos.x, Screen.width - pos.x), Mathf.Clamp(deltaY, -pos.y, Screen.height - pos.y));
			normalizedDelta = new Vector2(delta.x / Screen.width, delta.y / Screen.height);
			pos = pos + delta;
			normalizedPos = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
			if(IsMoving) {
				distance += delta.magnitude;
			}
		}

		public void ResetLastState() {
			lastPos = pos;
			HasGoneActive = false;
			HasGoneInactive = false;
		}
	}
}