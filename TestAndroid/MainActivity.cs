using Android.App;
using Android.Graphics;
using Android.OS;
using BarChart;

namespace TestAndroid
{
    [Activity(Label = "TestAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        //int count = 1;

        static readonly BarModel[] TestData = new BarModel[] {
            new BarModel () { Value =   -1f, Legend = "0", Color = Color.Red },
            new BarModel () { Value =    2f, Legend = "1" },
            new BarModel () { Value =    0f, Legend = "2" },
            new BarModel () { Value =    1f, Legend = "3" },
            new BarModel () { Value =   -1f, Legend = "4", Color = Color.Red },
            new BarModel () { Value =    1f, Legend = "5" },
            new BarModel () { Value =   -1f, Legend = "6", Color = Color.Red },
            new BarModel () { Value =    2f, Legend = "7" },
            new BarModel () { Value = -0.1f, Legend = "8", Color = Color.Red }
        };



        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            /*RealTimeChart rtc = new RealTimeChart();
            var ft = FragmentManager.BeginTransaction();
            //ft.Add(Resource.Id.ChartFragment, rtc);
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

