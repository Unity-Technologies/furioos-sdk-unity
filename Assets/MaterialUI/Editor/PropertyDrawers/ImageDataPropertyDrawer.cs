//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    [CustomPropertyDrawer(typeof(ImageData), true)]
    public class ImageDataPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty m_ImageDataType;
        private SerializedProperty m_Sprite;
        private SerializedProperty m_VectorImageData;

        private void GetProperties(SerializedProperty property)
        {
            m_ImageDataType = property.FindPropertyRelative("m_ImageDataType");
            m_Sprite = property.FindPropertyRelative("m_Sprite");
            m_VectorImageData = property.FindPropertyRelative("m_VectorImageData");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GetProperties(property);

            Rect pos = position;
            pos.height = EditorGUI.GetPropertyHeight(m_ImageDataType);

            float labelWidth = GUI.skin.GetStyle("Label").CalcSize(label).x;

            float i = EditorGUI.PrefixLabel(new Rect(pos.x, pos.y, labelWidth, pos.height), label).width;

            pos.x -= i - 54;
            pos.width += i - 55;

			if (property.name.Equals("m_ToggleOnIcon") || property.name.Equals("m_ToggleOffIcon"))
			{
				pos.x += 33;
				pos.width -= 33;
			}
			else if (property.name.Equals("m_TabIcon"))
			{
				pos.x -= 3;
				pos.width += 3;
			}

            if (m_ImageDataType.enumValueIndex == 0)
            {
                pos.height = EditorGUI.GetPropertyHeight(m_Sprite);
                EditorGUI.PropertyField(pos, m_Sprite, GUIContent.none);
            }

            if (m_ImageDataType.enumValueIndex == 1)
            {
                pos.height = EditorGUI.GetPropertyHeight(m_VectorImageData);

                SerializedProperty code = m_VectorImageData.FindPropertyRelative("m_Glyph.m_Unicode");
                SerializedProperty name = m_VectorImageData.FindPropertyRelative("m_Glyph.m_Name");
                SerializedProperty font = m_VectorImageData.FindPropertyRelative("m_Font");
                GUIStyle iconStyle = new GUIStyle { font = (Font)font.objectReferenceValue };

                float pickButtonWidth = 70;
                float pickButtonPadding = 88;
                float clearButtonWidth = 18;
                float clearButtonPadding = 14;

                if (!string.IsNullOrEmpty(name.stringValue))
                {
                    EditorGUI.LabelField(new Rect(pos.x, pos.y, 16, pos.height), IconDecoder.Decode(code.stringValue),
                        iconStyle);
                    pos.x += 16;
                    pos.width -= 16;
                    EditorGUI.LabelField(new Rect(pos.x, pos.y, pos.width, position.height), name.stringValue);
                }
                else
                {
                    EditorGUI.LabelField(
                        new Rect(pos.x, pos.y, pos.width - (pickButtonWidth + clearButtonWidth), pos.height),
                        "No icon selected");
                }

                if (GUI.Button(new Rect((pos.x + pos.width) - pickButtonPadding, pos.y, pickButtonWidth, pos.height),
                    "Pick Icon"))
                {
                    VectorImagePickerWindow.Show(
                        ((ImageData)fieldInfo.GetValue(m_VectorImageData.serializedObject.targetObject))
                            .vectorImageData, property.serializedObject.targetObject);
                }

                if (GUI.Button(new Rect((pos.x + pos.width - 1) - clearButtonPadding, pos.y, clearButtonWidth, pos.height),
                    IconDecoder.Decode(@"\ue14c"),
                    new GUIStyle { font = VectorImageManager.GetIconFont(VectorImageManager.materialDesignIconsFontName) }))
                {
                    VectorImageData data =
                        (((ImageData)fieldInfo.GetValue(m_VectorImageData.serializedObject.targetObject))
                            .vectorImageData);
                    data.font = null;
                    data.glyph = null;
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }
        }

        //public static void DrawWithoutPopup(SerializedProperty property)
        //{
        //    DrawWithoutPopup(property, new GUIContent(property.displayName));
        //}

        //public static void DrawWithoutPopup(SerializedProperty property, GUIContent label)
        //{
        //    SerializedProperty imageDataType = property.FindPropertyRelative("m_ImageDataType");
        //    SerializedProperty sprite = property.FindPropertyRelative("m_Sprite");
        //    SerializedProperty imageData = property.FindPropertyRelative("m_VectorImageData");

        //    if (imageDataType.enumValueIndex == 0)
        //    {
        //        EditorGUILayout.PropertyField(sprite, label);
        //    }

        //    if (imageDataType.enumValueIndex == 1)
        //    {
		//        SerializedProperty code = imageData.FindPropertyRelative("m_Glyph.m_Unicode");
        //        SerializedProperty name = imageData.FindPropertyRelative("m_Glyph.m_Name");
        //        SerializedProperty font = imageData.FindPropertyRelative("m_Font");
        //        GUIStyle iconStyle = new GUIStyle { font = (Font)font.objectReferenceValue };

        //        EditorGUILayout.BeginHorizontal();
        //        {
        //            EditorGUILayout.PrefixLabel(label);
        //            if (!string.IsNullOrEmpty(name.stringValue))
        //            {
        //                EditorGUILayout.LabelField(IconDecoder.Decode(code.stringValue), iconStyle, GUILayout.MaxWidth(16));
        //                EditorGUILayout.LabelField(name.stringValue);
        //            }
        //            else
        //            {
        //                EditorGUILayout.LabelField("No icon selected");
        //            }

        //            if (GUILayout.Button("Pick Icon"))
        //            {
        //                //VectorImagePickerWindow.Show((VectorImageData)fieldInfo.GetValue(property.serializedObject.targetObject), property.serializedObject.targetObject);
        //            }
        //            if (GUILayout.Button(IconDecoder.Decode(@"\ue14c"), new GUIStyle { font = VectorImageManager.GetIconFont(VectorImageManager.MaterialDesignIconsFontName) }))
        //            {
        //                //VectorImageData data = ((VectorImageData)fieldInfo.GetValue(property.serializedObject.targetObject));
        //                //data.font = null;
        //                //data.glyph = null;
        //                //EditorUtility.SetDirty(property.serializedObject.targetObject);
        //            }
        //        }
        //        EditorGUILayout.EndHorizontal();
        //    }
        //}
    }
}