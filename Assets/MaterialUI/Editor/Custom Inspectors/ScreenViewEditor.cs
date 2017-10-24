//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ScreenView))]
    public class ScreenViewEditor : Editor
    {
        private ScreenView m_ScreenView;

		private SerializedProperty m_MaterialScreens;

		private SerializedProperty m_AutoTrackScreens;
		private SerializedProperty m_OnlyShowSelectedScreen;

        private SerializedProperty m_FadeIn;
        private SerializedProperty m_FadeInTweenType;
        private SerializedProperty m_FadeInAlpha;

        private SerializedProperty m_ScaleIn;
        private SerializedProperty m_ScaleInTweenType;
        private SerializedProperty m_ScaleInScale;

        private SerializedProperty m_SlideIn;
        private SerializedProperty m_SlideInTweenType;
        private SerializedProperty m_SlideInDirection;
        private SerializedProperty m_AutoSlideInAmount;
        private SerializedProperty m_SlideInAmount;
        private SerializedProperty m_SlideInPercent;

        private SerializedProperty m_RippleIn;
        private SerializedProperty m_RippleInTweenType;
        private SerializedProperty m_RippleInType;
        private SerializedProperty m_RippleInPosition;

        private SerializedProperty m_FadeOut;
        private SerializedProperty m_FadeOutTweenType;
        private SerializedProperty m_FadeOutAlpha;

        private SerializedProperty m_ScaleOut;
        private SerializedProperty m_ScaleOutTweenType;
        private SerializedProperty m_ScaleOutScale;

        private SerializedProperty m_SlideOut;
        private SerializedProperty m_SlideOutTweenType;
        private SerializedProperty m_SlideOutDirection;
        private SerializedProperty m_AutoSlideOutAmount;
        private SerializedProperty m_SlideOutAmount;
        private SerializedProperty m_SlideOutPercent;

        private SerializedProperty m_RippleOut;
        private SerializedProperty m_RippleOutTweenType;
        private SerializedProperty m_RippleOutType;
        private SerializedProperty m_RippleOutPosition;

        private SerializedProperty m_TransitionDuration;
        private SerializedProperty m_TransitionType;

        private SerializedProperty m_OnScreenBeginTransition;
        private SerializedProperty m_OnScreenEndTransition;

        void OnEnable()
        {
			m_MaterialScreens = serializedObject.FindProperty("m_MaterialScreens");

            m_AutoTrackScreens = serializedObject.FindProperty("m_AutoTrackScreens");
            m_OnlyShowSelectedScreen = serializedObject.FindProperty("m_OnlyShowSelectedScreen");

            m_FadeIn = serializedObject.FindProperty("m_FadeIn");
            m_FadeInTweenType = serializedObject.FindProperty("m_FadeInTweenType");
            m_FadeInAlpha = serializedObject.FindProperty("m_FadeInAlpha");

            m_ScaleIn = serializedObject.FindProperty("m_ScaleIn");
            m_ScaleInTweenType = serializedObject.FindProperty("m_ScaleInTweenType");
            m_ScaleInScale = serializedObject.FindProperty("m_ScaleInScale");

            m_SlideIn = serializedObject.FindProperty("m_SlideIn");
            m_SlideInTweenType = serializedObject.FindProperty("m_SlideInTweenType");
            m_SlideInDirection = serializedObject.FindProperty("m_SlideInDirection");
            m_AutoSlideInAmount = serializedObject.FindProperty("m_AutoSlideInAmount");
            m_SlideInAmount = serializedObject.FindProperty("m_SlideInAmount");
            m_SlideInPercent = serializedObject.FindProperty("m_SlideInPercent");

            m_RippleIn = serializedObject.FindProperty("m_RippleIn");
            m_RippleInTweenType = serializedObject.FindProperty("m_RippleInTweenType");
            m_RippleInType = serializedObject.FindProperty("m_RippleInType");
            m_RippleInPosition = serializedObject.FindProperty("m_RippleInPosition");

            m_FadeOut = serializedObject.FindProperty("m_FadeOut");
            m_FadeOutTweenType = serializedObject.FindProperty("m_FadeOutTweenType");
            m_FadeOutAlpha = serializedObject.FindProperty("m_FadeOutAlpha");

            m_ScaleOut = serializedObject.FindProperty("m_ScaleOut");
            m_ScaleOutTweenType = serializedObject.FindProperty("m_ScaleOutTweenType");
            m_ScaleOutScale = serializedObject.FindProperty("m_ScaleOutScale");

            m_SlideOut = serializedObject.FindProperty("m_SlideOut");
            m_SlideOutTweenType = serializedObject.FindProperty("m_SlideOutTweenType");
            m_SlideOutDirection = serializedObject.FindProperty("m_SlideOutDirection");
            m_AutoSlideOutAmount = serializedObject.FindProperty("m_AutoSlideOutAmount");
            m_SlideOutAmount = serializedObject.FindProperty("m_SlideOutAmount");
            m_SlideOutPercent = serializedObject.FindProperty("m_SlideOutPercent");

            m_RippleOut = serializedObject.FindProperty("m_RippleOut");
            m_RippleOutTweenType = serializedObject.FindProperty("m_RippleOutTweenType");
            m_RippleOutType = serializedObject.FindProperty("m_RippleOutType");
            m_RippleOutPosition = serializedObject.FindProperty("m_RippleOutPosition");

            m_TransitionDuration = serializedObject.FindProperty("m_TransitionDuration");
            m_TransitionType = serializedObject.FindProperty("m_TransitionType");

            m_OnScreenBeginTransition = serializedObject.FindProperty("m_OnScreenBeginTransition");
            m_OnScreenEndTransition = serializedObject.FindProperty("m_OnScreenEndTransition");
        }

        public override void OnInspectorGUI()
        {
            m_ScreenView = (ScreenView)target;

            serializedObject.Update();

			EditorGUILayout.PropertyField(m_AutoTrackScreens);
            if (!m_AutoTrackScreens.boolValue)
            {
				EditorGUILayout.PropertyField(m_MaterialScreens, true);
            }

            EditorGUILayout.PropertyField(m_OnlyShowSelectedScreen);

            MaterialScreen[] materialScreen = m_ScreenView.materialScreen;

            if (materialScreen.Length > 0)
            {
                string[] names = new string[materialScreen.Length];

                for (int i = 0; i < materialScreen.Length; i++)
                {
                    names[i] = (i + 1) + " - " + materialScreen[i].name;
                }

                m_ScreenView.currentScreen = EditorGUILayout.Popup("Current screen", m_ScreenView.currentScreen, names);

				m_ScreenView.screensDirty = true;

                EditorGUILayout.Space();
            }

            bool controllingAnyScreenConfigs = false;
            for (int i = 0; i < m_ScreenView.materialScreen.Length; i++)
            {
                MaterialScreen t = m_ScreenView.materialScreen[i];
                if (t.optionsControlledByScreenView)
                {
                    controllingAnyScreenConfigs = true;
                    break;
                }
            }
            if (controllingAnyScreenConfigs)
            {
                bool fadeIn = m_FadeIn.boolValue;
                bool scaleIn = m_ScaleIn.boolValue;
                bool slideIn = m_SlideIn.boolValue;
                bool rippleIn = m_RippleIn.boolValue;
                bool fadeOut = m_FadeOut.boolValue;
                bool scaleOut = m_ScaleOut.boolValue;
                bool slideOut = m_SlideOut.boolValue;
                bool rippleOut = m_RippleOut.boolValue;

                if (fadeIn)
                {
					using (new GUILayout.VerticalScope("Box"))
					{
						EditorGUILayout.PropertyField(m_FadeIn);
						EditorGUILayout.PropertyField(m_FadeInTweenType,
							new GUIContent("Tween type"));
						if (m_FadeInTweenType.enumValueIndex == 0)
						{
							m_ScreenView.fadeInCustomCurve =
								Tween.CheckCurve(EditorGUILayout.CurveField(m_ScreenView.fadeInCustomCurve));
						}
						EditorGUILayout.PropertyField(m_FadeInAlpha,
							new GUIContent("Initial Alpha"));
					}
                    EditorGUILayout.Space();
                }
                else
                {
                    EditorGUILayout.PropertyField(m_FadeIn);
                }

                if (scaleIn)
                {
					using (new GUILayout.VerticalScope("Box"))
					{
						EditorGUILayout.PropertyField(m_ScaleIn);
						EditorGUILayout.PropertyField(m_ScaleInTweenType,
							new GUIContent("Tween type"));
						if (m_ScaleInTweenType.enumValueIndex == 0)
						{
							m_ScreenView.scaleInCustomCurve =
								Tween.CheckCurve(EditorGUILayout.CurveField(m_ScreenView.scaleInCustomCurve));
						}
						EditorGUILayout.PropertyField(m_ScaleInScale,
							new GUIContent("Initial Scale"));
					}
                    EditorGUILayout.Space();
                }
                else
                {
                    EditorGUILayout.PropertyField(m_ScaleIn);
                }

                if (slideIn)
                {
					using (new GUILayout.VerticalScope("Box"))
					{
						EditorGUILayout.PropertyField(m_SlideIn);
						EditorGUILayout.PropertyField(m_SlideInTweenType,
							new GUIContent("Tween type"));
						if (m_SlideInTweenType.enumValueIndex == 0)
						{
							m_ScreenView.slideInCustomCurve =
								Tween.CheckCurve(EditorGUILayout.CurveField(m_ScreenView.slideInCustomCurve));
						}
						EditorGUILayout.PropertyField(m_SlideInDirection,
							new GUIContent("From the"));
						EditorGUILayout.PropertyField(m_AutoSlideInAmount,
							new GUIContent("Auto distance"));
						if (m_AutoSlideInAmount.boolValue)
						{
							EditorGUILayout.PropertyField(m_SlideInPercent,
								new GUIContent("Auto distance percent"));
						}
						else
						{
							EditorGUILayout.PropertyField(m_SlideInAmount,
								new GUIContent("Distance"));
						}
					}
                    EditorGUILayout.Space();
                }
                else
                {
                    EditorGUILayout.PropertyField(m_SlideIn);
                }

                if (rippleIn)
                {
					using (new GUILayout.VerticalScope("Box"))
					{
						EditorGUILayout.PropertyField(m_RippleIn);
						EditorGUILayout.PropertyField(m_RippleInTweenType,
							new GUIContent("Tween type"));
						if (m_RippleInTweenType.enumValueIndex == 0)
						{
							m_ScreenView.rippleInCustomCurve =
								Tween.CheckCurve(EditorGUILayout.CurveField(m_ScreenView.rippleInCustomCurve));
						}
						EditorGUILayout.PropertyField(m_RippleInType,
							new GUIContent("Position type"));
						if (m_RippleInType.enumValueIndex == 1)
						{
							EditorGUILayout.PropertyField(m_RippleInPosition,
								new GUIContent("Position"));
						}
					}
                    EditorGUILayout.Space();
                }
                else
                {
                    EditorGUILayout.PropertyField(m_RippleIn);
                }

                EditorGUILayout.Space();

                if (fadeOut)
                {
					using (new GUILayout.VerticalScope("Box"))
					{
						EditorGUILayout.PropertyField(m_FadeOut);
						EditorGUILayout.PropertyField(m_FadeOutTweenType,
							new GUIContent("Tween type"));
						if (m_FadeOutTweenType.enumValueIndex == 0)
						{
							m_ScreenView.fadeOutCustomCurve =
								Tween.CheckCurve(EditorGUILayout.CurveField(m_ScreenView.fadeOutCustomCurve));
						}
						EditorGUILayout.PropertyField(m_FadeOutAlpha,
							new GUIContent("Initial Alpha"));
					}
                    EditorGUILayout.Space();
                }
                else
                {
                    EditorGUILayout.PropertyField(m_FadeOut);
                }

                if (scaleOut)
                {
					using (new GUILayout.VerticalScope("Box"))
					{
						EditorGUILayout.PropertyField(m_ScaleOut);
						EditorGUILayout.PropertyField(m_ScaleOutTweenType,
							new GUIContent("Tween type"));
						if (m_ScaleOutTweenType.enumValueIndex == 0)
						{
							m_ScreenView.scaleOutCustomCurve =
								Tween.CheckCurve(EditorGUILayout.CurveField(m_ScreenView.scaleOutCustomCurve));
						}
						EditorGUILayout.PropertyField(m_ScaleOutScale,
							new GUIContent("Initial Scale"));
					}
                    EditorGUILayout.Space();
                }
                else
                {
                    EditorGUILayout.PropertyField(m_ScaleOut);
                }

                if (slideOut)
                {
					using (new GUILayout.VerticalScope("Box"))
					{
						EditorGUILayout.PropertyField(m_SlideOut);
						EditorGUILayout.PropertyField(m_SlideOutTweenType,
							new GUIContent("Tween type"));
						if (m_SlideOutTweenType.enumValueIndex == 0)
						{
							m_ScreenView.slideOutCustomCurve =
								Tween.CheckCurve(EditorGUILayout.CurveField(m_ScreenView.slideOutCustomCurve));
						}
						EditorGUILayout.PropertyField(m_SlideOutDirection,
							new GUIContent("From the"));
						EditorGUILayout.PropertyField(m_AutoSlideOutAmount,
							new GUIContent("Auto distance"));
						if (m_AutoSlideOutAmount.boolValue)
						{
							EditorGUILayout.PropertyField(m_SlideOutPercent,
								new GUIContent("Auto distance percent"));
						}
						else
						{
							EditorGUILayout.PropertyField(m_SlideOutAmount,
								new GUIContent("Distance"));
						}
					}
                    EditorGUILayout.Space();
                }
                else
                {
                    EditorGUILayout.PropertyField(m_SlideOut);
                }

                if (rippleOut)
                {
					using (new GUILayout.VerticalScope("Box"))
					{
						EditorGUILayout.PropertyField(m_RippleOut);
						EditorGUILayout.PropertyField(m_RippleOutTweenType,
							new GUIContent("Tween type"));
						if (m_RippleOutTweenType.enumValueIndex == 0)
						{
							m_ScreenView.rippleOutCustomCurve =
								Tween.CheckCurve(EditorGUILayout.CurveField(m_ScreenView.rippleOutCustomCurve));
						}
						EditorGUILayout.PropertyField(m_RippleOutType,
							new GUIContent("Position type"));
						if (m_RippleOutType.enumValueIndex == 1)
						{
							EditorGUILayout.PropertyField(m_RippleOutPosition,
								new GUIContent("Position"));
						}
					}
                    EditorGUILayout.Space();
                }
                else
                {
                    EditorGUILayout.PropertyField(m_RippleOut);
                }

                EditorGUILayout.PropertyField(m_TransitionDuration);
                EditorGUILayout.PropertyField(m_TransitionType, new GUIContent("Default transition type"));
            }
            else
            {
                EditorGUILayout.LabelField("Not controlling any MaterialScreen settings", EditorStyles.boldLabel);
            }

            EditorGUILayout.PropertyField(m_OnScreenBeginTransition);
            EditorGUILayout.PropertyField(m_OnScreenEndTransition);

            serializedObject.ApplyModifiedProperties();
        }
    }
}