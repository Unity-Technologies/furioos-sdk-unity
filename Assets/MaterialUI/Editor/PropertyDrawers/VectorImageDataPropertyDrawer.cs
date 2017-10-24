//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    [CustomPropertyDrawer(typeof(VectorImageData), true)]
    class VectorImageDataPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty code = property.FindPropertyRelative("m_Glyph.m_Unicode");
            SerializedProperty name = property.FindPropertyRelative("m_Glyph.m_Name");
            SerializedProperty font = property.FindPropertyRelative("m_Font");
            GUIStyle iconStyle = new GUIStyle { font = (Font)font.objectReferenceValue, fontSize = 16 };

            RectOffset offset = new RectOffset(0, 0, -1, -3);
            position = offset.Add(position);
            position.height = EditorGUIUtility.singleLineHeight;

            float offsetH = 0;

            offsetH -= EditorGUI.PrefixLabel(new Rect(position.x + offsetH, position.y, 40, position.height), label).width;

            offsetH += 40;

            if (!string.IsNullOrEmpty(name.stringValue))
            {
                GUIContent iconLabel = new GUIContent(IconDecoder.Decode(code.stringValue));
                EditorGUI.LabelField(new Rect(position.x + offsetH, position.y, 16, position.height), iconLabel, iconStyle);

                float iconWidth = iconStyle.CalcSize(iconLabel).x;
                offsetH += iconWidth + 2f;

                EditorGUI.LabelField(new Rect(position.x + offsetH, position.y, position.width - offsetH - 80, position.height), name.stringValue);
            }
            else
            {
                EditorGUI.LabelField(new Rect(position.x + offsetH, position.y, position.width - 70 - 56, position.height), "No icon selected");
            }

            if (GUI.Button(new Rect(position.width - 74, position.y, 70, position.height), "Pick Icon"))
            {
                VectorImagePickerWindow.Show((VectorImageData)fieldInfo.GetValue(property.serializedObject.targetObject), property.serializedObject.targetObject);
            }
            if (GUI.Button(new Rect(position.width - 2, position.y, 18, position.height), IconDecoder.Decode(@"\ue14c"), new GUIStyle { font = VectorImageManager.GetIconFont(VectorImageManager.materialDesignIconsFontName) }))
            {
                VectorImageData data = ((VectorImageData)fieldInfo.GetValue(property.serializedObject.targetObject));
                data.font = null;
                data.glyph = null;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }
    }
}