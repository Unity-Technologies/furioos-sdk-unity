using UnityEngine;

public static class StringExtension 
{
    public static string UpperCaseFirst(this string s)
    {
		// Check for empty string.
		if (string.IsNullOrEmpty(s))
		{
		    return string.Empty;
		}
		// Return char and concat substring.
		return char.ToUpper(s[0]) + s.Substring(1);
    }
}
