//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Material Screen", 100)]
    public class MaterialScreen : MonoBehaviour
    {
        [SerializeField]
        private bool m_OptionsControlledByScreenView = true;
        public bool optionsControlledByScreenView
        {
            get { return m_OptionsControlledByScreenView; }
            set { m_OptionsControlledByScreenView = value; }
        }

        [SerializeField]
        private bool m_DisableWhenNotVisible = true;
        public bool disableWhenNotVisible
        {
            get { return m_DisableWhenNotVisible; }
            set { m_DisableWhenNotVisible = value; }
        }

        //  Transition In
        [SerializeField]
        private bool m_FadeIn = true;
        public bool fadeIn
        {
            get { return m_FadeIn; }
            set { m_FadeIn = value; }
        }

        [SerializeField]
        private Tween.TweenType m_FadeInTweenType = Tween.TweenType.EaseOutQuint;
        public Tween.TweenType fadeInTweenType
        {
            get { return m_FadeInTweenType; }
            set { m_FadeInTweenType = value; }
        }

        [SerializeField]
        private float m_FadeInAlpha;
        public float fadeInAlpha
        {
            get { return m_FadeInAlpha; }
            set { m_FadeInAlpha = value; }
        }

        [SerializeField]
        private AnimationCurve m_FadeInCustomCurve;
        public AnimationCurve fadeInCustomCurve
        {
            get { return m_FadeInCustomCurve; }
            set { m_FadeInCustomCurve = value; }
        }

        [SerializeField]
        private bool m_ScaleIn;
        public bool scaleIn
        {
            get { return m_ScaleIn; }
            set { m_ScaleIn = value; }
        }

        [SerializeField]
        private Tween.TweenType m_ScaleInTweenType = Tween.TweenType.EaseOutQuint;
        public Tween.TweenType scaleInTweenType
        {
            get { return m_ScaleInTweenType; }
            set { m_ScaleInTweenType = value; }
        }

        [SerializeField]
        private float m_ScaleInScale;
        public float scaleInScale
        {
            get { return m_ScaleInScale; }
            set { m_ScaleInScale = value; }
        }

        [SerializeField]
        private AnimationCurve m_ScaleInCustomCurve;
        public AnimationCurve scaleInCustomCurve
        {
            get { return m_ScaleInCustomCurve; }
            set { m_ScaleInCustomCurve = value; }
        }

        [SerializeField]
        private bool m_SlideIn;
        public bool slideIn
        {
            get { return m_SlideIn; }
            set { m_SlideIn = value; }
        }

        [SerializeField]
        private Tween.TweenType m_SlideInTweenType = Tween.TweenType.EaseOutQuint;
        public Tween.TweenType slideInTweenType
        {
            get { return m_SlideInTweenType; }
            set { m_SlideInTweenType = value; }
        }

        [SerializeField]
        private ScreenView.SlideDirection m_SlideInDirection = ScreenView.SlideDirection.Right;
        public ScreenView.SlideDirection slideInDirection
        {
            get { return m_SlideInDirection; }
            set { m_SlideInDirection = value; }
        }

        [SerializeField]
        private bool m_AutoSlideInAmount = true;
        public bool autoSlideInAmount
        {
            get { return m_AutoSlideInAmount; }
            set { m_AutoSlideInAmount = value; }
        }

        [SerializeField]
        private float m_SlideInAmount;
        public float slideInAmount
        {
            get { return m_SlideInAmount; }
            set { m_SlideInAmount = value; }
        }

        [SerializeField]
        private float m_SlideInPercent = 100f;
        public float slideInPercent
        {
            get { return m_SlideInPercent; }
            set { m_SlideInPercent = value; }
        }

        [SerializeField]
        private AnimationCurve m_SlideInCustomCurve;
        public AnimationCurve slideInCustomCurve
        {
            get { return m_SlideInCustomCurve; }
            set { m_SlideInCustomCurve = value; }
        }

        [SerializeField]
        private bool m_RippleIn;
        public bool rippleIn
        {
            get { return m_RippleIn; }
            set { m_RippleIn = value; }
        }

        [SerializeField]
        private Tween.TweenType m_RippleInTweenType = Tween.TweenType.EaseOutQuint;
        public Tween.TweenType rippleInTweenType
        {
            get { return m_RippleInTweenType; }
            set { m_RippleInTweenType = value; }
        }

        [SerializeField]
        private ScreenView.RippleType m_RippleInType;
        public ScreenView.RippleType rippleInType
        {
            get { return m_RippleInType; }
            set { m_RippleInType = value; }
        }

        [SerializeField]
        private Vector2 m_RippleInPosition;
        public Vector2 rippleInPosition
        {
            get { return m_RippleInPosition; }
            set { m_RippleInPosition = value; }
        }

        [SerializeField]
        private AnimationCurve m_RippleInCustomCurve;
        public AnimationCurve rippleInCustomCurve
        {
            get { return m_RippleInCustomCurve; }
            set { m_RippleInCustomCurve = value; }
        }

        //  Transition Out
        [SerializeField]
        private bool m_FadeOut;
        public bool fadeOut
        {
            get { return m_FadeOut; }
            set { m_FadeOut = value; }
        }

        [SerializeField]
        private Tween.TweenType m_FadeOutTweenType = Tween.TweenType.EaseOutQuint;
        public Tween.TweenType fadeOutTweenType
        {
            get { return m_FadeOutTweenType; }
            set { m_FadeOutTweenType = value; }
        }

        [SerializeField]
        private float m_FadeOutAlpha;
        public float fadeOutAlpha
        {
            get { return m_FadeOutAlpha; }
            set { m_FadeOutAlpha = value; }
        }

        [SerializeField]
        private AnimationCurve m_FadeOutCustomCurve;
        public AnimationCurve fadeOutCustomCurve
        {
            get { return m_FadeOutCustomCurve; }
            set { m_FadeOutCustomCurve = value; }
        }

        [SerializeField]
        private bool m_ScaleOut;
        public bool scaleOut
        {
            get { return m_ScaleOut; }
            set { m_ScaleOut = value; }
        }

        [SerializeField]
        private Tween.TweenType m_ScaleOutTweenType = Tween.TweenType.EaseOutQuint;
        public Tween.TweenType scaleOutTweenType
        {
            get { return m_ScaleOutTweenType; }
            set { m_ScaleOutTweenType = value; }
        }

        [SerializeField]
        private float m_ScaleOutScale;
        public float scaleOutScale
        {
            get { return m_ScaleOutScale; }
            set { m_ScaleOutScale = value; }
        }

        [SerializeField]
        private AnimationCurve m_ScaleOutCustomCurve;
        public AnimationCurve scaleOutCustomCurve
        {
            get { return m_ScaleOutCustomCurve; }
            set { m_ScaleOutCustomCurve = value; }
        }

        [SerializeField]
        private bool m_SlideOut;
        public bool slideOut
        {
            get { return m_SlideOut; }
            set { m_SlideOut = value; }
        }

        [SerializeField]
        private Tween.TweenType m_SlideOutTweenType = Tween.TweenType.EaseOutQuint;
        public Tween.TweenType slideOutTweenType
        {
            get { return m_SlideOutTweenType; }
            set { m_SlideOutTweenType = value; }
        }

        [SerializeField]
        private ScreenView.SlideDirection m_SlideOutDirection = ScreenView.SlideDirection.Left;
        public ScreenView.SlideDirection slideOutDirection
        {
            get { return m_SlideOutDirection; }
            set { m_SlideOutDirection = value; }
        }

        [SerializeField]
        private bool m_AutoSlideOutAmount = true;
        public bool autoSlideOutAmount
        {
            get { return m_AutoSlideOutAmount; }
            set { m_AutoSlideOutAmount = value; }
        }

        [SerializeField]
        private float m_SlideOutAmount;
        public float slideOutAmount
        {
            get { return m_SlideOutAmount; }
            set { m_SlideOutAmount = value; }
        }

        [SerializeField]
        private float m_SlideOutPercent = 100f;
        public float slideOutPercent
        {
            get { return m_SlideOutPercent; }
            set { m_SlideOutPercent = value; }
        }

        [SerializeField]
        private AnimationCurve m_SlideOutCustomCurve;
        public AnimationCurve slideOutCustomCurve
        {
            get { return m_SlideOutCustomCurve; }
            set { m_SlideOutCustomCurve = value; }
        }

        [SerializeField]
        private bool m_RippleOut;
        public bool rippleOut
        {
            get { return m_RippleOut; }
            set { m_RippleOut = value; }
        }

        [SerializeField]
        private Tween.TweenType m_RippleOutTweenType = Tween.TweenType.EaseOutQuint;
        public Tween.TweenType rippleOutTweenType
        {
            get { return m_RippleOutTweenType; }
            set { m_RippleOutTweenType = value; }
        }

        [SerializeField]
        private ScreenView.RippleType m_RippleOutType;
        public ScreenView.RippleType rippleOutType
        {
            get { return m_RippleOutType; }
            set { m_RippleOutType = value; }
        }

        [SerializeField]
        private Vector2 m_RippleOutPosition;
        public Vector2 rippleOutPosition
        {
            get { return m_RippleOutPosition; }
            set { m_RippleOutPosition = value; }
        }

        [SerializeField]
        private AnimationCurve m_RippleOutCustomCurve;
        public AnimationCurve rippleOutCustomCurve
        {
            get { return m_RippleOutCustomCurve; }
            set { m_RippleOutCustomCurve = value; }
        }

        [SerializeField]
        private float m_TransitionDuration = 0.5f;
        public float transitionDuration
        {
            get { return m_TransitionDuration; }
            set { m_TransitionDuration = value; }
        }

        private RectTransform m_Ripple;
        private RectTransform ripple
        {
            get
            {
                if (m_Ripple == null)
                {
                    m_Ripple = new GameObject("Ripple Mask", typeof(VectorImage)).GetComponent<RectTransform>();
                    m_Ripple.GetComponent<VectorImage>().vectorImageData = MaterialUIIconHelper.GetIcon("circle").vectorImageData;
                    m_Ripple.SetParent(rectTransform.parent);
                    m_Ripple.localScale = Vector3.one;
                    m_Ripple.gameObject.AddComponent<Mask>().showMaskGraphic = false;
                    m_Ripple.sizeDelta = Vector2.zero;
                    m_Ripple.position = GetRipplePosition();
                }
                return m_Ripple;
            }
        }

        private ScreenView m_ScreenView;
        public ScreenView screenView
        {
            get
            {
                if (m_ScreenView == null)
                {
                    m_ScreenView = GetComponentInParent<ScreenView>();
                }

                return m_ScreenView;
            }
        }

        private RectTransform m_RectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if (m_RectTransform == null)
                {
                    m_RectTransform = gameObject.GetComponent<RectTransform>();
                }
                return m_RectTransform;
            }
        }

        private CanvasGroup m_CanvasGroup;
        public CanvasGroup canvasGroup
        {
            get
            {
                if (m_CanvasGroup == null)
                {
                    m_CanvasGroup = gameObject.GetAddComponent<CanvasGroup>();
                    m_CanvasGroup.blocksRaycasts = true;
                    m_CanvasGroup.interactable = true;
                    m_CanvasGroup.ignoreParentGroups = true;
                }
                return m_CanvasGroup;
            }
        }

        [SerializeField]
        private int m_ScreenIndex = -1;
        public int screenIndex
        {
            get { return m_ScreenIndex; }
            set { m_ScreenIndex = value; }
        }

        private int m_IsTransitioning = 0;
        private float m_TransitionStartTime;
        private float m_TransitionDeltaTime;

        private Vector2 m_TempRippleSize;
        private Vector3 m_TempRippleScale;
        private Vector3 m_TargetRipplePos;
        private Vector3 m_CurrentRipplePos;

        private Vector3 m_TempScreenPos;
        private Vector2 m_SlideScreenPos;

        private void CheckValues()
        {
            if (optionsControlledByScreenView)
            {
                fadeIn = screenView.fadeIn;
                fadeInTweenType = screenView.fadeInTweenType;
                fadeInAlpha = screenView.fadeInAlpha;
                fadeInCustomCurve = screenView.fadeInCustomCurve;

                scaleIn = screenView.scaleIn;
                scaleInTweenType = screenView.scaleInTweenType;
                scaleInScale = screenView.scaleInScale;
                scaleInCustomCurve = screenView.scaleInCustomCurve;

                slideIn = screenView.slideIn;
                slideInTweenType = screenView.slideInTweenType;
                slideInDirection = screenView.slideInDirection;
                autoSlideInAmount = screenView.autoSlideInAmount;
                slideInAmount = screenView.slideInAmount;
                slideInPercent = screenView.slideInPercent;
                slideInCustomCurve = screenView.slideInCustomCurve;

                rippleIn = screenView.rippleIn;
                rippleInTweenType = screenView.rippleInTweenType;
                rippleInType = screenView.rippleInType;
                rippleInPosition = screenView.rippleInPosition;
                rippleInCustomCurve = screenView.rippleInCustomCurve;

                fadeOut = screenView.fadeOut;
                fadeOutTweenType = screenView.fadeOutTweenType;
                fadeOutAlpha = screenView.fadeOutAlpha;
                fadeOutCustomCurve = screenView.fadeOutCustomCurve;

                scaleOut = screenView.scaleOut;
                scaleOutTweenType = screenView.scaleOutTweenType;
                scaleOutScale = screenView.scaleOutScale;
                scaleOutCustomCurve = screenView.scaleOutCustomCurve;

                slideOut = screenView.slideOut;
                slideOutTweenType = screenView.slideOutTweenType;
                slideOutDirection = screenView.slideOutDirection;
                autoSlideOutAmount = screenView.autoSlideOutAmount;
                slideOutAmount = screenView.slideOutAmount;
                slideOutPercent = screenView.slideOutPercent;
                slideOutCustomCurve = screenView.slideOutCustomCurve;

                rippleOut = screenView.rippleOut;
                rippleOutTweenType = screenView.rippleOutTweenType;
                rippleOutType = screenView.rippleOutType;
                rippleOutPosition = screenView.rippleOutPosition;
                rippleOutCustomCurve = screenView.rippleOutCustomCurve;

                transitionDuration = screenView.transitionDuration;
            }
        }

        private void SetupRipple()
        {
            ripple.sizeDelta = Vector2.zero;
            m_CurrentRipplePos = GetRipplePosition();
            m_TargetRipplePos = GetRippleTargetPosition();
            m_TempRippleSize = GetRippleTargetSize();
            ripple.gameObject.SetActive(true);
        }

        public void TransitionIn()
        {
            CheckValues();

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            gameObject.SetActive(true);

            m_TempScreenPos = rectTransform.position;

            if (rippleIn)
            {
                if (screenView.graphicRaycaster == null)
                {
                    screenView.graphicRaycaster = screenView.GetAddComponent<GraphicRaycaster>();
                }
                if (screenView.canvas == null)
                {
                    screenView.canvas = screenView.GetAddComponent<Canvas>();
                }
                screenView.canvas.overridePixelPerfect = true;
                screenView.canvas.pixelPerfect = false;

                SetupRipple();
                ripple.SetSiblingIndex(rectTransform.GetSiblingIndex());
                Vector2 tempSize = rectTransform.GetProperSize();
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                m_TempRippleScale = rectTransform.localScale;
                rectTransform.SetParent(ripple, true);
                rectTransform.sizeDelta = tempSize;
            }
            if (fadeIn)
            {
                canvasGroup.alpha = fadeInAlpha;
            }
            if (scaleIn)
            {
                rectTransform.localScale = new Vector3(scaleInScale, scaleInScale, scaleInScale);
            }
            if (slideIn)
            {
                if (autoSlideInAmount)
                {
                    bool isVertical = (slideInDirection == ScreenView.SlideDirection.Up ||
                                       slideInDirection == ScreenView.SlideDirection.Down);

                    if (isVertical)
                    {
                        slideInAmount = rectTransform.GetProperSize().y * slideInPercent * 0.01f;
                    }
                    else
                    {
                        slideInAmount = rectTransform.GetProperSize().x * slideInPercent * 0.01f;
                    }
                }

                switch (slideInDirection)
                {
                    case ScreenView.SlideDirection.Left:
                        rectTransform.position = new Vector2(m_TempScreenPos.x - slideInAmount, m_TempScreenPos.y);
                        break;
                    case ScreenView.SlideDirection.Right:
                        rectTransform.position = new Vector2(m_TempScreenPos.x + slideInAmount, m_TempScreenPos.y);
                        break;
                    case ScreenView.SlideDirection.Up:
                        rectTransform.position = new Vector2(m_TempScreenPos.x, m_TempScreenPos.y + slideInAmount);
                        break;
                    case ScreenView.SlideDirection.Down:
                        rectTransform.position = new Vector2(m_TempScreenPos.x, m_TempScreenPos.y - slideInAmount);
                        break;
                }

                m_SlideScreenPos = rectTransform.position;
            }

            m_IsTransitioning = 1;
            m_TransitionStartTime = Time.realtimeSinceStartup;
        }

        public void TransitionOut()
        {
            CheckValues();

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            m_TempScreenPos = rectTransform.position;

            if (rippleOut)
            {
                if (screenView.graphicRaycaster == null)
                {
                    screenView.graphicRaycaster = screenView.GetAddComponent<GraphicRaycaster>();
                }
                if (screenView.canvas == null)
                {
                    screenView.canvas = screenView.GetAddComponent<Canvas>();
                }
                screenView.canvas.overridePixelPerfect = true;
                screenView.canvas.pixelPerfect = false;

                SetupRipple();
                m_TempRippleSize = GetRippleTargetSize();
                ripple.sizeDelta = m_TempRippleSize;
                ripple.anchoredPosition = Vector2.zero;
                ripple.SetSiblingIndex(rectTransform.GetSiblingIndex());
                Vector2 tempSize = rectTransform.GetProperSize();
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                m_TempRippleScale = rectTransform.localScale;
                rectTransform.SetParent(ripple, true);
                rectTransform.sizeDelta = tempSize;
            }
            if (fadeOut)
            {
                canvasGroup.alpha = 1f;
            }
            if (scaleOut)
            {
                rectTransform.localScale = new Vector3(1f, 1f, 1f);
            }
            if (slideOut)
            {
                if (autoSlideOutAmount)
                {
                    bool isVertical = (slideOutDirection == ScreenView.SlideDirection.Up ||
                                       slideOutDirection == ScreenView.SlideDirection.Down);

                    if (isVertical)
                    {
                        slideOutAmount = rectTransform.GetProperSize().y * slideOutPercent * 0.01f;
                    }
                    else
                    {
                        slideOutAmount = rectTransform.GetProperSize().x * slideOutPercent * 0.01f;
                    }
                }

                switch (slideOutDirection)
                {
                    case ScreenView.SlideDirection.Left:
                        m_SlideScreenPos = new Vector2(m_TempScreenPos.x - slideOutAmount, m_TempScreenPos.y);
                        break;
                    case ScreenView.SlideDirection.Right:
                        m_SlideScreenPos = new Vector2(m_TempScreenPos.x + slideOutAmount, m_TempScreenPos.y);
                        break;
                    case ScreenView.SlideDirection.Up:
                        m_SlideScreenPos = new Vector2(m_TempScreenPos.x, m_TempScreenPos.y + slideOutAmount);
                        break;
                    case ScreenView.SlideDirection.Down:
                        m_SlideScreenPos = new Vector2(m_TempScreenPos.x, m_TempScreenPos.y - slideOutAmount);
                        break;
                }
            }

            m_IsTransitioning = 2;
            m_TransitionStartTime = Time.realtimeSinceStartup;
        }

        public void TransitionOutWithoutTransition()
        {
            m_IsTransitioning = 3;
            m_TransitionStartTime = Time.realtimeSinceStartup;
        }

        void Update()
        {
            if (m_IsTransitioning > 0)
            {
                m_TransitionDeltaTime = Time.realtimeSinceStartup - m_TransitionStartTime;

                if (m_TransitionDeltaTime <= transitionDuration)
                {
                    if (m_IsTransitioning == 1)
                    {
                        if (rippleIn)
                        {
                            Vector3 tempVector3 = m_Ripple.position;
                            tempVector3.x = Tween.Evaluate(rippleInTweenType, m_CurrentRipplePos.x, m_TargetRipplePos.x, m_TransitionDeltaTime, m_TransitionDuration, rippleInCustomCurve);
                            tempVector3.y = Tween.Evaluate(rippleInTweenType, m_CurrentRipplePos.y, m_TargetRipplePos.y, m_TransitionDeltaTime, m_TransitionDuration, rippleInCustomCurve);
                            tempVector3.z = Tween.Evaluate(rippleInTweenType, m_CurrentRipplePos.z, m_TargetRipplePos.z, m_TransitionDeltaTime, m_TransitionDuration, rippleInCustomCurve);
                            m_Ripple.position = tempVector3;

                            Vector2 tempVector2 = m_Ripple.sizeDelta;
                            tempVector2.x = Tween.Evaluate(rippleInTweenType, 0, m_TempRippleSize.x, m_TransitionDeltaTime, m_TransitionDuration, rippleInCustomCurve);
                            tempVector2.y = Tween.Evaluate(rippleInTweenType, 0, m_TempRippleSize.y, m_TransitionDeltaTime, m_TransitionDuration, rippleInCustomCurve);
                            m_Ripple.sizeDelta = tempVector2;

                            rectTransform.position = m_TempScreenPos;

                            rectTransform.localScale = new Vector3(m_TempRippleScale.x / ripple.localScale.x, m_TempRippleScale.y / ripple.localScale.y, m_TempRippleScale.z / ripple.localScale.z);
                        }
                        if (fadeIn)
                        {
                            canvasGroup.alpha = Tween.Evaluate(fadeInTweenType, fadeInAlpha, 1f, m_TransitionDeltaTime,
                                transitionDuration, fadeInCustomCurve);
                        }
                        if (scaleIn)
                        {
                            Vector3 tempVector3 = rectTransform.localScale;
                            tempVector3.x = Tween.Evaluate(scaleInTweenType, scaleInScale, 1f, m_TransitionDeltaTime,
                                transitionDuration, scaleInCustomCurve);
                            tempVector3.y = tempVector3.x;
                            tempVector3.z = tempVector3.x;
                            rectTransform.localScale = tempVector3;
                        }
                        if (slideIn)
                        {
                            Vector3 tempVector3 = rectTransform.position;
                            tempVector3.x = Tween.Evaluate(slideInTweenType, m_SlideScreenPos.x, m_TempScreenPos.x, m_TransitionDeltaTime,
                                transitionDuration, slideInCustomCurve);
                            tempVector3.y = Tween.Evaluate(slideInTweenType, m_SlideScreenPos.y, m_TempScreenPos.y, m_TransitionDeltaTime,
                                transitionDuration, slideInCustomCurve);
                            rectTransform.position = tempVector3;
                        }
                    }
                    else if (m_IsTransitioning == 2)
                    {
                        if (rippleOut)
                        {
                            Vector3 tempVector3 = m_Ripple.position;
                            tempVector3.x = Tween.Evaluate(rippleInTweenType, m_CurrentRipplePos.x, m_TargetRipplePos.x, m_TransitionDeltaTime, m_TransitionDuration, rippleInCustomCurve);
                            tempVector3.y = Tween.Evaluate(rippleInTweenType, m_CurrentRipplePos.y, m_TargetRipplePos.y, m_TransitionDeltaTime, m_TransitionDuration, rippleInCustomCurve);
                            tempVector3.z = Tween.Evaluate(rippleInTweenType, m_CurrentRipplePos.z, m_TargetRipplePos.z, m_TransitionDeltaTime, m_TransitionDuration, rippleInCustomCurve);
                            m_Ripple.position = tempVector3;

                            Vector2 tempVector2 = m_Ripple.sizeDelta;
                            tempVector2.x = Tween.Evaluate(rippleInTweenType, m_TempRippleSize.x, 0,
                                m_TransitionDeltaTime, m_TransitionDuration, rippleInCustomCurve);
                            tempVector2.y = Tween.Evaluate(rippleInTweenType, m_TempRippleSize.y, 0,
                                m_TransitionDeltaTime, m_TransitionDuration, rippleInCustomCurve);
                            m_Ripple.sizeDelta = tempVector2;

                            rectTransform.position = m_TempScreenPos;

                            rectTransform.localScale = new Vector3(m_TempRippleScale.x / ripple.localScale.x, m_TempRippleScale.y / ripple.localScale.y, m_TempRippleScale.z / ripple.localScale.z);
                        }
                        if (fadeOut)
                        {
                            canvasGroup.alpha = Tween.Evaluate(fadeOutTweenType, 1f, fadeOutAlpha,
                                m_TransitionDeltaTime, transitionDuration, fadeOutCustomCurve);
                        }
                        if (scaleOut)
                        {
                            Vector3 tempVector3 = rectTransform.localScale;
                            tempVector3.x = Tween.Evaluate(scaleOutTweenType, 1f, scaleOutScale, m_TransitionDeltaTime,
                                transitionDuration, scaleOutCustomCurve);
                            tempVector3.y = tempVector3.x;
                            tempVector3.z = tempVector3.x;
                            rectTransform.localScale = tempVector3;
                        }
                        if (slideOut)
                        {
                            Vector3 tempVector3 = rectTransform.position;
                            tempVector3.x = Tween.Evaluate(slideOutTweenType, m_TempScreenPos.x, m_SlideScreenPos.x,
                                m_TransitionDeltaTime, transitionDuration, slideOutCustomCurve);
                            tempVector3.y = Tween.Evaluate(slideOutTweenType, m_TempScreenPos.y, m_SlideScreenPos.y, m_TransitionDeltaTime,
                                transitionDuration, slideOutCustomCurve);
                            rectTransform.position = tempVector3;
                        }
                    }
                }
                else
                {
                    if (m_IsTransitioning == 1)
                    {
                        if (rippleIn)
                        {
                            rectTransform.SetParent(screenView.transform, true);
                            rectTransform.position = m_TempScreenPos;
                            rectTransform.anchorMin = Vector2.zero;
                            rectTransform.anchorMax = Vector2.one;
                            rectTransform.sizeDelta = Vector2.zero;
                            rectTransform.anchoredPosition = Vector2.zero;
                            rectTransform.localScale = m_TempRippleScale;
                            ripple.gameObject.SetActive(false);
                        }
                        if (fadeIn)
                        {
                            canvasGroup.alpha = 1f;
                        }
                        if (scaleIn)
                        {
                            rectTransform.localScale = new Vector3(1f, 1f, 1f);
                        }
                        if (slideIn)
                        {
                            rectTransform.position = m_TempScreenPos;
                        }
                    }
                    else if (m_IsTransitioning == 2)
                    {
                        if (rippleOut)
                        {
                            rectTransform.SetParent(screenView.transform, true);
                            rectTransform.position = m_TempScreenPos;
                            rectTransform.anchorMin = Vector2.zero;
                            rectTransform.anchorMax = Vector2.one;
                            rectTransform.sizeDelta = Vector2.zero;
                            rectTransform.anchoredPosition = Vector2.zero;
                            rectTransform.localScale = m_TempRippleScale;
                            ripple.gameObject.SetActive(false);
                        }
                        if (fadeOut)
                        {
                            canvasGroup.alpha = 1f;
                        }
                        if (scaleOut)
                        {
                            rectTransform.localScale = new Vector3(1f, 1f, 1f);
                        }
                        if (slideOut)
                        {
                            rectTransform.position = m_TempScreenPos;
                        }
                    }

                    if (m_IsTransitioning > 1)
                    {
                        if (m_DisableWhenNotVisible)
                        {
                            gameObject.SetActive(false);
                        }
                    }

                    m_IsTransitioning = 0;
                    screenView.OnScreenEndTransition(screenIndex);
                }
            }
        }

        private Vector2 GetRipplePosition()
        {
            switch (m_RippleInType)
            {
                case ScreenView.RippleType.Manual:
                    return m_RippleInPosition;

                case ScreenView.RippleType.Center:
                    Vector3 rectPosition = rectTransform.GetPositionRegardlessOfPivot();
                    return new Vector2(rectPosition.x + rectTransform.sizeDelta.x * 0.5f, rectPosition.y + rectTransform.sizeDelta.y * 0.5f);

                default:
                    return Input.mousePosition;
            }
        }

        private Vector2 GetRippleTargetSize()
        {
            Vector2 size = rectTransform.GetProperSize();

            size.x *= size.x;
            size.y *= size.y;

            size.x = Mathf.Sqrt(size.x + size.y);
            size.y = size.x;

            return size;
        }

        private Vector3 GetRippleTargetPosition()
        {
            return rectTransform.GetPositionRegardlessOfPivot();
        }

        public void Interrupt()
        {
            if (m_IsTransitioning == 1)
            {
                if (rippleIn)
                {
                    rectTransform.SetParent(screenView.transform, true);
                    rectTransform.position = m_TempScreenPos;
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.anchoredPosition = Vector2.zero;
                    ripple.gameObject.SetActive(false);
                }
                if (fadeIn)
                {
                    canvasGroup.alpha = 1f;
                }
                if (scaleIn)
                {
                    rectTransform.localScale = new Vector3(1f, 1f, 1f);
                }
                if (slideIn)
                {
                    rectTransform.position = m_TempScreenPos;
                }
            }
            else if (m_IsTransitioning == 2)
            {
                if (rippleOut)
                {
                    rectTransform.SetParent(screenView.transform, true);
                    rectTransform.position = m_TempScreenPos;
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.anchoredPosition = Vector2.zero;
                    ripple.gameObject.SetActive(false);
                }
                if (fadeOut)
                {
                    canvasGroup.alpha = 1f;
                }
                if (scaleOut)
                {
                    rectTransform.localScale = new Vector3(1f, 1f, 1f);
                }
                if (slideOut)
                {
                    rectTransform.position = m_TempScreenPos;
                }
            }

            if (m_IsTransitioning > 1)
            {
                if (m_DisableWhenNotVisible)
                {
                    gameObject.SetActive(false);
                }
            }

            if (m_IsTransitioning > 0)
            {
                m_IsTransitioning = 0;
                screenView.OnScreenEndTransition(screenIndex);
            }
        }
    }
}