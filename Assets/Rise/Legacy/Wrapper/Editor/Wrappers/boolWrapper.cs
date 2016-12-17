using UnityEngine ;
using UnityEditor ;

public class boolWrapper : OBSWrapper 
{
	protected override bool GenerateHeaderWrap (string name, System.Type type, object input, out object output)
	{
		output = EditorGUILayout.Toggle(name, input == null ? false : (bool) input) ;
		
		return input == null ? output != null : !input.Equals(output) ;
	}
	
	public override string Serialize (System.Type type, object data)
	{
		return "" + data;
	}
	
	public override object Parse (System.Type type, string sdata)
	{
		return bool.Parse(sdata) ;
	}		
}
