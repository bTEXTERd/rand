using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MikePhil.Charting.Charts;

namespace TestAndroid
{
    [Activity(Label = "TestAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            WebSocketHelper wSH = new WebSocketHelper("wss://websockets9127.azurewebsites.net:443/ws.ashx?name=Android");

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it

            LineChart chart = new LineChart(Application.Context);
            LinearLayout rl = FindViewById<LinearLayout>(Resource.Id.LinearLayout);
            rl.AddView(chart);

            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = wSH.Send("ASS"); };
        }
    }
}

