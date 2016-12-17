using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rise.Core;

namespace Rise.Features.GPS {
	public class RSGPSManager : RSSceneManagerModule {
		private GPSParameter sceneParameters = new GPSParameter();
		private const float earthRadius = 6371000;

		bool sceneIsLocated;

		public float SceneOrientation {
			get { return sceneParameters.orientation; }
		}
		
		public float SceneScale {
			get { return sceneParameters.scale; }
		}
		
		public bool SceneIsLocated {
			get { return sceneIsLocated; }
		}
		
		// Use this for initialization
		void Start () {
			List<GPSLandmark> landmarks = RSSceneManager.GetAllInstances<GPSLandmark>();
			
			List<GPSParameter> parameters = new List<GPSParameter>();

			for(int i = 0;i < landmarks.Count - 1 ;i++){
				for(int j = i+1;j < landmarks.Count ;j++){
					parameters.Add(getSceneParameters(landmarks[i],landmarks[j]));
				}
			}
			
			if(parameters.Count>1){
				
				sceneParameters.scale=0;
				sceneParameters.orientation=0;
				sceneParameters.origin=new Vector3();
				
				
				
				for(int i = 0;i < parameters.Count ;i++){
					sceneParameters.scale+=parameters[i].scale/parameters.Count;
					sceneParameters.orientation+=parameters[i].orientation/parameters.Count;
				}
				
				int bestIndex = -1;
				float smallestOrientationDiff = float.MaxValue;
				for(int i = 0;i < parameters.Count ;i++){
					float diff = Mathf.Abs(parameters[i].orientation - sceneParameters.orientation);
					if(diff<smallestOrientationDiff){
						smallestOrientationDiff = diff;
						bestIndex = i;
					}
				}
				
				if(bestIndex!=-1){
					sceneParameters.origin = parameters[bestIndex].origin;
					sceneParameters.originLatitude = parameters[bestIndex].originLatitude;
					sceneParameters.originLongitude = parameters[bestIndex].originLongitude;
					sceneParameters.originMatrix = getGPSOffsetMatrix(sceneParameters.originLatitude, sceneParameters.originLongitude,sceneParameters.orientation);

					float latitudeNorm = 1/(earthRadius*Mathf.Deg2Rad);
					float longitudeNorm = 1/(earthRadius*Mathf.Cos((float)sceneParameters.originLatitude*Mathf.Deg2Rad)*Mathf.Deg2Rad);
					sceneParameters.latitudeDirection = new Vector3(Mathf.Sin(sceneParameters.orientation*Mathf.Deg2Rad)*latitudeNorm,0.0f,Mathf.Cos(sceneParameters.orientation*Mathf.Deg2Rad)*latitudeNorm);
					sceneParameters.longitudeDirection = new Vector3(Mathf.Cos(sceneParameters.orientation*Mathf.Deg2Rad)*longitudeNorm,0.0f,-Mathf.Sin(sceneParameters.orientation*Mathf.Deg2Rad)*longitudeNorm);
				}
				sceneIsLocated = true;

				Debug.Log("GPS Manager started. SceneOrientation : " + sceneParameters.orientation + ", scale : " + sceneParameters.scale + ", latitudeDirection : " + sceneParameters.latitudeDirection + ", longitudeDirection : " + sceneParameters.longitudeDirection);
				
			} else {
				sceneIsLocated = false;
				Debug.LogWarning("GPS Manager started, but there are not enought GPS Landmarks to determine the scene position");
			}
		}

		public void printMatrix(ref Matrix m) {
			Debug.Log("[ " + m.M11 + ", " + m.M12 + ", " + m.M13 + ", " + m.M14 + " ][ " + 
			m.M21 + ", " + m.M22 + ", " + m.M23 + ", " + m.M24 + " ][ " +
			m.M31 + ", " + m.M32 + ", " + m.M33 + ", " + m.M34 + " ][ " +
			m.M41 + ", " + m.M42 + ", " + m.M43 + ", " + m.M44 +  " ]" );
			
		}
		
