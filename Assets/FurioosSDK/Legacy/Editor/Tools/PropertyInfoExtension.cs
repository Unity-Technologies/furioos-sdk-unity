using UnityEngine ;
using System.Reflection ;

public static class PropertyInfoExtension 
{
	public static object GetValue(this PropertyInfo property, object target) 				   { return property.GetValue(target, null) ; }
	public static void SetValue(this PropertyInfo property, object target, object valueTarget) { property.SetValue(target, valueTarget, null) ; }	
}
