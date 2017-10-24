//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Copies a color from one graphic and applies it to another.
    /// </summary>
    /// <seealso cref="UnityEngine.EventSystems.UIBehaviour" />
    [ExecuteInEditMode]
    [AddComponentMenu("MaterialUI/Color Copier", 100)]
    public class ColorCopier : UIBehaviour
    {
        /// <summary>
        /// The source graphic.
        /// </summary>
        [SerializeField]
        private Graphic m_SourceGraphic;
        /// <summary>
        /// Gets or sets the source graphic.
        /// </summary>
        /// <value>
        /// The source graphic.
        /// </value>
        public Graphic sourceGraphic
        {
            get { return m_SourceGraphic; }
            set
            {
                m_SourceGraphic = value;
                UpdateColor();
            }
        }

        /// <summary>
        /// The destination graphic.
        /// </summary>
        [SerializeField]
        private Graphic m_DestinationGraphic;
        /// <summary>
        /// Gets or sets the destination graphic.
        /// </summary>
        /// <value>
        /// The destination graphic.
        /// </value>
        public Graphic destinationGraphic
        {
            get { return m_DestinationGraphic; }
            set
            {
                m_DestinationGraphic = value;
                UpdateColor();
            }
        }

        /// <summary>
        /// The last color copied from the source graphic.
        /// </summary>
        private Color m_LastColor;

        /// <summary>
        /// Called each frame after Update().
        /// </summary>
        private void LateUpdate()
        {
            UpdateColor();
        }

#if UNITY_EDITOR
        /// <summary>
        /// See MonoBehaviour.OnValidate.
        /// </summary>
        protected override void OnValidate()
        {
            UpdateColor();
        }
#endif

        /// <summary>
        /// Updates the color of the destination graphic to match the source graphic.
        /// </summary>
        public void UpdateColor()
        {
            if (sourceGraphic && destinationGraphic)
            {
                if (sourceGraphic.color != m_LastColor)
                {
                    destinationGraphic.color = sourceGraphic.color;
                    m_LastColor = sourceGraphic.color;
                }
            }
        }
    }
}