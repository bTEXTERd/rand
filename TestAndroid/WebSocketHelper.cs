using System;
using System.Threading;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Util;
using Java.Lang;
using WebSocket4Net;
using Thread = System.Threading.Thread;

namespace TestAndroid
{
    public class WebSocketHelper 
    {
        private readonly WebSocket _webSocket;
        private string _incomingMessage;
        private readonly AutoResetEvent _messageReceivedEvent = new AutoResetEvent(false);
        private RealTimeChart realTimeChart;
        //private Websockets.IWebSocketConnection connection;

        public WebSocketHelper(RealTimeChart realTimeChart, string url)
        {
            _webSocket = new WebSocket(url);
            this.realTimeChart = realTimeChart;
            _webSocket.Opened += Opened;
            _webSocket.MessageReceived += MessageReceived;
            _webSocket.Closed += new EventHandler(WebsocketClosed); ;
            _webSocket.Open();
        }

        private void Opened(object sender, EventArgs e)
        {
            Snackbar.Make(realTimeChart.View, "Connected", Snackbar.LengthLong)
                    .Show();
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

        private void ShowConnectedStatusTask()
        {
            Snackbar.Make(realTimeChart.View, "Connected", Snackbar.LengthLong)
                    .Show();
        }
    }
}

