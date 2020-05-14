# Furioos Unity SDK
## Requirements
Minimum requirements: Business subscription (or higher) on Furioos to use the SDK.
Then choose the app you want to use with the SDK and create a SDK link.
**Unity Editor 2019.3+**

## Installation
1. Download the latest version of the Unity package.
2. Import the package in your project
3. Put the prefab **FurioosSDK_Core** in your scene

You'll need to have the **FurioosSDK_Core** prefab in each scene with SDK interactions.

## Exemple
```javascript
using System;

using UnityEngine;
using UnityEngine.UI;

using WebSocketSharp;

using FurioosSDK.Core;

public class ExampleSocket : MonoBehaviour {
    void Start() {
		FSSocket.OnOpen += OnOpen;
		FSSocket.OnData += OnData;
		FSSocket.OnClose += OnClose;
		FSSocket.OnError += OnError;
	}

    void OnOpen() {}

	void OnClose(CloseEventArgs events) {}

	void OnError(ErrorEventArgs events) {}

	void OnData(string data, byte[] rawData) {
        var rawValue = System.Text.Encoding.UTF8.GetString(rawData);

		Debug.Log(rawValue); // JSON value

		Debug.Log(data); // JSON value
	}

    void Send(string data) {
        FSSocket.Send(data);
    }

    void SendRaw(byte[] rawData) {
        FSSocket.Send(rawData);
    }
}
```

## Properties
#### debug: Boolean
Enable the local debug mode. (Not available)

## Events
#### static OnOpen()
Bind an event that will be called when the application is connected to the Furioos streaming server and ready to communicate.

#### OnData(string data, byte[] rawData)
Bind an event that will be called everytime data is received.
- `string data`: The parsed data
- `byte[] rawData`: The raw data. Convert that data to JSON and parsed it by yourself to handle other types of data transfert. (float, int, etc...)

#### OnError(ErrorEventArgs events)
Bind an event that will be called each time an error is fired.
- `ErrorEventArgs events`

#### OnClose(CloseEventArgs events)
Bind an event that will be called when the connection with Furioos streaming server is closed.
- `callback: Function`: Implement your code.

## Methods
#### static Connect()
You can call this function to connect your application to the Furioos Streaming Server.
**As the script is a MonoBehavior script you don't need to call it at start.
Use this function to reconnect when you get an error or if the connection get close.**