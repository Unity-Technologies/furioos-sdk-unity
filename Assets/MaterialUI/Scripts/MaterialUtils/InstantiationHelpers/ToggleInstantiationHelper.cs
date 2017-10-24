//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Helper Component to handle the configuration of newly-instantiated Toggles.
    /// </summary>
    /// <seealso cref="MaterialUI.InstantiationHelper" />
    public class ToggleInstantiationHelper : InstantiationHelper
    {
        /// <summary>
        /// Should the toggle have a label?
        /// </summary>
        public const int optionLabel = 0;
        /// <summary>
        /// Should the toggle have an icon?
        /// </summary>
        public const int optionHasIcon = 1;

        /// <summary>
        /// The ToggleBase.
        /// </summary>
        [SerializeField]
        private ToggleBase m_ToggleBase;

        /// <summary>
        /// The label.
        /// </summary>
        [SerializeField]
        private Text m_Label;

        /// <summary>
        /// The icon.
        /// </summary>
        [SerializeField]
        private VectorImage m_Icon;

        /// <summary>
        /// Configures the object.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public override void HelpInstantiate(params int[] options)
        {
			if (options.Contains(optionLabel))
			{
				DestroyImmediate(m_Icon.gameObject);
				m_ToggleBase.graphic = m_Label;
				m_ToggleBase.graphicOffColor = MaterialColor.textDark;
			}
			else if (options.Contains(optionHasIcon))
			{
				DestroyImmediate(m_Label.gameObject);
				m_ToggleBase.graphic = m_Icon;
				m_ToggleBase.graphicOffColor = MaterialColor.iconDark;
			}

			base.HelpInstantiate(options);

#if UNITY_EDITOR
			m_ToggleBase.EditorValidate();
#endif
		}
    }
}