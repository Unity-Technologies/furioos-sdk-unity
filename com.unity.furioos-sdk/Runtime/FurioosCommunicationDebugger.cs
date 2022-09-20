using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Furioos.ConnectionKit;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

#if UNITY_EDITOR
public class FurioosCommunicationDebugger : MonoBehaviour
{

	private WebSocketServer wsServer;
	private TcpListener tcpListener;
	private Thread tcpListenerThread;
	private TcpClient connectedTcpClient;
	private bool isTcpRunning = false;
	private bool isWsRunning = false;

	public static FurioosCommunicationDebugger _instance;
	public bool runServer = true;
	public bool debugLog = false;

	void OnEnable()
	{
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this);
		}
	}

    public void StartServerWs()
    {

		if (runServer)
		{
			if (wsServer == null)
			{
				wsServer = new WebSocketServer(8081);
				wsServer.AddWebSocketService<FurioosStreamingWss>("/");
			}
			wsServer.Start();
			if (debugLog) Debug.Log("WebSocket Server started");
			FurioosCommunicationDebugger._instance.isWsRunning = true;
		}

	}
	public void StartTcpServerSocket()
    {
		if (runServer)
		{
			if (tcpListenerThread == null)
			{
				tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
				tcpListenerThread.IsBackground = true;
			}
			tcpListenerThread.Start();
			FurioosCommunicationDebugger._instance.isTcpRunning = true;
			if (debugLog) Debug.Log("Tcp socket Server started");
		}
	}

    public void StopServers() 
	{
		FurioosCommunicationDebugger._instance.isTcpRunning = false;
		FurioosCommunicationDebugger._instance.isWsRunning = false;
		if(tcpListener != null)
			tcpListenerThread.Abort();
		if(wsServer != null)
			wsServer.Stop();
    }

    private void sendSignin()
    {
		FsJsonMessage message = new FsJsonMessage(FsMessageMType.RESPONSE, "session", "signIn");
		FsSession session = new FsSession();
		session.peerId = "d7k2rb1Z1Ys";
		session.serverName = "Furioos streaming server simulator";
		session.serverVersion = "Debug mode";
		session.sessionId = "jGVqnpidlPw";
		message.setData(JObject.FromObject(session));

		FsSendMessage(message.ToString());
		
		if (FurioosCommunicationDebugger._instance.debugLog) Debug.Log("signin sended");
	}

	private void sendStartSdk()
    {
		FsJsonMessage message = new FsJsonMessage(FsMessageMType.REQUEST, "play-feat", "sdkStart");
		message.setSender("DUWN9T4ulmO");
		message.setRecipient("jGVqnpidlPw");
		message.setStatus(0);
		FsSendMessage(message.ToString());

		if (FurioosCommunicationDebugger._instance.debugLog) Debug.Log("Start SDk sended");
    }

	private void processFurioosMessage(FsJsonMessage message)
	{

		if (message.matchTask("signIn", FsMessageMType.REQUEST))
		{
			sendSignin();
		}

		if (message.matchTask("sdk", FsMessageMType.EVENT))
		{
			var messageClient = new SDKMessage<string>();
			messageClient.data = message.getData().ToString();
			wsServer.WebSocketServices.Broadcast(JsonConvert.SerializeObject(messageClient));
        }

	}

	private void ListenForIncommingRequests()
	{
		try
		{
			tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8083);
			tcpListener.Start();
			if (debugLog) Debug.Log("Server is listening");
			Byte[] bytes = new Byte[8192];
			while (true)
			{
				using (connectedTcpClient = tcpListener.AcceptTcpClient())
				{
					using (NetworkStream stream = connectedTcpClient.GetStream())
					{
						try
						{
							int length;
							while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
							{
								var incommingData = new byte[length];
								Array.Copy(bytes, 0, incommingData, 0, length);
								string clientMessage = Encoding.ASCII.GetString(incommingData);
								if (debugLog) Debug.Log("client message received as: " + clientMessage);
								processMessage(incommingData);
							}
						}
                        catch 
						{
						}
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.LogWarning("SocketException " + socketException.ToString());
		}
	}

	private void processMessage(byte[] incommingData)
	{
		try
		{
			FsMessage message = FsMessage.createFsMessage(incommingData);
			if (message.isJson())
			{
				if (message.isFurioosJson())
				{
					FsJsonMessage jsonMessage = (message as FsJsonMessage);
					processFurioosMessage(jsonMessage);
				}
			}

		}
		catch (Exception e)
		{
			Debug.LogWarning("processMessage failed : \n" + e.Message + "\n\n" + e.StackTrace);
		}
	}

	public void FsSendMessage(string message)
	{
		if (connectedTcpClient == null)
		{
			return;
		}

		try
		{

			NetworkStream stream = connectedTcpClient.GetStream();
			if (stream.CanWrite)
			{
				byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(message);
				stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
				if (FurioosCommunicationDebugger._instance.debugLog) Debug.Log("Server sent :" + message);
			}
		}
		catch (SocketException socketException)
		{
			Debug.LogWarning("Socket exception: " + socketException);
		}
	}

	void Update()
	{
		if (runServer)
		{
            if (!FurioosCommunicationDebugger._instance.isTcpRunning)
            {
                StartTcpServerSocket();
            }
            if (!FurioosCommunicationDebugger._instance.isWsRunning)
            {
                StartServerWs();
            }
        }
		else
		{
			if (FurioosCommunicationDebugger._instance.isTcpRunning || FurioosCommunicationDebugger._instance.isWsRunning)
			{
				StopServers();
				if (debugLog) Debug.Log("Servers TCP & WS stopped");
			}
		}
	}
}

public class FurioosStreamingWss : WebSocketBehavior
{

	private void sendStartSdk()
	{
		FsJsonMessage message = new FsJsonMessage(FsMessageMType.REQUEST, "play-feat", "sdkStart");
		message.setSender("DUWN9T4ulmO");
		message.setRecipient("jGVqnpidlPw");
		message.setStatus(0);
		FurioosCommunicationDebugger._instance.FsSendMessage(message.ToString());

		if (FurioosCommunicationDebugger._instance.debugLog) Debug.Log("Start SDk sended");
	}

	protected override void OnOpen()
	{
		sendStartSdk();
	}

	protected override void OnClose(CloseEventArgs e)
	{
		//Debug.Log("Web client Disconnected");
	}

    protected override void OnMessage(MessageEventArgs e)
    {
        if (FurioosCommunicationDebugger._instance.debugLog) Debug.Log("WS Data server:" + e.Data);
		
		var jsonValue = JsonUtility.FromJson<SDKMessage<JObject>>(e.Data);
		var js = JObject.Parse(e.Data);

		FsJsonMessage sdkMessage = new FsJsonMessage(FsMessageMType.EVENT, "play-feat", "sdk");
		sdkMessage.setRecipient("jGVqnpidlPw");
		sdkMessage.setSender("DUWN9T4ulmO");
		sdkMessage.setData(JObject.Parse(js["data"].ToString()));
		string message = sdkMessage.ToString();
		FurioosCommunicationDebugger._instance.FsSendMessage(message);
	}
}

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

public class FsSession
{
	public string peerId;
	public string serverName;
	public string serverVersion;
	public string sessionId;
}

#endif
