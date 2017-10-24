//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [ExecuteInEditMode]
    [AddComponentMenu("MaterialUI/Progress/Circular Progress Indicator")]
    public class CircularProgressIndicator : ProgressIndicator
    {
        [SerializeField]
        private RectTransform m_CircleRectTransform;
        public RectTransform circleRectTransform
        {
            get { return m_CircleRectTransform; }
            set { m_CircleRectTransform = value; }
        }

        private Image m_CircleImage;
        public Image circleImage
        {
            get
            {
                if (m_CircleImage == null)
                {
                    if (m_CircleRectTransform != null)
                    {
                        m_CircleImage = m_CircleRectTransform.GetComponent<Image>();
                    }
                }
                return m_CircleImage;
            }
        }

        private VectorImage m_CircleIcon;
        public VectorImage circleIcon
        {
            get
            {
                if (m_CircleIcon == null)
                {
                    if (circleImage != null)
                    {
                        m_CircleIcon = circleImage.GetComponentInChildren<VectorImage>();
                    }
                }
                return m_CircleIcon;
            }
        }

        private const float animDuration = 0.65f;

        private int m_AnimCircle;
        private float m_AnimCircleStartTime;
        private float m_AnimCircleCurrentFillAmount;
        private float m_AnimCircleCurrentRotation;

        private bool m_AnimColor;
        private float m_AnimColorStartTime;
        private Color m_AnimColorCurrentColor;
        private Color m_AnimColorTargetColor;

        private bool m_AnimSize;
        private float m_AnimSizeStartTime;
        private float m_AnimSizeCurrentSize;
        private float m_AnimSizeTargetSize;

        void Start()
        {
            if (!Application.isPlaying) return;

            if (m_StartsHidden)
            {
                scaledRectTransform.localScale = new Vector3(0f, 0f, 1f);
            }
            else if (m_StartsIndeterminate)
            {
                StartIndeterminate();
            }
        }

        void Update()
        {
            UpdateAnimCircle();
            UpdateAnimColor();
            UpdateAnimSize();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                SetProgress(m_CurrentProgress);
            }
#endif
        }

        private void UpdateAnimCircle()
        {
            if (circleImage == null) return;
            if (m_AnimCircle == 0) return;

            if (m_AnimCircle == 1)
            {
                float animDeltaTime = Time.realtimeSinceStartup - m_AnimCircleStartTime;

                if (animDeltaTime < animDuration)
                {
                    circleImage.fillAmount = Tween.CubeInOut(m_AnimCircleCurrentFillAmount, 0.75f, animDeltaTime,
                                                                            animDuration);
                }
                else
                {
                    m_AnimCircleCurrentFillAmount = 0.75f;
                    circleImage.fillAmount = 0.75f;
                    FlipCircle(false);
                    m_AnimCircleStartTime = Time.realtimeSinceStartup;
                    m_AnimCircle = 2;
                }

                m_CircleRectTransform.localEulerAngles = new Vector3(m_CircleRectTransform.localEulerAngles.x,
                                                                               m_CircleRectTransform.localEulerAngles.y,
                                                                               m_CircleRectTransform.localEulerAngles.z - Time.unscaledDeltaTime * 200f);
                return;
            }

            if (m_AnimCircle == 2)
            {
                float animDeltaTime = Time.realtimeSinceStartup - m_AnimCircleStartTime;

                if (animDeltaTime < animDuration)
                {
                    circleImage.fillAmount = Tween.CubeInOut(m_AnimCircleCurrentFillAmount, 0.1f, animDeltaTime,
                                                                            animDuration);
                }
                else
                {
                    m_AnimCircleCurrentFillAmount = 0.1f;
                    circleImage.fillAmount = 0.1f;
                    FlipCircle(true);
                    m_AnimCircleStartTime = Time.realtimeSinceStartup;
                    m_AnimCircle = 1;
                }

                m_CircleRectTransform.localEulerAngles = new Vector3(m_CircleRectTransform.localEulerAngles.x,
                                                                               m_CircleRectTransform.localEulerAngles.y,
                                                                               m_CircleRectTransform.localEulerAngles.z - Time.unscaledDeltaTime * 200f);
                return;
            }

            if (m_AnimCircle == 3)
            {
                float animDeltaTime = Time.realtimeSinceStartup - m_AnimCircleStartTime;

                if (animDeltaTime < animDuration)
                {
                    circleImage.fillAmount = Tween.CubeInOut(m_AnimCircleCurrentFillAmount, m_CurrentProgress, animDeltaTime,
                                                                            animDuration);
                    Vector3 tempVector3 = m_CircleRectTransform.localEulerAngles;
                    tempVector3.z = Tween.CubeInOut(m_AnimCircleCurrentRotation, 0f, animDeltaTime, animDuration);
                    m_CircleRectTransform.localEulerAngles = tempVector3;
                }
                else
                {
                    m_AnimCircleCurrentFillAmount = circleImage.fillAmount = m_CurrentProgress;
                    Vector3 tempVector3 = m_CircleRectTransform.localEulerAngles;
                    tempVector3.z = 0f;
                    m_CircleRectTransform.localEulerAngles = tempVector3;
                    m_AnimCircleStartTime = Time.realtimeSinceStartup;
                    m_AnimCircle = 0;
                }
            }
        }

        private void UpdateAnimColor()
        {
            if (!m_AnimColor) return;

            float animDeltaTime = Time.realtimeSinceStartup - m_AnimColorStartTime;

            if (animDeltaTime < animDuration)
            {
                circleIcon.color = Tween.CubeInOut(m_AnimColorCurrentColor, m_AnimColorTargetColor, animDeltaTime,
                                                                animDuration);
            }
            else
            {
                circleIcon.color = m_AnimColorTargetColor;
                m_AnimColor = false;
            }
        }

        private void UpdateAnimSize()
        {
            if (!m_AnimSize) return;

            float animDeltaTime = Time.realtimeSinceStartup - m_AnimSizeStartTime;

            if (animDeltaTime < animDuration)
            {
                Vector3 tempVector3 = scaledRectTransform.localScale;
                tempVector3.x = Tween.CubeInOut(m_AnimSizeCurrentSize, m_AnimSizeTargetSize, animDeltaTime, animDuration);
                tempVector3.y = tempVector3.x;
                tempVector3.z = tempVector3.x;
                scaledRectTransform.localScale = tempVector3;
            }
            else
            {
                Vector3 tempVector3 = scaledRectTransform.localScale;
                tempVector3.x = m_AnimSizeTargetSize;
                tempVector3.y = tempVector3.x;
                tempVector3.z = tempVector3.x;
                scaledRectTransform.localScale = tempVector3;
                m_AnimSize = false;
                if (m_AnimSizeTargetSize == 0f)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public override void Show(bool startIndeterminate = true)
        {
            if (scaledRectTransform == null) return;

            gameObject.SetActive(true);
            m_AnimSizeCurrentSize = scaledRectTransform.localScale.x;
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

            m_AnimSizeCurrentSize = scaledRectTransform.localScale.x;
            m_AnimSizeTargetSize = 0f;
            m_AnimSizeStartTime = Time.realtimeSinceStartup;
            m_AnimSize = true;
        }

        public override void StartIndeterminate()
        {
            FlipCircle(true);
            SetAnimCurrents();
            m_IsAnimatingIndeterminate = true;
            m_AnimCircle = 1;

            Show();
        }

        public override void SetProgress(float progress, bool animated = true)
        {
            if (circleImage == null) return;
            if (circleRectTransform == null) return;

            progress = Mathf.Clamp(progress, 0f, 1f);

            if (!animated)
            {
                FlipCircle(true);
                m_CurrentProgress = progress;
                m_IsAnimatingIndeterminate = false;
                circleImage.fillAmount = m_CurrentProgress;
                Vector3 tempVector3 = m_CircleRectTransform.localEulerAngles;
                tempVector3.z = 0f;
                m_CircleRectTransform.localEulerAngles = tempVector3;
                m_AnimCircle = 0;
            }
            else
            {
                FlipCircle(true);
                SetAnimCurrents();
                m_CurrentProgress = progress;
                m_IsAnimatingIndeterminate = false;
                m_AnimCircle = 3;
            }
        }

        public override void SetColor(Color color)
        {
            m_AnimColorCurrentColor = circleIcon.color;
            m_AnimColorTargetColor = color;
            m_AnimColorStartTime = Time.realtimeSinceStartup;
            m_AnimColor = true;
        }

        private void SetAnimCurrents()
        {
            if (circleImage == null) return;

            m_AnimCircleCurrentRotation = m_CircleRectTransform.localEulerAngles.z;
            m_AnimCircleCurrentFillAmount = circleImage.fillAmount;
            m_AnimCircleStartTime = Time.realtimeSinceStartup;
        }

        private void FlipCircle(bool clockwise)
        {
            if (circleImage == null) return;

            if (!circleImage.fillClockwise && clockwise)
            {
                m_CircleRectTransform.localEulerAngles = new Vector3(m_CircleRectTransform.localEulerAngles.x,
                                                                               m_CircleRectTransform.localEulerAngles.y,
                                                                               m_CircleRectTransform.localEulerAngles.z + (360f * circleImage.fillAmount));
                circleImage.fillClockwise = true;
            }
            else if (circleImage.fillClockwise && !clockwise)
            {
                m_CircleRectTransform.localEulerAngles = new Vector3(m_CircleRectTransform.localEulerAngles.x,
                                                                               m_CircleRectTransform.localEulerAngles.y,
                                                                               m_CircleRectTransform.localEulerAngles.z - (360f * circleImage.fillAmount));
                circleImage.fillClockwise = false;
            }
        }

        public override float GetMinWidth()
        {
            return 48f;
        }

        public override float GetMinHeight()
        {
            return 48f;
        }
    }
}