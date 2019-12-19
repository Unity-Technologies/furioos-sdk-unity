// Application Collection
namespace FurioosSDK.Editor {
	public class ApplicationCollection {
		public string _id;
		public string computeTypeID;
		public Organization organization;
		public string name;
		public string description;
		public string thumbnailUrl;
		public Binary[] binaries;
		public Parameters parameters;
		public string[] tags;
	}

	public struct Organization {
		public string _id;
		public string name;
		public string avatarUrl;
	}

	public struct Binary {
		public string archive;
		public string folder;
		public string exe;
		public string[] exeList;
		public float size;
		public bool uploaded;
	}

	public struct Parameters {
		public int defaultQuality;
		public bool mouseLock;
		public bool touchConvert;
	}
}