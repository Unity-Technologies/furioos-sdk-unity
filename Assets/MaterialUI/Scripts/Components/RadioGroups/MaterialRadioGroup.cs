//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace MaterialUI
{
    [ExecuteInEditMode]
    [AddComponentMenu("MaterialUI/Material Radio Group", 100)]
    public class MaterialRadioGroup : UIBehaviour
    {
        [SerializeField]
        private bool m_IsControllingChildren;
		public bool isControllingChildren
		{
			get { return m_IsControllingChildren; }
			set { m_IsControllingChildren = value; }
		}

        [SerializeField]
        private float m_AnimationDuration;
		public float animationDuration
		{
			get { return m_AnimationDuration; }
			set { m_AnimationDuration = value; }
		}

        [SerializeField]
        private Color m_OnColor;
		public Color onColor
		{
			get { return m_OnColor; }
			set { m_OnColor = value; }
		}

        [SerializeField]
        private Color m_OffColor;
		public Color offColor
		{
			get { return m_OffColor; }
			set { m_OffColor = value; }
		}

        [SerializeField]
        private Color m_DisabledColor;
		public Color disabledColor
		{
			get { return m_DisabledColor; }
			set { m_DisabledColor = value; }
		}

        [SerializeField]
        private bool m_ChangeGraphicColor;
		public bool changeGraphicColor
		{
			get { return m_ChangeGraphicColor; }
			set { m_ChangeGraphicColor = value; }
		}

        [SerializeField]
        private Color m_GraphicOnColor;
		public Color graphicOnColor
		{
			get { return m_GraphicOnColor; }
			set { m_GraphicOnColor = value; }
		}

        [SerializeField]
        private Color m_GraphicOffColor;
		public Color graphicOffColor
		{
			get { return m_GraphicOffColor; }
			set { m_GraphicOffColor = value; }
		}

        [SerializeField]
        private Color m_GraphicDisabledColor;
		public Color graphicDisabledColor
		{
			get { return m_GraphicDisabledColor; }
			set { m_GraphicDisabledColor = value; }
		}

        [SerializeField]
        private bool m_ChangeRippleColor;
		public bool changeRippleColor
		{
			get { return m_ChangeRippleColor; }
			set { m_ChangeRippleColor = value; }
		}

        [SerializeField]
        private Color m_RippleOnColor;
		public Color rippleOnColor
		{
			get { return m_RippleOnColor; }
			set { m_RippleOnColor = value; }
		}

        [SerializeField]
        private Color m_RippleOffColor;
        public Color rippleOffColor
        {
            get { return m_RippleOffColor; }
            set { m_RippleOffColor = value; }
        }

        public void Refresh()
        {
            if (m_IsControllingChildren)
            {
                for (int i = 0; i < GetComponentsInChildren<MaterialRadio>().Length; i++)
                {
                    MaterialRadio materialRadio = GetComponentsInChildren<MaterialRadio>()[i];
                    materialRadio.animationDuration = m_AnimationDuration;
                    materialRadio.onColor = m_OnColor;
                    materialRadio.offColor = m_OffColor;
                    materialRadio.disabledColor = m_DisabledColor;
                    materialRadio.changeGraphicColor = m_ChangeGraphicColor;
                    materialRadio.graphicOnColor = m_GraphicOnColor;
                    materialRadio.graphicOffColor = m_GraphicOffColor;
                    materialRadio.graphicDisabledColor = m_GraphicDisabledColor;
                    materialRadio.changeRippleColor = m_ChangeRippleColor;
                    materialRadio.rippleOnColor = m_RippleOnColor;
                    materialRadio.rippleOffColor = m_RippleOffColor;

#if UNITY_EDITOR
					materialRadio.EditorValidate();
#endif
                }
            }
        }

		//TODO: Maybe do a custom inspector here to avoid refreshing everything every frame?
#if UNITY_EDITOR
        void Update()
        {
            if (!Application.isPlaying)
            {
                Refresh();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            Refresh();
        }
#endif

        protected override void Start()
        {
            base.Start();
            Refresh();
        }
    }
}