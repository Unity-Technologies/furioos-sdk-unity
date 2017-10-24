//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Component that creates and animates Ripple objects, and modifies the target graphic's color with a highlight effect if interacted with.
    /// Responsible for handling the target graphic's color.
    /// </summary>
    /// <seealso cref="MaterialUI.Ripple" />
    /// <seealso cref="MaterialUI.RippleManager" />
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="UnityEngine.EventSystems.IPointerEnterHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IPointerDownHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IPointerUpHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IPointerExitHandler" />
    /// <seealso cref="MaterialUI.IRippleCreator" />
    /// <seealso cref="UnityEngine.EventSystems.ISelectHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IDeselectHandler" />
    /// <seealso cref="UnityEngine.EventSystems.ISubmitHandler" />
    [AddComponentMenu("MaterialUI/Material Ripple", 50)]
    [ExecuteInEditMode]
    public class MaterialRipple : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IRippleCreator, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        /// <summary>
        /// The type of event that can trigger a ripple animation.
        /// </summary>
        public enum HighlightActive
        {
            Never,
            Hovered,
            Clicked
        }

        [SerializeField]
        private bool m_RipplesEnabled = true;
        public bool ripplesEnabled
        {
            get { return m_RipplesEnabled; }
            set { m_RipplesEnabled = value; }
        }

        /// <summary>
        /// Should the ripple color match the color of a specified Graphic?
        /// </summary>
        [SerializeField]
        private bool m_AutoMatchGraphicColor = true;
        /// <summary>
        /// Should the ripple color match the color of a specified Graphic?
        /// </summary>
        public bool autoMatchGraphicColor
        {
            get { return m_AutoMatchGraphicColor; }
            set
            {
                m_AutoMatchGraphicColor = value;
                RefreshSettings();
            }
        }

        /// <summary>
        /// The Graphic to use to match the ripple color, if autoMatchGraphicColor is true.
        /// </summary>
        [SerializeField]
        private Graphic m_ColorMatchGraphic;
        /// <summary>
        /// The Graphic to use to match the ripple color, if autoMatchGraphicColor is true.
        /// </summary>
        public Graphic colorMatchGraphic
        {
            get { return m_ColorMatchGraphic; }
            set
            {
                m_ColorMatchGraphic = value;
                RefreshSettings();
            }
        }

        /// <summary>
        /// The RippleData to send to each Ripple as it is created.
        /// </summary>
        [SerializeField]
        private RippleData m_RippleData = new RippleData();
        /// <summary>
        /// The RippleData to send to each Ripple as it is created.
        /// </summary>
        public RippleData rippleData
        {
            get { return m_RippleData; }
            set { m_RippleData = value; }
        }

        /// <summary>
        /// Gets or sets the RippleData's rippleColor.
        /// </summary>
        public Color rippleColor
        {
            get { return m_RippleData.Color; }
            set
            {
                m_RippleData.Color = value;
                RefreshSettings();
            }
        }

        /// <summary>
        /// The Graphic to modify when applying the highlight effect.
        /// </summary>
        [SerializeField]
        private Graphic m_HighlightGraphic;
        /// <summary>
        /// The Graphic to modify when applying the highlight effect.
        /// If null, automatically gets the RippleData's RippleParent's Graphic component if one exists.
        /// </summary>
        public Graphic highlightGraphic
        {
            get
            {
                if (m_HighlightGraphic == null)
                {
                    if (m_RippleData.RippleParent != null)
                    {
                        m_HighlightGraphic = m_RippleData.RippleParent.GetComponent<Graphic>();
                    }
                }
                return m_HighlightGraphic;
            }
            set { m_HighlightGraphic = value; }
        }

        /// <summary>
        /// The type of event that can trigger a ripple animation
        /// </summary>
        [SerializeField]
        private HighlightActive m_HighlightWhen = HighlightActive.Clicked;
        /// <summary>
        /// The type of event that can trigger a ripple animation
        /// </summary>
        public HighlightActive highlightWhen
        {
            get { return m_HighlightWhen; }
            set { m_HighlightWhen = value; }
        }

        /// <summary>
        /// Should the (absolute) highlight color be calculated from the RippleData's rippleColor and the target graphic?
        /// </summary>
        [SerializeField]
        private bool m_AutoHighlightColor = true;
        /// <summary>
        /// Should the (absolute) highlight color be calculated from the RippleData's rippleColor and the target graphic?
        /// </summary>
        public bool autoHighlightColor
        {
            get { return m_AutoHighlightColor; }
            set
            {
                m_AutoHighlightColor = value;
                if (value)
                {
                    RefreshSettings();
                }
            }
        }

        /// <summary>
        /// The (absolute) color to use when highlighting the target graphic.
        /// </summary>
        [SerializeField]
        private Color m_HighlightColor = Color.black;
        /// <summary>
        /// The (absolute) color to use when highlighting the target graphic.
        /// </summary>
        public Color highlightColor
        {
            get { return m_HighlightColor; }
            set { m_HighlightColor = value; }
        }

        /// <summary>
        /// Should a mask be created when needed and destroyed when not needed, to clip the ripples the the target graphic?
        /// Disable if a mask is used on the graphic regardless, or the ripples don't need to be clipped to improve performance.
        /// </summary>
        [SerializeField]
        private bool m_ToggleMask = true;
        /// <summary>
        /// Should a mask be created when needed and destroyed when not needed, to clip the ripples the the target graphic?
        /// Disable if a mask is used on the graphic regardless, or the ripples don't need to be clipped to improve performance.
        /// </summary>
        public bool toggleMask
        {
            get { return m_ToggleMask; }
            set { m_ToggleMask = value; }
        }

        /// <summary>
        /// Should the ripple/highlight effect not be applied if the pointer drags?
        /// Useful for ripples in scrollable lists.
        /// </summary>
        [SerializeField]
        private bool m_CheckForScroll;
        /// <summary>
        /// Should the ripple/highlight effect not be applied if the pointer drags?
        /// Useful for ripples in scrollable lists.
        /// </summary>
        public bool checkForScroll
        {
            get { return m_CheckForScroll; }
            set { m_CheckForScroll = value; }
        }

        /// <summary>
        /// The number of active ripples.
        /// </summary>
        private int m_RippleCount;
        /// <summary>
        /// The number of active ripples.
        /// </summary>
        public int rippleCount
        {
            get { return m_RippleCount; }
        }

        /// <summary>
        /// The currently used mask used to clip the ripples to the target graphic.
        /// </summary>
        private Mask m_Mask;
        /// <summary>
        /// The currently used mask used to clip the ripples to the target graphic.
        /// If null, automatically gets the attached mask if one exists, and creates one if it doesn't exist.
        /// </summary>
        public Mask mask
        {
            get
            {
                if (m_Mask == null)
                {
                    if (m_ToggleMask && highlightGraphic != null)
                    {
                        m_Mask = highlightGraphic.GetAddComponent<Mask>();
                    }
                }
                return m_Mask;
            }
        }

        /// <summary>
        /// The amount of highlight color to blend into the target graphic color if autoHighlightColor is true.
        /// A lower value means the (absolute) highlight color will more closely resemble the graphic color, a higher value means it will more closely resemble the ripple color.
        /// </summary>
        [SerializeField]
        private float m_AutoHighlightBlendAmount = 0.2f;
        /// <summary>
        /// The amount of highlight color to blend into the target graphic color if autoHighlightColor is true.
        /// A lower value means the (absolute) highlight color will more closely resemble the graphic color, a higher value means it will more closely resemble the ripple color.
        /// Clamps set values between 0 and 1.
        /// </summary>
        public float autoHighlightBlendAmount
        {
            get { return m_AutoHighlightBlendAmount; }
            set
            {
                m_AutoHighlightBlendAmount = Mathf.Clamp(value, 0f, 1f);

            }
        }

        /// <summary>
        /// The current ripple
        /// </summary>
        private Ripple m_CurrentRipple;

        /// <summary>
        /// The target graphic's color when no highlight is applied.
        /// </summary>
        private Color m_NormalColor;
        /// <summary>
        /// The target graphic's current color.
        /// </summary>
        private Color m_CurrentColor;

        /// <summary>
        /// The state of the highlight animation.
        /// </summary>
        private int m_AnimState;
        /// <summary>
        /// The time that the highlight animation last started.
        /// </summary>
        private float m_AnimStartTime;
        /// <summary>
        /// The time since the highlight animation was last updated.
        /// </summary>
        private float m_AnimDeltaTime;
        /// <summary>
        /// The duration of the highlight animation.
        /// </summary>
        private float m_AnimDuration;

        /// <summary>
        /// Is the target graphic normally transparent?
        /// </summary>
        private bool m_ImageIsTransparent;
        /// <summary>
        /// Was the last pointer down -> pointer up event a valid click?
        /// </summary>
        private bool m_Clicked;

        /// <summary>
        /// See Monobehaviour.Start.
        /// </summary>
        private void Start()
        {
            if (!m_RippleData.RippleParent)
            {
                Image[] possibleTargets = GetComponentsInChildren<Image>();

                for (int i = 0; i < possibleTargets.Length; i++)
                {
                    if (!possibleTargets[i].GetComponent<AnimatedShadow>())
                    {
                        m_RippleData.RippleParent = possibleTargets[i].transform;
                        m_HighlightGraphic = null;
                        break;
                    }
                }

                Graphic tempGraphic = GetComponentInChildren<Graphic>();

                if (tempGraphic != null)
                {
                    m_RippleData.RippleParent = tempGraphic.transform;
                }
            }

            if (Application.isPlaying)
            {
                RefreshSettings();
            }
        }

        /// <summary>
        /// See Monobehaviour.Update.
        /// </summary>
        private void Update()
        {
            if (!highlightGraphic)
            {
                m_AnimState = 0;
                return;
            }

            if (m_AnimState == 1)
            {
                m_AnimDeltaTime = Time.realtimeSinceStartup - m_AnimStartTime;

                if (m_AnimDeltaTime <= m_AnimDuration)
                {
                    highlightGraphic.color = Tween.CubeOut(m_CurrentColor, highlightColor, m_AnimDeltaTime, m_AnimDuration);
                    CheckTransparency();
                }
                else
                {
                    highlightGraphic.color = highlightColor;
                    CheckTransparency();

                    m_AnimState = 0;
                }
            }
            else if (m_AnimState == 2)
            {
                m_AnimDeltaTime = Time.realtimeSinceStartup - m_AnimStartTime;

                if (m_AnimDeltaTime <= m_AnimDuration)
                {
                    highlightGraphic.color = Tween.CubeOut(m_CurrentColor, m_NormalColor.a < 0.015f ? m_HighlightColor.WithAlpha(0f) : m_NormalColor, m_AnimDeltaTime, m_AnimDuration);
                    CheckTransparency();
                }
                else
                {
                    highlightGraphic.color = m_NormalColor;
                    CheckTransparency();

                    m_AnimState = 0;
                }
            }
        }

        /// <summary>
        /// Checks if the highlight graphic is transparent (alpha is less than or equal to 0.015).
        /// If there are any active ripples and the graphic is 'supposed to be transparent', the graphic alpha is set to 0.015 so that the masking won't completely hide the ripples and the mask's showMaskGraphic value is set to false.
        /// Otherwise, sets the alpha to the true value, and shows the mask graphic.
        /// </summary>
        private void CheckTransparency()
        {
            if (m_ImageIsTransparent)
            {
                if (highlightGraphic.color.a <= 0.015f)
                {
                    highlightGraphic.color = new Color(highlightGraphic.color.r, highlightGraphic.color.g, highlightGraphic.color.b, m_RippleCount > 0 ? 0.015f : 0f);

                    if (m_Mask != null)
                    {
                        mask.showMaskGraphic = false;
                    }
                }
                else if (m_Mask != null)
                {
                    m_Mask.showMaskGraphic = true;
                }
            }
        }

        /// <summary>
        /// Sets the color of the target graphic. This is the recommended way to change the graphic's color (if any sort of highlighting is used) to reliably set the color without messing up the highlighting values.
        /// </summary>
        /// <param name="color">The color to set.</param>
        /// <param name="animate">Should the graphic fade to the new color? Otherwise the new color is applied instantly.</param>
        public void SetGraphicColor(Color color, bool animate = true)
        {
            if (animate)
            {
                m_NormalColor = color;

                if (m_AutoHighlightColor)
                {
                    m_HighlightColor = MaterialColor.HighlightColor(m_NormalColor, rippleData.Color, m_AutoHighlightBlendAmount);
                }

                m_CurrentColor = highlightGraphic.color;
                m_AnimStartTime = Time.realtimeSinceStartup;

                if (m_AnimState == 0)
                {
                    m_AnimState = 2;
                }
            }
            else
            {
                m_NormalColor = color;

                if (m_AutoHighlightColor)
                {
                    m_HighlightColor = MaterialColor.HighlightColor(m_NormalColor, rippleData.Color, m_AutoHighlightBlendAmount);
                }

                highlightGraphic.color = m_AnimState == 1 ? m_HighlightColor : m_NormalColor;
                m_AnimState = 0;
            }
        }

        /// <summary>
        /// Refreshes the animationDuration, graphic color matching settings, transparency settings and auto highlight color settings.
        /// </summary>
        public void RefreshSettings()
        {
            m_AnimDuration = 4f / rippleData.Speed;

            RefreshGraphicMatchColor();

            RefreshTransparencySettings();

            RefreshAutoHighlightColor();
        }

        /// <summary>
        /// Refreshes the transparency settings.
        /// </summary>
        public void RefreshTransparencySettings()
        {
            if (highlightGraphic)
            {
                m_ImageIsTransparent = (highlightGraphic.color.a == 0f && toggleMask);

                if (m_ToggleMask)
                {
                    if (highlightGraphic.GetComponent<Mask>())
                    {
                        Destroy(highlightGraphic.GetComponent<Mask>());
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the graphic color matching settings.
        /// </summary>
        public void RefreshGraphicMatchColor()
        {
            if (m_AutoMatchGraphicColor)
            {
                if (m_ColorMatchGraphic != null)
                {
                    if (m_AutoHighlightColor)
                    {
                        rippleData.Color = m_ColorMatchGraphic.color;
                    }
                    else
                    {
                        highlightColor = m_ColorMatchGraphic.color;
                    }
                }
            }

            RefreshAutoHighlightColor();
        }

        /// <summary>
        /// Calculates the (absolute) highlight color based on the graphic color and ripple color.
        /// </summary>
        public void RefreshAutoHighlightColor()
        {
            if (!highlightGraphic)
            {
                return;
            }

            m_NormalColor = highlightGraphic.color;

            if (autoHighlightColor)
            {
                if (highlightWhen != HighlightActive.Never)
                {
                    highlightColor = MaterialColor.HighlightColor(m_NormalColor, rippleData.Color, m_AutoHighlightBlendAmount);

                    if (m_ImageIsTransparent)
                    {
                        m_NormalColor = new Color(highlightColor.r, highlightColor.g, highlightColor.b, 0f);
                        highlightGraphic.color = m_NormalColor;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!enabled) return;

            if (highlightWhen == HighlightActive.Hovered)
            {
                Highlight(1);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            m_Clicked = true;

            DestroyRipple();

            if (!enabled) return;

            DestroyRipple();

            if (checkForScroll)
            {
                StartCoroutine(ScrollCheck());
            }
            else
            {
                CreateRipple(eventData.position);

                if (highlightWhen == HighlightActive.Clicked)
                {
                    Highlight(1);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            DestroyRipple();
            m_Clicked = false;

            if (highlightWhen == HighlightActive.Clicked)
            {
                Highlight(2);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            DestroyRipple();
            m_Clicked = false;

            if (highlightWhen != HighlightActive.Never)
            {
                Highlight(2);
            }
        }

        /// <summary>
        /// Begins animating the graphic color (if one exists).
        /// </summary>
        /// <param name="state">1 animates to normal color, 2 animates to highlight color.</param>
        private void Highlight(int state)
        {
            if (!highlightGraphic)
            {
                return;
            }

            m_CurrentColor = highlightGraphic.color.a < 0.015f ? highlightColor.WithAlpha(0f) : highlightGraphic.color;
            m_AnimStartTime = Time.realtimeSinceStartup;
            m_AnimState = state;
        }

        /// <summary>
        /// Creates a new ripple and sets its RippleData, then starts its expand animation.
        /// </summary>
        /// <param name="position">The position to create the Ripple.</param>
        /// <param name="oscillate">Should the Ripple oscillate in size?</param>
        private void CreateRipple(Vector2 position, bool oscillate = false)
        {
            if (!m_RipplesEnabled) return;

            m_CurrentRipple = RippleManager.instance.GetRipple();
            m_CurrentRipple.Setup(rippleData, position, this, oscillate);
            m_CurrentRipple.Expand();
        }

        /// <summary>
        /// Begins the current ripple's contract animation and destroys it upon completion.
        /// </summary>
        private void DestroyRipple()
        {
            if (m_CurrentRipple)
            {
                m_CurrentRipple.Contract();
                m_CurrentRipple = null;
            }
        }

        /// <summary>
        /// Called when a ripple is created and there are no other active ripples.
        /// </summary>
        private void OnFirstRippleCreate()
        {
            if (m_ImageIsTransparent && highlightGraphic.color.a < 0.015f)
            {
                highlightGraphic.color = new Color(highlightGraphic.color.r, highlightGraphic.color.g,
                        highlightGraphic.color.b, 0.015f);
            }
            if (toggleMask)
            {
                mask.enabled = true;
            }
        }

        /// <summary>
        /// Called when a ripple is destroyed and there are no other active ripples.
        /// </summary>
        private void OnLastRippleDestroy()
        {
            if (toggleMask)
            {
                if (m_ImageIsTransparent && highlightGraphic.color.a <= 0.015f)
                {
                    highlightGraphic.color = new Color(highlightGraphic.color.r, highlightGraphic.color.g,
                        highlightGraphic.color.b, 0f);
                }

                Destroy(m_Mask);
            }
        }

        /// <summary>
        /// Called when a ripple is created.
        /// </summary>
        public void OnCreateRipple()
        {
            if (rippleCount == 0)
            {
                OnFirstRippleCreate();
            }

            m_RippleCount++;
        }

        /// <summary>
        /// Called when a ripple is destroyed.
        /// </summary>
        public void OnDestroyRipple()
        {
            m_RippleCount--;

            if (rippleCount == 0)
            {
                OnLastRippleDestroy();
            }
        }

        /// <summary>
        /// Coroutine to check if a click is a valid click, or if it was also dragged.
        /// Applicable only if checkForScroll is true.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ScrollCheck()
        {
            Vector2 startPos = Input.mousePosition;

            yield return new WaitForSeconds(0.04f);

            Vector2 endPos = Input.mousePosition;

            if (Vector2.Distance(startPos, endPos) < 2f)
            {
                CreateRipple(startPos);

                if (highlightWhen == HighlightActive.Clicked)
                {
                    Highlight(1);
                }
            }
        }

        /// <summary>
        /// Checks to see if the graphic was selected and clicked, or just selected.
        /// </summary>
        /// <returns></returns>
        private IEnumerator SelectCheck()
        {
            yield return new WaitForEndOfFrame();
            if (m_Clicked == false)
            {
                CreateRipple(m_RippleData.RippleParent.position, true);
                if (highlightWhen == HighlightActive.Hovered)
                {
                    Highlight(1);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnSelect(BaseEventData eventData)
        {
            StartCoroutine(SelectCheck());
        }

        /// <summary>
        /// Called by the EventSystem when a new object is being selected.
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnDeselect(BaseEventData eventData)
        {
            DestroyRipple();
            if (highlightWhen == HighlightActive.Hovered)
            {
                Highlight(2);
            }
            m_Clicked = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnSubmit(BaseEventData eventData)
        {
            DestroyRipple();
            if (highlightWhen == HighlightActive.Hovered)
            {
                Highlight(2);
            }
            StartCoroutine(SelectCheck());
        }
    }
}