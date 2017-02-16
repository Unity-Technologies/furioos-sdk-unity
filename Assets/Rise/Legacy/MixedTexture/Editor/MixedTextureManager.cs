#pragma warning disable 0618
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MixedTextureManager : EditorWindow 
{	
/** Mixed Texture Manager ************************************************************************************************************************************************/
	
	/** Génère ou retourne une texture mixée prééxistante ayant les propriétées fournies en paramètre.
	 */
	public static Texture2D GetOrCreateMixedTexture(TextureUsage textureUsage, List<BaseDatas> bases)
	{
		string hashcode = textureUsage + "" ;
		
		foreach(BaseDatas baseData in bases)
		{
			hashcode += TextureToGUID(baseData.texture) + "" + baseData.mixingType + "" + baseData.ratio ;
		}
		
		Texture2D mixed ; 
		
		if(mixedTextures.TryGetValue(hashcode, out mixed))
		{
			return mixed ;
		}
		
		return CreateNewMixedTexture(textureUsage, bases, hashcode) ;
	}
	
	/** Dictionnaire de récupération des textures existantes.
	 */ 
	public static Dictionary<string, Texture2D> mixedTextures = new Dictionary<string, Texture2D>() ;
	
	/** Génère une texture mixée vide.
	 */
	//[MenuItem("Assets/Create/MixedTexture")]
	public static Texture2D CreateNewMixedTexture()
	{
		return CreateNewMixedTexture(TextureUsage.Lightmap, new List<BaseDatas>()) ;
	}
	
	/** Génère et remplit une texture mixée.
	 */
	public static Texture2D CreateNewMixedTexture(TextureUsage textureUsage, List<BaseDatas> bases, string hashcode = null)
	{
		if(xml == null || !File.Exists(filePath)) { Start() ; } 		
		
		// 1. Création de la texture.
		
		Texture2D texture = new Texture2D(2048, 2048, TextureFormat.ARGB32, false) ;
		
		string texturePathAndName = AssetDatabase.GenerateUniqueAssetPath(directoryPath + "/MixedTexture.png") ;
		File.WriteAllBytes(texturePathAndName, texture.EncodeToPNG()) ;		
		
		AssetDatabase.ImportAsset(texturePathAndName) ;
		
		texture = (Texture2D) AssetDatabase.LoadMainAssetAtPath(texturePathAndName) ;
		
		AssetDatabase.SaveAssets() ;
		
		TextureImporter textureImporter = (TextureImporter) AssetImporter.GetAtPath(texturePathAndName) ;
		
		textureImporter.textureType   = TextureImporterType.Default ;
		textureImporter.textureFormat = TextureImporterFormat.ARGB32 ;
		textureImporter.isReadable = true ;
		textureImporter.mipmapEnabled = false ;
		textureImporter.maxTextureSize = 2048 ;
		
		AssetDatabase.ImportAsset(texturePathAndName, ImportAssetOptions.ForceUpdate) ;
		
		
		// 3. Génération du xml.
		
		XmlElement xmlTextureNode = xml.CreateElement("mixedTexture") ;
		
			XmlElement xmlTextureGUIDNode  = xml.CreateElement("guid") ; xmlTextureGUIDNode.InnerText = TextureToGUID(texture) ;		
			XmlElement xmlTextureUsageNode = xml.CreateElement("textureUsage")  ; xmlTextureUsageNode.InnerText = "" + textureUsage ;		
		
		xmlTextureNode.AppendChild(xmlTextureGUIDNode)  ;
		xmlTextureNode.AppendChild(xmlTextureUsageNode) ;
		
		// Ajout des bases.
		
		XmlElement xmlBasesNode = xml.CreateElement("bases") ;
		
			foreach(var baseMap in bases)
			{
				xmlBasesNode.AppendChild(GenerateBaseNode(TextureToGUID(baseMap.texture), baseMap.ratio, baseMap.mixingType))  ;
			}
		
		xmlTextureNode.AppendChild(xmlBasesNode) ;
		
		xml.DocumentElement.AppendChild(xmlTextureNode) ; 
		
		FillTexture(ref texture, textureUsage, bases) ;
		
		// 4. Séléction de la texture dans l'éditeur.
		EditorUtility.FocusProjectWindow() ; Selection.activeObject = texture ;
		
		if(hashcode != null) 
		{
			mixedTextures.Add(hashcode, texture) ; 
		}
		
		SaveXml() ;
		
		return texture ;
	}
	
	/** Remplit une texture.
	 */
	private static void FillTexture(ref Texture2D output, TextureUsage textureUsage, List<BaseDatas> bases)
	{
		Color[] colors = new Color[output.width*output.height] ;
		int maxHeight, maxWidth;
		
		maxWidth = 2048;
		maxHeight = 2048;
		
		foreach(var bas in bases)
		{
			string path = AssetDatabase.GetAssetPath(bas.texture);
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
						
			if (textureImporter.isReadable == false)
			{
			   textureImporter.isReadable = true;
			   AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			}
			
			if(bas.texture.width>maxWidth)maxWidth=bas.texture.width;
			if(bas.texture.height>maxHeight)maxHeight=bas.texture.height;
		}
		
		if(output.width!=maxWidth || output.height!=maxHeight){
			
			output = new Texture2D(maxWidth, maxHeight, TextureFormat.ARGB32, false) ;
		}
		
		for(int i = 0 ; i < output.width ; i++)
		{
			for(int j = 0 ; j < output.height ; j++)
			{
				int index = i + j*output.width ;
				
				switch(textureUsage){
					case TextureUsage.Diffuse :
						colors[index] = new Color(0.0f,0.0f,0.0f,1.0f);
					break;
					
					case TextureUsage.Lightmap :
						colors[index] = new Color(0.5f,0.5f,0.5f,0.25f);
					break;
					
					case TextureUsage.Normalmap :
						colors[index] = new Color(0.5f,0.5f,1.0f,1.0f);
					break;
				}
				
				for(int k = 0 ; k < bases.Count;k++)
				{
					Color baseColor = bases[k].texture.GetPixelBilinear( (float)i /output.width,  (float)j /output.height);
					float ratio = bases[k].ratio;
					
					switch(bases[k].mixingType)
					{
						case MixingTypes.Add :
							
							if(textureUsage == TextureUsage.Diffuse)
							{
								colors[index].r += baseColor.r * ratio * baseColor.a;
								colors[index].g += baseColor.g * ratio * baseColor.a;
								colors[index].b += baseColor.b * ratio * baseColor.a;
							}
						
						break;
						
						case MixingTypes.Multiply :
							
							if(textureUsage == TextureUsage.Diffuse)
							{
								colors[index].r *= 1.0f-((1.0f-baseColor.r) * ratio * baseColor.a);
								colors[index].g *= 1.0f-((1.0f-baseColor.r) * ratio * baseColor.a);
								colors[index].b *= 1.0f-((1.0f-baseColor.r) * ratio * baseColor.a);
							}
						
						break;
						
						case MixingTypes.Mask :
							
							if(textureUsage == TextureUsage.Diffuse)
							{
								colors[index].a *= 1.0f-((1.0f-baseColor.a) * ratio);
							}
						
						break;
						
						case MixingTypes.Lightmap :
							
							if(textureUsage != TextureUsage.Normalmap)
							{
								colors[index].r *= 1.0f-((1.0f-8 * baseColor.r * baseColor.a) * ratio);
								colors[index].g *= 1.0f-((1.0f-8 * baseColor.g * baseColor.a) * ratio);
								colors[index].b *= 1.0f-((1.0f-8 * baseColor.b * baseColor.a) * ratio);
								
								
								if(textureUsage == TextureUsage.Lightmap)
								{
									float max = Mathf.Max(colors[index].r,colors[index].g,colors[index].b);
									if(max >1)
									{
										colors[index].r /= max;
										colors[index].g /= max;
										colors[index].b /= max;
										colors[index].a *= max;
									}
								}
							}
						
						break;
						
						case MixingTypes.Occlusion :
						{
							float grayScale = (baseColor.r + baseColor.g + baseColor.b) / 3;
							if(textureUsage == TextureUsage.Diffuse)
							{
								colors[index].r *= (1.0f-(1.0f - grayScale) * ratio);
								colors[index].g *= (1.0f-(1.0f - grayScale) * ratio);
								colors[index].b *= (1.0f-(1.0f - grayScale) * ratio);
							}
							else if(textureUsage == TextureUsage.Lightmap)
							{		
								colors[index].a *= (1.0f-(1.0f - grayScale) * ratio);
							}
									
							break;	
						}
					}
				}
			}
		}
		
		output.SetPixels(colors) ;
		output.Apply() ;
		
		File.WriteAllBytes(AssetDatabase.GetAssetPath(output), output.EncodeToPNG()) ;
		
		AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(output), ImportAssetOptions.ForceUpdate) ;
	}	
	
	/** Force le niveau d'occlusion pour toutes les mixed textures possédant une base taggé comme "Occlusion".
	 */
	public static void ForceOcclusionLevel(float level)
	{
		if(xml == null || !File.Exists(filePath)) { Start() ; } 				
		
		List<Texture2D> mixeds = GetAllMixedTexture() ;
		
		for(int i = 0; i < mixeds.Count ; i++){
			Texture2D mixed = mixeds[i];
			XmlNode node = GetNode(mixed) ;
			
			TextureUsage tUsage = GetTextureUsage(mixed) ;
			List<BaseDatas> bases = GetBases(mixed) ;
			bool changed = false ;
			
			foreach(BaseDatas bData in bases)
			{
				if(bData.mixingType == MixingTypes.Occlusion)
				{
					changed = true ;
					bData.ratio = level ;
					
					foreach(XmlNode b in node["bases"].ChildNodes)
					{
						if(b["guid"].InnerText == TextureToGUID(bData.texture))
						{
							b["ratio"].InnerText = "" + level ;
							break ;
						}
					}
					
					FillTexture(ref mixed, tUsage, bases) ;
				}
			}
			
			if(changed) { SaveXml() ; }
		}
	}
	
	/** Récupère l'ensemble des mixed textures de l'application.
	 */
	private static List<Texture2D> GetAllMixedTexture()
	{
		List<Texture2D> mixeds = new List<Texture2D>() ;
		XmlNodeList nodesList = xml.GetElementsByTagName("mixedTexture") ;
		
		foreach(XmlNode node in nodesList)
		{
			mixeds.Add(GUIDToTexture(node["guid"].InnerText)) ;
		}
		
		return mixeds ;		
	}
	
	/** Récupère l'ensemble des bases associées à une mixed texture.
	 */
	private static List<BaseDatas> GetBases(Texture2D mixed)
	{
		XmlNode node = GetNode(mixed) ;
		List<BaseDatas> bases = new List<BaseDatas>() ;
		
		if(node != null)
		{			
			foreach(XmlNode baseNode in node["bases"].ChildNodes)
			{
				BaseDatas baseData = new BaseDatas() ;
				bases.Add(baseData) ;
				
				baseData.texture = GUIDToTexture(baseNode["guid"].InnerText) ;
				baseData.ratio = float.Parse(baseNode["ratio"].InnerText) ;
				baseData.mixingType = (MixingTypes)  System.Enum.Parse(typeof(MixingTypes), baseNode["mixingType"].InnerText) ;
			}
		}
		
		return bases ;
	}	
	
	/** Récupère l'usage d'une mixed texture.
	 */
	private static TextureUsage GetTextureUsage(Texture2D mixed)
	{
		XmlNode node = GetNode(mixed) ;
		
		if(node != null)
		{
			return (TextureUsage) System.Enum.Parse(typeof(TextureUsage), node["textureUsage"].InnerText) ; ;
		}
		
		return TextureUsage.Diffuse ;		
	}
		
