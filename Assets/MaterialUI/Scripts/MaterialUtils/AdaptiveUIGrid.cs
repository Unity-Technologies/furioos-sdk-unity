//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;

namespace MaterialUI
{
    /// <summary>
    /// Helper component that disaplays layout keylines in the scene and game views.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [ExecuteInEditMode]
    [AddComponentMenu("MaterialUI/Layout/Adaptive UI Grid")]
    public class AdaptiveUIGrid : MonoBehaviour
    {
        /// <summary>
        /// The gutter color.
        /// </summary>
        [SerializeField]
        [Observe("GenerateGutterTexture")]
        private Color m_GutterColor = new Color(0.588f, 0.706f, 1.0f, 0.5f);

        /// <summary>
        /// The gutter width.
        /// </summary>
        [SerializeField]
        [Popup(8f, 16f, 24f, 40f)]
        private float m_GutterWidth = 24f;

        /// <summary>
        /// The margin width.
        /// </summary>
        [SerializeField]
        [Popup(8f, 16f, 24f, 40f)]
        private float m_MarginWidth = 24f;

        /// <summary>
        /// The number of colums.
        /// </summary>
        [SerializeField]
        [Popup(1, 2, 3, 4, 6, 12)]
        private int m_NumberOfColums = 12;

        /// <summary>
        /// The full screen height.
        /// </summary>
        [SerializeField]
        private bool m_FullScreenHeight;

        /// <summary>
        /// The gutter texture.
        /// </summary>
        private Texture2D m_GutterTexture;

        /// <summary>
        /// The rect transform.
        /// </summary>
        private RectTransform m_RectTransform;
        /// <summary>
        /// The canvas.
        /// </summary>
        private Canvas m_Canvas;
        /// <summary>
        /// The rect.
        /// </summary>
        private Rect m_Rect;

        /// <summary>
        /// See MonoBehaviour.OnEnable.
        /// </summary>
        void OnEnable()
        {
            GenerateGutterTexture();
            m_RectTransform = GetComponent<RectTransform>();
            m_Canvas = GetComponentInParent<Canvas>();
        }

        /// <summary>
        /// Generates the gutter texture.
        /// </summary>
        private void GenerateGutterTexture()
        {
            if (m_GutterTexture == null)
            {
                m_GutterTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                m_GutterTexture.hideFlags = HideFlags.HideAndDontSave;
            }

            m_GutterTexture.SetPixel(0, 0, m_GutterColor);
            m_GutterTexture.Apply();
        }

        /// <summary>
        /// Called when [draw gizmos].
        /// </summary>
        void OnDrawGizmos()
        {
            if (!enabled) return;

            DrawGrid(true);
        }

        /// <summary>
        /// Called when [GUI].
        /// </summary>
        void OnGUI()
        {
            if (Event.current.type.Equals(EventType.Repaint))
            {
                if (m_RectTransform)
                {
                    Vector2 componentSize = m_RectTransform.GetProperSize();

                    Vector3[] worldCorners = new Vector3[4];
                    m_RectTransform.GetWorldCorners(worldCorners);

                    m_Rect.x = worldCorners[1].x;
                    m_Rect.width = componentSize.x;

                    if (m_FullScreenHeight)
                    {
                        m_Rect.y = 0;
                        m_Rect.height = Screen.height;
                    }
                    else
                    {
                        m_Rect.y = worldCorners[1].y;
                        m_Rect.y = -m_Rect.y;
                        m_Rect.y += Screen.height;
                        m_Rect.height = componentSize.y;
                    }

                    if (m_Canvas)
                    {
                        if (m_Canvas.pixelPerfect)
                        {
                            m_Rect.x = Mathf.Ceil(m_Rect.x);
                            m_Rect.y = Mathf.Floor(m_Rect.y);
                            m_Rect.width = Mathf.Ceil(m_Rect.width);
                            m_Rect.height = Mathf.Floor(m_Rect.height);
                        }
                    }
                }
                else
                {
                    enabled = false;
                    return;
                }

                DrawGrid(false);
            }
        }

        /// <summary>
        /// Draws the grid.
        /// </summary>
        /// <param name="useGizmos">if set to <c>true</c> [use gizmos].</param>
        private void DrawGrid(bool useGizmos)
        {
            // Margins
            DrawTexture(new Rect(m_Rect.x, m_Rect.y, m_MarginWidth, m_Rect.height), useGizmos);
            DrawTexture(new Rect(m_Rect.x + (m_Rect.width - m_MarginWidth), m_Rect.y, m_MarginWidth, m_Rect.height), useGizmos);

            // Gutters
            float width = m_Rect.width - m_MarginWidth * 2.0f;
            float totalContentWidth = width - (m_NumberOfColums - 1) * m_GutterWidth;
            float columnWidth = totalContentWidth / m_NumberOfColums;

            float posX = m_Rect.x + m_MarginWidth;

            for (int i = 0; i < m_NumberOfColums - 1; i++)
            {
                posX += columnWidth;
                DrawTexture(new Rect(posX, m_Rect.y, m_GutterWidth, m_Rect.height), useGizmos);
                posX += m_GutterWidth;
            }
        }

        /// <summary>
        /// Draws the texture.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="useGizmos">if set to <c>true</c> [use gizmos].</param>
        private void DrawTexture(Rect rect, bool useGizmos)
        {
            if (useGizmos)
            {
                Gizmos.DrawGUITexture(rect, m_GutterTexture);
            }
            else
            {
                Graphics.DrawTexture(rect, m_GutterTexture);
            }
        }
    }
}
