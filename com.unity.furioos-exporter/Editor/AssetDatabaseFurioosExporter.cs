using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

[InitializeOnLoad]
public class AssetDatabaseFurioosExporter
{

    static AssetDatabaseFurioosExporter()
    {
        Events.registeredPackages += Events_registeredPackages;
    }

    private static void Events_registeredPackages(PackageRegistrationEventArgs eventArg)
    {
        var addedPackages = eventArg.added;

        foreach (var item in addedPackages)
        {
            if (item.name == "com.unity.furioos-exporter")
            {
                EditorUtility.DisplayDialog("Furioos Exporter package", "Restart your Editor to complete the installation", "Ok");
            }
        }

    }
}
