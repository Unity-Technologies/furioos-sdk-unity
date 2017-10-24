//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEditor.AnimatedValues;
using System;
using System.Linq;

namespace MaterialUI
{
    public class MaterialBaseEditor : Editor
    {
        protected class TargetArray<T>
        {
            private T[] m_TargetArray;

            public T[] targetArray
            {
                get { return m_TargetArray; }
                set { m_TargetArray = value; }
            }

            public TargetArray(UnityEngine.Object[] targets)
            {
                m_TargetArray = targets.Cast<T>().ToArray();
            }

            public void ExecuteAction(Action<T> executeAction)
            {
                for (int i = 0; i < m_TargetArray.Length; i++)
                {
                    executeAction(m_TargetArray[i]);
                }
            }
        }

        private bool m_ShowColors;
        private bool m_ShowComponents;
        private bool m_ShowExternalProperties;
        private bool m_ShowCustom1;
        private bool m_ShowCustom2;

        private string m_ColorsPrefKey;
        private string m_ComponentsPrefKey;
        private string m_ExternalPropertiesPrefKey;
        private string m_Custom1PrefKey;
        private string m_Custom2PrefKey;

        private AnimBool m_ShowColorsAnimBool;
        private AnimBool m_ShowComponentsAnimBool;
        private AnimBool m_ShowExternalPropertiesAnimBool;
        private AnimBool m_ShowCustom1AnimBool;
        private AnimBool m_ShowCustom2AnimBool;

        protected void OnBaseEnable()
        {
            string prefKey = GetType().Name;

            m_ColorsPrefKey = prefKey + "_Show_Colors";
            m_ComponentsPrefKey = prefKey + "_Show_Components";
            m_ExternalPropertiesPrefKey = prefKey + "_Show_External_Properties";
            m_Custom1PrefKey = prefKey + "_Custom_1";
            m_Custom2PrefKey = prefKey + "_Custom_1";

            m_ShowColors = EditorPrefs.GetBool(m_ColorsPrefKey, true);
            m_ShowComponents = EditorPrefs.GetBool(m_ComponentsPrefKey, false);
            m_ShowExternalProperties = EditorPrefs.GetBool(m_ExternalPropertiesPrefKey, true);
            m_ShowCustom1 = EditorPrefs.GetBool(m_Custom1PrefKey, false);
            m_ShowCustom2 = EditorPrefs.GetBool(m_Custom2PrefKey, false);

            m_ShowColorsAnimBool = new AnimBool { value = m_ShowColors };
            m_ShowColorsAnimBool.valueChanged.AddListener(Repaint);
            m_ShowComponentsAnimBool = new AnimBool { value = m_ShowComponents };
            m_ShowComponentsAnimBool.valueChanged.AddListener(Repaint);
            m_ShowExternalPropertiesAnimBool = new AnimBool { value = m_ShowExternalProperties };
            m_ShowExternalPropertiesAnimBool.valueChanged.AddListener(Repaint);
            m_ShowCustom1AnimBool = new AnimBool { value = m_ShowCustom1 };
            m_ShowCustom1AnimBool.valueChanged.AddListener(Repaint);
            m_ShowCustom2AnimBool = new AnimBool { value = m_ShowCustom2 };
            m_ShowCustom2AnimBool.valueChanged.AddListener(Repaint);
        }

        protected void OnBaseDisable()
        {
            if (m_ShowComponentsAnimBool != null)
            {
                m_ShowComponentsAnimBool.valueChanged.RemoveListener(Repaint);
            }
            if (m_ShowColorsAnimBool != null)
            {
                m_ShowColorsAnimBool.valueChanged.RemoveListener(Repaint);
            }
            if (m_ShowExternalPropertiesAnimBool != null)
            {
                m_ShowExternalPropertiesAnimBool.valueChanged.RemoveListener(Repaint);
            }
            if (m_ShowCustom1AnimBool != null)
            {
                m_ShowCustom1AnimBool.valueChanged.RemoveListener(Repaint);
            }
            if (m_ShowCustom2AnimBool != null)
            {
                m_ShowCustom2AnimBool.valueChanged.RemoveListener(Repaint);
            }
        }

        protected void DrawFoldoutColors(Action drawSection)
        {
            EditorGUI.BeginChangeCheck();
            m_ShowColors = EditorGUILayout.Foldout(m_ShowColors, "Colors");
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(m_ColorsPrefKey, m_ShowColors);
            }

            m_ShowColorsAnimBool.target = m_ShowColors;

            if (EditorGUILayout.BeginFadeGroup(m_ShowColorsAnimBool.faded))
            {
                drawSection();
            }
            EditorGUILayout.EndFadeGroup();
        }

        protected void DrawFoldoutComponents(Action drawSection)
        {
            EditorGUI.BeginChangeCheck();
            m_ShowComponents = EditorGUILayout.Foldout(m_ShowComponents, "Components");
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(m_ComponentsPrefKey, m_ShowComponents);
            }

            m_ShowComponentsAnimBool.target = m_ShowComponents;

            if (EditorGUILayout.BeginFadeGroup(m_ShowComponentsAnimBool.faded))
            {
                drawSection();
            }
            EditorGUILayout.EndFadeGroup();
        }

        protected void DrawFoldoutExternalProperties(Func<bool> drawSection)
        {
            EditorGUI.BeginChangeCheck();
            m_ShowExternalProperties = EditorGUILayout.Foldout(m_ShowExternalProperties, "External Properties");
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(m_ExternalPropertiesPrefKey, m_ShowExternalProperties);
            }

            m_ShowExternalPropertiesAnimBool.target = m_ShowExternalProperties;

            if (EditorGUILayout.BeginFadeGroup(m_ShowExternalPropertiesAnimBool.faded))
            {
                if (!drawSection.Invoke())
                {
                    EditorGUILayout.LabelField("No External Properties Editable");
                }
            }
            EditorGUILayout.EndFadeGroup();
        }

        protected void DrawFoldoutCustom1(string label, Func<bool> drawSection)
        {
            EditorGUI.BeginChangeCheck();
            m_ShowCustom1 = EditorGUILayout.Foldout(m_ShowCustom1, label);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(m_Custom1PrefKey, m_ShowCustom1);
            }

            m_ShowCustom1AnimBool.target = m_ShowCustom1;

            if (EditorGUILayout.BeginFadeGroup(m_ShowCustom1AnimBool.faded))
            {
                if (!drawSection.Invoke())
                {
                    EditorGUILayout.LabelField("No " + label + " Editable");
                }
            }
            EditorGUILayout.EndFadeGroup();
        }

        protected void DrawFoldoutCustom2(string label, Func<bool> drawSection)
        {
            EditorGUI.BeginChangeCheck();
            m_ShowCustom2 = EditorGUILayout.Foldout(m_ShowCustom2, label);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(m_Custom2PrefKey, m_ShowCustom2);
            }

            m_ShowCustom2AnimBool.target = m_ShowCustom2;

            if (EditorGUILayout.BeginFadeGroup(m_ShowCustom2AnimBool.faded))
            {
                if (!drawSection.Invoke())
                {
                    EditorGUILayout.LabelField("No " + label + " Editable");
                }
            }
            EditorGUILayout.EndFadeGroup();
        }


    }
}