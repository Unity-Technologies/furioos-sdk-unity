using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Text;


[Serializable]
public class SignalingMessage
{
    public string status;
    public string message;
    public string sessionId;
    public string connectionId;
    public string peerId;
    public string sdp;
    public string type;
    public string candidate;
    public string sdpMid;
    public int sdpMLineIndex;
}

[Serializable]
public class SDKMessage<T>
{
    public string type = "furioos";
    public string task = "sdk";
    public T data;
    public string status;
    public string message;
    public string sessionId;
    public string connectionId;
    public string peerId;
}

namespace FurioosSDK.Core {
    public class FSSocket : FSBehaviour {
        public bool debug = false;

        private string _sessionId;
        private string _connectionId;


        static WebSocket ws;
        static WebSocketServer wsServer;
        static List<Action> jobs;
        static readonly int maxJobsPerFrame = 1000;

        static int retryCount = 0;
        static readonly int maxRetry = 500;

        public delegate void OnDataHandler(string data, byte[] rawData);
        public static event OnDataHandler OnData;

        public delegate void OnOpenHander();
		public static event OnOpenHander OnOpen;

        public delegate void OnCloseHander(CloseEventArgs events);
		public static event OnCloseHander OnClose;

        public delegate void OnErrorHander(ErrorEventArgs events);
		public static event OnErrorHander OnError;

        public void Start() {
            jobs = new List<Action>();

			/*
			if(debug) {
                StartServer();
			}
			else {
            }
			*/

            Connect();
        }

        public void Connect() {
            ws = new WebSocket("ws://127.0.0.1:8081");

            ws.OnMessage += (sender, e) => {
                this.WSProcessMessage(sender, e);
            };

            ws.OnOpen += (sender, e) => {
                retryCount = 0;

				if(debug) {
					Debug.Log("WS connected.");
                }

                WSSend("{\"type\" :\"signIn\",\"peerName\" :\"SDK_APP\"}");
            };

            ws.OnClose += async (sender, e) => {
				OnClose(e);

				if (retryCount > maxRetry) {
                    return;
                }

                await Wait(1);

                Reconnect();
            };

            ws.OnError += (sender, e) => {
				OnError(e);
            };

            ws.ConnectAsync();
        }

		public void StartServer() {
            //wsServer = new WebSocketServer("ws://localhost:3001");
            //wsServer.AddWebSocketService<FSSocketDevService>("/dev");
            //wsServer.Start();
            //wsServer.Stop();
        }

		public void Reconnect() {
            retryCount++;

            ws.ConnectAsync();
        }

        private async Task Wait(int time) {
            await Task.Delay(TimeSpan.FromSeconds(time));
        }

        private void Update() {
            if (jobs != null) {
                var jobsExecutedCount = 0;
                while (jobs.Count > 0 && jobsExecutedCount++ < maxJobsPerFrame) {
                    var job = jobs[0];
                    jobs.RemoveAt(0);

                    try {
                        job.Invoke();
                    }
                    catch (System.Exception e) {
                        Debug.Log("Job invoke exception: " + e.Message);
                    }
                }
            }
        }

        private static void QueueJob(System.Action Job) {
            if (jobs == null) {
                jobs = new List<System.Action>();
            }

            jobs.Add(Job);
        }

        public static void Send(string data) {
            SDKMessage<string> message = new SDKMessage<string>();
            message.data = data;
            WSSend(message);
        }

        public static void Send(byte[] rawData) {
            SDKMessage<byte[]> message = new SDKMessage<byte[]>();
            message.data = rawData;
            WSSend(message);
        }

        private void WSProcessMessage(object sender, MessageEventArgs e){

            var content = Encoding.UTF8.GetString(e.RawData);

			if(debug) {
				Debug.Log($"Receiving message: {content}");
            }

            try {

                var msg = JsonUtility.FromJson<SDKMessage<string>>(content);

                if (!string.IsNullOrEmpty(msg.type))
                {

                    if (msg.type == "signIn")
                    {

                        if (msg.status == "SUCCESS")
                        {

                            this._connectionId = msg.connectionId;
                            this._sessionId = msg.peerId;

                            if (debug) {
                                Debug.Log("Signed in.");
                            }

                            void Handler() {
                                OnOpen?.Invoke();
                            }

                            QueueJob(Handler);

                        }
                        else
                        {
                            Debug.LogError("Sign-in error : " + msg.message);
                        }


                    } else if (msg.type == "reconnect") {

                        if (msg.status == "SUCCESS")
                        {
                            if (debug) {
								Debug.Log("Slot reconnected.");
                            }

                        }
                        else
                        {
                            if (debug) {
								Debug.LogError("Reconnect error : " + msg.message);
                            }
                        }


                    }
                    else if(msg.type == "furioos" && msg.task == "sdk") {
                        void Handler() {
                            OnData?.Invoke(msg.data, e.RawData);
                        }

                        QueueJob(Handler);
                    }
                   

                }

            }
            catch (Exception ex)
            {

                Debug.LogError("Failed to parse message: " + ex);
            }
        }


        private static void WSSend(object data)
        {

            if (ws == null || ws.ReadyState != WebSocketState.Open)
            {
                Debug.Log("WS is not connected. Unable to send message");
                return;
            }

            if (data is string)
            {
                Debug.Log("Sending WS data: " + (string)data);
                ws.Send((string)data);
            }
            else
            {
                string str = JsonUtility.ToJson(data);
                Debug.Log("Sending WS data: " + str);
                ws.Send(str);
            }
        }
    }
}