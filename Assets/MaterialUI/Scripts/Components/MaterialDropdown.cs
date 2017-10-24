//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Component that handles a dropdown control.
    /// </summary>
    /// <seealso cref="UnityEngine.EventSystems.UIBehaviour" />
    /// <seealso cref="MaterialUI.IOptionDataListContainer" />
    [AddComponentMenu("MaterialUI/Dropdown", 100)]
    public class MaterialDropdown : UIBehaviour, IOptionDataListContainer
    {
        /// <summary>
        /// The different ways a dropdown can be vertically pivoted.
        /// </summary>
        public enum VerticalPivotType
        {
            /// <summary>
            /// The dropdown's top will align with the bottom of the base rectTransform when Shown.
            /// </summary>
            BelowBase,
            /// <summary>
            /// The dropdown's top will align with the top of the base rectTransform when Shown.
            /// </summary>
            Top,
            /// <summary>
            /// The dropdown's first item will align with the center of the base rectTransform when Shown.
            /// </summary>
            FirstItem,
            /// <summary>
            /// The dropdown's center will align with the center of the base rectTransform when Shown.
            /// </summary>
            Center,
            /// <summary>
            /// The dropdown's last item will align with the center of the base rectTransform when Shown.
            /// </summary>
            LastItem,
            /// <summary>
            /// The dropdown's bottom will align with the bottom of the base rectTransform when Shown.
            /// </summary>
            Bottom,
            /// <summary>
            /// The dropdown's bottom will align with the top of the base rectTransform when Shown.
            /// </summary>
            AboveBase
        }

        /// <summary>
        /// The different ways a dropdown can be horizontally pivoted.
        /// </summary>
        public enum HorizontalPivotType
        {
            /// <summary>
            /// The dropdown's left edge will align with the left edge of the base rectTransform when shown.
            /// </summary>
            Left,
            /// <summary>
            /// The dropdown's center will align with the center of the base rectTransform when shown.
            /// </summary>
            Center,
            /// <summary>
            /// The dropdown's right edge will align with the right edge of the base rectTransform when shown.
            /// </summary>
            Right
        }

        /// <summary>
        /// The different sizes a dropdown can be when beginning to expand.
        /// </summary>
        public enum ExpandStartType
        {
            /// <summary>
            /// The dropdown's size will be (0,0) when it begins to expand.
            /// </summary>
            ExpandFromNothing,
            /// <summary>
            /// The dropdown's width will match the width of the base RectTransform when it begins to expand.
            /// </summary>
            ExpandFromBaseTransformWidth,
            /// <summary>
            /// The dropdown's height will match the height of the base RectTransform when it begins to expand.
            /// </summary>
            ExpandFromBaseTransformHeight,
            /// <summary>
            /// The dropdown's size will match the size of the base RectTransform when it begins to expand.
            /// </summary>
            ExpandFromBaseTransformSize
        }

        /// <summary>
        /// Contains data about a dropdown list item in the scene.
        /// </summary>
        [Serializable]
        public class DropdownListItem
        {
            /// <summary>
            /// The RectTransform of the item.
            /// </summary>
            public RectTransform m_RectTransform;
            /// <summary>
            /// The RectTransform of the item.
            /// </summary>
            public RectTransform rectTransform
            {
                get { return m_RectTransform; }
                set { m_RectTransform = value; }
            }

            /// <summary>
            /// The CanvasGroup of the item.
            /// </summary>
            public CanvasGroup m_CanvasGroup;
            /// <summary>
            /// The CanvasGroup of the item.
            /// </summary>
            public CanvasGroup canvasGroup
            {
                get { return m_CanvasGroup; }
                set { m_CanvasGroup = value; }
            }

            /// <summary>
            /// The Text object of the item.
            /// </summary>
            public Text m_Text;
            /// <summary>
            /// The Text object of the item.
            /// </summary>
            public Text text
            {
                get { return m_Text; }
                set { m_Text = value; }
            }

            /// <summary>
            /// The Icon object of the item.
            /// </summary>
            public Graphic m_Image;
            /// <summary>
            /// The Icon object of the item.
            /// </summary>
            public Graphic image
            {
                get { return m_Image; }
                set { m_Image = value; }
            }
        }

        /// <summary>
        /// Called by MaterialDropdown during certain events.
        /// </summary>
        /// <seealso cref="UnityEvent" />
        /// <seealso cref="int" />
        [Serializable]
        public class MaterialDropdownEvent : UnityEvent<int> { }

        /// <summary>
        /// The way the dropdown will be vertically pivoted.
        /// </summary>
        [SerializeField]
        private VerticalPivotType m_VerticalPivotType = VerticalPivotType.FirstItem;
        /// <summary>
        /// The way the dropdown will be vertically pivoted.
        /// </summary>
        public VerticalPivotType verticalPivotType
        {
            get { return m_VerticalPivotType; }
            set { m_VerticalPivotType = value; }
        }

        /// <summary>
        /// The way the dropdown will be horizontally pivoted.
        /// </summary>
        [SerializeField]
        private HorizontalPivotType m_HorizontalPivotType = HorizontalPivotType.Left;
        /// <summary>
        /// The way the dropdown will be horizontally pivoted.
        /// </summary>
        public HorizontalPivotType horizontalPivotType
        {
            get { return m_HorizontalPivotType; }
            set { m_HorizontalPivotType = value; }
        }

        /// <summary>
        /// The size of the dropdown when it is shown, before fully expanding.
        /// </summary>
        [SerializeField]
        private ExpandStartType m_ExpandStartType = ExpandStartType.ExpandFromBaseTransformSize;
        /// <summary>
        /// The size of the dropdown when it is shown, before fully expanding.
        /// </summary>
        public ExpandStartType expandStartType
        {
            get { return m_ExpandStartType; }
            set { m_ExpandStartType = value; }
        }

        /// <summary>
        /// The duration after expanding that dropdown that input over the dropdown will be ignored.
        /// </summary>
        [SerializeField]
        private float m_IgnoreInputAfterShowTimer;
        /// <summary>
        /// The duration after expanding that dropdown that input over the dropdown will be ignored.
        /// </summary>
        public float ignoreInputAfterShowTimer
        {
            get { return m_IgnoreInputAfterShowTimer; }
            set { m_IgnoreInputAfterShowTimer = value; }
        }

        /// <summary>
        /// The maximum height that the dropdown can be when fully expanded.
        /// </summary>
        [SerializeField]
        private float m_MaxHeight = 200;
        /// <summary>
        /// The maximum height that the dropdown can be when fully expanded.
        /// </summary>
        public float maxHeight
        {
            get { return m_MaxHeight; }
            set { m_MaxHeight = value; }
        }

        /// <summary>
        /// Should the base label's text be capitalized when being set, after a list item is selected?
        /// </summary>
        [SerializeField]
        private bool m_CapitalizeButtonText = true;
        /// <summary>
        /// Should the base label's text be capitalized when being set, after a list item is selected?
        /// </summary>
        public bool capitalizeButtonText
        {
            get { return m_CapitalizeButtonText; }
            set { m_CapitalizeButtonText = value; }
        }

        /// <summary>
        /// Should the current/last selected list item be highlighted in the dropdown?
        /// </summary>
        [SerializeField]
        private bool m_HighlightCurrentlySelected = true;
        /// <summary>
        /// Should the current/last selected list item be highlighted in the dropdown?
        /// </summary>
        public bool highlightCurrentlySelected
        {
            get { return m_HighlightCurrentlySelected; }
            set { m_HighlightCurrentlySelected = value; }
        }

        /// <summary>
        /// Should the base label/icon be updated to reflect the label/icon of the selected list item?
        /// </summary>
        [SerializeField]
        private bool m_UpdateHeader = true;
        /// <summary>
        /// Should the base label/icon be updated to reflect the label/icon of the selected list item?
        /// </summary>
        public bool updateHeader
        {
            get { return m_UpdateHeader; }
            set { m_UpdateHeader = value; }
        }

        /// <summary>
        /// The duration (in seconds) of the 's show/hide animations.
        /// </summary>
        [SerializeField]
        private float m_AnimationDuration = 0.3f;
        /// <summary>
        /// The duration (in seconds) of the dropdown's show/hide animations.
        /// </summary>
        public float animationDuration
        {
            get { return m_AnimationDuration; }
            set { m_AnimationDuration = value; }
        }

        /// <summary>
        /// The minimum distance the dropdown can be from the edge of the canvas, when fully expanded.
        /// </summary>
        [SerializeField]
        private float m_MinDistanceFromEdge = 16f;
        /// <summary>
        /// The minimum distance the dropdown can be from the edge of the canvas, when fully expanded.
        /// </summary>
        public float minDistanceFromEdge
        {
            get { return m_MinDistanceFromEdge; }
            set { m_MinDistanceFromEdge = value; }
        }

        /// <summary>
        /// The color of the dropdown panel.
        /// </summary>
        [SerializeField]
        private Color m_PanelColor = Color.white;
        /// <summary>
        /// The color of the dropdown panel.
        /// </summary>
        public Color panelColor
        {
            get { return m_PanelColor; }
            set { m_PanelColor = value; }
        }

        /// <summary>
        /// The RippleData to use for item ripples.
        /// </summary>
        [SerializeField]
        private RippleData m_ItemRippleData;
        /// <summary>
        /// The RippleData to use for item ripples.
        /// </summary>
        public RippleData itemRippleData
        {
            get { return m_ItemRippleData; }
            set { m_ItemRippleData = value; }
        }

        /// <summary>
        /// The color of the dropdown item text.
        /// </summary>
        [SerializeField]
        private Color m_ItemTextColor = MaterialColor.textDark;
        /// <summary>
        /// The color of the dropdown item text.
        /// </summary>
        public Color itemTextColor
        {
            get { return m_ItemTextColor; }
            set { m_ItemTextColor = value; }
        }

        /// <summary>
        /// The color of the dropdown item icons.
        /// </summary>
        [SerializeField]
        private Color m_ItemIconColor = MaterialColor.iconDark;
        /// <summary>
        /// The color of the dropdown item icons.
        /// </summary>
        public Color itemIconColor
        {
            get { return m_ItemIconColor; }
            set { m_ItemIconColor = value; }
        }

        /// <summary>
        /// The base RectTransform that the dropdown will appear from. Usually a button.
        /// </summary>
        [SerializeField]
        private RectTransform m_BaseTransform;
        /// <summary>
        /// The base RectTransform that the dropdown will appear from. Usually a button.
        /// </summary>
        public RectTransform baseTransform
        {
            get { return m_BaseTransform; }
            set { m_BaseTransform = value; }
        }

        /// <summary>
        /// The base selectable that a user can select when the dropdown is hidden.
        /// </summary>
        [SerializeField]
        private Selectable m_BaseSelectable;
        /// <summary>
        /// The base selectable that a user can select when the dropdown is hidden.
        /// </summary>
        public Selectable baseSelectable
        {
            get { return m_BaseSelectable; }
            set { m_BaseSelectable = value; }
        }

        /// <summary>
        /// The text to update when an item is selected.
        /// </summary>
        [SerializeField]
        private Text m_ButtonTextContent;
        /// <summary>
        /// The text to update when an item is selected.
        /// </summary>
        public Text buttonTextContent
        {
            get { return m_ButtonTextContent; }
            set { m_ButtonTextContent = value; }
        }

        /// <summary>
        /// The graphic to update when an item is selected.
        /// </summary>
        [SerializeField]
        private Graphic m_ButtonImageContent;
        /// <summary>
        /// The graphic to update when an item is selected.
        /// </summary>
        public Graphic buttonImageContent
        {
            get { return m_ButtonImageContent; }
            set { m_ButtonImageContent = value; }
        }

        /// <summary>
        /// The index of the currently selected item.
        /// </summary>
        [SerializeField]
        private int m_CurrentlySelected;
        /// <summary>
        /// The index of the currently selected item.
        /// If set, the base text and image objects will be updated to reflect the selected item, if they exist.
        /// </summary>
        public int currentlySelected
        {
            get { return m_CurrentlySelected; }
            set
            {
                m_CurrentlySelected = Mathf.Clamp(value, -1, m_OptionDataList.options.Count - 1);

                if (m_CurrentlySelected >= 0)
                {
                    if (m_ButtonImageContent != null)
                    {
                        m_ButtonImageContent.SetImage(m_OptionDataList.options[m_CurrentlySelected].imageData);
                    }

                    if (m_ButtonTextContent != null)
                    {
                        string itemText = m_OptionDataList.options[m_CurrentlySelected].text;

                        if (m_CapitalizeButtonText)
                        {
                            itemText = itemText.ToUpper();
                        }

                        m_ButtonTextContent.text = itemText;
                    }
                }
            }
        }

        /// <summary>
        /// The list of item data objects to use in the dropdown.
        /// </summary>
        [SerializeField]
        private OptionDataList m_OptionDataList;
        /// <summary>
        /// The list of item data objects to use in the dropdown.
        /// </summary>
        public OptionDataList optionDataList
        {
            get { return m_OptionDataList; }
            set { m_OptionDataList = value; }
        }

        /// <summary>
        /// Called when an item is selected. Like UnityEvents, accessible from inspector.
        /// </summary>
        [SerializeField]
        private MaterialDropdownEvent m_OnItemSelected = new MaterialDropdownEvent();
        /// <summary>
        /// Called when an item is selected. Like UnityEvents, accessible from inspector.
        /// </summary>
        public MaterialDropdownEvent onItemSelected
        {
            get { return m_OnItemSelected; }
            set { m_OnItemSelected = value; }
        }

        /// <summary>
        /// The currently instantiated list item objects.
        /// </summary>
        private List<DropdownListItem> m_ListItems = new List<DropdownListItem>();
        /// <summary>
        /// The currently instantiated list item objects.
        /// </summary>
        public List<DropdownListItem> listItems
        {
            get { return m_ListItems; }
            set { m_ListItems = value; }
        }

        /// <summary>
        /// The root MaterialUIScaler.
        /// </summary>
        private MaterialUIScaler m_Scaler;

        /// <summary>
        /// The root MaterialUIScaler.
        /// If null, automatically gets the root scaler, if one exists.
        /// </summary>
        private MaterialUIScaler scaler
        {
            get
            {
                if (m_Scaler == null)
                {
                    m_Scaler = MaterialUIScaler.GetParentScaler(transform);
                }

                return m_Scaler;
            }
        }

        /// <summary>
        /// The dropdown panel.
        /// </summary>
        private RectTransform m_DropdownPanel;
        /// <summary>
        /// The panel layer.
        /// </summary>
        private RectTransform m_PanelLayer;
        /// <summary>
        /// The dropdown canvas group.
        /// </summary>
        private CanvasGroup m_DropdownCanvasGroup;
        /// <summary>
        /// The dropdown canvas.
        /// </summary>
        private Canvas m_DropdownCanvas;
        /// <summary>
        /// The shadow canvas group.
        /// </summary>
        private CanvasGroup m_ShadowCanvasGroup;
        /// <summary>
        /// The list item template.
        /// </summary>
        private DropdownListItem m_ListItemTemplate;
        /// <summary>
        /// The cancel panel.
        /// </summary>
        private RectTransform m_CancelPanel;

        /// <summary>
        /// The expanded size.
        /// </summary>
        private Vector2 m_ExpandedSize;
        /// <summary>
        /// The expanded position.
        /// </summary>
        private Vector3 m_ExpandedPosition;
        /// <summary>
        /// The full height.
        /// </summary>
        private float m_FullHeight;
        /// <summary>
        /// The is exapanded.
        /// </summary>
        private bool m_IsExapanded;
        /// <summary>
        /// The temporary maximum height.
        /// </summary>
        private float m_TempMaxHeight;
        /// <summary>
        /// The scroll position offset.
        /// </summary>
        private float m_ScrollPosOffset;
        /// <summary>
        /// The time shown.
        /// </summary>
        private float m_TimeShown;

        /// <summary>
        /// The dropdown canvas game object.
        /// </summary>
        private GameObject m_DropdownCanvasGameObject;

        /// <summary>
        /// The dropdown automatic tweeners.
        /// </summary>
        private List<int> m_AutoTweeners;
        /// <summary>
        /// The list item automatic tweeners.
        /// </summary>
        private List<int> m_ListItemAutoTweeners = new List<int>();

        /// <summary>
        /// Adds an OptionData to the dropdown data list.
        /// Will not add an item to the scene if the dropdown is already expanded, but will add the item upon next expansion.
        /// </summary>
        /// <param name="data">The data to add to the list.</param>
        public void AddData(OptionData data)
        {
            m_OptionDataList.options.Add(data);
        }

        /// <summary>
        /// Adds an array of OptionDatas to the dropdown data list.
        /// Will not add items to the scene if the dropdown is already expanded, but will add the items upon next expansion.
        /// </summary>
        /// <param name="data">The data array to add to the list.</param>
        public void AddData(OptionData[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                AddData(data[i]);
            }
        }

        /// <summary>
        /// Removes an OptionData from the dropdown data list.
        /// Will not remove the item from the scene if the dropdown is open, so it's recommended to only call this while the list is closed.
        /// </summary>
        /// <param name="data">The data to remove from the list.</param>
        public void RemoveData(OptionData data)
        {
            m_OptionDataList.options.Remove(data);

            m_CurrentlySelected = Mathf.Clamp(m_CurrentlySelected, 0, m_OptionDataList.options.Count - 1);
        }

        /// <summary>
        /// Removes an OptionData array from the dropdown data list.
        /// Will not remove the items from the scene if the dropdown is open, so it's recommended to only call this while the list is closed.
        /// </summary>
        /// <param name="data">The data array to remove from the list.</param>
        public void RemoveData(OptionData[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                RemoveData(data[i]);
            }
        }

        /// <summary>
        /// Clears the data list entirely.
        /// Will not remove the items from the scene if the dropdown is open, so it's recommended to only call this while the list is closed.
        /// </summary>
        public void ClearData()
        {
            m_OptionDataList.options.Clear();

            m_CurrentlySelected = Mathf.Clamp(m_CurrentlySelected, 0, m_OptionDataList.options.Count - 1);
        }

        /// <summary>
        /// See MonoBehaviour.Start.
        /// </summary>
        protected override void Start()
        {
            Canvas[] canvasses = FindObjectsOfType<Canvas>();

            for (int i = 0; i < canvasses.Length; i++)
            {
                if (canvasses[i].name == "Dropdown Canvas")
                {
                    m_DropdownCanvasGameObject = canvasses[i].gameObject;
                }
            }

            if (m_DropdownCanvasGameObject == null)
            {
                m_DropdownCanvasGameObject = new GameObject("Dropdown Canvas");
            }

            m_DropdownCanvas = m_BaseTransform.root.GetComponent<Canvas>().Copy(m_DropdownCanvasGameObject);
        }

        /// <summary>
        /// Instantiates the dropdown object and expands it.
        /// </summary>
        public void Show()
        {
            m_BaseTransform.root.GetComponent<Canvas>().CopySettingsToOtherCanvas(m_DropdownCanvas);
            m_DropdownCanvas.pixelPerfect = true;
            m_DropdownCanvas.sortingOrder = 30000;

            m_DropdownPanel = PrefabManager.InstantiateGameObject(PrefabManager.ResourcePrefabs.dropdownPanel, m_DropdownCanvas.transform).GetComponent<RectTransform>();
            m_PanelLayer = m_DropdownPanel.GetChildByName<RectTransform>("PanelLayer");
            m_DropdownCanvasGroup = m_DropdownPanel.GetComponent<CanvasGroup>();
            m_ShadowCanvasGroup = m_DropdownPanel.GetChildByName<CanvasGroup>("Shadow");

            m_CancelPanel = m_DropdownPanel.GetChildByName<RectTransform>("Cancel Panel");
            m_CancelPanel.sizeDelta = scaler.targetCanvas.pixelRect.size * 2;
            DropdownTrigger trigger = m_DropdownPanel.gameObject.GetChildByName<DropdownTrigger>("Cancel Panel");
            trigger.index = -1;
            trigger.dropdown = this;

            m_DropdownPanel.gameObject.GetChildByName<Image>("ScrollRect").color = m_PanelColor;

            m_ListItemTemplate = new DropdownListItem();

            m_ListItemTemplate.rectTransform = m_DropdownPanel.GetChildByName<RectTransform>("Item");
            m_ListItemTemplate.canvasGroup = m_ListItemTemplate.rectTransform.GetComponent<CanvasGroup>();
            m_ListItemTemplate.text = m_ListItemTemplate.rectTransform.GetChildByName<Text>("Text");

            if (m_OptionDataList.imageType == ImageDataType.Sprite)
            {
                m_ListItemTemplate.image = m_ListItemTemplate.rectTransform.GetChildByName<Image>("Icon");
                Destroy(m_ListItemTemplate.rectTransform.GetChildByName<VectorImage>("Icon").gameObject);
            }
            else
            {
                m_ListItemTemplate.image = m_ListItemTemplate.rectTransform.GetChildByName<VectorImage>("Icon");
                Destroy(m_ListItemTemplate.rectTransform.GetChildByName<Image>("Icon").gameObject);
            }

            m_ListItems = new List<DropdownListItem>();

            for (int i = 0; i < m_OptionDataList.options.Count; i++)
            {
                m_ListItems.Add(CreateItem(m_OptionDataList.options[i], i));
            }

            for (int i = 0; i < m_ListItems.Count; i++)
            {
                Selectable selectable = m_ListItems[i].rectTransform.GetComponent<Selectable>();
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;

                if (i > 0)
                {
                    navigation.selectOnUp = m_ListItems[i - 1].rectTransform.GetComponent<Selectable>();
                }
                if (i < m_ListItems.Count - 1)
                {
                    navigation.selectOnDown = m_ListItems[i + 1].rectTransform.GetComponent<Selectable>();
                }

                selectable.navigation = navigation;
            }

            if (m_BaseSelectable != null)
            {
                if (m_ListItems.Count > 0)
                {
                    Navigation navigation = Navigation.defaultNavigation;
                    navigation.selectOnDown = m_ListItems[0].rectTransform.GetComponent<Selectable>();
                    m_BaseSelectable.navigation = navigation;
                }
            }

            float maxWidth = CalculateMaxItemWidth();
            float buttonWidth = m_BaseTransform.rect.width;

            m_FullHeight = m_OptionDataList.options.Count * LayoutUtility.GetPreferredHeight(m_ListItemTemplate.rectTransform) + 16;

            m_ExpandedSize = new Vector2(Mathf.Max(maxWidth, buttonWidth), m_FullHeight);

            m_TempMaxHeight = m_MaxHeight;

            if (m_TempMaxHeight == 0)
            {
                m_TempMaxHeight = MaterialUIScaler.GetParentScaler(m_DropdownPanel).targetCanvas.GetComponent<RectTransform>().rect.height - 32;
            }

            if (m_ExpandedSize.y > m_TempMaxHeight)
            {
                m_ExpandedSize.y = m_TempMaxHeight;
            }
            else
            {
                m_DropdownPanel.GetChildByName<Image>("Handle").gameObject.SetActive(false);
            }

            Destroy(m_ListItemTemplate.rectTransform.gameObject);

            m_DropdownPanel.position = m_BaseTransform.GetPositionRegardlessOfPivot();

            if (m_ExpandStartType == ExpandStartType.ExpandFromBaseTransformWidth)
            {
                if (m_VerticalPivotType == VerticalPivotType.BelowBase)
                {
                    m_DropdownPanel.position = new Vector3(m_DropdownPanel.position.x, m_DropdownPanel.position.y - m_BaseTransform.GetProperSize().y / 2, m_DropdownPanel.position.z);
                }
                else if (m_VerticalPivotType == VerticalPivotType.AboveBase)
                {
                    m_DropdownPanel.position = new Vector3(m_DropdownPanel.position.x, m_DropdownPanel.position.y + m_BaseTransform.GetProperSize().y / 2, m_DropdownPanel.position.z);
                }
            }


            m_ExpandedPosition = CalculatedPosition();
            m_ExpandedPosition.z = m_BaseTransform.position.z;

            m_DropdownCanvasGroup.alpha = 0f;
            m_ShadowCanvasGroup.alpha = 0f;

            if (m_ExpandStartType == ExpandStartType.ExpandFromBaseTransformWidth)
            {
                m_DropdownPanel.sizeDelta = new Vector2(m_BaseTransform.rect.size.x, 0f);
            }
            else if (m_ExpandStartType == ExpandStartType.ExpandFromBaseTransformHeight)
            {
                m_DropdownPanel.sizeDelta = new Vector2(0f, m_BaseTransform.rect.size.y);
            }
            else if (m_ExpandStartType == ExpandStartType.ExpandFromBaseTransformSize)
            {
                m_DropdownPanel.sizeDelta = m_BaseTransform.rect.size;
            }
            else
            {
                m_DropdownPanel.sizeDelta = Vector2.zero;
            }

            m_DropdownPanel.gameObject.SetActive(true);

            for (int i = 0; i < m_ListItemAutoTweeners.Count; i++)
            {
                TweenManager.EndTween(m_ListItemAutoTweeners[i]);
            }

            m_AutoTweeners = new List<int>();
            m_ListItemAutoTweeners = new List<int>();

            m_AutoTweeners.Add(TweenManager.TweenFloat(f => m_DropdownCanvasGroup.alpha = f, m_DropdownCanvasGroup.alpha, 1f, m_AnimationDuration * 0.66f, 0, null, false, Tween.TweenType.Linear));
            m_AutoTweeners.Add(TweenManager.TweenFloat(f => m_ShadowCanvasGroup.alpha = f, m_ShadowCanvasGroup.alpha, 1f, m_AnimationDuration * 0.66f, 0, null, false, Tween.TweenType.Linear));

            m_AutoTweeners.Add(TweenManager.TweenVector2(vector2 => m_DropdownPanel.sizeDelta = vector2, m_DropdownPanel.sizeDelta, m_ExpandedSize, m_AnimationDuration, m_AnimationDuration / 3, null, false, Tween.TweenType.EaseInOutQuint));

            m_AutoTweeners.Add(TweenManager.TweenVector3(vector3 => m_DropdownPanel.position = vector3, m_DropdownPanel.position, m_ExpandedPosition, m_AnimationDuration, m_AnimationDuration / 3, () =>
            {
                if (m_BaseSelectable != null && m_IsExapanded)
                {
                    m_BaseSelectable.interactable = false;
                }

                Vector2 tempVector2 = m_PanelLayer.anchoredPosition;
                tempVector2.x = Mathf.RoundToInt(tempVector2.x);
                tempVector2.y = Mathf.RoundToInt(tempVector2.y);
                m_PanelLayer.anchoredPosition = tempVector2;
            }, false, Tween.TweenType.EaseInOutQuint));

            for (int i = 0; i < m_ListItems.Count; i++)
            {
                int i1 = i;
                CanvasGroup canvasGroup = m_ListItems[i].canvasGroup;
                m_ListItemAutoTweeners.Add(TweenManager.TweenFloat(f => canvasGroup.alpha = f, canvasGroup.alpha, 1f, m_AnimationDuration * 1.66f, (i1 * (m_AnimationDuration / 6) + m_AnimationDuration) - m_ScrollPosOffset / 800, null, false, Tween.TweenType.Linear));
            }

            if (m_FullHeight > m_TempMaxHeight)
            {
                m_DropdownPanel.GetChildByName<ScrollRect>("ScrollRect").gameObject.AddComponent<RectMask2D>();
            }

            m_IsExapanded = true;

            m_TimeShown = Time.unscaledTime;
        }

        /// <summary>
        /// Hides and destroys the dropdown object.
        /// </summary>
        public void Hide()
        {
            for (int i = 0; i < m_ListItemAutoTweeners.Count; i++)
            {
                TweenManager.EndTween(m_ListItemAutoTweeners[i]);
            }

            m_IsExapanded = false;

            if (m_BaseSelectable != null)
            {
                m_BaseSelectable.interactable = true;
            }

            for (int i = 0; i < m_ListItems.Count; i++)
            {
                int i1 = i;
                CanvasGroup canvasGroup = m_ListItems[i].canvasGroup;
                TweenManager.TweenFloat(f => canvasGroup.alpha = f, canvasGroup.alpha, 0f, m_AnimationDuration * 0.66f, (m_ListItems.Count - i1) * (m_AnimationDuration / 6), null, false, Tween.TweenType.Linear);
            }

            m_AutoTweeners.Add(TweenManager.TweenFloat(f => m_DropdownCanvasGroup.alpha = f, m_DropdownCanvasGroup.alpha, 0f, m_AnimationDuration * 0.66f, m_AnimationDuration, null, false, Tween.TweenType.Linear));

            TweenManager.TweenFloat(f => m_ShadowCanvasGroup.alpha = f, m_ShadowCanvasGroup.alpha, 0f, m_AnimationDuration * 0.66f, m_AnimationDuration, () =>
            {
                for (int i = 0; i < m_AutoTweeners.Count; i++)
                {
                    TweenManager.EndTween(m_AutoTweeners[i]);
                }

                Destroy(m_DropdownPanel.gameObject);
            }, false, Tween.TweenType.Linear);
        }

        /// <summary>
        /// Selects a specified item.
        /// </summary>
        /// <param name="selectedItem">The index of the item to select.</param>
        /// <param name="submitted">If true, select the base selectable after the list is closed (for controllers etc).</param>
        public void Select(int selectedItem, bool submitted = false)
        {
            if (Time.unscaledTime - m_TimeShown < m_IgnoreInputAfterShowTimer) return;

            if (!m_IsExapanded) return;

            if (selectedItem >= 0)
            {
                if (m_ButtonImageContent != null && m_UpdateHeader)
                {
                    m_ButtonImageContent.SetImage(m_OptionDataList.options[selectedItem].imageData);
                }

                if (m_ButtonTextContent != null && m_UpdateHeader)
                {
                    string itemText = m_OptionDataList.options[selectedItem].text;

                    if (m_CapitalizeButtonText)
                    {
                        itemText = itemText.ToUpper();
                    }

                    m_ButtonTextContent.text = itemText;
                }

                m_CurrentlySelected = selectedItem;
            }

            Hide();

            if (submitted && m_BaseSelectable != null)
            {
                EventSystem.current.SetSelectedGameObject(m_BaseSelectable.gameObject);
            }

            if (m_OnItemSelected != null)
            {
                m_OnItemSelected.Invoke(selectedItem);
            }

            if (selectedItem >= 0 && selectedItem < m_OptionDataList.options.Count)
            {
                m_OptionDataList.options[selectedItem].onOptionSelected.InvokeIfNotNull();
            }
        }

        /// <summary>
        /// Calculateds the position of the fully-expanded dropdown.
        /// </summary>
        /// <returns></returns>
        private Vector3 CalculatedPosition()
        {
            Vector3 position = m_BaseTransform.GetPositionRegardlessOfPivot();
            float itemHeight = m_ListItemTemplate.rectTransform.GetProperSize().y;
            float minScrollPos = 0f;
            float maxScrollPos = Mathf.Clamp(m_FullHeight - m_TempMaxHeight, 0f, float.MaxValue);

            int flipper = (int)m_VerticalPivotType < 3 ? 1 : -1;

            if (m_VerticalPivotType == VerticalPivotType.BelowBase || m_VerticalPivotType == VerticalPivotType.AboveBase)
            {
                float baseHeight = m_BaseTransform.GetProperSize().y;

                position.y -= m_ExpandedSize.y * scaler.scaler.scaleFactor / 2 * flipper;
                position.y -= baseHeight * scaler.scaler.scaleFactor / 2 * flipper;
            }
            else if (m_VerticalPivotType == VerticalPivotType.Top || m_VerticalPivotType == VerticalPivotType.Bottom)
            {
                position.y -= m_ExpandedSize.y * scaler.scaler.scaleFactor / 2 * flipper;
                position.y += itemHeight * scaler.scaler.scaleFactor / 2 * flipper;
                //  I have absolutely no idea why 3 works better than 4 (according to my math, it should be 4). I've probably missed something, but it works :)
                position.y -= 3 * scaler.scaler.scaleFactor * flipper;
            }
            else if (m_VerticalPivotType == VerticalPivotType.FirstItem || m_VerticalPivotType == VerticalPivotType.LastItem)
            {
                position.y -= m_ExpandedSize.y * scaler.scaler.scaleFactor / 2 * flipper;
                position.y += itemHeight * scaler.scaler.scaleFactor / 2 * flipper;
                position.y += 8 * scaler.scaler.scaleFactor * flipper;
            }

            if (m_HighlightCurrentlySelected)
            {
                Vector2 tempVector2 = m_PanelLayer.anchoredPosition;
                tempVector2.y += itemHeight * Mathf.Clamp(m_CurrentlySelected, 0, int.MaxValue);
                if (m_VerticalPivotType == VerticalPivotType.Center)
                {
                    tempVector2.y -= m_ExpandedSize.y / 2;
                    tempVector2.y += itemHeight / 2;
                    tempVector2.y += 8;
                }
                else if (m_VerticalPivotType == VerticalPivotType.LastItem)
                {
                    tempVector2.y -= m_ExpandedSize.y;
                    tempVector2.y += itemHeight;
                    tempVector2.y += 16;
                }
                tempVector2.y = Mathf.Clamp(tempVector2.y, minScrollPos, maxScrollPos);
                m_PanelLayer.anchoredPosition = tempVector2;

                m_ScrollPosOffset = tempVector2.y;
            }
            else
            {
                m_ScrollPosOffset = 0;
            }

            flipper = m_HorizontalPivotType == HorizontalPivotType.Left ? 1 : -1;

            if (m_HorizontalPivotType != HorizontalPivotType.Center)
            {
                position.x -= m_BaseTransform.GetProperSize().x * scaler.scaler.scaleFactor / 2 * flipper;
                position.x += m_ExpandedSize.x * scaler.scaler.scaleFactor / 2 * flipper;
            }

            RectTransform rootCanvasRectTransform = MaterialUIScaler.GetParentScaler(m_DropdownPanel).GetComponent<RectTransform>();

            //  Left edge
            float canvasEdge = rootCanvasRectTransform.position.x / scaler.scaler.scaleFactor - rootCanvasRectTransform.rect.width / 2;
            float dropdownEdge = position.x / scaler.scaler.scaleFactor - m_ExpandedSize.x / 2;
            if (dropdownEdge < canvasEdge + m_MinDistanceFromEdge)
            {
                position.x += (canvasEdge + m_MinDistanceFromEdge - dropdownEdge) * scaler.scaler.scaleFactor;
            }

            //  Right edge
            canvasEdge = rootCanvasRectTransform.position.x / scaler.scaler.scaleFactor + rootCanvasRectTransform.rect.width / 2;
            dropdownEdge = position.x / scaler.scaler.scaleFactor + m_ExpandedSize.x / 2;
            if (dropdownEdge > canvasEdge - m_MinDistanceFromEdge)
            {
                position.x -= (dropdownEdge - (canvasEdge - m_MinDistanceFromEdge)) * scaler.scaler.scaleFactor;
            }

            //  Top edge
            canvasEdge = rootCanvasRectTransform.position.y / scaler.scaler.scaleFactor + rootCanvasRectTransform.rect.height / 2;
            dropdownEdge = position.y / scaler.scaler.scaleFactor + m_ExpandedSize.y / 2;
            if (dropdownEdge > canvasEdge - m_MinDistanceFromEdge)
            {
                position.y -= (dropdownEdge - (canvasEdge - m_MinDistanceFromEdge)) * scaler.scaler.scaleFactor;
            }

            //  Bottom edge
            canvasEdge = rootCanvasRectTransform.position.y / scaler.scaler.scaleFactor - rootCanvasRectTransform.rect.height / 2;
            dropdownEdge = position.y / scaler.scaler.scaleFactor - m_ExpandedSize.y / 2;
            if (dropdownEdge < canvasEdge + m_MinDistanceFromEdge)
            {
                position.y += ((canvasEdge + m_MinDistanceFromEdge) - dropdownEdge) * scaler.scaler.scaleFactor;
            }

            return position;
        }

        /// <summary>
        /// Creates a single dropdown item and adds it to the dropdown list object in the scene.
        /// </summary>
        /// <param name="data">The data of the item.</param>
        /// <param name="index">The index of the item.</param>
        /// <returns>The instantiated DropdownListItem.</returns>
        private DropdownListItem CreateItem(OptionData data, int index)
        {
            DropdownListItem item = new DropdownListItem();

            GameObject itemGameObject = Instantiate(m_ListItemTemplate.rectTransform.gameObject);

            item.rectTransform = itemGameObject.GetComponent<RectTransform>();

            item.rectTransform.SetParent(m_ListItemTemplate.rectTransform.parent);
            item.rectTransform.localScale = Vector3.one;
            item.rectTransform.localEulerAngles = Vector3.zero;
            item.rectTransform.anchoredPosition3D = Vector3.zero;

            item.canvasGroup = item.rectTransform.GetComponent<CanvasGroup>();
            item.text = item.rectTransform.GetChildByName<Text>("Text");

            if (m_OptionDataList.imageType == ImageDataType.Sprite)
            {
                item.image = item.rectTransform.GetChildByName<Image>("Icon");
                Destroy(item.rectTransform.GetChildByName<VectorImage>("Icon").gameObject);
            }
            else
            {
                item.image = item.rectTransform.GetChildByName<VectorImage>("Icon");
                Destroy(item.rectTransform.GetChildByName<Image>("Icon").gameObject);
            }

            DropdownTrigger trigger = itemGameObject.GetComponent<DropdownTrigger>();
            trigger.index = index;
            trigger.dropdown = this;

            if (!string.IsNullOrEmpty(data.text))
            {
                item.text.text = data.text;
            }
            else
            {
                Destroy(item.text.gameObject);
            }

            if (data.imageData != null && data.imageData.ContainsData(true))
            {
                item.image.SetImage(data.imageData);
            }
            else
            {
                Destroy(item.image.gameObject);
            }

            itemGameObject.GetComponent<MaterialRipple>().rippleData = m_ItemRippleData.Copy();

            if (m_HighlightCurrentlySelected && index == m_CurrentlySelected)
            {
                itemGameObject.GetComponent<Image>().color = m_ItemRippleData.Color.WithAlpha(m_ItemRippleData.EndAlpha);
            }

            item.text.color = m_ItemTextColor;
            item.image.color = m_ItemIconColor;

            item.canvasGroup.alpha = 0f;

            return item;
        }

        /// <summary>
        /// Calculates the maximum single width from all the dropdown items.
        /// </summary>
        /// <returns></returns>
        private float CalculateMaxItemWidth()
        {
            TextGenerator textGenerator = new TextGenerator();
            TextGenerationSettings textGenerationSettings = m_ListItemTemplate.text.GetGenerationSettings(new Vector2(float.MaxValue, float.MaxValue));

            float maxWidth = 0f;

            for (int i = 0; i < m_OptionDataList.options.Count; i++)
            {
                float currentWidth = 0f;

                if (!string.IsNullOrEmpty(m_OptionDataList.options[i].text))
                {
                    currentWidth = textGenerator.GetPreferredWidth(m_OptionDataList.options[i].text, textGenerationSettings) / scaler.scaler.scaleFactor;
                    currentWidth += 16;
                }

                if (m_OptionDataList.imageType == ImageDataType.Sprite)
                {
                    if (m_OptionDataList.options[i].imageData.sprite != null)
                    {
                        currentWidth += m_ListItemTemplate.image.rectTransform.rect.width;
                        currentWidth += 16;
                    }
                }
                else
                {
                    if (m_OptionDataList.options[i].imageData != null && m_OptionDataList.options[i].imageData.vectorImageData != null)
                    {
                        currentWidth += m_ListItemTemplate.image.rectTransform.rect.width;
                        currentWidth += 16;
                    }
                }

                currentWidth += 16;

                maxWidth = Mathf.Max(maxWidth, currentWidth);
            }

            return maxWidth;
        }

#if UNITY_EDITOR
        /// <summary>
        /// See MonoBehaviour.OnValidate.
        /// </summary>
        protected override void OnValidate()
        {
            for (int i = 0; i < m_OptionDataList.options.Count; i++)
            {
                m_OptionDataList.options[i].imageData.imageDataType = m_OptionDataList.imageType;
            }

            m_CurrentlySelected = Mathf.Clamp(m_CurrentlySelected, -1, m_OptionDataList.options.Count - 1);

            if (m_CurrentlySelected >= 0)
            {
                if (m_ButtonImageContent != null && m_UpdateHeader)
                {
                    m_ButtonImageContent.SetImage(m_OptionDataList.options[m_CurrentlySelected].imageData);
                }

                if (m_ButtonTextContent != null && m_UpdateHeader)
                {
                    string itemText = m_OptionDataList.options[m_CurrentlySelected].text;

                    if (m_CapitalizeButtonText)
                    {
                        itemText = itemText.ToUpper();
                    }

                    m_ButtonTextContent.text = itemText;
                }
            }
        }
#endif
    }
}