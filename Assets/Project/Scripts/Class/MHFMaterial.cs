using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MHFMaterial {
	public int id;
	public int material_category_id;
	public string name;
	public string color;
	public string diffuse_texture;
	public string normal_texture;
	public string occlusion_texture;
}