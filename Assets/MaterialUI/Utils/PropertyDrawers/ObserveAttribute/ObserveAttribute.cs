//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MaterialUI
{
	public class ObserveAttribute : PropertyAttribute
	{
	    public string[] callbackNames;

	    public ObserveAttribute(params string[] callbackNames)
	    {
	        this.callbackNames = callbackNames;
	    }
	}

	#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(ObserveAttribute))]
	public class ObserveDrawer : PropertyDrawer
	{
	    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	    {
	        EditorGUI.BeginChangeCheck();
	        EditorGUI.PropertyField(position, property, label);
	        if (EditorGUI.EndChangeCheck())
	        {
	            if (IsMonoBehaviour(property))
	            {

	                MonoBehaviour mono = (MonoBehaviour)property.serializedObject.targetObject;

	                foreach (var callbackName in observeAttribute.callbackNames)
	                {
	                    mono.Invoke(callbackName, 0);
	                }

	            }
	        }
	    }

	    bool IsMonoBehaviour(SerializedProperty property)
	    {
	        return property.serializedObject.targetObject.GetType().IsSubclassOf(typeof(MonoBehaviour));
	    }

	    ObserveAttribute observeAttribute
	    {
	        get
	        {
	            return (ObserveAttribute)attribute;
	        }
	    }
	}
	#endif
}