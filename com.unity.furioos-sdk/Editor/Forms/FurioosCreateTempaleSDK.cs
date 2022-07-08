using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using System;

[InitializeOnLoadAttribute]
public class CreateTemplateSDKForm : EditorWindow
{
    const string DIR_FURIOOS_ASSETS = @"Assets/Scripts/Furioos";
    const string DIR_FURIOOS_SDK_PACKAGE = @"Packages/com.unity.furioos-sdk/Editor/ScriptTemplates";
    const string FURIOOS_SDK_TEMPLATE_FILE = @"01-C# Furioos Templates__FurioosSdkSample-FurioosSdkSample.cs.txt";

    private string fileName = "FurioosSDK Handler";
    private string className = "";
    private GameObject gameObject;
    private bool generateButtonActive = false;
    private static bool justRecompiled;
    private bool waitingForRecompiling;
    private bool dynamicResolution = true;

    static CreateTemplateSDKForm()
    {
        justRecompiled = true;
    }

    [MenuItem("Furioos/SDK/Furioos SDK Generator")]
    public static void ShowWindow()
    {
        float windowWidth = 350;
        float windowHeight = 200;

        Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Packages/com.unity.furioos-exporter/Editor/Ressources/U_Furioos_Logo_Small.png");
        GUIContent titleContent = new GUIContent("Furioos SDK Generator", icon);
        var window = GetWindow<CreateTemplateSDKForm>("");
        window.titleContent = titleContent;

        var rect = EditorGUIUtility.GetMainWindowPosition();
        window.position = new Rect(rect.x + (rect.width/2) - (windowWidth / 2), rect.y + (rect.height/2) - (windowHeight / 2), windowWidth, windowHeight);
    }

    bool ClassNameIsValid()
    {
        className = className.Replace(" ", "").Trim();
        if (string.IsNullOrEmpty(className))
        {
            EditorUtility.DisplayDialog("Unable to generate", "Please specify a valid class name.", "Close");
            return false;
        }
        return true;
    }

    bool GenerateFurioosSdkSampleScript()
    {
        string filePath = DIR_FURIOOS_ASSETS + "/" + className + ".cs";
        if (File.Exists(filePath))
        {
            justRecompiled = true;
            return false;
        }

        IEnumerable<string> lines = File.ReadLines(DIR_FURIOOS_SDK_PACKAGE + "/" + FURIOOS_SDK_TEMPLATE_FILE);
        Directory.CreateDirectory(DIR_FURIOOS_ASSETS);
        var file = File.CreateText(filePath);
        foreach (var line in lines)
        {
            string newLine = line;
            if (line.Contains("#SCRIPTNAME#"))
            {
                newLine = line.Replace("#SCRIPTNAME#", className);
            }
            file.WriteLine(newLine);
        }
        file.Close();
        return true;
    }

    void GenerateScript()
    {
        GenerateFurioosSdkSampleScript();
        AssetDatabase.Refresh();
    }

    public void AddComponent()
    {
        MonoScript monoFurioosSdkScript = AssetDatabase.LoadAssetAtPath(DIR_FURIOOS_ASSETS + "/" + className + ".cs", typeof(MonoScript)) as MonoScript;
        Type monoFurioosSdkScriptClass = monoFurioosSdkScript.GetClass();
        if (gameObject.GetComponent(monoFurioosSdkScriptClass) == null)
            gameObject.AddComponent(monoFurioosSdkScriptClass);

        if (dynamicResolution)
        {
            gameObject.AddComponent<FurioosDynamicResolution>();
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = true;
        }
    }


    void DrawClassNameField()
    {
        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField("Enter your classname to generate a Furioos handler");
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        fileName = EditorGUILayout.TextField("Classname", fileName);
        GUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;
        DrawWarningMessage();
    }
    void DrawDynamicResolution()
    {
        EditorGUI.indentLevel++;

        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Dynamic resolution", "Dynamic resolution"), GUILayout.MaxWidth(135));
        dynamicResolution = EditorGUILayout.Toggle("", dynamicResolution);
        EditorGUILayout.EndHorizontal();

        if (dynamicResolution)
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = Color.grey;
            EditorGUILayout.LabelField("The following options will be actived: ", labelStyle);
            EditorGUILayout.LabelField(" - Run in Background", labelStyle);
            EditorGUILayout.LabelField(" - Resizable Window", labelStyle);
        }
        EditorGUI.indentLevel--;

    }

    void DrawWarningMessage()
    {
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginHorizontal();
        className = fileName;
        className = className.Replace(" ", "").Trim();
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.normal.textColor = Color.red;
        generateButtonActive = true;
        if (string.IsNullOrEmpty(className))
        {
            EditorGUILayout.LabelField("Your Classname is empty.", labelStyle);
            generateButtonActive = false;
        }

        string filePath = DIR_FURIOOS_ASSETS + "/" + className + ".cs";
        if (File.Exists(filePath))
        {
            EditorGUILayout.LabelField("Your Classname already exists.", labelStyle);
            generateButtonActive = false;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;
    }

    void DrawGenerateAndCancelButton()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUI.indentLevel++;
        GUILayout.Space(10);
        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
        GUI.enabled = generateButtonActive;
        if (GUILayout.Button("Generate"))
        {
            className = fileName;
            if (!ClassNameIsValid())
                return;
            GUI.enabled = false;
            EditorUtility.DisplayProgressBar("Generate Furioos SDK handler", "", 0);
            GenerateScript();
            var gameObj = new GameObject(fileName);
            EditorUtility.SetDirty(gameObj);
            Selection.activeGameObject = gameObj;
            if (Selection.activeGameObject != null)
            {
                waitingForRecompiling = true;
                gameObject = Selection.activeGameObject;
                AssetDatabase.ImportAsset(DIR_FURIOOS_ASSETS);
            }
            EditorUtility.DisplayProgressBar("Generate Furioos SDK handler", "", 0.5f);
        }
        GUI.enabled = true;
        GUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;
    }

    public void OnGUI()
    {
        GUILayout.Space(10);
        DrawClassNameField();
        DrawDynamicResolution();
        GUILayout.FlexibleSpace();
        DrawGenerateAndCancelButton();
        GUILayout.Space(5);
    }

    public void Update()
    {
        if (justRecompiled && waitingForRecompiling)
        {
            waitingForRecompiling = false;
            AddComponent();
            EditorUtility.ClearProgressBar();
            GUI.enabled = true;
            Close();
        }
        justRecompiled = false;
    }

}
