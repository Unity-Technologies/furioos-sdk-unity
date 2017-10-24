//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;

namespace MaterialUI
{
    public class Toast
    {
        private string m_Content;
		public string content
		{
			get { return m_Content; }
			set { m_Content = value; }
		}

        private float m_Duration;
		public float duration
		{
			get { return m_Duration; }
			set { m_Duration = value; }
		}

        private Color m_PanelColor;
		public Color panelColor
		{
			get { return m_PanelColor; }
			set { m_PanelColor = value; }
		}

        private Color m_TextColor;
		public Color textColor
		{
			get { return m_TextColor; }
			set { m_TextColor = value; }
		}

        private int m_FontSize;
		public int fontSize
		{
			get { return m_FontSize; }
			set { m_FontSize = value; }
		}

        public Toast(string content, float duration, Color panelColor, Color textColor, int fontSize)
        {
            m_Content = content;
            m_Duration = duration;
            m_PanelColor = panelColor;
            m_TextColor = textColor;
            m_FontSize = fontSize;
        }
    }
}