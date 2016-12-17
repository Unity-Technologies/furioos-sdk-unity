using UnityEngine ;
using UnityEditor ;

public class Vector3Wrapper : OBSWrapper 
{
	protected override bool GenerateHeaderWrap (string name, System.Type type, object input, out object output)
	{
		output = EditorGUILayout.Vector2Field(name, input == null ? new Vector3(0, 0, 0) : (Vector3) input) ;
		
		return input == null ? output != null : !input.Equals(output) ;
	}
	
	public override string Serialize (System.Type type, object data)
	{
		Vector3 cdata = (Vector3) data ;
		
		return "" + cdata.x + "," + cdata.y + "," + cdata.z ;
	}
	
	public override object Parse (System.Type type, string sdata)
	{
		string[] sdatas = sdata.Split(',') ;
		
		return new Vector3(float.Parse(sdatas[0]), float.Parse(sdatas[1]), float.Parse(sdatas[2])) ;
	}		
}
