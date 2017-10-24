//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace MaterialUI
{
    /// <summary>
    /// Component that contols up to 3 AnimatedShdow components to create a dynamic shadow effect.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="UnityEngine.EventSystems.IPointerEnterHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IPointerDownHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IPointerUpHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IPointerExitHandler" />
    /// <seealso cref="UnityEngine.EventSystems.ISelectHandler" />
    /// <seealso cref="UnityEngine.EventSystems.IDeselectHandler" />
    [AddComponentMenu("MaterialUI/Material Shadow", 50)]
    [ExecuteInEditMode]
    public class MaterialShadow : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        /// <summary>
        /// The types of events that can trigger 'active' shadowing.
        /// </summary>
        public enum ShadowsActive
        {
            Hovered,
            Clicked
        }

        /// <summary>
        /// The AnimatedShadows to control.
        /// </summary>
        [SerializeField]
        private AnimatedShadow[] m_AnimatedShadows;
        /// <summary>
        /// The AnimatedShadows to control.
        /// </summary>
        public AnimatedShadow[] animatedShadows
        {
            get { return m_AnimatedShadows; }
            set { m_AnimatedShadows = value; }
        }

        /// <summary>
        /// The index of m_AnimatedShadows to use for 'normal' shadowing.
        /// </summary>
        [SerializeField]
        [Range(0, 3)]
        private int m_ShadowNormalSize = 1;
        /// <summary>
        /// The index of m_AnimatedShadows to use for 'normal' shadowing.
        /// </summary>
        public int shadowNormalSize
        {
            get { return m_ShadowNormalSize; }
            set { m_ShadowNormalSize = value; }
        }

        /// <summary>
        /// The index of m_AnimatedShadows to use for 'active' shadowing.
        /// </summary>
        [SerializeField]
        [Range(0, 3)]
        private int m_ShadowActiveSize = 2;
        /// <summary>
        /// The index of m_AnimatedShadows to use for 'active' shadowing.
        /// </summary>
        public int shadowActiveSize
        {
            get { return m_ShadowActiveSize; }
            set { m_ShadowActiveSize = value; }
        }

        /// <summary>
        /// The event that will trigger 'active' shadowing.
        /// </summary>
        [SerializeField]
        private ShadowsActive m_ShadowsActiveWhen = ShadowsActive.Hovered;
        /// <summary>
        /// The event that will trigger 'active' shadowing.
        /// </summary>
        public ShadowsActive shadowsActiveWhen
        {
            get { return m_ShadowsActiveWhen; }
            set { m_ShadowsActiveWhen = value; }
        }

        /// <summary>
        /// Is this shadow enabled?
        /// </summary>
        [SerializeField]
        private bool m_IsEnabled = true;
        /// <summary>
        /// Is this shadow enabled?
        /// </summary>
        public bool isEnabled
        {
            get { return m_IsEnabled; }
            set { m_IsEnabled = value; }
        }

        /// <summary>
        /// The last value of shadowNormalSize.
        /// </summary>
        private int m_LastShadowNormalSize = int.MinValue;

        /// <summary>
        /// See MonoBehaviour.Update.
        /// </summary>
        void Update()
        {
            if (m_LastShadowNormalSize != shadowNormalSize)
            {
                SetShadowsInstant();
                m_LastShadowNormalSize = shadowNormalSize;
            }
        }

        /// <summary>
        /// See MonoBehaviour.Destroy.
        /// </summary>
        void OnDestroy()
        {
            m_AnimatedShadows = new AnimatedShadow[0];
        }

        /// <summary>
        /// Called when [pointer down].
        /// </summary>
        /// <param name="data">Current event data.</param>
        public void OnPointerDown(PointerEventData data)
        {
            if (shadowsActiveWhen == ShadowsActive.Clicked)
            {
                SetShadows(shadowActiveSize);
            }
        }

        /// <summary>
        /// Called when [pointer up].
        /// </summary>
        /// <param name="data">Current event data.</param>
        public void OnPointerUp(PointerEventData data)
        {
            if (shadowsActiveWhen == ShadowsActive.Clicked)
            {
                SetShadows(shadowNormalSize);
            }
        }

        /// <summary>
        /// Called when [pointer enter].
        /// </summary>
        /// <param name="data">Current event data.</param>
        public void OnPointerEnter(PointerEventData data)
        {
            if (shadowsActiveWhen == ShadowsActive.Hovered)
            {
                SetShadows(shadowActiveSize);
            }
        }

        /// <summary>
        /// Called when [pointer exit].
        /// </summary>
        /// <param name="data">Current event data.</param>
        public void OnPointerExit(PointerEventData data)
        {
            SetShadows(shadowNormalSize);
        }

        /// <summary>
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnSelect(BaseEventData eventData)
        {
            if (shadowsActiveWhen == ShadowsActive.Hovered)
            {
                SetShadows(shadowActiveSize);
            }
        }

        /// <summary>
        /// Called by the EventSystem when a new object is being selected.
        /// </summary>
        /// <param name="eventData">Current event data.</param>
        public void OnDeselect(BaseEventData eventData)
        {
            SetShadows(shadowNormalSize);
        }

        /// <summary>
        /// Sets which shadow should be shown.
        /// </summary>
        /// <param name="shadowOn">The index of the shadow to be shown.</param>
        public void SetShadows(int shadowOn)
        {
            if (isEnabled && m_AnimatedShadows != null)
            {
                for (int i = 0; i < m_AnimatedShadows.Length; i++)
                {
                    m_AnimatedShadows[i].SetShadow(i == shadowOn - 1);
                }
            }
        }

        /// <summary>
        /// Sets the 'normal' shadow to be shown, instantly with no animation.
        /// </summary>
        private void SetShadowsInstant()
        {
            if (isEnabled && m_AnimatedShadows != null)
            {
                for (int i = 0; i < m_AnimatedShadows.Length; i++)
                {
                    m_AnimatedShadows[i].SetShadow(i == m_ShadowNormalSize - 1);
                }
            }
        }
    }
}