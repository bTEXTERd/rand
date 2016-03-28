using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Interfaces;
using MikePhil.Charting.Listener;
using MikePhil.Charting.Util;
using MikePhil.Charting.Highlight;

namespace TestAndroid
{
    class RealTimeChart : Fragment, IOnChartValueSelectedListenerSupport
    {
        private LineChart mChart;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.chartFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            mChart = new LineChart(Application.Context);
            LinearLayout rl = view.FindViewById<LinearLayout>(Resource.Id.LinearLayout);
            rl.AddView(mChart);
        }

        public void OnNothingSelected()
        {
            throw new NotImplementedException();
        }

        public void OnValueSelected(Entry e, int dataSetIndex, Highlight h)
        {
            throw new NotImplementedException();
        }
    }
}