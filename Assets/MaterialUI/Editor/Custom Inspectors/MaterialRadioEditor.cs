//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;

namespace MaterialUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MaterialRadio))]
	class MaterialRadioEditor : MaterialToggleBaseEditor
    {
        private SerializedProperty m_RingImage;
        private SerializedProperty m_DotImage;

        private SerializedProperty m_OnColor;
        private SerializedProperty m_OffColor;
        private SerializedProperty m_DisabledColor;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            m_RingImage = serializedObject.FindProperty("m_RingImage");
            m_DotImage = serializedObject.FindProperty("m_DotImage");

            m_OnColor = serializedObject.FindProperty("m_OnColor");
            m_OffColor = serializedObject.FindProperty("m_OffColor");
            m_DisabledColor = serializedObject.FindProperty("m_DisabledColor");
        }

		public override void OnInspectorGUI()
		{
			m_IsControllingChildren = m_Toggle.GetComponentInParent<MaterialRadioGroup>() && m_Toggle.GetComponentInParent<MaterialRadioGroup>().isControllingChildren;
			if (m_IsControllingChildren)
			{
				EditorGUILayout.HelpBox("Some options are controlled by parent MaterialRadioGroup", MessageType.Warning);
			}

			base.OnInspectorGUI();
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
            EditorGUILayout.PropertyField(m_RingImage);
            EditorGUILayout.PropertyField(m_DotImage);
            EditorGUI.indentLevel--;

            base.ComponentsSection();
        }
    }
}