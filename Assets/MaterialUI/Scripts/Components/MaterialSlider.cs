//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MaterialUI
{
    /// <summary>
    /// Component that handles a horizontal slider control.
    /// </summary>
    /// <seealso cref="UnityEngine.EventSystems.UIBehaviour" />
    /// <seealso cref="UnityEngine.EventSystems.ISelectHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IDeselectHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IPointerDownHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IPointerUpHandler" />
    /// <seealso cref="UnityEngine.UI.ILayoutGroup" />
    /// <seealso cref="UnityEngine.UI.ILayoutElement" />
    [ExecuteInEditMode]
    [AddComponentMenu("MaterialUI/Material Slider", 100)]
    public class MaterialSlider : UIBehaviour, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler, ILayoutGroup, ILayoutElement
    {
        /// <summary>
        /// Does the slider have a popup displaying the current value when dragged?
        /// </summary>
        [SerializeField]
        private bool m_HasPopup = true;
        /// <summary>
        /// Does the slider have a popup displaying the current value when dragged?
        /// </summary>
        public bool hasPopup
        {
            get { return m_HasPopup; }
            set { m_HasPopup = value; }
        }

        /// <summary>
        /// Does the slider have a dot at each value number?
        /// Only applicable when slider is discrete and has a range of less than 200.
        /// </summary>
        [SerializeField]
        private bool m_HasDots = true;
        /// <summary>
        /// Does the slider have a dot at each value number?
        /// Only applicable when slider is discrete and has a range of less than 200.
        /// </summary>
        public bool hasDots
        {
            get { return m_HasDots; }
            set { m_HasDots = value; }
        }

        /// <summary>
        /// The duration of the popup animation.
        /// Only applicable if hasPopup is true.
        /// </summary>
        [SerializeField]
        private float m_AnimationDuration = 0.5f;
        /// <summary>
        /// The duration of the popup animation.
        /// Only applicable if hasPopup is true.
        /// </summary>
        public float animationDuration
        {
            get { return m_AnimationDuration; }
            set { m_AnimationDuration = value; }
        }

        /// <summary>
        /// The color of the slider when enabled.
        /// </summary>
        [SerializeField]
        private Color m_EnabledColor;
        /// <summary>
        /// The color of the slider when enabled.
        /// </summary>
        public Color enabledColor
        {
            get { return m_EnabledColor; }
            set
            {
                m_EnabledColor = value;

                if (m_HandleGraphic)
                {
                    m_HandleGraphic.color = m_Interactable ? m_EnabledColor : m_DisabledColor;
                }

                for (int i = 0; i < m_DotGraphics.Length; i++)
                {
                    if (m_DotGraphics[i] == null) continue;

                    if (slider.value > i)
                    {
                        m_DotGraphics[i].color = m_Interactable ? m_EnabledColor : m_DisabledColor;
                    }
                    else
                    {
                        m_DotGraphics[i].color = m_BackgroundColor;
                    }
                }
            }
        }

        /// <summary>
        /// The color of the slider when disabled.
        /// </summary>
        [SerializeField]
        private Color m_DisabledColor;
        /// <summary>
        /// The color of the slider when disabled.
        /// </summary>
        public Color disabledColor
        {
            get { return m_DisabledColor; }
            set
            {
                m_DisabledColor = value;

                if (m_HandleGraphic)
                {
                    m_HandleGraphic.color = m_Interactable ? m_EnabledColor : m_DisabledColor;
                }

                for (int i = 0; i < m_DotGraphics.Length; i++)
                {
                    if (m_DotGraphics[i] == null) continue;

                    if (slider.value > i)
                    {
                        m_DotGraphics[i].color = m_Interactable ? m_EnabledColor : m_DisabledColor;
                    }
                    else
                    {
                        m_DotGraphics[i].color = m_BackgroundColor;
                    }
                }
            }
        }

        /// <summary>
        /// The color of the background bar.
        /// </summary>
        [SerializeField]
        private Color m_BackgroundColor;
        /// <summary>
        /// The color of the background bar.
        /// </summary>
        public Color backgroundColor
        {
            get
            {
                if (m_BackgroundGraphic != null)
                {
                    if (m_BackgroundGraphic.color != m_BackgroundColor)
                    {
                        m_BackgroundColor = m_BackgroundGraphic.color;
                    }
                }
                return m_BackgroundColor;
            }
            set
            {
                m_BackgroundColor = value;
                if (m_BackgroundGraphic != null)
                {
                    m_BackgroundGraphic.color = m_BackgroundColor;
                }
            }
        }

        /// <summary>
        /// The handle RectTransform.
        /// </summary>
        [SerializeField]
        private RectTransform m_SliderHandleTransform;
        /// <summary>
        /// The handle RectTransform.
        /// </summary>
        public RectTransform sliderHandleTransform
        {
            get { return m_SliderHandleTransform; }
            set { m_SliderHandleTransform = value; }
        }

        /// <summary>
        /// The handle graphic.
        /// </summary>
        [SerializeField]
        private Graphic m_HandleGraphic;
        /// <summary>
        /// The handle graphic.
        /// </summary>
        public Graphic handleGraphic
        {
            get { return m_HandleGraphic; }
            set
            {
                m_HandleGraphic = value;
                if (m_HandleGraphic)
                {
                    m_HandleGraphic.color = m_Interactable ? m_EnabledColor : m_DisabledColor;
                }
            }
        }

        /// <summary>
        /// The handle graphic's RectTransform.
        /// </summary>
        private RectTransform m_HandleGraphicTransform;
        /// <summary>
        /// The handle graphic's RectTransform.
        /// If null, gets the handle graphic's transform if it exists.
        /// </summary>
        public RectTransform handleGraphicTransform
        {
            get
            {
                if (m_HandleGraphicTransform == null)
                {
                    if (m_HandleGraphic != null)
                    {
                        m_HandleGraphicTransform = m_HandleGraphic.rectTransform;
                    }
                }
                return m_HandleGraphicTransform;
            }
        }

        /// <summary>
        /// The popup transform.
        /// </summary>
        [SerializeField]
        private RectTransform m_PopupTransform;
        /// <summary>
        /// The popup transform.
        /// </summary>
        public RectTransform popupTransform
        {
            get { return m_PopupTransform; }
            set { m_PopupTransform = value; }
        }

        /// <summary>
        /// The popup text.
        /// </summary>
        [SerializeField]
        private Text m_PopupText;
        /// <summary>
        /// The popup text.
        /// </summary>
        public Text popupText
        {
            get { return m_PopupText; }
            set { m_PopupText = value; }
        }

        /// <summary>
        /// The value text.
        /// </summary>
        [SerializeField]
        private Text m_ValueText;
        /// <summary>
        /// The value text.
        /// </summary>
        public Text valueText
        {
            get { return m_ValueText; }
            set { m_ValueText = value; }
        }

        /// <summary>
        /// The input field.
        /// </summary>
        [SerializeField]
        private MaterialInputField m_InputField;
        /// <summary>
        /// The input field.
        /// </summary>
        public MaterialInputField inputField
        {
            get { return m_InputField; }
            set { m_InputField = value; }
        }

        /// <summary>
        /// The fill transform.
        /// </summary>
        [SerializeField]
        private RectTransform m_FillTransform;
        /// <summary>
        /// The fill transform.
        /// </summary>
        public RectTransform fillTransform
        {
            get { return m_FillTransform; }
            set { m_FillTransform = value; }
        }

        /// <summary>
        /// The background graphic.
        /// </summary>
        [SerializeField]
        private Graphic m_BackgroundGraphic;
        /// <summary>
        /// The background graphic.
        /// </summary>
        public Graphic backgroundGraphic
        {
            get { return m_BackgroundGraphic; }
            set
            {
                m_BackgroundGraphic = value;
                if (m_BackgroundGraphic != null)
                {
                    m_BackgroundGraphic.color = m_BackgroundColor;
                }
            }
        }

        /// <summary>
        /// The left content transform.
        /// </summary>
        [SerializeField]
        private RectTransform m_LeftContentTransform;
        /// <summary>
        /// The left content transform.
        /// </summary>
        public RectTransform leftContentTransform
        {
            get { return m_LeftContentTransform; }
            set { m_LeftContentTransform = value; }
        }

        /// <summary>
        /// The right content transform.
        /// </summary>
        [SerializeField]
        private RectTransform m_RightContentTransform;
        /// <summary>
        /// The right content transform.
        /// </summary>
        public RectTransform rightContentTransform
        {
            get { return m_RightContentTransform; }
            set { m_RightContentTransform = value; }
        }

        /// <summary>
        /// The slider content transform.
        /// </summary>
        [SerializeField]
        private RectTransform m_SliderContentTransform;
        /// <summary>
        /// The slider content transform.
        /// </summary>
        public RectTransform sliderContentTransform
        {
            get { return m_SliderContentTransform; }
            set { m_SliderContentTransform = value; }
        }

        /// <summary>
        /// The rect transform.
        /// </summary>
        [SerializeField]
        private RectTransform m_RectTransform;
        /// <summary>
        /// The rect transform.
        /// </summary>
        public RectTransform rectTransform
        {
            get { return m_RectTransform; }
            set { m_RectTransform = value; }
        }

        /// <summary>
        /// The dot template icon.
        /// </summary>
        [SerializeField]
        private VectorImageData m_DotTemplateIcon;
        /// <summary>
        /// The dot template icon.
        /// </summary>
        public VectorImageData dotTemplateIcon
        {
            get { return m_DotTemplateIcon; }
            set { m_DotTemplateIcon = value; }
        }

        /// <summary>
        /// The dot graphics.
        /// </summary>
        [SerializeField]
        private Graphic[] m_DotGraphics = new Graphic[0];

        /// <summary>
        /// The number of dots.
        /// </summary>
        [SerializeField]
        private int m_NumberOfDots;

        /// <summary>
        /// The fill area transform.
        /// </summary>
        private RectTransform m_FillAreaTransform;
        /// <summary>
        /// The fill area transform.
        /// </summary>
        public RectTransform fillAreaTransform
        {
            get
            {
                if (m_FillAreaTransform == null)
                {
                    if (m_FillTransform != null)
                    {
                        m_FillAreaTransform = m_FillTransform.parent as RectTransform;
                    }
                }

                return m_FillAreaTransform;
            }
        }

        /// <summary>
        /// The slider.
        /// </summary>
        private Slider m_Slider;
        /// <summary>
        /// The slider.
        /// If null, gets the attached Slider if it exists.
        /// </summary>
        public Slider slider
        {
            get
            {
                if (m_Slider == null)
                {
                    m_Slider = GetComponent<Slider>();
                }
                return m_Slider;
            }
        }

        /// <summary>
        /// The canvas group.
        /// </summary>
        private CanvasGroup m_CanvasGroup;
        /// <summary>
        /// The canvas group.
        /// If null, gets the attached CanvasGroup if it exists.
        /// </summary>
        public CanvasGroup canvasGroup
        {
            get
            {
                if (m_CanvasGroup == null)
                {
                    m_CanvasGroup = GetComponent<CanvasGroup>();
                }
                return m_CanvasGroup;
            }
        }

        /// <summary>
        /// The root canvas.
        /// </summary>
        private Canvas m_RootCanvas;
        /// <summary>
        /// The root canvas.
        /// If null, gets the root MaterialUIScaler's targetCanvas if it exists.
        /// </summary>
        public Canvas rootCanvas
        {
            get
            {
                if (m_RootCanvas == null)
                {
                    MaterialUIScaler rootScaler = MaterialUIScaler.GetParentScaler(transform);
                    if (rootScaler != null)
                    {
                        m_RootCanvas = rootScaler.targetCanvas;
                    }
                }
                return m_RootCanvas;
            }
        }

        /// <summary>
        /// Is the slider selected?
        /// </summary>
        private bool m_IsSelected;
        /// <summary>
        /// Is the slider selected?
        /// </summary>
        public bool isSelected
        {
            get { return m_IsSelected; }
        }

        /// <summary>
        /// Does the slider have a manual preferred width?
        /// </summary>
        [SerializeField]
        private bool m_HasManualPreferredWidth;
        /// <summary>
        /// Does the slider have a manual preferred width?
        /// </summary>
        public bool hasManualPreferredWidth
        {
            get { return m_HasManualPreferredWidth; }
            set
            {
                m_HasManualPreferredWidth = value;
                CalculateLayoutInputHorizontal();
                SetLayoutHorizontal();
            }
        }

        /// <summary>
        /// The manual preferred width.
        /// Only applicable if hasManualPreferredWidth is true.
        /// </summary>
        [SerializeField]
        private float m_ManualPreferredWidth = 200f;
        /// <summary>
        /// The manual preferred width.
        /// Only applicable if hasManualPreferredWidth is true.
        /// </summary>
        public float manualPreferredWidth
        {
            get { return m_ManualPreferredWidth; }
            set
            {
                m_ManualPreferredWidth = value;
                CalculateLayoutInputHorizontal();
                SetLayoutHorizontal();
            }
        }

        /// <summary>
        /// Is the slider interactable?
        /// </summary>
        [SerializeField]
        private bool m_Interactable = true;
        /// <summary>
        /// Is the slider interactable?
        /// </summary>
        public bool interactable
        {
            get { return m_Interactable; }
            set
            {
                m_Interactable = value;
                slider.interactable = value;
                canvasGroup.interactable = value;
                canvasGroup.blocksRaycasts = value;
                if (m_InputField)
                {
                    m_InputField.GetComponent<MaterialInputField>().interactable = value;
                }
            }
        }

        /// <summary>
        /// Should the left content have low opacity when non-interactable?
        /// </summary>
        [SerializeField]
        private bool m_LowLeftDisabledOpacity;
        /// <summary>
        /// Should the left content have low opacity when non-interactable?
        /// </summary>
        public bool lowLeftDisabledOpacity
        {
            get { return m_LowLeftDisabledOpacity; }
            set
            {
                m_LowLeftDisabledOpacity = value;

                if (m_LeftContentTransform)
                {
                    leftCanvasGroup.alpha = m_LowLeftDisabledOpacity ? (m_Interactable ? 1f : 0.5f) : 1f;
                }
            }
        }

        /// <summary>
        /// Should the right content have low opacity when non-interactable?
        /// </summary>
        [SerializeField]
        private bool m_LowRightDisabledOpacity;
        /// <summary>
        /// Should the right content have low opacity when non-interactable?
        /// </summary>
        public bool lowRightDisabledOpacity
        {
            get { return m_LowRightDisabledOpacity; }
            set
            {
                m_LowRightDisabledOpacity = value;

                if (m_RightContentTransform)
                {
                    rightCanvasGroup.alpha = m_LowRightDisabledOpacity ? (m_Interactable ? 1f : 0.5f) : 1f;
                }
            }
        }

        /// <summary>
        /// The left content canvas group.
        /// </summary>
        [SerializeField]
        private CanvasGroup m_LeftCanvasGroup;
        /// <summary>
        /// The left content canvas group.
        /// If null, gets the left content transform's CanvasGroup, and creates one if none exist.
        /// </summary>
        public CanvasGroup leftCanvasGroup
        {
            get
            {
                if (m_LeftCanvasGroup == null)
                {
                    if (m_LeftContentTransform != null)
                    {
                        m_LeftCanvasGroup = m_LeftContentTransform.gameObject.GetAddComponent<CanvasGroup>();
                    }
                }
                return m_LeftCanvasGroup;
            }
        }

        /// <summary>
        /// The right content canvas group.
        /// </summary>
        [SerializeField]
        private CanvasGroup m_RightCanvasGroup;
        /// <summary>
        /// The right content canvas group.
        /// If null, gets the right content transform's CanvasGroup, and creates one if none exist.
        /// </summary>
        public CanvasGroup rightCanvasGroup
        {
            get
            {
                if (m_RightCanvasGroup == null)
                {
                    if (m_RightContentTransform != null)
                    {
                        m_RightCanvasGroup = m_RightContentTransform.gameObject.GetAddComponent<CanvasGroup>();
                    }
                }
                return m_RightCanvasGroup;
            }
        }

        /// <summary>
        /// The handle size tweener.
        /// </summary>
        private int m_HandleSizeTweener;
        /// <summary>
        /// The popup scale tweener.
        /// </summary>
        private int m_PopupScaleTweener;

        /// <summary>
        /// The handle anchor minimum tweener.
        /// </summary>
        private int m_HandleAnchorMinTweener;
        /// <summary>
        /// The handle anchor maximum tweener.
        /// </summary>
        private int m_HandleAnchorMaxTweener;
        /// <summary>
        /// The handle position y tweener.
        /// </summary>
        private int m_HandlePositionYTweener;

        /// <summary>
        /// The popup text color tweener.
        /// </summary>
        private int m_PopupTextColorTweener;

        /// <summary>
        /// The tracker.
        /// </summary>
        private DrivenRectTransformTracker m_Tracker = new DrivenRectTransformTracker();

        /// <summary>
        /// The width.
        /// </summary>
        private float m_Width;
        /// <summary>
        /// The height.
        /// </summary>
        private float m_Height;
        /// <summary>
        /// The left width.
        /// </summary>
        private float m_LeftWidth;
        /// <summary>
        /// The right width.
        /// </summary>
        private float m_RightWidth;
        /// <summary>
        /// The last slider value.
        /// </summary>
        private float m_LastSliderValue;

        /// <summary>
        /// The current input value.
        /// </summary>
        private float m_CurrentInputValue;

#if UNITY_EDITOR
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialSlider"/> class.
        /// </summary>
        public MaterialSlider()
        {
            EditorUpdate.Init();
            EditorUpdate.onEditorUpdate += OnEditorUpdate;
        }
#endif

        /// <summary>
        /// See MonoBehaviour.OnEnable.
        /// </summary>
        protected override void OnEnable()
        {
            SetTracker();
        }

        /// <summary>
        /// See MonoBehaviour.OnDisable.
        /// </summary>
        protected override void OnDisable()
        {
            m_Tracker.Clear();
        }

        /// <summary>
        /// See MonoBehaviour.Start.
        /// </summary>
        protected override void Start()
        {
            if (!Application.isPlaying)
            {
                SetTracker();
            }

            if (m_InputField != null)
            {
                if (slider.wholeNumbers)
                {
                    m_InputField.inputField.contentType = InputField.ContentType.IntegerNumber;
                }
                else
                {
                    m_InputField.inputField.contentType = InputField.ContentType.DecimalNumber;
                }
            }

#if UNITY_EDITOR
            m_LastSliderValue = slider.value;
#endif
        }

        /// <summary>
        /// Sets the tracker.
        /// </summary>
        private void SetTracker()
        {
            m_Tracker.Clear();
            m_Tracker.Add(this, m_SliderContentTransform, DrivenTransformProperties.AnchorMinX);
            m_Tracker.Add(this, m_SliderContentTransform, DrivenTransformProperties.AnchorMaxX);
            m_Tracker.Add(this, m_SliderContentTransform, DrivenTransformProperties.AnchoredPositionX);
            m_Tracker.Add(this, m_SliderContentTransform, DrivenTransformProperties.SizeDeltaX);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Called by <see cref="EditorUpdate"/> in edit mode.
        /// </summary>
        public void OnEditorUpdate()
        {
            if (IsDestroyed())
            {
                EditorUpdate.onEditorUpdate -= OnEditorUpdate;
                return;
            }

            handleGraphicTransform.anchorMin = slider.handleRect.anchorMin;
            handleGraphicTransform.anchorMax = slider.handleRect.anchorMax;

            m_FillTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, fillAreaTransform.rect.width * m_HandleGraphicTransform.anchorMin.x);

            m_PopupText.text = slider.value.ToString("##.#");

            if (m_ValueText != null)
            {
				m_ValueText.text = slider.value == 0.0f ? "0" : slider.value.ToString("##.##");
            }

            if (m_InputField != null)
            {
				m_InputField.inputField.text = slider.value == 0.0f ? "0" : slider.value.ToString("##.##");
            }

            if (m_InputField != null)
            {
                if (slider.wholeNumbers)
                {
                    m_InputField.inputField.contentType = InputField.ContentType.IntegerNumber;
                }
                else
                {
                    m_InputField.inputField.contentType = InputField.ContentType.DecimalNumber;
                }
            }

            if (slider.wholeNumbers && m_HasDots)
            {
                if (m_NumberOfDots != SliderValueRange())
                {
                    RebuildDots();
                }
                for (int i = 0; i < m_DotGraphics.Length; i++)
                {
                    if (m_DotGraphics[i] != null)
                    {
                        if (slider.value > i)
                        {
                            m_DotGraphics[i].color = m_Interactable ? m_EnabledColor : m_DisabledColor;
                        }
                        else
                        {
                            m_DotGraphics[i].color = m_BackgroundColor;
                        }
                    }
                }
            }
            else
            {
                DestroyDots();
            }

            if (m_HandleGraphic)
            {
                m_HandleGraphic.color = m_Interactable ? m_EnabledColor : m_DisabledColor;
            }
            if (m_LeftContentTransform && m_LowLeftDisabledOpacity)
            {
                leftCanvasGroup.alpha = m_Interactable ? 1f : 0.5f;
            }
            if (m_RightContentTransform && m_LowRightDisabledOpacity)
            {
                rightCanvasGroup.alpha = m_Interactable ? 1f : 0.5f;
            }

            canvasGroup.interactable = m_Interactable;
            canvasGroup.blocksRaycasts = m_Interactable;

            if (m_InputField)
            {
                m_InputField.interactable = m_Interactable;
            }
        }
#endif

        /// <summary>
        /// See MonoBehaviour.Update.
        /// </summary>
        void Update()
        {
            if (TweenManager.TweenIsActive(m_HandleAnchorMinTweener))
            {
                m_FillTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, fillAreaTransform.rect.width * handleGraphicTransform.anchorMin.x);
            }

            if (m_InputField != null)
            {
                if (slider.wholeNumbers)
                {
                    m_InputField.inputField.contentType = InputField.ContentType.IntegerNumber;
                }
                else
                {
                    m_InputField.inputField.contentType = InputField.ContentType.DecimalNumber;
                }
            }

            if (m_IsSelected)
            {
                if (slider.wholeNumbers && m_HasDots)
                {
                    if (m_NumberOfDots != SliderValueRange())
                    {
                        RebuildDots();
                    }
                    for (int i = 0; i < m_DotGraphics.Length; i++)
                    {
                        if (slider.value > i)
                        {
                            m_DotGraphics[i].color = m_EnabledColor;
                        }
                        else
                        {
                            m_DotGraphics[i].color = m_BackgroundColor;
                        }
                    }
                }
                else
                {
                    DestroyDots();
                }

                if (m_HandleGraphic)
                {
                    m_HandleGraphic.color = m_Interactable ? m_EnabledColor : m_DisabledColor;
                }
            }

#if UNITY_EDITOR
            if (slider.value != m_LastSliderValue)
            {
                OnSliderValueChanged(m_LastSliderValue);
                m_LastSliderValue = slider.value;
            }
#endif
        }

        /// <summary>
        /// Destroys the dots.
        /// </summary>
        private void DestroyDots()
        {
            for (int i = 0; i < m_DotGraphics.Length; i++)
            {
                if (m_DotGraphics[i] != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(m_DotGraphics[i].gameObject);
                    }
                    else
                    {
                        DestroyImmediate(m_DotGraphics[i].gameObject);
                    }
                }
            }

            m_NumberOfDots = -1;

            m_DotGraphics = new Graphic[0];
        }

        /// <summary>
        /// Rebuilds the dots.
        /// </summary>
        private void RebuildDots()
        {
            DestroyDots();

            m_NumberOfDots = SliderValueRange();
            float dotDistance = 1 / (float)m_NumberOfDots;

            m_DotGraphics = new Graphic[m_NumberOfDots + 1];

            for (int i = 0; i < m_DotGraphics.Length; i++)
            {
                m_DotGraphics[i] = CreateDot();
                m_DotGraphics[i].rectTransform.SetAnchorX(dotDistance * i, dotDistance * i);

                if (slider.value > i)
                {
                    m_DotGraphics[i].color = m_Interactable ? m_EnabledColor : m_DisabledColor;
                }
                else
                {
                    m_DotGraphics[i].color = m_BackgroundColor;
                }
            }
        }

        /// <summary>
        /// Calculates the range between the min and max slider values.
        /// </summary>
        /// <returns>The range between the min and max slider values.</returns>
        private int SliderValueRange()
        {
            return Mathf.RoundToInt(slider.maxValue - slider.minValue);
        }

        /// <summary>
        /// Creates a dot.
        /// </summary>
        /// <returns>The Graphic of the created dot.</returns>
        private Graphic CreateDot()
        {
            RectTransform dot = PrefabManager.InstantiateGameObject(PrefabManager.ResourcePrefabs.sliderDot, m_SliderContentTransform).GetComponent<RectTransform>();
            dot.SetSiblingIndex(1);
            dot.anchoredPosition = Vector2.zero;
            dot.anchoredPosition = new Vector2(0f, 0.5f);
            return dot.GetComponent<Graphic>();
        }

        /// <summary>
        /// Animates the slider to the 'on' state.
        /// </summary>
        private void AnimateOn()
        {
            TweenManager.EndTween(m_HandleSizeTweener);
            TweenManager.EndTween(m_PopupScaleTweener);
            TweenManager.EndTween(m_HandlePositionYTweener);
            TweenManager.EndTween(m_PopupTextColorTweener);

            if (m_HasPopup)
            {
                m_HandleSizeTweener = TweenManager.TweenVector2(vector2 => handleGraphicTransform.sizeDelta = vector2,
                    handleGraphicTransform.sizeDelta, new Vector2(38, 38), m_AnimationDuration, 0f, null, false,
                    Tween.TweenType.SoftEaseOutQuint);

                m_HandlePositionYTweener = TweenManager.TweenFloat(
                    f => m_HandleGraphicTransform.anchoredPosition = new Vector2(m_HandleGraphicTransform.anchoredPosition.x, f),
                        m_HandleGraphicTransform.anchoredPosition.y, slider.wholeNumbers && m_HasDots ? 36 : 30,
                        m_AnimationDuration, 0, null, false, Tween.TweenType.EaseOutSept);

                m_PopupScaleTweener = TweenManager.TweenVector3(vector3 => m_PopupTransform.localScale = vector3,
                    m_PopupTransform.localScale, Vector3.one, m_AnimationDuration, 0, null, false, Tween.TweenType.EaseOutSept);
            }
            else
            {
                m_HandleSizeTweener = TweenManager.TweenVector2(vector2 => handleGraphicTransform.sizeDelta = vector2,
                    handleGraphicTransform.sizeDelta, new Vector2(24, 24), m_AnimationDuration, 0, null, false, Tween.TweenType.SoftEaseOutQuint);
            }

            m_PopupTextColorTweener = TweenManager.TweenColor(color => m_PopupText.color = color, () => m_PopupText.color,
               () => m_PopupText.color.WithAlpha(1f), m_AnimationDuration * 0.66f, m_AnimationDuration * 0.33f);
        }

        /// <summary>
        /// Animates the slider to the 'off' state.
        /// </summary>
        private void AnimateOff()
        {
            TweenManager.EndTween(m_HandleSizeTweener);
            TweenManager.EndTween(m_PopupScaleTweener);
            TweenManager.EndTween(m_HandlePositionYTweener);
            TweenManager.EndTween(m_PopupTextColorTweener);

            if (m_HasPopup)
            {
                m_HandlePositionYTweener =
                    TweenManager.TweenFloat(
                        f => m_HandleGraphicTransform.anchoredPosition = new Vector2(m_HandleGraphicTransform.anchoredPosition.x, f),
                        m_HandleGraphicTransform.anchoredPosition.y,
                        MaterialUIScaler.GetParentScaler(transform).targetCanvas.pixelPerfect ? 1f : 0f, m_AnimationDuration, 0, null, false, Tween.TweenType.EaseOutCubed);

                m_PopupScaleTweener = TweenManager.TweenVector3(vector3 => m_PopupTransform.localScale = vector3,
                    m_PopupTransform.localScale, Vector3.zero, m_AnimationDuration);
            }

            m_HandleSizeTweener = TweenManager.TweenVector2(vector2 => handleGraphicTransform.sizeDelta = vector2,
                    handleGraphicTransform.sizeDelta, new Vector2(16, 16), m_AnimationDuration, 0, null, false,
                    Tween.TweenType.EaseOutSept);

            m_PopupTextColorTweener = TweenManager.TweenColor(color => m_PopupText.color = color, m_PopupText.color,
                    m_PopupText.color.WithAlpha(0f), m_AnimationDuration * 0.25f);
        }

        /// <summary>
        /// Called when [input change].
        /// </summary>
        /// <param name="value">The value.</param>
        public void OnInputChange(string value)
        {
            float floatValue;
            if (float.TryParse(value, out floatValue))
            {
                m_CurrentInputValue = floatValue;
                if (floatValue >= slider.minValue && floatValue <= slider.maxValue)
                {
                    slider.value = floatValue;
                }
            }
        }

        /// <summary>
        /// Called when [input end].
        /// </summary>
        public void OnInputEnd()
        {
            if (m_InputField != null)
            {
                slider.value = m_CurrentInputValue;
                m_InputField.inputField.text = slider.value.ToString();
            }
        }

        /// <summary>
        /// Called when [slider value changed].
        /// </summary>
        /// <param name="value">The value.</param>
        public void OnSliderValueChanged(float value)
        {
            TweenManager.EndTween(m_HandleAnchorMinTweener);
            TweenManager.EndTween(m_HandleAnchorMaxTweener);

            if (slider.wholeNumbers && SliderValueRange() < 100)
            {
                m_HandleAnchorMinTweener = TweenManager.TweenFloat(
                        f => handleGraphicTransform.anchorMin = new Vector2(f, handleGraphicTransform.anchorMin.y),
                        handleGraphicTransform.anchorMin.x, m_Slider.handleRect.anchorMin.x, m_AnimationDuration * 0.5f,
                        0, null, false, Tween.TweenType.EaseOutSept);

                m_HandleAnchorMaxTweener = TweenManager.TweenFloat(
                        f => handleGraphicTransform.anchorMax = new Vector2(f, handleGraphicTransform.anchorMax.y),
                        handleGraphicTransform.anchorMax.x, m_Slider.handleRect.anchorMax.x, m_AnimationDuration * 0.5f,
                        0, null, false, Tween.TweenType.EaseOutSept);
            }
            else
            {
                Vector2 anchor = handleGraphicTransform.anchorMin;
                anchor.x = m_Slider.handleRect.anchorMin.x;
                handleGraphicTransform.anchorMin = anchor;

                anchor = handleGraphicTransform.anchorMax;
                anchor.x = m_Slider.handleRect.anchorMax.x;
                handleGraphicTransform.anchorMax = anchor;
                handleGraphicTransform.anchoredPosition = new Vector2(0f, handleGraphicTransform.anchoredPosition.y);
            }

            m_FillTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, fillAreaTransform.rect.width * m_HandleGraphicTransform.anchorMin.x);

            m_PopupText.text = slider.value.ToString("#0.#");

            if (m_ValueText != null)
            {
                m_ValueText.text = slider.value.ToString("#0.##");
            }

            if (m_InputField != null)
            {
                m_InputField.inputField.text = slider.value.ToString("#0.##");
            }
        }

        /// <summary>
        /// Called when [before validate].
        /// </summary>
        public void OnBeforeValidate()
        {
            if (m_BackgroundGraphic)
            {
                m_BackgroundColor = m_BackgroundGraphic.color;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// See MonoBehaviour.OnValidate.
        /// </summary>
        protected override void OnValidate()
        {
            UpdateColors();

            LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
        }
#endif

        public void UpdateColors()
        {
            if (m_BackgroundGraphic)
            {
                m_BackgroundGraphic.color = m_BackgroundColor;
            }
            if (m_HandleGraphic)
            {
                m_HandleGraphic.color = m_Interactable ? m_EnabledColor : m_DisabledColor;
            }

            for (int i = 0; i < m_DotGraphics.Length; i++)
            {
                if (m_DotGraphics[i] == null) continue;

                if (slider.value > i)
                {
                    m_DotGraphics[i].color = m_Interactable ? m_EnabledColor : m_DisabledColor;
                }
                else
                {
                    m_DotGraphics[i].color = m_BackgroundColor;
                }
            }

            if (m_LeftContentTransform)
            {
                leftCanvasGroup.alpha = m_LowLeftDisabledOpacity ? (m_Interactable ? 1f : 0.5f) : 1f;
            }
            if (m_RightContentTransform)
            {
                rightCanvasGroup.alpha = m_LowRightDisabledOpacity ? (m_Interactable ? 1f : 0.5f) : 1f;
            }
        }

        /// <summary>
        /// See MonoBehaviour.OnRectTransformDimensionsChange.
        /// </summary>
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            SetLayoutHorizontal();
        }

        /// <summary>
        /// See MonoBehaviour.OnRectTransformParentChanged.
        /// </summary>
        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            SetLayoutHorizontal();
        }

        /// <summary>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            AnimateOn();
        }

        /// <summary>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            AnimateOff();
        }

        /// <summary>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnSelect(BaseEventData eventData)
        {
            AnimateOn();
            m_IsSelected = true;
        }

        /// <summary>
        /// Called by the EventSystem when a new object is being selected.
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnDeselect(BaseEventData eventData)
        {
            AnimateOff();
            m_IsSelected = false;
        }

        /// <summary>
        /// The minWidth, preferredWidth, and flexibleWidth values may be calculated in this callback.
        /// </summary>
        public void CalculateLayoutInputHorizontal()
        {
            if (m_LeftContentTransform)
            {
                ILayoutElement[] leftElements = m_LeftContentTransform.GetComponentsInChildren<ILayoutElement>();

                leftElements = leftElements.Reverse().ToArray();

                for (int i = 0; i < leftElements.Length; i++)
                {
                    leftElements[i].CalculateLayoutInputHorizontal();
                }

                m_LeftWidth = LayoutUtility.GetPreferredWidth(m_LeftContentTransform) + 16;
            }

            if (m_RightContentTransform)
            {
                ILayoutElement[] rightElements = m_RightContentTransform.GetComponentsInChildren<ILayoutElement>();

                rightElements = rightElements.Reverse().ToArray();

                for (int i = 0; i < rightElements.Length; i++)
                {
                    rightElements[i].CalculateLayoutInputHorizontal();
                }

                m_RightWidth = LayoutUtility.GetPreferredWidth(m_RightContentTransform) + 16;
            }
            else
            {
                m_RightWidth = 0f;
            }

            m_Width = Mathf.Max(m_ManualPreferredWidth, m_LeftWidth + m_RightWidth + ((slider.wholeNumbers && m_HasDots) ? 6f : 0f));
        }

        /// <summary>
        /// Sets the layout horizontal.
        /// </summary>
        public void SetLayoutHorizontal()
        {
            SetTracker();
            if (m_LeftContentTransform)
            {
                ILayoutController[] leftControllers = m_LeftContentTransform.GetComponentsInChildren<ILayoutController>();

                for (int i = 0; i < leftControllers.Length; i++)
                {
                    leftControllers[i].SetLayoutHorizontal();
                }
            }

            if (m_RightContentTransform)
            {
                ILayoutController[] rightControllers = m_RightContentTransform.GetComponentsInChildren<ILayoutController>();

                for (int i = 0; i < rightControllers.Length; i++)
                {
                    rightControllers[i].SetLayoutHorizontal();
                }
            }

            m_SliderContentTransform.anchorMin = new Vector2(0, m_SliderContentTransform.anchorMin.y);
            m_SliderContentTransform.anchorMax = new Vector2(1, m_SliderContentTransform.anchorMax.y);

            m_SliderContentTransform.anchoredPosition = new Vector2(m_LeftWidth + ((slider.wholeNumbers && m_HasDots) ? 3f : 0f), m_SliderContentTransform.anchoredPosition.y);
            m_SliderContentTransform.sizeDelta = new Vector2(-(m_LeftWidth + m_RightWidth) - ((slider.wholeNumbers && m_HasDots) ? 6f : 0f), m_SliderContentTransform.sizeDelta.y);
        }

        /// <summary>
        /// The minHeight, preferredHeight, and flexibleHeight values may be calculated in this callback.
        /// </summary>
        public void CalculateLayoutInputVertical()
        {
            float leftHeight = 0;
            float rightHeight = 0;

            if (m_LeftContentTransform)
            {
                ILayoutElement[] elements = m_LeftContentTransform.GetComponentsInChildren<ILayoutElement>();
                elements = elements.Reverse().ToArray();
                for (int i = 0; i < elements.Length; i++)
                {
                    elements[i].CalculateLayoutInputVertical();
                }

                leftHeight = LayoutUtility.GetPreferredHeight(m_LeftContentTransform);
            }

            if (m_RightContentTransform)
            {
                ILayoutElement[] elements = m_RightContentTransform.GetComponentsInChildren<ILayoutElement>();
                elements = elements.Reverse().ToArray();
                for (int i = 0; i < elements.Length; i++)
                {
                    elements[i].CalculateLayoutInputVertical();
                }

                rightHeight = LayoutUtility.GetPreferredHeight(m_RightContentTransform);
            }

            m_Height = Mathf.Max(LayoutUtility.GetPreferredHeight(m_SliderContentTransform), Mathf.Max(leftHeight, rightHeight));
            m_Height = Mathf.Max(m_Height, 24f);
        }

        /// <summary>
        /// Sets the layout vertical.
        /// </summary>
        public void SetLayoutVertical()
        {
            if (m_LeftContentTransform)
            {
                ILayoutController[] controllers = m_LeftContentTransform.GetComponentsInChildren<ILayoutController>();
                for (int i = 0; i < controllers.Length; i++)
                {
                    controllers[i].SetLayoutVertical();
                }
            }

            if (m_RightContentTransform)
            {
                ILayoutController[] controllers = m_RightContentTransform.GetComponentsInChildren<ILayoutController>();
                for (int i = 0; i < controllers.Length; i++)
                {
                    controllers[i].SetLayoutVertical();
                }
            }

            if (rootCanvas != null && !m_IsSelected)
            {
                Vector2 tempVector2 = m_HandleGraphic.rectTransform.anchoredPosition;
                tempVector2.y = (rootCanvas.pixelPerfect) ? 1f : 0f;
                m_HandleGraphic.rectTransform.anchoredPosition = tempVector2;
            }
        }

        /// <summary>
        /// The minimum width this layout element may be allocated.
        /// </summary>
        public float minWidth
        {
            get { return -1; }
        }

        /// <summary>
        /// The preferred width this layout element should be allocated if there is sufficient space.
        /// </summary>
        public float preferredWidth
        {
            get { return m_HasManualPreferredWidth ? m_Width : -1; }
        }

        /// <summary>
        /// The extra relative width this layout element should be allocated if there is additional available space.
        /// </summary>
        public float flexibleWidth
        {
            get { return -1; }
        }

        /// <summary>
        /// The minimum height this layout element may be allocated.
        /// </summary>
        public float minHeight
        {
            get { return -1; }
        }

        /// <summary>
        /// The preferred height this layout element should be allocated if there is sufficient space.
        /// </summary>
        public float preferredHeight
        {
            get { return m_Height; }
        }

        /// <summary>
        /// The extra relative height this layout element should be allocated if there is additional available space.
        /// </summary>
        public float flexibleHeight
        {
            get { return -1; }
        }

        /// <summary>
        /// The layout priority of this component.
        /// </summary>
        public int layoutPriority
        {
            get { return -1; }
        }
    }
}