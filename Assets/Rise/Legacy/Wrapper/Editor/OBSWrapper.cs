#pragma warning disable 0618
using UnityEngine ;
using UnityEditor ;

using System.Reflection ;
using System.Collections ;
using System.Collections.Generic ;

/** Classe proposant un pipeline de travail afin de générer proprement des Custom Inspectors.
 */
public class OBSWrapper : Editor 
{
/** Génération des binders de description ***************************************************************************************************************************/	

	/**
	 */
	public static Dictionary<string, Pair<System.Type, object>> GenerateBinder(System.Type type)
	{
		// Initialisation
		
		Dictionary<string, Pair<System.Type, object>> binder = new Dictionary<string, Pair<System.Type, object>>() ;
		
		// On génère une envelope par propriété.
		
		PropertyInfo[] properties = type.GetProperties() ;	
		for(int i = 0 ; i < properties.Length ; i++) 
		{ 
			PropertyInfo property = properties[i] ;			
			
			if(CanBeWrap(property.PropertyType) && property.GetCustomAttributes(typeof(SerializeProperty), true).Length > 0) 
			{ 
				binder.Add(property.Name, new Pair<System.Type, object>(property.PropertyType, null)) ;
			} 
		}	
		
		// On génère une envelope par attribut.
		
		FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) ;
		for(int i = 0 ; i < fields.Length ; i++) 
		{
			FieldInfo field = fields[i] ;
			
			if(CanBeWrap(field.FieldType) && ((((field.GetCustomAttributes(typeof(SerializeField), false).Length > 0) && field.GetCustomAttributes(typeof(HideInInspector), true).Length == 0) || field.IsPublic))) 
			{ 
				binder.Add(field.Name, new Pair<System.Type, object>(field.FieldType, null)) ;
			}
		}
		
		binder.Remove("id") ;
		
		return binder ;
	}
	
	/**
	 */
	public static Dictionary<string, Pair<System.Type, object>> GenerateBinder(object target)
	{
		// Initialisation
		
		Dictionary<string, Pair<System.Type, object>> binder = new Dictionary<string, Pair<System.Type, object>>() ; if(target == null) { return binder ; }
		System.Type type = target.GetType() ;
		
		
		// On génère une envelope par propriété.
		
		PropertyInfo[] properties = type.GetProperties() ;	
		for(int i = 0 ; i < properties.Length ; i++) 
		{ 
			PropertyInfo property = properties[i] ;			
			
			if(CanBeWrap(property.PropertyType) && property.GetCustomAttributes(typeof(SerializeProperty), true).Length > 0) 
			{ 
				binder.Add(property.Name, new Pair<System.Type, object>(property.PropertyType, property.GetValue(target))) ;
			} 
		}	
		
		// On génère une envelope par attribut.
		
		FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) ;
		for(int i = 0 ; i < fields.Length ; i++) 
		{
			FieldInfo field = fields[i] ;
			
			if(CanBeWrap(field.FieldType) && ((((field.GetCustomAttributes(typeof(SerializeField), false).Length > 0) && field.GetCustomAttributes(typeof(HideInInspector), true).Length == 0) || field.IsPublic))) 
			{ 
				binder.Add(field.Name, new Pair<System.Type, object>(field.FieldType, field.GetValue(target))) ;
			}
		}
		
		binder.Remove("id") ;
		
		return binder ;
	}	
	
	/**
	 */
	public static Dictionary<string, Pair<System.Type, object>> GenerateBinder(string sBinder)
	{
		Dictionary<string, Pair<System.Type, object>> datas = new Dictionary<string, Pair<System.Type, object>>() ;
		
		foreach(string data in sBinder.Split(';'))
		{
			try
			{
				string[] sdata = data.Split(':') ; 
				
				System.Type tdata = System.Type.GetType(sdata[1]) ;
				
				datas.Add(sdata[0], new Pair<System.Type, object>(tdata, OBSWrapper.ParseData(tdata, sdata[2]))) ;
			} 
			catch {}
		}
		
		return datas ;
	}	
	
	/**
	 */
	public static string SerialiseBinder(Dictionary<string, Pair<System.Type, object>> binder)
	{
		string sBinder = "" ; 
		
		foreach(string key in binder.Keys) 
		{ 
			sBinder += 
				key 
					+ ":" + 
				(
				 binder[key].First.IsEnum ? 
				 binder[key].First.AssemblyQualifiedName : 
				 binder[key].First.FullName
				) 
					+ ":" + 
				OBSWrapper.SerializeData
				(
				 binder[key].First, 
				 binder[key].Second
				) + ";" ; 
		}  
		
		
		if(sBinder.Length > 0) { sBinder = sBinder.Substring(0, sBinder.Length-1) ; }
		
		return sBinder ;
	}
	
