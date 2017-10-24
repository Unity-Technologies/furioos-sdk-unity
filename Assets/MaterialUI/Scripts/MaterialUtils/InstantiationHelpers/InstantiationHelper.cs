//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;

namespace MaterialUI
{
    /// <summary>
    /// Helper Component to handle the configuration of newly-instantiated objects.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public abstract class InstantiationHelper : MonoBehaviour
    {
        /// <summary>
        /// The option 'none'.
        /// </summary>
        public const int optionNone = -1;

        /// <summary>
        /// Configures the object.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public virtual void HelpInstantiate(params int[] options)
        {
            VectorImage[] vectorImages = GetComponentsInChildren<VectorImage>();

            for (int i = 0; i < vectorImages.Length; i++)
            {
                vectorImages[i].Refresh();
            }

            DestroyImmediate(GetComponent<InstantiationHelper>());
        }
    }
}