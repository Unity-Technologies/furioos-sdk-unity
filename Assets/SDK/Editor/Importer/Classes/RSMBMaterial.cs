using System;
using System.Collections;
using System.Collections.Generic;

namespace Rise.SDK.ModelBuilder {
	[Serializable]
	public class RSMBMaterial {
		public int Id;
		public string Type;
		public string Path;
		public Dictionary<string, string> Parameters;
	}
}