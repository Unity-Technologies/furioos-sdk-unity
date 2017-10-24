//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;
using System.Linq;

namespace MaterialUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TweenManager))]
    class TweenManagerEditor : Editor
    {
        private TweenManager m_TweenManager;

        void OnEnable()
        {
            m_TweenManager = (TweenManager)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
			EditorGUILayout.LabelField("Total Tweens", m_TweenManager.totalTweenCount.ToString());
			EditorGUILayout.LabelField("Active Tweens", m_TweenManager.activeTweenCount.ToString());
			EditorGUILayout.LabelField("Dormant Tweens", m_TweenManager.dormantTweenCount.ToString());
        }
    }
}