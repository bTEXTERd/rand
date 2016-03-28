using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Listener;
using MikePhil.Charting.Data;
using MikePhil.Charting.Highlight;
using MikePhil.Charting.Components;
using MikePhil.Charting.Util;
using MikePhil.Charting.Interfaces;
using Android.Graphics;

namespace TestAndroid
{
    [Activity(Label = "TestAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IOnChartValueSelectedListenerSupport
    {
        //int count = 1;
        private LineChart mChart;

        public void OnNothingSelected()
        {
            throw new NotImplementedException();
        }

        public void OnValueSelected(Entry e, int dataSetIndex, Highlight h)
        {
            throw new NotImplementedException();
        }

        protected String[] mMonths = new String[] {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec"
    };

        protected String[] mParties = new String[] {
            "Party A", "Party B", "Party C", "Party D", "Party E", "Party F", "Party G", "Party H",
            "Party I", "Party J", "Party K", "Party L", "Party M", "Party N", "Party O", "Party P",
            "Party Q", "Party R", "Party S", "Party T", "Party U", "Party V", "Party W", "Party X",
            "Party Y", "Party Z"
    };

        private int year = 2015;

        public void AddEntry()
        {

            LineData data = (LineData) mChart.Data;

            if (data != null)
            {

                LineDataSet set = (LineDataSet) data.GetDataSetByIndex (0);
                // set.addEntry(...); // can be called as well

                if (set == null)
                {
                    set = CreateSet();
                    data.AddDataSet(set);
                }

                // add a new x-value first
                Random rand = new Random();
                data.AddXValue(mMonths[data.XValCount % 12] + " "
                        + (year + data.XValCount / 12));
                data.AddEntry(new Entry((float)(rand.Next(0,1000) * 40) + 30f, set.EntryCount), 0);


                // let the chart know it's data has changed
                mChart.NotifyDataSetChanged();

                // limit the number of visible entries
                mChart.SetVisibleXRangeMaximum(120);
                // mChart.setVisibleYRange(30, AxisDependency.LEFT);

                // move to the latest entry
                mChart.MoveViewToX(data.XValCount - 121);

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

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            WebSocketHelper wSH = new WebSocketHelper("wss://websockets9127.azurewebsites.net:443/ws.ashx?name=Android");

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            //RealTimeChart rtc = new RealTimeChart();
            //var ft = FragmentManager.BeginTransaction();
            //ft.Add(Resource.Id.LinearLayout, rtc);
            //ft.Commit();

            //mChart = FindViewById<LineChart>(Resource.Id.chart);
            mChart = new LineChart(Application.Context);
            mChart.SetOnChartValueSelectedListener(this);

            mChart.SetDescription("");
            mChart.SetNoDataTextDescription("You need to provide data for the chart.");
            mChart.SetTouchEnabled(true);
            mChart.DragEnabled = true;
            mChart.SetScaleEnabled(true);
            mChart.SetDrawGridBackground(false);
            mChart.SetPinchZoom(true);
            mChart.SetBackgroundColor(Color.LightGray);

            LineData data = new LineData();
            data.SetValueTextColor(Color.White);

            mChart.Data = data;

            Legend l = mChart.Legend;

            l.Form = Legend.LegendForm.Line;
            l.TextColor = Color.White;

            XAxis xl = mChart.XAxis;
            xl.TextColor = Color.White;
            xl.SetDrawGridLines(false);
            xl.SetAvoidFirstLastClipping(true);
            xl.SpaceBetweenLabels =  5 ;
            xl.Enabled = true;

            YAxis leftAxis = mChart.AxisLeft;
            leftAxis.TextColor = Color.White;
            leftAxis.AxisMaxValue = 100f;
            leftAxis.AxisMinValue = 0f;
            leftAxis.SetDrawGridLines(true);

            YAxis rightAxis = mChart.AxisRight;
            rightAxis.Enabled = false;

            LinearLayout rl = FindViewById<LinearLayout>(Resource.Id.LinearLayout);
            rl.AddView(mChart);

            Button button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += delegate { AddEntry(); };
            //button.Click += delegate { button.Text = wSH.Send("Text"); };
        }
    }
}

