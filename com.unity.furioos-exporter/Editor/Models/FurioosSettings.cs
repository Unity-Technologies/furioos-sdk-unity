using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;


public class FurioosSettings
{
    #region properties

    public static string apiUrl
    {
        get
        {
            return "https://api.furioos.com/v1";
        }
    }

    public static string apiToken
    {
        get
        {
            return EditorPrefs.GetString("FurioosExporter.ApiToken", "");
        }
        set
        {
            EditorPrefs.SetString("FurioosExporter.ApiToken", value);
        }
    }

    public static bool isApiTokenValid
    {
        get { return EditorPrefs.GetInt("FurioosExporter.IsApiTokenValid") == 0 ? false : true; }
        set { var valueInt = value == true ? 1 : 0; EditorPrefs.SetInt("FurioosExporter.IsApiTokenValid", valueInt); }
    }

    public static bool isDeploying = false;    

    public enum VirtualMachineConfiguration
    {
        Standard,
        High,
        Extreme
    }

    public static Tuple<string, int>[] QualityConfiguration
    {
        get
        {
            return new Tuple<string, int>[]
            {
                Tuple.Create("Auto", 0),
                Tuple.Create("High (1080p - 60 fps)", 1080),
                Tuple.Create("Medium (720p - 30 fps)", 720),
                Tuple.Create("Low (360p - 30 fps)", 360),

            };
        }
    }

    public enum RatioMode
    {
        Fixed,
        All,
        Landscape,
        Portrait
    }

    public static String[] RatioModeValues =
    {
        "Fixed ratio",
        "Dynamic ratio",
        "Force Landscape",
        "Force Portrait"
    };

    public static string[] FixedRatioValuesPreset =
    {
        "16:9",
        "16:10",
        "4:3",
        "1:1",
        "21:9",
        "21:10",
        "custom"
    };
    #endregion

    #region Static Methods
    public static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        string projectName = s[s.Length - 2];
        return projectName;
    }


    public static string[] GetEnabledScenes()
    {
        var scenes = new List<string>();
        foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenes.Add(scene.path);
        }
        return scenes.ToArray();
    }


    #endregion
}



