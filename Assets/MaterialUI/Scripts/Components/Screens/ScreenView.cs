//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [ExecuteInEditMode]
    [AddComponentMenu("MaterialUI/Screen View", 100)]
    public class ScreenView : UIBehaviour
    {
        public enum SlideDirection
        {
            Left,
            Right,
            Up,
            Down
        }

        public enum RippleType
        {
            MousePosition,
            Manual,
            Center,
        }

        public enum Type
        {
            In,
            Out,
            InOut
        }

#if UNITY_EDITOR
        [SerializeField]
        private bool m_AutoTrackScreens = true;

        [SerializeField]
        private bool m_OnlyShowSelectedScreen = true;

        private bool m_ScreensDirty = true;
        public bool screensDirty
        {
            set { m_ScreensDirty = value; }
        }

        private GameObject m_OldSelectionObjects;
#endif

        [SerializeField]
        private MaterialScreen[] m_MaterialScreens = new MaterialScreen[0];
        public MaterialScreen[] materialScreen
        {
            get { return m_MaterialScreens; }
            set { m_MaterialScreens = value; }
        }

        [SerializeField]
        private int m_CurrentScreen;
        public int currentScreen
        {
            get { return m_CurrentScreen; }
            set { m_CurrentScreen = value; }
        }

        private int m_LastScreen;
        public int lastScreen
        {
            get { return m_LastScreen; }
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
        private SlideDirection m_SlideInDirection = SlideDirection.Right;
        public SlideDirection slideInDirection
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
        private RippleType m_RippleInType;
        public RippleType rippleInType
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
        private SlideDirection m_SlideOutDirection = SlideDirection.Left;
        public SlideDirection slideOutDirection
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
        private RippleType m_RippleOutType;
        public RippleType rippleOutType
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

        [SerializeField]
        private Type m_TransitionType = Type.In;
        public Type transitionType
        {
            get { return m_TransitionType; }
            set { m_TransitionType = value; }
        }

        private Canvas m_Canvas;
        public Canvas canvas
        {
            get { return m_Canvas; }
            set { m_Canvas = value; }
        }

        private GraphicRaycaster m_GraphicRaycaster;
        public GraphicRaycaster graphicRaycaster
        {
            get { return m_GraphicRaycaster; }
            set { m_GraphicRaycaster = value; }
        }

        [Serializable]
        public class OnScreenTransitionUnityEvent : UnityEvent<int> { }

        [SerializeField]
        private OnScreenTransitionUnityEvent m_OnScreenEndTransition;
        public OnScreenTransitionUnityEvent onScreenEndTransition
        {
            get { return m_OnScreenEndTransition; }
            set { m_OnScreenEndTransition = value; }
        }

        [SerializeField]
        private OnScreenTransitionUnityEvent m_OnScreenBeginTransition;
        public OnScreenTransitionUnityEvent onScreenBeginTransition
        {
            get { return m_OnScreenBeginTransition; }
            set { m_OnScreenBeginTransition = value; }
        }

        private int m_ScreensTransitioning;

#if UNITY_EDITOR
        protected override void OnEnable()
        {
            if (PrefabUtility.GetPrefabType(this) == PrefabType.Prefab)
            {
                EditorUpdate.onEditorUpdate -= OnEditorUpdate;
                return;
            }

            EditorUpdate.Init();
            EditorUpdate.onEditorUpdate += OnEditorUpdate;

            m_GraphicRaycaster = GetComponent<GraphicRaycaster>();
            if (m_GraphicRaycaster != null)
            {
                DestroyImmediate(m_GraphicRaycaster);
            }

            m_Canvas = GetComponent<Canvas>();
            if (m_Canvas != null)
            {
                DestroyImmediate(m_Canvas);
            }
        }

        protected override void OnDisable()
        {
            MaterialUI.EditorUpdate.onEditorUpdate -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (IsDestroyed())
            {
                MaterialUI.EditorUpdate.onEditorUpdate -= OnEditorUpdate;
                return;
            }

            if (m_AutoTrackScreens)
            {
                MaterialScreen[] tempMaterialScreens = GetComponentsInChildren<MaterialScreen>(true);

                List<MaterialScreen> ownedTempScreens = new List<MaterialScreen>();

                for (int i = 0; i < tempMaterialScreens.Length; i++)
                {
                    if (tempMaterialScreens[i].transform.parent == transform)
                    {
                        ownedTempScreens.Add(tempMaterialScreens[i]);
                    }
                }

                materialScreen = new MaterialScreen[ownedTempScreens.Count];

                for (int i = 0; i < ownedTempScreens.Count; i++)
                {
                    materialScreen[i] = ownedTempScreens[i];
                }
            }

            if (m_OldSelectionObjects != Selection.activeGameObject)
            {
                m_OldSelectionObjects = Selection.activeGameObject;
                m_ScreensDirty = true;
            }

            if (m_MaterialScreens.Length > 0 && m_ScreensDirty)
            {
                m_ScreensDirty = false;

                bool screenSelected = false;

                if (m_OnlyShowSelectedScreen)
                {
                    for (int i = 0; i < materialScreen.Length; i++)
                    {
                        RectTransform[] children = materialScreen[i].GetComponentsInChildren<RectTransform>(true);

                        bool objectSelected = false;

                        for (int j = 0; j < children.Length; j++)
                        {
                            if (Selection.Contains(children[j].gameObject))
                            {
                                materialScreen[i].gameObject.SetActive(true);
                                screenSelected = true;
                                objectSelected = true;
                            }
                        }
                        if (!objectSelected)
                        {
                            materialScreen[i].gameObject.SetActive(false);
                        }
                    }

                    if (!screenSelected && !m_MaterialScreens[m_CurrentScreen].gameObject.activeSelf)
                    {
                        m_MaterialScreens[m_CurrentScreen].gameObject.SetActive(true);
                    }
                }
            }
            if (m_CurrentScreen < 0)
            {
                m_CurrentScreen = 0;
            }
            else if (m_MaterialScreens.Length > 0 && m_CurrentScreen >= m_MaterialScreens.Length)
            {
                m_CurrentScreen = m_MaterialScreens.Length - 1;
            }
        }
#endif

        protected override void Start()
        {
            if (Application.isPlaying)
            {
                if (m_MaterialScreens.Length > 0)
                {
                    for (int i = 0; i < materialScreen.Length; i++)
                    {
                        if (i != m_CurrentScreen)
                        {
                            materialScreen[i].gameObject.SetActive(!materialScreen[i].disableWhenNotVisible);
                        }

                        m_MaterialScreens[i].screenIndex = i;
                    }

                    m_MaterialScreens[m_CurrentScreen].gameObject.SetActive(true);
                    m_MaterialScreens[m_CurrentScreen].rectTransform.SetAsLastSibling();
                }
            }
        }

        public void Back()
        {
            Transition(m_LastScreen);
        }

        public void Back(Type transitionType)
        {
            Transition(m_LastScreen, transitionType);
        }

        public void Transition(int screenIndex)
        {
            Transition(screenIndex, transitionType);
        }

        public void Transition(int screenIndex, Type transitionType)
        {
            if (0 > screenIndex || screenIndex >= materialScreen.Length || screenIndex == currentScreen) return;

            m_LastScreen = m_CurrentScreen;
            m_CurrentScreen = screenIndex;

            m_MaterialScreens[m_LastScreen].Interrupt();
            m_MaterialScreens[m_CurrentScreen].Interrupt();

            if (transitionType == Type.In)
            {
                m_MaterialScreens[m_CurrentScreen].rectTransform.SetAsLastSibling();
                m_MaterialScreens[m_CurrentScreen].TransitionIn();
                m_MaterialScreens[m_LastScreen].TransitionOutWithoutTransition();
            }
            else if (transitionType == Type.Out)
            {
                m_MaterialScreens[m_CurrentScreen].rectTransform.SetAsLastSibling();
                m_MaterialScreens[m_LastScreen].rectTransform.SetAsLastSibling();
                m_MaterialScreens[m_CurrentScreen].gameObject.SetActive(true);
                m_MaterialScreens[m_LastScreen].TransitionOut();
            }
            else if (transitionType == Type.InOut)
            {
                m_MaterialScreens[m_CurrentScreen].rectTransform.SetAsLastSibling();
                m_MaterialScreens[m_CurrentScreen].TransitionIn();
                m_MaterialScreens[m_LastScreen].TransitionOut();
            }

            m_ScreensTransitioning += 2;

            m_OnScreenBeginTransition.InvokeIfNotNull(screenIndex);
        }

        public void NextScreen(bool wrap = true)
        {
            if (currentScreen != materialScreen.Length - 1)
            {
                Transition(currentScreen + 1);
            }
            else if (wrap)
            {
                Transition(0);
            }
        }

        public void PreviousScreen(bool wrap = true)
        {
            if (currentScreen >= 1)
            {
                Transition(currentScreen - 1);
            }
            else if (wrap)
            {
                Transition(materialScreen.Length - 1);
            }
        }

        public void Transition(string screenName)
        {
            Transition(screenName, transitionType);
        }

        public void Transition(string screenName, Type transitionType)
        {
            for (int i = 0; i < materialScreen.Length; i++)
            {
                if (materialScreen[i].name == screenName)
                {
                    Transition(i, transitionType);
                }
            }
        }

        public void OnScreenEndTransition(int screenIndex)
        {
            m_ScreensTransitioning--;

            if (m_ScreensTransitioning <= 0)
            {
                m_ScreensTransitioning = 0;

                m_OnScreenEndTransition.InvokeIfNotNull(screenIndex);

                if (m_GraphicRaycaster != null)
                {
                    Destroy(m_GraphicRaycaster);
                }
                if (m_Canvas != null)
                {
                    Destroy(m_Canvas);
                }
            }
        }
    }
}
