//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine.UI;

namespace MaterialUI
{
    public class MaterialToggleBaseEditor : MaterialBaseEditor
    {
        protected ToggleBase m_Toggle;

        protected SerializedProperty m_Interactable;

        protected SerializedProperty m_Graphic;
        protected SerializedProperty m_iconData;
        protected SerializedProperty m_LabelText;
        protected SerializedProperty m_GraphicChangesWithToggleState;
        protected SerializedProperty m_ToggleOnLabel;
        protected SerializedProperty m_ToggleOffLabel;
        protected SerializedProperty m_ToggleOnIcon;
        protected SerializedProperty m_ToggleOffIcon;

        protected SerializedProperty m_AnimationDuration;

        protected SerializedProperty m_ChangeGraphicColor;
        protected SerializedProperty m_GraphicOnColor;
        protected SerializedProperty m_GraphicOffColor;
        protected SerializedProperty m_GraphicDisabledColor;
        protected SerializedProperty m_ChangeRippleColor;
        protected SerializedProperty m_RippleOnColor;
        protected SerializedProperty m_RippleOffColor;

		protected bool m_IsControllingChildren = false;

        protected virtual void OnEnable()
        {
            OnBaseEnable();

            m_Toggle = (ToggleBase)serializedObject.targetObject;

			m_Interactable = serializedObject.FindProperty("m_Interactable");

            m_Graphic = serializedObject.FindProperty("m_Graphic");
            m_iconData = serializedObject.FindProperty("m_Icon");
            m_LabelText = serializedObject.FindProperty("m_Label");
            m_GraphicChangesWithToggleState = serializedObject.FindProperty("m_ToggleGraphic");
            m_ToggleOnLabel = serializedObject.FindProperty("m_ToggleOnLabel");
            m_ToggleOffLabel = serializedObject.FindProperty("m_ToggleOffLabel");
            m_ToggleOnIcon = serializedObject.FindProperty("m_ToggleOnIcon");
            m_ToggleOffIcon = serializedObject.FindProperty("m_ToggleOffIcon");

            m_AnimationDuration = serializedObject.FindProperty("m_AnimationDuration");

            m_ChangeGraphicColor = serializedObject.FindProperty("m_ChangeGraphicColor");
            m_GraphicOnColor = serializedObject.FindProperty("m_GraphicOnColor");
            m_GraphicOffColor = serializedObject.FindProperty("m_GraphicOffColor");
            m_GraphicDisabledColor = serializedObject.FindProperty("m_GraphicDisabledColor");
            m_ChangeRippleColor = serializedObject.FindProperty("m_ChangeRippleColor");
            m_RippleOnColor = serializedObject.FindProperty("m_RippleOnColor");
            m_RippleOffColor = serializedObject.FindProperty("m_RippleOffColor");
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
                m_Toggle.interactable = m_Interactable.boolValue;
            }

            if (m_Graphic.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(m_GraphicChangesWithToggleState);

                if (m_Graphic.objectReferenceValue.GetType() == typeof(Image) || m_Graphic.objectReferenceValue.GetType() == typeof(VectorImage))
                {
                    if (m_GraphicChangesWithToggleState.boolValue)
                    {
                        EditorGUILayout.PropertyField(m_ToggleOnIcon);
                        EditorGUILayout.PropertyField(m_ToggleOffIcon);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(m_iconData);
                    }
                }
                else
                {
                    if (m_GraphicChangesWithToggleState.boolValue)
                    {
                        EditorGUILayout.PropertyField(m_ToggleOnLabel);
                        EditorGUILayout.PropertyField(m_ToggleOffLabel);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(m_LabelText);
                    }
                }
            }

			EditorGUI.BeginDisabledGroup(m_IsControllingChildren);
			{
				EditorGUILayout.PropertyField(m_AnimationDuration);

				EditorGUI.BeginChangeCheck();
				{
					DrawFoldoutColors(ColorsSection);
				}
				if (EditorGUI.EndChangeCheck())
				{
					m_Toggle.EditorValidate();
				}
			}
			EditorGUI.EndDisabledGroup(); 

            EditorGUI.BeginChangeCheck();
            {
                DrawFoldoutComponents(ComponentsSection);
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_Toggle.EditorValidate();
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void ColorsSection()
        {
            EditorGUI.indentLevel++;
            if (m_Graphic.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(m_ChangeGraphicColor);

                if (m_ChangeGraphicColor.boolValue)
                {
                    EditorGUILayout.PropertyField(m_GraphicOnColor);
                    EditorGUILayout.PropertyField(m_GraphicOffColor);
                    EditorGUILayout.PropertyField(m_GraphicDisabledColor);
                }
            }

            if (m_Toggle.GetComponent<MaterialRipple>())
            {
                EditorGUILayout.PropertyField(m_ChangeRippleColor);
                if (m_ChangeRippleColor.boolValue)
                {
                    EditorGUILayout.PropertyField(m_RippleOnColor);
                    EditorGUILayout.PropertyField(m_RippleOffColor);
                }
            }
            EditorGUI.indentLevel--;
        }

        protected virtual void ComponentsSection()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_Graphic);
            EditorGUI.indentLevel--;
        }
    }
}