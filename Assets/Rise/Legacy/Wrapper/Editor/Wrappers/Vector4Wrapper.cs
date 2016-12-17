using UnityEngine ;
using UnityEditor ;

public class Vector4Wrapper : OBSWrapper 
{
	protected override bool GenerateHeaderWrap (string name, System.Type type, object input, out object output)
	{
		output = EditorGUILayout.Vector2Field(name, input == null ? new Vector4(0, 0, 0, 0) : (Vector4) input) ;
		
		return input == null ? output != null : !input.Equals(output) ;
	}
	
	public override string Serialize (System.Type type, object data)
	{
		Vector4 cdata = (Vector4) data ;
		
		return "" + cdata.x + "," + cdata.y + "," + cdata.z + "," + cdata.w ;
	}
	
	public override object Parse (System.Type type, string sdata)
	{
		string[] sdatas = sdata.Split(',') ;
		
		return new Vector4(float.Parse(sdatas[0]), float.Parse(sdatas[1]), float.Parse(sdatas[2]), float.Parse(sdatas[3])) ;
	}			
}