//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Component that controls the visuals of an AppBar and provides easy access in the inspector to some child components' settings.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class MaterialAppBar : MonoBehaviour
    {
        /// <summary>
        /// The elements of a MaterialButton to reference.
        /// </summary>
        public enum ButtonElement
        {
            Text,
            Icon,
            BackgroundImage
        }

        #region Variables

        /// <summary>
        /// The title text on the AppBar.
        /// </summary>
        [Tooltip("The title text on the AppBar.")]
        [SerializeField]
        private Text m_TitleText;
        /// <summary>
        /// The title text on the AppBar.
        /// </summary>
        /// <value>
        /// The title text.
        /// </value>
        public Text titleText
        {
            get { return m_TitleText; }
            set { m_TitleText = value; }
        }

        /// <summary>
        /// The background graphic of the panel.
        /// </summary>
        [Tooltip("The background graphic of the panel.")]
        [SerializeField]
        private Graphic m_PanelGraphic;
        /// <summary>
        /// The background graphic of the panel.
        /// </summary>
        /// <value>
        /// The panel graphic.
        /// </value>
        public Graphic panelGraphic
        {
            get { return m_PanelGraphic; }
            set { m_PanelGraphic = value; }
        }

        /// <summary>
        /// The AnimatedShadow controlling the shadow's state.
        /// </summary>
        [Tooltip("The AnimatedShadow controlling the shadow's state.")]
        [SerializeField]
        private AnimatedShadow m_Shadow;
        /// <summary>
        /// The AnimatedShadow controlling the shadow's state.
        /// </summary>
        /// <value>
        /// The shadow.
        /// </value>
        public AnimatedShadow shadow
        {
            get { return m_Shadow; }
            set { m_Shadow = value; }
        }

        /// <summary>
        /// Any buttons on the AppBar, for color control.
        /// </summary>
        [Tooltip("Any other buttons on the AppBar, for color control.")]
        [SerializeField]
        private MaterialButton[] m_Buttons;
        /// <summary>
        /// Any buttons on the AppBar, for color control.
        /// </summary>
        /// <value>
        /// The buttons.
        /// </value>
        public MaterialButton[] buttons
        {
            get { return m_Buttons; }
            set { m_Buttons = value; }
        }

        /// <summary>
        /// [Required] The duration of animations controlled by this component.
        /// </summary>
        [Tooltip("[Required] The duration of animations controlled by this component.")]
        [SerializeField]
        private float m_AnimationDuration = 0.5f;
        /// <summary>
        /// [Required] The duration of animations controlled by this component.
        /// </summary>
        /// <value>
        /// The duration of the animation.
        /// </value>
        public float animationDuration
        {
            get { return m_AnimationDuration; }
            set { m_AnimationDuration = value; }
        }

        /// <summary>
        /// [Required] The graphic of the shadow.
        /// </summary>
        [Tooltip("[Required] The graphic of the shadow.")]
        private Graphic m_ShadowGraphic;
        /// <summary>
        /// [Required] The graphic of the shadow.
        /// </summary>
        /// <value>
        /// The shadow graphic.
        /// </value>
        public Graphic shadowGraphic
        {
            get
            {
                if (m_ShadowGraphic == null)
                {
                    m_ShadowGraphic = m_Shadow.GetComponent<Graphic>();
                }
                return m_ShadowGraphic;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the color of the AppBar and hides the shadow if the new alpha is less than 1. Does not animate in edit mode.
        /// </summary>
        /// <param name="color">The new color to set the AppBar's graphic. The shadow will be shown if the color is fully opaque.</param>
        /// <param name="animate">Animate the transition? Always false if in edit mode.</param>
        public void SetPanelColor(Color color, bool animate = true)
        {
            bool transparent = color.a < 1f;

            m_Shadow.SetShadow(!transparent, !transparent ? Tween.TweenType.SoftEaseOutQuint : Tween.TweenType.EaseOutQuint, !animate);

            if (m_PanelGraphic.color.a < 0.015f)
            {
                m_PanelGraphic.color = color.WithAlpha(m_PanelGraphic.color.a);
            }

            if (animate && Application.isPlaying)
            {
                //  The action that is called each 'frame' of the tween
                Action<Color> tweenAction = c =>
                {
                    m_PanelGraphic.color = c;
                    shadowGraphic.color = m_PanelGraphic.color.WithAlpha(shadowGraphic.color.a);
                };

                TweenManager.TweenValue(tweenAction, m_PanelGraphic.color, color, m_AnimationDuration);
            }
            else
            {
                m_PanelGraphic.color = color;
                shadowGraphic.color = m_PanelGraphic.color.WithAlpha(shadowGraphic.color.a);
            }
        }

        /// <summary>
        /// Sets the color of the TitleText, if one exists.
        /// </summary>
        /// <param name="color">The color to set the TitleText.</param>
        /// <param name="animate">Animate the transition? Always false if in edit mode.</param>
        public void SetTitleTextColor(Color color, bool animate = true)
        {
            if (m_TitleText != null)
            {
                TweenGraphicColor(m_TitleText, color, animate);
            }
        }

        /// <summary>
        /// Sets an element's color on all referenced buttons.
        /// </summary>
        /// <param name="buttonIndex">The index of the button array to modify.</param>
        /// <param name="color">The color to set the button element.</param>
        /// <param name="element">The element of the buttons to modify.</param>
        /// <param name="animate">Animate the transition? Always false if in edit mode.</param>
        public void SetButtonGraphicColor(int buttonIndex, Color color, ButtonElement element, bool animate = true)
        {
            MaterialButton button = m_Buttons[buttonIndex];
            if (button != null)
            {
                Graphic graphic = element == ButtonElement.Text ? button.text : (element == ButtonElement.Icon ? button.icon : button.backgroundImage);

                if (graphic == null) return;

                TweenGraphicColor(graphic, color, animate, () => button.RefreshRippleMatchColor());
            }
        }

        /// <summary>
        /// Sets an element's color on all referenced buttons.
        /// </summary>
        /// <param name="color">The color to set the button element.</param>
        /// <param name="element">The element of the buttons to modify.</param>
        /// <param name="animate">Animate the transition? Always false if in edit mode.</param>
        public void SetButtonsGraphicColors(Color color, ButtonElement element, bool animate = true)
        {
            for (int i = 0; i < m_Buttons.Length; i++)
            {
                SetButtonGraphicColor(i, color, element, animate);
            }
        }

        /// <summary>
        /// Tweens the color of a Graphic.
        /// </summary>
        /// <param name="graphic">The Graphic to modify.</param>
        /// <param name="color">The target color to tween the Graphic.</param>
        /// <param name="animate">Animate the transition? Always false if in edit mode.</param>
        /// <param name="onTweenEndAction">The action to call upon tween completion.</param>
        private void TweenGraphicColor(Graphic graphic, Color color, bool animate, Action onTweenEndAction = null)
        {
            if (animate && Application.isPlaying)
            {
                TweenManager.TweenValue(c => graphic.color = c, graphic.color, color, m_AnimationDuration, 0f, onTweenEndAction);
            }
            else
            {
                graphic.color = color;
                onTweenEndAction.InvokeIfNotNull();
            }
        }

        #endregion
    }
}