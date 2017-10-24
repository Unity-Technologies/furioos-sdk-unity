//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    [CustomPropertyDrawer(typeof(RippleData), true)]
    class RippleDataPropertyDrawer : PropertyDrawer
    {
        private static bool m_DropdownExpanded = true;

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

        private void GetProperties(SerializedProperty property)
        {
            m_AutoSize = property.FindPropertyRelative("AutoSize");
            m_SizePercent = property.FindPropertyRelative("SizePercent");
            m_ManualSize = property.FindPropertyRelative("ManualSize");
            m_SizeMode = property.FindPropertyRelative("SizeMode");
            m_Speed = property.FindPropertyRelative("Speed");
            m_Color = property.FindPropertyRelative("Color");
            m_StartAlpha = property.FindPropertyRelative("StartAlpha");
            m_EndAlpha = property.FindPropertyRelative("EndAlpha");
            m_MoveTowardCenter = property.FindPropertyRelative("MoveTowardCenter");
            m_RippleParent = property.FindPropertyRelative("RippleParent");
            m_PlaceRippleBehind = property.FindPropertyRelative("PlaceRippleBehind");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GetProperties(property);

            position.height = 17;

            m_DropdownExpanded = EditorGUI.Foldout(position, m_DropdownExpanded, label.text);
            position = AddHeight(position, 17);

            if (m_DropdownExpanded)
            {
                EditorGUI.indentLevel++;
                {
                    EditorGUI.PropertyField(position, m_AutoSize);
                    position = AddHeight(position, 17);

                    if (m_AutoSize.boolValue)
                    {
                        EditorGUI.indentLevel++;

                        EditorGUI.PropertyField(position, m_SizePercent);
                        position = AddHeight(position, 17);

                        EditorGUI.PropertyField(position, m_SizeMode);
                        position = AddHeight(position, 17);

                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        EditorGUI.PropertyField(position, m_ManualSize);
                        position = AddHeight(position, 17);
                    }

                    EditorGUI.PropertyField(position, m_Speed);
                    position = AddHeight(position, 17);

                    EditorGUI.PropertyField(position, m_Color);
                    position = AddHeight(position, 17);

                    EditorGUI.PropertyField(position, m_StartAlpha);
                    position = AddHeight(position, 17);

                    EditorGUI.PropertyField(position, m_EndAlpha);
                    position = AddHeight(position, 17);

                    EditorGUI.PropertyField(position, m_MoveTowardCenter);
                    position = AddHeight(position, 17);

                    EditorGUI.PropertyField(position, m_RippleParent);
                    position = AddHeight(position, 17);

                    EditorGUI.PropertyField(position, m_PlaceRippleBehind);
                }
                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;

            if (m_DropdownExpanded)
            {
                GetProperties(property);

                height += m_AutoSize.boolValue ? 34 : 17;

                height += (17 * 8);
            }

            return base.GetPropertyHeight(property, label) + height;
        }

        private Rect AddHeight(Rect position, float height)
        {
            position.position = new Vector2(position.position.x, position.position.y + height);

            return position;
        }
    }
}
