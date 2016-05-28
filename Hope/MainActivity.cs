using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using BarChart;

namespace Hope
{
    [Activity(Label = "Hope", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it

            /*var data = new[] { 1f, 2f, 4f, 8f, 16f, 32f };
            var chart = new BarChartView(this)
            {
                ItemsSource = Array.ConvertAll(data, v => new BarModel { Value = v })
            };

            AddContentView(chart, new ViewGroup.LayoutParams(
              ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));

            chart.BarClick += (sender, args) => {
                BarModel bar = args.Bar;
                Console.WriteLine("Pressed {0}", bar);
            };

            chart.MinimumValue = null;
            chart.MaximumValue = null;*/

            BarChartView chart = FindViewById<BarChartView>(Resource.Id.barChart);

            chart.BarClick += (sender, args) => {
                BarModel bar = args.Bar;
                Console.WriteLine("Pressed {0}", bar);
            };

            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
        }
    }
}

