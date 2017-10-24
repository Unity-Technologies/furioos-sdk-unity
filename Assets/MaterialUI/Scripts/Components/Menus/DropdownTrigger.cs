//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dropdown Trigger", 100)]
    public class DropdownTrigger : MonoBehaviour, IPointerClickHandler, ISubmitHandler
    {
        [SerializeField]
        private MaterialDropdown m_Dropdown;
		public MaterialDropdown dropdown
		{
			get { return m_Dropdown; }
			set { m_Dropdown = value; }
		}

        [SerializeField]
        private int m_Index;
		public int index
		{
			get { return m_Index; }
			set { m_Index = value; }
		}

        public void OnPointerClick(PointerEventData eventData)
        {
            if (dropdown != null)
            {
                dropdown.Select(m_Index);
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (dropdown != null)
            {
                dropdown.Select(m_Index, true);
            }
        }
    }
}