// Quota collection
namespace FurioosSDK.Editor {
	public class QuotaCollection {
		public string _id;
		public double storage;
		public Extra extra;
		public Usage usage;
	}

	public struct Extra {
		public double storage;
	}

	public struct Usage {
		public double storage;
	}
}