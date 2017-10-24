//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;

namespace MaterialUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MaterialSwitch))]
    class MaterialSwitchEditor : MaterialToggleBaseEditor
    {
        private SerializedProperty m_SwitchImage;
        private SerializedProperty m_BackImage;
        private SerializedProperty m_ShadowImage;

        private SerializedProperty m_OnColor;
        private SerializedProperty m_OffColor;
        private SerializedProperty m_DisabledColor;

        private SerializedProperty m_BackOnColor;
        private SerializedProperty m_BackOffColor;
        private SerializedProperty m_BackDisabledColor;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_SwitchImage = serializedObject.FindProperty("m_SwitchImage");
            m_BackImage = serializedObject.FindProperty("m_BackImage");
            m_ShadowImage = serializedObject.FindProperty("m_ShadowImage");

            m_OnColor = serializedObject.FindProperty("m_OnColor");
            m_OffColor = serializedObject.FindProperty("m_OffColor");
            m_DisabledColor = serializedObject.FindProperty("m_DisabledColor");

            m_BackOnColor = serializedObject.FindProperty("m_BackOnColor");
            m_BackOffColor = serializedObject.FindProperty("m_BackOffColor");
            m_BackDisabledColor = serializedObject.FindProperty("m_BackDisabledColor");
        }

        protected override void ColorsSection()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_OnColor);
            EditorGUILayout.PropertyField(m_OffColor);
            EditorGUILayout.PropertyField(m_DisabledColor);
            EditorGUILayout.PropertyField(m_BackOnColor);
            EditorGUILayout.PropertyField(m_BackOffColor);
            EditorGUILayout.PropertyField(m_BackDisabledColor);
            EditorGUI.indentLevel--;

            base.ColorsSection();
        }

        protected override void ComponentsSection()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_SwitchImage);
            EditorGUILayout.PropertyField(m_BackImage);
            EditorGUILayout.PropertyField(m_ShadowImage);
            EditorGUI.indentLevel--;

            base.ComponentsSection();
        }
    }
}