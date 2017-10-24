//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// The base component class for all Toggles (Checkbox/RadioButton/Switch).
    /// </summary>
    /// <seealso cref="UnityEngine.EventSystems.UIBehaviour" />
    public class ToggleBase : UIBehaviour
    {
        /// <summary>
        /// The duration of the toggle (object) animation.
        /// </summary>
        [SerializeField]
        protected float m_AnimationDuration = 0.5f;
        /// <summary>
        /// The duration of the toggle (object) animation.
        /// </summary>
        public float animationDuration
        {
            get { return m_AnimationDuration; }
            set { m_AnimationDuration = value; }
        }

        /// <summary>
        /// The color of the toggle (object) when the toggle is toggled on.
        /// Prioritized under disabledColor.
        /// </summary>
        [SerializeField]
        protected Color m_OnColor = Color.black;
        /// <summary>
        /// The color of the toggle (object) when toggled on.
        /// Prioritized under disabledColor.
        /// </summary>
        public Color onColor
        {
            get { return m_OnColor; }
            set { m_OnColor = value; }
        }

        /// <summary>
        /// The color of the toggle (object) when the toggle is toggled off.
        /// Prioritized under disabledColor.
        /// </summary>
        [SerializeField]
        protected Color m_OffColor = Color.black;
        /// <summary>
        /// The color of the toggle (object) when the toggle is toggled off.
        /// Prioritized under disabledColor.
        /// </summary>
        public Color offColor
        {
            get { return m_OffColor; }
            set { m_OffColor = value; }
        }

        /// <summary>
        /// The color of the toggle (object) when disabled.
        /// Prioritized over onColor and offColor.
        /// </summary>
        [SerializeField]
        protected Color m_DisabledColor = Color.black;
        /// <summary>
        /// The color of the toggle (object) when disabled.
        /// Prioritized over onColor and offColor.
        /// </summary>
        public Color disabledColor
        {
            get { return m_DisabledColor; }
            set { m_DisabledColor = value; }
        }

        /// <summary>
        /// Whether the graphic (eg. Label/Icon) should change color depending on the toggle state.
        /// </summary>
        [SerializeField]
        protected bool m_ChangeGraphicColor = true;
        /// <summary>
        /// Whether the graphic (eg. Label/Icon) should change color depending on the toggle state.
        /// </summary>
        public bool changeGraphicColor
        {
            get { return m_ChangeGraphicColor; }
            set { m_ChangeGraphicColor = value; }
        }

        /// <summary>
        /// The color of the graphic when the toggle is toggled on.
        /// Prioritized under graphicDisabledColor.
        /// </summary>
        [SerializeField]
        protected Color m_GraphicOnColor = Color.black;
        /// <summary>
        /// The color of the graphic when the toggle is toggled on.
        /// Prioritized under graphicDisabledColor.
        /// </summary>
        public Color graphicOnColor
        {
            get { return m_GraphicOnColor; }
            set { m_GraphicOnColor = value; }
        }

        /// <summary>
        /// The color of the graphic when the toggle is toggled off.
        /// Prioritized under graphicDisabledColor.
        /// </summary>
        [SerializeField]
        protected Color m_GraphicOffColor = Color.black;
        /// <summary>
        /// The color of the graphic when the toggle is toggled off.
        /// Prioritized under graphicDisabledColor.
        /// </summary>
        public Color graphicOffColor
        {
            get { return m_GraphicOffColor; }
            set { m_GraphicOffColor = value; }
        }

        /// <summary>
        /// The color of the graphic when the toggle is disabled.
        /// Prioritized over graphicOnColor and graphicOffColor.
        /// </summary>
        [SerializeField]
        protected Color m_GraphicDisabledColor = Color.black;
        /// <summary>
        /// The color of the graphic when the toggle is disabled.
        /// Prioritized over graphicOnColor and graphicOffColor.
        /// </summary>
        public Color graphicDisabledColor
        {
            get { return m_GraphicDisabledColor; }
            set { m_GraphicDisabledColor = value; }
        }

        /// <summary>
        /// Whether the ripple should change color depending on the toggle state.
        /// </summary>
        [SerializeField]
        protected bool m_ChangeRippleColor = true;
        /// <summary>
        /// Whether the ripple should change color depending on the toggle state.
        /// </summary>
        public bool changeRippleColor
        {
            get { return m_ChangeRippleColor; }
            set { m_ChangeRippleColor = value; }
        }

        /// <summary>
        /// The color of the ripple when the toggle is toggled on.
        /// </summary>
        [SerializeField]
        protected Color m_RippleOnColor = Color.black;
        /// <summary>
        /// The color of the ripple when the toggle is toggled on.
        /// </summary>
        public Color rippleOnColor
        {
            get { return m_RippleOnColor; }
            set { m_RippleOnColor = value; }
        }

        /// <summary>
        /// The color of the ripple when the toggle is toggled off.
        /// </summary>
        [SerializeField]
        protected Color m_RippleOffColor = Color.black;
        /// <summary>
        /// The color of the ripple when the toggle is toggled off.
        /// </summary>
        public Color rippleOffColor
        {
            get { return m_RippleOffColor; }
            set { m_RippleOffColor = value; }
        }

        /// <summary>
        /// The external graphic (eg. Label/Icon).
        /// </summary>
        [SerializeField]
        protected internal Graphic m_Graphic;
        /// <summary>
        /// The external graphic (eg. Label/Icon).
        /// </summary>
        public Graphic graphic
        {
            get { return m_Graphic; }
            set { m_Graphic = value; }
        }

        /// <summary>
        /// Whether the graphic (eg. Label/Icon) should change depending on the toggle state.
        /// </summary>
        [SerializeField]
        protected bool m_ToggleGraphic;
        /// <summary>
        /// Whether the graphic (eg. Label/Icon) should change depending on the toggle state.
        /// </summary>
        public bool toggleGraphic
        {
            get { return m_ToggleGraphic; }
            set { m_ToggleGraphic = value; }
        }

        /// <summary>
        /// The label text when the toggle is toggled on.
        /// </summary>
        [SerializeField]
        protected string m_ToggleOnLabel;
        /// <summary>
        /// The label text when the toggle is toggled on.
        /// </summary>
        public string toggleOnLabel
        {
            get { return m_ToggleOnLabel; }
            set { m_ToggleOnLabel = value; }
        }

        /// <summary>
        /// The label text when the toggle is toggled off.
        /// </summary>
        [SerializeField]
        protected string m_ToggleOffLabel;
        /// <summary>
        /// The label text when the toggle is toggled off.
        /// </summary>
        public string toggleOffLabel
        {
            get { return m_ToggleOffLabel; }
            set { m_ToggleOffLabel = value; }
        }

        /// <summary>
        /// The icon ImageData when the toggle is toggled on.
        /// </summary>
        [SerializeField]
        protected ImageData m_ToggleOnIcon;
        /// <summary>
        /// The icon ImageData when the toggle is toggled on.
        /// </summary>
        public ImageData toggleOnIcon
        {
            get { return m_ToggleOnIcon; }
            set { m_ToggleOnIcon = value; }
        }

        /// <summary>
        /// The icon ImageData when the toggle is toggled off.
        /// </summary>
        [SerializeField]
        protected ImageData m_ToggleOffIcon;
        /// <summary>
        /// The icon ImageData when the toggle is toggled off.
        /// </summary>
        public ImageData toggleOffIcon
        {
            get { return m_ToggleOffIcon; }
            set { m_ToggleOffIcon = value; }
        }

        /// <summary>
        /// The MaterialRipple component.
        /// </summary>
        protected MaterialRipple m_MaterialRipple;
        /// <summary>
        /// The MaterialRipple component.
        /// </summary>
        public MaterialRipple materialRipple
        {
            get { return m_MaterialRipple; }
            set { m_MaterialRipple = value; }
        }

        /// <summary>
        /// The Toggle component.
        /// </summary>
        protected Toggle m_Toggle;
        /// <summary>
        /// The Toggle component.
        /// Automatically gets the attached Toggle if one exists.
        /// </summary>
        public Toggle toggle
        {
            get
            {
                if (!m_Toggle)
                {
                    m_Toggle = gameObject.GetComponent<Toggle>();
                }
                return m_Toggle;
            }
            set { m_Toggle = value; }
        }

        /// <summary>
        /// Gets or sets the label text.
        /// Also returns null if there is no label or the label is not a text.
        /// </summary>
        public string labelText
        {
            get
            {
                if (m_Graphic == null) return null;

                Text text = m_Graphic as Text;
                if (text != null)
                {
                    return text.text;
                }

                return null;
            }

            set
            {
                if (m_Graphic == null) return;

                Text text = m_Graphic as Text;
                if (text != null)
                {
                    text.text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the icon ImageData.
        /// Also returns null if there is no icon or the icon is not an Image or VectorImage.
        /// </summary>
        public ImageData icon
        {
            get
            {
                if (m_Graphic == null) return null;

                return m_Graphic.GetImageData();
            }

            set
            {
                if (m_Graphic == null) return;

                m_Graphic.SetImage(value);
            }
        }

        /// <summary>
        /// The CanvasGroup of the Toggle.
        /// </summary>
        protected CanvasGroup m_CanvasGroup;
        /// <summary>
        /// The CanvasGroup of the Toggle.
        /// Automatically gets the attached CanvasGroup and adds one if one doesn't exist.
        /// </summary>
        private CanvasGroup canvasGroup
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
        /// Mirrors the attached <see cref="Toggle"/> interactable value.
        /// If false, the toggle will be disabled and disabled properties (eg. colors) will be applied.
        /// </summary>
        [SerializeField]
        protected bool m_Interactable = true;
        /// <summary>
        /// Mirrors the attached <see cref="Toggle"/> interactable value.
        /// If false, the toggle will be disabled and disabled properties (eg. colors) will be applied.
        /// </summary>
        public bool interactable
        {
            get { return m_Interactable; }
            set
            {
                m_Interactable = value;
                toggle.interactable = value;

                if (value)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }

                UpdateGraphicToggleState();
            }
        }

        /// <summary>
        /// The icon's ImageData.
        /// </summary>
        [SerializeField]
        protected ImageData m_Icon;

        /// <summary>
        /// The label's text.
        /// </summary>
        [SerializeField]
        protected string m_Label;

        /// <summary>
        /// The icon's last VectorImageData.
        /// Only applicable if the icon is a VectorImage.
        /// </summary>
        protected VectorImageData m_LastIconVectorImageData;
        /// <summary>
        /// The icon's last sprite.
        /// Only applicable if the icon is an Image.
        /// </summary>
        protected Sprite m_LastIconSprite;

        /// <summary>
        /// The label's last string.
        /// Only applicable if the label is a Text.
        /// </summary>
        protected string m_LastLabelText;

#if UNITY_EDITOR
        /// <summary>
        /// The last toggle state.
        /// </summary>
        [SerializeField]
        protected bool m_LastToggleState;
#endif

        /// <summary>
        /// The current color of the toggle.
        /// </summary>
        protected Color m_CurrentColor;
        /// <summary>
        /// The current color of the graphic (eg. Label/Icon).
        /// </summary>
        protected Color m_CurrentGraphicColor;

        /// <summary>
        /// The state of the toggle's animation.
        /// </summary>
        protected int m_AnimState;

        /// <summary>
        /// The time that the toggle's last animation started.
        /// </summary>
        protected float m_AnimStartTime;

        /// <summary>
        /// The time since the last frame of the toggle's animation.
        /// </summary>
        protected float m_AnimDeltaTime;

        /// <summary>
        /// See MonoBehaviour.OnEnable.
        /// </summary>
        protected override void OnEnable()
        {
            m_Toggle = gameObject.GetComponent<Toggle>();
            materialRipple = gameObject.GetComponent<MaterialRipple>();
        }

        /// <summary>
        /// See MonoBehaviour.Start.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            UpdateGraphicToggleState();
        }

        /// <summary>
        /// Toggles the specified state.
        /// If toggled on, calls <see cref="TurnOff"/>, otherwise calls <see cref="TurnOn"/>.
        /// </summary>
        public void Toggle()
        {
            if (m_Toggle.isOn)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }

        /// <summary>
        /// Updates the text/icon of the graphic (Label/Icon).
        /// </summary>
        protected void UpdateGraphicToggleState()
        {
            UpdateIconDataType();

            if (m_Graphic == null || m_Toggle == null || !m_ToggleGraphic) return;

            Type graphicType = m_Graphic.GetType();

            if (graphicType == typeof(Image) || graphicType == typeof(VectorImage))
            {
                m_Graphic.SetImage(m_Toggle.isOn ? m_ToggleOnIcon : m_ToggleOffIcon);
            }
            else if (graphicType == typeof(Text))
            {
                ((Text)m_Graphic).text = m_Toggle.isOn ? m_ToggleOnLabel : m_ToggleOffLabel;
            }
        }

        /// <summary>
        /// Updates the imageDataType of the ImageData values used, depending on whether the graphic is an <see cref="Image"/> or <see cref="VectorImage"/>.
        /// </summary>
        protected void UpdateIconDataType()
        {
            if (m_Graphic == null) return;

            Type graphicType = m_Graphic.GetType();

            if (graphicType == typeof(Image))
            {
                m_ToggleOnIcon.imageDataType = ImageDataType.Sprite;
                m_ToggleOffIcon.imageDataType = ImageDataType.Sprite;
                m_Icon.imageDataType = ImageDataType.Sprite;
            }
            else if (graphicType == typeof(VectorImage))
            {
                m_ToggleOnIcon.imageDataType = ImageDataType.VectorImage;
                m_ToggleOffIcon.imageDataType = ImageDataType.VectorImage;
                m_Icon.imageDataType = ImageDataType.VectorImage;
            }
        }

        /// <summary>
        /// Sets and animates the toggle to 'on'.
        /// Updates the toggle, ripple and graphic color and graphic data if applicable.
        /// </summary>
        public virtual void TurnOn()
        {
            if (m_Graphic)
            {
                m_CurrentGraphicColor = m_Graphic.color;
            }

            AnimOn();
            m_AnimStartTime = Time.realtimeSinceStartup;
            m_AnimState = 1;

            UpdateGraphicToggleState();
        }

        /// <summary>
        /// Sets the toggle to 'on' without an animation.
        /// Updates the toggle, ripple and graphic color and graphic data if applicable.
        /// </summary>
        public virtual void TurnOnInstant()
        {
            if (m_Interactable)
            {
                SetOnColor();
            }

            UpdateGraphicToggleState();
        }

        /// <summary>
        /// Sets and animates the toggle to 'off'.
        /// Updates the toggle, ripple and graphic color and graphic data if applicable.
        /// </summary>
        public virtual void TurnOff()
        {
            if (m_Graphic)
            {
                m_CurrentGraphicColor = m_Graphic.color;
            }

            AnimOff();
            m_AnimStartTime = Time.realtimeSinceStartup;
            m_AnimState = 2;

            UpdateGraphicToggleState();
        }

        /// <summary>
        /// Sets the toggle to 'off' without an animation.
        /// Updates the toggle, ripple and graphic color and graphic data if applicable.
        /// </summary>
        public virtual void TurnOffInstant()
        {
            if (m_Interactable)
            {
                SetOffColor();
            }

            UpdateGraphicToggleState();
        }

        /// <summary>
        /// Makes the toggle object interactable, independant of the Toggle component's interactable value.
        /// Not to be confused with MonoBehaviour.OnEnable.
        /// </summary>
        public virtual void Enable()
        {
            if (m_Toggle.isOn)
            {
                SetOnColor();
            }
            else
            {
                SetOffColor();
            }

            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        /// <summary>
        /// Makes the toggle object not interactable, independant of the Toggle component's interactable value.
        /// Not to be confused with MonoBehaviour.OnDisable.
        /// </summary>
        public virtual void Disable()
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

#if UNITY_EDITOR
            OnValidate();
#endif
        }

        /// <summary>
        /// See Monobehaviour.Update.
        /// </summary>
        void Update()
        {
            m_AnimDeltaTime = Time.realtimeSinceStartup - m_AnimStartTime;

            if (m_AnimState == 1)
            {
                if (m_AnimDeltaTime <= animationDuration)
                {
                    AnimOn();
                }
                else
                {
                    AnimOnComplete();
                }
            }
            else if (m_AnimState == 2)
            {
                if (m_AnimDeltaTime <= animationDuration)
                {
                    AnimOff();
                }
                else
                {
                    AnimOffComplete();
                }
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (m_LastToggleState != m_Toggle.isOn)
                {
                    m_LastToggleState = m_Toggle.isOn;

                    if (m_LastToggleState)
                    {
                        TurnOnInstant();
                    }
                    else
                    {
                        TurnOffInstant();
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Begins animating the toggle to the 'on' visual state.
        /// </summary>
        public virtual void AnimOn()
        {
            if (m_Graphic && changeGraphicColor)
            {
                m_Graphic.color = Tween.QuintSoftOut(m_CurrentGraphicColor, m_Interactable ? m_GraphicOnColor : m_GraphicDisabledColor, m_AnimDeltaTime, animationDuration);
            }
            if (m_ChangeRippleColor && m_MaterialRipple != null)
            {
                materialRipple.rippleData.Color = m_RippleOnColor;
            }
        }

        /// <summary>
        /// Called when the toggle finishes animating to the 'on' visual state.
        /// </summary>
        public virtual void AnimOnComplete()
        {
            SetOnColor();

            m_AnimState = 0;
        }

        /// <summary>
        /// Begins animating the toggle to the 'off' visual state.
        /// </summary>
        public virtual void AnimOff()
        {
            if (m_Graphic && m_ChangeGraphicColor)
            {
                m_Graphic.color = Tween.QuintSoftOut(m_CurrentGraphicColor, m_Interactable ? m_GraphicOffColor : m_GraphicDisabledColor, m_AnimDeltaTime, animationDuration * 0.75f);
            }
            if (m_ChangeRippleColor && m_MaterialRipple != null)
            {
                materialRipple.rippleData.Color = m_RippleOffColor;
            }
        }

        /// <summary>
        /// Called when the toggle finishes animating to the 'off' visual state.
        /// </summary>
        public virtual void AnimOffComplete()
        {
            SetOffColor();

            m_AnimState = 0;
        }

        /// <summary>
        /// Sets the toggle, graphic (Label/Icon) and ripple colors to the 'on' colors.
        /// If the toggle's interactable value is false, disabled colors will be used regardless of off/on colors.
        /// </summary>
        private void SetOnColor()
        {
            if (m_Graphic && m_ChangeGraphicColor)
            {
                m_Graphic.color = m_Interactable ? m_GraphicOnColor : m_GraphicDisabledColor;
            }
            if (materialRipple && m_ChangeRippleColor)
            {
                materialRipple.rippleData.Color = m_RippleOnColor;
            }
        }

        /// <summary>
        /// Sets the toggle, graphic (Label/Icon) and ripple colors to the 'off' colors.
        /// If the toggle's interactable value is false, disabled colors will be used regardless of off/on colors.
        /// </summary>
        private void SetOffColor()
        {
            if (m_Graphic && m_ChangeGraphicColor)
            {
                m_Graphic.color = m_Interactable ? m_GraphicOffColor : m_GraphicDisabledColor;
            }
            if (materialRipple && m_ChangeRippleColor)
            {
                materialRipple.rippleData.Color = m_RippleOffColor;
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Called by <see cref="EditorUpdate"/> when in edit mode.
        /// </summary>
        public void EditorValidate()
        {
            OnValidate();
        }

        /// <summary>
        /// See MonoBehaviour.OnValidate.
        /// </summary>
        protected override void OnValidate()
        {
            if (m_Graphic && m_ChangeGraphicColor)
            {
                if (m_Interactable)
                {
                    m_Graphic.color = toggle.isOn ? m_GraphicOnColor : m_GraphicOffColor;
                }
                else
                {
                    m_Graphic.color = m_GraphicDisabledColor;
                }
            }
            if (materialRipple && m_ChangeRippleColor)
            {
                materialRipple.rippleData.Color = toggle.isOn ? m_RippleOnColor : m_RippleOffColor;
            }

            UpdateGraphicToggleState();

            if (m_Graphic != null)
            {
                if (m_Graphic.IsSpriteOrVectorImage())
                {
                    if (m_Graphic.GetType() == typeof(Image))
                    {
                        if (m_LastIconSprite == m_Icon.sprite)
                        {
                            m_Icon.sprite = m_Graphic.GetSpriteImage();
                        }
                        else
                        {
                            m_Graphic.SetImage(m_Icon.sprite);
                        }

                        m_LastIconSprite = m_Icon.sprite;
                    }
                    else
                    {
                        if (m_LastIconVectorImageData == m_Icon.vectorImageData)
                        {
                            m_Icon.vectorImageData = m_Graphic.GetVectorImage();
                        }
                        else
                        {
                            m_Graphic.SetImage(m_Icon.vectorImageData);
                        }

                        m_LastIconVectorImageData = m_Icon.vectorImageData;
                    }
                }
                else
                {
                    Text text = m_Graphic as Text;
                    if (text != null)
                    {
                        if (m_LastLabelText == m_Label)
                        {
                            m_Label = text.text;
                        }
                        else
                        {
                            text.text = m_Label;
                        }

                        m_LastLabelText = m_Label;
                    }
                }
            }

        }
#endif
    }
}