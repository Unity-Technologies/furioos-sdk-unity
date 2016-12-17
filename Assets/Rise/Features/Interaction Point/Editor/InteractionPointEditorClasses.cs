using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Rise.Features.InteractionPoint {
	public class PanTiltRollDistance
	{
		private bool hasBeenEdited = false;
		public bool HasBeenEdited {
			get { return hasBeenEdited; }
		}

		private bool startPositionHasBeenEdited = false;
		public bool StartPositionHasBeenEdited {
			get { return startPositionHasBeenEdited; }
		}

		private float pan;
		public float Pan {
			set {
				hasBeenEdited = true;
				startPositionHasBeenEdited = true;
				pan = value;
			}
			get { return pan; }
		}
		
		private float tilt;
		public float Tilt {
			set {
				hasBeenEdited = true;
				startPositionHasBeenEdited = true;
				tilt = value;
			}
			get { return tilt; }
		}
		
		private float roll;
		public float Roll {
			set {
				hasBeenEdited = true;
				startPositionHasBeenEdited = true;
				roll = value;
			}
			get { return roll; }
		}
		
		private float distance;
		public float Distance {
			set {
				hasBeenEdited = true;
				startPositionHasBeenEdited = true;
				distance = value;
			}
			get { return distance; }
		}

		private float minDistance = -1;
		public float MinDistance {
			set {
				hasBeenEdited = true;
				minDistance = value;
			}
			get { return minDistance; }
		}

		private float maxDistance = -1;
		public float MaxDistance {
			set {
				hasBeenEdited = true;
				maxDistance = value;
			}
			get { return maxDistance; }
		}
	}
}