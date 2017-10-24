//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Time Picker Clock", 100)]
    public class DialogTimePickerClock : MonoBehaviour, IDragHandler, IPointerClickHandler
	{
		[SerializeField] private DialogTimePicker m_TimePicker;

		private Vector2 m_ClockPosition;

		void Start()
		{
			Init();
		}

		public void Init()
		{
			m_ClockPosition = new Vector2(transform.position.x, transform.position.y);
		}

		public void OnDrag(PointerEventData eventData)
		{
			handleData(eventData);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			handleData(eventData);
		}

		private void handleData(PointerEventData eventData)
		{
			Vector2 clickPosition = eventData.position - m_ClockPosition;
			float degreeAngle = Mathf.Rad2Deg * Mathf.Atan(clickPosition.y / clickPosition.x);

			if (clickPosition.x < 0) degreeAngle += 180;
			m_TimePicker.SetAngle(degreeAngle);
		}
	}
}
