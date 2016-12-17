using UnityEngine;
using UnityEditor;
using System.Collections;

public class Flipper
{
	[MenuItem("Observ3d/Object/Flip normals and triangles")]
	public static void FlipNormalsAndTriangles()
	{
		FlipNormals();
		FlipTriangles();
	}
	
	[MenuItem("Observ3d/Object/Flip normals")]
	public static void FlipNormals()
	{
		GameObject go   = Selection.activeGameObject;
		MeshFilter mFlt = go.GetComponent<MeshFilter>();
		
		if(mFlt == null)
		{
			Debug.LogWarning("Cannot flip normals of this object because it has no MeshFilter component.");
			return;
		}
		
		Mesh msh = mFlt.sharedMesh;
		if(msh == null)
		{
			Debug.LogWarning("Cannot flip normals of this object because the MeshFilter component contains no mesh.");
			return;
		}
		
		Vector3[] normals = msh.normals;
		for(int i = 0; i < msh.normals.Length; ++i)
		{
			normals[i].x = -normals[i].x;
			normals[i].y = -normals[i].y;
			normals[i].z = -normals[i].z;
		}
		msh.normals = normals;
		
		Debug.Log("Flipped " + msh.normals.Length + " normals.");
	}
	
	[MenuItem("Observ3d/Object/Flip triangles")]
	public static void FlipTriangles()
	{
		GameObject go   = Selection.activeGameObject;
		MeshFilter mFlt = go.GetComponent<MeshFilter>();
		
		if(mFlt == null)
		{
			Debug.LogWarning("Cannot flip triangles of an object that has no MeshFilter component.");
			return;
		}
		
		Mesh msh = mFlt.sharedMesh;
		if(msh == null)
		{
			Debug.LogWarning("Cannot flip triangles of this object because the MeshFilter component contains no mesh.");
			return;
		}
		
		int[] triangles = msh.triangles;
		for(int i = 0; i < triangles.Length; i += 3)
		{
			int tmp          = triangles[i];
	        triangles[i]     = triangles[i + 2];
	        triangles[i + 2] = tmp;
		}
		msh.triangles = triangles;
		
		Debug.Log("Flipped " + msh.triangles.Length + " triangles.");
	}
}
