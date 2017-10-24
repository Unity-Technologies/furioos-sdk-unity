//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Collections.Generic;
using UnityEngine;

namespace MaterialUI
{
    /// <summary>
    /// Static class to help with the instantiation of MaterialUI objects.
    /// </summary>
    public static class PrefabManager
    {
        /// <summary>
        /// The list of cached objects.
        /// </summary>
        private static readonly List<GameObject> m_Prefabs = new List<GameObject>();
        /// <summary>
        /// The list of names of cached objects.
        /// </summary>
        private static readonly List<string> m_Names = new List<string>();

        /// <summary>
        /// Contains information about different MaterialUI prefabs that can be instantiated via <see cref="PrefabManager"/>.
        /// </summary>
        public static class ResourcePrefabs
        {
            /// <summary>
            /// Path to the circular progress indicator prefab.
            /// </summary>
            public const string progressIndicatorCircular = "Progress Indicators/Circle Progress Indicator";
            /// <summary>
            /// Path to the linear progress indicator prefab.
            /// </summary>
            public const string progressIndicatorLinear = "Progress Indicators/Linear Progress Indicator";

            /// <summary>
            /// Path to the alert dialog prefab.
            /// </summary>
            public const string dialogAlert = "Dialogs/DialogAlert";
            /// <summary>
            /// Path to the progress dialog prefab.
            /// </summary>
            public const string dialogProgress = "Dialogs/DialogProgress";
            /// <summary>
            /// Path to the simple list dialog prefab.
            /// </summary>
            public const string dialogSimpleList = "Dialogs/DialogSimpleList";
            /// <summary>
            /// Path to the checkbox list dialog prefab.
            /// </summary>
            public const string dialogCheckboxList = "Dialogs/DialogCheckboxList";
            /// <summary>
            /// Path to the radio list dialog prefab.
            /// </summary>
            public const string dialogRadioList = "Dialogs/DialogRadioList";
            /// <summary>
            /// Path to the time picker dialog prefab.
            /// </summary>
            public const string dialogTimePicker = "Dialogs/Pickers/DialogTimePicker";
            /// <summary>
            /// Path to the date picker dialog prefab.
            /// </summary>
            public const string dialogDatePicker = "Dialogs/Pickers/DialogDatePicker";
            /// <summary>
            /// Path to the prompt dialog prefab.
            /// </summary>
            public const string dialogPrompt = "Dialogs/DialogPrompt";

            /// <summary>
            /// Path to the disabled panel prefab.
            /// </summary>
            public const string disabledPanel = "DisabledPanel";
            /// <summary>
            /// Path to the slider dot prefab.
            /// </summary>
            public const string sliderDot = "SliderDot";
            /// <summary>
            /// Path to the dropdown panel prefab.
            /// </summary>
            public const string dropdownPanel = "Menus/Dropdown Panel";

            /// <summary>
            /// Path to the snackbar prefab.
            /// </summary>
            public const string snackbar = "Snackbar";
            /// <summary>
            /// Path to the toast prefab.
            /// </summary>
            public const string toast = "Toast";
        }

        /// <summary>
        /// Finds the GameObject with the matching name in the object pool, or if not pooled, from the path.
        /// </summary>
        /// <param name="nameWithPath">The name of the prefab, including the path.</param>
        /// <returns>The uninstantiated GameObject that matches the path, if found. If no GameObject is found, returns null.</returns>
        public static GameObject GetGameObject(string nameWithPath)
        {
            GameObject gameObject = null;

            if (!m_Names.Contains(nameWithPath))
            {
                gameObject = Resources.Load<GameObject>(nameWithPath);

                if (gameObject != null)
                {
                    m_Prefabs.Add(gameObject);
                    m_Names.Add(nameWithPath);
                }
            }
            else
            {
                for (int i = 0; i < m_Prefabs.Count; i++)
                {
                    if (m_Names[i] == nameWithPath)
                    {
                        if (m_Prefabs[i] != null)
                        {
                            gameObject = m_Prefabs[i];
                        }
                    }
                }
            }

            return gameObject;
        }

        /// <summary>
        /// Finds the GameObject with the matching name in the object pool, or if not pooled, from the path, then instantiates it.
        /// </summary>
        /// <param name="nameWithPath">The name of the prefab, including the path.</param>
        /// <param name="parent">The transform to set the parent of the instantiated GameObject.</param>
        /// <returns>The instantiated GameObject that matches the path, if found. If no GameObject is found, returns null.</returns>
        public static GameObject InstantiateGameObject(string nameWithPath, Transform parent)
        {
            GameObject go = GetGameObject(nameWithPath);

            if (go == null)
            {
                return null;
            }

            go = GameObject.Instantiate(go);

            if (parent == null)
            {
                return go;
            }

            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localPosition = Vector3.zero;

            return go;
        }
    }
}