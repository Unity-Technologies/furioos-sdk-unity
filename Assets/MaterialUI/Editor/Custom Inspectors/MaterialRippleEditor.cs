//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MaterialRipple))]
    class MaterialRippleEditor : MaterialBaseEditor
    {
        private SerializedProperty m_RippleData;
        private SerializedProperty m_RipplesEnabled;
        private SerializedProperty m_AutoSize;
        private SerializedProperty m_SizePercent;
        private SerializedProperty m_ManualSize;
        private SerializedProperty m_SizeMode;
        private SerializedProperty m_Speed;
        private SerializedProperty m_Color;
        private SerializedProperty m_StartAlpha;
        private SerializedProperty m_EndAlpha;
        private SerializedProperty m_MoveTowardCenter;
        private SerializedProperty m_RippleParent;
        private SerializedProperty m_PlaceRippleBehind;

        private SerializedProperty m_ToggleMask;
        private SerializedProperty m_HighlightWhen;
        private SerializedProperty m_HighlightGraphic;
        private SerializedProperty m_AutoHighlightColor;
        private SerializedProperty m_HighlightColor;
        private SerializedProperty m_CheckForScroll;
        private SerializedProperty m_AutoHighlightBlendAmount;

        private SerializedProperty m_AutoMatchGraphicColor;
        private SerializedProperty m_ColorMatchGraphic;

        void OnEnable()
        {
            OnBaseEnable();

            m_RippleData = serializedObject.FindProperty("m_RippleData");
            m_RipplesEnabled = serializedObject.FindProperty("m_RipplesEnabled");

            m_AutoSize = m_RippleData.FindPropertyRelative("AutoSize");
            m_SizePercent = m_RippleData.FindPropertyRelative("SizePercent");
            m_ManualSize = m_RippleData.FindPropertyRelative("ManualSize");
            m_SizeMode = m_RippleData.FindPropertyRelative("SizeMode");
            m_Speed = m_RippleData.FindPropertyRelative("Speed");
            m_Color = m_RippleData.FindPropertyRelative("Color");
            m_StartAlpha = m_RippleData.FindPropertyRelative("StartAlpha");
            m_EndAlpha = m_RippleData.FindPropertyRelative("EndAlpha");
            m_MoveTowardCenter = m_RippleData.FindPropertyRelative("MoveTowardCenter");
            m_RippleParent = m_RippleData.FindPropertyRelative("RippleParent");
            m_PlaceRippleBehind = m_RippleData.FindPropertyRelative("PlaceRippleBehind");

            m_ToggleMask = serializedObject.FindProperty("m_ToggleMask");
            m_HighlightWhen = serializedObject.FindProperty("m_HighlightWhen");
            m_HighlightGraphic = serializedObject.FindProperty("m_HighlightGraphic");
            m_AutoHighlightColor = serializedObject.FindProperty("m_AutoHighlightColor");
            m_HighlightColor = serializedObject.FindProperty("m_HighlightColor");
            m_CheckForScroll = serializedObject.FindProperty("m_CheckForScroll");
            m_AutoHighlightBlendAmount = serializedObject.FindProperty("m_AutoHighlightBlendAmount");

            m_AutoMatchGraphicColor = serializedObject.FindProperty("m_AutoMatchGraphicColor");
            m_ColorMatchGraphic = serializedObject.FindProperty("m_ColorMatchGraphic");
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
                EditorGUILayout.PropertyField(m_RipplesEnabled);

                EditorGUILayout.Space();

                if (m_RipplesEnabled.boolValue)
                {
                    EditorGUILayout.LabelField("Ripple Color", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(m_AutoMatchGraphicColor);
                    EditorGUI.indentLevel++;
                    if (m_AutoMatchGraphicColor.boolValue)
                    {
                        EditorGUILayout.PropertyField(m_ColorMatchGraphic);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(m_Color);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.PropertyField(m_StartAlpha);
                    EditorGUILayout.PropertyField(m_EndAlpha);

                    EditorGUILayout.Space();
                }

                EditorGUILayout.LabelField("Highlight Color", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_HighlightWhen, true);
                if (m_HighlightWhen.intValue > 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_HighlightGraphic, true);
                    EditorGUILayout.PropertyField(m_AutoHighlightColor, true);
                    if (m_AutoHighlightColor.boolValue)
                    {
                        EditorGUILayout.PropertyField(m_AutoHighlightBlendAmount, new GUIContent("Blend Amount"), true);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(m_HighlightColor, true);
                    }
                    EditorGUI.indentLevel--;

                    EditorGUILayout.Space();
                }

                if (m_RipplesEnabled.boolValue)
                {
                    EditorGUILayout.LabelField("Speed and Size", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(m_Speed);
                    EditorGUILayout.PropertyField(m_AutoSize);
                    EditorGUI.indentLevel++;
                    if (m_AutoSize.boolValue)
                    {
                        EditorGUILayout.PropertyField(m_SizePercent);
                        EditorGUILayout.PropertyField(m_SizeMode);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(m_ManualSize);
                    }
                    EditorGUI.indentLevel--;

                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Other Settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(m_RippleParent);
                    EditorGUILayout.PropertyField(m_MoveTowardCenter);
                    EditorGUILayout.PropertyField(m_PlaceRippleBehind);
                    EditorGUILayout.PropertyField(m_ToggleMask, true);
                    EditorGUILayout.PropertyField(m_CheckForScroll, true);
                }

                serializedObject.ApplyModifiedProperties();
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_AutoHighlightBlendAmount.floatValue = Mathf.Clamp(m_AutoHighlightBlendAmount.floatValue, 0f, 1f);
                ((MaterialRipple)serializedObject.targetObject).RefreshSettings();
            }
        }
    }
}