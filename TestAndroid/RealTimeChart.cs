using System;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Listener;
using MikePhil.Charting.Util;
using MikePhil.Charting.Highlight;
using String = System.String;
using Thread = Java.Lang.Thread;

namespace TestAndroid
{
    public class RealTimeChart : Fragment, IOnChartValueSelectedListenerSupport
    {
        private LineChart _mChart;
        private String _message;
        private bool addingNumbers;

        private WebSocket _webSocket;
        private WebSocketHelper wSH;
        private ClientWebSocket _webSocketClient;
        private const int MaxMessageSize = 1024;
        private ArraySegment<System.Byte> _receivedDataBuffer;
        private CancellationToken _cancellationToken;

        protected String[] MMonths = new String[] {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec"
        };

        protected String[] MParties = new String[] {
            "Party A", "Party B", "Party C", "Party D", "Party E", "Party F", "Party G", "Party H",
            "Party I", "Party J", "Party K", "Party L", "Party M", "Party N", "Party O", "Party P",
            "Party Q", "Party R", "Party S", "Party T", "Party U", "Party V", "Party W", "Party X",
            "Party Y", "Party Z"
        };

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.ChartFragment, container, false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            wSH = new WebSocketHelper(this, "wss://websockets9127.azurewebsites.net:443/ws.ashx?name=Android");
            //_webSocketClient = new ClientWebSocket();
            _mChart = new LineChart(this.Context);
            RelativeLayout rl = view.FindViewById<RelativeLayout>(Resource.Id.RelativeLayout);

            //_mChart = new LineChart(Application.Context);
            _mChart.SetOnChartValueSelectedListener(this);

            //_webSocket = new WebSocket();
            //await _webSocketClient.ConnectAsync(new Uri("wss://websockets9127.azurewebsites.net:443/ws.ashx?name=Android"), new CancellationToken());
            //await _webSocketClient.SendAsync(new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes("Hello")), WebSocketMessageType.Text, true, new CancellationToken());
            

            _mChart.SetDescription("");
            _mChart.SetNoDataTextDescription("You need to provide data for the chart.");
            _mChart.SetTouchEnabled(true);
            _mChart.DragEnabled = true;
            _mChart.SetScaleEnabled(true);
            _mChart.SetDrawGridBackground(false);
            _mChart.SetPinchZoom(true);
            _mChart.SetBackgroundColor(Color.LightGray);

            LineData data = new LineData();
            data.SetValueTextColor(Color.Black);

            _mChart.Data = data;

            Legend l = _mChart.Legend;

            l.Form = Legend.LegendForm.Line;
            l.TextColor = Color.Black;

            XAxis xl = _mChart.XAxis;
            xl.TextColor = Color.Black;
            xl.SetDrawGridLines(false);
            xl.SetAvoidFirstLastClipping(true);
            xl.SpaceBetweenLabels = 5;
            xl.Enabled = true;

            YAxis leftAxis = _mChart.AxisLeft;
            leftAxis.TextColor = Color.Black;
            leftAxis.AxisMaxValue = 100f;
            leftAxis.AxisMinValue = 0f;
            leftAxis.SetDrawGridLines(true);

            YAxis rightAxis = _mChart.AxisRight;
            rightAxis.Enabled = false;

            var layoutParams = new RelativeLayout.LayoutParams(300, 150);
            layoutParams.SetMargins(20, 1350, 0, 20);

            var layoutParams1 = new RelativeLayout.LayoutParams(300, 150);
            layoutParams1.SetMargins(400, 1350, 0, 20);

            var layoutParams2 = new RelativeLayout.LayoutParams(300, 150);
            layoutParams2.SetMargins(780, 1350, 0, 20);

            Button button = new Button(Context);
            Button button1 = new Button(Context);
            Button button2 = new Button(Context);
            button.Text = "Pause";
            button1.Text = "Play";
            button2.Text = "Stop";
            
            button.LayoutParameters = layoutParams;
            button1.LayoutParameters = layoutParams1;
            button2.LayoutParameters = layoutParams2;

            rl.AddView(_mChart, 1080, 1330);
            rl.AddView(button);
            rl.AddView(button1);
            rl.AddView(button2);

            button.Click += delegate
            {
                _mChart.ClearValues();
                button1.Text = "ADD";
                button1.Enabled = true;
            };
            button1.Click += delegate
            {
                FeedMultiple();
                button1.Text = "Added";
                button1.Enabled = false;
            };
            button2.Click += delegate
            {
                addingNumbers = false;
                wSH.Send("STOP");
                //await WebsocketSend("ST");
            };
        }

