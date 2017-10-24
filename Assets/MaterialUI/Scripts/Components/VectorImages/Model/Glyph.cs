//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;

namespace MaterialUI
{
    [Serializable]
    public class Glyph
    {
        [SerializeField]
        private string m_Name;
        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        [SerializeField]
        private string m_Unicode;
        public string unicode
        {
            get { return m_Unicode; }
            set { m_Unicode = value; }
        }

        public Glyph() { }
        public Glyph(string name, string unicode, bool fillSlashU)
        {
            m_Name = name;
            m_Unicode = unicode;

            if (fillSlashU)
            {
                if (!m_Unicode.StartsWith(@"\u"))
                {
                    m_Unicode = @"\u" + m_Unicode;
                }
            }
        }
    }
}
