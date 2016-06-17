using System;
using System.Threading;
//using System.Net.WebSockets;
using WebSocket4Net;

namespace TestAndroid
{
    public class WebSocketHelper 
    {
        private readonly WebSocket _webSocket;
        private string _incomingMessage;
        private readonly AutoResetEvent _messageReceivedEvent = new AutoResetEvent(false);
        private RealTimeChart realTimeChart;
        private Websockets.IWebSocketConnection connection;

        public WebSocketHelper(RealTimeChart realTimeChart, string url)
        {
            _webSocket = new WebSocket(url);
            this.realTimeChart = realTimeChart;
            _webSocket.MessageReceived += MessageReceived;
            _webSocket.Closed += new EventHandler(WebsocketClosed); ;
            _webSocket.Open();
            /*connection = Websockets.WebSocketFactory.Create();
            connection.OnLog += Connection_OnLog;
            connection.OnError += Connection_OnError;
            connection.OnMessage += Connection_OnMessage;
            connection.OnOpened += Connection_OnOpened;

            connection.Open(url);
            if (!connection.IsOpen)
                return;*/
        }

        private void Connection_OnLog(string obj)
        {
            throw new NotImplementedException();
        }

        private void Connection_OnError(string obj)
        {
            throw new NotImplementedException();
        }

        private void Connection_OnOpened()
        {
            throw new NotImplementedException();
        }

        private void Connection_OnMessage(string obj)
        {
            throw new NotImplementedException();
        }

        public string Send(string message)
        {
            _webSocket.Send(message);
            _messageReceivedEvent.WaitOne();
            return _incomingMessage;
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            _incomingMessage = e.Message;
            _messageReceivedEvent.Set();
            realTimeChart.Message = _incomingMessage;
            realTimeChart.AddEntry();
        }

        private void WebsocketClosed(object sender, EventArgs e)
        {
            _webSocket.Close();
        }
    }
}

