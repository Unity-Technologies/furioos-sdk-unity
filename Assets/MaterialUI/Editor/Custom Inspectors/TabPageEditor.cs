//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    [CustomEditor(typeof(TabPage))]
    public class TabPageEditor : Editor
    {
		private SerializedProperty m_Interactable;
		private SerializedProperty m_DisableWhenNotVisible;
		private SerializedProperty m_ShowDisabledPanel;
        private SerializedProperty m_TabName;
        private SerializedProperty m_TabIcon;
        private SerializedProperty m_TabIconType;

        void OnEnable()
        {
			m_Interactable = serializedObject.FindProperty("m_Interactable");
			m_DisableWhenNotVisible = serializedObject.FindProperty("m_DisableWhenNotVisible");
			m_ShowDisabledPanel = serializedObject.FindProperty("m_ShowDisabledPanel");
            m_TabName = serializedObject.FindProperty("m_TabName");
            m_TabIcon = serializedObject.FindProperty("m_TabIcon");
            m_TabIconType = serializedObject.FindProperty("m_TabIcon.m_ImageDataType");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

			EditorGUILayout.PropertyField(m_Interactable);
			EditorGUILayout.PropertyField(m_DisableWhenNotVisible);
			EditorGUILayout.PropertyField(m_ShowDisabledPanel);
            EditorGUILayout.PropertyField(m_TabName);
            EditorGUILayout.PropertyField(m_TabIconType, new GUIContent("Tab Icon Type"));
            EditorGUILayout.PropertyField(m_TabIcon);

            serializedObject.ApplyModifiedProperties();
        }
    }
}