//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [ExecuteInEditMode]
    [AddComponentMenu("MaterialUI/Material Radio", 100)]
    public class MaterialRadio : ToggleBase
    {
        [SerializeField]
        private Graphic m_DotImage;

        [SerializeField]
        private Graphic m_RingImage;

        private float m_CurrentDotSize;

        public override void TurnOn()
        {
            m_CurrentDotSize = m_DotImage.rectTransform.sizeDelta.x;
            m_CurrentColor = m_DotImage.color;

            base.TurnOn();
        }

        public override void TurnOnInstant()
        {
            base.TurnOnInstant();

            if (toggle.interactable)
            {
                m_DotImage.color = m_OnColor;
                m_RingImage.color = m_OnColor;
            }

            m_DotImage.rectTransform.sizeDelta = new Vector2(24, 24);
        }

        public override void TurnOff()
        {
            m_CurrentDotSize = m_DotImage.rectTransform.sizeDelta.x;
            m_CurrentColor = m_DotImage.color;

            base.TurnOff();
        }

        public override void TurnOffInstant()
        {
            base.TurnOffInstant();

            if (toggle.interactable)
            {
                m_DotImage.color = m_OffColor;
                m_RingImage.color = m_OffColor;
            }

            m_DotImage.rectTransform.sizeDelta = Vector2.zero;
        }

        public override void Enable()
        {
            if (toggle.isOn)
            {
                m_DotImage.color = m_OnColor;
                m_RingImage.color = m_OnColor;
            }
            else
            {
                m_DotImage.color = m_OffColor;
                m_RingImage.color = m_OffColor;
            }

            base.Enable();
        }

        public override void Disable()
        {
            m_DotImage.color = m_DisabledColor;
            m_RingImage.color = m_DisabledColor;

            base.Disable();
        }

        public override void AnimOn()
        {
            base.AnimOn();

            m_DotImage.color = Tween.QuintOut(m_CurrentColor, m_OnColor, m_AnimDeltaTime, m_AnimationDuration);
            m_RingImage.color = m_DotImage.color;

            float tempSize = Tween.QuintOut(m_CurrentDotSize, 24, m_AnimDeltaTime, m_AnimationDuration);

            m_DotImage.rectTransform.sizeDelta = new Vector2(tempSize, tempSize);
        }

        public override void AnimOnComplete()
        {
            base.AnimOnComplete();

            m_DotImage.color = m_OnColor;
            m_RingImage.color = m_OnColor;

            m_DotImage.rectTransform.sizeDelta = new Vector2(24, 24);
        }

        public override void AnimOff()
        {
            base.AnimOff();

            m_DotImage.color = Tween.QuintOut(m_CurrentColor, m_OffColor, m_AnimDeltaTime, m_AnimationDuration);
            m_RingImage.color = m_DotImage.color;

            float tempSize = Tween.QuintOut(m_CurrentDotSize, 0, m_AnimDeltaTime, m_AnimationDuration);

            m_DotImage.rectTransform.sizeDelta = new Vector2(tempSize, tempSize);
        }

        public override void AnimOffComplete()
        {
            base.AnimOffComplete();

            m_DotImage.color = m_OffColor;
            m_RingImage.color = m_OffColor;

            m_DotImage.rectTransform.sizeDelta = Vector2.zero;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (m_Interactable)
            {
                m_DotImage.color = toggle.isOn ? m_OnColor : m_OffColor;
                m_RingImage.color = toggle.isOn ? m_OnColor : m_OffColor;
            }
            else
            {
                m_DotImage.color = m_DisabledColor;
                m_RingImage.color = m_DisabledColor;
            }
        }
#endif
    }
}
