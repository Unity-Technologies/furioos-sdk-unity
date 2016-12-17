using UnityEngine ;
using UnityEditor ;

public class ColorWrapper : OBSWrapper 
{
	protected override bool GenerateHeaderWrap (string name, System.Type type, object input, out object output)
	{
		output = EditorGUILayout.ColorField(name, input == null ? new Color(0, 0, 0, 0) : (Color) input) ;
		
		return input == null ? output != null : !input.Equals(output) ;
	}
	
	public override string Serialize (System.Type type, object data)
	{
		Color cdata = (Color) data ;
		
		return "" + cdata.r + "," + cdata.g + "," + cdata.b + "," + cdata.a ;
	}
	
	public override object Parse (System.Type type, string sdata)
	{
		string[] sdatas = sdata.Split(',') ;
		
		return new Color(float.Parse(sdatas[0]), float.Parse(sdatas[1]), float.Parse(sdatas[2]), float.Parse(sdatas[3])) ;
	}	
}

