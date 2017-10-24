//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    [CustomEditor(typeof(MaterialInputField), true)]
    [CanEditMultipleObjects]
    public class MaterialInputFieldEditor : MaterialBaseEditor
    {
        //  Fields
		private SerializedProperty m_Interactable;

        private SerializedProperty m_HintText;
        private SerializedProperty m_FloatingHint;
        private SerializedProperty m_FloatingHintFontSize;

        private SerializedProperty m_HasCharacterCounter;
        private SerializedProperty m_MatchInputFieldCharacterLimit;
        private SerializedProperty m_CharacterLimit;

        private SerializedProperty m_HasValidation;
        private SerializedProperty m_TextValidator;
        private SerializedProperty m_ValidateOnStart;

        private SerializedProperty m_ManualPreferredWidth;
        private SerializedProperty m_FitHeightToContent;
        private SerializedProperty m_ManualPreferredHeight;
        private SerializedProperty m_ManualSizeX;
        private SerializedProperty m_ManualSizeY;

        private SerializedProperty m_LeftContentOffset;
        private SerializedProperty m_RightContentOffset;

        private SerializedProperty m_AnimationDuration;

        private SerializedProperty m_HintTextTransform;
        private SerializedProperty m_CounterTextTransform;
        private SerializedProperty m_ValidationTextTransform;
        private SerializedProperty m_LineTransform;
        private SerializedProperty m_ActiveLineTransform;
        private SerializedProperty m_LeftContentTransform;
        private SerializedProperty m_RightContentTransform;

        private SerializedProperty m_LeftContentActiveColor;
        private SerializedProperty m_LeftContentInactiveColor;
        private SerializedProperty m_RightContentActiveColor;
        private SerializedProperty m_RightContentInactiveColor;
        private SerializedProperty m_HintTextActiveColor;
        private SerializedProperty m_HintTextInactiveColor;
        private SerializedProperty m_LineActiveColor;
        private SerializedProperty m_LineInactiveColor;
        private SerializedProperty m_ValidationActiveColor;
        private SerializedProperty m_ValidationInactiveColor;
        private SerializedProperty m_CounterActiveColor;
        private SerializedProperty m_CounterInactiveColor;

        private SerializedProperty m_LeftContentGraphic;
        private SerializedProperty m_RightContentGraphic;

        void OnEnable()
        {
            OnBaseEnable();

            //  Fields
			m_Interactable = serializedObject.FindProperty("m_Interactable");

            m_HintText = serializedObject.FindProperty("m_HintText");
            m_FloatingHint = serializedObject.FindProperty("m_FloatingHint");
            m_FloatingHintFontSize = serializedObject.FindProperty("m_FloatingHintFontSize");

            m_HasCharacterCounter = serializedObject.FindProperty("m_HasCharacterCounter");
            m_MatchInputFieldCharacterLimit = serializedObject.FindProperty("m_MatchInputFieldCharacterLimit");
            m_CharacterLimit = serializedObject.FindProperty("m_CharacterLimit");

            m_HasValidation = serializedObject.FindProperty("m_HasValidation");
            m_TextValidator = serializedObject.FindProperty("m_TextValidator");
            m_ValidateOnStart = serializedObject.FindProperty("m_ValidateOnStart");

            m_ManualPreferredWidth = serializedObject.FindProperty("m_ManualPreferredWidth");
            m_FitHeightToContent = serializedObject.FindProperty("m_FitHeightToContent");
            m_ManualPreferredHeight = serializedObject.FindProperty("m_ManualPreferredHeight");
            m_ManualSizeX = serializedObject.FindProperty("m_ManualSize.x");
            m_ManualSizeY = serializedObject.FindProperty("m_ManualSize.y");

            m_LeftContentOffset = serializedObject.FindProperty("m_LeftContentOffset");
            m_RightContentOffset = serializedObject.FindProperty("m_RightContentOffset");

            m_AnimationDuration = serializedObject.FindProperty("m_AnimationDuration");

            m_HintTextTransform = serializedObject.FindProperty("m_HintTextTransform");
            m_CounterTextTransform = serializedObject.FindProperty("m_CounterTextTransform");
            m_ValidationTextTransform = serializedObject.FindProperty("m_ValidationTextTransform");
            m_LineTransform = serializedObject.FindProperty("m_LineTransform");
            m_ActiveLineTransform = serializedObject.FindProperty("m_ActiveLineTransform");
            m_LeftContentTransform = serializedObject.FindProperty("m_LeftContentTransform");
            m_RightContentTransform = serializedObject.FindProperty("m_RightContentTransform");

            m_LeftContentActiveColor = serializedObject.FindProperty("m_LeftContentActiveColor");
            m_LeftContentInactiveColor = serializedObject.FindProperty("m_LeftContentInactiveColor");
            m_RightContentActiveColor = serializedObject.FindProperty("m_RightContentActiveColor");
            m_RightContentInactiveColor = serializedObject.FindProperty("m_RightContentInactiveColor");
            m_HintTextActiveColor = serializedObject.FindProperty("m_HintTextActiveColor");
            m_HintTextInactiveColor = serializedObject.FindProperty("m_HintTextInactiveColor");
            m_LineActiveColor = serializedObject.FindProperty("m_LineActiveColor");
            m_LineInactiveColor = serializedObject.FindProperty("m_LineInactiveColor");
            m_ValidationActiveColor = serializedObject.FindProperty("m_ValidationActiveColor");
            m_ValidationInactiveColor = serializedObject.FindProperty("m_ValidationInactiveColor");
            m_CounterActiveColor = serializedObject.FindProperty("m_CounterActiveColor");
            m_CounterInactiveColor = serializedObject.FindProperty("m_CounterInactiveColor");

            m_LeftContentGraphic = serializedObject.FindProperty("m_LeftContentGraphic");
            m_RightContentGraphic = serializedObject.FindProperty("m_RightContentGraphic");
        }

        void OnDisable()
        {
            OnBaseDisable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(m_Interactable);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ((MaterialInputField)m_Interactable.serializedObject.targetObject).interactable = m_Interactable.boolValue;
            }

            if (m_HintTextTransform.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(m_HintText);
            }

			using (new GUILayout.HorizontalScope())
			{
				EditorGUILayout.PropertyField(m_FloatingHint);
				if (m_FloatingHint.boolValue)
				{
					EditorGUILayout.LabelField("Font Size", GUILayout.Width(GUI.skin.label.CalcSize(new GUIContent("Font Size")).x));
					EditorGUILayout.PropertyField(m_FloatingHintFontSize, new GUIContent(""));
				}
			}

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_HasCharacterCounter);
            if (m_HasCharacterCounter.boolValue)
            {
                EditorGUI.indentLevel++;
				using (new GUILayout.HorizontalScope())
				{
					EditorGUILayout.PropertyField(m_MatchInputFieldCharacterLimit);
					if (!m_MatchInputFieldCharacterLimit.boolValue)
					{
						EditorGUI.indentLevel--;
						EditorGUILayout.LabelField("Limit", GUILayout.Width(GUI.skin.label.CalcSize(new GUIContent("Limit")).x + 10));
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField(m_CharacterLimit, new GUIContent(""));
					}
				}
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_HasValidation);
            if (m_HasValidation.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_TextValidator);
                EditorGUILayout.PropertyField(m_ValidateOnStart);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

			using (new GUILayout.HorizontalScope())
			{
				EditorGUILayout.PropertyField(m_ManualPreferredWidth);
				if (m_ManualPreferredWidth.boolValue)
				{
					EditorGUILayout.PropertyField(m_ManualSizeX, new GUIContent(""));
				}
			}

            EditorGUILayout.PropertyField(m_FitHeightToContent);
            if (!m_FitHeightToContent.boolValue)
            {
                EditorGUI.indentLevel++;
				using (new GUILayout.HorizontalScope())
				{
					EditorGUILayout.PropertyField(m_ManualPreferredHeight);
					if (m_ManualPreferredHeight.boolValue)
					{
						EditorGUILayout.PropertyField(m_ManualSizeY, new GUIContent(""));
					}
				}
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            GUI.enabled = m_LeftContentTransform.objectReferenceValue != null;
            EditorGUILayout.PropertyField(m_LeftContentOffset);
            GUI.enabled = true;

            GUI.enabled = m_RightContentTransform.objectReferenceValue != null;
            EditorGUILayout.PropertyField(m_RightContentOffset);
            GUI.enabled = true;

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_AnimationDuration);

            EditorGUILayout.Space();

            DrawFoldoutColors(ColorsSection);
            DrawFoldoutComponents(ComponentsSection);

            serializedObject.ApplyModifiedProperties();
        }

        private void ColorsSection()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_LeftContentActiveColor);
            EditorGUILayout.PropertyField(m_LeftContentInactiveColor);
            EditorGUILayout.PropertyField(m_RightContentActiveColor);
            EditorGUILayout.PropertyField(m_RightContentInactiveColor);
            EditorGUILayout.PropertyField(m_HintTextActiveColor);
            EditorGUILayout.PropertyField(m_HintTextInactiveColor);
            EditorGUILayout.PropertyField(m_LineActiveColor);
            EditorGUILayout.PropertyField(m_LineInactiveColor);
            EditorGUILayout.PropertyField(m_ValidationActiveColor);
            EditorGUILayout.PropertyField(m_ValidationInactiveColor);
            EditorGUILayout.PropertyField(m_CounterActiveColor);
            EditorGUILayout.PropertyField(m_CounterInactiveColor);
            EditorGUI.indentLevel--;
        }

        private void ComponentsSection()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_LineTransform);
            EditorGUILayout.PropertyField(m_ActiveLineTransform);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_HintTextTransform);
            EditorGUILayout.PropertyField(m_CounterTextTransform);
            EditorGUILayout.PropertyField(m_ValidationTextTransform);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_LeftContentTransform);
            EditorGUILayout.PropertyField(m_LeftContentGraphic);
            EditorGUILayout.PropertyField(m_RightContentTransform);
            EditorGUILayout.PropertyField(m_RightContentGraphic);
            EditorGUI.indentLevel--;
        }
    }
}