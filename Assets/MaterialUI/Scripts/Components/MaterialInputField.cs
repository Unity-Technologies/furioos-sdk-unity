//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
#if UNITY_EDITOR
    /// <summary>
    /// Component that handles an InputField control.
    /// </summary>
    /// <seealso cref="UnityEngine.EventSystems.UIBehaviour" />
    /// <seealso cref="UnityEngine.UI.ILayoutGroup" />
    /// <seealso cref="UnityEngine.UI.ILayoutElement" />
    /// <seealso cref="UnityEngine.EventSystems.ISelectHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IDeselectHandler" />
    [InitializeOnLoad]
#endif
    [ExecuteInEditMode]
    [RequireComponent(typeof(InputField))]
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("MaterialUI/Material Input Field", 100)]
    public class MaterialInputField : UIBehaviour, ILayoutGroup, ILayoutElement, ISelectHandler, IDeselectHandler
    {
        /// <summary>
        /// The selection states that the InputField can be in, used for calculating which colors to apply at any given time..
        /// </summary>
        public enum ColorSelectionState
        {
            EnabledSelected,
            EnabledDeselected,
            DisabledSelected,
            DisabledDeselected
        }

        /// <summary>
        /// The hint text of the input field.
        /// </summary>
        [SerializeField]
        private string m_HintText;
        /// <summary>
        /// The hint text of the input field.
        /// </summary>
        public string hintText
        {
            get { return m_HintText; }
            set
            {
                m_HintText = value;
                hintTextObject.text = value;

#if UNITY_EDITOR
                m_LastHintText = value;
#endif
            }
        }

        /// <summary>
        /// Should the hint text float above the input text when the inputField is selected or contains text?
        /// </summary>
        [SerializeField]
        private bool m_FloatingHint = true;
        /// <summary>
        /// Should the hint text float above the input text when the inputField is selected or contains text?
        /// </summary>
        public bool floatingHint
        {
            get { return m_FloatingHint; }
            set
            {
                m_FloatingHint = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// Does this input field validate text?
        /// </summary>
        [SerializeField]
        private bool m_HasValidation = true;
        /// <summary>
        /// Does this input field validate text?
        /// </summary>
        public bool hasValidation
        {
            get { return m_HasValidation; }
            set
            {
                m_HasValidation = value;
                SetLayoutDirty();
                ValidateText();
            }
        }

        /// <summary>
        /// Should the text be validated upon start, if applicable?
        /// </summary>
        [SerializeField]
        private bool m_ValidateOnStart;
        /// <summary>
        /// Should the text be validated upon start, if applicable?
        /// </summary>
        public bool validateOnStart
        {
            get { return m_ValidateOnStart; }
            set
            {
                m_ValidateOnStart = value;
                if (value)
                {
                    ValidateText();
                }
            }
        }

        /// <summary>
        /// Should the input field have a character counter?
        /// </summary>
        [SerializeField]
        private bool m_HasCharacterCounter = true;
        /// <summary>
        /// Should the input field have a character counter?
        /// </summary>
        public bool hasCharacterCounter
        {
            get { return m_HasCharacterCounter; }
            set
            {
                m_HasCharacterCounter = value;
                m_CounterText.gameObject.SetActive(m_HasCharacterCounter);
                SetLayoutDirty();
                UpdateCounter();
            }
        }

        /// <summary>
        /// Should the character limit match the attached InoputField's limit?
        /// </summary>
        [SerializeField]
        private bool m_MatchInputFieldCharacterLimit = true;
        /// <summary>
        /// Should the character limit match the attached InoputField's limit?
        /// </summary>
        public bool matchInputFieldCharacterLimit
        {
            get { return m_MatchInputFieldCharacterLimit; }
            set
            {
                m_MatchInputFieldCharacterLimit = value;
                SetLayoutDirty();
                UpdateCounter();
            }
        }

        /// <summary>
        /// The character limit of the input field.
        /// </summary>
        [SerializeField]
        private int m_CharacterLimit;
        /// <summary>
        /// The character limit of the input field.
        /// </summary>
        public int characterLimit
        {
            get { return m_CharacterLimit; }
            set
            {
                m_CharacterLimit = value;
                SetLayoutDirty();
                UpdateCounter();
            }
        }

        /// <summary>
        /// The size of the hint text when floating.
        /// </summary>
        [SerializeField]
        private int m_FloatingHintFontSize = 12;
        /// <summary>
        /// The size of the hint text when floating.
        /// </summary>
        public int floatingHintFontSize
        {
            get { return m_FloatingHintFontSize; }
            set
            {
                m_FloatingHintFontSize = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// Should the height of the input field object rect be controlled by the height of the content?
        /// </summary>
        [SerializeField]
        private bool m_FitHeightToContent = true;
        /// <summary>
        /// Should the height of the input field object rect be controlled by the height of the content?
        /// </summary>
        public bool fitHeightToContent
        {
            get { return m_FitHeightToContent; }
            set
            {
                m_FitHeightToContent = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// The amount to offset the position of the left content RectTransform.
        /// </summary>
        [SerializeField]
        private Vector2 m_LeftContentOffset;

        /// <summary>
        /// The amount to offset the position of the right content RectTransform.
        /// </summary>
        [SerializeField]
        private Vector2 m_RightContentOffset;

        /// <summary>
        /// The manually-set preferredWidth of the input field rect.
        /// </summary>
        [SerializeField]
        private bool m_ManualPreferredWidth;
        /// <summary>
        /// The manually-set preferredWidth of the input field rect.
        /// </summary>
        public bool manualPreferredWidth
        {
            get { return m_ManualPreferredWidth; }
            set
            {
                m_ManualPreferredWidth = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// The manually-set preferredHeight of the input field rect.
        /// </summary>
        [SerializeField]
        private bool m_ManualPreferredHeight;
        /// <summary>
        /// The manually-set preferredHeight of the input field rect.
        /// </summary>
        public bool manualPreferredHeight
        {
            get { return m_ManualPreferredHeight; }
            set
            {
                m_ManualPreferredHeight = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// The manually-set size of the input field rect.
        /// </summary>
        [SerializeField]
        private Vector2 m_ManualSize;
        /// <summary>
        /// The manually-set size of the input field rect.
        /// </summary>
        public Vector2 manualSize
        {
            get { return m_ManualSize; }
            set
            {
                m_ManualSize = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// The GameObject containing a custom ITextValidator to use to validate text.
        /// </summary>
        [SerializeField]
        private GameObject m_TextValidator;
        /// <summary>
        /// The GameObject containing a custom ITextValidator to use to validate text.
        /// </summary>
        public GameObject textValidator
        {
            get { return m_TextValidator; }
            set
            {
                m_TextValidator = value;
                ValidateText();
            }
        }

        /// <summary>
        /// The RectTransform of the input field.
        /// </summary>
        [SerializeField]
        private RectTransform m_RectTransform;
        /// <summary>
        /// The RectTransform of the input field.
        /// If null, automatically gets the attached RectTransform.
        /// </summary>
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

        /// <summary>
        /// The attached InputField.
        /// </summary>
        [SerializeField]
        private InputField m_InputField;
        /// <summary>
        /// The attached InputField.
        /// If null, automatically gets the attached InputField.
        /// </summary>
        public InputField inputField
        {
            get
            {
                if (m_InputField == null)
                {
                    m_InputField = GetComponent<InputField>();
                }
                return m_InputField;
            }
        }

        /// <summary>
        /// The RectTransform of the input text.
        /// </summary>
        [SerializeField]
        private RectTransform m_InputTextTransform;
        /// <summary>
        /// The RectTransform of the input text.
        /// If null, automatically gets the InputField's input Text's RectTransform, if one exists.
        /// </summary>
        public RectTransform inputTextTransform
        {
            get
            {
                if (m_InputTextTransform == null)
                {
                    if (m_InputField != null)
                    {
                        if (m_InputField.textComponent != null)
                        {
                            m_InputTextTransform = m_InputField.textComponent.GetComponent<RectTransform>();
                        }
                    }
                }

                return m_InputTextTransform;
            }
        }

        /// <summary>
        /// The RectTransform of the hint text.
        /// </summary>
        [SerializeField]
        private RectTransform m_HintTextTransform;
        /// <summary>
        /// The RectTransform of the hint text.
        /// </summary>
        public RectTransform hintTextTransform
        {
            get { return m_HintTextTransform; }
            set
            {
                m_HintTextTransform = value;
                SetLayoutDirty();
                UpdateCounter();
                UpdateSelectionState();
                ValidateText();
            }
        }

        /// <summary>
        /// The RectTransform of the counter text.
        /// </summary>
        [SerializeField]
        private RectTransform m_CounterTextTransform;
        /// <summary>
        /// The RectTransform of the counter text.
        /// </summary>
        public RectTransform counterTextTransform
        {
            get { return m_CounterTextTransform; }
            set
            {
                m_CounterTextTransform = value;
                SetLayoutDirty();
                UpdateCounter();
                UpdateSelectionState();
                ValidateText();
            }
        }

        /// <summary>
        /// The RectTransform of the validation text.
        /// </summary>
        [SerializeField]
        private RectTransform m_ValidationTextTransform;
        /// <summary>
        /// The RectTransform of the validation text.
        /// </summary>
        public RectTransform validationTextTransform
        {
            get { return m_ValidationTextTransform; }
            set
            {
                m_ValidationTextTransform = value;
                SetLayoutDirty();
                UpdateCounter();
                UpdateSelectionState();
                ValidateText();
            }
        }

        /// <summary>
        /// The RectTransform of the bottom (background) line.
        /// </summary>
        [SerializeField]
        private RectTransform m_LineTransform;
        /// <summary>
        /// The RectTransform of the bottom (background) line.
        /// </summary>
        public RectTransform lineTransform
        {
            get { return m_LineTransform; }
            set
            {
                m_LineTransform = value;
                SetLayoutDirty();
                UpdateSelectionState();
            }
        }

        /// <summary>
        /// The RectTransform of the bottom (foreground) line.
        /// </summary>
        [SerializeField]
        private RectTransform m_ActiveLineTransform;
        /// <summary>
        /// The RectTransform of the bottom (foreground) line.
        /// </summary>
        public RectTransform activeLineTransform
        {
            get { return m_ActiveLineTransform; }
            set
            {
                m_ActiveLineTransform = value;
                SetLayoutDirty();
                UpdateSelectionState();
            }
        }

        /// <summary>
        /// The RectTransform of the left content (icon/label).
        /// </summary>
        [SerializeField]
        private RectTransform m_LeftContentTransform;
        /// <summary>
        /// The RectTransform of the left content (icon/label).
        /// </summary>
        public RectTransform leftContentTransform
        {
            get { return m_LeftContentTransform; }
            set
            {
                m_LeftContentTransform = value;
                SetLayoutDirty();
                UpdateSelectionState();
            }
        }

        /// <summary>
        /// The RectTransform of the right content (icon/label).
        /// </summary>
        [SerializeField]
        private RectTransform m_RightContentTransform;
        /// <summary>
        /// The RectTransform of the right content (icon/label).
        /// </summary>
        public RectTransform rightContentTransform
        {
            get { return m_RightContentTransform; }
            set
            {
                m_RightContentTransform = value;
                SetLayoutDirty();
                UpdateSelectionState();
            }
        }

        /// <summary>
        /// The input Text.
        /// </summary>
        [SerializeField]
        private Text m_InputText;
        /// <summary>
        /// The input Text.
        /// </summary>
        public Text inputText
        {
            get
            {
                if (m_InputText == null)
                {
                    if (inputTextTransform != null)
                    {
                        m_InputText = inputTextTransform.GetComponent<Text>();
                    }
                }
                return m_InputText;
            }
        }

        /// <summary>
        /// The hint Text.
        /// </summary>
        [SerializeField]
        private Text m_HintTextObject;
        /// <summary>
        /// The hint Text.
        /// If null, automatically gets the hint text RectTransform's attached Text, if one exists.
        /// </summary>
        public Text hintTextObject
        {
            get
            {
                if (m_HintTextObject == null)
                {
                    if (m_HintTextTransform != null)
                    {
                        m_HintTextObject = m_HintTextTransform.GetComponent<Text>();
                    }
                }
                return m_HintTextObject;
            }
        }

        /// <summary>
        /// The character counter Text.
        /// </summary>
        [SerializeField]
        private Text m_CounterText;
        /// <summary>
        /// The character counter Text.
        /// If null, automatically gets the counter text RectTransform's attached Text, if one exists.
        /// </summary>
        public Text counterText
        {
            get
            {
                if (m_CounterText == null)
                {
                    if (m_CounterTextTransform != null)
                    {
                        m_CounterText = m_CounterTextTransform.GetComponent<Text>();
                    }
                }
                return m_CounterText;
            }
        }

        /// <summary>
        /// The validation Text.
        /// </summary>
        [SerializeField]
        private Text m_ValidationText;
        /// <summary>
        /// The validation Text.
        /// If null, automatically gets the validation text RectTransform's attached Text, if one exists.
        /// </summary>
        public Text validationText
        {
            get
            {
                if (m_ValidationText == null)
                {
                    if (m_ValidationTextTransform != null)
                    {
                        m_ValidationText = m_ValidationTextTransform.GetComponent<Text>();
                    }
                }
                return m_ValidationText;
            }
        }

        /// <summary>
        /// The Image of the bottom (background) line.
        /// </summary>
        [SerializeField]
        private Image m_LineImage;
        /// <summary>
        /// The Image of the bottom (background) line.
        /// If null, automatically gets the line RectTransform's attached Image, if one exists.
        /// </summary>
        public Image lineImage
        {
            get
            {
                if (m_LineImage == null)
                {
                    if (m_LineTransform != null)
                    {
                        m_LineImage = m_LineTransform.GetComponent<Image>();
                    }
                }
                return m_LineImage;
            }
        }

        /// <summary>
        /// The CanvasGroup of the bottom (foreground) line.
        /// </summary>
        [SerializeField]
        private CanvasGroup m_ActiveLineCanvasGroup;
        /// <summary>
        /// The CanvasGroup of the bottom (foreground) line.
        /// If null, automatically gets the line RectTransform's attached CanvasGroup if one exists, otherwise will add one.
        /// </summary>
        public CanvasGroup activeLineCanvasGroup
        {
            get
            {
                if (m_ActiveLineCanvasGroup == null)
                {
                    if (m_ActiveLineTransform != null)
                    {
                        m_ActiveLineCanvasGroup = m_ActiveLineTransform.GetComponent<CanvasGroup>();
                    }
                }
                return m_ActiveLineCanvasGroup;
            }
        }

        /// <summary>
        /// The CanvasGroup of the hint text.
        /// </summary>
        [SerializeField]
        private CanvasGroup m_HintTextCanvasGroup;
        /// <summary>
        /// The CanvasGroup of the hint text.
        /// If null, automatically gets the hint text RectTransform's attached CanvasGroup, if one exists.
        /// </summary>
        public CanvasGroup hintTextCanvasGroup
        {
            get
            {
                if (m_HintTextCanvasGroup == null)
                {
                    if (m_HintTextTransform != null)
                    {
                        m_HintTextCanvasGroup = m_HintTextTransform.GetComponent<CanvasGroup>();
                    }
                }
                return m_HintTextCanvasGroup;
            }
        }

        /// <summary>
        /// The CanvasGroup of the validation text.
        /// </summary>
        [SerializeField]
        private CanvasGroup m_ValidationCanvasGroup;
        /// <summary>
        /// The CanvasGroup of the validation text.
        /// If null, automatically gets the validation text RectTransform's attached CanvasGroup, if one exists.
        /// </summary>
        public CanvasGroup validationCanvasGroup
        {
            get
            {
                if (m_ValidationCanvasGroup == null)
                {
                    if (m_ValidationTextTransform != null)
                    {
                        m_ValidationCanvasGroup = m_ValidationTextTransform.GetComponent<CanvasGroup>();
                    }
                }
                return m_ValidationCanvasGroup;
            }
        }

        /// <summary>
        /// The root MaterialUIScaler.
        /// </summary>
        private MaterialUIScaler m_MaterialUiScaler;
        /// <summary>
        /// The root MaterialUIScaler.
        /// If null, automatically gets the root scaler, if one exists.
        /// </summary>
        public MaterialUIScaler materialUiScaler
        {
            get
            {
                if (m_MaterialUiScaler == null)
                {
                    m_MaterialUiScaler = MaterialUIScaler.GetParentScaler(transform);
                }
                return m_MaterialUiScaler;
            }
        }

        /// <summary>
        /// The text carat's RectTransform.
        /// </summary>
        private RectTransform m_CaretTransform;
        /// <summary>
        /// The text carat's RectTransform.
        /// If null, automatically gets the carat's RectTransform, if one exists.
        /// </summary>
        public RectTransform caretTransform
        {
            get
            {
                if (m_CaretTransform == null)
                {
                    LayoutElement[] elements = GetComponentsInChildren<LayoutElement>();

                    for (int i = 0; i < elements.Length; i++)
                    {
                        if (elements[i].name == name + " Input Caret")
                        {
                            m_CaretTransform = (RectTransform)elements[i].transform;
                        }
                    }
                }
                return m_CaretTransform;
            }
        }

        /// <summary>
        /// The color of the left content (label/icon) when the input field is active.
        /// </summary>
        [SerializeField]
        private Color m_LeftContentActiveColor = MaterialColor.iconDark;
        /// <summary>
        /// The color of the left content (label/icon) when the input field is active.
        /// </summary>
        public Color leftContentActiveColor
        {
            get { return m_LeftContentActiveColor; }
            set { m_LeftContentActiveColor = value; }
        }

        /// <summary>
        /// The color of the left content (label/icon) when the input field is inactive.
        /// </summary>
        [SerializeField]
        private Color m_LeftContentInactiveColor = MaterialColor.disabledDark;
        /// <summary>
        /// The color of the left content (label/icon) when the input field is inactive.
        /// </summary>
        public Color leftContentInactiveColor
        {
            get { return m_LeftContentInactiveColor; }
            set { m_LeftContentInactiveColor = value; }
        }

        /// <summary>
        /// The color of the right content (label/icon) when the input field is active.
        /// </summary>
        [SerializeField]
        private Color m_RightContentActiveColor = MaterialColor.iconDark;
        /// <summary>
        /// The color of the right content (label/icon) when the input field is active.
        /// </summary>
        public Color rightContentActiveColor
        {
            get { return m_RightContentActiveColor; }
            set { m_RightContentActiveColor = value; }
        }

        /// <summary>
        /// The color of the right content (label/icon) when the input field is inactive.
        /// </summary>
        [SerializeField]
        private Color m_RightContentInactiveColor = MaterialColor.disabledDark;
        /// <summary>
        /// The color of the right content (label/icon) when the input field is inactive.
        /// </summary>
        public Color rightContentInactiveColor
        {
            get { return m_RightContentInactiveColor; }
            set { m_RightContentInactiveColor = value; }
        }

        /// <summary>
        /// The color of the hint text when the input field is active.
        /// </summary>
        [SerializeField]
        private Color m_HintTextActiveColor = MaterialColor.textHintDark;
        /// <summary>
        /// The color of the hint text when the input field is active.
        /// </summary>
        public Color hintTextActiveColor
        {
            get { return m_HintTextActiveColor; }
            set { m_HintTextActiveColor = value; }
        }

        /// <summary>
        /// The color of the hint text when the input field is inactive.
        /// </summary>
        [SerializeField]
        private Color m_HintTextInactiveColor = MaterialColor.disabledDark;
        /// <summary>
        /// The color of the hint text when the input field is inactive.
        /// </summary>
        public Color hintTextInactiveColor
        {
            get { return m_HintTextInactiveColor; }
            set { m_HintTextInactiveColor = value; }
        }

        /// <summary>
        /// The color of the bottom (foreground) line when the input field is active.
        /// </summary>
        [SerializeField]
        private Color m_LineActiveColor = Color.black;
        /// <summary>
        /// The color of the bottom (foreground) line when the input field is active.
        /// </summary>
        public Color lineActiveColor
        {
            get { return m_LineActiveColor; }
            set { m_LineActiveColor = value; }
        }

        /// <summary>
        /// The color of the bottom (foreground) line when the input field is inactive.
        /// </summary>
        [SerializeField]
        private Color m_LineInactiveColor = MaterialColor.disabledDark;
        /// <summary>
        /// The color of the bottom (foreground) line when the input field is inactive.
        /// </summary>
        public Color lineInactiveColor
        {
            get { return m_LineInactiveColor; }
            set { m_LineInactiveColor = value; }
        }

        /// <summary>
        /// The color of the validation text when the input field is active.
        /// </summary>
        [SerializeField]
        private Color m_ValidationActiveColor = MaterialColor.red500;
        /// <summary>
        /// The color of the validation text when the input field is active.
        /// </summary>
        public Color validationActiveColor
        {
            get { return m_ValidationActiveColor; }
            set { m_ValidationActiveColor = value; }
        }

        /// <summary>
        /// The color of the validation text when the input field is inactive.
        /// </summary>
        [SerializeField]
        private Color m_ValidationInactiveColor = MaterialColor.disabledDark;
        /// <summary>
        /// The color of the validation text when the input field is inactive.
        /// </summary>
        public Color validationInactiveColor
        {
            get { return m_ValidationInactiveColor; }
            set { m_ValidationInactiveColor = value; }
        }

        /// <summary>
        /// The color of the counter text when the input field is active.
        /// </summary>
        [SerializeField]
        private Color m_CounterActiveColor = MaterialColor.textSecondaryDark;
        /// <summary>
        /// The color of the counter text when the input field is active.
        /// </summary>
        public Color counterActiveColor
        {
            get { return m_CounterActiveColor; }
            set { m_CounterActiveColor = value; }
        }

        /// <summary>
        /// The color of the counter text when the input field is inactive.
        /// </summary>
        [SerializeField]
        private Color m_CounterInactiveColor = MaterialColor.disabledDark;
        /// <summary>
        /// The color of the counter text when the input field is inactive.
        /// </summary>
        public Color counterInactiveColor
        {
            get { return m_CounterInactiveColor; }
            set { m_CounterInactiveColor = value; }
        }

        /// <summary>
        /// The left content (label/icon) Graphic.
        /// </summary>
        [SerializeField]
        private Graphic m_LeftContentGraphic;
        /// <summary>
        /// The left content (label/icon) Graphic.
        /// </summary>
        public Graphic leftContentGraphic
        {
            get { return m_LeftContentGraphic; }
            set { m_LeftContentGraphic = value; }
        }

        /// <summary>
        /// The right content (label/icon) Graphic.
        /// </summary>
        [SerializeField]
        private Graphic m_RightContentGraphic;
        /// <summary>
        /// The right content (label/icon) Graphic.
        /// </summary>
        public Graphic rightContentGraphic
        {
            get { return m_RightContentGraphic; }
            set { m_RightContentGraphic = value; }
        }

        /// <summary>
        /// Determines the tween progression of the hint text.
        /// </summary>
        [SerializeField]
        private float m_HintTextFloatingValue;
        /// <summary>
        /// Determines the tween progression of the hint text.
        /// </summary>
        public float hintTextFloatingValue
        {
            get { return m_HintTextFloatingValue; }
            set { m_HintTextFloatingValue = value; }
        }

        /// <summary>
        /// Is the input field interactable?
        /// </summary>
        [SerializeField]
        private bool m_Interactable = true;
        /// <summary>
        /// Is the input field interactable?
        /// </summary>
        public bool interactable
        {
            get { return m_Interactable; }
            set
            {
                m_Interactable = value;
                UpdateSelectionState();
                inputField.interactable = value;
            }
        }

        /// <summary>
        /// The attached CanvasGroup.
        /// </summary>
        private CanvasGroup m_CanvasGroup;
        /// <summary>
        /// The attached CanvasGroup.
        /// If null, automatically gets the attached CanvasGroup if one exists.
        /// </summary>
        public CanvasGroup canvasGroup
        {
            get
            {
                if (!m_CanvasGroup)
                {
                    m_CanvasGroup = gameObject.GetComponent<CanvasGroup>();
                }
                return m_CanvasGroup;
            }
        }

        /// <summary>
        /// The sprite to use for the line when the input field is disabled.
        /// </summary>
        private static Sprite m_LineDisabledSprite;
        /// <summary>
        /// The sprite to use for the line when the input field is disabled.
        /// If null, creates one.
        /// </summary>
        private static Sprite lineDisabledSprite
        {
            get
            {
                if (m_LineDisabledSprite == null)
                {
                    Color[] colors =
                    {
                        Color.white,
                        Color.white,
                        Color.clear,
                        Color.clear,
                        Color.white,
                        Color.white,
                        Color.clear,
                        Color.clear
                    };

                    Texture2D texture = new Texture2D(4, 2, TextureFormat.ARGB32, false);
                    texture.filterMode = FilterMode.Point;
                    texture.SetPixels(colors);
                    texture.hideFlags = HideFlags.HideAndDontSave;

                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 4, 2), new Vector2(0.5f, 0.5f));
                    sprite.hideFlags = HideFlags.HideAndDontSave;

                    m_LineDisabledSprite = sprite;
                }

                return m_LineDisabledSprite;
            }
        }

        /// <summary>
        /// The custom validator to use to validate text.
        /// </summary>
        private ITextValidator m_CustomTextValidator;
        /// <summary>
        /// The custom validator to use to validate text.
        /// </summary>
        public ITextValidator customTextValidator
        {
            get { return m_CustomTextValidator; }
            set
            {
                m_CustomTextValidator = value;
                m_CustomTextValidator.Init(this);
            }
        }

        /// <summary>
        /// The last state of the counter.
        /// </summary>
        [SerializeField]
        private bool m_LastCounterState;

        /// <summary>
        /// Is the hint text being animated?
        /// </summary>
        private bool m_AnimateHintText;

        /// <summary>
        /// Is the input field selected?
        /// </summary>
        private bool m_HasBeenSelected;

        /// <summary>
        /// The id of the left content AutoTweener.
        /// </summary>
        private int m_LeftContentTweener;
        /// <summary>
        /// The id of the right content AutoTweener.
        /// </summary>
        private int m_RightContentTweener;
        /// <summary>
        /// The id of the hint text AutoTweener.
        /// </summary>
        private int m_HintTextTweener;
        /// <summary>
        /// The id of the validation color AutoTweener.
        /// </summary>
        private int m_ValidationColorTweener;
        /// <summary>
        /// The id of the counter AutoTweener.
        /// </summary>
        private int m_CounterTweener;

        /// <summary>
        /// The last size of the input field rect.
        /// </summary>
        private Vector2 m_LastSize;
        /// <summary>
        /// The last state of focus for the input field.
        /// </summary>
        private bool m_LastFocussedState;
        /// <summary>
        /// The current state of selection for the input field.
        /// </summary>
        private ColorSelectionState m_CurrentSelectionState;
        /// <summary>
        /// The last state of selection for the input field.
        /// </summary>
        private ColorSelectionState m_LastSelectionState;

        /// <summary>
        /// The top section height.
        /// </summary>
        private float m_TopSectionHeight;
        /// <summary>
        /// The bottom section height.
        /// </summary>
        private float m_BottomSectionHeight;
        /// <summary>
        /// The left section width.
        /// </summary>
        private float m_LeftSectionWidth;
        /// <summary>
        /// The right section width.
        /// </summary>
        private float m_RightSectionWidth;

        /// <summary>
        /// The animation duration.
        /// </summary>
        [SerializeField]
        private float m_AnimationDuration = 0.25f;
        /// <summary>
        /// The active line position tweener.
        /// </summary>
        private int m_ActiveLinePosTweener;
        /// <summary>
        /// The active line size tweener.
        /// </summary>
        private int m_ActiveLineSizeTweener;
        /// <summary>
        /// The active line alpha tweener.
        /// </summary>
        private int m_ActiveLineAlphaTweener;
        /// <summary>
        /// The hint text floating value tweener.
        /// </summary>
        private int m_HintTextFloatingValueTweener;
        /// <summary>
        /// The validation tweener.
        /// </summary>
        private int m_ValidationTweener;

        /// <summary>
        /// The last rect position.
        /// </summary>
        private Vector2 m_LastRectPosition;
        /// <summary>
        /// The last rect size.
        /// </summary>
        private Vector2 m_LastRectSize;
        /// <summary>
        /// The layout size.
        /// </summary>
        private Vector2 m_LayoutSize;

#if UNITY_EDITOR
        /// <summary>
        /// The last hint text.
        /// </summary>
        private string m_LastHintText;
#endif

#if UNITY_EDITOR
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialInputField"/> class.
        /// </summary>
        public MaterialInputField()
        {
            EditorUpdate.Init();
            EditorUpdate.onEditorUpdate += OnEditorUpdate;
        }

        /// <summary>
        /// Called by <see cref="EditorUpdate"/>.
        /// </summary>
        private void OnEditorUpdate()
        {
            if (IsDestroyed())
            {
                EditorUpdate.onEditorUpdate -= OnEditorUpdate;
                return;
            }

            if (inputField == null)
            {
                Debug.LogWarning("Please attach the InputField reference!");
                EditorUpdate.onEditorUpdate -= OnEditorUpdate;
                return;
            }

            if (m_LastSize != m_LayoutSize)
            {
                m_LastSize = m_LayoutSize;
                SetLayoutDirty();
            }

            if (m_LastRectPosition != rectTransform.anchoredPosition)
            {
                m_LastRectPosition = rectTransform.anchoredPosition;
                SetLayoutDirty();

                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(gameObject);
                }
            }
            if (m_LastRectSize != rectTransform.sizeDelta)
            {
                m_LastRectSize = rectTransform.sizeDelta;
                SetLayoutDirty();

                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(gameObject);
                }
            }
            UpdateCounter();
            CheckHintText();
        }
#endif

        /// <summary>
        /// See MonoBehaviour.OnEnable.
        /// </summary>
        protected override void OnEnable()
        {
            UpdateSelectionState();
            OnTextChanged();
            CheckHintText();
            SetLayoutDirty();
        }

        /// <summary>
        /// See MonoBehaviour.OnDisable.
        /// </summary>
        protected override void OnDisable()
        {
            UpdateSelectionState();
            CheckHintText();
            SetLayoutDirty();
        }

        /// <summary>
        /// See MonoBehaviour.OnRectTransformDimensionsChange.
        /// </summary>
        protected override void OnRectTransformDimensionsChange()
        {
            SetLayoutDirty();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(gameObject);
            }
#endif
        }

        /// <summary>
        /// See MonoBehaviour.OnDidApplyAnimationProperties.
        /// </summary>
        protected override void OnDidApplyAnimationProperties()
        {
            UpdateSelectionState();
            OnTextChanged();
            CheckHintText();
            SetLayoutDirty();
        }

#if UNITY_EDITOR
        /// <summary>
        /// See MonoBehaviour.OnValidate.
        /// </summary>
        protected override void OnValidate()
        {
            UpdateSelectionState();
            OnTextChanged();
            SetLayoutDirty();

            if (hintTextObject)
            {
                if (m_HintText != hintTextObject.text)
                {
                    if (m_LastHintText != hintText)
                    {
                        hintTextObject.text = m_HintText;
                        m_LastHintText = m_HintText;
                    }
                }
            }

            if (m_LeftContentGraphic)
            {
                m_LeftContentGraphic.color = IsSelected()
                    ? m_LeftContentActiveColor
                    : m_LeftContentInactiveColor;
            }

            if (m_RightContentGraphic)
            {
                m_RightContentGraphic.color = IsSelected()
                    ? m_RightContentActiveColor
                    : m_RightContentInactiveColor;
            }

            if (m_HasCharacterCounter != m_LastCounterState)
            {
                m_LastCounterState = m_HasCharacterCounter;
                m_CounterText.gameObject.SetActive(m_LastCounterState);
            }

            if (m_HintTextObject != null)
            {
                m_HintTextObject.color = IsSelected() ? m_HintTextActiveColor : m_HintTextInactiveColor;
            }

            if (m_CounterText != null)
            {
                m_CounterText.color = IsSelected() ? m_CounterActiveColor : m_CounterInactiveColor;
            }
            if (m_ValidationText != null)
            {
                m_ValidationText.color = IsSelected() ? m_ValidationActiveColor : m_ValidationInactiveColor;
            }

            m_LineTransform.GetComponent<Graphic>().color = m_LineInactiveColor;
            m_ActiveLineTransform.GetComponent<Graphic>().color = m_LineActiveColor;

            canvasGroup.alpha = inputField.interactable ? 1 : 0.5f;
            canvasGroup.interactable = inputField.interactable;
            canvasGroup.blocksRaycasts = inputField.interactable;
        }
#endif

        /// <summary>
        /// See MonoBehaviour.Start.
        /// </summary>
        protected override void Start()
        {
            UpdateSelectionState();
            OnTextChanged();
            CheckHintText();
            SetLayoutDirty();
        }

        /// <summary>
        /// See MonoBehaviour.Update.
        /// </summary>
        void Update()
        {
            if (Application.isPlaying)
            {
                if (inputField.isFocused)
                {
                    m_LastFocussedState = true;
                }
                else
                {
                    if (m_LastFocussedState)
                    {
                        m_LastFocussedState = false;
                        OnDeselect(new PointerEventData(EventSystem.current));
                    }
                }

                UpdateSelectionState();
                CheckHintText();

                if (m_AnimateHintText)
                {
                    SetHintLayoutToFloatingValue();
                }
            }
        }

        /// <summary>
        /// Clears the InputField text.
        /// </summary>
        public void ClearText()
        {
            inputField.text = "";
            OnTextChanged();
            SetLayoutDirty();
        }

        /// <summary>
        /// Called when the InputField text is changed.
        /// </summary>
        public void OnTextChanged()
        {
            UpdateCounter();
            ValidateText();
        }

        /// <summary>
        /// Refreshes the hint Text's text.
        /// </summary>
        private void CheckHintText()
        {
            if (hintTextObject == null) return;

            if (m_HintText != hintTextObject.text)
            {
                m_HintText = hintTextObject.text;

#if UNITY_EDITOR
                m_LastHintText = m_HintText;
#endif
            }
        }

        /// <summary>
        /// Validates the text.
        /// </summary>
        private void ValidateText()
        {
            if (validationText == null) return;

            if (!m_ValidateOnStart && !m_HasBeenSelected) return;

            m_ValidationText.color = IsSelected() ? m_ValidationActiveColor : m_ValidationInactiveColor;

            ITextValidator validator = null;
            if (m_TextValidator != null && m_TextValidator.GetComponent<ITextValidator>() != null)
            {
                validator = m_TextValidator.GetComponent<ITextValidator>();
                validator.Init(this);
            }

            if (m_CustomTextValidator != null)
            {
                validator = customTextValidator;
                m_HasValidation = true;
            }

            if (validator != null && m_HasValidation)
            {
                TweenManager.EndTween(m_ValidationTweener);

                if (!validator.IsTextValid())
                {
                    if (Application.isPlaying)
                    {
                        validationCanvasGroup.gameObject.SetActive(true);
                        validationCanvasGroup.interactable = true;
                        validationCanvasGroup.blocksRaycasts = true;

                        m_ValidationTweener = TweenManager.TweenFloat(f => validationCanvasGroup.alpha = f, validationCanvasGroup.alpha, 1f, m_AnimationDuration / 2, tweenType: Tween.TweenType.Linear);
                        return;
                    }
                }
                else
                {
                    if (Application.isPlaying)
                    {
                        m_ValidationTweener = TweenManager.TweenFloat(f => validationCanvasGroup.alpha = f, validationCanvasGroup.alpha, 0f, m_AnimationDuration / 2, 0, () =>
                        {
                            validationCanvasGroup.interactable = false;
                            validationCanvasGroup.blocksRaycasts = false;
                            validationCanvasGroup.gameObject.SetActive(false);
                        }, false, Tween.TweenType.Linear);
                        return;
                    }
                }
            }

            validationCanvasGroup.alpha = 0;
            validationCanvasGroup.interactable = false;
            validationCanvasGroup.blocksRaycasts = false;
            validationCanvasGroup.gameObject.SetActive(false);
        }

        /// <summary>
        /// Updates the counter.
        /// </summary>
        private void UpdateCounter()
        {
            if (counterText == null)
            {
                return;
            }

            int limit = m_MatchInputFieldCharacterLimit ? inputField.characterLimit : m_CharacterLimit;

            string outOf = limit > 0 ? " / " + limit : "";

            counterText.text = inputField.text.Length + outOf;
        }

        /// <summary>
        /// Updates the state of the selection.
        /// </summary>
        private void UpdateSelectionState()
        {
            if (m_Interactable)
            {
                m_CurrentSelectionState = inputField.isFocused ? ColorSelectionState.EnabledSelected : ColorSelectionState.EnabledDeselected;

                if (lineImage != null)
                {
                    lineImage.sprite = null;
                }
            }
            else
            {
                m_CurrentSelectionState = inputField.isFocused ? ColorSelectionState.DisabledSelected : ColorSelectionState.DisabledDeselected;

                if (lineImage != null)
                {
                    lineImage.sprite = lineDisabledSprite;
                    lineImage.type = Image.Type.Tiled;
                }
            }

            if (m_CurrentSelectionState != m_LastSelectionState)
            {
                m_LastSelectionState = m_CurrentSelectionState;

                TweenManager.EndTween(m_LeftContentTweener);

                if (Application.isPlaying)
                {
                    if (m_LeftContentGraphic)
                    {
                        m_LeftContentTweener = TweenManager.TweenColor(color => m_LeftContentGraphic.color = color,
                            m_LeftContentGraphic.color,
                            IsSelected() ? m_LeftContentActiveColor : m_LeftContentInactiveColor, m_AnimationDuration);
                    }
                }
                else
                {
                    if (m_LeftContentGraphic)
                    {
                        m_LeftContentGraphic.color = IsSelected()
                            ? m_LeftContentActiveColor
                            : m_LeftContentInactiveColor;
                    }
                }

                TweenManager.EndTween(m_RightContentTweener);

                if (Application.isPlaying)
                {
                    if (m_RightContentGraphic)
                    {
                        m_RightContentTweener = TweenManager.TweenColor(color => m_RightContentGraphic.color = color,
                            m_RightContentGraphic.color,
                            IsSelected() ? m_RightContentActiveColor : m_RightContentInactiveColor, m_AnimationDuration);
                    }
                }
                else
                {
                    if (m_RightContentGraphic)
                    {
                        m_RightContentGraphic.color = IsSelected()
                            ? m_RightContentActiveColor
                            : m_RightContentInactiveColor;
                    }
                }

                TweenManager.EndTween(m_HintTextTweener);

                if (m_HintTextObject != null)
                {
                    if (Application.isPlaying)
                    {
                        m_HintTextTweener = TweenManager.TweenColor(color => m_HintTextObject.color = color,
                            m_HintTextObject.color, IsSelected() ? m_HintTextActiveColor : m_HintTextInactiveColor,
                            m_AnimationDuration);
                    }
                    else
                    {
                        m_HintTextObject.color = IsSelected() ? m_HintTextActiveColor : m_HintTextInactiveColor;
                    }
                }

                TweenManager.EndTween(m_CounterTweener);

                if (m_CounterText != null)
                {
                    if (Application.isPlaying)
                    {
                        m_CounterTweener = TweenManager.TweenColor(color => m_CounterText.color = color,
                            m_CounterText.color, IsSelected() ? m_CounterActiveColor : m_CounterInactiveColor,
                            m_AnimationDuration);
                    }
                    else
                    {
                        m_CounterText.color = IsSelected() ? m_CounterActiveColor : m_CounterInactiveColor;
                    }
                }
                TweenManager.EndTween(m_ValidationColorTweener);

                if (m_ValidationText != null)
                {
                    if (Application.isPlaying)
                    {

                        m_ValidationColorTweener = TweenManager.TweenColor(color => m_ValidationText.color = color,
                            m_ValidationText.color, IsSelected() ? m_ValidationActiveColor : m_ValidationInactiveColor,
                            m_AnimationDuration);
                    }
                    else
                    {
                        m_ValidationText.color = IsSelected() ? m_ValidationActiveColor : m_ValidationInactiveColor;
                    }
                }

                m_LineTransform.GetComponent<Graphic>().color = m_LineInactiveColor;
                m_ActiveLineTransform.GetComponent<Graphic>().color = m_LineActiveColor;

                canvasGroup.alpha = m_Interactable ? 1 : 0.5f;
                canvasGroup.interactable = m_Interactable;
                canvasGroup.blocksRaycasts = m_Interactable;
            }
        }

        /// <summary>
        /// Determines whether this instance is selected.
        /// </summary>
        /// <returns></returns>
        public bool IsSelected()
        {
            return m_CurrentSelectionState == ColorSelectionState.DisabledSelected ||
                   m_CurrentSelectionState == ColorSelectionState.EnabledSelected;
        }

        /// <summary>
        /// Gets the height of the text.
        /// </summary>
        /// <returns></returns>
        private float GetTextHeight()
        {
            string layoutText = inputField.text;
            Vector2 generationSize = new Vector2();

            if (inputField.lineType == InputField.LineType.SingleLine)
            {
                generationSize = new Vector2(float.MaxValue, float.MaxValue);
                layoutText = layoutText.Replace("\n", "");
            }
            else
            {
                generationSize = new Vector2(inputText.rectTransform.rect.width, float.MaxValue);
            }

            TextGenerator textGenerator = inputText.cachedTextGeneratorForLayout;
            TextGenerationSettings textGenerationSettings = inputText.GetGenerationSettings(generationSize);

            if (materialUiScaler == null)
            {
                Debug.LogError("Must have a MaterialUIScaler atached to root canvas");
                return 0;
            }

            return textGenerator.GetPreferredHeight(layoutText, textGenerationSettings) / materialUiScaler.scaler.scaleFactor;
        }

        /// <summary>
        /// Gets the height of the small hint text.
        /// </summary>
        /// <returns></returns>
        private float GetSmallHintTextHeight()
        {
            if (hintTextObject == null)
            {
                return 0;
            }

            if (materialUiScaler == null)
            {
                return 0;
            }

            TextGenerator textGenerator = hintTextObject.cachedTextGeneratorForLayout;
            TextGenerationSettings textGenerationSettings = inputText.GetGenerationSettings(new Vector2(float.MaxValue, float.MaxValue));
            textGenerationSettings.fontSize = m_FloatingHintFontSize;
            return textGenerator.GetPreferredHeight(hintTextObject.text, textGenerationSettings) / materialUiScaler.scaler.scaleFactor;
        }

        /// <summary>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnSelect(BaseEventData eventData)
        {
            m_HasBeenSelected = true;

            AnimateActiveLineSelect();
            AnimateHintTextSelect();
            UpdateSelectionState();

            ValidateText();
        }

        /// <summary>
        /// Called by the EventSystem when a new object is being selected.
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnDeselect(BaseEventData eventData)
        {
            AnimateActiveLineDeselect();
            AnimateHintTextDeselect();
            UpdateSelectionState();
        }

        /// <summary>
        /// Animates the hint text to 'selected' state.
        /// </summary>
        private void AnimateHintTextSelect()
        {
            CancelHintTextTweeners();

            m_HintTextFloatingValueTweener = TweenManager.TweenFloat(f => hintTextFloatingValue = f, hintTextFloatingValue, 1f, m_AnimationDuration, 0, () =>
            {
                m_AnimateHintText = false;
                SetHintLayoutToFloatingValue();
            });
            m_AnimateHintText = true;
        }

        /// <summary>
        /// Animates the hint text to 'deselected' state.
        /// </summary>
        private void AnimateHintTextDeselect()
        {
            CancelHintTextTweeners();

            if (!inputField.isFocused && inputField.text.Length == 0)
            {
                m_HintTextFloatingValueTweener = TweenManager.TweenFloat(f => hintTextFloatingValue = f,
                    () => hintTextFloatingValue, 0f, m_AnimationDuration, 0f, () =>
                    {
                        m_AnimateHintText = false;
                        SetHintLayoutToFloatingValue();
                    });
                m_AnimateHintText = true;
            }
        }

        /// <summary>
        /// Animates the active line to 'deselected' state.
        /// </summary>
        /// <param name="instant">if set to <c>true</c> [instant].</param>
        private void AnimateActiveLineSelect(bool instant = false)
        {
            CancelActivelineTweeners();

            if (m_LineTransform == null || m_ActiveLineTransform == null) return;

            if (instant)
            {
                m_ActiveLineTransform.anchoredPosition = Vector2.zero;
                m_ActiveLineTransform.sizeDelta = new Vector2(m_LineTransform.GetProperSize().x, m_ActiveLineTransform.sizeDelta.y);
                activeLineCanvasGroup.alpha = 1;
            }
            else
            {
                float lineLength = m_LineTransform.GetProperSize().x;

                m_ActiveLineTransform.sizeDelta = new Vector2(0, m_ActiveLineTransform.sizeDelta.y);
                m_ActiveLineTransform.position = Input.mousePosition;
                m_ActiveLineTransform.anchoredPosition = new Vector2(Mathf.Clamp(m_ActiveLineTransform.anchoredPosition.x, -lineLength / 2, lineLength / 2), 0);
                activeLineCanvasGroup.alpha = 1;

                m_ActiveLinePosTweener = TweenManager.TweenFloat(f => m_ActiveLineTransform.anchoredPosition = new Vector2(f, m_ActiveLineTransform.anchoredPosition.y), m_ActiveLineTransform.anchoredPosition.x, 0f, m_AnimationDuration);

                m_ActiveLineSizeTweener = TweenManager.TweenFloat(f => m_ActiveLineTransform.sizeDelta = new Vector2(f, m_ActiveLineTransform.sizeDelta.y), m_ActiveLineTransform.sizeDelta.x, m_LineTransform.GetProperSize().x, m_AnimationDuration);
            }
        }

        /// <summary>
        /// Animates the active line to 'deselected' state.
        /// </summary>
        /// <param name="instant">if set to <c>true</c> [instant].</param>
        private void AnimateActiveLineDeselect(bool instant = false)
        {
            CancelActivelineTweeners();

            if (activeLineTransform == null) return;

            if (instant)
            {
                activeLineCanvasGroup.alpha = 0;
            }
            else
            {
                activeLineCanvasGroup.alpha = 1;

                m_ActiveLineAlphaTweener = TweenManager.TweenFloat(f => activeLineCanvasGroup.alpha = f, activeLineCanvasGroup.alpha, 0f, m_AnimationDuration);
            }
        }

        /// <summary>
        /// Cancels the hint text tweeners.
        /// </summary>
        private void CancelHintTextTweeners()
        {
            TweenManager.EndTween(m_HintTextFloatingValueTweener);
            m_AnimateHintText = false;
        }

        /// <summary>
        /// Cancels the active line tweeners.
        /// </summary>
        private void CancelActivelineTweeners()
        {
            TweenManager.EndTween(m_ActiveLineSizeTweener);
            TweenManager.EndTween(m_ActiveLinePosTweener);
            TweenManager.EndTween(m_ActiveLineAlphaTweener);
        }

        /// <summary>
        /// Sets the hint layout to floating value.
        /// </summary>
        private void SetHintLayoutToFloatingValue()
        {
            if (m_HintTextTransform == null) return;

            if (m_FloatingHint)
            {
                m_HintTextTransform.offsetMin = new Vector2(m_LeftSectionWidth, m_BottomSectionHeight);
                m_HintTextTransform.offsetMax = new Vector2(-m_RightSectionWidth, -Tween.Linear(m_TopSectionHeight, 0, hintTextFloatingValue, 1));
                hintTextObject.fontSize = Mathf.RoundToInt(Tween.Linear(inputText.fontSize, m_FloatingHintFontSize, hintTextFloatingValue, 1));

                float realFontSize = Tween.Linear(inputText.fontSize, m_FloatingHintFontSize, hintTextFloatingValue, 1);

                float scaleFactor = realFontSize / hintTextObject.fontSize;

                m_HintTextTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                hintTextCanvasGroup.alpha = 1;
            }
            else
            {
                m_HintTextTransform.offsetMin = new Vector2(m_LeftSectionWidth, m_BottomSectionHeight);
                m_HintTextTransform.offsetMax = new Vector2(-m_RightSectionWidth, 0);
                hintTextObject.fontSize = inputText.fontSize;
                m_HintTextTransform.localScale = Vector3.one;

                hintTextCanvasGroup.alpha = 1 - hintTextFloatingValue;
            }
        }

        #region Layout

        /// <summary>
        /// Sets the layout dirty.
        /// </summary>
        public void SetLayoutDirty()
        {
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        /// <summary>
        /// Sets the layout horizontal.
        /// </summary>
        public void SetLayoutHorizontal()
        {
            if (!TweenManager.TweenIsActive(m_HintTextFloatingValueTweener))
            {
                hintTextFloatingValue = (inputField.isFocused || inputField.text.Length > 0) ? 1 : 0;
                SetHintLayoutToFloatingValue();
            }

            inputTextTransform.offsetMin = new Vector2(m_LeftSectionWidth, inputTextTransform.offsetMin.y);
            inputTextTransform.offsetMax = new Vector2(-m_RightSectionWidth, inputTextTransform.offsetMax.y);

            if (m_LineTransform != null)
            {
                m_LineTransform.offsetMin = new Vector2(m_LeftSectionWidth, m_LineTransform.offsetMin.y);
                m_LineTransform.offsetMax = new Vector2(-m_RightSectionWidth, m_LineTransform.offsetMax.y);
            }

            if (m_ValidationTextTransform != null)
            {
                m_ValidationTextTransform.offsetMin = new Vector2(m_LeftSectionWidth,
                    m_ValidationTextTransform.offsetMin.y);
                m_ValidationTextTransform.offsetMax = new Vector2(-m_RightSectionWidth,
                    m_ValidationTextTransform.offsetMax.y);
            }

            if (m_CounterTextTransform != null)
            {
                m_CounterTextTransform.offsetMin = new Vector2(m_LeftSectionWidth, m_CounterTextTransform.offsetMin.y);
                m_CounterTextTransform.offsetMax = new Vector2(-m_RightSectionWidth, m_CounterTextTransform.offsetMax.y);
            }

            if (caretTransform != null)
            {
                caretTransform.offsetMin = new Vector2(inputTextTransform.offsetMin.x, caretTransform.offsetMin.y);
                caretTransform.offsetMax = new Vector2(inputTextTransform.offsetMax.x, caretTransform.offsetMax.y);
            }
        }

        /// <summary>
        /// Sets the layout vertical.
        /// </summary>
        public void SetLayoutVertical()
        {
            inputTextTransform.offsetMax = new Vector2(inputTextTransform.offsetMax.x, -m_TopSectionHeight);

            if (!TweenManager.TweenIsActive(m_HintTextFloatingValueTweener))
            {
                hintTextFloatingValue = (inputField.isFocused || inputField.text.Length > 0) ? 1 : 0;
                SetHintLayoutToFloatingValue();
            }

            if (m_LeftContentTransform)
            {
                ILayoutController[] controllers = m_LeftContentTransform.GetComponentsInChildren<ILayoutController>();
                for (int i = 0; i < controllers.Length; i++)
                {
                    controllers[i].SetLayoutVertical();
                }

                m_LeftContentTransform.anchoredPosition = new Vector2(m_LeftContentOffset.x, (inputTextTransform.offsetMax.y - (m_InputText.fontSize / 2) - 2) + m_LeftContentOffset.y);
            }

            if (m_RightContentTransform)
            {
                ILayoutController[] controllers = m_RightContentTransform.GetComponentsInChildren<ILayoutController>();
                for (int i = 0; i < controllers.Length; i++)
                {
                    controllers[i].SetLayoutVertical();
                }

                m_RightContentTransform.anchoredPosition = new Vector2(m_RightContentOffset.x, (inputTextTransform.offsetMax.y - (m_InputText.fontSize / 2) - 2) + m_RightContentOffset.y);
            }

            if (m_LineTransform != null)
            {
                m_LineTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, m_BottomSectionHeight, 1);
            }

            if (caretTransform != null)
            {
                caretTransform.offsetMin = new Vector2(caretTransform.offsetMin.x, inputTextTransform.offsetMin.y);
                caretTransform.offsetMax = new Vector2(caretTransform.offsetMax.x, inputTextTransform.offsetMax.y);
            }
        }

        /// <summary>
        /// The minWidth, preferredWidth, and flexibleWidth values may be calculated in this callback.
        /// </summary>
        public void CalculateLayoutInputHorizontal()
        {
            m_LayoutSize.x = m_ManualPreferredWidth ? m_ManualSize.x : -1;

            m_LeftSectionWidth = 0;
            m_RightSectionWidth = 0;

            if (m_LeftContentTransform != null)
            {
                if (m_LeftContentTransform.gameObject.activeSelf)
                {
                    m_LeftSectionWidth = m_LeftContentTransform.GetProperSize().x;
                    m_LeftSectionWidth += 8;
                }
            }

            if (m_RightContentTransform != null)
            {
                if (m_RightContentTransform.gameObject.activeSelf)
                {
                    m_RightSectionWidth = m_RightContentTransform.GetProperSize().x;
                    m_RightSectionWidth += 8;
                }
            }
        }

        /// <summary>
        /// The minHeight, preferredHeight, and flexibleHeight values may be calculated in this callback.
        /// </summary>
        public void CalculateLayoutInputVertical()
        {
            if (m_FloatingHint)
            {
                m_TopSectionHeight = GetSmallHintTextHeight();
                m_TopSectionHeight += 4;
            }
            else
            {
                m_TopSectionHeight = 0;
            }

            if (m_HasCharacterCounter || m_HasValidation)
            {
                m_BottomSectionHeight = 0;

                if (m_HasCharacterCounter && counterText != null)
                {
                    m_BottomSectionHeight = counterText.preferredHeight;
                }
                else if (m_HasValidation && validationText != null)
                {
                    m_BottomSectionHeight = validationText.preferredHeight;
                }

                m_BottomSectionHeight += 4;
            }
            else
            {
                m_BottomSectionHeight = 0;
            }

            if (m_FitHeightToContent)
            {
                m_LayoutSize.y = GetTextHeight() + 4;
                m_LayoutSize.y += m_TopSectionHeight;
                m_LayoutSize.y += m_BottomSectionHeight;
            }
            else
            {
                m_LayoutSize.y = m_ManualPreferredHeight ? m_ManualSize.y : -1;
            }

            if (m_LeftContentTransform)
            {
                ILayoutElement[] elements = m_LeftContentTransform.GetComponentsInChildren<ILayoutElement>();
                elements = elements.Reverse().ToArray();
                for (int i = 0; i < elements.Length; i++)
                {
                    elements[i].CalculateLayoutInputVertical();
                }
            }

            if (m_RightContentTransform)
            {
                ILayoutElement[] elements = m_RightContentTransform.GetComponentsInChildren<ILayoutElement>();
                elements = elements.Reverse().ToArray();
                for (int i = 0; i < elements.Length; i++)
                {
                    elements[i].CalculateLayoutInputVertical();
                }
            }
        }

        /// <summary>
        /// The minimum width this layout element may be allocated.
        /// </summary>
        public float minWidth { get { return -1; } }
        /// <summary>
        /// The preferred width this layout element should be allocated if there is sufficient space.
        /// </summary>
        public float preferredWidth { get { return m_LayoutSize.x; } }
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
        public float preferredHeight { get { return m_LayoutSize.y; } }
        /// <summary>
        /// The extra relative height this layout element should be allocated if there is additional available space.
        /// </summary>
        public float flexibleHeight { get { return -1; } }
        /// <summary>
        /// The layout priority of this component.
        /// </summary>
        public int layoutPriority { get { return 1; } }

        #endregion
    }
}