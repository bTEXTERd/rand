using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
using SQLite;
using String = System.String;
using Thread = Java.Lang.Thread;

namespace TestAndroid
{
    public class RealTimeChart : Fragment, IOnChartValueSelectedListenerSupport
    {
        public string Message { private get; set; }

        public Toast Toast;
        private ProgressDialog _progressDialog;

        private WebSocketHelper _wSh;
        private SQLiteConnection _db;
        private LineChart _mChart;
        private LineData _lineData;
        private LineDataSet _set;
        private Button _clearButton;
        private Button _startButton;
        private Button _pauseButton;

        private static readonly string[] MMonths = new String[] {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec"
        };
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

            _db = new SQLiteConnection(pathToDatabase);
            _db.CreateTable<Numbers>();

            _mChart = new LineChart(this.Context);

            _mChart.SetOnChartValueSelectedListener(this);
            _mChart.SetDescription("");
            _mChart.SetNoDataTextDescription("You need to provide data for the chart.");
            _mChart.SetTouchEnabled(true);
            _mChart.DragEnabled = true;
            _mChart.SetScaleEnabled(true);
            _mChart.SetDrawGridBackground(false);
            _mChart.SetPinchZoom(true);
            _mChart.SetBackgroundColor(Color.LightGray);
            _mChart.HighlightPerDragEnabled = true;
            _mChart.HighlightPerTapEnabled = true;
            

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

            var clearLayoutParams = new RelativeLayout.LayoutParams(300, 150);
            clearLayoutParams.SetMargins(20, 1350, 0, 20);

            var startLayoutParams = new RelativeLayout.LayoutParams(300, 150);
            startLayoutParams.SetMargins(390, 1350, 0, 20);

            var pauselayoutParams = new RelativeLayout.LayoutParams(300, 150);
            pauselayoutParams.SetMargins(770, 1350, 0, 20);

            _clearButton = new Button(Context);
            _startButton = new Button(Context);
            _pauseButton = new Button(Context);

            _clearButton.Text = Resources.GetString(Resource.String.ClearButton);
            _startButton.Text = Resources.GetString(Resource.String.AddButton);
            _pauseButton.Text = Resources.GetString(Resource.String.PauseButton);

            _clearButton.LayoutParameters = clearLayoutParams;
            _startButton.LayoutParameters = startLayoutParams;
            _pauseButton.LayoutParameters = pauselayoutParams;

            _wSh = new WebSocketHelper(this, "wss://websockets9127.azurewebsites.net:443/ws.ashx?name=Android");
            Toast = Toast.MakeText(this.Context, "Connected", ToastLength.Short);

            _clearButton.Click += delegate
            {
                _wSh.Send("STOP");
                _db.DeleteAll<Numbers>();
                _mChart.ClearValues();
                _lineData = new MikePhil.Charting.Data.LineData(new List<string>());
                _mChart.Data = _lineData;
                _startButton.Text = "ADD";
                _startButton.Enabled = true;
            };
            _startButton.Click += delegate
            {
                FeedMultiple();
                _startButton.Enabled = false;
                _pauseButton.Enabled = true;
            };
            _pauseButton.Click += delegate
            {
                _wSh.Send("STOP");
                _startButton.Enabled = true;
                _pauseButton.Enabled = false;
            };

            return inflater.Inflate(Resource.Layout.ChartFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            RelativeLayout rl = view.FindViewById<RelativeLayout>(Resource.Id.RelativeLayout);

            rl.AddView(_mChart, 1080, 1330);
            rl.AddView(_clearButton);
            rl.AddView(_startButton);
            rl.AddView(_pauseButton);

            Activity.RunOnUiThread(() =>  _progressDialog = ProgressDialog.Show(this.Context, "Please wait...", "Accessing Database...", true));

            IRunnable irr = new Runnable(() =>
            {
                Looper.Prepare();
                Activity.RunOnUiThread(new Runnable(() =>
                {
                    PopulateFromDb();
                    _progressDialog.Hide();
                }));
            });
            var runThread = new Thread(irr);
            runThread.Start();
        }

        private int year = 2016;

        private void PopulateFromDb()
        {
            if (!_db.Table<Numbers>().Any()) return;
            var table = _db.Table<Numbers>();
            var i = 0;
            foreach (var s in table)
            {
                this.Message = s.Number.ToString(CultureInfo.InvariantCulture);
                _lineData = (LineData)_mChart.Data;

                if (_lineData != null)
                {

                    _set = (LineDataSet)_lineData.GetDataSetByIndex(0);
                    // set.addEntry(...); // can be called as well

                    if (_set == null)
                    {
                        _set = CreateSet();
                        _lineData.AddDataSet(_set);
                    }

                    // add a new x-value first
                    _lineData.AddXValue(MMonths[_lineData.XValCount % 12] + " "
                            + (year + _lineData.XValCount / 12));

                    if (Message != null)
                    {
                        _lineData.AddEntry(
                            new Entry((float.Parse(Message, CultureInfo.InvariantCulture.NumberFormat)), _set.EntryCount), 0);
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
        }

        public void AddEntry()
        {
            _lineData = (LineData)_mChart.Data;

            if (_lineData != null)
            {

                _set = (LineDataSet)_lineData.GetDataSetByIndex(0);
                // set.addEntry(...); // can be called as well

                if (_set == null)
                {
                    _set = CreateSet();
                    _lineData.AddDataSet(_set);
                }

                // add a new x-value first
                _lineData.AddXValue(MMonths[_lineData.XValCount % 12] + " "
                        + (year + _lineData.XValCount / 12));

                if (Message != null)
                {
                    _db.Insert(new Numbers { Number = float.Parse(Message, CultureInfo.InvariantCulture.NumberFormat)});
                    _lineData.AddEntry(
                        new Entry((float.Parse(Message, CultureInfo.InvariantCulture.NumberFormat)), _set.EntryCount), 0);
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
            LineDataSet set = new LineDataSet(null, "Dynamic Data")
            {
                AxisDependency = YAxis.AxisDependency.Left,
                Color = Color.Rgb(250, 186, 122)
            };
            set.SetCircleColor(Color.Red);
            set.SetCircleColorHole(Color.Red);
            set.LineWidth = 2f;
            set.CircleSize = 2.8f;
            set.FillAlpha = 65;
            set.FillColor = ColorTemplate.HoloBlue;
            set.HighLightColor = (Color.Rgb(244, 117, 117));
            set.ValueTextColor = Color.Black;
            set.ValueTextSize = 9f;
            set.SetDrawValues(true);
            return set;
        }

        private void FeedMultiple()
        {
            _wSh.Send("Help");
        }

        public void OnNothingSelected()
        {
            throw new System.NotImplementedException();
        }

        public void OnValueSelected(Entry e, int dataSetIndex, Highlight h)
        {
            throw new System.NotImplementedException();
        }
    }
}