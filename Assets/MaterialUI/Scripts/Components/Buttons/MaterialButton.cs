//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>Component that handles a button control.</summary>
    /// <seealso cref="UnityEngine.EventSystems.UIBehaviour"></seealso>
    /// <seealso cref="UnityEngine.UI.ILayoutGroup"></seealso>
    /// <seealso cref="UnityEngine.UI.ILayoutElement"></seealso>
    /// <seealso cref="UnityEngine.UI.ILayoutSelfController"></seealso>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [ExecuteInEditMode]
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("MaterialUI/Material Button", 100)]
    public class MaterialButton : UIBehaviour, ILayoutGroup, ILayoutElement, ILayoutSelfController
    {
        #region Variables

        /// <summary>
        /// Path to the circle button prefab.
        /// </summary>
        private const string pathToCirclePrefab = "Assets/MaterialUI/Prefabs/Components/Buttons/Floating Action Button.prefab";
        /// <summary>
        /// Path to the rectangular button prefab.
        /// </summary>
        private const string pathToRectPrefab = "Assets/MaterialUI/Prefabs/Components/Buttons/Button.prefab";

        /// <summary>
        /// The button's rect transform.
        /// </summary>
        [SerializeField]
        private RectTransform m_RectTransform;
        /// <summary>
        /// The button's rect transform.
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
        /// The content rect transform.
        /// </summary>
        [SerializeField]
        private RectTransform m_ContentRectTransform;
        /// <summary>
        /// The content rect transform.
        /// </summary>
        public RectTransform contentRectTransform
        {
            get { return m_ContentRectTransform; }
            set
            {
                m_ContentRectTransform = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// The Button component.
        /// </summary>
        [SerializeField]
        private Button m_ButtonObject;
        /// <summary>
        /// The Button component.
        /// <para></para>
        /// If null, automatically gets the attached Button, if exists, otherwise, adds one.
        /// </summary>
        public Button buttonObject
        {
            get
            {
                if (m_ButtonObject == null)
                {
                    m_ButtonObject = gameObject.GetAddComponent<Button>();
                }
                return m_ButtonObject;
            }
        }

        /// <summary>
        /// The graphic representing the button's background.
        /// </summary>
        [SerializeField]
        private Graphic m_BackgroundImage;
        /// <summary>
        /// The graphic representing the button's background.
        /// </summary>
        public Graphic backgroundImage
        {
            get { return m_BackgroundImage; }
            set { m_BackgroundImage = value; }
        }

        /// <summary>
        /// The button's text.
        /// </summary>
        [SerializeField]
        private Text m_Text;
        /// <summary>
        /// The button's text.
        /// </summary>
        public Text text
        {
            get { return m_Text; }
            set
            {
                m_Text = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// The button's icon.
        /// </summary>
        [SerializeField]
        private Graphic m_Icon;
        /// <summary>
        /// The button's icon.
        /// </summary>
        public Graphic icon
        {
            get { return m_Icon; }
            set
            {
                m_Icon = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// The button's ripple controller.
        /// </summary>
        [SerializeField]
        private MaterialRipple m_MaterialRipple;
        /// <summary>
        /// The button's ripple controller.
        /// <para></para>
        /// If null, automatically gets the attached MaterialRipple, if one exists.
        /// </summary>
        public MaterialRipple materialRipple
        {
            get
            {
                if (m_MaterialRipple == null)
                {
                    m_MaterialRipple = GetComponent<MaterialRipple>();
                }
                return m_MaterialRipple;
            }
        }

        /// <summary>
        /// The button's shadow controller.
        /// </summary>
        [SerializeField]
        private MaterialShadow m_MaterialShadow;
        /// <summary>
        /// The button's shadow controller.
        /// </summary>
        public MaterialShadow materialShadow
        {
            get
            {
                if (m_MaterialShadow == null)
                {
                    m_MaterialShadow = GetComponent<MaterialShadow>();
                }
                return m_MaterialShadow;
            }
        }

        /// <summary>
        /// The button's canvas group.
        /// </summary>
        [SerializeField]
        private CanvasGroup m_CanvasGroup;
        /// <summary>
        /// The button's canvas group.
        /// <para></para>
        /// If null, automatically gets the attached CanvasGroup if one exists, otherwise adds one.
        /// </summary>
        public CanvasGroup canvasGroup
        {
            get
            {
                if (m_CanvasGroup == null)
                {
                    m_CanvasGroup = gameObject.GetAddComponent<CanvasGroup>();
                }
                return m_CanvasGroup;
            }
        }

        /// <summary>
        /// The shadow canvas group.
        /// </summary>
        [SerializeField]
        private CanvasGroup m_ShadowsCanvasGroup;
        /// <summary>
        /// The shadow canvas group.
        /// </summary>
        public CanvasGroup shadowsCanvasGroup
        {
            get { return m_ShadowsCanvasGroup; }
            set { m_ShadowsCanvasGroup = value; }
        }

        /// <summary>
        /// Is the button interactable?
        /// </summary>
        [SerializeField]
        private bool m_Interactable = true;
        /// <summary>
        /// Is the button interactable?
        /// <para></para>
        /// Also controls the visual appearance of the button.
        /// If you only want to change the interactability of the button itself, manually set the interactability of <see cref="canvasGroup"/>.
        /// </summary>
        public bool interactable
        {
            get { return m_Interactable; }
            set
            {
                m_Interactable = value;
                m_ButtonObject.interactable = m_Interactable;
                canvasGroup.alpha = m_Interactable ? 1f : 0.5f;
                canvasGroup.blocksRaycasts = m_Interactable;
                if (shadowsCanvasGroup)
                {
                    shadowsCanvasGroup.alpha = m_Interactable ? 1f : 0f;
                }
            }
        }

        /// <summary>
        /// The padding between the content and edge of the button.
        /// <para></para>
        /// Used when fitting the button to the content size.
        /// </summary>
        [SerializeField]
        private Vector2 m_ContentPadding = new Vector2(30f, 18f);
        /// <summary>
        /// The padding between the content and edge of the button.
        /// <para></para>
        /// Used when fitting the button to the content size.
        /// </summary>
        public Vector2 contentPadding
        {
            get { return m_ContentPadding; }
            set
            {
                m_ContentPadding = value;
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// The size of the content in the button.
        /// <para></para>
        /// Used when fitting the button to the content size.
        /// </summary>
        [SerializeField]
        private Vector2 m_ContentSize;
        /// <summary>
        /// The size of the content in the button.
        /// <para></para>
        /// Used when fitting the button to the content size.
        /// </summary>
        public Vector2 contentSize
        {
            get { return m_ContentSize; }
        }

        /// <summary>
        /// The size of the button given the layout system.
        /// </summary>
        [SerializeField]
        private Vector2 m_Size;
        /// <summary>
        /// The size of the button given the layout system.
        /// </summary>
        public Vector2 size
        {
            get { return m_Size; }
        }

        /// <summary>
        /// Should the button fit its width to match the content width + x padding?
        /// </summary>
        [SerializeField]
        private bool m_FitWidthToContent;
        /// <summary>
        /// Should the button fit its width to match the content width + x padding?
        /// </summary>
        public bool fitWidthToContent
        {
            get { return m_FitWidthToContent; }
            set
            {
                m_FitWidthToContent = value;
                m_Tracker.Clear();
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// Should the button fit its height to match the content height + y padding?
        /// </summary>
        [SerializeField]
        private bool m_FitHeightToContent;
        /// <summary>
        /// Should the button fit its height to match the content height + y padding?
        /// </summary>
        public bool fitHeightToContent
        {
            get { return m_FitHeightToContent; }
            set
            {
                m_FitHeightToContent = value;
                m_Tracker.Clear();
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// Is this button a circular button?
        /// <para></para>
        /// Only change this value if you know what you're doing.
        /// </summary>
        [SerializeField]
        private bool m_IsCircularButton;
        /// <summary>
        /// Is this button a circular button?
        /// <para></para>
        /// Only change this value if you know what you're doing.
        /// </summary>
        public bool isCircularButton
        {
            get { return m_IsCircularButton; }
            set { m_IsCircularButton = value; }
        }

        /// <summary>
        /// Is this button a raised button?
        /// <para></para>
        /// Only change this value if you know what you're doing.
        /// </summary>
        [SerializeField]
        private bool m_IsRaisedButton;
        /// <summary>
        /// Is this button a raised button?
        /// <para></para>
        /// Only change this value if you know what you're doing.
        /// </summary>
        public bool isRaisedButton
        {
            get { return m_IsRaisedButton; }
            set { m_IsRaisedButton = value; }
        }

#if UNITY_EDITOR
        /// <summary>
        /// The last cached position of the button.
        /// </summary>
        private Vector2 m_LastPosition;
#endif

        /// <summary>
        /// The driven rect transform tracker of the button.
        /// </summary>
        private DrivenRectTransformTracker m_Tracker = new DrivenRectTransformTracker();

        #endregion

        #region ExternalProperties

        /// <summary>
        /// Gets or sets the text's text.
        /// <para></para>
        /// Convinience property, make sure the button has a text before using this.
        /// </summary>
        public string textText
        {
            get { return m_Text.text; }
            set { m_Text.text = value; }
        }

        /// <summary>
        /// Gets or sets the text's color.
        /// <para></para>
        /// Convinience property, make sure the button has a text before using this.
        /// </summary>
        public Color textColor
        {
            get { return m_Text.color; }
            set { m_Text.color = value; }
        }

        /// <summary>
        /// Gets or sets the icon's vector image data.
        /// <para></para>
        /// Convinience property, make sure the button has an icon graphic before using this.
        /// </summary>
        public VectorImageData iconVectorImageData
        {
            get { return m_Icon.GetVectorImage(); }
            set { m_Icon.SetImage(value); }
        }

        /// <summary>
        /// Gets or sets the icon's sprite.
        /// <para></para>
        /// Convinience property, make sure the button has an icon graphic before using this.
        /// </summary>
        public Sprite iconSprite
        {
            get { return m_Icon.GetSpriteImage(); }
            set { m_Icon.SetImage(value); }
        }

        /// <summary>
        /// Gets or sets the icon's color.
        /// <para></para>
        /// Convinience property, make sure the button has an icon graphic before using this.
        /// </summary>
        public Color iconColor
        {
            get { return m_Icon.color; }
            set { m_Icon.color = value; }
        }

        /// <summary>
        /// Gets or sets the background's vector image data.
        /// <para></para>
        /// Convinience property, make sure the button has a background graphic before using this.
        /// </summary>
        public VectorImageData backgroundVectorImageData
        {
            get { return m_BackgroundImage.GetVectorImage(); }
            set { m_BackgroundImage.SetImage(value); }
        }

        /// <summary>
        /// Gets or sets the background's sprite.
        /// <para></para>
        /// Convinience property, make sure the button has a background graphic before using this.
        /// </summary>
        public Sprite backgroundSprite
        {
            get { return m_BackgroundImage.GetSpriteImage(); }
            set { m_BackgroundImage.SetImage(value); }
        }

        /// <summary>
        /// Gets or sets the background's color.
        /// <para></para>
        /// Convinience property, make sure the button has a background graphic before using this.
        /// </summary>
        public Color backgroundColor
        {
            get { return m_BackgroundImage.color; }
            set { m_BackgroundImage.color = value; }
        }

        #endregion

        #region Methods

#if UNITY_EDITOR
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialButton"/> class.
        /// </summary>
        public MaterialButton()
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
            SetLayoutDirty();

#if UNITY_EDITOR
            OnValidate();
#endif
        }

        /// <summary>
        /// See MonoBehaviour.OnDisable.
        /// </summary>
        protected override void OnDisable()
        {
            m_Tracker.Clear();
        }

#if UNITY_EDITOR
        /// <summary>
        /// See MonoBehaviour.OnDestroy.
        /// </summary>
        protected override void OnDestroy()
        {
            EditorUpdate.onEditorUpdate -= OnEditorUpdate;
        }
#endif

#if UNITY_EDITOR
        /// <summary>
        /// Called by <see cref="EditorUpdate"/> in edit mode.
        /// </summary>
        private void OnEditorUpdate()
        {
            if (IsDestroyed())
            {
                EditorUpdate.onEditorUpdate -= OnEditorUpdate;
                return;
            }

            if (rectTransform.anchoredPosition != m_LastPosition)
            {
                m_LastPosition = rectTransform.anchoredPosition;
                EditorUtility.SetDirty(rectTransform);
            }
        }
#endif

        /// <summary>
        /// Sets the color of the button background.
        /// <para></para>
        /// If the button has a MaterialRipple, then it is used to change the graphic color in a way that doesn't interrupt the ripple's highlighting, otherwise, the graphic's color is directly changed.
        /// </summary>
        /// <param name="color">The color to set.</param>
        /// <param name="animate">Should the color transition smoothly? Does not animate in edit mode.</param>
        public void SetButtonBackgroundColor(Color color, bool animate = true)
        {
            if (m_BackgroundImage == null) return;
            if (m_MaterialRipple != null)
            {
                m_MaterialRipple.SetGraphicColor(color, animate);
            }
            else
            {
                if (animate && Application.isPlaying)
                {
                    TweenManager.TweenColor(color1 => m_BackgroundImage.color = color1, m_BackgroundImage.color, color, 0.5f);
                }
                else
                {
                    m_BackgroundImage.color = color;
                }
            }
        }

        /// <summary>
        /// Calls <see cref="MaterialRipple.RefreshGraphicMatchColor"/> on <see cref="materialRipple"/>, if one exists.
        /// </summary>
        public void RefreshRippleMatchColor()
        {
            if (m_MaterialRipple != null)
            {
                m_MaterialRipple.RefreshGraphicMatchColor();
            }
        }

        /// <summary>
        /// Converts the button between flat and raised.
        /// Note that this doesn't animate the transition. If you want to simply hide/show the button's shadows, it's recommended to use instead use the <see cref="materialShadow"/>.
        /// </summary>
        /// <param name="noExitGUI">Should <see cref="GUIUtility.ExitGUI"/> be called? Prevents inspector errors in some situations.</param>
        public void Convert(bool noExitGUI = false)
        {
#if UNITY_EDITOR
            string flatRoundedSquare = "Assets/MaterialUI/Images/RoundedSquare/roundedsquare_";
            string raisedRoundedSquare = "Assets/MaterialUI/Images/RoundedSquare_Stroke/roundedsquare_stroke_";

            string imagePath = "";

            if (!isCircularButton)
            {
                imagePath = isRaisedButton ? flatRoundedSquare : raisedRoundedSquare;
            }

            if (isRaisedButton)
            {
                DestroyImmediate(m_ShadowsCanvasGroup.gameObject);
                m_ShadowsCanvasGroup = null;

                if (materialShadow)
                {
                    DestroyImmediate(materialShadow);
                }

                if (materialRipple != null)
                {
                    materialRipple.highlightWhen = MaterialRipple.HighlightActive.Hovered;
                }
            }
            else
            {
                string path = isCircularButton ? pathToCirclePrefab : pathToRectPrefab;

                GameObject tempButton = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path));

                GameObject newShadow = tempButton.transform.Find("Shadows").gameObject;

                m_ShadowsCanvasGroup = newShadow.GetComponent<CanvasGroup>();

                RectTransform newShadowRectTransform = (RectTransform)newShadow.transform;

                newShadowRectTransform.SetParent(rectTransform);
                newShadowRectTransform.SetAsFirstSibling();
                newShadowRectTransform.localScale = Vector3.one;
                newShadowRectTransform.localEulerAngles = Vector3.zero;

                RectTransform tempRectTransform = m_BackgroundImage != null
                    ? (RectTransform)m_BackgroundImage.transform
                    : rectTransform;

                if (isCircularButton)
                {
                    newShadowRectTransform.anchoredPosition = Vector2.zero;
                    RectTransformSnap newSnapper = newShadow.GetAddComponent<RectTransformSnap>();
                    newSnapper.sourceRectTransform = tempRectTransform;
                    newSnapper.valuesArePercentage = true;
                    newSnapper.snapWidth = true;
                    newSnapper.snapHeight = true;
                    newSnapper.snapEveryFrame = true;
                    newSnapper.paddingPercent = new Vector2(225, 225);
                    Vector3 tempVector3 = rectTransform.GetPositionRegardlessOfPivot();
                    tempVector3.y -= 1f;
                    newShadowRectTransform.position = tempVector3;
                }
                else
                {
                    newShadowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempRectTransform.GetProperSize().x + 54);
                    newShadowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tempRectTransform.GetProperSize().y + 54);
                    Vector3 tempVector3 = rectTransform.GetPositionRegardlessOfPivot();
                    newShadowRectTransform.position = tempVector3;
                }

                DestroyImmediate(tempButton);

                gameObject.AddComponent<MaterialShadow>();

                materialShadow.shadowsActiveWhen = MaterialShadow.ShadowsActive.Hovered;

                materialShadow.animatedShadows = newShadow.GetComponentsInChildren<AnimatedShadow>();

                materialShadow.isEnabled = true;

                if (materialRipple != null)
                {
                    materialRipple.highlightWhen = MaterialRipple.HighlightActive.Clicked;
                }
            }

            if (!isCircularButton)
            {
                SpriteSwapper spriteSwapper = GetComponent<SpriteSwapper>();

                if (spriteSwapper != null)
                {
                    spriteSwapper.sprite1X = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + "100%.png");
                    spriteSwapper.sprite2X = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + "200%.png");
                    spriteSwapper.sprite4X = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + "400%.png");
                }
                else
                {
                    if (m_BackgroundImage != null)
                    {
                        ((Image)m_BackgroundImage).sprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + "100%.png");
                    }
                }
            }
            else
            {
                if (!isRaisedButton)
                {

                    RectTransform tempRectTransform = (RectTransform)new GameObject("Stroke", typeof(VectorImage)).transform;

                    tempRectTransform.SetParent(m_BackgroundImage.rectTransform);
                    tempRectTransform.localScale = Vector3.one;
                    tempRectTransform.localEulerAngles = Vector3.zero;
                    tempRectTransform.anchorMin = Vector2.zero;
                    tempRectTransform.anchorMax = Vector2.one;
                    tempRectTransform.anchoredPosition = Vector2.zero;
                    tempRectTransform.sizeDelta = Vector2.zero;

                    VectorImage vectorImage = tempRectTransform.GetComponent<VectorImage>();
                    vectorImage.vectorImageData = MaterialUIIconHelper.GetIcon("circle_stroke_thin").vectorImageData;
                    vectorImage.sizeMode = VectorImage.SizeMode.MatchMin;
                    vectorImage.color = new Color(0f, 0f, 0f, 0.125f);

                    tempRectTransform.name = "Stroke";
                }
                else
                {
                    VectorImage[] images = backgroundImage.GetComponentsInChildren<VectorImage>();

                    for (int i = 0; i < images.Length; i++)
                    {
                        if (images[i].name == "Stroke")
                        {
                            DestroyImmediate(images[i].gameObject);
                        }
                    }
                }
            }

            name = isRaisedButton ? name.Replace("Raised", "Flat") : name.Replace("Flat", "Raised");

            if (m_BackgroundImage != null)
            {
                if (!isRaisedButton)
                {
                    if (m_BackgroundImage.color == Color.clear)
                    {
                        m_BackgroundImage.color = Color.white;
                    }
                }
                else
                {

                    if (m_BackgroundImage.color == Color.white)
                    {
                        m_BackgroundImage.color = Color.clear;
                    }
                }
            }

            m_IsRaisedButton = !m_IsRaisedButton;

            if (!noExitGUI)
            {
                GUIUtility.ExitGUI();
            }
#endif
        }

        /// <summary>
        /// Clears the tracker.
        /// </summary>
        public void ClearTracker()
        {
            m_Tracker.Clear();
        }

        /// <summary>
        /// See MonoBehaviour.OnRectTransformDimensionsChange.
        /// </summary>
        protected override void OnRectTransformDimensionsChange()
        {
            SetLayoutDirty();
        }

        /// <summary>
        /// See MonoBehaviour.OnCanvasGroupChanged.
        /// </summary>
        protected override void OnCanvasGroupChanged()
        {
            SetLayoutDirty();
        }

        /// <summary>
        /// See MonoBehaviour.OnDidApplyAnimationProperties.
        /// </summary>
        protected override void OnDidApplyAnimationProperties()
        {
            SetLayoutDirty();
        }

#if UNITY_EDITOR
        /// <summary>
        /// See MonoBehaviour.OnValidate.
        /// </summary>
        protected override void OnValidate()
        {
            if (m_RectTransform == null)
            {
                m_RectTransform = GetComponent<RectTransform>();
            }
            if (m_ButtonObject == null)
            {
                m_ButtonObject = gameObject.GetAddComponent<Button>();
            }
            if (m_CanvasGroup == null)
            {
                m_CanvasGroup = gameObject.GetAddComponent<CanvasGroup>();
            }

            SetLayoutDirty();
        }
#endif

        /// <summary>
        /// Marks the button's layout to be rebuilt in the next layout calculation.
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
            if (m_FitWidthToContent)
            {
                if (m_ContentRectTransform == null) return;
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_Size.x);
                m_Tracker.Add(this, m_ContentRectTransform, DrivenTransformProperties.SizeDeltaX);
                m_ContentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_ContentSize.x);
            }
        }

        /// <summary>
        /// Sets the layout vertical.
        /// </summary>
        public void SetLayoutVertical()
        {
            if (m_FitHeightToContent)
            {
                if (m_ContentRectTransform == null) return;
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_Size.y);
                m_Tracker.Add(this, m_ContentRectTransform, DrivenTransformProperties.SizeDeltaY);
                m_ContentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_ContentSize.y);
            }
        }

        /// <summary>
        /// The minWidth, preferredWidth, and flexibleWidth values may be calculated in this callback.
        /// </summary>
        public void CalculateLayoutInputHorizontal()
        {
            if (m_FitWidthToContent)
            {
                if (m_ContentRectTransform == null) return;
                m_ContentSize.x = LayoutUtility.GetPreferredWidth(m_ContentRectTransform);
                m_Size.x = m_ContentSize.x + m_ContentPadding.x;
            }
            else
            {
                m_Size.x = -1;
            }
        }

        /// <summary>
        /// The minHeight, preferredHeight, and flexibleHeight values may be calculated in this callback.
        /// </summary>
        public void CalculateLayoutInputVertical()
        {
            if (m_FitHeightToContent)
            {
                if (m_ContentRectTransform == null) return;
                m_ContentSize.y = LayoutUtility.GetPreferredHeight(m_ContentRectTransform);
                m_Size.y = m_ContentSize.y + m_ContentPadding.y;
            }
            else
            {
                m_Size.y = -1;
            }
        }

        #endregion

        #region LayoutProperties

        /// <summary>
        /// The minimum width this layout element may be allocated.
        /// </summary>
        public float minWidth { get { return enabled ? m_Size.x : 0; } }
        /// <summary>
        /// The preferred width this layout element should be allocated if there is sufficient space.
        /// </summary>
        public float preferredWidth { get { return minWidth; } }
        /// <summary>
        /// The extra relative width this layout element should be allocated if there is additional available space.
        /// </summary>
        public float flexibleWidth { get { return -1; } }
        /// <summary>
        /// The minimum height this layout element may be allocated.
        /// </summary>
        public float minHeight { get { return enabled ? m_Size.y : 0; } }
        /// <summary>
        /// The preferred height this layout element should be allocated if there is sufficient space.
        /// </summary>
        public float preferredHeight { get { return minHeight; } }
        /// <summary>
        /// The extra relative height this layout element should be allocated if there is additional available space.
        /// </summary>
        public float flexibleHeight { get { return -1; } }
        /// <summary>
        /// The layout priority of this component.
        /// </summary>
        public int layoutPriority { get { return 1; } }
    }

    #endregion
}