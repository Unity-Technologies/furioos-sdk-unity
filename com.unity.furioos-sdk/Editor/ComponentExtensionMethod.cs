using System;
using System.Reflection;
using UnityEngine;

public static class ComponentExtensionMethod
{
    public static Component AddExternalComponent(this GameObject obj, string scriptName)
    {
        Component component = addExternalComponent(obj, scriptName);
        if (component == null)
        {
            Debug.LogError("Failed to add component");
            return null;
        }
        return component;
    }

    private static Component addExternalComponent(GameObject obj, string className)
    {
        Assembly assembly = null;
        try
        {
            assembly = Assembly.Load("Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            if (assembly == null)
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to Load Assembly" + e.Message);
        }

        Type type = assembly.GetType(className);
        if (type == null)
            return null;

        Component cmpnt = obj.AddComponent(type);
        return cmpnt;
    }
}
