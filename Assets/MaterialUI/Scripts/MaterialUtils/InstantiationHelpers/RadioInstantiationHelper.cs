//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Helper Component to handle the configuration of newly-instantiated RadioGroups.
    /// </summary>
    /// <seealso cref="MaterialUI.InstantiationHelper" />
    public class RadioInstantiationHelper : InstantiationHelper
    {
        /// <summary>
        /// Should the RadioButtons have labels?
        /// </summary>
        public const int optionHasLabel = 0;
        /// <summary>
        /// Should the RadioButtons have icons?
        /// </summary>
        public const int optionHasIcon = 1;

        /// <summary>
        /// The MaterialRadioGroup.
        /// </summary>
        [SerializeField]
        private MaterialRadioGroup m_Group;

        /// <summary>
        /// The ToggleBase.
        /// </summary>
        [SerializeField]
        private ToggleBase m_ToggleBase;

        /// <summary>
        /// The label template.
        /// </summary>
        [SerializeField]
        private Text m_Label;

        /// <summary>
        /// The icon template.
        /// </summary>
        [SerializeField]
        private VectorImage m_Icon;

        /// <summary>
        /// Configures the object.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public override void HelpInstantiate(params int[] options)
        {
            if (options.Contains(optionHasLabel))
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

            for (int i = 0; i < 2; i++)
            {
                RectTransform instance = (RectTransform)Instantiate(m_ToggleBase.gameObject).transform;
                instance.SetParent(m_ToggleBase.transform.parent);
                instance.localScale = Vector3.one;
                instance.localEulerAngles = Vector3.zero;

                instance.name = "RadioButton " + (i + 2);
                instance.GetComponent<Toggle>().isOn = false;
            }

            base.HelpInstantiate(options);
        }
    }
}