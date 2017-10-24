//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Simple Option", 100)]
    public class DialogSimpleOption : DialogClickableOption
    {
        [SerializeField]
        private Text m_ItemText;
		public Text itemText
		{
			get { return m_ItemText; }
			set { m_ItemText = value; }
		}

        private Graphic m_ItemIcon;
		public Graphic itemIcon
		{
			get { return m_ItemIcon; }
			set { m_ItemIcon = value; }
		}

        [SerializeField]
        private MaterialRipple m_ItemRipple;
		public MaterialRipple itemRipple
		{
			get { return m_ItemRipple; }
			set { m_ItemRipple = value; }
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