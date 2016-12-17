using UnityEngine;
using System.Collections;

namespace Rise.Features.GPS {
	public class GPSParameter {
		public Vector3 origin;
		public float orientation = 0.0f;
		public float scale = 1.0f;
		public Matrix originMatrix;
		public double originLongitude;
		public double originLatitude;
		public Vector3 latitudeDirection;
		public Vector3 longitudeDirection;
		
		public GPSParameter(){
			orientation = 0.0f;
			scale = 1.0f;
			origin = new Vector3();
			originMatrix = Matrix.Identity;
		}
	}
}