using Android.App;
using Android.OS;

namespace TestAndroid
{
    [Activity(Label = "TestAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        //int count = 1;

        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            /*RealTimeChart rtc = new RealTimeChart();
            var ft = FragmentManager.BeginTransaction();
            //ft.Add(Resource.Id.ChartFragment, rtc);
            ft.Commit();*/

            WebSocketHelper wSH = new WebSocketHelper("wss://websockets9127.azurewebsites.net:443/ws.ashx?name=Android");

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            

            //mChart = FindViewById<LineChart>(Resource.Id.chart);

            //LinearLayout rl = FindViewById<LinearLayout>(Resource.Id.LinearLayout);
            //rl.AddView(mChart);

        }
    }
}

