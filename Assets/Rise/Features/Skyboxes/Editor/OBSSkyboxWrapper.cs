using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Rise.Features.Skybox;

[CustomEditor(typeof(RSSkybox))]
public class OBSSkyboxWrapper : OBSWrapper 
{ 

	/** Référence vers l'inspector à manager. 
	 */
	private RSSkybox _skybox = null;
	private Texture2D _sunThumb = null;
	private Texture2D _northLine = null;

	private static readonly string SUN_THUMB_PATH = "Assets/Common/Features/Skyboxes/Textures/GUI/LensFlareThumb.png";
	private static readonly string NORTH_LINE_PATH = "Assets/Common/Features/Skyboxes/Textures/GUI/northLine.png";

	private static readonly int NORTH_LINE_SIZE = 2;

	private static readonly string SKIES_PATH = "Assets/Common/Features/Skyboxes/Textures/Skies";
	private static readonly string CUPS_PATH  = "Assets/Common/Features/Skyboxes/Textures/Cups";

	
/******************************************************************************************************************************************************************/

	private void Start()
	{
		_skybox = target as RSSkybox;

		if(_skybox != null)
		{
			_skybox.sun = _skybox.transform.FindChild("Sun").gameObject;
			_skybox.sky = _skybox.transform.FindChild("Sky").gameObject;

			if(_skybox.sky != null)
			{
				_skybox.sky.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				_skybox.sky.GetComponent<Renderer>().receiveShadows = false;
			}

			_skybox.cup = _skybox.transform.FindChild("Cup").gameObject;

			if(_skybox.cup != null)
			{
				_skybox.cup.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				_skybox.cup.GetComponent<Renderer>().receiveShadows = false;
			}

			_skybox.dynamicCupBaseMaterial = AssetDatabase.LoadMainAssetAtPath("Assets/Common/Features/Skyboxes/Materials/DynamicCup.mat") as Material;
			_skybox.dynamicSkyBaseMaterial = AssetDatabase.LoadMainAssetAtPath("Assets/Common/Features/Skyboxes/Materials/DynamicSky.mat") as Material;
			_skybox.staticCupBaseMaterial = AssetDatabase.LoadMainAssetAtPath("Assets/Common/Features/Skyboxes/Materials/StaticCup.mat") as Material;
			_skybox.staticSkyBaseMaterial = AssetDatabase.LoadMainAssetAtPath("Assets/Common/Features/Skyboxes/Materials/StaticSky.mat") as Material;

			if(_skybox.gameObject.activeInHierarchy)
			{
				PrefabUtility.DisconnectPrefabInstance(_skybox.gameObject);
			}
		}

		_sunThumb = AssetDatabase.LoadMainAssetAtPath(SUN_THUMB_PATH) as Texture2D;
		_northLine = AssetDatabase.LoadMainAssetAtPath(NORTH_LINE_PATH) as Texture2D;
	}

	public override void OnInspectorGUI()
	{
		this.WrapScript((Component) target);
	}

