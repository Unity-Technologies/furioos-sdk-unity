//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;

namespace MaterialUI
{
    [CustomEditor(typeof(MaterialDropdown))]
	public class MaterialDropdownEditor : MaterialBaseEditor
    {
        private SerializedProperty m_VerticalPivotType;
        private SerializedProperty m_HorizontalPivotType;
        private SerializedProperty m_ExpandStartType;
        private SerializedProperty m_IgnoreInputAfterShowTimer;
        private SerializedProperty m_MaxHeight;
        private SerializedProperty m_CapitalizeButtonText;
		private SerializedProperty m_CurrentlySelected;
		private SerializedProperty m_HighlightCurrentlySelected;
		private SerializedProperty m_UpdateHeader;
        private SerializedProperty m_AnimationDuration;
        private SerializedProperty m_MinDistanceFromEdge;
        private SerializedProperty m_PanelColor;
        private SerializedProperty m_ItemTextColor;
        private SerializedProperty m_ItemIconColor;
        private SerializedProperty m_ItemRippleData;
        private SerializedProperty m_BaseTransform;
        private SerializedProperty m_BaseSelectable;
        private SerializedProperty m_ButtonTextContent;
        private SerializedProperty m_ButtonImageContent;
        private SerializedProperty m_OnItemSelected;

		private SerializedProperty m_OptionDataList;

        void OnEnable()
        {
			OnBaseEnable();

            m_VerticalPivotType = serializedObject.FindProperty("m_VerticalPivotType");
            m_HorizontalPivotType = serializedObject.FindProperty("m_HorizontalPivotType");
            m_ExpandStartType = serializedObject.FindProperty("m_ExpandStartType");
            m_IgnoreInputAfterShowTimer = serializedObject.FindProperty("m_IgnoreInputAfterShowTimer");
            m_MaxHeight = serializedObject.FindProperty("m_MaxHeight");
            m_CapitalizeButtonText = serializedObject.FindProperty("m_CapitalizeButtonText");
			m_CurrentlySelected = serializedObject.FindProperty("m_CurrentlySelected");
			m_HighlightCurrentlySelected = serializedObject.FindProperty("m_HighlightCurrentlySelected");
			m_UpdateHeader = serializedObject.FindProperty("m_UpdateHeader");
            m_AnimationDuration = serializedObject.FindProperty("m_AnimationDuration");
            m_MinDistanceFromEdge = serializedObject.FindProperty("m_MinDistanceFromEdge");
            m_PanelColor = serializedObject.FindProperty("m_PanelColor");
            m_ItemTextColor = serializedObject.FindProperty("m_ItemTextColor");
            m_ItemIconColor = serializedObject.FindProperty("m_ItemIconColor");
            m_ItemRippleData = serializedObject.FindProperty("m_ItemRippleData");
            m_BaseTransform = serializedObject.FindProperty("m_BaseTransform");
            m_BaseSelectable = serializedObject.FindProperty("m_BaseSelectable");
            m_ButtonTextContent = serializedObject.FindProperty("m_ButtonTextContent");
            m_ButtonImageContent = serializedObject.FindProperty("m_ButtonImageContent");
            m_OnItemSelected = serializedObject.FindProperty("m_OnItemSelected");

			m_OptionDataList = serializedObject.FindProperty("m_OptionDataList");
		}

		void OnDisable()
		{
			OnBaseDisable();
		}

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_VerticalPivotType);
            EditorGUILayout.PropertyField(m_HorizontalPivotType);
            EditorGUILayout.PropertyField(m_ExpandStartType);
            EditorGUILayout.PropertyField(m_IgnoreInputAfterShowTimer);
            EditorGUILayout.PropertyField(m_MaxHeight);
            EditorGUILayout.PropertyField(m_CapitalizeButtonText);
            EditorGUILayout.IntSlider(m_CurrentlySelected, -1, m_OptionDataList.FindPropertyRelative("m_Options").arraySize - 1);
            EditorGUILayout.PropertyField(m_HighlightCurrentlySelected);
			EditorGUILayout.PropertyField(m_UpdateHeader);
            EditorGUILayout.PropertyField(m_AnimationDuration);
            EditorGUILayout.PropertyField(m_MinDistanceFromEdge);
            EditorGUILayout.PropertyField(m_ItemRippleData, true);

			DrawFoldoutColors(ColorsSection);
			DrawFoldoutComponents(ComponentsSection);

			EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_OptionDataList);
            EditorGUILayout.PropertyField(m_OnItemSelected);

            serializedObject.ApplyModifiedProperties();
        }

		private void ColorsSection()
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(m_PanelColor);
			EditorGUILayout.PropertyField(m_ItemTextColor);
			EditorGUILayout.PropertyField(m_ItemIconColor);
			EditorGUI.indentLevel--;
		}

		private void ComponentsSection()
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(m_BaseTransform);
			EditorGUILayout.PropertyField(m_BaseSelectable);
			EditorGUILayout.PropertyField(m_ButtonTextContent);
			EditorGUILayout.PropertyField(m_ButtonImageContent);
			EditorGUI.indentLevel--;
		}
    }
}