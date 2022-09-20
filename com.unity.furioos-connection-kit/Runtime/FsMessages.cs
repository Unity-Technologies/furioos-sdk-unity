using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using UnityEngine;


namespace Furioos.ConnectionKit {


	public enum FsMessageType {

		FS_MESSAGE_TYPE_UNKNOWN = 0x0000,
		FS_MESSAGE_TYPE_BINARY = 0x0300,
		FS_MESSAGE_TYPE_JSON = 0x7B22,

	};


	public enum FsMessageMType  {
		REQUEST = 0,
		RESPONSE = 1,
		EVENT = 2
	};


	public abstract class FsMessage {


		/// <summary>
		/// check if there is a valid json in the buffer 
		/// </summary>
		/// <param name="data"></param>
		/// <returns>
		/// return -1 if the buffer is a valid json
		/// else the postion of the error 
		/// </returns>
		public static int jsonMessageSplit(byte[] data)
		{

			try
			{
				string dataStr = System.Text.Encoding.UTF8.GetString(data);
				JObject json = JObject.Parse(dataStr);
				return -1;
			}
			catch (JsonReaderException readerExeption)
			{
				return readerExeption.LinePosition;
			}
			catch (Exception ex)
            {
				Debug.LogError(ex.Message);
				throw ex;
            }
		}

		public static FsMessage createFsMessage(byte[] data) {


			if (data.Length >= 4 && data[0] == 'F' && data[1] == 'S') {

				Debug.Log("Can't create a Furioos binary message");

			} else if (data.Length > 0) {

				try { 

					//dynamic json = JsonConvert.DeserializeObject<ExpandoObject>(System.Text.Encoding.UTF8.GetString(data), new ExpandoObjectConverter());
					JObject json = JObject.Parse(System.Text.Encoding.UTF8.GetString(data));

					FsJsonMessage message = new FsJsonMessage();
					message.setJson(json);
					return message;

				} catch {

					return new FsBinaryMessage(data);
				}

			}

			return null;

		}

		public abstract FsMessageType getType() ;
		public virtual bool isBinary() { return false; }
		public virtual bool isFurioosBinary() { return false; }
		public virtual bool isJson() { return false; }
		public virtual bool isFurioosJson() { return false; }
		public virtual bool isFurioosGamepad() { return false; }
		public virtual int getSize() { return this.rawData.Length; }
		public virtual byte[] getRawData() { return this.rawData; }
		string toString() { return Encoding.UTF8.GetString(this.getRawData()); }


		protected FsMessage(byte[] data) {
			rawData = data;
		}
		protected FsMessage(uint size) {
			rawData = new byte[size];
		}

		protected byte[] rawData;
		
	};




	public class FsJsonMessage : FsMessage {

		public FsJsonMessage() : base(new byte[0]) {}
		public FsJsonMessage(FsMessageMType mtype, string realm, string task) : base(new byte[0]) {

			this.json = new JObject();

			this.json["mtype"] =  (int)mtype;

			this.json["realm"] = realm;

			this.json["task"] = task;


			if (this.json["mtype"].ToObject<int>() == (int)FsMessageMType.RESPONSE || this.json["mtype"].ToObject<int>() == (int)FsMessageMType.EVENT) {
				this.json["status"] = 0;
			}

			this.isFurioos = true;

			this.needUpdate = true;
		
	
		}

		public bool matchRealm(string realm) {
			return (this.isFurioos && this.json["realm"].ToString() == realm);
		}

		public string getRealm() {
			return this.isFurioos ? this.json["realm"].ToString() : "";
		}

		public bool matchTask(string task, FsMessageMType mtype) {
			return (this.isFurioos && this.json["mtype"].ToObject<int>() == (int)mtype && this.json["task"].ToString() == task);
		}

		public string getTask() {
			return this.isFurioos ? this.json["task"].ToString() : "";
		}

		public FsJsonMessage createResponse() {
			if (this.isFurioos && this.json["mtype"].ToObject<int>() == (int)FsMessageMType.REQUEST){
				FsJsonMessage response = new FsJsonMessage(FsMessageMType.RESPONSE, this.getRealm(), this.getTask());
				if (this.json.ContainsKey("from")) response.setRecipient(this.json["from"].ToString());
				return response;
			}
			else return null;
		}

