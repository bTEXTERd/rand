using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Test
{
	[Activity (Label = "Random Generator", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			WebSocketHelper wSH = new WebSocketHelper("ws://192.168.137.1:8080/WebApplication1/test");

			SetContentView (Resource.Layout.Main);

			Button button = FindViewById<Button> (Resource.Id.myButton);
			button.Click += delegate {
				button.Text = wSH.Send("");
			};
		}
	}
}


