using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Rise.Features.AutoAssign {

	public enum TextureType {
		Standard,
		Normalmap,
		Lightmap,
		Occlusion,
		Mixed
	}

	public class AutoAssignTools {

		public static Transform FindRecursive(Transform tr,string meshName) {
			Transform found = tr.Find(meshName);
			if(found!=null)return found;
			foreach(Transform trChild in tr){
				found = FindRecursive(trChild,meshName);
				if(found!=null)break;
			}
			return found;
		}
		
		public static string GetFilenameWithoutExtension(string path) {
			int start = Mathf.Max(path.LastIndexOf("/"),path.LastIndexOf("\\"))+1;
			int end = path.LastIndexOf(".");
			
			if(start<end) return path.Substring(start,end-start);
			else return path;
		}
		
		public static string GetFileExtension(string path) {
			int start = path.LastIndexOf(".");
			
			if(start>0) return path.Substring(start+1,path.Length-start-1);
			else return "";
		}

		public static bool AssignMaterialToMesh(Transform tr,XmlNode hNode,Dictionary<string,Material> materials) {
			if(hNode!=null){
				XmlElement matNode = (XmlElement)hNode.SelectSingleNode("material");
				if(matNode!=null){
					string meshName = ((XmlElement)hNode).GetAttribute("name");
					string materialId = matNode.GetAttribute("materialId");
					if(!string.IsNullOrEmpty(materialId)){
						if(materials.ContainsKey(materialId)){
							
							if(tr!=null && tr.gameObject!=null){
									if(tr.gameObject.GetComponent<Renderer>() != null)
										tr.gameObject.GetComponent<Renderer>().material = materials[materialId];
									else
									{
										foreach(Transform trChild in tr)
										{
											if(trChild.gameObject.GetComponent<Renderer>() != null) {
												trChild.gameObject.GetComponent<Renderer>().material = materials[materialId];
											}
										}
									}
									return true;
							}else{
								
								Debug.LogWarning("Supplied mesh \"" + meshName + "\" or its gameObject is null, impossible to assign material #" + materialId);
							}
						}else{
							Debug.LogWarning("Can't find Material #" + materialId + " to assign to mesh \"" + meshName + "\"");
						}
					}
				}
			}
			return false;
		}

		public static Vector3 ParseVector3Node(XmlNode colorNode) {
			string[] tokens = colorNode.InnerText.Split(' ');
			Vector3 vect = new Vector3(float.Parse(tokens[0]), float.Parse(tokens[1]), float.Parse(tokens[2]));
			
			return vect;
		}
		
		public static float ParseFloatNode(XmlNode node, float defaultValue) {
			if(node!=null)float.TryParse(node.InnerText,out defaultValue);
			return defaultValue;
		}
		
		public static float ParseFloatAttribute(XmlNode node, string attributeName, float defaultValue) {
			string attribute = ((XmlElement)node).GetAttribute(attributeName).ToString();
			float.TryParse(attribute,out defaultValue);
			return defaultValue;
		}
		
		public static bool ParseBoolNode(XmlNode node, bool defaultValue) {
			if(node!=null){
				string val = node.InnerText.Trim().ToLower();
				if(val == "1" || val == "true") return true;
				else if(val =="0" || val == "false")return false;
				else return defaultValue;
			}
			return defaultValue;
		}

		public static bool ParseBoolAttribute(XmlNode node, string attributeName,  bool defaultValue) {
			if(node!=null){
				string val = ((XmlElement)node).GetAttribute(attributeName).ToString().Trim().ToLower();
				if(val == "1" || val == "true") return true;
				else if(val =="0" || val == "false")return false;
				else return defaultValue;
			}
			return defaultValue;
		}
		
		public static Vector4 ParseVector4Node(XmlNode colorNode, float defaultR, float defaultG,float defaultB,float defaultA) {
			if(colorNode != null){
				string[] tokens = colorNode.InnerText.Split(' ');
				if(tokens.Length>0)float.TryParse(tokens[0],out defaultR);
				if(tokens.Length>1)float.TryParse(tokens[1],out defaultG);
				if(tokens.Length>2)float.TryParse(tokens[2],out defaultB);
				if(tokens.Length>3)float.TryParse(tokens[3],out defaultA);
				
			}
			return new Vector4(defaultR, defaultG, defaultB, defaultA);
		}
		
	}
}