	protected override bool WrapObjectDatas(object target)
	{
		if(_skybox == null) { this.Start(); }
		
		bool change = false;
			
		#region DynamicSky Wrap	
		
		{
			bool oldIsOn = IsDynamic;
			bool newIsOn = EditorGUILayout.Toggle("Sky is dynamic", oldIsOn);
			
			if(oldIsOn != newIsOn) 
			{ 
				IsDynamic = newIsOn;		
				change = true;
			}
		}
	
		
		#endregion

		EditorGUILayout.Separator();

		#region Sky Wrap	

		if(IsDynamic)
		{
			change |= _skybox.useRealDate != (_skybox.useRealDate = EditorGUILayout.Toggle("Use Real Date",_skybox.useRealDate));
			change |= _skybox.automaticTimeSpeed != (_skybox.automaticTimeSpeed = EditorGUILayout.FloatField("Automatic Time Speed",_skybox.automaticTimeSpeed));
			//change |= _skybox.latitude != (_skybox.latitude = EditorGUILayout.FloatField("Scene latitude",_skybox.latitude));
			//change |= _skybox.longitude != (_skybox.longitude = EditorGUILayout.FloatField("Scene longitude",_skybox.longitude));
		}

		if(!HasSky())
		{
			EditorGUILayout.LabelField("(!) Sky missing (!)"); 
		}
		else
		{
			bool oldIsOn = SkyOn;
			bool newIsOn = EditorGUILayout.Toggle("Display sky", oldIsOn); 
			
			if(oldIsOn != newIsOn)
			{
				SkyOn = newIsOn;
				change = true;
			}

			if(SkyOn && !IsDynamic)
			{
				var materialsNames = getSkiesMaterialsNames();
				
				int oldIndex = SkyTexture == null ? -1 : materialsNames.IndexOf(Path.GetFileName(AssetDatabase.GetAssetPath(SkyTexture)));
				int newIndex = EditorGUILayout.Popup("SkyMaterial", oldIndex, materialsNames.ToArray());
				
				if(oldIndex != newIndex)
				{
					SkyTexture = AssetDatabase.LoadMainAssetAtPath(SKIES_PATH + "/" + materialsNames[newIndex]) as Texture2D;
					change = true;
				}

				Rect controlRect = new Rect(); Vector2 pos = new Vector2();
				
				if(EditorGUILayoutExtension.ClickableTexture(SkyTexture, out pos, out controlRect))
				{
					SkyNorthPosition = pos.x;
				}

				GUI.DrawTexture(new Rect(controlRect.x + SkyNorthPosition*controlRect.width - NORTH_LINE_SIZE/2, controlRect.y, NORTH_LINE_SIZE, controlRect.height), _northLine);
			}
		}
		
		#endregion

		EditorGUILayout.Separator();

		#region Cup	Wrap
		
		if(!HasCup())
		{
			EditorGUILayout.LabelField("(!) Cup missing (!)"); 
		}
		else
		{
			bool oldIsOn = CupOn;
			bool newIsOn = EditorGUILayout.Toggle("Display cup", oldIsOn);
			
			if(oldIsOn != newIsOn)
			{
				CupOn = newIsOn;
				change = true;
			}

			if(CupOn)
			{
				var materialsNames = getCupsMaterialsNames();
				
				int oldIndex = CupTexture == null ? -1 : materialsNames.IndexOf(Path.GetFileName(AssetDatabase.GetAssetPath(CupTexture)));
				int newIndex = EditorGUILayout.Popup("CupMaterial", oldIndex, materialsNames.ToArray());
				
				if(oldIndex != newIndex)
				{
					CupTexture = AssetDatabase.LoadMainAssetAtPath(CUPS_PATH + "/" +  materialsNames[newIndex]) as Texture2D;
					change = true;
				}			

				Color oldColor = CupColor;
				Color newColor = EditorGUILayout.ColorField("Cup Color", oldColor);

				Color oldAmbienteColor = AmbienteColor;
				Color ambienteColor = EditorGUILayout.ColorField("Ambiente Color", AmbienteColor);
				
				if(oldColor != newColor)
				{
					CupColor = newColor;
					change = true;
				}

				if(oldAmbienteColor != ambienteColor)
				{
					AmbienteColor = ambienteColor;
					change = true;
				}

				Rect controlRect = new Rect(); Vector2 pos = new Vector2();
				
				if(EditorGUILayoutExtension.ClickableTexture(CupTexture, out pos, out controlRect))
				{
					CupNorthPosition = pos.x;
				}
				
				GUI.DrawTexture(new Rect(controlRect.x + CupNorthPosition*controlRect.width - NORTH_LINE_SIZE/2, controlRect.y, NORTH_LINE_SIZE, controlRect.height), _northLine);
			}
		}
		
		#endregion

		EditorGUILayout.Separator();

		#region Sun Wrap
		
		{
			bool oldIsOn = SunOn;
			bool newIsOn = EditorGUILayout.Toggle("Activate sun", oldIsOn);
			
			if(oldIsOn != newIsOn)
			{
				SunOn = newIsOn; change = true;
			}			

			if(SunOn && !IsDynamic)
			{
				Rect controlRect = new Rect(); Vector2 pos = new Vector2();

				if(EditorGUILayoutExtension.ClickableTexture(SkyTexture, out pos, out controlRect))
				{
					SunThumbPos = pos;
				}

				float sunSize = controlRect.height/10;

				GUI.DrawTexture(new Rect(controlRect.x + SunThumbPos.x*controlRect.width - sunSize/2, controlRect.y + SunThumbPos.y*controlRect.height - sunSize/2, sunSize, sunSize), _sunThumb);
			}
		}

		#endregion
		
		return change;
	}

