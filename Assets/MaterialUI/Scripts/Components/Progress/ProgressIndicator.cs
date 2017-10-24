//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    public class ProgressIndicator : MonoBehaviour, ILayoutElement
    {
        [SerializeField]
        [Range(0f, 1f)]
        protected float m_CurrentProgress;
        public float currentProgress
        {
            get { return m_CurrentProgress; }
        }

        [SerializeField]
        private RectTransform m_BaseObjectOverride;
        public RectTransform baseObjectOverride
        {
            get { return m_BaseObjectOverride; }
            set { m_BaseObjectOverride = value; }
        }

        protected RectTransform scaledRectTransform
        {
            get { return m_BaseObjectOverride != null ? m_BaseObjectOverride : rectTransform; }
        }

        private RectTransform m_RectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if (m_RectTransform == null)
                {
                    m_RectTransform = (RectTransform)transform;
                }
                return m_RectTransform;
            }
        }

        [SerializeField]
        protected bool m_StartsIndeterminate;

        [SerializeField]
        protected bool m_StartsHidden;

        protected bool m_IsAnimatingIndeterminate;

        public virtual void Show(bool startIndeterminate = true) { }
        public virtual void Hide() { }
        public virtual void StartIndeterminate() { }
        public virtual void SetProgress(float progress, bool animated = true) { }
        public virtual void SetColor(Color color) { }

        public virtual float GetMinWidth() { return -1; }
        public virtual float GetMinHeight() { return -1; }

        public void CalculateLayoutInputHorizontal() { }
        public void CalculateLayoutInputVertical() { }
        public float preferredWidth { get { return -1; } }
        public float minWidth { get { return GetMinWidth(); } }
        public float flexibleWidth { get { return -1; } }
        public float preferredHeight { get { return -1; } }
        public float minHeight { get { return GetMinHeight(); } }
        public float flexibleHeight { get { return -1; } }
        public int layoutPriority { get { return -1; } }
    }
}