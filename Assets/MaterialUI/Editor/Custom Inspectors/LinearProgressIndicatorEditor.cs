//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [CustomEditor(typeof(LinearProgressIndicator))]
    public class LinearProgressIndicatorEditor : MaterialBaseEditor
    {
        private SerializedProperty m_CurrentProgress;
        private SerializedProperty m_BaseObjectOverride;
        private SerializedProperty m_BarRectTransform;
        private SerializedProperty m_BackgroundImage;
        private SerializedProperty m_StartsIndeterminate;
        private SerializedProperty m_StartsHidden;

        void OnEnable()
        {
            OnBaseEnable();

            m_CurrentProgress = serializedObject.FindProperty("m_CurrentProgress");
            m_BaseObjectOverride = serializedObject.FindProperty("m_BaseObjectOverride");
            m_BarRectTransform = serializedObject.FindProperty("m_BarRectTransform");
            m_BackgroundImage = serializedObject.FindProperty("m_BackgroundImage");
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
            bool result = false;

            RectTransform barRectTransform = m_BarRectTransform.objectReferenceValue as RectTransform;
            if (barRectTransform != null)
            {
                Utils.SetBoolValueIfTrue(ref result, InspectorFields.GraphicColorField("Bar", barRectTransform.GetComponent<Graphic>()));
            }

            Image backgroundImage = m_BackgroundImage.objectReferenceValue as Image;
            if (backgroundImage != null)
            {
                Utils.SetBoolValueIfTrue(ref result, InspectorFields.GraphicColorField("Background", backgroundImage));
            }

            return result;
        }

        private void ComponentSection()
        {
            EditorGUILayout.PropertyField(m_BarRectTransform);
            EditorGUILayout.PropertyField(m_BackgroundImage);
            EditorGUILayout.PropertyField(m_BaseObjectOverride);
        }
    }
}