		public void setJson(JObject json) {

			this.json = json;

			if (this.json == null) return;

			int mtype = this.json.ContainsKey("mtype") ?  this.json["mtype"].ToObject<int>() : -1;

			this.data = this.json["data"];
			if (this.data != null && this.data.Type == JTokenType.String) // Try to convert String value to object
            {
				try
				{
					var jsonObject = JObject.Parse(this.data.ToString());
					if (jsonObject.Type == JTokenType.Object)
						this.data = jsonObject;
				}
				catch { } // do nothing
            }

			if (mtype >= (int)FsMessageMType.REQUEST && mtype <= (int)FsMessageMType.EVENT && this.json.ContainsKey("realm") && this.json.ContainsKey("task")) {

				this.isFurioos = true;

				this.json["mtype"] = mtype;

				if (!this.json.ContainsKey("status")) this.json["status"] = 0;

				this.needUpdate = true;

			}

		}

		public void setStatus(int status) {
			if (this.json["mtype"].ToObject<int>() == (int)FsMessageMType.RESPONSE || this.json["mtype"].ToObject<int>() == (int)FsMessageMType.EVENT) {
				this.json["status"] = status;
				this.needUpdate = true;
			}
		}

		public void setStatus(int status, string description) {
			if (this.json["mtype"].ToObject<int>() == (int)FsMessageMType.RESPONSE || this.json["mtype"].ToObject<int>() == (int)FsMessageMType.EVENT) {
				this.json["status"] = status;
				this.json["description"] = description;
				this.needUpdate = true;
			}
		}

		public int getStatus() {
			return this.json["status"].ToObject<int>();
		}

		public void setDescription(string description) {
			if (this.json["mtype"].ToObject<int>() == (int)FsMessageMType.RESPONSE || this.json["mtype"].ToObject<int>() == (int)FsMessageMType.EVENT) {
				this.json["description"] = description;
				this.needUpdate = true;
			}
		}

		public string getDescription() {
			return this.json["description"].ToObject<string>();
		}

		public void setDataValue(string key, string value) {
			this.createDataIfNeeded()[key] = value;
			this.needUpdate = true;
		}

		public void setDataValue(string key, string[] value) {
			this.createDataIfNeeded()[key] = JArray.FromObject(value);
			this.needUpdate = true;
		}

		

		public void setDataValue(string key, int value) {
			this.createDataIfNeeded()[key] = value;
			this.needUpdate = true;
		}

		public void setDataValue(string key, uint value) {
			this.createDataIfNeeded()[key] = value;
			this.needUpdate = true;
		}

		public void setDataValue(string key, long value) {
			this.createDataIfNeeded()[key] = value;
			this.needUpdate = true;
		}

		public void setDataValue(string key, ulong value) {
			this.createDataIfNeeded()[key] = value;
			this.needUpdate = true;
		}

		public void setDataValue(string key, float value) {
			this.createDataIfNeeded()[key] = value;
			this.needUpdate = true;
		}

		public void setDataValue(string key, double value) {
			this.createDataIfNeeded()[key] = value;
			this.needUpdate = true;
		}

		public void setDataValue(string key, bool value) {
			this.createDataIfNeeded()[key] = value;
			this.needUpdate = true;
		}

		/*public void setDataValue(string key, dynamic value) {
			if(!hasProperty(this.json,"data")) this.json.data = new ExpandoObject();
			((IDictionary<string, object>)this.json.data)[key] = value;
			this.needUpdate = true;
		}*/

		public bool tryGetDataValue(string key, ref string outputValue) {
			if (this.data != null && this.data[key] != null && this.data[key].Type == JTokenType.String) {
				outputValue = this.data[key].ToString();
				return true;
			}
			return false;
		}

		public bool tryGetDataValue(string key, ref int outputValue) {
			if (this.data != null && this.data[key] != null && this.data[key].Type == JTokenType.Integer) {
				outputValue = this.data[key].ToObject<int>();
				return true;
			}
			return false;
		}

		public bool tryGetDataValue(string key, ref uint outputValue) {
			if (this.data != null && this.data[key] != null && this.data[key].Type == JTokenType.Integer) {
				outputValue = this.data[key].ToObject<uint>();
				return true;
			}
			return false;
		}

		public bool tryGetDataValue(string key, ref float outputValue) {
			if (this.data != null && this.data[key] != null && this.data[key].Type == JTokenType.Float) {
				outputValue = this.data[key].ToObject<float>();
				return true;
			}
			return false;
		}

