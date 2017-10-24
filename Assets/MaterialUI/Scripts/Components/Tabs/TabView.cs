//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.


using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [ExecuteInEditMode]
    [AddComponentMenu("MaterialUI/TabView", 100)]
    public class TabView : UIBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        private bool m_AutoTrackPages = true;

        [SerializeField]
        private bool m_OnlyShowSelectedPage = true;

        private GameObject m_OldSelectionObjects;

        private bool m_PagesDirty;
#endif

        [SerializeField]
        private float m_ShrinkTabsToFitThreshold = 16f;
        public float shrinkTabsToFitThreshold
        {
            get { return m_ShrinkTabsToFitThreshold; }
            set { m_ShrinkTabsToFitThreshold = value; }
        }

        [SerializeField]
        private bool m_ForceStretchTabsOnLanscape = false;
        public bool forceStretchTabsOnLanscape
        {
            get { return m_ForceStretchTabsOnLanscape; }
            set { m_ForceStretchTabsOnLanscape = value; }
        }

        [SerializeField]
        private RectTransform m_TabsContainer;
        public RectTransform tabsContainer
        {
            get { return m_TabsContainer; }
            set { m_TabsContainer = value; }
        }

        [SerializeField]
        private TabPage[] m_Pages;
        public TabPage[] pages
        {
            get { return m_Pages; }
            set { m_Pages = value; }
        }

        [SerializeField]
        private int m_CurrentPage;
        public int currentPage
        {
            get { return m_CurrentPage; }
            set { m_CurrentPage = value; }
        }

        [SerializeField]
        private TabItem m_TabItemTemplate;
        public TabItem tabItemTemplate
        {
            get { return m_TabItemTemplate; }
            set { m_TabItemTemplate = value; }
        }

        [SerializeField]
        private RectTransform m_PagesContainer;
        public RectTransform pagesContainer
        {
            get { return m_PagesContainer; }
            set { m_PagesContainer = value; }
        }

        [SerializeField]
        private RectTransform m_PagesRect;
        public RectTransform pagesRect
        {
            get { return m_PagesRect; }
            set { m_PagesRect = value; }
        }

        [SerializeField]
        private RectTransform m_Indicator;
        public RectTransform indicator
        {
            get { return m_Indicator; }
            set { m_Indicator = value; }
        }

        private ScrollRect m_PagesScrollRect;
        public ScrollRect pagesScrollRect
        {
            get
            {
                if (m_PagesScrollRect == null)
                {
                    m_PagesScrollRect = m_PagesRect.GetComponent<ScrollRect>();
                }
                return m_PagesScrollRect;
            }
        }

        private float m_TabWidth;
        public float tabWidth
        {
            get { return m_TabWidth; }
        }

        private float m_TabPadding = 12;
        public float tabPadding
        {
            get { return m_TabPadding; }
            set { m_TabPadding = value; }
        }

        private TabItem[] m_Tabs;
        public TabItem[] tabs
        {
            get { return m_Tabs; }
        }

        [SerializeField]
        private bool m_LowerUnselectedTabAlpha = true;
        public bool lowerUnselectedTabAlpha
        {
            get { return m_LowerUnselectedTabAlpha; }
            set { m_LowerUnselectedTabAlpha = value; }
        }

        [SerializeField]
        private bool m_CanScrollBetweenTabs = true;
        public bool canScrollBetweenTabs
        {
            get { return m_CanScrollBetweenTabs; }
            set
            {
                m_CanScrollBetweenTabs = value;
                pagesScrollRect.enabled = value;
                OverscrollConfig overscroll = pagesScrollRect.GetComponent<OverscrollConfig>();
                if (overscroll != null)
                {
                    overscroll.enabled = value;
                }
            }
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


        private int m_IndicatorTweener;
        private int m_TabsContainerTweener;
        private int m_PagesContainerTweener;

        private Vector2 m_PageSize;
        private bool m_AlreadyInitialized;

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
        }

        protected override void OnDisable()
        {
            EditorUpdate.onEditorUpdate -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (IsDestroyed())
            {
                EditorUpdate.onEditorUpdate -= OnEditorUpdate;
                return;
            }

            if (m_AutoTrackPages)
            {
                TabPage[] tempPages = GetComponentsInChildren<TabPage>(true);

                List<TabPage> ownedTempPages = new List<TabPage>();

                for (int i = 0; i < tempPages.Length; i++)
                {
                    if (tempPages[i].transform.parent.parent.parent == transform)
                    {
                        ownedTempPages.Add(tempPages[i]);
                    }
                }

                m_Pages = new TabPage[ownedTempPages.Count];

                for (int i = 0; i < ownedTempPages.Count; i++)
                {
                    m_Pages[i] = ownedTempPages[i];
                }
            }

            if (m_OldSelectionObjects != Selection.activeGameObject)
            {
                m_OldSelectionObjects = Selection.activeGameObject;
                m_PagesDirty = true;
            }

            if (m_Pages.Length > 0 && m_PagesDirty)
            {
                m_PagesDirty = false;

                bool pageSelected = false;

                if (m_OnlyShowSelectedPage)
                {
                    for (int i = 0; i < m_Pages.Length; i++)
                    {
                        RectTransform[] children = m_Pages[i].GetComponentsInChildren<RectTransform>(true);

                        bool objectSelected = false;

                        for (int j = 0; j < children.Length; j++)
                        {
                            if (Selection.Contains(children[j].gameObject))
                            {
                                if (!m_Pages[i].gameObject.activeSelf)
                                {
                                    m_Pages[i].gameObject.SetActive(true);
                                }
                                pageSelected = true;
                                objectSelected = true;
                            }
                        }
                        if (!objectSelected)
                        {
                            if (m_Pages[i].gameObject.activeSelf)
                            {
                                m_Pages[i].gameObject.SetActive(false);
                            }
                        }
                    }

                    if (!pageSelected && !m_Pages[m_CurrentPage].gameObject.activeSelf)
                    {
                        if (!m_Pages[m_CurrentPage].gameObject.activeSelf)
                        {
                            m_Pages[m_CurrentPage].gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
#endif

        protected override void Start()
        {
            if (Application.isPlaying)
            {
                InitializeTabs();
                InitializePages();
            }

            MaterialUIScaler.GetParentScaler(transform).OnOrientationChange += resolution =>
            {
                if (Application.isPlaying)
                {
                    InitializeTabs();
                    InitializePages();
                }
            };
        }

#if UNITY_EDITOR
        public void SetPagesDirty()
        {
            m_PagesDirty = true;
        }
#endif

        public void InitializeTabs()
        {
            if (m_AlreadyInitialized)
            {
                for (int i = 0; i < m_Tabs.Length; i++)
                {
                    Destroy(m_Tabs[i].gameObject);
                }
                m_TabItemTemplate.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }

            float barWidth = rectTransform.GetProperSize().x;

            m_TabWidth = GetMaxTabTextWidth() + (2 * m_TabPadding);

            m_TabsContainer.GetComponent<LayoutElement>().minWidth = barWidth;
            m_TabsContainer.GetComponent<ContentSizeFitter>().enabled = true;

            float combinedWidth = m_TabWidth * m_Pages.Length;

            m_TabsContainer.GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = true;
            m_TabItemTemplate.GetComponent<LayoutElement>().minWidth = 72;

            if (Screen.width > Screen.height && !m_ForceStretchTabsOnLanscape)
            {
                if (Mathf.Abs(combinedWidth - barWidth) < m_ShrinkTabsToFitThreshold)
                {
                    m_TabWidth = barWidth / m_Pages.Length;
                }
                else
                {
                    m_TabsContainer.GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = false;
                    m_TabItemTemplate.GetComponent<LayoutElement>().minWidth = 160;
                }
            }
            else
            {
                if (combinedWidth - barWidth < m_ShrinkTabsToFitThreshold)
                {
                    m_TabWidth = barWidth / m_Pages.Length;
                }
            }

            m_TabWidth = Mathf.Max(m_TabWidth, m_TabItemTemplate.GetComponent<LayoutElement>().minWidth);

            m_TabItemTemplate.GetComponent<LayoutElement>().preferredWidth = m_TabWidth;
            m_Indicator.anchorMin = new Vector2(0, 0);
            m_Indicator.anchorMax = new Vector2(0, 0);
            m_Indicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_TabWidth);

            m_Tabs = new TabItem[m_Pages.Length];

            for (int i = 0; i < m_Pages.Length; i++)
            {
                TabItem tab = Instantiate(m_TabItemTemplate.gameObject).GetComponent<TabItem>();

                tab.rectTransform.SetParent(m_TabItemTemplate.transform.parent);

                tab.rectTransform.localScale = Vector3.one;
                tab.rectTransform.localEulerAngles = Vector3.zero;
                tab.rectTransform.localPosition = new Vector3(tab.rectTransform.localPosition.x, tab.rectTransform.localPosition.y, 0f);

                tab.id = i;

                if (!string.IsNullOrEmpty(m_Pages[i].tabName))
                {
                    tab.name = m_Pages[i].tabName;
                    if (tab.itemText != null)
                    {
                        tab.itemText.text = tab.name.ToUpper();
                    }
                }
                else
                {
                    tab.name = "Tab " + i;
                    if (tab.itemText != null)
                    {
                        tab.itemText.enabled = false;
                    }
                }

                tab.SetupGraphic(m_Pages[i].tabIcon.imageDataType);

                if (tab.itemIcon != null)
                {
                    if (m_Pages[i].tabIcon != null)
                    {
                        tab.itemIcon.SetImage(m_Pages[i].tabIcon);
                    }
                    else
                    {
                        tab.itemIcon.enabled = false;
                    }
                }

                m_Tabs[i] = tab;
            }

            m_TabItemTemplate.gameObject.SetActive(false);

            m_TabsContainer.anchorMin = Vector2.zero;
            m_TabsContainer.anchorMax = new Vector2(0, 1);

            OverscrollConfig overscrollConfig = m_TabsContainer.parent.GetComponent<OverscrollConfig>();

            if (overscrollConfig != null)
            {
                overscrollConfig.Setup();
            }

            m_AlreadyInitialized = true;
        }

        private void InitializePages()
        {
            if (m_Pages.Length > 0)
            {
                for (int i = 0; i < m_Pages.Length; i++)
                {
                    m_Pages[i].gameObject.SetActive(true);
                }
            }

            m_PageSize = m_PagesRect.GetProperSize();

            for (int i = 0; i < m_Pages.Length; i++)
            {
                RectTransform page = m_Pages[i].rectTransform;

                page.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, i * m_PageSize.x, m_PageSize.x);
            }

            m_PagesContainer.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, m_PageSize.x * m_Pages.Length);

            OverscrollConfig overscrollConfig = m_PagesRect.GetComponent<OverscrollConfig>();

            if (overscrollConfig != null)
            {
                overscrollConfig.Setup();
            }

            SetPage(m_CurrentPage, false);
        }

        private float GetMaxTabTextWidth()
        {
            float longestTextWidth = 0;

            if (m_TabItemTemplate.itemText != null)
            {
                TextGenerator textGenerator = m_TabItemTemplate.itemText.cachedTextGeneratorForLayout;
                TextGenerationSettings textGenerationSettings = m_TabItemTemplate.itemText.GetGenerationSettings(new Vector2(float.MaxValue, float.MaxValue));

                for (int i = 0; i < m_Pages.Length; i++)
                {
                    longestTextWidth = Mathf.Max(longestTextWidth, textGenerator.GetPreferredWidth(m_Pages[i].name.ToUpper(), textGenerationSettings));
                }
            }

            return longestTextWidth;
        }

        public void SetPage(int index)
        {
            SetPage(index, true);
        }

        public void SetPage(int index, bool animate)
        {
            index = Mathf.Clamp(index, 0, m_Pages.Length - 1);

            TweenIndicator(index, animate);
            TweenTabsContainer(index, animate);
            TweenPagesContainer(index, animate);

            if (m_LowerUnselectedTabAlpha)
            {
                if (animate)
                {
                    for (int i = 0; i < m_Tabs.Length; i++)
                    {
                        int i1 = i;
                        TweenManager.TweenFloat(f => m_Tabs[i1].canvasGroup.alpha = f, () => m_Tabs[i1].canvasGroup.alpha, () => m_Pages[i1].interactable ? (i1 == index ? 1f : 0.5f) : 0.15f, 0.5f);
                    }
                }
                else
                {
                    for (int i = 0; i < m_Tabs.Length; i++)
                    {
                        m_Tabs[i].canvasGroup.alpha = m_Pages[i].interactable ? (i == index ? 1f : 0.5f) : 0.15f;
                    }
                }
            }
        }

        private void OnPagesTweenEnd()
        {
            for (int i = 0; i < m_Pages.Length; i++)
            {
                if (i >= m_CurrentPage - 1 && i <= m_CurrentPage + 1)
                {
                    m_Pages[i].gameObject.SetActive(true);
                }
                else
                {
                    m_Pages[i].DisableIfAllowed();
                }
            }
        }

        private void TweenPagesContainer(int index, bool animate = true)
        {
            for (int i = 0; i < m_Pages.Length; i++)
            {
                int smaller = Mathf.Min(m_CurrentPage, index);
                int bigger = Mathf.Max(m_CurrentPage, index);

                if (i >= smaller - 1 && i <= bigger + 1)
                {
                    m_Pages[i].gameObject.SetActive(true);
                }
                else
                {
                    m_Pages[i].DisableIfAllowed();
                }
            }

            float targetPosition = -(index * m_PageSize.x);

            targetPosition = Mathf.Clamp(targetPosition, -(m_Pages.Length * m_PageSize.x), 0);

            TweenManager.EndTween(m_PagesContainerTweener);

            m_CurrentPage = index;

            if (animate)
            {
                m_PagesContainerTweener =
                    TweenManager.TweenVector2(vector2 => m_PagesContainer.anchoredPosition = vector2,
                        m_PagesContainer.anchoredPosition, new Vector2(targetPosition, 0), 0.5f);
            }
            else
            {
                m_PagesContainer.anchoredPosition = new Vector2(targetPosition, 0);
                OnPagesTweenEnd();
            }
        }

        private void TweenTabsContainer(int index, bool animate = true)
        {
            float targetPosition = -(index * m_TabWidth);

            targetPosition += rectTransform.GetProperSize().x / 2;
            targetPosition -= m_TabWidth / 2;

            targetPosition = Mathf.Clamp(targetPosition, -LayoutUtility.GetPreferredWidth(m_TabsContainer) + rectTransform.GetProperSize().x, 0);

            TweenManager.EndTween(m_TabsContainerTweener);

            if (animate)
            {
                m_TabsContainerTweener = TweenManager.TweenVector2(
                    vector2 => m_TabsContainer.anchoredPosition = vector2, m_TabsContainer.anchoredPosition,
                    new Vector2(targetPosition, 0), 0.5f);
            }
            else
            {
                m_TabsContainer.anchoredPosition = new Vector2(targetPosition, 0);
            }
        }

        private void TweenIndicator(int targetTab, bool animate = true)
        {
            float targetPosition = targetTab * m_TabWidth;


            TweenManager.EndTween(m_IndicatorTweener);

            if (animate)
            {
                m_IndicatorTweener = TweenManager.TweenVector2(vector2 => m_Indicator.anchoredPosition = vector2, m_Indicator.anchoredPosition, new Vector2(targetPosition, 0), 0.5f);
            }
            else
            {
                m_Indicator.anchoredPosition = new Vector2(targetPosition, 0);
            }
        }

        public void TabItemPointerDown(int id)
        {
            TweenManager.EndTween(m_TabsContainerTweener);
        }

        public void TabPagePointerUp(float delta)
        {
            if (m_CanScrollBetweenTabs)
            {
                pagesScrollRect.velocity = Vector2.zero;

                if (Mathf.Abs(delta) < 1)
                {
                    SetPage(NearestPage());
                }
                else
                {
                    if (delta < 0)
                    {
                        SetPage(NearestPage(1));
                    }
                    else
                    {
                        SetPage(NearestPage(-1));
                    }
                }
            }
        }

        private int NearestPage(int direction = 0)
        {
            float currentPosition = -m_PagesContainer.anchoredPosition.x;

            if (direction < 0)
            {
                return Mathf.FloorToInt(currentPosition / m_PageSize.x);
            }

            if (direction > 0)
            {
                return Mathf.CeilToInt(currentPosition / m_PageSize.x);
            }

            return Mathf.RoundToInt(currentPosition / m_PageSize.x);
        }

        public void TabPageDrag()
        {
            if (m_CanScrollBetweenTabs)
            {
                if (TweenManager.TweenIsActive(m_PagesContainerTweener))
                {
                    TweenManager.EndTween(m_PagesContainerTweener);

                    m_CurrentPage = NearestPage();

                    OnPagesTweenEnd();

                    TweenIndicator(m_CurrentPage);
                }

                TweenManager.EndTween(m_IndicatorTweener);

                float normalizedPagesContainerPosition = -m_PagesContainer.anchoredPosition.x / (m_PageSize.x * m_Pages.Length);

                m_Indicator.anchoredPosition = new Vector2((m_TabWidth * m_Tabs.Length) * normalizedPagesContainerPosition, 0);
            }
        }
    }
}