//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Snackbar Animator", 100)]
    public class SnackbarAnimator : ToastAnimator
    {
        [SerializeField]
        private Button m_ActionButton;

        [SerializeField]
        private Text m_ActionText;

        private Action m_OnActionButtonClicked;

        public void Show(Snackbar snackbar)
        {
            m_OnActionButtonClicked = snackbar.onActionButtonClicked;

            if (m_ActionButton != null)
            {
                m_ActionButton.gameObject.SetActive(!string.IsNullOrEmpty(snackbar.actionName));
                m_ActionText.text = snackbar.actionName.ToUpper();
            }

            base.Show(snackbar);
            StartCoroutine(Setup());
        }

        public void OnActionButtonClicked()
        {
            if (m_OnActionButtonClicked != null)
            {
                m_OnActionButtonClicked();
            }

            m_CurrentPosition = m_RectTransform.position.y;
            m_State = 2;
            m_AnimStartTime = Time.realtimeSinceStartup;
        }

        protected override void OnAnimDone()
        {
            if (!SnackbarManager.OnSnackbarCompleted())
            {
                Destroy(gameObject);
            }
        }

        private IEnumerator Setup()
        {
            yield return new WaitForEndOfFrame();

            LayoutElement layoutElement = m_Text.GetComponent<LayoutElement>();
            float buttonWidth = m_ActionButton.GetComponent<MaterialButton>().preferredWidth;
            HorizontalLayoutGroup horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
            float otherWidth = buttonWidth + horizontalLayoutGroup.padding.left + horizontalLayoutGroup.spacing;

            if (Screen.height > Screen.width)
            {
                float height = m_RectTransform.GetProperSize().y;
                GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                m_RectTransform.sizeDelta = new Vector2(Screen.width, height);
                layoutElement.minWidth = Screen.width - otherWidth - 4;
                m_PanelImage.sprite = null;

                m_MaterialMovableFab = FindObjectOfType<MaterialMovableFab>();
                if (m_MaterialMovableFab != null)
                {
                    m_FabRectTransform = m_MaterialMovableFab.GetComponent<RectTransform>();
                    m_FabStartPos = m_FabRectTransform.position.y;
                    m_MoveFab = true;
                }
                else
                {
                    m_FabRectTransform = null;
                    m_MoveFab = false;
                }
            }
            else
            {
                layoutElement.minWidth = 288f - otherWidth;
                layoutElement.preferredWidth = -1f;

                LayoutRebuilder.MarkLayoutForRebuild(m_RectTransform);

                if (m_RectTransform.GetProperSize().x > 568f)
                {
                    layoutElement.preferredWidth = 568f;
                }
            }

            m_OffPos.y = -m_RectTransform.GetProperSize().y * 1.05f;
            m_RectTransform.position = m_OffPos;
            m_CurrentPosition = m_RectTransform.position.y;

            GetComponent<CanvasGroup>().alpha = 1f;
            m_OnPos.y = 0f;
            m_OffAlpha = 1f;
        }
    }
}