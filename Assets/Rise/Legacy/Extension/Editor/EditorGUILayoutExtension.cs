using UnityEngine;
using UnityEditor;

public static class EditorGUILayoutExtension
{
	public static bool ClickableTexture(Texture2D texture, out Vector2 pos, out Rect controlRect, int width = -1, int height = -1)
	{
		GUIStyle style = new GUIStyle(); style.normal.background = texture;
		if(width == -1)  { width = Screen.width - 20; }
		if(height == -1) { height = (int) (width*style.normal.background.height/style.normal.background.width); }

		bool clicked = false;
		pos = Vector2.zero;
		
		controlRect = EditorGUILayout.BeginHorizontal() ;
		if(GUILayout.Button("", style, GUILayout.Width(width), GUILayout.Height(height)))
		{
			pos = new Vector2 (
				((float) (Event.current.mousePosition.x - controlRect.x))/controlRect.width,
				((float) (Event.current.mousePosition.y - controlRect.y))/controlRect.height
				) ;
			clicked = true;
		}
		EditorGUILayout.EndHorizontal() ;
		
		return clicked;
	}
}
