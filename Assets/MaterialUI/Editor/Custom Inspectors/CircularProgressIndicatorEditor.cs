//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [CustomEditor(typeof(CircularProgressIndicator))]
    public class CircularProgressIndicatorEditor : MaterialBaseEditor
    {
        private SerializedProperty m_CurrentProgress;
        private SerializedProperty m_BaseObjectOverride;
        private SerializedProperty m_CircleRectTransform;
        private SerializedProperty m_StartsIndeterminate;
        private SerializedProperty m_StartsHidden;

        void OnEnable()
        {
            OnBaseEnable();

            m_CurrentProgress = serializedObject.FindProperty("m_CurrentProgress");
            m_BaseObjectOverride = serializedObject.FindProperty("m_BaseObjectOverride");
            m_CircleRectTransform = serializedObject.FindProperty("m_CircleRectTransform");
            m_StartsIndeterminate = serializedObject.FindProperty("m_StartsIndeterminate");
            m_StartsHidden = serializedObject.FindProperty("m_StartsHidden");
        }

        void OnDisable()
        {
            OnBaseDisable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_CurrentProgress);
            EditorGUILayout.PropertyField(m_StartsIndeterminate);
            EditorGUILayout.PropertyField(m_StartsHidden);

            DrawFoldoutExternalProperties(ExternalPropertiesSection);
            DrawFoldoutComponents(ComponentSection);

            serializedObject.ApplyModifiedProperties();
        }

        private bool ExternalPropertiesSection()
        {
            RectTransform circleRectTransformValue = m_CircleRectTransform.objectReferenceValue as RectTransform;

            if (circleRectTransformValue == null) return false;
            if (circleRectTransformValue.childCount == 0) return false;

            return InspectorFields.GraphicColorField("Circle", circleRectTransformValue.GetChild(0).GetComponent<Graphic>());
        }

        private void ComponentSection()
        {
            EditorGUILayout.PropertyField(m_CircleRectTransform);
            EditorGUILayout.PropertyField(m_BaseObjectOverride);
        }
    }
}