        private int year = 2016;

        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                _message = value;
            }
        }

        public void AddEntry()
        {
            LineData data = (LineData)_mChart.Data;

            if (data != null)
            {

                LineDataSet set = (LineDataSet)data.GetDataSetByIndex(0);
                // set.addEntry(...); // can be called as well

                if (set == null)
                {
                    set = CreateSet();
                    data.AddDataSet(set);
                }

                // add a new x-value first
                data.AddXValue(MMonths[data.XValCount % 12] + " "
                        + (year + data.XValCount / 12));

                if (Message != null)
                {
                    Log.Debug("    PLEASE ", "JUST DO IT - " + Message);
                    data.AddEntry(
                        new Entry((float.Parse(Message, CultureInfo.InvariantCulture.NumberFormat)), set.EntryCount), 0);
                }


                // let the chart know it's data has changed
                _mChart.NotifyDataSetChanged();

                // limit the number of visible entries
                _mChart.SetVisibleXRangeMaximum(120);
                // mChart.setVisibleYRange(30, AxisDependency.LEFT);

                // move to the latest entry
                _mChart.MoveViewToX(data.XValCount - 121);

                // this automatically refreshes the chart (calls invalidate())
                // mChart.moveViewTo(data.getXValCount()-7, 55f,
                // AxisDependency.LEFT);
            }
        }

        private LineDataSet CreateSet()
        {

            LineDataSet set = new LineDataSet(null, "Dynamic Data");
            set.AxisDependency = YAxis.AxisDependency.Left;
            set.Color = ColorTemplate.HoloBlue;
            set.SetCircleColor(Color.White);
            set.LineWidth = 2f;
            set.CircleSize = 2f;
            set.FillAlpha = 65;
            set.FillColor = ColorTemplate.HoloBlue;
            set.HighLightColor = (Color.Rgb(244, 117, 117));
            set.ValueTextColor = Color.White;
            set.ValueTextSize = 9f;
            set.SetDrawValues(false);
            return set;

        }

        public void OnNothingSelected()
        {
            //throw new NotImplementedException();
        }

        public void OnValueSelected(Entry e, int dataSetIndex, Highlight h)
        {
            //throw new NotImplementedException();
        }

        public async void FeedMultiple()
        {
            Log.Debug("WHATTHEFUCK", "HALP");
            //await WebsocketHelp();
            
            IRunnable irr = new Runnable( () =>
            {
                Activity.RunOnUiThread(new Runnable(AddEntry));
                try
                {
                    this.Message = wSH.Send("Help");
                    //await Task.Delay(1000);
                    //await WebsocketHelp();
                    Thread.Sleep(100);
                    Log.Debug(" CHEESUS ", "YAY" + Message);
                }
                catch (InterruptedException e)
                {
                    // TODO Auto-generated catch block
                    e.PrintStackTrace();
                }
            });
            Thread runThread = new Thread(irr);
            runThread.Start();

            //return "Added";
        }

        public async Task WebsocketHelp()
        {
            //Buffer for received bits. 
            _receivedDataBuffer = new ArraySegment<System.Byte>(new System.Byte[MaxMessageSize]);

            _cancellationToken = new CancellationToken();

            //Checks WebSocket state. 
            while (_webSocketClient.State == WebSocketState.Open)
            {
                await WebsocketReceive();
                await WebsocketSend(Message);
            }
        }

        public async Task WebsocketSend(string message)
        {
            
            bool wtf = _webSocketClient.SendAsync(new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message)),
                             WebSocketMessageType.Text, true, _cancellationToken).IsCompleted;
            Log.Debug("SEND FUCKKKK", message + wtf.ToString());

        }

        public async Task WebsocketReceive()
        {
            WebSocketReceiveResult webSocketReceiveResult =
                    await _webSocketClient.ReceiveAsync(_receivedDataBuffer, _cancellationToken);

            //If input frame is cancelation frame, send close command. 
            if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
            {
                await _webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    String.Empty, _cancellationToken);
            }
            else
            {
                byte[] payloadData = _receivedDataBuffer.Array.Where(b => b != 0).ToArray();

                //Because we know that is a string, we convert it. 
                string receiveString =
                    System.Text.Encoding.UTF8.GetString(payloadData, 0, payloadData.Length);
                this.Message = receiveString;
                Log.Debug("RECEIVE FUCKKKK", Message);
            }
        }

    }
}