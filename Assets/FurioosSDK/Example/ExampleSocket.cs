using System;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using FurioosSDK.Core;

public class ExampleSocket : MonoBehaviour {
    public Text text;

    void Start() {
		FSSocket.OnOpen += OnOpen;
		FSSocket.OnData += OnData;
		FSSocket.OnClose += OnClose;
		FSSocket.OnError += OnError;
	}

    void OnOpen() {
		text.text = "connected";
	}

	void OnClose(CloseEventArgs events) {
		Debug.Log("closed");
	}

	void OnError(ErrorEventArgs events) {
		Debug.Log("error");
	}

	void OnData(string data, byte[] rawData) {
        var value = System.Text.Encoding.UTF8.GetString(rawData);
        text.text = value;
	}

    void Send(string data) {
        FSSocket.Send(data);
    }

    void SendRaw(byte[] rawData) {
        FSSocket.Send(rawData);
    }
}
