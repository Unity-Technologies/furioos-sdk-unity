//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Toast Animator", 100)]
    public class ToastAnimator : MonoBehaviour
    {
        [SerializeField]
        protected Text m_Text;

        [SerializeField]
        protected RectTransform m_RectTransform;

        [SerializeField]
        protected Image m_PanelImage;

        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        protected int m_State;
        protected bool m_AnimDown;
        protected Vector2 m_OnPos;
        protected Vector2 m_OffPos;
        protected Vector2 m_TempVec2;
        protected float m_TimeToWait;
        protected float m_AnimSpeed = 0.5f;
        protected float m_AnimStartTime;
        protected float m_AnimDeltaTime;
        protected float m_OffAlpha = 0f;
        protected float m_CurrentPosition;

        protected bool m_MoveFab;
        protected MaterialMovableFab m_MaterialMovableFab;
        protected RectTransform m_FabRectTransform;
        protected float m_FabStartPos;

		void OnEnable()
		{
			Init();

			transform.localScale = new Vector3(1, 1, 1);

			m_AnimStartTime = Time.realtimeSinceStartup;
			m_CurrentPosition = m_RectTransform.position.y;
			m_State = 2;
		}

		public void Init()
		{
			m_OnPos = new Vector2(Screen.width / 2f, (Screen.height / 8f));
			m_OffPos = new Vector2(Screen.width / 2f, Screen.height / 10f);
			m_RectTransform.position = m_OffPos;

			m_CanvasGroup.alpha = 0;
		}

        public void Show(Toast toast)
        {
            m_TimeToWait = toast.duration;
            m_Text.text = toast.content;
            m_PanelImage.color = toast.panelColor;
            m_Text.color = toast.textColor;
            m_Text.fontSize = toast.fontSize;

			Init();

            m_AnimStartTime = Time.realtimeSinceStartup;
            m_CurrentPosition = m_RectTransform.position.y;
            m_State = 1;
        }

        void Update()
        {
            m_AnimDeltaTime = Time.realtimeSinceStartup - m_AnimStartTime;

            if (m_State == 1)
            {
                if (m_AnimDeltaTime < m_AnimSpeed)
                {
                    m_TempVec2 = m_RectTransform.position;
                    m_TempVec2.y = Tween.CubeOut(m_CurrentPosition, m_OnPos.y, m_AnimDeltaTime, m_AnimSpeed);
                    m_RectTransform.position = m_TempVec2;
                    m_CanvasGroup.alpha = Tween.CubeInOut(m_CanvasGroup.alpha, 1f, m_AnimDeltaTime, m_AnimSpeed);
                    if (m_MoveFab)
                    {
                        m_FabRectTransform.position = new Vector3(m_FabRectTransform.position.x, m_FabStartPos + (m_RectTransform.position.y - m_OffPos.y), m_FabRectTransform.position.z);
                    }
                }
                else
                {
                    m_RectTransform.position = m_OnPos;
                    if (m_MoveFab)
                    {
                        m_FabRectTransform.position = new Vector3(m_FabRectTransform.position.x, m_FabStartPos + (m_RectTransform.position.y - m_OffPos.y), m_FabRectTransform.position.z);
                    }
                    StartCoroutine(WaitTime());
                    m_State = 0;
                }
            }
            else if (m_State == 2)
            {
                if (m_AnimDeltaTime < m_AnimSpeed)
                {
                    m_TempVec2 = m_RectTransform.position;
                    m_TempVec2.y = Tween.CubeInOut(m_CurrentPosition, m_OffPos.y, m_AnimDeltaTime, m_AnimSpeed);
                    m_RectTransform.position = m_TempVec2;
                    m_CanvasGroup.alpha = Tween.CubeIn(m_CanvasGroup.alpha, m_OffAlpha, m_AnimDeltaTime, m_AnimSpeed);
                    if (m_MoveFab)
                    {
                        m_FabRectTransform.position = new Vector3(m_FabRectTransform.position.x, m_FabStartPos + (m_RectTransform.position.y - m_OffPos.y), m_FabRectTransform.position.z);
                    }
                }
                else
                {
                    if (m_MoveFab)
                    {
                        m_FabRectTransform.position = new Vector3(m_FabRectTransform.position.x, m_FabStartPos, m_FabRectTransform.position.z);
                    }
                    m_State = 0;
                    OnAnimDone();
                }
            }
        }

        private IEnumerator WaitTime()
        {
            m_AnimDown = true;
            yield return new WaitForSeconds(m_TimeToWait);
            if (m_AnimDown)
            {
                m_AnimStartTime = Time.realtimeSinceStartup;
                m_CurrentPosition = m_RectTransform.position.y;
                m_State = 2;
            }
        }

        protected virtual void OnAnimDone()
        {
            if (!ToastManager.OnToastCompleted())
            {
                Destroy(gameObject);
            }
        }
    }
}
