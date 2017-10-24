//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
	public class VectorImageManagerWindow : EditorWindow
	{
		[MenuItem("Window/MaterialUI/VectorImageManager", false, 100)]
		private static void ShowWindow()
		{
			VectorImageManagerWindow window = (VectorImageManagerWindow)EditorWindow.GetWindow(typeof(VectorImageManagerWindow), false, " VectorImage Manager");
			window.minSize = new Vector2(390, 300);
		}

		/*
		* Renderers
		*/
		private ImportCustomFontSection m_ImportCustomFontSection;
		private ImportWebFontSection m_ImportWebFontSection;
		private ImportFontParametersSection m_ImportFontParametersSection;

		/*
		* Init
		*/
		void OnEnable()
		{
			hideFlags = HideFlags.HideAndDontSave;

			if (m_ImportCustomFontSection == null) m_ImportCustomFontSection = new ImportCustomFontSection();
			if (m_ImportWebFontSection == null) m_ImportWebFontSection = new ImportWebFontSection();
			if (m_ImportFontParametersSection == null) m_ImportFontParametersSection = new ImportFontParametersSection();
		}

		private void OnFocus()
		{
			Repaint();
		}
		
		private void OnSelectionChange()
		{
			Repaint();
		}

		/*
		* Drawing
		*/
		private Vector2 m_ScrollPosition;

		void OnGUI()
		{
			using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(m_ScrollPosition))
			{
				m_ScrollPosition = scrollViewScope.scrollPosition;

				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Space(5f);
					
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Space(10f);
						
						m_ImportFontParametersSection.DrawInspector();
						GUILayout.Space(5f);
						m_ImportCustomFontSection.DrawInspector();
						GUILayout.Space(5f);
						m_ImportWebFontSection.DrawInspector();
					}
					
					GUILayout.Space(5f);
				}
			}
		}

		/*
		* Drawing util methods
		*/
		public static void BeginContents()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(4f);
			EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
			EditorGUILayout.BeginVertical();
			GUILayout.Space(4f);
		}

		public static void EndContents()
		{
			GUILayout.Space(5f);
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(3f);
			EditorGUILayout.EndHorizontal();
		}

		public static void DrawHeader(string title)
		{
			GUILayout.Space(5f);

			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(3f);
				
				if (!GUILayout.Toggle(true, "<b><size=11>" + title + "</size></b>", "dragtab"))
				{
				}
				
				GUILayout.Space(2f);
			}
			
			GUI.backgroundColor = Color.white;
		}
	}
}