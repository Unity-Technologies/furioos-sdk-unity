using System;
using UnityEngine;
using System.Collections.Generic;
using WebSocketSharp;
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
    public string task = "SDK";
    public T data;
}

namespace FurioosSDK.Core {
    public class FSSocket : FSBehaviour {

        private string _sessionId;
        private string _connectionId;


        static WebSocket ws;
        static List<Action> jobs;
        static readonly int maxJobsPerFrame = 1000;

        static int retryCount = 0;
        static readonly int maxRetry = 10;

        public delegate void OnDataHandler(string data, byte[] rawData);
        public static event OnDataHandler OnData;

        public delegate void OnOpenHander(EventArgs events);
		public static event OnOpenHander OnOpen;

        public delegate void OnCloseHander(CloseEventArgs events);
		public static event OnCloseHander OnClose;

        public delegate void OnErrorHander(ErrorEventArgs events);
		public static event OnErrorHander OnError;

        public void Start() {
            jobs = new List<Action>();

            Connect();
        }

        public void Connect() {
            ws = new WebSocket("ws://localhost:8081");

            ws.OnMessage += (sender, e) => {

                this.WSProcessMessage(sender, e);

                void Handler() {
                    OnData?.Invoke(e.Data, e.RawData);
                }

                QueueJob(Handler);
            };

            ws.OnOpen += (sender, e) => {
                Debug.Log("WS connected.");
                WSSend("{\"type\" :\"signin\",\"peerName\" :\"Unity Test App\"}");
            };

            ws.OnClose += (sender, e) => {
				OnClose(e);

				if (retryCount > maxRetry) {
                    return;
                }

                ws.ConnectAsync();

                retryCount++;
            };

            ws.OnError += (sender, e) => {
				OnError(e);

                if(retryCount > maxRetry) {
                    return;
                }

                ws.ConnectAsync();

                retryCount++;
            };

            ws.ConnectAsync();
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
            Debug.Log($"Receiving message: {content}");

            try {

                var msg = JsonUtility.FromJson<SignalingMessage>(content);

                if (!string.IsNullOrEmpty(msg.type))
                {

                    if (msg.type == "signin")
                    {

                        if (msg.status == "SUCCESS")
                        {

                            this._connectionId = msg.connectionId;
                            this._sessionId = msg.peerId;
                            Debug.Log("Signed in.");

                            Send("SDK data test");

                            //this.WSSend("{\"type\" :\"furioos\",\"task\" : \"ACTIVATE_WEBRTC_ROUTING\",\"appType\" : \"RenderStreaming\",\"appName\" :\"Unity Test App\"}");

                            //OnSignedIn?.Invoke(this);

                        }
                        else
                        {
                            Debug.LogError("Sign-in error : " + msg.message);
                        }


                    } else if (msg.type == "reconnect") {

                        if (msg.status == "SUCCESS")
                        {
                            Debug.Log("Slot reconnected.");

                        }
                        else
                        {
                            Debug.LogError("Reconnect error : " + msg.message);
                        }


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