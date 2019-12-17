// Application Collection
namespace FurioosSDK.Editor {
	public class ApplicationCollection {
		public string _id;
		public string computeID;
		public OrganizationCollection organization;
		public string name;
		public string description;
		public string thumbnailUrl;
		public Binary[] binaries;
		public Parameters parameters;
	}

	public struct Binary {
		public string archive;
		public string folder;
		public string exe;
		public string size;
		public bool uploaded;
	}

	public struct Parameters {
		public int quality;
		public bool mouseLock;
		public bool touchConvert;
	}
}