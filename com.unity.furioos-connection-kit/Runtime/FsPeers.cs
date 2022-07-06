using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Furioos.ConnectionKit {
    public class FsPeer {

        //----------------------------------------------------------------------------------------------------------------------------------
        //Public
        //----------------------------------------------------------------------------------------------------------------------------------
        

        public FsPeer(FsConnectionHandler fsConnection,string peerName, bool killOnTimeout = true) {
            this.fsConnection = fsConnection;
            this.fsConnection.setPeer(this);
            this.peerName = peerName;
        }
        

        public bool hasFsConnectionHandler() { return this.fsConnection != null; }
        public FsConnectionHandler getFsConnectionHandler() { return this.fsConnection; }

        public void setFsConnectionHandler(FsConnectionHandler fsConnection) { this.fsConnection = fsConnection; }

        public string getPeerName() { return this.peerName; }
        public string getPeerId() { return this.peerId; }
        public string getSessionId() { return this.sessionId; }
        public string getKey() { return this.key; }

        public void setPeerId(string peerId) { this.peerId = peerId; }
        public void setSessionId(string sessionId) { this.sessionId = sessionId; }
        public void setKey(string key) { this.key = key; }


        public void sendSignIn(){
            if (this.fsConnection == null) return;
            this.fsConnection.sendSignIn();
        }


        //----------------------------------------------------------------------------------------------------------------------------------
        //Private
        //----------------------------------------------------------------------------------------------------------------------------------

        FsConnectionHandler fsConnection = null;
        string peerName = "";
        string peerId = "";
        string key = "";
        string sessionId = "";

    }
}
