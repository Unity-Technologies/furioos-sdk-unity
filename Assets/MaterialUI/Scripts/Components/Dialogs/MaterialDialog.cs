//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    public abstract class MaterialDialog : MonoBehaviour
    {
        private RectTransform m_RectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if (m_RectTransform == null)
                {
                    m_RectTransform = transform as RectTransform;
                }

                return m_RectTransform;
            }
        }

        private CanvasGroup m_CanvasGroup;
        public CanvasGroup canvasGroup
        {
            get
            {
                if (m_CanvasGroup == null)
                {
                    m_CanvasGroup = gameObject.GetAddComponent<CanvasGroup>();
                    m_CanvasGroup.blocksRaycasts = false;
                }
                return m_CanvasGroup;
            }
        }

        private DialogAnimator m_DialogAnimator;
        public DialogAnimator dialogAnimator
        {
            get
            {
                if (m_DialogAnimator == null)
                {
                    m_DialogAnimator = new DialogAnimatorSlide();
                }

                return m_DialogAnimator;
            }

            set { m_DialogAnimator = value; }
        }

        public CanvasGroup backgroundCanvasGroup
        {
            get
            {
                if (dialogAnimator != null)
                {
                    return dialogAnimator.background;
                }
                else
                {
                    return null;
                }
            }
        }

		private bool m_IsModal;
		public bool isModal
		{
			get { return m_IsModal; }
			set { m_IsModal = value; }
		}

		private bool m_DestroyOnHide = true;
		public bool destroyOnHide
		{
			get { return m_DestroyOnHide; }
			set { m_DestroyOnHide = value; }
		}
		
		private Action m_CallbackShowAnimationOver;
		public Action callbackShowAnimationOver
		{
			get { return m_CallbackShowAnimationOver; }
			set { m_CallbackShowAnimationOver = value; }
		}
		
		private Action m_CallbackHideAnimationOver;
		public Action callbackHideAnimationOver
		{
			get { return m_CallbackHideAnimationOver; }
			set { m_CallbackHideAnimationOver = value; }
		}

        public virtual void Initialize()
        {
            float canvasWidth = DialogManager.rectTransform.rect.width;

            if (canvasWidth < 448)
            {
                rectTransform.sizeDelta = new Vector2(canvasWidth - 48f, rectTransform.sizeDelta.y);
            }
            else if (canvasWidth < 648f)
            {
                rectTransform.sizeDelta = new Vector2(400, rectTransform.sizeDelta.y);
            }
            else
            {
                rectTransform.sizeDelta = new Vector2(600, rectTransform.sizeDelta.y);
            }

            gameObject.SetActive(false);
        }

        public void ShowModal()
        {
            m_IsModal = true;
            Show();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            dialogAnimator.dialog = this;
			dialogAnimator.AnimateShow(m_CallbackShowAnimationOver);

            canvasGroup.blocksRaycasts = true;
        }

        public virtual void Hide()
        {
            dialogAnimator.dialog = this;
            dialogAnimator.AnimateHide(() =>
			{
				if (m_CallbackHideAnimationOver != null)
				{
					m_CallbackHideAnimationOver();
				}
				
				if (m_DestroyOnHide)
				{
					Destroy(gameObject);
				}
				else
				{
					gameObject.SetActive(false);
				}
			});

            canvasGroup.blocksRaycasts = false;
        }

        public virtual void OnBackgroundClick()
        {
            if (!m_IsModal)
            {
                Hide();
            }
        }
    }
}