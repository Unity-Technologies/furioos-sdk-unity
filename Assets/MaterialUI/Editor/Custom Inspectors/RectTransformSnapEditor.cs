//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace MaterialUI
{
    [CustomEditor(typeof(RectTransformSnap))]
    [CanEditMultipleObjects]
    class RectTransformSnapEditor : Editor
    {
        private SerializedProperty m_SourceRectTransform;
        private SerializedProperty m_Padding;
        private SerializedProperty m_Offset;
        private SerializedProperty m_ValuesArePercentage;
        private SerializedProperty m_PaddingPercent;
        private SerializedProperty m_OffsetPercent;
        private SerializedProperty m_SnapEveryFrame;
        private SerializedProperty m_SnapWidth;
        private SerializedProperty m_SnapHeight;
        private SerializedProperty m_SnapPositionX;
        private SerializedProperty m_SnapPositionY;

        void OnEnable()
        {
            m_SourceRectTransform = serializedObject.FindProperty("m_SourceRectTransform");
            m_Padding = serializedObject.FindProperty("m_Padding");
            m_Offset = serializedObject.FindProperty("m_Offset");
            m_ValuesArePercentage = serializedObject.FindProperty("m_ValuesArePercentage");
            m_PaddingPercent = serializedObject.FindProperty("m_PaddingPercent");
            m_OffsetPercent = serializedObject.FindProperty("m_OffsetPercent");
            m_SnapEveryFrame = serializedObject.FindProperty("m_SnapEveryFrame");
            m_SnapWidth = serializedObject.FindProperty("m_SnapWidth");
            m_SnapHeight = serializedObject.FindProperty("m_SnapHeight");
            m_SnapPositionX = serializedObject.FindProperty("m_SnapPositionX");
            m_SnapPositionY = serializedObject.FindProperty("m_SnapPositionY");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_SourceRectTransform);
            EditorGUILayout.PropertyField(m_ValuesArePercentage);

            if (m_ValuesArePercentage.boolValue)
            {
				using (new GUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("Padding Percent");
					EditorGUILayout.LabelField("X", GUILayout.MaxWidth(12));
					EditorGUILayout.PropertyField(m_SnapWidth, GUIContent.none, GUILayout.MaxWidth(14));
					
					GUI.enabled = m_SnapWidth.boolValue;
					EditorGUILayout.PropertyField(m_PaddingPercent.FindPropertyRelative("x"), GUIContent.none);
					GUI.enabled = true;
					
					EditorGUILayout.LabelField("Y", GUILayout.MaxWidth(12));
					EditorGUILayout.PropertyField(m_SnapHeight, GUIContent.none, GUILayout.MaxWidth(14));
					
					GUI.enabled = m_SnapHeight.boolValue;
					EditorGUILayout.PropertyField(m_PaddingPercent.FindPropertyRelative("y"), GUIContent.none);
					GUI.enabled = true;
				}

				using (new GUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("Offset Percent");
					EditorGUILayout.LabelField("X", GUILayout.MaxWidth(12));
					EditorGUILayout.PropertyField(m_SnapPositionX, GUIContent.none, GUILayout.MaxWidth(14));
					
					GUI.enabled = m_SnapPositionX.boolValue;
					EditorGUILayout.PropertyField(m_OffsetPercent.FindPropertyRelative("x"), GUIContent.none);
					GUI.enabled = true;
					
					EditorGUILayout.LabelField("Y", GUILayout.MaxWidth(12));
					EditorGUILayout.PropertyField(m_SnapPositionY, GUIContent.none, GUILayout.MaxWidth(14));
					
					GUI.enabled = m_SnapPositionY.boolValue;
					EditorGUILayout.PropertyField(m_OffsetPercent.FindPropertyRelative("y"), GUIContent.none);
					GUI.enabled = true;
				}
            }
            else
            {
				using (new GUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("Padding");
					EditorGUILayout.LabelField("X", GUILayout.MaxWidth(12));
					EditorGUILayout.PropertyField(m_SnapWidth, GUIContent.none, GUILayout.MaxWidth(14));
					
					GUI.enabled = m_SnapWidth.boolValue;
					EditorGUILayout.PropertyField(m_Padding.FindPropertyRelative("x"), GUIContent.none);
					GUI.enabled = true;
					
					EditorGUILayout.LabelField("Y", GUILayout.MaxWidth(12));
					EditorGUILayout.PropertyField(m_SnapHeight, GUIContent.none, GUILayout.MaxWidth(14));
					
					GUI.enabled = m_SnapHeight.boolValue;
					EditorGUILayout.PropertyField(m_Padding.FindPropertyRelative("y"), GUIContent.none);
					GUI.enabled = true;
				}

				using (new GUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel("Offset");
					EditorGUILayout.LabelField("X", GUILayout.MaxWidth(12));
					EditorGUILayout.PropertyField(m_SnapPositionX, GUIContent.none, GUILayout.MaxWidth(14));
					
					GUI.enabled = m_SnapPositionX.boolValue;
					EditorGUILayout.PropertyField(m_Offset.FindPropertyRelative("x"), GUIContent.none);
					GUI.enabled = true;
					
					EditorGUILayout.LabelField("Y", GUILayout.MaxWidth(12));
					EditorGUILayout.PropertyField(m_SnapPositionY, GUIContent.none, GUILayout.MaxWidth(14));
					
					GUI.enabled = m_SnapPositionY.boolValue;
					EditorGUILayout.PropertyField(m_Offset.FindPropertyRelative("y"), GUIContent.none);
					GUI.enabled = true;
				}
            }

            EditorGUILayout.PropertyField(m_SnapEveryFrame);

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Snap"))
            {
                LayoutRebuilder.MarkLayoutForRebuild(((RectTransformSnap)target).rectTransform);
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
        }
    }
}