using System;
using UnityEngine;
using System.Collections.Generic;
using WebSocketSharp;

namespace FurioosSDK.Core {
    public class FSSocket : FSBehaviour {
        static WebSocket ws;
        static List<Action> jobs;
        static int maxJobsPerFrame = 1000;

        public delegate void OnDataHandler(string data, byte[] rawData);

        public static event OnDataHandler OnData;

        public void Start() {
            jobs = new List<Action>();

            Connect();
        }

        public void Connect() {
            ws = new WebSocket("ws://localhost:80");

            ws.OnMessage += (sender, e) => {
                Action Handler = () => {
                    OnData?.Invoke(e.Data, e.RawData);
                };

                FSSocket.QueueJob(Handler);
            };

            ws.OnOpen += (sender, e) => {
                Debug.Log("connected");
            };

            ws.OnClose += (sender, e) => {
                Debug.Log(e.Code);
                ws.ConnectAsync();
            };

            ws.OnError += (sender, e) => {
                Debug.Log(e.Message);
                ws.ConnectAsync();
            };

            ws.ConnectAsync();
        }

        public void Update() {
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

        public static void Send(string data)
        {
            ws.Send(data);
        }

        public static void Send(byte[] rawData)
        {
            ws.Send(rawData);
        }
    }
}