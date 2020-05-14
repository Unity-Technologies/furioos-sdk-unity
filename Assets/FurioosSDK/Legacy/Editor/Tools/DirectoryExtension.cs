using UnityEngine ;
using System.IO ;
using System.Collections.Generic ;


public static class DirectoryExtension 
{
	public static string[] GetFilesNames(string path, string endWith = "")
	{
		List<string> paths = new List<string>() ; paths.AddRange(Directory.GetFiles(path)) ;
		List<string> names = new List<string>() ;
		
		for(int i = 0 ; i < paths.Count ; i++)
		{
			if(paths[i].EndsWith(endWith)) { names.Add(paths[i].Substring(path.Length+1)) ; }
		}
		
		return names.ToArray() ;
	}
}
