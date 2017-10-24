//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Tab Pages Scroll Detector", 100)]
    public class TabPagesScrollDetector : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField] private TabView m_TabView;
        public TabView tabView
        {
            get { return m_TabView; }
            set { m_TabView = value; }
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_TabView.TabPageDrag();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_TabView.TabPagePointerUp(eventData.delta.x);
        }
    }
}