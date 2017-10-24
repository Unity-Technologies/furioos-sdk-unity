//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using System;

namespace MaterialUI
{
    public class Snackbar : Toast
    {
        private string m_ActionName;
		public string actionName
		{
			get { return m_ActionName; }
			set { m_ActionName = value; }
		}

        private Action m_OnActionButtonClicked;
		public Action onActionButtonClicked
		{
			get { return m_OnActionButtonClicked; }
			set { m_OnActionButtonClicked = value; }
		}

        public Snackbar(string content, float duration, Color panelColor, Color textColor, int fontSize, string actionName, Action onActionButtonClicked) : base(content, duration, panelColor, textColor, fontSize)
        {
            m_ActionName = actionName;
            m_OnActionButtonClicked = onActionButtonClicked;
        }
    }
}