/** Exploitation des binders ***************************************************************************************************************************************/	
	
	/**
	 */
	public static void Fill(object target, Dictionary<string, Pair<System.Type, object>> binder)
	{
		if(target == null) { return ; }
		
		System.Type type = target.GetType() ;
		
		foreach(string key in binder.Keys) 
		{ 
			PropertyInfo property = type.GetProperty(key) ; FieldInfo field = type.GetField(key) ;
			
			if(property != null)
			{
				property.SetValue(target, binder[key].Second) ;
			}
			else if(field != null)
			{
				field.SetValue(target, binder[key].Second) ;				
			}
		}  
	}
	
/** Génération des enveloppes **************************************************************************************************************************************/		
	
	/**
	 */
	public virtual bool GenerateWrap(System.Type type, object input, out object output) 
	{ 
		output = input ;
		
		return GenerateDefaultWrap(type, (Dictionary<string, Pair<System.Type, object>>) input) ;
	}
	
	/**
	 */
	public bool GenerateDefaultWrap(System.Type type, Dictionary<string, Pair<System.Type, object>> input)
	{
		bool change = false ;
		
		EditorGUILayout.BeginVertical() ; EditorGUI.indentLevel++ ;
			
		foreach(string name in input.Keys)
		{
			try
			{
				object output ; OBSWrapper wrapper = FindWrapper(input[name].First) ;
				
				if(wrapper != null) 
				{
					if(wrapper.GenerateHeaderWrap(name.UpperCaseFirst(), (input[name].Second == null ? input[name].First : input[name].Second.GetType()), input[name].Second, out output))
					{
						change = true ; 
						
						input[name].First  = output.GetType() ;
						input[name].Second = output ;
					}
				}
			}
			catch (System.Exception ex) { Debug.Log(name + " : " + ex) ; }
		}
			
		EditorGUILayout.EndVertical() ;	EditorGUI.indentLevel-- ;
		
		return change ;
	}
	
	protected virtual bool GenerateHeaderWrap(string name, System.Type type, object input, out object output)
	{
		return GetObjectFoldout(name.UpperCaseFirst(), input) && GenerateWrap(type, input, out output) ;
	}
	
/** Recherche des enveloppeurs *************************************************************************************************************************************/		
	
	/**
	 */
	public static OBSWrapper FindWrapper(System.Type type)
	{
		// Récupération d'un wrapper par défaut ?
		
		OBSWrapper wrapper = FindDefaultWrapper(type) ; if(wrapper != null) { return wrapper ; }
		
		
		// On parcourt l'arbre d'héritage à la recherche d'un type ayant pour nom {Type}Wrapper 
		
		while(type != typeof(object)) 
		{
			// Si on le trouve on essaye d'en obtenir la methode WrapObject.
			string name = type + "Wrapper" ;
			
			if(System.Type.GetType(name) != null) { return (OBSWrapper) ScriptableObject.CreateInstance(name) ; }
			
			type = type.BaseType ;
		}
		
		return (OBSWrapper) ScriptableObject.CreateInstance("OBSWrapper") ;
	}
	
	/**
	 */
	private static bool CanBeWrap(System.Type type)
	{
		return (
				type == typeof(int)   ||
				type == typeof(float) ||
				type == typeof(bool) ||
				type == typeof(string) ||
				type == typeof(Vector2) ||
				type == typeof(Vector3) ||
				type == typeof(Vector4) ||
				type == typeof(Color) ||
				type == typeof(Rect) ||
				type.IsEnum
			   ) ;
	}
	
	/**
	 */
	private static OBSWrapper FindDefaultWrapper(System.Type type)
	{
	 		 if(type == typeof(int)) 		{ return ScriptableObject.CreateInstance<intWrapper>() ; }
		else if(type == typeof(float)) 		{ return ScriptableObject.CreateInstance<floatWrapper>() ; }
		else if(type == typeof(bool)) 		{ return ScriptableObject.CreateInstance<boolWrapper>() ; }
		else if(type == typeof(string)) 	{ return ScriptableObject.CreateInstance<stringWrapper>() ; }
		else if(type == typeof(Vector2)) 	{ return ScriptableObject.CreateInstance<Vector2Wrapper>() ; }
		else if(type == typeof(Vector3)) 	{ return ScriptableObject.CreateInstance<Vector3Wrapper>() ; }
		else if(type == typeof(Vector4)) 	{ return ScriptableObject.CreateInstance<Vector4Wrapper>() ; }
		else if(type == typeof(Color)) 		{ return ScriptableObject.CreateInstance<ColorWrapper>() ; }
		else if(type == typeof(Rect)) 		{ return ScriptableObject.CreateInstance<RectWrapper>() ; }
		else if(type.IsEnum)				{ return ScriptableObject.CreateInstance<EnumWrapper>() ; }
		
		return null ;
	}
	
