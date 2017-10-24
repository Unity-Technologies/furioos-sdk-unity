//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    [CustomEditor(typeof(EasyTween))]
    [Serializable]
    public class EasyTweenEditor : Editor
    {
        private EasyTween m_ScriptTarget;
        private string m_TempString;
        private int m_TempTweenIndex;
        private int m_TempSubTweenIndex;

        private string[] m_AllowedValueTypes = { "Single", "Int", "Vector2", "Vector3", "Rect", "Color" };

        void OnEnable()
        {
            m_ScriptTarget = (EasyTween)target;
            EnforceListLengths();
        }

        private void EnforceListLengths()
        {
			if (m_ScriptTarget.tweens == null)
            {
				m_ScriptTarget.tweens = new List<EasyTween.EasyTweenObject>() { new EasyTween.EasyTweenObject() };
            }

			if (m_ScriptTarget.tweens.Count <= 0)
            {
				m_ScriptTarget.tweens.Add(new EasyTween.EasyTweenObject());
            }

			for (int i = 0; i < m_ScriptTarget.tweens.Count; i++)
            {
				if (m_ScriptTarget.tweens[i].subTweens.Count == 0)
                {
					m_ScriptTarget.tweens[i].subTweens.Add(new EasyTween.EasyTweenSubObject());
                }
            }
        }
        private string[] FindComponentStrings(GameObject targetGameObject)
        {
            List<string> componentStrings = new List<string>();
            Component[] components = targetGameObject.GetComponents<Component>();
            foreach (Component component in components)
            {
                componentStrings.Add(component.GetType().Name);
            }

            return componentStrings.ToArray();
        }
        private string[] FindVariableStrings(GameObject targetGameObject, string targetComponent)
        {
            List<string> variableStrings = new List<string>();

            if (targetGameObject)
            {
                if (targetGameObject.GetComponent(targetComponent))
                {
                    Component realComponent = targetGameObject.GetComponent(targetComponent);
                    Type behaviorType = realComponent.GetType();

                    PropertyInfo[] properties = behaviorType.GetProperties();
                    FieldInfo[] fields = behaviorType.GetFields();

                    for (int i = 0; i < properties.Length; i++)
                    {
                        string propertyType = properties[i].PropertyType.Name;

                        for (int j = 0; j < m_AllowedValueTypes.Length; j++)
                        {
                            if (propertyType == m_AllowedValueTypes[j] && properties[i].CanWrite)
                            {
                                variableStrings.Add(properties[i].Name);
                            }
                        }
                    }

                    for (int i = 0; i < fields.Length; i++)
                    {
                        string fieldType = fields[i].FieldType.Name;

                        for (int j = 0; j < m_AllowedValueTypes.Length; j++)
                        {
                            if (fieldType == m_AllowedValueTypes[j] && properties[j].CanWrite)
                            {
                                variableStrings.Add(properties[j].Name);
                            }
                        }
                    }
                }
            }

            return variableStrings.ToArray();
        }

        private string[] FindMethodStrings(GameObject targetGameObject, string targetComponent)
        {
            List<string> methodStrings = new List<string>();

            if (targetGameObject)
            {
                if (targetGameObject.GetComponent(targetComponent))
                {
                    Component realComponent = targetGameObject.GetComponent(targetComponent);
                    Type behaviorType = realComponent.GetType();

                    MethodInfo[] methods = behaviorType.GetMethods();

                    for (int i = 0; i < methods.Length; i++)
                    {
                        for (int j = 0; j < m_AllowedValueTypes.Length; j++)
                        {
                            if (methods[i].ReturnType == typeof(void) && methods[i].GetParameters().Length == 0)
                            {
                                methodStrings.Add(methods[i].Name);
                            }
                        }
                    }
                }
            }

            return methodStrings.ToArray();
        }

        private string StringPopup(string label, string[] strings, string str)
        {
            int strId = Array.IndexOf(strings, str);

            if (strId == -1)
                strId = strings.Length - 1;

            strId = EditorGUILayout.Popup(label, strId, strings, "popup");

            if (strId == -1 || strId == strings.Length - 1)
                return "";

            return strings[strId];
        }

        private GenericMenu VariableMenu(GameObject targetGameObject, EasyTween.EasyTweenObject tweenObject)
        {
            GenericMenu thisMenu = new GenericMenu();
            string[] componentStrings = FindComponentStrings(targetGameObject);

            for (int i = 0; i < componentStrings.Length; i++)
            {
                string[] variableStrings = FindVariableStrings(targetGameObject, componentStrings[i]);

                for (int j = 0; j < variableStrings.Length; j++)
                {
                    thisMenu.AddItem(new GUIContent(componentStrings[i] + "/" + variableStrings[j]), false, VariableMenuCallback, (object)componentStrings[i] + "/" + variableStrings[j]);
                }
            }

            return thisMenu;
        }

        private GenericMenu MethodMenu(EasyTween.EasyTweenObject tweenObject)
        {
            GenericMenu thisMenu = new GenericMenu();
            string[] componentStrings = FindComponentStrings(tweenObject.callbackGameObject);

            for (int i = 0; i < componentStrings.Length; i++)
            {
                string[] methodStrings = FindMethodStrings(tweenObject.callbackGameObject, componentStrings[i]);

                for (int j = 0; j < methodStrings.Length; j++)
                {
                    thisMenu.AddItem(new GUIContent(componentStrings[i] + "/" + methodStrings[j]), false, MethodMenuCallback, (object)componentStrings[i] + "/" + methodStrings[j]);
                }
            }

            return thisMenu;
        }

        public override void OnInspectorGUI()
        {
            EnforceListLengths();

			for (int i = 0; i < m_ScriptTarget.tweens.Count; i++)
            {
				EasyTween.EasyTweenObject tweenObject = m_ScriptTarget.tweens[i];

                ConfigureTweenObject(ref tweenObject, i);
            }
        }

        private void ConfigureTweenObject(ref EasyTween.EasyTweenObject tweenObject, int tweenIndex)
        {
			using (new GUILayout.HorizontalScope())
			{
				tweenObject.optionsVisible = EditorGUILayout.Foldout(tweenObject.optionsVisible, tweenObject.tag);
				if (tweenIndex == 0)
				{
					if (GUILayout.Button("+", GUILayout.Width(CalcWidth("+") + 8f)))
					{
						AddTweener();
					}
				}
				if (m_ScriptTarget.tweens.Count > 1)
				{
					if (GUILayout.Button("-", GUILayout.Width(CalcWidth("-") + 8f)))
					{
						RemoveTweener(tweenIndex);
					}
				}
			}

            if (tweenObject.optionsVisible)
            {
				using (new GUILayout.VerticalScope("Box"))
				{
					tweenObject.tag = EditorGUILayout.TextField("Tag", tweenObject.tag);
					
					using (new GUILayout.HorizontalScope ())
					{
						tweenObject.targetGameObject = (GameObject)EditorGUILayout.ObjectField("Target GameObject", tweenObject.targetGameObject, typeof(GameObject), true);
						
						m_TempString = "This";
						if (GUILayout.Button(m_TempString, GUILayout.Width(CalcWidth(m_TempString) + 8f)))
						{
							tweenObject.targetGameObject = m_ScriptTarget.gameObject;
						}
					}
					
					tweenObject.tweenType = (Tween.TweenType)EditorGUILayout.EnumPopup("Tween type", tweenObject.tweenType);
					
					if (tweenObject.tweenType == Tween.TweenType.Custom)
					{
						tweenObject.customCurve = EditorGUILayout.CurveField(tweenObject.customCurve);
						
						if (tweenObject.customCurve.keys[0].time != 0f || tweenObject.customCurve.keys[tweenObject.customCurve.keys.Length - 1].time != 1f)
						{
							tweenObject.customCurve = new AnimationCurve(new[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });
						}
						
					}
					
					tweenObject.duration = EditorGUILayout.FloatField("Duration", tweenObject.duration);
					
					tweenObject.delay = EditorGUILayout.FloatField("Delay", tweenObject.delay);
					
					tweenObject.tweenOnStart = EditorGUILayout.Toggle("Tween on Start", tweenObject.tweenOnStart);
					
					tweenObject.hasCallback = EditorGUILayout.Toggle("Call method when done", tweenObject.hasCallback);
					
					using (new GUILayout.HorizontalScope())
					{
						if (tweenObject.hasCallback)
						{
							tweenObject.callbackGameObject =
								(GameObject)EditorGUILayout.ObjectField("GameObject", tweenObject.callbackGameObject, typeof(GameObject), true);
						}
					}
					
					if (tweenObject.hasCallback && tweenObject.callbackGameObject != null)
					{
						using (new GUILayout.HorizontalScope())
						{
							if (tweenObject.hasCallback)
							{
								GUILayout.Label("Method");
								if (GUILayout.Button(tweenObject.callbackComponentName + "/" + tweenObject.callbackName, GUI.skin.GetStyle("Popup")))
								{
									m_TempTweenIndex = tweenIndex;
									MethodMenu(tweenObject).ShowAsContext();
									return;
								}
							}
						}
					}
					
					EditorGUILayout.Space();
					
					if (tweenObject.targetGameObject)
					{
						using (new GUILayout.HorizontalScope())
						{
							tweenObject.subOptionsVisible = EditorGUILayout.Foldout(tweenObject.subOptionsVisible,
								tweenObject.tag + " Subtweens");
							
							if (tweenObject.subOptionsVisible)
							{
								if (GUILayout.Button("+", GUILayout.Width(CalcWidth("+") + 8f)))
								{
									AddSubTweener(tweenObject);
								}
							}
						}
						
						if (tweenObject.subOptionsVisible)
						{
							for (int i = 0; i < tweenObject.subTweens.Count; i++)
							{
								EasyTween.EasyTweenSubObject subObject = tweenObject.subTweens[i];
								
								using (new GUILayout.VerticalScope("Box"))
								{
									ConfgureSubObject(ref subObject, tweenIndex, i);
								}
							}
						}
					}
				}
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
        }

        private void ConfgureSubObject(ref EasyTween.EasyTweenSubObject subObject, int tweenIndex, int subIndex)
        {
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Label("Variable");
				
				if (GUILayout.Button(subObject.targetComponentName + "/" + subObject.targetVariableName, GUI.skin.GetStyle("Popup")))
				{
					m_TempTweenIndex = tweenIndex;
					m_TempSubTweenIndex = subIndex;
					VariableMenu(m_ScriptTarget.tweens[m_TempTweenIndex].targetGameObject, m_ScriptTarget.tweens[m_TempTweenIndex]).ShowAsContext();
					return;
				}
				
				if (m_ScriptTarget.tweens[tweenIndex].subTweens.Count > 1)
				{
					if (GUILayout.Button("-", GUILayout.Width(CalcWidth("-") + 8f), GUILayout.Height(18f)))
					{
						RemoveSubTweener(tweenIndex, subIndex);
					}
				}
			}

            if (subObject.targetComponent != null)
            {
                ConfigureTargetValue(ref subObject, tweenIndex, subIndex);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private void ConfigureTargetValue(ref EasyTween.EasyTweenSubObject subObject, int tweenIndex, int subIndex)
        {
            string[] subStrings = { "" };

            if (subObject.variableType == "Int32" || subObject.variableType == "Single")
            {
                subStrings = new[] { "value" };
            }
            if (subObject.variableType == "Vector2")
            {
                subStrings = new[] { "X", "Y" };
            }
            if (subObject.variableType == "Vector3")
            {
                subStrings = new[] { "X", "Y", "Z" };
            }
            if (subObject.variableType == "Vector4")
            {
                subStrings = new[] { "X", "Y", "Z", "W" };
            }
            if (subObject.variableType == "Rect")
            {
                subStrings = new[] { "X", "Y", "Width", "Height" };
            }
            if (subObject.variableType == "Color")
            {
                subStrings = new[] { "R", "G", "B", "A" };
            }

            EditorGUILayout.Space();

			using (new GUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField("Target values", GUILayout.Width(CalcWidth("TargetValues") + 8f));
			}
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Copy from gameObject", GUILayout.Width(CalcWidth("Copy from gameObject") + 8f)))
            {
                object theValue;

                if (subObject.isProperty)
                {
					Component component = m_ScriptTarget.GetComponent<EasyTween>().tweens[tweenIndex].targetGameObject.GetComponent(
						m_ScriptTarget.GetComponent<EasyTween>().tweens[tweenIndex].subTweens[subIndex].targetComponentName);

                    string variableName =
						m_ScriptTarget.GetComponent<EasyTween>().tweens[tweenIndex].subTweens[subIndex].targetVariableName;

                    theValue = component.GetType().GetProperty(variableName).GetValue(component, null);
                }
                else
                {
					Component component = m_ScriptTarget.GetComponent<EasyTween>().tweens[tweenIndex].targetGameObject.GetComponent(
						m_ScriptTarget.GetComponent<EasyTween>().tweens[tweenIndex].subTweens[subIndex].targetComponentName);

                    string variableName =
						m_ScriptTarget.GetComponent<EasyTween>().tweens[tweenIndex].subTweens[subIndex].targetVariableName;

                    theValue = component.GetType().GetField(variableName).GetValue(component);
                }

                subObject.targetValue = ObjectToVector4(subObject.variableType, theValue);
            }

            if (GUILayout.Button("Apply to gameObject", GUILayout.Width(CalcWidth("Apply to gameObject") + 8f)))
            {
                if (subObject.isProperty)
                {
					Component component = m_ScriptTarget.GetComponent<EasyTween>().tweens[tweenIndex].targetGameObject.GetComponent(
						m_ScriptTarget.GetComponent<EasyTween>().tweens[tweenIndex].subTweens[subIndex].targetComponentName);

                    string variableName =
						m_ScriptTarget.GetComponent<EasyTween>().tweens[tweenIndex].subTweens[subIndex].targetVariableName;

                    component.GetType().GetProperty(variableName).SetValue(component, Vector4ToObject(subObject.variableType, subObject.targetValue), null);
                }
                else
                {
					Component component = m_ScriptTarget.GetComponent<EasyTween>().tweens[tweenIndex].targetGameObject.GetComponent(
						m_ScriptTarget.GetComponent<EasyTween>().tweens[tweenIndex].subTweens[subIndex].targetComponentName);

                    string variableName =
						m_ScriptTarget.GetComponent<EasyTween>().tweens[tweenIndex].subTweens[subIndex].targetVariableName;

                    component.GetType().GetField(variableName).SetValue(component, Vector4ToObject(subObject.variableType, subObject.targetValue));
                }
            }

            for (int i = 0; i < subObject.targetValueLength; i++)
            {
				using (new GUILayout.HorizontalScope())
				{
					subObject.modifyParameters[i] = GUILayout.Toggle(subObject.modifyParameters[i], "Tween " + subStrings[i]);
					
					if (subObject.modifyParameters[i])
					{
						subObject.targetValue[i] = EditorGUILayout.FloatField(subObject.targetValue[i]);
					}
				}
            }
        }

        public void VariableMenuCallback(object item)
        {
            string theString = (string)item;

            string[] stringArray = theString.Split('/');

			EasyTween.EasyTweenObject tweenObject = m_ScriptTarget.tweens[m_TempTweenIndex];
			EasyTween.EasyTweenSubObject subObject = m_ScriptTarget.tweens[m_TempTweenIndex].subTweens[m_TempSubTweenIndex];

            subObject.targetComponent = tweenObject.targetGameObject.GetComponent(stringArray[0]);
            subObject.targetComponentName = subObject.targetComponent.GetType().Name;

            subObject.targetFieldInfo = subObject.targetComponent.GetType().GetField(stringArray[1]);
            subObject.targetPropertyInfo = subObject.targetComponent.GetType().GetProperty(stringArray[1]);

            if (subObject.targetFieldInfo != null)
            {
                subObject.isProperty = false;
                subObject.variableType = subObject.targetFieldInfo.GetValue(subObject.targetComponent).GetType().Name;
                subObject.targetVariableName = subObject.targetFieldInfo.Name;
            }
            if (subObject.targetPropertyInfo != null)
            {
                subObject.isProperty = true;
                subObject.variableType = subObject.targetPropertyInfo.GetValue(subObject.targetComponent, null).GetType().Name;
                subObject.targetVariableName = subObject.targetPropertyInfo.Name;
            }

            if (subObject.variableType == "Int32" || subObject.variableType == "Single")
            {
                subObject.targetValueLength = 1;
            }
            if (subObject.variableType == "Vector2")
            {
                subObject.targetValueLength = 2;
            }
            if (subObject.variableType == "Vector3")
            {
                subObject.targetValueLength = 3;
            }
            if (subObject.variableType == "Vector4" || subObject.variableType == "Rect" || subObject.variableType == "Color")
            {
                subObject.targetValueLength = 4;
            }

			m_ScriptTarget.tweens[m_TempTweenIndex].subTweens[m_TempSubTweenIndex] = subObject;
        }

        public void MethodMenuCallback(object item)
        {
            string theString = (string)item;

            string[] stringArray = theString.Split('/');

			EasyTween.EasyTweenObject tweenObject = m_ScriptTarget.tweens[m_TempTweenIndex];

            tweenObject.callbackComponent = tweenObject.callbackGameObject.GetComponent(stringArray[0]);
            tweenObject.callbackComponentName = tweenObject.callbackComponent.GetType().Name;

            tweenObject.callbackMethodInfo = tweenObject.callbackComponent.GetType().GetMethod(stringArray[1]);
			tweenObject.callbackName = tweenObject.callbackMethodInfo.Name;

			m_ScriptTarget.tweens[m_TempTweenIndex] = tweenObject;
        }

        private Vector4 ObjectToVector4(string valueType, object value)
        {
            switch (valueType)
            {
                case "Int32":
                    return new Vector4((int)value, 0f, 0f, 0f);

                case "Single":
                    return new Vector4((float)value, 0f, 0f, 0f);

                case "Vector2":
                    Vector2 tempVector2 = (Vector2)value;
                    return new Vector4(tempVector2.x, tempVector2.y, 0f, 0f);

                case "Vector3":
                    Vector3 tempVector3 = (Vector3)value;
                    return new Vector4(tempVector3.x, tempVector3.y, tempVector3.z, 0f);

                case "Vector4":
                    return (Vector4)value;

                case "Rect":
                    Rect tempRect = (Rect)value;
                    return new Vector4(tempRect.x, tempRect.y, tempRect.width, tempRect.height);

                case "Color":
                    Color tempColor = (Color)value;
                    return new Vector4(tempColor.r, tempColor.g, tempColor.b, tempColor.a);
            }
            return new Vector4();
        }

        private object Vector4ToObject(string valueType, Vector4 currentVector4)
        {
            switch (valueType)
            {
                case "Int32":
                    return Mathf.RoundToInt(currentVector4.x);

                case "Single":
                    return currentVector4.x;

                case "Vector2":
                    return new Vector2(currentVector4.x, currentVector4.y);

                case "Vector3":
                    return new Vector3(currentVector4.x, currentVector4.y, currentVector4.z);

                case "Vector4":
                    return currentVector4;

                case "Rect":
                    return new Rect(currentVector4.x, currentVector4.y, currentVector4.z, currentVector4.w);

                case "Color":
                    return new Color(currentVector4.x, currentVector4.y, currentVector4.z, currentVector4.w);
            }
            return null;
        }

        private float CalcWidth(string theString)
        {
            return GUI.skin.label.CalcSize(new GUIContent(theString)).x;
        }

        private void AddTweener()
        {
			m_ScriptTarget.tweens.Add(new EasyTween.EasyTweenObject());
        }

        private void AddSubTweener(EasyTween.EasyTweenObject tweenObject)
        {
			tweenObject.subTweens.Add(new EasyTween.EasyTweenSubObject());
        }

        private void RemoveTweener(int tweenIndex)
        {
			m_ScriptTarget.tweens.RemoveAt(tweenIndex);
        }

        private void RemoveSubTweener(int tweenIndex, int subIndex)
        {
			m_ScriptTarget.tweens[tweenIndex].subTweens.RemoveAt(subIndex);
        }
    }
}