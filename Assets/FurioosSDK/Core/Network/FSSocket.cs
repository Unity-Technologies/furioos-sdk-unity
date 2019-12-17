using System;
using UnityEngine;
using System.Collections.Generic;
using WebSocketSharp;

namespace FurioosSDK.Core {
    public class FSSocket : FSBehaviour {
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
            ws = new WebSocket("ws://localhost:80");

            ws.OnMessage += (sender, e) => {
                void Handler() {
                    OnData?.Invoke(e.Data, e.RawData);
                }

                QueueJob(Handler);
            };

            ws.OnOpen += (sender, e) => {
				OnOpen(e);
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
            ws.Send(data);
        }

        public static void Send(byte[] rawData) {
            ws.Send(rawData);
        }
    }
}