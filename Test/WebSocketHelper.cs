using System;

using System.Threading;
using WebSocket4Net;

namespace Test
{
	public class WebSocketHelper
	{
		private WebSocket webSocket;
		private string _incomingMessage;
		private AutoResetEvent _messageReceivedEvent = new AutoResetEvent(false);

		public WebSocketHelper(string url)
		{
			webSocket = new WebSocket(url);
			webSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(webSocket_MessageReceived);
			webSocket.Open();
		}

		public string Send(string message)
		{
			webSocket.Send(message);
			this._messageReceivedEvent.WaitOne();
			return this._incomingMessage; 
		}

		private void webSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			this._incomingMessage = e.Message;
			this._messageReceivedEvent.Set();
		}
	}
}

