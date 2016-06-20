using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Listener;
using MikePhil.Charting.Util;
using MikePhil.Charting.Highlight;
using SQLite;
using ArrayList = System.Collections.ArrayList;
using String = System.String;
using Thread = Java.Lang.Thread;

namespace TestAndroid
{
    public class RealTimeChart : Fragment, IOnChartValueSelectedListenerSupport
    {
        public string Message { get; set; }

        public bool Connected { get; set; }

        private WebSocketHelper _wSh;
        private SQLiteConnection _db;
        private LineChart _mChart;
        private LineData _lineData;

        protected static string[] MMonths = new String[] {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec"
        };
        private ArrayList xVals = new ArrayList ();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.ChartFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _wSh = new WebSocketHelper(this, "wss://websockets9127.azurewebsites.net:443/ws.ashx?name=Android");
            
            var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

            _db = new SQLiteConnection(pathToDatabase);
            _db.CreateTable<Numbers>();
            
            _mChart = new LineChart(this.Context);
            //_mChart = new LineChart(Application.Context);
            RelativeLayout rl = view.FindViewById<RelativeLayout>(Resource.Id.RelativeLayout);

            _mChart.SetOnChartValueSelectedListener(this);
            _mChart.SetDescription("");
            _mChart.SetNoDataTextDescription("You need to provide data for the chart.");
            _mChart.SetTouchEnabled(true);
            _mChart.DragEnabled = true;
            _mChart.SetScaleEnabled(true);
            _mChart.SetDrawGridBackground(false);
            _mChart.SetPinchZoom(true);
            _mChart.SetBackgroundColor(Color.LightGray);

            _lineData = new LineData();
            _lineData.SetValueTextColor(Color.Black);

            _mChart.Data = _lineData;

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
                _db.DeleteAll<Numbers>();
                _mChart.ClearValues();
                /*IList<String> list = _lineData.XVals;
                foreach (var xval in _lineData.XVals)
                {
                    list.Remove(xval);
                }*/

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
                _wSh.Send("STOP");
            };

            foreach (var month in MMonths)
            {
                xVals.Add(month);
            }

            //var vto = view.ViewTreeObserver;
            //vto.GlobalLayout += (sender, args) =>
            //{
                PopulateFromDB();
            //};
            /*vto.addOnGlobalLayoutListener(new OnGlobalLayoutListener() {
                @Override
                public void onGlobalLayout()
                {
                // Put your code here. 

                    layout.getViewTreeObserver().removeOnGlobalLayoutListener(this);
                }
            }); */
        }

        private int year = 2016;

        private Task ShowConnectedStatusTask()
        {
            if (Connected)
                Snackbar.Make(this.View, "Connected", Snackbar.LengthLong)
                    .Show();
            return Task.CompletedTask;
        }

        private void PopulateFromDB()
        {
            if (_db.Table<Numbers>().Any())
            {
                var table = _db.Table<Numbers>();
                var i = 0;
                foreach (var s in table)
                {
                    this.Message = s.Number.ToString(CultureInfo.InvariantCulture);
                    _lineData.AddEntry(
                        new Entry((float.Parse(Message, CultureInfo.InvariantCulture.NumberFormat)), i++), 0);
                }
            }
        }

        public void AddEntry()
        {
            _lineData = (LineData)_mChart.Data;

            if (_lineData != null)
            {

                LineDataSet set = (LineDataSet)_lineData.GetDataSetByIndex(0);
                // set.addEntry(...); // can be called as well

                if (set == null)
                {
                    set = CreateSet();
                    _lineData.AddDataSet(set);
                }

                // add a new x-value first
                _lineData.AddXValue(MMonths[_lineData.XValCount % 12] + " "
                        + (year + _lineData.XValCount / 12));

                if (Message != null)
                {
                    Log.Debug("    PLEASE ", "JUST DO IT - " + Message);
                    _db.Insert(new Numbers { Number = float.Parse(Message, CultureInfo.InvariantCulture.NumberFormat)});
                    _lineData.AddEntry(
                        new Entry((float.Parse(Message, CultureInfo.InvariantCulture.NumberFormat)), set.EntryCount), 0);
                }


                // let the chart know it's data has changed
                _mChart.NotifyDataSetChanged();

                // limit the number of visible entries
                _mChart.SetVisibleXRangeMaximum(120);
                // mChart.setVisibleYRange(30, AxisDependency.LEFT);

                // move to the latest entry
                _mChart.MoveViewToX(_lineData.XValCount - 121);

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

        public void FeedMultiple()
        {
            IRunnable irr = new Runnable( () =>
            {
                Activity.RunOnUiThread(new Runnable(AddEntry));
                try
                {
                    this.Message = _wSh.Send("Help");
                    Log.Debug(" CHEESUS ", "YAY" + Message);
                    Thread.Sleep(100);
                    
                }
                catch (InterruptedException e)
                {
                    // TODO Auto-generated catch block
                    e.PrintStackTrace();
                }
            });
            Thread runThread = new Thread(irr);
            runThread.Start();
        }
    }
}