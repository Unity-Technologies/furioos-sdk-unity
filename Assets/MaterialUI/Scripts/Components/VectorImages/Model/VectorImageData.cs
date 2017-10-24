//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;

namespace MaterialUI
{
    [Serializable]
    public class VectorImageData
    {
        [SerializeField]
        private Glyph m_Glyph = new Glyph();
        public Glyph glyph
        {
            get { return m_Glyph; }
            set { m_Glyph = value; }
        }

        [SerializeField]
        private Font m_Font;
        public Font font
        {
            get { return m_Font; }
            set { m_Font = value; }
        }

        public VectorImageData() { }

        public VectorImageData(Glyph glyph, Font font)
        {
            m_Glyph = glyph;
            if (!m_Glyph.unicode.StartsWith(@"\u"))
            {
                m_Glyph.unicode = @"\u" + m_Glyph.unicode;
            }

            m_Font = font;
        }

        public bool ContainsData()
        {
            return m_Font != null && m_Glyph != null && !string.IsNullOrEmpty(m_Glyph.name) && !string.IsNullOrEmpty(m_Glyph.unicode);
        }
    }
}