/** Gestion du XML *******************************************************************************************************************************************************/	
	
	/** Initialise le gestionnaire des textures mixées.
	 */
	public static void Start()
	{
		xml = new XmlDocument() ;
		
		string path = "Assets/Project" ;
		if(!Directory.Exists(path)) { Directory.CreateDirectory(path) ; }
		
		path += "/Assets" ;
		if(!Directory.Exists(path)) { Directory.CreateDirectory(path) ; }
		
		path += "/Textures" ;
		if(!Directory.Exists(path)) { Directory.CreateDirectory(path) ; }		

		directoryPath = path += "/Mixed" ;
		if(!Directory.Exists(path)) { Directory.CreateDirectory(path) ; }				
		
		filePath = path += "/info.xml" ;
		
		if(!File.Exists(path)) 	
		{ 
			FileStream f = File.Create(path) ;
			
			xml.LoadXml("<mixedTextures></mixedTextures>") ; 
			xml.Save(f) ; 
			
			f.Close() ;
		} 
		else 						
		{ 
			xml.Load(path) ; 
		}
	}	
	
	
	/** Sauvegarde les modifications faites sur le xml de définition des textures mixées.
	 */
	private static void SaveXml() { xml.Save(filePath) ; }
	
	
	/** Retourne la node associée à la texture mixée fournie (ou null si la texture n'est pas enregistrée).
	 */
	private static XmlNode GetNode(Texture2D mixed)
	{	
		string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(mixed)) ;
		
		XmlNodeList nodesList = xml.GetElementsByTagName("mixedTexture") ;
		
		foreach(XmlNode node in nodesList)
		{
			if(node["guid"].InnerText == guid) 
			{
				return node ;
			}
		}
		
		return null ;
	}
	
