//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using Object = UnityEngine.Object;

namespace MaterialUI
{
    public class VectorImagePickerWindow : EditorWindow
    {
        private Vector2 m_ScrollPosition;
        private Vector2 m_IconViewScrollPosition;
        private static VectorImageData[] m_VectorImageDatas;
        private static Action m_RefreshAction;
        private static Object[] m_ObjectsToRefresh;
        private static int m_PreviewSize = 48;
        private VectorImageSet m_VectorImageSet;
        private Font m_IconFont;
        private GUIStyle m_GuiStyle;
        private static Texture2D m_BackdropTexture;

        private float m_LastClickTime = float.MinValue;
        private GUIStyle m_BottomBarBg = "ProjectBrowserBottomBarBg";

        private string m_SearchText;
        private Glyph[] m_GlyphArray;

        public static void Show(VectorImageData data, Object objectToRefresh)
        {
            Show(new[] { data }, new[] { objectToRefresh }, null);
        }

        public static void Show(VectorImageData data, Object objectToRefresh, Action refreshAction)
        {
            Show(new[] { data }, new[] { objectToRefresh }, refreshAction);
        }

        public static void Show(VectorImageData[] datas, Object[] objectsToRefresh)
        {
            Show(datas, objectsToRefresh, null);
        }

        public static void Show(VectorImageData[] datas, Object[] objectsToRefresh, Action refreshAction)
        {
            m_VectorImageDatas = datas;
            m_ObjectsToRefresh = objectsToRefresh;
            m_RefreshAction = refreshAction;

            VectorImagePickerWindow window = CreateInstance<VectorImagePickerWindow>();
            window.ShowAuxWindow();
            window.minSize = new Vector2(397, 446);
            window.titleContent = new GUIContent("Icon Picker");

            m_PreviewSize = EditorPrefs.GetInt("ICON_CONFIG_PREVIEW_SIZE", 48);
        }

        void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        private void OnFocus()
        {
            Repaint();
        }

        private void OnSelectionChange()
        {
            Repaint();
        }

