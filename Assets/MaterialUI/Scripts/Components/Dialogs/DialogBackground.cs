//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Background", 100)]
    public class DialogBackground : MonoBehaviour, IPointerClickHandler
    {
        private DialogAnimator m_DialogAnimator;
        public DialogAnimator dialogAnimator
        {
            get { return m_DialogAnimator; }
            set { m_DialogAnimator = value; }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_DialogAnimator != null)
            {
                m_DialogAnimator.OnBackgroundClick();
            }
        }
    }
}