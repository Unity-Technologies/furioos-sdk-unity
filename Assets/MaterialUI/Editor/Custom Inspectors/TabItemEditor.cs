//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;

namespace MaterialUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TabItem))]
	class TabItemEditor : MaterialBaseEditor
    {
		private SerializedProperty m_ItemText;
		private SerializedProperty m_ItemIcon;
		private SerializedProperty m_TabView;

        void OnEnable()
        {
			OnBaseEnable();
            
			m_ItemText = serializedObject.FindProperty("m_ItemText");
			m_ItemIcon = serializedObject.FindProperty("m_ItemIcon");
			m_TabView = serializedObject.FindProperty("m_TabView");
        }

        void OnDisable()
        {
			OnBaseDisable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

			DrawFoldoutComponents(ComponentsSection);

            serializedObject.ApplyModifiedProperties();
		}

		private void ComponentsSection()
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(m_ItemText);
			EditorGUILayout.PropertyField(m_ItemIcon);
			EditorGUILayout.PropertyField(m_TabView);
			EditorGUI.indentLevel--;
		}
    }
}