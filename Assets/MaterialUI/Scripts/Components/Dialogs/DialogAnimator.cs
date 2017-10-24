//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    public abstract class DialogAnimator
    {
        protected MaterialDialog m_Dialog;
        public MaterialDialog dialog
        {
            get { return m_Dialog; }
            set { m_Dialog = value; }
        }

        protected float m_AnimationDuration;
        public float animationDuration
        {
            get { return m_AnimationDuration; }
        }

        private CanvasGroup m_Background;
        public CanvasGroup background
        {
            get
            {
                if (m_Background == null)
                {
                    m_Background = PrefabManager.InstantiateGameObject("Dialogs/Dialog Background", m_Dialog.rectTransform.parent).GetComponent<CanvasGroup>();

                    RectTransform backgroundTransform = m_Background.transform as RectTransform;
                    backgroundTransform.SetAsFirstSibling();
                    backgroundTransform.anchoredPosition = Vector2.zero;
                    backgroundTransform.sizeDelta = Vector2.zero;

                    m_Background.GetComponent<DialogBackground>().dialogAnimator = this;
                }
                return m_Background;
            }
        }

        private float m_BackgroundAlpha = 0.5f;
        public float backgroundAlpha
        {
            get { return m_BackgroundAlpha; }
            set { m_BackgroundAlpha = value; }
        }

        public DialogAnimator(float animationDuration = 0.5f)
        {
            m_AnimationDuration = animationDuration;
        }

        public virtual void AnimateShow(Action callback)
        {
            AnimateShowBackground();
        }

        public virtual void AnimateShowBackground()
        {
            background.blocksRaycasts = true;
            TweenManager.TweenFloat(f => background.alpha = f, background.alpha, m_BackgroundAlpha, m_AnimationDuration);
        }

        public virtual void AnimateHide(Action callback)
        {
            AnimateHideBackground();
        }

        public virtual void AnimateHideBackground()
        {
            background.blocksRaycasts = false;
            TweenManager.TweenFloat(f => background.alpha = f, background.alpha, 0f, m_AnimationDuration, 0f, () => UnityEngine.Object.Destroy(background.gameObject), false, Tween.TweenType.Linear);
        }

        public virtual void OnBackgroundClick()
        {
            m_Dialog.OnBackgroundClick();
        }
    }

    public class DialogAnimatorSlide : DialogAnimator
    {
        public enum SlideDirection
        {
            Bottom,
            Left,
            Top,
            Right
        }

        [SerializeField]
        private SlideDirection m_SlideInDirection = SlideDirection.Bottom;
        public SlideDirection slideInDirection
        {
            get { return m_SlideInDirection; }
            set { m_SlideInDirection = value; }
        }

        [SerializeField]
        private SlideDirection m_SlideOutDirection = SlideDirection.Bottom;
        public SlideDirection slideOutDirection
        {
            get { return m_SlideOutDirection; }
            set { m_SlideOutDirection = value; }
        }



        [SerializeField]
        private Tween.TweenType m_SlideInTweenType = Tween.TweenType.EaseOutQuint;
        public Tween.TweenType slideInTweenType
        {
            get { return m_SlideInTweenType; }
            set
            {
                if (value == Tween.TweenType.Custom)
                {
                    Debug.LogWarning("Cannot set tween type to 'Custom'");
                    return;
                }
                m_SlideInTweenType = value;
            }
        }

        [SerializeField]
        private Tween.TweenType m_SlideOutTweenType = Tween.TweenType.EaseInCubed;
        public Tween.TweenType slideOutTweenType
        {
            get { return m_SlideOutTweenType; }
            set
            {
                if (value == Tween.TweenType.Custom)
                {
                    Debug.LogWarning("Cannot set tween type to 'Custom'");
                    return;
                }
                m_SlideOutTweenType = value;
            }
        }

        public DialogAnimatorSlide(float animationDuration = 0.5f)
        {
            m_AnimationDuration = animationDuration;
        }

        public DialogAnimatorSlide(float animationDuration, SlideDirection slideInDirection, SlideDirection slideOutDirection)
        {
            m_AnimationDuration = animationDuration;
            m_SlideInDirection = slideInDirection;
            m_SlideOutDirection = slideOutDirection;
        }

        public DialogAnimatorSlide(float animationDuration, SlideDirection slideInDirection, SlideDirection slideOutDirection, Tween.TweenType slideInTweenType, Tween.TweenType slideOutTweenType)
        {
            m_AnimationDuration = animationDuration;
            m_SlideInDirection = slideInDirection;
            m_SlideOutDirection = slideOutDirection;

            if (slideInTweenType == Tween.TweenType.Custom || slideOutTweenType == Tween.TweenType.Custom)
            {
                Debug.LogWarning("Cannot set tween type to 'Custom'");
            }
            else
            {
                m_SlideInTweenType = slideInTweenType;
                m_SlideOutTweenType = slideOutTweenType;
            }
        }

        public override void AnimateShow(Action callback)
        {
            base.AnimateShow(callback);

            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Dialog.rectTransform);

            m_Dialog.rectTransform.anchoredPosition = GetSlidePosition(m_SlideInDirection);

            m_Dialog.rectTransform.localScale = Vector3.one;

            TweenManager.TweenVector2(v2 => m_Dialog.rectTransform.anchoredPosition = v2, m_Dialog.rectTransform.anchoredPosition, Vector2.zero, m_AnimationDuration, 0, callback, false, m_SlideInTweenType);
        }

        public override void AnimateHide(Action callback)
        {
            base.AnimateHide(callback);

            TweenManager.TweenVector2(v2 => m_Dialog.rectTransform.anchoredPosition = v2, m_Dialog.rectTransform.anchoredPosition, GetSlidePosition(m_SlideOutDirection), m_AnimationDuration, 0, callback, false, m_SlideOutTweenType);
        }

        private Vector2 GetSlidePosition(SlideDirection direction)
        {
            float canvasSize = (direction == SlideDirection.Left || direction == SlideDirection.Right) ? m_Dialog.rectTransform.GetRootCanvas().pixelRect.width : m_Dialog.rectTransform.GetRootCanvas().pixelRect.height;
            float dialogSize = (direction == SlideDirection.Left || direction == SlideDirection.Right) ? LayoutUtility.GetPreferredWidth(m_Dialog.rectTransform) : LayoutUtility.GetPreferredHeight(m_Dialog.rectTransform);

            dialogSize *= 1.1f;

            switch (direction)
            {
                case SlideDirection.Bottom:
                    return new Vector2(0f, -(canvasSize + dialogSize));
                case SlideDirection.Left:
                    return new Vector2(-(canvasSize + dialogSize), 0f);
                case SlideDirection.Right:
                    return new Vector2(canvasSize + dialogSize, 0f);
                case SlideDirection.Top:
                    return new Vector2(0f, canvasSize + dialogSize);
            }

            return Vector2.zero;
        }
    }
}