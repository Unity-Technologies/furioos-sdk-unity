using Furioos.ConnectionKit;
using UnityEngine;
using System;
using System.Threading;
using Unity.RenderStreaming;
using Unity.RenderStreaming.Signaling;
using Unity.WebRTC;


namespace Furioos.RenderStreamingBridge {

    public class FurioosBridge : ISignaling {

        //----------------------------------------------------------------------------------------------------------------------------------
        //Public
        //----------------------------------------------------------------------------------------------------------------------------------

        public event OnStartHandler OnStart;
        public event OnConnectHandler OnCreateConnection;
        public event OnDisconnectHandler OnDestroyConnection;
        public event OnOfferHandler OnOffer;
        public event OnAnswerHandler OnAnswer;
        public event OnIceCandidateHandler OnIceCandidate;


        public FurioosBridge(string url, float timeout, SynchronizationContext mainThreadContext) {
            this.mainThreadContext = mainThreadContext;
        }

        public void Start() {

            this.fsConnection = FsClientSideConnectionHandler.getSharedFsConnectionHandler();
            this.fsConnection.OnFurioosRealmMessageReceived("webrtc").Event += this.processWebRTCMessage;
            this.fsConnection.IfOrOnPeerSignedIn(this.onSignedIn);

        }

        public void Stop() {

            this.fsConnection.OnFurioosRealmMessageReceived("webrtc").Event -= this.processWebRTCMessage;
        }


        public void OpenConnection(string connectionId) {
            //Debug.Log("FurioosBridge : OpenConnection " + connectionId);
        }

        
        public void CloseConnection(string connectionId) {
            //Debug.Log("FurioosBridge : CloseConnection " + connectionId);
        }

       

        public void SendAnswer(string connectionId, RTCSessionDescription answer) {

            FsJsonMessage answerMessage = new FsJsonMessage(FsMessageMType.RESPONSE, "webrtc", "connect");
            answerMessage.setDataValue("type", "answer");
            answerMessage.setDataValue("sdp", answer.sdp);
            answerMessage.setRecipient(connectionId);

            this.fsConnection.send(answerMessage);
        }

        public void SendCandidate(string connectionId, RTCIceCandidate candidate) {

            FurioosRoutedMessage<CandidateData> routedMessage = new FurioosRoutedMessage<CandidateData>();
            routedMessage.to = connectionId;

            FsJsonMessage candidateMessage = new FsJsonMessage(FsMessageMType.EVENT, "webrtc", "candidate");
            candidateMessage.setDataValue("candidate", candidate.Candidate);
            candidateMessage.setDataValue("sdpMLineIndex", candidate.SdpMLineIndex.GetValueOrDefault(0));
            candidateMessage.setDataValue("sdpMid", candidate.SdpMid);
            candidateMessage.setRecipient(connectionId);

            this.fsConnection.send(candidateMessage);
        }

        public void SendOffer(string connectionId, RTCSessionDescription offer) {
            //Debug.Log("FurioosBridge : SendOffer");
        }


        //----------------------------------------------------------------------------------------------------------------------------------
        //Private
        //----------------------------------------------------------------------------------------------------------------------------------

        private void onSignedIn(FsPeer peer) {

            //Debug.Log("FurioosBridge : Signed in");

            FsJsonMessage enableMessage = new FsJsonMessage(FsMessageMType.REQUEST, "app-com", "enableStreaming");
            enableMessage.setDataValue("streamName", "RenderStreaming Stream");
            enableMessage.setDataValue("streamType", "RenderStreaming");
            enableMessage.setDataValue("streamProtocols", new string[] { "WebRTC" });
            enableMessage.setDataValue("controlsTypes", new string[] { "RenderStreaming" });
            enableMessage.setDataValue("allowQuality", false);

            this.fsConnection.send(enableMessage);
        }

        private void processWebRTCMessage(FsJsonMessage message) {

            if (message.matchTask("connect", FsMessageMType.REQUEST)) {

                DescData offer = new DescData();

                if (message.tryGetDataValue("sdp", ref offer.sdp)) {

                    offer.connectionId = message.getSender();
                    this.mainThreadContext.Post(d => OnOffer?.Invoke(this, offer), null);

                }
            } else if (message.matchTask("candidate", FsMessageMType.EVENT)) {

                CandidateData candidate = new CandidateData();

                candidate.connectionId = message.getSender();
                message.tryGetDataValue("candidate", ref candidate.candidate);
                message.tryGetDataValue("sdpMLineIndex", ref candidate.sdpMLineIndex);
                message.tryGetDataValue("sdpMid", ref candidate.sdpMid);

                this.mainThreadContext.Post(d => OnIceCandidate?.Invoke(this, candidate), null);
            }
        }


        private FsClientSideConnectionHandler fsConnection = null;
        private readonly SynchronizationContext mainThreadContext = null;


        private string _url;
        public string Url 
        {
            get => _url;
            set => _url = value;
        }

        private float _interval;
        public float Interval
        {
            get => _interval;
            set => _interval = value;
        }

    }
}
