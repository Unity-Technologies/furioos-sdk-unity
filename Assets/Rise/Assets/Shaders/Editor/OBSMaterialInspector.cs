#pragma warning disable 0618
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class OBSMaterialInspector : MaterialEditor {
	
	// this is the same as the ShaderProperty function, show here so 
	// you can see how it works
	private void ShaderPropertyImpl(Shader shader, int propertyIndex)
	{
		int i = propertyIndex;
		string label = ShaderUtil.GetPropertyDescription(shader, i);
		string propertyName = ShaderUtil.GetPropertyName(shader, i);
		switch (ShaderUtil.GetPropertyType(shader, i))
		{
		case ShaderUtil.ShaderPropertyType.Range: // float ranges
		{
			GUILayout.BeginHorizontal();
			float v2 = ShaderUtil.GetRangeLimits(shader, i, 1);
			float v3 = ShaderUtil.GetRangeLimits(shader, i, 2);
			RangeProperty(propertyName, label, v2, v3);
			GUILayout.EndHorizontal();
			
			break;
		}
		case ShaderUtil.ShaderPropertyType.Float: // floats
		{
			FloatProperty(propertyName, label);
			break;
		}
		case ShaderUtil.ShaderPropertyType.Color: // colors
		{
			ColorProperty(propertyName, label);
			break;
		}
		case ShaderUtil.ShaderPropertyType.TexEnv: // textures
		{
			ShaderUtil.ShaderPropertyTexDim desiredTexdim = ShaderUtil.GetTexDim(shader, i);
			TextureProperty(propertyName, label, desiredTexdim);
			
			GUILayout.Space(6);
			break;
		}
		case ShaderUtil.ShaderPropertyType.Vector: // vectors
		{
			VectorProperty(propertyName, label);
			break;
		}
		default:
		{
			GUILayout.Label("ARGH" + label + " : " + ShaderUtil.GetPropertyType(shader, i));
			break;
		}
		}
	}
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		var theShader = serializedObject.FindProperty ("m_Shader");	
		if (isVisible && !theShader.hasMultipleDifferentValues && theShader.objectReferenceValue != null)
		{
			//float controlSize = 64;
			
			//EditorGUIUtility.LookLikeControls(Screen.width - controlSize - 20);
			Shader shader = theShader.objectReferenceValue as Shader;


			bool isLightResponsive = shader.name.Contains("LR");
			bool isTextured = shader.name.Contains("Texture");
			bool isTransparent = shader.name.Contains("Alpha");
			bool isLightmapped = shader.name.Contains("Lightmap");
			bool isBumped = shader.name.Contains("Bump");
			bool isReflective = shader.name.Contains("Reflect");
			bool isMasked = shader.name.Contains("Mask");
			bool isCutout = shader.name.Contains("Cutout");
			bool isDoubleSided = shader.name.Contains("DS");


			EditorGUILayout.BeginVertical();


			isLightResponsive = EditorGUILayout.Toggle("Is Light Responsive",isLightResponsive);
			isTextured = EditorGUILayout.Toggle("Is Textured",isTextured);
			isTransparent = EditorGUILayout.Toggle("Is Transparent",isTransparent);
			if(isTransparent)isCutout = EditorGUILayout.Toggle("Is Cutout",isCutout);
			else isCutout = false;
			if(isTransparent)isMasked = EditorGUILayout.Toggle("Is Masked",isMasked);
			else isMasked = false;
			isLightmapped = EditorGUILayout.Toggle("Is Lightmapped",isLightmapped);
			isBumped = EditorGUILayout.Toggle("Is Bumped",isBumped);
			isReflective = EditorGUILayout.Toggle("Is Reflective",isReflective);
			isDoubleSided = EditorGUILayout.Toggle("Is DoubleSided",isDoubleSided);


			string shaderName = "Observ3d/";
			shaderName += isLightResponsive ? "Light Responsive/" : "Light Independent/";
			shaderName += isTextured ? "Textured/" : "Color/";
			shaderName += isTransparent ? "Transparent/" : "Opaque/";
			shaderName += isLightResponsive ? "LR" : "LI";
			shaderName += isTextured ? "-Texture" : "-Color";
			shaderName += isTransparent ? "-Alpha" : "";
			shaderName += isLightmapped ? "-Lightmap" : "";
			shaderName += isBumped ? "-Bump" : "";
			shaderName += isReflective ? "-Reflect" : "";
			shaderName += isMasked ? "-Mask" : "";
			shaderName += isCutout ? "-Cutout" : "";
			shaderName += isDoubleSided ? "-DS" : "";

			EditorGUILayout.EndVertical();
				
			if (shaderName != shader.name){
				foreach( Material editedMat in targets){
					editedMat.shader = Shader.Find(shaderName);
				}
			}else{

				EditorGUI.BeginChangeCheck();

				for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++){
					ShaderPropertyImpl(shader, i);
				}
				
				if (EditorGUI.EndChangeCheck())PropertiesChanged ();
			}
		}
	}
}

