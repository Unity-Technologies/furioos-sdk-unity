//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Clickable Option", 100)]
    public class DialogClickableOption : MonoBehaviour, IPointerClickHandler, ISubmitHandler
    {
		private Action<int> m_OnClickAction;
        public Action<int> onClickAction
        {
            get { return m_OnClickAction; }
            set { m_OnClickAction = value; }
        }

		private int m_Index;
        public int index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_OnClickAction != null)
            {
                m_OnClickAction.Invoke(m_Index);
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (m_OnClickAction != null)
            {
                m_OnClickAction.Invoke(m_Index);
            }
        }
    }
}