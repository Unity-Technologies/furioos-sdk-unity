using System;
using System.Collections;
using System.Collections.Generic;

namespace Rise.SDK.Importer {
	[Serializable]
	public class RSImporterMaterial {
		public int Id;
		public string Type;
		public string Path;
		public Dictionary<string, string> Parameters;
	}
}