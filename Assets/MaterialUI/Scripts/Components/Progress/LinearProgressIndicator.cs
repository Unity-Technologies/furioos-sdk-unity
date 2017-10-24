//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [ExecuteInEditMode]
    [AddComponentMenu("MaterialUI/Progress/Linear Progress Indicator")]
    public class LinearProgressIndicator : ProgressIndicator
    {
        [SerializeField]
        private RectTransform m_BarRectTransform;
		public RectTransform barRectTransform
		{
			get { return m_BarRectTransform; }
			set { m_BarRectTransform = value; }
		}

        private Image m_BarImage;
		public Image barImage
		{
			get
			{
				if (m_BarImage == null)
				{
					if (m_BarRectTransform != null)
					{
						m_BarImage = m_BarRectTransform.GetComponent<Image>();
					}
				}
				return m_BarImage;
			}
		}

        [SerializeField]
        private Image m_BackgroundImage;
		public Image backgroundImage
		{
			get { return m_BackgroundImage; }
			set { m_BackgroundImage = value; }
		}

		private const float animDuration = 0.65f;

        private int m_AnimBarPos;
        private float m_AnimBarPosStartTime;
        private float m_AnimBarPosCurrentPos;

        private bool m_AnimColor;
        private float m_AnimColorStartTime;
        private Color m_AnimColorCurrentColor;
        private Color m_AnimColorTargetColor;

        private bool m_AnimSize;
        private float m_AnimSizeStartTime;
        private float m_AnimSizeCurrentSize;
        private float m_AnimSizeTargetSize;

        private int m_AnimBar;
        private float m_AnimBarStartTime;
        private float m_AnimBarCurrentSize;
        private float m_AnimBarTargetSize;
        private float m_AnimBarCurrentPos;

        private float m_BarLength;

		void Start()
		{
            if (!Application.isPlaying) return;

            if (m_StartsHidden)
            {
                scaledRectTransform.localScale = new Vector3(1f, 0f, 1f);
            }
            else if (m_StartsIndeterminate)
            {
                StartIndeterminate();
            }
        }

        private void Update()
        {
            UpdateAnimSize();
            UpdateAnimColor();
            UpdateAnimBar();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                SetProgress(m_CurrentProgress);
            }
#endif
        }

        private void UpdateAnimSize()
        {
            if (scaledRectTransform == null) return;
            if (!m_AnimSize) return;

            float deltaTime = Time.realtimeSinceStartup - m_AnimSizeStartTime;

            if (deltaTime < animDuration)
            {
                Vector3 tempVector3 = scaledRectTransform.localScale;
                tempVector3.y = Tween.CubeInOut(m_AnimSizeCurrentSize, m_AnimSizeTargetSize, deltaTime, animDuration);
                scaledRectTransform.localScale = tempVector3;
            }
            else
            {
                Vector3 tempVector3 = scaledRectTransform.localScale;
                tempVector3.y = m_AnimSizeTargetSize;
                scaledRectTransform.localScale = tempVector3;
                m_AnimSize = false;
                if (m_AnimSizeTargetSize == 0f)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private void UpdateAnimColor()
        {
            if (m_BackgroundImage == null) return;
            if (!m_AnimColor) return;

            float deltaTime = Time.realtimeSinceStartup - m_AnimColorStartTime;

            if (deltaTime < animDuration)
            {
                barImage.color = Tween.CubeInOut(m_AnimColorCurrentColor, m_AnimColorTargetColor, deltaTime, animDuration);
                m_BackgroundImage.color = barImage.color;
            }
            else
            {
                barImage.color = m_AnimColorTargetColor;
                m_BackgroundImage.color = barImage.color;
                m_AnimColor = false;
            }
        }

        private void UpdateAnimBar()
        {
            if (rectTransform == null) return;
            if (m_AnimBar == 0) return;

            float deltaTime = Time.realtimeSinceStartup - m_AnimBarStartTime;

            m_BarLength = rectTransform.GetProperSize().x;

            if (m_AnimBar == 1)
            {
                if (deltaTime < animDuration)
                {
                    Vector2 tempVector2 = m_BarRectTransform.sizeDelta;
                    tempVector2.x = Tween.CubeIn(m_AnimBarCurrentSize, m_BarLength / 2f, deltaTime, animDuration);
                    m_BarRectTransform.sizeDelta = tempVector2;

                    tempVector2 = m_BarRectTransform.anchoredPosition;
                    tempVector2.x = Tween.CubeIn(m_AnimBarCurrentPos, m_BarLength / 4f, deltaTime, animDuration);
                    m_BarRectTransform.anchoredPosition = tempVector2;
                }
                else
                {
                    Vector2 tempVector2 = m_BarRectTransform.sizeDelta;
                    tempVector2.x = m_BarLength / 2f;
                    m_BarRectTransform.sizeDelta = tempVector2;

                    tempVector2 = m_BarRectTransform.anchoredPosition;
                    tempVector2.x = m_BarLength / 4f;
                    m_BarRectTransform.anchoredPosition = tempVector2;

                    m_AnimBarCurrentSize = m_BarRectTransform.sizeDelta.x;
                    m_AnimBarCurrentPos = m_BarRectTransform.anchoredPosition.x;
                    m_AnimBarStartTime = Time.realtimeSinceStartup;
                    m_AnimBar = 2;
                }
                return;
            }
            if (m_AnimBar == 2)
            {
                if (deltaTime < animDuration)
                {
                    Vector2 tempVector2 = m_BarRectTransform.sizeDelta;
                    tempVector2.x = Tween.CubeOut(m_AnimBarCurrentSize, 0f, deltaTime, animDuration);
                    m_BarRectTransform.sizeDelta = tempVector2;

                    tempVector2 = m_BarRectTransform.anchoredPosition;
                    tempVector2.x = Tween.CubeOut(m_AnimBarCurrentPos, m_BarLength, deltaTime, animDuration);
                    m_BarRectTransform.anchoredPosition = tempVector2;
                }
                else
                {
                    Vector2 tempVector2 = m_BarRectTransform.sizeDelta;
                    tempVector2.x = 0f;
                    m_BarRectTransform.sizeDelta = tempVector2;

                    tempVector2 = m_BarRectTransform.anchoredPosition;
                    tempVector2.x = 0f;
                    m_BarRectTransform.anchoredPosition = tempVector2;

                    m_AnimBarCurrentSize = m_BarRectTransform.sizeDelta.x;
                    m_AnimBarCurrentPos = m_BarRectTransform.anchoredPosition.x;
                    m_AnimBarStartTime = Time.realtimeSinceStartup;
                    m_AnimBar = 1;
                }
                return;
            }
            if (m_AnimBar == 3)
            {
                if (deltaTime < animDuration && Application.isPlaying)
                {
                    Vector2 tempVector2 = m_BarRectTransform.sizeDelta;
                    tempVector2.x = Tween.CubeInOut(m_AnimBarCurrentSize, m_AnimBarTargetSize, deltaTime, animDuration);
                    m_BarRectTransform.sizeDelta = tempVector2;

                    tempVector2 = m_BarRectTransform.anchoredPosition;
                    tempVector2.x = Tween.CubeInOut(m_AnimBarCurrentPos, 0f, deltaTime, animDuration);
                    m_BarRectTransform.anchoredPosition = tempVector2;
                }
                else
                {
                    Vector2 tempVector2 = m_BarRectTransform.sizeDelta;
                    tempVector2.x = m_AnimBarTargetSize;
                    m_BarRectTransform.sizeDelta = tempVector2;

                    tempVector2 = m_BarRectTransform.anchoredPosition;
                    tempVector2.x = 0f;
                    m_BarRectTransform.anchoredPosition = tempVector2;

                    m_AnimBar = 0;
                }
            }
        }

        public override void Show(bool startIndeterminate = true)
        {
            if (scaledRectTransform == null) return;

            gameObject.SetActive(true);
            m_AnimSizeCurrentSize = scaledRectTransform.localScale.y;
            m_AnimSizeTargetSize = 1f;
            m_AnimSizeStartTime = Time.realtimeSinceStartup;
            m_AnimSize = true;

            if (!m_IsAnimatingIndeterminate && startIndeterminate)
            {
                StartIndeterminate();
            }
        }

        public override void Hide()
        {
            if (scaledRectTransform == null) return;

            m_AnimSizeCurrentSize = scaledRectTransform.localScale.y;
            m_AnimSizeTargetSize = 0f;
            m_AnimSizeStartTime = Time.realtimeSinceStartup;
            m_AnimSize = true;
        }

        public override void StartIndeterminate()
        {
            if (barRectTransform == null) return;
            if (rectTransform == null) return;

            m_BarLength = rectTransform.GetProperSize().x;
            m_IsAnimatingIndeterminate = true;
            m_AnimBarCurrentSize = m_BarRectTransform.sizeDelta.x;
            m_AnimBarCurrentPos = m_BarRectTransform.anchoredPosition.x;
            m_AnimBarStartTime = Time.realtimeSinceStartup;
            m_AnimBar = 2;

            Show();
        }

        public override void SetProgress(float progress, bool animated = true)
        {
            if (barRectTransform == null) return;
            if (rectTransform == null) return;
            progress = Mathf.Clamp(progress, 0f, 1f);

            m_BarLength = rectTransform.GetProperSize().x;
            m_IsAnimatingIndeterminate = false;

            if (!animated)
            {
                Vector2 tempVector2 = m_BarRectTransform.sizeDelta;
                tempVector2.x = m_BarLength * progress;
                m_BarRectTransform.sizeDelta = tempVector2;

                tempVector2 = m_BarRectTransform.anchoredPosition;
                tempVector2.x = 0f;
                m_BarRectTransform.anchoredPosition = tempVector2;
                m_AnimBar = 0;
            }
            else
            {
                m_AnimBarCurrentSize = m_BarRectTransform.sizeDelta.x;
                m_AnimBarCurrentPos = m_BarRectTransform.anchoredPosition.x;
                m_AnimBarTargetSize = m_BarLength * progress;
                m_AnimBarStartTime = Time.realtimeSinceStartup;
                m_AnimBar = 3;
            }
        }

        public override void SetColor(Color color)
        {
            if (barImage == null) return;

            m_AnimColorCurrentColor = barImage.color;
            m_AnimColorTargetColor = color;
            m_AnimColorStartTime = Time.realtimeSinceStartup;
            m_AnimColor = true;
        }

        public override float GetMinWidth()
        {
            return 24f;
        }

        public override float GetMinHeight()
        {
            return 4f;
        }
    }
}