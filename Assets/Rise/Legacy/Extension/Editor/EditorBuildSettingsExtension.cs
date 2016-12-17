using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class EditorBuildSettingsExtension
{
	public static void SetAsFirstScene(string path)
	{
		path = path.Replace("\\", "/");

		int index = EditorBuildSettingsExtension.IndexOfScene(path);

		if(index > 0)
		{
			EditorBuildSettingsScene temp = EditorBuildSettings.scenes[0];
			EditorBuildSettings.scenes[0] = EditorBuildSettings.scenes[index];
			EditorBuildSettings.scenes[0] = temp;
		}
	}

	public static void SetAsSecondScene(string path)
	{
		path = path.Replace("\\", "/");

		int index = EditorBuildSettingsExtension.IndexOfScene(path);

		if(index != -1 && index != 1)
		{
			EditorBuildSettingsScene temp = EditorBuildSettings.scenes[1];
			EditorBuildSettings.scenes[1] = EditorBuildSettings.scenes[index];
			EditorBuildSettings.scenes[1] = temp;
		}
	}

	public static int IndexOfScene(string path)
	{
		path = path.Replace("\\", "/");

		for(int i = 0; i < EditorBuildSettings.scenes.Length; i++)
		{
			if(EditorBuildSettings.scenes[i].path == path) { return i; }
		}

		return -1;
	}
}
