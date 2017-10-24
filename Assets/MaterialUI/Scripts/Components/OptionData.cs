//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialUI
{
    /// <summary>
    /// Contains information about the data of a list of options.
    /// </summary>
    [Serializable]
    public class OptionDataList
    {
        /// <summary>
        /// The type of the images in the list.
        /// </summary>
        [SerializeField]
		private ImageDataType m_ImageType = ImageDataType.VectorImage;
        /// <summary>
        /// The type of the images in the list.
        /// </summary>
        public ImageDataType imageType
		{
			get { return m_ImageType; }
			set { m_ImageType = value; }
		}

        /// <summary>
        /// The list of options.
        /// </summary>
        [SerializeField]
        private List<OptionData> m_Options = new List<OptionData>();
        /// <summary>
        /// The list of options.
        /// </summary>
        public List<OptionData> options
		{
			get { return m_Options; }
			set { m_Options = value; }
		}
    }

    /// <summary>
    /// Contains information about a list option's data.
    /// </summary>
    [Serializable]
    public class OptionData
    {
        /// <summary>
        /// The option's text.
        /// </summary>
        [SerializeField]
        private string m_Text;
        /// <summary>
        /// The option's text.
        /// </summary>
        public string text
		{
			get { return m_Text; }
			set { m_Text = value; }
		}

        /// <summary>
        /// The option's ImageData.
        /// </summary>
        [SerializeField]
        private ImageData m_ImageData;
        /// <summary>
        /// The option's ImageData.
        /// </summary>
        public ImageData imageData
        {
            get { return m_ImageData; }
            set { m_ImageData = value; }
        }

        /// <summary>
        /// Called when the option is selected.
        /// </summary>
        private Action m_OnOptionSelected;
        /// <summary>
        /// Called when the option is selected.
        /// </summary>
        public Action onOptionSelected
		{
			get { return m_OnOptionSelected; }
			set { m_OnOptionSelected = value; }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionData"/> class.
        /// </summary>
        public OptionData() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionData"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="imageData">The image data.</param>
        /// <param name="onOptionSelected">Called when the option is selected.</param>
        public OptionData(string text, ImageData imageData, Action onOptionSelected = null)
        {
            m_Text = text;
            m_ImageData = imageData;
			m_OnOptionSelected = onOptionSelected;
        }
    }
}