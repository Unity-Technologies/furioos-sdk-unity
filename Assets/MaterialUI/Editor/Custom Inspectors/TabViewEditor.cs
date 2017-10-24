//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace MaterialUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TabView))]
    class TabViewEditor : MaterialBaseEditor
    {
        private TabView m_TabView;

        private SerializedProperty m_AutoTrackPages;
        private SerializedProperty m_Pages;
        private SerializedProperty m_OnlyShowSelectedPage;
        private SerializedProperty m_TabItemTemplate;
        private SerializedProperty m_TabsContainer;
        private SerializedProperty m_PagesContainer;
        private SerializedProperty m_PagesRect;
        private SerializedProperty m_Indicator;
        private SerializedProperty m_ShrinkTabsToFitThreshold;
        private SerializedProperty m_ForceStretchTabsOnLanscape;
        private SerializedProperty m_LowerUnselectedTabAlpha;
        private SerializedProperty m_CanScrollBetweenTabs;

        private AnimBool m_PagesAnimBool;

        void OnEnable()
        {
            OnBaseEnable();

            m_TabView = (TabView)target;

            m_AutoTrackPages = serializedObject.FindProperty("m_AutoTrackPages");
            m_Pages = serializedObject.FindProperty("m_Pages");
            m_OnlyShowSelectedPage = serializedObject.FindProperty("m_OnlyShowSelectedPage");
            m_TabItemTemplate = serializedObject.FindProperty("m_TabItemTemplate");
            m_TabsContainer = serializedObject.FindProperty("m_TabsContainer");
            m_PagesContainer = serializedObject.FindProperty("m_PagesContainer");
            m_PagesRect = serializedObject.FindProperty("m_PagesRect");
            m_Indicator = serializedObject.FindProperty("m_Indicator");
            m_ShrinkTabsToFitThreshold = serializedObject.FindProperty("m_ShrinkTabsToFitThreshold");
            m_ForceStretchTabsOnLanscape = serializedObject.FindProperty("m_ForceStretchTabsOnLanscape");
            m_LowerUnselectedTabAlpha = serializedObject.FindProperty("m_LowerUnselectedTabAlpha");
            m_CanScrollBetweenTabs = serializedObject.FindProperty("m_CanScrollBetweenTabs");

            m_PagesAnimBool = new AnimBool { value = !m_AutoTrackPages.boolValue };
            m_PagesAnimBool.valueChanged.AddListener(Repaint);
        }

        void OnDisable()
        {
            OnBaseDisable();

            m_PagesAnimBool.valueChanged.RemoveListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_AutoTrackPages);

            m_PagesAnimBool.target = !m_AutoTrackPages.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_PagesAnimBool.faded))
            {
                EditorGUILayout.PropertyField(m_Pages, true);
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_OnlyShowSelectedPage);

            TabPage[] pages = m_TabView.pages;

            if (pages.Length > 0)
            {
                string[] names = new string[pages.Length];

                for (int i = 0; i < pages.Length; i++)
                {
                    names[i] = (i + 1) + " - " + pages[i].name;
                }

                m_TabView.currentPage = EditorGUILayout.Popup("Current page", m_TabView.currentPage, names);

                m_TabView.SetPagesDirty();

                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(m_ShrinkTabsToFitThreshold);
            EditorGUILayout.PropertyField(m_ForceStretchTabsOnLanscape);
            EditorGUILayout.PropertyField(m_LowerUnselectedTabAlpha);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CanScrollBetweenTabs);
            if (EditorGUI.EndChangeCheck())
            {
                ((TabView)serializedObject.targetObject).canScrollBetweenTabs = m_CanScrollBetweenTabs.boolValue;
            }

            DrawFoldoutComponents(ComponentsSection);

            serializedObject.ApplyModifiedProperties();
        }

        private void ComponentsSection()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_TabItemTemplate);
            EditorGUILayout.PropertyField(m_TabsContainer);
            EditorGUILayout.PropertyField(m_PagesRect);
            EditorGUILayout.PropertyField(m_PagesContainer);
            EditorGUILayout.PropertyField(m_Indicator);
            EditorGUI.indentLevel--;
        }
    }
}