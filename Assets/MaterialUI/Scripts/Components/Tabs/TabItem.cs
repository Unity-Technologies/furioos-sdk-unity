//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Tab Item", 100)]
    public class TabItem : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [SerializeField]
        private Text m_ItemText;
        public Text itemText
        {
            get { return m_ItemText; }
            set { m_ItemText = value; }
        }

        [SerializeField]
        private Graphic m_ItemIcon;
        public Graphic itemIcon
        {
            get { return m_ItemIcon; }
            set { m_ItemIcon = value; }
        }

        [SerializeField]
        private TabView m_TabView;
        public TabView tabView
        {
            get { return m_TabView; }
            set { m_TabView = value; }
        }

        private int m_Id;
        public int id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        private RectTransform m_RectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if (m_RectTransform == null)
                {
                    m_RectTransform = (RectTransform)transform;
                }
                return m_RectTransform;
            }
        }

        private CanvasGroup m_CanvasGroup;
        public CanvasGroup canvasGroup
        {
            get
            {
                if (m_CanvasGroup == null)
                {
                    m_CanvasGroup = gameObject.GetComponent<CanvasGroup>();
                }
                return m_CanvasGroup;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (interactable)
            {
                m_TabView.SetPage(id);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (interactable)
            {
                m_TabView.TabItemPointerDown(id);
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (interactable)
            {
                m_TabView.SetPage(id);
            }
        }

        public void SetupGraphic(ImageDataType type)
        {
            if (gameObject.GetChildByName<Image>("Icon") == null || gameObject.GetChildByName<VectorImage>("Icon") == null) return;

            if (type == ImageDataType.Sprite)
            {
                m_ItemIcon = gameObject.GetChildByName<Image>("Icon");
                Destroy(gameObject.GetChildByName<VectorImage>("Icon").gameObject);
            }
            else
            {
                m_ItemIcon = gameObject.GetChildByName<VectorImage>("Icon");
                Destroy(gameObject.GetChildByName<Image>("Icon").gameObject);
            }
        }
    }
}