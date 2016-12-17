using UnityEngine;
using UnityEditor;
using System.Collections;







public class EditorScatter : EditorWindow
{
	
	
	
	/*
	
	static SerializedObject serializedData;
	
	
	
	static EditorScatterData data;

	[MenuItem("Editor/Stereograph/Scatter")]
	public static void EditorScatterWindow()
	{
		data = (EditorScatterData)EditorScatterData.CreateInstance(typeof(EditorScatterData));
		serializedData = new SerializedObject(data);
		EditorScatter window = (EditorScatter)EditorWindow.GetWindow(typeof(EditorScatter));
	}
	
	
	void OnGUI() {
		if(serializedData!=null){
			serializedData.Update();
			
			EditorGUIUtility.LookLikeInspector();
	
	        EditorGUILayout.LabelField ("Scatter", EditorStyles.boldLabel);
			srcObject = (GameObject)EditorGUILayout.ObjectField("Source surface object", data.srcObject, typeof(GameObject), true);
			
			
			SerializedProperty sampleObjects = serializedData.FindProperty("sampleObjects");
			
			bool showChildren;
			while(true){
		        showChildren = EditorGUILayout.PropertyField(sampleObjects);
				if(!sampleObjects.NextVisible(showChildren)) break;
			}

			rotationRandomize = EditorGUILayout.Slider("Rotation random range",data.rotationRandomize, 0.0f, 360.0f);
			
			densityToScale = EditorGUILayout.Slider("Density to scale",data.densityToScale, 0.0f, 1.0f);
			
			serializedData.ApplyModifiedProperties();
			
			//vegetationId = EditorGUILayout.LayerField("vegetations", vegetationEnum, vegetationsNames);
			if(GUILayout.Button("Scatter !")) {
				//SetVegetation();
			}
			
		}

		
		
		//SetVegetationRotation();
	}
	
	
	public void Scatter (EditorScatterData data) {
		
		instantiateGameObject = new GameObject[sampleObjects.Length];
		sampleMeshes = new Mesh[sampleObjects.Length];

		for(int i = 0; i< sampleObjects.Length;i++){
			instantiateGameObject[i] = (GameObject)Instantiate(sampleObjects[i]);
			sampleMeshes[i] = instantiateGameObject[i].GetComponent<MeshFilter>().mesh;
		}
		
		
		
		srcMesh = gameObject.GetComponent<MeshFilter>().mesh;
		triangles = srcMesh.triangles;
		float[] triangleAreas = new float[triangles.Length/3];
		float totalArea = 0;
		
		for(int i = 0;i<triangles.Length;i+=3){
			
			Vector3 v1 = srcMesh.vertices[triangles[i]];
			Vector3 v2 = srcMesh.vertices[triangles[i+1]];
			Vector3 v3 = srcMesh.vertices[triangles[i+2]];
			
			Vector3 v12 = v2-v1;
			Vector3 v13 = v3-v1;
			
			float hp = (v12.x*v13.x + v12.y*v13.y + v12.z*v13.z) /(v12.x*v12.x + v12.y*v12.y + v12.z*v12.z);
			
			Vector3 hb = v1 + v12 * hp;
			Vector3 h = v3-hb;
			
			triangleAreas[i/3] = v12.magnitude * h.magnitude / 2;
			totalArea+=triangleAreas[i/3];
		}
		
		float fracPart = 0;
		int scatterIndex = 0;
		int totalItemsToAdd = 0;
		
		for(int i = 0;i<triangles.Length;i+=3){
			
			Mesh newMesh = null;
			GameObject newGameObject = null;
		
			Vector3 v1 = srcMesh.vertices[triangles[i]];
			Vector3 v2 = srcMesh.vertices[triangles[i+1]];
			Vector3 v3 = srcMesh.vertices[triangles[i+2]];
			
			Vector2 uv1 = srcMesh.uv1[triangles[i]];
			Vector2 uv2 = srcMesh.uv1[triangles[i+1]];
			Vector2 uv3 = srcMesh.uv1[triangles[i+2]];
			
			Vector3 v12 = v2-v1;
			Vector3 v13 = v3-v1;
			
			Vector2 uv12 = uv2-uv1;
			Vector2 uv13 = uv3-uv1;
			
			
			float rawMaxItemToAdd = triangleAreas[i/3]*(float)maxObjectsCount/totalArea + fracPart;
			int maxItemsToAdd = (int)Mathf.Floor(rawMaxItemToAdd);
			totalItemsToAdd+=maxItemsToAdd;
			fracPart = rawMaxItemToAdd-(float)maxItemsToAdd;
			
			int gridBase = (int)Mathf.Ceil(((1+Mathf.Sqrt(1.0f+8.0f*maxItemsToAdd))/2.0f)-1.0f);
			
			Debug.Log("triangle " + (i/3) + " : area = " + triangleAreas[i/3]+"/"+totalArea + ", "+rawMaxItemToAdd+" itemsToAdd");
			
			ObjectToScat[] objectsToScat = new ObjectToScat[(maxItemsToAdd*maxItemsToAdd)];
			
			int numItemsToAdd = 0;
			
			float gss = 1.0f/(gridBase);
			float gst = 1.0f/(gridBase+1);
			
			for(int j=0; j<gridBase;j++){
				
				for(int k=0; k<gridBase-j;k++){
					
					float rands = Random.Range(-0.25f,0.25f);
					float randt = Random.Range(-0.25f,0.25f);
					
					float s = (j+rands+0.5f)*gss;
					float t = (k+randt+0.5f)*gst;
					
					Vector2 uv =  uv1 + uv12*s + uv13*t;
					float density = 1.0f;
					if(densityMap!=null){
						Color color = densityMap.GetPixelBilinear(uv.x,uv.y);
						density = (color.r + color.g + color.b) /3.0f;
					}
					
					float prob = Random.Range(0.0f,0.5f);

					if(prob < density){
						
						float rotateRandom = rotationRandomize*Mathf.PI/360;
						
						objectsToScat[numItemsToAdd].position = transform.TransformPoint(v1 + v12*s + v13*t);
						objectsToScat[numItemsToAdd].rotation = new Quaternion(0,1,0,Random.Range(-rotateRandom,rotateRandom));
						objectsToScat[numItemsToAdd].scale = ((1-densityToScale) + densityToScale * density) * (float)Random.Range(scaleMin,scaleMax);
						
						numItemsToAdd++;
	
					}
				}
				
			}
			
			
			
			
			if(numItemsToAdd>0){
				
				if(sampleObjects.Length==1){
					
						newMesh = new Mesh();
						newGameObject = new GameObject(instantiateGameObject[0].name + "-" + scatterIndex);
						newGameObject.AddComponent("MeshRenderer");
						MeshFilter meshfilter = (MeshFilter)newGameObject.AddComponent("MeshFilter");
						meshfilter.mesh = newMesh;
						newMesh.vertices = new Vector3[0];
						newMesh.normals = new Vector3[0];
						newMesh.uv = new Vector2[0];
						newMesh.triangles = new int[0];
						newGameObject.transform.parent = transform;
						newGameObject.renderer.material = instantiateGameObject[0].renderer.material;
						
						scatterIndex++ ;
				
				
						int sampleMeshVerticesLength = sampleMeshes[0].vertices.Length;
						int sampleMeshNormalsLength = sampleMeshes[0].normals.Length;
						int sampleMeshUvLength = sampleMeshes[0].uv.Length;
						int sampleMeshTrianglesLength = sampleMeshes[0].triangles.Length;
				
				
						Vector3[] newVertices  = new Vector3[numItemsToAdd * sampleMeshVerticesLength];
						Vector3[] newNormals  = new Vector3[numItemsToAdd * sampleMeshNormalsLength];
						Vector2[] newUv  = new Vector2[numItemsToAdd * sampleMeshUvLength];
						int[] newTriangles  = new int[numItemsToAdd * sampleMeshTrianglesLength];
				
						for(int j = 0;j<numItemsToAdd; j++){
				
							int verticesOffset = j*sampleMeshVerticesLength;
							int normalOffset = j*sampleMeshNormalsLength;
							int uvOffset = j*sampleMeshUvLength;
							int trianglesOffset = j*sampleMeshTrianglesLength;
							
							for(int v = 0;v<sampleMeshVerticesLength;v++)newVertices[verticesOffset+v] = new Vector3(
																					sampleMeshes[0].vertices[v].x*instantiateGameObject[0].transform.localScale.x*objectsToScat[j].scale+objectsToScat[j].position.x,
																					sampleMeshes[0].vertices[v].y*instantiateGameObject[0].transform.localScale.y*objectsToScat[j].scale+objectsToScat[j].position.y,
																					sampleMeshes[0].vertices[v].z*instantiateGameObject[0].transform.localScale.z*objectsToScat[j].scale+objectsToScat[j].position.z);
							for(int v = 0;v<sampleMeshNormalsLength;v++)newNormals[normalOffset+v] = sampleMeshes[0].normals[v];
							for(int v = 0;v<sampleMeshUvLength;v++)newUv[uvOffset+v] = sampleMeshes[0].uv[v];
							for(int v = 0;v<sampleMeshTrianglesLength;v++)newTriangles[trianglesOffset+v] = sampleMeshes[0].triangles[v]+verticesOffset;
					
						}
						
						newMesh.vertices  = newVertices;
						newMesh.normals  = newNormals;
						newMesh.uv  = newUv;
						newMesh.triangles  = newTriangles;
				}else{
					
					for(int j = 0;j<numItemsToAdd; j++){
						
						int k = Random.Range(0,instantiateGameObject.Length);

						GameObject currentMesh = (GameObject)Instantiate(instantiateGameObject[k],objectsToScat[j].position,objectsToScat[j].rotation);
						currentMesh.transform.localScale = new Vector3(	objectsToScat[j].scale*instantiateGameObject[k].transform.localScale.x,
																		objectsToScat[j].scale*instantiateGameObject[k].transform.localScale.y,
																		objectsToScat[j].scale*instantiateGameObject[k].transform.localScale.z);
						currentMesh.transform.parent = transform;
						currentMesh.name = scatterIndex+"-"+instantiateGameObject[k].name;
						scatterIndex++ ;
						
						
						
					}
					
					
				}
				
			}
			
		}
		
		
		Debug.Log("Total max items to add : " + totalItemsToAdd);
		
		for(int i = 0; i< sampleObjects.Length;i++){
			Destroy(instantiateGameObject[i]);
		}
	}
	
	
	*/
	
	
}