        void OnGUI()
        {
            if (Event.current.isKey) // If we detect the user pressed the keyboard
            {
                EditorGUI.FocusTextInControl("SearchInputField");
            }

            if (Event.current.type == EventType.KeyDown)
            {
                KeyCode keyCode = Event.current.keyCode;
                if (keyCode != KeyCode.Return)
                {
                    if (keyCode == KeyCode.Escape)
                    {
                        base.Close();
                        GUIUtility.ExitGUI();
                        return;
                    }
                }
                else
                {
                    Close();
                    GUIUtility.ExitGUI();
                    return;
                }
            }

            using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(m_ScrollPosition))
            {
                m_ScrollPosition = scrollViewScope.scrollPosition;

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(5f);

                    using (new GUILayout.VerticalScope())
                    {
                        GUILayout.Space(10f);
                        DrawSearchTextField();
                        DrawPicker();
                    }

                    GUILayout.Space(5f);
                }
            }
        }

        public static void DrawIconPickLine(VectorImageData data, Object objectToRefresh, bool indent = false)
        {
            using (new GUILayout.HorizontalScope())
            {
                if (data.font == null)
                {
                    data.font = VectorImageManager.GetIconFont(VectorImageManager.GetAllIconSetNames()[0]);
                }

                GUIStyle iconGuiStyle = new GUIStyle { font = VectorImageManager.GetIconFont(data.font.name) };

                EditorGUILayout.PrefixLabel("Icon");

                if (indent)
                {
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.LabelField(IconDecoder.Decode(data.glyph.unicode), iconGuiStyle, GUILayout.Width(18f));
                EditorGUILayout.LabelField(data.glyph.name, GUILayout.MaxWidth(100f), GUILayout.MinWidth(0f));

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Pick icon", EditorStyles.miniButton, GUILayout.MaxWidth(60f)))
                {
                    Show(data, objectToRefresh);
                    return;
                }

                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.MaxWidth(20f)))
                {
                    for (int i = 0; i < m_VectorImageDatas.Length; i++)
                    {
                        m_VectorImageDatas[i] = null;
                    }
                    return;
                }

                if (indent)
                {
                    EditorGUI.indentLevel++;
                }
            }
        }

        private void DrawPicker()
        {
            if (m_VectorImageDatas[0] == null)
            {
                GUILayout.Label("Invalid vector image");
                return;
            }

            if (m_VectorImageDatas[0].glyph == null)
            {
                GUILayout.Label("Invalid glyph");
                return;
            }

            if (m_VectorImageDatas[0].font == null)
            {
                m_VectorImageDatas[0].font = VectorImageManager.GetIconFont(VectorImageManager.GetAllIconSetNames()[1]);
            }

            string[] names = VectorImageManager.GetAllIconSetNames();

            if (!names.Contains(m_VectorImageDatas[0].font.name))
            {
                m_VectorImageDatas[0].font.name = names[1];
            }

            if (VectorImageManager.GetAllIconSetNames().Length > 0)
            {
                EditorGUI.BeginChangeCheck();
                GUIContent[] namesContents = new GUIContent[names.Length];
                for (int i = 0; i < names.Length; i++)
                {
                    namesContents[i] = new GUIContent(names[i]);
                }

                m_VectorImageDatas[0].font = VectorImageManager.GetIconFont(names[EditorGUILayout.Popup(new GUIContent("Current Pack"), names.ToList().IndexOf(m_VectorImageDatas[0].font.name), namesContents)]);

                bool changed = EditorGUI.EndChangeCheck();

                if (changed)
                {
                    m_IconViewScrollPosition = Vector2.zero;
                }

                if (changed || m_VectorImageSet == null || m_IconFont == null)
                {
                    UpdateFontPackInfo();
                }

                DrawIconList();
            }
            else
            {
                EditorGUILayout.HelpBox("No VectorImage fonts detected!", MessageType.Warning);
            }

            DrawBottomBar();
        }

        private void DrawBottomBar()
        {
            Rect bottomBarRect = new Rect(0f, base.position.height - 17f, base.position.width, 17f);
            GUI.Label(bottomBarRect, GUIContent.none, m_BottomBarBg);

            Rect sliderRect = new Rect(bottomBarRect.x + bottomBarRect.width - 55f - 16f, bottomBarRect.y + bottomBarRect.height - 17f, 55f, 17f);

            EditorGUI.BeginChangeCheck();
            m_PreviewSize = (int)GUI.HorizontalSlider(sliderRect, m_PreviewSize, 15, 100);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt("ICON_CONFIG_PREVIEW_SIZE", m_PreviewSize);
            }
        }

        private void DrawSearchTextField()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUI.SetNextControlName("SearchInputField");
                EditorGUI.BeginChangeCheck();
                m_SearchText = EditorGUILayout.TextField("", m_SearchText, "SearchTextField");
                if (EditorGUI.EndChangeCheck())
                {
                    m_SearchText = m_SearchText.Trim();
                    UpdateGlyphList();
                }

                if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
                {
                    m_SearchText = String.Empty;
                    UpdateGlyphList();
                    GUI.FocusControl(null);
                }
            }

            GUILayout.Space(5f);
        }

        private void DrawIconList()
        {
            if (m_GlyphArray.Length == 0)
            {
                GUIStyle guiStyle = new GUIStyle();
                guiStyle.fontStyle = FontStyle.Bold;
                guiStyle.alignment = TextAnchor.MiddleCenter;

                EditorGUILayout.LabelField("No icon found for your search term: " + m_SearchText, guiStyle, GUILayout.Height(Screen.height - 80f));
                return;
            }

            float padded = m_PreviewSize + 5f;
            int columns = Mathf.FloorToInt((Screen.width - 25f) / padded);
            if (columns < 1) columns = 1;

            int offset = 0;
            Rect rect = new Rect(0f, 0, m_PreviewSize, m_PreviewSize);

            GUILayout.Space(5f);

            using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(m_IconViewScrollPosition, GUILayout.Height(Screen.height - 80f)))
            {
                m_IconViewScrollPosition = scrollViewScope.scrollPosition;

                while (offset < m_GlyphArray.Length)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        int col = 0;
                        rect.x = 0f;

                        for (; offset < m_GlyphArray.Length; ++offset)
                        {
                            // Change color of the selected VectorImage
                            if (m_VectorImageDatas[0].glyph.name == m_GlyphArray[offset].name)
                            {
                                GUI.backgroundColor = MaterialColor.iconDark;
                            }

                            if (GUI.Button(rect, new GUIContent("", m_GlyphArray[offset].name)))
                            {
                                if (Event.current.button == 0)
                                {
                                    SetGlyph(offset);

                                    if (Time.realtimeSinceStartup - m_LastClickTime < 0.3f)
                                    {
                                        Close();
                                    }

                                    m_LastClickTime = Time.realtimeSinceStartup;
                                }
                            }

                            if (Event.current.type == EventType.Repaint)
                            {
                                drawTiledTexture(rect);

                                m_GuiStyle.fontSize = m_PreviewSize;

                                string iconText = IconDecoder.Decode(@"\u" + m_GlyphArray[offset].unicode);
                                Vector2 size = m_GuiStyle.CalcSize(new GUIContent(iconText));

                                float maxSide = size.x > size.y ? size.x : size.y;
                                float scaleFactor = (m_PreviewSize / maxSide) * 0.9f;

                                m_GuiStyle.fontSize = Mathf.RoundToInt(m_PreviewSize * scaleFactor);
                                size *= scaleFactor;

                                Vector2 padding = new Vector2(rect.width - size.x, rect.height - size.y);
                                Rect iconRect = new Rect(rect.x + (padding.x / 2f), rect.y + (padding.y / 2f), rect.width - padding.x, rect.height - padding.y);

                                GUI.Label(iconRect, new GUIContent(iconText), m_GuiStyle);
                            }

                            GUI.backgroundColor = Color.white;

                            if (++col >= columns)
                            {
                                ++offset;
                                break;
                            }
                            rect.x += padded;
                        }
                    }
                    GUILayout.Space(padded);
                    rect.y += padded;
                }
            }
        }

        private void drawTiledTexture(Rect rect)
        {
            createCheckerTexture();

            GUI.BeginGroup(rect);
            {
                int width = Mathf.RoundToInt(rect.width);
                int height = Mathf.RoundToInt(rect.height);

                for (int y = 0; y < height; y += m_BackdropTexture.height)
                {
                    for (int x = 0; x < width; x += m_BackdropTexture.width)
                    {
                        GUI.DrawTexture(new Rect(x, y, m_BackdropTexture.width, m_BackdropTexture.height), m_BackdropTexture);
                    }
                }
            }
            GUI.EndGroup();
        }

        private static void createCheckerTexture()
        {
            if (m_BackdropTexture != null)
            {
                return;
            }

            Color c0 = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            Color c1 = new Color(0.2f, 0.2f, 0.2f, 0.5f);

            m_BackdropTexture = new Texture2D(16, 16);
            m_BackdropTexture.name = "[Generated] Checker Texture";
            m_BackdropTexture.hideFlags = HideFlags.DontSave;

            for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) m_BackdropTexture.SetPixel(x, y, c1);
            for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) m_BackdropTexture.SetPixel(x, y, c0);
            for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) m_BackdropTexture.SetPixel(x, y, c0);
            for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) m_BackdropTexture.SetPixel(x, y, c1);

            m_BackdropTexture.Apply();
            m_BackdropTexture.filterMode = FilterMode.Point;
        }

        private void UpdateFontPackInfo()
        {
            string name = m_VectorImageDatas[0].font.name;
            m_VectorImageSet = VectorImageManager.GetIconSet(name);
            m_IconFont = VectorImageManager.GetIconFont(name);
            m_GuiStyle = new GUIStyle { font = m_IconFont };
            m_GuiStyle.normal.textColor = Color.white;

            UpdateGlyphList();

            // Assign the very first icon of the imageSet if the glyph is null
            Glyph glyph = m_VectorImageSet.iconGlyphList.Where(x => x.name.Equals(m_VectorImageDatas[0].glyph.name) && x.unicode.Equals(m_VectorImageDatas[0].glyph.unicode.Replace("\\u", ""))).FirstOrDefault();
            if (glyph == null)
            {
                SetGlyph(0);
            }
        }

        private void UpdateGlyphList()
        {
            if (string.IsNullOrEmpty(m_SearchText))
            {
                m_GlyphArray = m_VectorImageSet.iconGlyphList.ToArray();
            }
            else
            {
                m_GlyphArray = m_VectorImageSet.iconGlyphList.Where(x => x.name.Contains(m_SearchText)).ToArray();
            }
        }

        private void SetGlyph(int index)
        {
            if (m_VectorImageDatas != null)
            {
                if (m_ObjectsToRefresh != null)
                {
                    Undo.RecordObjects(m_ObjectsToRefresh, "Set Icon");
                }

                Glyph glyph = new Glyph(m_GlyphArray[index].name, m_GlyphArray[index].unicode, true);

                for (int i = 0; i < m_VectorImageDatas.Length; i++)
                {
                    m_VectorImageDatas[i].glyph = glyph;
                    m_VectorImageDatas[i].font = m_IconFont;
                }

                m_RefreshAction.InvokeIfNotNull();
            }
        }
    }
}