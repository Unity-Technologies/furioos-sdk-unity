using System;
using UnityEngine;

public class FurioosPackageVersion: ScriptableObject
{

    public string FurioosSDKVersion;
    public string FurioosConnectionKitVersion;

    private static FurioosPackageVersion _instance;
    public static FurioosPackageVersion Instance
    {
        get 
        {
            if (_instance == null)
            {
                try
                {
                    _instance = Resources.Load<FurioosPackageVersion>("FurioosPackageVersion");
                } 
                catch { }                
            }
            return _instance;

        }

    }


}