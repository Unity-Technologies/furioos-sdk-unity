using UnityEngine ;
using UnityEditor ;

public class stringWrapper : OBSWrapper 
{
	protected override bool GenerateHeaderWrap (string name, System.Type type, object input, out object output)
	{
		output = EditorGUILayout.TextField(name, input == null ? "" : (string) input) ;
		
		return input == null ? output != null : !input.Equals(output) ;
	}
	
	public override string Serialize (System.Type type, object data) { return (string) data ; }
	public override object Parse (System.Type type, string sdata) 	 { return sdata 		; }		
}

