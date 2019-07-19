using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using FurioosSDK.Core;

public class JsonFurioos
{
    public string task;
    public string type;
    public string value;
}

public class Socket : FSBehaviour {
	public Text textField;

    void Start()
	{
		FSSocket.OnData += OnData;
	}

	void OnData(string data, byte[] rawData)
	{
        var json = System.Text.Encoding.UTF8.GetString(rawData);
        JsonFurioos parse = JsonConvert.DeserializeObject<JsonFurioos>(json);
        Debug.Log(parse.task);
        Debug.Log(parse.type);
        Debug.Log(parse.value);

		textField.text = parse.value;
	}
}