		private Matrix getGPSOffsetMatrix(double latitude, double longitude, float orientation = 0, float altitude = 0) {

	        Matrix offsetMatrix  = Matrix.Identity;
			
			Matrix3DTools.rotateLocal(0, 1, 0, -orientation, ref offsetMatrix);
			Matrix3DTools.translateLocal(0, -earthRadius - altitude, 0, ref offsetMatrix);
	        Matrix3DTools.rotateLocal(1, 0, 0, -latitude, ref offsetMatrix);
	        Matrix3DTools.rotateLocal(0, 0, 1, longitude, ref offsetMatrix);
			
			return offsetMatrix; 

	    }
		
		public Vector3 getGPSPositioning(double latitude, double longitude, float orientation = 0, float altitude = 0) {

	        return sceneParameters.origin + getGPSPositioning(ref sceneParameters.originMatrix, latitude, longitude, orientation, altitude) * sceneParameters.scale;
	    
	    }

		public void getCoordinates(Vector3 point,out double latitude,out double longitude) {

			Vector3 direction = new Vector3(point.x-sceneParameters.origin.x,0.0f,point.z-sceneParameters.origin.z);
			latitude = sceneParameters.originLatitude + Vector3.Dot(direction,sceneParameters.latitudeDirection);
			longitude = sceneParameters.originLongitude + Vector3.Dot(direction,sceneParameters.longitudeDirection);

		}
		
		
		private Vector3 getGPSPositioning(ref Matrix offsetMatrix, double latitude, double longitude, float orientation = 0, float altitude = 0) {
	        Matrix m = new Matrix();
	        Matrix3DTools.copy(ref offsetMatrix , ref m);

	        Matrix3DTools.rotateLocal(0, 0, 1, -longitude, ref m);
	        Matrix3DTools.rotateLocal(1, 0, 0, latitude, ref m);
			Matrix3DTools.translateLocal(0, earthRadius + altitude, 0, ref m);
	        Matrix3DTools.rotateLocal(0, 1, 0, orientation, ref m);

			return (new Vector3((float)m.M41,(float)m.M42,(float)m.M43));
	    }
		
		private GPSParameter getSceneParameters(GPSLandmark gpsLandmark1, GPSLandmark gpsLandmark2){
			GPSParameter gpsParam = new GPSParameter();	

			if(gpsLandmark1!=null && gpsLandmark2!=null){
				
				gpsParam.origin = gpsLandmark1.Position;
				gpsParam.originLatitude = gpsLandmark1.latitude;
				gpsParam.originLongitude = gpsLandmark1.longitude;
				
				Matrix gpsOriginMatrix = getGPSOffsetMatrix(gpsLandmark1.latitude, gpsLandmark1.longitude);
				
				Vector3 landmark2TmpPosition = getGPSPositioning(ref gpsOriginMatrix, gpsLandmark2.latitude, gpsLandmark2.longitude);
				
				Vector3 landmarksDirection = gpsLandmark2.Position - gpsLandmark1.Position;
				Vector3 desiredDirection = landmark2TmpPosition;
				
				float sceneDistance = landmarksDirection.magnitude;
				float desiredDistance = desiredDirection.magnitude;
				
				landmarksDirection.y=0;
				desiredDirection.y=0;
				landmarksDirection.Normalize();
				desiredDirection.Normalize();


				float landmarksOrientation = Mathf.Acos(landmarksDirection.z)*Mathf.Rad2Deg;
				if(landmarksDirection.x<0)landmarksOrientation = 360-landmarksOrientation;

				float desiredOrientation = Mathf.Acos(desiredDirection.z)*Mathf.Rad2Deg;
				if(desiredDirection.x<0)desiredOrientation = 360-desiredOrientation;
				
				gpsParam.orientation = landmarksOrientation-desiredOrientation;
				while(gpsParam.orientation>180)gpsParam.orientation-=360f;
				while(gpsParam.orientation<-180)gpsParam.orientation+=360f;
				
				gpsParam.scale = sceneDistance/desiredDistance;
		
				Debug.Log(gpsLandmark1.transform.name + " to " + gpsLandmark2.transform.name + " : landmarksOrientation : " + landmarksOrientation + " desiredOrientation : " + desiredOrientation + " sceneOrientation : " + gpsParam.orientation + " scale : " + gpsParam.scale);
				

				gpsParam.originMatrix = getGPSOffsetMatrix(gpsLandmark1.latitude, gpsLandmark1.longitude,gpsParam.orientation);
				
			}else{
				Debug.Log("Landmarks don't both have a GPSLandmark component");
			}
			
			return gpsParam;
			
		}
	}
}