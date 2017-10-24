//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MaterialUI
{
    /// <summary>
    /// Component that handles a NavDrawer control.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="UnityEngine.EventSystems.IBeginDragHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IDragHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IEndDragHandler" />
    [AddComponentMenu("MaterialUI/Material Nav Drawer", 100)]
    public class MaterialNavDrawer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// The background image of the drawer.
        /// </summary>
        [SerializeField]
        private Image m_BackgroundImage;
        /// <summary>
        /// The background image of the drawer.
        /// </summary>
        public Image backgroundImage
        {
            get { return m_BackgroundImage; }
            set { m_BackgroundImage = value; }
        }

        /// <summary>
        /// The image of the drawer's shadow.
        /// </summary>
        [SerializeField]
        private Image m_ShadowImage;
        /// <summary>
        /// The image of the drawer's shadow.
        /// </summary>
        public Image shadowImage
        {
            get { return m_ShadowImage; }
            set { m_ShadowImage = value; }
        }

        /// <summary>
        /// The drawer's panel layer.
        /// </summary>
        [SerializeField]
        private GameObject m_PanelLayer;
        /// <summary>
        /// The drawer's panel layer.
        /// </summary>
        public GameObject panelLayer
        {
            get { return m_PanelLayer; }
            set { m_PanelLayer = value; }
        }

        /// <summary>
        /// Should the background be darkened when the drawer is opened?
        /// </summary>
        [SerializeField]
        private bool m_DarkenBackground = true;
        /// <summary>
        /// Should the background be darkened when the drawer is opened?
        /// </summary>
        public bool darkenBackground
        {
            get { return m_DarkenBackground; }
            set { m_DarkenBackground = value; }
        }

        /// <summary>
        /// Should the drawer close if the backround area is tapped/clicked?
        /// </summary>
        [SerializeField]
        private bool m_TapBackgroundToClose = true;
        /// <summary>
        /// Should the drawer close if the backround area is tapped/clicked?
        /// </summary>
        public bool tapBackgroundToClose
        {
            get { return m_TapBackgroundToClose; }
            set { m_TapBackgroundToClose = value; }
        }

        /// <summary>
        /// Should the drawer open on Start?
        /// </summary>
        [SerializeField]
        private bool m_OpenOnStart;
        /// <summary>
        /// Should the drawer open on Start?
        /// </summary>
        public bool openOnStart
        {
            get { return m_OpenOnStart; }
            set { m_OpenOnStart = value; }
        }

        /// <summary>
        /// The duration of the open/close animations.
        /// </summary>
        [SerializeField]
        private float m_AnimationDuration = 0.5f;
        /// <summary>
        /// The duration of the open/close animations.
        /// </summary>
        public float animationDuration
        {
            get { return m_AnimationDuration; }
            set { m_AnimationDuration = value; }
        }

        /// <summary>
        /// The root MaterialUIScaler.
        /// </summary>
        private MaterialUIScaler m_Scaler;
        /// <summary>
        /// The root MaterialUIScaler.
        /// If null, gets the root scaler if one exists.
        /// </summary>
        public MaterialUIScaler scaler
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
        /// The maximum position.
        /// </summary>
        private float m_MaxPosition;
        /// <summary>
        /// The minimum position.
        /// </summary>
        private float m_MinPosition;
        /// <summary>
        /// The rect transform.
        /// </summary>
        private RectTransform m_RectTransform;

        /// <summary>
        /// The background game object.
        /// </summary>
        private GameObject m_BackgroundGameObject;
        /// <summary>
        /// The background rect transform.
        /// </summary>
        private RectTransform m_BackgroundRectTransform;
        /// <summary>
        /// The background canvas group.
        /// </summary>
        private CanvasGroup m_BackgroundCanvasGroup;
        /// <summary>
        /// The shadow game object.
        /// </summary>
        private GameObject m_ShadowGameObject;
        /// <summary>
        /// The shadow canvas group.
        /// </summary>
        private CanvasGroup m_ShadowCanvasGroup;

        /// <summary>
        /// The anim state.
        /// </summary>
        private byte m_AnimState;
        /// <summary>
        /// The anim start time.
        /// </summary>
        private float m_AnimStartTime;
        /// <summary>
        /// The anim delta time.
        /// </summary>
        private float m_AnimDeltaTime;

        /// <summary>
        /// The current position.
        /// </summary>
        private Vector2 m_CurrentPos;
        /// <summary>
        /// The current background alpha.
        /// </summary>
        private float m_CurrentBackgroundAlpha;
        /// <summary>
        /// The current shadow alpha.
        /// </summary>
        private float m_CurrentShadowAlpha;

        /// <summary>
        /// The temporary vector2.
        /// </summary>
        private Vector2 m_TempVector2;

        /// <summary>
        /// See Monobehaviour.Awake.
        /// </summary>
        void Awake()
        {
            m_RectTransform = gameObject.GetComponent<RectTransform>();
            m_BackgroundRectTransform = m_BackgroundImage.GetComponent<RectTransform>();
            m_BackgroundCanvasGroup = m_BackgroundImage.GetComponent<CanvasGroup>();
            m_ShadowCanvasGroup = m_ShadowImage.GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// See Monobehaviour.Start.
        /// </summary>
        void Start()
        {
            m_MaxPosition = m_RectTransform.rect.width / 2;
            m_MinPosition = -m_MaxPosition;

            RefreshBackgroundSize();

            m_BackgroundGameObject = m_BackgroundImage.gameObject;
            m_ShadowGameObject = m_ShadowImage.gameObject;

            if (m_OpenOnStart)
            {
                Open();
            }
            else
            {
                m_BackgroundGameObject.SetActive(false);
                m_ShadowGameObject.SetActive(false);
                m_PanelLayer.SetActive(false);
            }
        }

        /// <summary>
        /// Called when the background is tapped.
        /// Closes the drawer.
        /// </summary>
        public void BackgroundTap()
        {
            if (m_TapBackgroundToClose)
            {
                Close();
            }
        }

        /// <summary>
        /// Opens the drawer.
        /// </summary>
        public void Open()
        {
            RefreshBackgroundSize();
            m_BackgroundGameObject.SetActive(true);
            m_ShadowGameObject.SetActive(true);
            m_PanelLayer.SetActive(true);
            m_CurrentPos = m_RectTransform.anchoredPosition;
            m_CurrentBackgroundAlpha = m_BackgroundCanvasGroup.alpha;
            m_CurrentShadowAlpha = m_ShadowCanvasGroup.alpha;
            m_BackgroundCanvasGroup.blocksRaycasts = true;
            m_AnimStartTime = Time.realtimeSinceStartup;
            m_AnimState = 1;
        }

        /// <summary>
        /// Closes the drawer.
        /// </summary>
        public void Close()
        {
            m_CurrentPos = m_RectTransform.anchoredPosition;
            m_CurrentBackgroundAlpha = m_BackgroundCanvasGroup.alpha;
            m_CurrentShadowAlpha = m_ShadowCanvasGroup.alpha;
            m_BackgroundCanvasGroup.blocksRaycasts = false;
            m_AnimStartTime = Time.realtimeSinceStartup;
            m_AnimState = 2;
        }

        private void RefreshBackgroundSize()
        {
            m_BackgroundRectTransform.sizeDelta = new Vector2((Screen.width / scaler.scaler.scaleFactor) + 1f, m_BackgroundRectTransform.sizeDelta.y);
        }

        /// <summary>
        /// See Monobehaviour.Update.
        /// </summary>
        void Update()
        {
            if (m_AnimState == 1)
            {
                m_AnimDeltaTime = Time.realtimeSinceStartup - m_AnimStartTime;

                if (m_AnimDeltaTime <= m_AnimationDuration)
                {
                    m_RectTransform.anchoredPosition = Tween.QuintOut(m_CurrentPos, new Vector2(m_MaxPosition, m_RectTransform.anchoredPosition.y), m_AnimDeltaTime, m_AnimationDuration);

                    if (m_DarkenBackground)
                    {
                        m_BackgroundCanvasGroup.alpha = Tween.QuintOut(m_CurrentBackgroundAlpha, 1f, m_AnimDeltaTime, m_AnimationDuration);
                    }

                    m_ShadowCanvasGroup.alpha = Tween.QuintIn(m_CurrentShadowAlpha, 1f, m_AnimDeltaTime, m_AnimationDuration / 2f);
                }
                else
                {
                    m_RectTransform.anchoredPosition = new Vector2(m_MaxPosition, m_RectTransform.anchoredPosition.y);
                    if (m_DarkenBackground)
                    {
                        m_BackgroundCanvasGroup.alpha = 1f;
                    }
                    m_AnimState = 0;
                }
            }
            else if (m_AnimState == 2)
            {
                m_AnimDeltaTime = Time.realtimeSinceStartup - m_AnimStartTime;

                if (m_AnimDeltaTime <= m_AnimationDuration)
                {
                    m_RectTransform.anchoredPosition = Tween.QuintOut(m_CurrentPos, new Vector2(m_MinPosition, m_RectTransform.anchoredPosition.y), m_AnimDeltaTime, m_AnimationDuration);

                    if (m_DarkenBackground)
                    {
                        m_BackgroundCanvasGroup.alpha = Tween.QuintOut(m_CurrentBackgroundAlpha, 0f, m_AnimDeltaTime, m_AnimationDuration);
                    }

                    m_ShadowCanvasGroup.alpha = Tween.QuintIn(m_CurrentShadowAlpha, 0f, m_AnimDeltaTime, m_AnimationDuration);
                }
                else
                {
                    m_RectTransform.anchoredPosition = new Vector2(m_MinPosition, m_RectTransform.anchoredPosition.y);
                    if (m_DarkenBackground)
                    {
                        m_BackgroundCanvasGroup.alpha = 0f;
                    }

                    m_BackgroundGameObject.SetActive(false);
                    m_ShadowGameObject.SetActive(false);
                    m_PanelLayer.SetActive(false);

                    m_AnimState = 0;
                }
            }

            m_RectTransform.anchoredPosition = new Vector2(Mathf.Clamp(m_RectTransform.anchoredPosition.x, m_MinPosition, m_MaxPosition), m_RectTransform.anchoredPosition.y);
        }

        /// <summary>
        /// Called when [begin drag].
        /// </summary>
        /// <param name="data">The data.</param>
        public void OnBeginDrag(PointerEventData data)
        {
            RefreshBackgroundSize();

            m_AnimState = 0;

            m_BackgroundGameObject.SetActive(true);
            m_ShadowGameObject.SetActive(true);
            m_PanelLayer.SetActive(true);
        }

        /// <summary>
        /// Called when [drag].
        /// </summary>
        /// <param name="data">The data.</param>
        public void OnDrag(PointerEventData data)
        {
            m_TempVector2 = m_RectTransform.anchoredPosition;
            m_TempVector2.x += data.delta.x / scaler.scaler.scaleFactor;

            m_RectTransform.anchoredPosition = m_TempVector2;

            if (m_DarkenBackground)
            {
                m_BackgroundCanvasGroup.alpha = 1 - (m_MaxPosition - m_RectTransform.anchoredPosition.x) / (m_MaxPosition - m_MinPosition);
            }

            m_ShadowCanvasGroup.alpha = 1 - (m_MaxPosition - m_RectTransform.anchoredPosition.x) / ((m_MaxPosition - m_MinPosition) * 2);
        }

        /// <summary>
        /// Called when [end drag].
        /// </summary>
        /// <param name="data">The data.</param>
        public void OnEndDrag(PointerEventData data)
        {
            if (Mathf.Abs(data.delta.x) >= 0.5f)
            {
                if (data.delta.x > 0.5f)
                {
                    Open();
                }
                else
                {
                    Close();
                }
            }
            else
            {
                if ((m_RectTransform.anchoredPosition.x - m_MinPosition) >
                    (m_MaxPosition - m_RectTransform.anchoredPosition.x))
                {
                    Open();
                }
                else
                {
                    Close();
                }
            }
        }
    }
}