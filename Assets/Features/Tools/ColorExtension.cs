using UnityEngine;
using System.Collections;

public static class ColorExtensions 
{
	public static Color Parse(string input) 
	{
		input = input.Replace("RGBA(", ""); 
		input = input.Replace(")", "");
		input = input.Replace(" ", "");

		string[] values = input.Split(',');

		return new Color
		(
  		  (float) double.Parse(values[0]),
		  (float) double.Parse(values[1]),
		  (float) double.Parse(values[2]),
		  (float) double.Parse(values[3])
		);
	}
}
