using System;
using System.Threading;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Widget;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace TestAndroid
{
    public class WebSocketHelper
    {
        private readonly WebSocket _webSocket;
        private string _incomingMessage;
        private readonly AutoResetEvent _messageReceivedEvent = new AutoResetEvent(false);
        private readonly RealTimeChart _realTimeChart;

        public WebSocketHelper(RealTimeChart realTimeChart, string url)
        {
            _webSocket = new WebSocket(url);
            this._realTimeChart = realTimeChart;
            _webSocket.Opened += Opened;
            _webSocket.Error += Error;
            _webSocket.MessageReceived += MessageReceived;
            _webSocket.Closed += WebsocketClosed;
            ;
            _webSocket.Open();
        }

        private void Error(object sender, EventArgs e)
        {
            _realTimeChart.Toast.SetText("Connection Error");
            _realTimeChart.Toast.Show();
        }

        private void Opened(object sender, EventArgs e)
        {
            _realTimeChart.Toast.Show();
        }

        public string Send(string message)
        {
            if (!message.Equals("STOP"))
            {
                _webSocket.Send(message);
                _messageReceivedEvent.WaitOne();
                return _incomingMessage;
            }
            _webSocket.Send(message);
            return "";
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            _incomingMessage = e.Message;
            _messageReceivedEvent.Set();
            _realTimeChart.Message = _incomingMessage;
            _realTimeChart.Activity.RunOnUiThread(() => _realTimeChart.AddEntry());
        }

        private void WebsocketClosed(object sender, EventArgs e)
        {
            _webSocket.Close();
        }

        private void ShowConnectedStatusTask()
        {
            /*Snackbar.Make(realTimeChart.View, "Connected", Snackbar.LengthLong)
                    .Show();*/
            //Toast.MakeText(Application.Context, "Connected", ToastLength.Long);
        }
    }
}