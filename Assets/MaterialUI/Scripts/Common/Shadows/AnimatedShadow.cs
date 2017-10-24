//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Tweens a CanvasGroup's alpha to show or hide a shadow.
    /// Mutiple of these are used with <see cref="MaterialShadow"/> to create dynamic shadows.
    /// </summary>
    /// <seealso cref="MaterialUI.MaterialShadow" />
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [AddComponentMenu("MaterialUI/Shadow Anim", 100)]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Image))]
    public class AnimatedShadow : MonoBehaviour
    {
        /// <summary>
        /// Is this shadow visible?
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private bool m_IsVisible;
        /// <summary>
        /// Is this shadow visible?
        /// </summary>
        public bool isVisible
        {
            get { return m_IsVisible; }
            set { m_IsVisible = value; }
        }

        /// <summary>
        /// The CanvasGroup of the shadow.
        /// </summary>
        private CanvasGroup m_CanvasGroup;
        /// <summary>
        /// The CanvasGroup of the shadow.
        /// If null, automatically gets the attached CanvasGroup, if one exists.
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
        /// The id of the AutoTweener used to animate the shadow.
        /// </summary>
        private int m_Tweener;
        
        public void SetShadow(bool set, bool instant = false)
        {
            SetShadow(set, Tween.TweenType.EaseOutQuint, instant);
        }

        /// <summary>
        /// Starts animating a shadow on or off.
        /// </summary>
        /// <param name="set">If true, animate the shadow on. Otherwise animate it off.</param>
        /// <param name="tweenType">The type of tween curve to use. 'Custom' is not supported.</param>
        /// <param name="instant">Should the transition be instant and not animate?</param>
        public void SetShadow(bool set, Tween.TweenType tweenType, bool instant = false)
        {
            isVisible = set;
            
            if (set)
            {
                gameObject.SetActive(true);
            }

            if (Application.isPlaying && !instant)
            {
                TweenManager.EndTween(m_Tweener);

                m_Tweener = TweenManager.TweenFloat(f => canvasGroup.alpha = f, () => canvasGroup.alpha, set ? 1f : 0f, 0.5f, 0f, () => gameObject.SetActive(set), tweenType: tweenType);
            }
            else
            {
                canvasGroup.alpha = set ? 1f : 0f;
                gameObject.SetActive(set);
            }
        }
    }
}