/** Parse/Serialise méthodes ***************************************************************************************************************************************/
	
	public static string SerializeData(System.Type type, object data) 	{ return FindWrapper(type).Serialize(type, data) 	; }
	public static object ParseData(System.Type type, string sdata) 		{ return FindWrapper(type).Parse(type, sdata) 		; }
	
	
	public virtual string Serialize(System.Type type, object data) 	{ return "" + data  ; }
	public virtual object Parse(System.Type type, string sdata)		{ return null 		; }		

/*******************************************************************************************************************************************************************/
	
	/**
	 */
	public static bool GetObjectFoldout(string label, object target)
	{
		FieldInfo field = target == null ? null : target.GetType().GetField("__openInEditor", BindingFlags.Instance | BindingFlags.NonPublic) ; 
		
		bool openGroup = true ;
		
		if(field != null) 	{ openGroup = EditorGUILayout.Foldout((bool) field.GetValue(target), label) ; field.SetValue(target, openGroup) ; }
		else 				{ openGroup = EditorGUILayout.Foldout((bool) target.getMetaData("foldout", true), label) ; target.setMetaData("foldout", openGroup) ; }
		
		return openGroup ;
	}	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
/** Pipeline de travail ***********************************************************************************************************************************************/	
	
	/** Initialise le travail de génération en prennant un script pour lequel générer un editeur personnalisé.
	 */
	public void WrapScript(Component script) 
	{ 	
		if(WrapObject("", script)) 
		{ 
			//if(script.gameObject.activeInHierarchy) { PrefabUtility.DisconnectPrefabInstance(script.gameObject) ; }			
			
			EditorUtility.SetDirty(script.gameObject) ;
			
			//if(script.gameObject.activeInHierarchy) { PrefabUtility.ReconnectToLastPrefab(script.gameObject) ; }
		} 
	}
	
	/** Initialise l'envelope d'un objet quelconque, soit en cherchant le Wrapper le plus adapté, soit en utilisant le pipeline par défaut.
	 */
	public virtual bool WrapObject(string name, object target) 
	{
		/* On s'échappe si la valeur fournie est null. */ if(target == null) { Debug.LogWarning("Trying to wrap a null value : " + name) ; return false ; }
		
		// Initialisation des variables de travail.
		
		bool openGroup = OBSWrapper.ObjectFoldout(name, target) ; bool change = false ;
		
		// On cherche ensuite à enveloper le contenu de l'objet.
		
		if(openGroup) 
		{ 
			EditorGUI.indentLevel++ ;
			
				change = WrapObjectDatas(target) ; 
			
			EditorGUI.indentLevel-- ;
		}
		
		return change ;
	}	
	
	/** Génère l'enveloppe interne de l'objet.
	 */
	protected virtual bool WrapObjectDatas(object target) { return OBSWrapper.WrapObjectPropertiesAndFieldsDatas(target) ; }
	
	/** Recherche et génère les envelopes pour toutes les données (propriétés et attributs) que le pipeline par défaut prend en compte.
	 */
	public static bool WrapObjectPropertiesAndFieldsDatas(object target)
	{
		/* On s'échappe si la valeur fournie est null. */ if(target == null) { Debug.LogWarning("Trying to wrap a null value") ; return false ; }
		
		// Initialisation des variables de travail.
		
		System.Type targetType = target.GetType() ;	bool change = false ;
		
		
		// On génère une envelope par propriété.
		
		PropertyInfo[] properties = targetType.GetProperties() ;		
		for(int i = 0 ; i < properties.Length ; i++) 
		{ 
			PropertyInfo property = properties[i] ;			
			
			if(property.GetCustomAttributes(typeof(SerializeProperty), true).Length > 0) 
			{ 
				if(WrapProperty(target, property)) { change = true ; }  
			} 
		}	
		
		// On génère une envelope par attribut.
		
		FieldInfo[] fields = targetType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) ;
		for(int i = 0 ; i < fields.Length ; i++) 
		{
			FieldInfo field = fields[i] ;
			
			if((field.GetCustomAttributes(typeof(SerializeField), false).Length > 0) && field.GetCustomAttributes(typeof(HideInInspector), true).Length == 0)
			{ 
				if(WrapField(target, field)) { change = true ; } 
			}
		}
		
		// On retourne si quelque chose a changé ou non.
		
		return change ;		
	}
	
	/** Génère une enveloppe pour une propriété.
	 */
	public static bool WrapProperty(object target, PropertyInfo property)
	{
		// Initialisation des variables de travail.
		
		object toWrapValue = property.GetValue(target) ; System.Type toWrapType = toWrapValue == null ? property.PropertyType : toWrapValue.GetType() ;
		object res ; bool change ; 
		
		
		
		// On envelope la propriété.
		
		change = WrapData(property.Name.UpperCaseFirst(), toWrapType, toWrapValue, out res) ;
		
		// Si quelque chose a changé et que l'on est dans un des cas par défaut on faire subir cette modification. 
		
		if(change && IsWrapperByDefault(toWrapType)) { property.SetValue(target, res) ; }
		
		
		
		// On retourne si quelque chose a changé ou non.
		
		return change ;		
	}
	
	/** Génère une enveloppe pour un attribut.
	 */
	public static bool WrapField(object target, FieldInfo field)
	{
		// Initialisation des variables de travail.
		object toWrapValue = field.GetValue(target) ; 
		System.Type toWrapType = toWrapValue == null ? field.FieldType : toWrapValue.GetType() ;
		object res ; bool change ; 
		
		
		
		// On envelope l'attribut.
		
		change = WrapData(field.Name.UpperCaseFirst(), toWrapType, toWrapValue, out res) ;
		
		// Si quelque chose a changé et que l'on est dans un des cas par défaut on faire subir cette modification. 
		if(change && IsWrapperByDefault(toWrapType)) 
		{
			field.SetValue(target, res) ; 
		}

		
		
		// On retourne si quelque chose a changé ou non.
		
		return change ;			
	}
	
	/** Génère une enveloppe pour une donnée d'un objet.
	 */
	public static bool WrapData(string name, object dataValue) 						{ return WrapData(name, dataValue.GetType(), dataValue, out dataValue) ; }	
	public static bool WrapData(string name, System.Type dataType, object dataValue) 	{ return WrapData(name, dataType, dataValue, out dataValue) ; }
	public static bool WrapData(string name, object dataValue, out object res)			{ return WrapData(name, dataValue.GetType(), dataValue, out res) ; }	
	public static bool WrapData(string name, System.Type dataType, object dataValue, out object res)
	{
		// La donnée est wrappable par défaut.
		
		if(IsWrapperByDefault(dataType))  { return UseDefaultWrapper(name, dataType, dataValue, out res) ; }
		
		// Il existe un wrapper personalisé pour cette donnée.
		res = dataValue ; return findPersonalWrapper(dataType).WrapObject(name, dataValue) ;
	}
	
