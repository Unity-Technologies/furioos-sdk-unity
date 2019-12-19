// Organization Collection
namespace FurioosSDK.Editor {
	public class OrganizationCollection {
		public string _id;
		public string name;
		public string subscriptionID;
		public Storage storage;
		public string avatarUrl;
		public string coverUrl;
	}

	public struct Storage {
		public string build;
		public string regionID;
		public string storageID;
		public string fileShare;
	}
}