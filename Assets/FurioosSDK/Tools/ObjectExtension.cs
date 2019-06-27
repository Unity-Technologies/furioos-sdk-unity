using UnityEngine ;
using System.Collections.Generic ;

public static class ObjectExtension 
{
	public static void setMetaData(this object holder, string name, object val)
	{
		if(!holders.Contains(holder)) 
		{ 
			holders.Add(holder) ; 
			
			metaDatasNames.Add(new List<string>()) ;
			metaDatas.Add(new List<object>()) ;
		} 
		int holderIndex = holders.IndexOf(holder) ;
		
		if(!metaDatasNames[holderIndex].Contains(name)) 
		{ 
			metaDatasNames[holderIndex].Add(name) ;
			metaDatas[holderIndex].Add(val) ;
		}
		else
		{
			metaDatas[holderIndex][metaDatasNames[holderIndex].IndexOf(name)] = val ;
		}
	}
	
	public static object getMetaData(this object holder, string name)
	{
		if(holders.Contains(holder))
		{
			int holderIndex = holders.IndexOf(holder) ;
			
			if(metaDatasNames[holderIndex].Contains(name)) { return metaDatas[holderIndex][metaDatasNames[holderIndex].IndexOf(name)] ; }
		}
		
		return null ;
	}
	
	public static object getMetaData(this object holder, string name, object defaultReturn)
	{
		object ret = getMetaData(holder, name) ; return ret == null ? defaultReturn : ret ;
	}	
	
	private static List<object> holders = new List<object>() ;
	private static List<List<string>> metaDatasNames = new List<List<string>>() ;
	private static List<List<object>> metaDatas = new List<List<object>>() ;			
}
