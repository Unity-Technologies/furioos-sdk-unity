//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Radio Option", 100)]
    public class DialogRadioOption : DialogClickableOption
    {
		[SerializeField]
		private Text m_ItemText;
        public Text itemText
        {
            get { return m_ItemText; }
        }

		[SerializeField]
		private MaterialRadio m_ItemRadio;
        public MaterialRadio itemRadio
        {
            get { return m_ItemRadio; }
        }

		[SerializeField]
		private MaterialRipple m_ItemRipple;
        public MaterialRipple itemRipple
        {
            get { return m_ItemRipple; }
        }

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
    }
}