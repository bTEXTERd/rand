using System;
using System.Security.Cryptography;
using System.Threading;
using Android.App;
using Android.Graphics;
using Android.OS;
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
    class RealTimeChart : Fragment, IOnChartValueSelectedListenerSupport
    {
        private LineChart _mChart;

        WebSocketHelper wSH = new WebSocketHelper("wss://websockets9127.azurewebsites.net:443/ws.ashx?name=Android");

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

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            _mChart = new LineChart(this.Context);
            RelativeLayout rl = view.FindViewById<RelativeLayout>(Resource.Id.RelativeLayout);

            _mChart = new LineChart(Application.Context);
            _mChart.SetOnChartValueSelectedListener(this);

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

            //Typeface tf = Typeface.CreateFromAsset(Application.(), "OpenSans-Regular.ttf");

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

            Button button = new Button(this.Context);
            button.Text = "Add";
            button.Click += delegate { AddEntry(); };
            rl.AddView(_mChart, 1050, 1400);
            rl.AddView(button);
            button.Click += delegate { button.Text = FeedMultiple(); };
        }

        private int year = 2016;
        

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
                Random rand = new Random();
                data.AddXValue(MMonths[data.XValCount % 12] + " "
                        + (year + data.XValCount / 12));
                data.AddEntry(new Entry((float)(rand.Next(0, 45)) + 30f, set.EntryCount), 0);


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

        private String FeedMultiple(){
            IRunnable irr = new Runnable(() =>
            {
                for (int i = 0; i < 500; i++)
                {
                    Activity.RunOnUiThread(new Runnable(AddEntry));
                    try
                    {
                        Thread.Sleep(35);
                    }
                    catch (InterruptedException e)
                    {
                        // TODO Auto-generated catch block
                        e.PrintStackTrace();
                    }
                }
            });
            Thread runThread = new Thread(irr);
            runThread.Start();

            return "Added";
        }
        
    }
}