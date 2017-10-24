//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;

namespace MaterialUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DragEventSender))]
    public class DragEventSenderEditor : Editor
    {
        private SerializedProperty m_CombineLeftAndRight;
        private SerializedProperty m_HorizontalTargetObject;
        private SerializedProperty m_LeftTargetObject;
        private SerializedProperty m_RightTargetObject;
        private SerializedProperty m_CombineUpAndDown;
        private SerializedProperty m_VerticalTargetObject;
        private SerializedProperty m_UpTargetObject;
        private SerializedProperty m_DownTargetObject;
        private SerializedProperty m_AnyDirectionTargetObject;

        void OnEnable()
        {
            m_CombineLeftAndRight = serializedObject.FindProperty("m_CombineLeftAndRight");
            m_HorizontalTargetObject = serializedObject.FindProperty("m_HorizontalTargetObject");
            m_LeftTargetObject = serializedObject.FindProperty("m_LeftTargetObject");
            m_RightTargetObject = serializedObject.FindProperty("m_RightTargetObject");
            m_CombineUpAndDown = serializedObject.FindProperty("m_CombineUpAndDown");
            m_VerticalTargetObject = serializedObject.FindProperty("m_VerticalTargetObject");
            m_UpTargetObject = serializedObject.FindProperty("m_UpTargetObject");
            m_DownTargetObject = serializedObject.FindProperty("m_DownTargetObject");
            m_AnyDirectionTargetObject = serializedObject.FindProperty("m_AnyDirectionTargetObject");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_CombineLeftAndRight);

            if (m_CombineLeftAndRight.boolValue)
            {
                EditorGUILayout.PropertyField(m_HorizontalTargetObject);
            }
            else
            {
                EditorGUILayout.PropertyField(m_LeftTargetObject);
                EditorGUILayout.PropertyField(m_RightTargetObject);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_CombineUpAndDown);

            if (m_CombineUpAndDown.boolValue)
            {
                EditorGUILayout.PropertyField(m_VerticalTargetObject);
            }
            else
            {
                EditorGUILayout.PropertyField(m_UpTargetObject);
                EditorGUILayout.PropertyField(m_DownTargetObject);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_AnyDirectionTargetObject);

            serializedObject.ApplyModifiedProperties();
        }
    }
}