	private List<string> getSkiesMaterialsNames()
	{
		List<string> materialsNames = new List<string>();
		
		string[] materialsPaths = System.IO.Directory.GetFiles(SKIES_PATH);

		foreach(string materialPath in materialsPaths)
		{
			if(!materialPath.EndsWith(".meta")) { materialsNames.Add(Path.GetFileName(materialPath)); }
		}
		
		return materialsNames;
	}
	
	private List<string> getCupsMaterialsNames()
	{
		List<string> materialsNames = new List<string>();

		string[] materialsPaths = System.IO.Directory.GetFiles(CUPS_PATH);
		
		foreach(string materialPath in materialsPaths)
		{
			if(!materialPath.EndsWith(".meta")) { materialsNames.Add(Path.GetFileName(materialPath)); }
		}
		
		return materialsNames;
	}

	#region Dynamic Methods
	private bool IsDynamic
	{
		get 
		{ 
			return _skybox.isDynamic;
		} 
		set 
		{ 
			_skybox.isDynamic = value;
		} 
	}
	#endregion

	#region Sky Methods
	private bool HasSky()
	{ 
		return _skybox.sky != null; 
	}

	private Texture2D SkyTexture 
	{ 
		get { return _skybox.skyTexture; } 
		set { _skybox.skyTexture = value; _skybox.Reset();} 
	}

	private bool SkyOn 
	{ 
		get { return HasSky() && _skybox.sky.activeSelf; } 
		set { if(HasSky())_skybox.sky.SetActive(value);} 
	}	

	private float SkyNorthPosition
	{
		get 
		{ 
			return ((-1*_skybox.staticSkyOrientation) + 180.0f)/360.0f; 
		}
		set 
		{ 
		_skybox.staticSkyOrientation = 180.0f -value * 360.0f; _skybox.Reset(); _skybox.Update();
		}
	}
	#endregion
	
	#region Cup Methods	
	private bool HasCup()
	{
		return _skybox.cup != null;
	}

	private Color CupColor
	{
		get { return _skybox.cupColor; }
		set { _skybox.cupColor = value;_skybox.Reset(); }
	}

	private Color AmbienteColor {
		get { return _skybox.ambientColor; }
		set { 
			_skybox.ambientColor = value;
			_skybox.Reset (); 
		}
	}
	
	private Texture2D CupTexture
	{
		get { return _skybox.cupTexture; }
		set { _skybox.cupTexture = value; _skybox.Reset(); }
	}
	
	private bool CupOn
	{
		get { return HasCup() && _skybox.cup.activeSelf; }
		set { if(HasCup())_skybox.cup.SetActive(value) ; }
	}

	private float CupNorthPosition
	{
		get
		{
			return ((-1*_skybox.cupOrientation) + 180.0f)/360.0f; 
		}
		set
		{
			_skybox.cupOrientation = 180.0f -value*360.0f; _skybox.Reset(); _skybox.Update();
		}
	}
	#endregion
	
	#region Sun Methods
	public bool HasSun()
	{
		return _skybox.sun != null;
	}

	public bool SunOn
	{
		get { return HasSun() && _skybox.sun.activeSelf; }
		set { if(HasSun()) { _skybox.sun.SetActive(value); } }
	}

	public Vector2 SunThumbPos
	{
		get
		{
			return new Vector2(_skybox.staticSunOrientation/360.0f + 0.5f,1.0f-(_skybox.staticSunTilt / 90.0f));
		}
		set
		{

			_skybox.staticSunOrientation = value.x * 360.0f - 180.0f;
			_skybox.staticSunTilt = 90.0f - value.y * 90.0f;

			_skybox.Reset();
			_skybox.Update();

		}
	}
	#endregion
}
