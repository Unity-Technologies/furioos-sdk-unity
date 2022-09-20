
#if UNITY_EDITOR
using UnityEngine;
using Furioos.ConnectionKit;
using Furioos.SDK;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
#endif

public class FurioosPackageVersionHandler: ScriptableObject
{
#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    private static void Init()
    {
        CompilationPipeline.compilationFinished -= OnCompilationFinished;
        CompilationPipeline.compilationFinished += OnCompilationFinished;

    }

    private static void OnCompilationFinished(object obj)
    {
        var assemblyConnectionKit = typeof(FsConnectionHandler).Assembly;
        var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(assemblyConnectionKit);
         
        var assemblyFurioosSDK = typeof(FurioosSDK).Assembly;
        var packageInfoFurioosSDK = UnityEditor.PackageManager.PackageInfo.FindForAssembly(assemblyFurioosSDK);
         
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        var guid = AssetDatabase.FindAssets($"t:FurioosPackageVersion", new[] { "Assets/Resources"}).FirstOrDefault(); 

        FurioosPackageVersion asset;
        if (!string.IsNullOrWhiteSpace(guid))
        {
            asset = AssetDatabase.LoadAssetAtPath<FurioosPackageVersion>(AssetDatabase.GUIDToAssetPath(guid));
        }
        else
        {
            asset = ScriptableObject.CreateInstance<FurioosPackageVersion>();
            asset.name = nameof(FurioosPackageVersion);
            asset.hideFlags = HideFlags.NotEditable;

            AssetDatabase.CreateAsset(asset, "Assets/Resources/FurioosPackageVersion.asset");
        }

        asset.FurioosConnectionKitVersion = packageInfo.version;
        asset.FurioosSDKVersion = packageInfoFurioosSDK.version;

        EditorUtility.SetDirty(asset);
    }
#endif


}