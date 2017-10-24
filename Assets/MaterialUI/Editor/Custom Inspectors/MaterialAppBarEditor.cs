//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [CustomEditor(typeof(MaterialAppBar), true)]
    [CanEditMultipleObjects]
    public class MaterialAppBarEditor : MaterialBaseEditor
    {
        private MaterialAppBar m_MaterialAppBar;

        private SerializedProperty m_TitleText;
        private SerializedProperty m_PanelGraphic;
        private SerializedProperty m_Shadow;

        private SerializedProperty m_Buttons;

        private SerializedProperty m_AnimationDuration;

        void OnEnable()
        {
            OnBaseEnable();
            m_MaterialAppBar = (MaterialAppBar)serializedObject.targetObject;
            m_TitleText = serializedObject.FindProperty("m_TitleText");
            m_PanelGraphic = serializedObject.FindProperty("m_PanelGraphic");
            m_Shadow = serializedObject.FindProperty("m_Shadow");
            m_Buttons = serializedObject.FindProperty("m_Buttons");
            m_AnimationDuration = serializedObject.FindProperty("m_AnimationDuration");
        }

        void OnDisable()
        {
            OnBaseDisable();
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_AnimationDuration);

            DrawFoldoutExternalProperties(ExternalPropertiesSection);

            DrawFoldoutCustom1("Buttons", OtherButtonsSection);

            DrawFoldoutComponents(ComponentsSection);

            serializedObject.ApplyModifiedProperties();
        }

        private bool ExternalPropertiesSection()
        {
            bool result = false;

            Func<GameObject, Graphic> getTitleTextFunc = go =>
            {
                MaterialAppBar appBar = go.GetComponent<MaterialAppBar>();
                return appBar == null ? null : appBar.titleText;
            };

            Func<GameObject, Graphic> getPanelGraphicFunc = go =>
            {
                MaterialAppBar appBar = go.GetComponent<MaterialAppBar>();
                return appBar == null ? null : appBar.panelGraphic;
            };

            Utils.SetBoolValueIfTrue(ref result, InspectorFields.GraphicColorMultiField("Title Text", getTitleTextFunc, m_TitleText.objectReferenceValue as Graphic));
            Utils.SetBoolValueIfTrue(ref result, InspectorFields.GraphicColorMultiField("Panel Graphic", getPanelGraphicFunc, m_PanelGraphic.objectReferenceValue as Graphic));

            return result;
        }

        private bool OtherButtonsSection()
        {
            bool result = false;

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_Buttons, new GUIContent("References"), true);
            EditorGUI.indentLevel--;

            if (m_MaterialAppBar.buttons == null) return false;

            for (int i = 0; i < m_MaterialAppBar.buttons.Length; i++)
            {
                Utils.SetBoolValueIfTrue(ref result, InspectorFields.MaterialButtonField(m_MaterialAppBar.buttons[i]));
            }

            return result;
        }

        private void ComponentsSection()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_TitleText);
            EditorGUILayout.PropertyField(m_PanelGraphic);
            EditorGUILayout.PropertyField(m_Shadow);
            EditorGUI.indentLevel--;
        }
    }
}