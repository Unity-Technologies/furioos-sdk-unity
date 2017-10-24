//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;

namespace MaterialUI
{
    /// <summary>
    /// Helper Component to handle the configuration of newly-instantiated InputFields.
    /// </summary>
    /// <seealso cref="MaterialUI.InstantiationHelper" />
    public class InputFieldInstantiationHelper : InstantiationHelper
    {
        /// <summary>
        /// Should the InputField have an icon?
        /// </summary>
        public const int optionHasIcon = 0;
        /// <summary>
        /// Should the InputField have a clear button?
        /// </summary>
        public const int optionHasClearButton = 1;

        /// <summary>
        /// The MaterialInputField.
        /// </summary>
        [SerializeField]
        private MaterialInputField m_MaterialInputField;

        /// <summary>
        /// The icon.
        /// </summary>
        [SerializeField]
        private GameObject m_Icon;

        /// <summary>
        /// The clear button.
        /// </summary>
        [SerializeField]
        private GameObject m_ClearButton;

        /// <summary>
        /// Configures the InputField.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public override void HelpInstantiate(params int[] options)
        {
            if (!options.Contains(optionHasIcon))
            {
                DestroyImmediate(m_Icon);
                m_MaterialInputField.leftContentTransform = null;
                m_MaterialInputField.leftContentGraphic = null;
            }

            if (!options.Contains(optionHasClearButton)) // Clear button
            {
                DestroyImmediate(m_ClearButton);
                m_MaterialInputField.rightContentTransform = null;
                m_MaterialInputField.rightContentGraphic = null;
            }

            m_MaterialInputField.CalculateLayoutInputHorizontal();
            m_MaterialInputField.SetLayoutHorizontal();
            m_MaterialInputField.CalculateLayoutInputVertical();
            m_MaterialInputField.SetLayoutVertical();

            base.HelpInstantiate(options);
        }
    }
}