// Subscription Collection
namespace FurioosSDK.Editor {
	public class SubscriptionCollection {
		public string _id;
		public string planID;
		public string quotaID;
		public Permission permissions;
		public bool active;
	}

	public struct Permission {
		public bool allowUpload;
	}
}