		public bool tryGetDataValue(string key, ref double outputValue) {
			if (this.data != null && this.data[key] != null && this.data[key].Type == JTokenType.Float) {
				outputValue = this.data[key].ToObject<double>();
				return true;
			}
			return false;
		}

		public bool tryGetDataValue(string key, ref bool outputValue) {
			if (this.data != null && this.data[key] != null && this.data[key].Type == JTokenType.Boolean) {
				outputValue = this.data[key].ToObject<bool>();
				return true;
			}
			return false;
		}

		public string getStringDataValue(string key, string defValue) {
			if (this.data != null && this.data[key] != null && this.data[key].Type == JTokenType.String) {
				return this.data[key].ToString();
			}
			return defValue;
		}

		public int getIntDataValue(string key, int defValue) {
			if (this.data != null && this.data[key] != null && this.data[key].Type == JTokenType.Integer) {
				return this.data[key].ToObject<int>();
			}	
			return defValue;
		}

		public uint getUnsignedIntDataValue(string key, uint defValue) {
			if (this.data != null && this.data[key] != null && this.data[key].Type == JTokenType.Integer) {
				return this.data[key].ToObject<uint>();
			}
			return defValue;
		}

		public float getFloatDataValue(string key, float defValue) {
			if (this.data != null && this.data[key] != null && this.data[key].Type == JTokenType.Float) {
				return this.data[key].ToObject<float>();
			}	
			return defValue;
		}

		public double getDoubleDataValue(string key, double defValue) {
			if (this.data != null && this.data[key] != null && this.data[key].Type == JTokenType.Float)
			{
				return this.data[key].ToObject<double>();
			}
			return defValue;
		}

		public bool getBooleanDataValue(string key, bool defValue) {
			if (this.data != null && this.data[key] != null && this.data[key].Type == JTokenType.Boolean) {
				return this.data[key].ToObject<bool>();
			}	
			return defValue;
		}

		public void setData(JObject data) {
			this.json["data"] = data;
			this.needUpdate = true;
		}

		public void setData(string data)
		{
			this.json["data"] = data;
			this.needUpdate = true;
		}

		public JToken getData() {
			return this.data;
		}

		public void setRecipient(string dstPeerId) {
            if (dstPeerId != "") {
				this.json["to"] = dstPeerId;
				this.needUpdate = true;
			}
		}

		public string getRecipient() {
			return this.json.ContainsKey("to") ? this.json["to"].ToString() : "";
		}

		public void setSender(string srcPeerId) {
			if (srcPeerId != "") {
				this.json["from"] = srcPeerId;
				this.needUpdate = true;
			}
		}

		public string getSender() {
			return this.json.ContainsKey("from") ? this.json["from"].ToString() : "";
		}

		public override int getSize() {
			if (this.needUpdate)this.updateRawData();
			return this.rawData.Length;
		}

		public override byte[] getRawData() {
			if (this.needUpdate)this.updateRawData();
			return this.rawData;
		}


		//virtual ~FsJsonMessage() { SMG_SAFE_DELETE(this.json); }

		public override FsMessageType getType() { return FsMessageType.FS_MESSAGE_TYPE_JSON; }
		public override bool isJson() { return true; }
		public override bool isFurioosJson() { return this.isFurioos; }

		public override string ToString() {
			if (this.needUpdate) this.updateRawData();
			return this.rawDataString;
		}

		private void updateRawData() {
			this.rawDataString = JsonConvert.SerializeObject(this.json);
			this.rawData = Encoding.UTF8.GetBytes(this.rawDataString);
			this.needUpdate = false;
		}

		private JToken createDataIfNeeded() {
			if (this.data == null) {
				this.data = new JObject();
				JProperty property = new JProperty("data", this.data);
				this.json.Add(property);
				
			}
			return this.data;
		}

		/*public static bool hasProperty(dynamic settings, string name) {
			if (settings is ExpandoObject) return ((IDictionary<string, object>)settings).ContainsKey(name);
			return settings.GetType().GetProperty(name) != null;
		}*/

		private JObject json = null;
		private JToken data = null;
		private string rawDataString;
		private bool needUpdate = false;
		private bool isFurioos = false;

	};


	public class FsBinaryMessage : FsMessage {

		public FsBinaryMessage(byte[] data) : base(data){}
		public FsBinaryMessage(uint size) : base(size) {}

		public override bool isBinary() { return true; }
		public override FsMessageType getType() { return FsMessageType.FS_MESSAGE_TYPE_BINARY; }
		
	}

}