/** Enveloppage par défaut ********************************************************************************************************************************************/	
	
	/** Vérifie si un type fait parti de ceux pour lequel on dispose d'un wrapper par défaut.
	 */
	public static bool IsWrapperByDefault(System.Type type) 
	{
		
		if(type == null) { return false ; }
		
		return defaultTypes.Contains(type) || type.IsEnum || type.IsSubclassOf(typeof(Object)) ; 
	}
	
	/** Génère une envelope pour un élément par défaut.  
	 */
	public static bool UseDefaultWrapper(string name, System.Type dataType, object dataValue, out object res)
	{
		if     (dataType == typeof(int)) 				{ res = EditorGUILayout.IntField	(name, (int) dataValue) 	  			; }
		else if(dataType == typeof(float)) 				{ res = EditorGUILayout.FloatField	(name, (float) dataValue)   			; }
		else if(dataType == typeof(string)) 			{ res = EditorGUILayout.TextField	(name, (string) dataValue)  			; }
		else if(dataType == typeof(bool)) 				{ res = EditorGUILayout.Toggle		(name, (bool) dataValue) 	  			; }		
		else if(dataType == typeof(Color)) 				{ res = EditorGUILayout.ColorField	(name, (Color) dataValue)   			; }
		else if(dataType == typeof(Vector2)) 			{ res = EditorGUILayout.Vector2Field(name, (Vector2) dataValue) 			; }
		else if(dataType == typeof(Vector3))			{ res = EditorGUILayout.Vector3Field(name, (Vector3) dataValue) 			; }
		else if(dataType == typeof(Vector4))			{ res = EditorGUILayout.Vector4Field(name, (Vector4) dataValue) 			; }
		else if(dataType == typeof(Rect))				{ res = EditorGUILayout.RectField	(name, (Rect) dataValue) 	  			; }	
		else if(dataType == typeof(LayerMask)) 			
		{ 
			res = null ;
//			res = EditorGUILayout.EnumMaskField  (name, (LayerMask) dataValue) ; 
		}
		else if(dataType.IsEnum) 						{ res = EditorGUILayout.EnumPopup	(name, (System.Enum) dataValue) 	  	; }
		else if(dataType.IsSubclassOf(typeof(Object)))	{ res = EditorGUILayout.ObjectField	(name, (Object) dataValue, dataType) 	; }
		else if(dataValue is GUIContent)				{ res = dataValue ; return ((OBSWrapper) ScriptableObject.CreateInstance("GUIContentWrapper")).WrapObject(name, dataValue) ; }
		else 											{ res = dataValue ; }
	
		return (dataValue == null) ? res != null : !dataValue.Equals(res) ;
	}
	
	
	/** Vérifie s'il existe un wrapper personalisé pour un élément, retourne la méthode d'enveloppage de ce dernier si tel est le cas.
	 */
	public static OBSWrapper findPersonalWrapper(System.Type dataType)
	{
		// On parcourt l'arbre d'héritage à la recherche d'un type ayant pour nom {Type}Wrapper 
		
		while(dataType != typeof(object)) 
		{
			// Si on le trouve on essaye d'en obtenir la methode WrapObject.
			string name = dataType + "Wrapper" ;
			
			if(System.Type.GetType(name) != null) { return (OBSWrapper) ScriptableObject.CreateInstance(name) ; }
			
			dataType = dataType.BaseType ;
		}
		
		return (OBSWrapper) ScriptableObject.CreateInstance("OBSWrapper") ;
	}

/** Méthodes utilitaires réutilisables dans d'autres wrappers ******************************************************************************************************/
	
	public static bool ObjectFoldout(string label, object target)
	{
		FieldInfo field = target.GetType().GetField("__openInEditor", BindingFlags.Instance | BindingFlags.NonPublic) ; bool openGroup = true ;
		
		if(field != null) { openGroup = EditorGUILayout.Foldout((bool) field.GetValue(target), label) ; field.SetValue(target, openGroup) ; }
		
		return openGroup ;
	}
	
	/**
	 */ 
	private static List<System.Type> defaultTypes = new List<System.Type>(new System.Type[] { typeof(int), typeof(float), typeof(string), typeof(bool), typeof(Color), typeof(Vector2), typeof(Vector3), typeof(Vector4), typeof(Rect), typeof(GUIContent), typeof(LayerMask) }) ;
	
}
