using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

[InitializeOnLoad]
public class AssetDatabaseFurioosSDK
{
    const string DIR_TEMPLATE_ASSETS = "Assets/ScriptTemplates";
    const string DIR_FURIOOS_SDK_PACKAGE = "Packages/com.unity.furioos-sdk/Editor/ScriptTemplates";
    const string FURIOOS_TEMPLATE_FILE = "01-C# Furioos Templates__FurioosSdkSample-FurioosSdkSample.cs.txt";

    static AssetDatabaseFurioosSDK()
    {
        Events.registeredPackages += Events_registeredPackages;
    }

    private static void Events_registeredPackages(PackageRegistrationEventArgs eventArg)
    {
        var addedPackages = eventArg.added;
        var removedPackages = eventArg.removed;

        foreach (var item in addedPackages)
        {
            if (item.name == "com.unity.furioos-sdk")
            {
                CopyFurioosTemplate();
            }
        }

        foreach (var item in removedPackages)
        {
            if (item.name == "com.unity.furioos-sdk")
            {
                RemoveFurioosTemplate();
            }
        }

    }

    private static void CopyFurioosTemplate()
    {
        Directory.CreateDirectory(DIR_TEMPLATE_ASSETS);
        if (!File.Exists(DIR_TEMPLATE_ASSETS + "/" + FURIOOS_TEMPLATE_FILE))
        {
            File.Copy(DIR_FURIOOS_SDK_PACKAGE + "/" + FURIOOS_TEMPLATE_FILE, DIR_TEMPLATE_ASSETS + "/" + FURIOOS_TEMPLATE_FILE);
            Debug.Log("Copy Furioos template");
            EditorUtility.DisplayDialog("Furioos SDK package", "Restart your Editor to complete the installation", "Ok");
        }
    }

    private static void RemoveFurioosTemplate()
    {
        Directory.CreateDirectory(DIR_TEMPLATE_ASSETS);
        if (File.Exists(DIR_TEMPLATE_ASSETS + "/" + FURIOOS_TEMPLATE_FILE))
        {
            File.Delete(DIR_TEMPLATE_ASSETS + "/" + FURIOOS_TEMPLATE_FILE);
            Debug.Log("Delete Furioos template");
            EditorUtility.DisplayDialog("Furioos SDK package", "Restart your Editor to complete the uninstallation", "Ok");
        }
    }
}
