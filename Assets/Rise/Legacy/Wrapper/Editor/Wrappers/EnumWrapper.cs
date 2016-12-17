using UnityEngine ;
using UnityEditor ;

public class EnumWrapper : OBSWrapper 
{
	protected override bool GenerateHeaderWrap (string name, System.Type type, object input, out object output)
	{
		output = EditorGUILayout.EnumPopup(name, ((System.Enum) (input == null ? System.Enum.GetValues(type).GetValue(0) : input))) ;

		return input == null ? output != null : !input.Equals(output) ;
	}
	
	public override string Serialize (System.Type type, object data)
	{
		return "" + data;
	}
	
	public override object Parse (System.Type type, string sdata)
	{
		return System.Enum.Parse(type, sdata) ;
	}
}
