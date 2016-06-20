using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using BarChart;

namespace TestAndroid
{
    [Activity(Label = "TestAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            /*RealTimeChart rtc = new RealTimeChart();
            var ft = FragmentManager.BeginTransaction();
            ft.Add(Resource.Id.ChartFragment, rtc);
            ft.Commit();*/

            

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it


            /*var mChart = FindViewById<BarChartView>(Resource.Id.Chart);

            mChart.BarClick += delegate { wSH.Send("SSSSS"); };
            mChart.ItemsSource = TestData;*/

            //LinearLayout rl = FindViewById<LinearLayout>(Resource.Id.LinearLayout);
            //rl.AddView(mChart);

        }
    }
}

