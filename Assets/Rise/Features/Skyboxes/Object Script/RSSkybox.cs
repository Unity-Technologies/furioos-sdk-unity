#pragma warning disable 0414
using UnityEngine;
using System;
using Rise.Core;

namespace Rise.Features.Skybox {
	[ExecuteInEditMode]
	public class RSSkybox : RSBehaviour 
	{
		public GameObject sun;
		public GameObject sky;
		public GameObject cup;

		public Material dynamicCupBaseMaterial   = null ;
		public Material dynamicSkyBaseMaterial   = null ;
		public Material staticCupBaseMaterial   = null ;
		public Material staticSkyBaseMaterial   = null ;

		public bool  isDynamic = false ;
		public bool useRealDate = true;

		public float automaticTimeSpeed = 1.0f;
		

		public float staticSunOrientation = 180;
		public float staticSunTilt = 45;
		public float staticSkyOrientation = 0;

		public float cupOrientation = 0;

		private double latitude = 50.63326238137421;
		private double longitude = 3.019075384902976;

		public Texture2D cupTexture;
		public Texture2D skyTexture;

		public Color cupColor  = new Color(1f, 1f, 1f, 1f) ;
		public Color ambientColor  = new Color(1f, 1f, 1f, 1f) ;

		public Material skyMaterial = null;
		public Material cupMaterial = null;
		private float skyRadius = 950.0f;
		private static readonly float earthTilt = 23.43333f;
		private bool  lastIsDynamic = false ;
		private Vector3 lastSunPos = new Vector3(0,0,0);

		private bool reset = true;
		private float hourTimeSinceSolstice = 0;

		private DateTime startDate = DateTime.Now;
		float startTimeSinceStartup;
		float startHourTimeSinceSolstice;
		
		private void Start()
		{

			skyRadius = (sky.GetComponent<Renderer>().bounds.max.x - sky.GetComponent<Renderer>().bounds.min.x) / 2;
			Reset();

		}

		public virtual void Awake ()
		{
			base.Init();
			if(useRealDate) SetDate (DateTime.Now);
		}

		public void Reset(){

			reset = true;
		}
		
