using Furioos.ConnectionKit;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Furioos.SDK {

    public class FurioosSDK : MonoBehaviour {

        private static FurioosSDK _instance;
        // Start is called before the first frame update

        ConcurrentQueue<FsJsonMessage> messages = new ConcurrentQueue<FsJsonMessage>();
        ConcurrentQueue<string> sessionStarts = new ConcurrentQueue<string>();
        ConcurrentQueue<string> sessionStops = new ConcurrentQueue<string>();

        List<string> startedPeers = new List<string>();

        public delegate void OnSDKMessageHandler(JToken data,string from);
        public event OnSDKMessageHandler OnSDKMessage;

        public delegate void OnSDKSessionHander(string from);
        public event OnSDKSessionHander OnSDKSessionStart;
        public event OnSDKSessionHander OnSDKSessionStop;

#if UNITY_EDITOR
        private FurioosCommunicationDebugger streamingServerSimulator;
#endif


#if UNITY_EDITOR
        private void OnEnable()
        {
         
            var Fsdebugger = this.gameObject.GetComponents<FurioosCommunicationDebugger>();
            var FsSDK = this.gameObject.GetComponents<FurioosSDK>();
            var packageVersion = FurioosPackageVersion.Instance;
            
            if (Fsdebugger != null && Fsdebugger.Length>1)
                Destroy(Fsdebugger[0]);
            if (FsSDK != null && FsSDK.Length>1)
                Destroy(FsSDK[0]);

            this.streamingServerSimulator = gameObject.AddComponent<FurioosCommunicationDebugger>();

        }
#endif

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Debug.LogError("Only one Furioos SDK can be used in your scene");
                Destroy(this);
            }
        }

        void Start() {

            this.fsConnection = FsClientSideConnectionHandler.getSharedFsConnectionHandler();
            this.fsConnection.OnFurioosRealmMessageReceived("play-feat").Event += this.processFurioosMessage;
            this.fsConnection.IfOrOnPeerSignedIn(this.onSignedIn);
            
        }
        // Update is called once per frame
        void Update() {

            FsJsonMessage message = null;
            string peerId;

            while (this.sessionStarts.TryDequeue(out peerId)) {
                if (OnSDKSessionStart != null) OnSDKSessionStart(peerId);
            }
            while (this.messages.TryDequeue(out message)) {
                if (OnSDKMessage != null) OnSDKMessage(message.getData(),message.getSender());
            }
            while (this.sessionStops.TryDequeue(out peerId)) {
                if (OnSDKSessionStop != null) OnSDKSessionStop(peerId);
            }

        }

        public void send(string str,string dstPeerId = "") {
            if (String.IsNullOrEmpty(dstPeerId)) {
                foreach(string peerId in this.startedPeers) {
                    this.send(str, peerId);
                }
            } else {
                FsJsonMessage sdkMessage = new FsJsonMessage(FsMessageMType.EVENT, "play-feat", "sdk");
                sdkMessage.setRecipient(dstPeerId);
                sdkMessage.setData(str);
                this.fsConnection.send(sdkMessage);
            }
        }

        public void send(JObject data,string dstPeerId = "") {
            if (String.IsNullOrEmpty(dstPeerId)) {
                foreach(string peerId in this.startedPeers) {
                    this.send(data, peerId);
                }
            } else {
                FsJsonMessage sdkMessage = new FsJsonMessage(FsMessageMType.EVENT, "play-feat", "sdk");
                sdkMessage.setRecipient(dstPeerId);
                sdkMessage.setData(data);
                this.fsConnection.send(sdkMessage);
            }
        }

        private void processFurioosMessage(FsJsonMessage message) {

            if (message.matchTask("sdk", FsMessageMType.EVENT)) {

                string sender = message.getSender();

                if (this.startedPeers.Contains(sender)) {
                    this.messages.Enqueue(message);
                } else {
                    FsJsonMessage sdkMessage = message.createResponse();
                    sdkMessage.setStatus(-1, " SDK not started");
                    sdkMessage.setRecipient(sender);
                    this.fsConnection.send(sdkMessage);
                }

            }else if(message.matchTask("sdkStart", FsMessageMType.REQUEST)) {

                string sender = message.getSender();
                FsJsonMessage sdkMessage = null;

                if (String.IsNullOrEmpty(sender)) {

                    sdkMessage = message.createResponse();
                    sdkMessage.setStatus(-1, "Invalid peer id");

                } else if (this.startedPeers.Contains(message.getSender())) {

                    sdkMessage = new FsJsonMessage(FsMessageMType.EVENT, "play-feat", "sdkStop");
                    sdkMessage.setRecipient(sender);
                    sdkMessage.setStatus(0, "Restarting SDK session");
                    this.fsConnection.send(sdkMessage);

                    sdkMessage = message.createResponse();
                    sdkMessage.setStatus(0, "SDK session restarted");

                } else { 

                    this.startedPeers.Add(sender);
                    this.sessionStarts.Enqueue(sender);
                    sdkMessage = message.createResponse();
                    sdkMessage.setStatus(0);
                }

                sdkMessage.setRecipient(sender);
                this.fsConnection.send(sdkMessage);

            } else if (message.matchTask("sdkStop", FsMessageMType.REQUEST)) {

                string sender = message.getSender();
                FsJsonMessage sdkMessage = null;

                if (String.IsNullOrEmpty(sender)) {

                    sdkMessage = message.createResponse();
                    sdkMessage.setStatus(-1, "Invalid peer id");

                } else if (!this.startedPeers.Contains(message.getSender())) {

                    sdkMessage = message.createResponse();
                    sdkMessage.setStatus(-2, "Unknown SDK session or already stopped");

                } else {

                    this.startedPeers.Remove(sender);
                    this.sessionStops.Enqueue(sender);
                    sdkMessage = message.createResponse();
                    sdkMessage.setStatus(0);

                }

                sdkMessage.setRecipient(sender);
                this.fsConnection.send(sdkMessage);

            }

            
        }

        private void onSignedIn(FsPeer peer) {

            FsJsonMessage enableMessage = new FsJsonMessage(FsMessageMType.REQUEST, "app-com", "enableSDK");
            enableMessage.setDataValue("sdkVersion", FurioosPackageVersion.Instance!= null ? FurioosPackageVersion.Instance.FurioosSDKVersion : "-");

            this.fsConnection.send(enableMessage);
        }


        private FsClientSideConnectionHandler fsConnection = null;
    }
}
