
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;

public class ProjectInit : EditorWindow 
{
	#region Attributes
	
	/** Constantes Liées au Management des Inputs
	 */
	private const string INPUT_TYPE_KEY_OR_MOUSE = "0";
	private const string INPUT_TYPE_MOUSE_MOVEMENT = "1";
	private const string INPUT_TYPE_JOYSTICK_AXIS = "2";

	private const string ROOT_FOLDER = "Assets";
	private const string PROJECT_FOLDER = "Project";
	private const string STATICASSETS_FOLDER = "StaticAssets";
	private const string THREED_FOLDER = "3D";
	private const string MEDIAS_FOLDER = "Medias";
	private const string UI_FOLDER = "UI";
	private const string SCENES_FOLDER = "Scenes";
	private const string SCRIPTS_FOLDER = "Scripts";
	private const string RESOURCES_FOLDER = "Resources";

	private const string FOLDER_SEPARATOR = "/";

	private const string LOADER_SCENE = "Loader.unity";
	private const string BUILDING_SCENE = "Building.unity";

	private GameObject manager;
	private GameObject uiCommon;
	private GameObject uiProject;
	private GameObject mainCameras;
	private GameObject interactionPointsManager;


	#endregion	

	#region Displaying

	[MenuItem("Observ3d/Project/Init")]	
	public static void OpenWindow() 
	{
		ProjectInit window = (ProjectInit) EditorWindow.GetWindowWithRect(typeof(ProjectInit), new Rect(0, 0, 150, 80));
		window.title = "Project Init";
	}

	void OnGUI() {

		if (GUILayout.Button ("Apply")) {
			GenerateInputManager ();
			GenerateProjectArchitecture ();
			GenerateProjectScenes ();

			AssetDatabase.Refresh();
		}
	}
	
	#endregion

	#region Inputs Management
	
	private void WriteInput(StreamWriter sr, string name,string action,string type,string negativeAction = "")
	{
		sr.WriteLine("  - serializedVersion: 3");
		sr.WriteLine("    m_Name: "+name);
		sr.WriteLine("    descriptiveName: ");
		sr.WriteLine("    descriptiveNegativeName: ");
		
		if(type==INPUT_TYPE_KEY_OR_MOUSE)sr.WriteLine("    negativeButton: "+negativeAction);
		else sr.WriteLine("    negativeButton: ");
		
		if(type==INPUT_TYPE_KEY_OR_MOUSE)sr.WriteLine("    positiveButton: "+action);
		else sr.WriteLine("    positiveButton: ");
		
		sr.WriteLine("    altNegativeButton: ");
		sr.WriteLine("    altPositiveButton: ");
		sr.WriteLine("    gravity: 0");
		
		if(type==INPUT_TYPE_MOUSE_MOVEMENT)sr.WriteLine("    dead: 0");
		else sr.WriteLine("    dead: .35");

		if (type == INPUT_TYPE_KEY_OR_MOUSE)
			sr.WriteLine ("    sensitivity: 1000");
		else
			sr.WriteLine ("    sensitivity: 1");

		sr.WriteLine("    snap: 0");
		sr.WriteLine("    invert: 0");
		sr.WriteLine("    type: "+type);
		
		if(type==INPUT_TYPE_MOUSE_MOVEMENT || type==INPUT_TYPE_JOYSTICK_AXIS)sr.WriteLine("    axis: "+action);
		else sr.WriteLine("    axis: 0");
		
		sr.WriteLine("    joyNum: 0");
	}
	
	
	private void GenerateInputManager ()
	{
		StreamWriter sr = File.CreateText("ProjectSettings" + Path.DirectorySeparatorChar + "InputManager.asset");
		
		sr.WriteLine("%YAML 1.1");
		sr.WriteLine("%TAG !u! tag:unity3d.com,2011:");
		sr.WriteLine("--- !u!13 &1");
		sr.WriteLine("InputManager:");
		sr.WriteLine("  m_ObjectHideFlags: 0");
		sr.WriteLine("  m_Axes:");
		
		WriteInput(sr, "mouse scrollwheel", "2", INPUT_TYPE_MOUSE_MOVEMENT);
		
		for(int i = 0; i < 20; i++){
			WriteInput(sr, "joystick button " + i.ToString(), "joystick button " + i.ToString(), INPUT_TYPE_KEY_OR_MOUSE);
		}
		
		for(int i = 0; i < 20; i++){
			WriteInput(sr, "joystick axis " + i.ToString(), i.ToString(), INPUT_TYPE_JOYSTICK_AXIS);	
		}
		
		sr.Close();
	}
	