		// Update is called once per frame
		public void Update () {

			if(lastIsDynamic !=isDynamic || reset){

				if(isDynamic){
					cup.GetComponent<Renderer>().sharedMaterial = new Material(dynamicCupBaseMaterial) ;
					sky.GetComponent<Renderer>().sharedMaterial = new Material (dynamicSkyBaseMaterial);
				}else{
					cup.GetComponent<Renderer>().sharedMaterial = new Material(dynamicCupBaseMaterial) ;
					sky.GetComponent<Renderer>().sharedMaterial = new Material (staticSkyBaseMaterial);
					sky.GetComponent<Renderer>().sharedMaterial.mainTexture = skyTexture;


				}

				cupMaterial = cup.GetComponent<Renderer>().sharedMaterial;
				cupMaterial.mainTexture = cupTexture;
				cupMaterial.color = cupColor ;
				cupMaterial.SetColor ("_AmbientColor", ambientColor);
				cupMaterial.name  = "Cup" ;

				skyMaterial = sky.GetComponent<Renderer>().sharedMaterial;
				skyMaterial.name = "Sky";

				lastIsDynamic = isDynamic;
			}

			if (isDynamic) {


				hourTimeSinceSolstice = startHourTimeSinceSolstice + automaticTimeSpeed * (Time.realtimeSinceStartup - startTimeSinceStartup) / 3600.0f;

				Matrix sunMatrix  = Matrix.Identity;
				
				Matrix3DTools.rotateLocal(1, 0, 0, (float)-latitude, ref sunMatrix);
				Matrix3DTools.rotateLocal(0, 0, 1, (float)longitude + 360.0f * hourTimeSinceSolstice / 23.93447222f, ref sunMatrix);
				Matrix3DTools.rotateLocal(1, 0, 0, -earthTilt, ref sunMatrix);
				Matrix3DTools.rotateLocal(0, 0, 1, - 360.0f * hourTimeSinceSolstice / 8765.8125f, ref sunMatrix);
				Matrix3DTools.translateLocal(0, skyRadius, 0, ref sunMatrix);



				if (sun != null) {
					//add 0.01f to compensate atmosphere diffraction
					sun.transform.localPosition = new Vector3((float)sunMatrix.M41,(float)sunMatrix.M42 + skyRadius * 0.01f,(float)sunMatrix.M43);
					sun.transform.LookAt (new Vector3 (0, 0, 0));
				}

			}
			else
			{
				sun.transform.localPosition = new Vector3
				(
					(float) skyRadius * Mathf.Cos (staticSunTilt * Mathf.Deg2Rad)* Mathf.Sin ((staticSkyOrientation + staticSunOrientation) * Mathf.Deg2Rad),
					(float) skyRadius * Mathf.Sin (staticSunTilt * Mathf.Deg2Rad),
					(float) skyRadius * Mathf.Cos (staticSunTilt * Mathf.Deg2Rad) * Mathf.Cos ((staticSkyOrientation + staticSunOrientation) * Mathf.Deg2Rad)
				);
				sun.transform.LookAt (new Vector3 (0, 0, 0));
			}

			if(lastSunPos != sun.transform.localPosition || reset)
			{
				Vector3 normalizedPos = sun.transform.localPosition.normalized;
				Vector4 sunOrientation = new Vector4 (normalizedPos.x,normalizedPos.y,normalizedPos.z,1.0f);

				float nightIntensity = (1 - this.realSmoothStep (-0.40f, 0.02f, sunOrientation.y));
				float dayIntensity = this.realSmoothStep (0.01f, 0.50f, sunOrientation.y);
				float morningIntensity = (this.realSmoothStep (-0.20f, 0.20f, sunOrientation.x)) * this.realSmoothStep (-0.15f, 0.25f, sunOrientation.y) * (1.0f - dayIntensity);
				float eveningIntensity = (1.0f - this.realSmoothStep (-0.20f, 0.20f, sunOrientation.x)) * this.realSmoothStep (-0.15f, 0.25f, sunOrientation.y) * (1.0f - dayIntensity);
				
				float sunIntensity = this.realSmoothStep (0.01f, 0.03f, sunOrientation.y);

				/*
				RenderSettings.ambientLight = new Color 
				(
					0.10f * morningIntensity + 0.25f * dayIntensity + 0.12f * eveningIntensity + 0.06f,
					0.10f * morningIntensity + 0.31f * dayIntensity + 0.10f * eveningIntensity + 0.01f * nightIntensity + 0.06f,
					0.15f * morningIntensity + 0.37f * dayIntensity + 0.10f * eveningIntensity + 0.03f * nightIntensity + 0.06f,
					1.0f
				);

			

				RenderSettings.fogColor = RenderSettings.ambientLight / 2 ;
				*/
				/*RenderSettings.fogColor = new Color (
					0.10f * morningIntensity + 0.33f * dayIntensity + 0.12f * eveningIntensity + 0.06f,
					0.10f * morningIntensity + 0.40f * dayIntensity + 0.10f * eveningIntensity + 0.06f,
					0.15f * morningIntensity + 0.44f * dayIntensity + 0.10f * eveningIntensity + 0.06f,
					1.0f
				);*/


				if (sun != null) {
					sun.GetComponent<Light>().color = new Color (0.4f + 0.6f * dayIntensity, 0.3f + 0.6f * dayIntensity, 0.2f + 0.65f * dayIntensity, 1.0f);
					//sun.light.color = new Color (1.0f - RenderSettings.ambientLight.r, 1.0f - RenderSettings.ambientLight.g,1.0f - RenderSettings.ambientLight.b,1.0f);
					sun.GetComponent<Light>().intensity = sunIntensity * ( 0.90f - Mathf.Max (RenderSettings.ambientLight.r,RenderSettings.ambientLight.g,RenderSettings.ambientLight.b));

					LensFlare lensFlare = sun.GetComponent<LensFlare>();
					lensFlare.color = sun.GetComponent<Light>().color ;
					lensFlare.brightness = sunIntensity;
					
					if (isDynamic){
						skyMaterial.SetColor ("_SunColor", sun.GetComponent<Light>().color / 3.0f);
					}

					//cupMaterial.SetColor ("_AmbientColor", RenderSettings.ambientLight * (1.25f - 0.5f*sunIntensity) + sunIntensity * sun.light.color * 0.75f);
				}

				if (isDynamic){
					skyMaterial.SetVector ("_AmbianceCoefficients", new Vector4 (nightIntensity, morningIntensity, dayIntensity, eveningIntensity));
					skyMaterial.SetVector ("_SunOrientation", sunOrientation);
				}else{
					skyMaterial.SetVector ("_SunOrientation", sunOrientation);
					skyMaterial.SetFloat ("_OrientationOffset", -staticSkyOrientation/360.0f);
				}

				cupMaterial.SetFloat ("_OrientationOffset", -cupOrientation/360.0f);

				lastSunPos=sun.transform.localPosition;

			}
				
			reset = false;
		}

		public void SetDate(DateTime time){
			DateTime compTime = new DateTime(time.Year, 12, 20, 12, 0, 0, DateTimeKind.Utc);
			TimeSpan ts = time.ToUniversalTime() - compTime;

			startDate = time;
			startTimeSinceStartup = Time.realtimeSinceStartup;
			startHourTimeSinceSolstice = (float)ts.TotalHours;

			//Debug.Log (startDate.ToUniversalTime());
		}

		static float realSmoothStep(float min,float max, float v){
			if(v<min) return 0;
			else if(v>max) return 1;
			else return -0.5f * Mathf.Cos( Mathf.PI * (v-min) / (max-min) ) + 0.5f;
		}
	}
}