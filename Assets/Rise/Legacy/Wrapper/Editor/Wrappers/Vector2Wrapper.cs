using UnityEngine ;
using UnityEditor ;

public class Vector2Wrapper : OBSWrapper 
{
	protected override bool GenerateHeaderWrap (string name, System.Type type, object input, out object output)
	{
		output = EditorGUILayout.Vector2Field(name, input == null ? new Vector2(0, 0) : (Vector2) input) ;
		
		return input == null ? output != null : !input.Equals(output) ;
	}
	
	public override string Serialize (System.Type type, object data)
	{
		Vector2 cdata = (Vector2) data ;
		
		return "" + cdata.x + "," + cdata.y ;
	}
	
	public override object Parse (System.Type type, string sdata)
	{
		string[] sdatas = sdata.Split(',') ;
		
		return new Vector2(float.Parse(sdatas[0]), float.Parse(sdatas[1])) ;
	}			
}