using UnityEngine ;
using UnityEditor ;

public class RectWrapper : OBSWrapper 
{
	protected override bool GenerateHeaderWrap (string name, System.Type type, object input, out object output)
	{
		output = EditorGUILayout.RectField(name, input == null ? new Rect(0, 0, 0, 0) : (Rect) input) ;
		
		return input == null ? output != null : !input.Equals(output) ;
	}
	
	public override string Serialize (System.Type type, object data)
	{
		Rect cdata = (Rect) data ;
		
		return "" + cdata.x + "," + cdata.y + "," + cdata.width + "," + cdata.height ;
	}
	
	public override object Parse (System.Type type, string sdata)
	{
		string[] sdatas = sdata.Split(',') ;
		
		return new Rect(int.Parse(sdatas[0]), int.Parse(sdatas[1]), int.Parse(sdatas[2]), int.Parse(sdatas[3])) ;
	}		
}

