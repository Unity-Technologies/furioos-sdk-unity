using System;
using UnityEngine;
using UnityEngine.UI;
using FurioosSDK.Core;
using WebSocketSharp;

public class TestSocket : MonoBehaviour {
    public Text text;

    void Start() {
		FSSocket.OnOpen += OnOpen;
		FSSocket.OnData += OnData;
		FSSocket.OnClose += OnClose;
		FSSocket.OnError += OnError;
	}

    void OnOpen(EventArgs events) {

	}

	void OnClose(CloseEventArgs events) {

	}

	void OnError(ErrorEventArgs events) {

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
