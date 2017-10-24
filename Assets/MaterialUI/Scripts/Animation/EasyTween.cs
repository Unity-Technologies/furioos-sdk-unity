//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Easy Tween", 100)]
    public class EasyTween : MonoBehaviour, IEventSystemHandler
    {
        [Serializable]
        public class EasyTweenObject
        {
            [SerializeField]
            private string m_Tag;
			public string tag
			{
				get { return m_Tag; }
				set { m_Tag = value; }
			}

            [SerializeField]
            private GameObject m_TargetGameObject;
			public GameObject targetGameObject
			{
				get { return m_TargetGameObject; }
				set { m_TargetGameObject = value; }
			}

            [SerializeField]
            private Tween.TweenType m_TweenType;
			public Tween.TweenType tweenType
			{
				get { return m_TweenType; }
				set { m_TweenType = value; }
			}

            [SerializeField]
            private AnimationCurve m_CustomCurve;
			public AnimationCurve customCurve
			{
				get { return m_CustomCurve; }
				set { m_CustomCurve = value; }
			}

            [SerializeField]
            private float m_Duration;
			public float duration
			{
				get { return m_Duration; }
				set { m_Duration = value; }
			}

            [SerializeField]
            private float m_Delay;
			public float delay
			{
				get { return m_Delay; }
				set { m_Delay = value; }
			}

            [SerializeField]
            private bool m_TweenOnStart;
			public bool tweenOnStart
			{
				get { return m_TweenOnStart; }
				set { m_TweenOnStart = value; }
			}

            [SerializeField]
            private bool m_HasCallback;
			public bool hasCallback
			{
				get { return m_HasCallback; }
				set { m_HasCallback = value; }
			}

			[SerializeField]
			private GameObject m_CallbackGameObject;
			public GameObject callbackGameObject
			{
				get { return m_CallbackGameObject; }
				set { m_CallbackGameObject = value; }
			}

            [SerializeField]
            private Component m_CallbackComponent;
			public Component callbackComponent
			{
				get { return m_CallbackComponent; }
				set { m_CallbackComponent = value; }
			}
			
            [SerializeField]
            private string m_CallbackComponentName;
			public string callbackComponentName
			{
				get { return m_CallbackComponentName; }
				set { m_CallbackComponentName = value; }
			}

			[SerializeField]
			private MethodInfo m_CallbackMethodInfo;
			public MethodInfo callbackMethodInfo
			{
				get { return m_CallbackMethodInfo; }
				set { m_CallbackMethodInfo = value; }
			}

            [SerializeField]
            private string m_CallbackName;
			public string callbackName
			{
				get { return m_CallbackName; }
				set { m_CallbackName = value; }
			}

            [SerializeField]
            private bool m_OptionsVisible;
			public bool optionsVisible
			{
				get { return m_OptionsVisible; }
				set { m_OptionsVisible = value; }
			}

			[SerializeField]
			private bool m_SubOptionsVisible;
			public bool subOptionsVisible
			{
				get { return m_SubOptionsVisible; }
				set { m_SubOptionsVisible = value; }
			}

            [SerializeField]
            private List<EasyTweenSubObject> m_SubTweens;
			public List<EasyTweenSubObject> subTweens
			{
				get { return m_SubTweens; }
				set { m_SubTweens = value; }
			}

            public EasyTweenObject()
            {
                tag = "New Tween";
                tweenType = MaterialUI.Tween.TweenType.EaseOutQuint;
                duration = 1f;
                callbackComponentName = "--NONE--";
                callbackName = "--NONE--";
                optionsVisible = true;
                subOptionsVisible = true;
                customCurve = new AnimationCurve(new[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });
				subTweens = new List<EasyTweenSubObject>() { new EasyTweenSubObject() };
            }
        }

        [Serializable]
        public class EasyTweenSubObject
        {
            public Component targetComponent;
            public string targetComponentName;
            public string targetVariableName;
            public FieldInfo targetFieldInfo;
            public PropertyInfo targetPropertyInfo;
            public bool isProperty;
            public string variableType;
            public Vector4 targetValue;
            public int targetValueLength;
            public bool[] modifyParameters;
            public Material currentMaterial;

            public EasyTweenSubObject()
            {
                modifyParameters = new[] { true, true, true, true };
                targetComponent = null;
                targetComponentName = "--NONE--";
                targetVariableName = "--NONE--";
                targetFieldInfo = null;
                targetPropertyInfo = null;
                isProperty = false;
                variableType = null;
                targetValueLength = 0;
                targetValue = new Vector4();
            }
        }

        [SerializeField]
        private List<EasyTweenObject> m_Tweens;
		public List<EasyTweenObject> tweens
		{
			get { return m_Tweens; }
			set { m_Tweens = value; }
		}

        void Start()
        {
			for (int i = 0; i < tweens.Count; i++)
            {
				if (tweens[i].tweenOnStart)
                {
                    Tween(i);
                }
            }
        }

        public void TweenAll()
        {
			for (int i = 0; i < tweens.Count; i++)
            {
				TweenSet(tweens[i]);
            }
        }

        public void Tween(int index)
        {
			TweenSet(tweens[index]);
        }

        public void Tween(string tag)
        {
			for (int i = 0; i < tweens.Count; i++)
            {
				if (tweens[i].tag == tag)
                {
					TweenSet(tweens[i]);
                }
            }
        }

        private void TweenSet(EasyTweenObject tweenObject)
        {
            for (int i = 0; i < tweenObject.subTweens.Count; i++)
            {
				EasyTweenSubObject subObject = tweenObject.subTweens[i];

                //Action callbackAction = null;

                if (tweenObject.callbackName != "--NONE--" && tweenObject.hasCallback)
                {
                    //callbackAction = (Action)Delegate.CreateDelegate(typeof(Action), tweenObject.callbackComponent, tweenObject.callbackName);
                }

                switch (subObject.variableType)
                {
                    case "Int32":
                        //int tempInt = Mathf.RoundToInt(subObject.targetValue.x);
                        //TweenManager.AutoTween(tweenObject.targetGameObject, subObject.targetComponent, subObject.targetVariableName, tempInt, tweenObject.duration, tweenObject.delay, tweenObject.tweenType, tweenObject.customCurve, subObject.modifyParameters, callbackAction);
                        break;
                    case "Single":
                        //float tempFloat = subObject.targetValue.x;
                        //TweenManager.AutoTween(tweenObject.targetGameObject, subObject.targetComponent, subObject.targetVariableName, tempFloat, tweenObject.duration, tweenObject.delay, tweenObject.tweenType, tweenObject.customCurve, subObject.modifyParameters, callbackAction);
                        break;
                    case "Vector2":
                        //Vector2 tempVector2 = new Vector2(subObject.targetValue.x, subObject.targetValue.y);
                        //TweenManager.AutoTween(tweenObject.targetGameObject, subObject.targetComponent, subObject.targetVariableName, tempVector2, tweenObject.duration, tweenObject.delay, tweenObject.tweenType, tweenObject.customCurve, subObject.modifyParameters, callbackAction);
                        break;
                    case "Vector3":
                        //Vector3 tempVector3 = new Vector3(subObject.targetValue.x, subObject.targetValue.y, subObject.targetValue.z);
                        //TweenManager.AutoTween(tweenObject.targetGameObject, subObject.targetComponent, subObject.targetVariableName, tempVector3, tweenObject.duration, tweenObject.delay, tweenObject.tweenType, tweenObject.customCurve, subObject.modifyParameters, callbackAction);
                        break;
                    case "Vector4":
                        //TweenManager.AutoTween(tweenObject.targetGameObject, subObject.targetComponent, subObject.targetVariableName, subObject.targetValue, tweenObject.duration, tweenObject.delay, tweenObject.tweenType, tweenObject.customCurve, subObject.modifyParameters, callbackAction);
                        break;
                    case "Rect":
                        //Rect tempRect = new Rect(subObject.targetValue.x, subObject.targetValue.y, subObject.targetValue.z, subObject.targetValue.w);
                        //TweenManager.AutoTween(tweenObject.targetGameObject, subObject.targetComponent, subObject.targetVariableName, tempRect, tweenObject.duration, tweenObject.delay, tweenObject.tweenType, tweenObject.customCurve, subObject.modifyParameters, callbackAction);
                        break;
                    case "Color":
                        //Color tempColor = new Color(subObject.targetValue.x, subObject.targetValue.y, subObject.targetValue.z, subObject.targetValue.w);
                        //TweenManager.AutoTween(tweenObject.targetGameObject, subObject.targetComponent, subObject.targetVariableName, tempColor, tweenObject.duration, tweenObject.delay, tweenObject.tweenType, tweenObject.customCurve, subObject.modifyParameters, callbackAction);
                        break;
                }
            }
        }
    }
}