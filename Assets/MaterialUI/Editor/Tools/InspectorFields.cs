//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MaterialUI
{
    /// <summary>Disposable helper class for managing a block of disabled editor controls.</summary>
    public class DisabledScope : GUI.Scope
    {
        /// <summary>The GUI.enabled setting before this scope was opened.</summary>
        private bool m_PreviousSetting;

        /// <summary>Is this scope showing disabled controls?</summary>
        private bool m_Disabled;

        /// <summary>Create a new scope for the editor controls.</summary>
        /// <param name="disabled">Will the controls be disabled?</param>
        public DisabledScope(bool disabled = true)
        {
            m_PreviousSetting = GUI.enabled;
            m_Disabled = disabled;

            if (m_Disabled)
            {
                GUI.enabled = false;
            }
        }

        /// <summary>Closes the scope and returns the GUI.enabled setting to the previous value.</summary>
        protected override void CloseScope()
        {
            if (m_Disabled)
            {
                GUI.enabled = m_PreviousSetting;
            }
        }
    }

    /// <summary>Contains methods used in custom inspectors as alternatives to EditorGUILayout.PropertyField.</summary>
    public static class InspectorFields
    {
        public const float colorFieldMinWidth = 40f;
        public const float colorFieldMaxWidth = 90f;
        public const int vectorIconSize = 16;
        public const float spriteIconSize = 16f;
        public const float miniButtonHeight = 16f;
        public const float vectorImageDataIndentOffsetSize = 16f;
        public const float vectorImageDataMaxLabelWidth = 96f;

        #region GenericMethods

        /// <summary>Make a field for SerializedProperty with an asterisk at the start, signifying that it is important/required.</summary>
        /// <param name="property">The SerializedProperty to make a field for.</param>
        /// <param name="includeChildren">If true the property including children is drawn; otherwise only the control itself (such as only a foldout but nothing below it).</param>
        /// <returns>True if the property has children and is expanded and includeChildren was set to false; otherwise false.</returns>
        public static bool RequiredPropertyField(SerializedProperty property, bool includeChildren = true)
        {
            return EditorGUILayout.PropertyField(property, new GUIContent("*" + property.displayName), includeChildren);
        }

        /// <summary>Used to detect whether multiple GameObjects are selected and ignoreMultiple is false.</summary>
        /// <param name="ignoreMultiple">If false, method returns whether multiple GameObjects are selected. If true, returns false.</param>
        /// <returns>Whether multiple GameObjects are selected and ignoreMultiple is false.</returns>
        private static bool TooManySelected(bool ignoreMultiple)
        {
            if (ignoreMultiple) return false;
            return Selection.gameObjects.Length > 1;
        }

        /// <summary>Alternative to EditorGUILayout.PropertyField. Supports undo/redo.</summary>
        /// <typeparam name="T">The type of the object to mark for undo/redo.</typeparam>
        /// <param name="objectToDo">The object to mark for undo/redo.</param>
        /// <param name="fieldAction">This action will contain the field action eg: EditorGUILayout.TextField.</param>
        /// <param name="disableIfMultipleObjectsSelected">Is this field drawn disabled if multiple GameObjects are selected?</param>
        public static void PropertyField<T>(T objectToDo, Action fieldAction, bool disableIfMultipleObjectsSelected = true) where T : Object
        {
            PropertyField(objectToDo, fieldAction, null, null, disableIfMultipleObjectsSelected);
        }

        /// <summary>Alternative to EditorGUILayout.PropertyField. Supports undo/redo.</summary>
        /// <typeparam name="T">The type of the object to mark for undo/redo.</typeparam>
        /// <param name="objectToDo">The object to mark for undo/redo.</param>
        /// <param name="fieldAction">This action will contain the field action eg: EditorGUILayout.TextField.</param>
        /// <param name="onChangeAction">This action is called when the inspector control drawn in fieldAction is changed.</param>
        /// <param name="onNoChangeAction">This action is called if the control drawn in fieldAction is not changed.</param>
        /// <param name="disableIfMultipleObjectsSelected">Is this field drawn disabled if multiple GameObjects are selected?</param>
        public static void PropertyField<T>(T objectToDo, Action fieldAction, Action onChangeAction, Action onNoChangeAction, bool disableIfMultipleObjectsSelected = false) where T : Object
        {
            using (new DisabledScope(TooManySelected(!disableIfMultipleObjectsSelected)))
            {
                Undo.RecordObject(objectToDo, "edit value");
                EditorGUI.BeginChangeCheck();
                {
                    fieldAction.InvokeIfNotNull();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    onChangeAction.InvokeIfNotNull();
                    EditorUtility.SetDirty(objectToDo);
                }
                else
                {
                    Undo.ClearUndo(objectToDo);
                    onNoChangeAction.InvokeIfNotNull();
                }
            }
        }

        /// <summary>Alternative to EditorGUILayout.PropertyField. Supports undo/redo and multiple selected objects.</summary>
        /// <typeparam name="T1">The type of the objects to mark for undo/redo.</typeparam>
        /// <typeparam name="T2">The type that is passed from fieldFunc to onChangeAction.</typeparam>
        /// <param name="objectsToDo">Objects to mark for undo/redo.</param>
        /// <param name="fieldFunc">This func will contain the field action and return its 'output' value eg: EditorGUILayout.TextField.</param>
        /// <param name="onChangeAction">This action is called when the inspector control drawn in fieldAction is changed. Set the multiple objects' values in here.</param>
        public static void PropertyMultiField<T1, T2>(T1[] objectsToDo, Func<T2> fieldFunc, Action<T2> onChangeAction) where T1 : Object
        {
            PropertyMultiField(objectsToDo, fieldFunc, onChangeAction, null);
        }

        /// <summary>Alternative to EditorGUILayout.PropertyField. Supports undo/redo and multiple selected objects.</summary>
        /// <typeparam name="T1">The type of the objects to mark for undo/redo.</typeparam>
        /// <typeparam name="T2">The type that is passed from fieldFunc to onChangeAction.</typeparam>
        /// <param name="objectsToDo">Objects to mark for undo/redo.</param>
        /// <param name="fieldFunc">This func will contain the field action and return its 'output' value eg: EditorGUILayout.TextField.</param>
        /// <param name="onChangeAction">This action is called when the inspector control drawn in fieldAction is changed. Set the multiple objects' values in here.</param>
        /// <param name="onNoChangeAction">This action is called if the control drawn in fieldAction is not changed.</param>
        public static void PropertyMultiField<T1, T2>(T1[] objectsToDo, Func<T2> fieldFunc, Action<T2> onChangeAction, Action onNoChangeAction) where T1 : Object
        {
            for (int i = 0; i < objectsToDo.Length; i++)
            {
                Undo.RecordObject(objectsToDo[i], "edit value");
            }

            T2 value = default(T2);

            EditorGUI.BeginChangeCheck();
            {
                value = fieldFunc.InvokeIfNotNull();
            }
            if (EditorGUI.EndChangeCheck())
            {
                onChangeAction.InvokeIfNotNull(value);

                for (int i = 0; i < objectsToDo.Length; i++)
                {
                    EditorUtility.SetDirty(objectsToDo[i]);
                }
            }
            else
            {
                for (int i = 0; i < objectsToDo.Length; i++)
                {
                    Undo.ClearUndo(objectsToDo[i]);
                }

                onNoChangeAction.InvokeIfNotNull();
            }
        }

        #endregion

        #region SimpleFields

        /// <summary>Alternative inspector control (eg: EditorGUILayout.SpriteField) for Image.sprite/VectorImage.vectorImageData/Text.text. Supports undo/redo.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="graphic">The Graphic to modify.</param>
        /// <param name="disableIfMultipleObjectsSelected">Is this field drawn disabled if multiple GameObjects are selected?</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool GraphicField(string label, Graphic graphic, bool disableIfMultipleObjectsSelected = true)
        {

            using (new DisabledScope(TooManySelected(!disableIfMultipleObjectsSelected)))
            {
                Image image = graphic as Image;
                if (image != null)
                {
                    ImageField(label, image, disableIfMultipleObjectsSelected);
                    return true;
                }

                VectorImage vectorImage = graphic as VectorImage;
                if (vectorImage != null)
                {
                    VectorImageField(label, vectorImage, disableIfMultipleObjectsSelected);
                    return true;
                }

                Text text = graphic as Text;
                if (text != null)
                {
                    TextField(label, text);
                    return true;
                }
            }

            return false;
        }

        /// <summary>Alternative inspector control (eg: EditorGUILayout.SpriteField) for Image.sprite/VectorImage.vectorImageData/Text.text. Supports undo/redo and multiple selected objects.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="getReferenceFunc">The function used to get each graphic reference from each selected GameObject.</param>
        /// <param name="visualReference">The Graphic to use for the 'current' values in the control if the Graphic values are different.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool GraphicMultiField(string label, Func<GameObject, Graphic> getReferenceFunc, Graphic visualReference = null)
        {
            GraphicMultiField(label, Selection.gameObjects.Select(getReferenceFunc).ToArray());
            return true;
        }

        /// <summary>Alternative inspector control (eg: EditorGUILayout.SpriteField) for Image.sprite/VectorImage.vectorImageData/Text.text. Supports undo/redo and multiple selected objects.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="graphics">The Graphics that the modifications will be applied to.</param>
        /// <param name="visualReference">The Graphic to use for the 'current' values in the control if the Graphic values are different.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool GraphicMultiField(string label, Graphic[] graphics, Graphic visualReference = null)
        {

            if (visualReference == null)
            {
                visualReference = graphics[0];
            }

            Image image = visualReference as Image;
            if (image != null)
            {
                ImageMultiField(label, graphics.Select(graphic => graphic as Image).ToArray(), visualReference as Image);
                return true;
            }

            VectorImage vectorImage = visualReference as VectorImage;
            if (vectorImage != null)
            {
                VectorImageMultiField(label, graphics.Select(graphic => graphic as VectorImage).ToArray(), visualReference as VectorImage);
                return true;
            }

            Text text = visualReference as Text;
            if (text != null)
            {
                TextMultiField(label, graphics.Select(graphic => graphic as Text).ToArray(), visualReference as Text);
                return true;
            }

            return true;
        }

        /// <summary>Alternative inspector control (eg: EditorGUILayout.TextField) for Text.text. Supports undo/redo.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="text">The Text to modify.</param>
        /// <param name="disableIfMultipleObjectsSelected">Is this field drawn disabled if multiple GameObjects are selected?</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool TextField(string label, Text text, bool disableIfMultipleObjectsSelected = true)
        {
            PropertyField(text, () => text.text = EditorGUILayout.TextField(label, text.text), disableIfMultipleObjectsSelected);
            return true;
        }

        /// <summary>Alternative inspector control (eg: EditorGUILayout.TextField) for Text.text. Supports undo/redo and multiple selected objects.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="getReferenceFunc">The function used to get each text reference from each selected GameObject.</param>
        /// <param name="visualReference">The Text to use for the 'current' values in the control if the Text values are different.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool TextMultiField(string label, Func<GameObject, Text> getReferenceFunc, Text visualReference = null)
        {
            return TextMultiField(label, Selection.gameObjects.Select(getReferenceFunc).ToArray());
        }

        /// <summary>Alternative inspector control (eg: EditorGUILayout.TextField) for Text.text. Supports undo/redo and multiple selected objects.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="texts">The Texts that the modifications will be applied to.</param>
        /// <param name="visualReference">The Text to use for the 'current' values in the control if the Text values are different.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool TextMultiField(string label, Text[] texts, Text visualReference = null)
        {

            if (texts.ToList().TrueForAll(text => text == null)) return false;

            if (visualReference == null)
            {
                visualReference = texts.ToList().First(text => text != null);
            }

            Action<string> textOnChangeAction = text =>
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].text = text;
                }
            };

            PropertyMultiField(texts, () => EditorGUILayout.TextField(label, visualReference.text), textOnChangeAction);

            return true;
        }

        /// <summary>Alternative inspector control (eg: EditorGUILayout.SpriteField) for Image.sprite. Supports undo/redo.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="image">The Image to modify.</param>
        /// <param name="disableIfMultipleObjectsSelected">Is this field drawn disabled if multiple GameObjects are selected?</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool ImageField(string label, Image image, bool disableIfMultipleObjectsSelected = true)
        {

            using (new DisabledScope(TooManySelected(!disableIfMultipleObjectsSelected)))
            {
                EditorGUILayout.PrefixLabel(label);
                EditorGUILayout.LabelField(GUIContent.none, new GUIStyle { normal = { background = (Texture2D)image.mainTexture } }, GUILayout.Width(spriteIconSize), GUILayout.Height(spriteIconSize));

                if (GUILayout.Button("Pick Sprite", EditorStyles.miniButton, GUILayout.MinHeight(miniButtonHeight)))
                {
                    EditorGUIUtility.ShowObjectPicker<Sprite>(image.sprite, false, "", 0);
                }

                if (Event.current.commandName == "ObjectSelectorUpdated")
                {
                    Sprite sprite = EditorGUIUtility.GetObjectPickerObject() as Sprite;
                    image.sprite = sprite;
                    EditorUtility.SetDirty(image);
                }
            }

            return true;
        }

        /// <summary>Alternative inspector control (eg: EditorGUILayout.SpriteField) for Image.sprite. Supports undo/redo and multiple selected objects.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="getReferenceFunc">The function used to get each Image reference from each selected GameObject.</param>
        /// <param name="visualReference">The Image to use for the 'current' values in the control if the Image values are different.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool ImageMultiField(string label, Func<GameObject, Image> getReferenceFunc, Image visualReference = null)
        {
            return ImageMultiField(label, Selection.gameObjects.Select(getReferenceFunc).ToArray());
        }

        /// <summary>Alternative inspector control (eg: EditorGUILayout.SpriteField) for Image.sprite. Supports undo/redo and multiple selected objects.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="images">The images that the modifications will be applied to.</param>
        /// <param name="visualReference">The Image to use for the 'current' values in the control if the Image values are different.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool ImageMultiField(string label, Image[] images, Image visualReference = null)
        {

            if (images.ToList().TrueForAll(image => image == null)) return false;

            if (visualReference == null)
            {
                visualReference = images.ToList().First(image => image != null);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(label);
                EditorGUILayout.LabelField(GUIContent.none, new GUIStyle { normal = { background = (Texture2D)visualReference.mainTexture } }, GUILayout.Width(spriteIconSize), GUILayout.Height(spriteIconSize));

                if (GUILayout.Button("Pick Sprite", EditorStyles.miniButton, GUILayout.MinHeight(miniButtonHeight)))
                {
                    EditorGUIUtility.ShowObjectPicker<Sprite>(visualReference.sprite, false, "", 0);
                }
            }

            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                Sprite sprite = EditorGUIUtility.GetObjectPickerObject() as Sprite;

                for (int i = 0; i < images.Length; i++)
                {
                    Undo.RecordObject(images[i], "edit value");
                    images[i].sprite = sprite;
                    EditorUtility.SetDirty(images[i]);
                }
            }

            return true;
        }

        /// <summary>Inspector control (eg: EditorGUILayout.SpriteField) for VectorImageData. Supports undo/redo.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="targetData">The VectorImageData to modify.</param>
        /// <param name="objectToUndo">The object to mark for undo/redo.</param>
        /// <param name="onIconPickAction">Action to call when an icon is picked.</param>
        /// <param name="disableIfMultipleObjectsSelected">Is this field drawn disabled if multiple GameObjects are selected?</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool VectorImageDataField(string label, VectorImageData targetData, Object objectToUndo, Action onIconPickAction = null, bool disableIfMultipleObjectsSelected = true)
        {
            using (new DisabledScope(TooManySelected(!disableIfMultipleObjectsSelected)))
            {
                string code = targetData.glyph.unicode;
                string name = targetData.glyph.name;
                Font font = targetData.font;
                GUIStyle iconStyle = new GUIStyle { font = font, fontSize = vectorIconSize };
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel(label);
                    if (EditorGUI.indentLevel > 0)
                    {
                        //  2 is a good 'base' offset when there are indents
                        EditorGUILayout.LabelField("", GUILayout.MaxWidth(-(2 + vectorImageDataIndentOffsetSize * EditorGUI.indentLevel)));
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        GUIContent iconGUIContent = new GUIContent(IconDecoder.Decode(code), name);
                        EditorGUILayout.LabelField(iconGUIContent, iconStyle, GUILayout.MaxWidth(iconStyle.CalcSize(iconGUIContent).x));

                        if (EditorGUI.indentLevel > 0)
                        {
                            EditorGUILayout.LabelField("", GUILayout.MaxWidth(vectorImageDataIndentOffsetSize * EditorGUI.indentLevel));
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("No icon selected", GUILayout.MaxWidth(vectorImageDataMaxLabelWidth));
                    }

                    if (GUILayout.Button("Pick Icon", EditorStyles.miniButton, GUILayout.MinHeight(miniButtonHeight)))
                    {
                        VectorImagePickerWindow.Show(targetData, objectToUndo, onIconPickAction);
                    }
                }
            }

            return true;
        }

        /// <summary>Inspector control (eg: EditorGUILayout.SpriteField) for VectorImageData. Supports undo/redo and multiple selected objects.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="getReferenceFunc">The function used to get each VectorImageData reference from each selected GameObject.</param>
        /// <param name="objectsToUndo">The objects to mark for undo/redo.</param>
        /// <param name="onIconPickAction">Action to call when an icon is picked.</param>
        /// <param name="visualReference">The VectorImageData to use for the 'current' values in the control if the VectorImageData values are different.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool VectorImageDataMultiField(string label, Func<GameObject, VectorImageData> getReferenceFunc, Object[] objectsToUndo, Action onIconPickAction = null, VectorImageData visualReference = null)
        {
            return VectorImageDataMultiField(label, Selection.gameObjects.Select(getReferenceFunc).ToArray(), objectsToUndo, onIconPickAction, visualReference);
        }

        /// <summary>Inspector control (eg: EditorGUILayout.SpriteField) for VectorImageData. Supports undo/redo and multiple selected objects.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="targetDatas">The VectorImageDatas to modify.</param>
        /// <param name="objectsToUndo">The objects to mark for undo/redo.</param>
        /// <param name="onIconPickAction">Action to call when an icon is picked.</param>
        /// <param name="visualReference">The VectorImageData to use for the 'current' values in the control if the VectorImageData values are different.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool VectorImageDataMultiField(string label, VectorImageData[] targetDatas, Object[] objectsToUndo, Action onIconPickAction = null, VectorImageData visualReference = null)
        {

            if (targetDatas.ToList().TrueForAll(data => data == null)) return false;

            if (visualReference == null)
            {
                visualReference = targetDatas.ToList().First(data => data != null);
            }

            string code = visualReference.glyph.unicode;
            string name = visualReference.glyph.name;
            Font font = visualReference.font;
            GUIStyle iconStyle = new GUIStyle { font = font, fontSize = vectorIconSize };

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(label);
                if (EditorGUI.indentLevel > 0)
                {
                    //  2 is a good 'base' offset when there are indents
                    EditorGUILayout.LabelField("", GUILayout.MaxWidth(-(2 + vectorImageDataIndentOffsetSize * EditorGUI.indentLevel)));
                }

                if (!string.IsNullOrEmpty(name))
                {
                    GUIContent iconGUIContent = new GUIContent(IconDecoder.Decode(code), name);
                    EditorGUILayout.LabelField(iconGUIContent, iconStyle, GUILayout.MaxWidth(iconStyle.CalcSize(iconGUIContent).x));

                    if (EditorGUI.indentLevel > 0)
                    {
                        EditorGUILayout.LabelField("", GUILayout.MaxWidth(vectorImageDataIndentOffsetSize * EditorGUI.indentLevel));
                    }

                    GUILayout.FlexibleSpace();
                }
                else
                {
                    EditorGUILayout.LabelField("No icon selected", GUILayout.MaxWidth(vectorImageDataMaxLabelWidth));
                    GUILayout.FlexibleSpace();
                }

                if (GUILayout.Button("Pick Icon", EditorStyles.miniButton, GUILayout.MinHeight(miniButtonHeight)))
                {
                    VectorImagePickerWindow.Show(targetDatas, objectsToUndo, onIconPickAction);
                }
            }

            return true;
        }

        /// <summary>Inspector control (eg: EditorGUILayout.SpriteField) for VectorImage.vectorImageData. Supports undo/redo.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="vectorImage">The VectorImage to modify.</param>
        /// <param name="disableIfMultipleObjectsSelected">Is this field drawn disabled if multiple GameObjects are selected?</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool VectorImageField(string label, VectorImage vectorImage, bool disableIfMultipleObjectsSelected = true)
        {
            return VectorImageDataField(label, vectorImage.vectorImageData, vectorImage, vectorImage.Refresh, disableIfMultipleObjectsSelected);
        }

        /// <summary>Inspector control (eg: EditorGUILayout.SpriteField) for VectorImage.vectorImageData. Supports undo/redo.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="getReferenceFunc">The function used to get each VectorImage reference from each selected GameObject.</param>
        /// <param name="visualReference">The VectorImage to use for the 'current' values in the control if the VectorImage values are different.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool VectorImageMultiField(string label, Func<GameObject, VectorImage> getReferenceFunc, VectorImage visualReference = null)
        {
            return VectorImageMultiField(label, Selection.gameObjects.Select(getReferenceFunc).ToArray(), visualReference);
        }

        /// <summary>Inspector control (eg: EditorGUILayout.SpriteField) for VectorImage.vectorImageData. Supports undo/redo.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="vectorImages">The VectorImages to modify.</param>
        /// <param name="visualReference">The VectorImage to use for the 'current' values in the control if the VectorImage values are different.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool VectorImageMultiField(string label, VectorImage[] vectorImages, VectorImage visualReference = null)
        {
            if (vectorImages.ToList().TrueForAll(vectorImage => vectorImage == null)) return false;

            if (visualReference == null)
            {
                visualReference = vectorImages.ToList().First(vectorImage => vectorImage != null);
            }

            List<VectorImageData> vectorImageDatas = new List<VectorImageData>();

            for (int i = 0; i < vectorImages.Length; i++)
            {
                if (vectorImages[i] != null && vectorImages[i].vectorImageData != null)
                {
                    vectorImageDatas.Add(vectorImages[i].vectorImageData);
                }
            }

            Action iconPickAction = () =>
            {
                for (int i = 0; i < vectorImages.Length; i++)
                {
                    if (vectorImages[i] != null)
                    {
                        vectorImages[i].Refresh();
                    }
                }
            };

            return VectorImageDataMultiField(label, vectorImageDatas.ToArray(), vectorImages, iconPickAction, visualReference.vectorImageData);
        }

        #endregion

        #region ComplexFields

        /// <summary>Inspector control (eg: EditorGUILayout.SpriteField) for Image.sprite/VectorImage.vectorImageData/Text.text and .color. Supports undo/redo.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="graphic">The graphic to modify.</param>
        /// <param name="disableIfMultipleObjectsSelected">Is this field drawn disabled if multiple GameObjects are selected?</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool GraphicColorField(string label, Graphic graphic, bool disableIfMultipleObjectsSelected = true)
        {

            if (graphic == null) return false;

            using (new EditorGUILayout.HorizontalScope())
            {
                GraphicField(label, graphic, disableIfMultipleObjectsSelected);
                PropertyField(graphic, () => graphic.color = EditorGUILayout.ColorField(graphic.color, GUILayout.MinWidth(colorFieldMinWidth), GUILayout.MaxWidth(colorFieldMaxWidth)), disableIfMultipleObjectsSelected);
            }
            return true;
        }

        /// <summary>Inspector control (eg: EditorGUILayout.SpriteField) for Image.sprite/VectorImage.vectorImageData/Text.text and .color. Supports undo/redo and multiple selected objects.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="getReferenceFunc">The function used to get each Graphic reference from each selected GameObject.</param>
        /// <param name="visualReference">The Graphic to use for the 'current' values in the control if the Graphic values are different</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool GraphicColorMultiField(string label, Func<GameObject, Graphic> getReferenceFunc, Graphic visualReference = null)
        {
            return GraphicColorMultiField(label, Selection.gameObjects.Select(getReferenceFunc).ToArray(), visualReference);
        }

        /// <summary>Inspector control (eg: EditorGUILayout.SpriteField) for Image.sprite/VectorImage.vectorImageData/Text.text and .color. Supports undo/redo and multiple selected objects.</summary>
        /// <param name="label">The label to appear in the inspector control.</param>
        /// <param name="graphics">The graphics to modify.</param>
        /// <param name="visualReference">The Graphic to use for the 'current' values in the control if the Graphic values are different.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool GraphicColorMultiField(string label, Graphic[] graphics, Graphic visualReference = null)
        {
            if (graphics.ToList().TrueForAll(graphic => graphic == null)) return false;

            if (visualReference == null)
            {
                visualReference = graphics.ToList().First(graphic => graphic != null);
            }

            Action<Color> onChangeAction = color =>
            {
                for (int i = 0; i < graphics.Length; i++)
                {
                    graphics[i].color = color;
                }
            };

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new DisabledScope(!graphics.ToList().TrueForAll(graphic => graphic.GetType() == visualReference.GetType())))
                {
                    GraphicMultiField(label, graphics, visualReference);
                }
                PropertyMultiField(graphics, () => EditorGUILayout.ColorField(visualReference.color, GUILayout.MinWidth(colorFieldMinWidth), GUILayout.MaxWidth(colorFieldMaxWidth)), onChangeAction);
            }

            return true;
        }

        /// <summary>Inspector control (eg: EditorGUILayout.SpriteField) for all of MaterialButton's external values. Supports undo/redo.</summary>
        /// <param name="materialButton">The MaterialButton to modify.</param>
        /// <param name="showBox">Whether to surround the inspector controls with a box.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool MaterialButtonField(MaterialButton materialButton, bool showBox = true)
        {
            if (materialButton == null) return false;

            string nameLabel = "'" + materialButton.name + "' ";

            bool result = false;

            if (materialButton.text == null && materialButton.icon == null && materialButton.backgroundImage == null) return false;

            using (showBox ? new EditorGUILayout.VerticalScope("Box") : new EditorGUILayout.VerticalScope())
            {
                if (GraphicColorField(nameLabel + "Text", materialButton.text))
                {
                    nameLabel = showBox ? "" : "^ ";
                    result = true;
                }
                if (GraphicColorField(nameLabel + "Icon", materialButton.icon))
                {
                    nameLabel = showBox ? "" : "^ ";
                    result = true;
                }
                if (GraphicColorField(nameLabel + "Background", materialButton.backgroundImage))
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>Inspector control (eg: EditorGUILayout.SpriteField) for all of MaterialButton's external values. Supports undo/redo and multiple selected objects.</summary>
        /// <param name="getReferenceFunc">The function used to get each Graphic reference from each selected GameObject.</param>
        /// <param name="showBox">Whether to surround the inspector controls with a box</param>
        /// <param name="visualReference">The MaterialButton to use for the 'current' values in the control if the MaterialButton values are different.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool MaterialButtonMultiField(Func<GameObject, MaterialButton> getReferenceFunc, bool showBox = true, MaterialButton visualReference = null)
        {
            return MaterialButtonMultiField(Selection.gameObjects.Select(getReferenceFunc).ToArray(), showBox, visualReference);
        }

        /// <summary>Inspector control (eg: EditorGUILayout.SpriteField) for all of MaterialButton's external values. Supports undo/redo and multiple selected objects.</summary>
        /// <param name="materialButtons">The MaterialButtons to modify.</param>
        /// <param name="showBox">Whether to surround the inspector controls with a box</param>
        /// <param name="visualReference">The MaterialButton to use for the 'current' values in the control if the MaterialButton values are different.</param>
        /// <returns>Whether the control was successfully able to be drawn.</returns>
        public static bool MaterialButtonMultiField(MaterialButton[] materialButtons, bool showBox = true, MaterialButton visualReference = null)
        {
            if (materialButtons.ToList().TrueForAll(materialButton => materialButton == null)) return false;

            if (visualReference == null)
            {
                visualReference = materialButtons.ToList().First(materialButton => materialButton != null);
            }

            string nameLabel = "'" + visualReference.name + "' ";

            bool result = false;

            bool hasText = false;
            bool hasIcon = false;
            bool hasBackground = false;

            List<Graphic> textGraphics = new List<Graphic>();
            List<Graphic> iconGraphics = new List<Graphic>();
            List<Graphic> backgroundGraphics = new List<Graphic>();

            for (int i = 0; i < materialButtons.Length; i++)
            {
                MaterialButton button = materialButtons[i];
                if (button == null) continue;

                if (button.text != null)
                {
                    textGraphics.Add(button.text);
                    hasText = true;
                }

                if (button.icon != null)
                {
                    iconGraphics.Add(button.icon);
                    hasIcon = true;
                }

                if (button.backgroundImage != null)
                {
                    backgroundGraphics.Add(button.backgroundImage);
                    hasBackground = true;
                }
            }

            if (!hasText && !hasIcon && !hasBackground) return false;

            using (showBox ? new EditorGUILayout.VerticalScope("Box") : new EditorGUILayout.VerticalScope())
            {
                if (hasText)
                {
                    GraphicColorMultiField(nameLabel + "Text", textGraphics.ToArray());
                    nameLabel = showBox ? "" : "^ ";
                    result = true;
                }

                if (hasIcon)
                {
                    GraphicColorMultiField(nameLabel + "Icon", iconGraphics.ToArray());
                    nameLabel = showBox ? "" : "^ ";
                    result = true;
                }

                if (hasBackground)
                {
                    GraphicColorMultiField(nameLabel + "Background", backgroundGraphics.ToArray());
                    result = true;
                }
            }

            return result;
        }

        #endregion
    }
}