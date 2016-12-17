using UnityEngine ;
using UnityEditor ;

public class intWrapper : OBSWrapper 
{
	protected override bool GenerateHeaderWrap (string name, System.Type type, object input, out object output)
	{
		output = EditorGUILayout.IntField(name, input == null ? 0 : (int) input) ;
		
		return input == null ? output != null : !input.Equals(output) ;
	}
	
	public override string Serialize (System.Type type, object data)
	{
		return "" + data;
	}
	
	public override object Parse (System.Type type, string sdata)
	{
		return int.Parse(sdata) ;
	}		
}