	#endregion

	#region Project architecture

	public void GenerateProjectArchitecture() {
		Directory.CreateDirectory (ROOT_FOLDER + FOLDER_SEPARATOR + PROJECT_FOLDER);
		Directory.CreateDirectory (ROOT_FOLDER + FOLDER_SEPARATOR + PROJECT_FOLDER + FOLDER_SEPARATOR + STATICASSETS_FOLDER);
		Directory.CreateDirectory (ROOT_FOLDER + FOLDER_SEPARATOR + PROJECT_FOLDER + FOLDER_SEPARATOR + STATICASSETS_FOLDER + FOLDER_SEPARATOR + THREED_FOLDER);
		Directory.CreateDirectory (ROOT_FOLDER + FOLDER_SEPARATOR + PROJECT_FOLDER + FOLDER_SEPARATOR + STATICASSETS_FOLDER + FOLDER_SEPARATOR + MEDIAS_FOLDER);
		Directory.CreateDirectory (ROOT_FOLDER + FOLDER_SEPARATOR + PROJECT_FOLDER + FOLDER_SEPARATOR + STATICASSETS_FOLDER + FOLDER_SEPARATOR + UI_FOLDER);
		Directory.CreateDirectory (ROOT_FOLDER + FOLDER_SEPARATOR + PROJECT_FOLDER + FOLDER_SEPARATOR + SCENES_FOLDER);
		Directory.CreateDirectory (ROOT_FOLDER + FOLDER_SEPARATOR + PROJECT_FOLDER + FOLDER_SEPARATOR + SCRIPTS_FOLDER);
		Directory.CreateDirectory (ROOT_FOLDER + FOLDER_SEPARATOR + RESOURCES_FOLDER);
	}

	public void GenerateProjectScenes() {
		CreateScene (LOADER_SCENE);
		CreateScene (BUILDING_SCENE);
	}

	public void CreateScene(string name) {
		EditorApplication.NewScene ();
		InitScene ();
		EditorApplication.SaveScene (ROOT_FOLDER + FOLDER_SEPARATOR + PROJECT_FOLDER + FOLDER_SEPARATOR + SCENES_FOLDER + FOLDER_SEPARATOR + name);
	}

	public void InitScene() {
		DestroyImmediate (GameObject.Find ("Main Camera"));
		
		uiProject = (GameObject)Resources.Load ("Prefabs/UI_Project");
		interactionPointsManager = (GameObject)Resources.Load ("Prefabs/Interaction Points Manager");
		uiCommon = (GameObject)Resources.Load ("Prefabs/UI_Common");
		mainCameras = (GameObject)Resources.Load ("Prefabs/Main Cameras");
		manager = (GameObject)Resources.Load ("Prefabs/Manager");
		
		GameObject tmpUiProject = (GameObject)Instantiate (uiProject);
		GameObject tmpInteractionPointsManager = (GameObject)Instantiate (interactionPointsManager);
		GameObject tmpUiCommon = (GameObject)Instantiate (uiCommon);
		GameObject tmpMainCameras = (GameObject)Instantiate(mainCameras);
		GameObject tmpManager = (GameObject)Instantiate (manager);

		tmpManager.name = manager.name;
		tmpUiCommon.name = uiCommon.name;
		tmpUiProject.name = uiProject.name;
		tmpMainCameras.name = mainCameras.name;
		tmpInteractionPointsManager.name = interactionPointsManager.name;
	}

	#endregion
}