/** Mixed Texture Inspector **********************************************************************************************************************************************/	
	
	/** Ouvre la fenètre servant d'inspector.
	 */
	[MenuItem("Observ3d/Textures/Mixed Texture Inspector")]	
	public static void OpenWindow() 
	{
		MixedTextureManager window = (MixedTextureManager) EditorWindow.GetWindow(typeof(MixedTextureManager)) ;
		window.title = "MixedTextureInspector" ;
	}
	
	/** Affiche l'ensemble des textures séléctionnées.
	 */
	public void OnGUI()
	{
		if(xml == null || !File.Exists(filePath)) { Start() ; } 
		
		Object[] textures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets) ;
		
		scrollView = GUILayout.BeginScrollView(scrollView) ;
		
			GUILayout.Label ("Selected Textures", EditorStyles.boldLabel) ;
		
			foreach (Object otexture in textures) 
			{
				DrawInspector((Texture2D) otexture) ; 
			}
		
		GUILayout.EndScrollView();
		
		Repaint();		
	}
	
	/** Créé l'affichage "externe" pour une texture mixée.
	 */
 	private void DrawInspector(Texture2D texture) 
	{
		XmlNode node = GetNode(texture) ;
		
		if(node != null)
		{	
			bool extend = (bool) texture.getMetaData("foldout", true) ;
			texture.setMetaData("foldout", EditorGUILayout.InspectorTitlebar(extend, texture)) ; 
			
			if(extend) { DrawInternalInspector(texture, node) ; } 
		}
	}
	
	/** Créé l'affichage "interne" pour une texture mixée.
	 */	
	private void DrawInternalInspector(Texture2D texture, XmlNode node)
	{
		XmlNodeList baseTextureList = node["bases"].ChildNodes ;
		List<BaseDatas> bases = new List<BaseDatas>(node["bases"].ChildNodes.Count) ;
		
		
		TextureUsage oldTextureUsage = (TextureUsage) System.Enum.Parse(typeof(TextureUsage), node["textureUsage"].InnerText) ;
		TextureUsage textureUsage = (TextureUsage) EditorGUILayout.EnumPopup("Texture usage", oldTextureUsage) ;
		if(textureUsage != oldTextureUsage) { node["textureUsage"].InnerText = "" + textureUsage ; }
		
		EditorGUILayout.LabelField("") ;
		
		
		int index1 = -1 ; int index2 = -1 ; 
		for(int i = 0 ; i < baseTextureList.Count ; i++)
		{
			XmlNode baseNode = baseTextureList[i] ;
			bases.Add(new BaseDatas()) ;
			
			EditorGUILayout.BeginHorizontal() ;
			
				// Récupération des anciennes données.
					
				Texture2D oldBase = GUIDToTexture(baseNode["guid"].InnerText) ;
			
				float oldRatio    = float.Parse(baseNode["ratio"].InnerText) ;
				
				MixingTypes  oldMixingType   = (MixingTypes)  System.Enum.Parse(typeof(MixingTypes), baseNode["mixingType"].InnerText) ;
				
				// Récupération des nouvelles données.
					
				bases[i].texture = (Texture2D) EditorGUILayout.ObjectField("", oldBase, typeof(Texture2D)) ;
				
				EditorGUILayout.BeginVertical() ;
				
					bases[i].ratio = EditorGUILayout.Slider("Ratio", oldRatio, 0, 1) ;
					bases[i].mixingType   = (MixingTypes)  EditorGUILayout.EnumPopup("Mixing type", oldMixingType) ;
				
				EditorGUILayout.EndVertical() ;

				EditorGUILayout.BeginVertical() ;
			
					if(GUILayout.Button("Up") && i > 0)     						{ index1 = i-1 ; index2 = i ; }					
					if(GUILayout.Button("Delete")) 									{ node["bases"].RemoveChild(baseNode) ; xml.Save(filePath) ; }
					if(GUILayout.Button("Down") && i < (baseTextureList.Count-1))   { index1 = i ; index2 = i+1 ; }			
			
				EditorGUILayout.EndVertical() ;
			
				// Réactions
					
				if(bases[i].texture != oldBase)         
				{ 
					baseNode["guid"].InnerText  = TextureToGUID(bases[i].texture) ;
				}
			
				if(bases[i].ratio        != oldRatio)        { baseNode["ratio"].InnerText = "" + bases[i].ratio ; }
				if(bases[i].mixingType   != oldMixingType)   { baseNode["mixingType"].InnerText = "" + bases[i].mixingType ; }
			
			
			EditorGUILayout.EndHorizontal() ;
			
			EditorGUILayout.LabelField("") ;	
			
		}
		
		if(index1 != index2) 		  
		{ 
			XmlNode node1 = node["bases"].ChildNodes[index1] ;
			XmlNode node2 = node["bases"].ChildNodes[index2] ;
			
			node["bases"].InsertBefore(node2, node1) ;
			
			xml.Save(filePath) ;
		}
		if(GUILayout.Button("Add"))   { node["bases"].AppendChild(GenerateBaseNode("-1", 1, MixingTypes.Add)) ; xml.Save(filePath) ; }		
		if(GUILayout.Button("Apply")) { FillTexture(ref texture, textureUsage, bases) ; xml.Save(filePath) ; }
	}
	
