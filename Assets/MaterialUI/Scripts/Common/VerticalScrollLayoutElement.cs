//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Changes the state of a ScrollRect, depending on whether its content RectTransform is over a specified height.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="UnityEngine.UI.ILayoutElement" />
    [AddComponentMenu("MaterialUI/Vertical Scroll Layout Element", 50)]
    public class VerticalScrollLayoutElement : MonoBehaviour, ILayoutElement
    {
        /// <summary>
        /// The maximun height the content can be before the ScrollRect is activated.
        /// </summary>
        [SerializeField]
        private float m_MaxHeight;
        /// <summary>
        /// The maximun height the content can be before the ScrollRect is activated.
        /// Calls <see cref="RefreshLayout"/> if set.
        /// </summary>
        public float maxHeight
        {
            get { return m_MaxHeight; }
            set
            {
                m_MaxHeight = value;
                RefreshLayout();
            }
        }

        /// <summary>
        /// The content RectTransform.
        /// </summary>
        [SerializeField]
        private RectTransform m_ContentRectTransform;
        /// <summary>
        /// The content RectTransform.
        /// Calls <see cref="RefreshLayout"/> if set.
        /// </summary>
        public RectTransform contentRectTransform
        {
            get { return m_ContentRectTransform; }
            set
            {
                m_ContentRectTransform = value;
                RefreshLayout();
            }
        }

        /// <summary>
        /// The target ScrollRect.
        /// </summary>
        [SerializeField]
        private ScrollRect m_ScrollRect;
        /// <summary>
        /// The target ScrollRect.
        /// Calls <see cref="RefreshLayout"/> if set.
        /// </summary>
        public ScrollRect scrollRect
        {
            get { return m_ScrollRect; }
            set
            {
                m_ScrollRect = value;
                RefreshLayout();
            }
        }

        /// <summary>
        /// The target ScrollRect's transform.
        /// </summary>
        private RectTransform m_ScrollRectTransform;
        /// <summary>
        /// The target ScrollRect's transform.
        /// Calls <see cref="RefreshLayout"/> if set.
        /// </summary>
        public RectTransform scrollRectTransform
        {
            get { return m_ScrollRectTransform; }
            set
            {
                m_ScrollRectTransform = value;
                RefreshLayout();
            }
        }

        /// <summary>
        /// The scrollbar handle image.
        /// </summary>
        [SerializeField]
        private Image m_ScrollHandleImage;
        /// <summary>
        /// The scrollbar handle image.
        /// Calls <see cref="RefreshLayout"/> if set.
        /// </summary>
        public Image scrollHandleImage
        {
            get { return m_ScrollHandleImage; }
            set
            {
                m_ScrollHandleImage = value;
                RefreshLayout();
            }
        }

        /// <summary>
        /// The ScrollRect movement type when scrollable.
        /// </summary>
        [SerializeField]
        private ScrollRect.MovementType m_MovementTypeWhenScrollable;
        /// <summary>
        /// The ScrollRect movement type when scrollable.
        /// Calls <see cref="RefreshLayout"/> if set.
        /// </summary>
        public ScrollRect.MovementType movementTypeWhenScrollable
        {
            get { return m_MovementTypeWhenScrollable; }
            set
            {
                m_MovementTypeWhenScrollable = value;
                RefreshLayout();
            }
        }

        /// <summary>
        /// Optional other Images to show if ScrollRect is scrollable.
        /// </summary>
        [SerializeField]
        private Image[] m_ShowWhenScrollable;

        /// <summary>
        /// Is the ScrollRect scrollable?
        /// </summary>
        private bool m_ScrollEnabled;
        /// <summary>
        /// Is the ScrollRect scrollable?
        /// </summary>
        public bool scrollEnabled
        {
            get { return m_ScrollEnabled; }
        }

        /// <summary>
        /// The layout height of the ScrollRect.
        /// </summary>
        private float m_Height;

        /// <summary>
        /// Calculates if the ScrollRect should be scrollable based on the content height and sets the ScrollRect height.
        /// </summary>
        private void RefreshLayout()
        {
            if (!m_ScrollRect)
            {
                m_ScrollRect = GetComponent<ScrollRect>();
            }
            if (!m_ScrollRectTransform)
            {
                m_ScrollRectTransform = m_ScrollRect.GetComponent<RectTransform>();
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);

            float tempHeight = LayoutUtility.GetPreferredHeight(contentRectTransform);

            if (tempHeight > m_MaxHeight)
            {
                m_Height = maxHeight;
                m_ScrollRect.movementType = movementTypeWhenScrollable;
                m_ScrollHandleImage.enabled = true;

                m_ScrollEnabled = true;

                for (int i = 0; i < m_ShowWhenScrollable.Length; i++)
                {
                    m_ShowWhenScrollable[i].enabled = true;
                }
            }
            else
            {
                m_Height = tempHeight;
                m_ScrollRect.movementType = ScrollRect.MovementType.Clamped;
                m_ScrollHandleImage.enabled = false;

                m_ScrollEnabled = false;

                for (int i = 0; i < m_ShowWhenScrollable.Length; i++)
                {
                    m_ShowWhenScrollable[i].enabled = false;
                }
            }

            m_ScrollRectTransform.sizeDelta = new Vector2(m_ScrollRectTransform.sizeDelta.x, m_Height);
        }

        /// <summary>
        /// The minWidth, preferredWidth, and flexibleWidth values may be calculated in this callback.
        /// </summary>
        public void CalculateLayoutInputHorizontal() { }

        /// <summary>
        /// The minHeight, preferredHeight, and flexibleHeight values may be calculated in this callback.
        /// </summary>
        public void CalculateLayoutInputVertical()
        {
            RefreshLayout();
        }

        /// <summary>
        /// The minimum width this layout element may be allocated.
        /// </summary>
        public float minWidth { get { return -1; } }
        /// <summary>
        /// The preferred width this layout element should be allocated if there is sufficient space.
        /// </summary>
        public float preferredWidth { get { return -1; } }
        /// <summary>
        /// The extra relative width this layout element should be allocated if there is additional available space.
        /// </summary>
        public float flexibleWidth { get { return -1; } }
        /// <summary>
        /// The minimum height this layout element may be allocated.
        /// </summary>
        public float minHeight { get { return m_Height; } }
        /// <summary>
        /// The preferred height this layout element should be allocated if there is sufficient space.
        /// </summary>
        public float preferredHeight { get { return m_Height; } }
        /// <summary>
        /// The extra relative height this layout element should be allocated if there is additional available space.
        /// </summary>
        public float flexibleHeight { get { return m_Height; } }
        /// <summary>
        /// The layout priority of this component.
        /// </summary>
        public int layoutPriority { get { return 0; } }
    }
}
