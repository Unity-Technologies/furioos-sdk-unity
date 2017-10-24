//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Offsets and/or resizes a RectTransform based on the position/size of the source RectTransform.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="UnityEngine.UI.ILayoutElement" />
    /// <seealso cref="UnityEngine.UI.ILayoutSelfController" />
    [AddComponentMenu("MaterialUI/Rect Transform Snap", 50)]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformSnap : MonoBehaviour, ILayoutElement, ILayoutSelfController
    {
        /// <summary>
        /// The rectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_RectTransform;
        /// <summary>
        /// Gets the rectTransform.
        /// </summary>
        public RectTransform rectTransform
        {
            get
            {
                if (!m_RectTransform)
                {
                    m_RectTransform = transform as RectTransform;
                }
                return m_RectTransform;
            }
        }

        /// <summary>
        /// The source rectTransform.
        /// </summary>
        [SerializeField]
        private RectTransform m_SourceRectTransform;
        /// <summary>
        /// Gets or sets the source rectTransform.
        /// </summary>
        public RectTransform sourceRectTransform
        {
            get { return m_SourceRectTransform; }
            set
            {
                m_SourceRectTransform = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// The padding to add to the source size to apply to the rectTransform.
        /// </summary>
        [SerializeField]
        private Vector2 m_Padding;
        /// <summary>
        /// The padding to add to the source size to apply to the rectTransform.
        /// </summary>
        public Vector2 padding
        {
            get { return m_Padding; }
            set
            {
                m_Padding = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// The offset to add to the source position to apply to the rectTransform.
        /// </summary>
        [SerializeField]
        private Vector2 m_Offset;
        /// <summary>
        /// The offset to add to the source position to apply to the rectTransform.
        /// </summary>
        public Vector2 offset
        {
            get { return m_Offset; }
            set
            {
                m_Offset = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// If true, offset and padding are a percentage of the source values, rather than an additive.
        /// </summary>
        [SerializeField]
        private bool m_ValuesArePercentage;
        /// <summary>
        /// If true, offset and padding are a percentage of the source values, rather than an additive.
        /// </summary>
        public bool valuesArePercentage
        {
            get { return m_ValuesArePercentage; }
            set
            {
                m_ValuesArePercentage = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// The padding percent to multiply with the source size to apply to the rectTransform.
        /// </summary>
        [SerializeField]
        private Vector2 m_PaddingPercent = new Vector2(100, 100);
        /// <summary>
        /// The padding percent to multiply with the source size to apply to the rectTransform.
        /// </summary>
        public Vector2 paddingPercent
        {
            get { return m_PaddingPercent; }
            set
            {
                m_PaddingPercent = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// The offset percent to multiply with the source position to apply to the rectTransform.
        /// </summary>
        [SerializeField]
        private Vector2 m_OffsetPercent;
        /// <summary>
        /// The offset percent to multiply with the source position to apply to the rectTransform.
        /// </summary>
        public Vector2 offsetPercent
        {
            get { return m_OffsetPercent; }
            set
            {
                m_OffsetPercent = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// Should the rectTransform be snapped every frame?
        /// Even if false, snapping will still be applied when the LayoutSystem calls it.
        /// </summary>
        [SerializeField]
        private bool m_SnapEveryFrame = true;
        /// <summary>
        /// Should the rectTransform be snapped every frame?
        /// Even if false, snapping will still be applied when the LayoutSystem calls it.
        /// </summary>
        public bool snapEveryFrame
        {
            get { return m_SnapEveryFrame; }
            set { m_SnapEveryFrame = value; }
        }

        /// <summary>
        /// Snap the rectTransform's width?
        /// </summary>
        [SerializeField]
        private bool m_SnapWidth = true;
        /// <summary>
        /// Snap the rectTransform's width?
        /// </summary>
        public bool snapWidth
        {
            get { return m_SnapWidth; }
            set
            {
                m_SnapWidth = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// Snap the rectTransform's height?
        /// </summary>
        [SerializeField]
        private bool m_SnapHeight = true;
        /// <summary>
        /// Snap the rectTransform's height?
        /// </summary>
        public bool snapHeight
        {
            get { return m_SnapHeight; }
            set
            {
                m_SnapHeight = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// Snap the rectTransform's x position?
        /// </summary>
        [SerializeField]
        private bool m_SnapPositionX = true;
        /// <summary>
        /// Snap the rectTransform's x position?
        /// </summary>
        public bool snapPositionX
        {
            get { return m_SnapPositionX; }
            set
            {
                m_SnapPositionX = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// Snap the rectTransform's y position?
        /// </summary>
        [SerializeField]
        private bool m_SnapPositionY = true;
        /// <summary>
        /// Snap the rectTransform's y position?
        /// </summary>
        public bool snapPositionY
        {
            get { return m_SnapPositionY; }
            set
            {
                m_SnapPositionY = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// The last calculated layout rect.
        /// </summary>
        private Rect m_LastRect;
        /// <summary>
        /// The calculated layout rect.
        /// </summary>
        private Rect m_LayoutRect;

        /// <summary>
        /// Tracks the values being snapped for the inspector.
        /// </summary>
        private DrivenRectTransformTracker m_Tracker = new DrivenRectTransformTracker();

        /// <summary>
        /// See MonoBehaviour.Awake.
        /// </summary>
        void Awake()
        {
            SetLayoutDirty();
        }

        /// <summary>
        /// See MonoBehaviour.OnEnable.
        /// </summary>
        void OnEnable()
        {
            SetLayoutDirty();
        }

        /// <summary>
        /// See MonoBehaviour.OnDisable.
        /// </summary>
        void OnDisable()
        {
            m_Tracker.Clear();
            SetLayoutDirty();
        }

        /// <summary>
        /// See MonoBehaviour.OnValidate.
        /// </summary>
        void OnValidate()
        {
            SetLayoutDirty();
        }

        /// <summary>
        /// See MonoBehaviour.LateUpdate.
        /// </summary>
        void LateUpdate()
        {
            if (!m_SourceRectTransform) return;

            if (m_SnapEveryFrame)
            {
                Rect rect = new Rect(m_SourceRectTransform.position, m_SourceRectTransform.GetProperSize());
                if (m_LastRect != rect)
                {
                    m_LastRect = rect;

                    CalculateLayoutInputHorizontal();
                    SetLayoutHorizontal();
                    CalculateLayoutInputVertical();
                    SetLayoutVertical();
                }
            }
        }

        /// <summary>
        /// Sets the layout as dirty.
        /// </summary>
        public void SetLayoutDirty()
        {
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        /// <summary>
        /// Calculates the width and/or x position.
        /// </summary>
        public void CalculateLayoutInputHorizontal()
        {
            if (!m_SourceRectTransform) return;

            if (m_SnapPositionX)
            {
                float sourcePosX = m_SourceRectTransform.position.x;

                Vector2 tempVector2 = m_LayoutRect.position;

                if (m_ValuesArePercentage)
                {
                    tempVector2.x = sourcePosX * m_OffsetPercent.x * 0.01f;
                }
                else
                {
                    tempVector2.x = sourcePosX + m_Offset.x;
                }

                m_LayoutRect.position = tempVector2;
            }

            if (m_SnapWidth)
            {
                float sourceWidth = m_SourceRectTransform.GetProperSize().x;

                Rect tempRect = m_LayoutRect;

                if (m_ValuesArePercentage)
                {
                    tempRect.width = sourceWidth * m_PaddingPercent.x * 0.01f;
                }
                else
                {
                    tempRect.width = sourceWidth + m_Padding.x;
                }

                m_LayoutRect = tempRect;
            }
        }

        /// <summary>
        /// Calculates the height and/or y position.
        /// </summary>
        public void CalculateLayoutInputVertical()
        {
            if (!m_SourceRectTransform) return;

            if (m_SnapPositionY)
            {
                float sourcePosY = m_SourceRectTransform.position.y;

                Vector2 tempVector2 = m_LayoutRect.position;

                if (m_ValuesArePercentage)
                {
                    tempVector2.y = sourcePosY * m_OffsetPercent.y * 0.01f;
                }
                else
                {
                    tempVector2.y = sourcePosY + m_Offset.y;
                }

                m_LayoutRect.position = tempVector2;
            }

            if (m_SnapHeight)
            {
                float sourceHeight = m_SourceRectTransform.GetProperSize().y;

                Rect tempRect = m_LayoutRect;

                if (m_ValuesArePercentage)
                {
                    tempRect.height = sourceHeight * m_PaddingPercent.y * 0.01f;
                }
                else
                {
                    tempRect.height = sourceHeight + m_Padding.y;
                }

                m_LayoutRect = tempRect;
            }
        }

        /// <summary>
        /// Sets the horizontal layout.
        /// </summary>
        public void SetLayoutHorizontal()
        {
            m_Tracker.Clear();

            if (!m_SourceRectTransform) return;

            if (m_SnapPositionX)
            {
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX);

                Vector3 tempVector3 = rectTransform.position;
                tempVector3.x = m_LayoutRect.position.x;
                rectTransform.position = tempVector3;
            }

            if (m_SnapWidth)
            {
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_LayoutRect.width);
            }
        }

        /// <summary>
        /// Sets the vertical layout.
        /// </summary>
        public void SetLayoutVertical()
        {
            if (!m_SourceRectTransform) return;

            if (m_SnapPositionY)
            {
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionY);

                Vector3 tempVector3 = rectTransform.position;
                tempVector3.y = m_LayoutRect.position.y;
                rectTransform.position = tempVector3;
            }

            if (m_SnapHeight)
            {
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_LayoutRect.height);
            }
        }

        /// <summary>
        /// The minimum width this layout element may be allocated.
        /// </summary>
        public float minWidth { get { return -1; } }
        /// <summary>
        /// The preferred width this layout element should be allocated if there is sufficient space.
        /// </summary>
        public float preferredWidth { get { return m_LayoutRect.width; } }
        /// <summary>
        /// The extra relative width this layout element should be allocated if there is additional available space.
        /// </summary>
        public float flexibleWidth { get { return -1; } }
        /// <summary>
        /// The minimum height this layout element may be allocated.
        /// </summary>
        public float minHeight { get { return -1; } }
        /// <summary>
        /// The preferred height this layout element should be allocated if there is sufficient space.
        /// </summary>
        public float preferredHeight { get { return m_LayoutRect.height; } }
        /// <summary>
        /// The extra relative height this layout element should be allocated if there is additional available space.
        /// </summary>
        public float flexibleHeight { get { return -1; } }
        /// <summary>
        /// The layout priority of this component.
        /// </summary>
        public int layoutPriority { get { return 0; } }
    }
}