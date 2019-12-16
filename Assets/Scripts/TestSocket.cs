using UnityEngine;
using UnityEngine.UI;
using FurioosSDK.Core;

public class TestSocket : MonoBehaviour {
    public Text text;

    void Start()
	{
		FSSocket.OnData += OnData;
	}

	void OnData(string data, byte[] rawData)
	{
        var value = System.Text.Encoding.UTF8.GetString(rawData);
        text.text = value;
	}

    void Send(string data)
    {
        FSSocket.Send(data);
    }

    void SendRaw(byte[] rawData)
    {
        FSSocket.Send(rawData);
    }
}
