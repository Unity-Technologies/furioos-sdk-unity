//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    /// <summary>
    /// Static class to handle the updating of components at a consistent rate in edit mode.
    /// </summary>
    public static class EditorUpdate
    {
        /// <summary>
        /// The editor update delegate.
        /// </summary>
        public delegate void EditorUpdateDelegate();
        /// <summary>
        /// Called approximately 100 times per second in edit mode.
        /// </summary>
        public static EditorUpdateDelegate onEditorUpdate;

        /// <summary>
        /// Is the system initialized?
        /// </summary>
        private static bool m_IsInitialized;

        /// <summary>
        /// Initializes the system.
        /// </summary>
        public static void Init()
        {
            if (!m_IsInitialized)
            {
                EditorApplication.update += Update;
                m_IsInitialized = true;
            }
        }

        /// <summary>
        /// Called from EditorApplication.update, approximately 100 times per second in edit mode
        /// </summary>
        public static void Update()
        {
            if (!Application.isPlaying)
            {
                if (onEditorUpdate != null) onEditorUpdate.Invoke();
            }
        }
    }
}
#endif