/** Génère un nouveau noeud "base" **************************************************************************************************/
	
	private static XmlElement GenerateBaseNode(string guid, float ratio, MixingTypes mixingType)
	{
		XmlElement xmlBaseNode = xml.CreateElement("base") ;
			
		XmlElement xmlBaseTextureGUIDNode  = xml.CreateElement("guid")  		; xmlBaseTextureGUIDNode.InnerText  = guid ;
		XmlElement xmlBaseTextureRatioNode = xml.CreateElement("ratio") 		; xmlBaseTextureRatioNode.InnerText = "" + ratio  ;		
		XmlElement xmlBaseMixingModeNode   = xml.CreateElement("mixingType")    ; xmlBaseMixingModeNode.InnerText   = "" + mixingType ;	
		
		xmlBaseNode.AppendChild(xmlBaseTextureGUIDNode)  ;		
		xmlBaseNode.AppendChild(xmlBaseTextureRatioNode) ;
		xmlBaseNode.AppendChild(xmlBaseMixingModeNode)   ;
		
		return xmlBaseNode ;
	}
	
/** Retrouve les textures via leur GUID et vice versa *******************************************************************************/
	
	private static string TextureToGUID(Texture2D texture)
	{
		return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(texture)) ;
	}
	
	private static Texture2D GUIDToTexture(string guid)
	{
		return (Texture2D) AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid)) ;	
	}	
	
/** Structures et enumeration liée à la gestion des textures ************************************************************************/
	
	public enum MixingTypes  { Lightmap, Occlusion, Multiply, Add, Mask }
	public enum TextureUsage { Diffuse, Lightmap, Normalmap }
	
	public class BaseDatas
	{
		public Texture2D texture ;
		public float ratio ;
		public MixingTypes mixingType ;
	}
	
/** Attributs ***********************************************************************************************************************/
	
	/** Contient les données des textures mixées dans un xml simplifié.
	 */
	private static XmlDocument xml = null ;	
	
	/** Chemin vers le répertoire où enregistrer les textures mixées.
	 */
	private static string directoryPath = "Assets/Project/Assets/Texture/Mixed" ;
	
	/** Chemin vers le fichier contenant le xml de description.
	 */
	private static string filePath = "Assets/Project/Assets/Texture/Mixed/info.xml" ;	
	
	/** Etat de défillement dans la liste des textures mixées visibles dans l'inspecteur.
	 */
	private Vector2 scrollView = new Vector2() ;
}
