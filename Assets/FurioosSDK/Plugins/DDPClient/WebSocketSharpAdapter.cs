using System;
using WebSocketSharp;

namespace DdpClient
{
    internal class WebSocketSharpAdapter : WebSocketAdapterBase
    {
        private WebSocket _webSocket;

        public override event EventHandler<EventArgs> Opened;
        public override event EventHandler<EventArgs> Closed;
        public override event EventHandler<Exception> Error;

        private void Initialize(string url)
        {
            _webSocket = new WebSocket(url)
            {
                Log = {Level = LogLevel.Info}
            };
            _webSocket.OnMessage += WebSocketOnMessageReceived;
            _webSocket.OnError += WebSocketOnError;
            _webSocket.OnOpen += WebSocketOnOpened;
            _webSocket.OnClose += WebSocketOnClosed;
        }

        public override void Connect(string url)
        {
            Initialize(url);
            _webSocket.Connect();
        }

        public override void ConnectAsync(string url)
        {
            Initialize(url);
            _webSocket.ConnectAsync();
        }

        protected override void Send(string message) => _webSocket.Send(message);

        public override void Close() => _webSocket.Close();

        public override bool IsAlive() => _webSocket.ReadyState == WebSocketState.Open;

        private void WebSocketOnClosed(object sender, EventArgs e) => Closed?.Invoke(this, e);

        private void WebSocketOnError(object sender, ErrorEventArgs e) => Error?.Invoke(this, e.Exception);

        private void WebSocketOnOpened(object sender, EventArgs e) => Opened?.Invoke(this, e);

        private void WebSocketOnMessageReceived(object sender, MessageEventArgs e) => OnMessageReceived(e.Data);
    }
}