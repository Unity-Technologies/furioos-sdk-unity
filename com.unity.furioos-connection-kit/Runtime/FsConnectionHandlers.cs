using UnityEngine;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Furioos.ConnectionKit {

    public enum FsConnectionStatus  {

        FS_CONNECTION_STATUS_DISCONNECTED = 0,
        FS_CONNECTION_STATUS_SIGNING_IN = 1,
        FS_CONNECTION_STATUS_SIGNED_IN = 2,
        FS_CONNECTION_STATUS_SIGNING_OUT = 3
    };

    public abstract class FsConnectionHandler {

        //----------------------------------------------------------------------------------------------------------------------------------
        //Public
        //----------------------------------------------------------------------------------------------------------------------------------

        public delegate void OnBinaryMessageReceivedHandler(FsBinaryMessage message);
        public delegate void OnJsonMessageReceivedHandler(FsJsonMessage message);
        public delegate void OnPeerEventHandler(FsPeer peer);

        event OnBinaryMessageReceivedHandler OnBinaryMessageReceived;
        event OnJsonMessageReceivedHandler OnJsonMessageReceived;
        event OnJsonMessageReceivedHandler OnFurioosJsonMessageReceived;

        public event OnPeerEventHandler OnPeerSignedIn;

        public class RealmMessageReceivedWrapper {
            public event OnJsonMessageReceivedHandler Event;
            public void fire(FsJsonMessage message) { if(this.Event!=null)this.Event(message); }
        }


        public RealmMessageReceivedWrapper OnFurioosRealmMessageReceived(string realm) {
            if (!this.realmMessageReceivedEvents.ContainsKey(realm)) {
                this.realmMessageReceivedEvents.Add(realm, new RealmMessageReceivedWrapper());
            }
            return this.realmMessageReceivedEvents[realm];
        }



        ~FsConnectionHandler() {
        }

        public void IfOrOnPeerSignedIn(OnPeerEventHandler callback) {
            if (this.status == FsConnectionStatus.FS_CONNECTION_STATUS_SIGNED_IN) callback(this.peer);
            this.OnPeerSignedIn += callback;
        }

        public abstract void start();
        public abstract void stop();
        public abstract void send(FsMessage message);

        public FsPeer getPeer() { return this.peer; }

        public void setPeer(FsPeer peer) { this.peer = peer; }

        public abstract void sendSignIn();


        public static bool checkFurioosResponse(FsJsonMessage message, string task) {

            if (!message.matchTask(task, FsMessageMType.RESPONSE) && !message.matchTask(task, FsMessageMType.EVENT)) {

                return false;

            } else if (message.getStatus() < 0) {

                Debug.Log(task + " error " + message.getStatus() + ": " + message.getDescription());
                return false;

            } else {

                return true;
            }
        }


        //----------------------------------------------------------------------------------------------------------------------------------
        //Protected
        //----------------------------------------------------------------------------------------------------------------------------------

        protected FsConnectionHandler(string host,int port) {
            this.host = host;
            this.port = port;
            this.OnFurioosRealmMessageReceived("session").Event += this.processSessionMessage;
        }


        protected void processMessage(FsMessage message) {

            try { 

                if (message.isBinary()) {

                    if (this.OnBinaryMessageReceived != null) this.OnBinaryMessageReceived(message as FsBinaryMessage);

		            if (message.isFurioosBinary()) {
			       
		            }

	            } else if (message.isJson()) {


                    if (this.OnJsonMessageReceived != null) this.OnJsonMessageReceived(message as FsJsonMessage);

		            if (message.isFurioosJson()) {

                        if(this.OnFurioosJsonMessageReceived != null)this.OnFurioosJsonMessageReceived(message as FsJsonMessage);

                        string realm = ((FsJsonMessage)message).getRealm();
                        RealmMessageReceivedWrapper ev;
                        if (this.realmMessageReceivedEvents.TryGetValue(realm, out ev)){
                            if(ev != null) ev.fire(message as FsJsonMessage);
                        }

                    }
	            } else {
		            //LOG_FUNC_INFO("WS message from %s has not been processed : %s", this->peer != NULL ? this->peer->getPeerId().toCString() : "unidentified peer", message.toString().toCString());
	            }

            } catch(Exception e) {
                Debug.Log("processMessage failed : \n"+ e.Message + "\n\n" + e.StackTrace);
            }
        }

        protected abstract void processSessionMessage(FsJsonMessage message);

        protected void fireSignedIn(FsPeer peer) { if (this.OnPeerSignedIn != null) this.OnPeerSignedIn(peer); }

        protected String host = "localhost";
        protected Int32 port = 8083;

        protected FsConnectionStatus status = FsConnectionStatus.FS_CONNECTION_STATUS_DISCONNECTED;

        //----------------------------------------------------------------------------------------------------------------------------------
        //Private
        //----------------------------------------------------------------------------------------------------------------------------------

        private Dictionary<string, RealmMessageReceivedWrapper> realmMessageReceivedEvents = new Dictionary<string, RealmMessageReceivedWrapper>();
        private FsPeer peer = null;

}


    public class FsClientSideConnectionHandler : FsConnectionHandler {

        //----------------------------------------------------------------------------------------------------------------------------------
        //Public
        //----------------------------------------------------------------------------------------------------------------------------------

        FsClientSideConnectionHandler(string host, int port) : base(host, port) {

        }

        ~FsClientSideConnectionHandler() {
            this.stop();
        }

        public static FsClientSideConnectionHandler getSharedFsConnectionHandler() {

            if (sharedConnection == null) { 

                sharedConnection = new FsClientSideConnectionHandler("localhost", 8083);

                FsPeer peer = new FsPeer(sharedConnection, Application.productName);

                string[] args = System.Environment.GetCommandLineArgs();
                string peerId = "";

                for (int i = 0; i < args.Length - 1; i++){

                    if (args[i] == "-furioosPeerId"){
                        peerId = args[i + 1];
                        Debug.Log("Peer id found : " + peerId);
                        peer.setPeerId(peerId);
                    }
                }

                sharedConnection.start();
            }

            return sharedConnection;
        }

        // Start is called before the first frame update
        public override void start(){

            if (this.socket == null) {

                try
                {
                    this.socket = new TcpClient(this.host, this.port);
                    this.tcpStream = this.socket.GetStream();
                    this.receiveFromSocket();
                    this.sendSignIn();

                }
                catch(SocketException se) {

                    this.reconnectSocket(se.ToString());
                }
            }
        }

        public override void stop() {

            if (this.socket != null){
                this.socket.Close();
                this.socket = null;
                this.status = FsConnectionStatus.FS_CONNECTION_STATUS_DISCONNECTED;
            }
            //if (this.thread != null) this.thread.Stop();
        }

        public override void send(FsMessage message) {
            this.tcpStream.Write(message.getRawData(), 0, message.getSize());
        }

        public override void sendSignIn() {

            FsPeer peer = this.getPeer();

            if (peer != null && this.status == FsConnectionStatus.FS_CONNECTION_STATUS_DISCONNECTED) {

                FsJsonMessage signInMessage = new FsJsonMessage(FsMessageMType.REQUEST, "session", "signIn");
                signInMessage.setDataValue("peerName", peer.getPeerName());
                if (peer.getPeerId() != "") signInMessage.setDataValue("peerId", peer.getPeerId());
                if (peer.getKey() != "") signInMessage.setDataValue("key", peer.getKey());

                signInMessage.setDataValue("clientVersion", FurioosPackageVersion.Instance != null ? FurioosPackageVersion.Instance.FurioosConnectionKitVersion : "-");
                signInMessage.setDataValue("clientAgent", "Unity " + Application.unityVersion);

                this.status = FsConnectionStatus.FS_CONNECTION_STATUS_SIGNING_IN;

                this.send(signInMessage);
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------
        //Protected
        //----------------------------------------------------------------------------------------------------------------------------------


        protected override void processSessionMessage(FsJsonMessage message) {

            

            if (FsConnectionHandler.checkFurioosResponse(message, "signIn")) {

				FsPeer peer = this.getPeer();
                string peerId = "";
                string sessionId = "";

                if (peer != null && message.tryGetDataValue("peerId", ref peerId) && message.tryGetDataValue("sessionId", ref sessionId)) {

					peer.setPeerId(peerId);
					peer.setSessionId(sessionId);

                    Debug.Log("Peer signed in : " + peer.getPeerName() + " (" + peer.getPeerId() + "), sessionId : " + peer.getSessionId());

                    this.status = FsConnectionStatus.FS_CONNECTION_STATUS_SIGNED_IN;
                    this.fireSignedIn(peer);
				}
			}
	
        }


        //----------------------------------------------------------------------------------------------------------------------------------
        //Private
        //----------------------------------------------------------------------------------------------------------------------------------

       
        
        private void reconnectSocket(String error) {

            Debug.Log("Connection lost :  " + error + " - reconnecting ...");
            this.stop();

            FsPeer peer = this.getPeer();
            if (peer != null) if (peer.getKey() != "") peer.setPeerId("");

            Task.Delay(1000).ContinueWith(t => this.start());
        }

        private void receiveFromSocket() {

            try {

                byte[] buffer = new byte[8192];
                this.socket.Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, onDataReceived, buffer);

            } catch {

                Debug.Log("BeginReceive failed");

            }

        }

        private void onDataReceived(IAsyncResult ar) {

            int dataRead;

            try {
                dataRead = this.socket.Client.EndReceive(ar);
            } catch {
                this.reconnectSocket("reception has stopped");
                
                return;
            }

            if (dataRead == 0) {
                //DropConnection();
                Debug.Log("Nothing to read");
                return;
            }

            byte[] byteData = ar.AsyncState as byte[];
            this.processChunkMessage(byteData, dataRead);

            //Continue reading
            this.receiveFromSocket();
        }


        private void processChunkMessage(byte[] byteData, int size)
        {
 
            byte[] incommingData = new byte[size];
            Array.Copy(byteData, incommingData, size);

            dataBufferSendTo.AddRange(incommingData);
            while (dataBufferSendTo.Count > 0)
            {
                int splitPosition = FsMessage.jsonMessageSplit(dataBufferSendTo.ToArray());
                if (splitPosition == dataBufferSendTo.Count) // Json message is not ending 
                {
                    break; // must read TCP port to get new chunk
                }
                else if (splitPosition < 0) // full buffer is a json message
                {
                    FsMessage message = FsMessage.createFsMessage(dataBufferSendTo.ToArray());
                    this.processMessage(message);

                    dataBufferSendTo.Clear();
                }
                else if (splitPosition > 0 && splitPosition < dataBufferSendTo.Count) // There is probably a Json Message in the buffer
                {
                    byte[] dataMessage = new byte[splitPosition];
                    Array.Copy(dataBufferSendTo.ToArray(), dataMessage, splitPosition);

                    FsMessage message = FsMessage.createFsMessage(dataMessage);
                    this.processMessage(message);
                    dataBufferSendTo.RemoveRange(0, splitPosition);
                }
                else
                {
                    break;
                }
            }
        }

        private List<byte> dataBufferSendTo = new List<byte>();
        private TcpClient socket = null;
        private NetworkStream tcpStream = null;

        private static FsClientSideConnectionHandler sharedConnection = null;

    }
}
