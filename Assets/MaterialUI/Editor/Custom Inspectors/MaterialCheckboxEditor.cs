//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;

namespace MaterialUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MaterialCheckbox))]
    class MaterialCheckboxEditor : MaterialToggleBaseEditor
    {
        private SerializedProperty m_CheckImage;
        private SerializedProperty m_FrameImage;

        private SerializedProperty m_OnColor;
        private SerializedProperty m_OffColor;
        private SerializedProperty m_DisabledColor;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_CheckImage = serializedObject.FindProperty("m_CheckImage");
            m_FrameImage = serializedObject.FindProperty("m_FrameImage");

            m_OnColor = serializedObject.FindProperty("m_OnColor");
            m_OffColor = serializedObject.FindProperty("m_OffColor");
            m_DisabledColor = serializedObject.FindProperty("m_DisabledColor");
        }

        protected override void ColorsSection()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_OnColor);
            EditorGUILayout.PropertyField(m_OffColor);
            EditorGUILayout.PropertyField(m_DisabledColor);
            EditorGUI.indentLevel--;

            base.ColorsSection();
        }

        protected override void ComponentsSection()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_CheckImage);
            EditorGUILayout.PropertyField(m_FrameImage);
            EditorGUI.indentLevel--;

            base.ComponentsSection();
        }
    }
}