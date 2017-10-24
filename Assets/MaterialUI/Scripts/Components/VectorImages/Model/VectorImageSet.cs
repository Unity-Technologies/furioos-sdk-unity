//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using System.Collections.Generic;

namespace MaterialUI
{
    [Serializable]
    public class VectorImageSet
    {
		[SerializeField]
		private List<Glyph> m_IconGlyphList;
		public List<Glyph> iconGlyphList
		{
			get { return m_IconGlyphList; }
			set { m_IconGlyphList = value; }
		}

		public VectorImageSet()
		{
			m_IconGlyphList = new List<Glyph>();
		}
    }
}
