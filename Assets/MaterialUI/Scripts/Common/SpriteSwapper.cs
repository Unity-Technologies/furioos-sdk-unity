//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Swaps a sprite in an Image component between 3 possible sprites, based on the MaterialUIScaler scale factor.
    /// </summary>
    /// <seealso cref="UnityEngine.EventSystems.UIBehaviour" />
    [ExecuteInEditMode]
    [AddComponentMenu("MaterialUI/Sprite Swapper", 50)]
    public class SpriteSwapper : UIBehaviour
    {
        /// <summary>
        /// The target image to modify.
        /// </summary>
        [SerializeField]
        private Image m_TargetImage;
        /// <summary>
        /// The target image to modify.
        /// </summary>
        public Image targetImage
		{
			get { return m_TargetImage; }
			set
			{
				m_TargetImage = value;
				RefreshSprite();
			}
		}

        /// <summary>
        /// The root scaler to get the scale factor from.
        /// </summary>
        [SerializeField]
        private MaterialUIScaler m_RootScaler;
        /// <summary>
        /// The root scaler to get the scale factor from.
        /// </summary>
        public MaterialUIScaler rootScaler
		{
			get
			{
				if (m_RootScaler == null)
				{
					m_RootScaler = MaterialUIScaler.GetParentScaler(transform);
				}
				return m_RootScaler;
			}
		}

        /// <summary>
        /// The sprite to use when scaling is less than or equal to 1.
        /// </summary>
        [SerializeField]
        private Sprite m_Sprite1X;
        /// <summary>
        /// The sprite to use when scaling is less than or equal to 1.
        /// </summary>
        public Sprite sprite1X
		{
			get { return m_Sprite1X; }
			set
			{
				m_Sprite1X = value;
				RefreshSprite();
			}
		}

        /// <summary>
        /// The sprite to use when scaling is more than 1 and less than or equal to 2.
        /// </summary>
        [SerializeField]
        private Sprite m_Sprite2X;
        /// <summary>
        /// The sprite to use when scaling is more than 1 and less than or equal to 2.
        /// </summary>
        public Sprite sprite2X
		{
			get { return m_Sprite2X; }
			set
			{
				m_Sprite2X = value;
				RefreshSprite();
			}
		}

        /// <summary>
        /// The sprite to use when scaling is more than 2.
        /// </summary>
        [SerializeField]
        private Sprite m_Sprite4X;
        /// <summary>
        /// The sprite to use when scaling is more than 2.
        /// </summary>
        public Sprite sprite4X
		{
			get { return m_Sprite4X; }
			set
			{
				m_Sprite4X = value;
				RefreshSprite();
			}
		}

        /// <summary>
        /// The last 1x sprite used, for caching purposes.
        /// </summary>
        private Sprite m_LastSprite1X;
        /// <summary>
        /// The last 2x sprite used, for caching purposes.
        /// </summary>
        private Sprite m_LastSprite2X;
        /// <summary>
        /// The last 4x sprite used, for caching purposes.
        /// </summary>
        private Sprite m_LastSprite4X;

        /// <summary>
        /// See MonoBehaviour.OnEnable.
        /// </summary>
        protected override void OnEnable()
        {
            if (!targetImage)
            {
                targetImage = gameObject.GetComponent<Image>();
            }

            RefreshSprite();
        }

        /// <summary>
        /// See MonoBehaviour.Start.
        /// </summary>
        protected override void Start()
        {
            if (rootScaler == null) return;
            rootScaler.OnScaleFactorChange += SwapSprite;
            RefreshSprite();
        }

#if UNITY_EDITOR
        /// <summary>
        /// See MonoBehaviour.OnValidate.
        /// </summary>
        protected override void OnValidate()
        {
            RefreshSprite();
        }
#endif

        /// <summary>
        /// Checks to see if the sprite needs to be swapped.
        /// </summary>
        public void RefreshSprite()
        {
            if (rootScaler == null) return;
            SwapSprite(rootScaler.scaler.scaleFactor);
        }

        /// <summary>
        /// Swaps the sprite.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        private void SwapSprite(float scaleFactor)
        {
            if (!targetImage) return;

            if (scaleFactor > 2f && sprite4X)
            {
                targetImage.sprite = sprite4X;
            }
            else if (scaleFactor > 1f && sprite2X)
            {
                targetImage.sprite = sprite2X;
            }
            else
            {
                targetImage.sprite = sprite1X;
            }
        }
    }
}