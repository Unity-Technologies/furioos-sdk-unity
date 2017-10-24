//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [CustomEditor(typeof(MaterialButton), true)]
    [CanEditMultipleObjects]
    public class MaterialButtonEditor : MaterialBaseEditor
    {
        private MaterialButton m_SelectedMaterialButton;
        private TargetArray<MaterialButton> m_SelectedMaterialButtons;

        private SerializedProperty m_Interactable;

        private SerializedProperty m_ShadowsCanvasGroup;
        private SerializedProperty m_ContentRectTransform;
        private SerializedProperty m_BackgroundImage;
        private SerializedProperty m_Text;
        private SerializedProperty m_Icon;

        private SerializedProperty m_ContentPaddingX;
        private SerializedProperty m_ContentPaddingY;

        private SerializedProperty m_FitWidthToContent;
        private SerializedProperty m_FitHeightToContent;

        void OnEnable()
        {
            OnBaseEnable();

            m_SelectedMaterialButton = (MaterialButton)target;
            m_SelectedMaterialButtons = new TargetArray<MaterialButton>(targets);

            m_Interactable = serializedObject.FindProperty("m_Interactable");

            m_ShadowsCanvasGroup = serializedObject.FindProperty("m_ShadowsCanvasGroup");
            m_ContentRectTransform = serializedObject.FindProperty("m_ContentRectTransform");

            m_BackgroundImage = serializedObject.FindProperty("m_BackgroundImage");
            m_Text = serializedObject.FindProperty("m_Text");
            m_Icon = serializedObject.FindProperty("m_Icon");

            m_ContentPaddingX = serializedObject.FindProperty("m_ContentPadding.x");
            m_ContentPaddingY = serializedObject.FindProperty("m_ContentPadding.y");

            m_FitWidthToContent = serializedObject.FindProperty("m_FitWidthToContent");
            m_FitHeightToContent = serializedObject.FindProperty("m_FitHeightToContent");
        }

        void OnDisable()
        {
            OnBaseDisable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Interactable);
            if (EditorGUI.EndChangeCheck())
            {
                m_SelectedMaterialButtons.ExecuteAction(button => button.interactable = m_Interactable.boolValue);
            }

            using (new GUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_FitWidthToContent);
                if (EditorGUI.EndChangeCheck())
                {
                    m_SelectedMaterialButtons.ExecuteAction(button => button.ClearTracker());
                }
                if (m_FitWidthToContent.boolValue)
                {
                    EditorGUILayout.LabelField("Padding", GUILayout.Width(52));
                    EditorGUILayout.PropertyField(m_ContentPaddingX, new GUIContent());
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_FitHeightToContent);
                if (EditorGUI.EndChangeCheck())
                {
                    m_SelectedMaterialButtons.ExecuteAction(button => button.ClearTracker());
                }
                if (m_FitHeightToContent.boolValue)
                {
                    EditorGUILayout.LabelField("Padding", GUILayout.Width(52));
                    EditorGUILayout.PropertyField(m_ContentPaddingY, new GUIContent());
                }
            }

            ConvertButtonSection();

            DrawFoldoutExternalProperties(ExternalPropertiesSection);

            DrawFoldoutComponents(ComponentsSection);

            serializedObject.ApplyModifiedProperties();
        }

        private bool ExternalPropertiesSection()
        {
            return InspectorFields.MaterialButtonMultiField(go => go.GetComponent<MaterialButton>(), m_SelectedMaterialButton);
        }

        private void ComponentsSection()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_ContentRectTransform);
            EditorGUILayout.PropertyField(m_BackgroundImage);
            EditorGUILayout.PropertyField(m_ShadowsCanvasGroup);
            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_Icon);
            EditorGUI.indentLevel--;
        }

        private void ConvertButtonSection()
        {
            GUIContent convertText = new GUIContent();

            if (m_ShadowsCanvasGroup.objectReferenceValue != null)
            {
                convertText.text = "Convert to flat button";
            }
            else
            {
                convertText.text = "Convert to raised button";
            }

            if (Selection.objects.Length > 1)
            {
                GUI.enabled = false;
                convertText.text = "Convert button";
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(EditorGUIUtility.labelWidth);
                if (GUILayout.Button(convertText, EditorStyles.miniButton))
                {
                    m_SelectedMaterialButton.Convert();
                }
            }

            GUI.enabled = true;
        }
    }
}