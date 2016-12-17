using UnityEngine;
using System.Collections;
using Rise.Core;

namespace Rise.Features.GPS {
	public class GPSLandmark : RSBehaviour {
		public double latitude = 50.63326238137421;
		public double longitude = 3.019075384902976;

		public Vector3 Position{
			get { return transform.position;}
		}
	}
}