using UnityEngine;
using System.Collections;

public class EditorScatterData : ScriptableObject{
	
		public GameObject srcObject;
		public GameObject[] sampleObjects;
		public float densityToScale = 1.0f;
		public float rotationRandomize = 360f;
		public float scaleMin = 0.5f;
		public float scaleMax = 1.50f;
	
		public EditorScatterData(){
			sampleObjects = new GameObject[0];
		}

	
}
