using System;
using FurioosSDK.Core;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace FurioosSDK.Core {
    public class TestService : WebSocketBehavior {
        protected override void OnMessage(MessageEventArgs e) {
            Debug.Log(e.Data.ToString());
        }
    }

    public class FSWebSocketServer : FSBehaviour {
        WebSocketServer server;

        public void Start() {
            server = new WebSocketServer(4321);
            server.AddWebSocketService<TestService>("/test");
            server.Start();

            Debug.Log("Server started");
        }
    }
}