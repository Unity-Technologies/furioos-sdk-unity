// Quota collection
namespace FurioosSDK.Editor {
	public class QuotaCollection {
		public string _id;
		public float storage;
		public Extra extra;
		public Usage usage;
	}

	public struct Extra {
		public float storage;
	}

	public struct Usage {
		public float storage;
	}
}