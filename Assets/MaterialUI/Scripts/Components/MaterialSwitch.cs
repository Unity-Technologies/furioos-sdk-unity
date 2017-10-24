//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Component that handles a toggle switch control.
    /// </summary>
    /// <seealso cref="MaterialUI.ToggleBase" />
    [ExecuteInEditMode]
    [AddComponentMenu("MaterialUI/Material Switch", 100)]
    public class MaterialSwitch : ToggleBase
    {
        /// <summary>
        /// The color of the back graphic when the toggle is on.
        /// Overridden by <see cref="backDisabledColor"/>.
        /// </summary>
        [SerializeField]
        private Color m_BackOnColor = Color.black;
        /// <summary>
        /// The color of the back graphic when the toggle is on.
        /// Overridden by <see cref="backDisabledColor"/>.
        /// </summary>
        public Color backOnColor
        {
            get { return m_BackOnColor; }
            set { m_BackOnColor = value; }
        }

        /// <summary>
        /// The color of the back graphic when the toggle is off.
        /// Overridden by <see cref="backDisabledColor"/>.
        /// </summary>
        [SerializeField]
        private Color m_BackOffColor = Color.black;
        /// <summary>
        /// The color of the back graphic when the toggle is off.
        /// Overridden by <see cref="backDisabledColor"/>.
        /// </summary>
        public Color backOffColor
        {
            get { return m_BackOffColor; }
            set { m_BackOffColor = value; }
        }

        /// <summary>
        /// The color of the back graphic when the toggle is inactive.
        /// Overrides <see cref="backOnColor"/> and <see cref="backOffColor"/>.
        /// </summary>
        [SerializeField]
        private Color m_BackDisabledColor = Color.black;
        /// <summary>
        /// The color of the back graphic when the toggle is inactive.
        /// Overrides <see cref="backOnColor"/> and <see cref="backOffColor"/>.
        /// </summary>
        public Color backDisabledColor
        {
            get { return m_BackDisabledColor; }
            set { m_BackDisabledColor = value; }
        }

        /// <summary>
        /// The switch Graphic.
        /// </summary>
        [SerializeField]
        private Graphic m_SwitchImage;
        /// <summary>
        /// The switch Graphic.
        /// </summary>
        public Graphic switchImage
        {
            get { return m_SwitchImage; }
            set { m_SwitchImage = value; }
        }

        /// <summary>
        /// The back Graphic.
        /// </summary>
        [SerializeField]
        private Graphic m_BackImage;
        /// <summary>
        /// The back Graphic.
        /// </summary>
        public Graphic backImage
        {
            get { return m_BackImage; }
            set { m_BackImage = value; }
        }

        /// <summary>
        /// The shadow Graphic.
        /// </summary>
        [SerializeField]
        private Graphic m_ShadowImage;
        /// <summary>
        /// The shadow Graphic.
        /// </summary>
        public Graphic shadowImage
        {
            get { return m_ShadowImage; }
            set { m_ShadowImage = value; }
        }

        /// <summary>
        /// The switch RectTransform.
        /// </summary>
        private RectTransform m_SwitchRectTransform;
        /// <summary>
        /// The switch RectTransform.
        /// If null, gets the transform of the switch image if it exists.
        /// </summary>
        public RectTransform switchRectTransform
        {
            get
            {
                if (m_SwitchRectTransform == null)
                {
                    if (m_SwitchImage != null)
                    {
                        m_SwitchRectTransform = (RectTransform)m_SwitchImage.transform;
                    }
                }
                return m_SwitchRectTransform;
            }
        }

        /// <summary>
        /// The current switch position.
        /// </summary>
        private float m_CurrentSwitchPosition;
        /// <summary>
        /// The current back color.
        /// </summary>
        private Color m_CurrentBackColor;

        /// <summary>
        /// Sets and animates the toggle to 'on'.
        /// Updates the toggle, ripple and graphic color and graphic data if applicable.
        /// </summary>
        public override void TurnOn()
        {
            m_CurrentSwitchPosition = switchRectTransform.anchoredPosition.x;
            m_CurrentColor = switchImage.color;
            m_CurrentBackColor = backImage.color;

            base.TurnOn();
        }

        /// <summary>
        /// Sets the toggle to 'on' without an animation.
        /// Updates the toggle, ripple and graphic color and graphic data if applicable.
        /// </summary>
        public override void TurnOnInstant()
        {
            base.TurnOnInstant();

            if (toggle.interactable)
            {
                switchImage.color = m_OnColor;
                backImage.color = backOnColor;
            }

            switchRectTransform.anchoredPosition = new Vector2(8f, 0f);
        }

        /// <summary>
        /// Sets and animates the toggle to 'off'.
        /// Updates the toggle, ripple and graphic color and graphic data if applicable.
        /// </summary>
        public override void TurnOff()
        {
            m_CurrentSwitchPosition = switchRectTransform.anchoredPosition.x;
            m_CurrentColor = switchImage.color;
            m_CurrentBackColor = backImage.color;

            base.TurnOff();
        }

        /// <summary>
        /// Sets the toggle to 'off' without an animation.
        /// Updates the toggle, ripple and graphic color and graphic data if applicable.
        /// </summary>
        public override void TurnOffInstant()
        {
            base.TurnOffInstant();

            if (toggle.interactable)
            {
                switchImage.color = m_OffColor;
                backImage.color = backOffColor;
            }

            switchRectTransform.anchoredPosition = new Vector2(-8f, 0f);
        }

        /// <summary>
        /// Makes the toggle object interactable, independant of the Toggle component's interactable value.
        /// Not to be confused with MonoBehaviour.OnEnable.
        /// </summary>
        public override void Enable()
        {
            if (toggle.isOn)
            {
                switchImage.color = m_OnColor;
                backImage.color = backOnColor;
            }
            else
            {
                switchImage.color = m_OffColor;
                backImage.color = backOffColor;
            }

            shadowImage.enabled = true;

            base.Enable();
        }

        /// <summary>
        /// Makes the toggle object not interactable, independant of the Toggle component's interactable value.
        /// Not to be confused with MonoBehaviour.OnDisable.
        /// </summary>
        public override void Disable()
        {
            switchImage.color = m_DisabledColor;
            backImage.color = backDisabledColor;

            shadowImage.enabled = false;

            base.Disable();
        }

        /// <summary>
        /// Begins animating the toggle to the 'on' visual state.
        /// </summary>
        public override void AnimOn()
        {
            base.AnimOn();

            switchImage.color = Tween.QuintOut(m_CurrentColor, m_OnColor, m_AnimDeltaTime, m_AnimationDuration);
            backImage.color = Tween.QuintOut(m_CurrentBackColor, backOnColor, m_AnimDeltaTime, m_AnimationDuration);

            switchRectTransform.anchoredPosition = Tween.SeptSoftOut(new Vector2(m_CurrentSwitchPosition, 0f), new Vector2(8f, 0f), m_AnimDeltaTime, m_AnimationDuration);
        }

        /// <summary>
        /// Called when the toggle finishes animating to the 'on' visual state.
        /// </summary>
        public override void AnimOnComplete()
        {
            base.AnimOnComplete();

            switchImage.color = m_OnColor;
            backImage.color = backOnColor;

            switchRectTransform.anchoredPosition = new Vector2(8f, 0f);
        }

        /// <summary>
        /// Begins animating the toggle to the 'off' visual state.
        /// </summary>
        public override void AnimOff()
        {
            base.AnimOff();

            switchImage.color = Tween.QuintOut(m_CurrentColor, m_OffColor, m_AnimDeltaTime, m_AnimationDuration);
            backImage.color = Tween.QuintOut(m_CurrentBackColor, backOffColor, m_AnimDeltaTime, m_AnimationDuration);

            switchRectTransform.anchoredPosition = Tween.SeptSoftOut(new Vector2(m_CurrentSwitchPosition, 0f), new Vector2(-8f, 0f), m_AnimDeltaTime, m_AnimationDuration);
        }

        /// <summary>
        /// Called when the toggle finishes animating to the 'off' visual state.
        /// </summary>
        public override void AnimOffComplete()
        {
            base.AnimOffComplete();

            switchImage.color = m_OffColor;
            backImage.color = backOffColor;

            switchRectTransform.anchoredPosition = new Vector2(-8f, 0f);
        }

#if UNITY_EDITOR
        /// <summary>
        /// See MonoBehaviour.OnValidate.
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_Interactable)
            {
                m_SwitchImage.color = toggle.isOn ? m_OnColor : m_OffColor;
                m_BackImage.color = toggle.isOn ? m_BackOnColor : m_BackOffColor;
            }
            else
            {
                m_SwitchImage.color = m_DisabledColor;
                m_BackImage.color = m_BackDisabledColor;
            }
        }
#endif
    }
}
